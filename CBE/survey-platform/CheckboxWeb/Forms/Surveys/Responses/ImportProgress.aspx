<%@ Page Language="C#" CodeBehind="ImportProgress.aspx.cs" Inherits="CheckboxWeb.Forms.Surveys.Responses.ImportProgress" 
MasterPageFile="~/Dialog.master" Theme="CheckboxTheme" %>
<%@ Register TagPrefix="cc1" Namespace="Checkbox.Web.UI.Controls" Assembly="Checkbox.Web" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>

<asp:Content ID="script" ContentPlaceHolderID="_headContent" runat="server">
    <ckbx:ResolvingScriptElement runat="server" Source="~/Resources/DialogHandler.js" />
    
    <script type="text/javascript">
          //Show message on error
        function onProgressError(errorMessage) {
              $('#returnDiv').hide();
              $('#errorDiv').show();
              $('#progressDiv').hide();
              $('#progressText').html('<div class="ErrorMessage">An error occurred while updating progress: <span style="color:black;font-weight:normal;">&nbsp;&nbsp;' + errorMessage + '</span></div>');
          }

          //Update status
          function onProgressUpdate(progressData) {
              $('#progressDiv').show();
              $('#progressText').html(progressData.Message);
          }

          //Do something on progress complete
          function onProgressComplete(progressData) 
          {
              $('#progressText').html('Import complete.');
              $('#returnDiv').show();
              $('#errorDiv').hide();
              $('#_pageButtonsContainer').hide();
              $('#<%=Master.CancelClientID%>').attr('disabled', false);
              $('#<%=Master.CancelClientID%>').click(function () { closeWindow({action : 'import', success: 'true'}); });
          }
    </script>
</asp:Content>

<asp:Content ID="content" ContentPlaceHolderID="_pageContent" runat="server">
    <div style="padding-left:25px;padding-top:25px;">
        <div style="padding-bottom:15px;">
            <cc1:MultiLanguageLabel ID="_queueingLbl" runat="server" CssClass="PrezzaLabel" TextId="/pageText/doResponseImport.aspx/importData" style="font-size:14px;">Importing XML file.  Please do not navigate away from this page.</cc1:MultiLanguageLabel>
        </div>
        <div id="progressDiv"></div>
        <div class="ProgressText" id="progressText"></div>
    </div>
    <asp:Panel ID="_linkPanel" runat="server" style="margin-top:25px;margin-left:25px;">
        <div id="errorDiv" style="margin-top:25px;display:none;">
            <cc1:MultiLanguageHyperLink ID="_backToImportLink" runat="server" CssClass="PrezzaLink" TextId="/pageText/doExport.aspx/clickHere" NavigateUrl="Import.aspx" />
            <cc1:MultiLanguageLabel ID="_toReturnLbl" runat="server" CssClass="PrezzaNormal" TextId="/pageText/responseImportProgress.aspx/toReturn" />
        </div>
        <div id="returnDiv" style="margin-top:25px;display:none;">
            <cc1:MultiLanguageLabel ID="MultiLanguageLabel1" runat="server" CssClass="PrezzaNormal" TextId="/pageText/responseImportProgress.aspx/toReturn" />
        </div>
    </asp:Panel>
</asp:Content>