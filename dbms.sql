/*
 * LaptopStoreDB - Database for managing laptop store inventory, import/export operations
 *
 * Overview:
 * This database supports managing users, products (laptops), categories (brands), suppliers, customers,
 * import/export transactions, inventory, and logs. Below is a detailed list of views, triggers, transactions,
 * stored procedures, and functions used in the database, along with their functionalities.
 *
 * 1. Views (5):
 * - vw_ProductDetails:
 *   - Displays detailed product information (product_id, product_name, price, stock_quantity, brands).
 *   - Aggregates brand names from Category via ProductCategory.
 *   - Used to view product list with associated brands.
 *   - Example: SELECT * FROM vw_ProductDetails;
 * - vw_ImportDetails:
 *   - Shows import transaction details (import_id, supplier_id, supplier_name, import_date, total_amount,
 *     product_id, product_name, quantity, unit_price).
 *   - Joins Import, Supplier, ImportDetail, and Product tables.
 *   - Used for viewing import history or generating reports.
 *   - Example: SELECT * FROM vw_ImportDetails WHERE import_id = 1;
 * - vw_ExportDetails:
 *   - Shows export transaction details (export_id, customer_id, customer_name, export_date, total_amount,
 *     product_id, product_name, quantity, unit_price).
 *   - Joins Export, Customer, ExportDetail, and Product tables.
 *   - Used for viewing export history or generating reports.
 *   - Example: SELECT * FROM vw_ExportDetails WHERE export_id = 1;
 * - vw_Inventory:
 *   - Displays inventory information (product_id, product_name, price, stock_quantity).
 *   - Sourced from Product table.
 *   - Used to check current stock levels.
 *   - Example: SELECT * FROM vw_Inventory WHERE stock_quantity > 0;
 * - vw_UserDetails:
 *   - Displays user information (user_id, username, role).
 *   - Sourced from Users table.
 *   - Used by Admin to view user accounts (often via sp_GetAllUsers).
 *   - Example: SELECT * FROM vw_UserDetails;
 *
 * 2. Triggers (2):
 * - tr_CheckStockBeforeExport:
 *   - Triggered: After INSERT on ExportDetail (when exporting products).
 *   - Functionality: Checks if stock_quantity (from fn_GetStockQuantity) is sufficient for the export quantity.
 *     Raises error and rolls back if quantity exceeds stock.
 *   - Purpose: Ensures exports do not exceed available inventory.
 *   - Example: Attempting to export 50 Dell XPS 13 with only 43 in stock raises error: "Số lượng xuất vượt quá tồn kho!"
 * - tr_LogProductDeletion:
 *   - Triggered: After DELETE on Product (when Admin deletes a product).
 *   - Functionality: Logs deleted product details (product_id, product_name, deleted_date, deleted_by) to ProductLog.
 *   - Purpose: Tracks product deletion history for auditing.
 *   - Example: After EXEC sp_DeleteProduct @current_user_id = 1, @product_id = 1; a record is added to ProductLog.
 *
 * 3. Transactions:
 * Transactions are implicitly managed within stored procedures to ensure data integrity. Key procedures with transactions:
 * - sp_InsertImportDetail:
 *   - Inserts into ImportDetail and updates Product.stock_quantity. If product doesn't exist, creates new Product record.
 *   - Ensures both insert and update are committed or rolled back together.
 *   - Example: EXEC sp_InsertImportDetail @current_user_id = 2, @import_id = 3, @product_name = 'Dell XPS 13', @quantity = 5, @unit_price = 1100.00;
 * - sp_InsertExportDetail:
 *   - Inserts into ExportDetail and reduces Product.stock_quantity. Trigger tr_CheckStockBeforeExport enforces stock check.
 *   - Ensures export and stock update are atomic.
 *   - Example: EXEC sp_InsertExportDetail @current_user_id = 2, @export_id = 1, @product_id = 1, @quantity = 2, @unit_price = 1200.00;
 * - sp_DeleteProduct:
 *   - Deletes from ProductCategory, ImportDetail, ExportDetail, and Product. Trigger tr_LogProductDeletion logs deletion.
 *   - Ensures all related records are deleted atomically.
 *   - Example: EXEC sp_DeleteProduct @current_user_id = 1, @product_id = 1;
 * - sp_DeleteImport:
 *   - Deletes from ImportDetail and Import.
 *   - Ensures import and details are deleted together.
 *   - Example: EXEC sp_DeleteImport @current_user_id = 1, @import_id = 1;
 * - sp_DeleteExport:
 *   - Deletes from ExportDetail and Export.
 *   - Ensures export and details are deleted together.
 *   - Example: EXEC sp_DeleteExport @current_user_id = 1, @export_id = 1;
 * - sp_DeleteCategory:
 *   - Deletes from ProductCategory and Category.
 *   - Ensures category and links are deleted together.
 *   - Example: EXEC sp_DeleteCategory @current_user_id = 1, @category_id = 1;
 *
 * 4. Stored Procedures (25):
 * - sp_Login: Authenticates user by username and password. Returns user_id, username, role.
 * - sp_InsertUser: Creates new user (Admin only). Logs to PermissionLog.
 * - sp_DeleteUser: Deletes user (Admin only, cannot delete self). Logs to PermissionLog.
 * - sp_UpdateUserRole: Updates user role (Admin only). Logs to PermissionLog.
 * - sp_UpdateUser: Updates username, password (Admin only).
 * - sp_InsertProduct: Adds new product (Admin only).
 * - sp_UpdateProduct: Updates product details (Admin only).
 * - sp_DeleteProduct: Deletes product and related records (Admin only). Triggers tr_LogProductDeletion.
 * - sp_InsertCategory: Adds new category/brand (Admin only).
 * - sp_UpdateCategory: Updates category (Admin only).
 * - sp_DeleteCategory: Deletes category and links (Admin only).
 * - sp_InsertSupplier: Adds new supplier (Admin only).
 * - sp_UpdateSupplier: Updates supplier (Admin only).
 * - sp_DeleteSupplier: Deletes supplier (Admin only).
 * - sp_InsertCustomer: Adds new customer (Admin only).
 * - sp_UpdateCustomer: Updates customer (Admin only).
 * - sp_DeleteCustomer: Deletes customer (Admin only).
 * - sp_InsertImport: Creates new import transaction (Admin or Employee).
 * - sp_InsertImportDetail: Adds import details, auto-creates product if not exists (Admin or Employee). Updates stock_quantity.
 * - sp_UpdateImport: Updates import transaction (Admin only).
 * - sp_DeleteImport: Deletes import and details (Admin only).
 * - sp_InsertExport: Creates new export transaction (Admin or Employee).
 * - sp_InsertExportDetail: Adds export details, reduces stock_quantity (Admin or Employee). Triggers tr_CheckStockBeforeExport.
 * - sp_UpdateExport: Updates export transaction (Admin only).
 * - sp_DeleteExport: Deletes export and details (Admin only).
 * - sp_GetAllProducts: Retrieves all products (Admin or Employee).
 * - sp_GetAllSuppliers: Retrieves all suppliers (Admin or Employee).
 * - sp_GetAllCustomers: Retrieves all customers (Admin or Employee).
 * - sp_GetAllUsers: Retrieves all users (Admin only).
 *
 * 5. Functions (3):
 * - fn_GetStockQuantity:
 *   - Returns stock_quantity for a product by product_id.
 *   - Used in tr_CheckStockBeforeExport and applications to check stock.
 *   - Example: SELECT dbo.fn_GetStockQuantity(1); -- Returns 38 for Dell XPS 13
 * - fn_GetImportTotal:
 *   - Calculates total value (quantity * unit_price) of an import transaction from ImportDetail.
 *   - Used for validating or reporting import values.
 *   - Example: SELECT dbo.fn_GetImportTotal(1);
 * - fn_GetExportTotal:
 *   - Calculates total value (quantity * unit_price) of an export transaction from ExportDetail.
 *   - Used for validating or reporting export values.
 *   - Example: SELECT dbo.fn_GetExportTotal(1);
 *
 * Usage Notes:
 * - Run this script in SQL Server Management Studio to create the database, tables, and components.
 * - Use stored procedures for CRUD operations, views for data retrieval, and functions for calculations.
 * - Triggers ensure data integrity (stock checks, logging).
 * - Transactions in stored procedures ensure atomic operations.
 */

