<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="CreateQuick.aspx.cs" Inherits="CheckboxWeb.Forms.Surveys.CreateQuick" MasterPageFile="~/Dialog.Master" %>

<%@ MasterType VirtualPath="~/Dialog.Master" %>
<%@ Register Src="~/Forms/Surveys/Controls/Properties.ascx" TagPrefix="ckbx" TagName="SurveyProperties" %>

<asp:Content ID="_content" ContentPlaceHolderID="_pageContent" runat="server">
    <ckbx:ResolvingScriptElement runat="server" Source="~/Services/js/svcSurveyManagement.js" />
    <ckbx:ResolvingScriptElement runat="server" Source="~/Resources/templateEditor.js" />
    <ckbx:ResolvingScriptElement runat="server" Source="~/Services/js/serviceHelper.js" />
    
    <div class="padding10">
        <ckbx:SurveyProperties IsDetailedForm="False" ID="_properties" runat="server" />
        <div class="clear"></div>
    </div>
</asp:Content>