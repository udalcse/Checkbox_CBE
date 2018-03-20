
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Invitation_GetPendingRecipients]') AND type in (N'P', N'PC'))
DROP PROCEDURE [ckbx_sp_Invitation_GetPendingRecipients]
GO


IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Invitation_GetPendingRecipients]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [ckbx_sp_Invitation_GetPendingRecipients] AS' 
END
GO

ALTER PROCEDURE [ckbx_sp_Invitation_GetPendingRecipients]
(
	@InvitationID int,
	@BatchSize int
)
AS
BEGIN
	--temporary table for existing recipients 
    CREATE TABLE #existingRecipients
	(
		EmailAddress NVARCHAR(255) NULL		
	)

	IF @BatchSize IS NULL
	 BEGIN
		SET @BatchSize = 2147483647
	 END

	--fill the table
	INSERT INTO #existingRecipients 
	SELECT ir.EmailAddress  
	FROM ckbx_InvitationRecipients ir 
	INNER JOIN ckbx_Panel p ON p.PanelID = ir.PanelID
	WHERE ir.InvitationID = @InvitationID
	and ir.[UniqueIdentifier] <> 'Test recipient ' + ir.EmailAddress

	--temporary table for pending recipients 
	CREATE TABLE #pendingRecipients
	(
		PanelID int not null,		
		PanelTypeID int not null,		
		EmailAddress NVARCHAR(255) NULL,
		[UniqueIdentifier] NVARCHAR(611) NULL
	)

	DECLARE @PanelID int, @PanelTypeID int
	DECLARE @RemainRecipientsCount int

	--set init rest value
	SET @RemainRecipientsCount = @BatchSize

	--create a cursor
	DECLARE panel_cursor CURSOR LOCAL FOR
	SELECT p.PanelID, p.PanelTypeID FROM ckbx_InvitationPanels ip 
	INNER JOIN ckbx_Panel p ON p.PanelID = ip.PanelID
	WHERE ip.InvitationID = @InvitationID

	OPEN panel_cursor

	FETCH NEXT FROM panel_cursor
	INTO @PanelID, @PanelTypeID

	WHILE @@FETCH_STATUS = 0
		BEGIN
			--user panel type
			IF @PanelTypeID = 1
				BEGIN
					INSERT INTO #pendingRecipients
					SELECT TOP (@RemainRecipientsCount) @PanelID, @PanelTypeID, up.Email, MAX([UniqueIdentifier])
					FROM ckbx_UserPanel up INNER JOIN ckbx_Credential c
					ON up.UserIdentifier = c.[UniqueIdentifier] 
					WHERE up.PanelID = @PanelID AND c.Email COLLATE Latin1_General_CS_AS_KS_WS NOT IN
					(SELECT EmailAddress COLLATE Latin1_General_CS_AS_KS_WS FROM #existingRecipients) 
					AND c.Email COLLATE Latin1_General_CS_AS_KS_WS NOT IN
					(SELECT EmailAddress COLLATE Latin1_General_CS_AS_KS_WS FROM #pendingRecipients) 
					GROUP BY up.Email
				END
			--group panel type
			ELSE IF @PanelTypeID = 2
				BEGIN
					INSERT INTO #pendingRecipients
					SELECT TOP (@RemainRecipientsCount) @PanelID, @PanelTypeID, c.Email, max(c.[UniqueIdentifier])
					FROM ckbx_GroupPanel gp INNER JOIN ckbx_GroupMembers gm
					ON gp.GroupID = gm.GroupID INNER JOIN ckbx_Credential c 
					ON gm.MemberUniqueIdentifier = c.[UniqueIdentifier]
					WHERE gp.PanelID = @PanelID AND c.Email COLLATE Latin1_General_CS_AS_KS_WS NOT IN
					(SELECT EmailAddress COLLATE Latin1_General_CS_AS_KS_WS FROM #existingRecipients) 
					AND c.Email COLLATE Latin1_General_CS_AS_KS_WS NOT IN
					(SELECT EmailAddress COLLATE Latin1_General_CS_AS_KS_WS FROM #pendingRecipients) 
					GROUP BY c.Email
				END
			--email list panel type
			ELSE IF @PanelTypeID = 3
				BEGIN
					INSERT INTO #pendingRecipients
					SELECT TOP (@RemainRecipientsCount) @PanelID, @PanelTypeID, elp.EmailAddress, ''
					FROM ckbx_EmailListPanel elp 
					WHERE elp.PanelID = @PanelID AND elp.EmailAddress COLLATE Latin1_General_CS_AS_KS_WS NOT IN
					(SELECT EmailAddress COLLATE Latin1_General_CS_AS_KS_WS FROM #existingRecipients) 
					AND elp.EmailAddress COLLATE Latin1_General_CS_AS_KS_WS NOT IN
					(SELECT EmailAddress COLLATE Latin1_General_CS_AS_KS_WS FROM #pendingRecipients) 
					GROUP BY elp.EmailAddress
				END
			--ad hoc email list panel type
			ELSE IF @PanelTypeID = 4
				BEGIN
					INSERT INTO #pendingRecipients
					SELECT TOP (@RemainRecipientsCount) @PanelID, @PanelTypeID, ahep.EmailAddress, ''
					FROM ckbx_AdHocEmailPanel ahep 
					WHERE ahep.PanelID = @PanelID AND ahep.EmailAddress  COLLATE Latin1_General_CS_AS_KS_WS NOT IN
					(SELECT EmailAddress COLLATE Latin1_General_CS_AS_KS_WS FROM #existingRecipients) 
					AND ahep.EmailAddress COLLATE Latin1_General_CS_AS_KS_WS NOT IN
					(SELECT EmailAddress COLLATE Latin1_General_CS_AS_KS_WS FROM #pendingRecipients) 
					GROUP BY ahep.EmailAddress
					
					
				END
			
			DECLARE @PendingRecipientsCount int
			SET @PendingRecipientsCount = (SELECT COUNT(EmailAddress) FROM #pendingRecipients)

			--end query or update remaining rest of batch
			IF @PendingRecipientsCount >= @BatchSize
				BREAK
			ELSE 
				SET	@RemainRecipientsCount = @BatchSize - @PendingRecipientsCount

			FETCH NEXT FROM panel_cursor
			INTO @PanelID, @PanelTypeID
		END

		SELECT TOP (@BatchSize) * FROM #pendingRecipients

		DROP TABLE #pendingRecipients
		DROP TABLE #existingRecipients

		CLOSE panel_cursor
		DEALLOCATE panel_cursor

END

GO


