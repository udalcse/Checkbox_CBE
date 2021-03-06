-- Add new item type
IF NOT EXISTS (SELECT 1 FROM ckbx_ItemType WHERE ItemName = 'GradientColorDirectorSkillsMatrix')
BEGIN
	INSERT INTO ckbx_ItemType
	(
		   ItemDataAssemblyName ,
		   ItemDataClassName ,
		   DefaultAppearanceCode ,
		   ItemName ,
		   MobileCompatible ,
		   CategoryId ,
		   RTCompatible ,
		   LibraryCompatible ,
		   ReportCompatible ,
		   Position ,
		   TextIdPrefix ,
		   IsAnswerable
	)
	VALUES
	(
			'Checkbox',
			'Checkbox.Analytics.Items.Configuration.GradientColorDirectorSkillsMatrixGraphData' ,
			'ANALYSIS_GRADIENT_COLOR_DIRECTOR_SKILLS_MATRIX',
			'GradientColorDirectorSkillsMatrix',
		   0 ,
		   1003 ,
		   0 ,
		   0 ,
		   1 ,
		   0 ,
		   null,
		   0
	)
END


-- Description for report item
EXEC ckbx_sp_Text_Set '/itemType/GradientColorDirectorSkillsMatrix/description' , 'en-US' , N'Display a graphical summary of answers.'


-- Localized name for report item
EXEC ckbx_sp_Text_Set '/itemType/GradientColorDirectorSkillsMatrix/name', 'en-US', N'Gradient Color Director Skills Matrix'


-- Appearance
IF NOT EXISTS (SELECT 1 FROM ckbx_ItemAppearanceCode WHERE AppearanceCode = 'ANALYSIS_GRADIENT_COLOR_DIRECTOR_SKILLS_MATRIX')
BEGIN
	INSERT INTO ckbx_ItemAppearanceCode
	(
	   AppearanceCode , DataTypeName, DataTypeAssembly
	)
	VALUES
	(
	   'ANALYSIS_GRADIENT_COLOR_DIRECTOR_SKILLS_MATRIX', 'Checkbox.Analytics.Items.UI.GradientColorDirectorSkillsMatrixAppearanceData' , 'Checkbox'
	)
END


-- Add record to ItemEditors
IF NOT EXISTS (SELECT 1 FROM ckbx_ItemEditors WHERE ItemTypeId = (SELECT ItemTypeId FROM ckbx_ItemType WHERE ItemName = 'GradientColorDirectorSkillsMatrix' ))
BEGIN
	INSERT INTO ckbx_ItemEditors
	(ItemTypeId, EditorTypeName, EditorTypeAssembly )
	VALUES
	((SELECT ItemTypeId FROM ckbx_ItemType WHERE ItemName = 'GradientColorDirectorSkillsMatrix' ),'Checkbox.Web.Analytics.UI.Editing.UserControlHostAnalysisItemEditor', 'Checkbox.Web')
END


-- Create GradientColorDirectorSkillsMatrix table
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_ItemData_GradientColorDirectorSkillsMatrix]') AND type in (N'U'))
DROP TABLE [ckbx_ItemData_GradientColorDirectorSkillsMatrix]
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_ItemData_GradientColorDirectorSkillsMatrix]') AND type in (N'U'))
BEGIN
CREATE TABLE [ckbx_ItemData_GradientColorDirectorSkillsMatrix](
       [ItemID] [int] NULL,
       [UseAliases] [bit] NULL
) ON [PRIMARY]
END
GO

-- Drop foreign key constraint
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_ckbx_ItemData_GradientColorDirectorSkillsMatrix_ckbx_Item]') AND parent_object_id = OBJECT_ID(N'[ckbx_ItemData_GradientColorDirectorSkillsMatrix]'))
ALTER TABLE [ckbx_ItemData_GradientColorDirectorSkillsMatrix] DROP CONSTRAINT [FK_ckbx_ItemData_GradientColorDirectorSkillsMatrix_ckbx_Item ]
GO

-- Add foreign key constraint
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_ckbx_ItemData_GradientColorDirectorSkillsMatrix_ckbx_Item ]') AND parent_object_id = OBJECT_ID(N'[ckbx_ItemData_GradientColorDirectorSkillsMatrix]'))
ALTER TABLE [ckbx_ItemData_GradientColorDirectorSkillsMatrix]  WITH CHECK ADD  CONSTRAINT [FK_ckbx_ItemData_GradientColorDirectorSkillsMatrix_ckbx_Item ] FOREIGN KEY([ItemID])
REFERENCES [dbo].[ckbx_Item] ([ItemID])
GO


