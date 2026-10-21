# Quy tắc làm việc cho Codex — LTW / BE1–BE3

Bạn đang làm việc trên **solution ASP.NET Web Application (.NET Framework)** vừa được tạo bằng Visual Studio 2026. Đọc `CODEX_PLAN.md`, `ACCEPTANCE.md` và tất cả tài liệu trong `References/` trước khi sửa mã nguồn.

## Ưu tiên nguồn
1. Nội dung bài giao trực tiếp qua LMS, ảnh trong `References/DeBai/` (BE1, BE2, BE3).
2. PowerPoint giảng viên trong `References/` (phần Project Setup và Admin Site).
3. Script `References/database2024.txt` (nguồn schema gốc).
4. 11 video trong `References/videos.txt`, **chỉ nhận xét những thao tác thực sự kiểm chứng được** (nếu không truy cập được video/transcript, ghi rõ chưa kiểm chứng).
5. Giải pháp kỹ thuật chuẩn MVC5/EF6 cho các chi tiết mà tài liệu không quy định.

## Ràng buộc không được vi phạm
- ASP.NET **MVC5 trên .NET Framework**, **Entity Framework 6**, **SQL Server**, **DB First bằng EDMX**; tuyệt đối KHÔNG đổi sang ASP.NET Core, EF Core hoặc Code First.
- Giữ nguyên tên project/solution/namespace hiện có, TRỪ KHI người dùng yêu cầu đổi. Framework giữ nguyên phiên bản đang dùng nếu phù hợp MVC5; không nâng cấp tự động.
- Không xóa, di chuyển hay ghi đè project cũ / Lab02 / Lab03. Không tự tạo project thay thế khi solution MVC5 đã tồn tại.
- Giữ bản `References/database2024.txt` nguyên trạng. **Cột thực tế viết `ProductDecription`**, không tự sửa thành `ProductDescription`. Nếu muốn đổi, trình bày đề xuất/migration riêng và chờ duyệt.
- Không sửa, xóa, DROP, ALTER hay seed database thật khi chưa kiểm tra chính xác instance/database và được phép thực thi. Không dùng `sa`/quyền cao hơn để vượt qua lỗi quyền; lệnh `ALTER AUTHORIZATION ... TO sa` trong tài liệu cần được kiểm tra điều kiện.
- Không hardcode password, token hoặc thông tin xác thực SQL Server vào repo. `Web.config` mẫu chỉ nên dùng LocalDB/Windows integrated security hoặc placeholder không chứa bí mật.
- Không được **chế tạo file `.edmx` cho có** rồi báo hoàn tất DB First. EDMX phải sinh/kiểm chứng bằng EF6 Designer từ SQL Server; nếu không có công cụ UI, để checkpoint rõ ràng cho người dùng và tiếp tục phần code chỉ nếu có Model hợp lệ.
- Không tuyên bố PASS cho phần chưa build/chưa chạy trên SQL Server. Phân biệt rõ `implemented`, `build verified`, `runtime verified`, `blocked`.
- Không tự ý thêm Authentication/Authorization hoặc mở rộng Customer/Order/Cart/Payment — đó là tính năng tương lai, trừ khi bài giao thực sự bắt buộc.
- Chỉ commit/push GitHub sau khi người dùng đã đồng ý nhánh và repo; không lưu bí mật hoặc file build tạm (bin/obj/.vs/packages nếu được restore) lên repo.

## Phương pháp
- Audit read-only môi trường, solution, .NET Framework, Visual Studio/MSBuild, SQL Server, NuGet, file tham khảo trước.
- Đưa ra plan ngắn với rủi ro/thiếu đầu vào, sau đó thực hiện BE1 → BE2 → BE3, không dừng nếu việc còn lại có thể làm mà không vi phạm các checkpoint.
- Sau mỗi BE: build, smoke tests, chụp lại bằng chứng/lệnh/kết quả, cập nhật `Documentation/STATUS.md` với PASS/FAIL/BLOCKED.
- Khi gặp blocker buộc phải thao tác GUI như EDMX Wizard, báo chính xác click-path và đầu ra phải gửi lại để tiếp tục; không đoán rằng thao tác đã xảy ra.
- Chỉ kết luận hoàn thành khi tất cả mục P0 ở `ACCEPTANCE.md` đã PASS; nếu có BLOCKED thì nêu chính xác.
