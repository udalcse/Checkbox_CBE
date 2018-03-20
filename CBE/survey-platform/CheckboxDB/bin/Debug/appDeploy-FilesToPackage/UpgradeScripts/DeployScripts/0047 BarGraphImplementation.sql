IF NOT EXISTS (SELECT 1 FROM ckbx_ItemType WHERE ItemName = 'GovernancePrioritySummary')
BEGIN
   -- Insert a new item type
INSERT INTO ckbx_ItemType
(
       ItemDataAssemblyName ,
       ItemDataClassName ,
       DefaultAppearanceCode ,
       ItemName ,
       MobileCompatible ,
       CategoryId ,
       RTCompatible ,
       LibraryCompatible ,
       ReportCompatible ,
       Position ,
       TextIdPrefix ,
       IsAnswerable
)
VALUES
(
       'Checkbox',
       'Checkbox.Analytics.Items.Configuration.GovernancePriorityGraphData' ,
       'ANALYSIS_GOVERNANCE_PRIORITY_SUMMARY',
       'GovernancePrioritySummary',
       0 ,
       1003 ,
       0 ,
       0 ,
       1 ,
       0 ,
        null,
       0
)

END


IF NOT EXISTS (SELECT 1 FROM ckbx_ItemAppearanceCode WHERE AppearanceCode = 'ANALYSIS_GOVERNANCE_PRIORITY_SUMMARY')
BEGIN
	-- Insert new item to ItemAppearanceCode
	INSERT INTO ckbx_ItemAppearanceCode
	(
	   AppearanceCode , DataTypeName, DataTypeAssembly
	)
	VALUES
	(
	   'ANALYSIS_GOVERNANCE_PRIORITY_SUMMARY', 'Checkbox.Analytics.Items.UI.GovernancePrioritySummaryAppearanceData' , 'Checkbox'
	)
END

IF NOT EXISTS (SELECT 1 FROM ckbx_ItemEditors WHERE ItemTypeId = (SELECT ItemTypeId FROM ckbx_ItemType WHERE ItemName = 'GovernancePrioritySummary' ))
BEGIN
-- Insert a new item to ItemEditors
INSERT INTO ckbx_ItemEditors
(ItemTypeId, EditorTypeName, EditorTypeAssembly )
VALUES
((SELECT ItemTypeId FROM ckbx_ItemType WHERE ItemName = 'GovernancePrioritySummary' ),'Checkbox.Web.Analytics.UI.Editing.UserControlHostAnalysisItemEditor', 'Checkbox.Web')
END

IF NOT EXISTS (SELECT 1 FROM ckbx_ItemAppearanceEditor WHERE AppearanceCode = 'ANALYSIS_GOVERNANCE_PRIORITY_SUMMARY')
BEGIN
-- Insert a new item to ckbx_ItemAppearanceEditor
INSERT INTO ckbx_ItemAppearanceEditor (AppearanceCode, EditorTypeName, EditorTypeAssembly)
VALUES ('ANALYSIS_GOVERNANCE_PRIORITY_SUMMARY', 'Checkbox.Web.Analytics.UI.Editing.UserControlHostAnalysisItemAppearanceEditor' , 'Checkbox.Web')
END

IF NOT EXISTS (SELECT 1 FROM ckbx_ItemAppearanceRenderer WHERE AppearanceCode = 'ANALYSIS_GOVERNANCE_PRIORITY_SUMMARY')
BEGIN
-- Add record to ckbx_ItemAppearanceRenderer
INSERT INTO ckbx_ItemAppearanceRenderer
(AppearanceCode, RendererTypeName, RendererAssemblyName )
VALUES
('ANALYSIS_GOVERNANCE_PRIORITY_SUMMARY', 'Checkbox.Web.Analytics.UI.Rendering.UserControlHostAnalysisItemRenderer' , 'Checkbox.Web')
END

--DROP TABLE ckbx_ItemData_GovernancePriority
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_ItemData_GovernancePriority]') AND type in (N'U'))
DROP TABLE ckbx_ItemData_GovernancePriority
GO

-- Create table ckbx_ItemData_GovernancePriority
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_ItemData_GovernancePriority]') AND type in (N'U'))
BEGIN
CREATE TABLE ckbx_ItemData_GovernancePriority(
	[ItemID] [int] NULL,
	[UseAliases] [bit] NULL
) ON [PRIMARY]
END
GO


