<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GovernancePrioritySummary.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.Controls.ItemRenderers.GovernancePrioritySummary" %>
<%@ Import Namespace="Checkbox.Common" %>
<%@ Register Src="~/Controls/Charts/GovernancePrioritySummary.ascx" TagPrefix="ckbx" TagName="GovernancePrioritySummary" %>
<%@ Register Src="~/Forms/Surveys/Reports/Controls/AppliedFilterDisplay.ascx" TagPrefix="ckbx" TagName="FilterDisplay" %>
<%@ Register TagPrefix="ckbx" TagName="BasicConfig" Src="~/Forms/Surveys/Reports/Controls/ItemRenderers/ReportEditor/BasicConfiguration.ascx" %>

<div class="itemContent pageBreak" style="padding:30px 0px;">
    <ckbx:GovernancePrioritySummary ID="_governancePriorityGraph" runat="server" />

   <%-- <asp:Panel ID="_filterPanel" runat="server" style="border:1px solid #999999;margin-bottom:15px;text-align:left;margin-left:auto;margin-right:auto;"> 
        <div style="padding:10px;">
            <ckbx:FilterDisplay ID="_filterDisplay" runat="server" />
        </div>
    </asp:Panel>--%>
</div>

