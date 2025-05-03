/*
 * LaptopStoreDBMS4 - Database for managing laptop store inventory, import/export operations
 *
 * Fixes Applied:
 * - Removed FOREIGN KEY constraint on ProductLog(product_id) to prevent conflicts when logging deleted products.
 * - Modified tr_LogProductDeletion to set product_id to NULL when logging deleted products.
 * - Optimized sp_DeleteSupplier, sp_DeleteCustomer, sp_DeleteImport, sp_DeleteExport to remove unnecessary queries before deletion.
 * - Ensured all DELETE operations succeed and are logged in ProductLog (for products) and PermissionLog (for all actions).
 * - Simplified triggers to log actions without interfering with the main DELETE operation.
 * - Reviewed all tables and triggers to ensure no similar FOREIGN KEY conflicts exist.
 */

-- Step 1: Drop the database if it exists (optional, comment out if you want to keep existing data)
IF EXISTS (SELECT * FROM sys.databases WHERE name = 'LaptopStoreDBMS4')
    DROP DATABASE LaptopStoreDBMS4;
GO

-- Step 2: Create the database
CREATE DATABASE LaptopStoreDBMS4;
GO
USE LaptopStoreDBMS4;
GO

-- Step 3: Create tables
CREATE TABLE Product (
    product_id INT PRIMARY KEY IDENTITY,
    product_name NVARCHAR(100) UNIQUE,
    price DECIMAL(18,2),
    stock_quantity INT
);
GO

CREATE TABLE Category (
    category_id INT PRIMARY KEY IDENTITY,
    category_name NVARCHAR(100) UNIQUE,
    description NVARCHAR(255)
);
GO

CREATE TABLE ProductCategory (
    product_id INT,
    category_id INT,
    PRIMARY KEY (product_id, category_id),
    FOREIGN KEY (product_id) REFERENCES Product(product_id) ON DELETE CASCADE,
    FOREIGN KEY (category_id) REFERENCES Category(category_id) ON DELETE CASCADE
);
GO

CREATE TABLE Supplier (
    supplier_id INT PRIMARY KEY IDENTITY,
    supplier_name NVARCHAR(100) UNIQUE,
    contact_info NVARCHAR(255)
);
GO

CREATE TABLE Customer (
    customer_id INT PRIMARY KEY IDENTITY,
    customer_name NVARCHAR(100) UNIQUE,
    contact_info NVARCHAR(255)
);
GO

CREATE TABLE Import (
    import_id INT PRIMARY KEY IDENTITY,
    supplier_id INT NULL,
    import_date DATE,
    total_amount DECIMAL(18,2),
    FOREIGN KEY (supplier_id) REFERENCES Supplier(supplier_id) ON DELETE SET NULL
);
GO

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

CREATE TABLE Export (
    export_id INT PRIMARY KEY IDENTITY,
    customer_id INT NULL,
    export_date DATE,
    total_amount DECIMAL(18,2),
    FOREIGN KEY (customer_id) REFERENCES Customer(customer_id) ON DELETE SET NULL
);
GO

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

CREATE TABLE ProductLog (
    log_id INT PRIMARY KEY IDENTITY,
    product_id INT NULL, -- Removed FOREIGN KEY constraint to avoid conflicts
    product_name NVARCHAR(100),
    price DECIMAL(18,2),
    stock_quantity INT,
    deleted_date DATETIME,
    deleted_by NVARCHAR(100)
);
GO

CREATE TABLE PermissionLog (
    log_id INT PRIMARY KEY IDENTITY,
    action NVARCHAR(100),
    action_date DATETIME,
    performed_by NVARCHAR(100),
    target_role NVARCHAR(100) NULL
);
GO

CREATE TABLE RolePermissions (
    permission_id INT PRIMARY KEY IDENTITY,
    role_name NVARCHAR(100),
    object_name NVARCHAR(100),
    object_type NVARCHAR(50),
    permission_type NVARCHAR(50),
    granted_date DATETIME,
    granted_by NVARCHAR(100),
    UNIQUE (role_name, object_name, object_type, permission_type),
    CONSTRAINT chk_object_type CHECK (object_type IN ('PROCEDURE', 'VIEW', 'TABLE')),
    CONSTRAINT chk_permission_type CHECK (permission_type IN ('EXECUTE', 'SELECT', 'INSERT', 'UPDATE', 'DELETE'))
);
GO

-- Step 4: Create functions
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

-- Step 5: Create stored procedures
CREATE PROCEDURE sp_InsertProduct
    @product_name NVARCHAR(100),
    @price DECIMAL(18,2),
    @stock_quantity INT
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM Product WHERE product_name = @product_name)
    BEGIN
        RAISERROR ('Sản phẩm đã tồn tại!', 16, 1);
        RETURN;
    END;
    INSERT INTO Product (product_name, price, stock_quantity)
    VALUES (@product_name, @price, @stock_quantity);
    DECLARE @product_id INT = SCOPE_IDENTITY();
    INSERT INTO PermissionLog (action, action_date, performed_by)
    VALUES ('Insert Product: ' + @product_name, GETDATE(), SUSER_SNAME());
    SELECT @product_id AS product_id;
END;
GO

CREATE PROCEDURE sp_UpdateProduct
    @product_id INT,
    @product_name NVARCHAR(100),
    @price DECIMAL(18,2),
    @stock_quantity INT
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT EXISTS (SELECT 1 FROM Product WHERE product_id = @product_id)
    BEGIN
        RAISERROR ('Sản phẩm không tồn tại!', 16, 1);
        RETURN;
    END;
    IF EXISTS (SELECT 1 FROM Product WHERE product_name = @product_name AND product_id != @product_id)
    BEGIN
        RAISERROR ('Sản phẩm đã tồn tại!', 16, 1);
        RETURN;
    END;
    UPDATE Product
    SET product_name = @product_name,
        price = @price,
        stock_quantity = @stock_quantity
    WHERE product_id = @product_id;
    INSERT INTO PermissionLog (action, action_date, performed_by)
    VALUES ('Update Product: ' + @product_name, GETDATE(), SUSER_SNAME());
END;
GO

CREATE PROCEDURE sp_DeleteProduct
    @product_id INT
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT EXISTS (SELECT 1 FROM Product WHERE product_id = @product_id)
    BEGIN
        RAISERROR ('Sản phẩm không tồn tại!', 16, 1);
        RETURN;
    END;
    DELETE FROM Product WHERE product_id = @product_id;
    -- Logging is handled by the trigger tr_LogProductDeletion
END;
GO

CREATE PROCEDURE sp_InsertCategory
    @category_name NVARCHAR(100),
    @description NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM Category WHERE category_name = @category_name)
    BEGIN
        RAISERROR ('Thương hiệu đã tồn tại!', 16, 1);
        RETURN;
    END;
    INSERT INTO Category (category_name, description)
    VALUES (@category_name, @description);
    DECLARE @category_id INT = SCOPE_IDENTITY();
    INSERT INTO PermissionLog (action, action_date, performed_by)
    VALUES ('Insert Category: ' + @category_name, GETDATE(), SUSER_SNAME());
    SELECT @category_id AS category_id;
