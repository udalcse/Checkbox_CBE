EXEC ckbx_sp_Text_Set '/controlText/chartEditor/useMeanValues', 'en-US', N'Use Mean'
EXEC ckbx_sp_Text_Set '/controlText/chartEditor/heatmapSection', 'en-US', N'Section'
EXEC ckbx_sp_Text_Set '/controlText/chartEditor/eSigma', 'en-US', N'eSigma'

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
       @eSigmaValue decimal(18,2)
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