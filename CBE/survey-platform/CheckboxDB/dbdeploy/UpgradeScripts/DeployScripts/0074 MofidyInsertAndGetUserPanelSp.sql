EXEC ckbx_sp_Install_CustomUserFieldType 'Email'

ALTER TABLE ckbx_UserPanel ADD Email nvarchar(512) NULL

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Panel_InsertUser]') AND type in (N'P', N'PC'))
DROP PROCEDURE [ckbx_sp_Panel_InsertUser]
GO


IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Panel_InsertUser]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [ckbx_sp_Panel_InsertUser]

(
	@PanelID int,
	@UniqueIdentifier nvarchar (611),
	@Email nvarchar(512)
)
AS

	IF NOT EXISTS (SELECT PanelID FROM ckbx_UserPanel WHERE PanelID = @PanelID AND UserIdentifier = @UniqueIdentifier AND Email = @Email )
	BEGIN
		INSERT INTO ckbx_UserPanel (PanelID, UserIdentifier, Email) VALUES (@PanelID, @UniqueIdentifier, @Email)
	END
'
END
GO



IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Panel_GetUserPanelIDs]') AND type in (N'P', N'PC'))
DROP PROCEDURE [ckbx_sp_Panel_GetUserPanelIDs]
GO


IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Panel_GetUserPanelIDs]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [ckbx_sp_Panel_GetUserPanelIDs]
(
	@PanelID int
)
AS

	SELECT UserIdentifier, Email FROM ckbx_UserPanel WHERE PanelID = @PanelID
'

END

GO


