USE TechStore_HQ;
GO

PRINT '====================================================';
PRINT '  TECHSTORE DDB - DATA SEED SCRIPT';
PRINT '  This script will add 50 new products to HQ,';
PRINT '  sync them to Branch, and generate 150 random invoices.';
PRINT '====================================================';
PRINT '';

PRINT '--- 1. INSERTING DUMMY PRODUCTS TO HQ ---';
-- Bảng tạm để lưu ID được sinh tự động từ SQL Server (IDENTITY)
DECLARE @NewProducts TABLE (
    ProductID INT,
    ProductName NVARCHAR(200),
    Price DECIMAL(18,2),
    Category NVARCHAR(100)
);

INSERT INTO dbo.SanPham (ProductName, Price, Category)
OUTPUT INSERTED.ProductID, INSERTED.ProductName, INSERTED.Price, INSERTED.Category INTO @NewProducts
VALUES 
('Dell XPS 15 9530', 45000000, 'Laptop'),
('MacBook Pro 14 M3', 42000000, 'Laptop'),
('ThinkPad X1 Carbon Gen 11', 38000000, 'Laptop'),
('Asus ROG Zephyrus G14', 40000000, 'Laptop'),
('Razer Blade 15', 55000000, 'Laptop'),
('Lenovo Legion 5 Pro', 32000000, 'Laptop'),
('Acer Predator Helios', 35000000, 'Laptop'),
('HP Spectre x360', 36000000, 'Laptop'),
('LG Gram 16', 33000000, 'Laptop'),
('Surface Laptop 5', 28000000, 'Laptop'),
('MacBook Air M2 15-inch', 30000000, 'Laptop'),

('iPhone 15 Pro Max 256GB', 32990000, 'Smartphone'),
('Samsung Galaxy S24 Ultra', 33990000, 'Smartphone'),
('Google Pixel 8 Pro', 24000000, 'Smartphone'),
('iPhone 14', 18990000, 'Smartphone'),
('Samsung Galaxy Z Fold 5', 40990000, 'Smartphone'),
('Samsung Galaxy Z Flip 5', 22990000, 'Smartphone'),
('Xiaomi 14 Pro', 25000000, 'Smartphone'),
('Oppo Find X7 Ultra', 26000000, 'Smartphone'),
('OnePlus 12', 21000000, 'Smartphone'),
('Asus ROG Phone 8', 27000000, 'Smartphone'),

('iPad Pro 12.9 M2', 29000000, 'Tablet'),
('iPad Air 5', 15000000, 'Tablet'),
('Samsung Galaxy Tab S9 Ultra', 28000000, 'Tablet'),
('Lenovo Tab P12 Pro', 14000000, 'Tablet'),
('Xiaomi Pad 6 Max', 12000000, 'Tablet'),

('Apple Watch Ultra 2', 21000000, 'Smartwatch'),
('Apple Watch Series 9', 10000000, 'Smartwatch'),
('Samsung Galaxy Watch 6 Classic', 8000000, 'Smartwatch'),
('Garmin Fenix 7 Pro', 22000000, 'Smartwatch'),

('Sony WH-1000XM5', 7500000, 'Audio'),
('AirPods Pro Gen 2', 5500000, 'Audio'),
('Bose QuietComfort Ultra', 9000000, 'Audio'),
('Sennheiser Momentum 4', 8500000, 'Audio'),
('Jabra Elite 8 Active', 4500000, 'Audio'),
('Blue Yeti USB Microphone', 3200000, 'Audio'),
('Shure SM7B', 10000000, 'Audio'),

('Dell UltraSharp 27 4K', 15000000, 'Monitor'),
('LG UltraGear 27', 9000000, 'Monitor'),
('Samsung Odyssey G9', 35000000, 'Monitor'),
('Asus ProArt Display', 12000000, 'Monitor'),

('Logitech MX Master 3S', 2500000, 'Accessory'),
('Keychron Q1 Pro', 4500000, 'Accessory'),
('Logitech G Pro X Superlight', 3000000, 'Accessory'),
('Razer DeathAdder V3 Pro', 3500000, 'Accessory'),
('SteelSeries Apex Pro TKL', 4800000, 'Accessory'),
('Corsair K100 RGB', 5500000, 'Accessory'),
('Wacom Cintiq 16', 16000000, 'Accessory'),
('Elgato Stream Deck XL', 6000000, 'Accessory'),

