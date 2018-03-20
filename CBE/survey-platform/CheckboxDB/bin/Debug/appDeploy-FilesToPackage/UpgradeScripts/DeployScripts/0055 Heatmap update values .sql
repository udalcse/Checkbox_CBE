
EXEC ckbx_sp_Text_Set '/controlText/chartEditor/randomizeResponses', 'en-US', N'Randomize Responses'

	IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_ItemData_HeatMap_eSigma]') AND type in (N'U'))
DROP TABLE [ckbx_ItemData_HeatMap_eSigma]
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_ItemData_HeatMap_eSigma]') AND type in (N'U'))
BEGIN
CREATE TABLE [ckbx_ItemData_HeatMap_eSigma](
	   [ReportId] [int] NULL,
       [SectionId] [int] NULL,
       [eSigmaValue] [decimal](18,2) NULL ,
) ON [PRIMARY]
END
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_ItemData_HeatMap]') AND type in (N'U'))
DROP TABLE [ckbx_ItemData_HeatMap]
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_ItemData_HeatMap]') AND type in (N'U'))
BEGIN
CREATE TABLE [ckbx_ItemData_HeatMap](
       [ItemID] [int] NULL,
       [UseAliases] [bit] NULL,
       [UseMeanValues] [bit] NULL,
	   [RandomizeResponses] [bit] NULL
	   CONSTRAINT [RandomizeResponses_Enabled] DEFAULT 1
) ON [PRIMARY]
END
GO



IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Get_Items_eSigma_Values]') AND type in (N'P', N'PC'))
DROP PROCEDURE [ckbx_sp_Get_Items_eSigma_Values]
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Get_Items_eSigma_Values]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [ckbx_sp_Get_Items_eSigma_Values]
(
	   @ItemID int,
	   @CheckForEnable bit
)
AS
IF (@CheckForEnable = 0 OR EXISTS(SELECT TOP 1 * FROM ckbx_ItemData_HeatMap 
			WHERE ItemID = @ItemID AND UseMeanValues = 0))
	BEGIN
		SELECT SectionId, eSigmaValue FROM ckbx_ItemData_HeatMap_eSigma
			WHERE ReportId = @ItemID
	END

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
       @UseAliases bit,
	   @UseMeanValues bit,
	   @RandomizeResponses bit
)
AS
    INSERT INTO ckbx_ItemData_HeatMap
    (ItemId , UseAliases, UseMeanValues, RandomizeResponses)
    VALUES
    (@ItemID , @UseAliases, @UseMeanValues, @RandomizeResponses)
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
    @UseAliases bit,
	@UseMeanValues bit,
	@RandomizeResponses bit
)
AS
    UPDATE ckbx_ItemData_HeatMap
    SET UseAliases = @UseAliases,
		UseMeanValues = @UseMeanValues,
		RandomizeResponses = @RandomizeResponses 
    WHERE ItemID = @ItemID
'
END
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
	heatMap.UseAliases,
	heatMap.UseMeanValues,
	heatMap.RandomizeResponses
  FROM 
     ckbx_ItemData_HeatMap heatMap 
  WHERE 
    ItemID = @ItemID

  --Source Items
  SELECT AnalysisItemID , SourceItemID FROM ckbx_ItemData_AI_SourceItems WHERE AnalysisItemID = @ItemID

  --Response Templates
  SELECT AnalysisItemID , ResponseTemplateID FROM ckbx_ItemDATA_AI_ResponseTemplates WHERE AnalysisItemID = @ItemID
'
END
GO