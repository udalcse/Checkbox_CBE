IF NOT EXISTS(
    SELECT *
    FROM sys.columns 
    WHERE Name      = N'ReportableSectionBreak'
      AND Object_ID = Object_ID(N'ckbx_ItemData_Message'))
BEGIN
	ALTER TABLE ckbx_ItemData_Message ADD ReportableSectionBreak bit NULL
END



IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_ItemData_UpdateMessage]') AND type in (N'P', N'PC'))
DROP PROCEDURE [ckbx_sp_ItemData_UpdateMessage]
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_ItemData_UpdateMessage]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [ckbx_sp_ItemData_UpdateMessage] 

(
  @ItemID int,
  @TextID nvarchar(255),
  @ReportableSectionBreak bit
)
AS
  UPDATE ckbx_ItemData_Message SET TextID = @TextID, ReportableSectionBreak = @ReportableSectionBreak WHERE ItemID = @ItemID



'
END

GO



IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_ItemData_GetMessage]') AND type in (N'P', N'PC'))
DROP PROCEDURE [ckbx_sp_ItemData_GetMessage]
GO


IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_ItemData_GetMessage]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [ckbx_sp_ItemData_GetMessage]
(
  @ItemID int
)
AS
  SELECT ItemID, TextID, ReportableSectionBreak FROM ckbx_ItemData_Message WHERE ItemID = @ItemID


'

END
GO








