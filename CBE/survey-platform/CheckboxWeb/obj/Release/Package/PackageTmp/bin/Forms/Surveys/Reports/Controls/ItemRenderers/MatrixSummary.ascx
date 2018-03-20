<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="MatrixSummary.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.Controls.ItemRenderers.MatrixSummary" %>
<%@ Register Src="~/Controls/Charts/MatrixSummaryTable.ascx" TagPrefix="ckbx" TagName="MatrixSummaryTable" %>
<%@ Register Src="~/Forms/Surveys/Reports/Controls/AppliedFilterDisplay.ascx" TagPrefix="ckbx" TagName="FilterDisplay" %>

<div class="pageBreak" >
    <ckbx:MatrixSummaryTable ID="_table" runat="server"/>
    <asp:Panel ID="_filterPanel" runat="server" style="padding:10px;border:1px solid #999999;margin-bottom:15px;"> 
        <div>
            <ckbx:FilterDisplay ID="_filterDisplay" runat="server" />
        </div>
        
    </asp:Panel>
</div>

<%-- Renderer Initialization  --%>
<script type="text/C#" runat="server">
    /// <summary>
    /// 
    /// </summary>
    protected override void InlineInitialize()
    {
        base.InlineInitialize();

        _table.Initialize(Model, null);
        
    }

    /// <summary>
    /// 
    /// </summary>
    protected override void InlineBindModel()
    {
        _table.BindModel();
        _filterDisplay.InitializeAndBind(Model.AppliedFilterTexts);

        _filterPanel.Visible = Model.AppliedFilterTexts.Length > 0;
    }
</script>