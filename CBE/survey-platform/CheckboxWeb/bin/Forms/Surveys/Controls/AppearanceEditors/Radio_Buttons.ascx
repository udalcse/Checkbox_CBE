<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Radio_Buttons.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.AppearanceEditors.Radio_Buttons" %>

<script type="text/javascript">
    $(document).ready(function () {
        $('#<%=_columnsTxt.ClientID %>').numeric({ decimal: false, negative: false });
    });
</script>


<div class="formInput left fixed_250">
    <p><ckbx:MultiLanguageLabel ID="_labelPositionLbl" AssociatedControlID="_labelPositionList" runat="server" TextId="/controlText/selectItemAppearanceEditor/labelPosition" /></p>
</div>
<div class="formInput left">
     <ckbx:MultiLanguageDropDownList ID="_labelPositionList" runat="server">
            <asp:ListItem Value="Top" Text="Top" TextId="/enum/labelPosition/top" />
            <asp:ListItem Value="Left" Text="Left" TextId="/enum/labelPosition/left" />
            <asp:ListItem Value="Bottom" Text="Bottom" TextId="/enum/labelPosition/bottom" />
            <asp:ListItem Value="Right" Text="Right" TextId="/enum/labelPosition/right" />
        </ckbx:MultiLanguageDropDownList>
</div>
<br class="clear"/>

<div class="formInput left fixed_250">
     <p><ckbx:MultiLanguageLabel ID="_itemPositionLbl" AssociatedControlID="_itemPositionList" runat="server" TextId="/controlText/selectItemAppearanceEditor/itemPosition" /></p>
</div>
<div class="formInput left">
    <ckbx:MultiLanguageDropDownList ID="_itemPositionList" runat="server">
            <asp:ListItem Value="Left" Text="Left" TextId="/enum/itemPosition/left" />
            <asp:ListItem Value="Center" Text="Center" TextId="/enum/itemPosition/center" />
            <asp:ListItem Value="Right" Text="Right" TextId="/enum/itemPosition/right" />
        </ckbx:MultiLanguageDropDownList>
</div>
<br class="clear"/>

<div class="formInput left fixed_250">
    <p><ckbx:MultiLanguageLabel ID="_layoutLbl" AssociatedControlID="_layoutList" runat="server" TextId="/controlText/repeatDirectionEditor/layout" /></p>
</div>
<div class="formInput left">
    <ckbx:MultiLanguageDropDownList ID="_layoutList" runat="server">
            <asp:ListItem Value="Vertical" Text="Vertical" TextId="/enum/repeatDirection/vertical" />
            <asp:ListItem Value="Horizontal" Text="Horizontal" TextId="/enum/repeatDirection/horizontal" />
        </ckbx:MultiLanguageDropDownList>
</div>
<br class="clear"/>

<div class="formInput left fixed_250">
    <p><ckbx:MultiLanguageLabel ID="_columnsLbl" AssociatedControlID="_columnsTxt" runat="server" TextId="/controlText/selectItemAppearanceEditor/columns" /></p>
</div>
<div class="formInput left">
    <asp:TextBox ID="_columnsTxt" runat="server" Width="40px" />
</div>
<br class="clear"/>

<div class="formInput left fixed_250">
    <p><ckbx:MultiLanguageLabel ID="_numberLabelsLbl" AssociatedControlID="_numberLabelsChk" runat="server" TextId="/controlText/selectItemAppearanceEditor/showNumberLabels" /></p>
</div>
<div class="formInput left">
    <asp:CheckBox ID="_numberLabelsChk" runat="server" />
</div>
<br class="clear"/>
