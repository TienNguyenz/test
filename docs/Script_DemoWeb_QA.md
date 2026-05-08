# 🌐 KỊCH BẢN DEMO WEB TRỰC TIẾP + NGÂN HÀNG CÂU HỎI PHÒNG THỦ
*(Dùng khi thầy kêu chạy app demo trực tiếp, không cần PPT)*

---

## ⚡ PHẦN 1: DEMO WEB TRỰC TIẾP (7-10 phút)

### Bước 0: Chuẩn bị trước khi demo
- Mở sẵn **SSMS** (SQL Server Management Studio) — kết nối vào `TechStore_HQ`.
- Mở sẵn **Web app** trên trình duyệt (`https://localhost:5001`).
- Mở sẵn **Windows Services** (gõ `services.msc`) — tìm sẵn dòng MySQL để demo đứt mạng.
- Mở sẵn **Visual Studio Code** ở file `TechStoreRepository.cs` — phòng thầy kêu xem code.

---

### 🔹 Demo 1: Chứng minh kết nối (1 phút)
*(Đánh trúng Yêu cầu 1: Kết nối đa hệ quản trị)*

**Thao tác:** Vào tab **Evidence** trên Web.

**Nói:**
> "Đây là trang Distributed Evidence. Như thầy/cô thấy, hệ thống đang hiển thị trạng thái **Healthy** cho Linked Server. Bên cạnh đó nó đếm trực tiếp số sản phẩm tại Trụ sở (SQL Server) và số hóa đơn tại Chi nhánh (MySQL). Hai con số này được lấy từ 2 server khác nhau nhưng hiển thị chung trên 1 trang."

**Thao tác bổ sung (nếu thầy muốn xem SSMS):**
Chạy lệnh trên SSMS:
```sql
EXEC master.dbo.sp_testlinkedserver @servername = N'MYSQL';
```
> "Message 'Command(s) completed successfully' chứng minh Linked Server đang hoạt động bình thường ạ."

---

### 🔹 Demo 2: Truy vấn trong suốt — JOIN phân tán (2 phút)
*(Đánh trúng Yêu cầu 2 + Tiêu chí 1)*

**Thao tác:** Vào tab **Revenue Report** trên Web. Chọn khoảng ngày rồi bấm lọc.

**Nói:**
> "Trang Revenue Report này hiển thị doanh thu tổng hợp. Điểm mấu chốt: Tên sản phẩm và giá gốc lấy từ **SQL Server**, còn số lượng bán và ngày bán lấy từ **MySQL**. Nhưng người dùng không hề biết — họ chỉ thấy một bảng duy nhất."

**Thao tác bổ sung:** Qua SSMS chạy lệnh JOIN:
```sql
SELECT sp.ProductID, sp.ProductName, sp.Category,
       hd.Quantity, sp.Price, (hd.Quantity * sp.Price) AS DoanhThu
FROM TechStore_HQ.dbo.SanPham sp
INNER JOIN TechStore_HQ.dbo.vw_BranchInvoices_Logical hd
    ON sp.ProductID = hd.ProductID
ORDER BY hd.SaleDate DESC;
```
> "Câu lệnh này viết hoàn toàn bằng T-SQL, không dòng nào nhắc đến MySQL. SQL Server tự động chui qua Linked Server lấy data lên. Đây là **Location Transparency** — tính trong suốt vị trí."

---

### 🔹 Demo 3: Xử lý sai khác kiểu dữ liệu (1 phút)
*(Đánh trúng Yêu cầu 4)*

**Thao tác:** Vào tab **Data Contracts** trên Web.

**Nói:**
> "Trang Data Contracts hiển thị bảng ánh xạ kiểu dữ liệu giữa SQL Server và MySQL. Ví dụ `DATETIME2` bên SQL Server tương ứng `TIMESTAMP` bên MySQL. Việc xử lý xung đột này được thực hiện bên trong View Luận lý `vw_BranchInvoices_Logical` bằng hàm `CAST`."

---

### 🔹 Demo 4: Đồng bộ giá bình thường (2 phút)
*(Đánh trúng Yêu cầu 3)*

**Thao tác:**
1. Vào tab **Products** → Click **Edit** một sản phẩm bất kỳ.
2. Đổi giá (ví dụ từ 25,000,000 sang 26,000,000) → Bấm **Save**.
3. Ngay lập tức chuyển sang tab **Price Sync** (hoặc F5 lại trang).

