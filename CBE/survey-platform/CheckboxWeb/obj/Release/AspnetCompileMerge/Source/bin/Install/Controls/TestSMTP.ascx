<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TestSMTP.ascx.cs" Inherits="CheckboxWeb.Install.Controls.TestSMTP" %>
    <script type="text/javascript">

        function enableUserNameAndPassword() {
            var authEnabled = $("#<%=_useSmtpAuthenticationUI.ClientID%>").attr('checked') == 'checked';
            $("#<%=_userNameUI.ClientID%>").prop('disabled', !authEnabled);
            $("#<%=_passwordUI.ClientID%>").prop('disabled', !authEnabled);
            if (authEnabled)
                $("#<%=_userNameUI.ClientID%>").focus();
        }

        function hideMessage() {
            $("#<%=_message.ClientID%>").hide("slow");
        }

        $(document).ready(function () {
            $("#<%=_useSmtpAuthenticationUI.ClientID%>").click(enableUserNameAndPassword);
            setTimeout(enableUserNameAndPassword, 400);
            setTimeout(hideMessage, 5000);
        });
    </script>

    <div runat="server" id="_message" visible="false">
    </div>
    <div class="dashStatsContent">
        <div class="fixed_125 left"><label for="<%=_to.ClientID%>"><%=ToLabel%></label></div>
        <div class="left">
            <asp:TextBox ID="_to" runat="server" CssClass="settingsEmailAddress"></asp:TextBox>
            <asp:RequiredFieldValidator ID="_toRequiredValidator" runat="server" Display="Dynamic" ControlToValidate="_to" CssClass="error message">
                   <%=CannotBeEmptyValidatorText%>
            </asp:RequiredFieldValidator>
        </div>
        <br class="clear" />

        <div class="fixed_125 left"><label for="<%=_from.ClientID%>"><%=FromLabel%></label></div>
        <div class="left">
            <asp:TextBox ID="_from" runat="server" CssClass="settingsEmailAddress"></asp:TextBox>
        </div>
        <br class="clear" />

        <div class="fixed_125 left"><label for="<%=_subject.ClientID%>"><%=SubjectLabel%></label></div>
        <div class="left">
            <asp:TextBox ID="_subject" runat="server" CssClass="settingsEmailAddress"></asp:TextBox>
        </div>
        <br class="clear" />

        <div class="fixed_125 left"><label for="<%=_body.ClientID%>"><%=BodyLabel%></label></div>
        <div class="left">
            <asp:TextBox ID="_body" runat="server" TextMode="MultiLine" Rows="10" Width="100%"></asp:TextBox>
        </div>
        <br class="clear" />

        <div class="fixed_125 left"><label for="<%=_mailHostUI.ClientID%>"><%=SMTPHostLabel%></label></div>
        <div class="left">
            <asp:TextBox ID="_mailHostUI" runat="server"></asp:TextBox>
        </div>
        <br class="clear" />

        <div class="fixed_125 left"><label for="<%=_portUI.ClientID%>"><%=PortLabel%></label></div>
        <div class="left">
            <asp:TextBox ID="_portUI" runat="server" CssClass="settingsNumericInput_4" MaxLength="5"></asp:TextBox>
             <asp:RegularExpressionValidator ID="_portUIValidator" runat="server" ControlToValidate="_portUI" Display="Dynamic" ValidationExpression="^(6553[0-5]|655[0-2]\d|65[0-4]\d{2}|6[0-4]\d{3}|[1-5]\d{4}|[1-9]\d{0,3})" CssClass="error message">
                    <%=InvalidPortNumberValidatorText%>
       </asp:RegularExpressionValidator>
              <asp:RequiredFieldValidator ID="RequiredPortUIValidator" runat="server" Display="Dynamic" ControlToValidate="_portUI" CssClass="error message">
                   <%=EmptyPortNumberValidatorText%>
            </asp:RequiredFieldValidator>
        </div>
        <br class="clear" />

        <div class="fixed_125 left"></div>
        <div class="left">
            <asp:Checkbox id="_enableSslUI" runat="server"/>
        </div>
        <br class="clear" />

        <div class="fixed_125 left"></div>
        <div class="left">
            <asp:Checkbox id="_useSmtpAuthenticationUI" runat="server"/>
        </div>
        <br class="clear" />

        <div class="fixed_125 left"><label for="<%=_userNameUI.ClientID%>"><%=UserNameLabel%></label></div>
        <div class="left">
            <asp:TextBox ID="_userNameUI" runat="server" Enabled="false"></asp:TextBox>
        </div>
        <br class="clear" />

        <div class="fixed_125 left"><label for="<%=_passwordUI.ClientID%>"><%=PasswordLabel%></label></div>
        <div class="left">
            <asp:TextBox ID="_passwordUI" runat="server" TextMode="Password" Enabled="false"></asp:TextBox>
        </div>
        <br class="clear" />
    </div>
    <btn:CheckboxButton runat="server" ID="_sendEmail" CssClass="ckbxButton roundedCorners border999 shadow999 silverButton smallButton" OnClick="SendEmail_ClickEvent" TextId="/pageText/settings/testSmtp.aspx/sendEmailBtn"/>

<script language="C#" runat="server">
    protected override void OnInit(EventArgs e)
    {
        ToLabel = "To";
        FromLabel = "From";
        SubjectLabel = "Subject";
        BodyLabel = "Body";
        SMTPHostLabel = "SMTP host";
        PortLabel = "Port";
        EnableSSLLabel = "Enable SSL";
        UseSMTPAuthenticationLabel = "Use SMTP authentication";
        UserNameLabel = "Username";
        PasswordLabel = "Password";
        SendTestMailLabel = "Send test mail";
        CannotBeEmptyValidatorText = "\"To\" field can not be empty";
        InvalidPortNumberValidatorText = "Port number is invalid";
        EmptyPortNumberValidatorText = "Port number can not be empty";
        DefaultTitle = "Checkbox smtp configuration test";
        DefaultBody = "Hello world.";
        SuccessStatusMessage = "Your test email was sent without encountering an error; however, it is possible that your web server suppresses some or all error messages in order to combat unsolicited email. We strongly recommend that you confirm that the email was received in order to ensure that Checkbox is properly configured.";

        if (!Page.IsPostBack)
        {
            _from.Text = Session["Email_systemEmailAddress"] as string;
            _mailHostUI.Text = Session["Email_serverAddress"] as string;
            _portUI.Text = Session["Email_port"] as string;
            _enableSslUI.Checked = Session["Email_enableSsl"] == null ? false : (bool)Session["Email_enableSsl"];
            _useSmtpAuthenticationUI.Checked = Session["Email_useSmtpAuthentication"] == null ? false : (bool)Session["Email_useSmtpAuthentication"];
            _userNameUI.Text = Session["Email_username"] as string;
            _passwordUI.Attributes["value"] = Session["Email_password"] as string;
        }
    }
</script>