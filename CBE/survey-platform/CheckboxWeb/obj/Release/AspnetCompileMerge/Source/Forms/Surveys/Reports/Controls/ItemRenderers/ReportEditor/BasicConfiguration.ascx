<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="BasicConfiguration.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.Controls.ItemRenderers.ReportEditor.BasicConfiguration" %>
<%@ Import Namespace="Checkbox.Common" %>
<%@ Import Namespace="Checkbox.Forms.Data" %>
<%@ Import Namespace="Checkbox.Globalization.Text" %>
<%@ Import Namespace="Checkbox.Wcf.Services.Proxies" %>
<%@ Import Namespace="Checkbox.Web" %>


<ul class="dashStatsContent allMenu">
    <li class="fixed_100"><%= WebTextManager.GetText("/controlText/frequencyItemRenderer/useAliases") %></li>
    <li><%= AnalysisItem.UseAliases ? WebTextManager.GetText("/common/yes") : WebTextManager.GetText("/common/no")%></li>
    <div class="clear"></div>
</ul>
<div class="clear"></div>

<ul class="dashStatsContent detailZebra allMenu">
    <li class="fixed_100"><%= WebTextManager.GetText("/controlText/frequencyItemRenderer/sourceItems") %></li>
    <li>
        <ol style="list-style-type:square;">
            <asp:Repeater ID="_sourceItemRepeater" runat="server">
                <ItemTemplate>
                    <li><span><asp:Label ID="_itemTxtLbl" runat="server" Text='<%# GetItemText((int)DataBinder.Eval(Container.DataItem, "ItemId"))  %>' /></span></li>
                    <div class="clear"></div>
                </ItemTemplate>
                </asp:Repeater>
        </ol>
        <div class="clear"></div>
    </li>
    <div class="clear"></div>
</ul>
<div class="clear"></div>

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
    /// 
    /// </summary>
    /// <param name="itemId"></param>
    /// <returns></returns>
    protected string GetItemText(int itemId)
    {
        return Utilities.DecodeAndStripHtml(SurveyMetaDataProxy.GetItemText(itemId, LanguageCode, false, true), 64);
    }

    /// <summary>
    /// Initialize configuration control
    /// </summary>
    /// <param name="analysisItem"></param>
    protected override void InitializeConfigControl(ReportItemInstanceData analysisItem)
    {
        base.InitializeConfigControl(analysisItem);

        _sourceItemRepeater.DataSource = analysisItem.SourceItems;
        _sourceItemRepeater.DataBind();
    }
    
</script>