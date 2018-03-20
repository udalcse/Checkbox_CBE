<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Redirect.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.AppearanceEditors.Redirect" %>

<script type="text/javascript">
    $(document).ready(function () {
        $('#<%=ID %>_Color').mColorPicker({imageFolder:'<%=ResolveUrl("~/App_Themes/CheckboxTheme/images") %>/'});
        $('#<%=_fontSizeTxt.ClientID %>').numeric({ decimal: false, negative: false });
    });

    function <%=ID %>UpdateSelectedColor(newColor){
        $('#<%=_selectedColor.ClientID %>').val(newColor);
    }
</script>

<div style="display:none;">
    <asp:TextBox ID="_selectedColor" runat="server" />
</div>

<div class="padding10">
    <div class="formInput">
        <div class="left">
            <p><label><ckbx:MultiLanguageLiteral ID="_fontColorLbl" runat="server" TextId="/controlText/messageItemAppearanceEditor/color" /></label></p>
        </div>
        <div class="left" style="margin-left: 5px;margin-top:5px;">
            <input name="<%=ID %>_Color" id="<%=ID %>_Color" type="color" hex="true" text="hidden" style="width:20px;height:20px;display:none;" value="<%=_selectedColor.Text %>" onchange="<%=ID %>UpdateSelectedColor(this.value);" />
        </div>
    </div>
    
    <br class="clear" />
    <br class="clear" />
    
    <div class="formInput">
        <p><ckbx:MultiLanguageLabel AssociatedControlID="_fontSizeTxt" ID="_fontSizeLbl" runat="server" TextId="/controlText/messageItemAppearanceEditor/size" /></p>
        <asp:TextBox ID="_fontSizeTxt" Width="50px" runat="server" /> px
    </div>
    
    <br class="clear" />
    
    
    <div class="formInput">
        <p><ckbx:MultiLanguageLabel AssociatedControlID="_itemPositionList" ID="_itemPositionLbl" runat="server" TextId="/controlText/messageItemAppearanceEditor/itemPosition" /></p>
        <ckbx:MultiLanguageDropDownList ID="_itemPositionList" runat="server">
            <asp:ListItem Value="Left" Text="Left" TextId="/enum/itemPosition/left" />
            <asp:ListItem Value="Center" Text="Center" TextId="/enum/itemPosition/center" />
            <asp:ListItem Value="Right" Text="Right" TextId="/enum/itemPosition/right" />
        </ckbx:MultiLanguageDropDownList>
    </div>     
</div>