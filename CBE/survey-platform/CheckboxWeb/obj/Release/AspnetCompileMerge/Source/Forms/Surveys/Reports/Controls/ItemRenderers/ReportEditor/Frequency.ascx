<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Frequency.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.Controls.ItemRenderers.ReportEditor.Frequency" %>
<%@ Register TagPrefix="ckbx" TagName="BasicConfig" Src="~/Forms/Surveys/Reports/Controls/ItemRenderers/ReportEditor/BasicConfiguration.ascx" %>
<%@ Register Src="~/Forms/Surveys/Reports/Controls/ItemRenderers/ReportEditor/FrequencyConfig.ascx" TagPrefix="ckbx" TagName="FrequencyConfig" %>
<%@ Register Src="~/Controls/Charts/SummaryTable.ascx" TagPrefix="ckbx" TagName="SummaryTable" %>
<%@ Import Namespace="Checkbox.Analytics.Items.UI" %>
<%@ Import Namespace="Checkbox.Web.Forms.UI.Rendering" %>
<%@ Register Src="~/Forms/Surveys/Reports/Controls/AppliedFilterDisplay.ascx" TagPrefix="ckbx" TagName="FilterDisplay" %>

<div class="border999 shadow999">
    <ckbx:BasicConfig ID="_basicConfig" runat="server" />
    <ckbx:FrequencyConfig ID="_frequencyConfig" runat="server" />
</div>

<br class="clear"/>

<div style="margin-top:15px;">
    <ckbx:SummaryTable ID="_summaryTable" runat="server" HasTitle="true" />

    <asp:Panel ID="_filterPanel" runat="server" CssClass="border999 shadow999"  style="border:1px solid #999999;margin-top:15px;text-align:left;"> 
        <div style="padding:10px;">
            <ckbx:FilterDisplay ID="_filterDisplay" runat="server" />
        </div>
    </asp:Panel>
</div>

<script type="text/C#" runat="server">
    /// <summary>
    /// Get list of child user controls to be initialized/bound by renderer. Depending on appearance, we'll show/hide
    /// table, chart, etc.
    /// </summary>
    public override List<UserControlItemRendererBase> ChildUserControls
    {
        get
        {
            var controls = new List<UserControlItemRendererBase>();

            string graphType = Appearance["GraphType"];
            
            if(GraphType.SummaryTable.ToString().Equals(graphType, StringComparison.InvariantCultureIgnoreCase))
            {
                controls.Add(_summaryTable);
            }

            return controls;
        }
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

    /// <summary>
    /// 
    /// </summary>
    protected override void InlineBindModel()
    {
        base.InlineBindModel();

        _filterDisplay.InitializeAndBind(Model.AppliedFilterTexts);
        _filterPanel.Visible = Model.AppliedFilterTexts.Length > 0;
    }
    
</script>