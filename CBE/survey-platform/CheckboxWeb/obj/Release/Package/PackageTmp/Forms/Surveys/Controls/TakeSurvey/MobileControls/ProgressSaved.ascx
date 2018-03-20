<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="ProgressSaved.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.TakeSurvey.MobileControls.ProgressSaved" %>

<div>
    <div class="progressSavedTitle">
        <asp:Literal ID="_progressSavedTxt" runat="server" />
    </div>
    <div class="progressSavedResumeContainer">
        <asp:Literal ID="_toResumeLbl" runat="server" />
        <asp:HyperLink ID="_returnLinkMobile" CssClass="workflowAjaxGetAction" runat="server" />
    </div>

    <asp:Panel ID="_emailProgressLinkPanel" runat="server" CssClass="progressSavedEmailContainer">
        <div>
            <asp:Literal ID="_toEmailLbl" runat="server" />
        </div>
        <div >
            <asp:RequiredFieldValidator runat="server" ID="_emailAddressValidator" ControlToValidate="_emailAddressFieldMobile" style="color:#cc0000;">*</asp:RequiredFieldValidator>
            <asp:TextBox ID="_emailAddressFieldMobile" runat="server" CssClass="progressSavedEmailInput" />
            <asp:Button ID="_sendResumeEmailBtnMobile" CssClass="workflowAjaxPostAction" runat="server"  />       
        </div>
            
        <asp:Panel ID="_progressSavedEmailSentPanel" runat="server" Visible="false" CssClass="progressSavedEmailSent">
            <asp:Literal ID="_emailSentLabel" runat="server" />
        </asp:Panel>
        <asp:Panel ID="_progressSavedEmailErrorPanel" runat="server" Visible="false" CssClass="progressSavedEmailError">
            <asp:Literal ID="_emailErrorLabel" runat="server" />
        </asp:Panel>
    </asp:Panel>
</div>

<script type="text/javascript">
    $(function () {        
        $('#<%= _sendResumeEmailBtnMobile.ClientID %>').on('click', function () {
            return $('#<%= _emailAddressFieldMobile.ClientID %>').val().length > 0;
        });
    });
</script>