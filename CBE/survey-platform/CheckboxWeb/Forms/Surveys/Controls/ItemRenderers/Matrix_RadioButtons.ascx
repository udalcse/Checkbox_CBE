<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Matrix_RadioButtons.ascx.cs"  Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.Matrix_RadioButtons" %>
<%@ Import Namespace="Checkbox.Common" %>
<%@ Import Namespace="Checkbox.Web.Forms.UI.Rendering" %>

<%-- Add radio buttons in code loop rather than using repeater due to issues with GroupName.  This is a known .NET issue   --%>
<asp:PlaceHolder ID="_optionsPlace" runat="server" />

<script language="C#" runat="server">
    /// <summary>
    /// Bind to model
    /// </summary>
    protected override void InlineBindModel()
    {
        base.InlineBindModel();
        
        var options = GetNonOtherOptions().ToList();
        
        for(int index = 0; index < options.Count; index++)
        {
            var option = options[index];
            
            _optionsPlace.Controls.Add(new LiteralControl(string.Format("<td align=\"center\" valign=\"middle\" class=\"{0}\" style=\"width:{1}\" spcMarker={3}><div style=\"width:{2}\">", GetCellClassName(index), GetOptionCellWidth(), GetOptionDivWidth(), Model.IsSPCArgument ? "true" : "false")));

            Literal radioButton = new Literal { ID = option.OptionId.ToString() };

            const string literalFormat = "<input type=\"radio\" name=\"{0}\" value=\"{1}\" {2} {3}></input>";
            string selected = option.IsSelected ? "checked=\"checked\"" : string.Empty;
            string isDefault = option.IsDefault ? "dataDefaultValue = \"checked\"" : string.Empty;
            

            radioButton.Text = string.Format(literalFormat, GroupName, option.OptionId, selected, isDefault);

            _optionsPlace.Controls.Add(radioButton);
            _optionsPlace.Controls.Add(new LiteralControl("</div></td>"));
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private string OptionNamePrefix
    {
        get { return "MatrixRadioButtons_" + Model.ItemId; }
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

        int? optionId = SelectedOptionId;

        UpsertOptionAnswers(optionId.HasValue 
            ? new List<int> {optionId.Value} 
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
        get { return "MatrixRadio_" + Model.ItemId.ToString(); }
    }

</script>
