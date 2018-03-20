<%@ Page Title="" Language="C#" MasterPageFile="~/Dialog.Master" AutoEventWireup="false" CodeBehind="DefaultPolicyDialog.aspx.cs" Inherits="CheckboxWeb.Security.DefaultPolicyDialog" %>
<%@ Register TagPrefix="security" Src="~/Controls/Security/DefaultPolicyEditor.ascx" TagName="PolicyEditor" %>
<%@ Register src="../Controls/Status/StatusControl.ascx" tagname="StatusControl" tagprefix="status" %>

<asp:Content ID="Content1" ContentPlaceHolderID="_headContent" runat="server">
    <ckbx:ResolvingCssElement runat="server" Source="../ControlSkins/ACLEditor/Grid.ACLEditor.css" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="_pageContent" runat="server">
    <status:StatusControl ID="_statusControl" runat="server" Width="428" Height="35" style="margin-bottom:5px;" />  
    <ckbx:MultiLanguageLabel ID="_defaultPolicyAccessInstructions" runat="server" TextId="/pageText/security/defaultPolicyDialog.aspx/instructions" style="margin-bottom:5px;" />
    <asp:Panel ID="_policyWrapper" runat="server">
        <security:PolicyEditor ID="_policyEditor" runat="server" OnClosed="DefaultPolicyEditor_Closed" OnPolicyUpdated="DefaultPolicyEditor_PolicyUpdated" />
    </asp:Panel>

</asp:Content>
