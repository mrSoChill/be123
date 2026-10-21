# Checklist nghiệm thu BE 1–3 — P0

Trạng thái hợp lệ: `TODO`, `IMPLEMENTED`, `PASS (verified)`, `BLOCKED`, `FAIL`. Chỉ ghi PASS nếu có bằng chứng kiểm thử ở `Documentation/STATUS.md`.

## BE1
- [x] PASS (verified) — Project .NET Framework ASP.NET MVC5, không phải ASP.NET Core (ghi csproj TargetFrameworkVersion và package MVC).
- [x] PASS (verified) — EF6 6.5.1 có trong `packages.config`; MSBuild `/t:Restore` hoàn tất mã thoát 0, không có package còn thiếu, và build thành công.
- [x] PASS (verified) — MyStore tồn tại trên `(localdb)\MSSQLLocalDB`; có đúng 6 bảng User, Customer, Category, Product, Order, OrderDetail (query sys.tables).
- [x] PASS (verified) — PK/FK theo schema nguồn, cột `ProductDecription` không bị tự ý sửa (query sys.columns/sys.foreign_keys).
- [x] PASS (verified) — Model EDMX do EF6 Designer sinh, DbContext/Entity classes/navigation hoạt động tại runtime; xem `Documentation/EDMX_AUDIT.md`.
- [x] PASS (verified) — Area Admin và route `/Admin/Home/Index`; route `/Home/Index` đồng thời hoạt động.
- [ ] BLOCKED — Checkpoint hoàn nguyên đã sẵn sàng tại `Documentation/EDMX_UPDATE_CHECKPOINT.md`, nhưng chưa thể thực hiện/kiểm chứng wizard **Update Model from Database** vì không có cửa sổ Visual Studio Designer khả dụng trong phiên audit.

## BE2
- [x] PASS (verified) — Có `_AdminLayout.cshtml` được các View Admin sử dụng.
- [x] PASS (verified) — Category Index liệt kê dữ liệu SQL Server thật trong CRUD test.
- [x] PASS (verified) — Category Details theo ID có xử lý 404/not-found.
- [x] PASS (verified) — Category Create GET/POST lưu bản ghi hợp lệ trong SQL.
- [x] PASS (verified) — Category Edit GET/POST cập nhật bản ghi trong SQL.
- [x] PASS (verified) — Category Delete GET/POST xóa khi an toàn và báo lỗi thân thiện khi Product tham chiếu.
- [x] PASS (verified) — Partial View dùng chung Create/Edit.
- [x] PASS (verified) — UI Category Việt hóa/Bootstrap, validation hoạt động, mọi POST có AntiForgeryToken.

## BE3
- [x] PASS (verified) — Product Index hiển thị CategoryName đúng qua `Include` (Product + Category).
- [x] PASS (verified) — Product Details theo ID; xử lý null/not-found.
- [x] PASS (verified) — Product Create GET/POST có dropdown Category từ DB và lưu FK CategoryID đúng.
- [x] PASS (verified) — Product Edit GET/POST giữ Category đã chọn và lưu thay đổi.
- [x] PASS (verified) — Product Delete GET/POST xử lý lỗi FK/khóa ngoại thực tế.
- [x] PASS (verified) — Form Product sai dữ liệu vẫn hiển thị dropdown Category và lỗi tương ứng.
- [x] PASS (verified) — Partial View chung Create/Edit và Details/Delete, không duplicate markup lớn.
- [x] PASS (verified) — Giao diện Product thống nhất, Việt hóa trường/nút và hiển thị `ProductDecription`.

## Build, GitHub và báo cáo
- [x] PASS (verified) — MSBuild `/t:Restore` hoàn tất mã thoát 0 (không có package cần restore) và solution build PASS với Razor view compilation.
- [x] PASS (verified) — IIS Express smoke/CRUD test PASS; evidence trong `Documentation/TEST_REPORT.md`.
- [x] PASS (verified) — README có prerequisites, DB/EDMX/build/run và không hardcode bí mật.
- [x] PASS (verified) — Git repository cục bộ đã khởi tạo; `git status --ignored` xác minh `.gitignore` loại `.vs`, `bin`, `obj`, package cache và user settings. `bin/obj` đã được dọn bằng scoped `git clean`.
- [x] PASS (verified) — Không có Git remote, commit hoặc push; chờ repository đích/branch do người dùng cung cấp.

## Test scenarios tối thiểu
1. Tạo Category A → đọc danh sách → sửa thành B → xem Details → xóa khi không có Product tham chiếu.
2. Tạo Product thuộc Category B → kiểm tra Index/Details hiển thị danh mục → sửa giá/tên/danh mục → kiểm tra SQL → xóa Product.
3. Thử xóa Category đang chứa Product: không crash, có thông báo và vẫn giữ dữ liệu.
4. Thử submit Product/Category thiếu dữ liệu bắt buộc: validation xuất hiện; không lưu bản ghi rác.
5. Truy cập `/Admin/Home/Index` và `/Home/Index`: không ambiguous route.
6. Kiểm tra EDMX update sau thay đổi schema trong database thử nghiệm (nếu cần thao tác Visual Studio GUI, không suy diễn kết quả).
