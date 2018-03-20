<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="GraphOptions.ascx.cs" Inherits="CheckboxWeb.Styles.Charts.EditorControls.GraphOptions" %>
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
        $('#<%=_donutRadius.ClientID %>').numeric({ decimal: false, negative: false });

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

    //
    function openEditCustomColorsDialog(){
        showDialog('<%=ResolveUrl("~/Styles/PieGraphColors.aspx?onClose=updateColorList&colorList=") %>' + $("#<%=_customPalette.ClientID %>").val().replace(/#/g,''), 600, 580);
    }

    //Update color list with a new value
    function updateColorList(resultData){
        if (resultData.result == 'ok')
            $("#<%=_customPalette.ClientID %>").val(resultData.newColorList);
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

<asp:Panel ID="_chartTypePanel" runat="server" CssClass="formInput">
    <p><ckbx:MultiLanguageLabel runat="server" AssociatedControlID="_graphTypeList" TextId="/pageText/styles/charts/graphOptions.ascx/chartType" /></p>
    <ckbx:MultiLanguageDropDownList ID="_graphTypeList" runat="server" AutoPostBack="true">
        <asp:ListItem TextId="/enum/graphType/BarGraph" Value="BarGraph" />
        <asp:ListItem TextId="/enum/graphType/ColumnGraph" Value="ColumnGraph" />
        <asp:ListItem TextId="/enum/graphType/Doughnut" Value="Doughnut" />
        <asp:ListItem TextId="/enum/graphType/LineGraph" Value="LineGraph" />
        <asp:ListItem TextId="/enum/graphType/PieGraph" Value="PieGraph" />
    </ckbx:MultiLanguageDropDownList>
</asp:Panel>

<div class="formInput">
    <div class="left checkBox"><asp:CheckBox ID="_showTitle" runat="server" /></div>
    <div class="left"><p><ckbx:MultiLanguageLabel AssociatedControlID="_showTitle" ID="MultiLanguageLabel4" runat="server" TextId="/pageText/styles/charts/graphOptions.ascx/showTitle" /></p></div>
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

<asp:Panel ID="_barOptionsPanel" CssClass="formInput" runat="server" style="margin-top:25px;">
    <div class="left">
        <p><label for="<%=ID %>_barColor"><%=WebTextManager.GetText("/pageText/styles/charts/graphOptions.ascx/barLineColor")%></label></p>
    </div>
    <div class="left" style="margin-left:10px;margin-top:5px;">
        <input name="<%=ID %>_BarColor" id="<%=ID %>_BarColor" type="color" hex="true" text="hidden" style="width:20px;height:20px;display:none;" value="<%=_selectedBarColor.Text %>" onchange="<%=ID %>UpdateSelectedColor(this.value);" />
    </div>
    <br class="clear" />
</asp:Panel>

<asp:Panel ID="_pieOptionsPanel" runat="server" CssClass="formInput">
    <div class="left"><p><ckbx:MultiLanguageLabel AssociatedControlID="_pieBorderColor" ID="MultiLanguageLabel3" runat="server" TextId="/pageText/styles/charts/graphOptions.ascx/pieBorderColor" /></p></div>
    <div class="left" style="margin-left:10px;margin-top:5px;"><input name="<%=ID %>_pieBorderColor" id="<%=ID %>_pieBorderColor" type="color" hex="true" text="hidden" style="width:20px;height:20px;display:none;" value="<%=_pieBorderColor.Text %>" onchange="<%=ID %>_pieBorderColor_UpdateSelectedColor(this.value);" /></div>
    <br class="clear" />

    <p><ckbx:MultiLanguageLabel ID="colorPaletteLbl" AssociatedControlID="_colorPalette" runat="server" TextId="/pageText/styles/charts/graphOptions.ascx/colorPalette" /></p>
    <ckbx:MultiLanguageDropDownList ID="_colorPalette" runat="server" AutoPostBack="true" />
        
    <asp:PlaceHolder ID="_pieGraphColorsPlace" runat="server">
        <p><ckbx:MultiLanguageLabel ID="pieGraphColorLbl" AssociatedControlID="_customPalette" runat="server" TextId="/pageText/styles/charts/graphOptions.ascx/pieGraphColor" /></p>
        <textarea id="_customPalette" runat="server" rows="3" />
        <br />
        <ckbx:MultiLanguageLinkButton ID="_editPieColors" runat="server" TextId="/pageText/styles/charts/graphOptions.ascx/editColors" OnClientClick="openEditCustomColorsDialog(); return false;" uframeignore="true" />
    </asp:PlaceHolder>
    
    <asp:PlaceHolder ID="_donutPlace" runat="server">
        <p><ckbx:MultiLanguageLabel AssociatedControlID="_donutRadius" ID="donutRadiusLbl" runat="server" TextId="/pageText/styles/charts/graphOptions.ascx/donutRadius" /></p>
        <asp:TextBox ID="_donutRadius" runat="server" Width="45px" />
    </asp:PlaceHolder>
</asp:Panel>

 