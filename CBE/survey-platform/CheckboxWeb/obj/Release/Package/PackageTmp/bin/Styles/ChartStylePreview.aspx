<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="ChartStylePreview.aspx.cs" Inherits="CheckboxWeb.Styles.ChartStylePreview" %>
<%@ Import Namespace="Checkbox.Globalization.Text" %>
<%@ Import Namespace="Checkbox.Styles" %>
<%@ Import Namespace="Checkbox.Web.Charts" %>
<%@ Register TagPrefix="ckbx" TagName="ChartPreview" Src="~/Styles/Controls/ChartPreview.ascx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
    <head id="Head1" runat="server">
        <title></title>

        <%-- Specified script include placeholder --%>
        <asp:PlaceHolder ID="_scriptIncludesPlace" runat="server" />
    </head>
    <body>
        <form id="form1" runat="server">
            <div>
                <ckbx:ChartPreview ID="_stylePreview" runat="server" />
                <asp:Panel ID="_idErrorPanel" runat="server" Visible="false">Style template id not specified or is not a number.</asp:Panel>
                <asp:Panel ID="_styleErrorPanel" runat="server" Visible="false">Style template could not be loaded.</asp:Panel>
            </div>
        </form>
    </body>
</html>

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
        
        if(!int.TryParse(Request.QueryString["s"], out styleTemplateId))
        {
            _stylePreview.Visible = false;
            _idErrorPanel.Visible = true;
            _styleErrorPanel.Visible = false;
            return;
        }

        var chartAppearance = (SummaryChartItemAppearanceData)ChartStyleManager.GetChartStyleAppearance(styleTemplateId);

        if (chartAppearance == null)
        {
            _stylePreview.Visible = false;
            _idErrorPanel.Visible = false;
            _styleErrorPanel.Visible = true;
            return;
        }
        
        _stylePreview.Initialize(chartAppearance);
    }
</script>
