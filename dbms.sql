-- Danh sách các View, Stored Procedure, Trigger, Function
-- 1. View
-- vw_ProductDetails: Lấy thông tin sản phẩm và danh mục.
-- vw_ImportDetails: Lấy thông tin phiếu nhập và chi tiết (sử dụng fn_GetImportTotal để tính tổng tiền).
-- vw_ExportDetails: Lấy thông tin phiếu xuất và chi tiết (sử dụng fn_GetExportTotal để tính tổng tiền).
-- vw_UserDetails: Lấy thông tin người dùng và vai trò.
-- vw_Inventory: Tổng hợp tồn kho.
-- 2. Stored Procedure
-- Bảng Users
-- sp_InsertUser: Thêm người dùng.
-- sp_UpdateUser: Cập nhật thông tin người dùng (chỉ admin).
-- sp_UpdateUserRole: Cập nhật quyền người dùng (chỉ admin).
-- sp_DeleteUser: Xóa người dùng.
-- sp_GetAllUsers: Lấy tất cả người dùng.
-- Bảng Product
-- sp_InsertProduct: Thêm sản phẩm.
-- sp_UpdateProduct: Cập nhật sản phẩm.
-- sp_DeleteProduct: Xóa sản phẩm.
-- sp_GetAllProducts: Lấy tất cả sản phẩm.
-- Bảng Import
-- sp_InsertImport: Thêm phiếu nhập.
-- sp_InsertImportDetail: Thêm chi tiết phiếu nhập.
-- sp_UpdateImport: Cập nhật phiếu nhập.
-- sp_DeleteImport: Xóa phiếu nhập.
-- Bảng Export
-- sp_InsertExport: Thêm phiếu xuất.
-- sp_InsertExportDetail: Thêm chi tiết phiếu xuất.
-- sp_UpdateExport: Cập nhật phiếu xuất.
-- sp_DeleteExport: Xóa phiếu xuất.
-- Bảng Customer
-- sp_InsertCustomer: Thêm khách hàng.
-- sp_UpdateCustomer: Cập nhật khách hàng.
-- sp_DeleteCustomer: Xóa khách hàng.
-- sp_GetAllCustomers: Lấy tất cả khách hàng.
-- Bảng Supplier
-- sp_InsertSupplier: Thêm nhà cung cấp.
-- sp_UpdateSupplier: Cập nhật nhà cung cấp.
-- sp_DeleteSupplier: Xóa nhà cung cấp.
-- sp_GetAllSuppliers: Lấy tất cả nhà cung cấp.
-- Bảng Category
-- sp_InsertCategory: Thêm danh mục.
-- sp_UpdateCategory: Cập nhật danh mục.
-- sp_DeleteCategory: Xóa danh mục.
-- sp_GetAllCategories: Lấy tất cả danh mục.
-- 3. Trigger
-- tr_CheckStockBeforeExport: Kiểm tra tồn kho trước khi xuất.
-- tr_LogProductDeletion: Ghi log khi xóa sản phẩm.
-- 4. Function
-- fn_GetImportTotal: Tính tổng tiền phiếu nhập.
-- fn_GetStockQuantity: Lấy số lượng tồn kho.
-- fn_GetProductCategories: Lấy danh sách danh mục.
-- fn_GetExportTotal: Tính tổng tiền phiếu xuất.
-- fn_GetProductProfit: Tính lợi nhuận sản phẩm.
-- fn_GetInventoryValue: Tính giá trị tồn kho.

-- Tạo cơ sở dữ liệu và các thành phần liên quan

-- 1. Tạo các bảng
-- Tài khoản người dùng
CREATE TABLE Users (
    user_id INT PRIMARY KEY IDENTITY,
    username NVARCHAR(100),
    password NVARCHAR(100),
    role NVARCHAR(50)
);
GO

-- Nhà cung cấp
CREATE TABLE Supplier (
    supplier_id INT PRIMARY KEY IDENTITY,
    name NVARCHAR(100),
    gmail NVARCHAR(100),
    phone NVARCHAR(20)
);
GO

