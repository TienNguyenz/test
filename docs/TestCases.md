# TechStore Test Cases

This document tracks the 8 required test cases and 2 extended quality test cases.

## Environment
- HQ DBMS: SQL Server (database: TechStore_HQ)
- Branch DBMS: MySQL (database: TechStore_Branch)
- Integration: Linked Server `MYSQL` via ODBC/MSDASQL
- Main script: [scripts/test_cases.sql](../scripts/test_cases.sql)

## Test Matrix

| ID | Objective | Steps | Expected Result |
|---|---|---|---|
| TC01 | Verify SQL Server can reach linked server | Run `sp_testlinkedserver 'MYSQL'` | Command returns without error |
| TC02 | Verify branch invoice read via OPENQUERY | Run OPENQUERY select from `TechStore_Branch.HoaDon` | Invoice rows returned |
| TC03 | Verify integrated report procedure | Run `EXEC dbo.sp_BaoCaoDoanhThu` | Product + sales summary rows returned |
| TC04 | Verify HQ price update action | Update one product price in `dbo.SanPham` | 1 row affected |
| TC05 | Verify queue record creation and processing | Check `PriceSyncQueue` before/after `sp_ProcessPriceSyncQueue` | Status transitions from `P` to `S` |
| TC06 | Verify branch price synchronization | Compare branch price before/after sync | Branch price equals new HQ price |
| TC07 | Verify permission error handling | Revoke MySQL update permission, then process queue | Status `E` and `LastError` populated |
| TC08 | Verify DateTime filtering compatibility | Execute report with `@FromDate/@ToDate` | Correct filtered result set |
| TC09 | Verify product CRUD workflow | Create, update, and delete a test product | CRUD actions behave correctly and sync process runs |
| TC10 | Verify DB field constraints | Attempt invalid negative price update | SQL check constraint rejects invalid value |

## Evidence to Capture
- Screenshot of each test result grid.
- Queue transition screenshot for TC05.
- Branch price before/after screenshot for TC06.
- Permission failure screenshot for TC07.

## Quick Run
1. Open SSMS on HQ instance.
2. Run [scripts/test_cases.sql](../scripts/test_cases.sql).
3. Save screenshots and mark PASS/FAIL in your final report.
