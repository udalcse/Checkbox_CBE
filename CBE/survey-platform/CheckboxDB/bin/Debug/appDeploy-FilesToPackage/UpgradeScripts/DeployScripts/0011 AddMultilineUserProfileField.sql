

--
--drop
--

--FK_ckbx_CustomUserField_ckbx_CustomUserFieldType drop
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_ckbx_CustomUserField_ckbx_CustomUserFieldType]') AND parent_object_id = OBJECT_ID(N'[ckbx_CustomUserField]'))
ALTER TABLE [ckbx_CustomUserField] DROP CONSTRAINT [FK_ckbx_CustomUserField_ckbx_CustomUserFieldType]
GO

--FK_ckbx_CustomUserFieldMap_ckbx_CustomUserField drop
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_ckbx_CustomUserFieldMap_ckbx_CustomUserField]') AND parent_object_id = OBJECT_ID(N'[ckbx_CustomUserFieldMap]'))
ALTER TABLE [ckbx_CustomUserFieldMap] DROP CONSTRAINT [FK_ckbx_CustomUserFieldMap_ckbx_CustomUserField]
GO

--table drop ckbx_CustomUserField
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_CustomUserField]') AND type in (N'U'))
DROP TABLE [ckbx_CustomUserField]
GO

--table drop ckbx_CustomUserFieldType
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_CustomUserFieldType]') AND type in (N'U'))
DROP TABLE [ckbx_CustomUserFieldType]
GO

--table drop ckbx_CustomUserFieldMap
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_CustomUserFieldMap]') AND type in (N'U'))
DROP TABLE [ckbx_CustomUserFieldMap]
GO

--Proc drop ckbx_sp_Install_CustomUserFieldType
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Install_CustomUserFieldType]') AND type in (N'P', N'PC'))
DROP PROCEDURE [ckbx_sp_Install_CustomUserFieldType]
GO

--proc drop ckbx_sp_ProfileProperties_Get
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_ProfileProperties_Get]') AND type in (N'P', N'PC'))
DROP PROCEDURE [ckbx_sp_ProfileProperties_Get]
GO

--proc drop ckbx_sp_Profile_UpdateField
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Profile_UpdateField]') AND type in (N'P', N'PC'))
 DROP PROCEDURE [ckbx_sp_Profile_UpdateField]
GO

--proc drop ckbx_sp_ProfilePropertiesList_Get
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_ProfilePropertiesList_Get]') AND type in (N'P', N'PC'))
 DROP PROCEDURE [ckbx_sp_ProfilePropertiesList_Get]
GO

--proc drop ckbx_sp_Profile_UpSertProperty
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Profile_UpSertProperty]') AND type in (N'P', N'PC'))
DROP PROCEDURE [ckbx_sp_Profile_UpSertProperty]
GO


--
--create table
--

--CustomUserFieldType
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_CustomUserFieldType]') AND type in (N'U'))
BEGIN
CREATE TABLE [ckbx_CustomUserFieldType](
	[CustomUserFieldTypeID] [int] IDENTITY(1,1) NOT NULL,
	[CustomFieldType] [nvarchar](100) NULL,
CONSTRAINT [PK_ckbx_CustomUserFieldType] PRIMARY KEY CLUSTERED 
(
	[CustomUserFieldTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

--CustomUserField
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_CustomUserField]') AND type in (N'U'))
BEGIN
CREATE TABLE [ckbx_CustomUserField](
	[CustomUserFieldID] [int] IDENTITY(1,1) NOT NULL,
	[CustomUserFieldName] [nvarchar](510) NULL,
	[Position] [int] NULL,
    [CustomUserFieldTypeID] [int] NULL CONSTRAINT [DF_ckbx_CustomUserField_CustomUserFieldTypeID]  DEFAULT ((1)),
	[IsDeletable] [bit] NULL,
	[Hidden] [bit] NULL,
	[ShowInUserManager] [bit] NULL,
 CONSTRAINT [PK_ckbx_CustomUserField] PRIMARY KEY NONCLUSTERED 
(
	[CustomUserFieldID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

--CustomUserFieldMap
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_CustomUserFieldMap]') AND type in (N'U'))
BEGIN
CREATE TABLE [ckbx_CustomUserFieldMap](
	[UniqueIdentifier] [nvarchar](611) NOT NULL,
	[CustomUserFieldID] [int] NOT NULL,
	[Value] [nvarchar](MAX) NULL
) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[ckbx_CustomUserFieldMap]') AND name = N'idx_UserProperty')
CREATE CLUSTERED INDEX [idx_UserProperty] ON [ckbx_CustomUserFieldMap] 
(
	[UniqueIdentifier] ASC,
	[CustomUserFieldID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_ckbx_CustomUserField_ckbx_CustomUserFieldType]') AND parent_object_id = OBJECT_ID(N'[ckbx_CustomUserField]'))
ALTER TABLE [ckbx_CustomUserField]  WITH CHECK ADD  CONSTRAINT [FK_ckbx_CustomUserField_ckbx_CustomUserFieldType] FOREIGN KEY([CustomUserFieldTypeID])
REFERENCES [dbo].[ckbx_CustomUserFieldType] ([CustomUserFieldTypeID])
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_ckbx_CustomUserFieldMap_ckbx_CustomUserField]') AND parent_object_id = OBJECT_ID(N'[ckbx_CustomUserFieldMap]'))
ALTER TABLE [ckbx_CustomUserFieldMap]  WITH CHECK ADD  CONSTRAINT [FK_ckbx_CustomUserFieldMap_ckbx_CustomUserField] FOREIGN KEY([CustomUserFieldID])
REFERENCES [ckbx_CustomUserField] ([CustomUserFieldID])
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_ckbx_CustomUserFieldMap_ckbx_CustomUserField]') AND parent_object_id = OBJECT_ID(N'[ckbx_CustomUserFieldMap]'))
ALTER TABLE [ckbx_CustomUserFieldMap] CHECK CONSTRAINT [FK_ckbx_CustomUserFieldMap_ckbx_CustomUserField]
GO

--new proc ckbx_sp_Install_CustomUserFieldType
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Install_CustomUserFieldType]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement =  N'
CREATE PROCEDURE ckbx_sp_Install_CustomUserFieldType
(
  @CustomUserFieldType nvarchar(100)
)
AS
  IF NOT EXISTS(Select CustomFieldType FROM ckbx_CustomUserFieldType WHERE CustomFieldType = @CustomUserFieldType)
    BEGIN
      INSERT INTO ckbx_CustomUserFieldType (CustomFieldType) VALUES (@CustomUserFieldType)
    END
  ELSE
    BEGIN
      UPDATE ckbx_CustomUserFieldType SET CustomFieldType = @CustomUserFieldType WHERE CustomFieldType = @CustomUserFieldType
    END
 ' 
 END
 GO

--new proc ckbx_sp_ProfileProperties_Get
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_ProfileProperties_Get]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [ckbx_sp_ProfileProperties_Get]
(
	  @UniqueIdentifier nvarchar(511)
)
AS
	SELECT
    cuf.CustomUserFieldName AS PropertyName,
    cufm.Value AS PropertyValue,
    cuft.CustomFieldType AS FieldType
  FROM
    ckbx_CustomUserField cuf
    LEFT OUTER JOIN ckbx_CustomUserFieldMap cufm ON cufm.CustomUserFieldId = cuf.CustomUserFieldId AND cufm.[UniqueIdentifier] = @UniqueIdentifier
    INNER JOIN ckbx_CustomUserFieldType cuft ON cuf.CustomUserFieldTypeID = cuft.CustomUserFieldTypeID
  ORDER BY
	cuf.Position ASC
' 
END
GO

--new proc ckbx_sp_Profile_UpdateField
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Profile_UpdateField]') AND type in (N'P', N'PC')) -- sp adding
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [ckbx_sp_Profile_UpdateField]
(
	@FieldName nvarchar(500),
	@FieldTypeName nvarchar(100)
)

