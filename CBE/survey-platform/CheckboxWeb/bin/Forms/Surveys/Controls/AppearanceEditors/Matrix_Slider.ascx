<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Matrix_Slider.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.AppearanceEditors.MatrixSlider" %>

<script type="text/javascript">
    $(document).ready(function () {
        if (typeof sliderType != 'undefined' && sliderType == 'NumberRange') {
            $('#<%= _numericSliderOptionsPanel.ClientID %>').show();
        } else {
            $('#<%= _numericSliderOptionsPanel.ClientID %>').hide();
        }
    });
</script>

<div class="padding10">
    <asp:Panel id="_numericSliderOptionsPanel" runat="server">
        <div class="left input fixed_100">
            <ckbx:MultiLanguageLabel ID="_showValueLbl" runat="server" TextId="/controlText/singleLineTextAppearanceEditor/showValue" />
        </div>

        <div class="left input">
            <asp:CheckBox ID="_showLabelCkbx" runat="server" />
        </div>
    </asp:Panel>

    <br class="clear" />
</div>