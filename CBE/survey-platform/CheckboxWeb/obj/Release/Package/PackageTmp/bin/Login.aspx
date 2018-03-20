<%@ Page Language="C#" AutoEventWireup="False" CodeBehind="Login.aspx.cs" Inherits="CheckboxWeb.Login" %>
<%@ Import Namespace="Checkbox.Configuration.Install" %>
<%@ Import Namespace="Checkbox.Management" %>
<%@ Import Namespace="Checkbox.Web" %>

<!DOCTYPE html>
<html>
    <head runat="server">
        <meta charset="utf-8" />
        <title id="_title" runat="server">CBE</title>
        <link id="_favicon" runat="server" rel="icon" href="~/favicon.png" type="image/ico" />
        
        <asp:Literal runat="server" ID="TypeKit"></asp:Literal>
        <ckbx:ResolvingCssElement runat="server" Source="App_Themes/CheckboxTheme/Checkbox.css" />
        <ckbx:ResolvingCssElement runat="server" Source="Resources/css/smoothness/jquery-ui-1.10.2.custom.css"/>
    </head>
    <body class="loginPage">
        <form id="_loginForm" runat="server" DefaultButton="LoginBtn">
            <div class="loginForm">
                <asp:PlaceHolder ID="_trialMessagePlace" runat="server"></asp:PlaceHolder>
                <div class="loginLogo">
                    <img src="<%=ResolveUrl("~/App_Themes/CheckboxTheme/Images/CheckboxLogo.png") %>" height="42" width="195" alt="Checkbox Survey Logo" />
                </div>
                <div class="loginInputPlace roundedCorners shadow-white-inner">
                    <asp:Panel ID="_loginError" runat="server" CssClass="error StatusPanel" Visible="false">
                    </asp:Panel>
                    <div class="padding10">
                        <p>
                            <ckbx:MultiLanguageLabel ID="UserNameLabel" runat="server" AssociatedControlID="UserName" Text="Username" />
                            <ckbx:MultiLanguageHyperLink 
                                ID="_lnkNewUser" runat="server" 
                                CssClass="forgotPassword right" 
                                Text="Create new account" 
                                ToolTip="Create new account" 
                                NavigateUrl="Users/Register.aspx">New User?
                            </ckbx:MultiLanguageHyperLink>
                            <br class="clear" />
                        </p>
                        <asp:TextBox ID="UserName" runat="server" CssClass="logInInput roundedCorners" TabIndex="1"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="_userNameRequired" runat="server" 
                            ControlToValidate="UserName"
                            ValidationGroup="_login"
                            CssClass="loginError">Please enter a username.</asp:RequiredFieldValidator>
                            
                        <p>
                            <ckbx:MultiLanguageLabel ID="PasswordLabel" runat="server" AssociatedControlID="Password" Text="Password" />
                            <ckbx:MultiLanguageHyperLink 
                                ID="_lnkLoginLookup" 
                                runat="server" 
                                CssClass="forgotPassword right" 
                                Text="Forgot Password" 
                                ToolTip="Click here if you forgot your password"
                                NavigateUrl="ResetPassword.aspx">Forget Login Info?
                            </ckbx:MultiLanguageHyperLink>
                            <br class="clear" />
                        </p>
                        <asp:TextBox ID="Password" runat="server" TextMode="Password" CssClass="logInInput roundedCorners" TabIndex="2"></asp:TextBox>
                        <asp:RequiredFieldValidator
                            ID="_passwordRequired" runat="server" 
                            ControlToValidate="Password" 
                            ValidationGroup="_login"
                            CssClass="loginError">Please enter your password.</asp:RequiredFieldValidator>
                    </div>
                    
                    <ckbx:MultiLanguageButton TabIndex="3" ID="LoginBtn" runat="server" CssClass="ckbxButton silverButton loginBtn roundedCorners border999 shadow999 right" CommandName="Login" Text="Login" ValidationGroup="_login" />
                    <br class="clear" />
                </div>
            </div>
            <div class="loginFooter">
                <% if (ApplicationManager.AppSettings.DisplayMachineName)
                   { %>
                    <div style="float: right"><%= Environment.MachineName %></div>
                <div style="float: left">
                    <% }
                   else
                   { %>
                    <div style="text-align: center;">
                        <% } %>
                        <% if (ApplicationManager.AppSettings.EnableMultiDatabase)
                           { %>
                            &copy;2012-<%= DateTime.Now.Year %> <a href="http://www.boardevaluations.com" target="_blank">The Center for Board Excellence</a>
                        <% }
                           else
                           { %>
                            &copy;2012-<%= DateTime.Now.Year %> <a href="http://www.boardevaluations.com" target="_blank">The Center for Board Excellence</a>
                           
                        <% } %>
                    </div>
                            
                        </div>
                    </div>
            <%-- Script Includes --%>
            <ckbx:ResolvingScriptElement ID="_jqueryInclude" runat="server" Source="~/Resources/jquery-latest.min.js" />
            <ckbx:ResolvingScriptElement ID="_loginCheckInclude" runat="server" Source="~/Resources/LoginCheck.js" />
            <ckbx:ResolvingScriptElement ID="_dialogHandlerInclude" runat="server" Source="~/Resources/DialogHandler.js" />
           
            <ckbx:ResolvingScriptElement ID="_jqueryUI" runat="server" Source="~/Resources/jquery-ui-1.10.2.custom.min.js"/>
           
            <script language="javascript" type="text/javascript">
                $(document).ready(function () {
                    $("form input").keypress(function (e) {
                        if ((e.which && e.which == 13) || (e.keyCode && e.keyCode == 13)) {
                            //$('.SubmitButton').click();
                            WebForm_DoPostBackWithOptions(new WebForm_PostBackOptions('LoginBtn', '', true, '_loginForm', '', false, true));
                            return true;
                        }
                    });
                });
                $(document).tooltip();
            </script>
        </form>
    </body>
</html>