# 📊 KỊCH BẢN THUYẾT TRÌNH PPT — TỪNG SLIDE
*(Dùng khi thầy kêu trình bày slide trước, demo sau)*

---

## 📌 SLIDE 1 — Trang bìa
**Nội dung slide:** Tên đề tài, tên GVHD, tên SV.

**Lời thoại:**
> "Dạ chào thầy/cô. Nhóm em xin trình bày đồ án môn Cơ sở dữ liệu Phân tán với đề tài **TechStore: Quản lý Chuỗi Bán lẻ Đa nền tảng**. Đồ án giải quyết bài toán phân mảnh dữ liệu hậu M&A thông qua kiến trúc Linked Server và cơ chế tự phục hồi (Self-Healing). Nhóm gồm Nguyễn Hoàng Tiến và Lê Duy Quân, lớp DCT123C2, dưới sự hướng dẫn của thầy Vũ Ngọc Thanh Sang ạ."

⏱️ *~20 giây, nói gọn, không lan man.*

---

## 📌 SLIDE 2 — Nỗi Đau Hậu M&A: Các Ốc Đảo Dữ Liệu
**Nội dung slide:** Sơ đồ 2 tòa nhà Server (SQL Server vs MySQL), vết nứt ở giữa (đứt gãy luồng thông tin), 3 pain points.

**Lời thoại:**
> "Slide này mô tả bài toán thực tế. Khi một công ty mua lại chuỗi bán lẻ, bên mua dùng SQL Server, bên bị mua dùng MySQL. Hệ quả là hình thành các **Ốc đảo dữ liệu (Data Silos)** — dữ liệu bị phân mảnh, không ai nhìn được toàn cảnh.
>
> Cụ thể có 3 vấn đề lớn:
> 1. **Rào cản Báo cáo** — Không thể tổng hợp doanh thu thời gian thực, phải xuất Excel thủ công.
> 2. **Độ trễ Giá bán** — Cập nhật giá từ Trụ sở xuống Chi nhánh mất thời gian, dễ bán sai giá.
> 3. **Chi phí Chuyển đổi** — Tái cấu trúc toàn bộ hạ tầng chi nhánh sang SQL Server là không khả thi về mặt chi phí."

⏱️ *~40 giây. Chỉ tay vào từng pain point khi nói.*

---

## 📌 SLIDE 3 — Chẩn Đoán Nền Tảng: Lý Do Tồn Tại Kiến Trúc Lai
**Nội dung slide:** Bảng so sánh SQL Server vs MySQL (giấy phép, hiệu năng, hệ sinh thái, vị trí triển khai).

**Lời thoại:**
> "Thay vì đập bỏ MySQL để gom hết về SQL Server, nhóm em đi theo hướng **tận dụng ưu điểm tuyệt đối** của mỗi hệ quản trị.
>
> SQL Server là hệ thương mại, mạnh về phân tích dữ liệu lớn, tích hợp sẵn với hệ sinh thái Microsoft và Power BI — rất phù hợp đặt tại **Trụ sở** để quản trị tập trung.
>
> MySQL là mã nguồn mở, miễn phí, truy vấn nhanh và tối ưu cho các giao dịch đọc nhiều hơn ghi — rất phù hợp đặt tại **Chi nhánh bán lẻ** nơi chỉ cần xuất hóa đơn.
>
> Giải pháp của nhóm: Giữ nguyên cả 2, xây cầu nối ở giữa."

⏱️ *~40 giây. Dùng tay chỉ từ trái qua phải khi so sánh.*

---

## 📌 SLIDE 4 — Mô Hình Phân Mảnh Lai (Hybrid Fragmentation)
**Nội dung slide:** Sơ đồ 2 Node (Master Node Quận 1 + Transaction Node Thủ Đức), mũi tên đồng bộ giá và truy vấn doanh thu.

