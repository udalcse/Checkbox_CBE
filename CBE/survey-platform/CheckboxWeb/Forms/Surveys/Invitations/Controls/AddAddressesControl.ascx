<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="AddAddressesControl.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Invitations.Controls.AddAddressesControl" %>
<%@ Register Assembly="CheckboxWeb" Namespace="CheckboxWeb.Controls.Button" TagPrefix="btn" %>
<ckbx:MultiLanguageLabel ID="_textEntryInstructions" runat="server" CssClass="" TextId="/controlText/forms/surveys/invitations/controls/AddAddressesControl.ascx/instructions" Text="Enter the local path to the file your wish to upload"/><br />
<asp:Label ID="_textEntryError" runat="server" CssClass="error message" Visible="false" style="margin-left: 0; margin-bottom: 5px;" /><br />
<asp:TextBox ID="_importTxt" runat="server" Rows="15" Columns="80" TextMode="MultiLine" />
<%--<btn:CheckboxButton ID="_submitButton" runat="server" TextID="/controlText/forms/surveys/invitations/controls/AddAddressesControl.ascx/addButton" CssClass="ckbxButton roundedCorners border999 shadow999 orangeButton" OnClick="AddAddressesButton_Click" />
--%>