
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_ckbx_ItemData_Sections_ckbx_Item]') AND parent_object_id = OBJECT_ID(N'[ckbx_ItemData_Sections]'))
ALTER TABLE [ckbx_ItemData_Sections] DROP CONSTRAINT [FK_ckbx_ItemData_Sections_ckbx_Item]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_ItemData_Sections]') AND type in (N'U'))
DROP TABLE [ckbx_ItemData_Sections]
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_ItemData_Sections]') AND type in (N'U'))
BEGIN
CREATE TABLE [ckbx_ItemData_Sections](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ItemId] [int] NOT NULL,
	[SectionId] [int] NOT NULL,
	[SurveyId] [int] NOT NULL,
 CONSTRAINT [PK_ckbx_ItemData_Sections] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_ckbx_ItemData_Sections_ckbx_Item]') AND parent_object_id = OBJECT_ID(N'[ckbx_ItemData_Sections]'))
ALTER TABLE [ckbx_ItemData_Sections]  WITH CHECK ADD  CONSTRAINT [FK_ckbx_ItemData_Sections_ckbx_Item] FOREIGN KEY([SectionId])
REFERENCES [dbo].[ckbx_Item] ([ItemID])
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_ckbx_ItemData_Sections_ckbx_Item]') AND parent_object_id = OBJECT_ID(N'[ckbx_ItemData_Sections]'))
ALTER TABLE [ckbx_ItemData_Sections] CHECK CONSTRAINT [FK_ckbx_ItemData_Sections_ckbx_Item]
GO

