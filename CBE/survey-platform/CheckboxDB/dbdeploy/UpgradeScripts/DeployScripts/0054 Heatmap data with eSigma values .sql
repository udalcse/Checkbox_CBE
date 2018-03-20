EXEC ckbx_sp_Text_Set '/controlText/chartEditor/useMeanValues', 'en-US', N'Use Mean Values'
EXEC ckbx_sp_Text_Set '/controlText/chartEditor/data', 'en-US', N'Data'


IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_ItemData_HeatMap]') AND type in (N'U'))
DROP TABLE [ckbx_ItemData_HeatMap]
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_ItemData_HeatMap]') AND type in (N'U'))
BEGIN
CREATE TABLE [ckbx_ItemData_HeatMap](
       [ItemID] [int] NULL,
       [UseAliases] [bit] NULL,
       [UseMeanValues] [bit] NULL
) ON [PRIMARY]
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
	heatMap.UseMeanValues
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
	   @UseMeanValues bit
)
AS
    INSERT INTO ckbx_ItemData_HeatMap
    (ItemId , UseAliases, UseMeanValues)
    VALUES
    (@ItemID , @UseAliases, @UseMeanValues)
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
	@UseMeanValues bit
)
AS
    UPDATE ckbx_ItemData_HeatMap
    SET UseAliases = @UseAliases,
		UseMeanValues = @UseMeanValues
    WHERE ItemID = @ItemID
'
END
GO



IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_ItemData_HeatMap_eSigma]') AND type in (N'U'))
DROP TABLE [ckbx_ItemData_HeatMap_eSigma]
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_ItemData_HeatMap_eSigma]') AND type in (N'U'))
BEGIN
CREATE TABLE [ckbx_ItemData_HeatMap_eSigma](
	   [ReportId] [int] NULL,
       [SectionId] [int] NULL,
       [eSigmaValue] [decimal] NULL ,
) ON [PRIMARY]
END
GO


IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_ItemData_InsertHeatMap_eSigma]') AND type in (N'P', N'PC'))
DROP PROCEDURE [ckbx_sp_ItemData_InsertHeatMap_eSigma]
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_ItemData_InsertHeatMap_eSigma]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [ckbx_sp_ItemData_InsertHeatMap_eSigma]
(
	   @ReportId int,
       @SectionId int,
       @eSigmaValue decimal
)
AS
	IF EXISTS(SELECT TOP 1 * FROM ckbx_ItemData_HeatMap_eSigma 
		WHERE SectionId = @SectionId and @ReportId = ReportId) 
		BEGIN
			UPDATE ckbx_ItemData_HeatMap_eSigma 
				SET eSigmaValue = @eSigmaValue
				WHERE SectionId = @SectionId and @ReportId = ReportId
		END
	ELSE 
		BEGIN
		    INSERT INTO ckbx_ItemData_HeatMap_eSigma
				(ReportId , SectionId, eSigmaValue)
			VALUES
				(@ReportId, @SectionId, @eSigmaValue)
		END

'
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
	   @ItemID int
)
AS
IF EXISTS(SELECT TOP 1 * FROM ckbx_ItemData_HeatMap 
			WHERE ItemID = @ItemID AND UseMeanValues = 1)
	BEGIN
		SELECT SectionId, eSigmaValue FROM ckbx_ItemData_HeatMap_eSigma
	END

'
END
GO