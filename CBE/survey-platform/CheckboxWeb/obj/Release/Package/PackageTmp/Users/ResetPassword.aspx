<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="ResetPassword.aspx.cs" Inherits="CheckboxWeb.Users.ResetPassword" MasterPageFile="~/Admin.Master" %>
<%@ Import Namespace="Checkbox.Web" %>

<asp:Content ID="_pageContent" runat="server" ContentPlaceHolderID="_pageContent">
    <div class="padding10">

         <asp:Panel ID="_errorPanel" runat="server" Visible="false">
            <div class="error message" style="padding:5px;">
                <asp:Label ID="_errorLbl" runat="server" />
            </div>
        </asp:Panel>
    
        <asp:Panel ID="_sendSuccessPanel" runat="server" Visible="false" CssClass="success message">
            <asp:Label ID="_successLbl" runat="server" />
        </asp:Panel>
    
        <asp:Panel id="_emailsCannotBeSentWarningPanel" runat="server" CssClass="warning message" Visible="false">
            <ckbx:MultiLanguageLabel ID="_emailsCannotBeSentWarning" runat="server" TextId="/pageText/users/ResetPassword.aspx/emailsCannotBeSentWarning" Text="You have used all the emails provided be the license. No more emails can be sent now. Please, try to do it later."/><br />
        </asp:Panel>
    
        <asp:PlaceHolder ID="_enterEmailPlace" runat="server">
            <div class="formInput fixed_500">
                <ckbx:MultiLanguageLabel AssociatedControlID="_emailTxt" ID="_instructionsLbl" runat="server" TextId="/pageText/passwordReset.aspx/pageText" />
                <asp:TextBox ID="_emailTxt" runat="server" Width="300" />
            </div>
        </asp:PlaceHolder>
    
        <asp:PlaceHolder ID="_processPlace" runat="server">
            <div class="dialogSubTitle">
                <ckbx:MultiLanguageLabel ID="_changePasswordLbl" runat="server" CssClass="label" TextId="/pageText/passwordProcess.aspx/changePassword" />
            </div>
            <div class="clear"></div>
        
            <div class="formInput">
                <p><label><%=WebTextManager.GetText("/pageText/passwordProcess.aspx/userName") %></label></p>
                <asp:Label ID="_userNameLbl" runat="server" style="padding-left:15px;font-size:14px;" />
            </div>
        
            <div class="formInput">
                <p><ckbx:MultiLanguageLabel AssociatedControlID="_newPasswordTxt" ID="_passwordLbl" runat="server" TextId="/pageText/passwordProcess.aspx/newPassword" /></p>
                <asp:TextBox ID="_newPasswordTxt" runat="server" TextMode="Password" />
            </div>
        
            <div class="formInput">
                <p><ckbx:MultiLanguageLabel AssociatedControlID="_confirmPasswordTxt" ID="_confirmPasswordLbl" runat="server" TextId="/pageText/passwordProcess.aspx/confirmPassword" /></p>
                <asp:TextBox ID="_confirmPasswordTxt" runat="server" TextMode="Password" />
            </div>
        </asp:PlaceHolder>
    
        <div class="clear"></div>
    
        <br /><br />
    
        
        <btn:CheckboxButton ID="_okBtn" runat="server" TextID="/common/reset" CssClass="ckbxButton roundedCorners border999 shadow999 orangeButton" ToolTipTextID="/pageText/register.aspx/ResetTooltip" />
        <btn:CheckboxButton ID="_cancelBtn" runat="server" TextID="/common/cancel" CssClass="ckbxButton roundedCorners border999 shadow999 redButton" style="margin-left:25px; display:none;" ToolTipTextID="/pageText/register.aspx/cancelTooltip" />
        
        <div class="clear"></div>
    </div>
</asp:Content>