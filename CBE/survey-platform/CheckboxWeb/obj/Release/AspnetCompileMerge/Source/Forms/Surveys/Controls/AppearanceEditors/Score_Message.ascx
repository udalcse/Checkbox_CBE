<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Score_Message.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.AppearanceEditors.Score_Message" %>

<script type="text/javascript">
        $(document).ready(function () {
            $('#<%=ID %>_Color').mColorPicker({imageFolder:'<%=ResolveUrl("~/App_Themes/CheckboxTheme/images") %>/'});
            
            $('#<%=_fontSizeTxt.ClientID %>').numeric({ decimal: false, negative: false });

        });

        //
        function <%=ID %>UpdateSelectedColor(newColor){
            $('#<%=_selectedColor.ClientID %>').val(newColor);
        }
        
    </script>

<div style="display:none;">
    <asp:TextBox ID="_selectedColor" runat="server" />
</div>

<div class="formInput">
    <p><label for="fontColor"><%=Checkbox.Web.WebTextManager.GetText("/controlText/messageItemAppearanceEditor/color")%></label></p>
    <input name="fontColor" id="<%=ID %>_Color" type="color" hex="true" text="hidden" style="width:20px;height:20px;display:none;" value="<%=_selectedColor.Text %>" onchange="<%=ID %>UpdateSelectedColor(this.value);" />
    
    <p><ckbx:MultiLanguageLabel AssociatedControlID="_fontSizeTxt" ID="_fontSizeLbl" runat="server" TextId="/controlText/messageItemAppearanceEditor/size" /></p>
    <asp:TextBox ID="_fontSizeTxt" runat="server" /> px

    <p><ckbx:MultiLanguageLabel AssociatedControlID="_itemPositionList" ID="_itemPositionLbl" runat="server" TextId="/controlText/messageItemAppearanceEditor/itemPosition" /></p>
    <ckbx:MultiLanguageDropDownList ID="_itemPositionList" runat="server">
        <asp:ListItem Value="Left" Text="Left" TextId="/enum/itemPosition/left" />
        <asp:ListItem Value="Center" Text="Center" TextId="/enum/itemPosition/center" />
        <asp:ListItem Value="Right" Text="Right" TextId="/enum/itemPosition/right" />
    </ckbx:MultiLanguageDropDownList>
</div>