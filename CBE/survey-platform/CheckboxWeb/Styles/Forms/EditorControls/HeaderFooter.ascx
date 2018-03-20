<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="HeaderFooter.ascx.cs" Inherits="CheckboxWeb.Styles.Forms.EditorControls.HeaderFooter" %>
<%@ Import Namespace="Checkbox.Web" %>

<div class="styleSectionHeader"><%= WebTextManager.GetText("/pageText/styles/forms/editor.aspx/headerFooter") %></div>

<div class="field_150"><ckbx:MultiLanguageLabel ID="editLbl" runat="server" TextId="/pageText/styles/forms/editor.aspx/editType" /></div>
<div class="input"><ckbx:MultiLanguageDropDownList ID="_editorType" runat="server" AutoPostBack="true" /></div>
<div class="clear"></div>

<telerik:RadEditor 
    ID="_header" 
    runat="server" 
    Width="449px" 
    Height="520px"
    EnableResize="false" 
    Visible="true"
    ToolsFile="~/Resources/HeaderEditorTools.xml">

    <ImageManager
        ContentProviderTypeName="Checkbox.Web.UI.Controls.RadExtensions.CustomFileBrowser,Checkbox.Web"
        EnableImageEditor="false"
        UploadPaths="/images"
        ViewPaths="/images"
     />
     <DocumentManager
        ContentProviderTypeName="Checkbox.Web.UI.Controls.RadExtensions.CustomFileBrowser,Checkbox.Web"
        UploadPaths="/documents"
        ViewPaths="/documents"
     />
</telerik:RadEditor>
<telerik:RadEditor ID="_footer" runat="server" Width="449px" Height="520px" EnableResize="false" Visible="false" />
