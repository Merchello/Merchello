CREATE TABLE [merchPriceGroup](
	[pk] [uniqueidentifier] NOT NULL,
	[cultureName] nvarchar(255) NULL,
	[Name] nvarchar(255) NOT NULL,
	[Alias] nvarchar(255) NOT NULL,
	RestrictToMemberGroup bit not null,
	updateDate datetime null,
	createDate datetime NOT NULL,
	CONSTRAINT [PK_merchPriceGroup] PRIMARY KEY CLUSTERED 
	(
		[pk] ASC
	)
)

CREATE TABLE [dbo].[merchPriceGroup2MemberGroup](
	[priceGroupKey] [uniqueidentifier] NOT NULL,
	[MemberGroup] int NOT NULL,
	
	createDate DateTime NOT NULL
 CONSTRAINT [PK_merchPriceGroup2MemberGroup] PRIMARY KEY CLUSTERED 
(
	[priceGroupKey], [MemberGroup]
),
	CONSTRAINT [FK_merchPriceGroup2MemberGroup_merchPriceGroup] FOREIGN KEY([priceGroupKey])
	REFERENCES merchPriceGroup ([pk])  ON DELETE CASCADE ON UPDATE CASCADE
	,
	CONSTRAINT [FK_merchPriceGroup2MemberGroup_umbracoNode] FOREIGN KEY([MemberGroup])
	REFERENCES [umbracoNode] ([id]) ON DELETE CASCADE ON UPDATE CASCADE
)




CREATE TABLE [merchProductVariantPrice](
	[pk] [uniqueidentifier] NOT NULL,
	[productVariantKey] [uniqueidentifier] NOT NULL,
	[priceGroupKey] [uniqueidentifier] not null,
	[price] [decimal](20, 9) NOT NULL,
	[salePrice] [decimal](38, 6) NULL,
	[onSale] [bit] NOT NULL,
	[taxable] [bit] NOT NULL,
	[dtStart] datetime null,
	[dtEnd] datetime null,
	updateDate datetime null,
	createDate datetime NOT NULL,
	CONSTRAINT [PK_merchProductVariantPriceGroup] PRIMARY KEY CLUSTERED 
	(
		[pk] ASC
	),

	CONSTRAINT [FK_merchProductVariantPriceGroup_merchProductVariant] FOREIGN KEY([productVariantKey])
	REFERENCES [merchProductVariant] ([pk]) on update cascade on delete cascade

	,

	CONSTRAINT [FK_merchProductVariantPriceGroup_merchPriceGroup] FOREIGN KEY([priceGroupKey])
	REFERENCES merchPriceGroup ([pk])
)

