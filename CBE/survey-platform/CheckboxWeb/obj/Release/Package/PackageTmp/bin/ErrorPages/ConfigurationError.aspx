<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="ConfigurationError.aspx.cs" Inherits="CheckboxWeb.ErrorPages.ConfigurationError" %>
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
                        <img alt="Engauge Survey&reg;" src="<%=ResolveUrl("~/App_Themes/CheckboxTheme/Images/CheckboxLogo.png") %>" />
                    </div>
                    <br class="clear" />
                </div>
            </div>
            <div class="contentWrapper padding10">
                <div class="fixed_1400 centerContent">
                    <div class="headerWrapper">
                        <ul class="pageHeader allMenu"><li><h3>Configuration error</h3></li></ul>
                    </div>
                    <div class="introPage">
                        <p>
                            An error was found in one of the Engauge&reg; configuration files. Please review the error details below and many any necessary configuration changes to resolve the problem.
                        </p>
					    <p>
                            *Please note that the web server must be restarted after configuration files are edited before changes will take effect.
                        </p>
                        <div>&nbsp;</div>
                        <p>
                            <asp:Label ID="_errorMessages" runat="server"></asp:Label>
                        </p>
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