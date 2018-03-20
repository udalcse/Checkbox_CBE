<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="AverageScore.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.Controls.ItemRenderers.ReportEditor.AverageScore" %>
<%@ Register Src="~/Forms/Surveys/Reports/Controls/ItemRenderers/ReportEditor/AverageScoreItemConfig.ascx" TagPrefix="ckbx" TagName="AverageScoreConfig" %>
<%@ Register Src="~/Controls/Charts/AverageScoreGraph.ascx" TagPrefix="ckbx" TagName="AverageScoreGraph" %>
<%@ Register TagPrefix="ckbx" TagName="BasicConfig" Src="~/Forms/Surveys/Reports/Controls/ItemRenderers/ReportEditor/BasicConfiguration.ascx" %>
<%@ Register Src="~/Forms/Surveys/Reports/Controls/AppliedFilterDisplay.ascx" TagPrefix="ckbx" TagName="FilterDisplay" %>


<ckbx:BasicConfig ID="_basicConfig" runat="server" />

<ckbx:AverageScoreConfig ID="_scoreConfig" runat="server" />

<div style="margin-top:15px;">
    <ckbx:AverageScoreGraph ID="_scoreGraph" runat="server" />

     <asp:Panel ID="_filterPanel" runat="server" style="padding:10px;border:1px solid #999999;margin-bottom:15px;text-align:left;"> 
        <div>
            <ckbx:FilterDisplay ID="_filterDisplay" runat="server" />
        </div>
    </asp:Panel>
</div>

<script type="text/C#" runat="server">
    /// <summary>
    /// Initialize renderer and children
    /// </summary>
    protected override void InlineInitialize()
    {
        base.InlineInitialize();

        _basicConfig.Initialize(Model);
        _scoreConfig.Initialize(Model);
    }

    /// <summary>
    /// Bind score graph
    /// </summary>
    protected override void InlineBindModel()
    {
        base.InlineBindModel();
        
        _scoreGraph.InitializeAndBind(Model,  Appearance);
        _filterDisplay.InitializeAndBind(Model.AppliedFilterTexts);
        _filterPanel.Visible = Model.AppliedFilterTexts.Length > 0;
    }

</script>

