<%@ Page Title="" Language="C#" MasterPageFile="~/DetailList.Master" AutoEventWireup="false" CodeBehind="Search.aspx.cs" Inherits="CheckboxWeb.Search" %>
<%@ MasterType VirtualPath="~/DetailList.Master" %>
<%@ Register Assembly="CheckboxWeb" Namespace="CheckboxWeb.Controls.Button" TagPrefix="btn" %>
<%@ Register TagPrefix="ckbx" TagName="SurveyResults" Src="~/Controls/Search/SurveySearchResults.ascx" %>
<%@ Register TagPrefix="ckbx" TagName="ResponseResults" Src="~/Controls/Search/ResponsesSearchResults.ascx" %>
<%@ Register TagPrefix="ckbx" TagName="UserResults" Src="~/Controls/Search/UsersSearchResults.ascx" %>
<%@ Register TagPrefix="ckbx" TagName="ReportResults" Src="~/Controls/Search/ReportsSearchResults.ascx" %>
<%@ Register TagPrefix="ckbx" TagName="GroupResults" Src="~/Controls/Search/UserGroupsSearchResults.ascx" %>
<%@ Register TagPrefix="ckbx" TagName="InvitationResults" Src="~/Controls/Search/InvitationsSearchResults.ascx" %>

<asp:Content ID="head" ContentPlaceHolderID="_head" runat="server">
    <script type="text/javascript" language="javascript">
        $(document).ready(function () {
            $('#searchTermContainer').keypress(
                function (e) {
                    if (e.which == 13) {
                        onSearchClick();
                        return false;
                    }
                }
            );
        });

        $(document).ready(function(){
            $(document).on('mouseenter', '.gridContent', function () {
                $(this).addClass('gridHoverNoArrow');
            });
            $(document).on('mouseleave', '.gridContent', function () {
                $(this).removeClass('gridHoverNoArrow');
            });
            //$('#<%=Master.RightPaneClientId %>').hide();
        });
        
        //
        function startSearch(term, area) {
            //do nothing if term is empty
            if (term == null || term.replace(' ', '') == '') {
                return;
            }
            //Hide search areas so they can be explicitly shown
            $('.searchResultsContainer').hide();

            if (area == 'everywhere') {
                //Show search areas

                //Trigger searches
                <%if(CanListSurveys) {%>
                    $('#surveyResultsContainer').show();
                    <%=_surveySearchResults.ClientSearchMethod%>(term);
                <% } %>

                <%if(CanViewResponses) {%>
                    $('#responseResultsContainer').show();
                    <%=_responseSearchResults.ClientSearchMethod %>(term);
                <% } %>

                <%if(CanViewUsers) {%>
                    $('#userResultsContainer').show();
                    <%=_userSearchResults.ClientSearchMethod %>(term);
                <% } %>

                <%if(CanViewReports) {%>
                    $('#reportResultsContainer').show();
                    <%=_reportSearchResults.ClientSearchMethod %>(term);
                <% } %>

                <%if(CanViewUsers) {%>
                    $('#groupResultsContainer').show();
                    <%=_groupSearchResults.ClientSearchMethod %>(term);
                <% } %>

                <%if(CanViewInvitations) {%>
                    $('#invitationResultsContainer').show();
                    <%=_invitationSearchResults.ClientSearchMethod %>(term);
                <% } %>

            }

            if(area == 'surveys'){
                <% if(CanListSurveys) {%>
                    $('#surveyResultsContainer').show();
                    <%=_surveySearchResults.ClientSearchMethod %>(term);
                <% } %>
            }

              if(area == 'responses'){
                <% if(CanViewResponses) {%>
                    $('#responseResultsContainer').show();
                    <%=_responseSearchResults.ClientSearchMethod %>(term);
                <% } %>
            }

            if(area == 'users'){
                <% if(CanViewUsers) {%>
                    $('#userResultsContainer').show();
                    <%=_userSearchResults.ClientSearchMethod %>(term);
                <% } %>
            }

              if(area == 'reports'){
                <% if(CanViewReports) {%>
                    $('#reportResultsContainer').show();
                    <%=_reportSearchResults.ClientSearchMethod %>(term);
                <% } %>
            }

              if(area == 'usergroups'){
                <% if(CanViewUsers) {%>
                    $('#groupResultsContainer').show();
                    <%=_groupSearchResults.ClientSearchMethod %>(term);
                <% } %>
            }

            
              if(area == 'invitations'){
                <% if(CanViewInvitations) {%>
                    $('#invitationResultsContainer').show();
                    <%=_invitationSearchResults.ClientSearchMethod %>(term);
                <% } %>
            }
        }

        //
        function onSearchClick(){
            var searchVal = htmlEncode($('#<%=_searchText.ClientID %>').val());
            startSearch(searchVal, $('#<%=_searchAreaList.ClientID %>').val());
        }
    </script>
