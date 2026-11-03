# Báo cáo kiểm thử BE5–BE6

Ngày chạy: 2026-11-03, LocalDB `MyStore`, IIS Express port 51255.

| Hạng mục | Kết quả thực tế |
| --- | --- |
| MSBuild Restore + Build + Razor (`MvcBuildViews=true`) | PASS |
| Customer Home, search tên/mô tả/danh mục | PASS qua HTTP 200 với dữ liệu SQL test |
| ProductList sort/filter và ProductDetail | PASS qua HTTP 200 |
| ProductDetail thiếu/sản phẩm thiếu, danh mục thiếu | PASS: HTTP 404 |
| POST ProductDetail thêm giỏ, anti-forgery | PASS |
| Gộp item trùng, UpdateQuantity, quantity 0 | PASS: quantity giữ nguyên khi input 0 |
| Chọn/bỏ chọn, tổng tiền, Remove và Clear | PASS |
| Session isolation | PASS: session mới có cart count 0 khi session thứ nhất có 1 item |
| Cart Index2 nhóm danh mục và gợi ý loại trừ item đã chọn | PASS |
| GET AddToCart | PASS: HTTP 404, không đổi state |
| `/Admin/Categories`, `/Admin/Products` | PASS: HTTP 200 |
| CSS/placeholder asset | PASS: HTTP 200 |

Dữ liệu kiểm thử dùng tiền tố `__BE56_RuntimeTest_` và user
`__BE56_RuntimeUser_20261103`; đã xóa trong transaction sau test và query xác
nhận 0 Category/Product/User/Customer mang tiền tố này. Checkout không tạo
Order hoặc thanh toán, đúng giới hạn bài BE5–BE6.
