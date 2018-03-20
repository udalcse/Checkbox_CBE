<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="EnterPassword.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.TakeSurvey.MobileControls.EnterPassword" %>

<div class="wrapperMaster center borderRadius surveyContentFrame">
    <div style="margin-top:10px;margin-bottom:5px;">
        <asp:Label ID="_passwordLbl" runat="server" CssClass="Question" />
    </div>
    <div style="clear:both;"></div>
    <div>
        <asp:TextBox ID="_passwordTxt" runat="server" TextMode="Password"></asp:TextBox>
    
        <asp:RequiredFieldValidator ID="_passwordRequiredValidator" runat="server" 
            ControlToValidate="_passwordTxt" ErrorMessage="Password is required." Display="Dynamic"
            ToolTip="Password is required." ValidationGroup="_login" style="color:#cc0000;">*</asp:RequiredFieldValidator>

        <asp:Label ID="_wrongPasswordLbl" runat="server" style="color:#cc0000;" Visible="false"></asp:Label>
    </div>

    <div>
        <asp:Button ID="_passwordBtn" runat="server" ValidationGroup="_login" CssClass="workflowAjaxPostAction" />                       
    </div>
    <div style="clear:both;"></div>
</div>