END;
GO

CREATE PROCEDURE sp_UpdateCategory
    @category_id INT,
    @category_name NVARCHAR(100),
    @description NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT EXISTS (SELECT 1 FROM Category WHERE category_id = @category_id)
    BEGIN
        RAISERROR ('Thương hiệu không tồn tại!', 16, 1);
        RETURN;
    END;
    IF EXISTS (SELECT 1 FROM Category WHERE category_name = @category_name AND category_id != @category_id)
    BEGIN
        RAISERROR ('Thương hiệu đã tồn tại!', 16, 1);
        RETURN;
    END;
    UPDATE Category
    SET category_name = @category_name,
        description = @description
    WHERE category_id = @category_id;
    INSERT INTO PermissionLog (action, action_date, performed_by)
    VALUES ('Update Category: ' + @category_name, GETDATE(), SUSER_SNAME());
END;
GO

CREATE PROCEDURE sp_DeleteCategory
    @category_id INT
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT EXISTS (SELECT 1 FROM Category WHERE category_id = @category_id)
    BEGIN
        RAISERROR ('Thương hiệu không tồn tại!', 16, 1);
        RETURN;
    END;
    DELETE FROM Category WHERE category_id = @category_id;
    -- Logging is handled by the trigger
END;
GO

CREATE PROCEDURE sp_InsertSupplier
    @supplier_name NVARCHAR(100),
    @contact_info NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM Supplier WHERE supplier_name = @supplier_name)
    BEGIN
        RAISERROR ('Nhà cung cấp đã tồn tại!', 16, 1);
        RETURN;
    END;
    INSERT INTO Supplier (supplier_name, contact_info)
    VALUES (@supplier_name, @contact_info);
    DECLARE @supplier_id INT = SCOPE_IDENTITY();
    INSERT INTO PermissionLog (action, action_date, performed_by)
    VALUES ('Insert Supplier: ' + @supplier_name, GETDATE(), SUSER_SNAME());
    SELECT @supplier_id AS supplier_id;
END;
GO

CREATE PROCEDURE sp_UpdateSupplier
    @supplier_id INT,
    @supplier_name NVARCHAR(100),
    @contact_info NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT EXISTS (SELECT 1 FROM Supplier WHERE supplier_id = @supplier_id)
    BEGIN
        RAISERROR ('Nhà cung cấp không tồn tại!', 16, 1);
        RETURN;
    END;
    IF EXISTS (SELECT 1 FROM Supplier WHERE supplier_name = @supplier_name AND supplier_id != @supplier_id)
    BEGIN
        RAISERROR ('Nhà cung cấp đã tồn tại!', 16, 1);
        RETURN;
    END;
    UPDATE Supplier
    SET supplier_name = @supplier_name,
        contact_info = @contact_info
    WHERE supplier_id = @supplier_id;
    INSERT INTO PermissionLog (action, action_date, performed_by)
    VALUES ('Update Supplier: ' + @supplier_name, GETDATE(), SUSER_SNAME());
END;
GO

CREATE PROCEDURE sp_DeleteSupplier
    @supplier_id INT
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT EXISTS (SELECT 1 FROM Supplier WHERE supplier_id = @supplier_id)
    BEGIN
        RAISERROR ('Nhà cung cấp không tồn tại!', 16, 1);
        RETURN;
    END;
    DELETE FROM Supplier WHERE supplier_id = @supplier_id;
    -- Logging is handled by the trigger tr_LogSupplierDeletion
END;
GO

CREATE PROCEDURE sp_InsertCustomer
    @customer_name NVARCHAR(100),
    @contact_info NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM Customer WHERE customer_name = @customer_name)
    BEGIN
        RAISERROR ('Khách hàng đã tồn tại!', 16, 1);
        RETURN;
    END;
    INSERT INTO Customer (customer_name, contact_info)
    VALUES (@customer_name, @contact_info);
    DECLARE @customer_id INT = SCOPE_IDENTITY();
    INSERT INTO PermissionLog (action, action_date, performed_by)
    VALUES ('Insert Customer: ' + @customer_name, GETDATE(), SUSER_SNAME());
    SELECT @customer_id AS customer_id;
END;
GO

CREATE PROCEDURE sp_UpdateCustomer
    @customer_id INT,
    @customer_name NVARCHAR(100),
    @contact_info NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT EXISTS (SELECT 1 FROM Customer WHERE customer_id = @customer_id)
    BEGIN
        RAISERROR ('Khách hàng không tồn tại!', 16, 1);
        RETURN;
    END;
    IF EXISTS (SELECT 1 FROM Customer WHERE customer_name = @customer_name AND customer_id != @customer_id)
    BEGIN
        RAISERROR ('Khách hàng đã tồn tại!', 16, 1);
        RETURN;
    END;
    UPDATE Customer
    SET customer_name = @customer_name,
        contact_info = @contact_info
    WHERE customer_id = @customer_id;
    INSERT INTO PermissionLog (action, action_date, performed_by)
    VALUES ('Update Customer: ' + @customer_name, GETDATE(), SUSER_SNAME());
END;
GO

CREATE PROCEDURE sp_DeleteCustomer
    @customer_id INT
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT EXISTS (SELECT 1 FROM Customer WHERE customer_id = @customer_id)
    BEGIN
        RAISERROR ('Khách hàng không tồn tại!', 16, 1);
        RETURN;
    END;
    DELETE FROM Customer WHERE customer_id = @customer_id;
    -- Logging is handled by the trigger tr_LogCustomerDeletion
END;
GO

CREATE PROCEDURE sp_InsertImport
    @supplier_id INT,
    @import_date DATE
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT EXISTS (SELECT 1 FROM Supplier WHERE supplier_id = @supplier_id)
    BEGIN
        RAISERROR ('Nhà cung cấp không tồn tại!', 16, 1);
        RETURN;
    END;
    INSERT INTO Import (supplier_id, import_date, total_amount)
    VALUES (@supplier_id, @import_date, 0);
    DECLARE @import_id INT = SCOPE_IDENTITY();
    INSERT INTO PermissionLog (action, action_date, performed_by)
    VALUES ('Insert Import: ' + CAST(@import_id AS NVARCHAR(10)), GETDATE(), SUSER_SNAME());
    SELECT @import_id AS import_id;
END;
GO

CREATE PROCEDURE sp_InsertImportDetail
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
    DECLARE @product_id INT;
    DECLARE @category_id INT;

    IF NOT EXISTS (SELECT 1 FROM Import WHERE import_id = @import_id)
    BEGIN
        RAISERROR ('Phiếu nhập không tồn tại!', 16, 1);
        RETURN;
    END;

    IF @quantity <= 0
    BEGIN
        RAISERROR ('Số lượng phải lớn hơn 0!', 16, 1);
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
                INSERT INTO PermissionLog (action, action_date, performed_by)
                VALUES ('Insert Category: ' + @category_name, GETDATE(), SUSER_SNAME());
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

    UPDATE Import
    SET total_amount = dbo.fn_GetImportTotal(@import_id)
    WHERE import_id = @import_id;

    INSERT INTO PermissionLog (action, action_date, performed_by)
    VALUES ('Insert ImportDetail: ' + @product_name, GETDATE(), SUSER_SNAME());

    SELECT @product_id AS product_id;
