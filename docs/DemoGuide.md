# 🚀 KỊCH BẢN DEMO THỰC CHIẾN ĐỒ ÁN TECHSTORE
*(Thời lượng dự kiến: 10-15 phút)*

Mục tiêu của kịch bản này không chỉ là show tính năng, mà là thể hiện tư duy của một **Solution Architect (Kiến trúc sư giải pháp)**, hiểu rõ bài toán doanh nghiệp và các giới hạn vật lý của hệ thống mạng.

---

## 🎩 Phase 1: Đặt vấn đề (1 phút)
*(Không mở code ngay, mở Slide PPT lên trình bày)*

**Lời thoại:** 
> "Chào thầy/cô. Đồ án của nhóm em giải quyết bài toán sáp nhập doanh nghiệp (M&A). Tổng công ty dùng SQL Server (nằm ở Data Center Quận 1), trong khi chi nhánh mới mua lại dùng MySQL (nằm ở cửa hàng Thủ Đức). 
> 
> Thay vì đập đi xây lại tốn kém chi phí và gây gián đoạn kinh doanh, nhóm em xây dựng hệ thống **Cơ sở dữ liệu phân tán lai (Hybrid Distributed DB)**. Hệ thống kết nối 2 hệ quản trị khác biệt qua đường truyền Internet mà không cần phải bê toàn bộ dữ liệu về chung một chỗ."

---

## 🔮 Phase 2: Biểu diễn "Truy vấn trong suốt" và "Xử lý sai khác" (3 phút)
*(Mục tiêu: Đánh trúng Yêu cầu 1, 2, 4 và Tiêu chí 1 của Rubric)*

1. **Lời thoại:** "Tiêu chí cao nhất của CSDL phân tán là tính **Trong suốt (Transparency)**. Nghĩa là lập trình viên hoặc người dùng cuối không cần biết dữ liệu đang nằm ở đâu, dùng hệ quản trị gì."
2. **Thao tác:**
   - Mở màn hình Web: Tab **Revenue (Doanh thu)** hoặc Tab **Evidence**.
   - Mở SQL Server Management Studio (SSMS), mở source code của View `vw_BranchInvoices_Logical` cho thầy xem.
3. **Giải thích "Ăn tiền":** 
   > "Để làm được điều này, nhóm em dùng Linked Server kết nối qua ODBC xuống MySQL. Nhưng điểm mấu chốt là xử lý sai khác kiểu dữ liệu: SQL Server dùng `DATETIME2`, còn MySQL dùng `TIMESTAMP`. 
   > 
   > Nhóm em tạo ra một **View Luận lý** bọc hàm `OPENQUERY` lại, dùng hàm `CAST` để ép chuẩn kiểu dữ liệu. Sau đó, em viết lệnh `JOIN` giữa bảng `SanPham` (trên SQL Server) và cái View này. Câu lệnh JOIN được viết hoàn toàn bằng T-SQL thuần túy, cực kỳ trong suốt!"

---

## ⚡ Phase 3: Biểu diễn "Đồng bộ" và "Chống đứt cáp mạng" (5 phút)
*(Mục tiêu: Đánh trúng Yêu cầu 3 và Tiêu chí 2 - Khía cạnh địa lý mạng)*

1. **Lời thoại:** 
   > "Vì trụ sở và chi nhánh cách xa nhau về mặt vật lý, đường truyền WAN/Internet rất dễ bị đứt hoặc lag. Nhóm em không cập nhật thẳng xuống MySQL vì nếu rớt mạng, giao dịch sẽ bị treo, làm khóa luôn database trụ sở. Nhóm áp dụng kiến trúc **Bất đồng bộ qua Hàng đợi (Message Queue)**."
2. **Thao tác (Demo bình thường):** 
   - Mở song song 2 màn hình: Tab Web "Products" và Tab Web "Price Sync Queue".
   - Sửa giá 1 sản phẩm ở tab Products. 
   - Nhanh chóng qua tab Queue xem dòng trạng thái nhảy từ `P` (Pending) sang `S` (Success).