**Nói:**
> "Em vừa thay đổi giá tại Trụ sở. Ngay lập tức, một Trigger trong SQL Server bắt sự kiện thay đổi giá và nhét vào bảng `PriceSyncQueue`. Stored Procedure `sp_ProcessPriceSyncQueue` xử lý và đẩy giá mới xuống MySQL."
>
> *(Chỉ vào dòng mới nhất trong Queue)*
>
> "Trạng thái `S` (Success) — giá đã được đồng bộ thành công sang Chi nhánh MySQL."

---

### 🔹 Demo 5: 🔥 Giả lập đứt mạng — Self-Healing (3 phút)
*(Đánh trúng Tiêu chí 2: Khía cạnh địa lý + Fault Tolerance — ĐÂY LÀ MÀN TRÌNH DIỄN CHÍNH)*

**Thao tác:**
1. Mở **Windows Services** → Tìm dòng **MySQL** → Click **Stop** (Giả lập đứt mạng chi nhánh).
2. Quay lại Web → Vào tab **Products** → Edit một sản phẩm, đổi giá → Save.
3. Chuyển sang tab **Price Sync**.

**Nói:**
> "Em vừa tắt MySQL để giả lập tình huống đứt cáp mạng giữa Trụ sở (Quận 1) và Chi nhánh (Thủ Đức). Lưu ý: Web app vẫn chạy bình thường, SQL Server không bị sập. Giá mới vẫn được lưu thành công tại Trụ sở."
>
> *(Chỉ vào Queue)*
>
> "Trạng thái `E` (Error) — hệ thống ghi nhận lỗi mạng và lưu vết lỗi. Dữ liệu KHÔNG bị mất, nó nằm an toàn trong Queue chờ."

**Thao tác tiếp:**
4. Quay lại **Windows Services** → **Start** lại MySQL.
5. Chờ khoảng 30 giây.
6. **F5** lại trang Price Sync.

**Nói:**
> "Em vừa bật lại MySQL — giả lập kỹ thuật viên sửa xong cáp mạng. Hệ thống có một con **PriceSyncBackgroundService** chạy ngầm cứ 30 giây quét một lần. Nó tự phát hiện các gói tin lỗi `E`, reset về `P` (Pending) rồi đẩy lại."
>
> *(F5 — chỉ vào dòng vừa chuyển từ E sang S)*
>
> "Trạng thái đã tự động chuyển thành `S` (Success). Hệ thống đã **tự phục hồi** mà không cần con người can thiệp. Đây chính là kiến trúc **Self-Healing** — Fault Tolerance trong CSDL Phân tán."

---
---

## 🛡️ PHẦN 2: NGÂN HÀNG CÂU HỎI PHÒNG THỦ (20+ câu)

### 📂 NHÓM A: Câu hỏi về KIẾN TRÚC HỆ THỐNG

**❓ A1: "Tại sao em chọn Linked Server mà không viết API Middleware?"**
> "Dạ Linked Server là giải pháp tích hợp sẵn trong SQL Server, cho phép truy vấn trực tiếp bằng T-SQL mà không cần viết thêm tầng trung gian. Nếu dùng API Middleware, em phải code thêm một service riêng, deploy thêm một server, tăng complexity. Với quy mô đồ án (2 Node), Linked Server là giải pháp tối ưu nhất về chi phí và độ phức tạp ạ."

**❓ A2: "Linked Server có nhược điểm gì?"**
> "Dạ có ạ. Thứ nhất, hiệu năng phụ thuộc vào đường truyền mạng — nếu mạng chậm thì query chậm. Thứ hai, OPENQUERY không hỗ trợ truyền tham số trực tiếp, em phải bọc trong Dynamic SQL. Thứ ba, nó tạo tight coupling giữa 2 server — nếu scale lên nhiều chi nhánh thì nên chuyển sang Kafka/RabbitMQ. Chính vì vậy trong slide Tầm nhìn mở rộng, em đã đề cập hướng phát triển Level 2."

**❓ A3: "ODBC là gì? Tại sao cần ODBC?"**
> "ODBC là Open Database Connectivity — một chuẩn giao tiếp mở cho phép ứng dụng Windows kết nối với bất kỳ DBMS nào. SQL Server không 'nói' được tiếng MySQL trực tiếp, nên cần ODBC làm trung gian phiên dịch. Em cài MySQL Connector/ODBC 64-bit, tạo System DSN trỏ đến port 3306, rồi Linked Server dùng OLE DB Provider for ODBC để gọi qua."

