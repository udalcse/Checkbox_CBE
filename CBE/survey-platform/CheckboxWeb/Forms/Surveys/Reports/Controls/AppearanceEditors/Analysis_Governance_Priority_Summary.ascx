<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Analysis_Governance_Priority_Summary.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.Controls.AppearanceEditors.Analysis_Governance_Priority_Summary" %>

<script type="text/javascript">
        $(document).ready(function () {
            $('#<%=ID %>_barColor').mColorPicker({ imageFolder: '<%=ResolveUrl("~/App_Themes/CheckboxTheme/images") %>/' });
         
        });

        //
        function <%=ID %>UpdateTextColor(newColor){
            $('#<%=_barColor.ClientID %>').val(newColor);
        }

      
    </script>

<div style="display:none;">
    <asp:TextBox ID="_barColor" runat="server" />
   
</div>

<div class="formInput">
    
    <div class="left"><p><ckbx:MultiLanguageLabel ID="_barTextColorLbl" runat="server" AssociatedControlID="_barColor" Text="Bar color" /></p></div>
    <div class="left" style="margin-left:10px;margin-top:5px;"><input name="<%=ID %>_barColor" id="<%=ID %>_barColor" type="color" hex="true" text="hidden" style="width:20px;height:20px;display:none;" value="<%=_barColor.Text %>" onchange="<%=ID %>UpdateTextColor(this.value);" /></div>
    <br class="clear" />

     <p><ckbx:MultiLanguageLabel ID="titleSizeLbl" AssociatedControlID="_titleSize" runat="server" Text="Label size" /></p>
    <ckbx:MultiLanguageDropDownList ID="_titleSize" runat="server" />
    
     <p><ckbx:MultiLanguageLabel AssociatedControlID="_font" ID="fontLbl" runat="server" Text="Label font"/></p>
    <ckbx:MultiLanguageDropDownList ID="_font" runat="server" />
    
    <p><ckbx:MultiLanguageLabel AssociatedControlID="_gridLine" ID="_gridLineLabel" runat="server" Text="Grid line" />
    <asp:CheckBox ID="_gridLine" runat="server" CssClass="leftMargin10" />
        </p>
      <p><ckbx:MultiLanguageLabel AssociatedControlID="_showValuesOnBars" ID="_showValuesOnBarsLbl" runat="server" Text="Show values on bars" />
    <asp:CheckBox ID="_showValuesOnBars" runat="server" CssClass="leftMargin10" />
        </p>
</div>
