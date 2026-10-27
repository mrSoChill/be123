# BE4 Test Report — Admin Product Search, Sorting & Pagination

Ngày kiểm tra: 27/10/2026

## Thay đổi thực hiện

- Thêm `ProductSearchVM` để chứa điều kiện tìm kiếm, khoảng giá, sắp xếp và `IPagedList<Product>`.
- Cập nhật `Admin/ProductsController.Index` dùng `IQueryable`, lọc theo tên/mô tả/danh mục và khoảng giá trước khi sắp xếp, đếm và phân trang 10 bản ghi/trang.
- Hỗ trợ `name_asc`, `name_desc`, `price_asc`, `price_desc`, có `ProductID` làm tiebreaker ổn định.
- Chặn giá âm, khoảng giá ngược và giá không phải số bằng thông báo tiếng Việt; URL trang không hợp lệ, không dương hoặc vượt giới hạn được đưa về trang an toàn.
- Thêm form GET Bootstrap, trạng thái rỗng, bảng danh sách tách riêng trong `PVListProduct.cshtml`, và pager Bootstrap giữ toàn bộ bộ lọc/sắp xếp.
- Cài `PagedList` 1.17.0.0 và `PagedList.Mvc` 4.5.0.0 từ nuget.org; khai báo package và assembly reference trong project kiểu cũ.

## Kết quả build

Đã chạy thành công:

```powershell
& 'C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe' .\demoBE\demoBE.csproj /t:Build /p:Configuration=Debug /p:MvcBuildViews=true /v:minimal
```

Kết quả: `demoBE -> ...\demoBE\bin\demoBE.dll`, không có lỗi. `MvcBuildViews=true` đã biên dịch Razor Views, gồm `Index.cshtml` và `PVListProduct.cshtml`.

Lưu ý: lần thử bằng `dotnet msbuild` không phù hợp với dự án ASP.NET MVC5 kiểu cũ vì thiếu `Microsoft.WebApplication.targets`; build xác nhận ở trên dùng MSBuild Visual Studio có WebApplication targets.

## Kiểm tra runtime đã chạy

Ứng dụng được chạy bằng IIS Express với LocalDB `MSSQLLocalDB` và kiểm tra HTTP thực tế tại `/Admin/Products`.

| Ca kiểm tra | Kết quả |
| --- | --- |
| Danh sách mặc định | HTTP 200, không có Server Error |
| Từ khóa + `name_desc` | HTTP 200, không có Server Error |
| Khoảng giá + `price_asc` | HTTP 200, không có Server Error |
| `minPrice=-1` | HTTP 200, hiển thị validation theo trường |
| `minPrice=200&maxPrice=100` | HTTP 200, hiển thị validation tổng quát |
| `minPrice=abc` | HTTP 200, hiển thị validation theo trường và giữ giá trị đã nhập |
| `page=0`, `page=-1`, `page=2147483647` | HTTP 200, không có Server Error |
| Giữ điều kiện tìm kiếm/sắp xếp | HTML phản hồi giữ `searchTerm`, `minPrice`, `maxPrice`, `sortOrder` |
| `GET /Admin/Products/Create` | HTTP 200, không có Server Error |
| `GET /Admin/Categories` | HTTP 200, không có Server Error |

## Chưa thể xác thực runtime đầy đủ

LocalDB hiện trả về 0 sản phẩm. Vì không tự ý chèn/sửa dữ liệu dự án, các ca sau chưa thể xác nhận bằng dữ liệu thật:

- kết quả khớp riêng theo tên, mô tả và danh mục;
- thứ tự bốn chế độ sort trên nhiều sản phẩm, bao gồm tiebreaker;
- chuyển trang 1 → 2 → 1, số lượng tối đa 10 bản ghi/trang, trang cuối và giữ query string trên các link pager;
- Details, Edit và Delete của một sản phẩm hiện có.

Để nghiệm thu các ca trên, chủ dự án cần có ít nhất 11 sản phẩm với tên/giá trùng và khác nhau trong database `MyStore`, sau đó mở `/Admin/Products` và chạy các URL/filter tương ứng. Không có database schema nào bị thay đổi trong BE4.
