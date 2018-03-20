<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="DisplayAnalysis.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.SurveyEditor.DisplayAnalysis" %>
<%@ Import Namespace="Checkbox.Web"%>

<%-- Redirect Item View for Survey --%>
<div class="formInput">
    <asp:Label ID="_linkLbl" runat="server" CssClass="label" />
    <br />
    <ckbx:MultiLanguageLabel ID="_urlLbl" runat="server" TextId="/controlText/redirectItemRenderer/url" />
    &nbsp&nbsp
    <asp:Label ID="_url" runat="server" />
    <br />
    <ckbx:MultiLanguageLabel ID="_linkTextLbl" runat="server" TextId="/controlText/redirectItemRenderer/linkText" />

    <asp:HyperLink 
        ID="_previewLink" 
        runat="server" 
        NavigateUrl='<%# Model.InstanceData["redirectUrl"] %>' 
        Text='<%# Model.InstanceData["LinkText"] %>' 
        uframeignore="true" />
</div>
<br class="clear"/>

<script language="C#" runat="server">
    /// <summary>
    /// Bind model to control
    /// </summary>
    protected override void InlineBindModel()
    {
        bool redirectAutomatically;
        bool restartSurvey;
        bool showInNewTab;

        bool.TryParse(Model.Metadata["isRedirectAutomatic"], out redirectAutomatically);
        bool.TryParse(Model.Metadata["isRestartSurvey"], out restartSurvey);
        bool.TryParse(Model.Metadata["showInNewTab"], out showInNewTab);

        _previewLink.Visible = !redirectAutomatically;
        _linkTextLbl.Visible = !redirectAutomatically;

        if (showInNewTab)
            _previewLink.Target = "_blank";
        
        _previewLink.NavigateUrl = Model.InstanceData["redirectUrl"];
        _previewLink.Text = Model.InstanceData["linkText"];

        if (!_previewLink.NavigateUrl.StartsWith("http://") && !restartSurvey && !string.IsNullOrEmpty(_previewLink.NavigateUrl) && !_previewLink.NavigateUrl.StartsWith("https://") && !_previewLink.NavigateUrl.StartsWith("ftp://") && !_previewLink.NavigateUrl.StartsWith("file://"))
        {
            _previewLink.NavigateUrl = "http://" + _previewLink.NavigateUrl;
        }

        _url.Text = _previewLink.NavigateUrl;

        //Now set text for description
        _linkLbl.Text = redirectAutomatically
            ? WebTextManager.GetText("/controlText/redirectItemRenderer/automaticallyRedirect")
            : WebTextManager.GetText("/controlText/redirectItemRenderer/displayLink");
    }
</script>