<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="CrossTab.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.Controls.ItemRenderers.ReportEditor.CrossTab" %>
<%@ Import Namespace="Checkbox.Analytics.Items" %>
<%@ Register Src="~/Forms/Surveys/Reports/Controls/ItemRenderers/ReportEditor/CrossTabConfig.ascx" TagName="CrossTabConfig" TagPrefix="ckbx" %>
<%@ Register Src="~/Controls/Charts/CrossTabTable.ascx" TagName="CrossTabTable" TagPrefix="ckbx" %>
<%@ Register Src="~/Forms/Surveys/Reports/Controls/AppliedFilterDisplay.ascx" TagPrefix="ckbx" TagName="FilterDisplay" %>

<ckbx:CrossTabConfig ID="_crossTabConfig" runat="server" />

<div style="margin-top:15px;">
    <ckbx:CrossTabTable ID="_crossTabTable" runat="server" />
    <asp:Panel ID="_filterPanel" runat="server" style="padding:10px;border:1px solid #999999;margin-bottom:15px;text-align:left;"> 
        <div>
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
        
        _crossTabConfig.Initialize(Model);
        _crossTabTable.Initialize(Model, null);
    }

    /// <summary>
    /// 
    /// </summary>
    protected override void InlineBindModel()
    {
        _crossTabTable.BindModel();
        _filterDisplay.InitializeAndBind(Model.AppliedFilterTexts);
        _filterPanel.Visible = Model.AppliedFilterTexts.Length > 0;
    }
</script>