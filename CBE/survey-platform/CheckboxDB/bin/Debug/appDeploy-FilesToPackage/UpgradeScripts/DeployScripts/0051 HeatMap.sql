
IF NOT EXISTS (SELECT 1 FROM ckbx_ItemType WHERE ItemName = 'HeatMapSummary')
BEGIN
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
       'Checkbox.Analytics.Items.Configuration.HeatMapData' ,
       'ANALYSIS_HEAT_MAP_SUMMARY',
       'HeatMapSummary',
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

EXEC ckbx_sp_Text_Set '/itemType/HeatMapSummary/description' , 'en-US' , N'Display a graphical summary of answers.'



EXEC ckbx_sp_Text_Set '/itemType/HeatMapSummary/name', 'en-US', N'Heat Map Summary'


IF NOT EXISTS (SELECT 1 FROM ckbx_ItemAppearanceCode WHERE AppearanceCode = 'ANALYSIS_HEAT_MAP_SUMMARY')
BEGIN
	INSERT INTO ckbx_ItemAppearanceCode
	(
	   AppearanceCode , DataTypeName, DataTypeAssembly
	)
	VALUES
	(
		'ANALYSIS_HEAT_MAP_SUMMARY', 'Checkbox.Web.Charts.HeatMapAppearanceData' , 'Checkbox.Web.Charts'
	)
END


IF NOT EXISTS (SELECT 1 FROM ckbx_ItemEditors WHERE ItemTypeId = (SELECT ItemTypeId FROM ckbx_ItemType WHERE ItemName = 'HeatMapSummary' ))
BEGIN
	INSERT INTO ckbx_ItemEditors
	(ItemTypeId, EditorTypeName, EditorTypeAssembly )
	VALUES
	((SELECT ItemTypeId FROM ckbx_ItemType WHERE ItemName = 'HeatMapSummary' ),'Checkbox.Web.Analytics.UI.Editing.UserControlHostAnalysisItemEditor', 'Checkbox.Web')
END


IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_ItemData_HeatMap]') AND type in (N'U'))
DROP TABLE [ckbx_ItemData_HeatMap]
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_ItemData_HeatMap]') AND type in (N'U'))
BEGIN
CREATE TABLE [ckbx_ItemData_HeatMap](
       [ItemID] [int] NULL,
       [UseAliases] [bit] NULL
) ON [PRIMARY]
END
GO

-- Drop foreign key constraint
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_ckbx_ItemData_HeatMap_ckbx_Item]') AND parent_object_id = OBJECT_ID(N'[ckbx_ItemData_HeatMap]'))
ALTER TABLE [ckbx_ItemData_HeatMap] DROP CONSTRAINT [FK_ckbx_ItemData_HeatMap_ckbx_Item]
GO

-- Add foreign key constraint
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_ckbx_ItemData_HeatMap_ckbx_Item]') AND parent_object_id = OBJECT_ID(N'[ckbx_ItemData_HeatMap]'))
ALTER TABLE [ckbx_ItemData_HeatMap]  WITH CHECK ADD  CONSTRAINT [FK_ckbx_ItemData_HeatMap_ckbx_Item ] FOREIGN KEY([ItemID])
REFERENCES [dbo].[ckbx_Item] ([ItemID])
GO



IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_ItemData_GetHeatMapSummary]') AND type in (N'P', N'PC'))
DROP PROCEDURE [ckbx_sp_ItemData_GetHeatMapSummary ]
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_ItemData_GetHeatMapSummary ]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [ckbx_sp_ItemData_GetHeatMapSummary]
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
	heatMap.ItemID, 
	heatMap.UseAliases
  FROM 
     ckbx_ItemData_HeatMap heatMap 
  WHERE 
    ItemID = @ItemID

  --Source Items
  SELECT AnalysisItemID , SourceItemID FROM ckbx_ItemData_AI_SourceItems WHERE AnalysisItemID = @ItemID

  --Response Templates
  SELECT AnalysisItemID , ResponseTemplateID FROM ckbx_ItemDATA_AI_ResponseTemplates WHERE AnalysisItemID = @ItemID

  --Select from new table for GovernancePrioritySummary
  SELECT ItemID, UseAliases FROM ckbx_ItemData_HeatMap where ItemID = @ItemID

'
END
GO



IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_ItemData_InsertHeatMapSummary]') AND type in (N'P', N'PC'))
DROP PROCEDURE [ckbx_sp_ItemData_InsertHeatMapSummary]
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_ItemData_InsertHeatMapSummary]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [ckbx_sp_ItemData_InsertHeatMapSummary]
(
       @ItemID int,
       @UseAliases bit
)
AS
    INSERT INTO ckbx_ItemData_HeatMap
    (ItemId , UseAliases)
    VALUES
    (@ItemID , @UseAliases)
'
END
GO




IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_ItemData_UpdateHeatMapSummary]') AND type in (N'P', N'PC'))
DROP PROCEDURE [ckbx_sp_ItemData_UpdateHeatMapSummary]
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_ItemData_UpdateHeatMapSummary]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [ckbx_sp_ItemData_UpdateHeatMapSummary]
(
    @ItemID int,
    @UseAliases bit
)
AS
    UPDATE ckbx_ItemData_HeatMap
    SET UseAliases = @UseAliases
    WHERE ItemID = @ItemID
'
END
GO


IF NOT EXISTS (SELECT 1 FROM ckbx_ItemAppearanceEditor WHERE AppearanceCode = 'ANALYSIS_HEAT_MAP_SUMMARY')
BEGIN
	INSERT INTO ckbx_ItemAppearanceEditor (AppearanceCode, EditorTypeName, EditorTypeAssembly)
	VALUES ('ANALYSIS_HEAT_MAP_SUMMARY', 'Checkbox.Web.Analytics.UI.Editing.UserControlHostAnalysisItemAppearanceEditor' , 'Checkbox.Web')
END


IF NOT EXISTS (SELECT 1 FROM ckbx_ItemAppearanceRenderer WHERE AppearanceCode = 'ANALYSIS_HEAT_MAP_SUMMARY')
BEGIN
INSERT INTO ckbx_ItemAppearanceRenderer
(AppearanceCode, RendererTypeName, RendererAssemblyName )
VALUES
('ANALYSIS_HEAT_MAP_SUMMARY', 'Checkbox.Web.Analytics.UI.Rendering.UserControlHostAnalysisItemRenderer' , 'Checkbox.Web')

END
