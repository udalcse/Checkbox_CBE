<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="UserGroupsSearchResults.ascx.cs" Inherits="CheckboxWeb.Controls.Search.UserGroupsSearchResults" %>
<%@ Register Assembly="CheckboxWeb" Namespace="CheckboxWeb.Controls.Button" TagPrefix="btn" %>
<%@ Import Namespace="Checkbox.Management" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Register TagPrefix="ckbx" TagName="Grid" Src="~/Controls/Grid.ascx" %>

<script type="text/javascript" language="javascript">
    var searchResults_<%=ID %> = new searchResultsObj();
    var _dashGroupId = -1;

    $(document).ready(function(){
        $('.groupSearchResult').on('click', '.groupSearchResult', onGroupResultSelect);

        templateHelper.loadAndCompileTemplate('<%=ResolveUrl("~/Controls/Search/jqtmpl/groupMemberListTemplate.html") %>', 'groupMemberListTemplate.html');
        templateHelper.loadAndCompileTemplate('<%=ResolveUrl("~/Controls/Search/jqtmpl/groupMemberListItemTemplate.html") %>', 'groupMemberListItemTemplate.html');

         
        $('#<%=ID%>resultHeader').click(function(){
            if($('#<%=ID %>resultsContainer').is(':visible')){
                $('#<%=ID %>resultsContainer').hide('blind', null, 'fast');
            }
            else{
                $('#<%=ID %>resultsContainer').show('blind', null, 'fast');
            }
        });

        //Select all/none implementation
        $(document).on('click', '#_deleteSelectedMembersLink', function(){
            confirmRemoveGroupMembers(_dashGroupId);              
        });

        $(document).on('click', '#_selectAllMembers', function(){
            if ($(this).attr('checked'))
                $('.deleteMember').attr('checked', 'checked');
            else
                $('.deleteMember').removeAttr('checked');

        });

        searchResults_<%=ID %>.initialize(
            ['<%=ID %>matchingNameContainer', '<%=ID %>matchingOwnerContainer'],
            [<%=_groupResultsByNameGrid.ReloadGridHandler %>, <%=_groupResultsByOwnerGrid.ReloadGridHandler %>],
            'emptyGrid_<%=ID %>',
            'detailProgressContainer_<%=ID %>',
            '<%=ID %>_termTitle',
            svcUserManagement.searchGroups,
            '<%=ID %>'
        );
    });

   
    function <%=ID%>_startSearch(searchTerm){
        $('#resultCount_<%=ID %>').hide();
        $('#<%=ID %>resultsContainer').hide();
        searchResults_<%=ID %>.startSearch(searchTerm, <%=ID %>onResultsLoaded);
    }

    //
    function  <%=ID %>onResultsLoaded(resultCount){
        $('#resultCount_<%=ID %>').html('(' + resultCount + ')');
        $('#resultCount_<%=ID %>').show();
    }

     //
    function onGroupResultSelect(){
        loadGroupData($(this).attr('groupId'));
    }

    //
    function loadGroupData(groupId){
     if(groupId == null || groupId == ''){
            return;
        }
        
        _dashGroupId = groupId;

        $('#<%=DashContainer %>').html($('#detailProgressContainer_<%=ID %>').html());

        svcUserManagement.getUserGroupById(
            _at, 
            groupId, 
            function(resultData){
                if(resultData == null){
                    return;
                }

                templateHelper.loadAndApplyTemplate(
                    'userGroupSearchDashTemplate.html',
                    '<%=ResolveUrl("~/Controls/Search/jqtmpl/userGroupSearchDashTemplate.html") %>',
                    resultData,
                    null,
                    '<%=DashContainer %>',
                    true,
                    onGroupTemplateApplied,
                    {groupId: groupId}
                );
            }
        );
    }

    //
    function onGroupTemplateApplied(args){
        <%-- Load Member List --%>
        svcUserManagement.listUserGroupMembers(
            _at,
            args.groupId,
            {
                pageSize : <%=ApplicationManager.AppSettings.PagingResultsPerPage %>,
                pageNumber : 1,
                sortField : '',
                sortAscending : true,
                filterField : '',
                filterValue : ''
            },
            onGroupMemberListLoaded,
            args
        );
    }

    //Member list loaded
    function onGroupMemberListLoaded(resultData, args){
        //Render grid
        gridHelper.renderGrid(
            resultData.ResultPage, 
            'groupMembersPlace', 
            'groupMemberListTemplate.html',
            '<%=ResolveUrl("~/Controls/Search/jqtmpl/groupMemberListTemplate.html") %>'
        );
    }
     
    //
    function confirmRemoveGroupMembers(groupId){
        _dashGroupId = groupId;

        showConfirmDialogWithCallback(
            '<%=WebTextManager.GetText("/controlText/groupsSearchResults.ascx/deleteConfirmMessage") %>', 
                onDeleteGroupMembersConfirm,
                337,
                200,
            '<%=WebTextManager.GetText("/controlText/groupsSearchResults.ascx/deleteConfirm") %>'
        );
    }

    //
    function onDeleteGroupMembersConfirm(args){
        if(args.success){
            return;
        }

        var membersArray = new Array();
        $('.deleteMember:checked').each(function(index){
            membersArray.push($(this).attr('value'));
        });

        if (membersArray.length > 0){
            svcUserManagement.removeUsersFromGroup(
                _at,                        
                membersArray,
                _dashGroupId,
                function(){
                    loadGroupData(_dashGroupId);
                    <%if (!String.IsNullOrEmpty(StatusContainer))
                    {%>
                        statusControl.showStatusMessage(
                            '<%=WebTextManager.GetText("/controlText/groupsSearchResults.ascx/usersRemoved") %>'.replace('{0}', membersArray.length),
                            StatusMessageType.success);
                <%
                    }%>
                }
            );
        }
    }

    //
    function <%=ID%>_loadResultsByName(currentPage, sortField, sortAscending, loadCompleteCallback, loadCompleteArgs){
        searchResults_<%=ID %>.getStoredResults('matchingName', currentPage, <%=ApplicationManager.AppSettings.PagingResultsPerPage %>, loadCompleteCallback, loadCompleteArgs);
    }

    //
    function onGroupResultsByNameGridRendered(){
        $('.groupResultGroupName').highlight(searchResults_<%=ID %>.term);
    }

    //
    function <%=ID%>_loadResultsByOwner(currentPage, sortField, sortAscending, loadCompleteCallback, loadCompleteArgs){
        searchResults_<%=ID %>.getStoredResults('matchingOwner', currentPage, <%=ApplicationManager.AppSettings.PagingResultsPerPage %>, loadCompleteCallback, loadCompleteArgs);
    }

     //
    function onGroupResultsByOwnerGridRendered(){
        $('.groupResultsGroupOwner').highlight(searchResults_<%=ID %>.term);
    }

     //
    function onEditGroupDialogClosed(args){
        if(args == null) {
            return;
        }

         //Show status
        statusControl.showStatusMessage(
            '<%=WebTextManager.GetText("/controlText/groupsSearchResults.ascx/groupUPdated") %>'.replace('{0}', _dashGroupId),
            StatusMessageType.success);

        //Reload dash
        loadGroupData(_dashGroupId);
    }



