<%@ Page Title="" Language="C#" MasterPageFile="~/Dialog.Master" AutoEventWireup="false" CodeBehind="Properties.aspx.cs" Inherits="CheckboxWeb.Users.EmailLists.Properties" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>
<%@ Register src="~/Controls/Status/StatusControl.ascx" tagname="StatusControl" tagprefix="status" %>
<%@ Import Namespace="Checkbox.Web" %>


<asp:Content ID="Content2" ContentPlaceHolderID="_pageContent" runat="server">
    <div class="padding10">
        <status:StatusControl ID="_statusControl" runat="server" Height="35" />
        <div class="dialogSubTitle">
            <%=WebTextManager.GetText("/pageText/users/emailLists/properties.aspx/nameTitle") %>
        </div>
        <div class="dialogInstructions">
            <%=WebTextManager.GetText("/pageText/users/emailLists/properties.aspx/nameInstructions") %>
        </div>
        <div class="dialogSubContainer">
            <div class="formInput">
                <p>
                    <ckbx:MultiLanguageLabel ID="_emailListNameLabel" runat="server" AssociatedControlID="_emailListName" TextId="/pageText/users/emailLists/properties.aspx/emailListName" />
                </p>
                <asp:TextBox ID="_emailListName" runat="server" MaxLength="255" Width="300" />
                <asp:RequiredFieldValidator ID="_emailListNameRequired" runat="server" ControlToValidate="_emailListName" Display="Dynamic" CssClass="error message" ><%= WebTextManager.GetText("/pageText/users/emailLists/properties.aspx/emailListNameRequired")%></asp:RequiredFieldValidator>
                <ckbx:MultiLanguageLabel ID="_emailListNameErrorLabel" runat="server" CssClass="error message" Visible="false" TextId="/pageText/users/emaillists/properties.aspx/emailListNameExists" />
                <asp:RegularExpressionValidator ID="_groupnameLength" runat="server" Display="Dynamic" ControlToValidate="_emailListName" CssClass="error message" ValidationExpression="[\w\s]{1,255}"><%= WebTextManager.GetText("/pageText/users/emaillists/properties.aspx/emailListNameLength")%></asp:RegularExpressionValidator>
                <br class="clear"/>        
            </div>
            <div class="formInput">
                <p><ckbx:MultiLanguageLabel ID="_emailListasdfDescriptionLabel" runat="server" AssociatedControlID="_emailListDescription" TextId="/pageText/users/emailLists/properties.aspx/emailListDescription" /></p>
                <asp:TextBox ID="_emailListDescription" runat="server" TextMode="MultiLine" Rows="5" Columns="50" />
                <br class="clear" />
            </div>
        </div>
    </div>
</asp:Content>
