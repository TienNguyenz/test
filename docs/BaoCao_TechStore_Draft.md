BỘ GIÁO DỤC VÀ ĐÀO TẠO
TRƯỜNG ĐẠI HỌC SÀI GÒN
Khoa Công nghệ Thông tin


BÁO CÁO ĐỒ ÁN MÔN HỌC
CƠ SỞ DỮ LIỆU PHÂN TÁN

ĐỀ TÀI:
TECHSTORE - QUẢN LÝ CHUỖI BÁN LẺ ĐA NỀN TẢNG (LIÊN THÔNG MYSQL VÀ SQL SERVER)

Giảng viên hướng dẫn: Nguyễn Quốc Huy
Sinh viên thực hiện: Nguyễn Hoàng Tiến (3121411206)
Lê Duy Quân (3121411176)
Lớp: DTCT121C5
Năm học: 2023 - 2024

Thành phố Hồ Chí Minh, tháng 4 năm 2024.


---

# LỜI MỞ ĐẦU
Ngày nay, tin học ngày càng phát triển nhanh chóng và được ứng dụng rộng rãi trong mọi lĩnh vực của đời sống xã hội. Việc học tập và nắm bắt công nghệ mới đặc biệt là công nghệ thông tin ngày càng trở nên bức thiết. Đối với sinh viên khối ngành công nghệ thông tin, việc tích cực học tập, nắm vững mọi kiến thức về phân tích hệ thống dữ liệu lớn và cơ sở dữ liệu phân tán được xem là cơ sở, nền tảng để xây dựng các phần mềm cấp doanh nghiệp (Enterprise Software).
Đồ án Cơ sở dữ liệu phân tán giúp sinh viên hiểu được tầm quan trọng của việc kiến trúc dữ liệu và giải quyết những dự án cụ thể mang tính ứng dụng thực tế cao. Sau một thời gian học tập và nghiên cứu môn học để nắm bắt những kiến thức đã học một cách tốt hơn, nhóm chúng em đã thực hiện đồ án: "TechStore - Quản lý Chuỗi Bán lẻ Đa nền tảng (Liên thông MySQL và SQL Server)".

# LỜI CẢM ƠN
Đầu tiên, chúng em xin gửi lời cảm ơn chân thành đến Khoa Công nghệ thông tin, Trường đại học Sài Gòn đã tạo điều kiện thuận lợi cho chúng em học tập và hoàn thành đồ án môn học này. Đặc biệt, em xin bày tỏ lòng biết ơn sâu sắc đến thầy Nguyễn Quốc Huy đã dày công truyền đạt kiến thức và hướng dẫn tận tình cho chúng em.
Chúng em đã cố gắng vận dụng những kiến thức đã học được trong học kỳ qua để hoàn thành đồ án này. Tuy nhiên, do kiến thức còn hạn chế và chưa có nhiều kinh nghiệm cọ xát với các hệ thống phân tán đa hệ quản trị trên thực tiễn nên khó tránh khỏi những thiếu sót trong quá trình phân tích và lập trình. Chúng em rất mong nhận được sự góp ý của thầy để đồ án môn học của chúng em được hoàn thiện hơn.
Chúng em xin chân thành cảm ơn!

---

# MỤC LỤC
CHƯƠNG 1: TỔNG QUAN VỀ ĐỀ TÀI
CHƯƠNG 2: CƠ SỞ LÝ THUYẾT VÀ CÔNG NGHỆ ÁP DỤNG
CHƯƠNG 3: PHÂN TÍCH VÀ THIẾT KẾ CƠ SỞ DỮ LIỆU
CHƯƠNG 4: TRIỂN KHAI VÀ CÀI ĐẶT HỆ THỐNG
CHƯƠNG 5: KIỂM THỬ VÀ ĐÁNH GIÁ (TEST CASES)
CHƯƠNG 6: KẾT LUẬN VÀ HƯỚNG PHÁT TRIỂN

---

# CHƯƠNG 1: TỔNG QUAN VỀ ĐỀ TÀI

