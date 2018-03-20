<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="AverageScoreByPage.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.Controls.ItemRenderers.AverageScoreByPage" %>
<%@ Import Namespace="Checkbox.Common" %>
<%@ Register Src="~/Controls/Charts/AverageScoreByPageGraph.ascx" TagPrefix="ckbx" TagName="AverageScoreByPageGraph" %>
<%@ Register Src="~/Forms/Surveys/Reports/Controls/AppliedFilterDisplay.ascx" TagPrefix="ckbx" TagName="FilterDisplay" %>

<div class="itemContent pageBreak" style="margin:30px auto;">
    <ckbx:AverageScoreByPageGraph ID="_scoreGraph" runat="server" />
    <asp:Panel ID="_filterPanel" runat="server" style="border:1px solid #999999;margin-bottom:15px;text-align:left;margin-left:auto;margin-right:auto;"> 
        <div style="padding:10px;">
            <ckbx:FilterDisplay ID="_filterDisplay" runat="server" />
        </div>
    </asp:Panel>
</div>
<script type="text/C#" runat="server">
    
    /// <summary>
    /// 
    /// </summary>
    protected override void InlineInitialize()
    {
        base.InlineInitialize();

        _filterPanel.Width = Unit.Pixel(Utilities.AsInt(Appearance["Width"], 600));
    }

    /// <summary>
    /// Bind score graph
    /// </summary>
    protected override void InlineBindModel()
    {
        base.InlineBindModel();


        _scoreGraph.InitializeAndBind(Model, Appearance);
        _filterDisplay.InitializeAndBind(Model.AppliedFilterTexts);
        _filterPanel.Visible = Model.AppliedFilterTexts.Length > 0;
    }
</script>

