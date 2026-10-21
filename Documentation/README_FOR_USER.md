# Cách dùng gói bàn giao

1. Trong Visual Studio 2026 hoàn tất tạo project theo template MVC (không phải Empty/Web Forms), chọn No Authentication, sau đó thử `Ctrl + F5`.
2. Giải nén **nội dung** gói ZIP này vào thư mục gốc chứa file `.sln` của solution MVC5 mới. Khi đó `AGENTS.md`, `CODEX_PLAN.md`, `ACCEPTANCE.md` nằm cạnh solution.
3. Mở Codex trên máy có source. Chọn **thư mục gốc chứa file `.sln`**, không chỉ chọn thư mục con Controllers/ hoặc Views/.
4. Gửi Codex câu lệnh:

> Đọc AGENTS.md, CODEX_PLAN.md, ACCEPTANCE.md và toàn bộ References. Audit read-only solution/môi trường. Lập gap analysis BE1–BE3; sau đó triển khai toàn bộ P0 tuần tự BE1 → BE2 → BE3 theo source, tự build/test sau mỗi giai đoạn. Không chuyển công nghệ, không tự sửa schema, không chế EDMX, không chạy SQL thay đổi dữ liệu thật nếu chưa được phép. Ghi STATUS.md và nêu blocker cụ thể cần tôi thao tác; chưa được push GitHub trước khi tôi xác nhận repo.

5. Nếu Codex không thể sinh EDMX qua Wizard trên máy, yêu cầu nó hướng dẫn chính xác các cú click, bạn thao tác và cho Codex đọc file `.edmx` sinh ra để tiếp tục.
6. Đọc `Documentation/STATUS.md` sau khi Codex báo xong để phân biệt phần đã viết mã, đã build, và đã test SQL thực tế.

**Không cần đưa project Lab02/Lab03 cũ vào solution BE1–3 ở bước này.**
