<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="UsersSearchResults.ascx.cs" Inherits="CheckboxWeb.Controls.Search.UsersSearchResults" %>
<%@ Register Assembly="CheckboxWeb" Namespace="CheckboxWeb.Controls.Button" TagPrefix="btn" %>
<%@ Import Namespace="Checkbox.Management" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Register TagPrefix="ckbx" TagName="Grid" Src="~/Controls/Grid.ascx" %>

<script type="text/javascript" language="javascript">
    var searchResults_<%=ID %> = new searchResultsObj();

    var _userToDelete = '';
    var _selectedUser = '';

    $(document).ready(function(){
        $(document).on('click', '.userSearchResult', onUserResultClick);

        statusControl.initialize('<%=StatusContainer %>');

        $('#<%=ID%>resultHeader').click(function(){
            if($('#<%=ID %>resultsContainer').is(':visible')){
                $('#<%=ID %>resultsContainer').hide('blind', null, 'fast');
            }
            else{
                $('#<%=ID %>resultsContainer').show('blind', null, 'fast');
            }
        });

        searchResults_<%=ID %>.initialize(
            ['<%=ID %>matchingUserNameContainer', '<%=ID %>matchingEmailAddressContainer'],
            [<%=_usersByNameResultsGrid.ReloadGridHandler %>, <%=_usersByEmailAddressResultsGrid.ReloadGridHandler %>],
            'emptyGrid_<%=ID %>',
            'detailProgressContainer_<%=ID %>',
            '<%=ID %>_termTitle',
            svcUserManagement.searchUsers,
            '<%=ID %>'
        );
    });

    //
    function onUserResultClick(){
        loadUserDash($(this).attr('userId'));
    }

    //
    function loadUserDash(userId){
        if(userId == null || userId == ''){
            return;
        }

        _selectedUser = userId;

        $('#<%=DashContainer %>').html($('#detailProgressContainer_<%=ID %>').html());

        svcUserManagement.getUserData(
            _at, 
            userId, 
            function(resultData){
                if(resultData == null){
                    return;
                }

                templateHelper.loadAndApplyTemplate(
                    'userSearchDashTemplate.html',
                    '<%=ResolveUrl("~/Controls/Search/jqtmpl/userSearchDashTemplate.html") %>',
                    resultData,
                    null,
                    '<%=DashContainer %>',
                    true);
            }
        );
    }

  
    function <%=ID%>_startSearch(searchTerm){
        $('#resultCount_<%=ID %>').hide();
        $('#<%=ID %>resultsContainer').hide();
        searchResults_<%=ID %>.startSearch(searchTerm, <%=ID %>onResultsLoaded);
    }

    //
    function  <%=ID %>onResultsLoaded(resultCount){
        $('#resultCount_<%=ID %>').html('(' + resultCount + ')');
        $('#resultCount_<%=ID %>').show();
        _userToDelete = ''
    }

    //
    function confirmDeleteUser(userId){
        _userToDelete = userId;

        showConfirmDialogWithCallback(
            '<%=WebTextManager.GetText("/controlText/usersSearchResults.ascx/deleteConfirmMessage") %>', 
                onDeleteUserConfirm,
                337,
                200,
            '<%=WebTextManager.GetText("/controlText/usersSearchResults.ascx/deleteConfirm") %>'
        );
    }
    
    //
    function onDeleteUserConfirm(args){
        if(args.success){
            var idArray = new Array();
            var user = _userToDelete.substring(0, _userToDelete.indexOf("&"));
            idArray.push(user);

            if (idArray.length > 0){
                svcUserManagement.deleteUsers(
                     _at,
                     idArray,
                     true,
                     function(){
                        statusControl.showStatusMessage(
                            '<%=WebTextManager.GetText("/controlText/usersSearchResults.ascx/userDeleted") %>'.replace('{0}', _userToDelete),
                            StatusMessageType.success);

                        _userToDelete = '';
                        <%=ID%>_startSearch(searchResults_<%=ID %>.term);
                        $('#<%=DashContainer %>').empty();//.html('');
                     }                     
                 );
            }
        }
    }

    //
    function onEditUserDialogClosed(args){
        if(args == null) {
            return;
        }

        //Show status
        statusControl.showStatusMessage(
            '<%=WebTextManager.GetText("/controlText/usersSearchResults.ascx/userUpdated") %>'.replace('{0}', _selectedUser),
            StatusMessageType.success);

        //Reload dash
        loadUserDash(_selectedUser);
    }

    //
    function <%=ID%>_loadResultsByUserName(currentPage, sortField, sortAscending, loadCompleteCallback, loadCompleteArgs){
        searchResults_<%=ID %>.getStoredResults('matchingUserName', currentPage, <%=ApplicationManager.AppSettings.PagingResultsPerPage %>, loadCompleteCallback, loadCompleteArgs);
    }

    //
    function onUserResultsByUserGridRendered(){
        $('.userResultsUserName').highlight(searchResults_<%=ID %>.term);
    }

    //
    function <%=ID%>_loadResultsByEmailAddress(currentPage, sortField, sortAscending, loadCompleteCallback, loadCompleteArgs){
        searchResults_<%=ID %>.getStoredResults('matchingEmailAddress', currentPage, <%=ApplicationManager.AppSettings.PagingResultsPerPage %>, loadCompleteCallback, loadCompleteArgs);
    }

    //
    function onUserResultsByEmailAddressGridRendered(){
        $('.userResultsEmailAddress').highlight(searchResults_<%=ID %>.term);
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
        <span class="left">&nbsp;&nbsp;<%=WebTextManager.GetText("/controlText/usersSearchResults.ascx/usersMatching")%>&nbsp;</span>
        <span id="<%=ID %>_termTitle" style="font-style:italic;"></span>
    </span>
    <br class="clear" />
</div>

<div id="<%=ID %>resultsContainer" style="display:none;">
    <div id="emptyGrid_<%=ID %>" class="emptyGrid" style="display:none;"><%= WebTextManager.GetText("/controlText/usersSearchResults.ascx/noUsers")%></div>

    <div class="padding10" id="<%=ID %>matchingUserNameContainer">
        <div class="label">Matching USER NAME:</div>
        <ckbx:Grid ID="_usersByNameResultsGrid" runat="server" />
    </div>

    <div class="padding10" id="<%=ID %>matchingEmailAddressContainer">
        <div class="label">Matching EMAIL:</div>
        <ckbx:Grid ID="_usersByEmailAddressResultsGrid" runat="server" />
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

        _usersByNameResultsGrid.InitialSortField = "UserIdentifier";
        _usersByNameResultsGrid.ItemClickCallback = ClientResultSelectedHandler;
        _usersByNameResultsGrid.ListTemplatePath = ResolveUrl("~/Controls/Search/jqtmpl/usersByUserNameResultListTemplate.html");
        _usersByNameResultsGrid.ListItemTemplatePath = ResolveUrl("~/Controls/Search/jqtmpl/usersByUserNameResultListItemTemplate.html");
        _usersByNameResultsGrid.LoadDataCallback = ID + "_loadResultsByUserName";
        _usersByNameResultsGrid.EmptyGridText = WebTextManager.GetText("/controlText/usersSearchResults.ascx/noUsers");
        _usersByNameResultsGrid.DelayLoad = true;
        _usersByNameResultsGrid.LoadingTextId = "/controlText/usersSearchResults.ascx/findingUsers";
        _usersByNameResultsGrid.RenderCompleteCallback = "onUserResultsByUserGridRendered";

        _usersByEmailAddressResultsGrid.InitialSortField = "Email";
        _usersByEmailAddressResultsGrid.ItemClickCallback = ClientResultSelectedHandler;
        _usersByEmailAddressResultsGrid.ListTemplatePath = ResolveUrl("~/Controls/Search/jqtmpl/usersByEmailResultListTemplate.html");
        _usersByEmailAddressResultsGrid.ListItemTemplatePath = ResolveUrl("~/Controls/Search/jqtmpl/usersByEmailResultListItemTemplate.html");
        _usersByEmailAddressResultsGrid.LoadDataCallback = ID + "_loadResultsByEmailAddress";
        _usersByEmailAddressResultsGrid.EmptyGridText = WebTextManager.GetText("/controlText/usersSearchResults.ascx/noUsers");
        _usersByEmailAddressResultsGrid.DelayLoad = true;
        _usersByEmailAddressResultsGrid.LoadingTextId = "/controlText/usersSearchResults.ascx/findingUsers";
        _usersByEmailAddressResultsGrid.RenderCompleteCallback = "onUserResultsByEmailAddressGridRendered";
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

        //Date Utils
        RegisterClientScriptInclude(
            "dateUtils.js",
            ResolveUrl("~/Resources/dateUtils.js"));

        //Moment.js: datetime utilities
        RegisterClientScriptInclude(
            "moment.js",
            ResolveUrl("~/Resources/moment.js"));

        //Text helper
        RegisterClientScriptInclude(
            "textHelper.js",
            ResolveUrl("~/Resources/textHelper.js"));

        //Status control
        RegisterClientScriptInclude(
            "statusControl.js",
            ResolveUrl("~/Resources/statusControl.js"));
    }
</script>
