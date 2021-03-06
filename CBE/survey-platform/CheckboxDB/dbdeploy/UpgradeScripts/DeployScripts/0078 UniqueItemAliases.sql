IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_GetAliases]') AND type in (N'P', N'PC'))
DROP PROCEDURE ckbx_sp_GetAliases
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_GetAliases]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [ckbx_sp_GetAliases]
As
    SET NOCOUNT ON;  
    SELECT Alias
    FROM ckbx_Item
	WHERE Alias IS NOT NULL AND Alias <> ''''
'
END
GO


-- update current records
DECLARE
    @counter    INT = 1,
    @max        INT = 0

CREATE TABLE #TempTable (
	ItemID int,
	Alias nvarchar(255)
)

INSERT INTO #TempTable (ItemID, Alias) 
SELECT ItemID, Alias FROM ckbx_Item WHERE Alias IN (
    SELECT Alias FROM ckbx_Item
    GROUP BY Alias HAVING COUNT(*) > 1 AND Alias IS NOT NULL AND Alias <> ''
)

SELECT @max = COUNT(ItemID) FROM #TempTable

WHILE @counter <= @max
BEGIN
	Update ckbx_Item
	Set Alias = Alias + convert(nvarchar(255),(SELECT ItemID FROM #TempTable ORDER BY ItemId OFFSET @counter ROWS FETCH NEXT 1 ROWS ONLY))
	Where ItemID = (SELECT ItemID FROM #TempTable ORDER BY ItemId OFFSET @counter ROWS FETCH NEXT 1 ROWS ONLY)

	SET @counter = @counter + 1
END
DROP TABLE #TempTable

