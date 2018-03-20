<%@ Page Title="" Language="C#" MasterPageFile="~/DetailList.master" AutoEventWireup="false" CodeBehind="Manage.aspx.cs" Inherits="CheckboxWeb.Forms.Surveys.Invitations.Manage" IncludeJsLocalization="true" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Register TagPrefix="ckbx" TagName="InvitationList" Src="~/Forms/Surveys/Invitations/Controls/InvitationList.ascx" %>
<%@ Register TagPrefix="ckbx" TagName="InvitationDashboard" Src="~/Forms/Surveys/Invitations/Controls/InvitationDashboard.ascx"%>
<%@ MasterType VirtualPath="~/DetailList.Master" %>

<asp:Content ID="_head" ContentPlaceHolderID="_head" runat="server">
    <ckbx:ResolvingScriptElement runat="server" Source="../../../Resources/StatusControl.js" />
    <ckbx:ResolvingScriptElement runat="server" Source="../../../Resources/surveys/invitations/manage.js" />

    <script type="text/javascript">
        var _selected = "<%=InvitationID %>";
        var _term = "<%=SearchTerm %>";

        <%-- Ensure statusControl initialized--%>
        $(document).ready(function(){
            $(document).on('click', '.ckbx-closedialog', function(){ 
                loadInvitationData(_dashInvitationId, true);
            });

            statusControl.initialize('_statusPanel');
            <%if (!string.IsNullOrEmpty(SearchTerm)) { %>
            $('.header-actions').hide;
            <%} %>
        });

        <%-- Load invitation dashboard --%>
         function onInvitationSelected(invitation) {
             loadInvitationData(invitation.DatabaseId);
         }

        <%-- Handle Survey Deleted --%>
        function onInvitationDeleted(invitationId){
            <%-- Call clear js method exposed by Dashboard control --%>
            cleanDashboard(invitationId);
            
            <%-- Call load js method exposed by invitiation list control --%>
            reloadInvitationList();
        }

        <%-- Show status message --%>
        function showStatusMessage(message, isSuccess){
            statusControl.showStatusMessage(message, isSuccess? StatusMessageType.success : StatusMessageType.error);
        }

        //Handler for updating invitation list
        function invitationListUpdateHandler(){
            <%-- Call load js method exposed by invitiation list control --%>
            reloadInvitationList();
        }
    </script>

</asp:Content>

<asp:Content ID="buttons" runat="server" ContentPlaceHolderID="_titleLinks">
    <a id="_newInvitationLink" runat="server" class="ckbxButton silverButton"><%=WebTextManager.GetText("/pageText/forms/surveys/invitations/manage.aspx/createInvitation+")%></a>
</asp:Content>

<asp:Content ID="_left" ContentPlaceHolderID="_leftContent" runat="server">
    <ckbx:InvitationList Id="_invitationList" runat="server" InvitationSelectedClientCallback="onInvitationSelected"></ckbx:InvitationList>
</asp:Content>

<asp:Content ID="_right" ContentPlaceHolderID="_rightContent" runat="server">
    <div id="detailProgressContainer" style="display:none;">
        <div id="detailProgress" style="text-align:center;">
            <p>Loading...</p>
            <p>
                <asp:Image ID="_progressSpinner" runat="server" SkinId="ProgressSpinner" />
            </p>
        </div>
    </div>
    <ckbx:InvitationDashboard Id="_invitationDash" runat="server" OnInvitationDeleted="onInvitationDeleted" ShowStatusMessageHandler="showStatusMessage" InvitationListUpdateHandler="invitationListUpdateHandler" />
</asp:Content>
