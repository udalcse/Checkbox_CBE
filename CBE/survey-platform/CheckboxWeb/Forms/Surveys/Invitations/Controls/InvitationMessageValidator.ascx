<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="InvitationMessageValidator.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Invitations.Controls.InvitationMessageValidator" %>

<script type="text/javascript">
    //Show message about empty item before the continuation of the wizard
    function ShowConfirmMessage(message) {
        showConfirmDialogWithCallback(message, confirmationApproved);
    }

    //Confirmation approved handler
    function confirmationApproved() {
        $("#<%= _confirmed.ClientID %>").val('true');
        if ($('[id*=_nextButton]').length > 0) {
            window.location = $('[id*=_nextButton]').attr('href');
        }
        else
            window.location = $('[id*=_okBtn]').attr('href');
    }
</script>

<ckbx:ResolvingScriptElement ID="ResolvingScriptElement1" runat="server" Source="~/Resources/DialogHandler.js" />
<asp:HiddenField ID="_confirmed" EnableViewState="True" runat="server" />