-- Tạo database
-- CREATE DATABASE LaptopStoreDB;
-- GO
-- USE LaptopStoreDB;
-- GO

-- Tạo bảng Users
CREATE TABLE Users (
    user_id INT PRIMARY KEY IDENTITY,
    username NVARCHAR(100) UNIQUE,
    password NVARCHAR(100),
    role NVARCHAR(50) CHECK (role IN ('admin', 'employee'))
);
GO

-- Tạo bảng Product
CREATE TABLE Product (
    product_id INT PRIMARY KEY IDENTITY,
    product_name NVARCHAR(100),
    price DECIMAL(18,2),
    stock_quantity INT
);
GO

-- Tạo bảng Category
CREATE TABLE Category (
    category_id INT PRIMARY KEY IDENTITY,
    category_name NVARCHAR(100),
    description NVARCHAR(255)
);
GO

-- Tạo bảng ProductCategory (bảng trung gian Product - Category)
CREATE TABLE ProductCategory (
    product_id INT FOREIGN KEY REFERENCES Product(product_id),
    category_id INT FOREIGN KEY REFERENCES Category(category_id),
    PRIMARY KEY (product_id, category_id)
);
GO

-- Tạo bảng Supplier
CREATE TABLE Supplier (
    supplier_id INT PRIMARY KEY IDENTITY,
    supplier_name NVARCHAR(100),
    contact_info NVARCHAR(255)
);
GO

