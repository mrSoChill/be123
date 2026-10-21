/*
    Optional data for manual CRUD demonstrations only.
    Run only on an approved demo database. This script never deletes records.
*/
USE [MyStore];
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Category WHERE CategoryName = N'Danh mục demo')
BEGIN
    INSERT INTO dbo.Category (CategoryName) VALUES (N'Danh mục demo');
END
GO
