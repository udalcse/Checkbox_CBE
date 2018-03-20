<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="OptionEditWidget.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemEditors.OptionEditWidget" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Register Src="~/Controls/Piping/PipeControl.ascx" TagName="PipeSelector" TagPrefix="pipe" %>

<div>
    <asp:Panel ID="_errorPanel" runat="server" Visible="false">
        <ckbx:MultiLanguageLabel ID="_errorLabel" TextId="/pageText/forms/surveys/itemEditors/optionEditWidget/pointsFormat" runat="server" CssClass="error message"></ckbx:MultiLanguageLabel>
        <br />
    </asp:Panel>
    <div class="left input">
        <asp:CheckBox ID="_selectedCheck" runat="server" />
        <asp:RadioButton ID="_selectedRadio" runat="server" GroupName="DefaultSelected" />
        <asp:TextBox ID="_optionText" runat="server" Width="350px" />
        <asp:TextBox ID="_optionPoint" runat="server" Width="50px" />
    </div>
    <div class="right" style="margin-top: 3px; margin-right: 5px;">
        <ckbx:MultiLanguageLinkButton runat="server" ID="_deleteBtn" CommandName="DeleteOption" TextId="/pageText/forms/surveys/itemEditors/optionEditWidget/delete" />
    </div>
    <div class="right" style="padding:0 5px;">&nbsp;</div>
    <div class="right input">
        <a href="javascript:void(0);" onclick="$('#slide_<%# this.ID %>').slideToggle('normal');">
            <%#WebTextManager.GetText("/pageText/forms/surveys/itemEditors/optionEditWidget/toggle") %></a>
    </div>
    <div id="slide_<%# this.ID %>" style="display: none; margin-left: 20px;">
        <pipe:PipeSelector ID="_pipeSelector" runat="server" AllowProfilePipes="true" AllowResponseInfoPipes="true" AllowResponseTemplatePipes="true" AllowSurveyItemPipes="true" />
        <%# WebTextManager.GetText("/pageText/forms/surveys/itemEditors/optionsNormalEntry.ascx/alias") %>
        <asp:TextBox ID="_optionAlias" runat="server" />
    </div>
</div>
