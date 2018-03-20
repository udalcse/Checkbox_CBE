<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Matrix_RadioButtonScale.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.Matrix_RadioButtonScale" %>
<%@ Import Namespace="Checkbox.Common" %>
<%@ Import Namespace="Checkbox.Web.Forms.UI.Rendering" %>


<%-- Add radio buttons in code loop rather than using repeater due to issues with GroupName.  This is a known .NET issue   --%>

<asp:PlaceHolder ID="_optionsPlace" runat="server">
</asp:PlaceHolder>

<script language="C#" runat="server">
    private List<RadioButton> _radioButtons;

    /// <summary>
    /// Bind to model
    /// </summary>
    protected override void InlineBindModel()
    {
        base.InlineBindModel();

        _radioButtons = new List<RadioButton>();

        //Set selected
        var selectedOption = GetAllOptions().FirstOrDefault(opt => opt.IsSelected);

        var options = GetAllOptions().ToList();

        for (int index = 0; index < options.Count; index++)
        {
            var option = options[index];
            _optionsPlace.Controls.Add(new LiteralControl(string.Format("<td align=\"center\" valign=\"middle\" class=\"{0}\" style=\"width:{1}\">", GetCellClassName(index), GetOptionWidth())));

            var radioButton = new RadioButton
            {
                ID = option.OptionId.ToString(),
                GroupName = GroupName,
                Checked = selectedOption != null && option.OptionId == selectedOption.OptionId,
                CssClass = "no-mobile-radio"
            };

            _radioButtons.Add(radioButton);

            var label = new Label
            {
                ID = option.OptionId.ToString()+"_lbl",
                AssociatedControlID = option.OptionId.ToString(),
                CssClass = "no-mobile-label"
            };

            _radioButtons.Add(radioButton);

            _optionsPlace.Controls.Add(radioButton);
            _optionsPlace.Controls.Add(label);
            _optionsPlace.Controls.Add(new LiteralControl("</td>"));
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public string GetCellClassName(int itemIndex)
    {
        var parent = Parent as MatrixChildrensItemRenderer;

        if (parent == null)
        {
            return string.Empty;
        }

        var allOptions = GetAllOptions().ToList();
        
        //If NA option enabled, and using vertical or both, add right border to second to last option
        bool containsNa = allOptions.FirstOrDefault(opt => opt.IsOther) != null;

        //Only add right border to last option non-na option and na option
        if ("Vertical".Equals(parent.GridLineMode, StringComparison.InvariantCultureIgnoreCase)
            && ((itemIndex == allOptions.Count - 1) || (containsNa && itemIndex == allOptions.Count - 2)))
            return "BorderRight";

        //Add top border to all
        if ("Horizontal".Equals(parent.GridLineMode, StringComparison.InvariantCultureIgnoreCase))
            return "BorderTop";

        //If applying "both".  Use top border for all but last option and last non-na option
        // and "both" for last.
        if ("Both".Equals(parent.GridLineMode, StringComparison.InvariantCultureIgnoreCase)
            && ((itemIndex  == allOptions.Count - 1) || (containsNa && itemIndex == allOptions.Count - 2)))
        {
            return "BorderBoth";
        }

        return ("Both".Equals(parent.GridLineMode, StringComparison.InvariantCultureIgnoreCase)
                && itemIndex < allOptions.Count - 1)
                   ? "BorderTop"
                   : String.Empty;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private string GetOptionWidth()
    {
        if (Width.HasValue)
        {
            if (Width.Value.Type == UnitType.Pixel)
            {
                return Math.Truncate(Width.Value.Value / GetAllOptions().Count()) + "px";   
            }

            return Width.ToString();
        }

        return Math.Truncate(100.00 / GetAllOptions().Count()) + "%";
    }

    /// <summary>
    /// Update selected options
    /// </summary>
    protected override void InlineUpdateModel()
    {
        base.InlineUpdateModel();

        int? optionId = SelectedOptionId;
        
        UpsertOptionAnswers(optionId.HasValue
            ? new List<int> { optionId.Value }
            : new List<int>(), null);
    }

    private int? SelectedOptionId
    {
        get
        {
            var key = Request.Form.AllKeys.FirstOrDefault(k => k.EndsWith(GroupName));
            return key != null ? Utilities.AsInt(Request[key]) : null;
        }
    }

    private string GroupName
    {
        get { return "MatrixRadioScale_" + Model.ItemId.ToString(); }
    }

</script>
