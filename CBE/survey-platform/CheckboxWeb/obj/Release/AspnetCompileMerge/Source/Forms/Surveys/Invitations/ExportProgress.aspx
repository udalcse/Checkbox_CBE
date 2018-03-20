<%@ Page language="c#" Inherits="CheckboxWeb.Forms.Surveys.Invitations.ExportProgress" MasterPageFile="~/Dialog.Master" Codebehind="ExportProgress.aspx.cs" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ MasterType VirtualPath="~/Dialog.master" %>

<asp:Content ID="script" ContentPlaceHolderID="_headContent" runat="server">
    <script type="text/javascript">
          //Show message on error
        function onProgressError(errorMessage) {
              $('#returnDiv').show();
              $('#progressDiv').hide();
              $('#progressText').html('<div class="ErrorMessage">An error occurred while updating progress: <span style="color:black;font-weight:normal;">&nbsp;&nbsp;' + errorMessage + '</span></div>');
          }

          //Update status
          function onProgressUpdate(progressData) {
              $('#progressDiv').show();
              $('#progressText').html(progressData.StatusMessage);
          }

          //Do something on progress complete
          function onProgressComplete(progressData) {
              if (progressData.AdditionalData != null
                && progressData.AdditionalData != '') {
                  $('#<%=_s3DownloadButton.ClientID %>').attr('href', progressData.AdditionalData);
              }
              
              $('#progressText').empty();//.html('');
              $('#returnDiv').show();

              //S3
              $('#<%=_s3DownloadDesc.ClientID %>').show();
              $('#<%=_s3DownloadButton.ClientID %>').show();

              //Direct
              $('#<%=_exportReadyLbl.ClientID %>').show();
              $('#_dlButtonContainer').show();
          }
    </script>
</asp:Content>

<asp:Content ID="content" ContentPlaceHolderID="_pageContent" runat="server">
    <div class="dialogSubContainer">
        <div class="dashStatsWrapper border999 shadow999">
            <div class="dashStatsHeader" style="padding:5px;">
                <span class="mainStats left"><%=WebTextManager.GetText("/pageText/invitation/data/doexport.aspx/title")%></span>
            </div>
            <div class="padding10">
                <div id="progressDiv">&nbsp;</div>
                <div id="progressText">&nbsp;</div>
            </div>
        </div>
    </div>
    <asp:Panel ID="_linkPanel" runat="server" style="margin-top:25px;margin-left:25px;">
        <asp:Panel id="_s3ExportDiv" runat="server">
            <div>
                <ckbx:MultiLanguageLabel style="display:none;font-size:14px;" ID="_s3DownloadDesc" runat="server" CssClass="PrezzaNormal" TextId="/pageText/exportResults.aspx/downloadFromS3Desc" />
            </div>
            <div>
                <ckbx:MultiLanguageHyperLink ID="_s3DownloadButton" Style="font-weight:bold;font-size:12px;display:none;" Target="_blank" runat="server" CssClass="PrezzaLink" TextId="/pageText/exportResults.aspx/downloadFromS3" />
            </div>
        </asp:Panel>
        
        <asp:Panel id="_directDlDiv" runat="server">
            <div>
                <ckbx:MultiLanguageLabel ID="_exportReadyLbl" runat="server" CssClass="PrezzaNormal" style="font-size:14px;display:none;" TextId="/pageText/doExport.aspx/exportReady" />
            </div>
            <div style="margin-top:15px;">
                <div id="_dlButtonContainer" style="display:none;">
                    <btn:CheckboxButton id="_dlFileButton" runat="server" CssClass="ckbxButton roundedCorners border999 shadow999 silverButton" TextID="/pageText/doExport.aspx/downloadFile" />
                </div>
            </div>
        </asp:Panel>
    </asp:Panel>
</asp:Content>