**Lời thoại:**
> "Đây là slide quan trọng nhất về mặt lý thuyết CSDL Phân tán. Hệ thống được phân mảnh thành 2 Node vật lý, đặt tại 2 vị trí địa lý khác nhau:
>
> **Master Node (Quận 1)** — SQL Server chứa bảng `SanPham` gốc (Master) và bảng `PriceSyncQueue` (hàng đợi đồng bộ giá).
>
> **Transaction Node (Thủ Đức)** — MySQL chứa bảng `HoaDon` (hóa đơn bán hàng - Master) và bảng `SanPham` (bản sao Replica để hiển thị giá cho khách).
>
> Có 2 luồng dữ liệu chính:
> - Mũi tên đi xuống: **Đồng bộ giá bán** từ Trụ sở xuống Chi nhánh (bất đồng bộ qua Queue).
> - Mũi tên đi lên: **Truy vấn doanh thu** từ Chi nhánh lên Trụ sở (phân tán, trong suốt qua View Luận lý).
>
> 3 nguyên tắc thiết kế:
> 1. Tập trung hóa Danh mục — Trụ sở kiểm soát giá gốc.
> 2. Phân tán Giao dịch — Chi nhánh tự trị quản lý hóa đơn.
> 3. Đồng bộ Bất đồng bộ — Vượt rào cản mạng WAN."

⏱️ *~1 phút. Slide cốt lõi, nói chậm, rõ ràng.*

---

## 📌 SLIDE 5 — Xây Dựng Cầu Nối: Kiến Trúc Linked Server
**Nội dung slide:** Sơ đồ 5 tầng xếp chồng (SQL Server Engine → OLE DB → System DSN → MySQL Connector/ODBC → MySQL Engine).

**Lời thoại:**
> "Để 2 hệ quản trị giao tiếp được, nhóm em xây dựng một đường hầm (tunnel) 5 tầng:
>
> Tầng trên cùng là SQL Server Engine, nơi em viết lệnh T-SQL. Nó gọi xuống **Microsoft OLE DB Provider for ODBC**, Provider này dùng **System DSN** tên là `MySQL_TechStore` trỏ đến port 3306 của MySQL. Bên dưới DSN là driver **MySQL Connector/ODBC 64-bit** thực hiện chuyển đổi giao thức, và cuối cùng là **MySQL Engine** chứa database `TechStore_Branch`.
>
> Kết quả: Khi em gõ lệnh `OPENQUERY(MYSQL, 'SELECT ...')` trên SQL Server, dữ liệu từ MySQL được kéo lên trong suốt như thể nó nằm ngay trên SQL Server."

⏱️ *~40 giây. Chỉ tay từ tầng trên xuống tầng dưới.*

---

## 📌 SLIDE 6 — Che Giấu Sự Bất Đồng Nhất: View Luận Lý
**Nội dung slide:** Hình "Translator Box" chuyển đổi TIMESTAMP → DATETIME2 qua hàm CAST, kèm đoạn code SQL tạo View.

**Lời thoại:**
> "Khi dữ liệu MySQL được kéo lên SQL Server, kiểu dữ liệu bị xung đột. MySQL dùng `TIMESTAMP`, SQL Server dùng `DATETIME2`. Nếu không xử lý, ứng dụng C# sẽ báo lỗi runtime.
>
> Nhóm em tạo một View luận lý tên `vw_BranchInvoices_Logical`, bên trong bọc hàm `OPENQUERY` và dùng `CAST` để ép kiểu an toàn tuyệt đối. Slide này cho thấy cái View đóng vai trò như một **Translator Box** — hộp phiên dịch — giữa 2 hệ quản trị.
>
> Kết quả: Ứng dụng C# ở tầng trên truy vấn hoàn toàn trong suốt, không nhận thức được sự tồn tại của MySQL bên dưới. Đây chính là **Tính trong suốt vị trí (Location Transparency)** trong lý thuyết CSDL Phân tán."

⏱️ *~40 giây.*

---

## 📌 SLIDE 7 — Thực Thi Truy Vấn Phân Tán (Distributed Querying)
**Nội dung slide:** Sơ đồ JOIN giữa bảng SanPham (SQL Server) và vw_BranchInvoices_Logical (MySQL), ra bảng Báo Cáo Doanh Thu Tổng Hợp.

**Lời thoại:**
> "Đây là minh chứng trực tiếp cho **Yêu cầu 2** của đề bài: Viết lệnh JOIN giữa bảng SanPham tại Trụ sở và bảng HoaDon tại Chi nhánh.
>
> Như thầy/cô thấy, câu lệnh SQL hoàn toàn bằng cú pháp T-SQL tiêu chuẩn: `SELECT ... FROM SanPham INNER JOIN vw_BranchInvoices_Logical`. Không có dòng nào đề cập đến MySQL, không có IP address, không có connection string. Người viết câu lệnh này không hề biết dữ liệu HoaDon thực chất đang nằm ở máy chủ MySQL tại Thủ Đức.
>
> Kết quả ra bảng Báo Cáo Doanh Thu Tổng Hợp — gộp ProductID, ProductName từ SQL Server và Total từ MySQL."

