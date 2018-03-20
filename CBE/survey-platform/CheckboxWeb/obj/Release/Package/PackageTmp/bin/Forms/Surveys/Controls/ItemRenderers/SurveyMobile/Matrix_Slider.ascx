<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Matrix_Slider.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.SurveyMobile.Matrix_Slider" %>
<%@ Register TagPrefix="slider" TagName="MatrixNumericSlider" Src="~/Forms/Surveys/Controls/ItemRenderers/SurveyMobile/MatrixNumericSlider.ascx" %>
<%@ Register TagPrefix="slider" TagName="MatrixValueListSlider" Src="~/Forms/Surveys/Controls/ItemRenderers/SurveyMobile/MatrixValueListSlider.ascx" %>

<td colspan="<%= Math.Max(Model.Options.Count() - 1, 1) %>" class="<%=GetCellClassName() %>" style="width:<%= GetSliderWidth() %>; min-width:200px;" 
>
<slider:MatrixNumericSlider ID="_numericSlider" runat="server" Visible="false" />
<slider:MatrixValueListSlider ID="_valueListSlider" runat="server" Visible="false" />
</td>
