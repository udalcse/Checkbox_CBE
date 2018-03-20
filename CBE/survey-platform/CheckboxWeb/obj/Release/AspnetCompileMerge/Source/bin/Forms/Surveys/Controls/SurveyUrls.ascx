<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="SurveyUrls.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.SurveyUrls" %>
<%@ Import Namespace="Checkbox.Web" %>

<div class="dialogSubContainer">
    <div class="<%=ContainerCSSClass%>">
        <div class="launchSurveyShareStepInstructions error message">
            <%=WebTextManager.GetText(MessageTextID)%>
        </div>
        <br class="clear" />
        <br />
        <br />
        <div class="urlLabel">
            <ckbx:MultiLanguageLabel runat="server" ID="_guidUrlLabel"/>
            <br class="clear" />
            <br />
        </div>
        <asp:Panel runat="server" ID="_customUrlPanel">
            <asp:HyperLink ID="_customSurveyUrl" runat="server" CssClass="ckbxLink" target="_blank"></asp:HyperLink>
            <asp:Image ID="_customSurveyUrlImage" runat="server" SkinID="NewWindow" />
            <br class="clear" />
            <br class="clear" />
        </asp:Panel>
        <asp:HyperLink ID="_guidUrl" runat="server" CssClass="ckbxLink" target="_blank"></asp:HyperLink>
        <asp:Image ID="_guidUrlImage" runat="server" SkinID="NewWindow" />
        <asp:TextBox ID="_htmlCode" runat="server" TextMode="MultiLine" Width="425" Height="65" ReadOnly="true"  />
    </div>
</div>
<br class="clear" />