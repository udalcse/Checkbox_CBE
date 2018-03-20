<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="MatrixBehavior.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemEditors.MatrixBehavior" %>
<%@ Register src="~/Controls/Piping/PipeControl.ascx" tagname="PipeSelector" tagprefix="pipe" %>
<div class="padding10">
    <div class="formInput">
        <p><ckbx:MultiLanguageLabel ID="MultiLanguageLabel1" AssociatedControlID="_aliasText" runat="server" TextId="/controlText/listEditor/alias" /></p>
        <asp:TextBox ID="_aliasText" runat="server" />
    </div>
    <br class="clear" />
</div>