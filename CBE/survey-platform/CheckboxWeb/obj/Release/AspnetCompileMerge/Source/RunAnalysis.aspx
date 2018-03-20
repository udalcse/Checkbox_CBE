<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="RunAnalysis.aspx.cs" Inherits="CheckboxWeb.RunAnalysis" Theme="" %>
<%@ Import Namespace="Checkbox.Globalization.Text" %>
<%@ Import Namespace="Checkbox.Common" %>
<%@ Import Namespace="Checkbox.PdfExport" %>
<%@ Register Src="~/Forms/Surveys/Reports/Controls/RunReport/ReportView.ascx" TagPrefix="ckbx" TagName="ReportView" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
    <head runat="server" id="_head">
        <meta http-equiv="x-ua-compatible" content="IE=Edge">
        <link rel="stylesheet" href="App_Themes/CheckboxTheme/Checkbox.css"/>
        <title><asp:Literal ID="_pageTitle" runat="server" /></title>
     
        <%-- Global Report Stylesheets --%>
        <ckbx:ResolvingCssElement runat="server" Source="GlobalReportStyles.css" media="screen" />
        
        <%-- Script Includes --%>
        <ckbx:ResolvingScriptElement runat="server" Source="~/Resources/jquery-latest.min.js" />
        <ckbx:ResolvingScriptElement runat="server" Source="~/Resources/html2canvas.js" />
        <ckbx:ResolvingScriptElement runat="server" Source="~/Resources/html2canvas.svg.js" />
        <ckbx:ResolvingScriptElement runat="server" Source="~/Resources/jspdf.debug.js" />
        <ckbx:ResolvingScriptElement runat="server" Source="~/Resources/canvg/rgbcolor.js" />
        <ckbx:ResolvingScriptElement runat="server" Source="~/Resources/canvg/StackBlur.js" />
        <ckbx:ResolvingScriptElement runat="server" Source="~/Resources/canvg/canvg.js" />
        <ckbx:ResolvingScriptElement runat="server" Source="~/Resources/highchartCanvasExporting.js" />
        
        

        <%-- Report-Specific Stylesheet --%>
        <asp:PlaceHolder ID="_reportStyle" runat="server" />
         <script type="text/javascript" language="javascript">
             function htmlDecode(val) { if (val) { return $('<div/>').html(val).text(); } else { return ''; } }
         </script>
        
        <%-- Specified script include placeholder --%>
        <asp:PlaceHolder ID="_scriptIncludesPlace" runat="server" />
    </head>
    <body>
        <form id="_reportForm" runat="server">
            <%-- Add a dummy textbox because IE will not always submit the name/value of the button      --%>
            <%-- when the screen contains exactly one textbox --%>
            <asp:TextBox ID="_dummyBox" runat="server" style="visibility:hidden;display:none;" />
            
            <%-- Error Control --%>
            <asp:Panel CssClass="error" Visible="false" runat="server" ID="_errorPanel">
                <div style="margin:15px;">
                    <div style="float:left;">
                        <img src="<%=ResolveUrl("~/App_Themes/CheckboxTheme/images/Stop.png") %>" alt="Error" title="An error occurred." />
                    </div>
                    <div style="float:left;margin-left:10px;">
                        <div style="color:red;font-weight:bold;"><asp:Literal ID="_errorMessage" runat="server" /></div>
                        <div><asp:Literal ID="_errorSubMessage" runat="server"/></div>
                        <div style="font-size:10px;">
                            <asp:Literal ID="_moreInfoMessage" runat="server" />
                        </div>
                    </div>
                    <div style="clear:both;"></div>
                </div>
            </asp:Panel>
            <%-- Report View --%>
            
            <asp:Panel runat="server" ID="_pdfExportButton" CssClass="pdfExportButton" Visible="False" >
                <a class="ckbxButton roundedCorners border999 shadow999 redButton smallButton" href="#" ></a>                
            </asp:Panel>

            <ckbx:ReportView ID="_reportView" runat="server" />
        </form>
    </body>
</html>

<script type="text/javascript">
    function htmlDecode(val) { if (val) { return $('<div/>').html(val).text(); } else { return ''; } }

    $(function () {
        <% if(ShowPdfExportLink) { %>
        $('.pdfExportButton a').on('click', function () {
            window.open("Forms/Surveys/Reports/Export.aspx?r=<%= GetReportId() %>",
                    "mywindow", "menubar=0,resizable=0,width=600,height=370");
            }).text('<%= TextManager.GetText("/pageText/runAnalysis.aspx/exportPdf") %>');
        <% } else { %>
        $('.pdfExportButton').hide();
        <% } %>
    });

    $(function() {
        <% if (_exportMode == ExportMode.Pdf){ %>
            $(".pageBreak").removeClass("pageBreak");
            $(".surveyContentFrame").css("background", "transparent");

            setTimeout(function () {
                var allElements = $(".itemContent");
                var orientation = "<%= _orientation %>";
                var startPageHeight = orientation == "Portrait" ? 1230 : 750;
                var pageHeight = startPageHeight;
                var elementHeight = 0;
                var spacer = 15;

                for (var i = 0; i < allElements.length; i++) {
                    if (!i && pageHeight <= 0) {
                        pageHeight = startPageHeight;
                        var elemAfter = '<div class="reportPageBreak"></div>';
                        $(allElements[i]).before(elemAfter);
                    }
                    if ($(allElements[i]).find("svg").length > 0) {
                        elementHeight = $(allElements[i]).find("svg")[0].getBoundingClientRect().height;
                        pageHeight = pageHeight - elementHeight - spacer;
                    } else {
                        elementHeight = $(allElements[i]).outerHeight();
                        pageHeight = pageHeight - (elementHeight - spacer);
                        $(allElements[i]).css("page-break-inside", "avoid");
                    }

                    if (pageHeight <= 0) {
                        var elem = '<div class="reportPageBreak"></div>';
                        $(allElements[i]).before(elem);
                        pageHeight = startPageHeight - elementHeight;
                    }
                }

            },200);
            

        <% } %>
    });

    // Workaround for Highcharts, legend items not displaying in IE11
    (function (H) {
        if (/Trident.*?11\.0/.test(navigator.userAgent)) {
            H.wrap(H.Legend.prototype, 'positionItem', function (proceed, item) {
                var legend = this;
                setTimeout(function () {
                    proceed.call(legend, item);
                });
            });
        }
    }(Highcharts));
</script>
<script src="Resources/jquery.tmpl.min.js"></script>