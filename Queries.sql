CREATE TABLE [dbo].[Category] (
    [CategoryId]   INT           IDENTITY (1, 1) NOT NULL,
    [CategoryName] VARCHAR (255) NULL,
    PRIMARY KEY CLUSTERED ([CategoryId] ASC)
);
CREATE TABLE [dbo].[DeliveryService] (
    [DServiceId]   INT             IDENTITY (1, 1) NOT NULL,
    [ImageUrl]     VARCHAR (255)   NULL,
    [ServiceName]  VARCHAR (100)   NOT NULL,
    [Price]        DECIMAL (10, 2) NOT NULL,
    [DeliveryDays] INT             NOT NULL,
    PRIMARY KEY CLUSTERED ([DServiceId] ASC)
);

CREATE TABLE [dbo].[Cart] (
    [CartId] INT IDENTITY (1, 1) NOT NULL,
    PRIMARY KEY CLUSTERED ([CartId] ASC)
);

CREATE TABLE [dbo].[CartItem] (
    [CartItemId] INT IDENTITY (1, 1) NOT NULL,
    [ProductId]  INT NOT NULL,
    [Quantity]   INT NOT NULL,
    [CartId]     INT NULL,
    PRIMARY KEY CLUSTERED ([CartItemId] ASC),
    CONSTRAINT [Cart_CartItem] FOREIGN KEY ([CartId]) REFERENCES [dbo].[Cart] ([CartId]) ON DELETE CASCADE,
    CONSTRAINT [Product_CartItem] FOREIGN KEY ([ProductId]) REFERENCES [dbo].[Product] ([ProductId]) ON DELETE CASCADE
);


CREATE TABLE [dbo].[DeliveryService] (
    [DServiceId]   INT             IDENTITY (1, 1) NOT NULL,
    [ImageUrl]     VARCHAR (255)   NULL,
    [ServiceName]  VARCHAR (100)   NOT NULL,
    [Price]        DECIMAL (10, 2) NOT NULL,
    [DeliveryDays] INT             NOT NULL,
    PRIMARY KEY CLUSTERED ([DServiceId] ASC)
);


CREATE TABLE [dbo].[Product] (
    [ProductId]       INT             IDENTITY (1, 1) NOT NULL,
    [ProductName]     VARCHAR (255)   NULL,
    [ProductDesc]     TEXT            NULL,
    [ProductPrice]    DECIMAL (10, 2) NULL,
    [ProductImageUrl] TEXT            NULL,
    [CategoryId]      INT             NULL,
    PRIMARY KEY CLUSTERED ([ProductId] ASC),
    CONSTRAINT [Product_Category] FOREIGN KEY ([CategoryId]) REFERENCES [dbo].[Category] ([CategoryId]) ON DELETE CASCADE
);


CREATE TABLE [dbo].[Addresses] (
    [AddressId]   INT            IDENTITY (1, 1) NOT NULL,
    [Street]      VARCHAR (255)  NULL,
    [City]        VARCHAR (255)  NULL,
    [States]      VARCHAR (255)  NULL,
    [ZipCode]     VARCHAR (255)  NULL,
    [UserId]      NVARCHAR (450) NULL,
    [PhoneNumber] VARCHAR (20)   NULL,
    PRIMARY KEY CLUSTERED ([AddressId] ASC)
);

CREATE TABLE [dbo].[Invoice] (
    [InvoiceId] INT IDENTITY (1, 1) NOT NULL,
    [OrderId]   INT NULL,
    PRIMARY KEY CLUSTERED ([InvoiceId] ASC)
);

CREATE TABLE [dbo].[Orders] (
    [OrderId]       	INT         	IDENTITY (1, 1) NOT NULL,
    [OrderDate]     	DATE        	CONSTRAINT [DF_DateColumn] DEFAULT (CONVERT([date],switchoffset(getdate(),'+05:30'))) NULL,
    [UserId]        	NVARCHAR (450)  NULL,
    [AddressId]     	INT         	NULL,
    [OrderAmount]   	DECIMAL (10, 2) NULL,
    [OrderTime]     	TIME (7)    	DEFAULT (CONVERT([time],switchoffset(getdate(),'+05:30'))) NULL,
    [ProductId]     	INT         	NULL,
    [CartId]        	INT         	NULL,
    [DeliveryServiceID] INT         	NULL,
    [IsCancelled]   	BIT         	DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([OrderId] ASC),
    CONSTRAINT [Order_Address] FOREIGN KEY ([AddressId]) REFERENCES [dbo].[Addresses] ([AddressId]) ON DELETE CASCADE,
    CONSTRAINT [Order_Delivery] FOREIGN KEY ([DeliveryServiceID]) REFERENCES [dbo].[DeliveryService] ([DServiceId]) ON DELETE CASCADE,
    CONSTRAINT [Order_Product] FOREIGN KEY ([ProductId]) REFERENCES [dbo].[Product] ([ProductId]) ON DELETE CASCADE,
    CONSTRAINT [Order_Cart] FOREIGN KEY ([CartId]) REFERENCES [dbo].[Cart] ([CartId]),
    CONSTRAINT [FK_OtherTable_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]),
    CONSTRAINT [FK_Orders_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id])
);

-- OrderHistory Table
CREATE TABLE [dbo].[OrderHistory] (
    [OrderId] INT NOT NULL,
    [ProductId] INT NOT NULL,
    [Quantity] INT NOT NULL,
    CONSTRAINT [FK_OrderHistory_Orders] FOREIGN KEY ([OrderId]) REFERENCES [dbo].[Orders] ([OrderId]) ON DELETE CASCADE,
    CONSTRAINT [FK_OrderHistory_Product] FOREIGN KEY ([ProductId]) REFERENCES [dbo].[Product] ([ProductId]) ON DELETE CASCADE
);
