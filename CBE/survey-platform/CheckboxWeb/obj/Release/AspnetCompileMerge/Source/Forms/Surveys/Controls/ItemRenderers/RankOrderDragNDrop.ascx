<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="RankOrderDragNDrop.ascx.cs"
    Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.RankOrderDragNDrop" %>
<%@ Import Namespace="Checkbox.Common" %>
<%@ Import Namespace="Checkbox.Forms.Items.Configuration" %>
<%@ Import Namespace="Checkbox.Wcf.Services.Proxies" %>
<div id="<%=ClientID+"_rankOrder" %>" class="ui-sortable rankOrder rank-order-drag-n-drop">
    <asp:Repeater ID="_rows" runat="server" OnItemDataBound="_rows_ItemDataBound" DataSource="<%#DataSourceForRepeater %>">
        <ItemTemplate>
            <asp:Panel ID="_optionPanel" runat="server" CssClass="ui-state-default rankOrderLabelPlace drag-n-drop" optionId='<%#DataBinder.Eval(Container.DataItem, "OptionId") %>' />
        </ItemTemplate>
    </asp:Repeater>
</div>
<div style="display: none;">
    <asp:TextBox ID="_optionsOrder" runat="server" />
</div>
<script type="text/javascript">
    $('#<%=ClientID+"_rankOrder" %>').sortable({ update: onListOrderUpdate_<%=ClientID %> });

    function onListOrderUpdate_<%=ClientID %>() {
        var rowIds = $('#<%=ClientID + "_rankOrder" %>').sortable('toArray');
        var optionsOrder = '';
        for(var i=0; i < rowIds.length; i++){
            optionsOrder += $("#"+rowIds[i]).attr('optionId');
            if (i < (rowIds.length - 1))
                optionsOrder += ',';
        }

        $("#<%=_optionsOrder.ClientID %>").val(optionsOrder);
    }
</script>
<script type="text/C#" runat="server">

    /// <summary>
    /// 
    /// </summary>
    private List<SurveyResponseItemOption> DataSourceForRepeater
    {
        get { return Model.Options.OrderByDescending(p => p.Points).ToList(); }
    }

    /// <summary>
    /// 
    /// </summary>
    public override void BindModel()
    {
        base.BindModel();

        String optionsOrder = String.Empty;

        foreach (var itemOption in DataSourceForRepeater)
        {
            optionsOrder += itemOption.OptionId + ",";
        }

        //Remove the last comma.
        if (optionsOrder.Length > 0)
            optionsOrder = optionsOrder.Substring(0, optionsOrder.Length - 1);

        _optionsOrder.Text = optionsOrder;

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
        Panel optionPanel = e.Item.FindControl("_optionPanel") as Panel;
        SurveyResponseItemOption option = e.Item.DataItem as SurveyResponseItemOption;

        HtmlGenericControl label = new HtmlGenericControl("div");
        label.Attributes.Add("class", "rankOrderQuestion Answer");
        label.InnerHtml = GetOptionText(option);

        if (OptionType == RankOrderOptionType.Image)
        {
            if (OptionLabelOrientation.ToLower() == "top")
                optionPanel.Controls.Add(label);

            optionPanel.Controls.Add(new Image
            {
                ImageUrl = ResolveUrl("~/ViewContent.aspx") + "?ContentId=" + option.ContentId,
                CssClass = "Answer rankOrderQuestion"
            });

            if (OptionLabelOrientation.ToLower() == "bottom")
                optionPanel.Controls.Add(label);
        }
        else
            optionPanel.Controls.Add(label);

        optionPanel.Attributes["num"] = GetDefaultOrder(option).ToString();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="o"></param>
    /// <returns></returns>
    protected int GetDefaultOrder(SurveyResponseItemOption o)
    {
        if (Model == null || Model.Options == null || o == null)
            return 0;
        for (int i = 0; i < Model.Options.Length; i++)
        {
            if (Model.Options[i].OptionId == o.OptionId)
                return i;
        }
        return 0;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override Dictionary<int, double> GetOptionsWithPoints()
    {
        string optionOrder = Request[_optionsOrder.UniqueID] ?? _optionsOrder.Text;
        
        Dictionary<int, double> result = new Dictionary<int, double>();
        String[] options = optionOrder.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < options.Length; i++)
        {
            int temp;
            if (int.TryParse(options[i], out temp))
                result.Add(temp, ShownOptionsCount - i);                
        }

        return result;
    }
    
</script>
