# TechStore Project Workspace

## Structure
- `TechStoreWeb/`: Razor Pages demo app.
- `scripts/`: SQL scripts for setup, tests, and live demo.
- `docs/`: final report + test case document + demo guide.
- `CSDLPT.docx`: original source document.

## Web Features
- Dashboard with integration health metrics.
- Product CRUD (Add/Edit/Delete) with field validation.
- Price synchronization console (HQ -> Branch queue processing).
- Branch invoice viewer (OPENQUERY).
- Revenue report with date filter and type normalization.
- Distributed evidence page showing transparent JOIN output.
- Data contracts page showing DB constraints and type mapping.

## Quick Start
1. Ensure SQL Server linked server `MYSQL` is working.
2. Ensure MySQL service is running.
3. Run setup script if needed:
   - `scripts/setup_required_objects.sql`
   - `scripts/schema_constraints.sql`
4. Run web app:
   - `dotnet run --project TechStoreWeb`
5. Open URL from terminal output.

## Key Files
- Final report (Word): `docs/TechStore_Final_Report.docx`
- Final report (Markdown): `docs/TechStore_Final_Report.md`
- Test cases: `docs/TestCases.md`
- Demo flow: `docs/DemoGuide.md`
- Test script: `scripts/test_cases.sql`
- Live demo script: `scripts/demo_script.sql`
- SQL Agent job script: `scripts/setup_sql_agent_job.sql`
- Schema constraints: `scripts/schema_constraints.sql`
- Stitch prompt used for UI redesign: `docs/stitch_prompt_ui.md`