-- Khách hàng
CREATE TABLE Customer (
    customer_id INT PRIMARY KEY IDENTITY,
    name NVARCHAR(100),
    gmail NVARCHAR(100),
    phone NVARCHAR(20)
);
GO

-- Loại sản phẩm
CREATE TABLE Category (
    category_id INT PRIMARY KEY IDENTITY,
    category_name NVARCHAR(100),
    description NVARCHAR(255)
);
GO

-- Sản phẩm
CREATE TABLE Product (
    product_id INT PRIMARY KEY IDENTITY,
    product_name NVARCHAR(100),
    brand NVARCHAR(100),
    description NVARCHAR(255),
    import_price DECIMAL(18, 2),
    sell_price DECIMAL(18, 2)
);
GO

-- Bảng trung gian: Sản phẩm - Danh mục (nhiều-nhiều)
CREATE TABLE ProductCategory (
    product_id INT FOREIGN KEY REFERENCES Product(product_id),
    category_id INT FOREIGN KEY REFERENCES Category(category_id),
    PRIMARY KEY (product_id, category_id)
);
GO

-- Phiếu nhập
CREATE TABLE Import (
    import_id INT PRIMARY KEY IDENTITY,
    import_date DATE,
    supplier_id INT FOREIGN KEY REFERENCES Supplier(supplier_id)
);
GO

-- Bảng trung gian: Người dùng tham gia phiếu nhập (nhiều-nhiều)
CREATE TABLE UserImport (
    user_id INT FOREIGN KEY REFERENCES Users(user_id),
    import_id INT FOREIGN KEY REFERENCES Import(import_id),
    PRIMARY KEY (user_id, import_id)
);
GO

-- Chi tiết nhập
CREATE TABLE ImportDetail (
    import_detail_id INT PRIMARY KEY IDENTITY,
    import_id INT FOREIGN KEY REFERENCES Import(import_id),
    product_id INT FOREIGN KEY REFERENCES Product(product_id),
    quantity INT,
    unit_price DECIMAL(18, 2)
);
GO

-- Phiếu xuất
CREATE TABLE Export (
    export_id INT PRIMARY KEY IDENTITY,
    export_date DATE,
    customer_id INT FOREIGN KEY REFERENCES Customer(customer_id)
);
GO

-- Bảng trung gian: Người dùng tham gia phiếu xuất (nhiều-nhiều)
CREATE TABLE UserExport (
    user_id INT FOREIGN KEY REFERENCES Users(user_id),
    export_id INT FOREIGN KEY REFERENCES Export(export_id),
    PRIMARY KEY (user_id, export_id)
);
GO

-- Chi tiết xuất
CREATE TABLE ExportDetail (
    export_detail_id INT PRIMARY KEY IDENTITY,
    export_id INT FOREIGN KEY REFERENCES Export(export_id),
    product_id INT FOREIGN KEY REFERENCES Product(product_id),
    quantity INT,
    unit_price DECIMAL(18, 2)
);
GO

-- Bảng log xóa sản phẩm
CREATE TABLE ProductLog (
    log_id INT PRIMARY KEY IDENTITY,
    product_id INT,
    product_name NVARCHAR(100),
    deleted_date DATETIME,
    deleted_by NVARCHAR(100)
);
GO

-- 2. Tạo các Function
-- Function tính tổng tiền phiếu nhập
CREATE FUNCTION fn_GetImportTotal (@import_id INT)
RETURNS DECIMAL(18, 2)
AS
BEGIN
    DECLARE @total DECIMAL(18, 2);
    SELECT @total = SUM(quantity * unit_price)
    FROM ImportDetail
    WHERE import_id = @import_id;
    RETURN ISNULL(@total, 0);
END;
GO

