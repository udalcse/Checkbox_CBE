<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="RadioButtonScale.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.SurveyMobile.RadioButtonScale" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemRenderers/SurveyMobile/RadioButtonScale_Vertical.ascx" TagName="RadioButtonsVertical" TagPrefix="ckbx" %>
<%@ Register TagPrefix="ckbx" TagName="QuestionText" Src="~/Forms/Surveys/Controls/ItemRenderers/QuestionText.ascx" %>
<asp:Panel ID="_containerPanel" runat="server" CssClass="itemContainer">
    <asp:Panel ID="_contentPanel" runat="server" CssClass="itemContent">
        <asp:Panel ID="_topAndOrLeftPanel" runat="server" CssClass="topAndOrLeftContainer">
            <asp:Panel ID="_textContainer" runat="server" CssClass="textContainer">
                <ckbx:QuestionText ID="_questionText" runat="server" />
            </asp:Panel>
        </asp:Panel>
        <asp:Panel ID="_bottomAndOrRightPanel" runat="server" CssClass="bottomAndOrRightContainer">
            <asp:Panel ID="_inputPanel" runat="server" CssClass="rating-scale-container Answer">
                <ckbx:RadioButtonsVertical ID="_verticalButtons" runat="server" OnDataBound="VerticalButtons_DataBound" />
            </asp:Panel>
        </asp:Panel>
    </asp:Panel>
</asp:Panel>

<div class="clear"></div>
