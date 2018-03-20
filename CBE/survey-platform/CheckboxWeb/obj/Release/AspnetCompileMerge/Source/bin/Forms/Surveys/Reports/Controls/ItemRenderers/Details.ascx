<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Details.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.Controls.ItemRenderers.Details" %>
<%@ Import Namespace="Checkbox.Common" %>
<%@ Import Namespace="Checkbox.Web.Forms.UI.Rendering" %>
<%@ Register Src="~/Controls/Charts/DetailsTable.ascx" TagPrefix="ckbx" TagName="DetailsTable" %>
<%@ Register Src="~/Controls/Charts/GroupedDetailsTable.ascx" TagPrefix="ckbx" TagName="GroupedDetailsTable" %>
<%@ Register Src="~/Forms/Surveys/Reports/Controls/AppliedFilterDisplay.ascx" TagPrefix="ckbx" TagName="FilterDisplay" %>

<div class="itemContent pageBreak" style="margin:30px auto;">
    <ckbx:DetailsTable ID="_detailsTable" runat="server" />
    <ckbx:GroupedDetailsTable ID="_groupedDetailsTable" runat="server" />

    <asp:Panel ID="_filterPanel" runat="server" style="border:1px solid #999999;margin-bottom:15px;text-align:left;margin-left:auto;margin-right:auto;"> 
        <div style="padding:10px;">
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
    /// 
    /// </summary>
    protected override void InlineInitialize()
    {
        base.InlineInitialize();

        _filterPanel.Width = Unit.Pixel(Utilities.AsInt(Appearance["Width"], 600));
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