-- Function kiểm tra tồn kho của sản phẩm
CREATE FUNCTION fn_GetStockQuantity (@product_id INT)
RETURNS INT
AS
BEGIN
    DECLARE @stock INT;
    SELECT @stock = (SUM(id.quantity) - ISNULL(SUM(ed.quantity), 0))
    FROM Product p
    LEFT JOIN ImportDetail id ON p.product_id = id.product_id
    LEFT JOIN ExportDetail ed ON p.product_id = ed.product_id
    WHERE p.product_id = @product_id
    GROUP BY p.product_id;
    RETURN ISNULL(@stock, 0);
END;
GO

-- Function lấy danh sách danh mục của sản phẩm
CREATE FUNCTION fn_GetProductCategories (@product_id INT)
RETURNS NVARCHAR(MAX)
AS
BEGIN
    DECLARE @categories NVARCHAR(MAX);
    SELECT @categories = STRING_AGG(c.category_name, ', ')
    FROM ProductCategory pc
    JOIN Category c ON pc.category_id = c.category_id
    WHERE pc.product_id = @product_id;
    RETURN ISNULL(@categories, '');
END;
GO

-- Function tính tổng tiền phiếu xuất
CREATE FUNCTION fn_GetExportTotal (@export_id INT)
RETURNS DECIMAL(18, 2)
AS
BEGIN
    DECLARE @total DECIMAL(18, 2);
    SELECT @total = SUM(quantity * unit_price)
    FROM ExportDetail
    WHERE export_id = @export_id;
    RETURN ISNULL(@total, 0);
END;
GO

-- Function tính lợi nhuận của sản phẩm
CREATE FUNCTION fn_GetProductProfit (@product_id INT)
RETURNS DECIMAL(18, 2)
AS
BEGIN
    DECLARE @profit DECIMAL(18, 2);
    SELECT @profit = SUM(ed.quantity * (ed.unit_price - p.import_price))
    FROM ExportDetail ed
    JOIN Product p ON ed.product_id = p.product_id
    WHERE ed.product_id = @product_id;
    RETURN ISNULL(@profit, 0);
END;
GO

-- Function tính giá trị tồn kho của sản phẩm
CREATE FUNCTION fn_GetInventoryValue (@product_id INT)
RETURNS DECIMAL(18, 2)
AS
BEGIN
    DECLARE @value DECIMAL(18, 2);
    SELECT @value = (SUM(id.quantity) - ISNULL(SUM(ed.quantity), 0)) * p.import_price
    FROM Product p
    LEFT JOIN ImportDetail id ON p.product_id = id.product_id
    LEFT JOIN ExportDetail ed ON p.product_id = ed.product_id
    WHERE p.product_id = @product_id
    GROUP BY p.product_id, p.import_price;
    RETURN ISNULL(@value, 0);
END;
GO

-- 3. Tạo các View
-- View tổng hợp tồn kho
CREATE VIEW vw_Inventory AS
SELECT 
    p.product_id,
    p.product_name,
    SUM(id.quantity) AS total_imported,
    SUM(ed.quantity) AS total_exported,
    (SUM(id.quantity) - ISNULL(SUM(ed.quantity), 0)) AS stock_quantity
FROM Product p
LEFT JOIN ImportDetail id ON p.product_id = id.product_id
LEFT JOIN ExportDetail ed ON p.product_id = ed.product_id
GROUP BY p.product_id, p.product_name;
GO

-- View lấy thông tin sản phẩm và danh mục
CREATE VIEW vw_ProductDetails AS
SELECT 
    p.product_id,
    p.product_name,
    p.brand,
    p.description,
    p.import_price,
    p.sell_price,
    STRING_AGG(c.category_name, ', ') AS categories
FROM Product p
LEFT JOIN ProductCategory pc ON p.product_id = pc.product_id
LEFT JOIN Category c ON pc.category_id = c.category_id
GROUP BY p.product_id, p.product_name, p.brand, p.description, p.import_price, p.sell_price;
GO

