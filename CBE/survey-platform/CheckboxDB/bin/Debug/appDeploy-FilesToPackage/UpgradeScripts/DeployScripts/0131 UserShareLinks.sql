GO

IF NOT EXISTS (SELECT TOP 1 * FROM ckbx_Text WHERE TextID = N'/pageMenu/survey_editor/UserLinks')
BEGIN
	INSERT [dbo].[ckbx_Text] ([TextID], [LanguageCode], [TextValue], [PageId]) VALUES (N'/pageMenu/survey_editor/UserLinks', N'en-US', N'User Links', NULL)
END

GO

IF NOT EXISTS (SELECT TOP 1 * FROM ckbx_Text WHERE TextID = N'/pageMenu/survey_editor/plusUserLinks')
BEGIN
	INSERT [dbo].[ckbx_Text] ([TextID], [LanguageCode], [TextValue], [PageId]) VALUES (N'/pageMenu/survey_editor/plusUserLinks', N'en-US', N'+User Links', NULL)
END

GO

IF NOT EXISTS (SELECT TOP 1 * FROM ckbx_Text WHERE TextID = N'/pageMenu/survey_editor/manageUserLinks')
BEGIN
	INSERT [dbo].[ckbx_Text] ([TextID], [LanguageCode], [TextValue], [PageId]) VALUES (N'/pageMenu/survey_editor/manageUserLinks', N'en-US', N'Manage Links', NULL)
END

GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_UserShareLinks]') AND type in (N'U'))
BEGIN
CREATE TABLE ckbx_UserShareLinks(
	   [Id] [int] IDENTITY(1,1) PRIMARY KEY,
	   [SurveyId] [int],
	   [UserName] NVARCHAR(100),
	   [DirectInvitation] uniqueidentifier,
	   [Url] NVARCHAR(1000),
	   [DateGenerated] DateTime NULL,
	   [DatePublished] DateTime NULL
) ON [PRIMARY]
END

GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_GenerateUserInvitation]') AND type in (N'P', N'PC'))
BEGIN
	DROP PROCEDURE ckbx_sp_GenerateUserInvitation
END

GO

CREATE PROCEDURE [ckbx_sp_GenerateUserInvitation]
(
	@SurveyId int,
	@UserName nvarchar(100),
	@DirectInvitation uniqueidentifier,
	@Url nvarchar(1000)
)
AS
	IF NOT EXISTS ( SELECT 1 FROM ckbx_UserShareLinks WHERE SurveyId = @SurveyId AND UserName = @UserName)
		BEGIN
				INSERT INTO ckbx_UserShareLinks
				(SurveyId, UserName, DirectInvitation, Url, DateGenerated, DatePublished)
				VALUES
				(@SurveyId, @UserName, @DirectInvitation, @Url, GETDATE(), Null) 
		END

GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Invitation_ListUsersForSurvey]') AND type in (N'P', N'PC'))
BEGIN
	DROP PROCEDURE ckbx_sp_Invitation_ListUsersForSurvey
END

GO 

