IF NOT EXISTS (SELECT 1 FROM ckbx_Text WHERE TextID = '/pageText/settings/emailText.aspx/title')
	INSERT ckbx_Text (TextID, LanguageCode, TextValue, PageId) VALUES (N'/pageText/settings/emailText.aspx/title', N'en-US', N'Email Text Settings', NULL)


