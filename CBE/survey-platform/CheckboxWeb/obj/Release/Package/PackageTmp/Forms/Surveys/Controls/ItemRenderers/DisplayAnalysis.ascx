<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="DisplayAnalysis.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.DisplayAnalysis" %>
<%@ Import Namespace="Checkbox.Web"%>

<%-- Redirect Item View for Survey --%>

<asp:Panel runat="server" ID="_containerPanel">
    <asp:HyperLink 
        ID="_previewLink" 
        runat="server"
        NavigateUrl='<%# Model.InstanceData["redirectUrl"] %>' 
        Text='<%# Model.InstanceData["LinkText"] %>'/>
</asp:Panel>

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

        if (showInNewTab)
            _previewLink.Target = "_blank";
        
        _previewLink.Visible = !redirectAutomatically;

        _previewLink.NavigateUrl = Model.InstanceData["redirectUrl"];
        _previewLink.Text = Model.InstanceData["linkText"];

        if (!_previewLink.NavigateUrl.StartsWith("http://") && !restartSurvey && !string.IsNullOrEmpty(_previewLink.NavigateUrl) && !_previewLink.NavigateUrl.StartsWith("https://") && !_previewLink.NavigateUrl.StartsWith("ftp://") && !_previewLink.NavigateUrl.StartsWith("file://"))
        {
            _previewLink.NavigateUrl = "http://" + _previewLink.NavigateUrl;
        }        

        if (redirectAutomatically)
        {
            var redirect = new StringBuilder();
            redirect.Append("function openWindow()");
            redirect.Append("{ window.location.href = '" + _previewLink.NavigateUrl + "';};");
            Page.ClientScript.RegisterClientScriptBlock(GetType(), "openWindow", redirect.ToString(), true);
            Page.ClientScript.RegisterStartupScript(GetType(), "openWindowTimer", "setTimeout('openWindow()', 100);", true);
        }
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
        _containerPanel.CssClass = "itemContainer itemPosition" + (Checkbox.Common.Utilities.IsNotNullOrEmpty(Appearance["ItemPosition"]) ? Appearance["ItemPosition"] : "Left");
    }
    
</script>