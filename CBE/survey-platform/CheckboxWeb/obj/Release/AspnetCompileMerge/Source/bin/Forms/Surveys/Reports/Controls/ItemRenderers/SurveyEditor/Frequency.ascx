<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Frequency.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.Controls.ItemRenderers.SurveyEditor.Frequency" %>
<%@ Register TagPrefix="ckbx" TagName="BasicConfig" Src="~/Forms/Surveys/Reports/Controls/ItemRenderers/SurveyEditor/BasicConfiguration.ascx" %>
<%@ Register Src="~/Forms/Surveys/Reports/Controls/ItemRenderers/SurveyEditor/FrequencyConfig.ascx" TagPrefix="ckbx" TagName="FrequencyConfig" %>
<%@ Register Src="~/Controls/Charts/SummaryTable.ascx" TagPrefix="ckbx" TagName="SummaryTable" %>
<%@ Import Namespace="Checkbox.Web.Forms.UI.Rendering" %>

<ckbx:BasicConfig ID="_basicConfig" runat="server" />

<ckbx:FrequencyConfig ID="_frequencyConfig" runat="server" />

<div style="margin-top:5px;">
    <hr size="1" />
</div>

<ckbx:SummaryTable ID="_summaryTable" runat="server" />

<script type="text/C#" runat="server">
    /// <summary>
    /// Get list of child user controls to be initialized/bound by renderer. Depending on appearance, we'll show/hide
    /// table, chart, etc.
    /// </summary>
    public override List<UserControlItemRendererBase> ChildUserControls
    {
        get { return new List<UserControlItemRendererBase> {_summaryTable}; }
    }

    /// <summary>
    /// Initialize renderer and children
    /// </summary>
    protected override void InlineInitialize()
    {
        base.InlineInitialize();

        _basicConfig.Initialize(Model);
        _frequencyConfig.Initialize(Model);
    }
    
</script>