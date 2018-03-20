<%@ Page Title="" Language="C#" MasterPageFile="~/Dialog.Master" AutoEventWireup="false" CodeBehind="AddUsers.aspx.cs" Inherits="CheckboxWeb.Forms.Surveys.Invitations.AddUsers" EnableEventValidation="false" ValidateRequest="false" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>
<%@ Register src="../../../Controls/Wizard/WizardNavigator.ascx" tagname="WizardNavigator" tagprefix="nav" %>
<%@ Register src="../../../Controls/Wizard/WizardButtons.ascx" tagname="WizardButtons" tagprefix="btn" %>
<%@ Register TagName="RecipientEditor" TagPrefix="ckbx" Src="~/Forms/Surveys/Invitations/Controls/Recipients.ascx" %>
<%@ Register TagName="AddRecipients" TagPrefix="ckbx" Src="~/Forms/Surveys/Invitations/Controls/AddRecipients.ascx" %>
<%@ Register Src="~/Forms/Surveys/Invitations/Controls/EditMessageControl.ascx" TagPrefix="ckbx" TagName="EditMessage" %>
<%@ Import Namespace="Checkbox.Web" %>

<asp:Content ID="Content1" ContentPlaceHolderID="_headContent" runat="server">
    <ckbx:ResolvingScriptElement runat="server" Source="~/Resources/DialogHandler.js" />
    <ckbx:ResolvingScriptElement runat="server" Source="~/Resources/WizardHighlight.js" />
    <ckbx:ResolvingScriptElement runat="server" Source="~/Resources/AjaxProgress.js" />
    <ckbx:ResolvingScriptElement runat="server" Source="~/Resources/jquery.progressbar.min.js" />
    <ckbx:ResolvingScriptElement runat="server" Source="~/Resources/gridLiveSearch.js" />

    <script>
        $(document).ready(function () {
            $("#addRecipientTabs").ckbxTabs('hideTab', 0);
            $("#addRecipientTabs").ckbxTabs('hideTab', 2);
            $("#addRecipientTabs").ckbxTabs('hideTab', 3);
            $("#addRecipientTabs").ckbxTabs('hideTab', 3);
            $("#addRecipientTabs").ckbxTabs('initialTabIndex', 1);
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="_pageContent" runat="server">
    <ckbx:AddRecipients ID="_addRecipients" runat="server" OnRecipientsAdded="onRecipientsAdded" GridCssClass="ckbxInvitationWizardRecipientsGrid" />
</asp:Content>
