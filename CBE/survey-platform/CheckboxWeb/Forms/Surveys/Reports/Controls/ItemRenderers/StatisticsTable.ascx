<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="StatisticsTable.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.Controls.ItemRenderers.StatisticsTable" %>
<%@ Import Namespace="Checkbox.Analytics.Items" %>
<%@ Import Namespace="Checkbox.Globalization.Text" %>
<%@ Import Namespace="Checkbox.Common" %>
<%@ Import Namespace="Checkbox.Forms.Data" %>

<style runat="server" type="text/css" id="_dataControlStyles">
     /* Container for whole renderer */
     .itemContainer
     {
        clear:both;
     }

     /* Container for a row of the output */
    .rowDiv
    {
        clear:both;
        height:25px;
	    position:relative;
    }
    
     /* Bar showing average value */
	.bar
	{
	    background-color:blue;
	    height:20px;
	    border:1px solid black;
	    float:left;
	    margin-top:3px;
	    margin-bottom:3px;
    }
	
	 /* Text displayed in a row */
	.rowText
	{
	    width:100px;
	    float:left;
	    border-bottom:1px solid black;
	    border-left:1px solid black;
	    border-right:1px solid black;
	    text-align:center;
	    height:25px;
	    overflow:visible;
	    line-height:25px;
	    font-family:Arial;
        font-size:10px;
    }
    
    .questionCell
    {
        width:250px !important;
    }
    
    
    .verticalGridLine
    {
        height:25px;
        width:1px;
        background-color:#AAAAAA;
        overflow:hidden;
        float:left;
        position:absolute;
        top:2px;
    }
    
     /* Text displayed in a row */
	.rowHeaderText
	{
	    width:100px;
	    float:left;
	    background: lightgray;
	    border-top:1px solid black;
	    border-bottom:1px solid black;
	    border-right:1px solid black;
	    text-align:center;
	    height:25px;
	    overflow:visible;
	    line-height:25px;
    }
	
	 /* Labels displayed in header */
	.numberLabel
	{
	    width:60px;
	    float:left;
	    border:0px;
	    text-align:center;
	    height:25px;
	    line-height:25px;
	    border-top:1px solid black;
	    border-bottom:1px solid black;
        font-family:Arial;
        font-size:10px;
    }
	
	 /* Spacer div placed between bar and avg/median/std.dev text elements. */
	.spacer
	{
	    float:left;
	    height:25px;
    }
	
     /* Cap on end of error bar */
	.errorBarCap
	{
	    float:left;
	    position:absolute;
	    height:8px;
	    background-color:green;
	    width:2px;
	    margin-top:10px;
    }
	
	 /* Error bar */
	.errorBar
	{
	    margin-top:13px;
	    background-color:green;
	    float:left;
	    position:absolute;
	    height:2px;
    }

</style>

<div class="itemContent pageBreak" style="width: 825px; margin:30px auto;">
    <table class="matrix Matrix" cellspacing="0" cellpadding="2" rules="all" border="1" style="width: 825px; border-collapse:collapse;margin-left:auto;margin-right:auto;">
    	<tbody>
            <tr class="header">
        		<th align="center" scope="col">                
                        <ckbx:MultiLanguageLabel TextID="/controlText/StatisticsItemRenderer/question" runat="server"/>
                </th>
                <th align="right" width="100px" scope="col" runat="server" ID="_colShowResponses">
                        <ckbx:MultiLanguageLabel TextID="/controlText/StatisticsItemRenderer/responses" runat="server"/>
                </th>
                <th align="right" width="100px" scope="col" runat="server" ID="_colShowMean">
                        <ckbx:MultiLanguageLabel TextID="/controlText/StatisticsItemRenderer/mean" runat="server"/>
                </th>
                <th align="right" width="100px" scope="col" runat="server" ID="_colShowMedian">
                        <ckbx:MultiLanguageLabel TextID="/controlText/StatisticsItemRenderer/median" runat="server"/>
                </th>
                <th align="right" width="100px" scope="col" runat="server" ID="_colShowMode" >
                        <ckbx:MultiLanguageLabel TextID="/controlText/StatisticsItemRenderer/mode" runat="server"/>
                </th>
                <th align="right" width="100px" scope="col" runat="server" ID="_colShowStdDeviation" >
                        <ckbx:MultiLanguageLabel TextID="/controlText/StatisticsItemRenderer/standardDeviation" runat="server"/>
                </th>
			</tr>            
        <asp:Repeater ID="_rowRepeater" runat="server">
            <ItemTemplate>
                <tr class="<%# Container.ItemIndex % 2 == 0 ? "Item" : "AlternatingItem" %>">
				    <td align="left">
                        <asp:Label Text="<%#GetItemText((int)Container.DataItem)%>" runat="server"/>
                    </td>
                    <td align="right" width="100px" style="width:100px;" runat="server" Visible="<%#ShowResponses%>">
                        <asp:Label runat="server" Text='<%#GetResponseCount((int)Container.DataItem).ToString("0.#")%>' />
                    </td>
                    <td align="right" width="100px" style="width:100px;" runat="server" Visible="<%#ShowMean%>">
                        <asp:Label Text='<%#GetMean((int)Container.DataItem).ToString("0.0#")%>' runat="server" />                        
                    </td>
                    <td align="right" width="100px" style="width:100px;" runat="server" Visible="<%#ShowMedian%>">
                        <asp:Label Text='<%#GetMedian((int)Container.DataItem).ToString("0.0#")%>' runat="server"/>                        
                    </td>
                    <td align="right" width="100px" style="width:100px;" runat="server" Visible="<%#ShowMode%>">
                        <asp:Label Text='<%#GetMode((int)Container.DataItem).ToString("0.0#")%>' runat="server"/>                        
                    </td>
                    <td align="right" width="100px" style="width:100px;" runat="server" Visible="<%# ShowStdDeviation %>">
                        <asp:Label Text='<%#GetStdDeviation((int)Container.DataItem).ToString("0.0#")%>' runat="server" />
                    </td>
			    </tr>
            </ItemTemplate>
        </asp:Repeater>
		</tbody>
    </table>
