----------------------------------
-- ADD merchCustomer table --
----------------------------------
ALTER TABLE [dbo].[merchCustomerAddress] DROP CONSTRAINT [FK_merchCustomerAddress_merchCustomer]
GO

ALTER TABLE [dbo].[merchInvoice] DROP CONSTRAINT [FK_merchInvoice_merchCustomer]
GO

ALTER TABLE [dbo].[merchPayment] DROP CONSTRAINT [FK_merchPayment_merchCustomer]
GO

ALTER TABLE [dbo].[merchCustomer] DROP CONSTRAINT [DF_merchCustomer_createDate]
GO

ALTER TABLE [dbo].[merchCustomer] DROP CONSTRAINT [DF_merchCustomer_updateDate]
GO

ALTER TABLE [dbo].[merchCustomer] DROP CONSTRAINT [DF_merchCustomer_lastActivityDate]
GO

ALTER TABLE [dbo].[merchCustomer] DROP CONSTRAINT [DF_merchCustomer_pk]
GO

/****** Object:  Table [dbo].[merchCustomer]    Script Date: 7/24/2014 4:27:37 PM ******/
DROP TABLE [dbo].[merchCustomer]
GO

/****** Object:  Table [dbo].[merchCustomer]    Script Date: 7/24/2014 4:27:37 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[merchCustomer](
	[pk] [uniqueidentifier] NOT NULL,
	[loginName] [nvarchar](255) NOT NULL,
	[firstName] [nvarchar](255) NOT NULL,
	[lastName] [nvarchar](255) NOT NULL,
	[email] [nvarchar](255) NOT NULL,
	[taxExempt] [bit] NOT NULL,
	[lastActivityDate] [datetime] NOT NULL,
	[extendedData] [ntext] NULL,
	[notes] [ntext] NULL,
	[updateDate] [datetime] NOT NULL,
	[createDate] [datetime] NOT NULL,
 CONSTRAINT [PK_merchCustomer] PRIMARY KEY CLUSTERED 
(
	[pk] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

ALTER TABLE [dbo].[merchCustomer] ADD  CONSTRAINT [DF_merchCustomer_pk]  DEFAULT ('newid()') FOR [pk]
GO

ALTER TABLE [dbo].[merchCustomer] ADD  CONSTRAINT [DF_merchCustomer_lastActivityDate]  DEFAULT (getdate()) FOR [lastActivityDate]
GO

ALTER TABLE [dbo].[merchCustomer] ADD  CONSTRAINT [DF_merchCustomer_updateDate]  DEFAULT (getdate()) FOR [updateDate]
GO

ALTER TABLE [dbo].[merchCustomer] ADD  CONSTRAINT [DF_merchCustomer_createDate]  DEFAULT (getdate()) FOR [createDate]
GO

ALTER TABLE [dbo].[merchInvoice]  WITH CHECK ADD  CONSTRAINT [FK_merchInvoice_merchCustomer] FOREIGN KEY([customerKey])
REFERENCES [dbo].[merchCustomer] ([pk])
GO

ALTER TABLE [dbo].[merchPayment]  WITH CHECK ADD  CONSTRAINT [FK_merchPayment_merchCustomer] FOREIGN KEY([customerKey])
REFERENCES [dbo].[merchCustomer] ([pk])
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_merchCustomerLoginName]
    ON [dbo].[merchCustomer]([loginName] ASC);
GO

----------------------------------
-- Add merchCustomerAddress table
----------------------------------
ALTER TABLE [dbo].[merchCustomerAddress] DROP CONSTRAINT [DF_merchCustomerAddress_createDate]
GO

ALTER TABLE [dbo].[merchCustomerAddress] DROP CONSTRAINT [DF_merchCustomerAddress_updateDate]
GO

ALTER TABLE [dbo].[merchCustomerAddress] DROP CONSTRAINT [DF_merchCustomerAddress_pk]
GO

/****** Object:  Table [dbo].[merchCustomerAddress]    Script Date: 7/24/2014 4:28:40 PM ******/
DROP TABLE [dbo].[merchCustomerAddress]
GO

