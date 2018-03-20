<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="CrossTab.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.Controls.ItemRenderers.CrossTab" %>
<%@ Import Namespace="Checkbox.Common" %>
<%@ Register Src="~/Controls/Charts/CrossTabTable.ascx" TagName="CrossTabTable" TagPrefix="ckbx" %>
<%@ Register Src="~/Forms/Surveys/Reports/Controls/AppliedFilterDisplay.ascx" TagPrefix="ckbx" TagName="FilterDisplay" %>

<div class="itemContent pageBreak" style="margin:30px auto;">
    <ckbx:CrossTabTable ID="_crossTabTable" runat="server" />

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