## 1.1 Đặt vấn đề và Nhu cầu thực tế
Thế giới ngày càng hội nhập, tạo ra cho doanh nghiệp thêm rất nhiều cơ hội. Một trong những cơ hội đó là mở rộng thị trường kinh doanh thành các chuỗi bán lẻ, mua bán sáp nhập (M&A) các công ty con. Sau các thương vụ sáp nhập, bài toán đau đầu nhất của IT là khác biệt công nghệ cốt lõi. Công ty mẹ (Trụ sở chính) thường dùng công nghệ khác (ví dụ: SQL Server, Oracle) so với các chi nhánh mới sáp nhập (ví dụ: MySQL, PostgreSQL).
Việc đập bỏ và xây lại toàn bộ hệ thống từ đầu sẽ cực kỳ tốn kém chi phí, mất thời gian và gây gián đoạn hoạt động kinh doanh trực tiếp của chi nhánh. 

## 1.2 Mục tiêu của Đồ án
Mục tiêu chính của đề tài "TechStore - Quản lý Chuỗi Bán lẻ Đa nền tảng" bao gồm:
1. **Kết nối Đa hệ quản trị:** Cấu hình thành công kết nối (Linked Server) giữa SQL Server (Trụ sở) và MySQL (Chi nhánh).
2. **Truy vấn trong suốt:** Đứng tại Server Trụ sở có thể truy vấn, thực hiện phép kết (JOIN) xuyên qua Server Chi nhánh để lập báo cáo doanh thu.
3. **Đồng bộ tự động:** Khi giá sản phẩm thay đổi ở Trụ sở, giá ở Chi nhánh tự động cập nhật theo thông qua cơ chế Message Queue.
4. **Xử lý bất đồng bộ dữ liệu:** Xử lý triệt để các sai khác về kiểu dữ liệu (DateTime vs Timestamp) giữa 2 Engine.
5. **Trực quan hóa:** Xây dựng một ứng dụng Web (ASP.NET Core Razor Pages) để giao tiếp với Database, giúp quan sát kết quả một cách trực quan, sinh động.

---

# CHƯƠNG 2: CƠ SỞ LÝ THUYẾT VÀ CÔNG NGHỆ ÁP DỤNG

## 2.1 Hệ quản trị CSDL Phân tán và Liên thông đa hệ
Hệ quản trị cơ sở dữ liệu phân tán (DDBMS) là hệ thống phần mềm cho phép quản lý CSDL phân tán và làm cho sự phân tán này trở nên trong suốt đối với người sử dụng. Trong trường hợp hệ thống không đồng nhất (Heterogeneous), mỗi node có thể sử dụng phần mềm DBMS khác nhau.

## 2.2 Công nghệ ODBC và Linked Server
- **ODBC (Open Database Connectivity):** Là một giao diện lập trình ứng dụng (API) chuẩn để truy cập các hệ quản trị CSDL. Đồ án sử dụng MySQL ODBC Driver để làm cầu nối trung gian.
- **Linked Server:** Chức năng của SQL Server cho phép Server này thực thi các truy vấn trên OLE DB Data sources trên các server từ xa. Thông qua Linked Server, người lập trình có thể dùng câu lệnh `OPENQUERY` để thực hiện câu lệnh SQL theo chuẩn của hệ quản trị đích (MySQL) và trả kết quả về SQL Server dưới dạng Recordset có thể tương tác.

## 2.3 Bài toán Distributed Transaction và Kiến trúc Queue-based Sync
Khi thực hiện thao tác UPDATE/INSERT/DELETE thông qua Linked Server từ một Trigger của SQL Server, hệ thống sẽ kích hoạt giao dịch phân tán (Distributed Transaction Coordinator - MSDTC). Tuy nhiên MSDTC rất khó cấu hình để làm việc với MySQL, thường sinh ra lỗi `Msg 7391`.
Để khắc phục, đồ án áp dụng kiến trúc **Hàng đợi (Queue-based Sync)**:
- Không gửi trực tiếp qua Linked Server. Thay vào đó, ghi log thay đổi vào một bảng `PriceSyncQueue`.
- Dùng một Background Job hoặc Stored Procedure (`sp_ProcessPriceSyncQueue`) để tuần tự đọc hàng đợi, mở kết nối Linked Server và đẩy dữ liệu qua một cách độc lập.

