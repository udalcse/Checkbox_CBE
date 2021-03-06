
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Timeline_Collect_SurveyManager]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
ALTER PROCEDURE [ckbx_sp_Timeline_Collect_SurveyManager]
(
  @RequestID bigint
)
AS
begin
	begin try
		declare @Creator as nvarchar(611)
		declare @Manager as nvarchar(100)
		declare @StartDate as datetime
		declare @RoleID as int
		
		set @StartDate = getdate() - 30 --collect the event data for the last month in worst case
		
		select @Creator = Creator, @Manager = Manager from ckbx_Timeline_Request where RequestID = @RequestID
		
		--search for the date from which the events will be collected
		declare @LastEventDate as datetime

		select @LastEventDate = max(Occured) from ckbx_Timeline_Result
			where lower(Creator) = lower(@Creator) and @Manager = Manager
		
		if @LastEventDate is not null and @LastEventDate > @StartDate
			set @StartDate = @LastEventDate

		--check if the user is a System Admin
		select @RoleID = ir.RoleID from ckbx_IdentityRoles ir
			inner join ckbx_Role r on ir.RoleID = r.RoleID
			where ir.uniqueidentifier = @Creator and r.RoleName = ''System Administrator''
		
		--structure is the same as for ckbx_ResponseTemplate table
		create table #Surveys 
		(
			[ResponseTemplateID] [int] NOT NULL,
			[CreatedBy] [nvarchar](611) NULL,
			[TemplateName] [nvarchar](611) NULL,
			[NameTextID] [varchar](611) NULL,
			[TitleTextID] [varchar](611) NULL,
			[DescriptionTextID] [varchar](611) NULL,
			[IsActive] [bit] NULL,
			[ActivationStart] [datetime] NULL,
			[ActivationEnd] [datetime] NULL,
			[MaxTotalResponses] [int] NULL,
			[MaxResponsesPerUser] [int] NULL,
			[AllowContinue] [bit] NULL,
			[AllowEdit] [bit] NULL,
			[DisableBackButton] [bit] NULL,
			[StyleTemplateID] [int] NULL,
			[ShowPageNumbers] [bit] NULL,
			[ShowProgressBar] [bit] NULL,
			[ShowItemNumbers] [bit] NULL,
			[ShowTitle] [bit] NULL,
			[RandomizeItemsInPages] [bit] NULL,
			[MobileCompatible] [bit] NULL,
			[ButtonContinueTextID] [nvarchar](611) NULL,
			[ButtonBackTextID] [nvarchar](611) NULL,
			[CompletionType] [int] NULL,
			[SupportedLanguages] [nvarchar](1023) NULL,
			[DefaultLanguage] [nvarchar](5) NULL,
			[LanguageSourceToken] [nvarchar](100) NULL,
			[SecurityType] [int] NULL,
			[GuestPassword] [nvarchar](611) NULL,
			[LoginUrl] [nvarchar](500) NULL,
			[ReportSecurityType] [int] NULL,
			[EnableScoring] [bit] NULL,
			[GUID] [uniqueidentifier] NULL,
			[ShowValidationMessage] [bit] NULL,
			[RequiredFieldsAlert] [bit] NULL,
			[LanguageSource] [nvarchar](25) NULL,
			[EnableDynamicPageNumbers] [bit] NULL,
			[EnableDynamicItemNumbers] [bit] NULL,
			[ShowSaveAndQuit] [bit] NULL,
			[AllowSurveyEditWhileActive] [bit] NULL,
			[IsPoll] [bit] NULL,
			[ChartStyleID] [int] NULL,
			[Height] [int] NULL,
			[Width] [int] NULL,
			[BorderWidth] [int] NULL,
			[BorderColor] [varchar](63) NULL,
			[BorderStyle] [varchar](63) NULL,
			[AnonymizeResponses] [bit] NULL,
			[TabletStyleTemplateID] [int] NULL,
			[SmartPhoneStyleTemplateID] [int] NULL,
			[AllowFormReset] [bit] NULL,
			[ShowAsterisks] [bit] NULL,
			[HideFooterHeader] [bit] NULL,
			[MobileStyleId] [int] NULL,
			[ProgressBarOrientation] [int] NULL,
			[GoogleAnalyticsTrackingID] [varchar](32) NULL,
			[ShowTopSurveyButtons] [bit] NULL,
			[HideTopSurveyButtonsOnFirstAndLastPage] [bit] NULL
		)
			
		--get all available surveys
		if @RoleID is not null
			insert into #Surveys exec ckbx_sp_Security_ListAccessibleSurveysAdmin 
				@PageNumber=-1,
				@ResultsPerPage=-1,
				@SortField=N''ItemName'',
				@SortAscending=1,
				@FilterField=N''Name'',
				@FilterValue=N'''', 
				@AncestorFolder=null,
				@DisplayCount = 0
		
		declare @EventID int
		
		
		if @RoleID is not null or exists(select top 1 * from ckbx_IdentityRoles ir 
			inner join ckbx_RolePermissions rp on ir.RoleID = rp.RoleID
			inner join ckbx_Permission p on rp.PermissionID = p.PermissionID
			where ir.[UniqueIdentifier] = @Creator and PermissionName = ''Form.Administer''
		)
		begin	
			if @RoleID is null
			begin
				delete from #Surveys 
				insert into #Surveys exec ckbx_sp_Security_ListAccessibleSurveys @UniqueIdentifier=@Creator, @FirstPermissionName=''Form.Administer'', 
				@SecondPermissionName=null, 
				@RequireBothPermissions=0,
				@UseAclExclusion=1,	
				@PageNumber=-1,@ResultsPerPage=-1,@SortField=N''ItemName'',@SortAscending=1,@FilterField=N''Name'',@FilterValue=N'''',@AncestorFolder=null,
				@DisplayCount = 0
			end
			
			set @EventID = null
			select @EventID = EventID from ckbx_Timeline_Events where 
				EventName = ''SURVEY_CREATED''		
			
			--collect all recently created surveys
			if @EventID is not null
			begin
				insert into ckbx_Timeline_Result (RequestID, EventID, Occured, UserID, ObjectID, ObjectGUID, Manager, Creator, ObjectParentID, ObjectParentType)		
					select @RequestID, @EventID, t.CreatedDate, t.CreatedBy, t.TemplateID, s.[GUID], @Manager, @Creator, t.TemplateID, ''SURVEY'' from #Surveys s
						inner join ckbx_Template t on s.ResponseTemplateID = t.TemplateID
					where t.CreatedDate > @StartDate and (t.Deleted is null or t.Deleted = 0)
			end

			--collect all recently modified surveys
			set @EventID = null
			select @EventID = EventID from ckbx_Timeline_Events where 
				EventName = ''SURVEY_EDITED''		
			if @EventID is not null
			begin			
				insert into ckbx_Timeline_Result (RequestID, EventID, Occured, UserID, ObjectID, ObjectGUID, Manager, Creator, ObjectParentID, ObjectParentType)		
					select @RequestID, @EventID, t.ModifiedDate, t.ModifiedBy, t.TemplateID, s.[GUID], @Manager, @Creator, t.TemplateID, ''SURVEY'' from #Surveys s
						inner join ckbx_Template t on s.ResponseTemplateID = t.TemplateID
					where t.ModifiedDate > @StartDate and (t.Deleted is null or t.Deleted = 0)
			end		
			
			--collect all recently created invitations
			set @EventID = null
			select @EventID = EventID from ckbx_Timeline_Events where 
				EventName = ''INVITATION_CREATED''		
			if @EventID is not null
			begin			
				insert into ckbx_Timeline_Result (RequestID, EventID, Occured, UserID, ObjectID, ObjectGUID, ObjectParentID, Manager, Creator, ObjectParentType)		
					select @RequestID, @EventID, i.DateCreated, i.CreatedBy, i.InvitationID, i.[GUID], s.[ResponseTemplateID], @Manager, @Creator, ''SURVEY'' from #Surveys s
						inner join ckbx_Invitation i on s.ResponseTemplateID = i.ResponseTemplateID
					where i.DateCreated > @StartDate
			end		
			
			--collect all recently created invitations
			set @EventID = null
			select @EventID = EventID from ckbx_Timeline_Events where 
				EventName = ''INVITATION_SENT''		
			if @EventID is not null
			begin			
				insert into ckbx_Timeline_Result (RequestID, EventID, Occured, UserID, ObjectID, ObjectGUID, ObjectParentID, Manager, Creator, ObjectParentType)		
					select @RequestID, @EventID, i.LastSentOn, i.CreatedBy, i.InvitationID, i.[GUID], s.[ResponseTemplateID], @Manager, @Creator, ''SURVEY'' from #Surveys s
						inner join ckbx_Invitation i on s.ResponseTemplateID = i.ResponseTemplateID
					where i.LastSentOn > @StartDate
			end		

		end
		
		if @RoleID is not null or exists(select top 1 * from ckbx_IdentityRoles ir 
			inner join ckbx_RolePermissions rp on ir.RoleID = rp.RoleID
			inner join ckbx_Permission p on rp.PermissionID = p.PermissionID
			where ir.[UniqueIdentifier] = @Creator and PermissionName = ''Form.Edit''
		)
		begin	
			if @RoleID is null
			begin
				delete from #Surveys 
				insert into #Surveys exec ckbx_sp_Security_ListAccessibleSurveys @UniqueIdentifier=@Creator, @FirstPermissionName=''Form.Edit'', 
				@SecondPermissionName=null, 
				@RequireBothPermissions=0,
				@UseAclExclusion=1,	
				@PageNumber=-1,@ResultsPerPage=-1,@SortField=N''ItemName'',@SortAscending=1,@FilterField=N''Name'',@FilterValue=N'''',@AncestorFolder=null,
				@DisplayCount = 0
			end
			
			--collect all recently created items 
			set @EventID = null
			select @EventID = EventID from ckbx_Timeline_Events where 
				EventName = ''ITEM_CREATED''		
			if @EventID is not null
			begin
				insert into ckbx_Timeline_Result (RequestID, EventID, Occured, UserID, ObjectID, ObjectGUID, ObjectParentID, Manager, Creator, ObjectParentType)		
					select @RequestID, @EventID, i.CreatedDate, i.CreatedBy, i.ItemID, null, s.[ResponseTemplateID], @Manager, @Creator, ''SURVEY'' from #Surveys s
						inner join ckbx_Template_Items ti on s.ResponseTemplateID = ti.TemplateID
						inner join ckbx_Item i on ti.ItemID = i.ItemID
					where i.CreatedDate > @StartDate and (i.Deleted is null or i.Deleted = 0)
			end		
			
			--collect all recently edited items
			set @EventID = null
			select @EventID = EventID from ckbx_Timeline_Events where 
				EventName = ''ITEM_EDITED''		
			if @EventID is not null
			begin
				insert into ckbx_Timeline_Result (RequestID, EventID, Occured, UserID, ObjectID, ObjectGUID, ObjectParentID, Manager, Creator, ObjectParentType)		
					select @RequestID, @EventID, i.ModifiedDate, i.ModifiedBy, i.ItemID, null, s.[ResponseTemplateID], @Manager, @Creator, ''SURVEY'' from #Surveys s
						inner join ckbx_Template_Items ti on s.ResponseTemplateID = ti.TemplateID
						inner join ckbx_Item i on ti.ItemID = i.ItemID
					where i.ModifiedDate > @StartDate and (i.Deleted is null or i.Deleted = 0)
			end	
			
			--collect all recently opt outed users			
			set @EventID = null
			select @EventID = EventID from ckbx_Timeline_Events where 
				EventName = ''INVITEE_OPT_OUT''		
			if @EventID is not null
			begin			
				insert into ckbx_Timeline_Result (RequestID, EventID, Occured, UserID, ObjectID, ObjectGUID, ObjectParentID, Manager, Creator, ObjectParentType)		
					select @RequestID, @EventID, opt.DateOccur, opt.EmailAddress, opt.OptedOutEmailID, s.[GUID], s.ResponseTemplateID, @Manager, @Creator, ''SURVEY'' from #Surveys s
						inner join ckbx_Invitation_OptedOutEmails opt on s.ResponseTemplateID = opt.ResponseTemplateId
					where opt.DateOccur > @StartDate
			end		
		end	
		else
		begin
			--user''s role can not Form.Edit
			if @RoleID is null
			begin
				delete from #Surveys 
				insert into #Surveys exec ckbx_sp_Security_ListAccessibleSurveys @UniqueIdentifier=@Creator, @FirstPermissionName=''Analysis.Create'', 
				@SecondPermissionName=''Form.View'', 
				@RequireBothPermissions=0,
				@UseAclExclusion=1,	
				@PageNumber=-1,@ResultsPerPage=-1,@SortField=N''ItemName'',@SortAscending=1,@FilterField=N''Name'',@FilterValue=N'''',@AncestorFolder=null,
				@DisplayCount = 0
			end
		end
		
		CREATE TABLE #Reports(
			[AnalysisTemplateID] [int] NOT NULL,
			[StyleTemplateID] [int] NULL,
			[ResponseTemplateID] [int] NULL,
			[AnalysisName] [nvarchar](255) NULL,
			[NameTextID] [nvarchar](255) NULL,
			[GUID] [uniqueidentifier] NULL,
			[DateFilterStart] [datetime] NULL,
			[DateFilterEnd] [datetime] NULL,
			[ChartStyleID] [int] NULL,
			[DisplaySurveyTitle] [bit] NOT NULL,
			[DisplayPdfExportButton] [bit] NOT NULL,
			[IncludeIncompleteResponses] [bit] NULL,
			[IncludeTestResponses] [bit] NULL,
			[TemplateID] [int] NOT NULL,
			[ModifiedDate] [datetime] NULL,
			[Deleted] [bit] NULL,
			[DefaultPolicy] [int] NULL,
			[AclID] [int] NULL,
			[CreatedDate] [datetime] NULL,
			[CreatedBy] [nvarchar](611) NULL,
			[ModifiedBy] [nvarchar](611) NULL)

		insert into #Reports
			exec ckbx_sp_Security_ListAccessibleAnalyses @UniqueIdentifier=@Creator,
				@FirstPermissionName=N''Analysis.Run'',@SecondPermissionName=N''Analysis.Edit'',@RequireBothPermissions=0,@UseAclExclusion=1,
				@PageNumber=0,@ResultsPerPage=0,@SortField=N''AnalysisName'',@SortAscending=1,@FilterField=N'''',@FilterValue=null, @DisplayCount = 0

		--collect all recently created reports
		set @EventID = null
		select @EventID = EventID from ckbx_Timeline_Events where 
			EventName = ''REPORT_CREATED''		
		if @EventID is not null
		begin			
			insert into ckbx_Timeline_Result (RequestID, EventID, Occured, UserID, ObjectID, ObjectGUID, Manager, Creator, ObjectParentID, ObjectParentType)		
				select distinct @RequestID, @EventID, CreatedDate, CreatedBy, TemplateID, [GUID], @Manager, @Creator, ResponseTemplateID, ''SURVEY'' from 
					#Reports
				where CreatedDate > @StartDate and (Deleted is null or Deleted = 0)
		end		
		

		--collect all recently modified reports
		set @EventID = null
		select @EventID = EventID from ckbx_Timeline_Events where 
			EventName = ''REPORT_EDITED''		
		if @EventID is not null
		begin			
			insert into ckbx_Timeline_Result (RequestID, EventID, Occured, UserID, ObjectID, ObjectGUID, Manager, Creator, ObjectParentID, ObjectParentType)		
				select distinct @RequestID, @EventID, ModifiedDate, CreatedBy, TemplateID, [GUID], @Manager, @Creator, ResponseTemplateID, ''SURVEY'' from 
					#Reports
				where ModifiedDate > @StartDate and (Deleted is null or Deleted = 0)
		end		
		

		if @RoleID is not null or exists(select top 1 * from ckbx_IdentityRoles ir 
			inner join ckbx_RolePermissions rp on ir.RoleID = rp.RoleID
			inner join ckbx_Permission p on rp.PermissionID = p.PermissionID
			where ir.[UniqueIdentifier] = @Creator and PermissionName = ''Analysis.Responses.Edit''
		)
		begin
			if @RoleID is null	
			begin
				--remove surveys which doesn''t have Analysis.Responses.View in ACL for the given user
				delete from #Surveys where 
					ResponseTemplateID not in (
					  SELECT t.TemplateID
					  FROM	ckbx_Template t 
						INNER JOIN ckbx_AccessControlEntries ace ON ace.AclId = t.AclId
						INNER JOIN ckbx_AccessControlEntry ent ON ent.EntryId = ace.EntryId
						INNER JOIN ckbx_PolicyPermissions pp ON pp.PolicyId = ent.PolicyId
						INNER JOIN ckbx_Permission p ON p.PermissionId = pp.PermissionId
					  WHERE
						ent.EntryIdentifier = @Creator
						AND p.PermissionName = ''Analysis.Responses.View''
					)
			end
		
			--collect all recently created responses
			set @EventID = null
			select @EventID = EventID from ckbx_Timeline_Events where 
				EventName = ''RESPONSE_CREATED''		
			if @EventID is not null
			begin
				-- select immediate event
				insert into ckbx_Timeline_Result (RequestID, EventID, Occured, UserID, ObjectID, ObjectGUID, ObjectParentID, Manager, Creator, ObjectParentType)		
					select top 1 @RequestID, @EventID, r.Ended, r.[UniqueIdentifier], r.ResponseID, r.[GUID], s.[ResponseTemplateID], @Manager, @Creator, ''SURVEY''  from #Surveys s
						inner join ckbx_Response r on s.ResponseTemplateID = r.ResponseTemplateID					
					where r.Ended > @StartDate and (r.Deleted is null or r.Deleted = 0)
					order by r.Ended desc
				
				--cleanup previously collected results
				delete from ckbx_Timeline_Result
					where Manager = @Manager and Creator = @Creator and EventID = @EventID and PeriodID > 1

				-- select daily event
				insert into ckbx_Timeline_Result (RequestID, EventID, Occured, UserID, ObjectID, ObjectGUID, ObjectParentID, Manager, Creator, ObjectParentType, EventCount, PeriodID)		
					select @RequestID, @EventID, coalesce(min(r.Ended), getdate()), null, null, null, r.ResponseTemplateID, @Manager, @Creator, ''SURVEY'', count(*) as EventCount, 2 as PeriodID  from #Surveys s
						inner join ckbx_Response r on s.ResponseTemplateID = r.ResponseTemplateID					
					where DATEADD(day, 1, r.Ended) > getdate() and (r.Deleted is null or r.Deleted = 0)
					group by r.ResponseTemplateID
					
				-- select weekly event
				insert into ckbx_Timeline_Result (RequestID, EventID, Occured, UserID, ObjectID, ObjectGUID, ObjectParentID, Manager, Creator, ObjectParentType, EventCount, PeriodID)		
					select @RequestID, @EventID, coalesce(min(r.Ended), getdate()), null, null, null, r.ResponseTemplateID, @Manager, @Creator, ''SURVEY'', count(*) as EventCount, 3 as PeriodID  from #Surveys s
						inner join ckbx_Response r on s.ResponseTemplateID = r.ResponseTemplateID					
					where DATEADD(week, 1, r.Ended) > getdate() and (r.Deleted is null or r.Deleted = 0)
					group by r.ResponseTemplateID
					
				-- select monthly event
				insert into ckbx_Timeline_Result (RequestID, EventID, Occured, UserID, ObjectID, ObjectGUID, ObjectParentID, Manager, Creator, ObjectParentType, EventCount, PeriodID)		
					select @RequestID, @EventID, coalesce(min(r.Ended), getdate()), null, null, null, r.ResponseTemplateID, @Manager, @Creator, ''SURVEY'', count(*) as EventCount, 4 as PeriodID  from #Surveys s
						inner join ckbx_Response r on s.ResponseTemplateID = r.ResponseTemplateID					
					where DATEADD(month, 1, r.Ended) > getdate() and (r.Deleted is null or r.Deleted = 0)
					group by r.ResponseTemplateID
			end		
		end
		
		update ckbx_Timeline_Request set RequestStatus = ''Succeeded'' where RequestID = @RequestID
		
		
		drop table #Surveys
		drop table #Reports
	end try
	begin catch
		update ckbx_Timeline_Request set Message = ERROR_MESSAGE(), RequestStatus = ''Error'' 
			where RequestID = @RequestID
	end catch
	
end

' 
END