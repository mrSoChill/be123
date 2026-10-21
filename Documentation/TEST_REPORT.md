# Test report — BE1--BE3

Audit date: 2026-10-21 (Asia/Ho_Chi_Minh)

## Executed tests

| Test | Result | Evidence |
| --- | --- | --- |
| MVC5 project build | PASS | MSBuild 18.11 built `demoBE.slnx` in Debug configuration with `MvcBuildViews=true` after EDMX project integration and CRUD implementation. |
| NuGet restore check | PASS | MSBuild `/t:Restore` completed with exit code 0 and reported no packages remaining to restore; `EntityFramework` 6.5.1 DLL is present. |
| Default route | PASS | IIS Express returned HTTP 200 for `/Home/Index` on a fresh IIS Express run (port 51239). |
| Admin route | PASS | IIS Express returned HTTP 200 for `/Admin/Home/Index` on a fresh IIS Express run (port 51239). |
| Route collision regression | PASS | Default route now restricts namespaces to `demoBE.Controllers`; the root and Admin Home controllers no longer conflict. |
| MyStore schema | PASS | Direct SQL audit on `(localdb)\MSSQLLocalDB`, detailed in `SQL_AUDIT.md`. |
| Safe SQL script re-run | PASS | `Create_MyStore.sql` completed all eight batches with MyStore already registered; `Attach_Orphaned_MyStore_LocalDB.sql` completed its no-op branch. The required table count remained six. |
| EF6 package references | PASS (build verified) | EntityFramework 6.5.1 references compile with the project. |
| EDMX generation and EF connection | PASS | Visual Studio-generated `Model1.edmx`, T4 entities and `MyStoreEntities` are present. IIS Express queried MyStore via the EntityClient metadata connection. |
| Category validation | PASS | Empty Category POST returned its validation field and did not save a record. |
| Category CRUD | PASS | Created a uniquely prefixed Category, edited it, loaded Details, blocked deletion while a Product referenced it, then deleted it after Product cleanup. |
| Product validation and dropdown | PASS | Invalid Product POST returned validation fields and retained the Category dropdown. |
| Product CRUD and relationship | PASS | Created Product with Category FK, rendered Category through `Include`, edited name/price/category, rendered Details, and deleted it. |
| Product Edit selected value | PASS | Product Edit GET rendered the persisted Category option with `selected="selected"`; cleanup then deleted the dedicated Product/Category records. |
| Product FK delete protection | PASS | An OrderDetail reference was created for a dedicated test Product; Product Delete returned the friendly error and retained the record. |
| 404 handling | PASS | Missing Category Details and missing Product Details each returned HTTP 404. |
| Test cleanup | PASS | All `__CodexCrud_*` and `__CodexFk_*` records were removed. Final MyStore row counts are zero for all six tables. |
| Post-build regression check | PASS | After re-adding MVC project items that Visual Studio had omitted while writing generated model items, Category and Product routes again returned HTTP 200 on port 51239. |
| Repository hygiene | PASS | Local Git repository initialized; `.gitignore` was verified by `git check-ignore`; scoped `git clean` removed `demoBE/bin` and `demoBE/obj`. No remote, commit, or push exists. |
| Test-data cleanup | PASS | Direct LocalDB count query returned zero rows for Category, Product, User, Customer, Order, and OrderDetail. |

## Not executed

- No sample data remains after the runtime tests.
- No `ALTER AUTHORIZATION` command was run.
- No YouTube video workflow was claimed as verified; their pages/transcripts
  were unavailable during the prior audit.
- **Update Model from Database** remains BLOCKED: reversible scripts and the
  exact Designer procedure are recorded in `EDMX_UPDATE_CHECKPOINT.md`, but no
  native Visual Studio/EF6 Designer window was available to run the wizard and
  verify the generated property. Neither probe script was executed.
