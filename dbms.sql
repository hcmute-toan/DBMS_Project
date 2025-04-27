/*
 * LaptopStoreDB - Database for managing laptop store inventory, import/export operations
 *
 * Overview:
 * This database supports managing users, products (laptops), categories (brands), suppliers, customers,
 * import/export transactions, inventory, and logs. It includes views, triggers, transactions,
 * stored procedures, and functions to ensure data integrity and usability.
 *
 * Key Features:
 * - Handles foreign key constraints with ON DELETE NO ACTION for PermissionLog.user_id and performed_by.
 * - Manually manages PermissionLog references before deleting users.
 * - Prevents deletion of suppliers/customers with related imports/exports.
 * - Logs user actions and product deletions while maintaining data integrity.
 *
 * Usage Notes:
 * - Run this script in SQL Server Management Studio to create the database and components.
 * - Use stored procedures for CRUD operations, views for data retrieval, and functions for calculations.
 */

-- Tạo database
CREATE DATABASE LaptopStoreDBMS;
GO
USE LaptopStoreDBMS
GO

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
    product_name NVARCHAR(100) UNIQUE,
    price DECIMAL(18,2),
    stock_quantity INT
);
GO

-- Tạo bảng Category
CREATE TABLE Category (
    category_id INT PRIMARY KEY IDENTITY,
    category_name NVARCHAR(100) UNIQUE,
    description NVARCHAR(255)
);
GO

-- Tạo bảng ProductCategory
CREATE TABLE ProductCategory (
    product_id INT,
    category_id INT,
    PRIMARY KEY (product_id, category_id),
    FOREIGN KEY (product_id) REFERENCES Product(product_id) ON DELETE CASCADE,
    FOREIGN KEY (category_id) REFERENCES Category(category_id) ON DELETE CASCADE
);
GO

-- Tạo bảng Supplier
CREATE TABLE Supplier (
    supplier_id INT PRIMARY KEY IDENTITY,
    supplier_name NVARCHAR(100) UNIQUE,
    contact_info NVARCHAR(255)
);
GO

