<%@ Page Title="" Language="C#" MasterPageFile="~/Dialog.Master" AutoEventWireup="false" CodeBehind="Recipients.aspx.cs" Inherits="CheckboxWeb.Forms.Surveys.Invitations.Recipients" %>
<%@ Import Namespace="Checkbox.Globalization.Text" %>
<%@ MasterType VirtualPath="~/Dialog.master" %>
<%@ Register Src="~/Forms/Surveys/Invitations/Controls/Recipients.ascx" TagName="Recipients" TagPrefix="ckbx" %>

<asp:Content runat="server"  ContentPlaceHolderID="_headContent">
    <script type="text/javascript">
        $(function () {
            $(document).on("click", ".opt-out-details-link a", function () {
                $(".recipient-list-wrapper").hide();
                $("#details-loader").show();

                svcInvitationManagement.getEmailOptOutDetails(_at, $(this).attr('data-email'), $(this).attr('data-surveyid'), '<%=Invitation.ID %>', function (data) { onOptedOutDetailsLoaded(data); });
            });
            
            $("#details-goback a").click(function () {
                $("#details-loader, #opted-out-details").hide();
                $(".recipient-list-wrapper").show();
            });

            function onOptedOutDetailsLoaded(data) {
                if (data == null) {
                    $("#details-loader, #opted-out-details").hide();
                    $(".recipient-list-wrapper").show();
                    alert('<%= TextManager.GetText("/pageText/forms/surveys/invitations/recipients.aspx/noOptData") %>');
                } else {
                    templateHelper.loadAndApplyTemplate(
                        'optedOutInvitationDetails.html',
                        '<%=ResolveUrl("~/Forms/Surveys/Invitations/jqtmpl/optedOutInvitationDetails.html") %>',
                        data,
                        null,
                        'details-content',
                        true,
                        function () {
                            $('#details-loader').hide();
                            $('#opted-out-details').show();
                        }
                    );
                }
            }
        });
    </script>
</asp:Content>

<asp:Content ID="content" runat="server" ContentPlaceHolderID="_pageContent">
    <div class="recipient-list-wrapper">
        <ckbx:Recipients ID="_recipients" PendingMode="Ungrouped" runat="server" />
    </div>
    
    <div id="details-loader" style="display: none;">
        <img src="../../../App_Themes/CheckboxTheme/Images/ProgressSpinner.gif"
             style="text-align: center; vertical-align: middle;">        
    </div>
    
    <div id="opted-out-details" style="display: none;">
        <div id="details-content"></div>
        <div id="details-goback" class="opted-out-details-text" ><a href="#">Back</a></div>
    </div>
</asp:Content>


