INSERT [dbo].[ckbx_IdentityRoles] ([UniqueIdentifier], [RoleID]) VALUES (N'@@admin_username@@', 1)
INSERT [dbo].[ckbx_Credential] ([UserName], [Password], [Domain], [UniqueIdentifier], [GUID], [Encrypted], [Email], [Created], [LastActivity], [LastLogin], [LastPasswordChange], [LastLockedOut], [FailedLogins], [LockedOut], [CreatedBy], [ModifiedDate], [ModifiedBy]) VALUES (N'@@admin_username@@', N'@@admin_password@@', NULL, N'@@admin_username@@', N'@@admin_user_guid@@', 0, NULL, NULL, NULL, NULL, NULL, NULL, 0, 0, NULL, NULL, NULL)
INSERT [dbo].[ckbx_Settings] ([SettingName], [SettingValue]) VALUES (N'DefaultEmailFromName', N'Survey Emailer')
INSERT [dbo].[ckbx_Settings] ([SettingName], [SettingValue]) VALUES (N'EmailEnabled', N'false')
INSERT [dbo].[ckbx_Settings] ([SettingName], [SettingValue]) VALUES (N'InsertLineBreaksInEmails', N'false')
INSERT [dbo].[ckbx_Settings] ([SettingName], [SettingValue]) VALUES (N'SmtpServer', N'None Configured')
INSERT [dbo].[ckbx_Settings] ([SettingName], [SettingValue]) VALUES (N'SystemEmailAddress', N'@@system_email_address@@')
INSERT [dbo].[ckbx_Settings] ([SettingName], [SettingValue]) VALUES (N'DefaultFromEmailAddress', N'surveyadmin@yourdomain.com')
INSERT [dbo].[ckbx_Settings] ([SettingName], [SettingValue]) VALUES (N'TimeZone', N'-7')
/*
INSERT [dbo].[ckbx_Settings] ([SettingName], [SettingValue]) VALUES (N'SmtpPassword', N'smtppass')
INSERT [dbo].[ckbx_Settings] ([SettingName], [SettingValue]) VALUES (N'SmtpUserName', N'smptuser')
*/