/****** Object:  Table [dbo].[merchCustomerAddress]    Script Date: 7/24/2014 4:28:40 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[merchCustomerAddress](
	[pk] [uniqueidentifier] NOT NULL,
	[customerKey] [uniqueidentifier] NOT NULL,
	[label] [nvarchar](255) NULL,
	[fullName] [nvarchar](255) NULL,
	[company] [nvarchar](255) NULL,
	[addressTfKey] [uniqueidentifier] NOT NULL,
	[address1] [nvarchar](255) NULL,
	[address2] [nvarchar](255) NULL,
	[locality] [nvarchar](255) NULL,
	[region] [nvarchar](255) NULL,
	[postalCode] [nvarchar](255) NULL,
	[countryCode] [nvarchar](255) NULL,
	[phone] [nvarchar](255) NULL,
	[isDefault] [bit] NOT NULL,
	[updateDate] [datetime] NOT NULL,
	[createDate] [datetime] NOT NULL,
 CONSTRAINT [PK_merchCustomerAddress] PRIMARY KEY CLUSTERED 
(
	[pk] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[merchCustomerAddress] ADD  CONSTRAINT [DF_merchCustomerAddress_pk]  DEFAULT ('newid()') FOR [pk]
GO

ALTER TABLE [dbo].[merchCustomerAddress] ADD  CONSTRAINT [DF_merchCustomerAddress_updateDate]  DEFAULT (getdate()) FOR [updateDate]
GO

ALTER TABLE [dbo].[merchCustomerAddress] ADD  CONSTRAINT [DF_merchCustomerAddress_createDate]  DEFAULT (getdate()) FOR [createDate]
GO

ALTER TABLE [dbo].[merchCustomerAddress]  WITH CHECK ADD  CONSTRAINT [FK_merchCustomerAddress_merchCustomer] FOREIGN KEY([customerKey])
REFERENCES [dbo].[merchCustomer] ([pk])
GO

ALTER TABLE [dbo].[merchCustomerAddress] CHECK CONSTRAINT [FK_merchCustomerAddress_merchCustomer]
GO

----------------------------------
-- add merchCustomerIndex table --
----------------------------------
CREATE TABLE [dbo].[merchCustomerIndex](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[customerKey] [uniqueidentifier] NOT NULL,
	[updateDate] [datetime] NOT NULL,
	[createDate] [datetime] NOT NULL,
 CONSTRAINT [PK_merchCustomerIndex] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[merchCustomerIndex] ADD  CONSTRAINT [DF_merchCustomerIndex_updateDate]  DEFAULT (getdate()) FOR [updateDate]
GO

ALTER TABLE [dbo].[merchCustomerIndex] ADD  CONSTRAINT [DF_merchCustomerIndex_createDate]  DEFAULT (getdate()) FOR [createDate]
GO

ALTER TABLE [dbo].[merchCustomerIndex]  WITH CHECK ADD  CONSTRAINT [FK_merchCustomerIndex_merchCustomer] FOREIGN KEY([customerKey])
REFERENCES [dbo].[merchCustomer] ([pk])
GO

ALTER TABLE [dbo].[merchCustomerIndex] CHECK CONSTRAINT [FK_merchCustomerIndex_merchCustomer]
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_merchCustomerIndex]
    ON [dbo].[merchCustomerIndex]([customerKey] ASC);
GO


-------------------------------------
-- add merchCatalogInventory table --
-------------------------------------

ALTER TABLE [dbo].[merchCatalogInventory] DROP CONSTRAINT [FK_merchWarehouseInventory_merchProductVariant]
GO

ALTER TABLE [dbo].[merchCatalogInventory] DROP CONSTRAINT [FK_merchCatalogInventory_merchWarehouseCatalog]
GO

ALTER TABLE [dbo].[merchCatalogInventory] DROP CONSTRAINT [DF_merchCatalogInventory_createDate]
GO

ALTER TABLE [dbo].[merchCatalogInventory] DROP CONSTRAINT [DF_merchCatalogInventory_updateDate]
GO

/****** Object:  Table [dbo].[merchCatalogInventory]    Script Date: 7/30/2014 9:56:20 AM ******/
DROP TABLE [dbo].[merchCatalogInventory]
GO

/****** Object:  Table [dbo].[merchCatalogInventory]    Script Date: 7/30/2014 9:56:20 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[merchCatalogInventory](
	[catalogKey] [uniqueidentifier] NOT NULL,
	[productVariantKey] [uniqueidentifier] NOT NULL,
	[count] [int] NOT NULL,
	[lowCount] [int] NOT NULL,
	[location] [nvarchar](255) NULL,
	[updateDate] [datetime] NOT NULL,
	[createDate] [datetime] NOT NULL,
 CONSTRAINT [PK_merchCatalogInventory] PRIMARY KEY CLUSTERED 
(
	[catalogKey] ASC,
	[productVariantKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[merchCatalogInventory] ADD  CONSTRAINT [DF_merchCatalogInventory_updateDate]  DEFAULT (getdate()) FOR [updateDate]
GO

ALTER TABLE [dbo].[merchCatalogInventory] ADD  CONSTRAINT [DF_merchCatalogInventory_createDate]  DEFAULT (getdate()) FOR [createDate]
GO

ALTER TABLE [dbo].[merchCatalogInventory]  WITH CHECK ADD  CONSTRAINT [FK_merchCatalogInventory_merchWarehouseCatalog] FOREIGN KEY([catalogKey])
REFERENCES [dbo].[merchWarehouseCatalog] ([pk])
GO

ALTER TABLE [dbo].[merchCatalogInventory] CHECK CONSTRAINT [FK_merchCatalogInventory_merchWarehouseCatalog]
GO

ALTER TABLE [dbo].[merchCatalogInventory]  WITH CHECK ADD  CONSTRAINT [FK_merchWarehouseInventory_merchProductVariant] FOREIGN KEY([productVariantKey])
REFERENCES [dbo].[merchProductVariant] ([pk])
GO

ALTER TABLE [dbo].[merchCatalogInventory] CHECK CONSTRAINT [FK_merchWarehouseInventory_merchProductVariant]
GO

CREATE NONCLUSTERED INDEX [IX_merchCatalogInventoryLocation]
    ON [dbo].[merchCatalogInventory]([location] ASC);
GO

 
 