</script>

<div id="<%=ID%>resultHeader" class="dashStatsHeader">
    <span class="left mainStats">
        <div id="detailProgressContainer_<%=ID %>" class="left">
            <div id="detailProgress_<%=ID %>" class="ProgressSpinner">
                <asp:Image ID="_progressSpinner" runat="server" SkinId="ProgressSpinner" Style="height:24px;" />
            </div>
        </div>
        <span id="resultCount_<%=ID %>" style="display:none;" class="left"></span>
        <span class="left">&nbsp;&nbsp;<%=WebTextManager.GetText("/controlText/groupsSearchResults.ascx/groupsMatching")%>&nbsp;</span>
        <span id="<%=ID %>_termTitle" style="font-style:italic;"></span>
    </span>
    <br class="clear" />
</div>

<div id="<%=ID %>resultsContainer" style="display:none;">

    <div id="emptyGrid_<%=ID %>" class="emptyGrid" style="display:none;"><%= WebTextManager.GetText("/controlText/groupsSearchResults.ascx/noGroups")%></div>

    <div class="padding10" id="<%=ID %>matchingNameContainer">
        <div class="label">Groups matching NAME:</div>
        <ckbx:Grid ID="_groupResultsByNameGrid" runat="server" />
    </div>

    <div class="padding10" id="<%=ID %>matchingOwnerContainer">
        <div class="label">Groups matching CREATED BY:</div>
        <ckbx:Grid ID="_groupResultsByOwnerGrid" runat="server" />
    </div>
