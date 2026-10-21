/*
  Revert the isolated, nullable EF6 Designer update probe column.

  Before executing, use EF6 Designer's Update Model from Database again so
  the EDMX no longer contains Category.EF6UpdateProbe. This script does not
  touch rows or original MyStore columns.
*/
USE [MyStore];
GO

IF COL_LENGTH(N'dbo.Category', N'EF6UpdateProbe') IS NOT NULL
BEGIN
    ALTER TABLE dbo.Category DROP COLUMN EF6UpdateProbe;
END;
GO
