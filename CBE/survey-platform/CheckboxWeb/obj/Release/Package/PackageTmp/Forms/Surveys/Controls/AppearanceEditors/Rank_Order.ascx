<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Rank_Order.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.AppearanceEditors.Rank_Order" %>

<script type="text/javascript">
    $(document).ready(function () {
        if (rankOrderType == 'Text') {
            $('#optionLabelOrientation').hide();
        }
    });
</script>


<div class="formInput left fixed_250">
    <p><ckbx:MultiLanguageLabel ID="_itemPositionLbl" AssociatedControlID="_itemPositionList" runat="server" TextId="/controlText/selectItemAppearanceEditor/itemPosition" /></p>
</div>
<div class="formInput left">
        <ckbx:MultiLanguageDropDownList ID="_itemPositionList" runat="server">
        <asp:ListItem Value="Left" TextId="/enum/itemPosition/left" />
        <asp:ListItem Value="Center" TextId="/enum/itemPosition/center" />
        <asp:ListItem Value="Right" TextId="/enum/itemPosition/right" />
    </ckbx:MultiLanguageDropDownList>
</div>
<br class="clear"/>

<div class="formInput left fixed_250">
    <p><ckbx:MultiLanguageLabel ID="_labelPositionLbl" AssociatedControlID="_labelPositionList" runat="server" TextId="/controlText/selectItemAppearanceEditor/labelPosition" /></p>
</div>
<div class="formInput left">
    <ckbx:MultiLanguageDropDownList ID="_labelPositionList" runat="server">
        <asp:ListItem Value="Top" TextId="/enum/labelPosition/top" />
        <asp:ListItem Value="Left" TextId="/enum/labelPosition/left" />
        <asp:ListItem Value="Bottom" TextId="/enum/labelPosition/bottom" />
        <asp:ListItem Value="Right" TextId="/enum/labelPosition/right" />
    </ckbx:MultiLanguageDropDownList>
</div>
<br class="clear"/>

<div id="optionLabelOrientation">
    <div class="formInput left fixed_250">
        <p><ckbx:MultiLanguageLabel AssociatedControlID="_optionLabelOrientationList" ID="_optionLabelOrientationLbl" runat="server" TextId="/itemType/rankOrderEditor/optionLabelPosition" /></p>
    </div>
    <div class="formInput left">
        <ckbx:MultiLanguageDropDownList ID="_optionLabelOrientationList" runat="server" uframeIgnore="true">
            <asp:ListItem Value="DontShow" Text="Don't show" TextId="/enum/aliasPosition/dontShow" />
            <asp:ListItem Value="Top" Text="Top" TextId="/enum/aliasPosition/top" />
            <asp:ListItem Value="Bottom" Text="Bottom" TextId="/enum/aliasPosition/bottom" />
        </ckbx:MultiLanguageDropDownList>
    </div>
</div>
<br class="clear"/>
