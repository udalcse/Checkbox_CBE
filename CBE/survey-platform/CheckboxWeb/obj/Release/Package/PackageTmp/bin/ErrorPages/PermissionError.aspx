<%@ Page Language="C#" Theme="" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
    <head id="Head1" runat="server">
        <title>Engauge&reg; Survey</title>
        <link id="ctl00_ctl00__favicon" rel="icon" href="../favicon.png" type="image/ico" />
        <ckbx:ResolvingCssElement runat="server" Source="../App_Themes/CheckboxTheme/Checkbox.css" />
    </head>
    <body>
        <div class="formContainer">
            <div class="topWrapper">
                <div class="fixed_1400 centerContent">
                    <div class="logoWrapper left">
                        <img alt="Engauge&reg;" src="<%=ResolveUrl("~/App_Themes/CheckboxTheme/Images/CheckboxLogo.png") %>" />
                    </div>
                    <br class="clear" />
                </div>
            </div>
            <div class="contentWrapper padding10">
                <div class="fixed_1400 centerContent">
                    <div class="headerWrapper">
                        <ul class="pageHeader allMenu">
                            <li><h3>The logged-in user does not have permission to access this page or secured resource.</h3></li>
                            <li>&nbsp;Please call The Center for Board Excellence at 336-645-8200 or email <a href="mailto:support@boardevaluations.com">support@boardevaluations.com</a> for assistance.</li>
                        </ul>
                    </div>
                    <div class="introPage">
                        <p>
                            <span class="fixed_150 left label">Request URL:</span>
                            <span class="left" style="font-family:Courier New"><%= Server.HtmlEncode(Request.Url.PathAndQuery) %></span>
                            <br class="clear" />
                        </p>
                       <div class="spacing">&nbsp;</div>
                   </div>
                    <div class="url-default">
                        <a href="<%= ResolveUrl("~/Logout.aspx") %>">Return to default page</a>
                        <br class="clear" />
                       <div class="spacing">&nbsp;</div>
                   </div>
                </div>
            </div>
            <div class="pushFooter"></div>
        </div>
        <div class="Footer">
            EnGauge <a href="https://www.boardevaluations.com"> Software</a> - Copyright &copy;2016-<%=DateTime.Now.Year %> <a href="https://www.boardevaluations.com">The Center for Board Excellence</a><br />
            EnGauge is a trademark of <a href="https://www.boardevaluations.com">The Center for Board Excellence</a>
        </div>
    </body>
</html>