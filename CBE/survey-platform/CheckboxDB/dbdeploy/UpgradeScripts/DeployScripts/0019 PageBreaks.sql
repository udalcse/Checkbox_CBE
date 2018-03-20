

--Alter table
ALTER TABLE [ckbx_Template_Pages] ADD ShouldForceBreak bit NULL

-- Drops
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_TemplatePage_Get]') AND type in (N'P', N'PC'))
DROP PROCEDURE [ckbx_sp_TemplatePage_Get]
GO


IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_TemplatePage_AddPageBreak]') AND type in (N'P', N'PC'))
DROP PROCEDURE [ckbx_sp_TemplatePage_AddPageBreak]
GO


-- Create procs
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Template_InsertPage]') AND type in (N'P', N'PC'))
DROP PROCEDURE [ckbx_sp_Template_InsertPage]
GO


IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_TemplatePage_Get]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE ckbx_sp_TemplatePage_Get
(
  @PageId int
)
AS
  SELECT
    TemplateId,
    PageId,
    PagePosition,
    RandomizeItems,
    LayoutTemplateId,
    Title,
    PageType,
    ShouldForceBreak
  FROM
    ckbx_Template_Pages   
  WHERE
    PageId = @PageId' 
END
GO


IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_TemplatePage_AddPageBreak]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [ckbx_sp_TemplatePage_AddPageBreak]
(
	@PageId int,
    @ShouldPageBreak bit
)
AS
	UPDATE [ckbx_Template_Pages]
    SET ShouldForceBreak = @ShouldPageBreak
    WHERE PageId = @PageId
' 
END
GO


IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Template_InsertPage]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE  PROCEDURE [ckbx_sp_Template_InsertPage]
(
	@TemplateID int,
	@PagePosition int,
	@LayoutTemplateID int,
    @Title nvarchar(32),
    @PageType varchar(32),
    @ShouldForceBreak bit,
	@PageID int out
)
AS
	INSERT INTO ckbx_Template_Pages (TemplateID, PagePosition, LayoutTemplateID, PageType, ShouldForceBreak) VALUES (@TemplateID, @PagePosition, @LayoutTemplateID, @PageType, @ShouldForceBreak)

	SET @PageID = SCOPE_IDENTITY()
' 
END
GO

