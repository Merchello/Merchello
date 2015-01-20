-- Upgrade Merchello tables from 1.3.0 to 1.4.0 in Sql Ce 4.0

-- Add versionKey column to existing table merchProductVariant
ALTER TABLE merchProductVariant
ADD COLUMN versionKey UNIQUEIDENTIFIER NOT NULL
CONSTRAINT [DF_merchProductVariant_versionKey] DEFAULT ('newid()')

Go

-- Add poNumber column to existing table merchInvoice
ALTER TABLE merchInvoice
ADD COLUMN poNumber NVARCHAR(255) NULL

GO

-- Create new table merchAuditLog 
CREATE TABLE merchAuditLog(
	[pk] [uniqueidentifier] DEFAULT ('newid()') NOT NULL,
	[entityKey] [uniqueidentifier] NULL,
	[entityTfKey] [uniqueidentifier] NULL,
	[message] [ntext] NULL,
	[verbosity] [int] DEFAULT ('0') NOT NULL,
	[extendedData] [ntext] NULL,
	[isError] [bit] NOT NULL,
	[updateDate] [datetime]  DEFAULT (getdate()) NOT NULL,
	[createDate] [datetime] DEFAULT (getdate()) NOT NULL)
GO

-- Alter new table merchAuditLog to have primary key
ALTER TABLE [merchAuditLog] ADD CONSTRAINT [PK_merchAuditLog] PRIMARY KEY ([pk]);
GO
