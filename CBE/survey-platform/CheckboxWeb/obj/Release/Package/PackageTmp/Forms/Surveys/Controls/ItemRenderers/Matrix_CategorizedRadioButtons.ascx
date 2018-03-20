<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Matrix_CategorizedRadioButtons.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.Matrix_CategorizedRadioButtons" %>
<%@ Import Namespace="Checkbox.Common" %>
<%@ Import Namespace="Checkbox.Web.Forms.UI.Rendering" %>

<%-- Add radio buttons in code loop rather than using repeater due to issues with GroupName.  This is a known .NET issue   --%>
<asp:PlaceHolder ID="_optionsPlace" runat="server" />


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
        var selectedOption = GetNonOtherOptions().FirstOrDefault(opt => opt.IsSelected);
        var options = GetNonOtherOptions().ToList();
        
        for(int index = 0; index < options.Count; index++)
        {
            var option = options[index];

            _optionsPlace.Controls.Add(new LiteralControl(string.Format("<td align=\"center\" valign=\"middle\" class=\"{0}\" style=\"width:{1}\"><div style=\"width:{2}\">", GetCellClassName(index), GetOptionCellWidth(), GetOptionDivWidth())));

            var radioButton = new RadioButton 
            {
                ID = option.OptionId.ToString(), 
                GroupName = Model.ItemId.ToString(), 
                Checked = selectedOption != null && option.OptionId == selectedOption.OptionId
            };
            
            _radioButtons.Add(radioButton);

            _optionsPlace.Controls.Add(radioButton);
            _optionsPlace.Controls.Add(new LiteralControl("</div></td>"));
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

        //Only add right border to last option.
        if ("Vertical".Equals(parent.GridLineMode, StringComparison.InvariantCultureIgnoreCase)
            && itemIndex == GetAllOptions().Count() - 1)
            return "BorderRight";

        //Add top border to all
        if ("Horizontal".Equals(parent.GridLineMode, StringComparison.InvariantCultureIgnoreCase))
            return "BorderTop";

        //If applying "both".  Use top border for all but last option
        // and "both" for last.
        if ("Both".Equals(parent.GridLineMode, StringComparison.InvariantCultureIgnoreCase)
            && itemIndex < GetAllOptions().Count() - 1)
        {
            return "BorderTop";
        }

        return ("Both".Equals(parent.GridLineMode, StringComparison.InvariantCultureIgnoreCase)
                && itemIndex == GetAllOptions().Count() - 1)
                   ? "BorderBoth"
                   : String.Empty;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private string GetOptionCellWidth()
    {
        if (Width.HasValue)
        {
            if (Width.Value.Type == UnitType.Pixel)
            {
                return Math.Truncate(Width.Value.Value / GetNonOtherOptions().Count()) + "px";
            }

            return Width.ToString();
        }

        return Math.Truncate(100.00 / GetNonOtherOptions().Count()) + "%";
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private string GetOptionDivWidth()
    {
        if (Width.HasValue)
        {
            if (Width.Value.Type == UnitType.Pixel)
            {
                return Math.Truncate(Width.Value.Value / GetNonOtherOptions().Count()) + "px";
            }
        }

        return string.Empty;
    }
    
    /// <summary>
    /// Update selected options
    /// </summary>
    protected override void InlineUpdateModel()
    {
        base.InlineUpdateModel();

        UpsertOptionAnswers(
            _radioButtons
                .Where(
                    input =>
                        input.Checked
                        && Utilities.AsInt(input.ID).HasValue)
                .Select(input => Utilities.AsInt(input.ID).Value)
                .ToList(),
            null);
    }

</script>
