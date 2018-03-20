EXEC ckbx_sp_Text_Set N'/pageText/forms/surveys/responses/manage.aspx/exportToPdf', 'en-US', N'Export Selected to PDF'

EXEC ckbx_sp_Text_Set N'/pageText/forms/surveys/responses/manage.aspx/respondentFilter', 'en-US', N'Respondent Filter'

EXEC ckbx_sp_Text_Set N'/pageText/forms/surveys/responses/manage.aspx/respondentFilterEqual', 'en-US', N'Equals'

EXEC ckbx_sp_Text_Set N'/pageText/forms/surveys/responses/manage.aspx/clearRespondentFilter', 'en-US', N'Clear Respondent Filter'

GO

ALTER PROCEDURE [dbo].[ckbx_sp_Response_List]
(
	@ResponseTemplateId int,
	@IncludeComplete bit,
	@IncludeIncomplete bit,
	@IncludeTest bit,
    @MinResponseCompletedDate DATETIME,
	@MaxResponseCompletedDate DATETIME,
	@PageNumber int, 
	@ResultsPerPage int,
	@SortField varchar(64),
	@SortAscending bit,
	@FilterField varchar(64),
	@FilterValue nvarchar(255),   
    @DateFieldName nvarchar(255) = null,
	@ProfileFieldId int = 0
)
AS
BEGIN
  DECLARE @OrderBy varchar(64)
  
  IF (@SortField is NOT NULL AND len(@SortField) > 0)
	BEGIN
		IF @SortAscending = 1
			BEGIN
				SET @OrderBy = ' ASC'
			END
		ELSE
			BEGIN
				SET @OrderBy = ' DESC'
			END
	END
	
  DECLARE @Query nvarchar(4000)
  DECLARE @WhereClause nvarchar(1000)
  DECLARE @OrderByClause nvarchar(1000)
  DECLARE @QueryParams nvarchar(500)
  
  SET @QueryParams = '@ResponseTemplateId int, @FilterValue nvarchar(255), @MinResponseCompletedDate datetime, @MaxResponseCompletedDate datetime'
  
  --create the WHERE clause  
  SET @WhereClause = ' (Deleted IS NULL OR Deleted = 0) '
  IF @ResponseTemplateID IS NOT NULL AND @ResponseTemplateID >= 1000
  BEGIN
    SET @WhereClause = @WhereClause + ' AND ckbx_Response.ResponseTemplateID = @ResponseTemplateId '
  END

  IF @IncludeIncomplete = 0 AND @IncludeComplete = 0
	RETURN

  IF @IncludeIncomplete = 0
  BEGIN
    SET @WhereClause = @WhereClause + ' AND IsComplete = 1 '
  END
  ELSE IF @IncludeComplete = 0
  BEGIN
    SET @WhereClause = @WhereClause + ' AND IsComplete = 0 '
  END

  IF @IncludeTest = 0
  BEGIN
    SET @WhereClause = @WhereClause + ' AND (IsTest = 0 OR IsTest = NULL) '
  END

  IF (@FilterValue IS NOT NULL AND len(@FilterValue) > 0)
  BEGIN
    IF (@FilterField = 'UniqueIdentifier')
    BEGIN
      SET @WhereClause = @WhereClause + ' AND [UniqueIdentifier] = ''' + @FilterValue +  ''''	
    END

    IF (@FilterField = 'Invitee')
    BEGIN
      SET @WhereClause = @WhereClause + ' AND [Invitee] like ''%'' + @FilterValue + ''%'' '	
    END

    IF (@FilterField = 'ResponseID')
    BEGIN
      SET @WhereClause = @WhereClause + ' AND CAST(ResponseID AS NVARCHAR) = @FilterValue '	
    END	

    IF (@FilterField = 'Guid')
    BEGIN
      SET @WhereClause = @WhereClause + ' AND Replace ([GUID], ''-'','''') like ''%'' + cast(Replace(@FilterValue, ''-'','''') as NVARCHAR) + ''%'' '   
    END

    IF (@FilterField = 'RespondentGuid')
    BEGIN
      SET @WhereClause = @WhereClause + ' AND [RespondentGuid] = @FilterValue'   
    END

    IF (@FilterField = 'ResumeKey')
    BEGIN
      SET @WhereClause = @WhereClause + ' AND [ResumeKey] like ''%'' + @FilterValue + ''%'' '	
    END

	IF (@FilterField = 'StartDate')
	BEGIN
	  SET @WhereClause = @WhereClause + ' AND [Started] > @FilterValue '
	END

	IF (@FilterField = 'ProfileField')
	BEGIN
	  SET @WhereClause = @WhereClause + ' AND [UniqueIdentifier] in (SELECT [UniqueIdentifier] from [ckbx_CustomUserFieldMap] where [CustomUserFieldID] = ' + CONVERT(nvarchar, @ProfileFieldId) + ' and [Value] = @FilterValue)'
	END
  END
		PRint 	@WhereClause
  IF (@MinResponseCompletedDate IS NOT NULL)
	  BEGIN
		IF @ResponseTemplateID >= 1000
			BEGIN 
				SET @WhereClause = @WhereClause + ' AND (ended IS NULL OR (@MinResponseCompletedDate <= ended)) ';
			END
		ELSE
			BEGIN
				SET @WhereClause = @WhereClause + ' AND (ended IS NOT NULL AND (@MinResponseCompletedDate <= ended)) ';
			END
	  END
  
  IF (@MaxResponseCompletedDate IS NOT NULL)
	  BEGIN
		IF @ResponseTemplateID >= 1000
			BEGIN
				SET @WhereClause = @WhereClause + ' AND (ended IS NULL OR (@MaxResponseCompletedDate >= ended)) ';
			END
		ELSE
			BEGIN
				SET @WhereClause = @WhereClause + ' AND (ended IS NOT NULL AND (@MaxResponseCompletedDate >= ended)) ';
			END
	  END
  
  --build the ORDER BY clause
  IF (@SortField is NOT NULL AND len(@SortField) > 0)
  BEGIN
    SET @OrderByClause = ' ORDER BY ['+@SortField+'] ' + @OrderBy
  END
  ELSE
  BEGIN
	SET @OrderByClause = ''
  END 
  
  --If not paging, simply return the results
  IF @PageNumber <= 0 OR @ResultsPerPage <= 0
  BEGIN
	--select results
    SET @Query = '
      SELECT ckbx_response.*, rt.TemplateName FROM ckbx_Response 
		LEFT JOIN ckbx_ResponseTemplate rt ON ckbx_Response.ResponseTemplateID = rt.ResponseTemplateID 
		WHERE ' + @WhereClause + ' ' + @OrderByClause    
    exec sp_executesql @Query, @QueryParams, @ResponseTemplateId = @ResponseTemplateId, @FilterValue = @FilterValue, @MinResponseCompletedDate = @MinResponseCompletedDate, @MaxResponseCompletedDate = @MaxResponseCompletedDate					
  END
  ELSE
  BEGIN
	DECLARE @PageLowerBound int
	DECLARE @PageUpperBound int
	
	-- Set the page bounds
	SET @PageLowerBound = @ResultsPerPage * (@PageNumber - 1) + 1
	SET @PageUpperBound = @PageLowerBound + @ResultsPerPage - 1

    CREATE TABLE #indexedResults
    (
       [Index] int,
       [ResponseID] bigint 
    )
    
	/*
    SET @Query = 'INSERT INTO #indexedResults (ResponseID) SELECT TOP ' 
        + CONVERT(nvarchar, @PageUpperBound)
        + 'ResponseID FROM ckbx_Response WHERE ' + @WhereClause + ' ' + @OrderByClause
        */
    SET @Query = 'INSERT INTO #indexedResults ([Index], ResponseID) 
		SELECT * FROM (SELECT ROW_NUMBER() OVER (' + @OrderByClause + ') as RN, ResponseID FROM ckbx_Response
		WHERE ' + @WhereClause + ') a WHERE a.RN BETWEEN ' 
		+ CAST (@PageLowerBound as nvarchar) + ' AND ' + CAST (@PageUpperBound as nvarchar)

    exec sp_executesql @Query, @QueryParams, @ResponseTemplateId = @ResponseTemplateId, @FilterValue = @FilterValue, @MinResponseCompletedDate = @MinResponseCompletedDate, @MaxResponseCompletedDate = @MaxResponseCompletedDate					
    
    SELECT ckbx_Response.*, RT.TemplateName FROM ckbx_Response 
    LEFT JOIN ckbx_ResponseTemplate RT ON ckbx_Response.ResponseTemplateID = RT.ResponseTemplateID,
		#indexedResults I
	WHERE ckbx_Response.ResponseID = I.ResponseID 
	ORDER BY [INDEX]
  END			
  
  --select total count				
  SET @Query = '
    SELECT COUNT(ResponseID) AS TotalItemCount FROM ckbx_Response  
      WHERE ' + @WhereClause
  exec sp_executesql @Query, @QueryParams, @ResponseTemplateId = @ResponseTemplateId, @FilterValue = @FilterValue, @MinResponseCompletedDate = @MinResponseCompletedDate, @MaxResponseCompletedDate = @MaxResponseCompletedDate				
END
	
GO
