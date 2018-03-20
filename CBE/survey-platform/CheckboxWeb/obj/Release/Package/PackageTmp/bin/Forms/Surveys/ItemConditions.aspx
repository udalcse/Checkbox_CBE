<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="ItemConditions.aspx.cs" Inherits="CheckboxWeb.Forms.Surveys.ItemConditions" MasterPageFile="~/Dialog.Master"  ValidateRequest="false" EnableEventValidation="false" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>
<%@ Register Src="~/Forms/Surveys/Controls/RuleEditor.ascx" TagName="RuleEditor" TagPrefix="ckbx" %>

<asp:Content ID="_headContent" runat="server" ContentPlaceHolderID="_headContent">

    <ckbx:ResolvingScriptElement ID="_serviceHelper" runat="server" Source="~/Services/js/serviceHelper.js" />
    <ckbx:ResolvingScriptElement ID="_surveyManagementInclude" runat="server" Source="~/Services/js/svcSurveyManagement.js" />

<script type="text/javascript">
    //
    function okClick(hasConditions) {
        var inDialogWindow = (window.self != window.top);

        var args = { op: 'editItemConditions', result: 'ok', hasConditions: hasConditions };

        if (inDialogWindow) {
            closeWindow(window.top.templateEditor.onDialogClosed, args);
        }
        else if (parent && parent.templateEditor) {
            parent.templateEditor.onDialogClosed(args);
        }
    }
</script>

</asp:Content>

<asp:Content ID="_pageContent" runat="server" ContentPlaceHolderID="_pageContent">
    <ckbx:RuleEditor ID="_ruleEditor" runat="server" />   
</asp:Content>
