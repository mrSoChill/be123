# SQL audit — MyStore

Audit date: 2026-10-21 (Asia/Ho_Chi_Minh)

## Sources compared

| Source | Result |
| --- | --- |
| `database2024.txt` at workspace root | Lecturer source supplied by user. |
| `References/database2024.txt` | Byte-identical content to the root copy: SHA-256 `813DA1553A12AFD6ADD91DEFF71C0D1C5446198D7BCC738DF5C45987FC5B610A`. |
| `Database/Create_MyStore.sql` | Separate, safe bootstrap implementation; it does not replace either lecturer file. |
| `(localdb)\MSSQLLocalDB`, database `MyStore` | Attached and queried directly after audit. |

## Schema comparison

The following source requirements were checked against `sys.tables`,
`sys.columns`, `sys.key_constraints`, and `sys.foreign_keys` in MyStore:

- Tables: `User`, `Customer`, `Category`, `Product`, `Order`, `OrderDetail`
  — all six exist in schema `dbo`.
- Primary keys: one per table, matching the source key columns.
- Foreign keys: Customer → User; Product → Category; Order → Customer;
  OrderDetail → Product and OrderDetail → Order — all five exist, with
  `NO_ACTION` delete behavior.
- Types, nullability, identity columns and decimal `(18,2)` fields match the
  source.
- `Product.ProductDecription` exists exactly with that spelling and is
  `NVARCHAR(MAX) NOT NULL`.
- All six tables had zero rows when audited. No sample data was inserted.

## Findings

| Classification | Finding | Resolution |
| --- | --- | --- |
| Confirmed setup defect | The lecturer script comments that MyStore is new, but has neither `CREATE DATABASE` nor `USE [MyStore]`. Executing it while connected to `master` can create tables in the wrong database; the final ownership statement also then targets a non-existent MyStore. | `Database/Create_MyStore.sql` first conditionally creates MyStore, switches to it, and conditionally creates source-equivalent tables. It never drops or alters existing objects. |
| Confirmed unsafe/non-portable command | `ALTER AUTHORIZATION ON DATABASE::MyStore TO sa` requires the database to exist and an appropriate `sa` principal/permission; it is unnecessary for MVC5/EF6. | It is isolated in `Database/Optional_ChangeOwner.sql` and was not executed. |
| Environment recovery event | LocalDB master did not list MyStore while `C:\Users\RENE\MyStore.mdf` and its log still existed. A fresh `CREATE DATABASE` correctly failed rather than overwrite the files. | The verified database files were attached to `(localdb)\MSSQLLocalDB` without deleting/replacing data. `Database/Attach_Orphaned_MyStore_LocalDB.sql` records the guarded recovery for this machine only. |
| Intentional source difference | `ProductDecription` is misspelled relative to normal English but is explicit in both lecturer copies and course instructions. | Preserved everywhere; no migration created. |
| No defect verified | Unnamed FKs in the source `Order` and `OrderDetail` declarations are valid SQL Server syntax. | The bootstrap gives equivalent explicit names only for deterministic auditability; relationship semantics are unchanged. |

## Commands/evidence

The database was queried with Windows Integrated Security against
`Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=MyStore`. Evidence showed
the six required tables, six primary keys, five foreign keys and the full
column list described above. After recovery, both `Create_MyStore.sql` and the
guarded attach script were re-run successfully; the database retained exactly
six required tables. No command with `DROP`, `DELETE`, `ALTER TABLE`, or
`ALTER AUTHORIZATION` was run.
