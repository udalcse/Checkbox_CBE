<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Border.ascx.cs" Inherits="CheckboxWeb.Styles.Charts.EditorControls.Border" %>
<%@ Import Namespace="Checkbox.Web"%>


 <script type="text/javascript">
    $(document).ready(function () {
        $('#<%=ID %>_FrameBgColor').mColorPicker({imageFolder:'<%=ResolveUrl("~/App_Themes/CheckboxTheme/images") %>/'});
        $('#<%=ID %>_LineColor').mColorPicker({imageFolder:'<%=ResolveUrl("~/App_Themes/CheckboxTheme/images") %>/'});
        $('#<%=_lineWidth.ClientID %>').numeric({ decimal: false, negative: false });
    });

    //
    function <%=ID %>_FrameBgColor_UpdateSelectedColor(newColor){
        $('#<%=_frameBgColor.ClientID %>').val(newColor);
    }

      //
    function <%=ID %>_LineColor_UpdateSelectedColor(newColor){
        $('#<%=_lineColor.ClientID %>').val(newColor);
    }
</script>

<!--
<div class="styleSectionHeader"><%= WebTextManager.GetText("/pageText/styles/charts/editor.aspx/border")%></div>
-->

<div style="display:none;">
    <asp:TextBox ID="_frameBgColor" runat="server" />
    <asp:TextBox ID="_lineColor" runat="server" />
</div>

<div class="formInput">
    <% if (ShowBorderFrameStyle) { %>
    <p><ckbx:MultiLanguageLabel AssociatedControlID="_borderStyle" ID="borderStyleLbl" runat="server" TextId="/pageText/styles/charts/border.ascx/borderStyle" /></p>
    <ckbx:MultiLanguageDropDownList ID="_borderStyle" runat="server" />    
    <% } %>
    
    <% if (ShowBorderLineStyle) {%>
    <p><ckbx:MultiLanguageLabel AssociatedControlID="_lineStyle" ID="lineStyleLbl" runat="server" TextId="/pageText/styles/charts/border.ascx/lineStyle" /></p>
    <ckbx:MultiLanguageDropDownList ID="_lineStyle" runat="server" />
    <% } %>

    <p><ckbx:MultiLanguageLabel AssociatedControlID="_lineWidth" ID="lineWidthLbl" runat="server" TextId="/pageText/styles/charts/border.ascx/lineWidth" /></p>
    <asp:TextBox ID="_lineWidth" runat="server" Width="40px" />
    <br class="clear" />

    <p><ckbx:MultiLanguageLabel AssociatedControlID="_borderRadius" ID="borderRadiusLbl" runat="server" TextId="/pageText/styles/charts/border.ascx/borderRadius" /></p>
    <asp:TextBox ID="_borderRadius" runat="server" Width="40px" />
    <br class="clear" />

    <% if (ShowFrameBgColor)
       { %>
    <div class="left"><p><ckbx:MultiLanguageLabel AssociatedControlID="_frameBgColor" ID="backgroundColorLbl" runat="server" TextId="/pageText/styles/charts/border.ascx/backgroundColor" /></p></div>
    <div class="left" style="margin-left:10px;margin-top:5px;"><input name="<%=ID %>_FrameBgColor" id="<%=ID %>_FrameBgColor" type="color" hex="true" text="hidden" style="width:20px;height:20px;display:none;" value="<%=_frameBgColor.Text %>" onchange="<%=ID %>_FrameBgColor_UpdateSelectedColor(this.value);" /></div>
    <br class="clear" />
    <% } %>

    <div class="left"><p><ckbx:MultiLanguageLabel AssociatedControlID="_lineColor" ID="lineColorLbl" runat="server" TextId="/pageText/styles/charts/border.ascx/lineColor" /></p></div>
    <div class="left" style="margin-left:10px;margin-top:5px;"><input name="<%=ID %>_LineColor" id="<%=ID %>_LineColor" type="color" hex="true" text="hidden" style="width:20px;height:20px;display:none;" value="<%=_lineColor.Text %>" onchange="<%=ID %>_LineColor_UpdateSelectedColor(this.value);" /></div>
    <br class="clear" />
</div>