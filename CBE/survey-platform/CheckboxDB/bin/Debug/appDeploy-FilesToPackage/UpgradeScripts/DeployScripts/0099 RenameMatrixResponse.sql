-- Drop(old - in future un-used)

--DROP PK
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[PK_ckbx_MatrixResponseState') AND parent_object_id = OBJECT_ID(N'[ckbx_MatrixResponseState]'))
BEGIN
	ALTER TABLE [ckbx_MatrixResponseState] DROP CONSTRAINT [PK_ckbx_MatrixResponseState]
END
GO

--DROP FK's
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_ckbx_MatrixResponseState_ckbx_Item]') AND parent_object_id = OBJECT_ID(N'[ckbx_MatrixResponseState]'))
BEGIN
	ALTER TABLE [ckbx_MatrixResponseState] DROP CONSTRAINT [FK_ckbx_MatrixResponseState_ckbx_Item]
END
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_ckbx_MatrixResponseState_ckbx_Response]') AND parent_object_id = OBJECT_ID(N'[ckbx_MatrixResponseState]'))
BEGIN
	ALTER TABLE [ckbx_MatrixResponseState] DROP CONSTRAINT [FK_ckbx_MatrixResponseState_ckbx_Response]
END
GO


-- Drop TABLE
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'ckbx_MatrixResponseState') AND type in (N'U'))
BEGIN
	DROP TABLE ckbx_MatrixResponseState
END
GO


-- Drop(new)

-- Drop CONSTRAINTS

--DROP PK
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[PK_ckbx_UserFieldResponseState') AND parent_object_id = OBJECT_ID(N'[ckbx_UserFieldResponseState]'))
BEGIN
	ALTER TABLE [ckbx_UserFieldResponseState] DROP CONSTRAINT [PK_ckbx_UserFieldResponseState]
END
GO

--DROP FK's
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_ckbx_UserFieldResponseState_ckbx_Item]') AND parent_object_id = OBJECT_ID(N'[ckbx_UserFieldResponseState]'))
BEGIN
	ALTER TABLE [ckbx_UserFieldResponseState] DROP CONSTRAINT [FK_ckbx_UserFieldResponseState_ckbx_Item]
END
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_ckbx_UserFieldResponseState_ckbx_Response]') AND parent_object_id = OBJECT_ID(N'[ckbx_UserFieldResponseState]'))
BEGIN
	ALTER TABLE [ckbx_UserFieldResponseState] DROP CONSTRAINT [FK_ckbx_UserFieldResponseState_ckbx_Response]
END
GO

-- Drop TABLE
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'ckbx_UserFieldResponseState') AND type in (N'U'))
BEGIN
	DROP TABLE ckbx_UserFieldResponseState
END
GO


-- CREATION
-- Table Creation
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'ckbx_UserFieldResponseState') AND type in (N'U'))
BEGIN
CREATE TABLE ckbx_UserFieldResponseState(
	[ID] [int] IDENTITY(1,1) NOT NULL,							--PK
	[CustromUserFieldID] [int] NOT NULL,
	[ItemID] [int] NOT NULL,									--FK to Item_ItemID
	[ResponseID] [bigint],										--FK to Response_ResponseID
	[FieldName] [nvarchar](510) NULL,
	[UserName] [nvarchar] (510) NULL,
	[DateCreated] [datetime],
	[ResponseData] [nvarchar] (MAX),
	[Response] [uniqueidentifier],
	[ResponseTemplateID] int
	 CONSTRAINT PK_ckbx_UserFieldResponseState PRIMARY KEY CLUSTERED 
	(
		[ID] ASC
	)
	WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	)
	END
GO


-- Adding CONSTRAINTS

-- ADDING FK

-- State -> Item FK
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_ckbx_UserFieldResponseState_ckbx_Item]') AND parent_object_id = OBJECT_ID(N'[ckbx_UserFieldResponseState]'))
ALTER TABLE [ckbx_UserFieldResponseState] WITH CHECK
ADD CONSTRAINT [FK_ckbx_UserFieldResponseState_ckbx_Item] FOREIGN KEY([ItemID])
REFERENCES [ckbx_Item] ([ItemID])
GO
-- Check
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_ckbx_UserFieldResponseState_ckbx_Item]') AND parent_object_id = OBJECT_ID(N'[ckbx_UserFieldResponseState]'))
ALTER TABLE [ckbx_UserFieldResponseState] CHECK CONSTRAINT [FK_ckbx_UserFieldResponseState_ckbx_Item]
GO

