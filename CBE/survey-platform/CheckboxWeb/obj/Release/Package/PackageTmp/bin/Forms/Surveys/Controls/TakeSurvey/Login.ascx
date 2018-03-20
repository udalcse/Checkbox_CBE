<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Login.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.TakeSurvey.Login" %>
<%@ Import Namespace="Checkbox.Management"%>
<%@ Import Namespace="Checkbox.Wcf.Services" %>

    <div class="wrapperMaster center borderRadius surveyContentFrame surveyDialogFrame">
        <div class="innerSurveyContentFrame">
            <asp:Panel ID="_loginFailedWrapper" runat="server" class="loginErrorWrapper" Visible="false">
                <div class="error message"><%=SurveyEditorServiceImplementation.GetSurveyText("LOGIN_LOGINFAILED", ResponseTemplateId, LanguageCode, "en-US")%></div>
                <asp:Literal ID="FailureText" runat="server" EnableViewState="False"></asp:Literal>                                                         
            </asp:Panel>
            
            <div class="loginText">                
                <label><%=SurveyEditorServiceImplementation.GetSurveyText("LOGIN_LOGINTEXT", ResponseTemplateId, LanguageCode, "en-US")%></label>
            </div>

            <label for="<%=_userName.ClientID%>" class="Question"><%=SurveyEditorServiceImplementation.GetSurveyText("LOGIN_USERNAME", ResponseTemplateId, LanguageCode, "en-US")%></label><br />
            <asp:TextBox ID="_userName" runat="server" CssClass="loginTextBox"></asp:TextBox>

            <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" 
                ControlToValidate="_userName" ErrorMessage="User Name is required." 
                ToolTip="User Name is required." ValidationGroup="_login" style="color:#cc0000;">*</asp:RequiredFieldValidator><br />
        
            <label for="<%=_password.ClientID%>" class="Question"><%=SurveyEditorServiceImplementation.GetSurveyText("LOGIN_PASSWORD", ResponseTemplateId, LanguageCode, "en-US")%></label><br />
            
            <asp:TextBox ID="_password" runat="server" TextMode="Password" CssClass="loginTextBox"></asp:TextBox>
            <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" 
                ControlToValidate="_password" ErrorMessage="Password is required." 
                ToolTip="Password is required." ValidationGroup="_login" style="color:#cc0000;">*</asp:RequiredFieldValidator>

            <div>
                <% if(ApplicationManager.AppSettings.AllowPasswordReset && ApplicationManager.AppSettings.EmailEnabled) { %>
                    <a 
                        class="forgotPassword" 
                        href="<%=ResolveUrl("~/ResetPassword.aspx") %>" 
                        title="<%=SurveyEditorServiceImplementation.GetSurveyText("LOGIN_RESETPASSWORD", ResponseTemplateId, LanguageCode, "en-US") %>">

                        <%=SurveyEditorServiceImplementation.GetSurveyText("LOGIN_RESETPASSWORD", ResponseTemplateId, LanguageCode, "en-US")%>
                    </a>
                    <br/>
                <% } %>
             
                <%if (ApplicationManager.AppSettings.AllowPublicRegistration) {  %>
                    <a 
                        class="forgotPassword" 
                        href="<%=ResolveUrl("~/Users/Register.aspx") + "?s=" + Request.QueryString["s"] + "&i=" + InviteeGuid + "&q=" + Server.UrlEncode(Request.Url.Query) + "&l=" + LanguageCode %>" 
                        title="<%=SurveyEditorServiceImplementation.GetSurveyText("LOGIN_NEWUSER", ResponseTemplateId, LanguageCode, "en-US") %>">

                        <%=SurveyEditorServiceImplementation.GetSurveyText("LOGIN_NEWUSER", ResponseTemplateId, LanguageCode, "en-US")%>
                    </a>
                <%} %>
            </div>
            

                <%--<ckbx:MultiLanguageCheckBox ID="RememberMe" runat="server" TextId="" />--%>
            <br />          
         
            <div>
                <asp:Button ID="_loginButton" runat="server" ValidationGroup="_login" CssClass="ckbxButton roundedCorners border999 shadow999 silverButton workflowAjaxPostAction" Style="color:black;font-size:12px;" />                       
            </div>
        </div>
    </div>