---

# CHƯƠNG 3: PHÂN TÍCH VÀ THIẾT KẾ CƠ SỞ DỮ LIỆU

## 3.1 Mô hình thực thể liên kết (ERD)
*(Sinh viên chèn hình ảnh sơ đồ ERD mô tả mối quan hệ giữa SanPham, HoaDon và PriceSyncQueue vào đây)*

## 3.2 Cấu trúc các bảng của hệ thống
Hệ thống lưu trữ trên 2 Server độc lập. Sau đây là chi tiết cấu trúc dữ liệu vật lý (Data Dictionary):

### 3.2.1 Bảng SanPham (Tại SQL Server - HQ)
Lưu thông tin sản phẩm gốc tại tổng công ty.
| STT | Tên thuộc tính | Kiểu dữ liệu | Ràng buộc | Khóa | Mô tả |
|---|---|---|---|---|---|
| 1 | ProductID | int | Not null, Identity | PK | Mã sản phẩm |
| 2 | ProductName | varchar(200) | Not null | | Tên sản phẩm |
| 3 | Category | varchar(100) | Not null | | Danh mục sản phẩm |
| 4 | Price | decimal(18, 2) | Not null, >= 0 | | Giá sản phẩm gốc |

### 3.2.2 Bảng PriceSyncQueue (Tại SQL Server - HQ)
Lưu trữ trạng thái hàng đợi các tác vụ đồng bộ giá sang chi nhánh.
| STT | Tên thuộc tính | Kiểu dữ liệu | Ràng buộc | Khóa | Mô tả |
|---|---|---|---|---|---|
| 1 | QueueID | bigint | Not null, Identity | PK | ID hàng đợi |
| 2 | ProductID | int | Not null | FK | Mã sản phẩm cần update |
| 3 | NewPrice | decimal(18, 2) | Not null | | Giá mới cần đồng bộ |
| 4 | Status | char(1) | Default 'P' | | P: Pending, S: Success, E: Error |
| 5 | CreatedAt | datetime2(0) | Default GetDate | | Thời gian tạo Queue |
| 6 | ProcessedAt | datetime2(0) | Null | | Thời gian thực thi xong |
| 7 | LastError | nvarchar(1000) | Null | | Chuỗi ghi nhận lỗi (nếu có) |

### 3.2.3 Bảng SanPham (Tại MySQL - Branch)
Bản sao của bảng sản phẩm tại chi nhánh dùng để bán hàng.
| STT | Tên thuộc tính | Kiểu dữ liệu | Ràng buộc | Khóa | Mô tả |
|---|---|---|---|---|---|
| 1 | ProductID | int | Not null | PK | Mã sản phẩm đồng bộ từ HQ |
| 2 | ProductName | varchar(200) | Not null | | Tên sản phẩm |
| 3 | Category | varchar(100) | Not null | | Danh mục sản phẩm |
| 4 | Price | decimal(18, 2) | Not null | | Giá bán tại chi nhánh |

### 3.2.4 Bảng HoaDon (Tại MySQL - Branch)
Ghi nhận các giao dịch xuất bán thực tế tại cơ sở.
| STT | Tên thuộc tính | Kiểu dữ liệu | Ràng buộc | Khóa | Mô tả |
|---|---|---|---|---|---|
| 1 | InvoiceID | int | Not null, Auto_Inc | PK | Mã hóa đơn |
| 2 | ProductID | int | Not null | FK | Mã sản phẩm bán ra |
| 3 | Quantity | int | Not null | | Số lượng bán |
| 4 | Total | decimal(18, 2) | Not null | | Tổng tiền (Quantity * Price) |
| 5 | SaleDate | datetime | Not null | | Ngày giờ xuất hóa đơn |

---

# CHƯƠNG 4: TRIỂN KHAI VÀ CÀI ĐẶT HỆ THỐNG

