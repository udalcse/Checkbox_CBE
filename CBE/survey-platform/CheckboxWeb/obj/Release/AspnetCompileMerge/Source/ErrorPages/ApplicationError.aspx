<%@ Page Language="C#" Theme="" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
    <head id="Head1" runat="server">
        <title>Engauge&reg; Survey</title>
        <link id="_favicon" runat="server" rel="icon" href="~/favicon.png" type="image/ico" />
        <ckbx:ResolvingCssElement runat="server" Source="../App_Themes/CheckboxTheme/Checkbox.css" />
    </head>
    <body>
        <div class="formContainer">
            <div class="topWrapper">
                <div class="fixed_1400 centerContent">
                    <div class="logoWrapper left">
                        <img alt="Engauge&reg;" src="../App_Themes/CheckboxTheme/Images/CheckboxLogo.png" />
                    </div>
                    <br class="clear" />
                </div>
            </div>
            <div class="fixed_1400 centerContent">
                <div class="contentWrapper padding10">
                    <div class="headerWrapper">
                        <ul class="pageHeader allMenu">
                            <li><h3>An error occurred.</h3></li>
                            <li>&nbsp;Please call The Center for Board Excellence at 336-645-8200 or email <a href="mailto:support@boardevaluations.com">support@boardevaluations.com</a> for assistance.</li>
                        </ul>
                    </div>

                    <div class="dashStatsWrapper border999 shadow999">
                        <div class="dashStatsHeader">
                            <span class="fixed_150 left mainStats">Request URL:</span>
                            <span class="left mainStats" style="font-family:Courier New"><%= Server.HtmlEncode(Request.Url.PathAndQuery) %></span>
                            <br class="clear" />
                        </div>

                        <div class="dashStatsContentLarge zebra">
                            <div class="fixed_150 left label">Error Message:</div>
                            <div class="left" style="font-family:Courier New"><%= (Server.GetLastError().Message.Contains("user '") || Server.GetLastError().Message.Contains("database '") || Server.GetLastError().Message.Contains("user \"") || Server.GetLastError().Message.Contains("database \"")) ? "An error has occured" : Server.HtmlEncode(Server.GetLastError().Message)%></div>
                            <br class="clear" />
                        </div>

                        <div class="dashStatsContentLarge detailZebra">
                            <div class="fixed_150 left label">Exception Detail 1:</div>
                            <div class="left" style="font-family:Courier New">
                                <%= Server.GetLastError().InnerException != null ? ((Server.GetLastError().InnerException.Message.Contains("user '") || Server.GetLastError().InnerException.Message.Contains("database '") || Server.GetLastError().InnerException.Message.Contains("user \"") || Server.GetLastError().InnerException.Message.Contains("database \"")) ? "An error has occured" : Server.HtmlEncode(Server.GetLastError().InnerException.Message)) : string.Empty%>
                            </div>
                            <br class="clear" />
                        </div>

                        <div class="dashStatsContentLarge zebra">
                            <div class="fixed_150 left label">Exception Detail 2:</div>
                            <div class="left" style="font-family:Courier New">
                                <%= Server.GetLastError().InnerException != null && Server.GetLastError().InnerException.TargetSite != null ? Server.HtmlEncode(Server.GetLastError().InnerException.TargetSite.ToString()) : string.Empty %>
                            </div>
                            <br class="clear" />
                        </div>

                        <div class="dashStatsContentLarge detailZebra">
                            <div class="fixed_150 left label">Exception Detail 3:</div>
                            <div class="left" style="font-family:Courier New">
                                <%= Server.GetLastError().InnerException != null && Server.GetLastError().InnerException.InnerException != null ? Server.HtmlEncode(Server.GetLastError().InnerException.InnerException.Message) : string.Empty %>
                            </div>
                            <br class="clear" />
                        </div>
                    </div>
                    <div class="spacing">&nbsp;</div>
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