-- State -> Response FK
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_ckbx_UserFieldResponseState_ckbx_Response]') AND parent_object_id = OBJECT_ID(N'[ckbx_UserFieldResponseState]'))
ALTER TABLE [ckbx_UserFieldResponseState] WITH CHECK
ADD CONSTRAINT [FK_ckbx_UserFieldResponseState_ckbx_Response] FOREIGN KEY([ResponseID])
REFERENCES [ckbx_Response] ([ResponseID])
GO
-- Check
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_ckbx_UserFieldResponseState_ckbx_Response]') AND parent_object_id = OBJECT_ID(N'[ckbx_UserFieldResponseState]'))
ALTER TABLE [ckbx_UserFieldResponseState] CHECK CONSTRAINT [FK_ckbx_UserFieldResponseState_ckbx_Response]
GO

-----------
-- SPROC --
-----------

-- INSERT
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_UserFieldResponseState_InsertResponse]') AND type in (N'P', N'PC'))
BEGIN
	DROP PROCEDURE [ckbx_sp_UserFieldResponseState_InsertResponse]
END
GO
CREATE PROCEDURE [ckbx_sp_UserFieldResponseState_InsertResponse]
(
	@ItemId int,
	@UserName nvarchar (510),
	@ResponseData nvarchar (MAX),
	@Response uniqueidentifier,
	@CustomUserFieldId int,
	@FieldName nvarchar(510)
)
AS
BEGIN
	DECLARE @ResponseId bigint
	DECLARE @ResponseTemplateId int

	SELECT @ResponseId = [ResponseID], @ResponseTemplateId = [ResponseTemplateID]  FROM ckbx_Response WHERE GUID = @Response

	IF (@ResponseId IS NOT NULL)
	BEGIN
		BEGIN TRANSACTION
			BEGIN TRY
				IF NOT EXISTS (SELECT ID FROM ckbx_UserFieldResponseState as cufrs WHERE cufrs.ItemId = @ItemId AND cufrs.Response = @Response)
				BEGIN
					INSERT INTO ckbx_UserFieldResponseState
					(ItemID, UserName, ResponseData, Response, CustromUserFieldID, FieldName, ResponseID, DateCreated, ResponseTemplateID)
					VALUES
					(@ItemId, @UserName,@ResponseData,@Response, @CustomUserFieldId, @FieldName , @ResponseId, GETDATE(), @ResponseTemplateId)
				END
				ELSE
				BEGIN
					UPDATE ckbx_UserFieldResponseState
					SET ResponseData = @ResponseData,
					DateCreated = GETDATE()
					WHERE ItemID = @ItemId AND Response = @Response
				END
				COMMIT TRANSACTION
			END TRY
		BEGIN CATCH
			RAISERROR(N'Error on insert or update for item %i using sproc %s',
						10,
						1,
						@ItemId,
						'ckbx_sp_UserFieldResponseState_InsertResponse')
			ROLLBACK TRANSACTION
		END CATCH
	END
END
GO

-- DELETE
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_UserFieldResponseState_DeleteResponse]') AND type in (N'P', N'PC'))
BEGIN
	DROP PROCEDURE [ckbx_sp_UserFieldResponseState_DeleteResponse]
END
GO
CREATE PROCEDURE [ckbx_sp_UserFieldResponseState_DeleteResponse]
(
	@ResponseId int
)
AS
BEGIN
	DELETE FROM ckbx_UserFieldResponseState
	WHERE ResponseID = @ResponseId
END
GO

-- DELETE RESPONS FOR RT
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_UserFieldResponseState_DeleteAllResponsesForTemplate]') AND type in (N'P', N'PC'))
BEGIN
	DROP PROCEDURE [ckbx_sp_UserFieldResponseState_DeleteAllResponsesForTemplate]
END
GO
CREATE PROCEDURE [ckbx_sp_UserFieldResponseState_DeleteAllResponsesForTemplate]
(
	@ResponseTemaplteId int
)
AS
BEGIN
	DELETE FROM ckbx_UserFieldResponseState
	WHERE ResponseTemplateID = @ResponseTemaplteId
END
GO

-- GET
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_UserFieldResponseState_GetResponse]') AND type in (N'P', N'PC'))
BEGIN
	DROP PROCEDURE [ckbx_sp_UserFieldResponseState_GetResponse]
