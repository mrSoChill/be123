# Implementation Specification — BE1, BE2, BE3 (ASP.NET MVC5 / EF6 Database First)

## Bối cảnh
Người dùng đã tạo solution mới từ `ASP.NET Web Application (.NET Framework)` trong Visual Studio 2026, template **MVC** và **No Authentication**. Đây là project C# cho BE 1–3; không phải project ASP.NET Core của Lab02/03. **Tên thực tế trên đĩa là nguồn sự thật** — không tự đổi thành tên gợi ý từ hội thoại.

Đọc toàn bộ `AGENTS.md`, `ACCEPTANCE.md`, PowerPoint, file SQL và `References/videos.txt`. Chỉ có ảnh tiêu đề video không đủ chứng minh đã xem nội dung phát. Nếu không xem được, tuân theo đề LMS + PowerPoint và báo khoảng trống.

## Bước 0 — Audit trước khi code
1. Tìm `.sln`/`.csproj`, xác nhận loại project là ASP.NET MVC5 `.NET Framework` (không phải SDK-style ASP.NET Core). Ghi target framework, NuGet packages, config, tên assembly/namespace, đường dẫn project.
2. Kiểm tra công cụ build của Visual Studio 2026, SDK .NET Framework, EF6 Designer availability, local SQL Server instance/SSMS. Chỉ sử dụng lệnh kiểm tra, không tạo DB hoặc thay đổi dự án ở bước này.
3. So sánh nguồn PPTX, SQL, LMS images với trạng thái thực tế. Xuất bảng gap theo BE1–BE3; chỉ triển khai sau khi xác định project chuẩn.

## BE1 — 4 video #1–#4: project setup (P0)
### BE1.1 Database MyStore
- Giữ schema gốc ở `References/database2024.txt` gồm `User`, `Customer`, `Category`, `Product`, `Order`, `OrderDetail`; toàn bộ PK/FK. Tên cột Product **`ProductDecription`** là typo có thật của nguồn và phải khớp với thực tế.
- Tạo `Database/Create_MyStore.sql`: gồm `IF DB_ID(N'MyStore') IS NULL CREATE DATABASE...`, `USE MyStore` và SQL tạo bảng từ nguồn, kèm điều kiện / chú thích rõ lần chạy. Không tự DROP database / dữ liệu cũ.
- **Lưu ý:** tập tin SQL nguồn kết thúc bằng `ALTER AUTHORIZATION ON DATABASE::MyStore TO sa;`. Đây không phải bước luôn cần và có thể yêu cầu quyền cao; tách ra thành `Database/Optional_ChangeOwner.sql` hoặc ghi chú chỉ thực thi khi giảng viên/máy thực tế yêu cầu và có quyền. Không tự chạy lệnh này.
- Tạo `Database/Sample_Data.sql` riêng, chỉ dùng khi được phép trên DB demo, hỗ trợ CRUD thử nghiệm; tránh dữ liệu sản xuất.
- Chạy/kiểm thử tạo DB bằng SQL Server được chỉ định **nếu có kết nối và được phép**; xác minh `sys.tables`, FK và truy vấn một vài hàng. Nếu không, ghi blocker và lệnh chính xác cho người dùng.
### BE1.2 MVC5 + EF6 DB First
- Restore MVC5 NuGet, cài EntityFramework 6 nếu chưa có.
- Tạo `Models/MyStoreModel.edmx` **qua ADO.NET Entity Data Model → EF Designer from database** với SQL Server database `MyStore`, entity names đúng, navigation `Product.Category` / `Category.Products` nếu sinh ra.
- Kiểm tra DbContext thực tế được sinh bởi EDMX (không giả định tên). Connection string cấu hình phù hợp và không lộ mật khẩu.
- Nếu Codex không có khả năng điều khiển EF Designer UI, dừng ở checkpoint DBFirst và đưa click-path để user tạo `.edmx` rồi audit các file sinh ra. Không tự biên thủ công một `.edmx` tượng trưng.
### BE1.3 Area Admin
- Thêm Area `Admin`, `AdminAreaRegistration`, `Areas/Admin/Controllers/HomeController.cs`, `Areas/Admin/Views/Home/Index.cshtml`.
- Routes `/Admin/Home/Index` hoạt động; route mặc định `/Home/Index` không xung đột namespace/HomeController. Không gắn authentication giả khi No Authentication.
### BE1.4 DB → Model update
- Tạo hướng dẫn thực hành `Documentation/BE1_Update_EDMX.md`: thao tác thêm/sửa field trên DB demo, click `MyStoreModel.edmx` → Update Model from Database, build/kiểm chứng.
- Nếu làm demo schema change: SQL migration riêng, không mutate DB đang dùng nếu chưa được phép. Báo rõ phần demo GUI đã/chưa thực hiện.