⏱️ *~40 giây. Đây là slide "ăn tiền" nhất cho Tiêu chí 1.*

---

## 📌 SLIDE 8 — Xử Lý Giao Dịch Xuyên Hệ Thống
**Nội dung slide:** So sánh 2 cách: Truyền thống (MSDTC — dấu X đỏ, nguy cơ treo) vs TechStore (Dynamic SQL — dấu tick xanh).

**Lời thoại:**
> "Khi cần **ghi dữ liệu** xuống MySQL (ví dụ update tên sản phẩm), nếu dùng cách truyền thống qua MSDTC (Distributed Transaction Coordinator), SQL Server sẽ cố mở một giao dịch phân tán 2 pha (Two-Phase Commit). Nhưng MSDTC giữa Windows và MySQL cực kỳ thiếu ổn định, rất hay treo hệ thống.
>
> Nhóm em chọn giải pháp **Dynamic SQL**: Đóng gói nguyên câu lệnh UPDATE thành một chuỗi ký tự, rồi dùng lệnh `EXEC('UPDATE ...') AT MYSQL` để đẩy thẳng sang MySQL thực thi. MySQL tự xử lý độc lập, không cần MSDTC, không có nguy cơ treo."

⏱️ *~40 giây.*

---

## 📌 SLIDE 9 — Kiến Trúc Ứng Dụng Quản Trị (Web Dashboard)
**Nội dung slide:** Sơ đồ 4 module Web (Evidence, Reporting, Master Data, Sync Monitoring) bọc quanh lõi Repository Pattern + Distributed Database.

**Lời thoại:**
> "Nhóm em không chỉ dừng ở cấu hình database mà còn xây dựng một **Web Dashboard** bằng C# ASP.NET Core (.NET 9) để trực quan hóa toàn bộ hệ thống phân tán:
>
> 1. **Distributed Evidence** — Chứng minh kết nối Linked Server hoạt động, hiển thị Data Contracts (ánh xạ kiểu dữ liệu).
> 2. **Logical View Reporting** — Xem báo cáo doanh thu xuyên hệ thống, người dùng không biết dữ liệu đến từ MySQL.
> 3. **Master Data Management** — Trụ sở kiểm soát sản phẩm và giá bán, mọi thay đổi tự động đồng bộ xuống.
> 4. **Sync Monitoring** — Giám sát luồng hàng đợi theo thời gian thực (P/S/E).
>
> Toàn bộ logic truy cập dữ liệu được gói trong một class `TechStoreRepository` theo **Repository Pattern**, tách hoàn toàn khỏi UI."

⏱️ *~50 giây.*

---

## 📌 SLIDE 10 — Rủi Ro Đứt Mạng & Vùng Đệm Asynchronous Queue
**Nội dung slide:** Hình sấm sét (Network Partition) giữa Trụ Sở và Chi Nhánh, mũi tên đổ vào "Vùng Đệm PriceSyncQueue" với 3 trạng thái P/S/E.

**Lời thoại:**
> "Slide này giải quyết **khía cạnh địa lý vật lý** — yêu cầu quan trọng nhất của Tiêu chí 2.
>
> Trụ sở đặt tại Quận 1 và Chi nhánh đặt tại Thủ Đức, nối qua mạng WAN/Internet. Đường truyền này có thể bị đứt bất cứ lúc nào. Nếu hệ thống cập nhật giá thẳng xuống MySQL mà mạng đứt, toàn bộ SQL Server ở trụ sở sẽ bị treo.
>
> Giải pháp: Khi giá thay đổi, một **Database Trigger** tự động bắt sự kiện và đẩy vào bảng `PriceSyncQueue` (Vùng Đệm). Bảng này có 3 trạng thái:
> - **[P] Pending** — Đang chờ đồng bộ.
> - **[S] Success** — Đã đẩy sang MySQL thành công.
> - **[E] Error** — Thất bại do rớt mạng, lưu vết lỗi để retry sau."

⏱️ *~50 giây. Slide "đinh" cho khía cạnh địa lý.*

---

