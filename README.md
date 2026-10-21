# demoBE — ASP.NET MVC5 / EF6 Database First

Thực hành BE1--BE3 quản trị `Category` và `Product` trên ASP.NET MVC5 (.NET
Framework 4.7.2), Entity Framework 6 và SQL Server.  Đây không phải ASP.NET
Core và không dùng EF Core.

## Trạng thái hiện tại

Database demo `MyStore` đã được tạo và kiểm chứng trên `(localdb)\MSSQLLocalDB`.
EF6 Database First đã được tạo bằng Designer ở `Models/Model1.edmx`, với
context `MyStoreEntities`; Category/Product CRUD đã được kiểm thử trên LocalDB.
Xem `Documentation/STATUS.md` để biết các hạng mục còn bị chặn.

## Yêu cầu máy chạy

- Visual Studio 2026 Community (hoặc Visual Studio có workload ASP.NET and web
  development và Entity Framework Tools).
- .NET Framework 4.7.2 Developer Pack.
- SQL Server Database Engine hoặc SQL Server LocalDB.
- SSMS là tùy chọn nhưng được khuyến nghị để chạy/kiểm tra script.

## 1. Khởi tạo database

1. Mở SSMS và kết nối instance SQL Server được phép. Máy audit dùng
   `(localdb)\MSSQLLocalDB` với Windows Authentication.
2. Mở và chạy `Database/Create_MyStore.sql`. Script chỉ tạo `MyStore` và các
   bảng còn thiếu; không có `DROP`, `ALTER` hay seed dữ liệu.
3. Không chạy `Database/Optional_ChangeOwner.sql` trừ khi giảng viên yêu cầu
   rõ và bạn có quyền phù hợp.
4. Chỉ dùng `Database/Sample_Data.sql` trên database demo đã được cho phép.

## 2. EF6 Database First model

1. Trong Visual Studio, mở `demoBE.slnx`; Restore NuGet packages nếu IDE yêu
   cầu. Bảo đảm gói `EntityFramework` 6.5.1 được restore.
2. Project hiện dùng model Designer sinh là `Models/Model1.edmx`; không đổi tên
   thủ công vì connection string đang tham chiếu `Models.Model1.csdl/.ssdl/.msl`.
3. Nếu phải tạo lại model, nhấp phải thư mục `Models` → **Add** → **New
   Item...** → **ADO.NET Entity Data Model**, đặt tên khớp model đang dùng hoặc
   đồng bộ toàn bộ metadata/connection string sau đó.
4. Chọn server và database `MyStore`; dùng Windows Authentication hoặc thông
   tin do quản trị viên cấp, không lưu password vào source control.
5. Chọn toàn bộ sáu bảng: `User`, `Customer`, `Category`, `Product`, `Order`,
   `OrderDetail`. Giữ tùy chọn lưu connection setting; đặt tên theo wizard
   (thường là `MyStoreEntities`).
6. Hoàn tất wizard, lưu EDMX, rồi Build solution. Xác minh entity `Product`
   có thuộc tính `ProductDecription` và navigation tới `Category`.

> Không tự tạo hoặc sửa XML `.edmx` để thay thế wizard. Chi tiết audit model
> hiện có ở `Documentation/EDMX_AUDIT.md`.

## 3. Cấu hình connection string

Wizard ghi connection string vào `demoBE/Web.config`. Ví dụ an toàn cho LocalDB
(không có password):

```xml
<add name="MyStoreEntities"
     connectionString="metadata=res://*/Models.Model1.csdl|res://*/Models.Model1.ssdl|res://*/Models.Model1.msl;provider=System.Data.SqlClient;provider connection string=&quot;data source=(LocalDB)\MSSQLLocalDB;initial catalog=MyStore;integrated security=True;encrypt=False;MultipleActiveResultSets=True;App=EntityFramework&quot;"
     providerName="System.Data.EntityClient" />
```

Tên `MyStoreEntities` và metadata trong ví dụ khớp model hiện tại. Nếu tạo lại
model với tên khác, phải để wizard sinh connection string rồi kiểm tra lại.

## 4. Build và chạy

Từ Developer PowerShell for Visual Studio:

```powershell
& 'C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe' .\demoBE.slnx /t:Restore,Build /p:Configuration=Debug
```

Chạy với IIS Express từ Visual Studio. Khi BE2/BE3 được hoàn tất, trang admin
sẽ ở `/Admin/Home/Index`; trang mặc định vẫn là `/Home/Index`.

## Cập nhật EDMX

Xem hướng dẫn chi tiết ở `Documentation/BE1_Update_EDMX.md`. Checkpoint có
thể hoàn nguyên, kèm script thêm/gỡ cột probe, nằm ở
`Documentation/EDMX_UPDATE_CHECKPOINT.md`. Chỉ cập nhật từ database demo đã
được phép thay đổi; không sửa XML EDMX bằng tay.

## 5. Kiểm thử chức năng đã có

- **BE1:** truy cập `/Home/Index` và `/Admin/Home/Index`.
- **BE2:** `/Admin/Categories` cho Index, Details, Create, Edit và Delete.
  Xóa Category đang có Product sẽ nhận thông báo bảo vệ khóa ngoại.
- **BE3:** `/Admin/Products` cho CRUD Product; Create/Edit lấy Category từ SQL
  Server và Product Details/Index hiển thị Category liên quan.

Các kiểm thử CRUD LocalDB đã thực hiện và bằng chứng nằm ở
`Documentation/TEST_REPORT.md`. Bản database demo hiện không chứa dữ liệu
thử; `Database/Sample_Data.sql` chỉ dùng khi bạn chủ động muốn thêm dữ liệu
mẫu trên database đã được phép.

## 6. Git và dữ liệu nhạy cảm

Repository Git cục bộ đã được khởi tạo nhưng chưa có remote, commit hoặc push.
`.gitignore` loại `.vs`, `bin`, `obj`, package cache, LocalDB files, publish
settings và user settings. `Web.config` dùng Windows Integrated Security cho
LocalDB và không chứa mật khẩu. Chỉ thêm remote/push sau khi có repository đích
và branch được chấp thuận.