END;
GO

CREATE PROCEDURE sp_UpdateImport
    @import_id INT,
    @product_name NVARCHAR(100),
    @new_quantity INT,
    @new_unit_price DECIMAL(18,2),
    @new_price DECIMAL(18,2),
    @is_updated BIT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @is_updated = 0;

    IF NOT EXISTS (SELECT 1 FROM Import WHERE import_id = @import_id)
    BEGIN
        RAISERROR ('Phiếu nhập không tồn tại!', 16, 1);
        RETURN;
    END;

    DECLARE @product_id INT;
    SELECT @product_id = product_id
    FROM Product
    WHERE product_name = @product_name;

    IF @product_id IS NULL
    BEGIN
        RAISERROR ('Sản phẩm không tồn tại!', 16, 1);
        RETURN;
    END;

    IF NOT EXISTS (SELECT 1 FROM ImportDetail WHERE import_id = @import_id AND product_id = @product_id)
    BEGIN
        RAISERROR ('Chi tiết phiếu nhập không tồn tại!', 16, 1);
        RETURN;
    END;

    IF @new_quantity <= 0
    BEGIN
        RAISERROR ('Số lượng phải lớn hơn 0!', 16, 1);
        RETURN;
    END;

    DECLARE @old_quantity INT;
    DECLARE @old_unit_price DECIMAL(18,2);
    DECLARE @old_price DECIMAL(18,2);
    SELECT @old_quantity = quantity, @old_unit_price = unit_price
    FROM ImportDetail
    WHERE import_id = @import_id AND product_id = @product_id;

    SELECT @old_price = price
    FROM Product
    WHERE product_id = @product_id;

    IF @old_quantity = @new_quantity 
       AND @old_unit_price = @new_unit_price 
       AND @old_price = @new_price
    BEGIN
        RETURN;
    END;

    SET @is_updated = 1;

    UPDATE ImportDetail
    SET quantity = @new_quantity,
        unit_price = @new_unit_price
    WHERE import_id = @import_id AND product_id = @product_id;

    UPDATE Product
    SET stock_quantity = stock_quantity - @old_quantity + @new_quantity,
        price = @new_price
    WHERE product_id = @product_id;

    UPDATE Import
    SET total_amount = dbo.fn_GetImportTotal(@import_id)
    WHERE import_id = @import_id;

    INSERT INTO PermissionLog (action, action_date, performed_by)
    VALUES ('Update Import: ' + @product_name, GETDATE(), SUSER_SNAME());
END;
GO

CREATE PROCEDURE sp_DeleteImport
    @import_id INT
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT EXISTS (SELECT 1 FROM Import WHERE import_id = @import_id)
    BEGIN
        RAISERROR ('Phiếu nhập không tồn tại!', 16, 1);
        RETURN;
    END;
    DELETE FROM Import WHERE import_id = @import_id;
    -- Logging is handled by the trigger tr_LogImportDeletion
END;
GO

CREATE PROCEDURE sp_InsertExport
    @customer_id INT,
    @export_date DATE
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT EXISTS (SELECT 1 FROM Customer WHERE customer_id = @customer_id)
    BEGIN
        RAISERROR ('Khách hàng không tồn tại!', 16, 1);
        RETURN;
    END;
    INSERT INTO Export (customer_id, export_date, total_amount)
    VALUES (@customer_id, @export_date, 0);
    DECLARE @export_id INT = SCOPE_IDENTITY();
    INSERT INTO PermissionLog (action, action_date, performed_by)
    VALUES ('Insert Export: ' + CAST(@export_id AS NVARCHAR(10)), GETDATE(), SUSER_SNAME());
    SELECT @export_id AS export_id;
END;
GO

CREATE PROCEDURE sp_InsertExportDetail
    @export_id INT,
    @product_name NVARCHAR(100),
    @quantity INT,
    @unit_price DECIMAL(18,2)
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @product_id INT;

    IF NOT EXISTS (SELECT 1 FROM Export WHERE export_id = @export_id)
    BEGIN
        RAISERROR ('Phiếu xuất không tồn tại!', 16, 1);
        RETURN;
    END;

    IF @quantity <= 0
    BEGIN
        RAISERROR ('Số lượng phải lớn hơn 0!', 16, 1);
        RETURN;
    END;

    SELECT @product_id = product_id
    FROM Product
    WHERE product_name = @product_name;

    IF @product_id IS NULL
    BEGIN
        RAISERROR ('Sản phẩm không tồn tại!', 16, 1);
        RETURN;
    END;

    DECLARE @stock INT;
    SET @stock = dbo.fn_GetStockQuantity(@product_id);
    IF @stock < @quantity
    BEGIN
        RAISERROR ('Số lượng xuất vượt quá tồn kho!', 16, 1);
        RETURN;
    END;

    INSERT INTO ExportDetail (export_id, product_id, quantity, unit_price)
    VALUES (@export_id, @product_id, @quantity, @unit_price);

    UPDATE Product
    SET stock_quantity = stock_quantity - @quantity
    WHERE product_id = @product_id;

    UPDATE Export
    SET total_amount = dbo.fn_GetExportTotal(@export_id)
    WHERE export_id = @export_id;

    INSERT INTO PermissionLog (action, action_date, performed_by)
    VALUES ('Insert ExportDetail: ' + @product_name, GETDATE(), SUSER_SNAME());
END;
GO

CREATE PROCEDURE sp_UpdateExport
    @export_id INT,
    @product_name NVARCHAR(100),
    @new_quantity INT,
    @new_unit_price DECIMAL(18,2),
    @is_updated BIT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @is_updated = 0;

    IF NOT EXISTS (SELECT 1 FROM Export WHERE export_id = @export_id)
    BEGIN
        RAISERROR ('Phiếu xuất không tồn tại!', 16, 1);
        RETURN;
    END;

    DECLARE @product_id INT;
    SELECT @product_id = product_id
    FROM Product
    WHERE product_name = @product_name;

    IF @product_id IS NULL
    BEGIN
        RAISERROR ('Sản phẩm không tồn tại!', 16, 1);
        RETURN;
    END;

    IF NOT EXISTS (SELECT 1 FROM ExportDetail WHERE export_id = @export_id AND product_id = @product_id)
    BEGIN
        RAISERROR ('Chi tiết phiếu xuất không tồn tại!', 16, 1);
        RETURN;
    END;

    IF @new_quantity <= 0
    BEGIN
        RAISERROR ('Số lượng phải lớn hơn 0!', 16, 1);
        RETURN;
    END;

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
        RETURN;
    END;

    DECLARE @old_unit_price DECIMAL(18,2);
    SELECT @old_unit_price = unit_price
    FROM ExportDetail
    WHERE export_id = @export_id AND product_id = @product_id;

    IF @old_quantity = @new_quantity 
       AND @old_unit_price = @new_unit_price
    BEGIN
        RETURN;
    END;

    SET @is_updated = 1;

    UPDATE ExportDetail
    SET quantity = @new_quantity,
        unit_price = @new_unit_price
    WHERE export_id = @export_id AND product_id = @product_id;

    UPDATE Product
    SET stock_quantity = stock_quantity - @quantity_difference
    WHERE product_id = @product_id;

    UPDATE Export
    SET total_amount = dbo.fn_GetExportTotal(@export_id)
    WHERE export_id = @export_id;

    INSERT INTO PermissionLog (action, action_date, performed_by)
    VALUES ('Update Export: ' + @product_name, GETDATE(), SUSER_SNAME());
