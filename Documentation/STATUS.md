# Trạng thái triển khai BE1--BE3

Ngày audit: 2026-10-21 (Asia/Ho_Chi_Minh)

## Môi trường

| Hạng mục | Trạng thái | Bằng chứng |
| --- | --- | --- |
| Project MVC5/.NET Framework | PASS (verified) | `demoBE.csproj`: Web Application project type, `TargetFrameworkVersion` `v4.7.2`; `Microsoft.AspNet.Mvc` 5.2.9. |
| Visual Studio/MSBuild | PASS (verified) | Visual Studio Community 18.11 và MSBuild 18.11.0 đã phát hiện. |
| EF6 package | PASS (verified) | `EntityFramework` 6.5.1 có trong `packages.config`, DLL package có mặt và MSBuild `/t:Restore` hoàn tất mã thoát 0 (không có package còn thiếu); build đã copy/compile thành công. |
| SQL Server/SSMS | PASS (verified) | SQL Server LocalDB 2025 v17.0.4025.3 instance `MSSQLLocalDB`, SSMS 22.11.1. |
| Video hướng dẫn | BLOCKED | 11 URL YouTube đã thử truy cập nhưng fetch bị lỗi/throttled; không có transcript/video nào được xác minh. PPTX và ảnh LMS là nguồn đã dùng. |
| Build | PASS (verified) | `MSBuild.exe demoBE.slnx /t:Build /p:Configuration=Debug /p:MvcBuildViews=true /m /v:minimal` thành công sau khi tích hợp EDMX/T4, CRUD và Razor view compilation. |
| IIS Express smoke test | PASS (verified) | IIS Express port 51239 đã trả HTTP 200 cho Home, Admin Home, Category và Product routes sau build cuối. Process test đã được dừng. |

## BE1

| Hạng mục P0 | Trạng thái | Bằng chứng / ghi chú |
| --- | --- | --- |
| MyStore + sáu bảng | PASS (verified) | Đã query lại trực tiếp `(localdb)\MSSQLLocalDB`; MyStore có `Category`, `Customer`, `Order`, `OrderDetail`, `Product`, `User`. Khi LocalDB mất catalog registration nhưng file MyStore còn tồn tại, database được attach lại an toàn, không ghi đè dữ liệu. |
| PK/FK và `ProductDecription` | PASS (verified) | Direct audit trả về 6 PK, 5 FK nguồn và `ProductDecription NVARCHAR(MAX) NOT NULL`. Không chạy `Optional_ChangeOwner.sql`. Xem `Documentation/SQL_AUDIT.md`. |
| EF6 DB First EDMX | PASS (verified) | Visual Studio đã sinh `Models/Model1.edmx`, T4 entity classes và `MyStoreEntities`. Model name thực tế là `Model1`, giữ nguyên để metadata resource khớp. Runtime query qua EntityClient PASS. Xem `Documentation/EDMX_AUDIT.md`. |
| Area Admin + route | PASS (verified) | `AdminAreaRegistration`, `Admin/HomeController`, View Index, `_AdminLayout` và Views `Web.config` đã thêm. Sau khi giới hạn default route ở `demoBE.Controllers`, `GET /Admin/Home/Index` và `GET /Home/Index` đều trả HTTP 200. |
| Update EDMX | BLOCKED | Checkpoint hoàn nguyên (`Database/EDMX_Update_Probe_Add.sql`, `..._Revert.sql`) và click-path tại `Documentation/EDMX_UPDATE_CHECKPOINT.md` đã sẵn sàng. Không có native Visual Studio/EF6 Designer surface để thực hiện wizard và xác minh property sinh ra; hai script chưa chạy, nên không suy diễn PASS/build-after-update. |

## BE2

| Hạng mục P0 | Trạng thái | Bằng chứng / ghi chú |
| --- | --- | --- |
| Category CRUD, partial view, SQL runtime test | PASS (verified) | `CategoriesController` dùng `MyStoreEntities.Category`; views Index/Details/Create/Edit/Delete + `PVCreateEditCategory`; AntiForgeryToken, validation, 404, FK protection và CRUD LocalDB đều PASS. |
| Admin layout | PASS (verified) | `_AdminLayout.cshtml` áp dụng cho Admin Home, Category và Product views; runtime routes trả HTTP 200. |

## BE3

| Hạng mục P0 | Trạng thái | Bằng chứng / ghi chú |
| --- | --- | --- |
| Product CRUD, Category dropdown, partial view, SQL runtime test | PASS (verified) | `ProductsController` dùng `Include(product => product.Category)`, dropdown DB, `PVCreateEditProduct`, `PVProductInfo`, AntiForgeryToken, validation, 404, FK protection và CRUD LocalDB đều PASS. |

## Đầu vào cần để tiếp tục

1. Mở `demoBE.slnx` trong Visual Studio, thực hiện checkpoint
   `Documentation/EDMX_UPDATE_CHECKPOINT.md`, rồi lưu `Models/Model1.edmx` và
   gửi kết quả build sau bước thêm/gỡ probe. Đây là điều kiện còn thiếu duy
   nhất để xác minh Update Model from Database thực tế.
2. Nếu muốn publish GitHub, cung cấp URL repository đích và branch được phép
   dùng. Hiện chưa có remote, commit hoặc push.

## Git

- Git repository cục bộ đã khởi tạo trên branch `main`; `git status --ignored`
  xác minh `.gitignore` loại `.vs`, `bin`, `obj`, package cache, LocalDB và
  user settings. Scoped `git clean` đã dọn `demoBE/bin` và `demoBE/obj`.
- Query LocalDB sau cleanup trả về 0 hàng cho cả sáu bảng. Không có remote,
  commit hoặc push; chờ repository đích/branch từ người dùng.

## File đã thêm/sửa

- `Database/Create_MyStore.sql`, `Database/Optional_ChangeOwner.sql`, `Database/Sample_Data.sql`, `Database/Attach_Orphaned_MyStore_LocalDB.sql`, `Database/EDMX_Update_Probe_Add.sql`, `Database/EDMX_Update_Probe_Revert.sql`
- `demoBE/Areas/Admin/*`, `demoBE/App_Start/RouteConfig.cs`, `demoBE/Models/Model1.*`, `demoBE/Models/Category.cs`, `demoBE/Models/Product.cs`, `demoBE/Models/Customer.cs`, `demoBE/Models/Order.cs`, `demoBE/Models/OrderDetail.cs`, `demoBE/Models/User.cs`
- `demoBE/packages.config`, `demoBE/demoBE.csproj`
- `README.md`, `.gitignore`, `Documentation/BE1_Update_EDMX.md`, `Documentation/EDMX_UPDATE_CHECKPOINT.md`, `Documentation/OPTIONAL_SCOPE.md`, `Documentation/SQL_AUDIT.md`, `Documentation/EDMX_AUDIT.md`, `Documentation/TEST_REPORT.md`
