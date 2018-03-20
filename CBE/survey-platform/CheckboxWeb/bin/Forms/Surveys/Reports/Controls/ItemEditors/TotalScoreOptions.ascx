<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="TotalScoreOptions.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.Controls.ItemEditors.TotalScoreOptions" %>
<%@ Import Namespace="Checkbox.Web"%>

<div class="dialogSubTitle"><%=WebTextManager.GetText("/controlText/averageScoreItemEditor/behavior")%></div>

<div class="dialogContainer">
    <div class="field_150"><%=WebTextManager.GetText("/controlText/useAliasEditor/useAliases") %></div>
    <div class="input"><asp:CheckBox ID="_aliasesChk" runat="server" /></div>
    <div class="clear"></div>    
</div>