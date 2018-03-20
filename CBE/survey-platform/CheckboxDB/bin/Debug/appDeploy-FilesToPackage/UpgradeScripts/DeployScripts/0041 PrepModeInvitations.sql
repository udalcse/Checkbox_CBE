
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_PrepModeUsers]') AND type in (N'U'))
DROP TABLE [ckbx_PrepModeUsers]
GO


IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_PrepModeUsers]') AND type in (N'U'))
BEGIN
CREATE TABLE [ckbx_PrepModeUsers](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserGuid] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_dbo.ckbx_PrepModeUsers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO



IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_SystemMode_AddPrepModeUserToIvitationList]') AND type in (N'P', N'PC'))
DROP PROCEDURE [ckbx_sp_SystemMode_AddPrepModeUserToIvitationList]
GO


IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_SystemMode_AddPrepModeUserToIvitationList]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [ckbx_sp_SystemMode_AddPrepModeUserToIvitationList]  
@UserGuid nvarchar(255),
@Checked bit
AS

SET NOCOUNT ON

	IF @Checked = 1
		BEGIN
			IF NOT EXISTS(SELECT 1 FROM ckbx_PrepModeUsers  WHERE UserGuid = @UserGuid) 		
				INSERT INTO ckbx_PrepModeUsers(UserGuid) VALUES (@UserGuid);
		END
	ELSE
		BEGIN
			DELETE FROM ckbx_PrepModeUsers WHERE UserGuid = @UserGuid
		END

'
END
GO


IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_SystemMode_GetPrepModeUsers]') AND type in (N'P', N'PC'))
DROP PROCEDURE [ckbx_sp_SystemMode_GetPrepModeUsers]
GO


IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_SystemMode_GetPrepModeUsers]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [ckbx_sp_SystemMode_GetPrepModeUsers]

AS

SET NOCOUNT ON
	
   SELECT UserGuid FROM ckbx_PrepModeUsers

'
END
GO


IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_SystemMode_IsPrepModeUser]') AND type in (N'P', N'PC'))
DROP PROCEDURE [ckbx_sp_SystemMode_IsPrepModeUser]
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_SystemMode_IsPrepModeUser]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [ckbx_sp_SystemMode_IsPrepModeUser]

@UserGuid nvarchar(255)

AS

SET NOCOUNT ON
	
	SELECT CASE WHEN EXISTS ( SELECT * FROM ckbx_PrepModeUsers WHERE UserGuid = @UserGuid)
	THEN CAST(1 AS BIT)
	ELSE CAST(0 AS BIT) END

'
END
GO
