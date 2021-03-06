IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_ResponseTerms]') AND type in (N'U'))
BEGIN
CREATE TABLE ckbx_ResponseTerms(
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[Term] [nvarchar](max) NOT NULL,
	[Definition] [nvarchar](max) NULL,
	[Link] [nvarchar](1000) NULL,
	[TemplateID] [int] NOT NULL
 CONSTRAINT [PK_ckbx_ResponseTerms] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO


ALTER PROCEDURE [dbo].[ckbx_sp_RT_GetResponseTemplate]

(

  @ResponseTemplateID int

)

AS

--1

  -- Master ResponseTemplate table
	SELECT
		t.TemplateID,
		t.ModifiedDate,
		t.Deleted,
		t.DefaultPolicy,
		t.AclID,
		t.CreatedDate,
		rt.*
	FROM
		ckbx_ResponseTemplate rt

      INNER JOIN ckbx_Template t on t.TemplateID = rt.ResponseTemplateID 

  WHERE 

      rt.ResponseTemplateID = @ResponseTemplateID

 

  --2

  -- Select All ItemData data for this ResponseTemplate

  SELECT 

      ckbx_Item.ItemID,

      ckbx_Item.ItemTypeID,

      ckbx_Item.Alias,

      ckbx_Item.CreatedDate,

      ckbx_Item.ModifiedDate,

      ckbx_Item.Deleted,

	  ckbx_Item.IsActive,

      ckbx_Template_Items.TemplateID,

      ckbx_ItemType.ItemDataClassName,

      ckbx_ItemType.ItemDataAssemblyName,

      ckbx_ItemType.DefaultAppearanceCode,

      ckbx_ItemType.ItemName,

      ckbx_ItemType.MobileCompatible

  FROM 

      ckbx_Item 

      INNER JOIN ckbx_Template_Items ON ckbx_Item.ItemID = ckbx_Template_Items.ItemID

      INNER JOIN ckbx_ItemType on ckbx_ItemType.ItemTypeID = ckbx_Item.ItemTypeID

  WHERE 

    ckbx_Template_Items.TemplateID = @ResponseTemplateID

    AND (ckbx_Item.Deleted IS NULL OR ckbx_Item.Deleted = 0)

 

  --3

  -- Select all PageData for this ResponseTemplate

  SELECT TemplateID, PageID, PagePosition, LayoutTemplateID FROM ckbx_Template_Pages WHERE TemplateID = @ResponseTemplateID ORDER BY PagePosition

 

  --4

  -- Select List Data from ItemOptions table for any Items with list data

    SELECT

            opt.OptionID, opt.ItemID, opt.TextID, opt.Alias, opt.IsDefault, opt.[Position], opt.IsOther, opt.Points, opt.Deleted, opt.ListID

    FROM 

            ckbx_ItemOptions opt

            INNER JOIN ckbx_Template_Items ti ON ti.ItemID = opt.ItemID

      WHERE

            ti.TemplateID = @ResponseTemplateID

            AND (opt.Deleted IS NULL OR (opt.Deleted IS NOT NULL AND opt.Deleted = 0))

 

  --5

  /* SELECTS from ckbx_TemplatePage_Items */

  SELECT 

      tp.PageID, tpi.ItemID, tpi.[Position]

  FROM

      ckbx_Template_Pages tp

      INNER JOIN ckbx_TemplatePage_Items tpi ON tpi.PageID = tp.PageID

      INNER JOIN ckbx_Template_Items ti ON ti.ItemID = tpi.ItemID and ti.TemplateID = tp.TemplateID

      INNER JOIN ckbx_Item i ON i.ItemID = ti.ItemID

      INNER JOIN ckbx_ItemType it ON it.ItemTypeID = i.ItemTypeID

  WHERE

      tp.TemplateID = @ResponseTemplateID

  ORDER BY

      tp.PagePosition, tpi.Position

 

  --6
/*
  Rule loading now performed by ckbx_sp_RT_GetResponseTemplate_Rules
  
  --Item Rules

  SELECT

      ir.ItemID, ir.RuleID

  FROM

      ckbx_ItemRules ir

      LEFT OUTER JOIN ckbx_Template_Items ti ON ti.ItemID = ir.ItemID

      LEFT OUTER JOIN ckbx_ItemData_MatrixItems mi ON mi.ItemID = ir.ItemID

      LEFT OUTER JOIN ckbx_Template_Items ti2 ON ti2.ItemID = mi.MatrixID

  WHERE

      (ti.TemplateID = @ResponseTemplateID OR ti2.TemplateID = @ResponseTemplateID)

      AND ((ti.ItemID IS NOT NULL) OR (mi.ItemID IS NOT NULL AND ti2.ItemID IS NOT NULL))

   

--SELECT ItemID, RuleID FROM ckbx_ItemRules WHERE ItemID IN 

--  (SELECT ItemID FROM ckbx_Template_Items WHERE TemplateID = @ResponseTemplateID)

-- OR ItemID IN

-- (SELECT ItemID FROM ckbx_ItemData_MatrixItems WHERE MatrixID IN 

--  (SELECT ItemID FROM ckbx_Template_Items WHERE TemplateID = @ResponseTemplateID))

  

  --7

  --Page Rules

  SELECT

      tpr.PageID, tpr.RuleID, tpr.EventTrigger

  FROM

      ckbx_TemplatePage_Rules tpr

      INNER JOIN ckbx_Template_Pages tp ON tp.PageID = tpr.PageID

  WHERE

      tp.TemplateID = @ResponseTemplateID

 

  --Prepare the temp tables

  IF object_id('tempdb..#Rules') IS NOT NULL

  BEGIN

      DROP Table #Rules

  END

  IF object_id('tempdb..#Expressions') IS NOT NULL

  BEGIN

      DROP Table #Expressions

  END

  IF object_id('tempdb..#Operands') IS NOT NULL

  BEGIN

      DROP Table #Operands

  END

 

  --GET ALL THE RULES DATA INTO 3 TABLES

  select RuleID, ExpressionID

  INTO #Rules

  FROM ckbx_Rule WHERE RuleID IN 

 

(SELECT RuleID FROM ckbx_ItemRules WHERE ItemID IN 

  (SELECT ItemID FROM ckbx_Template_Items WHERE TemplateID = @ResponseTemplateID)

 OR ItemID IN

 (SELECT ItemID FROM ckbx_ItemData_MatrixItems WHERE MatrixID IN 

  (SELECT ItemID FROM ckbx_Template_Items WHERE TemplateID = @ResponseTemplateID)))

  OR

  RuleID IN (SELECT RuleID FROM ckbx_TemplatePage_Rules WHERE PageID IN (

  SELECT PageID FROM ckbx_Template_Pages WHERE TemplateID = @ResponseTemplateID))

 

  SELECT * 

  INTO #Expressions

  FROM ckbx_Expression WHERE ExpressionID IN (

  SELECT ExpressionID FROM #Rules)

  OR ExpressionID IN (

  SELECT ExpressionID FROM ckbx_Expression

  WHERE Root IN (

  SELECT ExpressionID FROM #Rules))

 

  SELECT * INTO #Operands FROM ckbx_Operand WHERE OperandID IN (

  SELECT LeftOperand FROM #Expressions WHERE LeftOperand IS NOT NULL) 

  OR

  OperandID IN (

  SELECT RightOperand FROM #Expressions WHERE RightOperand IS NOT NULL )

 

  -- SELECT FROM TMP TABLES

  --8

  SELECT RuleID, ExpressionID FROM #Rules

 

  --9

  SELECT ExpressionID, Operator, LeftOperand, RightOperand, Parent, Depth, Lineage, Root, ChildRelation FROM #Expressions

 

  ---10

  SELECT OperandID, TypeName, TypeAssembly FROM #Operands

 

  ---11

  SELECT

      op.OperandID, op.ItemID, op.ParentItemID, op.ColumnNumber, op.Category

  FROM

      #Operands o

      INNER JOIN ckbx_ItemOperand op ON op.OperandID = o.OperandID

  

  ---12

  SELECT

      vo.OperandID, vo.ItemID, vo.OptionID, vo.AnswerValue

  FROM

      #Operands o

      INNER JOIN ckbx_ValueOperand vo ON vo.OperandID = o.OperandID

 

   --13

  SELECT

      po.OperandID, po.ProfileKey

  FROM

      #Operands o

      INNER JOIN ckbx_ProfileOperand po ON po.OperandID = o.OperandID

  

  -- SELECT THE RULEACTIONS

  -- 14

  SELECT

      ra.RuleID, ra.ActionID

  FROM

      #Rules r

      INNER JOIN ckbx_RuleActions ra ON ra.RuleID = r.RuleID

  --SELECT THE ACTIONS

  

  --15

  SELECT

      a.ActionID, a.ActionTypeName, a.ActionAssembly

  FROM

      #Rules r

      INNER JOIN ckbx_RuleActions ra ON ra.RuleID = r.RuleID

      INNER JOIN ckbx_Action a ON a.ActionID = ra.ActionID

 */

  --16

  --ResponsePipes

  SELECT ResponseTemplateID, PipeName, ItemID FROM ckbx_ResponsePipe WHERE ResponseTemplateID = @ResponseTemplateID

 

  --17
/*
  SELECT 

      ro.OperandID, ro.ResponseKey 

  FROM 

    #Operands o

      INNER JOIN ckbx_ResponseOperand ro ON ro.OperandID = o.OperandID

 */

  --18 Item Appearance Data

  SELECT 

    ia.ItemID, ia.AppearanceID 

  FROM 

    ckbx_ItemAppearances ia

    INNER JOIN ckbx_Template_Items ti ON ti.ItemID = ia.ItemID

    INNER JOIN ckbx_Item i ON i.ItemID = ti.ItemID

      INNER JOIN ckbx_ItemType it ON it.ItemTypeID = i.ItemTypeID

  WHERE

    ti.TemplateID = @ResponseTemplateID

    AND (i.Deleted IS NULL OR i.Deleted = 0)

 

  --19 Item Appearance

  SELECT 

    a.*

  FROM 

    ckbx_ItemAppearance a

    INNER JOIN ckbx_ItemAppearances ia ON ia.AppearanceID = a.AppearanceID

    INNER JOIN ckbx_Template_Items ti ON ti.ItemID = ia.ItemID

    INNER JOIN ckbx_Item i ON i.ItemID = ti.ItemID

      INNER JOIN ckbx_ItemType it ON it.ItemTypeID = i.ItemTypeID

  WHERE

    ti.TemplateID = @ResponseTemplateID

    AND (i.Deleted IS NULL OR i.Deleted = 0)

 

 

-- 20 BranchAction
/*
  SELECT 

      pba.ActionID,

      pba.GoToPageID

  FROM

      #Rules r

      INNER JOIN ckbx_RuleActions ra ON ra.RuleID = r.RuleID

      INNER JOIN ckbx_PageBranchAction pba ON pba.ActionID = ra.ActionID      
*/
 

 

  --CLEAN UP
/*
  DROP TABLE #Rules

  DROP TABLE #Expressions

  DROP TABLE #Operands */

 

--21 -- Item Lists

  SELECT 

      il.ListID, il.ItemID 

  FROM 

      ckbx_ItemLists il

      INNER JOIN ckbx_Template_Items ti ON ti.ItemID = il.ItemID

  WHERE

      ti.TemplateID = @ResponseTemplateID

  

--22 -- Profile Updater Item Data

  SELECT ItemID, SourceItemID, ProviderName, PropertyName FROM ckbx_ItemData_PUProps WHERE ItemID IN (Select ti.ItemID FROM ckbx_Template_Items ti WHERE ti.TemplateID = @ResponseTemplateID)


--23 -- Score Messages

  SELECT ScoreMessageID
      ,sm.ItemID
      ,LowScore
      ,HighScore
      ,MessageTextID
  FROM [ckbx_ItemData_ScoreMessages] sm
  inner join ckbx_Template_Items ti ON ti.ItemID = sm.ItemID
  WHERE
      ti.TemplateID = @ResponseTemplateID
 
 --24 -- Response Terms

  SELECT Id, 
	Name, 
	Term,
	[Definition],
	Link,
	TemplateID
  FROM ckbx_ResponseTerms 
  WHERE TemplateID = @ResponseTemplateID	

GO
  

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_RT_CreateOrUpdateResponseTerm]') AND type in (N'P', N'PC'))
DROP PROCEDURE [ckbx_sp_RT_CreateOrUpdateResponseTerm]
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_RT_CreateOrUpdateResponseTerm]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [ckbx_sp_RT_CreateOrUpdateResponseTerm]
(
	   @ResponseTemplateID int,
       @ID int,
       @Name nvarchar (200),
	   @Term nvarchar (max),
	   @Definition nvarchar(max),
	   @Link nvarchar(1000)
)
AS
	IF EXISTS(SELECT TOP 1 * FROM ckbx_ResponseTerms 
		WHERE Id = @ID) 
		BEGIN
			UPDATE ckbx_ResponseTerms 
				SET Name = @Name, 
				Term = @Term, 
				[Definition] = @Definition, 
				Link = @Link
			WHERE Id = @ID
		END
	ELSE 
		BEGIN
		    INSERT INTO ckbx_ResponseTerms
				(Name, Term, [Definition], Link, TemplateID)
			VALUES
				(@Name, @Term, @Definition, @Link, @ResponseTemplateID)

			SELECT SCOPE_IDENTITY()
		END

'
END
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_RT_DeleteResponseTerm]') AND type in (N'P', N'PC'))
DROP PROCEDURE ckbx_sp_RT_DeleteResponseTerm
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_RT_DeleteResponseTerm]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [ckbx_sp_RT_DeleteResponseTerm]
(
       @ID int
)
AS
	DELETE FROM ckbx_ResponseTerms
		WHERE Id = @ID
'
END
GO