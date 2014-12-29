ALTER TABLE merchShipment
ADD COLUMN 	shipmentStatusKey UNIQUEIDENTIFIER NULL

GO

ALTER TABLE merchShipment
ADD COLUMN 	shipmentNumber INT NULL

GO

ALTER TABLE merchShipment
ADD COLUMN 	shipmentNumberPrefix NVARCHAR(255) NULL

GO


CREATE TABLE merchShipmentStatus(
	[pk] [uniqueidentifier] DEFAULT ('newid()') NOT NULL,
	[name] [nvarchar](255) NOT NULL,
	[alias] [nvarchar](255) NOT NULL,
	[reportable] [bit] NOT NULL,
	[active] [bit] NOT NULL,
	[sortOrder] [int] NOT NULL,
	[updateDate] [datetime] DEFAULT (getdate()) NOT NULL,
	[createDate] [datetime] DEFAULT (getdate()) NOT NULL)

GO

-- Alter new table merchAuditLog to have primary key
ALTER TABLE merchShipmentStatus ADD CONSTRAINT PK_merchShipmentStatus PRIMARY KEY ([pk]);
GO

ALTER TABLE merchShipment ADD CONSTRAINT FK_merchShipment_merchShipmentStatus FOREIGN KEY([shipmentStatusKey])
REFERENCES merchShipmentStatus ([pk])

GO

-- INSERT THE new shipment statuses

INSERT INTO merchShipmentStatus
( pk, name, alias, reportable, active, sortOrder, updateDate, createDate)
VALUES
('6FA425A9-7802-4DA0-BD33-083C100E30F3', 'Quoted', 'quoted', 1, 1, 1, GETDATE(), GETDATE())

GO

INSERT INTO merchShipmentStatus
( pk, name, alias, reportable, active, sortOrder, updateDate, createDate)
VALUES
('7342DCD6-8113-44B6-BFD0-4555B82F9503', 'Packaging', 'packaging', 1, 2, 1, GETDATE(), GETDATE())

GO

INSERT INTO merchShipmentStatus
( pk, name, alias, reportable, active, sortOrder, updateDate, createDate)
VALUES
('CB24D43F-2774-4E56-85D8-653E49E3F542', 'Ready', 'ready', 1, 3, 1, GETDATE(), GETDATE())

GO

INSERT INTO merchShipmentStatus
( pk, name, alias, reportable, active, sortOrder, updateDate, createDate)
VALUES
('B37BE101-CEC9-4608-9330-54E56FA0537A', 'Shipped', 'shipped', 1, 4, 1, GETDATE(), GETDATE())

GO

INSERT INTO merchShipmentStatus
( pk, name, alias, reportable, active, sortOrder, updateDate, createDate)
VALUES
('3A279633-4919-485D-8C3B-479848A053D9', 'Delivered', 'delivered', 1, 5, 1, GETDATE(), GETDATE())

GO

-- INSERT the new store setting
INSERT INTO merchStoreSetting
( pk, name, [value], typeName, updateDate, createDate)
VALUES
( '487F1C4E-DDBC-4DCD-9882-A9F7C78892B5', 'nextShipmentNumber', 1, 'System.Int32', GETDATE(), GETDATE() )

GO

-- UPDATE EXISTING SHIPMENT RECORDS
-- DOESN'T SET ShipmentNumber -- this must be set in C# code or by hand as SQL CE doesn't support update join from join
-- http://stackoverflow.com/questions/4895907/updating-rows-based-on-multiple-tables-in-sql-server-compact-edition

UPDATE	merchShipment
SET	shipmentStatusKey = 'B37BE101-CEC9-4608-9330-54E56FA0537A'
     ,	updateDate = GETDATE()

GO