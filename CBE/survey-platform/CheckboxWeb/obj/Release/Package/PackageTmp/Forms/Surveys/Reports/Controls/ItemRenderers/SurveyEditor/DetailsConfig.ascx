<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="DetailsConfig.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.Controls.ItemRenderers.SurveyEditor.DetailsConfig" %>
<%@ Import Namespace="Checkbox.Web"%>

<ul class="dashStatsContent allMenu">
    <li class="fixed_100"><%= WebTextManager.GetText("/controlText/detailsItemEditor/groupAnswers")%></li>
    <li><%= GroupAnswers ? WebTextManager.GetText("/common/yes") : WebTextManager.GetText("/common/no")%></li>
    <div class="clear"></div>
</ul>
<div class="clear"></div>

<ul class="dashStatsContent detailZebra allMenu">
    <li class="fixed_100"><%= WebTextManager.GetText("/controlText/detailsItemEditor/linkToResponses")%></li>
    <li><%= LinkToResponseDetails ? WebTextManager.GetText("/common/yes") : WebTextManager.GetText("/common/no")%></li>
    <div class="clear"></div>
</ul>
<div class="clear"></div>

<ul class="dashStatsContent allMenu">
    <li class="fixed_100"><%= WebTextManager.GetText("/controlText/detailsItemEditor/includeResponseDetails")%></li>
    <li><%= IncludeResponseDetails ? WebTextManager.GetText("/common/yes") : WebTextManager.GetText("/common/no")%></li>
    <div class="clear"></div>
</ul>
<div class="clear"></div>

<ul class="dashStatsContent detailZebra allMenu">
    <li class="fixed_100"><%= WebTextManager.GetText("/controlText/detailsItemEditor/showPageNumbers")%></li>
    <li><%= ShowPageNumbers ? WebTextManager.GetText("/common/yes") : WebTextManager.GetText("/common/no")%></li>
    <div class="clear"></div>
</ul>
<div class="clear"></div>

<ul class="dashStatsContent allMenu">
    <li class="fixed_100"><%= WebTextManager.GetText("/controlText/detailsItemEditor/includeMessageItems")%></li>
    <li><%= IncludeMessageItems ? WebTextManager.GetText("/common/yes") : WebTextManager.GetText("/common/no")%></li>
    <div class="clear"></div>
</ul>
<div class="clear"></div>

<ul class="dashStatsContent detailZebra allMenu">
    <li class="fixed_100"><%= WebTextManager.GetText("/controlText/detailsItemEditor/showHiddenItems")%></li>
    <li><%= ShowHiddenItems ? WebTextManager.GetText("/common/yes") : WebTextManager.GetText("/common/no")%></li>
    <div class="clear"></div>
</ul>
<div class="clear"></div>

<script type="text/C#" runat="server"> 
    /// <summary>
    /// Get whether answers are grouped by response or not
    /// </summary>
    public bool GroupAnswers
    {
        get { return "true".Equals(AnalysisItem.Metadata["GroupAnswers"], StringComparison.InvariantCultureIgnoreCase); }
    }

    /// <summary>
    /// Get whether answers are grouped by response or not
    /// </summary>
    public bool LinkToResponseDetails
    {
        get { return "true".Equals(AnalysisItem.Metadata["LinkToResponseDetails"], StringComparison.InvariantCultureIgnoreCase); }
    }

    /// <summary>
    /// Get whether Include Response Details option is enabled or not
    /// </summary>
    public bool IncludeResponseDetails
    {
        get { return "true".Equals(AnalysisItem.Metadata["IncludeResponseDetails"], StringComparison.InvariantCultureIgnoreCase); }
    }

    /// <summary>
    /// Get whether Show Page Numbers option is enabled or not
    /// </summary>
    public bool ShowPageNumbers
    {
        get { return "true".Equals(AnalysisItem.Metadata["ShowPageNumbers"], StringComparison.InvariantCultureIgnoreCase); }
    }

    /// <summary>
    /// Get whether Include Message/HTML Items option is enabled or not
    /// </summary>
    public bool IncludeMessageItems
    {
        get { return "true".Equals(AnalysisItem.Metadata["IncludeMessageItems"], StringComparison.InvariantCultureIgnoreCase); }
    }

    /// <summary>
    /// Get whether Show Hidden Items option is enabled or not
    /// </summary>
    public bool ShowHiddenItems
    {
        get { return "true".Equals(AnalysisItem.Metadata["ShowHiddenItems"], StringComparison.InvariantCultureIgnoreCase); }
    }
</script>
