<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Matrix_CategorizedCheckboxes.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.Matrix_CategorizedCheckboxes" %>
<%@ Import Namespace="Checkbox.Common" %>
<%@ Import Namespace="Checkbox.Wcf.Services.Proxies" %>
<%@ Import Namespace="Checkbox.Web.Forms.UI.Rendering" %>

<%-- DataSource selects NON other options.  Other option is handled separately.  --%>
<asp:ObjectDataSource 
    ID="_optionsSource" 
    runat="server" 
    SelectMethod="GetNonOtherOptions" 
    TypeName="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.Matrix_CategorizedCheckboxes" 
    DataObjectTypeName="Checkbox.Wcf.Services.Proxies.SurveyResponseItemOption"  
    OnObjectCreating="OptionsSource_ObjectCreating" />

<asp:ListView 
    runat="server"
    ID="_optionRepeater"
    OnItemCreated="OptionRepeaterItemCreated"
    DataSourceID="_optionsSource">
    
    <LayoutTemplate>
        <asp:PlaceHolder ID="itemPlaceholder" runat="server" />
    </LayoutTemplate>
    
    <ItemTemplate>
        <td align="center" valign="middle" class="<%#GetCellClassName(Container.DataItemIndex) %>" style="width:<%#GetOptionCellWidth() %>;"><div style="text-align:center;width:<%#GetOptionDivWidth() %>;"><asp:CheckBox ID="CheckBox1" runat="server" /></div></td>
    </ItemTemplate>

    <EmptyDataTemplate>
        <td align="center" valign="middle">&nbsp;</td>
    </EmptyDataTemplate>
</asp:ListView>


<script language="C#" runat="server">

    private readonly List<CheckBox> _checkboxes = new List<CheckBox>();

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
        {
            return;
        }
        
        var checkbox = e.Item.Controls.OfType<CheckBox>().FirstOrDefault();

        if (checkbox == null)
        {
            return;
        }

        //Set item id
        checkbox.ID = ((SurveyResponseItemOption) e.Item.DataItem).OptionId.ToString();
        
        //Set checked
        checkbox.Checked = ((SurveyResponseItemOption) e.Item.DataItem).IsSelected;
        
        //Add to collection
        _checkboxes.Add(checkbox);
    }
    
    /// <summary>
    /// Update selected options
    /// </summary>
    protected override void InlineUpdateModel()
    {
        base.InlineUpdateModel();
   
        UpsertOptionAnswers(
            _checkboxes
                .Where(
                    input =>
                        input.Checked
                        && Utilities.AsInt(input.ID).HasValue)
                .Select(input => Utilities.AsInt(input.ID).Value)
                .ToList(),
            null);
    }

 

</script>
