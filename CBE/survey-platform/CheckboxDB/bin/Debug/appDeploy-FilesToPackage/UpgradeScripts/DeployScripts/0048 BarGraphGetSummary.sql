IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_ItemData_GetGovernancePrioritySummary]') AND type in (N'P', N'PC'))
DROP PROCEDURE [ckbx_sp_ItemData_GetGovernancePrioritySummary]
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_ItemData_GetGovernancePrioritySummary]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [ckbx_sp_ItemData_GetGovernancePrioritySummary]
@ItemID int
AS
SELECT
    ItemId,
    UseAliases
  FROM
    ckbx_ItemData_GovernancePriority
  WHERE
    ItemID = @ItemID
'
END
GO
