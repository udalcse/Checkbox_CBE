<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="MessageViewControl.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Invitations.Controls.MessageViewControl" %>
<div style="border: solid 1px #cccccc; padding:5px;">
    <div style="width:80px; height: 30px; vertical-align:middle; float:left;">
        <ckbx:MultiLanguageLabel ID="_fromNameLabel" runat="server" TextId="/controlText/forms/surveys/invitations/controls/messageViewControl.ascx/fromLabel" AssociatedControlID="_fromName" />
    </div>
    <div style="display:inline-block;height: 30px; vertical-align:middle; float:left;">
        <asp:TextBox ID="_fromName" runat="server" Enabled="false" Width="615" />        
    </div>
    <div class="clear"></div>
    <div style="width:80px; height: 30px; vertical-align:middle; float:left;">
        <ckbx:MultiLanguageLabel ID="_subjectLabel" runat="server" TextId="/controlText/forms/surveys/invitations/controls/messageViewControl.ascx/subjectLabel" AssociatedControlID="_subject" />
    </div>
    <div style="display:inline-block; vertical-align:middle; float:left;">
        <asp:TextBox ID="_subject" runat="server" Enabled="false" Width="615"/>
    </div>
    <div class="clear"></div>  
<%--    <div style="width:150px; height: 30px; vertical-align:middle; float:left;">
        <ckbx:MultiLanguageLabel ID="_bodyLabel" runat="server" TextId="/controlText/forms/surveys/invitations/controls/messageViewControl.ascx/bodyLabel" />
    </div> --%>
    <div style="border: solid 1px #cccccc; padding:5px;">                  
        <asp:Panel ID="_messageBodyPanel" runat="server">
            <pre>
            <asp:Literal ID="_messageBodyContents" runat="server" />
            </pre>
        </asp:Panel>
    </div>
    <div class="clear"></div>
</div>