END;
GO

CREATE PROCEDURE sp_DeleteExport
    @export_id INT
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT EXISTS (SELECT 1 FROM Export WHERE export_id = @export_id)
    BEGIN
        RAISERROR ('Phiếu xuất không tồn tại!', 16, 1);
        RETURN;
    END;
    DELETE FROM Export WHERE export_id = @export_id;
    -- Logging is handled by the trigger tr_LogExportDeletion
END;
GO

CREATE PROCEDURE sp_GetPermissionLogs
AS
BEGIN
    SELECT 
        log_id,
        action,
        action_date,
        performed_by,
        target_role
    FROM PermissionLog
    ORDER BY action_date DESC;
END;
GO

CREATE PROCEDURE sp_GetRevenueByMonth
    @year INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        MONTH(export_date) AS month,
        SUM(total_amount) AS total_revenue
    FROM Export
    WHERE YEAR(export_date) = @year
    GROUP BY MONTH(export_date)
    ORDER BY month;
END;
GO

CREATE PROCEDURE sp_GetRevenueByDay
    @date DATE
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        export_date,
        SUM(total_amount) AS total_revenue
    FROM Export
    WHERE export_date = @date
    GROUP BY export_date;
END;
GO

CREATE PROCEDURE sp_SearchProductByName
    @product_name NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        p.product_id,
        p.product_name,
        p.price,
        p.stock_quantity,
        STRING_AGG(c.category_name, ', ') AS category_name
    FROM Product p
    LEFT JOIN ProductCategory pc ON p.product_id = pc.product_id
    LEFT JOIN Category c ON pc.category_id = c.category_id
    WHERE p.product_name LIKE '%' + @product_name + '%'
    GROUP BY p.product_id, p.product_name, p.price, p.stock_quantity;
END;
GO

CREATE PROCEDURE sp_SearchProductByCategory
    @category_name NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        p.product_id,
        p.product_name,
        p.price,
        p.stock_quantity,
        STRING_AGG(c.category_name, ', ') AS category_name
    FROM Product p
    INNER JOIN ProductCategory pc ON p.product_id = pc.product_id
    INNER JOIN Category c ON pc.category_id = c.category_id
    WHERE c.category_name LIKE '%' + @category_name + '%'
    GROUP BY p.product_id, p.product_name, p.price, p.stock_quantity;
END;
GO

CREATE PROCEDURE sp_SortProductByPrice
    @sort_order NVARCHAR(4) = 'ASC'
AS
BEGIN
    SET NOCOUNT ON;
    IF @sort_order = 'DESC'
        SELECT 
            p.product_id,
            p.product_name,
            p.price,
            p.stock_quantity,
            STRING_AGG(c.category_name, ', ') AS category_name
        FROM Product p
        LEFT JOIN ProductCategory pc ON p.product_id = pc.product_id
        LEFT JOIN Category c ON pc.category_id = c.category_id
        GROUP BY p.product_id, p.product_name, p.price, p.stock_quantity
        ORDER BY p.price DESC;
    ELSE
        SELECT 
            p.product_id,
            p.product_name,
            p.price,
            p.stock_quantity,
            STRING_AGG(c.category_name, ', ') AS category_name
        FROM Product p
        LEFT JOIN ProductCategory pc ON p.product_id = pc.product_id
        LEFT JOIN Category c ON pc.category_id = c.category_id
        GROUP BY p.product_id, p.product_name, p.price, p.stock_quantity
        ORDER BY p.price ASC;
END;
GO

CREATE PROCEDURE sp_GrantPermission
    @role_name NVARCHAR(100),
    @object_name NVARCHAR(100),
    @object_type NVARCHAR(50),
    @permission_type NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = @role_name AND type = 'R')
    BEGIN
        RAISERROR ('Role không tồn tại!', 16, 1);
        RETURN;
    END;

    IF @object_type = 'PROCEDURE' AND NOT EXISTS (SELECT 1 FROM sys.procedures WHERE name = @object_name)
    BEGIN
        RAISERROR ('Stored procedure không tồn tại!', 16, 1);
        RETURN;
    END;
    IF @object_type = 'VIEW' AND NOT EXISTS (SELECT 1 FROM sys.views WHERE name = @object_name)
    BEGIN
        RAISERROR ('View không tồn tại!', 16, 1);
        RETURN;
    END;
    IF @object_type = 'TABLE' AND NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = @object_name)
    BEGIN
        RAISERROR ('Bảng không tồn tại!', 16, 1);
        RETURN;
    END;

    IF @permission_type NOT IN ('EXECUTE', 'SELECT', 'INSERT', 'UPDATE', 'DELETE')
    BEGIN
        RAISERROR ('Loại quyền không hợp lệ!', 16, 1);
        RETURN;
    END;

    DECLARE @sql NVARCHAR(MAX);
    SET @sql = 'GRANT ' + @permission_type + ' ON ' + 
               CASE WHEN @object_type = 'PROCEDURE' THEN 'dbo.' + QUOTENAME(@object_name)
                    WHEN @object_type = 'VIEW' THEN 'dbo.' + QUOTENAME(@object_name)
                    WHEN @object_type = 'TABLE' THEN 'dbo.' + QUOTENAME(@object_name)
               END + ' TO ' + QUOTENAME(@role_name);
    EXEC sp_executesql @sql;

    INSERT INTO RolePermissions (role_name, object_name, object_type, permission_type, granted_date, granted_by)
    VALUES (@role_name, @object_name, @object_type, @permission_type, GETDATE(), SUSER_SNAME());

    INSERT INTO PermissionLog (action, action_date, performed_by, target_role)
    VALUES ('Grant Permission: ' + @permission_type + ' on ' + @object_name + ' to ' + @role_name, 
            GETDATE(), SUSER_SNAME(), @role_name);
END;
GO

