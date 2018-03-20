<%@ Page Title="" Language="C#" MasterPageFile="~/Dialog.Master" AutoEventWireup="false" CodeBehind="Properties.aspx.cs" Inherits="CheckboxWeb.Users.Groups.Properties" %>
<%@ MasterType VirtualPath="~/Dialog.master" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Register src="../../Controls/Status/StatusControl.ascx" tagname="StatusControl" tagprefix="status" %>

<asp:Content ID="Content2" ContentPlaceHolderID="_pageContent" runat="server">
    <div class="padding10">
        <div class="dialogSubTitle">
            <ckbx:MultiLanguageLabel ID="_profileTitle" runat="server" TextId="/pageText/users/groups/add.aspx/nameTitle" Text="Group name" />
        </div>
        <div class="dialogInstructions">
            <ckbx:MultiLanguageLabel ID="_profileInstructions" runat="server" TextId="/pageText/users/groups/add.aspx/nameInstructions" Text="Enter a name and optional description for this group" />
        </div>
        <status:StatusControl id="_statusControl" runat="server" />

        <div class="formInput">
            <p>
                <ckbx:MultiLanguageLabel ID="_groupNameLabel" runat="server" AssociatedControlID="_groupName" TextId="/pageText/users/groups/add.aspx/groupName" />
            </p>
            <p>
                <asp:TextBox ID="_groupName" runat="server" MaxLength="32" Width="250" />
            </p>
            <asp:RequiredFieldValidator ID="_groupNameRequired" runat="server" ControlToValidate="_groupName" Display="Dynamic" CssClass="error message"><%= WebTextManager.GetText("/pageText/users/groups/add.aspx/groupNameRequired") %></asp:RequiredFieldValidator>
            <ckbx:MultiLanguageLabel ID="_groupNameErrorLabel" runat="server" CssClass="error message" Visible="false" TextId="/pageText/users/groups/add.aspx/groupNameExists" />
            <asp:RegularExpressionValidator ID="_groupnameLength" runat="server" Display="Dynamic" ControlToValidate="_groupName" CssClass="error message" ValidationExpression="[\w\s-.]{1,255}"><%= WebTextManager.GetText("/pageText/users/groups/add.aspx/groupNameLength")%></asp:RegularExpressionValidator>
            <br class="clear"/>
        </div>
        <div class="formInput">
            <p><ckbx:MultiLanguageLabel ID="_groupDescriptionLabel" runat="server" AssociatedControlID="_groupDescription" TextId="/pageText/users/groups/add.aspx/groupDescription" /></p>
             <asp:TextBox ID="_groupDescription" runat="server" TextMode="MultiLine" Rows="5" Columns="60" />
            <br class="clear" />
        </div>
    </div>
</asp:Content>