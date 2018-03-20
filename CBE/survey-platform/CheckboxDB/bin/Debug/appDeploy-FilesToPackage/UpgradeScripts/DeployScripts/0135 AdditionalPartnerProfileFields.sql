GO

IF NOT EXISTS (SELECT TOP 1 * FROM ckbx_CustomUserField WHERE CustomUserFieldName = 'DDCompanyName')
BEGIN 
	EXEC ckbx_sp_Profile_CreateProperty 'DDCompanyName', 0, 0

	DECLARE @cnt INT = 0;
	DECLARE @cnt_total INT = 0;

	select @cnt_total = (max(Position) - 3) from ckbx_customUserField

	WHILE @cnt < @cnt_total
	BEGIN
   		exec [ckbx_sp_Profile_MoveProperty] 'DDCompanyName', 1
	   SET @cnt = @cnt + 1;
	END;
END
GO

IF NOT EXISTS (SELECT TOP 1 * FROM ckbx_CustomUserField WHERE CustomUserFieldName = 'ExternalSurveyId')
BEGIN 
	EXEC ckbx_sp_Profile_CreateProperty 'ExternalSurveyId', 0, 0

	DECLARE @cnt INT = 0;
	DECLARE @cnt_total INT = 0;

	select @cnt_total = (max(Position) - 4) from ckbx_customUserField

	WHILE @cnt < @cnt_total
	BEGIN
   		exec [ckbx_sp_Profile_MoveProperty] 'ExternalSurveyId', 1
	   SET @cnt = @cnt + 1;
	END;
END
GO

IF NOT EXISTS (SELECT TOP 1 * FROM ckbx_CustomUserField WHERE CustomUserFieldName = 'ExternalUsername')
BEGIN 
	EXEC ckbx_sp_Profile_CreateProperty 'ExternalUsername', 0, 0

	DECLARE @cnt INT = 0;
	DECLARE @cnt_total INT = 0;

	select @cnt_total = (max(Position) - 5) from ckbx_customUserField

	WHILE @cnt < @cnt_total
	BEGIN
   		exec [ckbx_sp_Profile_MoveProperty] 'ExternalUsername', 1
	   SET @cnt = @cnt + 1;
	END;
END
GO
