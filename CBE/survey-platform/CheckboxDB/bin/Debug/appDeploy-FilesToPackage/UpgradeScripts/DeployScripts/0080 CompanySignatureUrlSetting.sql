IF NOT EXISTS (SELECT 1 FROM ckbx_Settings WHERE SettingName = 'CompanySignatureImageUrl')
	INSERT ckbx_Settings (SettingName, SettingValue) VALUES (N'CompanySignatureImageUrl', N'')


IF NOT EXISTS (SELECT 1 FROM ckbx_Settings WHERE SettingName = 'CompanySignatureEnabled')
	INSERT ckbx_Settings (SettingName, SettingValue) VALUES (N'CompanySignatureEnabled', N'false')