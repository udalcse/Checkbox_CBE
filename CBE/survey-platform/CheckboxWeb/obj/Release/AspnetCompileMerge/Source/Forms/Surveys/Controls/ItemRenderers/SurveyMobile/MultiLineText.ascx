<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="MultiLineText.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.SurveyMobile.MultiLineText" %>
<%@ Register TagPrefix="ckbx" TagName="QuestionText" Src="~/Forms/Surveys/Controls/ItemRenderers/QuestionText.ascx" %>
<ckbx:ResolvingScriptElement runat="server" Source="~/Resources/jquery.ui.touch-punch.min.js" />
<style>
    .ui-wrapper {
        padding-bottom: 30px !important;
        width: 100% !important;
    }
    .ui-icon {
        width: 19px;
        height: 19px;
    }
    .multilineInput {
        width: 100%;
    }
    .mobile-multiline {
        height: initial !important;
    }
</style>
<asp:Panel ID="_containerPanel" runat="server" CssClass="itemContainer">
    <asp:Panel ID="_contentPanel" runat="server" CssClass="itemContent">
        <asp:Panel ID="_topAndOrLeftPanel" runat="server" CssClass="topAndOrLeftContainer">
            <asp:Panel ID="_textContainer" runat="server" CssClass="textContainer">
                <ckbx:QuestionText ID="_questionText" runat="server" />
            </asp:Panel>
        </asp:Panel>

        <asp:Panel ID="_bottomAndOrRightPanel" runat="server" CssClass="bottomAndOrRightContainer">
            <asp:Panel ID="_inputPanel" runat="server" CssClass="inputContainer multilineInput">
               <div class="validationError" style="color: red; display:none">Answer must contain 2 or more characters.</div>
               <asp:TextBox TextMode="MultiLine" ID="_textInput" CssClass="mobile-multiline" runat="server" />
            </asp:Panel>
        </asp:Panel>
    </asp:Panel>
</asp:Panel>
<script>
    //need for ipad fix
    var container = $('<%=_inputPanel.ClientID%>');

</script>

