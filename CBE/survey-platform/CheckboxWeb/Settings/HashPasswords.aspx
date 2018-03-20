<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="HashPasswords.aspx.cs" Inherits="CheckboxWeb.Settings.HashPasswords" MasterPageFile="~/Dialog.Master" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>
<%@ Import Namespace="Checkbox.Web"%>

<asp:Content ID="_head" ContentPlaceHolderID="_headContent" runat="server">
    <ckbx:ResolvingScriptElement runat="server" Source="~/Resources/AjaxProgress.js" />
    <ckbx:ResolvingScriptElement runat="server" Source="~/Resources/jquery.progressbar.min.js" />
    
    <script type="text/javascript">
        /************** PROGRESS *************************/
        var progressStatusSuccess = false;

        //Start progress monitoring
        function startProgress(progressKey) {
            $('#<%= _closeBtn.ClientID %>').hide();
            $('#<%= _doEncrypt.ClientID %>').unbind();
            $('#<%= _doEncrypt.ClientID %>').hide();

            //Start send process
            $.ajax({
                type: "GET",
                url: '<%=ResolveUrl("~/Settings/HashWorker.aspx")%>',
                async: true,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                timeout: 100
            });
            //Set a short timeout to work around issues where load balancers and the like
            // may timeout after 60 seconds, etc.  This essentially makes the call fire and
            // forget instead of waiting for error/success error to return.

            //Start monitoring progress
            ajaxProgress.startProgress(
                progressKey,
                'progressDiv',
                '<%=ResolveUrl("~/") %>',
                onProgressUpdate,
                onProgressError,
                onProgressComplete);
        }

        //Show message on error
        function onProgressError(errorMessage) {
            $('#progressDiv').hide();
            $('#progressText').html('<div class="error message">An error occurred: <span style="font-weight:bold;">&nbsp;&nbsp;' + errorMessage + '</span></div>');
            $('#<%= _closeBtn.ClientID %>').fadeIn(300);
        }

        //Update status
        function onProgressUpdate(progressData) {
            $('#progressDiv').show();
            $('#progressText').html(progressData.StatusMessage);
        }

        //Do something on progress complete
        function onProgressComplete(progressData) {
            $('#hasherDiv').fadeOut('300', function () {
                $('#progressText').empty();
                $('#completedDiv').fadeIn('300');
            });
            setTimeout(function () { $('#<%= _closeBtn.ClientID %>').click(); }, 1500);
        }
    </script>  
</asp:Content>

<asp:Content ID="_content" ContentPlaceHolderID="_pageContent" runat="server">
    <div style="width:90%;height:300px;margin-top:10px;" class="centerContent">
        <div id="hasherDiv">
            <div class="warning message">
                <%=WebTextManager.GetText("/pageText/settings/security.aspx/usePasswordEncryptionDescription") %>
            </div>
            <div style="padding:15px;">
                <div id="progressDiv"></div>
                <div id="progressText"></div>
            </div>
        </div>
        <div id="completedDiv" class="success message" style="display:none;padding:10px;">
            <%= WebTextManager.GetText("/pageText/settings/security.aspx/encryptionCompleted") %>
        </div>
    </div>
    <div class="DialogButtonsContainer centerContent" style="width:50%;">
        <btn:CheckboxButton ID="_doEncrypt" runat="server" TextID="/common/yes" CssClass="ckbxButton roundedCorners shadow999 border999 orangeButton left" />
        <btn:CheckboxButton ID="_closeBtn" runat="server" TextID="/common/close" CssClass="ckbxButton roundedCorners border999 shadow999 redButton right" OnClientClick="closeWindow(refreshParentPage);return false;" />
        <br class="clear" />
    </div>
</asp:Content>