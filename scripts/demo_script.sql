/*
TechStore - Live demo script (10-12 minutes)

Goal A (Requirement #1):
Show user queries a single logical interface and does not need to know distributed details.

Goal B (Requirement #2):
Show how data is physically/geographically distributed and how query/sync pipeline works.
*/

USE TechStore_HQ;
GO

/* ===========================================================
   A) LOGICAL TRANSPARENCY DEMO (user-facing)
   =========================================================== */

/* A1 - User query: no OPENQUERY syntax, only logical procedure */
PRINT 'A1 - Logical unified query';
EXEC dbo.sp_GetUnifiedSales @FromDate = NULL, @ToDate = NULL, @Take = 20;
GO

/* A2 - User report: one entry point for integrated revenue */
PRINT 'A2 - Logical revenue report';
EXEC dbo.sp_BaoCaoDoanhThu @FromDate = NULL, @ToDate = NULL;
GO

/* ===========================================================
   B) PHYSICAL / GEOGRAPHIC DISTRIBUTION EVIDENCE (technical)
   =========================================================== */

/* B1 - Linked server health (HQ SQL Server can reach Branch MySQL) */
PRINT 'B1 - Linked server connectivity';
EXEC master.dbo.sp_testlinkedserver @servername = N'MYSQL';
GO

/* B2 - Show physical remote read is encapsulated in logical view */
PRINT 'B2 - Remote fragment read via logical view';
SELECT TOP (5)
    InvoiceID,
    ProductID,
    Quantity,
    SaleDate,
    Total
FROM dbo.vw_BranchInvoices_Logical
ORDER BY SaleDate DESC, InvoiceID DESC;
GO

/* ===========================================================
   C) DISTRIBUTED UPDATE + QUEUE-BASED SYNC (HQ -> Branch)
   =========================================================== */

DECLARE @ProductID INT = 1;

/* C1 - Before update */
PRINT 'C1 - Price before update';
SELECT ProductID, ProductName, Price AS HQPrice_Before
FROM dbo.SanPham
WHERE ProductID = @ProductID;

SELECT ProductID, Price AS BranchPrice_Before
FROM dbo.vw_BranchProducts_Logical
WHERE ProductID = @ProductID;
GO

/* C2 - Update at HQ, trigger writes queue */
PRINT 'C2 - Update HQ price';
UPDATE dbo.SanPham
SET Price = Price + 50000
WHERE ProductID = 1;
GO

PRINT 'C3 - Queue item after update (expect P)';
SELECT TOP (1)
    QueueID,
    ProductID,
    NewPrice,
    Status,
    CreatedAt,
    ProcessedAt,
    LastError
FROM dbo.PriceSyncQueue
WHERE ProductID = 1
ORDER BY QueueID DESC;
GO

/* C4 - Process queue */
PRINT 'C4 - Process queue';
EXEC dbo.sp_ProcessPriceSyncQueue;
GO

PRINT 'C5 - Queue item after processing (expect S or E)';
SELECT TOP (1)
    QueueID,
    ProductID,
    NewPrice,
    Status,
    CreatedAt,
    ProcessedAt,
    LastError
FROM dbo.PriceSyncQueue
WHERE ProductID = 1
ORDER BY QueueID DESC;
GO

/* C5 - Confirm branch price */
PRINT 'C6 - Branch price after sync';
SELECT ProductID, Price AS BranchPrice_After
FROM dbo.vw_BranchProducts_Logical
WHERE ProductID = 1;
GO
