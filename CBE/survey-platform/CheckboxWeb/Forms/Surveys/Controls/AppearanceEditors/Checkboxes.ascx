<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Checkboxes.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.AppearanceEditors.Checkboxes" %>
<script type="text/javascript">
    $(document).ready(function() {
        $('#<%=_columnsTxt.ClientID %>').numeric({ decimal: false, negative: false });
    });
</script>


<div class="formInput left fixed_250">
    <p><ckbx:MultiLanguageLabel AssociatedControlID="_labelPositionList" ID="_labelPositionLbl" runat="server" TextId="/controlText/selectItemAppearanceEditor/labelPosition" /></p>
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

<div class="formInput left fixed_250">
    <p><ckbx:MultiLanguageLabel AssociatedControlID="_itemPositionList" ID="_itemPositionLbl" runat="server" TextId="/controlText/selectItemAppearanceEditor/itemPosition" /></p>
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
    <p><ckbx:MultiLanguageLabel AssociatedControlID="_layoutList" ID="_layoutLbl" runat="server" TextId="/controlText/repeatDirectionEditor/layout" /></p>
</div>
<div class="formInput left">
    <ckbx:MultiLanguageDropDownList ID="_layoutList" runat="server">
            <asp:ListItem Value="Vertical" TextId="/enum/repeatDirection/vertical" />
            <asp:ListItem Value="Horizontal" TextId="/enum/repeatDirection/horizontal" />
        </ckbx:MultiLanguageDropDownList>
</div>
<br class="clear"/>

<div class="formInput left fixed_250">
    <p><ckbx:MultiLanguageLabel AssociatedControlID="_columnsTxt" ID="_columnsLbl" runat="server" TextId="/controlText/selectItemAppearanceEditor/columns" /></p>
</div>
<div class="formInput left">
    <asp:TextBox ID="_columnsTxt" runat="server" />
</div>
<br class="clear"/>

<div class="formInput left fixed_250">
    <p><ckbx:MultiLanguageLabel AssociatedControlID="_numberLabelsChk" ID="_numberLabelsLbl" runat="server" TextId="/controlText/selectItemAppearanceEditor/showNumberLabels" /></p>
</div>
<div class="formInput left">
    <asp:CheckBox ID="_numberLabelsChk" runat="server" />
</div>
<br class="clear"/>
    
