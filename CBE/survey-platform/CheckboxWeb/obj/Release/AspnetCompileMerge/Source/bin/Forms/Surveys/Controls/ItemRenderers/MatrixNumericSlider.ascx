<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="MatrixNumericSlider.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.MatrixNumericSlider" %>


<script type="text/javascript">
    $(document).ready(function () {
        $("#<%=sliderPanel.ClientID %>").slider({
            min: <%=MinValue %>,
            max: <%=MaxValue %>,
            step: <%=StepSize %>,
            tooltip: false,
            value: <%=_currentValue.Text %>,
            orientation:'horizontal',
            slide: function(event, ui){
                $("#<%=_currentValue.ClientID %>").val(ui.value);
                <%if(ShowValue) {%>
                    $("#selectedValue_<%=Model.ItemId %>").val(ui.value);
                <% }%>
              },
            change:function(event, ui){       
                $("#<%=_currentValue.ClientID %>").change();
            }
        });

        $("a.ui-slider-handle.ui-state-default.ui-corner-all").attr('uframeIgnore','true');
    });
</script>
<div class="matrix-slider" <% if("true".Equals(Attributes["spcMarker"], StringComparison.InvariantCultureIgnoreCase)) {%>spcmarker="true"<%} %>>
    <div style="position:relative; <%= (ShowValue ? "min-height: 65px;padding-top:5px;" : "min-height: 30px;") %>"> 
        <div>        
            <% if(ShowValue) {%>
                <input id="selectedValue_<%=Model.ItemId %>"  readonly="true" class="sliderSelectedValue" value="<%=AnsweredValue%>" />
            <% } %>

            <div style="width:<%=Width%>" class="right">
                <asp:Panel ID="sliderPanel" runat="server" Width="100%"/>
            </div>
        </div>         
    </div>

    <div style="display:none;">
        <asp:TextBox ID="_currentValue" runat="server" />
        <div class="sliderDefaultValues" dataDefaultValue="<%= DefaultValue %>" dataMaxValue="<%= MaxValue %>" />
    </div>
</div>