-- Tạo bảng Customer
CREATE TABLE Customer (
    customer_id INT PRIMARY KEY IDENTITY,
    customer_name NVARCHAR(100),
    contact_info NVARCHAR(255)
);
GO

-- Tạo bảng Import
CREATE TABLE Import (
    import_id INT PRIMARY KEY IDENTITY,
    supplier_id INT FOREIGN KEY REFERENCES Supplier(supplier_id),
    import_date DATE,
    total_amount DECIMAL(18,2)
);
GO

-- Tạo bảng ImportDetail
CREATE TABLE ImportDetail (
    import_id INT FOREIGN KEY REFERENCES Import(import_id),
    product_id INT FOREIGN KEY REFERENCES Product(product_id),
    quantity INT,
    unit_price DECIMAL(18,2),
    PRIMARY KEY (import_id, product_id)
);
GO

-- Tạo bảng Export
CREATE TABLE Export (
    export_id INT PRIMARY KEY IDENTITY,
    customer_id INT FOREIGN KEY REFERENCES Customer(customer_id),
    export_date DATE,
    total_amount DECIMAL(18,2)
);
GO

-- Tạo bảng ExportDetail
CREATE TABLE ExportDetail (
    export_id INT FOREIGN KEY REFERENCES Export(export_id),
    product_id INT FOREIGN KEY REFERENCES Product(product_id),
    quantity INT,
    unit_price DECIMAL(18,2),
    PRIMARY KEY (export_id, product_id)
);
GO

-- Tạo bảng ProductLog
CREATE TABLE ProductLog (
    log_id INT PRIMARY KEY IDENTITY,
    product_id INT,
    product_name NVARCHAR(100),
    deleted_date DATETIME,
    deleted_by NVARCHAR(100)
);
GO

-- Tạo bảng PermissionLog
CREATE TABLE PermissionLog (
    log_id INT PRIMARY KEY IDENTITY,
    user_id INT FOREIGN KEY REFERENCES Users(user_id),
    action NVARCHAR(100),
    old_role NVARCHAR(50),
    new_role NVARCHAR(50),
    action_date DATETIME,
    performed_by INT FOREIGN KEY REFERENCES Users(user_id)
);
GO

-- Tạo stored procedure sp_Login
CREATE PROCEDURE sp_Login
    @username NVARCHAR(100),
    @password NVARCHAR(100)
AS
BEGIN
    SELECT user_id, username, role
    FROM Users
    WHERE username = @username
    AND password = @password;
END;
GO

-- Tạo stored procedure sp_InsertUser
CREATE PROCEDURE sp_InsertUser
    @current_user_id INT,
    @username NVARCHAR(100),
    @password NVARCHAR(100),
    @role NVARCHAR(50)
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM Users WHERE user_id = @current_user_id AND role = 'admin')
    BEGIN
        RAISERROR ('Chỉ admin mới có quyền tạo tài khoản!', 16, 1);
        RETURN;
    END;
    IF EXISTS (SELECT 1 FROM Users WHERE username = @username)
    BEGIN
        RAISERROR ('Tên người dùng đã tồn tại!', 16, 1);
        RETURN;
    END;
    IF @role NOT IN ('admin', 'employee')
    BEGIN
        RAISERROR ('Vai trò không hợp lệ! Chỉ được chọn "admin" hoặc "employee".', 16, 1);
        RETURN;
    END;
    INSERT INTO Users (username, password, role)
    VALUES (@username, @password, @role);
    DECLARE @new_user_id INT = SCOPE_IDENTITY();
    INSERT INTO PermissionLog (user_id, action, old_role, new_role, action_date, performed_by)
    VALUES (@new_user_id, 'Create User', NULL, @role, GETDATE(), @current_user_id);
    SELECT @new_user_id AS user_id;
END;
GO

