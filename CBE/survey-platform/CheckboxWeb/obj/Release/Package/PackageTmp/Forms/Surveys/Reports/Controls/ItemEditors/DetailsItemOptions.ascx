<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="DetailsItemOptions.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.Controls.ItemEditors.DetailsItemOptions" %>
<%@ Import Namespace="Checkbox.Web"%>

<div class="formInput">
    <div class="left checkBox"><asp:CheckBox ID="_linkChk" runat="server" /></div>
    <div class="left"><p><label><%= WebTextManager.GetText("/controlText/detailsItemEditor/linkToResponses") %></label></p></div>
    <br class="clear" />
    
    <div class="left checkBox"><asp:CheckBox ID="_aliasesChk" runat="server" /></div>
    <div class="left"><p><label><%=WebTextManager.GetText("/controlText/useAliasEditor/useAliases") %></label></p></div>
    <br class="clear" />    

    <div class="left checkBox"><asp:CheckBox ID="_groupAnswersChk" runat="server" /></div>
    <div class="left"><p><label><%=WebTextManager.GetText("/controlText/detailsItemEditor/groupAnswers")%></label></p></div>
    <br class="clear" />   
</div>