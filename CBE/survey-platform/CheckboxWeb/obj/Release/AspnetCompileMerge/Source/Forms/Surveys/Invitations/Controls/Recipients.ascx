<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Recipients.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Invitations.Controls.Recipients" %>
<%@ Import Namespace="Checkbox.Management" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Register TagPrefix="ckbx" TagName="Grid" Src="~/Controls/Grid.ascx" %>

<script type="text/javascript">    
    var _liveSearchRecipients;

    //
    function reloadRecipientsGrid() {
        //Reload grid
        <%=_recipientsGrid.ReloadGridHandler %>(true);
    }

    // Used to determine whether or not to show action
    // buttons above a left-column grid of items
    function toggleGridActionButtons(itemType, eventItem, show) {
        var itemTypeCap = itemType.substr(0, 1).toUpperCase() + itemType.substr(1);
        var $grid = $(eventItem).closest('.ckbxInvitationWizardRecipientsGrid').length > 0 ? $(eventItem).closest('.ckbxInvitationWizardRecipientsGrid') : $(eventItem).closest('.ckbxRecipientsGrid');
        var $menu = $grid.parent().find('.gridMenu');
        if ($menu.length > 0) {
            show = (typeof show != 'undefined') ? show : ($grid.find('.delete' + itemTypeCap + ':checked').length > 0 || $grid.find('.select' + itemTypeCap + ':checked').length > 0);
            if (show) {
                $menu.addClass('actions-available');
                $('#btnContainer').show();
            }
            else {
                $menu.removeClass('actions-available');
                $('#btnContainer').hide();
            }
        }
    }

    <%-- Ensure service initialized --%>
    $(document).ready(function() {
        _liveSearchRecipients = new LiveSearchObj('searchRecipients', 'recipients');

        //implementation all/none selection
        $(document).on('click', '.selectRecipient', function(){
           toggleGridActionButtons('recipient', this);
        });
        $(document).on('click', '#_selectAllRecipients', function(){
            var actionsAvailable = false;
            if ($(this).prop('checked')) {
                actionsAvailable = true;
                $('.selectRecipient').prop('checked', true);
            } else {
                $('.selectRecipient').prop('checked', false);
            }
            $.uniform.update('.selectRecipient');
            toggleGridActionButtons('recipient', this, actionsAvailable);
        });

        $('#<%=_filterList.ClientID %>').bind('change', onFilterChange);

        updateButtonVisibility();

        //Select all recipients
        $(document).on('click', '#_selectAllRecipients', function(){
            if ($(this).prop('checked'))
                $('.selectRecipient').prop('checked', true);
            else
                 $('.selectRecipient').prop('checked', false);
            $.uniform.update('.selectRecipient');
        });

        // Remove
         $('#removeButton').bind('click', function(){
            if($('.selectRecipient:checked').length > 0){
                showConfirmDialogWithCallback(
                    '<%=WebTextManager.GetText("/pageText/forms/surveys/invitations/recipients.aspx/removeConfirmation") %>', 
                    onRemoveConfirm,
                    390,
                    240,
                    '<%=WebTextManager.GetText("/pageText/forms/surveys/invitations/recipients.aspx/removeTitle") %>'
                );
            }
        });

        // Opt Out
        $('#markOptOutButton').bind('click', function(){
            if($('.selectRecipient:checked').length > 0){
                showConfirmDialogWithCallback(
                    '<%=WebTextManager.GetText("/pageText/forms/surveys/invitations/recipients.aspx/optOutConfirmation") %>', 
                    onOptOutConfirm,
                    337,
                    200,
                    '<%=WebTextManager.GetText("/pageText/forms/surveys/invitations/recipients.aspx/optOutTitle") %>'
                );
            }
        });

        // Responded
        $('#markRespondedButton').bind('click', function(){
            if($('.selectRecipient:checked').length > 0){
                showConfirmDialogWithCallback(
                    '<%=WebTextManager.GetText("/pageText/forms/surveys/invitations/recipients.aspx/respondedConfirmation") %>', 
                    onRespondedConfirm,
                    337,
                    200,
                    '<%=WebTextManager.GetText("/pageText/forms/surveys/invitations/recipients.aspx/respondedTitle") %>'
                );
            }
        });
    });
     
    <%-- Load Recipient list --%>
    function loadCurrentRecipientList(currentPage, sortField, sortAscending, loadCompleteCallback, loadCompleteArgs, filterValue ) {
        svcInvitationManagement.listInvitationRecipients(
            _at, 
            <%=InvitationId %>,
            $('#<%=_filterList.ClientID %>').val(),
            filterValue,
            currentPage,
            <%=ApplicationManager.AppSettings.PagingResultsPerPage %>,
            loadCompleteCallback,
            loadCompleteArgs
        ); 
    }

    <%-- Hide Columns --%>
    function hideColumns() {
        if ($("#<%=_filterList.ClientID %>").val() == 'OptOut') {
            $('.messages-sent-count').hide();
            $('.opt-out-details').show();
        } else {
            $('.messages-sent-count').show();
            $('.opt-out-details').hide();
        }
    }

    <%-- Reload grid on filter change --%>
    function onFilterChange(){
        $('#btnContainer').hide();
        $('.gridMenu').removeClass('actions-available');
        updateButtonVisibility();
        reloadRecipientsGrid();
    }

    //
    function updateButtonVisibility(){
        if($('#<%=_filterList.ClientID %>').val() == '<%= PendingValue %>')
        {
            $('#markRespondedButton').hide();
            $('#markOptOutButton').hide();
            $('#resendButton').hide();
        }
        else{
            if($('#<%=_filterList.ClientID %>').val() == 'Responded')
            {
                $('#markRespondedButton').hide();
                $('#markOptOutButton').show();
            } else
            if($('#<%=_filterList.ClientID %>').val() == 'OptOut')
            {
                $('#markRespondedButton').show();
                $('#markOptOutButton').hide();
            }
            else
            {
                $('#markRespondedButton').show();
                $('#markOptOutButton').show();
                $('#resendButton').show();
            }
        }
    }

    //
    function onOptOutConfirm(args){
        if(args.success){
            var recipients = getSelectedRecipientArray();

            svcInvitationManagement.markRecipientsOptedOut(
                _at,
                <%=InvitationId %>,
                recipients.ids,
                function(){
                    statusControl.initialize('recipientStatusContainer');
                    statusControl.showStatusMessage(
                        '<%=WebTextManager.GetText("/pageText/forms/surveys/invitations/recipients.aspx/optOutSuccessful") %>'.replace('{0}', recipients.ids.length),
                        StatusMessageType.success);

                    onFilterChange();
                }
            );
        }
    }

    //
    function onRespondedConfirm(args){
        if(args.success){
             var recipients = getSelectedRecipientArray();

            svcInvitationManagement.markRecipientsResponded(
                _at,
                <%=InvitationId %>,
                recipients.ids,
                function(){
                    statusControl.initialize('recipientStatusContainer');
                    statusControl.showStatusMessage(
                        '<%=WebTextManager.GetText("/pageText/forms/surveys/invitations/recipients.aspx/respondedSuccessful") %>'.replace('{0}', recipients.ids.length),
                        StatusMessageType.success);

                    onFilterChange();
                }
            );

            reloadRecipientsGrid();
        }
    }

    //
    function onRemoveConfirm(args){
        if(args.success){
             var recipients = getSelectedRecipientArray();

             if($('#<%=_filterList.ClientID %>').val() == '<%= PendingValue %>')
             {
                svcInvitationManagement.removePendingRecipients(
                    _at,
                    <%=InvitationId %>,
                    recipients.userNames,
                    recipients.emails,
                    recipients.groupIds,
                    recipients.emailListIds,
                    function(){
                        statusControl.initialize('recipientStatusContainer');
                        statusControl.showStatusMessage(
                            '<%=WebTextManager.GetText("/pageText/forms/surveys/invitations/recipients.aspx/removeSuccessful") %>'.replace('{0}', recipients.userNames.length),
                            StatusMessageType.success);

                        reloadRecipientsGrid();
                        reloadGrids();

                        <%if (!String.IsNullOrEmpty(OnRecipientsDeleted))
                          {%>
                          <%=OnRecipientsDeleted %>();
                        <%
                          }%>
                    }
                );
             }
             else{
                svcInvitationManagement.removeRecipients(
                    _at,
                    <%=InvitationId %>,
                    recipients.ids,
                    function(){
                        statusControl.initialize('recipientStatusContainer');
                        statusControl.showStatusMessage(
                            '<%=WebTextManager.GetText("/pageText/forms/surveys/invitations/recipients.aspx/removeSuccessful") %>'.replace('{0}', recipients.ids.length),
                            StatusMessageType.success);

                        reloadRecipientsGrid();

                        <%if (!String.IsNullOrEmpty(OnRecipientsDeleted))
                          {%>
                          <%=OnRecipientsDeleted %>();
                        <%
                          }%>
                    }
                );
            }

            reloadRecipientsGrid();
        }
    }

    //
    function getSelectedRecipientArray(){
        var recipientIdArray = new Array();
        var recipientEmailArray = new Array();
        var recipientGroupIdArray = new Array();
        var recipientUserNameArray = new Array();
        var recipientEmailListArray = new Array();
        
        $('.selectRecipient:checked').each(function(index){
            recipientIdArray.push($(this).attr('recipientId'));
            recipientEmailArray.push($(this).attr('recipientEmail'));
            recipientGroupIdArray.push($(this).attr('groupId'));
            recipientUserNameArray.push($(this).attr('recipientUserName').replace("'", "&#39;"));
            recipientEmailListArray.push($(this).attr('emaillistid'));
        });

        return {ids:recipientIdArray, emails:recipientEmailArray,userNames:recipientUserNameArray, groupIds: recipientGroupIdArray, emailListIds: recipientEmailListArray};
    }

    function expandGroupList(groupId) {
        var members = $('hidden.selectRecipient[groupId="'+groupId+'"]').parent().parent();
        if(members.is(':visible')) {
            var collapser = $('.collapser[groupId="'+groupId+'"]');
            collapser.removeClass('collapser');
            collapser.addClass('expander');
            members.hide();
        }
        else {
            var expander = $('.expander[groupId="'+groupId+'"]');
            expander.removeClass('expander');
            expander.addClass('collapser');
            members.show();
        }
    }
    
    function expandEmailList(emailListId) {
        var members = $('hidden.selectRecipient[emailListId="'+emailListId+'"]').parent().parent();
        if(members.is(':visible')) {
            var collapser = $('.collapser[emailListId="'+emailListId+'"]');
            collapser.removeClass('collapser');
            collapser.addClass('expander');
            members.hide();
        }
        else {
            var expander = $('.expander[emailListId="'+emailListId+'"]');
            expander.removeClass('expander');
            expander.addClass('collapser');
            members.show();
        }
    }
         