## 4.1 Chuẩn bị môi trường và cài đặt ODBC Driver
**Bước 1: Cài đặt ODBC Driver cho MySQL**
- Truy cập trang chủ MySQL, tải `MySQL ODBC 8.0 Driver`.
- Tiến hành cài đặt (Next > Next > Finish).
*(Chèn hình ảnh bộ cài ODBC)*

**Bước 2: Cấu hình System DSN**
- Mở "ODBC Data Sources (64-bit)" trên Windows. Chọn tab System DSN > Add.
- Chọn `MySQL ODBC 8.0 Unicode Driver`.
- Điền các thông số:
  + Data Source Name: `MySQL_Branch`
  + TCP/IP Server: `localhost` hoặc IP của máy ảo MySQL.
  + User / Password: `tien` / `*******`
  + Database: `TechStore_Branch`
- Nhấn "Test" để xác nhận kết nối thành công.
*(Chèn hình chụp cửa sổ Test Connection ODBC Success)*

## 4.2 Cấu hình CSDL phân tán Linked Server
Trong Microsoft SQL Server Management Studio (SSMS), kết nối với máy chủ SQL Server (HQ).
**Bước 1:** Mở rộng thư mục Server Objects > Linked Servers.
**Bước 2:** Chuột phải chọn "New Linked Server...".
**Bước 3:** Ở tab General, điền:
- Linked server: `MYSQL`
- Provider: `Microsoft OLE DB Provider for ODBC Drivers`
- Data source: `MySQL_Branch`
**Bước 4:** Ở tab Security, chọn "Be made using this security context" và điền User/Password của MySQL. Nhấn OK.
*(Chèn hình chụp cửa sổ cấu hình New Linked Server)*

## 4.3 Triển khai Stored Procedure truy vấn trong suốt (OPENQUERY)
Tại cơ sở dữ liệu `TechStore_HQ`, viết Stored Procedure sử dụng cú pháp OPENQUERY để lấy hóa đơn từ MySQL và JOIN với bảng Sản Phẩm tại SQL Server. Chú ý bước ép kiểu dữ liệu để xử lý sai khác nền tảng:

```sql
CREATE OR ALTER PROCEDURE dbo.sp_BaoCaoDoanhThu
    @FromDate DATETIME2(0) = NULL,
    @ToDate   DATETIME2(0) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    ;WITH x AS (
        SELECT h.ProductID, SUM(h.Quantity) AS SoLuongBan, SUM(h.Total) AS DoanhThu, MAX(h.SaleDate) AS LanBanGanNhat
        FROM (
            SELECT 
                CAST(ProductID AS INT) AS ProductID, 
                CAST(Quantity AS INT) AS Quantity,
                CAST(SaleDate AS DATETIME2(0)) AS SaleDate, 
                CAST(Total AS DECIMAL(18,2)) AS Total
            FROM OPENQUERY(MYSQL, 'SELECT ProductID, Quantity, SaleDate, Total FROM TechStore_Branch.HoaDon')
        ) h
        WHERE (@FromDate IS NULL OR h.SaleDate >= @FromDate)
          AND (@ToDate IS NULL OR h.SaleDate < DATEADD(DAY, 1, @ToDate))
        GROUP BY h.ProductID
    )
    SELECT p.ProductID, p.ProductName, p.Category, CAST(p.Price AS DECIMAL(18,2)) AS GiaTaiHQ,
           ISNULL(x.SoLuongBan, 0) AS SoLuongBan, ISNULL(x.DoanhThu, 0) AS DoanhThu, x.LanBanGanNhat
    FROM dbo.SanPham p LEFT JOIN x ON p.ProductID = x.ProductID
    ORDER BY DoanhThu DESC, p.ProductID ASC;
END;
```

## 4.4 Triển khai cơ chế Hàng đợi (Queue-based Sync)
**Bước 1: Tạo Trigger thu thập dữ liệu giá thay đổi**
```sql
CREATE OR ALTER TRIGGER dbo.trg_SanPham_QueuePriceSync
ON dbo.SanPham AFTER UPDATE
AS BEGIN
    SET NOCOUNT ON;
    IF NOT UPDATE(Price) RETURN;
    
    INSERT INTO dbo.PriceSyncQueue(ProductID, NewPrice)
    SELECT i.ProductID, i.Price
    FROM inserted i JOIN deleted d ON i.ProductID = d.ProductID
    WHERE ISNULL(i.Price, -1) <> ISNULL(d.Price, -1);
END;
```

