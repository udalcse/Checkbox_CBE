INSERT INTO [ckbx_Timeline_Events] (EventName, EventID) VALUES ('PREPMODE_ON', 17)
INSERT INTO [ckbx_Timeline_Events] (EventName, EventID) VALUES ('PREPMODE_OFF', 18)
INSERT INTO [ckbx_Timeline_Config] (EventID, PeriodID, Image) VALUES (17, 1, 'user')
INSERT INTO [ckbx_Timeline_Config] (EventID, PeriodID, Image) VALUES (18, 1, 'user')
INSERT INTO [ckbx_Timeline_Settings] (Manager, EventID, Single ,Daily, Weekly, Monthly, EventOrder) VALUES ('UserManager', 17, 1, 1, 1, 1, 1)
INSERT INTO [ckbx_Timeline_Settings] (Manager, EventID, Single ,Daily, Weekly, Monthly, EventOrder) VALUES ('UserManager', 18, 1, 1, 1, 1, 2)
INSERT INTO [ckbx_PageId] (PagePath) VALUES ('~/Settings/Default.aspx')
INSERT INTO [ckbx_PageTextIds] (PageId, TextId) VALUES ((SELECT PageID FROM [ckbx_PageId] WHERE PagePath = '~/Settings/Default.aspx'), '/timeline/event/description/prepmode_on/1')
INSERT INTO [ckbx_PageTextIds] (PageId, TextId) VALUES ((SELECT PageID FROM [ckbx_PageId] WHERE PagePath = '~/Settings/Default.aspx'), '/timeline/event/description/prepmode_off/1')
INSERT INTO [ckbx_Text] (TextID, TextValue, LanguageCode) VALUES ('/timeline/event/description/prepmode_off/1', '{UserID} switched to Production Mode at {Time} on {Date}', 'en-US')
INSERT INTO [ckbx_Text] (TextID, TextValue, LanguageCode) VALUES ('/timeline/event/description/prepmode_on/1', '{UserID} switched to Prep Mode at {Time} on {Date}', 'en-US')


--drop constraint
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_ckbx_SystemMode_EventType_ckbx_SystemMode_Log]') AND parent_object_id = OBJECT_ID(N'[ckbx_SystemMode_Log]'))
ALTER TABLE [ckbx_SystemMode_Log] DROP CONSTRAINT [FK_ckbx_SystemMode_EventType_ckbx_SystemMode_Log]
GO

--drop table
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_SystemMode_EventType]') AND type in (N'U'))
DROP TABLE [ckbx_SystemMode_EventType]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_SystemMode_Log]') AND type in (N'U'))
DROP TABLE [ckbx_SystemMode_Log]
GO

--drop sp
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_spSystemMode_LogEvent]') AND type in (N'P', N'PC'))
DROP PROCEDURE [ckbx_spSystemMode_LogEvent]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Timeline_Collect_UserManager]') AND type in (N'P', N'PC'))
DROP PROCEDURE [ckbx_sp_Timeline_Collect_UserManager]
GO

--create

--create table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_SystemMode_EventType') AND type in (N'U'))
BEGIN
	CREATE TABLE [ckbx_SystemMode_EventType](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[EventType] [nvarchar](255) NULL,
	CONSTRAINT [PK_ckbx_SystemMode_EventType] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_SystemMode_Log') AND type in (N'U'))
BEGIN
	CREATE TABLE [ckbx_SystemMode_Log](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[EventTypeId] [int] NULL,
		[DateCreated] [datetime] NULL,
		[DateModified] [datetime] NULL,
		[CreatedBy] [nvarchar](255) NULL,
	CONSTRAINT [PK_ckbx_SystemMode_Log] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]
END
GO

--create constraint
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_ckbx_SystemMode_EventType_ckbx_SystemMode_Log]') AND parent_object_id = OBJECT_ID(N'[ckbx_SystemMode_Log]'))
ALTER TABLE [ckbx_SystemMode_Log]  WITH CHECK ADD  CONSTRAINT [FK_ckbx_SystemMode_EventType_ckbx_SystemMode_Log] FOREIGN KEY([EventTypeId])
REFERENCES [dbo].[ckbx_SystemMode_EventType] ([Id])
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[FK_ckbx_SystemMode_EventType_ckbx_SystemMode_Log]') AND parent_object_id = OBJECT_ID(N'[ckbx_StyleTemplate_Properties]'))
ALTER TABLE [ckbx_StyleTemplate_Properties] CHECK CONSTRAINT [FK_ckbx_SystemMode_EventType_ckbx_SystemMode_Log]
GO

