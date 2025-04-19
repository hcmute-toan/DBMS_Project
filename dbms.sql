-- 1. B?ng Specs
CREATE TABLE Specs (
    spec_id INT PRIMARY KEY IDENTITY(1,1),
    spec_name NVARCHAR(100) NOT NULL UNIQUE
);

-- 2. B?ng Category
CREATE TABLE Category (
    category_id INT PRIMARY KEY IDENTITY(1,1),
    category_name NVARCHAR(100) NOT NULL UNIQUE,
    description NVARCHAR(255)
);

-- 3. B?ng Supplier
CREATE TABLE Supplier (
    supplier_id INT PRIMARY KEY IDENTITY(1,1),
    supplier_name NVARCHAR(100) NOT NULL,
    phone NVARCHAR(20),
    email NVARCHAR(100),
    created_at DATETIME NOT NULL DEFAULT GETDATE()
);

-- 4. B?ng Warehouse
CREATE TABLE Warehouse (
    warehouse_id INT PRIMARY KEY IDENTITY(1,1),
    name NVARCHAR(100) NOT NULL,
    address NVARCHAR(255),
    slot INT NOT NULL
);

-- 5. B?ng Customer
CREATE TABLE Customer (
    customer_id INT PRIMARY KEY IDENTITY(1,1),
    name NVARCHAR(100) NOT NULL,
    phone NVARCHAR(20),
    email NVARCHAR(100),
    address NVARCHAR(255)
);

-- 6. B?ng Product
CREATE TABLE Product (
    product_id INT PRIMARY KEY IDENTITY(1,1),
    product_name NVARCHAR(100) NOT NULL,
    description NVARCHAR(255),
    wholesale_price DECIMAL(18,2) NOT NULL,
    sell_price DECIMAL(18,2) NOT NULL,
    EAN13 NVARCHAR(13),
    brand NVARCHAR(50),
    status BIT NOT NULL DEFAULT 1, -- 1: Active, 0: Inactive
    img NVARCHAR(255), -- ???ng d?n hình ?nh
    import_price DECIMAL(18,2) NOT NULL,
    warehouse_id INT,
    FOREIGN KEY (warehouse_id) REFERENCES Warehouse(warehouse_id)
);

-- 7. B?ng ProductCategory (b?ng trung gian Product - Category)
CREATE TABLE ProductCategory (
    product_id INT,
    category_id INT,
    PRIMARY KEY (product_id, category_id),
    FOREIGN KEY (product_id) REFERENCES Product(product_id),
    FOREIGN KEY (category_id) REFERENCES Category(category_id)
);

-- 8. B?ng ProductSpecs (b?ng trung gian Product - Specs)
CREATE TABLE ProductSpecs (
    product_id INT,
    spec_id INT,
    PRIMARY KEY (product_id, spec_id),
    FOREIGN KEY (product_id) REFERENCES Product(product_id),
    FOREIGN KEY (spec_id) REFERENCES Specs(spec_id)
);

-- 9. B?ng Role (?? h? tr? phân quy?n)
CREATE TABLE Role (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(50) NOT NULL UNIQUE
);

-- 10. B?ng Permission (?? h? tr? phân quy?n)
CREATE TABLE Permission (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL UNIQUE,
    Description NVARCHAR(255)
);

-- 11. B?ng RolePermission (b?ng trung gian Role - Permission)
CREATE TABLE RolePermission (
    RoleId INT,
    PermissionId INT,
    PRIMARY KEY (RoleId, PermissionId),
    FOREIGN KEY (RoleId) REFERENCES Role(Id),
    FOREIGN KEY (PermissionId) REFERENCES Permission(Id)
);

-- 12. B?ng User
CREATE TABLE [User] (
    user_id INT PRIMARY KEY IDENTITY(1,1),
    username NVARCHAR(50) NOT NULL UNIQUE,
    password NVARCHAR(255) NOT NULL, -- L?u m?t kh?u mã hóa
    role NVARCHAR(50) NOT NULL, -- Trong bi?u ?? là role, có th? liên k?t v?i Role n?u c?n
    phone NVARCHAR(20),
    address NVARCHAR(255),
    email NVARCHAR(100),
    img NVARCHAR(255), -- ???ng d?n hình ?nh
    created_at DATETIME NOT NULL DEFAULT GETDATE(),
    RoleId INT, -- Thêm khóa ngo?i ?? liên k?t v?i Role (phân quy?n)
    FOREIGN KEY (RoleId) REFERENCES Role(Id)
);

-- 13. B?ng Import
CREATE TABLE [Import] (
    import_id INT PRIMARY KEY IDENTITY(1,1),
    total_amount DECIMAL(18,2) NOT NULL,
    import_date DATETIME NOT NULL,
    supplier_id INT,
    warehouse_id INT,
    FOREIGN KEY (supplier_id) REFERENCES Supplier(supplier_id),
    FOREIGN KEY (warehouse_id) REFERENCES Warehouse(warehouse_id)
);

-- 14. B?ng ImportDetail
CREATE TABLE ImportDetail (
    import_detail_id INT PRIMARY KEY IDENTITY(1,1),
    import_id INT,
    product_id INT,
    quantity INT NOT NULL,
    unit_price DECIMAL(18,2) NOT NULL,
    FOREIGN KEY (import_id) REFERENCES [Import](import_id),
    FOREIGN KEY (product_id) REFERENCES Product(product_id)
);

-- 15. B?ng ImportUser (b?ng trung gian Import - User)
CREATE TABLE ImportUser (
    import_id INT,
    user_id INT,
    PRIMARY KEY (import_id, user_id),
    FOREIGN KEY (import_id) REFERENCES [Import](import_id),
    FOREIGN KEY (user_id) REFERENCES [User](user_id)
);

-- 16. B?ng Export
CREATE TABLE [Export] (
    export_id INT PRIMARY KEY IDENTITY(1,1),
    export_date DATETIME NOT NULL,
    purpose NVARCHAR(255),
    customer_id INT,
    warehouse_id INT,
    FOREIGN KEY (customer_id) REFERENCES Customer(customer_id),
    FOREIGN KEY (warehouse_id) REFERENCES Warehouse(warehouse_id)
);

-- 17. B?ng ExportDetail
CREATE TABLE ExportDetail (
    export_detail_id INT PRIMARY KEY IDENTITY(1,1),
    export_id INT,
    product_id INT,
    quantity INT NOT NULL,
    unit_price DECIMAL(18,2) NOT NULL,
    FOREIGN KEY (export_id) REFERENCES [Export](export_id),
    FOREIGN KEY (product_id) REFERENCES Product(product_id)
);

-- 18. B?ng ExportUser (b?ng trung gian Export - User)
CREATE TABLE ExportUser (
    export_id INT,
    user_id INT,
    PRIMARY KEY (export_id, user_id),
    FOREIGN KEY (export_id) REFERENCES [Export](export_id),
    FOREIGN KEY (user_id) REFERENCES [User](user_id)
);

-- 19. B?ng Notification
CREATE TABLE Notification (
    notification_id INT PRIMARY KEY IDENTITY(1,1),
    message NVARCHAR(255) NOT NULL,
    notification_date DATETIME NOT NULL,
    is_read BIT NOT NULL DEFAULT 0, -- 0: Ch?a ??c, 1: ?ã ??c
    user_id INT,
    FOREIGN KEY (user_id) REFERENCES [User](user_id)
);