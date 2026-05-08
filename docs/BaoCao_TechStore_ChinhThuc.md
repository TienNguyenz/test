BỘ GIÁO DỤC VÀ ĐÀO TẠO
TRƯỜNG ĐẠI HỌC SÀI GÒN


<br><br><br><br>

# BÁO CÁO ĐỒ ÁN
## ĐỀ TÀI:
## LIÊN THÔNG MYSQL VÀ SQL SERVER
*(HỆ THỐNG QUẢN LÝ CHUỖI BÁN LẺ TECHSTORE ĐA NỀN TẢNG)*

<br>

**Giảng viên hướng dẫn :** Vũ Ngọc Thanh Sang
**Sinh viên thực hiện (MSSV) :** 
1. Nguyễn Hoàng Tiến (3121411206)
2. Lê Duy Quân (3121411176)

**Lớp :** DCT123C2
**Năm học :** 2026 - 2027

<br><br><br><br>

*Thành phố Hồ Chí Minh, tháng 4 năm 2026.*


---

## LỜI MỞ ĐẦU

Ngày nay, tin học ngày càng phát triển nhanh chóng và được ứng dụng rộng rãi trong mọi lĩnh vực của đời sống xã hội, việc học và nắm bắt công nghệ mới đặc biệt là công nghệ thông tin ngày càng trở nên bức thiết. Đối với sinh viên trong ngành cũng phải tích cực học tập, nắm vững mọi kiến thức về công nghệ thông tin, trong đó thương mại điện tử và ứng dụng CSDL Phân Tán được xem là cơ sở, nền tảng đầu tiên.

Thương mại điện tử và ứng dụng cho sinh viên hiểu được tầm quan trọng của phân tích và cách thiết kế hệ thống thông tin để giải quyết những dự án cụ thể.
Sau một thời gian học tập và nghiên cứu môn học phân tích thiết kế hệ thống thông tin, để nắm bắt những kiến thức đã học một cách tốt hơn, nhóm chúng em đã thực hiện đề tài xây dựng hệ thống quản lý chuỗi bán lẻ TechStore đa nền tảng (SQL Server và MySQL).

---

## LỜI CẢM ƠN

Đầu tiên, chúng em xin gửi lời cảm ơn chân thành đến Khoa Công nghệ thông tin, Trường đại học Sài Gòn đã tạo điều kiện thuận lợi cho chúng em học tập và hoàn thành đồ án môn học này. Đặc biệt, em xin bày tỏ lòng biết ơn sâu sắc đến thầy Vũ Ngọc Thanh Sang đã dày công truyền đạt kiến thức cho chúng em.

Chúng em đã cố gắng vận dụng những kiến thức đã học được trong học kỳ qua để hoàn thành đồ án này. Nhưng do kiến thức hạn chế và không có nhiều kinh nghiệm thực tiễn nên khó tránh khỏi những thiếu sót trong quá trình làm bài và trình bày. Chúng em rất mong nhận được sự góp ý của thầy để đồ án môn học của chúng em được hoàn thiện hơn.

Chúng em xin chân thành cảm ơn.

---

## NHẬN XÉT, ĐÁNH GIÁ

...........................................................................................................................................
...........................................................................................................................................
...........................................................................................................................................
...........................................................................................................................................
...........................................................................................................................................
...........................................................................................................................................
...........................................................................................................................................
...........................................................................................................................................
...........................................................................................................................................
...........................................................................................................................................

**Điểm:** ................................... (Bằng chữ: ...................................)

---
<div style="page-break-after: always;"></div>

## PHẦN 1: PHÂN TÁN DỮ LIỆU

### PHẦN A: KHÁI NIỆM

#### 1. Đặt vấn đề
**1.1 Nhu cầu và tầm quan trọng của dự án:**
Thế giới ngày càng hội nhập, tạo ra cho doanh nghiệp rất nhiều cơ hội mở rộng thị trường kinh doanh thành các chuỗi chi nhánh nhằm tối đa hóa lợi nhuận. Tuy nhiên, sau các thương vụ mua bán và sáp nhập, doanh nghiệp TechStore phải đối mặt với bài toán khác biệt công nghệ. Các chi nhánh mới sáp nhập thường dùng công nghệ khác (ví dụ: MySQL) so với Trụ sở chính (dùng SQL Server). 
Việc xây lại toàn bộ hệ thống là cực kỳ tốn kém và gây gián đoạn kinh doanh. Việc ứng dụng CSDL phân tán và công nghệ liên thông đa hệ quản trị (Heterogeneous Distributed Database) giúp tích hợp hệ thống mà không cần di chuyển dữ liệu (Data Migration), giảm thiểu tối đa chi phí quản lý.