**Bước 2: Viết hàm quét và xử lý Queue**
```sql
CREATE OR ALTER PROCEDURE dbo.sp_ProcessPriceSyncQueue
AS BEGIN
    SET NOCOUNT ON;
    DECLARE @QueueID BIGINT, @ProductID INT, @NewPrice DECIMAL(18,2), @sql NVARCHAR(MAX);
    WHILE 1 = 1 BEGIN
        SELECT TOP (1) @QueueID = QueueID, @ProductID = ProductID, @NewPrice = NewPrice
        FROM dbo.PriceSyncQueue WITH (READPAST, UPDLOCK, ROWLOCK) WHERE Status = 'P' ORDER BY QueueID;
        
        IF @QueueID IS NULL BREAK;

        BEGIN TRY
            SET @sql = N'UPDATE OPENQUERY(MYSQL, ''SELECT ProductID, Price FROM TechStore_Branch.SanPham WHERE ProductID = ' + CAST(@ProductID AS NVARCHAR(20)) + N''') SET Price = ' + REPLACE(CONVERT(VARCHAR(50), @NewPrice), ',', '.') + N';';
            EXEC (@sql);
            UPDATE dbo.PriceSyncQueue SET Status = 'S', ProcessedAt = SYSDATETIME(), LastError = NULL WHERE QueueID = @QueueID;
        END TRY
        BEGIN CATCH
            UPDATE dbo.PriceSyncQueue SET Status = 'E', ProcessedAt = SYSDATETIME(), LastError = ERROR_MESSAGE() WHERE QueueID = @QueueID;
        END CATCH
    END
END;
```

---

# CHƯƠNG 5: KIỂM THỬ VÀ ĐÁNH GIÁ (TEST CASES)

Dự án đã tiến hành xây dựng 10 Test Cases (8 Test Cases bắt buộc theo yêu cầu đề tài, 2 Test Cases mở rộng) nhằm kiểm định độ tin cậy của toàn hệ thống phân tán.

| Mã TC | Mục tiêu kiểm thử | Các bước thực hiện | Kết quả mong đợi | Đánh giá |
|---|---|---|---|---|
| TC01 | Kiểm tra kết nối Linked Server | Chạy lệnh `sp_testlinkedserver 'MYSQL'` trong SSMS. | Hệ thống trả về trạng thái Success, không bắn lỗi network. | PASS |
| TC02 | Kiểm tra truy vấn xuyên hệ thống | Thực thi OPENQUERY chọn dữ liệu từ `TechStore_Branch.HoaDon` | Dữ liệu hóa đơn từ MySQL hiển thị trực tiếp trên màn hình SSMS. | PASS |
| TC03 | Kiểm tra Báo cáo liên thông | Chạy procedure `sp_BaoCaoDoanhThu` tại HQ. | Xuất ra bảng dữ liệu đã JOIN thành công giữa bảng Sản Phẩm (SQL) và Hóa Đơn (MySQL). | PASS |
| TC04 | Kiểm tra tạo Trigger đồng bộ | Cập nhật giá một sản phẩm bất kỳ trong bảng `SanPham` tại HQ. | 1 row affected. Dữ liệu trong bảng `SanPham` thay đổi. | PASS |
| TC05 | Kiểm tra ghi nhận Hàng đợi (Queue) | Xem bảng `PriceSyncQueue` sau khi làm TC04. | Sinh ra 1 bản ghi mới trạng thái 'P' (Pending). | PASS |
| TC06 | Kiểm tra Đồng bộ dữ liệu sang Chi nhánh | Chạy hàm `sp_ProcessPriceSyncQueue`. Sau đó SELECT bảng SanPham ở MySQL. | Record Queue chuyển sang 'S'. Giá sản phẩm bên MySQL bằng giá bên HQ. | PASS |
| TC07 | Kiểm thử khả năng chịu lỗi mạng/quyền | Cố tình thu hồi quyền UPDATE bên MySQL. Chạy Queue processor. | Tiến trình không Crash. Trạng thái Queue thành 'E'. Lưu lại mã lỗi vào cột LastError. | PASS |
| TC08 | Kiểm tra tương thích dữ liệu Filter | Gọi hàm Báo cáo kèm tham số truyền vào `@FromDate` và `@ToDate`. | Trả về đúng kết quả doanh thu trong khoảng thời gian, không lỗi Cast DateTime. | PASS |
| TC09 | Kiểm thử luồng CRUD mở rộng | Thêm/Sửa/Xóa sản phẩm qua giao diện Web Razor Pages. | Dữ liệu thay đổi chuẩn xác, luồng Sync Queue được kích hoạt tự động qua code C# | PASS |
| TC10 | Kiểm thử Ràng buộc dữ liệu (Constraints) | Nhập giá trị âm cho sản phẩm tại giao diện Web hoặc SSMS. | Hệ thống báo lỗi vi phạm Check Constraint (CK_SanPham_Price_NonNegative). | PASS |

