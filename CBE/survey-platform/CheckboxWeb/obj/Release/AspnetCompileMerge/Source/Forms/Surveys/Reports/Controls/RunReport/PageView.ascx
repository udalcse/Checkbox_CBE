<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="PageView.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.Controls.RunReport.PageView" %>
<%@ Import Namespace="Checkbox.Web" %>

<%-- 
    Control Showing a view of a report page.   The other page related controls will be moved
    to the page layout template at run time and the template will be loaded based on report
    page configuration.
--%>

<asp:PlaceHolder ID="_pageLayoutPlace" runat="server" Visible="true" />

<%-- 
    Placeholder for page controls to be added to layout template.  Adding them to layout moves
    them from this location to be a child of the layout.  Items not added should be hidden.
--%>

<%-- Title --%>
<asp:Panel ID="_titlePanel" runat="server" CssClass="titleContainer">
    <asp:Label ID="_titleLbl" runat="server" CssClass="title" />
</asp:Panel>

