# Kịch bản demo TechStore (10-12 phút)

## 0) Bạn cần hiểu nhanh trong 30 giây
Hệ thống này là **CSDL phân tán không đồng nhất**:
- SQL Server (HQ) và MySQL (Branch) là 2 nơi lưu dữ liệu khác nhau.
- Web thao tác ở HQ, sau đó đồng bộ sang Branch qua hàng đợi.
- Báo cáo tổng hợp lấy dữ liệu từ cả 2 hệ.

## 1) Mở đầu khi thuyết trình (20-30 giây)
Bạn nói:
"Đề tài của nhóm em giải quyết bài toán tích hợp SQL Server (trụ sở) và MySQL (chi nhánh) mà không cần migrate toàn bộ dữ liệu. Em demo 3 ý chính: kết nối đa hệ, truy vấn tổng hợp, và đồng bộ giá HQ -> Branch."

## 2) Chuẩn bị màn hình trước demo (30 giây)
- Cửa sổ 1: SSMS mở file [scripts/demo_script.sql](../scripts/demo_script.sql)
- Cửa sổ 2: MySQL Workbench (hoặc tab truy vấn MySQL)
- Cửa sổ 3: Web app TechStore

Bạn nói:
"Em bắt đầu từ SQL Server, sau đó đối chiếu bên MySQL để thấy khác hệ quản trị nhưng dữ liệu vẫn thống nhất."

## 3) Bước A - Kiểm tra liên thông (45 giây)
Trong SSMS chạy:
```sql
EXEC master.dbo.sp_testlinkedserver @servername = N'MYSQL';
SELECT * FROM OPENQUERY(MYSQL, 'SELECT 1 AS ok');
```
Bạn nói:
- "Lệnh chạy thành công chứng minh SQL Server đã liên thông được MySQL qua Linked Server/ODBC."
- "Đây là nền tảng để truy vấn xuyên hệ quản trị."

## 4) Bước B - Hiển thị cùng một sản phẩm ở 2 hệ (60 giây)
Chạy block 1 trong [scripts/demo_script.sql](../scripts/demo_script.sql):
- `HQPrice` tu `dbo.SanPham` (SQL Server)
- `BranchPrice` tu `OPENQUERY(MYSQL, ...)`

Bạn nói:
- "Đây là điểm so sánh 1: dữ liệu gốc bên SQL Server và dữ liệu bên MySQL hiện tại."
- "Lúc này 2 giá có thể bằng nhau hoặc lệch nhau tùy trạng thái đồng bộ trước đó."

## 5) Bước C - Cập nhật giá tại HQ, xem Queue (2 phút)
Chạy block 2 + 3:
- `UPDATE dbo.SanPham ...`
- `SELECT TOP(1) ... FROM dbo.PriceSyncQueue`

Bạn nói:
- "Điểm so sánh 2: SQL Server không update trực tiếp MySQL trong trigger."
- "Trigger chỉ ghi vào `PriceSyncQueue` với trạng thái `P` (Pending)."
- "Cách này tránh lỗi distributed transaction (Msg 7391) và dễ retry khi lỗi mạng/quyền."

## 6) Bước D - Xử lý queue và đối chiếu MySQL (2 phút)
Chạy block 4 + 5:
- `EXEC dbo.sp_ProcessPriceSyncQueue`
- Xem queue chuyển `P -> S`
- Query lại `BranchPrice`

Bạn nói:
- "Điểm so sánh 3: bên SQL Server có cơ chế queue + stored procedure điều phối."
- "Bên MySQL là nơi nhận update giá."
- "Sau khi process, giá tại Branch bằng giá mới tại HQ."

## 7) Bước E - Báo cáo tổng hợp (90 giây)
Chạy block 6:
```sql
EXEC dbo.sp_BaoCaoDoanhThu @FromDate = NULL, @ToDate = NULL;
```
Bạn nói:
- "Điểm so sánh 4: SQL Server đóng vai trò hợp nhất báo cáo; hóa đơn branch được đọc từ MySQL qua OPENQUERY."
- "Đã xử lý sai khác kiểu dữ liệu DateTime/Decimal bằng CAST rõ ràng."

## 8) Bước F - Demo nhanh trên Web (2-3 phút)
- Vào trang Update Price: sửa giá 1 sản phẩm.
- Bấm xử lý sync.
- Vào Revenue xem báo cáo.

Bạn nói:
- "Web chỉ là lớp giao diện; nghiệp vụ chính nằm ở DB objects: trigger, queue, stored procedures."

## 9) Câu kết (20 giây)
Bạn nói:
"Hệ thống đạt 5 yêu cầu đề bài: kết nối đa hệ quản trị, truy vấn trong suốt, đồng bộ giá, xử lý sai khác kiểu dữ liệu, và có bộ test case xác thực."

## 10) Nếu thầy hỏi: “So sánh SQL Server và MySQL gì?”
Trả lời ngắn:
- SQL Server: trung tam dieu phoi (HQ), trigger/queue/SP, tong hop bao cao.
- MySQL: branch operational data (HoaDon, SanPham chi nhanh), duoc cap nhat thong qua co che sync.
- Diem khac biet da xu ly: DateTime va Decimal, quyen truy cap, cu phap truy van khac he.

## 11) Tình huống dự phòng (30 giây)
Nếu lỗi live:
1. Chạy lại `sp_testlinkedserver 'MYSQL'`.
2. Kiểm tra MySQL service đang chạy.
3. Chạy lại `EXEC dbo.sp_ProcessPriceSyncQueue;`.
4. Mở cột `LastError` trong `PriceSyncQueue` để giải thích lỗi minh bạch.
