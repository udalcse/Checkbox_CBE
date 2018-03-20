<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="RankOrderTopN.ascx.cs"
    Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.RankOrderTopN" %>
<%@ Import Namespace="Checkbox.Common" %>
<%@ Import Namespace="Checkbox.Wcf.Services.Proxies" %>
<%@ Import Namespace="Checkbox.Web" %>
<table id="<%=ClientID %>_rankOrder">
    <asp:Repeater ID="_rows" runat="server" OnItemDataBound="_rows_ItemDataBound" DataSource="<%#new byte[ShownOptionsCount] %>">
        <ItemTemplate>
            <tr>
                <td>
                    <asp:Label ID="_label" runat="server" class="Answer"/>
                    &nbsp
                </td>
                <td>
                    <asp:DropDownList ID="_dropDown" runat="server" CssClass="rankOrderDropDown" uframeignore="true"/>
                </td>
            </tr>
        </ItemTemplate>
    </asp:Repeater>
</table>
<script type="text/javascript">
    $('#<%=ClientID+"_rankOrder" %> .rankOrderDropDown').change(function () {
        var thisVal = $(this).val();
        var thisID = $(this).attr('id');

        //Check if this value isn't used.
        $('#<%=ClientID+"_rankOrder" %> .rankOrderDropDown').each(function () {
            if ($(this).val() == thisVal && $(this).attr('id') != thisID) {
                $(this).val('-1');
                if (typeof ($.uniform) != 'undefined') {
                    $.uniform.update('#' + $(this).attr('id'));
                }
                if (typeof ($.mobile) != 'undefined') {
                    $(this).selectmenu("refresh");
                }
            }
        });

    });
</script>
<script type="text/C#" runat="server">

    private Dictionary<int, DropDownList> Inputs { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="radioButtonScaleItem"></param>
    public override void Initialize(SurveyResponseItem radioButtonScaleItem)
    {
        base.Initialize(radioButtonScaleItem);

        Inputs = new Dictionary<int, DropDownList>();
    }

    /// <summary>
    /// 
    /// </summary>
    public override void BindModel()
    {
        base.BindModel();

        Inputs = new Dictionary<int, DropDownList>();
        _rows.DataBind();
    }

    /// <summary>
    /// Override this method to do nothing. Data binding is handled in BindModel() method.
    /// </summary>
    public override void DataBind()
    {
        //do nothing
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void _rows_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        Label label = e.Item.FindControl("_label") as Label;

        int position = e.Item.ItemIndex + 1;
        
        label.Text = position + ") ";

        DropDownList dropDown = e.Item.FindControl("_dropDown") as DropDownList;

        FillDropDown(position, dropDown);

        Inputs[position] = dropDown;
    }

    /// <summary>
    /// Fill the specified dropDown
    /// </summary>
    /// <param name="position"></param>
    /// <param name="control"></param>
    protected void FillDropDown(int position, DropDownList control)
    {
        control.Items.Clear();

        foreach (var option in Model.Options)
        {
            int positionOfTheOption = ShownOptionsCount -(int)Math.Round(option.Points) + 1;
            var li = new ListItem(Utilities.DecodeAndStripHtml(option.Text), option.OptionId.ToString()) { Selected = position == positionOfTheOption };
            control.Items.Add(li);
        }

        var defaultItem = new ListItem(WebTextManager.GetText("/common/dropDownDefault"), "-1");

        if (string.IsNullOrEmpty(defaultItem.Text))
            defaultItem.Text = " ";

        control.Items.Insert(0, defaultItem);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override Dictionary<int, double> GetOptionsWithPoints()
    {
        Dictionary<int, double> result = new Dictionary<int, double>();

        foreach (var input in Inputs)
        {
            string selectedValue = Request[input.Value.UniqueID] ?? input.Value.SelectedValue;
            
            int position = input.Key;
            int optionId;
            if (int.TryParse(selectedValue, out optionId) && optionId != -1)
                result.Add(optionId, ShownOptionsCount - position + 1);
        }

        return result;
    }

</script>