</div>

<script type="text/C#" runat="server">
    private StatisticsItemReportingOption ReportOption { get; set; }

    /// <summary>
    /// Override OnInit to add styles to the page
    /// </summary>
    /// <param name="e"></param>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        if (Page != null && Page.Header != null)
        {
            Page.Header.Controls.Add(_dataControlStyles);
        }

        _colShowResponses.Visible = ShowResponses;
        _colShowMean.Visible = ShowMean;
        _colShowMedian.Visible = ShowMedian;
        _colShowMode.Visible = ShowMode;
        _colShowStdDeviation.Visible = ShowStdDeviation;
    }
    
    protected override void InlineInitialize()
    {        
        base.InlineInitialize();

        ReportOption = (StatisticsItemReportingOption)Enum.Parse(typeof(StatisticsItemReportingOption), Model.InstanceData["ReportOption"]);
    }

    protected override void InlineBindModel()
    {
        base.InlineBindModel();

        _rowRepeater.DataSource = Model.SourceItems.OrderBy(i => i.ItemPosition).OrderBy(i => i.PagePosition).Select(p => p.ItemId);
        _rowRepeater.DataBind();
    }

    /// <summary>
    /// Get language code for text
    /// </summary>
    public string LanguageCode
    {
        get
        {
            return string.IsNullOrEmpty(Model.InstanceData["LanguageCode"])
                ? TextManager.DefaultLanguage
                : Model.InstanceData["LanguageCode"];
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="itemId"></param>
    /// <returns></returns>
    protected string GetItemText(int itemId)
    {
        return Utilities.StripHtml(SurveyMetaDataProxy.GetItemText(itemId, LanguageCode, Model.UseAliases, true), 32);
    }

    /// <summary>
    /// Determine if responses should be shown
    /// </summary>
    protected bool ShowResponses { get { return Model.AppearanceData["ShowResponses"].Equals("True", StringComparison.InvariantCultureIgnoreCase) && (ReportOption == StatisticsItemReportingOption.All || ReportOption == StatisticsItemReportingOption.Responses); } }

    /// <summary>
    /// Determine if mean should be shown
    /// </summary>
    protected bool ShowMean { get { return Model.AppearanceData["ShowMean"].Equals("True", StringComparison.InvariantCultureIgnoreCase) && (ReportOption == StatisticsItemReportingOption.All || ReportOption == StatisticsItemReportingOption.Mean); } }

    /// <summary>
    /// Determine if median should be shown
    /// </summary>
    protected bool ShowMedian { get { return Model.AppearanceData["ShowMedian"].Equals("True", StringComparison.InvariantCultureIgnoreCase) && (ReportOption == StatisticsItemReportingOption.All || ReportOption == StatisticsItemReportingOption.Median); } }

    /// <summary>
    /// Determine if mode should be shown
    /// </summary>
    protected bool ShowMode { get { return Model.AppearanceData["ShowMode"].Equals("True", StringComparison.InvariantCultureIgnoreCase) && (ReportOption == StatisticsItemReportingOption.All || ReportOption == StatisticsItemReportingOption.Mode); } }

    /// <summary>
    /// Determine if standard deviation should be shown
    /// </summary>
    protected bool ShowStdDeviation { get { return Model.AppearanceData["ShowStdDeviation"].Equals("True", StringComparison.InvariantCultureIgnoreCase) && (ReportOption == StatisticsItemReportingOption.All || ReportOption == StatisticsItemReportingOption.StdDeviation); } }

    /// <summary>
    /// Get Response Count
    /// </summary>
    protected double GetResponseCount(int itemId)
    {
        return Model.CalculateResult.First(p => (p.ResultKey.Equals(itemId.ToString())) && p.ResultText.Equals("Responses")).ResultValue;
    }

    /// <summary>
    /// Get Mean
    /// </summary>
    protected double GetMean(int itemId)
    {
        return Model.CalculateResult.First(p => (p.ResultKey.Equals(itemId.ToString())) && p.ResultText.Equals("Mean")).ResultValue;
    }

    /// <summary>
    /// Get Median
    /// </summary>
    protected double GetMedian(int itemId)
    {
        return Model.CalculateResult.First(p => (p.ResultKey.Equals(itemId.ToString())) && p.ResultText.Equals("Median")).ResultValue;
    }

    /// <summary>
    /// Get Mode
    /// </summary>
    protected double GetMode(int itemId)
    {
        return Model.CalculateResult.First(p => (p.ResultKey.Equals(itemId.ToString())) && p.ResultText.Equals("Mode")).ResultValue;
    }

    /// <summary>
    /// Get Standard Deviation
    /// </summary>
    protected double GetStdDeviation(int itemId)
    {
        return Model.CalculateResult.First(p => (p.ResultKey.Equals(itemId.ToString())) && p.ResultText.Equals("StdDev")).ResultValue;
    }
    
</script>
