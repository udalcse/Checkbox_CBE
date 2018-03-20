<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MatrixValueListSlider.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.SurveyMobile.MatrixValueListSlider" %>

<style type="text/css">
    #sliderPanel_<%= ClientID %> [type="number"] { display: none; }
    #sliderPanel_<%= ClientID %> .ui-slider-track { margin-left: 45px;}
    #sliderPanel_<%= ClientID %> .left-value-label { margin-left: 10px; }
</style>

<div class="matrix-slider" id="sliderPanel_<%= ClientID %>" <%= Model.IsSPCArgument ? "spcMarker='true'" : "" %>>
    <input type="range" name="numericSlider_<%= ClientID %>" id="numericSlider_<%= ClientID %>" 
        value="<%= AnsweredIndex %>" min="0" max="<%= Model.Options.Count() - 1 %>" step="1"  />

    <div style="display: none;" dataDefaultId="<%= DefaultOptionId %>" dataDefaultValue="<%= DefaultOptionIndex %>" 
        dataMaxValue="<%= Model.Options.Count() - 1 %>" class="clear sliderDefaultValues"></div>
</div>