-- Create sp for ckbx_sp_ItemData_GetGradientColorDirectorSkillsMatrix

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_ItemData_GetGradientColorDirectorSkillsMatrix]') AND type in (N'P', N'PC'))
DROP PROCEDURE [ckbx_sp_ItemData_GetGradientColorDirectorSkillsMatrix]
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_ItemData_GetGradientColorDirectorSkillsMatrix]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [ckbx_sp_ItemData_GetGradientColorDirectorSkillsMatrix]
@ItemID int
AS
SELECT
    ItemId ,
    ItemTypeId ,
    Alias ,
    CreatedDate ,
    ModifiedDate ,
    IsActive
  FROM
    ckbx_Item
  WHERE
    ItemId = @ItemId

   SELECT 
	skillsMatrix.ItemID, 
	skillsMatrix.UseAliases
  FROM 
     ckbx_ItemData_GradientColorDirectorSkillsMatrix skillsMatrix 
  WHERE 
    ItemID = @ItemID

  --Source Items
  SELECT AnalysisItemID , SourceItemID FROM ckbx_ItemData_AI_SourceItems WHERE AnalysisItemID = @ItemID

  --Response Templates
  SELECT AnalysisItemID , ResponseTemplateID FROM ckbx_ItemDATA_AI_ResponseTemplates WHERE AnalysisItemID = @ItemID

'
END
GO



-- Create sp for InsertGradientColorDirectorSkillsMatrix
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_ItemData_InsertGradientColorDirectorSkillsMatrix]') AND type in (N'P', N'PC'))
DROP PROCEDURE [ckbx_sp_ItemData_InsertGradientColorDirectorSkillsMatrix]
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_ItemData_InsertGradientColorDirectorSkillsMatrix]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [ckbx_sp_ItemData_InsertGradientColorDirectorSkillsMatrix]
(
       @ItemID int,
       @UseAliases bit
)
AS
    INSERT INTO ckbx_ItemData_GradientColorDirectorSkillsMatrix
    (ItemId , UseAliases)
    VALUES
    (@ItemId , @UseAliases)
'
END
GO



-- Create sp for UpdateGradientColorDirectorSkillsMatrix
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_ItemData_UpdateGradientColorDirectorSkillsMatrix]') AND type in (N'P', N'PC'))
DROP PROCEDURE [ckbx_sp_ItemData_UpdateGradientColorDirectorSkillsMatrix]
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_ItemData_UpdateGradientColorDirectorSkillsMatrix]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [ckbx_sp_ItemData_UpdateGradientColorDirectorSkillsMatrix]
(
    @ItemID int,
    @UseAliases bit
)
AS
    UPDATE ckbx_ItemData_GradientColorDirectorSkillsMatrix
    SET UseAliases = @UseAliases
    WHERE ItemID = @ItemID
'
END
GO


-- Add record to ckbx_ItemAppearanceEditor
IF NOT EXISTS (SELECT 1 FROM ckbx_ItemAppearanceEditor WHERE AppearanceCode = 'ANALYSIS_GRADIENT_COLOR_DIRECTOR_SKILLS_MATRIX')
BEGIN
	INSERT INTO ckbx_ItemAppearanceEditor (AppearanceCode, EditorTypeName, EditorTypeAssembly)
	VALUES ('ANALYSIS_GRADIENT_COLOR_DIRECTOR_SKILLS_MATRIX', 'Checkbox.Web.Analytics.UI.Editing.UserControlHostAnalysisItemAppearanceEditor' , 'Checkbox.Web')
end


-- Add record to ckbx_ItemAppearanceRenderer. Must be executed after ckbx_ItemAppearanceCode
IF NOT EXISTS (SELECT 1 FROM ckbx_ItemAppearanceRenderer WHERE AppearanceCode = 'ANALYSIS_GRADIENT_COLOR_DIRECTOR_SKILLS_MATRIX')
BEGIN
	INSERT INTO ckbx_ItemAppearanceRenderer
	(AppearanceCode, RendererTypeName, RendererAssemblyName )
	VALUES
	('ANALYSIS_GRADIENT_COLOR_DIRECTOR_SKILLS_MATRIX', 'Checkbox.Web.Analytics.UI.Rendering.UserControlHostAnalysisItemRenderer' , 'Checkbox.Web')
END
