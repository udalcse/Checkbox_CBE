IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_LibraryItemSettings]') AND type in (N'U'))
BEGIN
CREATE TABLE [ckbx_LibraryItemSettings](
	[ItemID] [int] NULL,
	[ShowInMenu] [bit] NULL
) ON [PRIMARY]
END
GO


IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Library_UpdateItemSettings]') AND type in (N'P', N'PC'))
DROP PROCEDURE [ckbx_sp_Library_UpdateItemSettings]
GO

CREATE PROCEDURE [dbo].[ckbx_sp_Library_UpdateItemSettings]
(
  @LibraryItemID int,
  @ShowShow bit
)
AS
IF EXISTS (SELECT TOP 1 * FROM ckbx_LibraryItemSettings WHERE ItemID = @LibraryItemID)
	BEGIN
		UPDATE ckbx_LibraryItemSettings 
			SET ShowInMenu = @ShowShow
			WHERE ItemID = @LibraryItemID
	END
ELSE 
	BEGIN
		INSERT INTO ckbx_LibraryItemSettings VALUES (@LibraryItemID, @ShowShow)
	END

GO


IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Library_GetItemsSettings]') AND type in (N'P', N'PC'))
DROP PROCEDURE [ckbx_sp_Library_GetItemsSettings]
GO

CREATE PROCEDURE [dbo].[ckbx_sp_Library_GetItemsSettings]
(
  @LibraryItemIds nvarchar(max)
)
AS

DECLARE @Result TABLE(CsvConfig NVARCHAR(510))

INSERT INTO 
	@Result
SELECT CsvConfig FROM Split(@LibraryItemIds)


SELECT lis.ItemID, lis.ShowInMenu FROM ckbx_LibraryItemSettings lis
	LEFT JOIN @Result ON CsvConfig = lis.ItemID


GO
