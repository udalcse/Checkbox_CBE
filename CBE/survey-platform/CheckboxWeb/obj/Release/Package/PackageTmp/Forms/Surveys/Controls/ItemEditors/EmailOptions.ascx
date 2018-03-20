<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="EmailOptions.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemEditors.EmailOptions" %>
<%@ Register Src="~/Controls/Piping/PipeControl.ascx" TagName="PipeSelector" TagPrefix="pipe" %>

<div class="padding10">
    <div class="formInput radioList">
        <p><ckbx:MultiLanguageLabel AssociatedControlID="_formatList" ID="_formatLbl" runat="server" TextId="/controlText/emailItemEditor/emailFormat" /></p>
        <ckbx:MultiLanguageRadioButtonList ID="_formatList" runat="server" AutoPostBack="true">
            <asp:ListItem Text="Html" Value="Html" TextId="/controlText/emailItemEditor/formatHtml" />
            <asp:ListItem Text="Text" Value="Text" TextId="/controlText/emailItemEditor/formatText" />
        </ckbx:MultiLanguageRadioButtonList>
    </div>
    
    <asp:Panel ID="_stylePanel" runat="server">
        <div class="formInput">
            <p><ckbx:MultiLanguageLabel AssociatedControlID="_styleList" ID="_styleLbl" runat="server" TextId="/controlText/emailItemEditor/styleTemplate" /></p>
            <asp:DropDownList ID="_styleList" runat="server" />
        </div>

    </asp:Panel>

    
    <div class="formInput">
        <div class="left"><p><ckbx:MultiLanguageLabel AssociatedControlID="_sendOnceChk" ID="_sendOnceLbl" runat="server" TextId="/controlText/emailItemEditor/sendOnce" /></p></div>
        <div class="left checkBox" style="margin-left:5px"><asp:CheckBox ID="_sendOnceChk" runat="server" /></div>
    </div>
    
    <br class="clear" />


    <div class="formInput" style="display:block;float:left;width:45%">
        <p><ckbx:MultiLanguageLabel AssociatedControlID="_fromTxt" ID="_fromLbl" runat="server" TextId="/controlText/emailItemEditor/emailFrom" /></p>
        <asp:TextBox ID="_fromTxt" runat="server" Width="300" />
        <pipe:PipeSelector ID="_pipeSelector_fromTxt" runat="server" AllowProfilePipes="true" AllowResponseInfoPipes="true" AllowResponseTemplatePipes="true" AllowSurveyItemPipes="true" />
    </div>
    
    <div class="formInput" style="display:block;float:right;width:45%">
        <p><ckbx:MultiLanguageLabel AssociatedControlID="_toTxt" ID="_toLbl" runat="server" TextId="/controlText/emailItemEditor/emailTo" /></p>
        <asp:TextBox ID="_toTxt" runat="server" Width="300" />
        <pipe:PipeSelector ID="_pipeSelector_toTxt" runat="server" AllowProfilePipes="true" AllowResponseInfoPipes="true" AllowResponseTemplatePipes="true" AllowSurveyItemPipes="true" />
    </div>

    <div class="formInput" style="display:block;float:right;width:45%">
        <p><ckbx:MultiLanguageLabel AssociatedControlID="_bccTxt" ID="_bccLbl" runat="server" TextId="/controlText/emailItemEditor/emailBcc" /></p>
        <asp:TextBox ID="_bccTxt" runat="server" Width="300" />
        <pipe:PipeSelector ID="_pipeSelector_bccTxt" runat="server" AllowProfilePipes="true" AllowResponseInfoPipes="true" AllowResponseTemplatePipes="true" AllowSurveyItemPipes="true" />
    </div>
    
     <div class="formInput" style="display:block;float:left;width:45%">
        <p><ckbx:MultiLanguageLabel AssociatedControlID="_subjectTxt" ID="_subjectLbl" runat="server" TextId="/controlText/emailItemEditor/emailSubject" /></p>
        <asp:TextBox ID="_subjectTxt" runat="server" Width="300" />
        <pipe:PipeSelector ID="_pipeSelector_subjectTxt" runat="server" AllowProfilePipes="true" AllowResponseInfoPipes="true" AllowResponseTemplatePipes="true" AllowSurveyItemPipes="true"/>
    </div>
    <br class="clear"/>
</div>