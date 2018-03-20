<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="HorizontalLine.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.HorizontalLine" %>
<%@ Import Namespace="Checkbox.Common"%>

<hr class="horizontalLineControl" style="border-color:<%=GetColorAsString()%> !important; background:<%=GetColorAsString()%>;width:<%=GetWidthAsString()%>;height:<%=GetThicknessAsString() %>;" size="<%=GetThicknessAsString().Replace("px", string.Empty) %>" />

<script type="text/C#" runat="server">
    /// <summary>
    /// Get rule color in string format
    /// </summary>
    /// <returns></returns>
    protected string GetColorAsString()
    {
        return Utilities.IsNotNullOrEmpty(Model.Metadata["Color"])
            ? Model.Metadata["Color"] 
            : "#000000";
    }

    /// <summary>
    /// Get item width as string
    /// </summary>
    /// <returns></returns>
    protected string GetWidthAsString()
    {
        return Utilities.IsNotNullOrEmpty(Model.Metadata["Width"])
            ? Model.Metadata["Width"]
            : "100%";
    }

    /// <summary>
    /// Get thickness of item
    /// </summary>
    /// <returns></returns>
    protected string GetThicknessAsString()
    {
        return Utilities.IsNotNullOrEmpty(Model.Metadata["Thickness"])
            ? Model.Metadata["Thickness"] + "px"
            : "1px";
    }
    
</script>
