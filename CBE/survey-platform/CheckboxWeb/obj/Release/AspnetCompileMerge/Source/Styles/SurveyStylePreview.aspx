<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="SurveyStylePreview.aspx.cs" Inherits="CheckboxWeb.Styles.SurveyStylePreview" %>
<%@ Import Namespace="Checkbox.Globalization.Text" %>
<%@ Import Namespace="Checkbox.Styles" %>
<%@ Register TagPrefix="ckbx" TagName="StylePreview" Src="~/Styles/Controls/StylePreview.ascx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
    <head runat="server">
        <title></title>
        
        <%-- Specified script include placeholder --%>
        <asp:PlaceHolder ID="_scriptIncludesPlace" runat="server" />
    </head>
    <body>
        <form id="form1" runat="server">
            <div>
                <ckbx:StylePreview ID="_stylePreview" runat="server" />
                <asp:Panel ID="_idErrorPanel" runat="server" Visible="false">Style template id not specified or is not a number.</asp:Panel>
                <asp:Panel ID="_styleErrorPanel" runat="server" Visible="false">Style template could not be loaded.</asp:Panel>
            </div>
        </form>
    </body>
</html>

<script type="text/javascript">
    $(function () {
        if (typeof (_reloaded) != 'undefined' && !_reloaded) {
            loadStyleData(_dashStyleId, _dashStyleType);
            _reloaded = true;
        }
    });
</script>

<script type="text/C#" runat="server">
    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="e"></param>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        //Load template and bind data to preview
        int styleTemplateId;
        
        HtmlGenericControl ctrl = new HtmlGenericControl();
        ctrl.TagName = "link";
        ctrl.Attributes.Add("type", "text/css");
        ctrl.Attributes.Add("rel", "stylesheet");
        ctrl.Attributes.Add("media", "screen");
        ctrl.Attributes.Add("href", ResolveUrl("~/GlobalSurveyStyles.css"));
        ctrl.Attributes.Add("href", ResolveUrl("~/ScreenSurveyStyles.css"));
        Header.Controls.Add(ctrl);

        ctrl = new HtmlGenericControl();
        ctrl.TagName = "link";
        ctrl.Attributes.Add("type", "text/css");
        ctrl.Attributes.Add("rel", "stylesheet");
        ctrl.Attributes.Add("media", "screen"); 
        ctrl.Attributes.Add("href", ResolveUrl("~/CustomSurveyStyles.css"));
        Header.Controls.Add(ctrl);

        ctrl = new HtmlGenericControl();
        ctrl.TagName = "link";
        ctrl.Attributes.Add("type", "text/css");
        ctrl.Attributes.Add("rel", "stylesheet");
        ctrl.Attributes.Add("href", ResolveUrl("~/Styles/StylePreview.ashx") + "?s=" + Request.Params["s"] + "&d=" + (new Random()).Next(32535));
        Header.Controls.Add(ctrl);
        
        if(!int.TryParse(Request.QueryString["s"], out styleTemplateId))
        {
            _stylePreview.Visible = false;
            _idErrorPanel.Visible = true;
            _styleErrorPanel.Visible = false;
            return;
        }

        var styleTemplate = StyleTemplateManager.GetStyleTemplate(styleTemplateId);

        if (styleTemplate == null)
        {
            _stylePreview.Visible = false;
            _idErrorPanel.Visible = false;
            _styleErrorPanel.Visible = true;
            return;
        }

        _stylePreview.HeaderHtml = TextManager.GetText(styleTemplate.HeaderTextID, TextManager.DefaultLanguage);
        _stylePreview.FooterHtml = TextManager.GetText(styleTemplate.FooterTextID, TextManager.DefaultLanguage);
    }
</script>
