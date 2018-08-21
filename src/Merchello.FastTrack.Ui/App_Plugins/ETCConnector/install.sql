CREATE TABLE [spdrcatalogobject](
	[ObjectID] [int] NOT NULL,
	[SpiderSiteID] [smallint] NOT NULL,
	[SpiderObjectID] [nvarchar](255) NOT NULL,
	[DTCreate] [datetime] NULL,
	[DTChange] [datetime] NULL,
	[DTNotFound] [datetime] NULL,
 CONSTRAINT [PK_spdrcatalogobject] PRIMARY KEY  
(
	[ObjectID] 
))


CREATE TABLE [spdrsite](
	[SpiderSiteID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [ntext] NULL,
	[URL] [nvarchar](255) NOT NULL,
	[Disabled] [tinyint] NOT NULL,
	[DTEnable] [datetime] NULL,
	[DTDisable] [datetime] NULL,
	[DTCreate] [datetime] NOT NULL,
	[UIDCreate] [smallint] NOT NULL,
	[DTChange] [datetime] NOT NULL,
	[UIDChange] [smallint] NOT NULL,
	[ListSelector] [nvarchar](255) NULL,
	[ListExcludeSelector] [nvarchar](255) NULL,
	[ObjTypeID] [smallint] NULL,
	[DTLastSpidered] [datetime] NULL,
	[IDRuleID] [smallint] NOT NULL,
	[DetailUrlRuleID] [smallint] NULL,
	[CategoryRuleID] [smallint] NULL,
	[PagerRuleID] [int] NULL,
	[CountRuleID] [smallint] NULL,
	[RootContentNodeId] [int] NULL,
    [SpiderDatabase] [tinyint] NULL
 CONSTRAINT [spdrsite_PK] PRIMARY KEY 
(
	[SpiderSiteID] ))
GO


CREATE TABLE [spdrsiterule](
	[RuleID] [int] IDENTITY(1,1) NOT NULL,
	[SpiderSiteID] [int] NOT NULL,
	[OrderID] [smallint] NULL,
	[RuleName] [nvarchar](255) NULL,
	[Selector] [nvarchar](255) NULL,
	[SelectorAttribute] [nvarchar](50) NULL,
	[Regex] [nvarchar](255) NULL,
	[Disabled] [tinyint] NOT NULL,
	[DTEnable] [datetime] NULL,
	[DTDisable] [datetime] NULL,
	[DTCreate] [datetime] NOT NULL,
	[UIDCreate] [smallint] NOT NULL,
	[DTChange] [datetime] NOT NULL,
	[UIDChange] [smallint] NOT NULL,
	[IsRuleForListView] [tinyint] NOT NULL,
	[FixedValue] [nvarchar](255) NULL,
	[SpecID] [smallint] NULL,
	[SelectHtml] [tinyint] NULL,
	[Seperator] [nvarchar](50) NULL,
	[Mandatory] [tinyint] NULL,
	[Exclude] [tinyint] NULL,
 CONSTRAINT [PK_spdrsiterule] PRIMARY KEY  
(
	[RuleID] 
))
ALTER TABLE [spdrsiterule]  ADD  CONSTRAINT [spdrsiterule_spdrsite] FOREIGN KEY([SpiderSiteID])
REFERENCES [spdrsite] ([SpiderSiteID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO


CREATE TABLE [spdrsiteruleexcludeitem](
	[SpiderSiteID] [int] NOT NULL,
	[SpecID] [int] NOT NULL,
	[SpecItemName] [nvarchar](255) NOT NULL,
	[MapTo] [nvarchar](255) NULL,
 CONSTRAINT [spdrsiteruleexcludeitem_PK] PRIMARY KEY  
(
	[SpiderSiteID] ,
	[SpecID] ,
	[SpecItemName] 
)
) 

ALTER TABLE [spdrsiteruleexcludeitem] ADD  CONSTRAINT [spdrsiteruleexcludeitem_spdrsite] FOREIGN KEY([SpiderSiteID])
REFERENCES [spdrsite] ([SpiderSiteID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO




CREATE TABLE [spdrsiterulespecitem](
	[SpiderSiteID] [int] NOT NULL,
	[SpecID] [int] NOT NULL,
	[SpecItemName] [nvarchar](255) NOT NULL,
	[MapTo] [nvarchar](255) NULL,
 CONSTRAINT [spdrsiterulespecitem_PK] PRIMARY KEY  
(
	[SpiderSiteID] ,
	[SpecID] ,
	[SpecItemName] 
)
) 

GO

ALTER TABLE [spdrsiterulespecitem] ADD  CONSTRAINT [spdrsiterulespecitem_spdrsite] FOREIGN KEY([SpiderSiteID])
REFERENCES [spdrsite] ([SpiderSiteID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO


ALTER TABLE [spdrsite] ADD [DocumentTypeName] nvarchar(255)  NULL ;
GO


ALTER TABLE [spdrsite] ADD [DatePart] nvarchar(4) NULL ;
UPDATE [spdrsite] SET DatePart='DD';
ALTER TABLE [spdrsite] ALTER COLUMN [DatePart] nvarchar(4) NOT NULL ;

ALTER TABLE [spdrsite] ADD [Interval] INT NULL ;
UPDATE [spdrsite] SET Interval=1;
ALTER TABLE [spdrsite] ALTER COLUMN [Interval] nvarchar(4) NOT NULL ;

ALTER TABLE [spdrsite] ADD [ExecDT] DATETIME NULL ;

ALTER TABLE [spdrsite] ADD [StartDT] datetime NULL ;