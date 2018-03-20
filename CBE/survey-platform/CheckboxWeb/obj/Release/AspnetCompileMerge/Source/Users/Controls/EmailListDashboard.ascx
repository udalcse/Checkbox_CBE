<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="EmailListDashboard.ascx.cs" Inherits="CheckboxWeb.Users.Controls.EmailListDashboard" %>
<%@ Import Namespace="Checkbox.Management" %>
<%@ Import Namespace="Checkbox.Web" %>

<script type="text/javascript">
    var _dashEmailListPanelId = null;

    <%-- Ensure services initialized --%>
    $(document).ready(function () {
        //Precompile templates used
        templateHelper.loadAndCompileTemplate('<%=ResolveUrl("~/Users/jqtmpl/emailListDashboardTemplate.html") %>', 'emailListDashboardTemplate.html');
        templateHelper.loadAndCompileTemplate('<%=ResolveUrl("~/Users/jqtmpl/emailAddressesListTemplate.html") %>', 'emailAddressesListTemplate.html');
        templateHelper.loadAndCompileTemplate('<%=ResolveUrl("~/Users/jqtmpl/emailAddressesListItemTemplate.html") %>', 'emailAddressesListItemTemplate.html');
        templateHelper.loadAndCompileTemplate('<%=ResolveUrl("~/Users/jqtmpl/emailListItemTemplate.html") %>', 'emailListItemTemplate.html');

        $(document).on('click', '#_deleteSelectedEmailAddressesLink', function(){
            if ($('.deleteEmailAddress:checked').length > 0){
                var addressArray = new Array();

                $('.deleteEmailAddress:checked').each(function(index){
                    addressArray.push($(this).attr('value'));
                });

                if (addressArray.length > 0){
                    svcInvitationManagement.removeEmailAddressesFromEmailListPanel(
                        _at,
                        _dashEmailListPanelId,
                        addressArray,
                        function(){
                            loadEmailListPanelData(_dashEmailListPanelId, true);
                            <%if (!String.IsNullOrEmpty(ShowStatusMessageHandler))
                            {%>
                            var message = '<%=WebTextManager.GetText("/pageText/Users/emailLists/addresses.aspx/usersDeleted") %>';
                            <%=ShowStatusMessageHandler %>(message.replace('{0}', addressArray.length), true);
                        <%
                            }%>
                             <%if(!String.IsNullOrEmpty(EmailListUpdateHandler))
                              {%>
                              <%=EmailListUpdateHandler %>();
                            <%
                              }%>
                        }
                    );
                }
            }
        });

        //Select all/none implementation
        $(document).on('click', '#_selectAllEmailAddresses', function(){
            if ($(this).prop('checked'))
                $('.deleteEmailAddress').prop('checked', 'checked');
            else
                $('.deleteEmailAddress').removeAttr('checked');
            $.uniform.update('.deleteEmailAddress');
        });
    });

    <%-- Expose Method to Load EmailListPanel --%>
    function loadEmailListPanelData(emailListPanelId, reloadListData)
    {
        if (emailListPanelId == null || emailListPanelId == ''){
            return;
        }

        $('#infoPlace').empty();
        $('#infoPlace').hide();
        $('#detailProgressContainer_<%=ID %>').show();

        _dashEmailListPanelId = emailListPanelId;

        svcInvitationManagement.getEmailListPanelInfoD(_at, emailListPanelId, {reloadListData: reloadListData, emailListPanelId: emailListPanelId, at: _at})
            .then(
                function(emailListResult, emailListResultArgs){
                    authorizeEmailList(emailListResult, emailListResultArgs)
                        .then(
                            function(securityResultData, securityResultArgs){
                                emailListResultArgs.allowEdit = securityResultData;
                                OnEmailListPanelInfoLoaded(emailListResult, emailListResultArgs);
                                applyEmailListTemplate(emailListResult, emailListResultArgs)
                                    .then(
                                        function(){
                                            loadEmailListAddresses(emailListResult, emailListResultArgs)
                                                .then(onEmailListPanelLoaded);
                                        }
                                    )
                                    .then(
                                        function(){
                                            onEmailListPanelTemplateApplied(emailListResult, emailListResultArgs);
                                        }
                                    );
                            }
                        );
                }
            );

        //svcInvitationManagement.getEmailListPanelInfo(_at, emailListPanelId, OnEmailListPanelInfoLoaded, {reloadListData: reloadListData, emailListPanelId: emailListPanelId, at: _at});
    }

    <%-- Authorize email list result --%>
    function authorizeEmailList(resultData, args){
        if(resultData == null){
            return;
        }

        return svcAuthorization.authorizeAccessD('<%= WebUtilities.GetCurrentUserEncodedName()%>', svcAuthorization.RESOURCETYPE_EMAILLIST, args.emailListPanelId, 'EmailList.Edit', args);
    }

    <%-- Apply email list panel template --%>
    function applyEmailListTemplate(resultData, args){
        resultData.allowEdit = args.allowEdit;
     
        //Start loading the Dashboard
        return templateHelper.loadAndApplyTemplateD(
            'emailListDashboardTemplate.html',
            '<%=ResolveUrl("~/Users/jqtmpl/emailListDashboardTemplate.html") %>',
            resultData,
            null,
            'infoPlace',
            true
        );
    }

    <%-- Display Load Error for NULL Data --%>
    function OnEmailListPanelInfoLoaded(resultData, args){
        if (resultData == null) {
            $('#infoPlace').html('<div class="error message" style="margin:15px;padding:5px;">Unable to load information about email list panel with id: ' + args.emailListPanelId + '.</div>');
            return;
        }
     

        <%-- Reload EmailListList, if necessary --%>
        if(args.reloadListData) {
             //Pulsate
             $('#emailList_' + resultData.DatabaseId).effect(
                'shake', 
                {
                    times:2,
                    distance:10,
                    duration:250
                },
                function(){
                    //Required to remove jagged text left behind in IE for some reason
                    // when pulsate is called.
                    if (typeof(this.style.removeAttribute) != 'undefined' && this.style.removeAttribute != null)
                        this.style.removeAttribute('filter');
                    
                    var index = $('#emailListContainer_' + resultData.DatabaseId).attr('index');
                    if (typeof(index)=='undefined' || index == null)
                        return;

                    //Apply template
                    templateHelper.loadAndApplyTemplate(
                        'emailListItemTemplate.html',
                        '<%=ResolveUrl("~/Users/jqtmpl/emailListItemTemplate.html") %>',
                        resultData,
                        {index: index},
                        'emailListContainer_' + resultData.DatabaseId,
                        true)
                });
        }
    }

    var addressesPage = -1;

    <%-- Load Email List Addresses --%>
    function loadEmailListAddresses(resultData, args){
        
        if (addressesPage == -1)
        {
            addressesPage = 1;
        }

        $('#emailAddressesPlace').html($('#detailProgressContainer_<%=ID %>').html());

       //Load address list
        return svcInvitationManagement.listEmailListPanelAddressesD(
            _at,
            resultData.DatabaseId,
            addressesPage,
            <%=ApplicationManager.AppSettings.PagingResultsPerPage %>,
            args
        );
    }



    <%-- Apply Security --%>
    function onEmailListPanelTemplateApplied(resultData, args){
        return securityHelper.protect(
            '<%=WebUtilities.GetCurrentUserEncodedName() %>',
            svcAuthorization.RESOURCETYPE_EMAILLIST,
            _dashEmailListPanelId,
            '#infoPlace');
    }

    function onEmailListRendered() {
        $('select, input:checkbox, input:radio, input:text').filter(':not([uniformIgnore])').uniform();
    }

    //EmailListPanel loaded - render it.
    function onEmailListPanelLoaded(resultData, args){
        var itemCount = resultData.TotalItemCount;

        //The dashboard should be shown before rendering the grid, because 
        //at the end of the rendering 'resizePanels' function is invoked. An this function requires
        //the dashboard to be shown.
        $('#detailProgressContainer_<%=ID %>').hide();
        $('#infoPlace').show();

        //Render grid
        gridHelper.renderGrid(
            resultData.ResultPage,
            'emailAddressesPlace',
            'emailAddressesListTemplate.html',
            '<%=ResolveUrl("~/Users/jqtmpl/emailAddressesListTemplate.html") %>',
            {allowEdit: args.allowEdit},
            onEmailListRendered
        );

        //Hide pager if not necessary
        if(itemCount == 0
            || itemCount <  <%= ApplicationManager.AppSettings.PagingResultsPerPage %>){
            $('#gridPaginationTopContainer').hide();
            $('#gridPaginationBottomContainer').hide();
        }else{
            $('#gridPaginationTopContainer').show();
            $('#gridPaginationBottomContainer').show();

            $('#paginationTop').pager({
                totalItems:itemCount,
                currentPage: addressesPage,
                pageSize: <%= ApplicationManager.AppSettings.PagingResultsPerPage %>,
                pageChanged: function(newPage){
                    
                    gridHelper.currentPage = newPage; 
                    addressesPage = newPage;

                    loadEmailListPanelData(_dashEmailListPanelId, true);

                }
            });

             $('#paginationBottom').pager({
                totalItems:itemCount,
                currentPage: addressesPage,
                pageSize:  <%= ApplicationManager.AppSettings.PagingResultsPerPage %>,
                pageChanged: function(newPage){

                    gridHelper.currentPage = newPage; 
                    addressesPage = newPage;

                    loadEmailListPanelData(_dashEmailListPanelId, true);
                }
            });
        }
    }

    //Determine if the window is a confirm window
    function checkConfirmDialog(window){
        var re = new RegExp("^confirm");
        return re.test(window.get_name());
    }

    //Handle dialog close and reload Email List dashboard
    function onDialogClosed(args){
        if(args == null || args == 'cancel') {
            return;
        }

        var reloadListData = true;

        if (args.page=='properties'){
            _dashEmailListPanelId = args.emailListId;
            <%if(!String.IsNullOrEmpty(EmailListUpdateHandler))
              {%>
              <%=EmailListUpdateHandler %>();
            <%
              }%>
            reloadListData = true;
        }

        if (args.page=='addEmailList'){
            _dashEmailListPanelId = args.newEmailListId;
            <% if (!String.IsNullOrEmpty(EmailListUpdateHandler))
               {%>
               <%=EmailListUpdateHandler %>();
            <%
               }%>
            reloadListData = false;
        }

           if (args.page=='addAddresses'){
            _dashGroupId = args.newGroupId;
            <%if(!String.IsNullOrEmpty(EmailListUpdateHandler))
              {%>
              <%=EmailListUpdateHandler %>();
            <%
              }%>
            reloadListData = false;
        }


        //Reload dash
        loadEmailListPanelData(_dashEmailListPanelId, reloadListData);
    }

    //Clean emailList dashboard
    function cleanEmailListDashboard(){
        $("#infoPlace").empty();//.html('');
    }

