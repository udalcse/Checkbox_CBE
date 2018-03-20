<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Properties.ascx.cs" Inherits="CheckboxWeb.Styles.Forms.EditorControls.Properties" %>
<%@ Import Namespace="Checkbox.Web" %>

<div class="styleSectionHeader"><%= WebTextManager.GetText("/pageText/styles/forms/editor.aspx/properties") %></div>

<div class="styleSectionContainer">
    <div style="float:left;width:150px;line-height:25px;">
	<ckbx:MultiLanguageLabel ID="_nameLbl" runat="server" TextId="/pageText/styles/forms/editor.aspx/styleName" /></div>
    <div class="input">

        <asp:TextBox ID="_styleName" runat="server" Width="200px"></asp:TextBox>
        <asp:RequiredFieldValidator ID="_nameRequired" runat="server" ControlToValidate="_styleName" Display="Dynamic" CssClass="validationError" ForeColor="#cc0000" Text="Style name required." />
        <ckbx:CalloutStyleNameInUseValidator ID="inUseValidator" runat="server" IsChartStyle="false" ControlToValidate="_styleName" TextID="/pageText/styles/forms/editor.aspx/nameInUse" />
    </div>
    <div class="clear"></div>

    <div class="field_150"><ckbx:MultiLanguageLabel ID="publicLbl" runat="server" TextId="/pageText/styles/forms/editor.aspx/public" /></div>
    <div class="input"><asp:CheckBox ID="_publicStyle" runat="server" /></div>
    <div class="clear"></div>

    <div class="field_150"><ckbx:MultiLanguageLabel ID="_editableStyleLbl" runat="server" TextId="/pageText/styles/forms/editor.aspx/editable" /></div>
    <div class="input"><asp:CheckBox ID="_editableStyle" runat="server" /></div>
    <div class="clear"></div>
</div>