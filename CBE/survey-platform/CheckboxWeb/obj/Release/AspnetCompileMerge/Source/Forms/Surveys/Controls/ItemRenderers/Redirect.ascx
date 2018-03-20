<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Redirect.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.Redirect" %>
<%@ Import Namespace="System.Drawing" %>
<%@ Import Namespace="Checkbox.Common" %>

<%-- Redirect Item View for Survey --%>
<asp:Panel runat="server" ID="_containerPanel">
 <asp:HyperLink 
    ID="_previewLink" 
    runat="server"
    Text='<%# Model.InstanceData["LinkText"] %>'/>
</asp:Panel>

<% if (RedirectAutomatically) { %>
<script type="text/javascript" class="redirect-item-script">
    var isOpenInNewWindow =  <%=IsOpenInNewWindow.ToString().ToLower()%>;
    var autoRedirectDelay =  <%=AutoRedirectDelay%>;

    var openWindow = function() {
        if (isOpenInNewWindow)
            window.open('<%=Url%>');
        else
            window.location.href = '<%=Url%>';
    }

    if (!window.redirectTimeoutIds)
        window.redirectTimeoutIds = [];

    var id = setTimeout(openWindow, autoRedirectDelay * 1000);
    window.redirectTimeoutIds.push(id);
</script>
<% } %>


<script language="C#" runat="server">

    /// <summary>
    /// Build the URL
    /// </summary>
    /// <returns></returns>
    protected string getURL(bool restartSurvey)
    {
        var url = Model.InstanceData["redirectUrl"];

        if (!url.StartsWith("http://") && !restartSurvey && !string.IsNullOrEmpty(url) && !url.StartsWith("https://") && !url.StartsWith("ftp://") && !url.StartsWith("file://"))
        {
            url = "http://" +
                  (string.IsNullOrEmpty(_previewLink.NavigateUrl) ? url : _previewLink.NavigateUrl);
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

    protected bool RedirectAutomatically { set; get; }
    protected bool IsOpenInNewWindow { set; get; }
    protected int AutoRedirectDelay { set; get; }
    protected string Url { set; get; }

    /// <summary>
    /// Bind model to control
    /// </summary>
    protected override void InlineBindModel()
    {
        bool restartSurvey;
        bool redirectAutomatically;
        bool printing = ExportMode != ExportMode.None;
        bool isPreviewMode;
        bool isOpenInNewWindow;
        int autoRedirectDelay;

        bool.TryParse(Model.Metadata["isRestartSurvey"], out restartSurvey);
        bool.TryParse(Model.Metadata["isOpenInNewWindow"], out isOpenInNewWindow);
        bool.TryParse(Model.Metadata["isRedirectAutomatic"], out redirectAutomatically);
        bool.TryParse(HttpContext.Current.Request.QueryString["preview"], out isPreviewMode);
        if (!int.TryParse(Model.Metadata["redirectAutomaticallyDelay"], out autoRedirectDelay))
            autoRedirectDelay = 3;

        _previewLink.CssClass = redirectAutomatically? "hidden" : string.Empty;
        if (autoRedirectDelay == 0 && !isOpenInNewWindow && redirectAutomatically)
            _previewLink.CssClass += " automatic-redirect";

        _previewLink.Text = Model.InstanceData["LinkText"];

        string url = getURL(restartSurvey);
        _previewLink.NavigateUrl = url;

        if (isOpenInNewWindow)
            _previewLink.Target = "Blank";

        if (restartSurvey)
            _previewLink.CssClass = "workflowAjaxGetAction";

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

        RedirectAutomatically = redirectAutomatically && !printing && Page != null && !isPreviewMode;
        AutoRedirectDelay = autoRedirectDelay;
        IsOpenInNewWindow = isOpenInNewWindow;
        Url = url;
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