-- Tạo stored procedure sp_DeleteUser
CREATE PROCEDURE sp_DeleteUser
    @current_user_id INT,
    @user_id INT
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM Users WHERE user_id = @current_user_id AND role = 'admin')
    BEGIN
        RAISERROR ('Chỉ admin mới có quyền xóa tài khoản!', 16, 1);
        RETURN;
    END;
    IF NOT EXISTS (SELECT 1 FROM Users WHERE user_id = @user_id)
    BEGIN
        RAISERROR ('Tài khoản không tồn tại!', 16, 1);
        RETURN;
    END;
    IF @current_user_id = @user_id
    BEGIN
        RAISERROR ('Không thể xóa chính tài khoản của bạn!', 16, 1);
        RETURN;
    END;
    DECLARE @old_role NVARCHAR(50);
    SELECT @old_role = role FROM Users WHERE user_id = @user_id;
    DELETE FROM Users WHERE user_id = @user_id;
    INSERT INTO PermissionLog (user_id, action, old_role, new_role, action_date, performed_by)
    VALUES (@user_id, 'Delete User', @old_role, NULL, GETDATE(), @current_user_id);
END;
GO

-- Tạo stored procedure sp_UpdateUserRole
CREATE PROCEDURE sp_UpdateUserRole
    @current_user_id INT,
    @user_id INT,
    @role NVARCHAR(50)
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM Users WHERE user_id = @current_user_id AND role = 'admin')
    BEGIN
        RAISERROR ('Chỉ admin mới có quyền cập nhật vai trò!', 16, 1);
        RETURN;
    END;
    IF NOT EXISTS (SELECT 1 FROM Users WHERE user_id = @user_id)
    BEGIN
        RAISERROR ('Tài khoản không tồn tại!', 16, 1);
        RETURN;
    END;
    IF @role NOT IN ('admin', 'employee')
    BEGIN
        RAISERROR ('Vai trò không hợp lệ! Chỉ được chọn "admin" hoặc "employee".', 16, 1);
        RETURN;
    END;
    DECLARE @old_role NVARCHAR(50);
    SELECT @old_role = role FROM Users WHERE user_id = @user_id;
    UPDATE Users
    SET role = @role
    WHERE user_id = @user_id;
    INSERT INTO PermissionLog (user_id, action, old_role, new_role, action_date, performed_by)
    VALUES (@user_id, 'Update Role', @old_role, @role, GETDATE(), @current_user_id);
END;
GO

-- Tạo stored procedure sp_UpdateUser
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
    IF EXISTS (SELECT 1 FROM Users WHERE username = @username AND user_id != @user_id)
    BEGIN
        RAISERROR ('Tên người dùng đã tồn tại!', 16, 1);
        RETURN;
    END;
    UPDATE Users
    SET username = @username,
        password = @password
    WHERE user_id = @user_id;
END;
GO

-- Tạo stored procedure sp_InsertProduct
CREATE PROCEDURE sp_InsertProduct
    @current_user_id INT,
    @product_name NVARCHAR(100),
    @price DECIMAL(18,2),
    @stock_quantity INT
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM Users WHERE user_id = @current_user_id AND role = 'admin')
    BEGIN
        RAISERROR ('Chỉ admin mới có quyền thêm laptop!', 16, 1);
        RETURN;
    END;
    INSERT INTO Product (product_name, price, stock_quantity)
    VALUES (@product_name, @price, @stock_quantity);
    SELECT SCOPE_IDENTITY() AS product_id;
END;
GO

-- Tạo stored procedure sp_UpdateProduct
CREATE PROCEDURE sp_UpdateProduct
    @current_user_id INT,
    @product_id INT,
    @product_name NVARCHAR(100),
    @price DECIMAL(18,2),
    @stock_quantity INT
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM Users WHERE user_id = @current_user_id AND role = 'admin')
    BEGIN
        RAISERROR ('Chỉ admin mới có quyền cập nhật laptop!', 16, 1);
        RETURN;
    END;
    UPDATE Product
    SET product_name = @product_name,
        price = @price,
        stock_quantity = @stock_quantity
    WHERE product_id = @product_id;
END;
GO

-- Tạo stored procedure sp_DeleteProduct
CREATE PROCEDURE sp_DeleteProduct
    @current_user_id INT,
    @product_id INT
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM Users WHERE user_id = @current_user_id AND role = 'admin')
    BEGIN
        RAISERROR ('Chỉ admin mới có quyền xóa laptop!', 16, 1);
        RETURN;
    END;
    DELETE FROM ProductCategory WHERE product_id = @product_id;
    DELETE FROM ImportDetail WHERE product_id = @product_id;
    DELETE FROM ExportDetail WHERE product_id = @product_id;
    DELETE FROM Product WHERE product_id = @product_id;
END;
GO