3. **🔥 Thao tác (Demo đứt cáp - Killer Feature):**
   - Mở Services của Windows, tìm service MySQL và **Stop** nó lại (Giả lập đứt cáp chi nhánh).
   - Lên Web sửa giá một sản phẩm khác. App vẫn chạy mượt mà không bị crash.
   - Bấm sang tab Queue: Sẽ thấy trạng thái là `E` (Error) cùng dòng lỗi *"Unable to connect to MySQL"*. 
   - **Giải thích:** *"Hệ thống SQL Server ở trụ sở vẫn hoạt động bình thường, không bị sập theo chi nhánh. Data thay đổi được nhét an toàn vào Queue chờ."*
   - Bật **Start** lại MySQL Service. Chờ tối đa 30 giây.
   - **Giải thích tiếp:** *"Hệ thống Web của em có một con `PriceSyncBackgroundService` (Worker) chạy ngầm 30 giây một lần. Nó sẽ tự quét các gói tin bị lỗi mạng `E` và đồng bộ lại."*
   - F5 lại trang Queue, thấy chữ `E` tự động chuyển thành chữ `S` (Hệ thống đã tự phục hồi).

---

## 🏁 Phase 4: Tổng kết Test Cases (1 phút)
*(Mục tiêu: Đánh trúng Yêu cầu 5)*

1. **Thao tác:** Mở file báo cáo Word đến phần Ma trận Test Cases.
2. **Lời thoại:** 
   > "Nhóm em đã thực hiện 10 test cases, tập trung hoàn toàn vào các đặc thù của CSDL Phân tán. Các test case trải dài từ việc Test rớt mạng nhánh, Test truy vấn xuyên Server, Test đồng bộ dị bộ, đến Test hàm CAST xử lý xung đột kiểu dữ liệu. Tất cả đều PASS và có minh chứng hình ảnh đầy đủ trong báo cáo ạ."

---
---

## 🛡️ PHÒNG THỦ: CÁC CÂU HỎI "XOÁY" CỦA GIẢNG VIÊN

Học thuộc 3 câu trả lời này để chứng minh bạn thực sự là người thiết kế hệ thống.

### ❓ Câu 1: "Tại sao em dùng Dynamic SQL `EXEC ('...') AT MYSQL` để update giá mà không dùng `UPDATE OPENQUERY...`?"
**💡 Trả lời (Tư duy Kỹ sư hệ thống):**
> "Dạ thưa thầy, ban đầu em định dùng `UPDATE OPENQUERY` nhưng em phát hiện ra một rủi ro nghiêm trọng. Khi thao tác ghi (Write/Update) xuyên 2 hệ quản trị khác nhau, SQL Server bắt buộc phải kích hoạt dịch vụ **MSDTC (Distributed Transaction Coordinator)**. 
> Nhưng MSDTC cấu hình giữa Windows và MySQL qua ODBC cực kỳ thiếu ổn định và hay báo lỗi. Vì vậy, em dùng Dynamic SQL đẩy nguyên chuỗi truy vấn xuống cho MySQL tự thực thi. Cách này bỏ qua được gánh nặng của MSDTC mà vẫn đảm bảo dữ liệu được cập nhật chính xác."

### ❓ Câu 2: "Tại sao đồng bộ giá lại phải dùng Trigger nhét vào bảng Queue rồi mới chạy Background Service? Sao không dùng Trigger update thẳng xuống MySQL luôn cho lẹ?"
**💡 Trả lời (Tư duy Kiến trúc mạng):**
> "Dạ vì khía cạnh vật lý và địa lý ạ (Tiêu chí 2). Trụ sở và chi nhánh nối nhau qua Internet. Nếu Trigger update thẳng xuống MySQL, lỡ lúc đó đứt mạng, cái Trigger đó bị timeout và nó sẽ block (khóa) luôn lệnh Update đang chạy ở SQL Server. Nghĩa là mạng chi nhánh đứt làm sập luôn database trụ sở. 
> Việc nhét vào Queue giúp tách rời (decouple) 2 hệ thống. Trụ sở cứ ghi nhận sự thay đổi vào Queue (tốn vỏn vẹn 0.001s), còn việc đẩy qua mạng chậm hay đứt cáp thì để con Background Service của ứng dụng Web từ từ xử lý."

### ❓ Câu 3: "Cái View Luận lý (`vw_BranchInvoices_Logical`) của em giải quyết được vấn đề gì?"
**💡 Trả lời (Tư duy Lập trình ứng dụng):**
> "Dạ nó dùng để bọc sự phức tạp lại. Quan trọng nhất là em dùng nó để ép kiểu `TIMESTAMP` của MySQL về `DATETIME2` tiêu chuẩn của SQL Server. Nhờ cái View này, các bạn làm code C# Entity Framework hoặc người vẽ báo cáo PowerBI ở tầng trên chỉ cần `SELECT * FROM vw_BranchInvoices_Logical` như một bảng nội bộ bình thường. Họ không hề biết bên dưới là hàm `OPENQUERY` đang lội qua mạng kết nối vào MySQL."
