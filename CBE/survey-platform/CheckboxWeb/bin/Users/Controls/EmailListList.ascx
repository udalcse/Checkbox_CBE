<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="EmailListList.ascx.cs" Inherits="CheckboxWeb.Users.Controls.EmailListList" %>
<%@ Register TagName="Grid" TagPrefix="ckbx" Src="~/Controls/Grid.ascx" %>
<%@ Register TagPrefix="sort" Namespace="CheckboxWeb.Controls" assembly="CheckboxWeb"%>
<%@ Import Namespace="Checkbox.Management" %>
<%@ Import Namespace="Checkbox.Web" %>

<script type="text/javascript">
  <%-- Ensure service initialized --%>
    $(document).ready(function() {
        $(document).on('click', '.deleteEmailList', function(){
           toggleGridActionButtons('emailList', this);
        });
        $(document).on('click', '#_selectAllEmailLists', function(){
            var actionsAllowed = false;
            if ($(this).prop('checked')) {
                actionsAllowed = true;
                $('.deleteEmailList').prop('checked', true);
            } else {
                $('.deleteEmailList').prop('checked', false);
            }
            $.uniform.update('.deleteEmailList');
            toggleGridActionButtons('emailList', this, actionsAllowed);
        });

        //Bind delete selected emailListPanels click
        $('#_deleteSelectedEmailListLink').click(function(){
            if($('.deleteEmailList:checked').length > 0){
                showConfirmDialogWithCallback(
                    '<%=WebTextManager.GetText("/pageText/users/emaillists/manage.aspx/deleteSeveralConfirm") %>', 
                    onDeleteSelectedEmailListsConfirm,
                    337,
                    200,
                    '<%=WebTextManager.GetText("/pageText/users/emaillists/manage.aspx/deleteSelectedLists") %>'
                );
            }
        });

        //Bind copy selected emailListPanels click
        $('#_copySelectedEmailListLink').click(function(){
            var idArray = new Array();

            $('.deleteEmailList:checked').each(function(index){
                idArray.push($(this).attr('value'));
            });
            if (idArray.length > 0){           
                svcInvitationManagement.copyEmailListPanels(
                    _at,
                    idArray,
                    '<%=WebTextManager.GetUserLanguage() %>',
                    onCopiedEmailListPanels
                );
            }
        });
    });
     
    <%-- Load Email lists --%>
    function loadEmailList(currentPage, sortField, sortAscending, loadCompleteCallback, loadCompleteArgs){
        svcInvitationManagement.listViewableEmailPanels(
            _at, 
             {
                pageNumber: currentPage,
                pageSize: <%= ApplicationManager.AppSettings.PagingResultsPerPage %>,
                filterField: '',
                filterValue: '',
                sortField: sortField,
                sortAscending: sortAscending
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

    <%-- Load Email lists --%>
    function loadEmailListAjax(currentPage, sortField, sortAscending, filterField, filterValue, loadCompleteCallback, loadCompleteArgs ){        
        filterValue = (typeof(_lastSearchTerm) == 'undefined' || _lastSearchTerm == null || _lastSearchTerm.length == 0) ? filterValue : _lastSearchTerm;
        svcInvitationManagement.listViewableEmailPanels(
            _at, 
             {
                pageNumber: currentPage,
                pageSize: <%= ApplicationManager.AppSettings.PagingResultsPerPage %>,
                filterField: filterField,
                filterValue: filterValue,
                sortField: sortField,
                sortAscending: sortAscending,
                period: _period,
                dateFieldName: getDateFieldName(_eventName)
            }, 
            loadCompleteCallback,
            loadCompleteArgs
        );       
    }

    function deleteEmailList(id) {
        showConfirmDialogWithCallback(
            '<%=WebTextManager.GetText("/pageText/users/emaillists/manage.aspx/deleteConfirm") %>',
            onDeleteEmailListConfirm,
            337,
            200,
            '<%=WebTextManager.GetText("/pageText/users/emaillists/manage.aspx/deleteSelectedList") %>',
            id
        );
    }
    
    function onDeleteEmailListConfirm(id) {
        svcInvitationManagement.deleteEmailListPanel(_at, id, onEmailListsDeleted, 1);
    }

    //Confirm handler for deleting selected emailLists
    function onDeleteSelectedEmailListsConfirm(){
        var idArray = new Array();
            
        $('.deleteEmailList:checked').each(function(index){
            idArray.push($(this).attr('value'));
        });

        if (idArray.length > 0){
            svcInvitationManagement.deleteEmailListPanels(_at, idArray, onEmailListsDeleted, idArray.length);
        }
    }

    //EmailLists deleted handler
    function onEmailListsDeleted(resultData, count){      
        reloadEmailListList();
        resetGridMenu();
        <%if (!String.IsNullOrEmpty(OnEmailListDeleted))
          {%>
            <%=OnEmailListDeleted %>();
        <%
          }%>  

        <%if (!String.IsNullOrEmpty(ShowStatusMessageHandler))
          {%>
          var message = '<%=WebTextManager.GetText("/pageText/Users/Manage.aspx/emailListsDeleted") %>';
          <%=ShowStatusMessageHandler %>(message.replace('{0}', count), true);
        <%
          }%>
    }

    function copyEmailList(id) {
        svcInvitationManagement.copyEmailListPanel(_at, id, '<%=WebTextManager.GetUserLanguage() %>', onCopiedEmailListPanels);
    }

    //Copied emailLists handler
    function onCopiedEmailListPanels(resultData, count){  
        var message = '<%=WebTextManager.GetText("/pageText/users/emaillists/manage.aspx/copyEmailListsSuccess") %>';
        
        reloadEmailListList();
        resetGridMenu();

        <%if (!String.IsNullOrEmpty(ShowStatusMessageHandler))
            {%>
            <%=ShowStatusMessageHandler %>(message.replace('{0}', resultData.length), true);
        <%
            }%>      
    }

    var emailListToSelect = null;

    //Reload emailList list
    function reloadEmailListList(args){
        <%=_emailListGrid.ReloadGridHandler %>(true);
        if (!(typeof args==='undefined') && !(typeof args.newEmailListId==='undefined') && args.newEmailListId!=null){
            emailListToSelect = {DatabaseId : args.newEmailListId};
        }
    }

    //postponed selection of the added email list
    function onEMailListGridRendered()
    {
        //onEmailListSelected(emailListToSelect);        
        groupToSelect = null;
    }
    
    //Render Grid comlete handler
    function gridRenderComplete() {
        <%=_emailListGrid.ShowSorter %>();
    }
</script>


<div class="gridMenu">
    <div class="right gridButtons">
        <div class="itemActionMenu">
            <a class="ckbxButton roundedCorners border999 shadow999 redButton" href="javascript:void(0);" id="_deleteSelectedEmailListLink"><%=WebTextManager.GetText("/pageText/users/emaillists/manage.aspx/deleteSelectedLists")%></a>
            <a class="ckbxButton roundedCorners border999 shadow999 silverButton" href="javascript:void(0);" id="_copySelectedEmailListLink"><%=WebTextManager.GetText("/pageText/users/emaillists/manage.aspx/copySelectedLists")%></a>
        </div>
    </div>
    <br class="clear" />
</div>

<%-- Grid Control --%>
<ckbx:Grid ID="_emailListGrid" runat="server" GridCssClass="ckbxGrid" RenderCompleteCallback="onEMailListGridRendered"  />

<%-- Event Handling  --%>
<script type="text/C#" runat="server">
    
    /// <summary>
    /// Get/set handler for showing status message.
    /// The first parameter must be a message.
    /// The second parameter must determine if an operation was succeeded or not.
    /// </summary>
    public string ShowStatusMessageHandler { get; set; }
    
    /// <summary>
    /// Get/set callback for handling emailList delete event
    /// </summary>
    public string OnEmailListDeleted { get; set; }
    
    /// <summary>
    /// Initialize grid control
    /// </summary>
    /// <param name="e"></param>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        _emailListGrid.InitialSortField = "Name";
        _emailListGrid.ItemClickCallback = EmailListSelectedClientCallback;
        _emailListGrid.ListTemplatePath = ResolveUrl("~/Users/jqtmpl/emailListTemplate.html");
        _emailListGrid.ListItemTemplatePath = ResolveUrl("~/Users/jqtmpl/emailListItemTemplate.html");
        _emailListGrid.LoadDataCallback = "loadEmailListAjax";
        _emailListGrid.EmptyGridText = WebTextManager.GetText("/pageText/users/emaillists/manage.aspx/noEmailListFound");
        _emailListGrid.IsAjaxScrollModeEnabled = true;
        _emailListGrid.InitialFilterField = string.IsNullOrEmpty(Request.Params["term"]) ? "Name" : "";
        _emailListGrid.FilterItemType = "user";
        _emailListGrid.RenderCompleteCallback = "gridRenderComplete";
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
                "svcInvitationManagement.js",
                ResolveUrl("~/Services/js/svcInvitationManagement.js"));
        }
    }
</script>
