﻿<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="InvitationList.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Invitations.Controls.InvitationList" %>
<%@ Register TagPrefix="ckbx" TagName="Grid" Src="~/Controls/Grid.ascx" %>
<%@ Register TagPrefix="sort" TagName="Sorter" Src="~/Controls/Sorter.ascx" %>
<%@ Register TagPrefix="sort" Namespace="CheckboxWeb.Controls" Assembly="CheckboxWeb" %>
<%@ Import Namespace="Checkbox.Management" %>
<%@ Import Namespace="Checkbox.Web" %>

<script type="text/javascript">
    $(document).ready(function() {
        //implementation all/none selection
        $(document).on('click', '.deleteInvitation', function(event){
            toggleGridActionButtons('invitation', this);
            event.stopPropagation();
        });
        $(document).on('click', '#_selectAllInvitations', function(){
            var actionsAvailable = false;
            if ($(this).prop('checked')) {
                actionsAvailable = true;
                $('.deleteInvitation').prop('checked', true);
            } else {
                $('.deleteInvitation').prop('checked', false);
            }
            $.uniform.update('.deleteInvitation');
            toggleGridActionButtons('invitation', this, actionsAvailable);
        });


        //Bind delete selected users click
        $('#_deleteSelectedInvitationsLink').click(function(){
            if($('.deleteInvitation:checked').length > 0){
                showConfirmDialogWithCallback(
                    '<%=WebTextManager.GetText("/pageText/forms/surveys/invitations/manage.aspx/deleteSelectedInvitationConfirm") %>', 
                    onDeleteSelectedInvitationsConfirm,
                    337,
                    200,
                    '<%=WebTextManager.GetText("/pageText/forms/surveys/invitations/manage.aspx/deleteSelectedInvitation") %>'
                );
            }
        });
        //set the default value of invitation filter
        $('#filter').val('');
        //hide scheduled filter item if invitation scheduler is being used
        <%if(!ApplicationManager.AppSettings.MSSMode.Equals("SES"))
          {%>
            $('#scheduledOption').hide();
        <% } %>
        
        $('#filter').change( function() {
            gridHelper_<%=_invitationGrid.ClientID %>.filterKey = $('#filter').val();
            reloadInvitationList();
        });
    });

    //Reload list
    function reloadInvitationList() {
        <%=_invitationGrid.ReloadGridHandler %>();
    }
     
    <%-- Load survey invitation list --%>
    function loadInvitationList(currentPage, sortField, sortAscending, loadCompleteCallback, loadCompleteArgs, filterKey){
        svcInvitationManagement.listInvitationsForSurvey(
            _at, 
            <%=SurveyId %>, 
             {
                pageNumber: currentPage,
                pageSize: <%= ApplicationManager.AppSettings.PagingResultsPerPage %>,
                filterField: '',
                filterValue: _term,
                sortField: sortField,
                sortAscending: sortAscending
            }, 
            loadCompleteCallback,
            loadCompleteArgs,
            filterKey
        );
    }
    
    function onDeleteSelectedInvitationsConfirm(args){
        if(args.success){
            var idArray = new Array();

            $('.deleteInvitation:checked').each(function(index){
                var inv_id = $(this).attr('value');
                
                svcInvitationManagement.deleteInvitation(_at, inv_id, invitationDeletedCallback, null);

                //idArray.push(inv_id);
            });

//            if (idArray.length > 0){
//                svcInvitationManagement.deleteInvitations(
//                     _at,
//                     idArray,
//                     true,
//                     onInvitationsDeleted,
//                     idArray.length
//                 );
//            }
        }//if(args.success){
    }

    function onInvitationsDeleted(resultData, count) {
        reloadInvitationList();
        <%if (!String.IsNullOrEmpty(OnInvitationDeleted))
          {%>
            <%=OnInvitationDeleted %>();
        <%
          }%>  

        <%if (!String.IsNullOrEmpty(ShowStatusMessageHandler))
          {%>
          var message = '<%=WebTextManager.GetText("/pageText/forms/surveys/invitations/manage.aspx/invitationsDeleted") %>';
          <%=ShowStatusMessageHandler %>(message.replace('{0}', count), true);
        <%
          }%> 
    }

    function invitationsRendered()
    {
        if (_selected && _selected != '0')
        {
            loadInvitationData(_selected);
            $('.rightPanel').show();
        }
    }
    
    //Render Grid comlete handler
    function gridRenderComplete() {
        <%=_invitationGrid.ShowSorter %>();
        invitationsRendered();
    }
