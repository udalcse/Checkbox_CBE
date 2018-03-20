<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Details.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.Controls.ItemRenderers.ReportEditor.Details" %>
<%@ Import Namespace="Checkbox.Web.Forms.UI.Rendering" %>
<%@ Register Src="~/Forms/Surveys/Reports/Controls/ItemRenderers/ReportEditor/DetailsConfig.ascx" TagPrefix="ckbx" TagName="DetailsConfig" %>
<%@ Register Src="~/Forms/Surveys/Reports/Controls/ItemRenderers/ReportEditor/BasicConfiguration.ascx" TagPrefix="ckbx" TagName="BasicConfig" %>
<%@ Register Src="~/Controls/Charts/DetailsTable.ascx" TagPrefix="ckbx" TagName="DetailsTable" %>
<%@ Register Src="~/Controls/Charts/GroupedDetailsTable.ascx" TagPrefix="ckbx" TagName="GroupedDetailsTable" %>
<%@ Register Src="~/Forms/Surveys/Reports/Controls/AppliedFilterDisplay.ascx" TagPrefix="ckbx" TagName="FilterDisplay" %>

<ckbx:BasicConfig ID="_basicConfig" runat="server" />

<ckbx:DetailsConfig ID="_detailsConfig" runat="server" />

<div style="margin-top:15px;">
    <ckbx:DetailsTable ID="_detailsTable" runat="server" />
    <ckbx:GroupedDetailsTable ID="_groupedDetailsTable" runat="server" />

     <asp:Panel ID="_filterPanel" runat="server" style="padding:10px;border:1px solid #999999;margin-bottom:15px;text-align:left;"> 
        <div>
            <ckbx:FilterDisplay ID="_filterDisplay" runat="server" />
        </div>
    </asp:Panel>
</div>

<%-- Renderer Initialization  --%>
<script type="text/C#" runat="server">
    /// <summary>
    /// Get whether answers are grouped by response or not
    /// </summary>
    public bool GroupAnswers
    {
        get { return Model.GroupedDetailResults != null; }//"yes".Equals(Model.Metadata["GroupAnswers"], StringComparison.InvariantCultureIgnoreCase); }
    }

    /// <summary>
    /// Get list of child user controls that should also be initialized with analysis item data
    /// </summary>
    public override List<UserControlItemRendererBase> ChildUserControls
    {
        get
        {
            var childControls = new List<UserControlItemRendererBase>();

            if (GroupAnswers)
            {
                childControls.Add(_groupedDetailsTable);
            }
            else
            {
                childControls.Add(_detailsTable);
            }

            return childControls;
        }
    }

    /// <summary>
    /// Initialize renderer and children
    /// </summary>
    protected override void InlineInitialize()
    {
        base.InlineInitialize();
        
        _basicConfig.Initialize(Model);
        _detailsConfig.Initialize(Model);
    }
    
    /// <summary>
    /// Bind analysis item to renderers
    /// </summary>
    public override void BindModel()
    {
        base.BindModel();

        _groupedDetailsTable.Visible = GroupAnswers;
        _detailsTable.Visible = !_groupedDetailsTable.Visible;
        
        _filterDisplay.InitializeAndBind(Model.AppliedFilterTexts);
        _filterPanel.Visible = Model.AppliedFilterTexts.Length > 0;
    }
</script>