</script>

<div id="recipientStatusContainer" style="position:absolute;top:200px;left:200px;"></div>

<!-- Grid Actions -->
<div class="gridMenu clearfix">
    <div class="gridButtons">
        <!--<a class="ckbxButton orangeButton" id="resendButton"><%=WebTextManager.GetText("/pageText/forms/surveys/invitations/recipients.aspx/resendSelected")%></a>-->
        <a class="ckbxButton redButton" id="removeButton"><%=WebTextManager.GetText("/pageText/forms/surveys/invitations/recipients.aspx/removeSelected")%></a>
    </div>
    <div id="searchRecipients" class="groupMembersFilter right">
        <asp:TextBox runat="server" Width="150px" autocomplete="off" />
        <ckbx:MultiLanguageImageButton runat="server" CssClass="liveSearch-applyBtn" SkinID="ACLFilterOn" ToolTipTextId="/controlText/grantAccessControl.ascx/filterOnTip" />
        <ckbx:MultiLanguageImageButton runat="server" CssClass="liveSearch-cancelBtn" SkinID="ACLFilterOff" ToolTipTextId="/controlText/grantAccessControl.ascx/filterOffTip"/>
    </div>
    <asp:Panel ID="_filterPanel" runat="server" CssClass="filter-container">
        <span style=""><%= WebTextManager.GetText("/pageText/forms/surveys/invitations/recipients.aspx/filterTitle")%></span><ckbx:MultiLanguageDropDownList ID="_filterList" runat="server" style="font-size:16px;">
            <asp:ListItem TextId="/enum/recipientFilter/pending" Value="Pending" />
            <asp:ListItem TextId="/enum/recipientFilter/current" Value="Current" />
            <asp:ListItem TextId="/enum/recipientFilter/responded" Value="Responded" />
            <asp:ListItem TextId="/enum/recipientFilter/notResponded" Value="NotResponded" />
            <asp:ListItem TextId="/enum/recipientFilter/optOut" Value="OptOut" />
            <asp:ListItem TextId="/enum/recipientFilter/bounced" Value="Bounced" />
            <%-- <asp:ListItem TextId="/enum/recipientFilter/deleted" Value="Deleted" /> --%>
        </ckbx:MultiLanguageDropDownList>
    </asp:Panel>