-- View lấy thông tin phiếu nhập và chi tiết
CREATE VIEW vw_ImportDetails AS
SELECT 
    i.import_id,
    i.import_date,
    s.name AS supplier_name,
    id.product_id,
    p.product_name,
    id.quantity,
    id.unit_price,
    dbo.fn_GetImportTotal(i.import_id) AS total_amount
FROM Import i
JOIN Supplier s ON i.supplier_id = s.supplier_id
JOIN ImportDetail id ON i.import_id = id.import_id
JOIN Product p ON id.product_id = p.product_id;
GO

-- View lấy thông tin phiếu xuất và chi tiết
CREATE VIEW vw_ExportDetails AS
SELECT 
    e.export_id,
    e.export_date,
    c.name AS customer_name,
    ed.product_id,
    p.product_name,
    ed.quantity,
    ed.unit_price,
    dbo.fn_GetExportTotal(e.export_id) AS total_amount
FROM Export e
JOIN ExportDetail ed ON e.export_id = ed.export_id
JOIN Product p ON ed.product_id = p.product_id
JOIN Customer c ON e.customer_id = c.customer_id;
GO

-- View lấy thông tin người dùng và vai trò
CREATE VIEW vw_UserDetails AS
SELECT 
    user_id,
    username,
    role
FROM Users;
GO

-- 4. Tạo các Stored Procedure
-- 4.1. CRUD cho bảng Users
-- Thêm người dùng
CREATE PROCEDURE sp_InsertUser
    @username NVARCHAR(100),
    @password NVARCHAR(100),
    @role NVARCHAR(50)
AS
BEGIN
    INSERT INTO Users (username, password, role)
    VALUES (@username, @password, @role);
    SELECT SCOPE_IDENTITY() AS user_id;
END;
GO

-- Cập nhật thông tin người dùng
CREATE PROCEDURE sp_UpdateUser
    @current_user_id INT,
    @user_id INT,
    @username NVARCHAR(100),
    @password NVARCHAR(100)
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM Users WHERE user_id = @current_user_id AND role = 'admin')
    BEGIN
        RAISERROR ('Chỉ admin mới có quyền cập nhật thông tin người dùng!', 16, 1);
        RETURN;
    END;

    UPDATE Users
    SET username = @username,
        password = @password
    WHERE user_id = @user_id;
END;
GO

-- Cập nhật quyền người dùng
CREATE PROCEDURE sp_UpdateUserRole
    @current_user_id INT,
    @user_id INT,
    @role NVARCHAR(50)
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM Users WHERE user_id = @current_user_id AND role = 'admin')
    BEGIN
        RAISERROR ('Chỉ admin mới có quyền cập nhật vai trò người dùng!', 16, 1);
        RETURN;
    END;

    UPDATE Users
    SET role = @role
    WHERE user_id = @user_id;
END;
GO

-- Xóa người dùng
CREATE PROCEDURE sp_DeleteUser
    @user_id INT
AS
BEGIN
    DELETE FROM UserImport WHERE user_id = @user_id;
    DELETE FROM UserExport WHERE user_id = @user_id;
    DELETE FROM Users WHERE user_id = @user_id;
END;
GO

-- Lấy tất cả người dùng
CREATE PROCEDURE sp_GetAllUsers
AS
BEGIN
    SELECT * FROM vw_UserDetails;
END;
GO

-- 4.2. CRUD cho bảng Product
-- Thêm sản phẩm
CREATE PROCEDURE sp_InsertProduct
    @product_name NVARCHAR(100),
    @brand NVARCHAR(100),
    @description NVARCHAR(255),
    @import_price DECIMAL(18, 2),
    @sell_price DECIMAL(18, 2)
AS
BEGIN
    INSERT INTO Product (product_name, brand, description, import_price, sell_price)
    VALUES (@product_name, @brand, @description, @import_price, @sell_price);
    SELECT SCOPE_IDENTITY() AS product_id;
END;
GO

-- Cập nhật sản phẩm
CREATE PROCEDURE sp_UpdateProduct
    @product_id INT,
    @product_name NVARCHAR(100),
    @brand NVARCHAR(100),
    @description NVARCHAR(255),
    @import_price DECIMAL(18, 2),
    @sell_price DECIMAL(18, 2)
