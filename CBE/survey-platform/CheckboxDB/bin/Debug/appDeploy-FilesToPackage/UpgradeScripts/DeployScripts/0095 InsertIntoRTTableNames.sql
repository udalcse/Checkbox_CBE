  IF (NOT EXISTS (SELECT TOP 1 * FROM [dbo].[ckbx_RTTableNames] WHERE TableName = 'ResponseTerms'))
  BEGIN
	INSERT INTO [dbo].[ckbx_RTTableNames] VALUES ('ResponseTerms', 12)
  END
  
  GO