/*
    Recovery script for the audited LocalDB demo database only.

    Use this only when all of the following are true:
    1. Connected to (localdb)\MSSQLLocalDB as the current Windows user.
    2. DB_ID(N'MyStore') is NULL.
    3. The two exact paths below were checked and are known to be the MyStore
       files for this project.

    It does not DROP, overwrite, or detach any database. If MyStore is already
    registered, it performs no action. Do not reuse the hard-coded paths on a
    different machine; inspect that machine's database files first.
*/
USE [master];
GO

IF DB_ID(N'MyStore') IS NULL
BEGIN
    CREATE DATABASE [MyStore]
    ON
    (FILENAME = N'C:\Users\RENE\MyStore.mdf'),
    (FILENAME = N'C:\Users\RENE\MyStore_log.ldf')
    FOR ATTACH;
END
ELSE
BEGIN
    PRINT N'MyStore is already registered; no attach action was taken.';
END
GO
