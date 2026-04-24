# TechStore Live Demo Guide (3-5 minutes)

Use this script when presenting to lecturer.

## Goal
Show that the system satisfies:
- Cross-DBMS integration (SQL Server + MySQL)
- Transparent query/reporting
- Price synchronization from HQ to Branch

## Demo Flow
1. Open SSMS and run the first block in [scripts/demo_script.sql](../scripts/demo_script.sql) to show current HQ price and Branch price.
2. Update HQ price (`UPDATE dbo.SanPham`).
3. Show queue item (`PriceSyncQueue`) with status `P`.
4. Run `EXEC dbo.sp_ProcessPriceSyncQueue`.
5. Show queue status changed to `S`.
6. Query branch price again via OPENQUERY and show updated value.
7. Run `EXEC dbo.sp_BaoCaoDoanhThu` to show integrated sales report.
8. Open web app and repeat update/report actions from UI.

## Presenter Notes
- Emphasize "no full data migration": only integrated access and targeted sync.
- Mention queue-based sync avoids distributed transaction limitation (Msg 7391).
- Mention MySQL permission issue was solved by proper GRANT.

## Backup Plan (if live issue happens)
- Re-run `sp_testlinkedserver 'MYSQL'`.
- Check MySQL service is running.
- Re-run queue processor once.