</script>

<%-- Container for Results --%>
<div id="detailProgressContainer_<%=ID %>" style="display:none;">
	<div id="detailProgress_<%=ID %>" style="text-align:center;">
		<p><%=WebTextManager.GetText("/common/loading")%></p>
		<p>
			<asp:Image ID="_progressSpinner" runat="server" SkinId="ProgressSpinner" />
		</p>
	</div>
</div>
<div id="infoPlace" class="dashboard"></div>

<script type="text/C#" runat="server">
    
    /// <summary>
    /// Get/set handler for showing status message.
    /// The first parameter must be a message.
    /// The second parameter must determine if an operation was succeeded or not.
    /// </summary>
    public string ShowStatusMessageHandler { get; set; }

    /// <summary>
    /// Get/set handler for updating emailList
    /// </summary>
    public string EmailListUpdateHandler { get; set; }
    
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
            "svcInvitationManagement.js",
            ResolveUrl("~/Services/js/svcInvitationManagement.js"));

        RegisterClientScriptInclude(
            "svcAuthorization.js",
            ResolveUrl("~/Services/js/svcAuthorization.js"));

        RegisterClientScriptInclude(
          "securityHelper.js",
          ResolveUrl("~/Resources/securityHelper.js"));

        RegisterClientScriptInclude(
         "jquery.ckbxprotect.js",
          ResolveUrl("~/Resources/jquery.ckbxProtect.js"));

		RegisterClientScriptInclude(
			"templateHelper.js",
			ResolveUrl("~/Resources/templateHelper.js"));

        RegisterClientScriptInclude(
            "dateUtils.js",
            ResolveUrl("~/Resources/dateUtils.js"));

        //Moment.js: datetime utilities
        RegisterClientScriptInclude(
            "moment.js",
            ResolveUrl("~/Resources/moment.js"));

        RegisterClientScriptInclude(
            "jquery.pager.js",
            ResolveUrl("~/Resources/jquery.pager.js"));
	}
</script>