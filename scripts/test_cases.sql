/*
TechStore - Distributed DB focused test cases
Run on SQL Server instance that contains TechStore_HQ and linked server MYSQL.

Important:
- These test cases validate distributed DB behavior only.
- Input validation/UI checks are intentionally excluded.
*/

USE TechStore_HQ;
GO

/* TC01 - Linked server connectivity (cross-site reachability) */
PRINT 'TC01 - Linked server connectivity';
EXEC master.dbo.sp_testlinkedserver @servername = N'MYSQL';
GO

/* TC02 - Logical transparency for invoice query (no OPENQUERY in caller) */
PRINT 'TC02 - Logical invoice query';
EXEC dbo.sp_GetUnifiedInvoices;
GO

/* TC03 - Logical transparency for joined sales query */
PRINT 'TC03 - Logical joined sales query';
EXEC dbo.sp_GetUnifiedSales @FromDate = NULL, @ToDate = NULL, @Take = 30;
GO

/* TC04 - Equivalence check: logical view vs direct physical remote read */
PRINT 'TC04 - Logical/physical equivalence';
;WITH LogicalAgg AS
(
    SELECT
        ProductID,
        SUM(Quantity) AS QtyLogical,
        SUM(Total) AS TotalLogical
    FROM dbo.vw_BranchInvoices_Logical
    GROUP BY ProductID
),
PhysicalAgg AS
(
    SELECT
        CAST(ProductID AS INT) AS ProductID,
        SUM(CAST(Quantity AS INT)) AS QtyPhysical,
        SUM(CAST(Total AS DECIMAL(18,2))) AS TotalPhysical
    FROM OPENQUERY(MYSQL,
    '    SELECT ProductID, Quantity, Total
         FROM TechStore_Branch.HoaDon')
    GROUP BY CAST(ProductID AS INT)
)
SELECT
    COALESCE(l.ProductID, p.ProductID) AS ProductID,
    l.QtyLogical,
    p.QtyPhysical,
    l.TotalLogical,
    p.TotalPhysical
FROM LogicalAgg l
FULL OUTER JOIN PhysicalAgg p
    ON l.ProductID = p.ProductID
WHERE ISNULL(l.QtyLogical, -1) <> ISNULL(p.QtyPhysical, -1)
   OR ISNULL(l.TotalLogical, -1) <> ISNULL(p.TotalPhysical, -1);
/* Expected: 0 rows (equivalent result set). */
GO

/* TC05 - Distributed write pipeline: HQ update creates queue record */
PRINT 'TC05 - Queue record creation';
DECLARE @ProductID_TC05 INT = 1;
DECLARE @Delta_TC05 DECIMAL(18,2) = 1000;

UPDATE dbo.SanPham
SET Price = Price + @Delta_TC05
WHERE ProductID = @ProductID_TC05;

SELECT TOP (1)
    QueueID,
    ProductID,
    NewPrice,
    Status,
    CreatedAt,
    ProcessedAt,
    LastError
FROM dbo.PriceSyncQueue
WHERE ProductID = @ProductID_TC05
ORDER BY QueueID DESC;
/* Expected: latest status = 'P' before processor runs. */
GO

/* TC06 - Queue processing updates branch data and marks success */
PRINT 'TC06 - Queue process success + cross-site consistency';
DECLARE @ProductID_TC06 INT = 1;

EXEC dbo.sp_ProcessPriceSyncQueue;

SELECT TOP (1)
    QueueID,
    ProductID,
    NewPrice,
    Status,
    ProcessedAt,
    LastError
FROM dbo.PriceSyncQueue
WHERE ProductID = @ProductID_TC06
ORDER BY QueueID DESC;

SELECT
    hq.ProductID,
    hq.Price AS HQPrice,
    br.Price AS BranchPrice
FROM dbo.SanPham hq
INNER JOIN dbo.vw_BranchProducts_Logical br
    ON hq.ProductID = br.ProductID
WHERE hq.ProductID = @ProductID_TC06;
/* Expected: status = 'S' and HQPrice = BranchPrice for tested product. */
GO

/* TC07 - Fault handling: permission/network issue must produce queue status E
Part A (run on MySQL first):
REVOKE UPDATE ON TechStore_Branch.SanPham FROM 'tien'@'localhost';
FLUSH PRIVILEGES;

Part B (run below on SQL Server):
*/
PRINT 'TC07 - Fault handling to queue error state';
UPDATE dbo.SanPham
SET Price = Price + 1000
WHERE ProductID = 1;

EXEC dbo.sp_ProcessPriceSyncQueue;

SELECT TOP (1)
    QueueID,
    ProductID,
    NewPrice,
    Status,
    ProcessedAt,
    LastError
FROM dbo.PriceSyncQueue
ORDER BY QueueID DESC;
/* Expected: status = 'E' and LastError is populated. */
GO

/* TC08 - Recovery and retry after restoring permission
Run in MySQL before TC08:
GRANT SELECT, UPDATE ON TechStore_Branch.SanPham TO 'tien'@'localhost';
FLUSH PRIVILEGES;
*/
PRINT 'TC08 - Retry failed queue item';
UPDATE dbo.PriceSyncQueue
SET Status = 'P',
    ProcessedAt = NULL
WHERE QueueID = (
    SELECT TOP (1) QueueID
    FROM dbo.PriceSyncQueue
    WHERE Status = 'E'
    ORDER BY QueueID DESC
);

EXEC dbo.sp_ProcessPriceSyncQueue;

SELECT TOP (1)
    QueueID,
    ProductID,
    NewPrice,
    Status,
    ProcessedAt,
    LastError
FROM dbo.PriceSyncQueue
ORDER BY QueueID DESC;
/* Expected: failed item eventually transitions to status = 'S'. */
GO

/* TC09 - Cross-site DateTime filter through logical report interface */
PRINT 'TC09 - Date filter on logical report';
EXEC dbo.sp_BaoCaoDoanhThu
    @FromDate = '2026-04-21',
    @ToDate = '2026-04-21';
/* Expected: filtered set returns correctly without datetime cast error. */
GO

/* TC10 - Revenue reconciliation (logical report vs distributed join base) */
PRINT 'TC10 - Reconciliation check';
DECLARE @FromDate_TC10 DATETIME2(0) = '2026-04-01';
DECLARE @ToDate_TC10   DATETIME2(0) = '2026-04-30';

CREATE TABLE #Report
(
    ProductID INT,
    ProductName NVARCHAR(200),
    Category NVARCHAR(100),
    GiaTaiHQ DECIMAL(18,2),
    SoLuongBan INT,
    DoanhThu DECIMAL(18,2),
    LanBanGanNhat DATETIME2(0) NULL
);

INSERT INTO #Report
EXEC dbo.sp_BaoCaoDoanhThu @FromDate = @FromDate_TC10, @ToDate = @ToDate_TC10;

SELECT
    (SELECT ISNULL(SUM(DoanhThu), 0) FROM #Report) AS RevenueFromLogicalReport,
    (SELECT ISNULL(SUM(h.Total), 0)
     FROM dbo.vw_BranchInvoices_Logical h
     INNER JOIN dbo.SanPham p ON p.ProductID = h.ProductID
     WHERE h.SaleDate >= @FromDate_TC10
       AND h.SaleDate < DATEADD(DAY, 1, @ToDate_TC10)) AS RevenueFromDistributedBase;
/* Expected: two totals are equal (or differ only if source data has orphan product keys). */

DROP TABLE #Report;
GO
