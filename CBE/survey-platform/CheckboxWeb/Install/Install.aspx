<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="Install.aspx.cs" Inherits="CheckboxWeb.Install.Install" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
    <head id="Head1" runat="server">
        <title>Checkbox&reg; 5 Installation</title>
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

                //For some reason, forms auth has issues and will report this message a single time
                // after webconfig updated. Refresh makes issue go away.
                if (errorMessage.indexOf('401') >= 0) {
                    onProgressComplete(null);
                }
                else {
                    $('#progressText').append('<div style="margin-top:15px;color:red;font-weight:bold;">' + errorMessage + '</div>');
                }
            }

            //Update status
            function onProgressUpdate(progressData) {
                $('#progressDiv').show();
                $('#progressText').html(progressData.StatusMessage);
            }

            //Do something on progress complete
            function onProgressComplete(progressData) {
                parent.document.location = _redirectUrl; 
            }
        </script>
        
        <%-- Specified script include placeholder --%>
        <asp:PlaceHolder ID="_scriptIncludesPlace" runat="server" />
    </head>
    <body>
        <form id="_installForm" runat="server">
            <div class="padding15">
                <div class="left"><asp:Image ID="_progressSpinner" runat="server" SkinID="ProgressSpinner" /></div>
                <div class="left" style="margin-left:10px;margin-top:5px;">
                    <div class="ProgressText" id="progressText">Preparing to install Checkbox&reg; Survey 6...</div>
                </div>
                <br class="clear" />
                <div id="progressDiv" style="width:120px;height:50px;margin-left:auto;margin-right:auto;display:none;" class="padding15"></div>
                <br class="clear" />
            </div>
        </form>
    </body>
</html>