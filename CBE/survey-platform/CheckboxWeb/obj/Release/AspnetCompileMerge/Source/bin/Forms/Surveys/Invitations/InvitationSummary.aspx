<%@ Page Language="C#" MasterPageFile="~/Dialog.Master" AutoEventWireup="false" CodeBehind="InvitationSummary.aspx.cs" Inherits="CheckboxWeb.Forms.Surveys.Invitations.InvitationSummary" EnableEventValidation="false" ValidateRequest="false" IncludeJsLocalization="true" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Import Namespace="Checkbox.Messaging.Email" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>
<%@ Register TagName="MessageEditor" TagPrefix="ckbx" Src="~/Forms/Surveys/Invitations/Controls/EditMessageControl.ascx" %>
<%@ Register TagName="RecipientEditor" TagPrefix="ckbx" Src="~/Forms/Surveys/Invitations/Controls/Recipients.ascx" %>
<%@ Register TagName="AddRecipients" TagPrefix="ckbx" Src="~/Forms/Surveys/Invitations/Controls/AddRecipients.ascx" %>
<%@ Register TagName="InvitationProperties" TagPrefix="ckbx" Src="~/Forms/Surveys/Invitations/Controls/Properties.ascx" %>
<%@ Register TagName="SendInvitation" TagPrefix="ckbx" Src="~/Forms/Surveys/Invitations/Controls/Send.ascx" %>
<%@ Register TagName="Schedule" TagPrefix="ckbx" Src="~/Forms/Surveys/Invitations/Controls/Schedule.ascx" %>
<%@ Register TagName="CopyInvitation" TagPrefix="ckbx" Src="~/Forms/Surveys/Invitations/Controls/CopyInvitation.ascx" %>


<asp:Content ID="head" ContentPlaceHolderID="_headContent" runat="server">

     <script type="text/javascript">
        $(document).ready(function () {
            $('#invitationSummaryTabs').ckbxTabs({
                tabName: 'invitationSummaryTabs',
                initialTabIndex: <%=_currentTabIndex.Text %>,
                onTabClick: function(index){ onTabChange(index); },
                onTabsLoaded: function(){$('.tabContainer').show();$('.tabContentContainer').show();}
            });
        });

        //
        function onTabChange(index) {
            $('#<%=_currentTabIndex.ClientID %>').val(index);

            //Show loading until div reloads
            $('#summaryContent').html($('#summaryProgressContainer').html());

            //Do postback to save values
            eval($("#<%=_updateBtn.ClientID %>").attr('href'));
        }
      
    </script>
</asp:Content>

<asp:Content ID="page" ContentPlaceHolderID="_pageContent" runat="server">
     <div style="display:none;">
        <asp:TextBox ID="_currentTabIndex" runat="server" Text="0" />
        <asp:LinkButton ID="_updateBtn" runat="server"  CausesValidation="false" />
    </div>

    <%-- Loading Div for Simple --%>
    <div id="summaryProgressContainer" style="display:none;">
        <div style="text-align:center;background-color:white;border-style:double;border-width:3px;width:250px;margin-top:15px;margin-left:auto;margin-right:auto;border-color:#DEDEDE;">
            <p><%=WebTextManager.GetText("/common/loading")%></p>
            <p>
                <asp:Image ID="_progressSpinnerSimple" runat="server" SkinId="ProgressSpinner" />
            </p>
        </div>
    </div>


    <ul id="invitationSummaryTabs" class="tabContainer">
        <li><%=WebTextManager.GetText("/pageText/forms/surveys/invitations/invitationPropertiesPage.cs/overview")%></li>
        <li><%=WebTextManager.GetText("/pageText/forms/surveys/invitations/invitationPropertiesPage.cs/recipientsTab")%></li>
        <% if (!GetCurrentInvitation().InvitationLocked)
           { %>
        <li><%=WebTextManager.GetText("/pageText/forms/surveys/invitations/invitationPropertiesPage.cs/inviteAdditional")%></li>
        <% } %>
        <% if (!GetCurrentInvitation().LastSent.HasValue) { %>
            <li><%= WebTextManager.GetText("/pageText/forms/surveys/invitations/invitationPropertiesPage.cs/messageTab")%></li>
            <li><%= WebTextManager.GetText("/pageText/forms/surveys/invitations/invitationPropertiesPage.cs/propertiesTab")%></li>
            <li class="green"><%= WebTextManager.GetText("/pageText/forms/surveys/invitations/invitationPropertiesPage.cs/sendInvite")%></li>
        <% } %>
        <li><%=WebTextManager.GetText("/pageText/forms/surveys/invitations/invitationPropertiesPage.cs/copyInvitation")%></li>
    </ul>
    <div class="clear"></div>
    <div style="height:550px;" id="summaryContent">
         <div class="tabContentContainer padding10">
            <div id="invitationSummaryTabs-0-tabContent">
                <ckbx:Schedule ID="_schedule" runat="server" />
            </div>
            <div id="invitationSummaryTabs-1-tabContent">
                <ckbx:RecipientEditor ID="_recipientEditor" runat="server" />
            </div>
            <% if (!GetCurrentInvitation().InvitationLocked)
               { %>
                <div id="invitationSummaryTabs-2-tabContent">
                    <ckbx:AddRecipients ID="_addRecipients" runat="server" />
                </div>
            <% } %>
            <% if (!GetCurrentInvitation().LastSent.HasValue) { %>
                <div id="invitationSummaryTabs-<%=GetCurrentInvitation().Locked?"2":"3"%>-tabContent">
                    <ckbx:MessageEditor ID="_messageEditor" runat="server" />
                </div>        
                <div id="invitationSummaryTabs-<%=GetCurrentInvitation().Locked?"3":"4"%>-tabContent">
                    <ckbx:InvitationProperties id="_propertiesEditor" runat="server" />
                </div>
                <div id="invitationSummaryTabs-<%=GetCurrentInvitation().Locked?"4":"5"%>-tabContent">
                    <ckbx:SendInvitation ID="_sendInvitation" runat="server" />
                </div>
            <% } %>
            <div id="invitationSummaryTabs-<%=(2 + (GetCurrentInvitation().Locked ? 0 : 1) + (GetCurrentInvitation().LastSent.HasValue ? 0 : 3)).ToString()%>-tabContent">
                <ckbx:CopyInvitation ID="_copyInvitation" runat="server" />
            </div>
        </div>
    </div>
</asp:Content>

