/* =====================================================================
   e-SHOPPING - Script tạo CSDL SQL Server (chạy trong SSMS: Ctrl+A rồi F5)
   - Schema ext  : giả lập "Hệ thống quản lý sản phẩm" bên ngoài (chỉ đọc)
   - Schema dbo  : dữ liệu của chính hệ thống e-Shopping
   ===================================================================== */
USE master;
GO
IF DB_ID(N'eShoppingDB') IS NOT NULL
BEGIN
    ALTER DATABASE eShoppingDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE eShoppingDB;
END
GO
CREATE DATABASE eShoppingDB;
GO
USE eShoppingDB;
GO
CREATE SCHEMA ext;
GO

/* ---------- HỆ THỐNG NGOÀI: quản lý sản phẩm ---------- */
CREATE TABLE ext.ProductGroup (
    GroupId     INT IDENTITY(1,1) PRIMARY KEY,
    GroupName   NVARCHAR(100) NOT NULL UNIQUE
);
CREATE TABLE ext.Product (
    ProductId     INT IDENTITY(1,1) PRIMARY KEY,
    ProductCode   VARCHAR(20)    NOT NULL UNIQUE,
    ProductName   NVARCHAR(200)  NOT NULL,
    Manufacturer  NVARCHAR(100)  NOT NULL,
    ImageUrl      NVARCHAR(300)  NULL,
    Description   NVARCHAR(MAX)  NULL,
    Specs         NVARCHAR(MAX)  NULL,
    Price         DECIMAL(18,0)  NOT NULL CHECK (Price >= 0),
    InStock       BIT            NOT NULL DEFAULT 1,
    GroupId       INT            NOT NULL REFERENCES ext.ProductGroup(GroupId)
);
GO

/* ---------- HỆ THỐNG E-SHOPPING ---------- */
CREATE TABLE dbo.Customer (
    CustomerId    INT IDENTITY(1,1) PRIMARY KEY,
    FullName      NVARCHAR(100) NOT NULL,
    BirthDate     DATE          NOT NULL,
    IdNumber      VARCHAR(20)   NOT NULL UNIQUE,
    Address       NVARCHAR(250) NOT NULL,
    Phone         VARCHAR(15)   NOT NULL,
    Username      VARCHAR(50)   NOT NULL UNIQUE,
    PasswordHash  CHAR(64)      NOT NULL,
    Email         VARCHAR(150)  NULL
);

CREATE TABLE dbo.ShippingZone (
    ZoneId       INT IDENTITY(1,1) PRIMARY KEY,
    ZoneName     NVARCHAR(100) NOT NULL UNIQUE,
    NormalFee    DECIMAL(18,0) NOT NULL,
    ExpressFee   DECIMAL(18,0) NOT NULL,
    SameDayFee   DECIMAL(18,0) NOT NULL
);

CREATE TABLE dbo.CardFee (
    CardType   TINYINT       PRIMARY KEY,
    CardName   NVARCHAR(30)  NOT NULL,
    UsageFee   DECIMAL(18,0) NOT NULL
);

CREATE TABLE dbo.Orders (
    OrderId          INT IDENTITY(1,1) PRIMARY KEY,
    CustomerId       INT           NOT NULL REFERENCES dbo.Customer(CustomerId),
    OrderDate        DATETIME      NOT NULL DEFAULT GETDATE(),
    OrderType        TINYINT       NOT NULL CHECK (OrderType IN (1,2,3)),
    ReceiverName     NVARCHAR(100) NOT NULL,
    ReceiverAddress  NVARCHAR(250) NOT NULL,
    ReceiverPhone    VARCHAR(15)   NOT NULL,
    ZoneId           INT           NOT NULL REFERENCES dbo.ShippingZone(ZoneId),
    CardType         TINYINT       NOT NULL REFERENCES dbo.CardFee(CardType),
    CardLast4        CHAR(4)       NOT NULL,
    CardHolder       NVARCHAR(100) NOT NULL,
    AuthCode         VARCHAR(20)   NOT NULL,
    ItemsTotal       DECIMAL(18,0) NOT NULL,
    ShippingFee      DECIMAL(18,0) NOT NULL,
    CardFee          DECIMAL(18,0) NOT NULL,
    TotalAmount      DECIMAL(18,0) NOT NULL,
    Status           TINYINT       NOT NULL DEFAULT 1
);

CREATE TABLE dbo.OrderItem (
    OrderItemId  INT IDENTITY(1,1) PRIMARY KEY,
    OrderId      INT           NOT NULL REFERENCES dbo.Orders(OrderId),
    ProductId    INT           NOT NULL,
    ProductName  NVARCHAR(200) NOT NULL,
    Quantity     INT           NOT NULL CHECK (Quantity > 0),
    UnitPrice    DECIMAL(18,0) NOT NULL
);

