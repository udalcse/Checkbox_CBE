<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Other.ascx.cs" Inherits="CheckboxWeb.Styles.Charts.AverageScore.EditorControls.Other" %>
<%@ Import Namespace="Checkbox.Web"%>

<div class="formInput">
    
    <div class="left checkBox"><asp:CheckBox ID="_showAnswerCount" runat="server" /></div>
    <div class="left"><p><ckbx:MultiLanguageLabel AssociatedControlID="_showAnswerCount" ID="showAnswerCountLbl" runat="server" TextId="/pageText/styles/charts/other.ascx/showAnswerCount" /></p></div>
    <br class="clear" />

    <div class="left checkBox"><asp:CheckBox ID="_allowExporting" runat="server" /></div>
    <div class="left"><p><ckbx:MultiLanguageLabel AssociatedControlID="_allowExporting" ID="MultiLanguageLabel1" runat="server" TextId="/pageText/styles/charts/other.ascx/allowExporting" /></p></div>
    <br class="clear" />
    <br class="clear" />
    
</div>