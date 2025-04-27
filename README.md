# 🎓 Đồ án Hệ Quản Trị Cơ Sở Dữ Liệu

## **Ứng dụng WinForms Quản Lý Nhập Xuất Kho Cửa Hàng Laptop**

---

## 👥 Thông tin nhóm

- **Đỗ Gia Huấn** — MSSV: 22110030
- **Văn Công Toàn** — MSSV: 22110079
- **Phan Đình Trung** — MSSV: 22110083
- **Trần Văn Tuyến** — MSSV: 22110086

---

## 📝 Mô tả dự án

Dự án xây dựng một ứng dụng **WinForms (Windows Forms)** nhằm hỗ trợ quản lý quy trình nhập và xuất kho tại một cửa hàng kinh doanh laptop. Ứng dụng cung cấp các chức năng quản lý kho hàng, theo dõi tồn kho, và báo cáo hoạt động kinh doanh. Các tính năng chính bao gồm:

- Quản lý nhập hàng từ nhà cung cấp.
- Quản lý xuất hàng cho khách hàng.
- Theo dõi tồn kho theo thời gian thực.
- Quản lý danh mục sản phẩm, nhà cung cấp, khách hàng, và người dùng.
- Tạo báo cáo thống kê về nhập, xuất, và tồn kho.

---

## 🎯 Mục tiêu

- **Tự động hóa** quy trình nhập và xuất kho, giảm thiểu sai sót do thao tác thủ công.
- Cung cấp **giao diện thân thiện** với người dùng, dễ sử dụng cho nhân viên và quản lý.
- **Tối ưu hóa truy vấn dữ liệu** bằng cách tích hợp cơ sở dữ liệu SQL Server hiệu quả.
- Hỗ trợ **báo cáo thống kê** để phân tích hoạt động kinh doanh.

---

## 🔧 Chức năng chính

### 📦 Quản lý nhập hàng

- Nhập thông tin phiếu nhập: ngày nhập, nhà cung cấp, sản phẩm, số lượng, giá nhập.
- Tự động cập nhật số lượng tồn kho sau khi nhập hàng (sử dụng `fn_GetStockQuantity` để kiểm tra tồn kho).

### 📤 Quản lý xuất hàng

- Ghi nhận thông tin phiếu xuất: ngày xuất, khách hàng, sản phẩm, số lượng, giá xuất.
- Kiểm tra tồn kho trước khi xuất hàng (sử dụng trigger `tr_CheckStockBeforeExport`).

### 🗃️ Quản lý kho

- Theo dõi danh sách sản phẩm: mã, tên, hãng, mô tả, giá nhập, giá bán, số lượng tồn (sử dụng view `vw_Inventory`).
- Hỗ trợ tìm kiếm và lọc sản phẩm theo hãng hoặc danh mục.

### 📊 Báo cáo

- Thống kê số lượng nhập, xuất, tồn theo khoảng thời gian (sử dụng view `vw_ImportDetails` và `vw_ExportDetails`).
- Tính lợi nhuận và giá trị tồn kho (sử dụng `fn_GetProductProfit` và `fn_GetInventoryValue`).
- Xuất báo cáo ra file Excel hoặc PDF.

### 📚 Quản lý danh mục

- Quản lý thông tin sản phẩm: thêm, sửa, xóa sản phẩm (`sp_InsertProduct`, `sp_UpdateProduct`, `sp_DeleteProduct`).
- Quản lý nhà cung cấp (`sp_InsertSupplier`, `sp_UpdateSupplier`, `sp_DeleteSupplier`).
- Quản lý khách hàng (`sp_InsertCustomer`, `sp_UpdateCustomer`, `sp_DeleteCustomer`).
- Quản lý danh mục sản phẩm (`sp_InsertCategory`, `sp_UpdateCategory`, `sp_DeleteCategory`).

### 👤 Quản lý người dùng

- Quản lý tài khoản người dùng: thêm, sửa, xóa (`sp_InsertUser`, `sp_UpdateUser`, `sp_DeleteUser`).
- Phân quyền người dùng: chỉ admin mới có thể cập nhật thông tin hoặc vai trò người dùng (`sp_UpdateUserRole`).

---

## 🛠️ Công nghệ sử dụng

- **Ngôn ngữ**: C#
- **Framework**: WinForms (.NET Framework 4.8 hoặc mới hơn)
- **Cơ sở dữ liệu**: SQL Server
- **Truy cập dữ liệu**: ADO.NET
- **Quản lý mã nguồn**: Git (GitHub / GitLab)

---

## 🚀 Hướng dẫn cài đặt và chạy ứng dụng

### Yêu cầu:

- Visual Studio 2022 (hoặc mới hơn) + workload **.NET Desktop Development**
- SQL Server hoặc một công cụ quản lý cơ sở dữ liệu tương ứng
- .NET Framework 4.8

### Các bước cài đặt:

1. **Clone dự án từ GitHub hoặc GitLab**

   ```bash
   git clone https://github.com/hcmute-toan/DBMS_Project
   ```

2. **Mở file `.sln` trong Visual Studio**

   - Tìm file `InventoryManagementSystem.sln` và mở bằng Visual Studio.

