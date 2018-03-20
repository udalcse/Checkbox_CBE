<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="HeatMapBehavior.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.Controls.ItemEditors.HeatMapBehavior" %>

<%@ Import Namespace="Checkbox.Web"%>

<div class="formInput">
    <div class="left"><p><label><%=WebTextManager.GetText("/controlText/chartEditor/useAliases") %></label></p></div>
    <div class="left checkBox" style="margin-left:15px;"><asp:CheckBox ID="_aliasesChk" runat="server" /></div>
    <br class="clear" />
    
</div>