('Sony A7 IV Camera', 55000000, 'Camera');

PRINT '   ✓ Added 50 products to HQ (dbo.SanPham).';


PRINT '--- 2. SYNCING PRODUCTS TO BRANCH (MYSQL) ---';
DECLARE @ProductID INT, @ProductName NVARCHAR(200), @Price DECIMAL(18,2), @Category NVARCHAR(100);
DECLARE @sql NVARCHAR(MAX);

-- Dùng Cursor để duyệt qua các sản phẩm vừa tạo và đẩy sang MySQL qua Linked Server
DECLARE curProducts CURSOR FOR SELECT ProductID, ProductName, Price, Category FROM @NewProducts;
OPEN curProducts;
FETCH NEXT FROM curProducts INTO @ProductID, @ProductName, @Price, @Category;

WHILE @@FETCH_STATUS = 0
BEGIN
    SET @sql = N'
    INSERT OPENQUERY(MYSQL, ''SELECT ProductID, ProductName, Price, Category FROM TechStore_Branch.SanPham'')
    VALUES (' + CAST(@ProductID AS NVARCHAR) + N', N''' + REPLACE(@ProductName, '''', '''''') + N''', ' + CAST(@Price AS NVARCHAR) + N', N''' + REPLACE(@Category, '''', '''''') + N''');';
    
    EXEC(@sql);
    FETCH NEXT FROM curProducts INTO @ProductID, @ProductName, @Price, @Category;
END
CLOSE curProducts;
DEALLOCATE curProducts;

PRINT '   ✓ Synced 50 products to Branch (MySQL).';


PRINT '--- 3. INSERTING 150 DUMMY INVOICES TO BRANCH (MYSQL) ---';
-- Sinh ngẫu nhiên 150 hóa đơn phân bổ trong 30 ngày qua để báo cáo đẹp hơn
DECLARE @i INT = 1;
DECLARE @RandProdID INT;
DECLARE @RandQty INT;
DECLARE @RandPrice DECIMAL(18,2);
DECLARE @RandDate DATETIME2(0);
DECLARE @Total DECIMAL(18,2);
DECLARE @MySqlDateStr NVARCHAR(50);

WHILE @i <= 150
BEGIN
    -- Lấy ngẫu nhiên 1 sản phẩm từ danh sách 50 món vừa tạo
    SELECT TOP 1 @RandProdID = ProductID, @RandPrice = Price 
    FROM @NewProducts 
    ORDER BY NEWID();

    -- Random số lượng (1 đến 3)
    SET @RandQty = ABS(CHECKSUM(NEWID())) % 3 + 1;
    SET @Total = @RandQty * @RandPrice;

    -- Random ngày giờ (trong vòng 30 ngày qua)
    SET @RandDate = DATEADD(DAY, - (ABS(CHECKSUM(NEWID())) % 30), SYSDATETIME());
    SET @RandDate = DATEADD(HOUR, - (ABS(CHECKSUM(NEWID())) % 24), @RandDate);
    SET @RandDate = DATEADD(MINUTE, - (ABS(CHECKSUM(NEWID())) % 60), @RandDate);

    -- Format ngày giờ cho MySQL (YYYY-MM-DD HH:MM:SS)
    SET @MySqlDateStr = FORMAT(@RandDate, 'yyyy-MM-dd HH:mm:ss');

    -- Insert vào MySQL (InvoiceID sẽ tự động tăng bên MySQL)
    SET @sql = N'
    INSERT OPENQUERY(MYSQL, ''SELECT ProductID, Quantity, SaleDate, Total FROM TechStore_Branch.HoaDon'')
    VALUES (' + CAST(@RandProdID AS NVARCHAR) + N', ' + CAST(@RandQty AS NVARCHAR) + N', ''' + @MySqlDateStr + N''', ' + CAST(@Total AS NVARCHAR) + N');';
    
    EXEC(@sql);
    
    SET @i = @i + 1;
END

PRINT '   ✓ Added 150 randomized invoices to Branch (MySQL).';
PRINT '';
PRINT '====================================================';
PRINT '  SUCCESS! REFRESH YOUR WEB DASHBOARD TO SEE DATA.';
PRINT '====================================================';
GO
