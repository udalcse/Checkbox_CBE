<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Text.ascx.cs" Inherits="CheckboxWeb.Styles.Charts.EditorControls.Text" %>
<%@ Import Namespace="Checkbox.Web"%>

 <script type="text/javascript">
        $(document).ready(function () {
            $('#<%=ID %>_TextColor').mColorPicker({imageFolder:'<%=ResolveUrl("~/App_Themes/CheckboxTheme/images") %>/'});
            $('#<%=ID %>_legendTextColor').mColorPicker({imageFolder:'<%=ResolveUrl("~/App_Themes/CheckboxTheme/images") %>/'});
            $('#<%=ID %>_hintTextColor').mColorPicker({imageFolder:'<%=ResolveUrl("~/App_Themes/CheckboxTheme/images") %>/'});
        });

        //
        function <%=ID %>UpdateTextColor(newColor){
            $('#<%=_textColor.ClientID %>').val(newColor);
        }

        function <%=ID %>_legendTextColor_UpdateSelectedColor(newColor){
            $('#<%=_legendTextColor.ClientID %>').val(newColor);
        }

        function <%=ID %>_hintTextColor_UpdateSelectedColor(newColor){
            $('#<%=_hintTextColor.ClientID %>').val(newColor);
        }
    </script>

<!--
<div class="styleSectionHeader"><%= WebTextManager.GetText("/pageText/styles/charts/editor.aspx/text")%></div>
-->

<div style="display:none;">
    <asp:TextBox ID="_textColor" runat="server" />
    <asp:TextBox ID="_legendTextColor" runat="server" />
    <asp:TextBox ID="_hintTextColor" runat="server" />
</div>

<div class="formInput">
    <p><ckbx:MultiLanguageLabel AssociatedControlID="_font" ID="fontLbl" runat="server" TextId="/pageText/styles/charts/text.ascx/font" /></p>
    <ckbx:MultiLanguageDropDownList ID="_font" runat="server" />
    
    <p><ckbx:MultiLanguageLabel ID="titleSizeLbl" AssociatedControlID="_titleSize" runat="server" TextId="/pageText/styles/charts/text.ascx/titleSize" /></p>
    <ckbx:MultiLanguageDropDownList ID="_titleSize" runat="server" />

    <p><ckbx:MultiLanguageLabel ID="wrapTitleCharsLbl" AssociatedControlID="_wrapTitleChars" runat="server" TextId="/pageText/styles/charts/text.ascx/wrapTitleChars" /></p>
    <asp:TextBox ID="_wrapTitleChars" runat="server" />

    <p><ckbx:MultiLanguageLabel ID="legendSizeLbl" AssociatedControlID="_legendSize" runat="server" TextId="/pageText/styles/charts/text.ascx/legendSize" /></p>
    <ckbx:MultiLanguageDropDownList ID="_legendSize" runat="server" />

    <p><ckbx:MultiLanguageLabel ID="labelSizeLbl" runat="server" AssociatedControlID="_labelSize" TextId="/pageText/styles/charts/text.ascx/labelSize" /></p>
    <ckbx:MultiLanguageDropDownList ID="_labelSize" runat="server" />
    <br class="clear" />

    <div class="left"><p><ckbx:MultiLanguageLabel ID="textColorLbl" runat="server" AssociatedControlID="_textColor" TextId="/pageText/styles/charts/text.ascx/textColor" /></p></div>
    <div class="left" style="margin-left:10px;margin-top:5px;"><input name="<%=ID %>_TextColor" id="<%=ID %>_TextColor" type="color" hex="true" text="hidden" style="width:20px;height:20px;display:none;" value="<%=_textColor.Text %>" onchange="<%=ID %>UpdateTextColor(this.value);" /></div>
    <br class="clear" />

    <div class="left"><p><ckbx:MultiLanguageLabel AssociatedControlID="_legendTextColor" ID="legendColorLbl" runat="server" TextId="/pageText/styles/charts/text.ascx/legendTextColor" /></p></div>
    <div class="left" style="margin-left:10px;margin-top:5px;"><input name="<%=ID %>_legendTextColor" id="<%=ID %>_legendTextColor" type="color" hex="true" text="hidden" style="width:20px;height:20px;display:none;" value="<%=_legendTextColor.Text %>" onchange="<%=ID %>_legendTextColor_UpdateSelectedColor(this.value);" /></div>
    <br class="clear" />

    <div class="left"><p><ckbx:MultiLanguageLabel AssociatedControlID="_hintTextColor" ID="MultiLanguageLabel1" runat="server" TextId="/pageText/styles/charts/text.ascx/hintTextColor" /></p></div>
    <div class="left" style="margin-left:10px;margin-top:5px;"><input name="<%=ID %>_hintTextColor" id="<%=ID %>_hintTextColor" type="color" hex="true" text="hidden" style="width:20px;height:20px;display:none;" value="<%=_hintTextColor.Text %>" onchange="<%=ID %>_hintTextColor_UpdateSelectedColor(this.value);" /></div>
    <br class="clear" />
</div>