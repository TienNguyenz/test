/*
Run this script in SQL Server (TechStore_HQ) to create required objects for demo.
Assumes linked server MYSQL already works and Branch tables already exist.
*/

USE TechStore_HQ;
GO

/* Revenue procedure */
CREATE OR ALTER PROCEDURE dbo.sp_BaoCaoDoanhThu
    @FromDate DATETIME2(0) = NULL,
    @ToDate   DATETIME2(0) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    ;WITH x AS
    (
        SELECT
            h.ProductID,
            SUM(h.Quantity) AS SoLuongBan,
            SUM(h.Total) AS DoanhThu,
            MAX(h.SaleDate) AS LanBanGanNhat
        FROM (
            SELECT
                CAST(ProductID AS INT) AS ProductID,
                CAST(Quantity AS INT) AS Quantity,
                CAST(SaleDate AS DATETIME2(0)) AS SaleDate,
                CAST(Total AS DECIMAL(18,2)) AS Total
            FROM OPENQUERY(MYSQL,
            '    SELECT ProductID, Quantity, SaleDate, Total
                 FROM TechStore_Branch.HoaDon')
        ) h
        WHERE (@FromDate IS NULL OR h.SaleDate >= @FromDate)
          AND (@ToDate IS NULL OR h.SaleDate < DATEADD(DAY, 1, @ToDate))
        GROUP BY h.ProductID
    )
    SELECT
        p.ProductID,
        p.ProductName,
        p.Category,
        CAST(p.Price AS DECIMAL(18,2)) AS GiaTaiHQ,
        ISNULL(x.SoLuongBan, 0) AS SoLuongBan,
        ISNULL(x.DoanhThu, 0) AS DoanhThu,
        x.LanBanGanNhat
    FROM dbo.SanPham p
    LEFT JOIN x ON p.ProductID = x.ProductID
    ORDER BY DoanhThu DESC, p.ProductID ASC;
END;
GO

/* Queue table */
IF OBJECT_ID('dbo.PriceSyncQueue', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.PriceSyncQueue
    (
        QueueID BIGINT IDENTITY(1,1) PRIMARY KEY,
        ProductID INT NOT NULL,
        NewPrice DECIMAL(18,2) NOT NULL,
        Status CHAR(1) NOT NULL DEFAULT 'P',
        CreatedAt DATETIME2(0) NOT NULL DEFAULT SYSDATETIME(),
        ProcessedAt DATETIME2(0) NULL,
        LastError NVARCHAR(1000) NULL
    );
END
GO

/* Trigger writes queue */
CREATE OR ALTER TRIGGER dbo.trg_SanPham_QueuePriceSync
ON dbo.SanPham
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT UPDATE(Price)
        RETURN;

    INSERT INTO dbo.PriceSyncQueue(ProductID, NewPrice)
    SELECT i.ProductID, i.Price
    FROM inserted i
    JOIN deleted d ON i.ProductID = d.ProductID
    WHERE ISNULL(i.Price, -1) <> ISNULL(d.Price, -1);
END;
GO

/* Queue processor */
CREATE OR ALTER PROCEDURE dbo.sp_ProcessPriceSyncQueue
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @QueueID BIGINT;
    DECLARE @ProductID INT;
    DECLARE @NewPrice DECIMAL(18,2);
    DECLARE @priceText VARCHAR(50);
    DECLARE @sql NVARCHAR(MAX);

    WHILE 1 = 1
    BEGIN
        SET @QueueID = NULL;

        SELECT TOP (1)
            @QueueID = QueueID,
            @ProductID = ProductID,
            @NewPrice = NewPrice
        FROM dbo.PriceSyncQueue WITH (READPAST, UPDLOCK, ROWLOCK)
        WHERE Status = 'P'
        ORDER BY QueueID;

        IF @QueueID IS NULL
            BREAK;

        BEGIN TRY
            SET @priceText = REPLACE(CONVERT(VARCHAR(50), @NewPrice), ',', '.');

            SET @sql = N'
UPDATE OPENQUERY(MYSQL,
''SELECT ProductID, Price
  FROM TechStore_Branch.SanPham
  WHERE ProductID = ' + CAST(@ProductID AS NVARCHAR(20)) + N''')
SET Price = ' + @priceText + N';';

            EXEC (@sql);

            UPDATE dbo.PriceSyncQueue
            SET Status = 'S',
                ProcessedAt = SYSDATETIME(),
                LastError = NULL
            WHERE QueueID = @QueueID;
        END TRY
        BEGIN CATCH
            UPDATE dbo.PriceSyncQueue
            SET Status = 'E',
                ProcessedAt = SYSDATETIME(),
                LastError = ERROR_MESSAGE()
            WHERE QueueID = @QueueID;
        END CATCH
    END
END;
GO
