UPDATE [dbo].[ckbx_Text] 
	SET [TextValue] = N'Follow the link in your original invitation to resume the questionnaire where you left off.'
WHERE [TextID] = N'/controlText/responseView/toResume'

GO

UPDATE [dbo].[ckbx_Text] 
	SET [TextValue] = N'Follow the link in your original invitation to resume the questionnaire where you left off.'
WHERE [TextID] = N'/controlText/responseView/mobileResume'

GO