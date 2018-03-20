<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="MatrixRowEditWidget.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemEditors.MatrixRowEditWidget" %>
<%@ Import Namespace="Checkbox.Web"%>

 <div style="clear:both;">
     
    <div style="float:left;height:30px;">
        <telerik:RadTextBox ID="_rowText" runat="server" Width="300px" />
        &nbsp;
        <ckbx:MultiLanguageDropDownList ID="_rowTypeList" runat="server">
            <asp:ListItem Value="Normal" TextId="/pageText/editMatrixRows.aspx/normal" />
            <asp:ListItem Value="Subheading" TextId="/pageText/editMatrixRows.aspx/header" />
            <asp:ListItem Value="Other" TextId="/pageText/editMatrixRows.aspx/other" />
        </ckbx:MultiLanguageDropDownList>
    </div>

    <div style="float:right;margin-top:3px;margin-right:5px;">
        <ckbx:MultiLanguageLinkButton runat="server" ID="_deleteBtn" CommandName="DeleteRow" TextId="/pageText/editMatrixRows.aspx/delete" />
    </div>

    <div style="float:right;margin-left:10px;">
        &nbsp;
    </div>

    <div style="float:right;margin-top:3px;">
        <a href="#"onclick="$('#slide_<%=ID %>').slideToggle('normal');"><%=WebTextManager.GetText("/pageText/editMatrixRows.aspx/toggle")%></a>
    </div>
</div>

<div id="slide_<%=ID %>" style="display:none;margin-left:20px;clear:both;">
    <p>
        <%= WebTextManager.GetText("/pageText/forms/surveys/itemEditors/optionsNormalEntry.ascx/mergeFrom") %> <asp:DropDownList runat="server" ID="_mergeList"><asp:ListItem>Field 1</asp:ListItem><asp:ListItem>Field 2</asp:ListItem><asp:ListItem>Field 3</asp:ListItem></asp:DropDownList>
        <br />
        <%= WebTextManager.GetText("/pageText/forms/surveys/itemEditors/optionsNormalEntry.ascx/alias") %>  <telerik:RadTextBox ID="_rowAlias" runat="server" EmptyMessage="[Enter Alias]" />
    </p>
</div>