## 📌 SLIDE 11 — Bác Sĩ Hệ Thống: Cơ Chế Tự Phục Hồi (Self-Healing)
**Nội dung slide:** Hình vòng lặp vô cực (∞) với các bước: Thức dậy mỗi 30s → Quét Queue → Phát hiện P/E → Đẩy thử lại → Thành công → Cập nhật [S].

**Lời thoại:**
> "Câu hỏi đặt ra: Khi mạng khôi phục, ai sẽ xử lý các gói tin bị nghẽn?
>
> Nhóm em xây dựng một **Background Hosted Service** trong ASP.NET Core — có thể ví như một 'bác sĩ trực' chạy ngầm 24/7.  Cứ mỗi 30 giây, nó tự thức dậy, quét bảng `PriceSyncQueue`, tìm các dòng có trạng thái `P` (Pending) hoặc `E` (Error do mất mạng trước đó), rồi đẩy thử lại xuống MySQL.
>
> Nếu thành công → đổi trạng thái sang `S`. Nếu vẫn lỗi → giữ `E` và ghi lại lỗi, vòng lặp tiếp theo sẽ thử lại.
>
> Kết quả: Hệ thống web **không bao giờ crash** vì lỗi mạng. Dữ liệu không bao giờ bị mất. Chỉ cần mạng khôi phục, mọi thứ tự động đồng bộ. Đây là cơ chế **Fault Tolerance** (Chịu lỗi) trong CSDL Phân tán."

⏱️ *~1 phút. Nói chậm và tự tin, đây là điểm sáng nhất.*

---

## 📌 SLIDE 12 — Chứng Thực Hệ Thống (Validation & Test Cases)
**Nội dung slide:** Dashboard 4 nhóm test case (TC02-03: Truy vấn xuyên hệ thống, TC05-06: Hàng đợi, TC07: Chaos Test, TC10: Ràng buộc toàn vẹn). Tất cả PASS.

**Lời thoại:**
> "Nhóm em thiết kế 10 test cases, tập trung hoàn toàn vào các đặc thù của CSDL Phân tán, không có test case validation input thông thường.
>
> - **TC02 & TC03**: Truy vấn xuyên hệ thống — JOIN trả về dữ liệu chính xác, ép kiểu an toàn.
> - **TC05 & TC06**: Bắt sự kiện hàng đợi — Trigger ghi nhận chính xác trạng thái P rồi chuyển S sau khi đồng bộ.
> - **TC07**: Đây là test quan trọng nhất — **Thử nghiệm phá hoại (Chaos Test)**. Em cố tình tắt MySQL để giả lập đứt mạng. Hệ thống không crash, lưu lỗi vào Queue, và tự phục hồi khi mạng khôi phục.
> - **TC10**: Ràng buộc toàn vẹn — Check Constraint chặn giá trị âm trên cả 2 nền tảng.
>
> Tất cả 10/10 PASS."

⏱️ *~50 giây.*

---

## 📌 SLIDE 13 — Tổng Kết & Tầm Nhìn Mở Rộng
**Nội dung slide:** Roadmap 3 cấp (Level 1: Hiện tại → Level 2: Kafka/RabbitMQ → Level 3: AWS RDS/Azure + Đồng bộ 2 chiều).

**Lời thoại:**
> "Tổng kết: Nhóm em đã thành công xóa bỏ ranh giới giữa 2 hệ quản trị, giải quyết Data Silo hậu M&A với kiến trúc Tự phục hồi. Hệ thống hiện tại đảm bảo **Zero Data Loss** — không mất dữ liệu trong mọi tình huống mạng.
>
> Về tầm nhìn mở rộng:
> - **Level 2 (Tương lai gần)**: Thay thế bảng Queue bằng hệ thống Message Broker chuyên nghiệp như **Kafka hoặc RabbitMQ**, chịu tải hàng triệu sự kiện mỗi ngày.
> - **Level 3 (Mục tiêu doanh nghiệp)**: Dịch chuyển lên Cloud (AWS RDS / Azure SQL), hỗ trợ đồng bộ 2 chiều và kiểm chứng độ trễ toàn cầu.
>
> Em xin cảm ơn thầy/cô đã lắng nghe ạ."

⏱️ *~40 giây. Kết thúc đanh thép.*

---

**TỔNG THỜI GIAN DỰ KIẾN: ~8-10 phút thuyết trình PPT.**
