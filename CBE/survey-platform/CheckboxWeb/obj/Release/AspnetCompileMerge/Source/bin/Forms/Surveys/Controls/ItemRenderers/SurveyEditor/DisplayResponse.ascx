<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="DisplayResponse.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.SurveyEditor.DisplayResponse" %>
<%@ Import Namespace="Checkbox.Common" %>
<%@ Import Namespace="Checkbox.Wcf.Services.Proxies" %>

<asp:Panel ID="_containerPanel" runat="server" CssClass="itemContainer">

</asp:Panel>

<script type="text/C#" runat="server">


    public override void Initialize(IItemProxyObject dataTransferObject, int? itemNumber)
    {

        if (dataTransferObject is SurveyResponseItem)
        {
            SurveyResponseItem model = (SurveyResponseItem)dataTransferObject;

            EnsureChildControls();

            Visible = model.Visible;
            _containerPanel.Controls.Clear();

            string displayFormText = model.Metadata["DisplayInForm"];
            bool displayInForm = false;

            bool.TryParse(displayFormText, out displayInForm);

            string html = model.InstanceData["ResponseHtml"];
            string linkText = model.InstanceData["LinkText"];
            string linkUrl = model.InstanceData["LinkUrl"];

            if (string.IsNullOrEmpty(linkText))
                linkText = "View Response";

            string msg = string.Empty;

            if (!displayInForm)
                _containerPanel.Controls.Add(new LiteralControl(msg + "<br/><a href=\"#\" uframeignore=\"true\">" + linkText + "</a>"));
            else
                _containerPanel.Controls.Add(new LiteralControl(html));
        }

        base.Initialize(dataTransferObject, itemNumber);
    }

    /// <summary>
    /// Initialize child user controls to set repeat columns and other properties
    /// </summary>
    protected override void InlineInitialize()
    {
        SetItemPosition();
    }

    /// <summary>
    /// Set item position.
    /// </summary>
    protected void SetItemPosition()
    {
        _containerPanel.CssClass = "itemContainer itemPosition" + (Utilities.IsNotNullOrEmpty(Appearance["ItemPosition"]) ? Appearance["ItemPosition"] : "Left");
    }


</script>