-- Tạo bảng Customer
CREATE TABLE Customer (
    customer_id INT PRIMARY KEY IDENTITY,
    customer_name NVARCHAR(100) UNIQUE,
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
    import_id INT,
    product_id INT,
    quantity INT,
    unit_price DECIMAL(18,2),
    PRIMARY KEY (import_id, product_id),
    FOREIGN KEY (import_id) REFERENCES Import(import_id) ON DELETE CASCADE,
    FOREIGN KEY (product_id) REFERENCES Product(product_id) ON DELETE CASCADE
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
    export_id INT,
    product_id INT,
    quantity INT,
    unit_price DECIMAL(18,2),
    PRIMARY KEY (export_id, product_id),
    FOREIGN KEY (export_id) REFERENCES Export(export_id) ON DELETE CASCADE,
    FOREIGN KEY (product_id) REFERENCES Product(product_id) ON DELETE CASCADE
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
    user_id INT,
    username NVARCHAR(100),
    action NVARCHAR(100),
    old_role NVARCHAR(50),
    new_role NVARCHAR(50),
    action_date DATETIME,
    performed_by INT,
    performed_by_username NVARCHAR(100),
    FOREIGN KEY (user_id) REFERENCES Users(user_id) ON DELETE SET NULL,
    FOREIGN KEY (performed_by) REFERENCES Users(user_id) ON DELETE NO ACTION
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
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        IF NOT EXISTS (SELECT 1 FROM Users WHERE user_id = @current_user_id AND role = 'admin')
        BEGIN
            RAISERROR ('Chỉ admin mới có quyền tạo tài khoản!', 16, 1);
            ROLLBACK;
            RETURN;
        END;
        IF @role NOT IN ('admin', 'employee')
        BEGIN
            RAISERROR ('Vai trò không hợp lệ! Chỉ được chọn "admin" hoặc "employee".', 16, 1);
            ROLLBACK;
            RETURN;
        END;
        IF EXISTS (SELECT 1 FROM Users WHERE username = @username)
        BEGIN
            RAISERROR ('Tài khoản đã tồn tại!', 16, 1);
            ROLLBACK;
            RETURN;
        END;
        INSERT INTO Users (username, password, role)
        VALUES (@username, @password, @role);
        DECLARE @new_user_id INT = SCOPE_IDENTITY();
        DECLARE @current_username NVARCHAR(100);
        SELECT @current_username = username FROM Users WHERE user_id = @current_user_id;
        INSERT INTO PermissionLog (user_id, username, action, old_role, new_role, action_date, performed_by, performed_by_username)
        VALUES (@new_user_id, @username, 'Create User', NULL, @role, GETDATE(), @current_user_id, @current_username);
        COMMIT TRANSACTION;
        SELECT @new_user_id AS user_id;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR (@ErrorMessage, 16, 1);
    END CATCH;
END;
GO

-- Tạo stored procedure sp_DeleteUser
CREATE PROCEDURE sp_DeleteUser
    @current_user_id INT,
    @user_id INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        IF NOT EXISTS (SELECT 1 FROM Users WHERE user_id = @current_user_id AND role = 'admin')
        BEGIN
            RAISERROR ('Chỉ admin mới có quyền xóa tài khoản!', 16, 1);
            ROLLBACK;
            RETURN;
        END;
        IF NOT EXISTS (SELECT 1 FROM Users WHERE user_id = @user_id)
        BEGIN
            RAISERROR ('Tài khoản không tồn tại!', 16, 1);
            ROLLBACK;
            RETURN;
        END;
        IF @current_user_id = @user_id
        BEGIN
            RAISERROR ('Không thể xóa chính tài khoản của bạn!', 16, 1);
            ROLLBACK;
            RETURN;
        END;

        -- Variables for logging
        DECLARE @old_role NVARCHAR(50);
        DECLARE @deleted_username NVARCHAR(100);
        DECLARE @current_username NVARCHAR(100);
        SELECT @old_role = role, @deleted_username = username FROM Users WHERE user_id = @user_id;
        SELECT @current_username = username FROM Users WHERE user_id = @current_user_id;

        -- Set user_id and performed_by to NULL in PermissionLog for the user being deleted
        UPDATE PermissionLog
        SET user_id = NULL
        WHERE user_id = @user_id;

        UPDATE PermissionLog
        SET performed_by = NULL
        WHERE performed_by = @user_id;

        -- Insert into PermissionLog before deletion
        INSERT INTO PermissionLog (user_id, username, action, old_role, new_role, action_date, performed_by, performed_by_username)
        VALUES (@user_id, @deleted_username, 'Delete User', @old_role, NULL, GETDATE(), @current_user_id, @current_username);

        -- Now delete the user
        DELETE FROM Users WHERE user_id = @user_id;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR (@ErrorMessage, 16, 1);
    END CATCH;
END;
GO

-- Tạo stored procedure sp_UpdateUserRole
CREATE PROCEDURE sp_UpdateUserRole
    @current_user_id INT,
    @user_id INT,
    @role NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        IF NOT EXISTS (SELECT 1 FROM Users WHERE user_id = @current_user_id AND role = 'admin')
        BEGIN
            RAISERROR ('Chỉ admin mới có quyền cập nhật vai trò!', 16, 1);
            ROLLBACK;
            RETURN;
        END;
        IF NOT EXISTS (SELECT 1 FROM Users WHERE user_id = @user_id)
        BEGIN
            RAISERROR ('Tài khoản không tồn tại!', 16, 1);
            ROLLBACK;
            RETURN;
        END;
        IF @role NOT IN ('admin', 'employee')
        BEGIN
            RAISERROR ('Vai trò không hợp lệ! Chỉ được chọn "admin" hoặc "employee".', 16, 1);
            ROLLBACK;
            RETURN;
        END;
        DECLARE @old_role NVARCHAR(50);
        DECLARE @target_username NVARCHAR(100);
        DECLARE @current_username NVARCHAR(100);
        SELECT @old_role = role, @target_username = username FROM Users WHERE user_id = @user_id;
        SELECT @current_username = username FROM Users WHERE user_id = @current_user_id;
        UPDATE Users
        SET role = @role
        WHERE user_id = @user_id;
        INSERT INTO PermissionLog (user_id, username, action, old_role, new_role, action_date, performed_by, performed_by_username)
        VALUES (@user_id, @target_username, 'Update Role', @old_role, @role, GETDATE(), @current_user_id, @current_username);
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR (@ErrorMessage, 16, 1);
    END CATCH;
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
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        IF NOT EXISTS (SELECT 1 FROM Users WHERE user_id = @current_user_id AND role = 'admin')
        BEGIN
            RAISERROR ('Chỉ admin mới có quyền cập nhật thông tin người dùng!', 16, 1);
            ROLLBACK;
            RETURN;
        END;
        IF NOT EXISTS (SELECT 1 FROM Users WHERE user_id = @user_id)
        BEGIN
            RAISERROR ('Tài khoản không tồn tại!', 16, 1);
            ROLLBACK;
            RETURN;
        END;
        IF EXISTS (SELECT 1 FROM Users WHERE username = @username AND user_id != @user_id)
        BEGIN
            RAISERROR ('Tài khoản đã tồn tại!', 16, 1);
            ROLLBACK;
            RETURN;
        END;
        UPDATE Users
        SET username = @username,
            password = CASE 
                          WHEN @password IS NULL THEN password
                          ELSE @password 
                       END
        WHERE user_id = @user_id;
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR (@ErrorMessage, 16, 1);
    END CATCH;
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
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        IF NOT EXISTS (SELECT 1 FROM Users WHERE user_id = @current_user_id AND role = 'admin')
        BEGIN
            RAISERROR ('Chỉ admin mới có quyền thêm laptop!', 16, 1);
            ROLLBACK;
            RETURN;
        END;
        IF EXISTS (SELECT 1 FROM Product WHERE product_name = @product_name)
        BEGIN
            RAISERROR ('Sản phẩm đã tồn tại!', 16, 1);
            ROLLBACK;
            RETURN;
        END;
        INSERT INTO Product (product_name, price, stock_quantity)
        VALUES (@product_name, @price, @stock_quantity);
        DECLARE @product_id INT = SCOPE_IDENTITY();
        COMMIT TRANSACTION;
        SELECT @product_id AS product_id;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR (@ErrorMessage, 16, 1);
    END CATCH;
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
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        IF NOT EXISTS (SELECT 1 FROM Users WHERE user_id = @current_user_id AND role = 'admin')
        BEGIN
            RAISERROR ('Chỉ admin mới có quyền cập nhật laptop!', 16, 1);
            ROLLBACK;
            RETURN;
        END;
        IF NOT EXISTS (SELECT 1 FROM Product WHERE product_id = @product_id)
        BEGIN
            RAISERROR ('Sản phẩm không tồn tại!', 16, 1);
            ROLLBACK;
            RETURN;
        END;
        IF EXISTS (SELECT 1 FROM Product WHERE product_name = @product_name AND product_id != @product_id)
        BEGIN
            RAISERROR ('Sản phẩm đã tồn tại!', 16, 1);
            ROLLBACK;
            RETURN;
        END;
        UPDATE Product
        SET product_name = @product_name,
            price = @price,
            stock_quantity = @stock_quantity
        WHERE product_id = @product_id;
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR (@ErrorMessage, 16, 1);
    END CATCH;
END;
GO

-- Tạo stored procedure sp_DeleteProduct
CREATE PROCEDURE sp_DeleteProduct
    @current_user_id INT,
    @product_id INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        IF NOT EXISTS (SELECT 1 FROM Users WHERE user_id = @current_user_id AND role = 'admin')
        BEGIN
            RAISERROR ('Chỉ admin mới có quyền xóa laptop!', 16, 1);
            ROLLBACK;
            RETURN;
        END;
        IF NOT EXISTS (SELECT 1 FROM Product WHERE product_id = @product_id)
        BEGIN
            RAISERROR ('Sản phẩm không tồn tại!', 16, 1);
            ROLLBACK;
            RETURN;
        END;
        DELETE FROM Product WHERE product_id = @product_id;
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR (@ErrorMessage, 16, 1);
    END CATCH;
END;
GO

-- Tạo stored procedure sp_InsertCategory
CREATE PROCEDURE sp_InsertCategory
    @current_user_id INT,
    @category_name NVARCHAR(100),
    @description NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        IF NOT EXISTS (SELECT 1 FROM Users WHERE user_id = @current_user_id)
        BEGIN
            RAISERROR ('Người dùng không tồn tại!', 16, 1);
            ROLLBACK;
            RETURN;
        END;
        IF EXISTS (SELECT 1 FROM Category WHERE category_name = @category_name)
        BEGIN
            RAISERROR ('Thương hiệu đã tồn tại!', 16, 1);
            ROLLBACK;
            RETURN;
        END;
        INSERT INTO Category (category_name, description)
        VALUES (@category_name, @description);
        DECLARE @category_id INT = SCOPE_IDENTITY();
        DECLARE @current_username NVARCHAR(100);
        SELECT @current_username = username FROM Users WHERE user_id = @current_user_id;
        INSERT INTO PermissionLog (user_id, username, action, old_role, new_role, action_date, performed_by, performed_by_username)
        VALUES (@current_user_id, @current_username, 'Insert Category: ' + @category_name, NULL, NULL, GETDATE(), @current_user_id, @current_username);
        COMMIT TRANSACTION;
        SELECT @category_id AS category_id;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR (@ErrorMessage, 16, 1);
    END CATCH;
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
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        IF NOT EXISTS (SELECT 1 FROM Users WHERE user_id = @current_user_id AND role = 'admin')
        BEGIN
            RAISERROR ('Chỉ admin mới có quyền cập nhật thương hiệu!', 16, 1);
            ROLLBACK;
            RETURN;
        END;
        IF NOT EXISTS (SELECT 1 FROM Category WHERE category_id = @category_id)
        BEGIN
            RAISERROR ('Thương hiệu không tồn tại!', 16, 1);
            ROLLBACK;
            RETURN;
        END;
        IF EXISTS (SELECT 1 FROM Category WHERE category_name = @category_name AND category_id != @category_id)
        BEGIN
            RAISERROR ('Thương hiệu đã tồn tại!', 16, 1);
            ROLLBACK;
            RETURN;
        END;
        UPDATE Category
        SET category_name = @category_name,
            description = @description
        WHERE category_id = @category_id;
        DECLARE @current_username NVARCHAR(100);
        SELECT @current_username = username FROM Users WHERE user_id = @current_user_id;
        INSERT INTO PermissionLog (user_id, username, action, old_role, new_role, action_date, performed_by, performed_by_username)
        VALUES (@current_user_id, @current_username, 'Update Category: ' + @category_name, NULL, NULL, GETDATE(), @current_user_id, @current_username);
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR (@ErrorMessage, 16, 1);
    END CATCH;
END;
GO

-- Tạo stored procedure sp_DeleteCategory
CREATE PROCEDURE sp_DeleteCategory
    @current_user_id INT,
    @category_id INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        IF NOT EXISTS (SELECT 1 FROM Users WHERE user_id = @current_user_id AND role = 'admin')
        BEGIN
            RAISERROR ('Chỉ admin mới có quyền xóa thương hiệu!', 16, 1);
            ROLLBACK;
            RETURN;
        END;
        IF NOT EXISTS (SELECT 1 FROM Category WHERE category_id = @category_id)
        BEGIN
            RAISERROR ('Thương hiệu không tồn tại!', 16, 1);
            ROLLBACK;
            RETURN;
        END;
        DELETE FROM Category WHERE category_id = @category_id;
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR (@ErrorMessage, 16, 1);
    END CATCH;
END;
GO

-- Tạo stored procedure sp_InsertSupplier
CREATE PROCEDURE sp_InsertSupplier
    @current_user_id INT,
    @supplier_name NVARCHAR(100),
    @contact_info NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        IF NOT EXISTS (SELECT 1 FROM Users WHERE user_id = @current_user_id AND role = 'admin')
        BEGIN
            RAISERROR ('Chỉ admin mới có quyền thêm nhà cung cấp!', 16, 1);
            ROLLBACK;
            RETURN;
        END;
        IF EXISTS (SELECT 1 FROM Supplier WHERE supplier_name = @supplier_name)
        BEGIN
            RAISERROR ('Nhà cung cấp đã tồn tại!', 16, 1);
            ROLLBACK;
            RETURN;
        END;
        INSERT INTO Supplier (supplier_name, contact_info)
        VALUES (@supplier_name, @contact_info);
        DECLARE @supplier_id INT = SCOPE_IDENTITY();
        COMMIT TRANSACTION;
        SELECT @supplier_id AS supplier_id;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR (@ErrorMessage, 16, 1);
    END CATCH;
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
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        IF NOT EXISTS (SELECT 1 FROM Users WHERE user_id = @current_user_id AND role = 'admin')
        BEGIN
            RAISERROR ('Chỉ admin mới có quyền cập nhật nhà cung cấp!', 16, 1);
            ROLLBACK;
            RETURN;
        END;
        IF NOT EXISTS (SELECT 1 FROM Supplier WHERE supplier_id = @supplier_id)
        BEGIN
            RAISERROR ('Nhà cung cấp không tồn tại!', 16, 1);
            ROLLBACK;
            RETURN;
        END;
        IF EXISTS (SELECT 1 FROM Supplier WHERE supplier_name = @supplier_name AND supplier_id != @supplier_id)
        BEGIN
            RAISERROR ('Nhà cung cấp đã tồn tại!', 16, 1);
            ROLLBACK;
            RETURN;
        END;
        UPDATE Supplier
        SET supplier_name = @supplier_name,
            contact_info = @contact_info
        WHERE supplier_id = @supplier_id;
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR (@ErrorMessage, 16, 1);
    END CATCH;
END;
GO

-- Tạo stored procedure sp_DeleteSupplier
CREATE PROCEDURE sp_DeleteSupplier
    @current_user_id INT,
    @supplier_id INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        IF NOT EXISTS (SELECT 1 FROM Users WHERE user_id = @current_user_id AND role = 'admin')
        BEGIN
            RAISERROR ('Chỉ admin mới có quyền xóa nhà cung cấp!', 16, 1);
            ROLLBACK;
            RETURN;
        END;
        IF NOT EXISTS (SELECT 1 FROM Supplier WHERE supplier_id = @supplier_id)
        BEGIN
            RAISERROR ('Nhà cung cấp không tồn tại!', 16, 1);
            ROLLBACK;
            RETURN;
        END;
        IF EXISTS (SELECT 1 FROM Import WHERE supplier_id = @supplier_id)
        BEGIN
            RAISERROR ('Không thể xóa nhà cung cấp vì đã có phiếu nhập liên quan!', 16, 1);
            ROLLBACK;
            RETURN;
        END;
        DELETE FROM Supplier WHERE supplier_id = @supplier_id;
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR (@ErrorMessage, 16, 1);
    END CATCH;
END;
GO

-- Tạo stored procedure sp_InsertCustomer
CREATE PROCEDURE sp_InsertCustomer
    @current_user_id INT,
    @customer_name NVARCHAR(100),
    @contact_info NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        IF NOT EXISTS (SELECT 1 FROM Users WHERE user_id = @current_user_id AND role = 'admin')
        BEGIN
            RAISERROR ('Chỉ admin mới có quyền thêm khách hàng!', 16, 1);
            ROLLBACK;
            RETURN;
        END;
        IF EXISTS (SELECT 1 FROM Customer WHERE customer_name = @customer_name)
        BEGIN
            RAISERROR ('Khách hàng đã tồn tại!', 16, 1);
            ROLLBACK;
            RETURN;
        END;
        INSERT INTO Customer (customer_name, contact_info)
        VALUES (@customer_name, @contact_info);
        DECLARE @customer_id INT = SCOPE_IDENTITY();
        COMMIT TRANSACTION;
        SELECT @customer_id AS customer_id;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR (@ErrorMessage, 16, 1);
    END CATCH;
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
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        IF NOT EXISTS (SELECT 1 FROM Users WHERE user_id = @current_user_id AND role = 'admin')
        BEGIN
            RAISERROR ('Chỉ admin mới có quyền cập nhật khách hàng!', 16, 1);
            ROLLBACK;
            RETURN;
        END;
        IF NOT EXISTS (SELECT 1 FROM Customer WHERE customer_id = @customer_id)
        BEGIN
            RAISERROR ('Khách hàng không tồn tại!', 16, 1);
            ROLLBACK;
            RETURN;
        END;
        IF EXISTS (SELECT 1 FROM Customer WHERE customer_name = @customer_name AND customer_id != @customer_id)
        BEGIN
            RAISERROR ('Khách hàng đã tồn tại!', 16, 1);
            ROLLBACK;
            RETURN;
        END;
        UPDATE Customer
        SET customer_name = @customer_name,
            contact_info = @contact_info
        WHERE customer_id = @customer_id;
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR (@ErrorMessage, 16, 1);
    END CATCH;
END;
GO

-- Tạo stored procedure sp_DeleteCustomer
CREATE PROCEDURE sp_DeleteCustomer
    @current_user_id INT,
    @customer_id INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        IF NOT EXISTS (SELECT 1 FROM Users WHERE user_id = @current_user_id AND role = 'admin')
        BEGIN
            RAISERROR ('Chỉ admin mới có quyền xóa khách hàng!', 16, 1);
            ROLLBACK;
            RETURN;
        END;
        IF NOT EXISTS (SELECT 1 FROM Customer WHERE customer_id = @customer_id)
        BEGIN
            RAISERROR ('Khách hàng không tồn tại!', 16, 1);
            ROLLBACK;
            RETURN;
        END;
        IF EXISTS (SELECT 1 FROM Export WHERE customer_id = @customer_id)
        BEGIN
            RAISERROR ('Không thể xóa khách hàng vì đã có phiếu xuất liên quan!', 16, 1);
            ROLLBACK;
            RETURN;
        END;
        DELETE FROM Customer WHERE customer_id = @customer_id;
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR (@ErrorMessage, 16, 1);
    END CATCH;
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
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        -- Bỏ tham số total_amount và khởi tạo bằng 0
        INSERT INTO Import (supplier_id, import_date, total_amount)
        VALUES (@supplier_id, @import_date, 0); -- Khởi tạo total_amount = 0
        DECLARE @import_id INT = SCOPE_IDENTITY();
        COMMIT TRANSACTION;
        SELECT @import_id AS import_id;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR (@ErrorMessage, 16, 1);
    END CATCH;
END;
GO
GO

-- Tạo stored procedure sp_InsertImportDetail
CREATE PROCEDURE sp_InsertImportDetail
    @current_user_id INT,
    @import_id INT,
    @product_name NVARCHAR(100),
    @quantity INT,
    @unit_price DECIMAL(18,2),
    @price DECIMAL(18,2) = NULL,
    @category_name NVARCHAR(100) = NULL,
    @category_description NVARCHAR(255) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        DECLARE @product_id INT;
        DECLARE @category_id INT;
        DECLARE @current_username NVARCHAR(100);

        IF NOT EXISTS (SELECT 1 FROM Import WHERE import_id = @import_id)
        BEGIN
            RAISERROR ('Phiếu nhập không tồn tại!', 16, 1);
            ROLLBACK;
            RETURN;
        END;

        IF @quantity <= 0
        BEGIN
            RAISERROR ('Số lượng phải lớn hơn 0!', 16, 1);
            ROLLBACK;
            RETURN;
        END;

        SELECT @product_id = product_id
        FROM Product
        WHERE product_name = @product_name;

        IF @product_id IS NULL
        BEGIN
            INSERT INTO Product (product_name, price, stock_quantity)
            VALUES (@product_name, ISNULL(@price, @unit_price * 1.2), 0);
            SET @product_id = SCOPE_IDENTITY();

            IF @category_name IS NOT NULL
            BEGIN
                SELECT @category_id = category_id
                FROM Category
                WHERE category_name = @category_name;

                IF @category_id IS NULL
                BEGIN
                    INSERT INTO Category (category_name, description)
                    VALUES (@category_name, ISNULL(@category_description, 'Thêm tự động khi nhập hàng'));
                    SET @category_id = SCOPE_IDENTITY();

                    SELECT @current_username = username FROM Users WHERE user_id = @current_user_id;
                    INSERT INTO PermissionLog (user_id, username, action, old_role, new_role, action_date, performed_by, performed_by_username)
                    VALUES (@current_user_id, @current_username, 'Insert Category: ' + @category_name, NULL, NULL, GETDATE(), @current_user_id, @current_username);
                END;

                INSERT INTO ProductCategory (product_id, category_id)
VALUES (@product_id, @category_id);
            END;
        END;

        INSERT INTO ImportDetail (import_id, product_id, quantity, unit_price)
        VALUES (@import_id, @product_id, @quantity, @unit_price);

        UPDATE Product
        SET stock_quantity = stock_quantity + @quantity
        WHERE product_id = @product_id;

        -- Cập nhật total_amount trong bảng Import
        UPDATE Import
        SET total_amount = dbo.fn_GetImportTotal(@import_id)
        WHERE import_id = @import_id;

        COMMIT TRANSACTION;
        SELECT @product_id AS product_id;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR (@ErrorMessage, 16, 1);
    END CATCH;
END;
GO

-- Tạo stored procedure sp_UpdateImport
CREATE PROCEDURE sp_UpdateImport
    @current_user_id INT,
    @import_id INT,
    @product_name NVARCHAR(100),
    @new_quantity INT,
    @new_unit_price DECIMAL(18,2),
    @new_price DECIMAL(18,2),
    @is_updated BIT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        SET @is_updated = 0; -- Khởi tạo biến đầu ra là 0 (không có thay đổi)

        -- Kiểm tra quyền admin
        IF NOT EXISTS (SELECT 1 FROM Users WHERE user_id = @current_user_id AND role = 'admin')
        BEGIN
            RAISERROR ('Chỉ admin mới có quyền cập nhật chi tiết phiếu nhập và giá sản phẩm!', 16, 1);
            ROLLBACK;
            RETURN;
        END;

        -- Kiểm tra phiếu nhập tồn tại
        IF NOT EXISTS (SELECT 1 FROM Import WHERE import_id = @import_id)
        BEGIN
            RAISERROR ('Phiếu nhập không tồn tại!', 16, 1);
            ROLLBACK;
            RETURN;
        END;

        -- Lấy product_id từ product_name
        DECLARE @product_id INT;
        SELECT @product_id = product_id
        FROM Product
        WHERE product_name = @product_name;

        IF @product_id IS NULL
        BEGIN
            RAISERROR ('Sản phẩm không tồn tại!', 16, 1);
            ROLLBACK;
            RETURN;
        END;

        -- Kiểm tra chi tiết phiếu nhập tồn tại
        IF NOT EXISTS (SELECT 1 FROM ImportDetail WHERE import_id = @import_id AND product_id = @product_id)
        BEGIN
            RAISERROR ('Chi tiết phiếu nhập không tồn tại!', 16, 1);
            ROLLBACK;
            RETURN;
        END;

        -- Kiểm tra số lượng hợp lệ
        IF @new_quantity <= 0
        BEGIN
            RAISERROR ('Số lượng phải lớn hơn 0!', 16, 1);
            ROLLBACK;
            RETURN;
        END;

        -- Lấy giá trị hiện tại của quantity, unit_price và price
        DECLARE @old_quantity INT;
        DECLARE @old_unit_price DECIMAL(18,2);
        DECLARE @old_price DECIMAL(18,2);
        SELECT @old_quantity = quantity, @old_unit_price = unit_price
        FROM ImportDetail
        WHERE import_id = @import_id AND product_id = @product_id;

        SELECT @old_price = price
        FROM Product
        WHERE product_id = @product_id;

        -- Kiểm tra xem có thay đổi không
        IF @old_quantity = @new_quantity 
           AND @old_unit_price = @new_unit_price 
           AND @old_price = @new_price
        BEGIN
            -- Không có thay đổi, đặt @is_updated = 0 và thoát
            COMMIT TRANSACTION;
            RETURN;
        END;

        -- Có thay đổi, đặt @is_updated = 1
        SET @is_updated = 1;

        -- Cập nhật chi tiết nhập hàng
        UPDATE ImportDetail
        SET quantity = @new_quantity,
            unit_price = @new_unit_price
        WHERE import_id = @import_id AND product_id = @product_id;

        -- Cập nhật stock_quantity trong bảng Product
        UPDATE Product
        SET stock_quantity = stock_quantity - @old_quantity + @new_quantity,
            price = @new_price
        WHERE product_id = @product_id;

        -- Cập nhật total_amount trong bảng Import
        UPDATE Import
        SET total_amount = dbo.fn_GetImportTotal(@import_id)
        WHERE import_id = @import_id;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR (@ErrorMessage, 16, 1);
    END CATCH;
END;
GO
GO

-- Tạo stored procedure sp_DeleteImport
CREATE PROCEDURE sp_DeleteImport
    @current_user_id INT,
    @import_id INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        IF NOT EXISTS (SELECT 1 FROM Users WHERE user_id = @current_user_id AND role = 'admin')
        BEGIN
            RAISERROR ('Chỉ admin mới có quyền xóa phiếu nhập!', 16, 1);
            ROLLBACK;
            RETURN;
        END;
        IF NOT EXISTS (SELECT 1 FROM Import WHERE import_id = @import_id)
        BEGIN
            RAISERROR ('Phiếu nhập không tồn tại!', 16, 1);
            ROLLBACK;
            RETURN;
        END;
        DELETE FROM Import WHERE import_id = @import_id;
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR (@ErrorMessage, 16, 1);
    END CATCH;
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
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        INSERT INTO Export (customer_id, export_date, total_amount)
        VALUES (@customer_id, @export_date, @total_amount);
        DECLARE @export_id INT = SCOPE_IDENTITY();
        COMMIT TRANSACTION;
        SELECT @export_id AS export_id;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR (@ErrorMessage, 16, 1);
    END CATCH;
END;
GO

-- Tạo stored procedure sp_InsertExportDetail
CREATE PROCEDURE sp_InsertExportDetail
    @current_user_id INT,
    @export_id INT,
    @product_name NVARCHAR(100),
    @quantity INT,
    @unit_price DECIMAL(18,2)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        DECLARE @product_id INT;

        -- Check if the export exists
        IF NOT EXISTS (SELECT 1 FROM Export WHERE export_id = @export_id)
        BEGIN
            RAISERROR ('Phiếu xuất không tồn tại!', 16, 1);
            ROLLBACK;
            RETURN;
        END;

        -- Check if quantity is valid
        IF @quantity <= 0
        BEGIN
            RAISERROR ('Số lượng phải lớn hơn 0!', 16, 1);
            ROLLBACK;
            RETURN;
        END;

        -- Look up the product_id based on product_name
        SELECT @product_id = product_id
        FROM Product
        WHERE product_name = @product_name;

        -- If the product doesn't exist, raise an error
        IF @product_id IS NULL
        BEGIN
            RAISERROR ('Sản phẩm không tồn tại!', 16, 1);
            ROLLBACK;
            RETURN;
        END;

        -- Check stock quantity before exporting
        DECLARE @stock INT;
        SET @stock = dbo.fn_GetStockQuantity(@product_id);
        IF @stock < @quantity
        BEGIN
            RAISERROR ('Số lượng xuất vượt quá tồn kho!', 16, 1);
            ROLLBACK;
            RETURN;
        END;

        -- Insert the export detail
        INSERT INTO ExportDetail (export_id, product_id, quantity, unit_price)
        VALUES (@export_id, @product_id, @quantity, @unit_price);

        -- Update the stock quantity
        UPDATE Product
        SET stock_quantity = stock_quantity - @quantity
        WHERE product_id = @product_id;

        -- Cập nhật total_amount trong bảng Export
        UPDATE Export
        SET total_amount = dbo.fn_GetExportTotal(@export_id)
        WHERE export_id = @export_id;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR (@ErrorMessage, 16, 1);
    END CATCH;
END;
GO


-- Tạo stored procedure sp_UpdateExport
CREATE PROCEDURE sp_UpdateExport
    @current_user_id INT,
    @export_id INT,
    @product_name NVARCHAR(100),
    @new_quantity INT,
    @new_unit_price DECIMAL(18,2),
    @is_updated BIT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        SET @is_updated = 0; -- Khởi tạo biến đầu ra là 0 (không có thay đổi)

        -- Kiểm tra quyền admin
        IF NOT EXISTS (SELECT 1 FROM Users WHERE user_id = @current_user_id AND role = 'admin')
        BEGIN
            RAISERROR ('Chỉ admin mới có quyền cập nhật chi tiết phiếu xuất!', 16, 1);
            ROLLBACK;
            RETURN;
        END;

        -- Kiểm tra phiếu xuất tồn tại
        IF NOT EXISTS (SELECT 1 FROM Export WHERE export_id = @export_id)
        BEGIN
            RAISERROR ('Phiếu xuất không tồn tại!', 16, 1);
            ROLLBACK;
            RETURN;
        END;

        -- Lấy product_id từ product_name
        DECLARE @product_id INT;
        SELECT @product_id = product_id
        FROM Product
        WHERE product_name = @product_name;

        IF @product_id IS NULL
        BEGIN
            RAISERROR ('Sản phẩm không tồn tại!', 16, 1);
            ROLLBACK;
            RETURN;
        END;

        -- Kiểm tra chi tiết phiếu xuất tồn tại
        IF NOT EXISTS (SELECT 1 FROM ExportDetail WHERE export_id = @export_id AND product_id = @product_id)
        BEGIN
            RAISERROR ('Chi tiết phiếu xuất không tồn tại!', 16, 1);
            ROLLBACK;
            RETURN;
        END;

        -- Kiểm tra số lượng hợp lệ
        IF @new_quantity <= 0
        BEGIN
            RAISERROR ('Số lượng phải lớn hơn 0!', 16, 1);
            ROLLBACK;
            RETURN;
        END;

        -- Kiểm tra stock_quantity đủ để xuất
        DECLARE @stock_quantity INT;
        SELECT @stock_quantity = stock_quantity
        FROM Product
        WHERE product_id = @product_id;

        DECLARE @old_quantity INT;
        SELECT @old_quantity = quantity
        FROM ExportDetail
        WHERE export_id = @export_id AND product_id = @product_id;

        DECLARE @quantity_difference INT = @new_quantity - @old_quantity;
        IF @stock_quantity - @quantity_difference < 0
        BEGIN
            RAISERROR ('Số lượng xuất vượt quá số lượng tồn kho!', 16, 1);
            ROLLBACK;
            RETURN;
        END;

        -- Lấy giá trị hiện tại của quantity và unit_price
        DECLARE @old_unit_price DECIMAL(18,2);
        SELECT @old_unit_price = unit_price
        FROM ExportDetail
        WHERE export_id = @export_id AND product_id = @product_id;

        -- Kiểm tra xem có thay đổi không
        IF @old_quantity = @new_quantity 
           AND @old_unit_price = @new_unit_price
        BEGIN
            -- Không có thay đổi, đặt @is_updated = 0 và thoát
            COMMIT TRANSACTION;
            RETURN;
        END;

        -- Có thay đổi, đặt @is_updated = 1
        SET @is_updated = 1;

        -- Cập nhật chi tiết xuất hàng
        UPDATE ExportDetail
        SET quantity = @new_quantity,
            unit_price = @new_unit_price
        WHERE export_id = @export_id AND product_id = @product_id;

        -- Cập nhật stock_quantity trong bảng Product
        UPDATE Product
        SET stock_quantity = stock_quantity - @quantity_difference
        WHERE product_id = @product_id;

        -- Cập nhật total_amount trong bảng Export
        UPDATE Export
        SET total_amount = dbo.fn_GetExportTotal(@export_id)
        WHERE export_id = @export_id;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR (@ErrorMessage, 16, 1);
    END CATCH;
END;
GO

-- Tạo stored procedure sp_DeleteExport
CREATE PROCEDURE sp_DeleteExport
    @current_user_id INT,
    @export_id INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        IF NOT EXISTS (SELECT 1 FROM Users WHERE user_id = @current_user_id AND role = 'admin')
        BEGIN
            RAISERROR ('Chỉ admin mới có quyền xóa phiếu xuất!', 16, 1);
            ROLLBACK;
            RETURN;
        END;
        IF NOT EXISTS (SELECT 1 FROM Export WHERE export_id = @export_id)
        BEGIN
            RAISERROR ('Phiếu xuất không tồn tại!', 16, 1);
            ROLLBACK;
            RETURN;
        END;
        DELETE FROM Export WHERE export_id = @export_id;
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR (@ErrorMessage, 16, 1);
    END CATCH;
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

-- Tạo stored procedure sp_GetPermissionLogs
CREATE PROCEDURE sp_GetPermissionLogs
    @current_user_id INT
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM Users WHERE user_id = @current_user_id AND role = 'admin')
    BEGIN
        RAISERROR ('Only admin can view permission logs!', 16, 1);
        RETURN;
    END;
    SELECT 
        log_id,
        username,
        action,
        old_role,
        new_role,
        action_date,
        performed_by_username
    FROM PermissionLog
    ORDER BY action_date DESC;
