<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PageView.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.TakeSurvey.PageView" %>
<%@ Import Namespace="System.Web.Optimization" %>
<%@ Import Namespace="System.Web.Script.Serialization" %>

<%-- 
    Control Showing a view of a survey page.   The other page related controls will be moved
    to the page layout template at run time and the template will be loaded based on survey
    page configuration.
--%>

<asp:PlaceHolder ID="_pageLayoutPlace" runat="server" Visible="true" />

<%-- 
    Placeholder for page controls to be added to layout template.  Adding them to layout moves
    them from this location to be a child of the layout.  Items not added should be hidden.
--%>

<%-- Header --%>
<asp:Panel ID="_headerPanel" runat="server" CssClass="surveyHeaderWrapper">
    <asp:Literal ID="_headerTxt" runat="server" />
</asp:Panel>

<%-- Title --%>
<asp:Panel ID="_titlePanel" runat="server" CssClass="titleWrapper">
    <asp:Label ID="_titleLbl" runat="server" CssClass="title" />
</asp:Panel>

<%-- Progress Bar --%>
<asp:Panel ID="_progressPanel" runat="server" CssClass="progressWrapper">
    <ckbx:ProgressBar ID="_progressBar" runat="server" />
</asp:Panel>

<%-- Page Numbers --%>
<asp:Panel ID="_pageNumbersPanel" runat="server" CssClass="pageNumberWrapper">
    <asp:Label ID="_pageNumberLbl" runat="server" CssClass="PageNumber" />
   
</asp:Panel>



<%-- Navigation --%>
<asp:Button ID="_prevBtn" runat="server" Text="<< Back" CssClass="surveyFooterButton workflowAjaxPostAction" CausesValidation="false" />
<asp:Button ID="_saveBtn" runat="server" Text="Save and Exit" CssClass="surveyFooterButton workflowAjaxPostAction" CausesValidation="false"/>
<asp:Button ID="_resetBtn" runat="server" Text="Form Reset" CssClass="surveyFooterButton reset-form-button" UseSubmitBehavior="False" CausesValidation="false" />
<asp:Button ID="_resetLnk" runat="server" Text="Form Reset" CssClass="surveyFooterButton reset-form-button" />
<asp:Button ID="_nextBtn" runat="server" Text="Next" CssClass="surveyFooterButton workflowAjaxPostAction" CausesValidation="True" />       
<asp:Button ID="_finishBtn" runat="server" Text="Finish" CssClass="surveyFooterButton workflowAjaxPostAction" CausesValidation="true" />  
<div class="survey-print-btn-container">     
<asp:LinkButton ID="_printButton" Visible="False"  runat="server"  Text="Save Responses to PDF" CssClass="ckbxButton silverButton roundedCorners surveyPrintBtn" />  
<div><asp:Label ID="_limitReachedWarnMessage" Visible="False"  runat="server"  Text="You will not be able to save your responses after leaving this page." /></div>
</div>
  <div id="ckbx_dialogContainer" class="ckbx_simplemodal-container"></div>
<%-- CheckBox Logo Footer --%>
<asp:Panel ID="_ckbxLogoFooterPanel" runat="server">
    <asp:Literal ID="_logoText" runat="server" />
</asp:Panel>
 <asp:HiddenField runat="server" ID="_hiddenCurrentPage" />
<%-- Footer --%>
<asp:Panel ID="_footerPanel" runat="server" CssClass="surveyFooterWrapper">
    <asp:Literal ID="_footerTxt" runat="server" />
</asp:Panel>
     
    <asp:PlaceHolder runat="server">        
         <%: Scripts.Render("~/bundles/PageView.js") %>
        <script>
            var styleConfigs = <%= new JavaScriptSerializer().Serialize(ResponseTemplate.StyleSettings) %>;
            var currentPageNumber = <%= CurrentPageNumber %>;
            var totalPageNumbers = <%= TotalPageNumbers %>;
        </script>
    </asp:PlaceHolder>

<script>
    $(function () {
        function handleMatrixPageBreaks() {
            $("div[data-matrix-break='true']").parent().parent().next().append('<div style="page-break-after:always"></div>');
        }
       
        handleMatrixPageBreaks();
    });
</script>
<style>
    div.itemZoneWrapper [id$='_Wrapper'], div.bottomAndOrRightContainer.inputForLabelLeft, .rank-order-bottom-right-container{
        page-break-inside: avoid;
    }
</style>
