<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Matrix_MatrixSumTotal.ascx.cs"    Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.Matrix_MatrixSumTotal" %>
<%@ Import Namespace="Checkbox.Common" %>
<%@ Import Namespace="Checkbox.Web.Forms.UI.Rendering" %>

<td align="center" valign="middle" class="<%=GetCellClassName() %>"><asp:TextBox ID="_textBox" TextMode="SingleLine" CssClass="Answer" runat="server" /></td>

<script language="C#" runat="server">
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public string GetCellWidthStyle()
    {
        var parent = Parent as MatrixChildrensItemRenderer;

        //Otherwise use column width
        if (parent == null || !parent.ColumnWidth.HasValue)
        {
            return string.Empty;
        }

        return "width: " + parent.ColumnWidth + "px;";
    }
    
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

        if ("Vertical".Equals(parent.GridLineMode, StringComparison.InvariantCultureIgnoreCase))
            return "BorderRight";

        if ("Horizontal".Equals(parent.GridLineMode, StringComparison.InvariantCultureIgnoreCase))
            return "BorderTop";

        return "Both".Equals(parent.GridLineMode, StringComparison.InvariantCultureIgnoreCase)
            ? "BorderBoth"
            : String.Empty;
    }

    /// <summary>
    /// Bind control with the model
    /// </summary>
    protected override void InlineBindModel()
    {
        base.InlineBindModel();

        _textBox.Text = Model.Answers.Length > 0
                            ? Model.Answers[0].AnswerText
                            : (Model.Metadata["DefaultText"] ?? String.Empty);

        bool widthSet = false;

        //Check for explicit width
        if (Appearance != null)
        {
            var width = Utilities.AsInt(Appearance["Width"]);

            if (width.HasValue)
            {
                _textBox.Width = Unit.Pixel(width.Value);
                widthSet = true;
            }
        }

        //set the default text
        _textBox.Attributes["dataDefaultValue"] = Model.Metadata["DefaultText"] ?? String.Empty;

        var parent = Parent as MatrixChildrensItemRenderer;

        //Otherwise use column width
        if (widthSet || parent == null || !parent.ColumnWidth.HasValue || parent.ColumnWidth.Value < 20)
        {
            return;
        }

        _textBox.Width = Unit.Pixel(parent.ColumnWidth.Value - 10);
    }


    /// <summary>
    /// Update answered text
    /// </summary>
    protected override void InlineUpdateModel()
    {
        _textBox.Text = Request[_textBox.UniqueID] ?? string.Empty;

        UpsertTextAnswer(
            _textBox != null
                ? _textBox.Text.Trim()
                : string.Empty
        );
    }

</script>