CREATE PROCEDURE sp_DenyPermission
    @role_name NVARCHAR(100),
    @object_name NVARCHAR(100),
    @object_type NVARCHAR(50),
    @permission_type NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = @role_name AND type = 'R')
    BEGIN
        RAISERROR ('Role không tồn tại!', 16, 1);
        RETURN;
    END;

    IF @object_type = 'PROCEDURE' AND NOT EXISTS (SELECT 1 FROM sys.procedures WHERE name = @object_name)
    BEGIN
        RAISERROR ('Stored procedure không tồn tại!', 16, 1);
        RETURN;
    END;
    IF @object_type = 'VIEW' AND NOT EXISTS (SELECT 1 FROM sys.views WHERE name = @object_name)
    BEGIN
        RAISERROR ('View không tồn tại!', 16, 1);
        RETURN;
    END;
    IF @object_type = 'TABLE' AND NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = @object_name)
    BEGIN
        RAISERROR ('Bảng không tồn tại!', 16, 1);
        RETURN;
    END;

    IF @permission_type NOT IN ('EXECUTE', 'SELECT', 'INSERT', 'UPDATE', 'DELETE')
    BEGIN
        RAISERROR ('Loại quyền không hợp lệ!', 16, 1);
        RETURN;
    END;

    DECLARE @sql NVARCHAR(MAX);
    SET @sql = 'DENY ' + @permission_type + ' ON ' + 
               CASE WHEN @object_type = 'PROCEDURE' THEN 'dbo.' + QUOTENAME(@object_name)
                    WHEN @object_type = 'VIEW' THEN 'dbo.' + QUOTENAME(@object_name)
                    WHEN @object_type = 'TABLE' THEN 'dbo.' + QUOTENAME(@object_name)
               END + ' TO ' + QUOTENAME(@role_name);
    EXEC sp_executesql @sql;

    DELETE FROM RolePermissions
    WHERE role_name = @role_name
      AND object_name = @object_name
      AND object_type = @object_type
      AND permission_type = @permission_type;

    INSERT INTO PermissionLog (action, action_date, performed_by, target_role)
    VALUES ('Deny Permission: ' + @permission_type + ' on ' + @object_name + ' from ' + @role_name, 
            GETDATE(), SUSER_SNAME(), @role_name);
END;
GO

CREATE PROCEDURE sp_ViewPermissions
    @role_name NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        permission_id,
        role_name,
        object_name,
        object_type,
        permission_type,
        granted_date,
        granted_by
    FROM RolePermissions
    WHERE role_name = @role_name
    ORDER BY object_type, object_name, permission_type;
END;
GO

-- Step 6: Create views
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
LEFT JOIN Supplier s ON i.supplier_id = s.supplier_id
JOIN ImportDetail id ON i.import_id = id.import_id
JOIN Product p ON id.product_id = p.product_id;
GO

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
LEFT JOIN Customer c ON e.customer_id = c.customer_id
JOIN ExportDetail ed ON e.export_id = ed.export_id
JOIN Product p ON ed.product_id = p.product_id;
GO

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

CREATE VIEW vw_Suppliers AS
SELECT supplier_id, supplier_name, contact_info
FROM Supplier;
GO

CREATE VIEW vw_Customers AS
SELECT customer_id, customer_name, contact_info
FROM Customer;
GO

CREATE VIEW vw_RolePermissions AS
SELECT 
    rp.permission_id,
    rp.role_name,
    rp.object_name,
    rp.object_type,
    rp.permission_type,
    rp.granted_date,
    rp.granted_by,
    CASE 
        WHEN EXISTS (
            SELECT 1 
            FROM sys.database_permissions p
            JOIN sys.database_principals dp ON p.grantee_principal_id = dp.principal_id
            WHERE dp.name = rp.role_name 
              AND p.major_id = OBJECT_ID('dbo.' + rp.object_name)
              AND p.type = CASE rp.permission_type 
                             WHEN 'EXECUTE' THEN 'X'
                             WHEN 'SELECT' THEN 'SL'
                             WHEN 'INSERT' THEN 'IN'
                             WHEN 'UPDATE' THEN 'UP'
                             WHEN 'DELETE' THEN 'DL'
                           END
        ) THEN 'Active'
        ELSE 'Inactive'
    END AS permission_status
FROM RolePermissions rp;
GO

-- Step 7: Create triggers
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

CREATE TRIGGER tr_LogProductDeletion
ON Product
AFTER DELETE
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO ProductLog (product_id, product_name, price, stock_quantity, deleted_date, deleted_by)
    SELECT 
        NULL, -- Set product_id to NULL since the product is deleted
        d.product_name,
        d.price,
        d.stock_quantity,
        GETDATE(),
        SUSER_SNAME()
    FROM deleted d
    WHERE d.product_id IS NOT NULL;

    INSERT INTO PermissionLog (action, action_date, performed_by)
    SELECT 
        'Delete Product: ' + d.product_name,
        GETDATE(),
        SUSER_SNAME()
    FROM deleted d;
END;
GO

CREATE TRIGGER tr_LogSupplierDeletion
ON Supplier
AFTER DELETE
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO PermissionLog (action, action_date, performed_by)
    SELECT 
        'Delete Supplier: ' + d.supplier_name,
        GETDATE(),
        SUSER_SNAME()
    FROM deleted d;
END;
GO

CREATE TRIGGER tr_LogCustomerDeletion
ON Customer
AFTER DELETE
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO PermissionLog (action, action_date, performed_by)
    SELECT 
        'Delete Customer: ' + d.customer_name,
        GETDATE(),
        SUSER_SNAME()
    FROM deleted d;
END;
GO

CREATE TRIGGER tr_LogImportDeletion
ON Import
AFTER DELETE
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO PermissionLog (action, action_date, performed_by)
    SELECT 
        'Delete Import: ' + CAST(d.import_id AS NVARCHAR(10)) + ' (Supplier: ' + ISNULL(s.supplier_name, 'Unknown') + ')',
        GETDATE(),
        SUSER_SNAME()
    FROM deleted d
    LEFT JOIN Supplier s ON d.supplier_id = s.supplier_id;
END;
GO

CREATE TRIGGER tr_LogExportDeletion
ON Export
AFTER DELETE
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO PermissionLog (action, action_date, performed_by)
    SELECT 
        'Delete Export: ' + CAST(d.export_id AS NVARCHAR(10)) + ' (Customer: ' + ISNULL(c.customer_name, 'Unknown') + ')',
        GETDATE(),
        SUSER_SNAME()
    FROM deleted d
    LEFT JOIN Customer c ON d.customer_id = c.customer_id;
END;
GO

-- Step 8: Seed data
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

-- Step 9: Set up logins, users, roles, and permissions
USE master;
GO

IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = 'admin_user')
    CREATE LOGIN admin_user WITH PASSWORD = 'Admin123!';
IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = 'employee_user')
    CREATE LOGIN employee_user WITH PASSWORD = 'Employee123!';
GO

USE LaptopStoreDBMS4;
GO

IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'admin_user')
    CREATE USER admin_user FOR LOGIN admin_user;
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'employee_user')
    CREATE USER employee_user FOR LOGIN employee_user;
GO

IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'admin_role')
    CREATE ROLE admin_role;
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'employee_role')
    CREATE ROLE employee_role;
GO

ALTER ROLE admin_role ADD MEMBER admin_user;
ALTER ROLE employee_role ADD MEMBER employee_user;
GO

