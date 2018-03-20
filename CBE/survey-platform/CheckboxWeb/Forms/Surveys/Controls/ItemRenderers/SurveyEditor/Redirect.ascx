<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Redirect.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.SurveyEditor.Redirect" %>
<%@ Import Namespace="Checkbox.Common" %>
<%@ Import Namespace="Checkbox.Web"%>
<%@ Import Namespace="System.Drawing" %>

<%-- Redirect Item View for Survey Editor --%>
<asp:Panel runat="server" ID="_containerPanel">
    <div class="formInput" style="min-height: <%#Model.AppearanceData["FontSize"]%>px; display:block;">
        <p><label><asp:Literal ID="_linkLbl" runat="server" /></label></p>
        <br class="clear:both"/>
        <asp:HyperLink 
            ID="_previewLink" 
            runat="server" 
            NavigateUrl='<%# Model.InstanceData["redirectUrl"] %>' 
            Text='<%# LinkText %>' />
    </div>
    <br class="clear"/>

    [<asp:Label ID="_linkUrlLbl" runat="server" />]
</asp:Panel>

<script type="text/C#" runat="server">
    /// <summary>
    /// 
    /// </summary>
    private string LinkText
    {
        get { return Utilities.AdvancedHtmlEncode(Model.InstanceData["LinkText"]); }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected Color? GetLinkColor()
    {
        if (string.IsNullOrEmpty(Model.AppearanceData["FontColor"]))
        {
            return null;
        }

        return Utilities.GetColor(Model.AppearanceData["FontColor"], false);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected FontUnit? GetLinkSize()
    {
        if (string.IsNullOrEmpty(Model.AppearanceData["FontSize"]))
        {
            return null;
        }

        var fontSize = Utilities.AsInt(Model.AppearanceData["FontSize"]);

        if (!fontSize.HasValue)
        {
            return null;
        }

        return FontUnit.Point(fontSize.Value);
    }

    /// <summary>
    /// Bind model to control
    /// </summary>
    protected override void InlineBindModel()
    {
        bool redirectAutomatically;
        bool restartSurvey;

        bool.TryParse(Model.Metadata["isRedirectAutomatic"], out redirectAutomatically);
        bool.TryParse(Model.Metadata["isRestartSurvey"], out restartSurvey);

        _previewLink.Visible = !redirectAutomatically;

        //Set url label
        _linkUrlLbl.Text = restartSurvey
            ? WebTextManager.GetText("/controlText/redirectItemRenderer/restartSurvey")
            : Model.InstanceData["redirectUrl"];

        //Now set text for description
        _linkLbl.Text = redirectAutomatically
            ? WebTextManager.GetText("/controlText/redirectItemRenderer/automaticallyRedirect")
            : WebTextManager.GetText("/controlText/redirectItemRenderer/displayLink");

        var linkColor = GetLinkColor();

        if (linkColor.HasValue)
        {
            _previewLink.ForeColor = linkColor.Value;
        }

        var fontSize = GetLinkSize();

        if (fontSize.HasValue)
        {
            _previewLink.Font.Size = fontSize.Value;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    protected override void InlineInitialize()
    {
        base.InlineInitialize();

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