<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EMailOptions.ascx.cs" Inherits="CheckboxWeb.Install.Controls.EMailOptions" %>
<%@ Import Namespace="Checkbox.Management" %>
<script type="text/javascript">
        function enableLength() {
            var authEnabled = $("#<%=_limitEmailLineLength.ClientID%>").attr('checked') == 'checked';
            ValidatorEnable(document.getElementById('<%=_lineLengthValidator.ClientID%>'), authEnabled);
            $("#<%=_lineLength.ClientID%>").prop('disabled', !authEnabled);
            if (authEnabled)
                $("#<%=_lineLength.ClientID%>").focus();
        }

        $(document).ready(function () {
            $("#<%=_limitEmailLineLength.ClientID%>").click(enableLength);
            setTimeout(enableLength, 400);
        });

        function EnableValidators<%=ClientID%>(enable)
        {
            ValidatorEnable(document.getElementById('<%=_systemEmailAddressValidator.ClientID %>'), enable);
            ValidatorEnable(document.getElementById('<%=_defaultInvitationSenderNameValidator.ClientID %>'), enable);
            ValidatorEnable(document.getElementById('<%=_lineLengthValidator.ClientID %>'), enable);
        }
    </script>


<div class="left formInput fixed_150">
    <label for="<%=_systemEmailAddress.ClientID%>"><%=SystemEmailAddressCaption%></label>
</div>
<div class="left formInput">
    <asp:TextBox id="_systemEmailAddress" runat="server" CssClass="settingsEmailAddress" />
</div>
<div class="left formInput">
    <asp:RequiredFieldValidator ID="_systemEmailAddressValidator" runat="server" ControlToValidate="_systemEmailAddress" CssClass="error message condensed condensed">
        <%=SystemEmailAddressValidatorMessage%>
    </asp:RequiredFieldValidator>
    <span class="error message condensed" runat="server" ID="_systemEmailAddressError" Visible="false">An invalid email address has been specified</span>
</div>
<br class="clear" />
<!-- The default from name when creating email invitations. -->
<div class="left formInput fixed_150">
    <label for="<%=_defaultInvitationSenderName.ClientID%>"><%=DefaultInvitationSenderNameCaption%></label>
</div>
<div class="left formInput">
    <asp:TextBox id="_defaultInvitationSenderName" runat="server" CssClass="settingsEmailAddress" />
</div>
<div class="left formInput">
    <asp:RequiredFieldValidator ID="_defaultInvitationSenderNameValidator" runat="server" ControlToValidate="_defaultInvitationSenderName" CssClass="error message condensed">
        <%=DefaultInvitationSenderNameValidatorMessage%>
    </asp:RequiredFieldValidator>
</div>
<br class="clear" />
<div class="spacing">&nbsp;</div>
<!-- Determines if a maximum line length is enforced when sending invitations. -->
 <% if (!ApplicationManager.AppSettings.EnableMultiDatabase)
    { %>
<div class="formInput condensed">
    <asp:CheckBox runat="server" id="_limitEmailLineLength" /> 
</div>
<div class="left formInput fixed_150">
    <label for="<%=_lineLength.ClientID%>"><%=LineLengthCaption%></label>
</div>
<div class="left formInput">
    <asp:TextBox id="_lineLength" runat="server" />
</div>

<div class="left formInput">
    <asp:RegularExpressionValidator ID="_lineLengthValidator" runat="server" ControlToValidate="_lineLength" ValidationExpression="^[1-9][0-9]*$">
        <%=LineLengthValidatorMessage%>
    </asp:RegularExpressionValidator>
</div>
<% } %>
<br class="clear" />
<script language="C#" runat="server">
    protected override void OnInit(EventArgs e)
    {
        SystemEmailAddressValidatorMessage = "A default email address must be specified";
        DefaultInvitationSenderNameValidatorMessage = "A default sender name must be specified";
        LineLengthValidatorMessage = "The line length must be an integer greater than zero";
        SystemEmailAddressCaption = "Default System email address";
        DefaultInvitationSenderNameCaption = "Default sender name";
        LineLengthCaption = "Length";
        LimitEmailLineLengthCaption = "Limit email message line length";
    }
</script>