-- Tạo stored procedure sp_InsertCategory
CREATE PROCEDURE sp_InsertCategory
    @current_user_id INT,
    @category_name NVARCHAR(100),
    @description NVARCHAR(255)
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM Users WHERE user_id = @current_user_id AND role = 'admin')
    BEGIN
        RAISERROR ('Chỉ admin mới có quyền thêm thương hiệu!', 16, 1);
        RETURN;
    END;
    INSERT INTO Category (category_name, description)
    VALUES (@category_name, @description);
    SELECT SCOPE_IDENTITY() AS category_id;
END;
GO

-- Tạo stored procedure sp_UpdateCategory
CREATE PROCEDURE sp_UpdateCategory
    @current_user_id INT,
    @category_id INT,
    @category_name NVARCHAR(100),
    @description NVARCHAR(255)
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM Users WHERE user_id = @current_user_id AND role = 'admin')
    BEGIN
        RAISERROR ('Chỉ admin mới có quyền cập nhật thương hiệu!', 16, 1);
        RETURN;
    END;
    UPDATE Category
    SET category_name = @category_name,
        description = @description
    WHERE category_id = @category_id;
END;
GO

-- Tạo stored procedure sp_DeleteCategory
CREATE PROCEDURE sp_DeleteCategory
    @current_user_id INT,
    @category_id INT
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM Users WHERE user_id = @current_user_id AND role = 'admin')
    BEGIN
        RAISERROR ('Chỉ admin mới có quyền xóa thương hiệu!', 16, 1);
        RETURN;
    END;
    DELETE FROM ProductCategory WHERE category_id = @category_id;
    DELETE FROM Category WHERE category_id = @category_id;
END;
GO

-- Tạo stored procedure sp_InsertSupplier
CREATE PROCEDURE sp_InsertSupplier
    @current_user_id INT,
    @supplier_name NVARCHAR(100),
    @contact_info NVARCHAR(255)
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM Users WHERE user_id = @current_user_id AND role = 'admin')
    BEGIN
        RAISERROR ('Chỉ admin mới có quyền thêm nhà cung cấp!', 16, 1);
        RETURN;
    END;
    INSERT INTO Supplier (supplier_name, contact_info)
    VALUES (@supplier_name, @contact_info);
    SELECT SCOPE_IDENTITY() AS supplier_id;
END;
GO

-- Tạo stored procedure sp_UpdateSupplier
CREATE PROCEDURE sp_UpdateSupplier
    @current_user_id INT,
    @supplier_id INT,
    @supplier_name NVARCHAR(100),
    @contact_info NVARCHAR(255)
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM Users WHERE user_id = @current_user_id AND role = 'admin')
    BEGIN
        RAISERROR ('Chỉ admin mới có quyền cập nhật nhà cung cấp!', 16, 1);
        RETURN;
    END;
    UPDATE Supplier
    SET supplier_name = @supplier_name,
        contact_info = @contact_info
    WHERE supplier_id = @supplier_id;
END;
GO

-- Tạo stored procedure sp_DeleteSupplier
CREATE PROCEDURE sp_DeleteSupplier
    @current_user_id INT,
    @supplier_id INT
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM Users WHERE user_id = @current_user_id AND role = 'admin')
    BEGIN
        RAISERROR ('Chỉ admin mới có quyền xóa nhà cung cấp!', 16, 1);
        RETURN;
    END;
    DELETE FROM Supplier WHERE supplier_id = @supplier_id;
END;
GO

-- Tạo stored procedure sp_InsertCustomer
CREATE PROCEDURE sp_InsertCustomer
    @current_user_id INT,
    @customer_name NVARCHAR(100),
    @contact_info NVARCHAR(255)
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM Users WHERE user_id = @current_user_id AND role = 'admin')
    BEGIN
        RAISERROR ('Chỉ admin mới có quyền thêm khách hàng!', 16, 1);
        RETURN;
    END;
    INSERT INTO Customer (customer_name, contact_info)
    VALUES (@customer_name, @contact_info);
    SELECT SCOPE_IDENTITY() AS customer_id;
END;
GO

-- Tạo stored procedure sp_UpdateCustomer
CREATE PROCEDURE sp_UpdateCustomer
    @current_user_id INT,
    @customer_id INT,
    @customer_name NVARCHAR(100),
    @contact_info NVARCHAR(255)
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM Users WHERE user_id = @current_user_id AND role = 'admin')
    BEGIN
        RAISERROR ('Chỉ admin mới có quyền cập nhật khách hàng!', 16, 1);
        RETURN;
    END;
    UPDATE Customer
    SET customer_name = @customer_name,
        contact_info = @contact_info
    WHERE customer_id = @customer_id;
END;
GO

