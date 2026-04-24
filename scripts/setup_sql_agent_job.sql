/*
Optional: create SQL Agent job to auto-process queue every 1 minute.
Run in SQL Server (msdb database).
*/

USE msdb;
GO

IF NOT EXISTS (SELECT 1 FROM msdb.dbo.sysjobs WHERE name = N'Job_ProcessPriceSyncQueue')
BEGIN
    EXEC sp_add_job
        @job_name = N'Job_ProcessPriceSyncQueue',
        @enabled = 1,
        @description = N'Process pending price sync queue from HQ to MySQL';

    EXEC sp_add_jobstep
        @job_name = N'Job_ProcessPriceSyncQueue',
        @step_name = N'RunQueueProcessor',
        @subsystem = N'TSQL',
        @database_name = N'TechStore_HQ',
        @command = N'EXEC dbo.sp_ProcessPriceSyncQueue;';

    EXEC sp_add_schedule
        @schedule_name = N'Sch_Every_1_Minute_ProcessPriceSyncQueue',
        @enabled = 1,
        @freq_type = 4,
        @freq_interval = 1,
        @freq_subday_type = 4,
        @freq_subday_interval = 1,
        @active_start_time = 0;

    EXEC sp_attach_schedule
        @job_name = N'Job_ProcessPriceSyncQueue',
        @schedule_name = N'Sch_Every_1_Minute_ProcessPriceSyncQueue';

    EXEC sp_add_jobserver
        @job_name = N'Job_ProcessPriceSyncQueue';
END
GO