END;
GO

-- Tạo view vw_ProductDetails
CREATE VIEW vw_ProductDetails
AS
SELECT 
    p.product_id,
    p.product_name,
    p.price,
    p.stock_quantity,
    STRING_AGG(c.category_name, ', ') AS category_name
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
    p.stock_quantity,
    STRING_AGG(c.category_name, ', ') AS brands
FROM Product p
LEFT JOIN ProductCategory pc ON p.product_id = pc.product_id
LEFT JOIN Category c ON pc.category_id = c.category_id
GROUP BY p.product_id, p.product_name, p.price, p.stock_quantity;
GO

-- Tạo view vw_Suppliers
CREATE VIEW vw_Suppliers AS
SELECT supplier_id, supplier_name, contact_info
FROM Supplier;
GO

-- Tạo view vw_Customers
CREATE VIEW vw_Customers AS
SELECT customer_id, customer_name, contact_info
FROM Customer;
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
INSERT INTO Users (username, password, role)
VALUES ('admin1', 'admin_pass', 'admin'),
       ('employee1', 'emp_pass', 'employee');
GO

INSERT INTO Category (category_name, description)
VALUES ('Dell', 'Dell laptops'),
       ('Apple', 'Apple MacBooks'),
       ('HP', 'HP laptops');
GO

