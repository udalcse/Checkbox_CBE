IF NOT EXISTS (SELECT 1 FROM ckbx_Settings WHERE SettingName = 'IsInvitationTextEnabled')
	INSERT ckbx_Settings (SettingName, SettingValue) VALUES (N'IsInvitationTextEnabled', N'false')