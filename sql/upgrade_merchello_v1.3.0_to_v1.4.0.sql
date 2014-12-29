ALTER TABLE merchProductVariant
ADD versionKey UNIQUEIDENTIFIER NOT NULL
CONSTRAINT [DF_merchProductVariant_versionKey] DEFAULT (newid())

GO

ALTER TABLE merchInvoice
ADD poNumber NVARCHAR(255) NULL

GO


CREATE TABLE [dbo].[merchAuditLog](
	[pk] [uniqueidentifier] NOT NULL,
	[entityKey] [uniqueidentifier] NULL,
	[entityTfKey] [uniqueidentifier] NULL,
	[message] [ntext] NULL,
	[verbosity] [int] NOT NULL,
	[extendedData] [ntext] NULL,
	[isError] [bit] NOT NULL,
	[updateDate] [datetime] NOT NULL,
	[createDate] [datetime] NOT NULL,
 CONSTRAINT [PK_merchAuditLog] PRIMARY KEY CLUSTERED 
(
	[pk] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

ALTER TABLE [dbo].[merchAuditLog] ADD  CONSTRAINT [DF_merchAuditLog_pk]  DEFAULT (newid()) FOR [pk]
GO

ALTER TABLE [dbo].[merchAuditLog] ADD  CONSTRAINT [DF_merchAuditLog_verbosity]  DEFAULT ('0') FOR [verbosity]
GO

ALTER TABLE [dbo].[merchAuditLog] ADD  CONSTRAINT [DF_merchAuditLog_updateDate]  DEFAULT (getdate()) FOR [updateDate]
GO

ALTER TABLE [dbo].[merchAuditLog] ADD  CONSTRAINT [DF_merchAuditLog_createDate]  DEFAULT (getdate()) FOR [createDate]
