<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="ProgressSaved.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.TakeSurvey.ProgressSaved" %>
    <div class="wrapperMaster center borderRadius textContainer">
        <div class="innerSurveyContentFrame">
            <div class="progressSavedTitle">
                <asp:Literal ID="_progressSavedTxt" runat="server" />
            </div>
            <div class="progressSavedResumeContainer">
                <div>
                    <asp:Literal ID="_toResumeLbl" runat="server" />
                    <asp:HyperLink ID="_returnLink" CssClass="workflowAjaxGetAction" runat="server" />
                    <asp:Literal ID="_toContinueLbl" runat="server" />
                </div>
                <asp:TextBox ID="_linkTxt" ReadOnly="true" runat="server" Width="800" CssClass="progressSavedResumeLink" />
            </div>

            <asp:Panel ID="_emailProgressLinkPanel" runat="server" CssClass="progressSavedEmailContainer">
                <div>
                    <asp:Literal ID="_toEmailLbl" runat="server" />
                </div>
                <div class="progressSavedEmailAddressLabel" style="float:left;">
                    <asp:Literal ID="_emailAddressLbl" runat="server" />
                </div>
                <div class="progressSavedEmailAddressInputContainer" style="float:left;">
                    <asp:TextBox ID="_emailAddressField" runat="server" CssClass="progressSavedEmailInput" />
                    <span class="required">*</span>
                </div>

                <div class="progressSavedEmailButtonContainer" style="float:left;">
                    <asp:Button ID="_sendResumeEmailBtn" runat="server" CssClass="ckbxButton roundedCorners border999 shadow999 orangeButton workflowAjaxPostAction" />       
                </div>
                <div style="clear:both;"></div>
            
                <asp:Panel ID="_progressSavedEmailSentPanel" runat="server" Visible="false" CssClass="progressSavedEmailSent">
                    <asp:Literal ID="_emailSentLabel" runat="server" />
                </asp:Panel>
                <asp:Panel ID="_progressSavedEmailErrorPanel" runat="server" Visible="false" CssClass="progressSavedEmailError">
                    <asp:Literal ID="_emailErrorLabel" runat="server" />
                </asp:Panel>
            </asp:Panel>
         </div>
    </div>