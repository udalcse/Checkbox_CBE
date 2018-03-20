GO

IF NOT EXISTS (SELECT TOP 1 * FROM ckbx_CustomUserField WHERE CustomUserFieldName = 'TenantId')
BEGIN 
	EXEC ckbx_sp_Profile_CreateProperty 'TenantId', 0, 0

	DECLARE @cnt INT = 0;
	DECLARE @cnt_total INT = 0;

	select @cnt_total = (max(Position) - 3) from ckbx_customUserField

	WHILE @cnt < @cnt_total
	BEGIN
   		exec [ckbx_sp_Profile_MoveProperty] 'TenantId', 1
	   SET @cnt = @cnt + 1;
	END;
END
GO

IF NOT EXISTS (SELECT TOP 1 * FROM ckbx_CustomUserField WHERE CustomUserFieldName = 'CompanyId')
BEGIN 
	EXEC ckbx_sp_Profile_CreateProperty 'CompanyId', 0, 0

	DECLARE @cnt INT = 0;
	DECLARE @cnt_total INT = 0;

	select @cnt_total = (max(Position) - 4) from ckbx_customUserField

	WHILE @cnt < @cnt_total
	BEGIN
   		exec [ckbx_sp_Profile_MoveProperty] 'CompanyId', 1
	   SET @cnt = @cnt + 1;
	END;
END
GO

IF NOT EXISTS (SELECT TOP 1 * FROM ckbx_CustomUserField WHERE CustomUserFieldName = 'ExternalUserId')
BEGIN 
	EXEC ckbx_sp_Profile_CreateProperty 'ExternalUserId', 0, 0

	DECLARE @cnt INT = 0;
	DECLARE @cnt_total INT = 0;

	select @cnt_total = (max(Position) - 5) from ckbx_customUserField

	WHILE @cnt < @cnt_total
	BEGIN
   		exec [ckbx_sp_Profile_MoveProperty] 'ExternalUserId', 1
	   SET @cnt = @cnt + 1;
	END;
END

GO