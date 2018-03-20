<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Matrix_RowSelector.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.Matrix_RowSelector" %>
<%@ Import Namespace="Checkbox.Common" %>
<%@ Import Namespace="CheckboxWeb.Controls.ControlsForRepeater" %>
<%@ Import Namespace="Checkbox.Web.Forms.UI.Rendering" %>


<script language="C#" runat="server">
    private bool? _allowMultipleSelection;

    /// <summary>
    /// Determine if multiple selection is allow or not.
    /// </summary>
    private bool AllowMultipleSelection
    {
        get
        {
            if (!_allowMultipleSelection.HasValue)
                _allowMultipleSelection = bool.Parse(Model.InstanceData["allowMultipleSelection"]);
            return _allowMultipleSelection.Value;
        }
    }

    //List of inputs
    protected List<CheckBox> InputList;

    /// <summary>
    /// Bind control with the model
    /// </summary>
    protected override void InlineBindModel()
    {  //Clear controls
        Controls.Clear();
        InputList = new List<CheckBox>();

        base.InlineBindModel();

        //Add empty cell if no options
        if (Model.Options.Length == 0)
        {
            Controls.Add(new TableCell { HorizontalAlign = HorizontalAlign.Center, VerticalAlign = VerticalAlign.Middle});
            return;
        }

        foreach (var itemOption in Model.Options)
        {
            CheckBox button;

            if (AllowMultipleSelection)
            {
                button = new CheckBox
                             {
                                 Checked = itemOption.IsSelected,
                                 ID = itemOption.OptionId.ToString()
                             };
            }
            else
            {
                button = new RadioButtonForRepeater
                             {
                                 Checked = itemOption.IsSelected,
                                 GroupName = Model.ParentTemplateId.ToString(),
                                 ID = itemOption.OptionId.ToString()
                             };
            }
            
            var cell = new TableCell
            {
                HorizontalAlign = HorizontalAlign.Center,
                VerticalAlign = VerticalAlign.Middle,
                CssClass = GetCellClassName()
            };

            cell.Controls.Add(button);
            Controls.Add(cell);

            InputList.Add(button);
        }
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

        //Only add right border to last option.
        if ("Vertical".Equals(parent.GridLineMode, StringComparison.InvariantCultureIgnoreCase)) {
            return "BorderRight";
        }

        //Add top border to all
        if ("Horizontal".Equals(parent.GridLineMode, StringComparison.InvariantCultureIgnoreCase))
        {
            return "BorderTop";
        }

        return "Both".Equals(parent.GridLineMode, StringComparison.InvariantCultureIgnoreCase)
                   ? "BorderBoth"
                   : String.Empty;
    }


    /// <summary>
    /// Update selected options
    /// </summary>
    protected override void InlineUpdateModel()
    {
        base.InlineUpdateModel();

        if (InputList == null)
        {
            InputList = new List<CheckBox>();
        }

        UpsertOptionAnswers(
            InputList
                .Where(
                    input =>
                        input.Checked
                        && Utilities.AsInt(input.ID).HasValue)
                .Select(input => Utilities.AsInt(input.ID).Value)
                .ToList(),
            null);
    }

</script>
