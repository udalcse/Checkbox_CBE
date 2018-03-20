GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_GetOnlyTenantUsers]') AND type in (N'P', N'PC'))
BEGIN
	DROP PROCEDURE ckbx_sp_GetOnlyTenantUsers
END

GO

CREATE PROCEDURE [dbo].[ckbx_sp_GetOnlyTenantUsers]
(
  
  @PageIndex int,
  @PageSize int
)
AS

	DECLARE @Query NVARCHAR(max)
	DECLARE @QueryParams NVARCHAR(500)
	DECLARE @OrderBy varchar(64)
	DECLARE @CountQuery NVARCHAR(max)

	SET @OrderBy = 'ORDER BY ckbx_Credential.UniqueIdentifier ASC'

	SET @CountQuery = 'SELECT COUNT(distinct ckbx_Credential.UniqueIdentifier) AS TotalRecords FROM ckbx_Credential 
						left outer join ckbx_CustomUserFieldMap cufm on ckbx_Credential.[UniqueIdentifier] = cufm.[UniqueIdentifier]
						left outer join ckbx_CustomUserField cuf on cufm.CustomUserFieldID = cuf.CustomUserFieldID
						where cuf.CustomUserFieldName = ''TenantId'' and cufm.Value is not null'

	IF @PageIndex <= 0 OR @PageSize <=0
		BEGIN
			DECLARE @BaseQuery NVARCHAR(max)
		
			SET @BaseQuery = '
				SELECT distinct
					ckbx_Credential.UserName,
					ckbx_Credential.[Password],
					ckbx_Credential.Domain,
					ckbx_Credential.[UniqueIdentifier],
					ckbx_Credential.[GUID],
					ckbx_Credential.Encrypted,
					ckbx_Credential.Email,
					ckbx_Credential.Created,
					ckbx_Credential.LastActivity,
					ckbx_Credential.LastLogin,
					ckbx_Credential.LastPasswordChange,
					ckbx_Credential.LastLockedOut,
					ckbx_Credential.FailedLogins,
					ckbx_Credential.LockedOut
					
				FROM
					ckbx_Credential 
						left outer join ckbx_CustomUserFieldMap cufm on ckbx_Credential.[UniqueIdentifier] = cufm.[UniqueIdentifier]
						left outer join ckbx_CustomUserField cuf on cufm.CustomUserFieldID = cuf.CustomUserFieldID
						where cuf.CustomUserFieldName = ''TenantId'' and cufm.Value is not null'
			
					
			SET @Query = @BaseQuery
			
			print @Query	
			--Results
			exec sp_executesql @Query

			exec sp_executesql @CountQuery
		END
	ELSE
		BEGIN
			DECLARE @PageQuery NVARCHAR(max)
			
			DECLARE @RowStart int
			DECLARE @RowEnd int
			
			SET @RowStart = @PageSize * (@PageIndex - 1) + 1;
			SET @RowEnd = @RowStart + @PageSize - 1;
			
			DECLARE @SelectQuery NVARCHAR(max)
			SET @SelectQuery = 
			' 
			With CredentialTemp AS
				( 
				select ckbx_Credential.*, ROW_NUMBER() 
 						OVER 
						(' + @OrderBy + ') AS RowNumber 
				from (
					SELECT distinct
						ckbx_Credential.UserName,
						ckbx_Credential.[Password],
						ckbx_Credential.Domain,
						ckbx_Credential.[UniqueIdentifier],
						ckbx_Credential.[GUID],
						ckbx_Credential.Encrypted,
						ckbx_Credential.Email,
						ckbx_Credential.Created,
						ckbx_Credential.LastActivity,
						ckbx_Credential.LastLogin,
						ckbx_Credential.LastPasswordChange,
						ckbx_Credential.LastLockedOut,
						ckbx_Credential.FailedLogins,
						ckbx_Credential.LockedOut
					FROM 
						ckbx_Credential 
							left outer join ckbx_CustomUserFieldMap cufm on ckbx_Credential.[UniqueIdentifier] = cufm.[UniqueIdentifier]
							left outer join ckbx_CustomUserField cuf on cufm.CustomUserFieldID = cuf.CustomUserFieldID
							where cuf.CustomUserFieldName = ''TenantId'' and cufm.Value is not null
							'	
			
			SET @PageQuery =	
				' SELECT 
					*
				FROM 
					CredentialTemp
				WHERE 
					RowNumber BETWEEN ' + CAST(@RowStart AS VARCHAR(10)) + ' AND ' + CAST(@RowEnd AS VARCHAR(10))

			SET @Query = @SelectQuery + ' ) ckbx_Credential )' + @PageQuery
			
			PRINT @Query
			--Results
			exec sp_executesql @Query

			exec sp_executesql @CountQuery
	END

GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_GetUserShareLink]') AND type in (N'P', N'PC'))
BEGIN
	DROP PROCEDURE ckbx_sp_GetUserShareLink
END

GO

CREATE PROCEDURE [dbo].[ckbx_sp_GetUserShareLink]
(
	@SurveyId int,
	@UserName nvarchar(100)
)
AS
	SELECT Url FROM ckbx_UserShareLinks WHERE SurveyId = @SurveyId AND UserName = @UserName;
	RETURN
GO
