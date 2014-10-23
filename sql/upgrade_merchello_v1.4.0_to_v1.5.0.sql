-- TODO make shipmentStatusKey NULL and ShipmentNumber NULL - assign values and then reset

ALTER TABLE merchShipment
ADD 	shipmentStatusKey UNIQUEIDENTIFIER NOT NULL,
		shipmentNumber INT NOT NULL,
		shipmentNumberPrefix NVARCHAR(255) NULL

GO


CREATE TABLE [dbo].[merchShipmentStatus](
	[pk] [uniqueidentifier] NOT NULL,
	[name] [nvarchar](255) NOT NULL,
	[alias] [nvarchar](255) NOT NULL,
	[reportable] [bit] NOT NULL,
	[active] [bit] NOT NULL,
	[sortOrder] [int] NOT NULL,
	[updateDate] [datetime] NOT NULL,
	[createDate] [datetime] NOT NULL,
 CONSTRAINT [PK_merchShipmentStatus] PRIMARY KEY CLUSTERED 
(
	[pk] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[merchShipmentStatus] ADD  CONSTRAINT [DF_merchShipmentStatus_pk]  DEFAULT ('newid()') FOR [pk]
GO

ALTER TABLE [dbo].[merchShipmentStatus] ADD  CONSTRAINT [DF_merchShipmentStatus_updateDate]  DEFAULT (getdate()) FOR [updateDate]
GO

ALTER TABLE [dbo].[merchShipmentStatus] ADD  CONSTRAINT [DF_merchShipmentStatus_createDate]  DEFAULT (getdate()) FOR [createDate]
GO

ALTER TABLE [dbo].[merchShipment]  WITH CHECK ADD  CONSTRAINT [FK_merchShipment_merchShipmentStatus] FOREIGN KEY([shipmentStatusKey])
REFERENCES [dbo].[merchShipmentStatus] ([pk])
GO

-- INSERT THE new shipment statuses

INSERT INTO merchShipmentStatus
( pk, name, alias, reportable, active, sortOrder, updateDate, createDate)
VALUES
('6FA425A9-7802-4DA0-BD33-083C100E30F3', 'Quoted', 'quoted', 1, 1, 1, GETDATE(), GETDATE())


INSERT INTO merchShipmentStatus
( pk, name, alias, reportable, active, sortOrder, updateDate, createDate)
VALUES
('7342DCD6-8113-44B6-BFD0-4555B82F9503', 'Packaging', 'packaging', 1, 1, 1, GETDATE(), GETDATE())


INSERT INTO merchShipmentStatus
( pk, name, alias, reportable, active, sortOrder, updateDate, createDate)
VALUES
('CB24D43F-2774-4E56-85D8-653E49E3F542', 'Ready', 'ready', 1, 1, 1, GETDATE(), GETDATE())

INSERT INTO merchShipmentStatus
( pk, name, alias, reportable, active, sortOrder, updateDate, createDate)
VALUES
('B37BE101-CEC9-4608-9330-54E56FA0537A', 'Shipped', 'shipped', 1, 1, 1, GETDATE(), GETDATE())

INSERT INTO merchShipmentStatus
( pk, name, alias, reportable, active, sortOrder, updateDate, createDate)
VALUES
('3A279633-4919-485D-8C3B-479848A053D9', 'Delivered', 'delivered', 1, 1, 1, GETDATE(), GETDATE())


-- INSERT the new store setting
INSERT INTO merchStoreSetting
( pk, name, [value], typeName, updateDate, createDate)
VALUES
( '487F1C4E-DDBC-4DCD-9882-A9F7C78892B5', 'nextShipmentNumber', 1, 'System.Int32', GETDATE(), GETDATE() )