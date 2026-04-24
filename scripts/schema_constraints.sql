/*
Optional hardening script for schema constraints.
Run SQL Server part on HQ and MySQL part on Branch.
*/

/* =======================
   SQL Server (TechStore_HQ)
   ======================= */
USE TechStore_HQ;
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.check_constraints
    WHERE name = 'CK_SanPham_Price_NonNegative'
)
BEGIN
    ALTER TABLE dbo.SanPham
    ADD CONSTRAINT CK_SanPham_Price_NonNegative CHECK (Price >= 0);
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.check_constraints
    WHERE name = 'CK_SanPham_ProductName_NotEmpty'
)
BEGIN
    ALTER TABLE dbo.SanPham
    ADD CONSTRAINT CK_SanPham_ProductName_NotEmpty CHECK (LEN(LTRIM(RTRIM(ProductName))) >= 2);
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.check_constraints
    WHERE name = 'CK_SanPham_Category_NotEmpty'
)
BEGIN
    ALTER TABLE dbo.SanPham
    ADD CONSTRAINT CK_SanPham_Category_NotEmpty CHECK (LEN(LTRIM(RTRIM(Category))) >= 2);
END
GO

/* =======================
   MySQL (TechStore_Branch)
   Run this section in MySQL Workbench
   =======================

USE TechStore_Branch;

ALTER TABLE SanPham
    MODIFY ProductName VARCHAR(200) NOT NULL,
    MODIFY Price DECIMAL(18,2) NOT NULL,
    MODIFY Category VARCHAR(100) NOT NULL;

ALTER TABLE HoaDon
    MODIFY Quantity INT NOT NULL,
    MODIFY SaleDate DATETIME NOT NULL,
    MODIFY Total DECIMAL(18,2) NOT NULL;

*/
