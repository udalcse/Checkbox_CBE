<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="GroupList.ascx.cs" Inherits="CheckboxWeb.Users.Controls.GroupList" %>
<%@ Register TagPrefix="ckbx" TagName="Grid" Src="~/Controls/Grid.ascx" %>
<%@ Register TagPrefix="sort" Namespace="CheckboxWeb.Controls" Assembly="CheckboxWeb" %>
<%@ Import Namespace="Checkbox.Management" %>
<%@ Import Namespace="Checkbox.Users" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Import Namespace="Prezza.Framework.Security" %>

<script type="text/javascript">
  <%-- Ensure service initialized --%>
    $(document).ready(function() {
        $(document).on('click', '.deleteGroup', function(){
           toggleGridActionButtons('group', this);
        });
        $(document).on('click', '#_selectAllGroups', function(){
            var actionsAllowed = false;
            if ($(this).prop('checked')) {
                actionsAllowed = true;
                $('.deleteGroup').prop('checked', true);
            } else {
                $('.deleteGroup').prop('checked', false);
            }
            $.uniform.update('.deleteGroup');
            toggleGridActionButtons('group', this, actionsAllowed);
        });

        //Bind delete selected groups click
        $('#_deleteSelectedGroupLink').click(function(){
            if ($('.deleteGroup:checked').length == $('.deleteGroup:checked[candelete=false]').length && $('.deleteGroup:checked[candelete=false]').length!=0) {
                $('.deleteGroup:checked[candelete=false]').prop('checked', false);
                //show no permission dialog
                alert('<%=WebTextManager.GetText("/pageText/users/groups/Manage.aspx/noPermission") %>');
            }
            else {
                $('.deleteGroup:checked[candelete=false]').prop('checked', false);
                if ($('.deleteGroup:checked').length > 0) {
                    //make a list from selected group names
                    var groupListToDelete = '<div class="groupListContainer"><ul>';
                    $('.deleteGroup:checked').each(function(index) {
                        groupListToDelete +='<li class="truncate">'+ $(this).attr('groupName') +'</li>';
                    });
                    groupListToDelete += '</ul></div>';
                    showConfirmDialogWithCallback(
                        '<%=WebTextManager.GetText("/pageText/users/groups/Manage.aspx/deleteSelectedGroupsConfirm") %>' + groupListToDelete,
                        onDeleteSelectedGroupsConfirm,
                        337,
                        270,
                        '<%=WebTextManager.GetText("/pageText/users/groups/Manage.aspx/deleteSelectedGroups") %>'
                    );
                }
            }
        });

        //Bind copy selected groups click
        $('#_copySelectedGroupLink').click(function(){
            var idArray = new Array();

            $('.deleteGroup:checked').each(function(index){
                idArray.push($(this).attr('value'));
            });
            if (idArray.length > 0)
            {
                svcUserManagement.copyUserGroups(
                    _at,
                    idArray,
                    '<%=WebTextManager.GetUserLanguage() %>',
                    onCopiedUserGroups,
                    idArray.length     
                ); 
            }
        });


    });
     
    <%-- Load group list --%>
    function loadGroupList(currentPage, sortField, sortAscending, loadCompleteCallback, loadCompleteArgs){
        svcUserManagement.listUserGroups(
            _at, 
             {
                pageNumber: currentPage,
                pageSize: <%= ApplicationManager.AppSettings.PagingResultsPerPage %>,
                filterField: '',
                filterValue: '',
                sortField: sortField,
                sortAscending: sortAscending,
                includeEveryoneGroup: true
            }, 
            loadCompleteCallback,
            loadCompleteArgs
        );       
    }


    <%-- calculates the field name date for the given period --%>
    function getDateFieldName(eventName)
    {
        if (typeof(eventName) != 'undefined')
        {
            if (eventName.indexOf('CREATE') > 0)
                return 'DateCreated';
            if (eventName.indexOf('EDIT') > 0)
                return 'ModifiedDate';
        }
        return null;
    }

    <%-- Load user list --%>
    function loadGroupListAjax(currentPage, sortField, sortAscending, filterField, filterValue, loadCompleteCallback, loadCompleteArgs ){
        filterValue = (typeof(_lastSearchTerm) == 'undefined' || _lastSearchTerm == null || _lastSearchTerm.length == 0) ? filterValue : _lastSearchTerm;
        svcUserManagement.listUserGroups(
            _at, 
             {
                pageNumber: currentPage,
                pageSize: <%= ApplicationManager.AppSettings.PagingResultsPerPage %>,
                filterField: filterField,
                filterValue: filterValue,
                sortField: sortField,
                sortAscending: sortAscending,
                period: _period,
                dateFieldName: getDateFieldName(_eventName),
                includeEveryoneGroup: true
            }, 
            loadCompleteCallback,
            loadCompleteArgs
        );       
    }

    function deleteGroup(id) {
        
        showConfirmDialogWithCallback(
            '<%=WebTextManager.GetText("/pageText/users/groups/Manage.aspx/deleteSelectedGroupConfirm") %>',
            onDeleteGroupConfirm,
            337,
            200,
            '<%=WebTextManager.GetText("/pageText/users/groups/Manage.aspx/deleteSelectedGroup") %>',
            id
        );
    }

    function onDeleteGroupConfirm(id) {
        svcUserManagement.deleteUserGroup(_at, id, onGroupsDeleted, 1);
    }

    //Confirm handler for deleting selected groups
    function onDeleteSelectedGroupsConfirm(){
        var idArray = new Array();

        $('.deleteGroup:checked').each(function(index){
            idArray.push($(this).attr('value'));
        });       

        svcUserManagement.deleteUserGroups(_at, idArray, onGroupsDeleted, idArray.length);
    }

    //Groups deleted handler
    function onGroupsDeleted(resultData, count){
        resetGridMenu();
        reloadGroupList();
        <%if (!String.IsNullOrEmpty(OnGroupDeleted))
          {%>
            <%=OnGroupDeleted %>();
        <%
          }%>  

        <%if (!String.IsNullOrEmpty(ShowStatusMessageHandler))
          {%>
            var message = '';
            var success = null;
            if (resultData.length>0)
            {
                message += '<%=WebTextManager.GetText("/pageText/Users/Manage.aspx/groupsDeleted") %>';
                message = message.replace('{0}', resultData.length);
                success = true;
            }

            if (resultData.length != count)
            {
                if (message!='')
                    message +='<br />';
                message += '<%=WebTextManager.GetText("/pageText/Users/Manage.aspx/groupCouldNotDelete") %>'
                success = null;
            }

          <%=ShowStatusMessageHandler %>(message, success);
        <%
          }%>
    }

    //Copied user groups handler
    function onCopiedUserGroups(resultData){      
        var message = '<%=WebTextManager.GetText("/pageText/users/groups/Manage.aspx/copyGroupsSuccess") %>';

        reloadGroupList();
        resetGridMenu();

        <%if (!String.IsNullOrEmpty(ShowStatusMessageHandler))
            {%>
            <%=ShowStatusMessageHandler %>(message.replace('{0}', resultData.length), true);
        <%
            }%>   
          
    }

    var groupToSelect = null;

    //Reload group list
    function reloadGroupList(args){
        <%=_groupGrid.ReloadGridHandler %>(true);
        if(typeof(args) != 'undefined') {
            groupToSelect = {DatabaseId : args.newGroupId};
        }
    }

    //postponed selection of the added group
    function onGroupsGridRendered()
    {
        onGroupSelected(groupToSelect);        
        groupToSelect = null;
    }
    
    //Render Grid comlete handler
    function gridRenderComplete() {
        <%=_groupGrid.ShowSorter %>();
    }
