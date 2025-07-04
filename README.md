# BookStore_Microservice


---

## 1. Tổng quan cấu trúc thư mục

```
/<root-solution-folder>
│
├── <Tên bảng>ManagementApi/
│   ├── Controllers/
│   ├── Data/
│   ├── DTOs/
│   ├── Migrations/
│   ├── Models/
│   ├── Profile/
│   ├── Repositories/
│   ├── Services/
│   ├── Validations/
│   └── BookManagementApi.csproj
│
└── README.md
```

---

## 2. Ý nghĩa các thư mục chính

- **Controllers/**: Chứa các controller xử lý request/response cho API.
- **DTOs/**: Định nghĩa các Data Transfer Object dùng để truyền dữ liệu giữa client và server.
- **Migrations/**: Lưu trữ các file migration của Entity Framework để quản lý schema database.
- **Models/**: Chứa các entity/model ánh xạ với bảng dữ liệu.
- **Profile/**: Cấu hình AutoMapper để map giữa Model và DTO.
- **Repositories/**: Định nghĩa interface và implement các thao tác với database.
- **Services/**: Chứa business logic, xử lý nghiệp vụ chính của từng microservice.
- **Validations/**: Chứa các class FluentValidation để validate dữ liệu đầu vào.
- **Utilities/** (chỉ có ở UserManagementApi): Chứa các tiện ích, helper dùng chung (ví dụ: JWT, gửi email...).

---

## 3. Hướng dẫn sử dụng

- **Clone dự án**:  
```sh
git clone <repository-url>
```

- **Mở solution bằng Visual Studio 2022**  
  Mở file `.sln` hoặc mở từng project tùy nhu cầu phát triển.
  
- **Mở File appsetting.json để chỉnh sửa ConnectionString**
  
- **Chạy migration và update database**  
  Sử dụng lệnh `dotnet ef database update` trong từng project để tạo database.

- **Chạy từng service**  
  Có thể chạy từng API độc lập hoặc đồng thời (mỗi API là một project riêng biệt).

---

## 4. Lưu ý

- Mỗi API là một microservice độc lập, có thể deploy riêng.
- Các project sử dụng .NET 8, C# 12, Entity Framework Core, AutoMapper, FluentValidation, Swashbuckle (Swagger).
- Đảm bảo cấu hình connection string và các biến môi trường phù hợp trước khi chạy.
- Đảm bảo trong database có dữ liệu phù hợp với cấu hình dự án. 
---

## 5. Đóng góp

- Fork repository, tạo branch mới, commit thay đổi và gửi pull request.
- Đảm bảo tuân thủ coding convention và cấu trúc thư mục như trên.

---

## 6. Hướng dẫn chạy dự án
- B1: Chuột phải vào dự án ApiGateWay -> Open in terminal -> Chạy lệnh: dotnet run --launch-profile ApiGateWay
- B2: Chạy các Api cần thiết và UI. 
