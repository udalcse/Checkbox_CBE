<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Other.ascx.cs" Inherits="CheckboxWeb.Styles.Charts.EditorControls.Other" %>
<%@ Import Namespace="Checkbox.Web"%>


 <script type="text/javascript">
    $(document).ready(function () {
        $('#<%=ID %>_HatchColor').mColorPicker({imageFolder:'<%=ResolveUrl("~/App_Themes/CheckboxTheme/images") %>/'});
         $('#<%=_xAngle.ClientID %>').numeric({ decimal: false, negative: true });
        $('#<%=_yAngle.ClientID %>').numeric({ decimal: false, negative: true });
        $('#<%=_perspective.ClientID %>').numeric({ decimal: false, negative: false });
        $('#<%=_opacity.ClientID %>').numeric({ decimal: false, negative: false });
    });

    //
    function <%=ID %>UpdateHatchColor(newColor){
        $('#<%=_hatchColor.ClientID %>').val(newColor);
    }
</script>

<!--
<div class="styleSectionHeader"><%= WebTextManager.GetText("/pageText/styles/charts/editor.aspx/other") %></div>
-->


<div style="display:none;">
    <asp:TextBox ID="_hatchColor" runat="server" />
</div>

<div class="formInput">
    <div class="left checkBox"><asp:CheckBox ID="_showPercent" runat="server" /></div>
    <div class="left"><p><ckbx:MultiLanguageLabel AssociatedControlID="_showPercent" ID="showPercentLbl" runat="server" TextId="/pageText/styles/charts/other.ascx/showPercent" /></p></div>
    <br class="clear" />
    
    <div class="left checkBox"><asp:CheckBox ID="_showAnswerCount" runat="server" /></div>
    <div class="left"><p><ckbx:MultiLanguageLabel AssociatedControlID="_showAnswerCount" ID="showAnswerCountLbl" runat="server" TextId="/pageText/styles/charts/other.ascx/showAnswerCount" /></p></div>
    <br class="clear" />
    
    <div class="left checkBox"><asp:CheckBox ID="_includeAllAnswers" runat="server" /></div>
    <div class="left"><p><ckbx:MultiLanguageLabel AssociatedControlID="_includeAllAnswers" ID="includeAllAnswersLbl" runat="server" TextId="/pageText/styles/charts/other.ascx/includeAllAnswers" /></p></div>
    <br class="clear" />

    <div class="left checkBox"><asp:CheckBox ID="_allowExporting" runat="server" /></div>
    <div class="left"><p><ckbx:MultiLanguageLabel AssociatedControlID="_allowExporting" ID="MultiLanguageLabel1" runat="server" TextId="/pageText/styles/charts/other.ascx/allowExporting" /></p></div>
    <br class="clear" />
    <br class="clear" />
    
    <p style="text-decoration:underline;"><ckbx:MultiLanguageLabel ID="optionsOrderLbl" AssociatedControlID="_optionsOrder" runat="server" TextId="/pageText/styles/charts/other.ascx/optionsOrder" /></p>
    <ckbx:MultiLanguageRadioButtonList ID="_optionsOrder" runat="server" Width="100%">
        <asp:ListItem Value="Survey" Text="Survey Order (Default)" TextId="/pageText/styles/charts/other.ascx/optionsOrder/survey" />
        <asp:ListItem Value="Default" Text="Alphabetic" TextId="/pageText/styles/charts/other.ascx/optionsOrder/default" />
    </ckbx:MultiLanguageRadioButtonList>
    <br class="clear" />    
    <% if (ShowHatchingSettings)
       { %>
    <div id="_hatchContainer" runat="server" >
    <p><ckbx:MultiLanguageLabel ID="hatchStyleLbl" AssociatedControlID="_hatchStyle" runat="server" TextId="/pageText/styles/charts/other.ascx/hatchStyle" /></p>
    <ckbx:MultiLanguageDropDownList ID="_hatchStyle" runat="server" />
    <br class="clear" />    
    <p><ckbx:MultiLanguageLabel ID="hatchColorLbl" AssociatedControlID="_hatchColor" runat="server" TextId="/pageText/styles/charts/other.ascx/hatchColor" /></p>
    <input name="<%=ID %>_HatchColor" id="<%=ID %>_HatchColor" type="color" hex="true" text="hidden" style="width:20px;height:20px;display:none;" value="<%=_hatchColor.Text %>" onchange="<%=ID %>UpdateHatchColor(this.value);" />
    <br class="clear" />
    </div>
    <% } %>
    <% if (Show3DSettings)
       { %>
    <div class="left"><p><ckbx:MultiLanguageLabel ID="show3dLbl" AssociatedControlID="_show3d" runat="server" TextId="/pageText/styles/charts/other.ascx/show3d" /></p></div>
    <div class="left checkBox" style="margin-left:10px;"><asp:CheckBox ID="_show3d" runat="server" /></div>
    <br class="clear" />
    
    <p><ckbx:MultiLanguageLabel ID="xAngleLbl" AssociatedControlID="_xAngle" runat="server" TextId="/pageText/styles/charts/other.ascx/xAngle" /></p>
    <asp:TextBox ID="_xAngle" runat="server" Width="40px" />
    
    <p><ckbx:MultiLanguageLabel ID="yAngleLbl" AssociatedControlID="_yAngle" runat="server" TextId="/pageText/styles/charts/other.ascx/yAngle" /></p>
    <asp:TextBox ID="_yAngle" runat="server" Width="40px" />
    
    <p><ckbx:MultiLanguageLabel ID="perspectiveLbl" AssociatedControlID="_perspective" runat="server" TextId="/pageText/styles/charts/other.ascx/perspective" /></p>
    <asp:TextBox ID="_perspective" runat="server" Width="40px" />
    <% } %>
    <p><ckbx:MultiLanguageLabel ID="opacityLbl" AssociatedControlID="_opacity" runat="server" TextId="/pageText/styles/charts/other.ascx/opacity" /></p>
    <asp:TextBox ID="_opacity" runat="server" Width="40px" />%
</div>