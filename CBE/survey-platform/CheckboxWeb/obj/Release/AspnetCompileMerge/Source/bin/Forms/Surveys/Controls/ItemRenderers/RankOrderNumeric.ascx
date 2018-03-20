<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="RankOrderNumeric.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.RankOrderNumeric" %>
<%@ Import Namespace="Checkbox.Common" %>
<%@ Import Namespace="Checkbox.Forms.Items.Configuration" %>
<%@ Import Namespace="Checkbox.Wcf.Services.Proxies" %>
<%@ Import Namespace="Checkbox.Web" %>
<table id="<%=ClientID+"_rankOrder" %>">
    <asp:Repeater ID="_rows" runat="server" OnItemDataBound="_rows_ItemDataBound" DataSource="<%#Model.Options %>">
        <ItemTemplate>
            <tr >
                <td class="rankOrderLabelPlace numeric-rank-order-label-place">
                    <asp:PlaceHolder ID="_labelPlace" runat="server" />
                </td>
                <td style="padding-left:7px;vertical-align: middle;">
                    <asp:TextBox ID="_input" runat="server" CssClass="numeric" Columns="10" />                        
                </td>
            </tr>
        </ItemTemplate>
    </asp:Repeater>
</table>

<script type="text/javascript">   
    var <%=ClientID %>_maxPoint = <%= Model.Options.Length %>;
    var <%=ClientID %>_maxRank = <%= ShownOptionsCount %>;
    var <%=ClientID %>_resultMaxPoint = Math.min(<%=ClientID %>_maxPoint, <%=ClientID %>_maxRank);
    
    $('#<%=ClientID+"_rankOrder" %> .numeric').numeric({ decimal: false, negative: false });
           
    $('#<%=ClientID+"_rankOrder" %> .numeric').on('blur', function(){    
        var thisVal = $(this).val();    

        //Check on maxValue
        if (thisVal > <%=ClientID %>_resultMaxPoint) {
            $(this).val(<%=ClientID %>_resultMaxPoint);
            thisVal = $(this).val();
        }
        var thisID = $(this).attr('id');

        var filledRanks = 0;
        //Check if this value isn't used.
        $('#<%=ClientID+"_rankOrder" %> .numeric').each(function() {
            if ($(this).val() == thisVal && $(this).attr('id') != thisID)
                $(this).val('');
            
            if ($(this).val() != '') {
                filledRanks += 1;
            }
        });

        var enableTextbox = filledRanks - <%=ClientID %>_maxRank >= 0;
        $('#<%=ClientID+"_rankOrder" %> .numeric').each(function() {
            if ($(this).val() == '') {
                $(this).prop('disabled', enableTextbox);
            }
        });
    });
</script>
<script type="text/C#" runat="server">

    private Dictionary<int, TextBox> Inputs { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="radioButtonScaleItem"></param>
    public override void Initialize(SurveyResponseItem radioButtonScaleItem)
    {
        base.Initialize(radioButtonScaleItem);

        Inputs = new Dictionary<int, TextBox>();
    }

    /// <summary>
    /// 
    /// </summary>
    public override void BindModel()
    {
        base.BindModel();

        Inputs = new Dictionary<int, TextBox>();
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
        PlaceHolder optionPanel = e.Item.FindControl("_labelPlace") as PlaceHolder;
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

        TextBox textBox = e.Item.FindControl("_input") as TextBox;
        if (textBox != null)
        {
            if (WebUtilities.IsBrowserMobile(HttpContext.Current.Request))
                textBox.Attributes["type"] = "number";
            
            if (option.IsSelected)
                textBox.Text = (ShownOptionsCount - option.Points + 1).ToString();

            Inputs.Add(option.OptionId, textBox);            
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public override Dictionary<int, double> GetOptionsWithPoints()
    {
        Dictionary<int, double> optionIdsWithPoints = new Dictionary<int, double>();

        foreach (var item in Inputs)
        {
            string optionOrder = Request[item.Value.UniqueID] ?? item.Value.Text;
            
            double temp;
            if (double.TryParse(optionOrder, out temp))
                optionIdsWithPoints.Add(item.Key, ShownOptionsCount - temp + 1);
        }

        return optionIdsWithPoints;
    }

</script>
