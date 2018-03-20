<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Matrix_Slider.ascx.cs"  Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.Matrix_Slider" %>
<%@ Register TagPrefix="slider" TagName="MatrixNumericSlider" Src="~/Forms/Surveys/Controls/ItemRenderers/MatrixNumericSlider.ascx" %>
<%@ Register TagPrefix="slider" TagName="MatrixValueListSlider" Src="~/Forms/Surveys/Controls/ItemRenderers/MatrixValueListSlider.ascx" %>

<td colspan="<%= Math.Max(Model.Options.Count() - 1, 1) %>" class="<%=GetCellClassName() %>" style="width:<%= GetSliderWidth() %>; min-width:200px;" 
>
<slider:MatrixNumericSlider ID="_numericSlider" runat="server" Visible="false" />
<slider:MatrixValueListSlider ID="_valueListSlider" runat="server" Visible="false" />
</td>