**1.2 Sơ lược về dự án:**
- Dữ liệu Sản phẩm (Products) là dữ liệu chung được quản lý và cập nhật tại Máy chủ Trụ sở chính.
- Dữ liệu Hóa đơn (Invoices), Giao dịch là dữ liệu phát sinh tại Chi nhánh và được cập nhật tại Máy trạm Chi nhánh.
- Dự án được triển khai tích hợp 2 hệ quản trị khác nhau: SQL Server và MySQL thông qua Linked Server.

**1.3 Vị trí và nhiệm vụ, dữ liệu khi triển khai dự án:**
Trong phạm vi đồ án học thuật, kiến trúc vật lý phân tán được **mô phỏng trên môi trường cục bộ (Localhost)**. Tuy chạy chung trên một máy tính, nhưng đây là 2 tiến trình mạng (Network Processes) hoàn toàn độc lập:
- **Trụ sở chính (HQ - SQL Server qua SSMS - Port 1433):** Đóng vai trò máy chủ trung tâm. Nhiệm vụ quản lý dữ liệu gốc (Sản phẩm), cấu hình cơ chế đồng bộ giá và nhận báo cáo doanh thu.
- **Chi nhánh (Branch - MySQL qua MySQL Workbench - Port 3306):** Đóng vai trò máy trạm. Nhiệm vụ quản lý dữ liệu giao dịch bán hàng, hóa đơn. Hai hệ thống giao tiếp với nhau qua giao thức mạng ODBC, phản ánh chính xác bản chất hệ thống phân tán thực tế. 

**1.4 Các đối tượng tham gia sử dụng dự án:**
- **Nhân viên bán hàng (tại Chi nhánh):** Có quyền xem thông tin sản phẩm và thực hiện giao dịch (tạo hóa đơn) trên MySQL.
- **Quản lý hệ thống / Tổng giám đốc (tại Trụ sở chính):** Quản lý toàn bộ danh mục sản phẩm, có quyền thay đổi giá. Hệ thống sẽ tự động đồng bộ giá về chi nhánh. Xem báo cáo thống kê hợp nhất xuyên CSDL.

#### 2. Phân tích hệ thống
**2.1 Phân tích các chức năng chính của hệ thống:**
- Quản lý thông tin Sản phẩm (HQ).
- Quản lý thông tin Hóa đơn (Branch).
- Truy vấn hợp nhất (Trong suốt phân tán): Xem báo cáo tổng doanh thu kết hợp dữ liệu giữa HQ và Branch.
- Đồng bộ dữ liệu bất đồng bộ: Sử dụng Message Queue để tự động đồng bộ giá từ HQ sang Branch một cách an toàn.

**2.2 Chức năng của máy trạm và máy chủ:**
- **Máy trạm (MySQL):** Quản lý lưu trữ hóa đơn phân tán. Đảm bảo nhân viên bán hàng vẫn thao tác bình thường ngay cả khi đứt kết nối mạng với trụ sở chính.
- **Máy chủ (SQL Server):** Chịu trách nhiệm đồng bộ hóa, cài đặt Logical View với `OPENQUERY` để truy vấn trực tiếp vào MySQL mà ứng dụng phía trên (Web App) không hề hay biết.

**2.3 Mô hình thực thể liên kết (ERD) và Cấu trúc bảng:**
- **Bảng `SanPham` (Tại SQL Server):** `ProductID` (PK), `ProductName`, `Category`, `Price`.
- **Bảng `PriceSyncQueue` (Tại SQL Server):** `QueueID` (PK), `ProductID`, `NewPrice`, `Status`, `CreatedAt`. Dùng để ghi nhận lịch sử đồng bộ.
- **Bảng `HoaDon` (Tại MySQL):** `InvoiceID` (PK), `ProductID` (FK), `Quantity`, `SaleDate`, `Total`.

**2.4 Thiết kế hệ thống mạng tổng quan:**
- Máy chủ HQ (SQL Server) đóng vai trò là Trung tâm điều phối (Coordinator).
- Sử dụng Microsoft OLE DB Provider for ODBC Drivers (MSDASQL) tạo Linked Server kết nối mạng sang máy trạm Chi nhánh (MySQL).

---

### PHẦN B: PHƯƠNG PHÁP VÀ CÀI ĐẶT

#### 1. Database sử dụng & Lựa chọn phân mảnh
Khác với phương pháp phân mảnh ngang/dọc trên cùng 1 hệ quản trị, dự án sử dụng mô hình **Phân mảnh theo ranh giới ứng dụng đa nền tảng (Heterogeneous Database)**. 
- Dữ liệu Sản phẩm nằm ở SQL Server.
- Dữ liệu Hóa đơn nằm ở MySQL.
- Ứng dụng tích hợp chúng lại bằng Lớp ảo hóa (Logical View) sử dụng lệnh `OPENQUERY`.

