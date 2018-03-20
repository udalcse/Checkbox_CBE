<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="CrossTabConfig.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.Controls.ItemRenderers.ReportEditor.CrossTabConfig" %>
<%@ Import Namespace="Checkbox.Common"%>
<%@ Import Namespace="Checkbox.Forms.Data"%>
<%@ Import Namespace="Checkbox.Globalization.Text" %>
<%@ Import Namespace="Checkbox.Wcf.Services.Proxies" %>
<%@ Import Namespace="Checkbox.Web"%>

<ul class="dashStatsContent allMenu">
    <li class="fixed_150"><%= WebTextManager.GetText("/controlText/frequencyItemRenderer/useAliases") %></li>
    <li><%= AnalysisItem.UseAliases ? WebTextManager.GetText("/common/yes") : WebTextManager.GetText("/common/no")%></li>
    <div class="clear"></div>
</ul>
<div class="clear"></div>

<ul class="dashStatsContent detailZebra allMenu">
    <li class="fixed_125"><%= WebTextManager.GetText("/controlText/crossTabItemEditor/xAxisItems")%></li>
     <li>
        <ol style="list-style-type:square;">
            <asp:Repeater ID="_xAxisItemRepeater" runat="server">
                <ItemTemplate>
                    <li><asp:Label ID="_itemTxtLbl" runat="server" Text='<%# GetItemText((int)Container.DataItem) %>' /></li>
                    <div class="clear"></div>
                </ItemTemplate>
            </asp:Repeater>
        </ol>
        <div class="clear"></div>
    </li>
    <div class="clear"></div>
</ul>
<div class="clear"></div>


<ul class="dashStatsContent allMenu">
    <li class="fixed_125"><%= WebTextManager.GetText("/controlText/crossTabItemEditor/yAxisItems")%></li>
     <li>
        <ol style="list-style-type:square;">
            <asp:Repeater ID="_yAxisItemRepeater" runat="server">
                <ItemTemplate>
                    <li><asp:Label ID="_itemTxtLbl" runat="server" Text='<%# GetItemText((int)Container.DataItem)  %>' /></li>
                    <div class="clear"></div>
                </ItemTemplate>
            </asp:Repeater>
        </ol>
        <div class="clear"></div>
    </li>
    <div class="clear"></div>
</ul>
<div class="clear"></div>

<script type="text/C#" runat="server">
    /// <summary>
    /// Get language code for text
    /// </summary>
    public string LanguageCode
    {
        get
        {
            return string.IsNullOrEmpty(AnalysisItem.InstanceData["LanguageCode"])
                ? TextManager.DefaultLanguage
                : AnalysisItem.InstanceData["LanguageCode"];
        }
    }
    
    /// <summary>
    /// Get Ids of X-Axis items
    /// </summary>
    public int[] XAxisItemIds
    {
        get { return ConvertCsvToIntArray(AnalysisItem.InstanceData["XAxisItemIds"]); }
    }

    /// <summary>
    /// Get Ids of Y-Axis items
    /// </summary>
    public int[] YAxisItemIds
    {
        get { return ConvertCsvToIntArray(AnalysisItem.InstanceData["YAxisItemIds"]); }
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="csv"></param>
    /// <returns></returns>
    protected int[] ConvertCsvToIntArray(string csv)
    {
        if (string.IsNullOrEmpty(csv))
        {
            return new int[] { };
        }

        var stringItemIds = csv.Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries);

        var itemIds = new List<int>();

        foreach (var stringItemId in stringItemIds)
        {
            int itemId;

            if (int.TryParse(stringItemId, out itemId))
            {
                itemIds.Add(itemId);
            }
        }

        return itemIds.ToArray();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="itemId"></param>
    /// <returns></returns>
    protected string GetItemText(int itemId)
    {
        return Utilities.StripHtmlAndEncode(SurveyMetaDataProxy.GetItemText(itemId, LanguageCode, false, true), 64);
    }
    
    /// <summary>
    /// Initialize control
    /// </summary>
    /// <param name="analysisItem"></param>
    protected override void InitializeConfigControl(ReportItemInstanceData analysisItem)
    {
        base.InitializeConfigControl(analysisItem);

        _xAxisItemRepeater.DataSource = XAxisItemIds;
        _xAxisItemRepeater.DataBind();

        _yAxisItemRepeater.DataSource = YAxisItemIds;
        _yAxisItemRepeater.DataBind();
    }

</script>
