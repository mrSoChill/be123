/*
  Reversible EF6 Designer update probe for the MyStore demo database.

  Run this script only immediately before the "Update Model from Database"
  wizard is available. The paired revert script removes this nullable column.
  It does not seed, delete, or change existing application columns/data.
*/
USE [MyStore];
GO

IF COL_LENGTH(N'dbo.Category', N'EF6UpdateProbe') IS NULL
BEGIN
    ALTER TABLE dbo.Category ADD EF6UpdateProbe NVARCHAR(64) NULL;
END;
GO
