<%@ Page Language="C#" MasterPageFile="~/Dialog.Master" AutoEventWireup="true" CodeBehind="Copy.aspx.cs" Inherits="CheckboxWeb.Forms.Surveys.Invitations.Copy" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>

<asp:Content ID="content" ContentPlaceHolderID="_pageContent" runat="server">
    <div class="padding10">
        <script>
            function setCopyStatus(type, text) {
                $('#_copyInvitationMsg' + type).html(text);
                $('#_copyInvitationMsg' + type).show();
                setTimeout(function () {
                    $('#_copyInvitationMsgInfo').hide('slow');
                    $('#_copyInvitationMsgError').hide('slow');
                }, 5000);
            }

            $(document).ready(function () {
                $('#<%=_invitationName.ClientID%>').focus(function () {
                    if ($('#<%=_invitationName.ClientID%>').val() == 'Invitation #<%=InvitationId %> Copy') {
                        $('#<%=_invitationName.ClientID%>').val('');
                    }
                });
            });

        </script>
        <div ID="_copyInvitationMsgError" style="display:none;" class="error message" style="font-size:14px;">                        
        </div>
        <div ID="_copyInvitationMsgInfo" style="display:none;" class="warning message" style="font-size:14px;">                        
        </div>
        <div class="formInput">
            <div class="fixed_200 left">
                <p><ckbx:MultiLanguageLabel id="_invitationNameLabel" runat="server" AssociatedControlId="_invitationName" TextId="/controlText/forms/surveys/invitations/controls/copyInvitation.ascx/invitationNameLabel" /></p>
             </div>
            <div class="left"><asp:TextBox ID="_invitationName" runat="server" Width="400" /></div>
            <div class="left" style="margin-top:8px;">
                <asp:RequiredFieldValidator ID="_invitationNameValidator" runat="server" Display="Dynamic" ControlToValidate="_invitationName"  CssClass="error message"><%= WebTextManager.GetText("/controlText/forms/surveys/invitations/controls/copyInvitation.ascx/invitationNameMissing")%></asp:RequiredFieldValidator>
            </div>
            <br class="clear" />
            <div class="left">
                <p><ckbx:MultiLanguageCheckBox id="_useDefaultText" runat="server" TextId="/controlText/forms/surveys/invitations/controls/copyInvitation.ascx/useDefaultInvitationText" /></p>
             </div>
            <br class="clear" />
            <div class="left">
                <p><ckbx:MultiLanguageCheckBox id="_copyRecipientsList" runat="server" TextId="/controlText/forms/surveys/invitations/controls/copyInvitation.ascx/copyRecipientsList" /></p>
             </div>
            <br class="clear" />
        </div>
    </div>
</asp:Content>
