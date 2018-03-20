<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Redirect.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.SurveyMobile.Redirect" %>
<%@ Import Namespace="System.Drawing" %>
<%@ Import Namespace="Checkbox.Common" %>

<%-- Redirect Item View for Survey --%>
<asp:Panel runat="server" ID="_containerPanel">
 <asp:HyperLink 
    ID="_previewLink" 
    runat="server"
    Text='<%# Model.InstanceData["LinkText"] %>'/>
</asp:Panel>

<script type="text/javascript">
    $(function () {
        $('#<%=_previewLink.ClientID%>').on('click', function () {
            location.href = '<%=getURL() %>';
            return false;
        });
    });
</script>

<script language="C#" runat="server">

    /// <summary>
    /// Build the URL
    /// </summary>
    /// <returns></returns>
    protected string getURL()
    {
        bool restartSurvey;
        bool.TryParse(Model.Metadata["isRestartSurvey"], out restartSurvey);

        var url = Model.InstanceData["redirectUrl"];
            
        if (!url.StartsWith("http://") && !restartSurvey && !string.IsNullOrEmpty(url) && !url.StartsWith("https://") && !url.StartsWith("ftp://") && !url.StartsWith("file://"))
        {
            url = "http://" + url;
        }
        if (HttpContext.Current.Request.Params["test"] != null)
        {
            if (url.Contains("?"))
            {
                url = url + "&test=" + Request.Params["test"];
            }
            else
            {
                url = url + "?test=" + Request.Params["test"];
            }
        }

        return url;
    }
    
    /// <summary>
    /// Bind model to control
    /// </summary>
    protected override void InlineBindModel()
    {
        bool redirectAutomatically;
        bool printing = ExportMode != ExportMode.None;
        bool isPreviewMode;
        bool isOpenInNewWindow;
        int autoRedirectDelay;

        var url = getURL();

        bool.TryParse(Model.Metadata["isOpenInNewWindow"], out isOpenInNewWindow);
        bool.TryParse(Model.Metadata["isRedirectAutomatic"], out redirectAutomatically);
        bool.TryParse(HttpContext.Current.Request.QueryString["preview"], out isPreviewMode);
        if (!int.TryParse(Model.Metadata["redirectAutomaticallyDelay"], out autoRedirectDelay))
            autoRedirectDelay = 3;

        _previewLink.Text = Model.InstanceData["LinkText"];
        _previewLink.Attributes["href"] = "javascript: void(0);";

        _previewLink.CssClass = redirectAutomatically ? "hidden" : string.Empty;
        if (autoRedirectDelay == 0 && !isOpenInNewWindow && redirectAutomatically)
        {
            _previewLink.Attributes["href"] = url;
            _previewLink.CssClass += " automatic-redirect";
        }
        
        if (isOpenInNewWindow)
            _previewLink.Target = "Blank";

        //bind the appearence to redirect item
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

        if (redirectAutomatically && !printing && Page != null && !isPreviewMode)
        {
            var redirect = new StringBuilder();
            redirect.Append("function openWindow()");

            if (!isOpenInNewWindow)
                redirect.Append("{ window.location.href = '" + url + "';};");
            else
                redirect.Append("{ window.open('" + url + "');};");

            Page.ClientScript.RegisterClientScriptBlock(GetType(), "openWindow", redirect.ToString(), true);
            Page.ClientScript.RegisterStartupScript(GetType(), "openWindowTimer", "setTimeout('openWindow()', " + autoRedirectDelay * 1000 + ");", true);
        }
    }

    /// <summary>
    /// Get item font color
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
    /// Get item font size
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