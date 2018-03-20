<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="MatrixValueListSlider.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.MatrixValueListSlider" %>
<%@ Import Namespace="Checkbox.Management" %>

<script type="text/javascript">
    $(document).ready(function () {
        var options = {
            labels : <%=Model.Options.Length %>,            
            labelSrc: 'text',
            tooltip: false,
            showImages: false,
            applicationRoot: '<%=ApplicationManager.ApplicationPath %>',
            sliderOptions: {orientation: 'horizontal'}
        };

        $("#<%=_options.ClientID %>").selectToUISlider(options);

        $("a.ui-slider-handle.ui-state-default.ui-corner-all").attr('uframeIgnore','true');
    });
</script>

<div class="matrix-slider" >
    <div style="position:relative; min-height: 30px; "> 
        <asp:DropDownList ID="_options" runat="server" uniformIgnore="true" style="display:none;"/>
    </div>
    
    <div dataDefaultId="<%= DefaultOptionId %>" dataDefaultValue="<%= DefaultOptionIndex %>" dataMaxValue="<%= Model.Options.Count() - 1 %>" 
        class="clear sliderDefaultValues"></div>
</div>
