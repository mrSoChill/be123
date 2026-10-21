# Cập nhật MyStoreModel.edmx sau khi database đổi

Không thay đổi database đang dùng để làm ví dụ. Thực hiện trên một database demo
được phép thay đổi, sau đó hoàn tác thay đổi demo nếu không còn cần.

1. Trong **SQL Server Object Explorer** hoặc SSMS, chọn đúng database demo và
   thêm/sửa một cột, bảng, hoặc khóa ngoại theo bài thực hành.
2. Lưu thay đổi trên database và xác minh lại schema bằng Refresh/F5 hoặc truy
   vấn `sys.columns` / `sys.foreign_keys`.
3. Trong Visual Studio, mở `Models/Model1.edmx` bằng **EF Designer** (đây là
   model thực tế hiện có trong project).
4. Nhấp phải vào vùng trống của designer, chọn **Update Model from Database...**.
5. Trong wizard, chọn đúng kết nối `MyStoreEntities` (hoặc tên connection thực
   tế), chọn tab **Add**, **Refresh** hoặc **Delete** phù hợp, rồi bấm **Finish**.
6. Lưu EDMX, build solution, và kiểm tra entity/context/navigation được sinh ra.

Chỉ đánh dấu PASS sau khi có bằng chứng cho các bước 2 và 6. Không tự sửa `.edmx`
hoặc các file sinh tự động để giả lập kết quả wizard.
