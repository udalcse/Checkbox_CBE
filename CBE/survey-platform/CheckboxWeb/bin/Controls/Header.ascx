<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Header.ascx.cs" Inherits="CheckboxWeb.Controls.Header" %>
<%@ Register src="~/Controls/Navigation/NavMenu.ascx" TagName="NavMenu" TagPrefix="ckbx" %>
<%@ Register src="~/Controls/AccountInfo.ascx" TagName="AccountInfo" TagPrefix="ckbx" %>
<%@ Register src="~/Controls/Navigation/BreadCrumbNavigator.ascx" TagName="BreadCrumbNavigator" TagPrefix="ckbx" %>

<%@ Import Namespace="Checkbox.Management" %>
<%@ Import Namespace="Checkbox.Web" %>

<header id="header">
    <div id="header_search_results" class="floating-search-wrapper">
         <ul></ul>
    </div>
    <div class="highlight-bar"></div>
    <%if (ApplicationManager.AppSettings.CustomerUpdateHeader)
      { %>
    <div id="header_messages"></div>
    <% } %>
    <div class="topWrapper">
        <div class="logoPlace left">
            <img id="_siteLogo" runat="server" alt="Engauge Survey Logo" style=""/>
            <asp:Panel ID="_textLogoPanel" runat="server" CssClass="LogoText" Visible='<%# ApplicationManager.AppSettings.HeaderTypeChosen != AppSettings.HeaderType.Logo %>'>
                <p>
                    <asp:Literal ID="_headerLbl" runat="server" Text='<%#WebTextManager.GetText("/siteText/headerText") %>' Visible="true"></asp:Literal>
                </p>
            </asp:Panel>
        </div>

        <nav>
            <ckbx:NavMenu ID="_navMenu" runat="server" />
        </nav>

        <div class="settings-gear right" >   
            <ckbx:SecuredControlContainer ID="_settingsGear"  RequiredRoles="System Administrator"  runat="server">
                <asp:Panel runat="server" ID="settings" Visible="true">   
                    <a href="<%=ResolveUrl("~/Settings/Default.aspx") %>">
                        <img src="<%=ResolveUrl("~/App_Themes/CheckboxTheme/Images/Gear-orange.png")%>" alt="Settings" title="System Settings" />
                    </a> 
                </asp:Panel> 
           </ckbx:SecuredControlContainer>    
        </div>

        <div class="right">
            <ckbx:AccountInfo ID="_infoPlace" runat="server"></ckbx:AccountInfo>
        </div>

        <asp:Placeholder ID="_searchPlace" runat="server">
            <div class="header-search">
                <div class="searchMainMenuContainer left search-input-container">
                   <input type="text" class="searchMainMenuInput search-text" id="headerSearchText" placeholder="Search everywhere..." />
                   <span id="universal_search_loading" class="search-action loading-icon"></span>
                </div>
                <br class="clear" />
            </div>
        </asp:Placeholder>

        <br class="clear" />
    </div>
</header>