<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="PageConditions.aspx.cs" Inherits="CheckboxWeb.Forms.Surveys.PageConditions" MasterPageFile="~/Dialog.Master" ValidateRequest="false" EnableEventValidation="false" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>
<%@ Register Assembly="CheckboxWeb" Namespace="CheckboxWeb.Controls.Button" TagPrefix="btn" %>
<%@ Register Src="~/Forms/Surveys/Controls/RuleEditor.ascx" TagName="RuleEditor" TagPrefix="ckbx" %>

<asp:Content ID="_headContent" runat="server" ContentPlaceHolderID="_headContent">
    <script type="text/javascript">
        function okClick(pageId) {
            parent.surveyEditor.onConditionsEdited(pageId);
        }
    </script>
</asp:Content>

<asp:Content ID="_pageContent" runat="server" ContentPlaceHolderID="_pageContent">
    <div class="item-editor-content">
        <ckbx:RuleEditor ID="_ruleEditor" runat="server" />
    </div>
    <div class="dialogFormPush">&nbsp;</div>
</asp:Content>