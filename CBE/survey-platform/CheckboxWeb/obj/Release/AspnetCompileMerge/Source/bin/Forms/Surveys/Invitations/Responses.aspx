<%@ Page Language="C#" MasterPageFile="~/Dialog.Master" AutoEventWireup="false" CodeBehind="Responses.aspx.cs" Inherits="CheckboxWeb.Forms.Surveys.Invitations.Responses" %>
<%@ Import Namespace="Checkbox.Management" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>
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
                    pageSize: <%= ApplicationManager.AppSettings.PagingResultsPerPage %>,
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
    <div class="padding10">
        <ckbx:Grid ID="_responsesGrid" runat="server" GridCssClass="ckbxResponsesGrid" />
    </div>
</asp:Content>
