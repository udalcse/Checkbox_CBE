<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="FormControls.ascx.cs" Inherits="CheckboxWeb.Styles.Forms.EditorControls.FormControls" %>
<%@ Register TagPrefix="style" Src="~/Styles/Controls/FontSelector.ascx" TagName="FontSelector" %>
<%@ Import Namespace="Checkbox.Web" %>

<asp:XmlDataSource ID="_borderStyleDataSource" runat="server" DataFile="~/Resources/CodeDependentResources.xml" XPath="/CodeDependentResources/BorderStyles/BorderStyle" />
<asp:XmlDataSource ID="_borderSizeDataSource" runat="server" DataFile="~/Resources/CodeDependentResources.xml" XPath="/CodeDependentResources/BorderSizes/BorderSize" />

<div class="styleSectionHeader"><ckbx:MultiLanguageLabel ID="FormLbl" runat="server" TextId="/pageText/styles/forms/editor.aspx/formControls" /></div>

<div class="styleSectionContainer">
    <fieldset style="display:block; clear:left;">
        <legend><ckbx:MultiLanguageLabel ID="progressBarLbl" runat="server" TextId="/pageText/styles/forms/editor.aspx/progressBar" SkinID="PageViewSubHeader" /></legend>
        <div><ckbx:MultiLanguageLabel ID="progressBarHeightLbl" runat="server" TextId="/pageText/styles/forms/editor.aspx/height" SkinID="PageViewItemLabel" /><asp:TextBox ID="_progressBarHeight" runat="server" Width="60px" />px</div>
        <div><ckbx:MultiLanguageLabel ID="progressBarWidthLbl" runat="server" TextId="/pageText/styles/forms/editor.aspx/width" SkinID="PageViewItemLabel" /><asp:TextBox ID="_progressBarWidth" runat="server" Width="60px" />px</div>
        <div style="clear:left;">
            <div style="float:left;"><ckbx:MultiLanguageLabel ID="_progressBarBorderStyleLbl" runat="server" TextId="/pageText/styles/forms/editor.aspx/progressBarBorder" SkinID="PageViewItemLabel" /></div>
            <div style="float:left;"><asp:DropDownList ID="_progressBarBorderSize" runat="server" SkinID="StyleEditorDropDown" DataTextField="Text" DataValueField="Value" DataSourceID="_borderSizeDataSource" /></div>
            <div style="float:left;"><asp:DropDownList ID="_progressBarBorderStyle" runat="server" SkinID="StyleEditorDropDown" DataTextField="Text" DataValueField="Value" DataSourceID="_borderStyleDataSource" /></div>
            <div style="float:left;"><telerik:RadColorPicker ID="_progressBarBorderColor" runat="server" PaletteModes="WebPalette,RGBSliders" ShowIcon="true" /></div>
        </div>
        <div style="clear:left;"><div style="float:left;"><ckbx:MultiLanguageLabel ID="progressBarForeColorLbl" runat="server" TextId="/pageText/styles/forms/editor.aspx/foreColor" SkinID="PageViewItemLabel" /></div><div style="float:left;"><telerik:RadColorPicker ID="_progressBarForeColor" runat="server" PaletteModes="WebPalette,RGBSliders" ShowIcon="true" Overlay="true" /></div></div>
        <div style="clear:left;"><div style="float:left;"><ckbx:MultiLanguageLabel ID="progressBarBackColorLbl" runat="server" TextId="/pageText/styles/forms/editor.aspx/backColor" SkinID="PageViewItemLabel" /></div><div style="float:left;"><telerik:RadColorPicker ID="_progressBarBackColor" runat="server" PaletteModes="WebPalette,RGBSliders" ShowIcon="true" Overlay="true" /></div></div>
     </fieldset>
     
     <fieldset>
        <legend><ckbx:MultiLanguageLabel ID="buttonStyleLbl" runat="server" TextId="/pageText/styles/forms/editor.aspx/buttonStyle" /></legend>
        <style:FontSelector ID="_buttonFont" runat="server" TextId="/pageText/styles/forms/editor.aspx/buttonFont" ElementName=".button" EnableStyleBuilder="false" />
        <div style="clear:left;">
            <div style="float:left;"><ckbx:MultiLanguageLabel ID="_buttonBorder" runat="server" TextId="/pageText/styles/forms/editor.aspx/buttonBorder" SkinID="PageViewItemLabel" /></div>
            <div style="float:left;"><asp:DropDownList ID="_buttonBorderSize" runat="server" SkinID="StyleEditorDropDown" DataTextField="Text" DataValueField="Value" DataSourceID="_borderSizeDataSource" /></div>
            <div style="float:left;"><asp:DropDownList ID="_buttonBorderStyle" runat="server" SkinID="StyleEditorDropDown" DataTextField="Text" DataValueField="Value" DataSourceID="_borderStyleDataSource" /></div>
            <div style="float:left;"><telerik:RadColorPicker ID="_buttonBorderColor" runat="server" PaletteModes="WebPalette,RGBSliders" ShowIcon="true" /></div>
        </div>
        <div style="clear:left;"><div style="float:left;"><ckbx:MultiLanguageLabel ID="buttonColorLbl" runat="server" TextId="/pageText/styles/forms/editor.aspx/buttonBackColor" SkinID="PageViewItemLabel" /></div><div style="float:left;"><telerik:RadColorPicker ID="_buttonBackgroundColor" runat="server" PaletteModes="WebPalette,RGBSliders" ShowIcon="true" /></div></div>
     </fieldset>
 </div>