</script>

<div class="gridMenu">
    <div class="right gridButtons">
        <a class="ckbxButton roundedCorners border999 shadow999 redButton" href="javascript:void(0);" id="_deleteSelectedGroupLink"><%=WebTextManager.GetText("/pageText/users/groups/Manage.aspx/deleteSelectedGroups")%></a>
        <a class="ckbxButton roundedCorners border999 shadow999 silverButton" href="javascript:void(0);" id="_copySelectedGroupLink"><%=WebTextManager.GetText("/pageText/users/groups/Manage.aspx/copySelectedGroups")%></a>
    </div>
    <br class="clear" />
</div>

<%-- Container for Results --%>
<ckbx:Grid ID="_groupGrid" runat="server" GridCssClass="ckbxGrid" RenderCompleteCallback="onGroupsGridRendered" />
    
<%-- Event Handling  --%>
<script type="text/C#" runat="server">
    
    /// <summary>
    /// Get/set handler for showing status message.
    /// The first parameter must be a message.
    /// The second parameter must determine if an operation was succeeded or not.
    /// </summary>
    public string ShowStatusMessageHandler { get; set; }
    
    /// <summary>
    /// Get/set callback for handling group delete event
    /// </summary>
    public string OnGroupDeleted { get; set; }
    
    /// <summary>
    /// Initialize grid control
    /// </summary>
    /// <param name="e"></param>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        _groupGrid.InitialSortField = "GroupName";
        _groupGrid.ItemClickCallback = GroupSelectedClientCallback;
        _groupGrid.ListTemplatePath = ResolveUrl("~/Users/jqtmpl/groupListTemplate.html");
        _groupGrid.ListItemTemplatePath = ResolveUrl("~/Users/jqtmpl/groupListItemTemplate.html");
        _groupGrid.LoadDataCallback = "loadGroupListAjax";
        _groupGrid.EmptyGridText = WebTextManager.GetText("/pageText/userGroups.aspx/noUserGroups");
        _groupGrid.IsAjaxScrollModeEnabled = true;
        _groupGrid.InitialFilterField = string.IsNullOrEmpty(Request.Params["term"]) ? "GroupName" : "";
        _groupGrid.FilterItemType = "user";
        _groupGrid.RenderCompleteCallback = "gridRenderComplete";
    }
    
    /// <summary>
    /// Override OnLoad to ensure necessary scripts are loaded.
    /// </summary>
    /// <param name="e"></param>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (Visible)
        {
            RegisterClientScriptInclude(
                "serviceHelper.js",
                ResolveUrl("~/Services/js/serviceHelper.js"));

            RegisterClientScriptInclude(
                "svcUserManagement.js",
                ResolveUrl("~/Services/js/svcUserManagement.js"));
        }
    }
</script>

