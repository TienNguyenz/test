# Sườn Word báo cáo TechStore (dễ hiểu, dùng ngay)

## 0. Viết đoạn này trước để thầy hiểu bạn nắm bài
Bạn có thể đặt ngay sau trang bìa:

"Đây là mô hình CSDL phân tán không đồng nhất (heterogeneous distributed database), vì dữ liệu nằm trên 2 hệ quản trị khác nhau: SQL Server và MySQL. Hệ thống không di trú toàn bộ dữ liệu về một nơi, mà liên thông truy vấn và đồng bộ nghiệp vụ thông qua Linked Server, Queue và Stored Procedure."

## 1. Trang bìa
- Trường, khoa, môn học
- Tên đề tài: LIÊN THÔNG MYSQL VÀ SQL SERVER - TECHSTORE
- GVHD, sinh viên, MSSV, lớp, ngày nộp

## 2. Giới thiệu đề tài
- Bài toán sau sáp nhập hệ thống: HQ dùng SQL Server, chi nhánh dùng MySQL.
- Mục tiêu: tích hợp, không migrate full, vẫn vận hành liên tục.

Đoạn mẫu:
"Đề tài TechStore giải quyết bài toán tích hợp dữ liệu khác hệ quản trị trong bối cảnh doanh nghiệp sau sáp nhập. Giải pháp hướng tới việc tận dụng hệ thống hiện hữu, hạn chế gián đoạn và giảm chi phí chuyển đổi."

## 3. Yêu cầu đề bài và phạm vi thực hiện
- Kết nối SQL Server và MySQL.
- JOIN/báo cáo tổng hợp dữ liệu khác hệ.
- Đồng bộ giá từ HQ sang Branch.
- Xử lý sai khác kiểu dữ liệu.
- Xây dựng bộ test case.

## 4. Kiến trúc hệ thống
### 4.1 Thành phần
- SQL Server (TechStore_HQ): `SanPham`, `PriceSyncQueue`, trigger, stored procedures.
- MySQL (TechStore_Branch): `SanPham`, `HoaDon`.
- Linked Server `MYSQL` qua ODBC.
- Web Razor Pages để demo thao tác.

### 4.2 Luồng xử lý (nên có hình mũi tên)
1. Cập nhật giá tại HQ.
2. Trigger tạo bản ghi queue (`P` - Pending).
3. Procedure xử lý queue và đẩy giá qua MySQL.
4. Báo cáo doanh thu tổng hợp dữ liệu HQ + Branch.

## 5. Cài đặt và cấu hình
### 5.1 SQL Server
- Tạo DB và các đối tượng (`sp_BaoCaoDoanhThu`, `sp_ProcessPriceSyncQueue`, `trg_SanPham_QueuePriceSync`).

### 5.2 MySQL
- Tạo DB `TechStore_Branch`, bảng `SanPham`, `HoaDon`.
- Cấp quyền user linked server (`SELECT`, `UPDATE`).

### 5.3 Linked Server
- Kiểm tra:
```sql
EXEC sp_testlinkedserver 'MYSQL';
SELECT * FROM OPENQUERY(MYSQL, 'SELECT 1 AS ok');
```

## 6. Phần so sánh SQL Server và MySQL (quan trọng khi vấn đáp)
| Nội dung | SQL Server (HQ) | MySQL (Branch) |
|---|---|---|
| Vai trò | Điều phối trung tâm, báo cáo tổng hợp | Dữ liệu giao dịch chi nhánh |
| Dữ liệu chính | `SanPham`, `PriceSyncQueue` | `HoaDon`, `SanPham` chi nhánh |
| Cơ chế đồng bộ | Trigger + Queue + Stored Procedure | Nhận lệnh update từ SQL Server |
| Truy vấn liên thông | `OPENQUERY`, SP tổng hợp | Cung cấp nguồn dữ liệu từ xa |
| Vấn đề xử lý | Distributed txn, ép kiểu DateTime/Decimal | Quyền user, tương thích kiểu dữ liệu |

Nhận xét mẫu:
"SQL Server đóng vai trò coordinator, MySQL đóng vai trò data source tại chi nhánh. Hai hệ được liên thông theo hướng tích hợp, không phải thay thế."

## 7. Xử lý sai khác dữ liệu giữa 2 DBMS
- DateTime: CAST sang `datetime2` để thống nhất.
- Số tiền: dùng `decimal(18,2)`.
- Kiểm soát lỗi quyền qua `LastError` trong queue.

## 8. Kết quả test case
Dẫn chiếu: [scripts/test_cases.sql](../scripts/test_cases.sql)
- TC01..TC08: yêu cầu tối thiểu đề bài.
- TC09..TC10: mở rộng chất lượng.

Bảng tổng kết nên có:
| Test case | Kết quả | Minh chứng |
|---|---|---|
| TC01 | PASS | Anh 01 |
| ... | ... | ... |

## 9. Demo và đánh giá
- Tóm tắt demo: HQ update -> Queue -> Process -> Branch update -> Report.
- Ưu điểm: rõ ràng, dễ mở rộng, hạn chế phụ thuộc giao dịch phân tán trực tiếp.
- Hạn chế: độ trễ đồng bộ phụ thuộc lịch chạy queue/job.

## 10. Kết luận
Đoạn mẫu:
"Nhóm đã xây dựng thành công mô hình tích hợp SQL Server và MySQL theo hướng thực tế doanh nghiệp. Giải pháp đáp ứng yêu cầu đề bài về kết nối đa hệ quản trị, truy vấn tổng hợp, đồng bộ dữ liệu và kiểm thử hệ thống."

## 11. Phụ lục
- Lệnh chạy web: `dotnet run --project TechStoreWeb`
- Script demo: [scripts/demo_script.sql](../scripts/demo_script.sql)
- Demo guide: [docs/DemoGuide.md](DemoGuide.md)