## BE2 — 4 video #5–#8: Admin Category từ 1 Model (P0)
1. `Areas/Admin/Views/Shared/_AdminLayout.cshtml`; Header, menu Admin, Footer; dùng ở Admin views, không làm ảnh hưởng root layout.
2. `Areas/Admin/Controllers/CategoriesController.cs` EF6 `System.Data.Entity`, thực hiện: `Index GET`, `Details GET`, `Create GET/POST`, `Edit GET/POST`, `Delete GET/POST`; lưu SQL Server qua `SaveChanges()`, kiểm tra tồn tại/id/null hợp lý.
3. Views `Index`, `Details`, `Create`, `Edit`, `Delete` tại `Areas/Admin/Views/Categories/`; form POST có AntiForgeryToken, kiểm tra ModelState; tránh overposting khi bind; không xóa category khi FK Product đang tham chiếu mà làm crash không kiểm soát.
4. Partial View cho nội dung chung form Create/Edit — ví dụ `PVCreateEditCategory.cshtml` — không nhân đôi form.
5. Tối ưu code chung Controller hợp lý; không ép gọi Action MVC trực tiếp nếu tạo helper/private method rõ hơn.
6. Việt hóa nhãn/nút/cột theo mẫu, style Bootstrap, thể hiện thông báo validate, dùng đúng `_AdminLayout`.

## BE3 — 3 video #9–#11: Admin Product từ Product + Category (P0)
1. `Areas/Admin/Controllers/ProductsController.cs`: `Index GET`, `Details GET`, `Create GET/POST`, `Edit GET/POST`, `Delete GET/POST` bằng EF6 DBFirst.
2. `Index` lấy Product cùng Category qua `Include(p => p.Category)` nếu navigation được EDMX sinh đúng; nếu khác, dùng tên navigation thực tế. Không query N+1 không cần thiết.
3. Form Create và Edit có dropdown danh mục từ `db.Categories` (`SelectList` hoặc view model phù hợp), lưu `CategoryID`; khi ModelState invalid phải populate dropdown lại. Đảm bảo CategoryID hợp lệ (FK).
4. Views `Index`, `Details`, `Create`, `Edit`, `Delete`; Partial `PVCreateEditProduct.cshtml` và `PVProductInfo.cshtml` dùng lại phần nội dung chính; tất cả form POST có AntiForgeryToken, MVC validation, không overposting.
5. Form/Index hiển thị đúng `ProductName`, `ProductDecription`, `ProductPrice`, `ProductImage`, `Category.CategoryName`; hiển thị tên cột/nút thân thiện bằng tiếng Việt; Bootstrap giao diện nhất quán.
6. Phải chạy thử Create/Read/Update/Delete trên SQL Server DB demo và kiểm tra quan hệ Category/Product, lỗi input sai, lỗi FK; DB production không bị dùng thử.

## Hạng mục ngoài phạm vi P0 — chỉ làm khi xác minh cô yêu cầu chính thức
PowerPoint `Web BE Development_p2 Admin site.pptx` còn các phần: upload ảnh Product vào `Content/images`, Search theo keyword/giá, Sort, Pagination (`PagedList.Mvc`), Customer/Order/Admin account. Bài LMS BE3 ảnh user gửi chỉ hiển thị video #9, #10, #11 cho **Product CRUD + View customization**. Hãy ghi chúng trong `Documentation/OPTIONAL_SCOPE.md` theo mức ưu tiên, KHÔNG tự triển khai Customer/Order/Cart/Payment. Nếu evidence từ video #9–#11 yêu cầu upload, đưa mục đó vào P0 và mô tả evidence.

## Hoàn tất
- Khôi phục gói NuGet và build bằng Visual Studio/MSBuild (.NET Framework), không dùng `dotnet run` như ASP.NET Core.
- Test trên IIS Express / browser, với SQL Server được phép truy cập; nếu thiếu môi trường, nêu blocker chi tiết thay vì gắn PASS.
- `README.md`: prerequisites, VS 2026, SQL instance, cách chạy Database/Create_MyStore.sql, EDMX wizard/refresh, cấu hình connection string (không mật khẩu thật), build/run, route admin, test.
- `Documentation/STATUS.md` ghi BE1/BE2/BE3 từng điều kiện, evidence, blocker, thay đổi file.
- `.gitignore` loại `.vs/`, `bin/`, `obj/`, build artifacts, user-specific config và credentials; không lưu credentials SQL / tài khoản thật. Chỉ push khi user cấp repo/đồng ý.
- Không đặt tất cả là DONE nếu chưa có SQL DB thật, EDMX thật, build và runtime tests.
