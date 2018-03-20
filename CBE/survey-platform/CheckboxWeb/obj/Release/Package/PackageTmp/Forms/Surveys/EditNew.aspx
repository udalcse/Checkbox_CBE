<%@ Page Title="" Language="C#" MasterPageFile="~/Admin.Master" AutoEventWireup="false" CodeBehind="EditNew.aspx.cs" Inherits="CheckboxWeb.Forms.Surveys.EditNew" %>
<%@ MasterType VirtualPath="~/Admin.Master" %>

<asp:Content ID="pageContent" ContentPlaceHolderID="_pageContent" runat="server">
    <iframe src="EditFrame.aspx?s=<%=ResponseTemplateId %>" seamless="true" scrolling="no" width="1300" height="768" style="margin-left:10px;"></iframe>   
</asp:Content>