INSERT INTO Product (product_name, price, stock_quantity)
VALUES ('Dell XPS 13', 1200.00, 30),
       ('MacBook Pro 14', 2000.00, 20),
       ('HP Spectre x360', 1500.00, 25);
GO

INSERT INTO ProductCategory (product_id, category_id)
VALUES (1, 1),
       (2, 2),
       (3, 3);
GO

INSERT INTO Supplier (supplier_name, contact_info)
VALUES ('Tech Distributor', 'contact@techdist.com'),
       ('Apple Inc.', 'contact@apple.com');
GO

INSERT INTO Customer (customer_name, contact_info)
VALUES ('John Doe', 'john.doe@email.com'),
       ('Tech Corp', 'sales@techcorp.com');
GO

INSERT INTO Import (supplier_id, import_date, total_amount)
VALUES (1, '2025-04-24', 0),
       (2, '2025-04-24', 0);
GO

INSERT INTO ImportDetail (import_id, product_id, quantity, unit_price)
VALUES (1, 1, 10, 1100.00),
       (1, 3, 10, 1400.00),
       (2, 2, 10, 1900.00);
GO

INSERT INTO Export (customer_id, export_date, total_amount)
VALUES (1, '2025-04-24', 0),
       (2, '2025-04-24', 0);
GO

INSERT INTO ExportDetail (export_id, product_id, quantity, unit_price)
VALUES (1, 1, 2, 1200.00),
       (2, 3, 3, 1500.00);
GO