AS
BEGIN
    UPDATE Product
    SET product_name = @product_name,
        brand = @brand,
        description = @description,
        import_price = @import_price,
        sell_price = @sell_price
    WHERE product_id = @product_id;
END;
GO

-- Xóa sản phẩm
CREATE PROCEDURE sp_DeleteProduct
    @product_id INT
AS
BEGIN
    DELETE FROM ProductCategory WHERE product_id = @product_id;
    DELETE FROM ImportDetail WHERE product_id = @product_id;
    DELETE FROM ExportDetail WHERE product_id = @product_id;
    DELETE FROM Product WHERE product_id = @product_id;
END;
GO

-- Lấy tất cả sản phẩm
CREATE PROCEDURE sp_GetAllProducts
AS
BEGIN
    SELECT * FROM vw_ProductDetails;
END;
GO

-- 4.3. CRUD cho bảng Import
-- Thêm phiếu nhập
CREATE PROCEDURE sp_InsertImport
    @import_date DATE,
    @supplier_id INT
AS
BEGIN
    INSERT INTO Import (import_date, supplier_id)
    VALUES (@import_date, @supplier_id);
    SELECT SCOPE_IDENTITY() AS import_id;
END;
GO

-- Thêm chi tiết phiếu nhập
CREATE PROCEDURE sp_InsertImportDetail
    @import_id INT,
    @product_id INT,
    @quantity INT,
    @unit_price DECIMAL(18, 2)
AS
BEGIN
    INSERT INTO ImportDetail (import_id, product_id, quantity, unit_price)
    VALUES (@import_id, @product_id, @quantity, @unit_price);
END;
GO

-- Cập nhật phiếu nhập
CREATE PROCEDURE sp_UpdateImport
    @import_id INT,
    @import_date DATE,
    @supplier_id INT
AS
BEGIN
    UPDATE Import
    SET import_date = @import_date,
        supplier_id = @supplier_id
    WHERE import_id = @import_id;
END;
GO

-- Xóa phiếu nhập
CREATE PROCEDURE sp_DeleteImport
    @import_id INT
AS
BEGIN
    DELETE FROM ImportDetail WHERE import_id = @import_id;
    DELETE FROM UserImport WHERE import_id = @import_id;
    DELETE FROM Import WHERE import_id = @import_id;
END;
GO

-- 4.4. CRUD cho bảng Export
-- Thêm phiếu xuất
CREATE PROCEDURE sp_InsertExport
    @export_date DATE,
    @customer_id INT
AS
BEGIN
    INSERT INTO Export (export_date, customer_id)
    VALUES (@export_date, @customer_id);
    SELECT SCOPE_IDENTITY() AS export_id;
END;
GO

-- Thêm chi tiết phiếu xuất
CREATE PROCEDURE sp_InsertExportDetail
    @export_id INT,
    @product_id INT,
    @quantity INT,
    @unit_price DECIMAL(18, 2)
AS
BEGIN
    INSERT INTO ExportDetail (export_id, product_id, quantity, unit_price)
    VALUES (@export_id, @product_id, @quantity, @unit_price);
END;
GO

-- Cập nhật phiếu xuất
CREATE PROCEDURE sp_UpdateExport
    @export_id INT,
    @export_date DATE,
    @customer_id INT
AS
BEGIN
    UPDATE Export
    SET export_date = @export_date,
        customer_id = @customer_id
    WHERE export_id = @export_id;
END;
GO

-- Xóa phiếu xuất
CREATE PROCEDURE sp_DeleteExport
    @export_id INT
AS
BEGIN
    DELETE FROM ExportDetail WHERE export_id = @export_id;
    DELETE FROM UserExport WHERE export_id = @export_id;
    DELETE FROM Export WHERE export_id = @export_id;
END;
GO

