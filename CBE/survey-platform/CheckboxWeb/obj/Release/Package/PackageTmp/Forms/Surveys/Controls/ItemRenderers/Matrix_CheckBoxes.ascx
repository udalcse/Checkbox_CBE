<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Matrix_CheckBoxes.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.Matrix_CheckBoxes" %>
<%@ Import Namespace="Checkbox.Wcf.Services.Proxies" %>
<%@ Import Namespace="Checkbox.Web.Forms.UI.Rendering" %>

<%-- DataSource selects NON other options.  Other option is handled separately.  --%>
<asp:ObjectDataSource 
    ID="_optionsSource" 
    runat="server" 
    SelectMethod="GetNonOtherOptions" 
    TypeName="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.Matrix_CheckBoxes" 
    DataObjectTypeName="Checkbox.Wcf.Services.Proxies.SurveyResponseItemOption"  
    OnObjectCreating="OptionsSource_ObjectCreating" />

<asp:ListView 
    runat="server"
    ID="_optionRepeater"
    OnItemCreated="OptionRepeaterItemCreated"
    DataSourceID="_optionsSource" >
    
    <LayoutTemplate>
        <asp:PlaceHolder ID="itemPlaceholder" runat="server" />
    </LayoutTemplate>
    
    <ItemTemplate>
        <td align="center" valign="middle" class="<%#GetCellClassName(Container.DataItemIndex) %>" style="width:<%#GetOptionCellWidth() %>;"><div style="text-align:center;width:<%#GetOptionDivWidth() %>;" <% if (Model.IsSPCArgument) { %> spcMarker="true" <% } %>><asp:PlaceHolder runat="server" /></div></td>
    </ItemTemplate>

    <EmptyDataTemplate>
        <td align="center" valign="middle">&nbsp;</td>
    </EmptyDataTemplate>
</asp:ListView>

<script type="text/javascript">
    $(function () {
        //provide 'none of above' functionality
        $('.<%= CommonOptionsName %>').on('click', function () {
            if ($(this).prop('checked')) {
                if ($(this).attr('noneofabove') == 'true') 
                    $('.<%= CommonOptionsName %>').not('[noneofabove="true"]').prop("checked", false);
                else
                    $('[noneofabove="true"].<%= CommonOptionsName %>').prop("checked", false);
                
                $.uniform.update($('.<%= CommonOptionsName %>'));
            }
        });
    });
</script>    

<script language="C#" runat="server">
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
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OptionRepeaterItemCreated(object sender, ListViewItemEventArgs e)
    {
        if (e.Item.DataItem == null)
            return;
        
        var placeholder = e.Item.Controls.OfType<PlaceHolder>().FirstOrDefault();

        if (placeholder == null)
            return;

        var data = ((SurveyResponseItemOption) e.Item.DataItem);
        
        Literal checkbox = new Literal();

        const string literalFormat = "<input type=\"checkbox\" class=\"{0}\" name=\"{1}\" {2} {3} {4}></input>";
        string selected = data.IsSelected ? "checked=\"checked\"" : string.Empty;
        string nonOfAbove = data.IsNoneOfAbove ? "noneOfAbove=\"true\"" : string.Empty;
        string isDefault = data.IsDefault ? "dataDefaultValue = \"checked\"" : string.Empty;

        checkbox.Text = string.Format(literalFormat, CommonOptionsName, CommonOptionsName + "_" + data.OptionId, selected, nonOfAbove, isDefault);
        
        //Set item id
        checkbox.ID = data.OptionId.ToString();
                
        placeholder.Controls.Add(checkbox);
    }
    
    /// <summary>
    /// Update selected options
    /// </summary>
    protected override void InlineUpdateModel()
    {
        base.InlineUpdateModel();

        var selected = new List<int>();
        foreach (var key in Request.Form.AllKeys.Where(k => k.StartsWith(CommonOptionsName)))
        {
            var id = key.Replace(CommonOptionsName + "_", string.Empty);
            int optionId;
            if (int.TryParse(id, out optionId))
                selected.Add(optionId);
        }
        
        UpsertOptionAnswers(selected, null);
    }

    private string CommonOptionsName
    {
        get { return "MatrixCheckboxes_" + Model.ItemId; }
    }

</script>
