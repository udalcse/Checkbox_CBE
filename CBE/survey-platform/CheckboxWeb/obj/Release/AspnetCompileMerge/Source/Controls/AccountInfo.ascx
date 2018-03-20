<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="AccountInfo.ascx.cs" Inherits="CheckboxWeb.Controls.AccountInfo" %>
<%@ Import Namespace="Checkbox.Web" %>
<div id="loginContainer">
    <div id="loginName">
        <span><asp:Literal ID="_loggedInName" runat="server"></asp:Literal></span>
    </div>
    <div class="loginInfoContainer groupMenu">
        <ul>
            <asp:PlaceHolder ID="_editInfoPlace" runat="server" Visible="false">
                <li class="loginInfoSection"><a class="action-button label" href="<%=ResolveUrl("~/Users/EditInfo.aspx")%>"><%=WebTextManager.GetText("/pageText/siteHeader.ascx/editMyInfo")%></a></li>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="_availableSurveysPlace" runat="server" Visible="false">
                <li class="loginInfoSection"><a class="action-button label" href="<%=ResolveUrl("~/AvailableSurveys.aspx")%>"><%=WebTextManager.GetText("/pageText/siteHeader.ascx/availableSurveys")%></a></li>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="_availableReportsPlace" runat="server" Visible="false">
                <li class="loginInfoSection"><a class="action-button label" href="<%=ResolveUrl("~/AvailableReports.aspx")%>"><%=WebTextManager.GetText("/pageText/siteHeader.ascx/availableReports")%></a></li>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="_supportPlace" runat="server" Visible="false">
                <li class="loginInfoSection"><a class="action-button label" href="<%=ResolveUrl("~/RequestSupport.aspx?o=t") %>" target="_blank">
                                <%= WebTextManager.GetText("/pageText/siteHeader.ascx/customerSupport")%></a></li>
            </asp:PlaceHolder>
            <li class="loginInfoSection">
                <% if (LoggedIn)
                   {%>
                <a href="<%=ResolveUrl("~/Logout.aspx")%>" class="action-button label"><%=WebTextManager.GetText("/pageText/siteHeader.ascx/logout")%></a>
                <%}
                   else
                   { %>
                <a href="<%=ResolveUrl("~/Login.aspx")%>" class="action-button label"><%=WebTextManager.GetText("/pageText/siteHeader.ascx/login")%></a>
                  <%
                   } %>
            </li>
        </ul>
    </div>
</div>