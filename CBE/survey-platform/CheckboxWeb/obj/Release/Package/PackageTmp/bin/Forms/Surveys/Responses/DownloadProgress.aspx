<%@ Page Language="C#" AutoEventWireup="false" MasterPageFile="~/Dialog.Master" CodeBehind="DownloadProgress.aspx.cs" Inherits="CheckboxWeb.Forms.Surveys.Responses.DownloadProgress" %>
<%@ Register Assembly="CheckboxWeb" Namespace="CheckboxWeb.Controls.Button" TagPrefix="btn" %>
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
            $('#<%=_s3DownloadButton.ClientID %>').show();

            //Direct
            $('#<%=_exportReadyLbl.ClientID %>').show();
            $('#_dlButtonContainer').show();
        }
    </script>
</asp:Content>

<asp:Content ID="content" ContentPlaceHolderID="_pageContent" runat="server">
    <div style="margin-left:25px;margin-top:25px;">
        <div style="margin-bottom:15px;">
            <ckbx:MultiLanguageLabel ID="_queueingLbl" runat="server" CssClass="PrezzaLabel" TextId="/pageText/downloadFiles.aspx/generatingZip" style="font-size:14px;">Generating zip file.  Please do not navigate away from this page.</ckbx:MultiLanguageLabel>
        </div>
        <div id="progressDiv"></div>
        <div class="ProgressText" id="progressText"></div>
    </div>
    
    <asp:Panel ID="_linkPanel" runat="server" style="margin-top:25px;margin-left:25px;">
        <div>
            <ckbx:MultiLanguageLabel ID="_exportReadyLbl" runat="server" CssClass="PrezzaNormal" style="font-size:14px;display:none;" TextId="/pageText/downloadFiles.aspx/fileReady" />
        </div>
        <asp:Panel id="_s3ExportDiv" runat="server" style="margin-top:15px;">
            <div>
                <ckbx:MultiLanguageHyperLink ID="_s3DownloadButton" Style="font-weight:bold;font-size:12px;display:none;" Target="_blank" runat="server" CssClass="PrezzaLink" TextId="/pageText/exportResults.aspx/downloadFromS3" />
            </div>
        </asp:Panel>
        
        <asp:Panel id="_directDlDiv" runat="server">
            <div style="margin-top:15px;">
                <div id="_dlButtonContainer" style="display:none;">
                    <btn:CheckboxButton id="_dlFileButton" runat="server" CssClass="ckbxButton roundedCorners border999 shadow999 silverButton" TextID="/pageText/downloadFiles.aspx/downloadFile" />
                </div>
            </div>
        </asp:Panel>
        <div id="returnDiv" style="margin-top:25px;display:none;">
            <ckbx:MultiLanguageHyperLink ID="_backToExportLink" runat="server" CssClass="PrezzaLink" TextId="/pageText/downloadFiles.aspx/clickHere" />
            <ckbx:MultiLanguageLabel ID="_toReturnLbl" runat="server" CssClass="PrezzaNormal" TextId="/pageText/downloadFiles.aspx/toReturn" />
        </div>
    </asp:Panel>
</asp:Content>