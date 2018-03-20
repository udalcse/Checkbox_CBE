<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="EnterPassword.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.TakeSurvey.EnterPassword" %>

<div class="wrapperMaster center borderRadius surveyContentFrame">
    <div class="innerSurveyContentFrame">
        <div style="margin-top:10px;margin-bottom:5px;">
            <asp:Label ID="_passwordLbl" runat="server" CssClass="Question" />
        </div>
        <div style="clear:both;"></div>
        <div style="float:left;">
            <asp:TextBox ID="_passwordTxt" runat="server" TextMode="Password" CssClass="loginTextBox"></asp:TextBox>
    
            <asp:RequiredFieldValidator ID="_passwordRequiredValidator" runat="server" 
                ControlToValidate="_passwordTxt" ErrorMessage="Password is required." Display="Dynamic"
                ToolTip="Password is required." ValidationGroup="_login" style="color:#cc0000;">*</asp:RequiredFieldValidator>

            <asp:Label ID="_wrongPasswordLbl" runat="server" style="color:#cc0000;" Visible="false"></asp:Label>
        </div>

        <div style="float:left;margin-left:25px;">
            <asp:Button ID="_passwordBtn" runat="server" CssClass="ckbxButton roundedCorners border999 shadow999 silverButton workflowAjaxPostAction" Style="color:black;font-size:12px;"/>                       
        </div>
        <div style="clear:both;"></div>
    </div>
</div>