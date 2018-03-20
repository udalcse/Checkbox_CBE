<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Matrix_DropdownList.ascx.cs"    Inherits="CheckboxWeb.Forms.Surveys.Controls.Matrix_DropdownList" %>
<%@ Import Namespace="Checkbox.Common" %>
<%@ Import Namespace="Checkbox.Wcf.Services.Proxies" %>
<%@ Import Namespace="Checkbox.Web.Forms.UI.Rendering" %>

<asp:ObjectDataSource 
    ID="_optionsSource" 
    runat="server" 
    SelectMethod="GetOptions" 
    TypeName="CheckboxWeb.Forms.Surveys.Controls.Matrix_DropdownList" 
    DataObjectTypeName="Checkbox.Forms.Items.ListOption"  
    OnObjectCreating="OptionsSource_ObjectCreating" />

    <td align="center" valign="middle" class="<%=GetCellClassName() %>">
        <div style="text-align:center;overflow:hidden;<%= HasParentWidth ? string.Format("width:{0}px", ParentWidth) : "" %>">
            <asp:DropDownList 
                        ID="_dropdownList" 
                        runat="server" 
                        DataSourceID="_optionsSource" 
                        DataTextField="Text" 
                        DataValueField="OptionId"
                        CssClass="Answer"
                        ondatabound="DropDownList_DataBound"
                    />
        </div>
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

        if ("Vertical".Equals(parent.GridLineMode, StringComparison.InvariantCultureIgnoreCase))
            return "BorderRight";

        if ("Horizontal".Equals(parent.GridLineMode, StringComparison.InvariantCultureIgnoreCase))
            return "BorderTop";

        return "Both".Equals(parent.GridLineMode, StringComparison.InvariantCultureIgnoreCase)
            ? "BorderBoth"
            : String.Empty;
    }

    /// <summary>
    /// 
    /// </summary>
    public bool HasParentWidth
    {
        get
        {
            return true;
        }        
    }

    /// <summary>
    /// 
    /// </summary>
    public int ParentWidth
    {
        get
        {
            var parent = Parent as MatrixChildrensItemRenderer;

            return parent != null && parent.ColumnWidth.HasValue ? parent.ColumnWidth.Value : 150;
        }
    }

    /// <summary>
    /// Drop down list data bound
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void DropDownList_DataBound(object sender, EventArgs e)
    {
        if (HasParentWidth)
        {
            _dropdownList.Attributes["uframeWidth"] = ParentWidth.ToString();
        }
        
        //Get default option
        SurveyResponseItemOption defaultOption = Model.Options.FirstOrDefault(o => o.IsDefault);

        if (defaultOption != null)
        {
            ListItem listItem = _dropdownList.Items.FindByValue(defaultOption.OptionId.ToString());
            listItem.Attributes["dataDefaultValue"] = "selected";
        }

        //Get selected option
        SurveyResponseItemOption selectedOption = Model.Options.FirstOrDefault(opt => opt.IsSelected);

        //Do nothing if no options selected
        if (selectedOption == null)
        {
            return;
        }

        if (_dropdownList.Items.FindByValue(selectedOption.OptionId.ToString()) != null)
        {
            _dropdownList.SelectedValue = selectedOption.OptionId.ToString();
        }
    }

    /// <summary>
    /// Update model
    /// </summary>
    protected override void InlineUpdateModel()
    {
        base.InlineUpdateModel();

        var selectedOptionIds = new List<int>();
        int? selectedOptionId = Utilities.AsInt(Request[_dropdownList.UniqueID]);

        string otherText = string.Empty;

        if (selectedOptionId.HasValue)
            selectedOptionIds.Add(selectedOptionId.Value);

        UpsertOptionAnswers(
            selectedOptionIds,
            otherText);
    }

</script>
