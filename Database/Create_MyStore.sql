/*
    MyStore database bootstrap for BE1.

    Safe re-run policy:
    - Creates MyStore only when it does not already exist.
    - Creates each source table only when it is missing.
    - Never drops, alters, or seeds existing data.
    - The source schema's ProductDecription spelling is intentional.

    Run this file against the intended SQL Server instance, for example:
      (localdb)\MSSQLLocalDB
*/
IF DB_ID(N'MyStore') IS NULL
BEGIN
    CREATE DATABASE [MyStore];
END
GO

USE [MyStore];
GO

-- Source table: User
IF OBJECT_ID(N'dbo.[User]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[User] (
        [Username] NVARCHAR(255) NOT NULL,
        [Password] NCHAR(50) NOT NULL,
        [UserRole] NCHAR(1) NOT NULL,
        CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ([Username] ASC)
    );
END
GO

-- Source table: Customer
IF OBJECT_ID(N'dbo.[Customer]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Customer] (
        [CustomerID] INT IDENTITY(1, 1) NOT NULL,
        [CustomerName] NVARCHAR(MAX) NOT NULL,
        [CustomerPhone] NVARCHAR(15) NOT NULL,
        [CustomerEmail] NVARCHAR(MAX) NULL,
        [CustomerAddress] NVARCHAR(MAX) NULL,
        [Username] NVARCHAR(255) NOT NULL,
        CONSTRAINT [PK_Customer] PRIMARY KEY CLUSTERED ([CustomerID] ASC),
        CONSTRAINT [FK_User_Customer] FOREIGN KEY ([Username]) REFERENCES [dbo].[User] ([Username])
    );
END
GO

-- Source table: Category
IF OBJECT_ID(N'dbo.[Category]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Category] (
        [CategoryID] INT IDENTITY(1, 1) NOT NULL,
        [CategoryName] NVARCHAR(MAX) NOT NULL,
        CONSTRAINT [PK_Category] PRIMARY KEY CLUSTERED ([CategoryID] ASC)
    );
END
GO

-- Source table: Product. ProductDecription is the spelling in the supplied schema.
IF OBJECT_ID(N'dbo.[Product]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Product] (
        [ProductID] INT IDENTITY(1, 1) NOT NULL,
        [CategoryID] INT NOT NULL,
        [ProductName] NVARCHAR(MAX) NOT NULL,
        [ProductDecription] NVARCHAR(MAX) NOT NULL,
        [ProductPrice] DECIMAL(18, 2) NOT NULL,
        [ProductImage] NVARCHAR(MAX) NULL,
        CONSTRAINT [PK_Product] PRIMARY KEY CLUSTERED ([ProductID] ASC),
        CONSTRAINT [FK_Pro_Category] FOREIGN KEY ([CategoryID]) REFERENCES [dbo].[Category] ([CategoryID])
    );
END
GO

-- Source table: Order
IF OBJECT_ID(N'dbo.[Order]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Order] (
        [OrderID] INT IDENTITY(1, 1) NOT NULL,
        [CustomerID] INT NOT NULL,
        [OrderDate] DATE NOT NULL,
        [TotalAmount] DECIMAL(18, 2) NOT NULL,
        [PaymentStatus] NVARCHAR(MAX) NULL,
        [AddressDelivery] NVARCHAR(MAX) NOT NULL,
        CONSTRAINT [PK_Order] PRIMARY KEY CLUSTERED ([OrderID] ASC),
        CONSTRAINT [FK_Order_Customer] FOREIGN KEY ([CustomerID]) REFERENCES [dbo].[Customer] ([CustomerID])
    );
END
GO

-- Source table: OrderDetail
IF OBJECT_ID(N'dbo.[OrderDetail]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[OrderDetail] (
        [ID] INT IDENTITY(1, 1) NOT NULL,
        [ProductID] INT NOT NULL,
        [OrderID] INT NOT NULL,
        [Quantity] INT NOT NULL,
        [UnitPrice] DECIMAL(18, 2) NOT NULL,
        CONSTRAINT [PK_OrderDetail] PRIMARY KEY CLUSTERED ([ID] ASC),
        CONSTRAINT [FK_OrderDetail_Product] FOREIGN KEY ([ProductID]) REFERENCES [dbo].[Product] ([ProductID]),
        CONSTRAINT [FK_OrderDetail_Order] FOREIGN KEY ([OrderID]) REFERENCES [dbo].[Order] ([OrderID])
    );
END
GO
