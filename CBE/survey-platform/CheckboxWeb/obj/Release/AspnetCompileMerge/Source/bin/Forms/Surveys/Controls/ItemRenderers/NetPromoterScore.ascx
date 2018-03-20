<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="NetPromoterScore.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.NetPromoterScore" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemRenderers/NetPromoterScore_Horizontal.ascx" TagName="RadioButtonsHorizontal" TagPrefix="ckbx" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemRenderers/NetPromoterScore_Vertical.ascx" TagName="RadioButtonsVertical" TagPrefix="ckbx" %>
<%@ Register TagPrefix="ckbx" TagName="QuestionText" Src="~/Forms/Surveys/Controls/ItemRenderers/QuestionText.ascx" %>
    <script type="text/javascript">
        $(function () {
            if ($('#<%= _topAndOrLeftPanel.ClientID %>').hasClass('labelRight')) {

                var question = $('#<%= _textContainer.ClientID %>').find('.Question.itemNumber');

                if (question.length > 0) {
                    var margin = -$('#<%= _inputPanel.ClientID %>').width() - 5 + parseInt(question.css('margin-left').replace('px', ''));
                    question.css('margin-left', margin + 'px');
                }
            }
        });        
    </script>
   
<asp:Panel ID="_containerPanel" runat="server" CssClass="itemContainer">
    <asp:Panel ID="_contentPanel" runat="server" CssClass="itemContent">
        <asp:Panel ID="_topAndOrLeftPanel" runat="server" CssClass="topAndOrLeftContainer">
            <asp:Panel ID="_textContainer" runat="server" CssClass="textContainer">
                <ckbx:QuestionText ID="_questionText" runat="server" />
            </asp:Panel>
        </asp:Panel>
        <asp:Panel ID="_bottomAndOrRightPanel" runat="server" CssClass="bottomAndOrRightContainer">
            <asp:Panel ID="_inputPanel" runat="server" CssClass="inputContainer Answer">
                <ckbx:RadioButtonsHorizontal ID="_horizontalButtons" runat="server" OnDataBound="HorizontalButtons_DataBound" />
                <ckbx:RadioButtonsVertical ID="_verticalButtons" runat="server" OnDataBound="VerticalButtons_DataBound" />
            </asp:Panel>
        </asp:Panel>
    </asp:Panel>
</asp:Panel>

<div class="clear"></div>
