<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Slider.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.Slider" %>
<%@ Register TagPrefix="ckbx" TagName="QuestionText" Src="~/Forms/Surveys/Controls/ItemRenderers/QuestionText.ascx" %>
<%@ Register TagPrefix="slider" TagName="NumericSlider" Src="~/Forms/Surveys/Controls/ItemRenderers/NumericSlider.ascx" %>
<%@ Register TagPrefix="slider" TagName="ValueListSlider" Src="~/Forms/Surveys/Controls/ItemRenderers/ValueListSlider.ascx" %>

<asp:Panel ID="_textContainer" runat="server" CssClass="textContainer" style="margin-bottom:10px;">
    <ckbx:QuestionText ID="_questionText" runat="server" />
</asp:Panel>
<br class="clear:both" />
<div class="slider-value-list-container <%= CssClass %> Answer" id="slider_panel_<%= Model.ItemId %>">
    <slider:NumericSlider ID="_numericSlider" runat="server" Visible="false" />
    <slider:ValueListSlider ID="_valueListSlider" runat="server" Visible="false" />
</div>