**❓ A4: "Nếu có 10 chi nhánh dùng 10 hệ quản trị khác nhau thì sao?"**
> "Thì kiến trúc Linked Server sẽ không còn tối ưu vì phải tạo 10 Linked Server, 10 DSN. Lúc đó em sẽ chuyển sang **API Gateway + Message Broker (Kafka)** — mỗi chi nhánh expose một REST API, Gateway ở trụ sở tổng hợp. Đó là Level 2-3 trong roadmap của em."

---

### 📂 NHÓM B: Câu hỏi về ĐỒNG BỘ DỮ LIỆU

**❓ B1: "Tại sao dùng Trigger nhét Queue mà không dùng Trigger update thẳng xuống MySQL?"**
> "Dạ vì nếu Trigger gọi thẳng MySQL qua mạng mà lúc đó mạng đứt, cái Trigger sẽ bị timeout. Mà Trigger chạy trong cùng transaction với lệnh UPDATE gốc, nên transaction đó cũng bị treo, khóa luôn bảng SanPham ở SQL Server. Nghĩa là đứt mạng chi nhánh → sập luôn trụ sở. Nhét vào Queue chỉ tốn 0.001s (ghi nội bộ), tách rời hoàn toàn 2 hệ thống."

**❓ B2: "Đồng bộ bất đồng bộ (Async) có nhược điểm gì?"**
> "Có ạ. Nhược điểm là **Eventual Consistency** — dữ liệu giữa 2 Node không nhất quán tức thời. Trong khoảng thời gian Queue chưa được xử lý (vài giây đến vài phút), giá ở Chi nhánh vẫn là giá cũ. Nhưng với bài toán bán lẻ, điều này chấp nhận được vì khách hàng không nhạy cảm với vài phút trễ giá. Nếu cần Strong Consistency thì phải dùng Two-Phase Commit, nhưng sẽ đánh đổi bằng hiệu năng và nguy cơ treo."

**❓ B3: "PriceSyncBackgroundService chạy 30 giây một lần, nếu lượng Queue quá lớn thì sao?"**
> "Nếu Queue tích tụ hàng nghìn gói tin, Stored Procedure `sp_ProcessPriceSyncQueue` sẽ xử lý từng dòng một trong vòng lặp CURSOR. Với quy mô đồ án thì OK, nhưng production thực tế thì em sẽ chuyển sang xử lý batch (UPDATE TOP 100) hoặc dùng Kafka consumer group để scale horizontal."

**❓ B4: "Nếu Background Service bị crash thì sao?"**
> "ASP.NET Core tự động restart Hosted Service khi app restart. Ngoài ra, các gói tin lỗi vẫn nằm an toàn trong bảng PriceSyncQueue với trạng thái E, không bị mất. Khi service chạy lại, nó sẽ quét và xử lý tiếp. Đây là ưu điểm của kiến trúc Queue — dữ liệu persist trên disk, không nằm trong RAM."

**❓ B5: "Tại sao đồng bộ một chiều (HQ → Branch) mà không phải hai chiều?"**
> "Vì theo nghiệp vụ, giá gốc chỉ do Trụ sở quyết định. Chi nhánh không được phép tự ý đổi giá. Nếu cho đồng bộ 2 chiều sẽ phát sinh xung đột (conflict): 2 bên cùng sửa giá khác nhau thì lấy giá nào? Đồng bộ 1 chiều đơn giản, an toàn, phù hợp mô hình Master-Replica."

---

### 📂 NHÓM C: Câu hỏi về TRUY VẤN PHÂN TÁN

**❓ C1: "View Luận lý giải quyết vấn đề gì?"**
> "Nó giải quyết 2 vấn đề: Một là **che giấu sự phân tán** — ứng dụng C# chỉ cần query View như bảng nội bộ. Hai là **ép kiểu dữ liệu** — CAST từ TIMESTAMP (MySQL) sang DATETIME2 (SQL Server) để tránh lỗi runtime khi C# mapping."

**❓ C2: "OPENQUERY là gì? Khác gì với 4-part name (Server.DB.Schema.Table)?"**
> "OPENQUERY gửi nguyên câu SELECT sang MySQL xử lý, chỉ trả kết quả về SQL Server. Còn 4-part name (ví dụ MYSQL.TechStore_Branch..SanPham) thì SQL Server sẽ cố kéo toàn bộ bảng về local rồi mới filter — rất chậm. OPENQUERY hiệu quả hơn vì MySQL filter trước rồi mới gửi kết quả."

