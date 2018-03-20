
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_RemoveUserImportConfig]') AND type in (N'P', N'PC'))
DROP PROCEDURE [ckbx_sp_RemoveUserImportConfig]
GO


IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_RemoveUserImportConfig]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [ckbx_sp_RemoveUserImportConfig]
As
    SET NOCOUNT ON;  
   
    DELETE FROM ckbx_UserImportConfig

'
END
GO


