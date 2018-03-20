<%@ Page Title="" Language="C#" MasterPageFile="~/Admin.Master" AutoEventWireup="false" CodeBehind="Progress.aspx.cs" Inherits="CheckboxWeb.Forms.Surveys.Invitations.Progress" %>

<asp:Content ID="scriptIncludes" ContentPlaceHolderID="_scriptIncludes" runat="server">
    <ckbx:ResolvingScriptElement runat="server" Source="~/Resources/jquery.progressbar.min.js" />
    <ckbx:ResolvingScriptElement runat="server" Source="~/Resources/AjaxProgress.js" />
</asp:Content>
    
<asp:Content ID="scriptContent" ContentPlaceHolderID="_scriptContent" runat="server">
    <script type="text/javascript">
        var progressStatusSuccess = false;

        //Start progress monitoring
        function startProgress(progressKey) {
            //Start send process
            $.ajax({
                type: "GET",
                url: '<%=ResolveUrl("~/Invitations/Worker.aspx")%>',
                async: true,
                contentType: "application/json; charset=utf-8",
                data: { mode: '<%=Request["mode"] %>', i: '<%=Invitation.ID %>' },
                dataType: "json",
                timeout: 100
            });         //Set a short timeout to work around issues where load balancers and the like
            // may timeout after 60 seconds, etc.  This essentially makes the call fire and
            // forget instead of waiting for error/success error to return.

            //Start monitoring progress
            ajaxProgress.startProgress(
                progressKey,
                'progressDiv',
                '<%=ResolveUrl("~/") %>',
                onProgressUpdate,
                onProgressError,
                onProgressComplete);
        }

        //Show message on error
        function onProgressError(errorMessage) {
            $('#<%= _returnDiv.ClientID %>').show();
            $('#progressDiv').hide();
            $('#progressText').html('<div class="ErrorMessage">An error occurred while updating progress: <span style="color:black;font-weight:normal;">&nbsp;&nbsp;' + errorMessage + '</span></div>');
        }

        //Update status
        function onProgressUpdate(progressData) {
            $('#progressDiv').show();
            $('#progressText').html(progressData.Message);
        }

        //Do something on progress complete
        function onProgressComplete(progressData) {
            document.location = 'BatchSendSummary.aspx?complete=true';
        }          
    </script>
</asp:Content>

<asp:Content ID="content" ContentPlaceHolderID="_pageContent" runat="server">
    <div style="margin-left:25px;margin-top:25px;">
        <div style="margin-bottom:15px;">
            <ckbx:MultiLanguageLabel ID="_queueingLbl" runat="server" CssClass="PrezzaLabel" style="font-size:14px;" TextId="/pageText/batchSendSummary.aspx/queuingMessage">Queuing invitations for sending.  Please do not navigate away from this page until sending is complete.</ckbx:MultiLanguageLabel>
            <ckbx:MultiLanguageLabel ID="_queuingCompletedLbl" runat="server" CssClass="PrezzaLabel" style="display:none;font-size:14px;" TextId="/pageText/invitations/batchSendSummary/queingCompleted">Invitiations have been queued and will be sent shortly.</ckbx:MultiLanguageLabel>
        </div>
        <div id="progressDiv"></div>
        <div class="ProgressText" id="progressText"></div>
        <asp:Panel runat="server" id="_returnDiv" style="margin-top:25px;display:none;">
            <ckbx:MultiLanguageHyperLink ID="_backToInvitationLink" runat="server" CssClass="PrezzaLink" TextId="/pageText/batchSendSummary.aspx/clickHere" NavigateUrl="Recipients.aspx" />
            <ckbx:MultiLanguageLabel ID="_toReturnLbl" runat="server" CssClass="PrezzaNormal" TextId="/pageText/batchSendSummary.aspx/toReturn" />
        </asp:Panel>
    </div>
</asp:Content>

