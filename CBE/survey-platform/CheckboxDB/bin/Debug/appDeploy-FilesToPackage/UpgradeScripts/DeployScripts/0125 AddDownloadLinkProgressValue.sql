ALTER TABLE ckbx_Progress ADD DownloadLink nvarchar(1024) NULL

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Progress_Add]') AND type in (N'P', N'PC'))
DROP PROCEDURE [ckbx_sp_Progress_Add]
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Progress_Add]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [ckbx_sp_Progress_Add] 
(
	@Key nvarchar(64),
	@Message nvarchar(1024),
	@ErrorMessage nvarchar(1024),
	@CurrentItem int,
	@TotalItemCount int,
	@Status nvarchar(32),
	@DownloadLink nvarchar(1024)
)
AS
	BEGIN
		IF (NOT EXISTS(SELECT [Key] FROM ckbx_Progress WHERE [Key] = @Key))
			BEGIN
				INSERT INTO ckbx_Progress ([Key]) VALUES (@Key)
			END

		UPDATE ckbx_Progress 
			SET [Message] = @Message,
			ErrorMessage = @ErrorMessage, 
			CurrentItem = @CurrentItem,
			TotalItemCount = @TotalItemCount,
			Status = @Status,
			DownloadLink = @DownloadLink
		WHERE 
			[Key] = @Key
	END

'
END

GO


