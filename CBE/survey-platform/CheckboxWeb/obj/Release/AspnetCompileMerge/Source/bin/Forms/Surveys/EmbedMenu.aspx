<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EmbedMenu.aspx.cs" Inherits="CheckboxWeb.Forms.Surveys.EmbedMenu" MasterPageFile="~/Dialog.Master" %>
<%@ Import Namespace="Checkbox.Management" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>

<asp:Content ID="_headerContent" ContentPlaceHolderID="_headContent" runat="server">
        <script type="text/javascript">
            $(function() {
                <% if(ApplicationManager.AppSettings.AllowSurveyUrlRewriting) { %>
                $('.urlSwitcher').on('change', function() {
                    if ($(this).prop('checked')) {
                        if ($(this).val() == 'customUrl') {
                            $('#<%= _htmlCode.ClientID %>').hide();
                            $('#<%= _customUrlhtmlCode.ClientID %>').show();
                        } else {
                            $('#<%= _customUrlhtmlCode.ClientID %>').hide();
                            $('#<%= _htmlCode.ClientID %>').show();
                        }
                    }
                });
                <% } %>
                $('.clickToSelect').on('click', function() {
                    if (typeof this.select === 'function') {
                        this.select();
                    }
                });
            });
        </script>
</asp:Content>

<asp:Content ID="_content" ContentPlaceHolderID="_pageContent" runat="server">
    <div id="_embedPanel" class="padding15">
        <% if (IsProtectedReport) { %>
            <div class="padding10">
                <div class="StatusPanel warning">
                    <span><%= WebTextManager.GetText("/pageText/forms/surveys/embedMenu.aspx/reportWarning")%></span>
                </div>
            </div>
        <% } %>
        
        <div class="dialogInstructions"><%= DialogInstructionsText %></div>

        <div class="dialogSubTitle"><%= WebTextManager.GetText("/pageText/forms/surveys/embedMenu.aspx/links")%></div>
        <input class="clickToSelect" id="_standardUrl" runat="server" type="text" readonly="readonly" style="width:98%;" />
    <% if (ApplicationManager.AppSettings.AllowSurveyUrlRewriting && !string.IsNullOrEmpty(SurveyCustomURLRelative) && !AnalysisTemplateId.HasValue)
       { %>
        <input class="clickToSelect" id="_customUrl" runat="server" type="text" readonly="readonly" style="width:98%;" />
    <% } %>
        <br/>
        <div class="dialogSubTitle"><%= WebTextManager.GetText("/pageText/forms/surveys/embedMenu.aspx/embedCode")%></div>
        <% if(ApplicationManager.AppSettings.AllowSurveyUrlRewriting && !string.IsNullOrEmpty(SurveyCustomURLRelative)) { %>
            <div class="padding10">
                <input class="urlSwitcher" type="radio" name="urlSwitcher" checked="checked" id="defaultUrl" value="defaultUrl" />
                <label for="defaultUrl"><%= WebTextManager.GetText("/pageText/forms/surveys/embedMenu.aspx/defaultUrl")%></label>
                <br />
                <input class="urlSwitcher" type="radio" name="urlSwitcher" id="customUrl" value="customUrl" />
                <label for="customUrl"><%= WebTextManager.GetText("/pageText/forms/surveys/embedMenu.aspx/customUrl")%></label>
            </div>

            <asp:TextBox style="display:none;" ID="_customUrlhtmlCode" runat="server" TextMode="MultiLine" ReadOnly="true" CssClass="roundedCorners embedCode clickToSelect" />            
        <% } %>
        <asp:TextBox ID="_htmlCode" runat="server" TextMode="MultiLine" ReadOnly="true" CssClass="roundedCorners embedCode clickToSelect" />
        <div class="smallDescription"><%=WebTextManager.GetText("") %></div>
    </div>
</asp:Content>