#### 2. Cấu trúc Database và Import CSDL
**2.1 Tạo cấu trúc phía Trụ sở chính (SQL Server):**
```sql
CREATE TABLE dbo.SanPham (
    ProductID INT IDENTITY(1,1) PRIMARY KEY,
    ProductName NVARCHAR(255) NOT NULL,
    Category NVARCHAR(100),
    Price DECIMAL(18,2) NOT NULL
);

CREATE TABLE dbo.PriceSyncQueue (
    QueueID BIGINT IDENTITY(1,1) PRIMARY KEY,
    ProductID INT NOT NULL,
    NewPrice DECIMAL(18,2) NOT NULL,
    Status CHAR(1) DEFAULT 'P', -- P: Pending, S: Success, E: Error
    CreatedAt DATETIME2 DEFAULT SYSDATETIME(),
    ProcessedAt DATETIME2,
    LastError NVARCHAR(MAX)
);
```

**2.2 Tạo cấu trúc phía Chi nhánh (MySQL):**
```sql
CREATE TABLE TechStore_Branch.HoaDon (
    InvoiceID INT AUTO_INCREMENT PRIMARY KEY,
    ProductID INT NOT NULL,
    Quantity INT NOT NULL,
    SaleDate DATETIME NOT NULL,
    Total DECIMAL(18,2) NOT NULL
);
```

#### 3. Cài đặt Liên thông (Linked Server) và Kỹ thuật OPENQUERY
Để Web App có thể lấy dữ liệu Hóa đơn từ MySQL thông qua SQL Server một cách "trong suốt", chúng ta định nghĩa Logical View tại SQL Server:

```sql
CREATE VIEW dbo.vw_BranchInvoices_Logical AS
SELECT InvoiceID, ProductID, Quantity, SaleDate, Total
FROM OPENQUERY(MYSQL, 'SELECT * FROM TechStore_Branch.HoaDon');
```

#### 4. Giải pháp Đồng bộ dữ liệu (Chịu lỗi mạng)
Sử dụng **Trigger** kết hợp **Stored Procedure** để đảm bảo tính toàn vẹn dữ liệu.
Khi giá Sản Phẩm tại HQ thay đổi, Trigger tự động lưu vào Queue:

```sql
CREATE TRIGGER trg_SanPham_QueuePriceSync
ON dbo.SanPham AFTER UPDATE AS
BEGIN
    INSERT INTO dbo.PriceSyncQueue (ProductID, NewPrice, Status)
    SELECT i.ProductID, i.Price, 'P' FROM inserted i;
END;
```

Sau đó, tiến trình ngầm sẽ quét Queue và gọi lệnh `UPDATE OPENQUERY` sang MySQL:
```sql
UPDATE OPENQUERY(MYSQL, 'SELECT ProductID, Price FROM TechStore_Branch.SanPham ...')
SET Price = @NewPrice;
```
*(Nếu đứt mạng, Status sẽ chuyển thành 'E' và chờ mạng có lại để đồng bộ tiếp. Tránh hoàn toàn lỗi crash hệ thống).*

---

### PHẦN C: KỊCH BẢN KIỂM THỬ VÀ DEMO

*(Sinh viên thêm các hình ảnh giao diện Web tại đây để minh chứng cho hệ thống)*

1. **Test case 1: Truy vấn hợp nhất (Trong suốt phân tán)**
- Truy cập trang Báo cáo Doanh thu (Revenue).
- Hệ thống hiển thị doanh thu thành công dù Dữ liệu Hóa đơn nằm bên MySQL và Sản phẩm nằm bên SQL Server.
- *(Chèn ảnh trang Revenue)*

2. **Test case 2: Đồng bộ giá an toàn (Fault Tolerance)**
- Truy cập trang Price Synchronization. Cập nhật giá sản phẩm "iPhone 15".
- Hệ thống lập tức lưu vào Queue (Status: P), đồng bộ sang MySQL thành công (Status chuyển sang S).
- *(Chèn ảnh trang Price Sync có bảng Recent Queue Events màu xanh)*

3. **Test case 3: Chứng minh liên kết đa nền tảng**
- Truy cập trang Evidence. Hiển thị số lượng bản ghi Live từ cả 2 hệ quản trị (HQ Products: 50, Branch Invoices: 150).
- Ngắt kết nối MySQL, trang Evidence lập tức báo lỗi Failed, chứng minh truy vấn đi qua Linked Server thực tế.
- *(Chèn ảnh trang Evidence)*

---
**[HẾT BÁO CÁO]**
