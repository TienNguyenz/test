/*
TechStore - Test cases script
Run on SQL Server instance that contains TechStore_HQ and linked server MYSQL.
*/

USE TechStore_HQ;
GO

/* TC01 - Linked server connectivity */
PRINT 'TC01 - Linked server connectivity';
EXEC master.dbo.sp_testlinkedserver @servername = N'MYSQL';
GO

/* TC02 - Read branch invoices through OPENQUERY */
PRINT 'TC02 - Read branch invoices';
SELECT TOP (5)
    InvoiceID,
    ProductID,
    Quantity,
    CAST(SaleDate AS DATETIME2(0)) AS SaleDate,
    CAST(Total AS DECIMAL(18,2)) AS Total
FROM OPENQUERY(MYSQL,
'    SELECT InvoiceID, ProductID, Quantity, SaleDate, Total
     FROM TechStore_Branch.HoaDon
     ORDER BY SaleDate DESC');
GO

/* TC03 - Revenue report from stored procedure */
PRINT 'TC03 - Revenue stored procedure';
EXEC dbo.sp_BaoCaoDoanhThu @FromDate = NULL, @ToDate = NULL;
GO

/* TC04/TC05/TC06 - Price update + queue + branch synchronization */
PRINT 'TC04/TC05/TC06 - Price sync flow';
DECLARE @ProductID INT = 1;
DECLARE @Delta DECIMAL(18,2) = 1000;

SELECT p.ProductID, p.Price AS HQPrice_Before
FROM dbo.SanPham p
WHERE p.ProductID = @ProductID;

SELECT b.ProductID, b.Price AS BranchPrice_Before
FROM OPENQUERY(MYSQL,
'    SELECT ProductID, Price
     FROM TechStore_Branch.SanPham') b
WHERE b.ProductID = @ProductID;

UPDATE dbo.SanPham
SET Price = Price + @Delta
WHERE ProductID = @ProductID;

SELECT TOP (1)
    QueueID,
    ProductID,
    NewPrice,
    Status,
    CreatedAt,
    ProcessedAt,
    LastError
FROM dbo.PriceSyncQueue
WHERE ProductID = @ProductID
ORDER BY QueueID DESC;

EXEC dbo.sp_ProcessPriceSyncQueue;

SELECT TOP (1)
    QueueID,
    ProductID,
    NewPrice,
    Status,
    CreatedAt,
    ProcessedAt,
    LastError
FROM dbo.PriceSyncQueue
WHERE ProductID = @ProductID
ORDER BY QueueID DESC;

SELECT p.ProductID, p.Price AS HQPrice_After
FROM dbo.SanPham p
WHERE p.ProductID = @ProductID;

SELECT b.ProductID, b.Price AS BranchPrice_After
FROM OPENQUERY(MYSQL,
'    SELECT ProductID, Price
     FROM TechStore_Branch.SanPham') b
WHERE b.ProductID = @ProductID;
GO

/* TC07 - Permission failure handling (manual two-part test)
Part A (run in MySQL):
REVOKE UPDATE ON TechStore_Branch.SanPham FROM 'tien'@'localhost';
FLUSH PRIVILEGES;

Part B (run again in SQL Server):
UPDATE dbo.SanPham SET Price = Price + 1000 WHERE ProductID = 1;
EXEC dbo.sp_ProcessPriceSyncQueue;
SELECT TOP(1) * FROM dbo.PriceSyncQueue ORDER BY QueueID DESC;

Expected: queue Status = 'E' and LastError has permission error.
After test, restore permission in MySQL:
GRANT SELECT, UPDATE ON TechStore_Branch.SanPham TO 'tien'@'localhost';
FLUSH PRIVILEGES;
*/

/* TC08 - Date filter and datetime compatibility */
PRINT 'TC08 - Date filter';
EXEC dbo.sp_BaoCaoDoanhThu @FromDate = '2026-04-21', @ToDate = '2026-04-21';
GO

/* TC09 - Product CRUD flow (create -> update -> delete attempt) */
PRINT 'TC09 - Product CRUD flow';
DECLARE @NewProductID INT;

INSERT INTO dbo.SanPham(ProductName, Price, Category)
VALUES (N'TC Product', 12345000, N'Test');

SET @NewProductID = CAST(SCOPE_IDENTITY() AS INT);

SELECT ProductID, ProductName, Price, Category
FROM dbo.SanPham
WHERE ProductID = @NewProductID;

UPDATE dbo.SanPham
SET ProductName = N'TC Product Updated',
    Price = 13345000,
    Category = N'Test-Updated'
WHERE ProductID = @NewProductID;

EXEC dbo.sp_ProcessPriceSyncQueue;

SELECT ProductID, ProductName, Price, Category
FROM dbo.SanPham
WHERE ProductID = @NewProductID;

DELETE FROM dbo.SanPham
WHERE ProductID = @NewProductID;

SELECT ProductID
FROM dbo.SanPham
WHERE ProductID = @NewProductID;
GO

/* TC10 - Field constraint validation: price must be non-negative */
PRINT 'TC10 - Constraint validation';
BEGIN TRY
    UPDATE dbo.SanPham
    SET Price = -1
    WHERE ProductID = 1;
END TRY
BEGIN CATCH
    SELECT
        ERROR_NUMBER() AS ErrorNumber,
        ERROR_MESSAGE() AS ErrorMessage;
END CATCH;
GO
