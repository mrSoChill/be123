# EF6 Update Model from Database checkpoint

The genuine Designer model is `demoBE/Models/Model1.edmx`. The following is a
reversible, isolated test for the EF6 **Update Model from Database** workflow;
it intentionally leaves the original `MyStore` schema unchanged when complete.

1. Make sure no application/test process is using `MyStore`.
2. In SSMS, execute `Database/EDMX_Update_Probe_Add.sql` against the approved
   demo `MyStore` database. It only adds nullable
   `dbo.Category.EF6UpdateProbe` if absent.
3. In Visual Studio, open `Models/Model1.edmx` in the EF6 Designer. Right-click
   an empty area of the designer and choose **Update Model from Database...**.
4. On the **Add** tab, expand `Tables`, choose `Category`, complete the wizard,
   save the EDMX, and rebuild the solution.
5. Confirm `Category` exposes `EF6UpdateProbe` in the generated entity (and
   that the project builds). Record the result in `TEST_REPORT.md`.
6. Execute `Database/EDMX_Update_Probe_Revert.sql` in SSMS. Repeat steps 3--5
   so the property is removed from the EDMX, then build once more.

Do not hand-edit EDMX XML or run either script against production data. The
scripts were prepared but deliberately not executed in this audit because no
Visual Studio native window/EF6 Designer surface was available to finish and
verify the paired wizard operation.