-- Drop foreign key constraint
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_ckbx_ItemData_GovernancePriority_ckbx_Item]') AND parent_object_id = OBJECT_ID(N'[ckbx_ItemData_GovernancePriority]'))
ALTER TABLE [ckbx_ItemData_GovernancePriority] DROP CONSTRAINT [FK_ckbx_ItemData_GovernancePriority_ckbx_Item ]
GO


-- Add foreign key constraint
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_ckbx_ItemData_GovernancePriority_ckbx_Item ]') AND parent_object_id = OBJECT_ID(N'[ckbx_ItemData_GovernancePriority]'))
ALTER TABLE [ckbx_ItemData_GovernancePriority]  WITH CHECK ADD  CONSTRAINT [FK_ckbx_ItemData_GovernancePriority_ckbx_Item ] FOREIGN KEY([ItemID])
REFERENCES [dbo].[ckbx_Item] ([ItemID])
GO


-- drop sp ckbx_sp_ItemData_InsertGovernancePrioritySummary
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_ItemData_InsertGovernancePrioritySummary]') AND type in (N'P', N'PC'))
DROP PROCEDURE ckbx_sp_ItemData_InsertGovernancePrioritySummary
GO


-- drop sp ckbx_sp_ItemData_UpdateGovernancePrioritySummary
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_ItemData_UpdateGovernancePrioritySummary]') AND type in (N'P', N'PC'))
DROP PROCEDURE ckbx_sp_ItemData_UpdateGovernancePrioritySummary
GO


-- create sp ckbx_sp_ItemData_InsertGovernancePrioritySummary
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_ItemData_InsertGovernancePrioritySummary]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [ckbx_sp_ItemData_InsertGovernancePrioritySummary]

	@ItemID int,
	@UseAliases bit

As

	SET NOCOUNT ON
	INSERT INTO ckbx_ItemData_GovernancePriority
	(ItemId , UseAliases)
	VALUES
	(@ItemId , @UseAliases)

'
END
GO


-- create sp ckbx_sp_ItemData_UpdateGovernancePrioritySummary
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_ItemData_UpdateGovernancePrioritySummary]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [ckbx_sp_ItemData_UpdateGovernancePrioritySummary]

	@ItemID int,
	@UseAliases bit

As

	SET NOCOUNT ON
	UPDATE ckbx_ItemData_GovernancePriority
	SET UseAliases = @UseAliases
	WHERE ItemID = @ItemID

'
END
GO

-- Description for report item
EXEC ckbx_sp_Text_Set '/itemType/GovernancePrioritySummary/description' , 'en-US' , N'Display a graphical summary of answers.'


-- Localized name for report item
EXEC ckbx_sp_Text_Set '/itemType/GovernancePrioritySummary/name', 'en-US', N'Governance Priority Summary'


IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Get_Item_Sections]') AND type in (N'P', N'PC'))
DROP PROCEDURE [ckbx_sp_Get_Item_Sections]
GO

Create PROCEDURE [ckbx_sp_Get_Item_Sections]
@TemplateID int
AS
BEGIN

 SELECT item.ItemID, txt.TextValue FROM ckbx_Item item 
  JOIN ckbx_ItemData_Message item_message ON item.ItemID = item_message.ItemID
  JOIN ckbx_Template_Items tmp_items ON item.ItemID = tmp_items.ItemID
  JOIN ckbx_Text txt ON item_message.TextID= txt.TextID 
   WHERE tmp_items.TemplateID = @TemplateID  and item_message.ReportableSectionBreak = 1
   
END

GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_ItemData_GetGovernancePrioritySummary]') AND type in (N'P', N'PC'))
DROP PROCEDURE [ckbx_sp_ItemData_GetGovernancePrioritySummary]
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_ItemData_GetGovernancePrioritySummary]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [ckbx_sp_ItemData_GetGovernancePrioritySummary]
@ItemID int
AS
SELECT
    ItemId ,
    ItemTypeId ,
    Alias ,
    CreatedDate ,
    ModifiedDate ,
    IsActive
  FROM
    ckbx_Item
  WHERE
    ItemId = @ItemId

  --Source Items
  SELECT AnalysisItemID , SourceItemID FROM ckbx_ItemData_AI_SourceItems WHERE AnalysisItemID = @ItemID

  --Response Templates
  SELECT AnalysisItemID , ResponseTemplateID FROM ckbx_ItemDATA_AI_ResponseTemplates WHERE AnalysisItemID = @ItemID

  --Select from new table for GovernancePrioritySummary
  SELECT ItemID, UseAliases FROM ckbx_ItemData_GovernancePriority where ItemID = @ItemID

'
END
GO