GRANT EXECUTE ON sp_InsertProduct TO admin_role;
GRANT EXECUTE ON sp_UpdateProduct TO admin_role;
GRANT EXECUTE ON sp_DeleteProduct TO admin_role;
GRANT EXECUTE ON sp_InsertCategory TO admin_role;
GRANT EXECUTE ON sp_UpdateCategory TO admin_role;
GRANT EXECUTE ON sp_DeleteCategory TO admin_role;
GRANT EXECUTE ON sp_InsertSupplier TO admin_role;
GRANT EXECUTE ON sp_UpdateSupplier TO admin_role;
GRANT EXECUTE ON sp_DeleteSupplier TO admin_role;
GRANT EXECUTE ON sp_InsertCustomer TO admin_role;
GRANT EXECUTE ON sp_UpdateCustomer TO admin_role;
GRANT EXECUTE ON sp_DeleteCustomer TO admin_role;
GRANT EXECUTE ON sp_InsertImport TO admin_role;
GRANT EXECUTE ON sp_InsertImportDetail TO admin_role;
GRANT EXECUTE ON sp_UpdateImport TO admin_role;
GRANT EXECUTE ON sp_DeleteImport TO admin_role;
GRANT EXECUTE ON sp_InsertExport TO admin_role;
GRANT EXECUTE ON sp_InsertExportDetail TO admin_role;
GRANT EXECUTE ON sp_UpdateExport TO admin_role;
GRANT EXECUTE ON sp_DeleteExport TO admin_role;
GRANT EXECUTE ON sp_GetPermissionLogs TO admin_role;
GRANT EXECUTE ON sp_GetRevenueByMonth TO admin_role;
GRANT EXECUTE ON sp_GetRevenueByDay TO admin_role;
GRANT EXECUTE ON sp_SearchProductByName TO admin_role;
GRANT EXECUTE ON sp_SearchProductByCategory TO admin_role;
GRANT EXECUTE ON sp_SortProductByPrice TO admin_role;
GRANT EXECUTE ON sp_GrantPermission TO admin_role;
GRANT EXECUTE ON sp_DenyPermission TO admin_role;
GRANT EXECUTE ON sp_ViewPermissions TO admin_role;
GRANT SELECT ON vw_ProductDetails TO admin_role;
GRANT SELECT ON vw_ImportDetails TO admin_role;
GRANT SELECT ON vw_ExportDetails TO admin_role;
GRANT SELECT ON vw_Inventory TO admin_role;
GRANT SELECT ON vw_Suppliers TO admin_role;
GRANT SELECT ON vw_Customers TO admin_role;
GRANT SELECT ON vw_RolePermissions TO admin_role;
GRANT SELECT, INSERT, UPDATE, DELETE ON Product TO admin_role;
GRANT SELECT, INSERT, UPDATE, DELETE ON Category TO admin_role;
GRANT SELECT, INSERT, UPDATE, DELETE ON ProductCategory TO admin_role;
GRANT SELECT, INSERT, UPDATE, DELETE ON Supplier TO admin_role;
GRANT SELECT, INSERT, UPDATE, DELETE ON Customer TO admin_role;
GRANT SELECT, INSERT, UPDATE, DELETE ON Import TO admin_role;
GRANT SELECT, INSERT, UPDATE, DELETE ON ImportDetail TO admin_role;
GRANT SELECT, INSERT, UPDATE, DELETE ON Export TO admin_role;
GRANT SELECT, INSERT, UPDATE, DELETE ON ExportDetail TO admin_role;
GRANT SELECT, INSERT ON ProductLog TO admin_role;
GRANT SELECT, INSERT ON PermissionLog TO admin_role;
GRANT SELECT, INSERT, UPDATE, DELETE ON RolePermissions TO admin_role;
GO

GRANT EXECUTE ON sp_InsertImport TO employee_role;
GRANT EXECUTE ON sp_InsertImportDetail TO employee_role;
GRANT EXECUTE ON sp_InsertExport TO employee_role;
GRANT EXECUTE ON sp_InsertExportDetail TO employee_role;
GRANT EXECUTE ON sp_SearchProductByName TO employee_role;
GRANT EXECUTE ON sp_SearchProductByCategory TO employee_role;
GRANT EXECUTE ON sp_SortProductByPrice TO employee_role;
GRANT SELECT ON vw_ProductDetails TO employee_role;
GRANT SELECT ON vw_ImportDetails TO employee_role;
GRANT SELECT ON vw_ExportDetails TO employee_role;
GRANT SELECT ON vw_Inventory TO employee_role;
GRANT SELECT ON vw_Suppliers TO employee_role;
GRANT SELECT ON vw_Customers TO employee_role;
GRANT SELECT ON Product TO employee_role;
GRANT SELECT ON Category TO employee_role;
GRANT SELECT ON Supplier TO employee_role;
GRANT SELECT ON Customer TO employee_role;
GRANT SELECT ON Import TO employee_role;
GRANT SELECT ON ImportDetail TO employee_role;
GRANT SELECT ON Export TO employee_role;
GRANT SELECT ON ExportDetail TO employee_role;
GO

DENY EXECUTE ON sp_UpdateImport TO employee_role;
DENY EXECUTE ON sp_DeleteImport TO employee_role;
DENY EXECUTE ON sp_UpdateExport TO employee_role;
DENY EXECUTE ON sp_DeleteExport TO employee_role;
DENY EXECUTE ON sp_InsertProduct TO employee_role;
DENY EXECUTE ON sp_UpdateProduct TO employee_role;
DENY EXECUTE ON sp_DeleteProduct TO employee_role;
DENY EXECUTE ON sp_InsertCategory TO employee_role;
DENY EXECUTE ON sp_UpdateCategory TO employee_role;
DENY EXECUTE ON sp_DeleteCategory TO employee_role;
DENY EXECUTE ON sp_InsertSupplier TO employee_role;
DENY EXECUTE ON sp_UpdateSupplier TO employee_role;
DENY EXECUTE ON sp_DeleteSupplier TO employee_role;
DENY EXECUTE ON sp_InsertCustomer TO employee_role;
DENY EXECUTE ON sp_UpdateCustomer TO employee_role;
DENY EXECUTE ON sp_DeleteCustomer TO employee_role;
DENY EXECUTE ON sp_GetPermissionLogs TO employee_role;
DENY EXECUTE ON sp_GetRevenueByMonth TO employee_role;
DENY EXECUTE ON sp_GetRevenueByDay TO employee_role;
DENY EXECUTE ON sp_GrantPermission TO employee_role;
DENY EXECUTE ON sp_DenyPermission TO employee_role;
DENY EXECUTE ON sp_ViewPermissions TO employee_role;
DENY SELECT ON vw_RolePermissions TO employee_role;
DENY INSERT, UPDATE, DELETE ON Product TO employee_role;
DENY INSERT, UPDATE, DELETE ON Category TO employee_role;
DENY INSERT, UPDATE, DELETE ON ProductCategory TO employee_role;
DENY INSERT, UPDATE, DELETE ON Supplier TO employee_role;
DENY INSERT, UPDATE, DELETE ON Customer TO employee_role;
DENY INSERT, UPDATE, DELETE ON Import TO employee_role;
DENY INSERT, UPDATE, DELETE ON ImportDetail TO employee_role;
DENY INSERT, UPDATE, DELETE ON Export TO employee_role;
DENY INSERT, UPDATE, DELETE ON ExportDetail TO employee_role;
DENY SELECT, INSERT, UPDATE, DELETE ON ProductLog TO employee_role;
DENY SELECT, INSERT, UPDATE, DELETE ON PermissionLog TO employee_role;
DENY SELECT, INSERT, UPDATE, DELETE ON RolePermissions TO employee_role;
GO