</div>

<div id="searchedForDiv" style="display:none;font-size:10px;font-weight:bold;">
    <span id="searchedForText"></span>
    &nbsp;&nbsp;
    <a id="clearSearchLink" href="javascript:void(0);"><%=WebTextManager.GetText("/pageText/forms/surveys/invitations/recipients.aspx/clearSearch")%></a>
</div>

<%-- Container for Grid --%>
<ckbx:Grid ID="_recipientsGrid" runat="server" GridCssClass="ckbxRecipientsGrid" />

<ckbx:MultiLanguageHyperLink ID="_returnLink" runat="server" Visible="false" CssClass="ckbxButton roundedCorners border999 shadow999 redButton" TextId="/pageText/forms/surveys/invitations/recipients.aspx/returnToSend" />

<%if(!PendingInvitationMode){ %>
<div id="btnContainer" style="display:none"> 
    <a class="ckbxButton silverButton" id="markRespondedButton"><%=WebTextManager.GetText("/pageText/forms/surveys/invitations/recipients.aspx/markResponded")%></a>
    <a class="ckbxButton silverButton" id="markOptOutButton"><%=WebTextManager.GetText("/pageText/forms/surveys/invitations/recipients.aspx/markOptOut")%></a>
</div>
<% } %>
<%if (PendingInvitationMode){ %>
    <a class="ckbxButton silverButton view-pending-recipients" href="javascript:void(0);"><%=WebTextManager.GetText("/pageText/forms/surveys/invitations/AddRecipients.aspx/backToRecipientList")%></a>
<% } %>


