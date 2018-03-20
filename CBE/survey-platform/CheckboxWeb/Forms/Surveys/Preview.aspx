<%@ Page Language="C#" CodeBehind="Preview.aspx.cs" Inherits="CheckboxWeb.Forms.Surveys.Preview" Theme="" %>

<%@ Register TagPrefix="ckbx" TagName="PreviewControl" Src="~/Forms/Surveys/Controls/SurveyPreview.ascx" %>
<%@ Register Src="~/Forms/Surveys/Controls/TermRenderer.ascx" TagPrefix="ckbx" TagName="TermRenderer" %>
<%@ Import Namespace="Checkbox.Common" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Import Namespace="Newtonsoft.Json" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title><%=GetTitle() %></title>

    <% if (ExportMode == ExportMode.Pdf)
        { %>
    <style type="text/css">
        body {
            background-image: none !important;
            background-color: transparent !important;
        }
    </style>
    <% } %>

    <!-- Global Survey Stylesheets -->
    <ckbx:ResolvingCssElement runat="server" Media="screen, print" Source="../../GlobalSurveyStyles.css" />
    <ckbx:ResolvingCssElement runat="server" Media="screen, print" Source="../../Resources/css/smoothness/jquery-ui-1.10.2.custom.css" />

    <ckbx:ResolvingScriptElement runat="server" ID="_jqueryInclude" />

    <ckbx:ResolvingScriptElement runat="server" Source="../../Resources/tiny_mce/jquery.tinymce.min.js" />
    <ckbx:ResolvingScriptElement runat="server" Source="../../Resources/jquery-ui-1.10.2.custom.min.js" />
    <ckbx:ResolvingScriptElement runat="server" Source="../../Resources/tooltip.js" />
    <ckbx:ResolvingScriptElement runat="server" Source="../../Resources/globalHelper.js" />
    <ckbx:ResolvingScriptElement runat="server" Source="../../Resources/jquery.uniform.min.js" />

    <% if (isMobileBrowser)
        { %>
    <ckbx:ResolvingCssElement runat="server" Media="all" Source="../../CheckboxHandheld.css" />
    <ckbx:ResolvingCssElement runat="server" Media="all" Source="../../HandheldSurveyStyles.css" />
    <ckbx:ResolvingCssElement runat="server" ID="_mobileStyleInclude" />

    <ckbx:ResolvingScriptElement runat="server" Source="../../Resources/jquery.mobile-latest.min.js" />
    <% }
    else
    { %>
    <ckbx:ResolvingCssElement runat="server" Media="all" Source="../../CheckboxScreen.css" />
    <ckbx:ResolvingCssElement runat="server" Media="all" Source="../../ScreenSurveyStyles.css" />
    <ckbx:ResolvingScriptElement runat="server" Source="../../Resources/jquery-ui-combobox.js" />
    <% } %>

    <!--[if lte IE 7]>
            <link rel="Stylesheet" type="text/css" media="screen" href="../../GlobalSurveyStyles_IE.css" />
        <![endif]-->

    <!-- Survey-Specific Stylesheet -->
    <asp:PlaceHolder ID="_surveyStylePlace" runat="server" />

    <!--Style Overrides / Additional styles -->
    <ckbx:ResolvingCssElement runat="server" Media="screen, print" Source="../../CustomSurveyStyles.css" />
    <ckbx:ResolvingCssElement runat="server" Media="screen" Source="../../App_Themes/CheckboxTheme/Checkbox.css" />

    <% if (ExportMode == ExportMode.Default)
        { %>
    <ckbx:ResolvingScriptElement runat="server" Source="../../Resources/print.js" />
    <% }
        else if (!isMobileBrowser)
        { %>
    <ckbx:ResolvingScriptElement runat="server" Source="../../Resources/preview.js" />
    <% } %>

    <script type="text/javascript">
            $(function () {
                fixTableProperties();
            });
            $(document).tooltip();
    </script>

    <%-- Specified script include placeholder --%>
    <asp:PlaceHolder ID="_scriptIncludesPlace" runat="server" />

</head>

<body>
    <form id="_previewForm" runat="server">
        <div>
            <ckbx:PreviewControl ID="_previewControl" runat="server" />

            <ckbx:TermRenderer runat="server" ID="_termRenderer" />
        </div>
    </form>
</body>
</html>

<script type="text/C#" runat="server">
    /// <summary>
    /// Get title for preview window
    /// </summary>
    /// <returns></returns>
    protected string GetTitle()
    {
        return WebTextManager.GetText("/pageText/previewSurvey.aspx/surveyPreview") + " - " + Utilities.StripHtml(ResponseTemplate.Name, 64);
    }

    /*
          protected string ResolveInlineMobileCssElement()
    {
        if (isMobileBrowser)
        {
            const string template = "<link rel=\"Stylesheet\" type=\"text/css\" href=\"{0}\" />";
            string mobileTheme = MobileTheme ?? "default";
            string href = string.Format("../../Resources/mobile_themes/jquery.mobile-{0}.css", mobileTheme);
            return string.Format(template, href);
        }

        return string.Empty;
    }

     */
</script>

<script type="text/javascript">
    window.SurveyTerms = <%= JsonConvert.SerializeObject(ResponseTemplate.SurveyTerms) %>;

    $("html").click(function() {
        $(parent.document).find("html").click();
    });

    <% if (ExportMode == ExportMode.Pdf) {%>
         setTimeout(function() {

             $("a").each(function (index, elem){
                 var html = $(elem).html();
                 $(elem).replaceWith(html);
             });

         }, 2000);
    <% } %>
</script>
