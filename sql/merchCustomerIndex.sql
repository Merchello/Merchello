/****** Object:  Table [dbo].[merchCustomerIndex]    Script Date: 7/15/2014 1:47:31 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

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


