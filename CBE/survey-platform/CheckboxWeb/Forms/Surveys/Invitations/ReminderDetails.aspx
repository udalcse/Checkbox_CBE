<%@ Page Language="C#" MasterPageFile="~/Dialog.Master" AutoEventWireup="false" CodeBehind="ReminderDetails.aspx.cs" EnableEventValidation="false" ValidateRequest="false" Inherits="CheckboxWeb.Forms.Surveys.Invitations.ReminderDetails" %>
<%@ MasterType VirtualPath="~/Dialog.master" %>
<%@ Import Namespace="Checkbox.Management" %>
<%@ Register TagPrefix="ckbx" TagName="Grid" Src="~/Controls/Grid.ascx" %>

<asp:Content ID="head" ContentPlaceHolderID="_headContent" runat="server">
    <script type="text/javascript">
        
        $(document).ready(function(){
            $(document).on('click', '.invitationResponse', function(){
                window.open(
                    '<%=ResolveUrl("~/Forms/Surveys/Responses/View.aspx") %>?ResponseGuid=' + $(this).attr('responseGuid'),
                    'ViewResponse_' + $(this).attr('responseGuid'));
                    
            });
        });
        
        function loadResponseList(currentPage, sortField, sortAscending, loadCompleteCallback, loadCompleteArgs ){
            svcInvitationManagement.listInvitationResponses(
                _at, 
                <%=Invitation.ID %>,
                {
                    pageNumber: currentPage,
                    pageSize: 10,
                    filterField: '',
                    filterValue: '',
                    sortField: sortField,
                    sortAscending: sortAscending
                }, 
                loadCompleteCallback,
                loadCompleteArgs
            );       
        }
    </script>
</asp:Content>
<asp:Content ID="page" ContentPlaceHolderID="_pageContent" runat="server">
    <div class="padding10 ckbxResponsesGrid"  style="width:875px;">
        <ckbx:Grid ID="_responsesGrid" runat="server" />
    </div>
    <div class="left padding10">
        <btn:CheckboxButton ID="_backButton" runat="server" CssClass="ckbxButton roundedCorners border999 shadow999 redButton" />
    </div>
</asp:Content>
