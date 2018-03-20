<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MatrixNumericSlider.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.SurveyMobile.MatrixNumericSlider" %>

<style type="text/css">
    #sliderPanel_<%= ClientID %> [type="number"] { display: none; }
    #sliderPanel_<%= ClientID %> .ui-slider-track { margin: 0;}
    #sliderPanel_<%= ClientID %> .left-value-label { margin-left: 10px; }
</style>

<script type="text/javascript">
    $(document).ready(function () {
        $(document).on('change', '#numericSlider_<%= ClientID %>', function () {
            $('#sliderPanel_<%= ClientID %> .left-value-label').html($(this).val());
        });
    });
</script>

<div class="matrix-slider" id="sliderPanel_<%= ClientID %>" <% if(Model.IsSPCArgument) {%>spcmarker="true"<%} %>>
    <% if(ShowValue) { %>
    <div style="display: inline-block; width: 100%;">
        <span style="float: left;" class="left-value-label"><%= AnsweredValue %></span>
    </div>    
    <% } %>
    <div>
        <input type="range" name="numericSlider_<%= ClientID %>" id="numericSlider_<%= ClientID %>" 
            value="<%= AnsweredValue %>" min="<%= MinValue %>" max="<%= MaxValue %>" step="<%=StepSize %>"  />
    </div>

    <span class="sliderDefaultValues" dataDefaultValue="<%= DefaultValue %>" dataMaxValue="<%= MaxValue %>" style="display:none;" />
</div>        