<script type="text/C#" runat="server">

    /// <summary>
    /// Get/set callback for recipients deleted event
    /// </summary>
    public String OnRecipientsDeleted { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="e"></param>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        _recipientsGrid.InitialSortField = "EmailToAddress";
        _recipientsGrid.ListTemplatePath = 
            PendingInvitationMode
                ? ResolveUrl("~/Forms/Surveys/Invitations/jqtmpl/pendingRecipientEditorListTemplate.html")
                : ResolveUrl("~/Forms/Surveys/Invitations/jqtmpl/recipientEditorListTemplate.html");
        
        _recipientsGrid.ListItemTemplatePath = 
            PendingInvitationMode
                ? ResolveUrl("~/Forms/Surveys/Invitations/jqtmpl/pendingRecipientEditorListItemTemplate.html")
                : ResolveUrl("~/Forms/Surveys/Invitations/jqtmpl/recipientEditorListItemTemplate.html");
        _recipientsGrid.LoadDataCallback = "loadCurrentRecipientList";
        _recipientsGrid.EmptyGridText = WebTextManager.GetText("/controlText/invitationDashboard.ascx/noRecipients");
        _recipientsGrid.RenderCompleteCallback = "hideColumns";
        _recipientsGrid.FilterItemType = "recipients";
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="e"></param>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        RegisterClientScriptInclude(
          "serviceHelper.js",
          ResolveUrl("~/Services/js/serviceHelper.js"));

        RegisterClientScriptInclude(
            "svcInvitationManagement.js",
            ResolveUrl("~/Services/js/svcInvitationManagement.js"));
        
        RegisterClientScriptInclude(
            "jquery.localize.js",
            ResolveUrl("~/Resources/jquery.localize.js"));
        
        RegisterClientScriptInclude(
            "dateUtils.js",
            ResolveUrl("~/Resources/dateUtils.js"));

        //Moment.js: datetime utilities
        RegisterClientScriptInclude(
            "moment.js",
            ResolveUrl("~/Resources/moment.js"));

        RegisterClientScriptInclude(
            "statusControl.js",
            ResolveUrl("~/Resources/statusControl.js"));

        RegisterClientScriptInclude(
            "gridLiveSearch.js",
            ResolveUrl("~/Resources/gridLiveSearch.js"));
    }

</script>
