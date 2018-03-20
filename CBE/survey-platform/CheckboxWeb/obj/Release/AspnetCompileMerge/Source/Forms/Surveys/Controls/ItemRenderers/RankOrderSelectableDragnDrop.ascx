<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="RankOrderSelectableDragnDrop.ascx.cs"
    Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.RankOrderSelectableDragnDrop" %>
<%@ Import Namespace="Checkbox.Forms.Items.Configuration" %>
<%@ Import Namespace="Checkbox.Wcf.Services.Proxies" %>

<div class="rankOrderSelectableContainer">
<div id="<%=ClientID+"_rankOrder" %>" class="ui-sortable rankOrder rankOrderSelectable rightMargin20 roundedCorners left" style="">
    <asp:Repeater ID="_rows" runat="server" OnItemDataBound="_rows_ItemDataBound" DataSource="<%#DataSourceForRepeater %>">
        <ItemTemplate>
            <asp:Panel ID="_optionPanel" runat="server" CssClass="ui-state-default rankorder-draggable rankOrderLabelPlace" optionId='<%#DataBinder.Eval(Container.DataItem, "OptionId") %>'/>
        </ItemTemplate>
    </asp:Repeater>
</div>
<div id="<%=ClientID+"_rankOrderSelected" %>" class="ui-sortable rankOrder rankOrderSelectable roundedCorners left rankOrderSelected">
    <asp:Repeater ID="_selectedRows" runat="server" OnItemDataBound="_selectedRows_ItemDataBound" DataSource="<%#DataSourceForSelected %>">
        <ItemTemplate>
            <asp:Panel ID="_optionPanel" runat="server" CssClass="ui-state-default rankorder-draggable rankOrderLabelPlace" optionId='<%#DataBinder.Eval(Container.DataItem, "OptionId") %>'/>
        </ItemTemplate>
    </asp:Repeater>
</div></div>
<div style="display: none;">
    <asp:TextBox ID="_optionsOrder" runat="server" />
</div>
<script type="text/javascript">    
    var selectedOptionCount<%=ClientID%> = <%=ShownOptionsCount%>;

    function onResize<%=ClientID%>()
    {
        var rankOrder = $('#<%=ClientID+"_rankOrder"%>');
        var selected = $('#<%=ClientID+"_rankOrderSelected"%>');

        rankOrder.css("min-height", '');
        selected.css("min-height", '');

        var h1 = rankOrder.height();
        var h2 = selected.height();
        var h = h1 > h2 ? h1 : h2;
        rankOrder.css("min-height", h);
        selected.css("min-height", h);
    }

    function onListOrderUpdate_<%=ClientID %>($item) {
        var optionsOrder = '';
        var removedOptionID = typeof($item) == 'undefined' ? 0 : (typeof($item.attr("optionId")) == 'undefined' ? 0 : $item.attr("optionId"));
        $('#<%=ClientID + "_rankOrderSelected" %> > div').each(
            function (idx, val){
                if (typeof($(val).attr("optionId")) != 'undefined' && $(val).attr("class").indexOf("dragging") < 0)
                {
                    if (removedOptionID != $(val).attr("optionId"))
                        optionsOrder += $(val).attr("optionId") + ",";
                }
            });

        if (optionsOrder.length > 0)
            optionsOrder = optionsOrder.substr(0, optionsOrder.length - 1);
        
        $("#<%=_optionsOrder.ClientID %>").val(optionsOrder);
    }

    $(document).ready(function() {
        $('#<%=ClientID+"_rankOrder" %> .rankOrderLabelPlace img, #<%=ClientID+"_rankOrderSelected" %> .rankOrderLabelPlace img').load(function () {
            onResize<%=ClientID%>();
        });

        $(window).bind("orientationchange", function(evt){
            onResize<%=ClientID%>();
        });

        $('#<%=ClientID+"_rankOrder" %> > .rankorder-draggable').draggable({
          revert: "invalid",
          containment: "document",
          helper: "clone",
          cursor: "move"
        });
        
        $('#<%=ClientID+"_rankOrderSelected" %> > .rankorder-draggable').draggable({
          revert: "invalid",
          containment: "document",
          helper: "clone",
          cursor: "move"
        });
        
        $('#<%=ClientID+"_rankOrderSelected" %>').droppable({ 
            accept: '#<%=ClientID+"_rankOrder" %> > div,#<%=ClientID+"_rankOrderSelected" %> > div', 
            tolerance: 'touch',
            drop: function( event, ui ) {
                if ($('#<%=ClientID+"_rankOrderSelected" %>').children().length >= selectedOptionCount<%=ClientID%> 
                    && ui.draggable.parent().prop('id') == '<%=ClientID+"_rankOrder"%>')
                {
                    return;
                }
                var afterWhich = null;
                var beforeWhich = null;
                $('#<%=ClientID+"_rankOrderSelected" %>').children().each(function(i, e)
                {
                    if ($(e).offset().top < ui.offset.top && $(e).attr("id") != ui.draggable.attr("id"))
                        afterWhich = $(e);
                    if (beforeWhich == null && $(e).offset().top > ui.offset.top && $(e).attr("id") != ui.draggable.attr("id"))
                        beforeWhich = $(e);
                });
                if (beforeWhich != null)
                    beforeWhich.before($(ui.draggable));
                else
                    if (afterWhich != null)
                        afterWhich.after($(ui.draggable));
                    else
                        ui.draggable.appendTo('#<%=ClientID+"_rankOrderSelected" %>');

                onListOrderUpdate_<%=ClientID %>();
            } 
        });


        $('#<%=ClientID+"_rankOrder" %>').droppable({ 
            accept: '#<%=ClientID+"_rankOrderSelected" %> > div',
            tolerance: 'touch',
            drop: function( event, ui ) {
                $(ui.draggable).appendTo('#<%=ClientID+"_rankOrder" %>');
                onListOrderUpdate_<%=ClientID %>();
            } 
        });

        onListOrderUpdate_<%=ClientID %>();
    });
</script>
<script type="text/C#" runat="server">

    /// <summary>
    /// 
    /// </summary>
    private List<SurveyResponseItemOption> DataSourceForRepeater
    {
        get 
        {
            var AnswerOptions = (from a in Model.Answers select a.OptionId).ToList();
            return Model.Options.Where(r => !AnswerOptions.Contains(r.OptionId)).OrderByDescending(p => p.Points).ToList(); 
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private List<SurveyResponseItemOption> DataSourceForSelected
    {
        get 
        {
            var AnswerOptions = (from a in Model.Answers select a.OptionId).ToList();
            return Model.Options.Where(r => AnswerOptions.Contains(r.OptionId)).OrderByDescending(p => p.Points).ToList();
        }
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
        _selectedRows.DataBind();
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

        optionPanel.Attributes["isSelected"] = "false";
        optionPanel.Attributes["num"] = GetDefaultOrder(option).ToString();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void _selectedRows_ItemDataBound(object sender, RepeaterItemEventArgs e)
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
        
        optionPanel.Attributes["isSelected"] = "true";
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
        string optionsOrder = Request[_optionsOrder.UniqueID] ?? _optionsOrder.Text;

        Dictionary<int, double> result = new Dictionary<int, double>();
        var options = optionsOrder.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < options.Length; i++)
        {
            int temp;
            if (int.TryParse(options[i], out temp))
                result.Add(temp, ShownOptionsCount - i);                
        }

        return result;
    }
    
</script>
