# BAO CAO DO AN - TECHSTORE

## 1. Gioi thieu
De tai: TechStore - Quan ly Chuoi Ban le Da nen tang.

Boi canh nghiep vu:
- Tru so chinh su dung SQL Server.
- Chi nhanh su dung MySQL.
- Sau sap nhap, khong muon migrate full du lieu vi ton kem va gian doan.

Muc tieu:
- Tich hop he thong SQL Server va MySQL.
- Truy van tong hop du lieu khac he quan tri.
- Dong bo gia san pham tu HQ sang Branch.
- Trinh bay demo bang web Razor Pages.

## 2. Kien truc giai phap
### 2.1 Thanh phan
- SQL Server (HQ): `TechStore_HQ`, bang `SanPham`, queue dong bo.
- MySQL (Branch): `TechStore_Branch`, bang `HoaDon`, bang `SanPham`.
- Linked Server `MYSQL`: SQL Server ket noi MySQL qua ODBC (MSDASQL).
- Web app: ASP.NET Core Razor Pages.

### 2.2 Luong du lieu
1. Web cap nhat gia tai HQ (`SanPham`).
2. Trigger ghi su kien vao `PriceSyncQueue`.
3. Procedure `sp_ProcessPriceSyncQueue` day gia sang MySQL.
4. Bao cao doanh thu goi `sp_BaoCaoDoanhThu` de tong hop HQ + Branch.

## 3. Cai dat va cau hinh
### 3.1 SQL Server
- Tao CSDL `TechStore_HQ`.
- Bang chinh: `dbo.SanPham`, `dbo.PriceSyncQueue`.
- Procedure:
  - `dbo.sp_BaoCaoDoanhThu`
  - `dbo.sp_ProcessPriceSyncQueue`
- Trigger:
  - `dbo.trg_SanPham_QueuePriceSync`

### 3.2 MySQL
- Tao CSDL `TechStore_Branch`.
- Bang chinh:
  - `SanPham`
  - `HoaDon`
- Phan quyen user linked server (`tien@localhost`) cho SELECT/UPDATE can thiet.

### 3.3 Linked Server
- DSN ODBC: MySQL ODBC 8.x
- SQL Server linked server: `MYSQL`
- Kiem tra ket noi:
  - `EXEC sp_testlinkedserver 'MYSQL'`
  - `SELECT * FROM OPENQUERY(MYSQL, 'SELECT 1 AS ok')`

## 4. Truy van tong hop va dong bo
### 4.1 Truy van tong hop
- HQ doc hoa don Branch thong qua OPENQUERY.
- Bao cao doanh thu thong qua `sp_BaoCaoDoanhThu`.

### 4.2 Dong bo gia
- Trigger khong cap nhat MySQL truc tiep (tranh distributed transaction Msg 7391).
- Trigger ghi queue, procedure xu ly queue de day update sang MySQL.
- Co the dat SQL Server Agent Job chay 1 phut/lan de gan realtime.

## 5. Xu ly sai khac du lieu
- DateTime: CAST tu MySQL datetime sang SQL Server datetime2.
- So tien: dung decimal(18,2) dong nhat 2 he.
- OPENQUERY tra ve kieu can CAST ro rang trong query SQL Server.

## 6. Web demo
Project: [TechStoreWeb](../TechStoreWeb)

Trang chinh:
- Dashboard: tong quan he thong.
- Products: CRUD san pham HQ (them/sua/xoa).
- Update Price: cap nhat gia + xu ly sync queue.
- Invoices: xem hoa don Branch.
- Revenue: bao cao tong hop theo ngay.
- Distributed Evidence: minh chung linked server + transparent JOIN.
- Data Contracts: rang buoc truong du lieu va mapping kieu du lieu.

Connection string duoc cau hinh tai:
- [TechStoreWeb/appsettings.json](../TechStoreWeb/appsettings.json)

## 7. Test case
Tai lieu test case:
- [docs/TestCases.md](TestCases.md)
- Script chay test:
  - [scripts/test_cases.sql](../scripts/test_cases.sql)

Ket qua mong doi:
- TC01..TC08 dat yeu cau toi thieu cua de bai.
- TC09..TC10 la test mo rong cho CRUD va rang buoc CSDL.

## 8. Script demo
- [scripts/demo_script.sql](../scripts/demo_script.sql)
- Huong dan demo:
  - [docs/DemoGuide.md](DemoGuide.md)

## 9. Ket luan
He thong da dat cac yeu cau chinh cua de tai:
1. Ket noi da he quan tri SQL Server + MySQL.
2. Truy van tong hop trong suot qua OPENQUERY/stored procedure.
3. Dong bo gia HQ -> Branch thanh cong qua queue processor.
4. Xu ly sai khac kieu du lieu DateTime/Decimal.
5. Co bo test case va web demo truc quan de bao ve do an.
6. Co giao dien CRUD va trang minh chung CSDL phan tan de trinh bay ro bang chung hoc thuat.

## 10. Phu luc
### 10.1 Lenh chay web
```bash
dotnet run --project TechStoreWeb
```

### 10.2 URL mac dinh
- `https://localhost:xxxx`
- `http://localhost:xxxx`

### 10.3 Luu y khi demo
- Dam bao MySQL service dang chay.
- Dam bao linked server `MYSQL` test pass truoc khi demo.
- Neu queue co `Status = E`, kiem tra `LastError` va quyen MySQL.
