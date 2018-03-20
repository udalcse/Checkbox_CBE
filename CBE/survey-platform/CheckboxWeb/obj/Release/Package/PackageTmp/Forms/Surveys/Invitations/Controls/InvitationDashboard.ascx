<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="InvitationDashboard.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Invitations.Controls.InvitationDashboard" %>
<%@ Import Namespace="Checkbox.Management" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Import Namespace="Checkbox.Messaging.Email" %>

<script  type="text/javascript">
    var _dashInvitationId = null;

    <%-- Ensure services initialized --%>
	$(document).ready(function () {
        //Precompile templates used
        templateHelper.loadAndCompileTemplate('<%=ResolveUrl("~/Forms/Surveys/Invitations/jqtmpl/invitationDashboardTemplate.html") %>', 'invitationDashboardTemplate.html');
        templateHelper.loadAndCompileTemplate('<%=ResolveUrl("~/Forms/Surveys/Invitations/jqtmpl/recipientSummaryTemplate.html") %>', 'recipientSummaryTemplate.html');
        templateHelper.loadAndCompileTemplate('<%=ResolveUrl("~/Forms/Surveys/Invitations/jqtmpl/recipientListTemplate.html") %>', 'recipientListTemplate.html');
        templateHelper.loadAndCompileTemplate('<%=ResolveUrl("~/Forms/Surveys/Invitations/jqtmpl/responseListTemplate.html") %>', 'responseListTemplate.html');
	});

    <%-- Clear dashboard --%>
    function cleanDashboard(){
        _dashInvitationId = -1;
        $('#infoPlace').empty();//.html('');    
    }

    <%-- Expose Method to Load Invitation --%>
    function loadInvitationData(invitationId, reloadListData){
        if(invitationId == null || invitationId == '' || invitationId == -1) {
            return;
        }

        _dashInvitationId = invitationId;

        <%-- Start by loading invitation data --%>
        svcInvitationManagement.getInvitation(_at, invitationId, onInvitationMetaDataLoaded, {reloadListData: reloadListData, invitationId: invitationId, at: _at});
    }
	
	<%-- Apply template to loaded metadata and then apply child templates --%>
	function onInvitationMetaDataLoaded(resultData, args) {
		if (resultData == null) {
			$('#infoPlace').html('<div class="error message" style="margin:15px;padding:5px;">Unable to load information for invitation with id: ' + args.invitationId + '.</div>');
            return;
		}
	    resultData['isScheduleActive'] = '<%=ApplicationManager.AppSettings.MSSMode.Equals("SES") %>'.toLowerCase();
        //Replace newlines in body with <br /> for better printing
        //if(resultData.Body != null){
        //    resultData.Body = resultData.Body.replace(/\n/g, '<br />\n' );
        //}


        
		<%-- Load/Compile/Run Dashboard Template --%>
		templateHelper.loadAndApplyTemplate(
            'invitationDashboardTemplate.html', 
            '<%=ResolveUrl("~/Forms/Surveys/Invitations/jqtmpl/invitationDashboardTemplate.html") %>',
            resultData,
            null,
            'infoPlace',
            true,
            onInvitationDataTemplateLoaded,
            args);

        <%-- Reload List data, if necessary --%>

        if(args.reloadListData) {
            $('#invitation' + resultData.DatabaseId).effect(
                'shake', 
                {
                    times:2,
                    distance:10,
                    duration:250
                },
                function(){
                    //Required to remove jagged text left behind in IE for some reason
                    // when pulsate is called.
                    this.style.removeAttribute('filter');

                    //Apply template
                    templateHelper.loadAndApplyTemplate(
                        'invitationListItemTemplate.html',
                        '<%=(!ApplicationManager.AppSettings.MSSMode.Equals("SES"))? ResolveUrl("~/Forms/Surveys/Invitations/jqtmpl/invitationListItemTemplate.html"):ResolveUrl("~/Forms/Surveys/Invitations/jqtmpl/scheduledInvitationListItemTemplate.html") %>',
                        resultData,
                        null,
                        'invitation_' + resultData.DatabaseId,
                        true)
                });
        }
	}
    
    <%-- Load invitation dashboard --%>
    function openInvitation(params) {
        loadInvitationData(params.invitationId);
        $('.invitation-dashboard-container').show();
    }

    <%-- Invitation message preview on load handler --%>
    function messageBodyIframeOnLoad() {
        $('#messageBody').contents().find('html').html($('#messageBodyHtml').html());
        $('#messageBody').contents().find('head').append('<link rel="Stylesheet" type="text/css" href="<%=ResolveUrl("~/Resources/tiny_mce/skins/lightgray/content.min.css")%>" />');

        //set interval which will update the iframe height based on its content size
        var intervalID = setInterval(function () {
            var iframe = document.getElementById('messageBody');
            var iframeHeight = parseInt(iframe.style.height.replace('px', ''));
            var bodyHeight =  iframe.contentWindow.document.body.offsetHeight;

            //we need some fault in the heights comparison because of different browsers specificity
            if (iframeHeight < bodyHeight + 15 /*|| iframeHeight > bodyHeight + 30*/) {
                iframe.style.height = bodyHeight + 30 + 'px';                                    
            } else {
                clearInterval(intervalID);
            }
        }, 500);  
    }

    <%-- Start loading child templates --%>
    function onInvitationDataTemplateLoaded(args){
        //Bind delete click
        $('#deleteInvitationLink').bind('click', function() { onDeleteInvitationClick(); });
        <% if (EmailGateway.ProviderSupportsBatches)
        {  %>
        $("#scheduleBtn").show();        
        <% }  %>

        <%-- Recipient Summary --%>;
        svcInvitationManagement.getRecipientSummary(
            args.at, 
            args.invitationId,
            function(resultData, funcArgs){
                templateHelper.loadAndApplyTemplate(
                    'recipientSummaryTemplate.html',
                    '<%=ResolveUrl("~/Forms/Surveys/Invitations/jqtmpl/recipientSummaryTemplate.html") %>',
                    resultData,
                    function (){ resizePanels();},
                    'invitationDashRecipientSummaryPlace',
                    true);
            }
        );

        <%-- Recent Recipients --%>
        svcInvitationManagement.listRecentInvitationRecipients(
            args.at, 
            args.invitationId,
            10,
            function(resultData, funcArgs){
                templateHelper.loadAndApplyTemplate(
                    'recipientListTemplate.html',
                    '<%=ResolveUrl("~/Forms/Surveys/Invitations/jqtmpl/recipientListTemplate.html") %>',
                    {invitationId: args.invitationId, recipients: resultData},
                    null,
                    'invitationDashRecentRecipientPlace',
                    true,
                    function(){
                        if(resultData.length == 0){
                            $('#recipientSummary').hide();
                            $('#recipentSummaryNoRecipients').show();
                        }

                        resizePanels();
                });
            }
        );

         <%-- Recent Responses --%>
        svcInvitationManagement.listRecentInvitationResponses(
            args.at, 
            args.invitationId,
            10,
            function(resultData, funcArgs){
                templateHelper.loadAndApplyTemplate(
                    'responseListTemplate.html',
                    '<%=ResolveUrl("~/Forms/Surveys/Invitations/jqtmpl/responseListTemplate.html") %>',
                    {invitationId: args.invitationId, responses: resultData},
                    null,
                    'invitationDashRecentResponsePlace',
                    true,
                    function(){
                        if(resultData.length == 0){
                            $('#responseSummaryHeader').hide();
                            $('#responseSummaryNoResponses').show();
                        }

                        resizePanels();
                });
            }
        );
    }

    //
    function checkForListRefresh(args) {
        if (args == null) {
            return;
        }

        var reloadListData = true;

        if(typeof(args.op)!='undefined') {
            if(args.op=='createInvitation') {
                _dashInvitationId = args.invitationId;

                <%if (!String.IsNullOrEmpty(InvitationListUpdateHandler))
                  {%>
                  <%=InvitationListUpdateHandler %>();
                <%
                  }%>

                reloadListData = false;
            } else if(args.op=='copyInvitation' && typeof(args.surveyId) != 'undefined') {
                window.location.href = '<%= ResolveUrl("~/Forms/Surveys/Invitations/Manage.aspx") %>?s=' + args.surveyId + '&i=' + args.invitationId;
            }
        }

        //Reload dash
        loadInvitationData(_dashInvitationId, reloadListData);
    }

    //Handle dialog close and reload invitation dashboard
    function onDialogClosed(args) {
        checkForListRefresh(args);
    }

    //Show confirm dialog about deleting a survey
    function onDeleteInvitationClick(){
        if(_dashInvitationId == null || _dashInvitationId == ''){
            return;
        }
        showConfirmDialogWithCallback(
            '<%=WebTextManager.GetText("/controlText/invitationDashboard.ascx/deleteInvitationConfirm") %>',
            deleteInvitationConfirmCallback,
            337,
            250,
            '<%=WebTextManager.GetText("/controlText/invitationDashboard.ascx/deleteInvitationConfirmTitle") %>'
        );
    }

     //Callback for confirm dialog about deleting an invitation
    function deleteInvitationConfirmCallback(args){
        if(args.success){
            svcInvitationManagement.deleteInvitation(_at, _dashInvitationId, invitationDeletedCallback, null);
            _dashInvitationId = -1;
            _selected = '0';
        }
    }

     //Handle invitation deleted
    function invitationDeletedCallback(resultData){
        var message;
        
        if (resultData)
        {
            $('.simplemodal-close').click();
            message = '<%=WebTextManager.GetText("/controlText/invitationDashboard.ascx/invitationDeleted") %>';

            //Bind survey delete event, if any specified
            <% if(!string.IsNullOrEmpty(OnInvitationDeleted)) { %>
               <%=OnInvitationDeleted %>(_dashInvitationId); 
            <%} %>
        }
        else
        {
            message = '<%=WebTextManager.GetText("/controlText/invitationDashboard.ascx/invitationDeleteError") %>';
        }
       

        <% if (!String.IsNullOrEmpty(ShowStatusMessageHandler))
           {%>
           <%=ShowStatusMessageHandler%>(message, resultData);
        <%
           }%>              
    }
    
    function toggleDashSection(item, id) {
         if ($('#' + id).is(':visible')) {
             $(item).removeClass('pageArrowUp');
             $(item).addClass('pageArrowDown');
             $('#' + id).hide('blind', null, 'fast', resizePanels());
         }
         else {
             $(item).removeClass('pageArrowDown');
             $(item).addClass('pageArrowUp');
             $('#' + id).show('blind', null, 'fast', resizePanels());
         }
     }

		
</script>

<%-- Container for Results --%>
<div id="infoPlace" class="dashboard padding10"></div>

<script type="text/C#" runat="server">
    /// <summary>
    /// Get/set callback for handling invitation delete event
    /// </summary>
    public string OnInvitationDeleted { get; set; }

    /// <summary>
    /// Get/set handler for updating invitation list.
    /// </summary>
    public string InvitationListUpdateHandler { get; set; }

    /// <summary>
    /// Get/set handler for showing status message.
    /// The first parameter must be a message.
    /// The second parameter must determine if an operation was succeeded or not.
    /// </summary>
    public string ShowStatusMessageHandler { get; set; }
    
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
		   "svcResponseData.js",
		   ResolveUrl("~/Services/js/svcResponseData.js"));

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
	}
</script>
