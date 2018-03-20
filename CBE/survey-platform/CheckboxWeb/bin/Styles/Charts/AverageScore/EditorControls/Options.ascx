<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Options.ascx.cs" Inherits="CheckboxWeb.Styles.Charts.AverageScore.EditorControls.Options" %>
<%@ Import Namespace="Checkbox.Web"%>

 <script type="text/javascript">
    $(document).ready(function () {
        $('#<%=ID %>_BarColor').mColorPicker({imageFolder:'<%=ResolveUrl("~/App_Themes/CheckboxTheme/images") %>/'});
        $('#<%=ID %>_backgroundColor').mColorPicker({imageFolder:'<%=ResolveUrl("~/App_Themes/CheckboxTheme/images") %>/'});
        $('#<%=ID %>_legendBackgroundColor').mColorPicker({imageFolder:'<%=ResolveUrl("~/App_Themes/CheckboxTheme/images") %>/'});
        $('#<%=ID %>_plotBackgroundColor').mColorPicker({imageFolder:'<%=ResolveUrl("~/App_Themes/CheckboxTheme/images") %>/'});
        $('#<%=ID %>_pieBorderColor').mColorPicker({imageFolder:'<%=ResolveUrl("~/App_Themes/CheckboxTheme/images") %>/'});
        
        $('#<%=_widthTxt.ClientID %>').numeric({ decimal: false, negative: false });
        $('#<%=_heightTxt.ClientID %>').numeric({ decimal: false, negative: false });

        $('#itemEditorTabs a').click(function(){$(window).resize();});
    });

    //
    function <%=ID %>UpdateSelectedColor(newColor){
        $('#<%=_selectedBarColor.ClientID %>').val(newColor);
    }

    //
    function <%=ID %>_backgroundColor_UpdateSelectedColor(newColor){
        $('#<%=_backgroundColor.ClientID %>').val(newColor);
    }

    //
    function <%=ID %>_plotBackgroundColor_UpdateSelectedColor(newColor){
        $('#<%=_plotBackgroundColor.ClientID %>').val(newColor);
    }

    //
    function <%=ID %>_legendBackgroundColor_UpdateSelectedColor(newColor){
        $('#<%=_legendBackgroundColor.ClientID %>').val(newColor);
    }

    //
    function <%=ID %>_pieBorderColor_UpdateSelectedColor(newColor){
        $('#<%=_pieBorderColor.ClientID %>').val(newColor);
    }
    
</script>



<!--
<div class="styleSectionHeader"><%= WebTextManager.GetText("/pageText/styles/charts/editor.aspx/graphType")%></div>
-->

<div style="display:none;">
    <asp:TextBox ID="_selectedBarColor" runat="server" />
    <asp:TextBox ID="_backgroundColor" runat="server" />
    <asp:TextBox ID="_plotBackgroundColor" runat="server" />
    <asp:TextBox ID="_legendBackgroundColor" runat="server" />
    <asp:TextBox ID="_pieBorderColor" runat="server" />
</div>

<div class="formInput">
    <div class="left checkBox"><asp:CheckBox ID="_showTitle" runat="server" /></div>
    <div class="left"><p><ckbx:MultiLanguageLabel AssociatedControlID="_animation" ID="MultiLanguageLabel4" runat="server" TextId="/pageText/styles/charts/graphOptions.ascx/showTitle" /></p></div>
    <br class="clear" />
</div>

<div class="formInput">
    <div class="left checkBox"><asp:CheckBox ID="_animation" runat="server" /></div>
    <div class="left"><p><ckbx:MultiLanguageLabel AssociatedControlID="_animation" ID="MultiLanguageLabel2" runat="server" TextId="/pageText/styles/charts/graphOptions.ascx/animation" /></p></div>
    <br class="clear" />
</div>

<div class="formInput">
    <div class="left"><p><ckbx:MultiLanguageLabel AssociatedControlID="_backgroundColor" ID="backgroundColorLbl" runat="server" TextId="/pageText/styles/charts/graphOptions.ascx/backgroundColor" /></p></div>
    <div class="left" style="margin-left:10px;margin-top:5px;"><input name="<%=ID %>_backgroundColor" id="<%=ID %>_backgroundColor" type="color" hex="true" text="hidden" style="width:20px;height:20px;display:none;" value="<%=_backgroundColor.Text %>" onchange="<%=ID %>_backgroundColor_UpdateSelectedColor(this.value);" /></div>
    <br class="clear" />
    <div class="left"><p><ckbx:MultiLanguageLabel AssociatedControlID="_plotBackgroundColor" ID="plotBackgroundColorLbl" runat="server" TextId="/pageText/styles/charts/graphOptions.ascx/plotBackgroundColor" /></p></div>
    <div class="left" style="margin-left:10px;margin-top:5px;"><input name="<%=ID %>_plotBackgroundColor" id="<%=ID %>_plotBackgroundColor" type="color" hex="true" text="hidden" style="width:20px;height:20px;display:none;" value="<%=_plotBackgroundColor.Text %>" onchange="<%=ID %>_plotBackgroundColor_UpdateSelectedColor(this.value);" /></div>
    <br class="clear" />
    <div class="left"><p><ckbx:MultiLanguageLabel AssociatedControlID="_legendBackgroundColor" ID="legendBackgroundColorLbl" runat="server" TextId="/pageText/styles/charts/graphOptions.ascx/legendBackgroundColor" /></p></div>
    <div class="left" style="margin-left:10px;margin-top:5px;"><input name="<%=ID %>_legendBackgroundColor" id="<%=ID %>_legendBackgroundColor" type="color" hex="true" text="hidden" style="width:20px;height:20px;display:none;" value="<%=_legendBackgroundColor.Text %>" onchange="<%=ID %>_legendBackgroundColor_UpdateSelectedColor(this.value);" /></div>
    <br class="clear" />
</div>

<div class="formInput">
    <p><ckbx:MultiLanguageLabel ID="_widthLbl" runat="server" AssociatedControlID="_widthTxt" TextId="/controlText/AppearanceEditor/width" /></p>
    <asp:TextBox ID="_widthTxt" runat="server" Width="40px" />

    <p><ckbx:MultiLanguageLabel ID="MultiLanguageLabel1" runat="server" AssociatedControlID="_heightTxt" TextId="/controlText/AppearanceEditor/height" /></p>
    <asp:TextBox ID="_heightTxt" runat="server" Width="40px" />
</div>


 