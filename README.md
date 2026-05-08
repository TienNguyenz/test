# 🛒 TechStore — Heterogeneous Distributed Database System

> **Academic Project** — Distributed Database Course, Sai Gon University  
> Demonstrates cross-DBMS integration between **Microsoft SQL Server** (HQ) and **MySQL** (Branch) using ODBC Linked Server, without migrating or duplicating data.

---

## 🏗️ Architecture Overview

```
┌─────────────────────────────────────┐      ┌─────────────────────────────────────┐
│   HQ Node — SQL Server              │      │   Branch Node — MySQL               │
│   TechStore_HQ                      │      │   TechStore_Branch                  │
│                                     │      │                                     │
│   • SanPham (Master catalog)        │◄────►│   • SanPham  (Price replica)        │
│   • PriceSyncQueue (Message queue)  │      │   • HoaDon   (Sales invoices)       │
│   • vw_BranchInvoices_Logical       │      │                                     │
│   • sp_BaoCaoDoanhThu               │      │                                     │
│   • sp_ProcessPriceSyncQueue        │      │                                     │
└─────────────────────────────────────┘      └─────────────────────────────────────┘
              │  Linked Server (ODBC / MYSQL)  │
              └────────────────────────────────┘
```

**Key design decisions:**
- **Logical Transparency** — App queries `vw_BranchInvoices_Logical` on SQL Server; MySQL's location is fully hidden.
- **Async Queue Sync** — Price changes flow: `UPDATE SanPham` → `Trigger` → `PriceSyncQueue (P)` → `sp_ProcessPriceSyncQueue` → MySQL.
- **Fault Tolerance** — `PriceSyncBackgroundService` (ASP.NET Core Hosted Service) retries failed queue items every 30 seconds automatically.
- **Dynamic SQL** — Cross-server updates use `EXEC('...') AT MYSQL` to avoid MSDTC instability.

---

## 🚀 Quick Start

### Prerequisites
| Requirement | Details |
|---|---|
| SQL Server | Any edition with Linked Server support |
| MySQL 8.x | Running on port `3306` |
| MySQL Connector/ODBC 64-bit | [Download](https://dev.mysql.com/downloads/connector/odbc/) |
| .NET 9 SDK | [Download](https://dotnet.microsoft.com/download) |

### 1. Configure Linked Server
1. Install **MySQL Connector/ODBC** (64-bit).
2. Open **ODBC Data Source Administrator (64-bit)** → Add System DSN named `MySQL_TechStore`.
   - Host: `localhost` | Port: `3306` | Database: `TechStore_Branch`
3. In SSMS, create a Linked Server named **`MYSQL`**:
   - Provider: `Microsoft OLE DB Provider for ODBC Drivers`
   - Data Source: `MySQL_TechStore`

### 2. Initialize Database Objects
Run in order on **SQL Server (TechStore_HQ)**:
```bash
# Required: views, stored procedures, queue table, trigger
scripts/setup_required_objects.sql

# Optional: add CHECK constraints for data integrity  
scripts/schema_constraints.sql

# Optional: SQL Agent job to auto-process queue every 1 minute
scripts/setup_sql_agent_job.sql
```

### 3. Run the Web App
```bash
dotnet run --project TechStoreWeb
```
Open the URL printed in the terminal (typically `https://localhost:5001`).

---

## 📁 Project Structure

```
TechStore/
├── TechStoreWeb/               # ASP.NET Core Razor Pages application
│   ├── Services/
│   │   ├── TechStoreRepository.cs          # All DB access (ADO.NET, no ORM)
│   │   └── PriceSyncBackgroundService.cs   # Fault-tolerant retry worker
│   ├── Models/                 # Strongly-typed result/view models
│   ├── Pages/
│   │   ├── Index              # Dashboard — integration health & live stats
│   │   ├── Products/          # CRUD with HQ→Branch sync
│   │   ├── Invoices/          # Branch invoices via Logical View
│   │   ├── Reports/           # Revenue report (sp_BaoCaoDoanhThu)
│   │   └── Distributed/
│   │       ├── Evidence       # Transparent JOIN proof page
│   │       ├── PriceSync      # Real-time queue monitoring
│   │       └── DataContracts  # Type mapping between SQL Server & MySQL
│   └── wwwroot/               # Static assets
├── scripts/
│   ├── setup_required_objects.sql    # Core DB objects (run this first)
│   ├── schema_constraints.sql        # CHECK constraints hardening
│   ├── setup_sql_agent_job.sql       # Auto-process queue via SQL Agent
│   ├── seed_dummy_data.sql           # Sample data (66 products, 183 invoices)
│   ├── test_cases.sql                # TC01–TC10 verification scripts
│   └── demo_script.sql               # Live demo walkthrough script
└── docs/
    └── BaoCao_TechStore_ChinhThuc.md # Academic report (Markdown)
```

---

## 🌐 Web Application Modules

| Module | Route | Description |
|---|---|---|
| Dashboard | `/` | Integration health, live node stats (HQ & Branch) |
| Products | `/Products` | Full CRUD — syncs to MySQL on create/update/delete |
| Invoices | `/Invoices` | Branch sales data via `vw_BranchInvoices_Logical` |
| Revenue Report | `/Reports/Revenue` | Cross-node aggregation via `sp_BaoCaoDoanhThu` |
| Price Sync | `/Distributed/PriceSync` | Real-time queue status (P/S/E per item) |
| Evidence | `/Distributed/Evidence` | Live transparent JOIN proof |
| Data Contracts | `/Distributed/DataContracts` | Type mapping: `DATETIME2` ↔ `TIMESTAMP`, etc. |

---

## 🧪 Test Cases

| ID | Objective | Expected Result |
|---|---|---|
| TC01 | Linked Server connectivity | `sp_testlinkedserver` returns Success |
| TC02 | Cross-system OPENQUERY read | MySQL invoices visible in SSMS |
| TC03 | Unified revenue report | `sp_BaoCaoDoanhThu` JOINs both nodes |
| TC04 | Price update trigger | Queue row inserted on `SanPham` price change |
| TC05 | Queue record (Pending) | Status = `P` after update |
| TC06 | Branch sync success | Status = `S`, MySQL price matches HQ |
| TC07 | Network failure tolerance | Status = `E`, `LastError` populated, app does not crash |
| TC08 | DateTime filter compatibility | Date filter works across `DATETIME2`/`TIMESTAMP` boundary |
| TC09 | Full CRUD via Web UI | All operations trigger sync queue automatically |
| TC10 | Check constraint enforcement | Negative price rejected by `CK_SanPham_Price_NonNegative` |

Run all test cases:
```sql
-- scripts/test_cases.sql (run on SQL Server / TechStore_HQ)
```

---

## 👥 Authors

| Name | Student ID |
|---|---|
| Nguyễn Hoàng Tiến | 3121411206 |
| Lê Duy Quân | 3121411176 |

**Course:** Distributed Database Systems  
**Instructor:** Vũ Ngọc Thanh Sang  
**University:** Sai Gon University — Faculty of Information Technology  
**Class:** DCT123C2 | **Year:** 2026–2027