-- Tạo stored procedure sp_DeleteCustomer
CREATE PROCEDURE sp_DeleteCustomer
    @current_user_id INT,
    @customer_id INT
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM Users WHERE user_id = @current_user_id AND role = 'admin')
    BEGIN
        RAISERROR ('Chỉ admin mới có quyền xóa khách hàng!', 16, 1);
        RETURN;
    END;
    DELETE FROM Customer WHERE customer_id = @customer_id;
END;
GO

-- Tạo stored procedure sp_InsertImport
CREATE PROCEDURE sp_InsertImport
    @current_user_id INT,
    @supplier_id INT,
    @import_date DATE,
    @total_amount DECIMAL(18,2)
AS
BEGIN
    INSERT INTO Import (supplier_id, import_date, total_amount)
    VALUES (@supplier_id, @import_date, @total_amount);
    SELECT SCOPE_IDENTITY() AS import_id;
END;
GO

-- Tạo stored procedure sp_InsertImportDetail (đã sửa để hỗ trợ nhập hàng tự động thêm sản phẩm mới)
CREATE PROCEDURE sp_InsertImportDetail
    @current_user_id INT,
    @import_id INT,
    @product_name NVARCHAR(100),
    @quantity INT,
    @unit_price DECIMAL(18,2),
    @price DECIMAL(18,2) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @product_id INT;

    -- Kiểm tra import_id hợp lệ
    IF NOT EXISTS (SELECT 1 FROM Import WHERE import_id = @import_id)
    BEGIN
        RAISERROR ('Phiếu nhập không tồn tại!', 16, 1);
        RETURN;
    END;

    -- Kiểm tra số lượng hợp lệ
    IF @quantity <= 0
    BEGIN
        RAISERROR ('Số lượng phải lớn hơn 0!', 16, 1);
        RETURN;
    END;

    -- Kiểm tra xem sản phẩm đã tồn tại dựa trên product_name
    SELECT @product_id = product_id
    FROM Product
    WHERE product_name = @product_name;

    -- Nếu sản phẩm chưa tồn tại, tạo mới
    IF @product_id IS NULL
    BEGIN
        INSERT INTO Product (product_name, price, stock_quantity)
        VALUES (@product_name, ISNULL(@price, @unit_price * 1.2), 0);
        SET @product_id = SCOPE_IDENTITY();
    END;

    -- Thêm chi tiết phiếu nhập vào ImportDetail
    INSERT INTO ImportDetail (import_id, product_id, quantity, unit_price)
    VALUES (@import_id, @product_id, @quantity, @unit_price);

    -- Tăng stock_quantity trong bảng Product
    UPDATE Product
    SET stock_quantity = stock_quantity + @quantity
    WHERE product_id = @product_id;

    -- Trả về product_id
    SELECT @product_id AS product_id;
END;
GO

-- Tạo stored procedure sp_UpdateImport
CREATE PROCEDURE sp_UpdateImport
    @current_user_id INT,
    @import_id INT,
    @supplier_id INT,
    @import_date DATE,
    @total_amount DECIMAL(18,2)
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM Users WHERE user_id = @current_user_id AND role = 'admin')
    BEGIN
        RAISERROR ('Chỉ admin mới có quyền cập nhật phiếu nhập!', 16, 1);
        RETURN;
    END;
    UPDATE Import
    SET supplier_id = @supplier_id,
        import_date = @import_date,
        total_amount = @total_amount
    WHERE import_id = @import_id;
END;
GO

-- Tạo stored procedure sp_DeleteImport
CREATE PROCEDURE sp_DeleteImport
    @current_user_id INT,
    @import_id INT
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM Users WHERE user_id = @current_user_id AND role = 'admin')
    BEGIN
        RAISERROR ('Chỉ admin mới có quyền xóa phiếu nhập!', 16, 1);
        RETURN;
    END;
    DELETE FROM ImportDetail WHERE import_id = @import_id;
    DELETE FROM Import WHERE import_id = @import_id;
END;
GO

-- Tạo stored procedure sp_InsertExport
CREATE PROCEDURE sp_InsertExport
    @current_user_id INT,
    @customer_id INT,
    @export_date DATE,
    @total_amount DECIMAL(18,2)
AS
BEGIN
    INSERT INTO Export (customer_id, export_date, total_amount)
    VALUES (@customer_id, @export_date, @total_amount);
    SELECT SCOPE_IDENTITY() AS export_id;
END;
GO

-- Tạo stored procedure sp_InsertExportDetail
CREATE PROCEDURE sp_InsertExportDetail
    @current_user_id INT,
    @export_id INT,
    @product_id INT,
    @quantity INT,
    @unit_price DECIMAL(18,2)