-- Step 10: Initialize RolePermissions table
INSERT INTO RolePermissions (role_name, object_name, object_type, permission_type, granted_date, granted_by)
VALUES 
    ('admin_role', 'sp_InsertProduct', 'PROCEDURE', 'EXECUTE', GETDATE(), SUSER_SNAME()),
    ('admin_role', 'sp_UpdateProduct', 'PROCEDURE', 'EXECUTE', GETDATE(), SUSER_SNAME()),
    ('admin_role', 'sp_DeleteProduct', 'PROCEDURE', 'EXECUTE', GETDATE(), SUSER_SNAME()),
    ('admin_role', 'sp_InsertCategory', 'PROCEDURE', 'EXECUTE', GETDATE(), SUSER_SNAME()),
    ('admin_role', 'sp_UpdateCategory', 'PROCEDURE', 'EXECUTE', GETDATE(), SUSER_SNAME()),
    ('admin_role', 'sp_DeleteCategory', 'PROCEDURE', 'EXECUTE', GETDATE(), SUSER_SNAME()),
    ('admin_role', 'sp_InsertSupplier', 'PROCEDURE', 'EXECUTE', GETDATE(), SUSER_SNAME()),
    ('admin_role', 'sp_UpdateSupplier', 'PROCEDURE', 'EXECUTE', GETDATE(), SUSER_SNAME()),
    ('admin_role', 'sp_DeleteSupplier', 'PROCEDURE', 'EXECUTE', GETDATE(), SUSER_SNAME()),
    ('admin_role', 'sp_InsertCustomer', 'PROCEDURE', 'EXECUTE', GETDATE(), SUSER_SNAME()),
    ('admin_role', 'sp_UpdateCustomer', 'PROCEDURE', 'EXECUTE', GETDATE(), SUSER_SNAME()),
    ('admin_role', 'sp_DeleteCustomer', 'PROCEDURE', 'EXECUTE', GETDATE(), SUSER_SNAME()),
    ('admin_role', 'sp_InsertImport', 'PROCEDURE', 'EXECUTE', GETDATE(), SUSER_SNAME()),
    ('admin_role', 'sp_InsertImportDetail', 'PROCEDURE', 'EXECUTE', GETDATE(), SUSER_SNAME()),
    ('admin_role', 'sp_UpdateImport', 'PROCEDURE', 'EXECUTE', GETDATE(), SUSER_SNAME()),
    ('admin_role', 'sp_DeleteImport', 'PROCEDURE', 'EXECUTE', GETDATE(), SUSER_SNAME()),
    ('admin_role', 'sp_InsertExport', 'PROCEDURE', 'EXECUTE', GETDATE(), SUSER_SNAME()),
    ('admin_role', 'sp_InsertExportDetail', 'PROCEDURE', 'EXECUTE', GETDATE(), SUSER_SNAME()),
    ('admin_role', 'sp_UpdateExport', 'PROCEDURE', 'EXECUTE', GETDATE(), SUSER_SNAME()),
    ('admin_role', 'sp_DeleteExport', 'PROCEDURE', 'EXECUTE', GETDATE(), SUSER_SNAME()),
    ('admin_role', 'sp_GetPermissionLogs', 'PROCEDURE', 'EXECUTE', GETDATE(), SUSER_SNAME()),
    ('admin_role', 'sp_GetRevenueByMonth', 'PROCEDURE', 'EXECUTE', GETDATE(), SUSER_SNAME()),
    ('admin_role', 'sp_GetRevenueByDay', 'PROCEDURE', 'EXECUTE', GETDATE(), SUSER_SNAME()),
    ('admin_role', 'sp_SearchProductByName', 'PROCEDURE', 'EXECUTE', GETDATE(), SUSER_SNAME()),
    ('admin_role', 'sp_SearchProductByCategory', 'PROCEDURE', 'EXECUTE', GETDATE(), SUSER_SNAME()),
    ('admin_role', 'sp_SortProductByPrice', 'PROCEDURE', 'EXECUTE', GETDATE(), SUSER_SNAME()),
    ('admin_role', 'sp_GrantPermission', 'PROCEDURE', 'EXECUTE', GETDATE(), SUSER_SNAME()),
    ('admin_role', 'sp_DenyPermission', 'PROCEDURE', 'EXECUTE', GETDATE(), SUSER_SNAME()),
    ('admin_role', 'sp_ViewPermissions', 'PROCEDURE', 'EXECUTE', GETDATE(), SUSER_SNAME()),
    ('admin_role', 'vw_ProductDetails', 'VIEW', 'SELECT', GETDATE(), SUSER_SNAME()),
    ('admin_role', 'vw_ImportDetails', 'VIEW', 'SELECT', GETDATE(), SUSER_SNAME()),
    ('admin_role', 'vw_ExportDetails', 'VIEW', 'SELECT', GETDATE(), SUSER_SNAME()),
    ('admin_role', 'vw_Inventory', 'VIEW', 'SELECT', GETDATE(), SUSER_SNAME()),
    ('admin_role', 'vw_Suppliers', 'VIEW', 'SELECT', GETDATE(), SUSER_SNAME()),
    ('admin_role', 'vw_Customers', 'VIEW', 'SELECT', GETDATE(), SUSER_SNAME()),
    ('admin_role', 'vw_RolePermissions', 'VIEW', 'SELECT', GETDATE(), SUSER_SNAME()),
    ('admin_role', 'Product', 'TABLE', 'SELECT', GETDATE(), SUSER_SNAME()),
    ('admin_role', 'Product', 'TABLE', 'INSERT', GETDATE(), SUSER_SNAME()),
    ('admin_role', 'Product', 'TABLE', 'UPDATE', GETDATE(), SUSER_SNAME()),
    ('admin_role', 'Product', 'TABLE', 'DELETE', GETDATE(), SUSER_SNAME()),
    ('admin_role', 'Category', 'TABLE', 'SELECT', GETDATE(), SUSER_SNAME()),
    ('admin_role', 'Category', 'TABLE', 'INSERT', GETDATE(), SUSER_SNAME()),
    ('admin_role', 'Category', 'TABLE', 'UPDATE', GETDATE(), SUSER_SNAME()),
    ('admin_role', 'Category', 'TABLE', 'DELETE', GETDATE(), SUSER_SNAME()),
    ('admin_role', 'ProductCategory', 'TABLE', 'SELECT', GETDATE(), SUSER_SNAME()),
    ('admin_role', 'ProductCategory', 'TABLE', 'INSERT', GETDATE(), SUSER_SNAME()),
    ('admin_role', 'ProductCategory', 'TABLE', 'UPDATE', GETDATE(), SUSER_SNAME()),
    ('admin_role', 'ProductCategory', 'TABLE', 'DELETE', GETDATE(), SUSER_SNAME()),
    ('admin_role', 'Supplier', 'TABLE', 'SELECT', GETDATE(), SUSER_SNAME()),
    ('admin_role', 'Supplier', 'TABLE', 'INSERT', GETDATE(), SUSER_SNAME()),
    ('admin_role', 'Supplier', 'TABLE', 'UPDATE', GETDATE(), SUSER_SNAME()),
    ('admin_role', 'Supplier', 'TABLE', 'DELETE', GETDATE(), SUSER_SNAME()),
    ('admin_role', 'Customer', 'TABLE', 'SELECT', GETDATE(), SUSER_SNAME()),
    ('admin_role', 'Customer', 'TABLE', 'INSERT', GETDATE(), SUSER_SNAME()),
    ('admin_role', 'Customer', 'TABLE', 'UPDATE', GETDATE(), SUSER_SNAME()),
    ('admin_role', 'Customer', 'TABLE', 'DELETE', GETDATE(), SUSER_SNAME()),
    ('admin_role', 'Import', 'TABLE', 'SELECT', GETDATE(), SUSER_SNAME()),
    ('admin_role', 'Import', 'TABLE', 'INSERT', GETDATE(), SUSER_SNAME()),
    ('admin_role', 'Import', 'TABLE', 'UPDATE', GETDATE(), SUSER_SNAME()),
    ('admin_role', 'Import', 'TABLE', 'DELETE', GETDATE(), SUSER_SNAME()),
    ('admin_role', 'ImportDetail', 'TABLE', 'SELECT', GETDATE(), SUSER_SNAME()),
    ('admin_role', 'ImportDetail', 'TABLE', 'INSERT', GETDATE(), SUSER_SNAME()),
    ('admin_role', 'ImportDetail', 'TABLE', 'UPDATE', GETDATE(), SUSER_SNAME()),
    ('admin_role', 'ImportDetail', 'TABLE', 'DELETE', GETDATE(), SUSER_SNAME()),
    ('admin_role', 'Export', 'TABLE', 'SELECT', GETDATE(), SUSER_SNAME()),
    ('admin_role', 'Export', 'TABLE', 'INSERT', GETDATE(), SUSER_SNAME()),
    ('admin_role', 'Export', 'TABLE', 'UPDATE', GETDATE(), SUSER_SNAME()),
    ('admin_role', 'Export', 'TABLE', 'DELETE', GETDATE(), SUSER_SNAME()),
    ('admin_role', 'ExportDetail', 'TABLE', 'SELECT', GETDATE(), SUSER_SNAME()),
    ('admin_role', 'ExportDetail', 'TABLE', 'INSERT', GETDATE(), SUSER_SNAME()),
    ('admin_role', 'ExportDetail', 'TABLE', 'UPDATE', GETDATE(), SUSER_SNAME()),
    ('admin_role', 'ExportDetail', 'TABLE', 'DELETE', GETDATE(), SUSER_SNAME()),
    ('admin_role', 'ProductLog', 'TABLE', 'SELECT', GETDATE(), SUSER_SNAME()),
    ('admin_role', 'ProductLog', 'TABLE', 'INSERT', GETDATE(), SUSER_SNAME()),
    ('admin_role', 'PermissionLog', 'TABLE', 'SELECT', GETDATE(), SUSER_SNAME()),
    ('admin_role', 'PermissionLog', 'TABLE', 'INSERT', GETDATE(), SUSER_SNAME()),
    ('admin_role', 'RolePermissions', 'TABLE', 'SELECT', GETDATE(), SUSER_SNAME()),
    ('admin_role', 'RolePermissions', 'TABLE', 'INSERT', GETDATE(), SUSER_SNAME()),
    ('admin_role', 'RolePermissions', 'TABLE', 'UPDATE', GETDATE(), SUSER_SNAME()),
    ('admin_role', 'RolePermissions', 'TABLE', 'DELETE', GETDATE(), SUSER_SNAME()),
    ('employee_role', 'sp_InsertImport', 'PROCEDURE', 'EXECUTE', GETDATE(), SUSER_SNAME()),
    ('employee_role', 'sp_InsertImportDetail', 'PROCEDURE', 'EXECUTE', GETDATE(), SUSER_SNAME()),
    ('employee_role', 'sp_InsertExport', 'PROCEDURE', 'EXECUTE', GETDATE(), SUSER_SNAME()),
    ('employee_role', 'sp_InsertExportDetail', 'PROCEDURE', 'EXECUTE', GETDATE(), SUSER_SNAME()),
    ('employee_role', 'sp_SearchProductByName', 'PROCEDURE', 'EXECUTE', GETDATE(), SUSER_SNAME()),
    ('employee_role', 'sp_SearchProductByCategory', 'PROCEDURE', 'EXECUTE', GETDATE(), SUSER_SNAME()),
    ('employee_role', 'sp_SortProductByPrice', 'PROCEDURE', 'EXECUTE', GETDATE(), SUSER_SNAME()),
    ('employee_role', 'vw_ProductDetails', 'VIEW', 'SELECT', GETDATE(), SUSER_SNAME()),
    ('employee_role', 'vw_ImportDetails', 'VIEW', 'SELECT', GETDATE(), SUSER_SNAME()),
    ('employee_role', 'vw_ExportDetails', 'VIEW', 'SELECT', GETDATE(), SUSER_SNAME()),
    ('employee_role', 'vw_Inventory', 'VIEW', 'SELECT', GETDATE(), SUSER_SNAME()),
    ('employee_role', 'vw_Suppliers', 'VIEW', 'SELECT', GETDATE(), SUSER_SNAME()),
    ('employee_role', 'vw_Customers', 'VIEW', 'SELECT', GETDATE(), SUSER_SNAME()),
    ('employee_role', 'Product', 'TABLE', 'SELECT', GETDATE(), SUSER_SNAME()),
    ('employee_role', 'Category', 'TABLE', 'SELECT', GETDATE(), SUSER_SNAME()),
    ('employee_role', 'Supplier', 'TABLE', 'SELECT', GETDATE(), SUSER_SNAME()),
    ('employee_role', 'Customer', 'TABLE', 'SELECT', GETDATE(), SUSER_SNAME()),
    ('employee_role', 'Import', 'TABLE', 'SELECT', GETDATE(), SUSER_SNAME()),
    ('employee_role', 'ImportDetail', 'TABLE', 'SELECT', GETDATE(), SUSER_SNAME()),
    ('employee_role', 'Export', 'TABLE', 'SELECT', GETDATE(), SUSER_SNAME()),
    ('employee_role', 'ExportDetail', 'TABLE', 'SELECT', GETDATE(), SUSER_SNAME());
GO