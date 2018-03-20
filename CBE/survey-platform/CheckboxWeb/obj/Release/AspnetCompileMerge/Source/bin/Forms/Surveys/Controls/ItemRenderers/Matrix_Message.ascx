<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Matrix_Message.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.Matrix_Message" %>
<%@ Import Namespace="Checkbox.Common" %>
<%@ Import Namespace="Checkbox.Forms.Items" %>
<%@ Import Namespace="Checkbox.Web.Forms.UI.Rendering" %>

<td align="left" valign="middle" class="<%=GetCellClassName() %>">
    <asp:Literal ID="_messageLiteral" runat="server" />
</td>


<script language="C#" runat="server">

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public string GetCellClassName()
    {
        var parent = Parent as MatrixChildrensItemRenderer;

        if (parent == null)
        {
            return string.Empty;
        }
        
        string borderClass;
        if ("Vertical".Equals(parent.GridLineMode, StringComparison.InvariantCultureIgnoreCase))
            borderClass = "BorderRight";
        else
        if ("Horizontal".Equals(parent.GridLineMode, StringComparison.InvariantCultureIgnoreCase))
            borderClass = "BorderTop";
        else
            borderClass = "Both".Equals(parent.GridLineMode, StringComparison.InvariantCultureIgnoreCase)
                ? "BorderBoth"
                : String.Empty;

        string alignClass = parent.ChildType == MatrixChildType.RowText && Utilities.IsNotNullOrEmpty(parent.RowTextPosition)
                                 ? "rowTextPosition" + parent.RowTextPosition
                                 : String.Empty;
        var separator = Utilities.IsNotNullOrEmpty(borderClass) && Utilities.IsNotNullOrEmpty(alignClass)
                            ? " "
                            : String.Empty;
        return borderClass + separator + alignClass;
    }
    
</script>