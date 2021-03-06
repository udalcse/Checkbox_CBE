IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Sections_GetSectionIds]') AND type in (N'P', N'PC'))
DROP PROCEDURE [ckbx_sp_Sections_GetSectionIds]
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Sections_GetSectionIds]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [ckbx_sp_Sections_GetSectionIds] 
(
	@SectionId int
)
AS
	SELECT ItemId FROM ckbx_ItemData_Sections WHERE SectionId = @SectionId

'
END
GO



IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Sections_DeleteSection]') AND type in (N'P', N'PC'))
DROP PROCEDURE [ckbx_sp_Sections_DeleteSection]
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Sections_DeleteSection]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [ckbx_sp_Sections_DeleteSection] 
(
	@SectionId int
)
AS
	DELETE FROM ckbx_ItemData_Sections WHERE SectionId = @SectionId

'
END
GO



IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Sections_AddSectionItemId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [ckbx_sp_Sections_AddSectionItemId]
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Sections_AddSectionItemId]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [ckbx_sp_Sections_AddSectionItemId]
(
	@ItemId int,
	@SectionId int,
	@SurveyId int
)
AS
	DELETE FROM ckbx_ItemData_Sections WHERE ItemId = @ItemId
	INSERT INTO ckbx_ItemData_Sections(ItemId,SectionId,SurveyId) VALUES (@ItemId, @SectionId,@SurveyId)


'
END
GO


IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Sections_GetAllSurveySectionItems]') AND type in (N'P', N'PC'))
DROP PROCEDURE [ckbx_sp_Sections_GetAllSurveySectionItems]
GO


IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Sections_GetAllSurveySectionItems]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [ckbx_sp_Sections_GetAllSurveySectionItems] 
(
	@SurveyId int
)
AS
	SELECT ItemId FROM ckbx_ItemData_Sections WHERE SurveyId = @SurveyId


'
END
GO