**❓ C3: "Nếu bảng HoaDon ở MySQL có 10 triệu dòng, query có chậm không?"**
> "Có thể chậm vì OPENQUERY kéo toàn bộ kết quả qua mạng. Giải pháp: Một là thêm WHERE filter ngay trong chuỗi OPENQUERY để MySQL lọc trước. Hai là tạo Index trên cột ProductID và SaleDate bên MySQL. Ba là cache kết quả bằng Materialized View hoặc snapshot table ở SQL Server."

**❓ C4: "Dynamic SQL có nguy cơ SQL Injection không?"**
> "Có rủi ro nếu không xử lý đúng. Trong code C# của em, em dùng `.Replace(\"'\", \"''\")`  để escape single quote trước khi ghép chuỗi. Đây là biện pháp cơ bản. Trong production thực tế, em sẽ dùng parameterized stored procedure hoặc API middleware để tránh hoàn toàn injection."

---

### 📂 NHÓM D: Câu hỏi về CODE & WEB

**❓ D1: "Tại sao dùng ADO.NET thuần mà không dùng Entity Framework?"**
> "Vì hệ thống cần gọi OPENQUERY, Dynamic SQL, và Stored Procedure — toàn bộ đều là lệnh SQL thô. Entity Framework không hỗ trợ tốt các lệnh này, đặc biệt là EXEC AT MYSQL. ADO.NET cho em toàn quyền kiểm soát từng câu lệnh SQL gửi xuống database."

**❓ D2: "Repository Pattern là gì? Tại sao dùng?"**
> "Repository Pattern là mẫu thiết kế tách logic truy cập dữ liệu ra khỏi UI. Toàn bộ SQL nằm trong class `TechStoreRepository`, các trang Razor Pages chỉ gọi hàm như `GetProductsAsync()`, `UpdateProductAsync()`. Nếu mai mốt đổi từ ADO.NET sang Entity Framework, em chỉ cần sửa trong Repository, không ảnh hưởng UI."

**❓ D3: "Thầy kêu mở file code, giải thích hàm UpdateProductAsync."**
> *(Mở file TechStoreRepository.cs, line 112)*
> "Hàm này làm 4 việc:
> 1. Lấy giá cũ từ SQL Server (để so sánh).
> 2. UPDATE tên, giá, danh mục vào bảng SanPham tại SQL Server.
> 3. Dùng Dynamic SQL `EXEC('UPDATE ...') AT MYSQL` để cập nhật tên và danh mục sang MySQL.
> 4. Nếu giá thay đổi (oldPrice != newPrice), gọi `sp_ProcessPriceSyncQueue` để đẩy giá mới qua Queue xuống MySQL."

**❓ D4: "Giải thích hàm ProcessQueueAsync trong Background Service."**
> *(Mở file TechStoreRepository.cs, line 646)*
> "Hàm này chỉ 2 bước:
> 1. **Reset** tất cả dòng có Status `E` (Error) về `P` (Pending) — cho phép Stored Procedure retry.
> 2. **Gọi** `sp_ProcessPriceSyncQueue` — SP này sẽ quét các dòng `P`, thử đẩy giá xuống MySQL. Nếu thành công → `S`. Nếu lỗi → `E` kèm message lỗi."

---

### 📂 NHÓM E: Câu hỏi về LÝ THUYẾT CSDL PHÂN TÁN

**❓ E1: "Hệ thống của em thuộc loại CSDL phân tán nào?"**
> "Hệ thống phân tán **bất đồng nhất (Heterogeneous)** ạ. Vì 2 Node dùng 2 hệ quản trị khác nhau (SQL Server và MySQL), khác schema, khác kiểu dữ liệu. Đây là loại khó nhất trong CSDL phân tán."

**❓ E2: "12 quy tắc của Date về CSDL phân tán, em đáp ứng được những quy tắc nào?"**
> "Em đáp ứng được:
> - **Tính trong suốt vị trí (Location Transparency)** — View Luận lý che giấu vị trí MySQL.
> - **Tính trong suốt phân mảnh (Fragmentation Transparency)** — User query một bảng thống nhất.
> - **Tính độc lập DBMS** — Hệ thống hoạt động dù 2 Node dùng engine khác nhau.
> - **Xử lý giao dịch phân tán** — Queue + Background Service thay thế Two-Phase Commit."

