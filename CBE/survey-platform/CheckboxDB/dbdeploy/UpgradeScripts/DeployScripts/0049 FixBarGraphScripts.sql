
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

SELECT 
	governancePriority.ItemID, 
	governancePriority.UseAliases
  FROM 
     ckbx_ItemData_GovernancePriority governancePriority 
  WHERE 
    ItemID = @ItemID

  --Source Items
  SELECT AnalysisItemID , SourceItemID FROM ckbx_ItemData_AI_SourceItems WHERE AnalysisItemID = @ItemID

  --Response Templates
  SELECT AnalysisItemID , ResponseTemplateID FROM ckbx_ItemDATA_AI_ResponseTemplates WHERE AnalysisItemID = @ItemID

  --Select from new table for GovernancePrioritySummary
  SELECT ItemID, UseAliases FROM ckbx_ItemData_GovernancePriority where ItemID = @ItemID

'
END
GO


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
