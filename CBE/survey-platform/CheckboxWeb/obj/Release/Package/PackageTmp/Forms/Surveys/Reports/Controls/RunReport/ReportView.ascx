<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="ReportView.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.Controls.RunReport.ReportView" %>
<%@ Register Src="~/Forms/Surveys/Reports/Controls/RunReport/EnterPassword.ascx" TagPrefix="ckbx" TagName="EnterPassword" %>
<%@ Register Src="~/Forms/Surveys/Reports/Controls/RunReport/Login.ascx" TagPrefix="ckbx" TagName="Login" %>

<ckbx:ResolvingScriptElement runat="server" Source="~/Resources/jquery-latest.min.js" />
    
<% if (DisableStyleBackground) { %>
<style type="text/css">
    body {
        background-image:none !important;
        background-color:transparent !important;
    }
</style> 
<% } %>

<asp:PlaceHolder ID="_pagingPlace" runat="server" Visible="false">
    <script type="text/javascript">
        var analysisId = '<%= ReportId %>';
        var print = '<%=Request.QueryString["print"] %>';
        var aGuid = '<%= Request.QueryString["ag"] %>';
        var language = '<%= Request.QueryString["l"] %>';
        var ticket = '<%=Request.QueryString["tg"] %>';

        $(document).ready(function () {
            $('#<%=_pageNumberList.ClientID %>').change(function () {
                var pageSelValue = $('#<%=_pageNumberList.ClientID %>').val();
                var newPage = '';

                if (pageSelValue == 'ALL_PAGES') {
                    newPage = 'ALL_PAGES';
                }
                else {
                    newPage = parseInt($('#<%=_pageNumberList.ClientID %>').val(), 10);
                     
                    if (isNaN(newPage)) {
                        newPage = '';
                    }
                }

                document.location = 'RunAnalysis.aspx' 
                    + '?aid=' + analysisId
                    + '&print=' + print
                    + '&tg=' + ticket
                    + '&ag=' + aGuid
                    + '&l=' + language
                    + '&p=' + newPage;
            });
        });
    </script>
</asp:PlaceHolder>

<script type="text/javascript">
    $(document).ready(function () {
        $('.pageBreak').last().removeClass('pageBreak');
    });
</script>

<%-- Header --%>
<div class="headWrapper">
    <asp:Panel ID="_surveyTitlePanel" runat="server" visible="false" style="text-align:center;">
        <h1 id="_surveyTitle" runat="server"></h1>
    </asp:Panel>
    <asp:Literal ID="_headerTxt" runat="server" />
    <div class="clear"></div>
</div>
<div class="clear"></div>

<%-- Enter Password --%>
<ckbx:EnterPassword ID="_enterPassword" runat="server" />

<%-- Login to Checkbox  --%>
<ckbx:Login ID="_login" runat="server" />

<%-- Page View Repeater --%>
<asp:PlaceHolder ID="_pageViewPlaceHolder" runat="server" />

<%-- Footer --%>
<div class="footerWrapper">
    <asp:Literal ID="_footerTxt" runat="server" />
    <div class="clear"></div>
</div>
<div class="clear"></div>