-- 4.5. CRUD cho bảng Customer
-- Thêm khách hàng
CREATE PROCEDURE sp_InsertCustomer
    @name NVARCHAR(100),
    @gmail NVARCHAR(100),
    @phone NVARCHAR(20)
AS
BEGIN
    INSERT INTO Customer (name, gmail, phone)
    VALUES (@name, @gmail, @phone);
    SELECT SCOPE_IDENTITY() AS customer_id;
END;
GO

-- Cập nhật khách hàng
CREATE PROCEDURE sp_UpdateCustomer
    @customer_id INT,
    @name NVARCHAR(100),
    @gmail NVARCHAR(100),
    @phone NVARCHAR(20)
AS
BEGIN
    UPDATE Customer
    SET name = @name,
        gmail = @gmail,
        phone = @phone
    WHERE customer_id = @customer_id;
END;
GO

-- Xóa khách hàng
CREATE PROCEDURE sp_DeleteCustomer
    @customer_id INT
AS
BEGIN
    DELETE FROM Export WHERE customer_id = @customer_id;
    DELETE FROM Customer WHERE customer_id = @customer_id;
END;
GO

-- Lấy tất cả khách hàng
CREATE PROCEDURE sp_GetAllCustomers
AS
BEGIN
    SELECT * FROM Customer;
END;
GO

-- 4.6. CRUD cho bảng Supplier
-- Thêm nhà cung cấp
CREATE PROCEDURE sp_InsertSupplier
    @name NVARCHAR(100),
    @gmail NVARCHAR(100),
    @phone NVARCHAR(20)
AS
BEGIN
    INSERT INTO Supplier (name, gmail, phone)
    VALUES (@name, @gmail, @phone);
    SELECT SCOPE_IDENTITY() AS supplier_id;
END;
GO

-- Cập nhật nhà cung cấp
CREATE PROCEDURE sp_UpdateSupplier
    @supplier_id INT,
    @name NVARCHAR(100),
    @gmail NVARCHAR(100),
    @phone NVARCHAR(20)
AS
BEGIN
    UPDATE Supplier
    SET name = @name,
        gmail = @gmail,
        phone = @phone
    WHERE supplier_id = @supplier_id;
END;
GO

-- Xóa nhà cung cấp
CREATE PROCEDURE sp_DeleteSupplier
    @supplier_id INT
AS
BEGIN
    DELETE FROM Import WHERE supplier_id = @supplier_id;
    DELETE FROM Supplier WHERE supplier_id = @supplier_id;
END;
GO

-- Lấy tất cả nhà cung cấp
CREATE PROCEDURE sp_GetAllSuppliers
AS
BEGIN
    SELECT * FROM Supplier;
END;
GO

-- 4.7. CRUD cho bảng Category
-- Thêm danh mục
CREATE PROCEDURE sp_InsertCategory
    @category_name NVARCHAR(100),
    @description NVARCHAR(255)
AS
BEGIN
    INSERT INTO Category (category_name, description)
    VALUES (@category_name, @description);
    SELECT SCOPE_IDENTITY() AS category_id;
END;
GO

-- Cập nhật danh mục
CREATE PROCEDURE sp_UpdateCategory
    @category_id INT,
    @category_name NVARCHAR(100),
    @description NVARCHAR(255)
AS
BEGIN
    UPDATE Category
    SET category_name = @category_name,
        description = @description
    WHERE category_id = @category_id;
END;
GO

-- Xóa danh mục
CREATE PROCEDURE sp_DeleteCategory
    @category_id INT
AS
BEGIN
    DELETE FROM ProductCategory WHERE category_id = @category_id;
    DELETE FROM Category WHERE category_id = @category_id;
END;
GO

-- Lấy tất cả danh mục
CREATE PROCEDURE sp_GetAllCategories
AS
BEGIN
    SELECT * FROM Category;
END;
GO

