/*
TechStore - 5 minute live demo script
Goal: show HQ update -> queue -> branch synchronized + integrated revenue report.
*/

USE TechStore_HQ;
GO

/* 1) Show initial product price on HQ and Branch */
DECLARE @ProductID INT = 1;

SELECT ProductID, ProductName, Price AS HQPrice
FROM dbo.SanPham
WHERE ProductID = @ProductID;

SELECT b.ProductID, b.Price AS BranchPrice
FROM OPENQUERY(MYSQL,
'    SELECT ProductID, Price
     FROM TechStore_Branch.SanPham') b
WHERE b.ProductID = @ProductID;
GO

/* 2) Update HQ price */
UPDATE dbo.SanPham
SET Price = Price + 50000
WHERE ProductID = 1;
GO

/* 3) Show queue item created by trigger */
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

/* 4) Process queue and show final queue status */
EXEC dbo.sp_ProcessPriceSyncQueue;
GO

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

/* 5) Confirm branch price changed */
SELECT b.ProductID, b.Price AS BranchPrice
FROM OPENQUERY(MYSQL,
'    SELECT ProductID, Price
     FROM TechStore_Branch.SanPham') b
WHERE b.ProductID = 1;
GO

/* 6) Run integrated revenue report */
EXEC dbo.sp_BaoCaoDoanhThu @FromDate = NULL, @ToDate = NULL;
GO