</div>


<script type="text/C#" runat="server">
    /// <summary>
    /// 
    /// </summary>
    /// <param name="e"></param>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        _groupResultsByNameGrid.InitialSortField = "Name";
        _groupResultsByNameGrid.ItemClickCallback = ClientResultSelectedHandler;
        _groupResultsByNameGrid.ListTemplatePath = ResolveUrl("~/Controls/Search/jqtmpl/groupsByNameResultListTemplate.html");
        _groupResultsByNameGrid.ListItemTemplatePath = ResolveUrl("~/Controls/Search/jqtmpl/groupsByNameResultListItemTemplate.html");
        _groupResultsByNameGrid.LoadDataCallback = ID + "_loadResultsByName";
        _groupResultsByNameGrid.EmptyGridText = WebTextManager.GetText("/controlText/groupsSearchResults.ascx/noGroups");
        _groupResultsByNameGrid.DelayLoad = true;
        _groupResultsByNameGrid.LoadingTextId = "/controlText/groupsSearchResults.ascx/findingGroups";
        _groupResultsByNameGrid.RenderCompleteCallback = "onGroupResultsByNameGridRendered";

        _groupResultsByOwnerGrid.InitialSortField = "Name";
        _groupResultsByOwnerGrid.ItemClickCallback = ClientResultSelectedHandler;
        _groupResultsByOwnerGrid.ListTemplatePath = ResolveUrl("~/Controls/Search/jqtmpl/groupsByOwnerResultListTemplate.html");
        _groupResultsByOwnerGrid.ListItemTemplatePath = ResolveUrl("~/Controls/Search/jqtmpl/groupsByOwnerResultListItemTemplate.html");
        _groupResultsByOwnerGrid.LoadDataCallback = ID + "_loadResultsByOwner";
        _groupResultsByOwnerGrid.EmptyGridText = WebTextManager.GetText("/controlText/groupsSearchResults.ascx/noGroups");
        _groupResultsByOwnerGrid.DelayLoad = true;
        _groupResultsByOwnerGrid.LoadingTextId = "/controlText/groupsSearchResults.ascx/findingGroups";
        _groupResultsByOwnerGrid.RenderCompleteCallback = "onGroupResultsByOwnerGridRendered";
    }
    
    /// <summary>
    /// Ensure necessary scripts are loaded
    /// </summary>
    /// <param name="e"></param>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        
        //Service helper
        RegisterClientScriptInclude(
            "serviceHelper.js",
            ResolveUrl("~/Services/js/serviceHelper.js"));

        //Survey management
        RegisterClientScriptInclude(
            "svcUserManagement.js",
            ResolveUrl("~/Services/js/svcUserManagement.js"));

        //Search results client script
        RegisterClientScriptInclude(
            "searchResults.js",
            ResolveUrl("~/Controls/Search/searchResults.js"));
        
        //Highlight
        RegisterClientScriptInclude(
            "jquery.highlight-3.yui.js",
            ResolveUrl("~/Resources/jquery.highlight-3.yui.js"));

        //Grid
        RegisterClientScriptInclude(
            "grid.js",
            ResolveUrl("~/Resources/grid.js"));
    }
</script>