-- 5. Tạo các Trigger
-- Trigger kiểm tra số lượng tồn kho trước khi xuất
CREATE TRIGGER tr_CheckStockBeforeExport
ON ExportDetail
AFTER INSERT
AS
BEGIN
    DECLARE @product_id INT, @quantity INT, @stock INT;

    SELECT @product_id = product_id, @quantity = quantity
    FROM inserted;

    SELECT @stock = dbo.fn_GetStockQuantity(@product_id);

    IF @quantity > @stock
    BEGIN
        RAISERROR ('Số lượng xuất vượt quá tồn kho!', 16, 1);
        ROLLBACK TRANSACTION;
    END;
END;
GO

-- Trigger ghi log khi xóa sản phẩm
CREATE TRIGGER tr_LogProductDeletion
ON Product
AFTER DELETE
AS
BEGIN
    INSERT INTO ProductLog (product_id, product_name, deleted_date, deleted_by)
    SELECT 
        d.product_id,
        d.product_name,
        GETDATE(),
        SYSTEM_USER
    FROM deleted d;
END;
GO

-- 6. Thêm dữ liệu mẫu
-- Users
INSERT INTO Users (username, password, role) VALUES
('admin1', 'admin123', 'admin'),
('user1', 'user123', 'user'),
('user2', 'user456', 'user');
GO

-- Supplier
INSERT INTO Supplier (name, gmail, phone) VALUES
('Công ty A', 'contact@companyA.com', '0901234567'),
('Công ty B', 'contact@companyB.com', '0912345678'),
('Công ty C', 'contact@companyC.com', '0923456789');
GO

-- Customer
INSERT INTO Customer (name, gmail, phone) VALUES
('Nguyễn Văn A', 'nguyenvana@gmail.com', '0931234567'),
('Trần Thị B', 'tranthib@gmail.com', '0942345678'),
('Lê Văn C', 'levanc@gmail.com', '0953456789');
GO

-- Category
INSERT INTO Category (category_name, description) VALUES
('Điện tử', 'Sản phẩm công nghệ và điện tử'),
('Gia dụng', 'Đồ dùng gia đình'),
('Thực phẩm', 'Sản phẩm thực phẩm đóng gói');
GO

-- Product
INSERT INTO Product (product_name, brand, description, import_price, sell_price) VALUES
('Tivi 4K', 'Sony', 'Tivi 4K 55 inch', 12000000, 15000000),
('Máy giặt', 'LG', 'Máy giặt 8kg', 8000000, 10000000),
('Bánh quy', 'Orion', 'Bánh quy bơ 500g', 50000, 70000);
GO

-- ProductCategory
INSERT INTO ProductCategory (product_id, category_id) VALUES
(1, 1), -- Tivi 4K thuộc danh mục Điện tử
(2, 2), -- Máy giặt thuộc danh mục Gia dụng
(3, 3); -- Bánh quy thuộc danh mục Thực phẩm
GO

-- Import
INSERT INTO Import (import_date, supplier_id) VALUES
('2025-04-20', 1),
('2025-04-21', 2),
('2025-04-22', 3);
GO

-- ImportDetail
INSERT INTO ImportDetail (import_id, product_id, quantity, unit_price) VALUES
(1, 1, 10, 12000000), -- Nhập 10 Tivi 4K
(2, 2, 5, 8000000),   -- Nhập 5 Máy giặt
(3, 3, 100, 50000);   -- Nhập 100 Bánh quy
GO

-- Export
INSERT INTO Export (export_date, customer_id) VALUES
('2025-04-23', 1), -- Nguyễn Văn A
('2025-04-23', 2), -- Trần Thị B
('2025-04-24', 3); -- Lê Văn C
GO

-- ExportDetail
INSERT INTO ExportDetail (export_id, product_id, quantity, unit_price) VALUES
(1, 1, 2, 15000000), -- Xuất 2 Tivi 4K (cho Nguyễn Văn A)
(2, 2, 1, 10000000), -- Xuất 1 Máy giặt (cho Trần Thị B)
(3, 3, 20, 70000);   -- Xuất 20 Bánh quy (cho Lê Văn C)
GO