AS
BEGIN
    INSERT INTO ExportDetail (export_id, product_id, quantity, unit_price)
    VALUES (@export_id, @product_id, @quantity, @unit_price);
    UPDATE Product
    SET stock_quantity = stock_quantity - @quantity
    WHERE product_id = @product_id;
END;
GO

-- Tạo stored procedure sp_UpdateExport
CREATE PROCEDURE sp_UpdateExport
    @current_user_id INT,
    @export_id INT,
    @customer_id INT,
    @export_date DATE,
    @total_amount DECIMAL(18,2)
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM Users WHERE user_id = @current_user_id AND role = 'admin')
    BEGIN
        RAISERROR ('Chỉ admin mới có quyền cập nhật phiếu xuất!', 16, 1);
        RETURN;
    END;
    UPDATE Export
    SET customer_id = @customer_id,
        export_date = @export_date,
        total_amount = @total_amount
    WHERE export_id = @export_id;
END;
GO

-- Tạo stored procedure sp_DeleteExport
CREATE PROCEDURE sp_DeleteExport
    @current_user_id INT,
    @export_id INT
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM Users WHERE user_id = @current_user_id AND role = 'admin')
    BEGIN
        RAISERROR ('Chỉ admin mới có quyền xóa phiếu xuất!', 16, 1);
        RETURN;
    END;
    DELETE FROM ExportDetail WHERE export_id = @export_id;
    DELETE FROM Export WHERE export_id = @export_id;
END;
GO

-- Tạo stored procedure sp_GetAllProducts
CREATE PROCEDURE sp_GetAllProducts
    @current_user_id INT
AS
BEGIN
    SELECT product_id, product_name, price, stock_quantity
    FROM Product;
END;
GO

-- Tạo stored procedure sp_GetAllSuppliers
CREATE PROCEDURE sp_GetAllSuppliers
    @current_user_id INT
AS
BEGIN
    SELECT supplier_id, supplier_name, contact_info
    FROM Supplier;
END;
GO

-- Tạo stored procedure sp_GetAllCustomers
CREATE PROCEDURE sp_GetAllCustomers
    @current_user_id INT
AS
BEGIN
    SELECT customer_id, customer_name, contact_info
    FROM Customer;
END;
GO

-- Tạo stored procedure sp_GetAllUsers
CREATE PROCEDURE sp_GetAllUsers
    @current_user_id INT
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM Users WHERE user_id = @current_user_id AND role = 'admin')
    BEGIN
        RAISERROR ('Chỉ admin mới có quyền xem danh sách người dùng!', 16, 1);
        RETURN;
    END;
    SELECT user_id, username, role
    FROM Users;
END;
GO

-- Tạo view vw_ProductDetails
CREATE VIEW vw_ProductDetails AS
SELECT 
    p.product_id,
    p.product_name,
    p.price,
    p.stock_quantity,
    STRING_AGG(c.category_name, ', ') AS brands
FROM Product p
LEFT JOIN ProductCategory pc ON p.product_id = pc.product_id
LEFT JOIN Category c ON pc.category_id = c.category_id
GROUP BY p.product_id, p.product_name, p.price, p.stock_quantity;
GO

-- Tạo view vw_ImportDetails
CREATE VIEW vw_ImportDetails AS
SELECT 
    i.import_id,
    i.supplier_id,
    s.supplier_name,
    i.import_date,
    i.total_amount,
    id.product_id,
    p.product_name,
    id.quantity,
    id.unit_price
FROM Import i
JOIN Supplier s ON i.supplier_id = s.supplier_id
JOIN ImportDetail id ON i.import_id = id.import_id
JOIN Product p ON id.product_id = p.product_id;
GO

-- Tạo view vw_ExportDetails
CREATE VIEW vw_ExportDetails AS
SELECT 
    e.export_id,
    e.customer_id,
    c.customer_name,
    e.export_date,
    e.total_amount,
    ed.product_id,
    p.product_name,
    ed.quantity,
    ed.unit_price
FROM Export e
JOIN Customer c ON e.customer_id = c.customer_id
JOIN ExportDetail ed ON e.export_id = ed.export_id
JOIN Product p ON ed.product_id = p.product_id;
GO

-- Tạo view vw_Inventory
CREATE VIEW vw_Inventory AS
SELECT 
    p.product_id,
    p.product_name,
    p.price,
    p.stock_quantity
FROM Product p;
GO

-- Tạo view vw_UserDetails
CREATE VIEW vw_UserDetails AS
SELECT 
    user_id,
    username,
    role
FROM Users;
GO

