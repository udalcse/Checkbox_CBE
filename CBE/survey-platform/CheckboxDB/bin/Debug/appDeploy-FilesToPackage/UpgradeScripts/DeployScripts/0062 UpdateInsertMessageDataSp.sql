
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_ItemData_InsertMessage]') AND type in (N'P', N'PC'))
DROP PROCEDURE [ckbx_sp_ItemData_InsertMessage]
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_ItemData_InsertMessage]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [ckbx_sp_ItemData_InsertMessage] 
(
  @ItemID int,
  @TextID nvarchar(255),
  @ReportableSectionBreak bit
)
AS
    INSERT INTO ckbx_ItemData_Message (ItemID, TextID, ReportableSectionBreak) VALUES (@ItemID, @TextID, @ReportableSectionBreak)
    RETURN
'
END
GO