</script>

<div class="gridMenu clearfix">
    <div class="left gridSorter">
        <div class="gridFilter">
            <span>Filter </span><select name="filter" ID="filter" class="gridActions border999 shadow999 roundedCorners">
                <option value="">All</option>
                <option value="drafts">Drafts</option>
                <option value="scheduled" id="scheduledOption">Scheduled</option>
                <option value="sent">Sent</option>
            </select>
        </div>
    </div>
    <div class="gridButtons itemActionMenu" style="float: right; margin: 0 5px 0 0;">
        <a class="cancelButton" style="text-decoration: underline; padding-right:10px;" href="#" id="_deleteSelectedInvitationsLink">
                <%=WebTextManager.GetText("/pageText/forms/surveys/invitations/manage.aspx/deleteSelectedInvitation")%></a>
    </div>
</div>

<ckbx:Grid ID="_invitationGrid" runat="server" GridCssClass="ckbxGrid"/>

<%-- Event Handling  --%>
<script type="text/C#" runat="server">
    
    /// <summary>
    /// Get/set handler for showing status message.
    /// The first parameter must be a message.
    /// The second parameter must determine if an operation was succeeded or not.
    /// </summary>
    public string ShowStatusMessageHandler { get; set; }

    /// <summary>
    /// Get/set callback for handling user delete event
    /// </summary>
    public string OnInvitationDeleted { get; set; }

    /// <summary>
    /// Initialize grid control
    /// </summary>
    /// <param name="e"></param>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        _invitationGrid.InitialSortField = "Name";
        _invitationGrid.ItemClickCallback = InvitationSelectedClientCallback;
        if (ApplicationManager.AppSettings.MSSMode.Equals("SES"))
        {
            _invitationGrid.ListTemplatePath = ResolveUrl("~/Forms/Surveys/Invitations/jqtmpl/scheduledInvitationListTemplate.html");
            _invitationGrid.ListItemTemplatePath = ResolveUrl("~/Forms/Surveys/Invitations/jqtmpl/scheduledInvitationListItemTemplate.html");
        }
        else
        {
            _invitationGrid.ListTemplatePath = ResolveUrl("~/Forms/Surveys/Invitations/jqtmpl/invitationListTemplate.html");
            _invitationGrid.ListItemTemplatePath = ResolveUrl("~/Forms/Surveys/Invitations/jqtmpl/invitationListItemTemplate.html");
        }
        _invitationGrid.LoadDataCallback = "loadInvitationList";
        _invitationGrid.EmptyGridText = WebTextManager.GetText("/pageText/forms/surveys/invitations/manage.aspx/noInvitationsFound");
        _invitationGrid.RenderCompleteCallback = "gridRenderComplete";
    }

     /// <summary>
    /// Override OnLoad to ensure necessary scripts are loaded.
    /// </summary>
    /// <param name="e"></param>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        RegisterClientScriptInclude(
           "serviceHelper.js",
           ResolveUrl("~/Services/js/serviceHelper.js"));

        RegisterClientScriptInclude(
         "templateHelper.js",
         ResolveUrl("~/Resources/templateHelper.js"));

        RegisterClientScriptInclude(
           "svcInvitationManagement.js",
           ResolveUrl("~/Services/js/svcInvitationManagement.js"));

        RegisterClientScriptInclude(
            "dateUtils.js",
            ResolveUrl("~/Resources/dateUtils.js"));

        //Moment.js: datetime utilities
        RegisterClientScriptInclude(
            "moment.js",
            ResolveUrl("~/Resources/moment.js"));

        RegisterClientScriptInclude(
          "grid.js",
          ResolveUrl("~/Resources/grid.js"));

        RegisterClientScriptInclude(
         "jquery.pager.js",
         ResolveUrl("~/Resources/jquery.pager.js"));
    }
</script>
