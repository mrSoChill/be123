# BE5 và BE6 — Customer Site, Product Detail, Session Cart

Project giữ nguyên ASP.NET MVC 5.2.9, .NET Framework 4.7.2, Entity Framework
6 Database First và SQL Server. Không tạo project mới, không đổi EDMX hoặc
entity do EF sinh.

## Yêu cầu

- Visual Studio 2022/2026 với workload ASP.NET and web development.
- .NET Framework 4.7.2 Developer Pack.
- SQL Server/LocalDB có database `MyStore` đúng với connection string
  `MyStoreEntities` trong `demoBE/Web.config`.
- NuGet packages theo `packages.config`, gồm EF6 và PagedList.

Connection string hiện dùng Windows Integrated Security cho `(localdb)\MSSQLLocalDB`.
Không lưu SQL password vào repository. Trên máy khác, thay đổi connection string
theo instance MyStore của máy đó nhưng không commit thông tin xác thực.

## Mở và build

Mở `demoBE.slnx` trong Visual Studio, Restore NuGet nếu IDE yêu cầu, sau đó
Build Solution. Có thể dùng Developer PowerShell:

```powershell
& 'C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe' .\demoBE.slnx /t:Restore,Build /p:Configuration=Debug /p:MvcBuildViews=true /m
```

## Kiểm tra BE5

- `/Home/Index`: customer home, search theo tên/mô tả/danh mục, 10 sản phẩm
  nổi bật theo tổng `OrderDetail.Quantity`, danh sách tối đa 20 sản phẩm ít mua
  nhất với phân trang 6 item.
- `/Home/ProductList?categoryId={id}`: sản phẩm thuộc một danh mục, search,
  sort và pagination.
- `/Home/ProductDetail/{id}`: chi tiết, lượng đã bán, số lượng mua, tạm tính,
  tối đa 8 sản phẩm tương tự và Top Deals theo danh mục.
- Metadata/partial classes cho `Category`, `Customer`, `Order`, `OrderDetail`,
  `Product`, `User` nằm ở `Models/Metadata/EntityMetadata.cs`.
- `_CustomerLayout`, menu danh mục DB, badge số lượng giỏ và fallback ảnh nằm
  trong `Views/Shared` và `Content`.

Lưu ý: bảng Product không có trường ngày tạo. Theo slide bài tập, nhãn “Sản
phẩm mới” sử dụng thứ tự sản phẩm có lượng mua ít nhất, không bịa thêm dữ liệu
ngày tạo.

## Kiểm tra BE6

- `/Cart`: giỏ cơ bản trong `Session["Cart"]`, gộp item trùng ProductID.
- `/Cart/Index2`: nhóm item theo tên danh mục và đề xuất sản phẩm cùng danh mục
  từ database, không gợi ý item đã có trong giỏ, phân trang 6 item.
- POST `AddToCart`, `UpdateQuantity`, `UpdateSelection`, `RemoveFromCart`,
  `ClearCart` đều có AntiForgeryToken, PRG và kiểm tra quantity 1–99.
- Giá/tên/ảnh/danh mục ở thời điểm thêm giỏ luôn lấy lại từ `db.Product`; client
  chỉ gửi ProductID và Quantity.
- `/Cart/Checkout` chỉ là thông báo/tóm tắt. Thanh toán, tạo Order và xác nhận
  đơn hàng thuộc bài sau nên trang này không ghi dữ liệu SQL.

## Giới hạn và bảo vệ dữ liệu

Không có ảnh sản phẩm trong dữ liệu nguồn sẽ hiển thị
`Content/images/product-placeholder.svg`. Không có danh mục/sản phẩm hardcode.
Giỏ hàng không tạo bảng SQL và mỗi browser session có giỏ riêng. Không push Git
từ project này trừ khi có chỉ định repository/branch riêng.