**❓ E3: "CAP Theorem — hệ thống của em chọn gì?"**
> "Em chọn **AP (Availability + Partition Tolerance)**, hy sinh **Consistency** tức thời. Khi mạng đứt (Partition), hệ thống vẫn available (Trụ sở vẫn hoạt động bình thường). Dữ liệu sẽ eventually consistent khi mạng khôi phục và Queue được xử lý."

**❓ E4: "Phân mảnh ngang vs dọc — em dùng loại nào?"**
> "Nhóm em dùng **Phân mảnh lai (Hybrid)**. Theo chiều ngang: bảng SanPham bị replicate (1 bản gốc ở HQ, 1 bản sao ở Branch). Theo chiều dọc: bảng HoaDon chỉ nằm ở Branch (phân tán theo chức năng — chi nhánh quản lý giao dịch bán hàng)."

**❓ E5: "Two-Phase Commit (2PC) là gì? Tại sao em không dùng?"**
> "2PC là giao thức đảm bảo cả 2 Node cùng commit hoặc cùng rollback. Pha 1: Coordinator hỏi 'Sẵn sàng chưa?'. Pha 2: Nếu cả 2 OK thì commit, có 1 Node lỗi thì rollback hết. Em không dùng vì MSDTC (bộ điều phối 2PC của Microsoft) không ổn định khi làm việc với MySQL qua ODBC. Thay vào đó em dùng Queue + Retry — đạt Eventual Consistency mà không cần 2PC."

---

### 📂 NHÓM F: Câu hỏi "BẪY" (Thầy cố tình hỏi khó)

**❓ F1: "Em có chắc hệ thống không mất dữ liệu không? Nếu SQL Server cũng sập thì sao?"**
> "Nếu cả SQL Server cũng sập thì dữ liệu Queue vẫn an toàn trên ổ cứng vì nó là bảng vật lý, không nằm trong RAM. Khi SQL Server restart, Background Service tự chạy lại và xử lý tiếp. Tất nhiên, nếu ổ cứng hỏng vật lý thì cần backup/restore — đó là bài toán Disaster Recovery, nằm ngoài scope đồ án nhưng em biết cần dùng SQL Server Always On hoặc backup tự động."

**❓ F2: "Code coi như AI viết hết, em có hiểu không?"**
> *(Bình tĩnh, không run)* "Dạ em hiểu rõ ạ. Em xin giải thích bất kỳ hàm nào thầy chỉ định. Ví dụ hàm `UpdateProductAsync` gồm 4 bước: lấy giá cũ, update HQ, dynamic SQL update branch, rồi gọi queue nếu giá đổi. Em có thể mở code và chỉ từng dòng cho thầy."

**❓ F3: "Nếu thầy kêu em thêm một tính năng mới ngay bây giờ, ví dụ đồng bộ Category, em làm thế nào?"**
> "Em sẽ thêm một Trigger mới trên bảng SanPham bắt sự kiện UPDATE trên cột Category, nhét vào một bảng Queue mới (hoặc thêm cột CategoryChanged vào PriceSyncQueue hiện tại). Rồi trong Background Service, em thêm logic xử lý cho loại sự kiện mới này. Pattern hoàn toàn giống Price Sync, chỉ khác cột dữ liệu."

**❓ F4: "Tại sao em chọn C# ASP.NET mà không dùng Python/Java?"**
> "Vì SQL Server tích hợp tốt nhất với hệ sinh thái Microsoft. ADO.NET (SqlClient) là thư viện native cho SQL Server, hiệu năng cao nhất. ASP.NET Core hỗ trợ Background Hosted Service sẵn — em không cần cài thêm gì để chạy Worker. Ngoài ra, .NET 9 là phiên bản mới nhất, cho thấy nhóm em cập nhật công nghệ."

---

**💡 MẸO VÀNG KHI BỊ HỎI MÀ KHÔNG BIẾT:**
> "Dạ câu hỏi này rất hay ạ. Trong phạm vi đồ án, em chưa triển khai phần đó, nhưng em nghĩ hướng giải quyết sẽ là [nêu ý tưởng chung]. Em sẽ nghiên cứu thêm sau khi kết thúc môn học ạ."

Đừng bao giờ nói "Em không biết" mà hãy nói "Em chưa triển khai nhưng em nghĩ..." — Điều này cho thấy em có tư duy giải quyết vấn đề.