END
GO
CREATE PROCEDURE [ckbx_sp_UserFieldResponseState_GetResponse]
(
	@ItemId int,
	@UserName nvarchar (510),
	@Response uniqueidentifier
)
AS
BEGIN
	SELECT (ResponseData) FROM ckbx_UserFieldResponseState
	WHERE ItemId = @ItemId 
	AND Response = @Response
	AND UserName = @UserName
END
GO

--ALTER EXISTING SP's FOR RESPONSE REMOVE
ALTER PROCEDURE [dbo].[ckbx_sp_Response_Delete]
(
	@ResponseID bigint

)
AS
BEGIN
  UPDATE ckbx_Response SET Deleted = 1 WHERE ResponseID = @ResponseID
  DELETE FROM ckbx_Response_PageItemOrder WHERE ResponseID = @ResponseID
  DELETE FROM ckbx_Response_ItemOptionOrder WHERE ResponseID = @ResponseID
  DELETE FROM ckbx_UserFieldResponseState	WHERE ResponseID = @ResponseID
END
GO

ALTER PROCEDURE [dbo].[ckbx_sp_Response_DeleteForRT]
(
	@ResponseTemplateID int
)
AS
BEGIN
	UPDATE ckbx_Response SET Deleted = 1 WHERE ResponseTemplateID = @ResponseTemplateID /*AND Ended is not null*/
	UPDATE ckbx_ResponseAnswers SET Deleted = 1 WHERE ResponseID 
		IN(Select responseID FROM ckbx_response WHERE ResponseTemplateID = @ResponseTemplateID /*AND Ended is not null*/)
	DELETE FROM ckbx_ResponseLog WHERE ResponseID
		IN(Select responseID FROM ckbx_response WHERE ResponseTemplateID = @ResponseTemplateID /*AND Ended is not null*/)
	DELETE FROM ckbx_UserFieldResponseState WHERE ResponseTemplateID = @ResponseTemplateID
END
GO



-----------
-- SPROC --
-----------

-- INSERT
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_MatrixResponseState_InsertResponse]') AND type in (N'P', N'PC'))
BEGIN
	DROP PROCEDURE [ckbx_sp_MatrixResponseState_InsertResponse]
END
GO

-- DELETE
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_MatrixResponseState_DeleteResponse]') AND type in (N'P', N'PC'))
BEGIN
	DROP PROCEDURE [ckbx_sp_MatrixResponseState_DeleteResponse]
END
GO


-- DELETE RESPONS FOR RT
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_MatrixResponseState_DeleteAllResponsesForTemplate]') AND type in (N'P', N'PC'))
BEGIN
	DROP PROCEDURE [ckbx_sp_MatrixResponseState_DeleteAllResponsesForTemplate]
END
GO

-- GET
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_MatrixResponseState_GetResponse]') AND type in (N'P', N'PC'))
BEGIN
	DROP PROCEDURE [ckbx_sp_MatrixResponseState_GetResponse]
END
GO


--ALTER EXISTING SP's FOR RESPONSE REMOVE
ALTER PROCEDURE [dbo].[ckbx_sp_Response_Delete]
(
	@ResponseID bigint
)
AS
BEGIN
  UPDATE ckbx_Response SET Deleted = 1 WHERE ResponseID = @ResponseID
  DELETE FROM ckbx_Response_PageItemOrder WHERE ResponseID = @ResponseID
  DELETE FROM ckbx_Response_ItemOptionOrder WHERE ResponseID = @ResponseID
  DELETE FROM ckbx_UserFieldResponseState WHERE ResponseID = @ResponseID
END
GO

ALTER PROCEDURE [dbo].[ckbx_sp_Response_DeleteForRT]
(
	@ResponseTemplateID int
)
AS
BEGIN
	UPDATE ckbx_Response SET Deleted = 1 WHERE ResponseTemplateID = @ResponseTemplateID /*AND Ended is not null*/
	UPDATE ckbx_ResponseAnswers SET Deleted = 1 WHERE ResponseID 
		IN(Select responseID FROM ckbx_response WHERE ResponseTemplateID = @ResponseTemplateID /*AND Ended is not null*/)
	DELETE FROM ckbx_ResponseLog WHERE ResponseID
		IN(Select responseID FROM ckbx_response WHERE ResponseTemplateID = @ResponseTemplateID /*AND Ended is not null*/)
	DELETE FROM ckbx_UserFieldResponseState WHERE ResponseTemplateID = @ResponseTemplateID
END
GO