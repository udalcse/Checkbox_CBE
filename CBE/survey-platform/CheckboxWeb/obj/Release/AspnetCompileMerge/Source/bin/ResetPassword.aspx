<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ResetPassword.aspx.cs" Inherits="CheckboxWeb.ResetPassword" %>
<%@ Import Namespace="Checkbox.Management" %>

<!DOCTYPE html>
<html>
    <head runat="server">
        <meta charset="utf-8" />
        <title id="_title" runat="server">Reset Password</title>
        <link id="_favicon" runat="server" rel="icon" href="~/favicon.png" type="image/ico" />
        
        <asp:Literal runat="server" ID="TypeKit"></asp:Literal>

    
        <ckbx:ResolvingCssElement runat="server" Source="App_Themes/CheckboxTheme/Checkbox.css" />
        <script language="javascript" type="text/javascript">
            var okBtn = '';
        </script>
    </head>
    <body class="loginPage">
        <form id="_resetForm" runat="server" DefaultButton="ResetBtn">
            <div class="loginForm">
                <asp:PlaceHolder ID="_trialMessagePlace" runat="server"></asp:PlaceHolder>
                <div class="loginLogo">
                    <img src="<%=ResolveUrl("~/App_Themes/CheckboxTheme/Images/CheckboxLogo.png") %>" height="35" width="195" style="margin-top:15px;" alt="Checkbox Survey Logo" />
                </div>
                
                <asp:Panel ID="_errorPanel" runat="server" CssClass="error StatusPanel" Visible="false">
                    <asp:Label ID="_errorLbl" runat="server" />
                </asp:Panel>
    
                <asp:Panel ID="_sendSuccessPanel" runat="server" Visible="false" CssClass="success StatusPanel">
                    <asp:Label ID="_successLbl" runat="server" />
                </asp:Panel>
    
                <asp:Panel id="_emailsCannotBeSentWarningPanel" runat="server" CssClass="warning StatusPanel" Visible="false">
                    <ckbx:MultiLanguageLabel ID="_emailsCannotBeSentWarning" TextId="/pageText/users/ResetPassword.aspx/emailsCannotBeSentWarning" runat="server" Text="You have used all the emails provided be the license. No more emails can be sent now. Please, try to do it later."/><br />
                </asp:Panel>

                <asp:PlaceHolder ID="_enterEmailPlace" runat="server">
                    <div class="loginInputPlace roundedCorners shadow-white-inner">
                        <div class="padding10">
                            <p>
                                <ckbx:MultiLanguageLabel TextId="/pageText/passwordReset.aspx/pageText" runat="server" Text="If you have forgotten your login information and/or would like to reset your password, enter your email address below. Instructions will be sent to you via email." />
                                <br class="clear" />
                            </p>
                        
                            <asp:RegularExpressionValidator
                                ID="_emailFormatValidator" runat="server" 
                                ControlToValidate="_emailTxt" 
                                ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"
                                ValidationGroup="_email"
                                CssClass="loginError">Invalid email format.</asp:RegularExpressionValidator>

                            <p>
                                <ckbx:MultiLanguageLabel runat="server" AssociatedControlID="_emailTxt" Text="Email" />
                                <ckbx:MultiLanguageHyperLink 
                                    ID="_lnkLoginLookup" 
                                    runat="server" 
                                    CssClass="forgotPassword right" 
                                    Text="Go Back" 
                                    ToolTip="Click here to return to the login page"
                                    NavigateUrl="Login.aspx">Go To Login
                                </ckbx:MultiLanguageHyperLink>
                                <br class="clear" />
                            </p>
                            <asp:TextBox ID="_emailTxt" runat="server" CssClass="logInInput roundedCorners" TabIndex="1"></asp:TextBox>
                            <asp:RequiredFieldValidator
                                ID="_emailRequired" runat="server" 
                                ControlToValidate="_emailTxt" 
                                ValidationGroup="_email"
                                CssClass="loginError">Please enter your email.</asp:RequiredFieldValidator>

                        </div>
                        <ckbx:MultiLanguageButton TabIndex="2" ID="ResetBtn" runat="server" CssClass="ckbxButton silverButton loginBtn roundedCorners border999 shadow999 right" Text="Reset Password" ValidationGroup="_email" />
                        <br class="clear" />
                    </div>
                    
                    <script language="javascript" type="text/javascript">
                        okBtn = 'ResetBtn';
                    </script>
                </asp:PlaceHolder>
                
                <asp:PlaceHolder ID="_processPlace" runat="server">
                    <div class="loginInputPlace roundedCorners shadow-white-inner">
                        <div class="padding10">
                            <p>
                                <asp:Label runat="server" Text="Change Password for " />
                                "<asp:Label ID="_userNameLbl" runat="server" />"
                                <br class="clear" />
                            </p>
                            
                            <asp:CompareValidator runat="server" 
                                ControlToValidate="_newPasswordTxt"
                                ValidationGroup="_newPassword"
                                ControlToCompare="_confirmPasswordTxt"
                                CssClass="loginError">The Password and Confirm Password do not match.</asp:CompareValidator>

                            <!--new password-->
                            <p>
                                <ckbx:MultiLanguageLabel runat="server" AssociatedControlID="_newPasswordTxt" TextId="/pageText/passwordProcess.aspx/newPassword" Text="New Password" />
                                <br class="clear" />
                            </p>
                            <asp:TextBox ID="_newPasswordTxt" runat="server" TextMode="Password" CssClass="logInInput roundedCorners" TabIndex="3"></asp:TextBox>
                            <asp:RequiredFieldValidator
                                runat="server" 
                                ControlToValidate="_newPasswordTxt" 
                                ValidationGroup="_newPassword"
                                CssClass="loginError">Please choose a password.</asp:RequiredFieldValidator>

                            <!--confirm password-->
                            <p>
                                <ckbx:MultiLanguageLabel runat="server" AssociatedControlID="_confirmPasswordTxt" TextId="/pageText/passwordProcess.aspx/confirmPassword" Text="Confirm New Password" />
                                <br class="clear" />
                            </p>
                            <asp:TextBox ID="_confirmPasswordTxt" runat="server" TextMode="Password" CssClass="logInInput roundedCorners" TabIndex="4"></asp:TextBox>
                            <asp:RequiredFieldValidator 
                                runat="server" 
                                ControlToValidate="_confirmPasswordTxt" 
                                ValidationGroup="_newPassword"
                                CssClass="loginError">Please confirm your password.</asp:RequiredFieldValidator>

                        </div>
                        <ckbx:MultiLanguageButton ID="ResetConfirm" TabIndex="5" runat="server" CssClass="ckbxButton silverButton loginBtn roundedCorners border999 shadow999 right" Text="Reset Password" ValidationGroup="_newPassword" />
                        <br class="clear" />
                    </div>
                    
                    <script language="javascript" type="text/javascript">
                        okBtn = 'ResetConfirm';
                    </script>
                </asp:PlaceHolder>
                
                <asp:Panel runat="server" ID="GoBackLinkPanel" style="display: inline-block;width: 100%;" Visible="False">
                    <div class="padding10 right">
                        <ckbx:MultiLanguageHyperLink NavigateUrl="Login.aspx" TabIndex="5" runat="server" CssClass="ckbxButton silverButton loginBtn roundedCorners border999 shadow999 right goBackLink" Text="Go To Login" />  
                    </div>                  
                </asp:Panel>

            </div>
            <div class="loginFooter">
                <% if (ApplicationManager.AppSettings.DisplayMachineName) { %>
                        <div style="float:right"><%= Environment.MachineName%></div>
                        <div style="float:left">
                  <% } else { %>
                        <div style="text-align:center;">
                   <%} %>
                   <% if (ApplicationManager.AppSettings.EnableMultiDatabase)
                           { %>
                            &copy;2002-<%= DateTime.Now.Year %> <a href="http://www.checkbox.com" target="_blank">Checkbox Survey, Inc. - Checkbox Online</a>
                        <% }
                           else
                           { %>
                            &copy;2002-<%= DateTime.Now.Year %> <a href="http://www.checkbox.com" target="_blank">Checkbox Survey, Inc. - 2016 Q2 </a>
                           
                        <% } %>
                        </div>
            </div>
                </div>
            <%-- Script Includes --%>
            <ckbx:ResolvingScriptElement ID="_jqueryInclude" runat="server" Source="~/Resources/jquery-latest.min.js" />
            <ckbx:ResolvingScriptElement ID="_loginCheckInclude" runat="server" Source="~/Resources/LoginCheck.js" />
            <ckbx:ResolvingScriptElement ID="_dialogHandlerInclude" runat="server" Source="~/Resources/DialogHandler.js" />
            
            <script language="javascript" type="text/javascript">
                $(document).ready(function () {
                    $("form input").keypress(function (e) {
                        if ((e.which && e.which == 13) || (e.keyCode && e.keyCode == 13)) {
                            //$('.SubmitButton').click();
                            WebForm_DoPostBackWithOptions(new WebForm_PostBackOptions(okBtn, '', true, '_resetForm', '', false, true));
                            return true;
                        }
                    });
                });
            </script>
        </form>
    </body>
</html>