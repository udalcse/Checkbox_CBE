<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="CrossTabOptions.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.Controls.ItemEditors.CrossTabOptions" %>
<%@ Import Namespace="Checkbox.Web"%>

    
<div class="formInput">
    <div class="left"><p><label><%=WebTextManager.GetText("/controlText/useAliasEditor/useAliases") %></label></p></div>
    <div class="left checkBox" style="margin-left:15px;"><asp:CheckBox ID="_aliasesChk" runat="server" /></div>
    <br class="clear"/>
</div>