CREATE TABLE dbo.PaymentTransaction (
    TxId       INT IDENTITY(1,1) PRIMARY KEY,
    OrderId    INT           NULL REFERENCES dbo.Orders(OrderId),
    CardType   TINYINT       NOT NULL,
    CardLast4  CHAR(4)       NOT NULL,
    Amount     DECIMAL(18,0) NOT NULL,
    Approved   BIT           NOT NULL,
    Message    NVARCHAR(200) NULL,
    CreatedAt  DATETIME      NOT NULL DEFAULT GETDATE()
);

CREATE TABLE dbo.EmailLog (
    EmailLogId  INT IDENTITY(1,1) PRIMARY KEY,
    OrderId     INT           NOT NULL REFERENCES dbo.Orders(OrderId),
    ToAddress   VARCHAR(150)  NOT NULL,
    Subject     NVARCHAR(200) NOT NULL,
    Body        NVARCHAR(MAX) NOT NULL,
    Sent        BIT           NOT NULL,
    SentAt      DATETIME      NOT NULL DEFAULT GETDATE()
);
GO

/* ---------- DỮ LIỆU MẪU ---------- */
INSERT ext.ProductGroup(GroupName) VALUES
 (N'Máy chụp hình kỹ thuật số'), (N'Đồ chơi'), (N'Thiết bị điện gia dụng'), (N'Thiết bị máy tính');

INSERT ext.Product(ProductCode,ProductName,Manufacturer,ImageUrl,Description,Specs,Price,InStock,GroupId) VALUES
 ('CAM001',N'Canon IXUS 185',N'Canon',NULL,N'Máy ảnh compact nhỏ gọn, dễ dùng.',N'20MP; zoom quang 8x; quay HD 720p',2900000,1,1),
 ('CAM002',N'Sony Cyber-shot W830',N'Sony',NULL,N'Máy ảnh du lịch mỏng nhẹ.',N'20.1MP; zoom 8x',3200000,1,1),
 ('CAM003',N'Nikon COOLPIX A10',N'Nikon',NULL,N'Máy ảnh giá rẻ cho người mới.',N'16.1MP; zoom 5x',2100000,0,1),
 ('TOY001',N'Bộ xếp hình Lego City',N'Lego',NULL,N'Bộ xếp hình thành phố 300 chi tiết.',N'300 chi tiết; tuổi 6+',650000,1,2),
 ('TOY002',N'Xe điều khiển từ xa',N'Hot Wheels',NULL,N'Xe địa hình điều khiển từ xa.',N'Pin sạc; tầm xa 30m',450000,1,2),
 ('TOY003',N'Gấu bông Teddy 80cm',N'Gấu Việt',NULL,N'Gấu bông cao cấp, mềm mịn.',N'Cao 80cm; bông PP',320000,1,2),
 ('APP001',N'Nồi chiên không dầu 5L',N'Philips',NULL,N'Nồi chiên không dầu dung tích 5L.',N'1700W; 5L; hẹn giờ 60 phút',2490000,1,3),
 ('APP002',N'Máy xay sinh tố',N'Panasonic',NULL,N'Máy xay đa năng.',N'600W; cối thủy tinh 1.5L',890000,1,3),
 ('APP003',N'Bàn ủi hơi nước',N'Tefal',NULL,N'Bàn ủi hơi nước công suất cao.',N'2400W; bình 270ml',780000,1,3),
 ('PC001',N'Laptop Dell Inspiron 15',N'Dell',NULL,N'Laptop văn phòng.',N'i5-1235U; 8GB; 512GB SSD; 15.6 inch',15990000,1,4),
 ('PC002',N'Chuột không dây Logitech M331',N'Logitech',NULL,N'Chuột không dây im lặng.',N'Wireless 2.4GHz; DPI 1000',250000,1,4),
 ('PC003',N'Bàn phím cơ Keychron K2',N'Keychron',NULL,N'Bàn phím cơ không dây.',N'75%; Bluetooth 5.1',1850000,1,4);

INSERT dbo.ShippingZone(ZoneName,NormalFee,ExpressFee,SameDayFee) VALUES
 (N'Nội thành TP.HCM',20000,40000,70000),
 (N'Ngoại thành TP.HCM',30000,55000,90000),
 (N'Tỉnh thành khác',45000,80000,150000);

INSERT dbo.CardFee(CardType,CardName,UsageFee) VALUES
 (1,N'Visa',10000),(2,N'Master',10000),(3,N'Discover',15000),(4,N'American Express',20000);

-- Khách hàng mẫu: demo / 123456
INSERT dbo.Customer(FullName,BirthDate,IdNumber,Address,Phone,Username,PasswordHash,Email) VALUES
 (N'Nguyễn Văn A','2000-01-15','079123456789',N'12 Lê Lợi, Q.1, TP.HCM','0901234567','demo',
  '8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92','demo@example.com');
GO
SELECT 'OK' AS Result, (SELECT COUNT(*) FROM ext.Product) AS Products;