### HÌNH ẢNH MINH CHỨNG TEST CASES
**Minh chứng TC01: Kết nối Linked Server**
*(Chèn hình ảnh Message Success khi chạy sp_testlinkedserver)*

**Minh chứng TC03: Báo cáo liên thông từ Web Application**
*(Chèn hình ảnh giao diện Web Razor Pages phần Revenue Report hiển thị biểu đồ và bảng)*

**Minh chứng TC05 & TC06: Luồng đồng bộ Queue**
*(Chèn hình ảnh xem dữ liệu bảng PriceSyncQueue có Status = S và bảng SanPham bên MySQL đã đổi giá)*

---

# CHƯƠNG 6: KẾT LUẬN VÀ HƯỚNG PHÁT TRIỂN

## 6.1 Kết luận
Sau quá trình nghiên cứu, phân tích và triển khai code thực tế, đồ án "TechStore - Quản lý Chuỗi Bán lẻ Đa nền tảng" đã hoàn thành 100% các tiêu chí mà Đề tài 2 yêu cầu.
Hệ thống giải quyết triệt để bài toán doanh nghiệp: Không cần chi phí di chuyển, sáp nhập dữ liệu lớn mà vẫn có thể lập báo cáo tổng hợp nhanh chóng và đồng bộ hóa thông số thiết yếu (giá cả) giữa hai nền tảng công nghệ dị đồng (SQL Server & MySQL). Giải pháp áp dụng Hàng đợi (Queue-based Sync) chứng minh sự ưu việt trong khả năng chịu lỗi (Fault-tolerant) thay vì dùng Distributed Transaction cổ điển.
Việc xây dựng một hệ thống Web quản trị hoàn chỉnh bằng ASP.NET Core Razor Pages góp phần giúp trải nghiệm đồ án môn học trở nên trực quan, hiện đại, mang tính ứng dụng thực tế cao.

## 6.2 Hướng phát triển trong tương lai
- Phát triển thêm hệ thống SQL Server Agent Job tự động quét Queue mỗi 1 phút một lần thay vì phải gọi thủ công.
- Tích hợp thêm các chi nhánh sử dụng PostgreSQL hoặc Oracle thông qua việc thêm các Linked Server và Middleware tương ứng.
- Viết API riêng (RESTful/gRPC) để đồng bộ dữ liệu thay vì dùng Linked Server khi số lượng máy trạm vượt quá vài chục node nhằm giảm tải cho Database chính.

---

# TÀI LIỆU THAM KHẢO
1. Slide bài giảng Cơ sở dữ liệu phân tán - Trường Đại học Sài Gòn (Thầy Nguyễn Quốc Huy).
2. Microsoft Learn Documentation (SQL Server Linked Servers & OPENQUERY).
3. MySQL 8.0 Reference Manual - ODBC Connectors.
4. Tài liệu hướng dẫn phát triển ứng dụng Web bằng ASP.NET Core Razor Pages (Microsoft).