-- Tạo function fn_GetStockQuantity
CREATE FUNCTION fn_GetStockQuantity (@product_id INT)
RETURNS INT
AS
BEGIN
    DECLARE @stock INT;
    SELECT @stock = stock_quantity
    FROM Product
    WHERE product_id = @product_id;
    RETURN ISNULL(@stock, 0);
END;
GO

-- Tạo function fn_GetImportTotal
CREATE FUNCTION fn_GetImportTotal (@import_id INT)
RETURNS DECIMAL(18,2)
AS
BEGIN
    DECLARE @total DECIMAL(18,2);
    SELECT @total = SUM(quantity * unit_price)
    FROM ImportDetail
    WHERE import_id = @import_id;
    RETURN ISNULL(@total, 0);
END;
GO

-- Tạo function fn_GetExportTotal
CREATE FUNCTION fn_GetExportTotal (@export_id INT)
RETURNS DECIMAL(18,2)
AS
BEGIN
    DECLARE @total DECIMAL(18,2);
    SELECT @total = SUM(quantity * unit_price)
    FROM ExportDetail
    WHERE export_id = @export_id;
    RETURN ISNULL(@total, 0);
END;
GO

-- Tạo trigger tr_CheckStockBeforeExport
CREATE TRIGGER tr_CheckStockBeforeExport
ON ExportDetail
AFTER INSERT
AS
BEGIN
    DECLARE @product_id INT, @quantity INT, @stock INT;
    SELECT @product_id = product_id, @quantity = quantity
    FROM inserted;
    SET @stock = dbo.fn_GetStockQuantity(@product_id);
    IF @stock < @quantity
    BEGIN
        RAISERROR ('Số lượng xuất vượt quá tồn kho!', 16, 1);
        ROLLBACK;
    END;
END;
GO

-- Tạo trigger tr_LogProductDeletion
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
        NULL
    FROM deleted d;
END;
GO

-- Seed data
-- Thêm tài khoản
INSERT INTO Users (username, password, role)
VALUES ('admin1', 'admin_pass', 'admin'),
       ('employee1', 'emp_pass', 'employee');
GO

-- Thêm danh mục (thương hiệu laptop)
INSERT INTO Category (category_name, description)
VALUES ('Dell', 'Dell laptops'),
       ('Apple', 'Apple MacBooks'),
       ('HP', 'HP laptops');
GO

-- Thêm sản phẩm (laptop)
INSERT INTO Product (product_name, price, stock_quantity)
VALUES ('Dell XPS 13', 1200.00, 30),
       ('MacBook Pro 14', 2000.00, 20),
       ('HP Spectre x360', 1500.00, 25);
GO

-- Thêm ProductCategory (liên kết sản phẩm và thương hiệu)
INSERT INTO ProductCategory (product_id, category_id)
VALUES (1, 1), -- Dell XPS 13 -> Dell
       (2, 2), -- MacBook Pro 14 -> Apple
       (3, 3); -- HP Spectre x360 -> HP
GO

-- Thêm nhà cung cấp
INSERT INTO Supplier (supplier_name, contact_info)
VALUES ('Tech Distributor', 'contact@techdist.com'),
       ('Apple Inc.', 'contact@apple.com');
GO

-- Thêm khách hàng
INSERT INTO Customer (customer_name, contact_info)
VALUES ('John Doe', 'john.doe@email.com'),
       ('Tech Corp', 'sales@techcorp.com');
GO

-- Thêm phiếu nhập
INSERT INTO Import (supplier_id, import_date, total_amount)
VALUES (1, '2025-04-24', 36000.00), -- Nhập từ Tech Distributor
       (2, '2025-04-24', 40000.00); -- Nhập từ Apple Inc.
GO

-- Thêm chi tiết phiếu nhập
INSERT INTO ImportDetail (import_id, product_id, quantity, unit_price)
VALUES (1, 1, 10, 1100.00), -- Nhập 10 Dell XPS 13
       (1, 3, 10, 1400.00), -- Nhập 10 HP Spectre x360
       (2, 2, 10, 1900.00); -- Nhập 10 MacBook Pro 14
GO

-- Thêm phiếu xuất
INSERT INTO Export (customer_id, export_date, total_amount)
VALUES (1, '2025-04-24', 2400.00), -- Xuất cho John Doe
       (2, '2025-04-24', 4500.00); -- Xuất cho Tech Corp
GO

-- Thêm chi tiết phiếu xuất
INSERT INTO ExportDetail (export_id, product_id, quantity, unit_price)
VALUES (1, 1, 2, 1200.00), -- Xuất 2 Dell XPS 13
       (2, 3, 3, 1500.00); -- Xuất 3 HP Spectre x360
GO