CREATE PROCEDURE [dbo].[ckbx_sp_Invitation_ListUsersForSurvey]
(
	@ResponseTemplateId int,
    @PageNumber int,
    @ResultsPerPage int,
    @SortField varchar(64),
    @SortAscending bit
)
AS

	SET NOCOUNT ON

	--Create the order-by clause, if any
    DECLARE @OrderBy varchar(255);
    DECLARE @WhereCondition nvarchar(250);

	SET @WhereCondition = '';
    SET @OrderBy = ''

	IF len(@SortField) > 0
      BEGIN
        SET @OrderBy = ' ORDER BY ' + @SortField

		IF @SortAscending = 1
			BEGIN
				SET @OrderBy = @OrderBy + ' ASC'
			END
		ELSE
			BEGIN
				SET @OrderBy = @OrderBy + ' DESC'
			END
      END
      
	--If not paging, simply return the results
	DECLARE @Query nvarchar(1000)
	DECLARE @QueryParams nvarchar(500)
	
	SET @QueryParams = '@ResponseTemplateId int'
	
	--Otherwise, perform any necessary paging and return result set
	IF @PageNumber <= 0 OR @ResultsPerPage <= 0
		BEGIN
			SET @Query = '
				SELECT
					*					
				FROM
					ckbx_UserShareLinks
				WHERE
					(SurveyId = @ResponseTemplateId) '
			
			SET @Query = @Query + @WhereCondition + @OrderBy; 
			
			exec sp_executesql @Query, @QueryParams, @ResponseTemplateId = @ResponseTemplateId
			
			--Now select count
			SET @Query = 'SELECT COUNT(Id) AS TotalItemCount FROM ckbx_UserShareLinks WHERE SurveyId = @ResponseTemplateId '
			SET @Query = @Query + @WhereCondition
			
			exec sp_executesql @Query, @QueryParams, @ResponseTemplateId = @ResponseTemplateId
		END
	ELSE
		BEGIN
			--Put results in temp table, then select
			 --Make sure temp table does not exist
			DECLARE @StartRow int
			DECLARE @EndRow int			

			SET @StartRow = ((@PageNumber - 1) * @ResultsPerPage)  + 1
			SET @EndRow = @StartRow + @ResultsPerPage - 1  --i.e. Start = 1 and results per page = 10 means get rows 1-10

			IF object_id('tempdb..#orderedResults') IS NOT NULL
				BEGIN
					DROP TABLE #orderedResults
				END

			SELECT @Query = '


					CREATE TABLE #orderedResults
					(
						ItemIndex INT IDENTITY(1, 1),
						Id INT
					)

				INSERT INTO #orderedResults
				SELECT Id
				FROM
					ckbx_UserShareLinks
				WHERE
					(SurveyId = @ResponseTemplateId)
				' + @WhereCondition + @OrderBy + '
		
				--Select Rows
				SELECT i.* FROM ckbx_UserShareLinks i WHERE i.Id in (SELECT Id from #orderedResults r WHERE r.ItemIndex BETWEEN ' + CAST(@StartRow AS VARCHAR(6)) + ' AND ' + CAST(@EndRow AS VARCHAR(6)) + ');
				
				--Select count
				SELECT COUNT(Id) AS TotalItemCount FROM #orderedResults'
			exec sp_executesql @Query, @QueryParams, @ResponseTemplateId = @ResponseTemplateId
		END
		
GO


IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Invitation_GetDirectResponseTemplateGuid]') AND type in (N'P', N'PC'))
BEGIN
	DROP PROCEDURE ckbx_sp_Invitation_GetDirectResponseTemplateGuid
END

GO

CREATE PROCEDURE [dbo].[ckbx_sp_Invitation_GetDirectResponseTemplateGuid]
(
  @DirectInvitation uniqueidentifier
)
AS
  SELECT 
	rt.Guid
  FROM 
	ckbx_UserShareLinks usl
	INNER JOIN ckbx_ResponseTemplate rt ON rt.ResponseTemplateID = usl.SurveyId
	INNER JOIN ckbx_Template t ON t.TemplateID = rt.ResponseTemplateID
 WHERE 
	usl.[DirectInvitation] = @DirectInvitation 
	AND (t.Deleted IS NULL OR t.Deleted = 0)

GO


IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Invitation_GetUserFromDirectInvite]') AND type in (N'P', N'PC'))
BEGIN
	DROP PROCEDURE ckbx_sp_Invitation_GetUserFromDirectInvite
END

GO

CREATE PROCEDURE [dbo].[ckbx_sp_Invitation_GetUserFromDirectInvite]
(
  @DirectInvitation uniqueidentifier
)
AS
  SELECT UserName 
  FROM ckbx_UserShareLinks 
  WHERE DirectInvitation = @DirectInvitation

GO




