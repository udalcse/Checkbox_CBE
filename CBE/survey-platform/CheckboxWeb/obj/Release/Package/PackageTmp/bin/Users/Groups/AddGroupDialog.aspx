<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Dialog.Master" CodeBehind="AddGroupDialog.aspx.cs"
    Inherits="CheckboxWeb.Users.Groups.AddGroupDialog" %>
<%@ Import Namespace="Checkbox.Web" %>

<%@ MasterType VirtualPath="~/Dialog.Master" %>

<asp:Content ID="_head" runat="server" ContentPlaceHolderID="_headContent">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="_pageContent" runat="server">
    <asp:Panel ID="Panel1" runat="server" CssClass="padding10 ">
        <div class="dialogInstructions" style="padding-bottom:30px;">
            <ckbx:MultiLanguageLabel ID="_dialogInstructionsLabel" runat="server" TextId="/pageText/users/groups/addGroupDialog.aspx/dialogInstructions"
            Text="Please, add a new group. Otherwise you will not be able to add users."/>
        </div>
        <div class="dialogSubTitle">
            <ckbx:MultiLanguageLabel ID="_profileTitle" runat="server" TextId="/pageText/users/groups/add.aspx/nameTitle"
                Text="Group name" />
        </div>
        <div class="dialogInstructions">
            <ckbx:MultiLanguageLabel ID="_profileInstructions" runat="server" TextId="/pageText/users/groups/add.aspx/nameInstructions"
                Text="Enter a name and optional description for this group" />
        </div>
        <div class="formInput">
            <p>
                <ckbx:MultiLanguageLabel ID="_groupNameLabel" runat="server" AssociatedControlID="_groupName"
                    TextId="/pageText/users/groups/add.aspx/groupName" CssClass="loginInfoLabel" />
            </p>
            <asp:TextBox ID="_groupName" runat="server" MaxLength="32" Width="250" />
            <div>
                <asp:RequiredFieldValidator ID="_groupNameRequired" runat="server" ControlToValidate="_groupName"
                    Display="Dynamic" CssClass="error message">*</asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="_groupNameLength" runat="server" Display="Dynamic"
                    ControlToValidate="_groupName" CssClass="error message" ValidationExpression="[\w\s-.]{1,255}"
                    >*</asp:RegularExpressionValidator>
                <asp:CustomValidator ID="_groupExistValidator" runat="server" ControlToValidate="_groupName"
                    OnServerValidate="_groupExistValidator_ServerValidate" CssClass="error message"><%=WebTextManager.GetText("/pageText/users/groups/properties.aspx/groupNameExists")%></asp:CustomValidator>
            </div>
            <br class="clear" />
        </div>
        <div class="formInput">
            <p>
                <ckbx:MultiLanguageLabel ID="_groupDescriptionLabel" runat="server" AssociatedControlID="_groupDescription"
                    TextId="/pageText/users/groups/add.aspx/groupDescription" /></p>
            <asp:TextBox ID="_groupDescription" runat="server" TextMode="MultiLine" Rows="8"
                Columns="100" />
        </div>
        <br class="clear" />
    </asp:Panel>
</asp:Content>
