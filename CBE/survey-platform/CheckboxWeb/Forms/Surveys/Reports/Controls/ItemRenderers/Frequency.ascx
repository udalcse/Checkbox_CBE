<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Frequency.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.Controls.ItemRenderers.Frequency" %>
<%@ Import Namespace="Checkbox.Common" %>
<%@ Import Namespace="Checkbox.Web.Forms.UI.Rendering" %>
<%@ Register Src="~/Controls/Charts/SummaryTable.ascx" TagPrefix="ckbx" TagName="SummaryTable" %>
<%@ Register Src="~/Forms/Surveys/Reports/Controls/AppliedFilterDisplay.ascx" TagPrefix="ckbx" TagName="FilterDisplay" %>
<%@ Register TagPrefix="ckbx" TagName="OthersTable" Src="~/Controls/Charts/OthersTable.ascx" %>

<div class="itemContent pageBreak" style="margin:30px auto;">
    <ckbx:SummaryTable ID="_summaryTable" runat="server" HasTitle="true"/>

    <asp:Panel ID="_filterPanel" runat="server" style="border:1px solid #999999;margin-bottom:15px;text-align:left;margin-left:auto;margin-right:auto;"> 
        <div style="padding:10px;">
            <ckbx:FilterDisplay ID="_filterDisplay" runat="server" />
        </div>
    </asp:Panel>

    <asp:Panel ID="_otherPanel" runat="server"> 
        <div style="padding:10px;">
            <ckbx:OthersTable ID="_othersTable" runat="server" />
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
            return new List<Checkbox.Web.Forms.UI.Rendering.UserControlItemRendererBase> {_summaryTable, _othersTable};
        }
    }

    /// <summary>
    /// 
    /// </summary>
    protected override void InlineInitialize()
    {
        base.InlineInitialize();

        _filterPanel.Width = Unit.Pixel(Utilities.AsInt(Appearance["Width"], 600));

        //other options summary
        _otherPanel.Visible = "AggregateAndDisplay".Equals(Model.Metadata["OtherOption"], StringComparison.InvariantCultureIgnoreCase)
            || "Display".Equals(Model.Metadata["OtherOption"], StringComparison.InvariantCultureIgnoreCase);
        if (_otherPanel.Visible)
        {
            Model.Metadata["DataSource"] = "DetailedResults";
        }
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