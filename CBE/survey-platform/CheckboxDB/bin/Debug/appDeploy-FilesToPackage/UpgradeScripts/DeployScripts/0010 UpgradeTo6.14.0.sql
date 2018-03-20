
/************** INSERT PATCH INFO ****************************/
DECLARE @ProductID int
DECLARE @PatchName varchar(64)
DECLARE @PatchVersion varchar(16)

SET @PatchVersion = '6.14.0'
SET @PatchName = '2016 Q2'


SELECT @ProductID = ProductID FROM ckbx_Product_Info WHERE ProductName = 'CheckboxWeb Survey'

IF @ProductID IS NOT NULL
  BEGIN
	IF NOT EXISTS(Select PatchName FROM ckbx_Product_Patches WHERE PatchName = @PatchName AND [Version] = @PatchVersion)
		BEGIN
			EXEC ckbx_sp_Product_InsertPatch @ProductID, NULL, @PatchName, @PatchVersion
		END
  END
GO