--create sp


IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_SystemMode_LogEvent]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'	CREATE PROCEDURE ckbx_sp_SystemMode_LogEvent
(
  @EventName nvarchar(255),
  @CreatedBy nvarchar(255)
)
AS
  DECLARE @EventTypeId int

  SELECT @EventTypeId = Id FROM [ckbx_SystemMode_EventType] WHERE EventType = @EventName

  IF @EventTypeId is NULL
  BEGIN
	RAISERROR(''Invalid event %e, Event with such name does not exist in ckbx_SystemMode_EventType'', 10, 1, @EventName)
	RETURN
  END

  INSERT INTO [ckbx_SystemMode_Log] (EventTypeId, DateModified, CreatedBy)
  VALUES (@EventTypeId, GETDATE(), @CreatedBy)

' 
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[ckbx_sp_Timeline_Collect_UserManager]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
--collect events for UserManager for the last month or from the latest request up to now
Create PROCEDURE [ckbx_sp_Timeline_Collect_UserManager]
(
  @RequestID bigint
)
AS
begin
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
	
		declare @CanViewAllUsers int
		set @CanViewAllUsers = 0
	
		if @RoleID is not null
		begin
			set @CanViewAllUsers = 1 --system admin can
		end	
		else
		begin
			--check if Everyone group is available for everyone
			select @CanViewAllUsers = count(*) from ckbx_Group g
				inner join ckbx_PolicyPermissions pp on g.DefaultPolicy = pp.PolicyID
				inner join ckbx_Permission p on pp.PermissionID = p.PermissionID
				where PermissionName = ''Group.View'' and g.GroupId = 1
		
			if @CanViewAllUsers = 0
			begin
				--check if regular user can view Everyone group
				select @CanViewAllUsers = count(*) from ckbx_Group g
					inner join ckbx_AccessControlEntries aces on g.AclID = aces.AclID
					inner join ckbx_AccessControlEntry ace on aces.EntryID = ace.EntryID
					inner join ckbx_PolicyPermissions pp on ace.PolicyID = pp.PolicyID
					inner join ckbx_Permission p on pp.PermissionID = p.PermissionID
					where GroupId = 1 and EntryType like ''%ExtendedPrincipal%'' and EntryIdentifier = @Creator and PermissionName like ''%Group%''
			
				if @CanViewAllUsers = 0
				begin
					--check if the user is a member of the group
					select @CanViewAllUsers = count(*) from ckbx_Group g
						inner join ckbx_AccessControlEntries aces on g.AclID = aces.AclID
						inner join ckbx_AccessControlEntry ace on aces.EntryID = ace.EntryID
						inner join ckbx_PolicyPermissions pp on ace.PolicyID = pp.PolicyID
						inner join ckbx_Permission p on pp.PermissionID = p.PermissionID
						inner join ckbx_GroupMembers gm on EntryType like ''%Group%'' and gm.GroupID = cast(EntryIdentifier as int)
						where g.GroupId = 1 and gm.MemberUniqueIdentifier = @Creator and PermissionName = ''Group.View''
				end
			end
		end

		if @CanViewAllUsers > 0
		begin		
			declare @EventID int

			set @EventID = null
			select @EventID = EventID from ckbx_Timeline_Events where 
				EventName = ''USER_CREATED''		
		
			--collect all recently created users
			if @EventID is not null
			begin
				--select * from ckbx_Credential
				insert into ckbx_Timeline_Result (RequestID, EventID, Occured, UserID, ObjectID, ObjectGUID, Manager, Creator)
					select @RequestID, @EventID, c.Created, c.CreatedBy, c.[UniqueIdentifier], c.[GUID], @Manager, @Creator from ckbx_Credential c					
					where c.Created > @StartDate
			end


			set @EventID = null
			select @EventID = EventID from ckbx_Timeline_Events where 
				EventName = ''USER_EDITED''		
		
			--collect all recently edited users
			if @EventID is not null
			begin
				--select * from ckbx_Credential
				insert into ckbx_Timeline_Result (RequestID, EventID, Occured, UserID, ObjectID, ObjectGUID, Manager, Creator)		
					select @RequestID, @EventID, c.ModifiedDate, c.ModifiedBy, c.[UniqueIdentifier], c.[GUID], @Manager, @Creator from ckbx_Credential c					
					where c.ModifiedDate > @StartDate
			end
		end
		else
		begin
			--find available group members
			create table #accesibleUsers
			(GroupID int, [UniqueIdentifier] nvarchar(611), Email nvarchar(255))

			insert into #accesibleUsers
				exec ckbx_sp_Security_ListAccessibleGroupMembers ''sa'', ''Group.View'', ''Group.Edit'', 0, 0, 0, 0, '''', 1, '''', '''', 0			
			
		
			--collect all recently created users
			if @EventID is not null
			begin
				--select * from ckbx_Credential
				insert into ckbx_Timeline_Result (RequestID, EventID, Occured, UserID, ObjectID, ObjectGUID, Manager, Creator)
					select @RequestID, @EventID, c.Created, c.CreatedBy, c.[UniqueIdentifier], c.[GUID], @Manager, @Creator from ckbx_Credential c
					inner join #accesibleUsers au on c.[UniqueIdentifier] = au.[UniqueIdentifier]
					where c.Created > @StartDate
			end


			set @EventID = null
			select @EventID = EventID from ckbx_Timeline_Events where 
				EventName = ''USER_EDITED''		
		
			--collect all recently edited users
			if @EventID is not null
			begin
				--select * from ckbx_Credential
				insert into ckbx_Timeline_Result (RequestID, EventID, Occured, UserID, ObjectID, ObjectGUID, Manager, Creator)		
					select @RequestID, @EventID, c.ModifiedDate, c.ModifiedBy, c.[UniqueIdentifier], c.[GUID], @Manager, @Creator from ckbx_Credential c					
					inner join #accesibleUsers au on c.[UniqueIdentifier] = au.[UniqueIdentifier]
					where c.ModifiedDate > @StartDate
			end
		end
		
		--collect group information
		declare @GroupEventCount int
	
		select @GroupEventCount = count(*) from ckbx_Timeline_Events 
			where EventName like ''GROUP%''
	
		if @GroupEventCount > 0 
		begin	
			CREATE TABLE #Groups (
				[GroupID] [int] NOT NULL,
				[GroupName] [nvarchar](510) NULL,
				[Description] [nvarchar](510) NULL,
				[DateCreated] [datetime] NULL,
				[CreatedBy] [nvarchar](611) NULL,
				[AclID] [int] NULL,
				[DefaultPolicy] [int] NULL,
				[ModifiedDate] [datetime] NULL,
				[ModifiedBy] [nvarchar](611) NULL
				)
		
			if @RoleID is not null
			begin
				insert into #Groups ([GroupID],[GroupName],[Description],[DateCreated],[CreatedBy],[AclID],[DefaultPolicy],[ModifiedDate],[ModifiedBy])			
					exec ckbx_sp_Security_ListAllGroups @PageNumber=-1,@ResultsPerPage=-1,@SortField=N''GroupName'',@SortAscending=1,@FilterField=N''GroupName'',@FilterValue=N'''',@IncludeEveryoneGroup=0, @DisplayCount=0
			end
			else
			begin
				insert into #Groups ([GroupID],[GroupName],[Description],[DateCreated],[CreatedBy],[AclID],[DefaultPolicy],[ModifiedDate],[ModifiedBy])			
					exec ckbx_sp_Security_ListAccessibleGroups @UniqueIdentifier=@Creator,@FirstPermissionName=N''Group.View'',@SecondPermissionName=N'''',@RequireBothPermissions=1,@UseAclExclusion=1,@PageNumber=-1,@ResultsPerPage=-1,@SortField=N''GroupName'',@SortAscending=1,@FilterField=N''GroupName'',@FilterValue=N'''',@IncludeEveryoneGroup=0,@DisplayCount=0
			end 
		
		
			set @EventID = null
			select @EventID = EventID from ckbx_Timeline_Events where 
				EventName = ''GROUP_CREATED''		
		
			--collect all recently created groups
			if @EventID is not null
			begin
				insert into ckbx_Timeline_Result (RequestID, EventID, Occured, UserID, ObjectID, ObjectGUID, Manager, Creator)		
					select @RequestID, @EventID, g.[DateCreated], g.CreatedBy, g.GroupID, null, @Manager, @Creator from #Groups g					
					where g.[DateCreated] > @StartDate
			end

			set @EventID = null
			select @EventID = EventID from ckbx_Timeline_Events where 
				EventName = ''GROUP_EDITED''		

			--collect all recently created groups
			if @EventID is not null
			begin
				insert into ckbx_Timeline_Result (RequestID, EventID, Occured, UserID, ObjectID, ObjectGUID, Manager, Creator)		
					select @RequestID, @EventID, g.ModifiedDate, g.ModifiedBy, g.GroupID, null, @Manager, @Creator from #Groups g					
					where g.ModifiedDate > @StartDate
			end

		end
	
		--collect email list information
		declare @emailListEventCount int
	
		select @emailListEventCount = count(*) from ckbx_Timeline_Events 
			where EventName like ''EMAILLIST%''

		if @emailListEventCount > 0 
		begin	
			CREATE TABLE #emailLists (
				[PanelID] [int] NOT NULL,
				[Name] [nvarchar](510) NOT NULL,
				[Description] [nvarchar](510) NULL,
				[DateCreated] [datetime] NOT NULL,
				[CreatedBy] [nvarchar](611) NOT NULL,
				[PanelTypeID] [int] NOT NULL,
				[Deleted] [bit] NULL,
				[AclID] [int] NULL,
				[DefaultPolicy] [int] NULL,
				[ModifiedDate] [datetime] NULL,
				[ModifiedBy] [nvarchar](611) NULL
			) 
			if @RoleID is not null
			begin
				CREATE TABLE #emailListsAdmin (
					[PanelID] [int] NOT NULL
					)
				insert into #emailListsAdmin
					([PanelID]) 
					exec ckbx_sp_Security_ListAccessibleEmailListsAdmin @PageNumber=-1,@ResultsPerPage=-1,@SortField=N''Name'',@SortAscending=1,@FilterField=N'''',@FilterValue=N'''', @DisplayCount=0 
			
				insert into #emailLists
					select p.PanelID, p.Name, cast(p.Description as nvarchar(511)) as Description, p.DateCreated, p.CreatedBy, p.PanelTypeID, p.Deleted, p.AclID, p.DefaultPolicy, p.ModifiedDate, p.ModifiedBy as Description  from ckbx_Panel p  inner join #emailListsAdmin a on p.PanelId = a.PanelId
			end
			else
			begin
				insert into #emailLists 
					([PanelID],[Name],[Description],[DateCreated],[CreatedBy],[PanelTypeID],[Deleted],[AclID],[DefaultPolicy],[ModifiedDate],[ModifiedBy]) 				
					exec ckbx_sp_Security_ListAccessibleEmailLists @UniqueIdentifier=@Creator,@FirstPermissionName=N''EmailList.View'',@SecondPermissionName=N'''',@RequireBothPermissions=1,@UseAclExclusion=1,@PageNumber=-1,@ResultsPerPage=-1,@SortField=N''Name'',@SortAscending=1,@FilterField=N'''',@FilterValue=N'''', @DisplayCount=0
			end 
		
			set @EventID = null
			select @EventID = EventID from ckbx_Timeline_Events where 
				EventName = ''EMAILLIST_CREATED''		
		
			--collect all recently created groups
		
			if @EventID is not null
			begin
				insert into ckbx_Timeline_Result (RequestID, EventID, Occured, UserID, ObjectID, ObjectParentID, ObjectGUID, Manager, Creator)		
					select @RequestID, @EventID, e.[DateCreated], e.CreatedBy, e.PanelID, e.PanelID, null, @Manager, @Creator from #emailLists e					
					where e.[DateCreated] > @StartDate
			end
			set @EventID = null
			select @EventID = EventID from ckbx_Timeline_Events where 
				EventName = ''EMAILLIST_EDITED''		

			--collect all recently created groups
		
			if @EventID is not null
			begin
				insert into ckbx_Timeline_Result (RequestID, EventID, Occured, UserID, ObjectID, ObjectParentID, ObjectGUID, Manager, Creator)		
					select @RequestID, @EventID, e.ModifiedDate, e.ModifiedBy, e.PanelID, e.PanelID, null, @Manager, @Creator from #emailLists e					
					where e.ModifiedDate > @StartDate
			end
		end

		
	--collect system switch events
	declare @systemModeSwitch int
	declare @EventType nvarchar(255)
	select @systemModeSwitch = count(*) from ckbx_Timeline_Events 
		where EventName like ''PREPMODE%''

	if @systemModeSwitch > 0
	begin
		--system mode on
		set @EventType = ''PREPMODE_ON''
		set @EventID = null

		select @EventID = EventID from ckbx_Timeline_Events where 
			EventName = @EventType	
		
		declare @EventTypeId int

		select @EventTypeId = Id from [ckbx_SystemMode_EventType] as sme
		where EventType = @EventType

		if @EventID is not null
		begin
			--select * from ckbx_Credential
			insert into ckbx_Timeline_Result (RequestID, EventID, Occured, UserID, ObjectID, ObjectGUID, Manager, Creator)
				select @RequestID, @EventID, sml.DateModified, sml.CreatedBy, sml.CreatedBy, NEWID(), @Manager, @Creator from ckbx_SystemMode_Log as sml
				where sml.DateModified > @StartDate and sml.EventTypeId = @EventTypeId
		end
		--system mode off
		set @EventType = ''PREPMODE_OFF''
		set @EventID = null
		select @EventID = EventID from ckbx_Timeline_Events where 
			EventName = @EventType		
		
		select @EventTypeId = Id from [ckbx_SystemMode_EventType] as sme
		where EventType = @EventType

		if @EventID is not null
		begin
			--select * from ckbx_Credential
			insert into ckbx_Timeline_Result (RequestID, EventID, Occured, UserID, ObjectID, ObjectGUID, Manager, Creator)
				select @RequestID, @EventID, sml.DateModified, sml.CreatedBy, sml.CreatedBy, NEWID(), @Manager, @Creator from ckbx_SystemMode_Log as sml
				where sml.DateModified > @StartDate and sml.EventTypeId = @EventTypeId
		end
	end

	update ckbx_Timeline_Request set RequestStatus = ''Succeeded'' where RequestID = @RequestID
end


' 
END
GO


--Insert adittional data
INSERT INTO ckbx_SystemMode_EventType (EventType) VALUES ('PREPMODE_ON')
INSERT INTO ckbx_SystemMode_EventType (EventType) VALUES ('PREPMODE_OFF')