<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Details.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.Controls.ItemRenderers.SurveyEditor.Details" %>
<%@ Import Namespace="Checkbox.Web.Forms.UI.Rendering" %>
<%@ Register Src="~/Forms/Surveys/Reports/Controls/ItemRenderers/SurveyEditor/DetailsConfig.ascx" TagPrefix="ckbx" TagName="DetailsConfig" %>
<%@ Register Src="~/Forms/Surveys/Reports/Controls/ItemRenderers/SurveyEditor/BasicConfiguration.ascx" TagPrefix="ckbx" TagName="BasicConfig" %>
<%@ Register Src="~/Controls/Charts/DetailsTable.ascx" TagPrefix="ckbx" TagName="DetailsTable" %>
<%@ Register Src="~/Controls/Charts/GroupedDetailsTable.ascx" TagPrefix="ckbx" TagName="GroupedDetailsTable" %>

<ckbx:BasicConfig ID="_basicConfig" runat="server" />

<ckbx:DetailsConfig ID="_detailsConfig" runat="server" />

<ckbx:DetailsTable ID="_detailsTable" runat="server" />

<ckbx:GroupedDetailsTable ID="_groupedDetailsTable" runat="server" />

<%-- Renderer Initialization  --%>
<script type="text/C#" runat="server">
    /// <summary>
    /// Get whether answers are grouped by response or not
    /// </summary>
    public bool GroupAnswers
    {
        get { return "yes".Equals(Model.Metadata["GroupAnswers"], StringComparison.InvariantCultureIgnoreCase); }
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
    }
</script>