</asp:Content>

<asp:Content ID="titleLinks" runat="server" ContentPlaceHolderID="_titleLinks">
    <div id="searchTermContainer" style="padding: 10px 0;">
        <div class="left" style="font-size:20px;">
            <label for="<%=_searchText.ClientID %>">Search for:</label>
            <asp:TextBox ID="_searchText" runat="server" size="16" style="font-size:20px;" />
            
            <label for="<%=_searchAreaList.ClientID %>">in</label>
            <ckbx:MultiLanguageDropDownList ID="_searchAreaList" runat="server" style="font-size:20px;">
                <asp:ListItem Value="surveys" TextId="/pageText/search.aspx/surveys" />
                <asp:ListItem Value="responses" TextId="/pageText/search.aspx/responses" />
                <asp:ListItem Value="users" TextId="/pageText/search.aspx/users" />
                <asp:ListItem Value="reports" TextId="/pageText/search.aspx/reports" />
                <asp:ListItem Value="invitations" TextId="/pageText/search.aspx/invitations" />
                <asp:ListItem Value="usergroups" TextId="/pageText/search.aspx/userGroups" />
                <asp:ListItem Value="everywhere" TextId="/pageText/search.aspx/everywhere" />
            </ckbx:MultiLanguageDropDownList>

            <%-- 
                Do we need these to search?  I can't recall any customer having so many of the following items
                that a search is required.
                
                <asp:ListItem Value="emaillists" TextId="/pageText/search.aspx/emailLists" />
                <asp:ListItem Value="styletemplates" TextId="/pageText/search.aspx/styleTemplates" />
                <asp:ListItem Value="itemlibraries" TextId="/pageText/search.aspx/itemLibraries" /> 
            
            --%>
            <btn:CheckboxButton ID="_searchBtn" runat="server" OnClientClick="onSearchClick();return false;" CssClass="ckbxButton roundedCorners border999 shadow999 orangeButton" TextID="/pageText/search.aspx/search" />
        </div>
        <br class="clear" />
    </div>
</asp:Content>

<asp:Content ID="leftContent" runat="server" ContentPlaceHolderID="_leftContent">
    <div style="width:525px;">
        <div class="dashStatsWrapper border999 shadow999" id="surveyResultsContainer" style="display:none;">
            <ckbx:SurveyResults ID="_surveySearchResults" runat="server" DashContainer="miniDashContent" StatusContainer="_statusPanel" />
        </div>
        <div class="dashStatsWrapper border999 shadow999" id="responseResultsContainer" style="display:none;">
            <ckbx:ResponseResults ID="_responseSearchResults" runat="server" DashContainer="miniDashContent" StatusContainer="_statusPanel" />
        </div>
        <div class="dashStatsWrapper border999 shadow999" id="userResultsContainer" style="display:none;">
            <ckbx:UserResults ID="_userSearchResults" runat="server" DashContainer="miniDashContent" StatusContainer="_statusPanel"  />
        </div>
        <div class="dashStatsWrapper border999 shadow999" id="reportResultsContainer" style="display:none;">
            <ckbx:ReportResults ID="_reportSearchResults" runat="server" DashContainer="miniDashContent" StatusContainer="_statusPanel"  />
        </div>
        <div class="dashStatsWrapper border999 shadow999" id="groupResultsContainer" style="display:none;">
            <ckbx:GroupResults ID="_groupSearchResults" runat="server" DashContainer="miniDashContent" StatusContainer="_statusPanel"  />
        </div>
        <div class="dashStatsWrapper border999 shadow999" id="invitationResultsContainer" style="display:none;">
            <ckbx:InvitationResults ID="_invitationSearchResults" runat="server" DashContainer="miniDashContent" StatusContainer="_statusPanel"  />
        </div>
    </div>
</asp:Content>

<asp:Content ID="rightContent" runat="server" ContentPlaceHolderID="_rightContent">
    <div id="miniDashContent" class="padding10"></div>
</asp:Content>
