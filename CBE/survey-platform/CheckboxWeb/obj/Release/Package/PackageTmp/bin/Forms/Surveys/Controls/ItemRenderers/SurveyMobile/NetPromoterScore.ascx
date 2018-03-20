<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="NetPromoterScore.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.SurveyMobile.NetPromoterScore" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemRenderers/SurveyMobile/NetPromoterScore_Vertical.ascx" TagName="NetPromoterScore" TagPrefix="ckbx" %>
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
                <ckbx:NetPromoterScore ID="_verticalButtons" runat="server" OnDataBound="VerticalButtons_DataBound" />
            </asp:Panel>
        </asp:Panel>
    </asp:Panel>
</asp:Panel>

<div class="clear"></div>