3. **Cấu hình chuỗi kết nối** trong `App.config`

   - Mở file `App.config` và cập nhật chuỗi kết nối tới SQL Server của bạn:

   ```xml
     <applicationSettings>
      <LaptopShopProject.Properties.Settings>
         <setting name="ConnStr" serializeAs="String">
               <value>
               Data Source=Your server name;Initial Catalog=LaptopStoreDBMS;Integrated Security=True;Encrypt=True;Trust Server Certificate=True
               </value>
         </setting>
      </LaptopShopProject.Properties.Settings>
      </applicationSettings>
   ```

4. **Tạo cơ sở dữ liệu**

   - Mở SQL Server Management Studio (SSMS).
   - Chạy script SQL (`dbms.sql`) để tạo cơ sở dữ liệu và dữ liệu mẫu.

5. **Build và chạy ứng dụng**:

   - Build: `Ctrl + Shift + B`
   - Run: `F5`

> ⚠️ Đảm bảo cơ sở dữ liệu đã được tạo và có dữ liệu mẫu trước khi chạy ứng dụng.

---

## 📂 Cấu trúc thư mục dự án

```
InventoryManagementSystem/
├── bin/                              # Thư mục chứa file thực thi sau khi build
│   ├── Debug/                        # Chứa file debug
│   └── Release/                      # Chứa file release
├── obj/                              # Thư mục chứa các file tạm thời khi build
├── Properties/                       # Cấu hình dự án
│   ├── AssemblyInfo.cs               # Thông tin metadata của dự án
│   └── Resources.resx               # Tài nguyên (nếu có)
├── Forms/                            # Chứa các form giao diện người dùng
│   ├── LoginForm.cs                 # Form đăng nhập
│   ├── MainForm.cs                  # Form chính
│   ├── UserManagementForm.cs        # Form quản lý người dùng
│   ├── ProductManagementForm.cs     # Form quản lý sản phẩm
│   ├── ImportManagementForm.cs      # Form quản lý phiếu nhập
│   ├── ExportManagementForm.cs      # Form quản lý phiếu xuất
│   ├── SupplierManagementForm.cs    # Form quản lý nhà cung cấp
│   ├── CustomerManagementForm.cs    # Form quản lý khách hàng
│   ├── CategoryManagementForm.cs    # Form quản lý danh mục
│   └── ReportForm.cs                # Form báo cáo
├── Models/                           # Chứa các lớp mô hình dữ liệu
│   ├── User.cs                      # Lớp mô hình cho Users
│   ├── Supplier.cs                  # Lớp mô hình cho Supplier
│   ├── Customer.cs                  # Lớp mô hình cho Customer
│   ├── Category.cs                  # Lớp mô hình cho Category
│   ├── Product.cs                   # Lớp mô hình cho Product
│   ├── Import.cs                    # Lớp mô hình cho Import
│   ├── ImportDetail.cs              # Lớp mô hình cho ImportDetail
│   ├── Export.cs                    # Lớp mô hình cho Export
│   ├── ProductLog.cs                # Lớp mô hình cho ProductLog
│   ├── PermissonLog.cs              # Lớp mô hình cho Permission
│   └── ExportDetail.cs              # Lớp mô hình cho ExportDetail
├── DataAccess/                       # Chứa các lớp truy cập dữ liệu
│   ├── DatabaseConnection.cs        # Lớp kết nối cơ sở dữ liệu
│   ├── UserRepository.cs            # Lớp truy cập dữ liệu cho Users
│   ├── ProductRepository.cs         # Lớp truy cập dữ liệu cho Product
│   ├── ImportRepository.cs          # Lớp truy cập dữ liệu cho Import
│   ├── ExportRepository.cs          # Lớp truy cập dữ liệu cho Export
│   ├── SupplierRepository.cs        # Lớp truy cập dữ liệu cho Supplier
│   ├── CustomerRepository.cs        # Lớp truy cập dữ liệu cho Customer
│   ├── CategoryRepository.cs        # Lớp truy cập dữ liệu cho Category
│   └── ReportRepository.cs          # Lớp truy cập dữ liệu cho báo cáo
├── Utilities/                        # Chứa các lớp tiện ích
│   ├── Logger.cs                    # Lớp ghi log
│   └── Helper.cs                    # Lớp hỗ trợ (format tiền tệ, ngày tháng, v.v.)
├── Resources/                        # Chứa tài nguyên như hình ảnh, biểu tượng
│   ├── logo.png                     # Logo ứng dụng
│   └── icons/                       # Thư mục chứa các biểu tượng
│       ├── user_icon.png            # Biểu tượng người dùng
│       ├── product_icon.png         # Biểu tượng sản phẩm
│       ├── import_icon.png          # Biểu tượng phiếu nhập
│       └── export_icon.png          # Biểu tượng phiếu xuất
├── App.config                        # Cấu hình ứng dụng (chuỗi kết nối, v.v.)
└── Program.cs                        # Điểm vào của ứng dụng
```

---

## 📬 Liên hệ

Mọi thắc mắc hoặc góp ý, vui lòng liên hệ nhóm qua email: **tuyenbest1234@gmail.com** hoặc thông qua hệ thống GitHub Issues của dự án.

---

### **Lưu ý**

- Đảm bảo đã chạy script SQL để tạo cơ sở dữ liệu trước khi khởi động ứng dụng.
- Tài khoản đăng nhập mẫu:
  - Username: `admin1` / Password: `admin_pass` (vai trò: admin)
  - Username: `employee1` / Password: `emp_pass` (vai trò: user)

---

**Nhóm phát triển**  
Trường Đại học Sư phạm Kỹ thuật TP.HCM

Khoa Đào tạo quốc tế

Năm học 2024-2025
