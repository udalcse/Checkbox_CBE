<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SMTPConfigurator.ascx.cs" Inherits="CheckboxWeb.Install.Controls.SMTPConfigurator" %>
<%@ Register Assembly="CheckboxWeb" Namespace="CheckboxWeb.Controls.Button" TagPrefix="btn" %>
    
    <script type="text/javascript">

        var <%=ClientID%>ValidatorsEnabled = true;

        function enableUserNameAndPassword()
        {
            var authEnabled = $("#<%=_useSmtpAuthentication.ClientID%>").prop('checked');
            ValidatorEnable(document.getElementById('<%=_usernameValidator.ClientID%>'), authEnabled && <%=ClientID%>ValidatorsEnabled);
            ValidatorEnable(document.getElementById('<%=_passwordValidator.ClientID%>'), authEnabled && <%=ClientID%>ValidatorsEnabled);
            $("#<%=_username.ClientID%>").prop('disabled', !authEnabled);
            $("#<%=_password.ClientID%>").prop('disabled', !authEnabled);
            if (authEnabled)
                $("#<%=_username.ClientID%>").focus();
        }

        $(document).ready(function () {
            $("#<%=_useSmtpAuthentication.ClientID%>").click(enableUserNameAndPassword);
            setTimeout(enableUserNameAndPassword, 400);
        });

        function EnableValidators<%=ClientID%>(enable)
        {
            <%=ClientID%>ValidatorsEnabled = enable;
            ValidatorEnable(document.getElementById('<%=_serverAddressValidator.ClientID %>'), enable);
            ValidatorEnable(document.getElementById('<%=_portRequiredValidator.ClientID %>'), enable);
            ValidatorEnable(document.getElementById('<%=_portValidator.ClientID %>'), enable);
            ValidatorEnable(document.getElementById('<%=_usernameValidator.ClientID %>'), enable);
            ValidatorEnable(document.getElementById('<%=_passwordValidator.ClientID %>'), enable);
        }
    </script>

<div class="left formInput fixed_150">
    <label for="<%=_serverAddress.ClientID%>"><%=ServerAddressCaption%></label>
</div>
<div class="left formInput">
    <asp:TextBox id="_serverAddress" runat="server" />
</div>
<div class="left formInput">
    <asp:RequiredFieldValidator ID="_serverAddressValidator" runat="server" ControlToValidate="_serverAddress" CssClass="error message condensed">
        <%=ServerAddressValidatorMessage%>
    </asp:RequiredFieldValidator>
</div>
<br class="clear" />
<!-- The mail servers port. -->
<div class="left formInput fixed_150">
    <label for="<%=_port.ClientID%>"><%=PortCaption%></label>
</div>
<div class="left formInput">
    <asp:TextBox ID="_port" runat="server" CssClass="settingsNumericInput_4" MaxLength="5"/>
</div>
<div class="left formInput">
    <asp:RequiredFieldValidator ID="_portRequiredValidator" runat="server" ControlToValidate="_port" CssClass="error message condensed">
        <%=PortRequiredValidatorMessage %>
    </asp:RequiredFieldValidator>
    <asp:RegularExpressionValidator ID="_portValidator" runat="server" ControlToValidate="_port" ValidationExpression="^[1-9][0-9]*$" CssClass="error message condensed">
        <%=PortValidatorMessage%>
    </asp:RegularExpressionValidator>
</div>
<br class="clear" />
<!-- Determines if SSL is used when communicating with the mail server. -->
<div class="left formInput fixed_150">&nbsp;</div>
<div class="left formInput">
    <asp:CheckBox id="_enableSsl" runat="server"/>
</div>
<br class="clear" />
<!-- Determines if the connection to the STMP server uses authentication or not. -->
<div class="left formInput fixed_150">&nbsp;</div>
<div class="left formInput">
    <asp:CheckBox id="_useSmtpAuthentication" runat="server"/>
</div>
<br class="clear" />
<!-- The username that is used authenticating against the SMTP server. -->
<div class="left formInput fixed_150">
    <label for="<%=_username.ClientID%>"><%=UsernameCaption%></label>
</div>
<div class="left formInput">
    <asp:TextBox id="_username" runat="server"  Width="150"/>
</div>
<div class="left formInput">
    <asp:RequiredFieldValidator ID="_usernameValidator" runat="server" ControlToValidate="_username" CssClass="error message condensed">
        <%=UsernameValidatorMessage%>
    </asp:RequiredFieldValidator>
</div>
<br class="clear" />
<!-- The password that is used authenticating against the SMTP server. -->
<div class="left formInput fixed_150">
    <label for="<%=_password.ClientID%>"><%=PasswordCaption%></label>
</div>
<div class="left formInput">
    <asp:TextBox id="_password" runat="server" TextMode="Password" Width="150"/>
</div>
<div class="left formInput">
    <asp:RequiredFieldValidator ID="_passwordValidator" runat="server" ControlToValidate="_password" CssClass="error message condensed">
        <%=PasswordValidatorMessage%>
    </asp:RequiredFieldValidator>
</div>
<br class="clear" />
<div class="spacing">&nbsp;</div>
<div class="left fixed_150">&nbsp;</div>
<div class="left">
    <asp:Button runat="server" ID="_testSmtp" CssClass="ckbxButton roundedCorners border999 shadow999 silverButton smallButton" OnClick="EmailTest_Click" />
</div>
<br class="clear" />
<div class="spacing">&nbsp;</div>

<script language="C#" runat="server">
    protected override void OnInit(EventArgs e)
    {
        ServerAddressValidatorMessage = "The mail server address is a required field";
        PortRequiredValidatorMessage = "The mail server's port is a required field";
        PortValidatorMessage = "Invalid format. The port must be an integer";
        UsernameValidatorMessage = "Username is a required field";
        PasswordValidatorMessage = "Password is a required field";
        ServerAddressCaption = "Mail server address";
        PortCaption = "Port";
        EnableSSLCaption = "Enable SSL";
        UseSmtpAuthenticationCaption = "Use SMTP authentication";
        UsernameCaption = "Username";
        PasswordCaption = "Password";
        TestSMTPCaption = "TEST SMTP SETTINGS";
    }
</script>