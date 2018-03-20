
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_ProfileProperties_Get]') AND type in (N'P', N'PC'))
DROP PROCEDURE [ckbx_sp_ProfileProperties_Get]
GO


IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_ProfileProperties_Get]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [ckbx_sp_ProfileProperties_Get] 
(
	  @UniqueIdentifier nvarchar(511)
)
AS
	SELECT
    cuf.CustomUserFieldName AS PropertyName,
    cufm.Value AS PropertyValue,
    cuft.CustomFieldType AS FieldType,
	cuf.CustomUserFieldID as CustomUserFieldID
  FROM
    ckbx_CustomUserField cuf
    LEFT OUTER JOIN ckbx_CustomUserFieldMap cufm ON cufm.CustomUserFieldId = cuf.CustomUserFieldId AND cufm.[UniqueIdentifier] = @UniqueIdentifier
    INNER JOIN ckbx_CustomUserFieldType cuft ON cuf.CustomUserFieldTypeID = cuft.CustomUserFieldTypeID
  ORDER BY
	cuf.Position ASC

'
END
GO


IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_ItemData_GetSLText]') AND type in (N'P', N'PC'))
DROP PROCEDURE [ckbx_sp_ItemData_GetSLText]
GO


IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_ItemData_GetSLText]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [ckbx_sp_ItemData_GetSLText] 
(
  @ItemID int
)
AS

DECLARE @BindedPropId int 

SET @BindedPropId = (SELECT CustomUserFieldId from ckbx_CustomUserFieldItemMap WHERE ItemId = @ItemID)

-- Get Item data
  SELECT
    ItemId,
    ItemTypeId,
    Alias,
    CreatedDate,
    ModifiedDate,
    IsActive
  FROM
    ckbx_Item
  WHERE
    ItemId = @ItemId
    
  SELECT
    ItemID,
    TextID,
    SubTextID,
    IsRequired,
    DefaultTextID,
    TextFormat,
    MaxLength,
    MaxValue,
    MinValue,
	AutocompleteListId,
	AutocompleteRemote,
	@BindedPropId as BindedPropertyId

  FROM
    ckbx_ItemData_SLText
  WHERE
    ItemID = @ItemID

  
'
END
GO


IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_ItemData_GetMLText]') AND type in (N'P', N'PC'))
DROP PROCEDURE [ckbx_sp_ItemData_GetMLText]
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_ItemData_GetMLText]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [ckbx_sp_ItemData_GetMLText]
(
  @ItemID int
)
AS

DECLARE @BindedPropId int 

SET @BindedPropId = (SELECT CustomUserFieldId from ckbx_CustomUserFieldItemMap WHERE ItemId = @ItemID)

-- Get Item data
  SELECT
    ItemId,
    ItemTypeId,
    Alias,
    CreatedDate,
    ModifiedDate,
    IsActive
  FROM
    ckbx_Item
  WHERE
    ItemId = @ItemId
    
    
  SELECT
    ItemID,
    TextID,
    SubTextID,
    IsRequired,
    DefaultTextID,
	IsHtmlFormattedData,
	[MaxLength],
	MinLength,
	@BindedPropId as BindedPropertyId 
  FROM
    ckbx_ItemData_MLText
  WHERE
    ItemID = @ItemID

'
END
GO



IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_ItemData_GetMatrix]') AND type in (N'P', N'PC'))
DROP PROCEDURE [ckbx_sp_ItemData_GetMatrix]
GO


IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_ItemData_GetMatrix]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [ckbx_sp_ItemData_GetMatrix] 
(
  @ItemID int
)
AS
 
  DECLARE @BindedPropId int 
  SET @BindedPropId = (SELECT CustomUserFieldId from ckbx_CustomUserFieldItemMap WHERE ItemId = @ItemID)

  /* Matrix Item Data */
  SELECT ItemID, TextID, SubTextID, PKIndex, IsRequired, @BindedPropId as BindedPropertyId FROM ckbx_ItemData_Matrix WHERE ItemID = @ItemID

 

  /* Matrix Item Data */
  SELECT
    mi.MatrixID,
    mi.Row,
    mi.[Column],
    mi.ItemID,
    it.ItemName as ItemTypeName
  FROM
    ckbx_ItemData_MatrixItems mi
    INNER JOIN ckbx_Item i ON i.ItemID = mi.ItemID
    INNER JOIN ckbx_ItemType it ON it.ItemTypeID = i.ItemTypeID
  WHERE
    mi.MatrixID = @ItemID


  /* Matrix Column Data */
  SELECT
    mc.MatrixID,
    mc.[Column],
    mc.ColumnPrototypeID,
    mc.UniqueAnswers,
	mc.Width,
	it.ItemName as ItemTypeName
  FROM
    ckbx_ItemData_MatrixColumns mc
    INNER JOIN ckbx_Item i ON i.ItemID = mc.ColumnPrototypeID
    INNER JOIN ckbx_ItemType it ON it.ItemTypeID = i.ItemTypeID
  WHERE
    mc.MatrixID = @ItemID

  /* Matrix Row Data */
  SELECT
    mr.MatrixID,
    mr.Row,
    mr.IsSubheading,
    mr.IsOther,
    i.Alias
  FROM
    ckbx_ItemData_MatrixRows mr
	INNER JOIN ckbx_ItemData_Matrix m ON m.ItemID = mr.MatrixID
    LEFT OUTER JOIN ckbx_ItemData_MatrixItems mi ON mi.MatrixID = mr.MatrixID AND mi.Row = mr.Row AND mi.[Column] = m.PKIndex
	LEFT OUTER JOIN ckbx_Item i ON i.ItemID = mi.ItemID
  WHERE
    mr.MatrixID = @ItemID

	/* Matrix Column Prototypes */
	SELECT	
		i.ItemID,
		i.ItemTypeID,
		i.Alias,
		i.CreatedDate,
		i.ModifiedDate,
		i.Deleted,
		it.ItemDataClassName,
		it.ItemDataAssemblyName,
		it.DefaultAppearanceCode,
		it.ItemName,
		it.MobileCompatible
	FROM
		ckbx_ItemData_MatrixColumns mc
		INNER JOIN ckbx_Item i ON i.ItemID = mc.ColumnPrototypeID
		INNER JOIN ckbx_ItemType it ON it.ItemTypeID = i.ItemTypeID
	WHERE
		mc.MatrixID = @ItemID
'
END
GO



IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_ItemData_GetSelect1]') AND type in (N'P', N'PC'))
DROP PROCEDURE [ckbx_sp_ItemData_GetSelect1]
GO


IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_ItemData_GetSelect1]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [ckbx_sp_ItemData_GetSelect1]
(
  @ItemID int
)
AS

 DECLARE @BindedPropId int 
 SET @BindedPropId = (SELECT CustomUserFieldId from ckbx_CustomUserFieldItemMap WHERE ItemId = @ItemID)

  -- Get Item data
  SELECT
    ItemId,
    ItemTypeId,
    Alias,
    CreatedDate,
    ModifiedDate,
    IsActive
  FROM
    ckbx_Item
  WHERE
    ItemId = @ItemId

  --Get Base Select1 Data
  SELECT
    ItemID,
    TextID,
    SubTextID,
    IsRequired,
    AllowOther,
    OtherTextID,
    Randomize,
    @BindedPropId as BindedPropertyId
  FROM
    ckbx_ItemData_Select1
  WHERE
    ItemID = @ItemID

  --Get Item Options Lists
  SELECT
    ItemID,
    ListID
  FROM
    ckbx_ItemLists
  WHERE
    ItemID = @ItemID

  --Get item options data
  SELECT
    OptionID,
    @ItemID as ItemID,
    TextID,
    Alias,
    Category,
    IsDefault,
    Position,
    IsOther,
    Points,
    Deleted,
--    [io].ListID
    ListID
  FROM
    ckbx_ItemOptions [io]
    --INNER join ckbx_ItemLists il ON il.ListID = [io].ListID
  WHERE
    --il.ItemID = @ItemID
    [io].ItemID = @ItemID
    AND ([io].Deleted IS NULL OR [io].Deleted = 0)

'
END
GO









