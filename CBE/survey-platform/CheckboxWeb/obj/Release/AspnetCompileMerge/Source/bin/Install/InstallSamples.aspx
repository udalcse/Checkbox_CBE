<%@ Page Language="C#" MasterPageFile="~/Install/Install.Master" AutoEventWireup="false" CodeBehind="InstallSamples.aspx.cs" Inherits="CheckboxWeb.Install.InstallSamples" %>

<asp:Content ID="script" ContentPlaceHolderID="scriptContent" runat="server">
<script type="text/javascript">
    var _redirectUrl;

    //
    function setRedirectUrl(url) {
        _redirectUrl = url;
    }

    //Show message on error
    function onProgressError(errorMessage) {
        //Since install writes to web.config and cache factories are not set up yet, app pool will recycle, so treat
        // missing progress key as success, since app-based cache will be cleared on app recycle
        if (errorMessage.indexOf('No progress data found for specified key') >= 0) {
            onProgressComplete('');
            return;
        }

        $('#progressText').append('<div style="margin-top:15px;color:red;font-weight:bold;">' + errorMessage + '</div>');
    }

    //Update status
    function onProgressUpdate(progressData) {
        $('#progressDiv').show();
        $('#progressText').html(progressData.StatusMessage);
    }

    //Do something on progress complete
    function onProgressComplete(progressData) {
        document.location = _redirectUrl;
    }
    </script>
</asp:Content>

<asp:Content ID="content" ContentPlaceHolderID="ContentPlace" runat="server">
    <div class="clear" style="margin-top:25px;"></div>
    <div class="grid_12">
         <div style="margin-left:25px;margin-top:25px;">
            <div style="float:left;"><asp:Image ID="_progressSpinner" runat="server" SkinID="ProgressSpinner" /></div>
            <div style="float:left;margin-left:10px;margin-top:5px;">
                <div class="ProgressText" id="progressText">Preparing to install Checkbox&reg; Survey 5.0 Samples...</div>
            </div>
            <div style="clear:both;"></div>
        </div>
    </div>
</asp:Content>