AS
	DECLARE @FieldTypeID INT
	DECLARE @CurrentTypeID INT

	SELECT @FieldTypeID = CustomUserFieldTypeID 
	FROM [ckbx_CustomUserFieldType] as cuft
	WHERE cuft.CustomFieldType = @FieldTypeName

	IF EXISTS(SELECT * FROM [ckbx_CustomUserField]
			WHERE CustomUserFieldName = @FieldName)
		SET @CurrentTypeID = (SELECT CustomUserFieldTypeID FROM [ckbx_CustomUserField]
			WHERE CustomUserFieldName = @FieldName )
		IF (@FieldTypeID != @CurrentTypeID)
			UPDATE	[ckbx_CustomUserField]
			SET CustomUserFieldTypeID = @FieldTypeID
			WHERE CustomUserFieldName = @FieldName
' 
END
GO 

--new proc ckbx_sp_ProfilePropertiesList_Get
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_ProfilePropertiesList_Get]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [ckbx_sp_ProfilePropertiesList_Get]

AS
 	SELECT
     cuf.CustomUserFieldName AS PropertyName,
	 cuf.Hidden as IsHidden,
     cuft.CustomFieldType AS FieldType
   FROM ckbx_CustomUserField as cuf
   JOIN ckbx_CustomUserFieldType cuft ON cuf.CustomUserFieldTypeID = cuft.CustomUserFieldTypeID
' 
END
GO

--re-creating proc ckbx_sp_Profile_UpSertProperty with new MAX value
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Profile_UpSertProperty]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE ckbx_sp_Profile_UpSertProperty
(
	@UniqueIdentifier nvarchar(611),
	@PropertyName nvarchar(510),
    @PropertyValue nvarchar(MAX)
)
AS
BEGIN
	
	SET NOCOUNT ON

	DECLARE @CustomUserFieldID int

	SELECT	@CustomUserFieldID = CustomUserFieldID
	FROM	ckbx_CustomUserField
	WHERE	CustomUserFieldName = @PropertyName

	IF (@CustomUserFieldID > 0)
	BEGIN
		IF EXISTS (SELECT [Value] FROM ckbx_CustomUserFieldMap WHERE [UniqueIdentifier] = @UniqueIdentifier AND CustomUserFieldID = @CustomUserFieldID)
		BEGIN		
			UPDATE	ckbx_CustomUserFieldMap
			SET		[Value] = @PropertyValue 
			WHERE	[UniqueIdentifier] = @UniqueIdentifier 
			AND		CustomUserFieldID = @CustomUserFieldID
		END
		ELSE
		BEGIN
			INSERT INTO ckbx_CustomUserFieldMap([UniqueIdentifier], CustomUserFieldID, [Value])
			VALUES (@UniqueIdentifier, @CustomUserFieldID, @PropertyValue)
		END
	END
END' 
END
GO

--
--insert data
--
exec ckbx_sp_Text_Set '/pageText/settings/customUserFields.aspx/fieldType', 'en-US', 'Field Type' --adding field type name FIll check if already exists

EXEC ckbx_sp_Install_CustomUserFieldType 'SingleLine'
EXEC ckbx_sp_Install_CustomUserFieldType 'MultiLine'
EXEC ckbx_sp_Profile_CreateProperty 'FirstName', 0, 0
EXEC ckbx_sp_Profile_CreateProperty 'LastName', 0, 0
EXEC ckbx_sp_Text_Set '/siteText/siteName', 'en-US', N'CBE'

GO