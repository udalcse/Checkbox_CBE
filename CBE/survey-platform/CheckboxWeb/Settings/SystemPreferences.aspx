<%@ Page Title="" Language="C#" MasterPageFile="~/Dialog.Master" CodeBehind="SystemPreferences.aspx.cs" Inherits="CheckboxWeb.Settings.SystemPreferences" %>
<%@ Import Namespace="Checkbox.Web"%>
<%@ MasterType VirtualPath="~/Dialog.Master" %>

<asp:Content ID="settingContent" ContentPlaceHolderID="_pageContent" runat="server">
<script language="javascript" type="text/javascript">
    $(document).ready(function () {
        $('#<%=_customerUpdateHeader.ClientID %>').change(function () {
            if (this.checked) {
                $('#header_messages').show();
                if (!$('#header_messages').length) {
                    $('#customerUpdateHeaderContainer').append("<div class='warning' id='customerUpdateHeaderWarning'>Customer Update Header will be shown after master page reload</div>");
                    setTimeout(function () { $('#customerUpdateHeaderWarning').hide('slow').delay(2000).remove(); }, 5000);
                }
            }
            else {
                $('#header_messages').hide();
            }
        });
    });
</script>
    <h3><%= WebTextManager.GetText("/pageText/settings/navigation.ascx/systemPreferences")%></h3>

    <div class="dashStatsWrapper border999 shadow999">
        <div class="dashStatsHeader">
            <span class="mainStats left"><%= WebTextManager.GetText("/pageText/settings/systemPreferences.aspx/editorOptions") %></span>
        </div>
        <div class="dashStatsContent">
            <div class="input">
                <ckbx:MultiLanguageCheckBox id="_useHtmlEditor" runat="server" TextId="/pageText/settings/systemPreferences.aspx/useHtmlEditor" />
            </div>
            <div class="input">
                <ckbx:MultiLanguageCheckBox id="_useDatePicker" runat="server" TextId="/pageText/settings/systemPreferences.aspx/useDatePicker" />
            </div>
            <div class="input">
                <ckbx:MultiLanguageCheckBox id="_storeImagesInDatabase" runat="server" TextId="/pageText/settings/systemPreferences.aspx/storeImagesInDatabase" />
            </div>
        </div>
    </div>

    <div class="dashStatsWrapper border999 shadow999">
        <div class="dashStatsHeader">
            <span class="mainStats left"><%= WebTextManager.GetText("/pageText/settings/systemPreferences.aspx/navigationrOptions") %></span>
        </div>
        <div class="dashStatsContent allMenu">
            <div class="input">
                <ckbx:MultiLanguageCheckBox id="_showNavigationLinksWhenNotLoggedIn" runat="server" TextId="/pageText/settings/systemPreferences.aspx/showNavigationLinksWhenNotLoggedIn" />
            </div>
            <div class="input">
                <ckbx:MultiLanguageCheckBox id="_showAvailableSurveysList" runat="server" TextId="/pageText/settings/systemPreferences.aspx/showAvailableSurveysList" />
            </div>
            <div class="input">
                <ckbx:MultiLanguageCheckBox id="_showAvailableReportsList" runat="server" TextId="/pageText/settings/systemPreferences.aspx/showAvailableReportsList" />
            </div>
        </div>
    </div>
    
     <div class="dashStatsWrapper border999 shadow999">
        <div class="dashStatsHeader">
            <span class="mainStats left">Display Options</span>
        </div>
        <div class="dashStatsContent allMenu">
            <div class="input">
                <ckbx:MultiLanguageCheckBox ID="_showCreatedByForFolders" runat="server" TextId="/pageText/settings/systemPreferences.aspx/showUserCreatedFolder" />
            </div>
            <div class="input">
                <ckbx:MultiLanguageCheckBox ID="_displayMachineName" runat="server" TextId="/pageText/settings/systemPreferences.aspx/displayMachineName"/>
            </div>
        </div>
     </div>

    <div class="dashStatsWrapper border999 shadow999">
        <div class="dashStatsHeader">
            <span class="mainStats left"><%= WebTextManager.GetText("/pageText/settings/systemPreferences.aspx/globalOptions")%></span>
        </div>
        <div class="dashStatsContent allMenu">
            <div class="input">
                <ckbx:MultiLanguageCheckBox id="_enableEmailAddressValidation" runat="server" TextId="/pageText/settings/systemPreferences.aspx/enableEmailAddressValidation" />
            </div>
            <div class="input">
                <ckbx:MultiLanguageCheckBox id="_displayProductTour" runat="server" TextId="/pageText/settings/systemPreferences.aspx/displayProductTour" />
            </div>
            <div class="input">
                <ckbx:MultiLanguageLabel ID="_timeZoneLbl" runat="server" TextId="/pageText/settings/systemPreferences.aspx/timeZone" />
                <ckbx:MultiLanguageDropDownList ID="_timeZone" runat="server">
                    <asp:ListItem TextId="/pageText/settings/systemPreferences.aspx/gmt-12" Value="-12" />
                    <asp:ListItem TextId="/pageText/settings/systemPreferences.aspx/gmt-11" Value="-11" />
                    <asp:ListItem TextId="/pageText/settings/systemPreferences.aspx/gmt-10" Value="-10" />
                    <asp:ListItem TextId="/pageText/settings/systemPreferences.aspx/gmt-9" Value="-9" />
                    <asp:ListItem TextId="/pageText/settings/systemPreferences.aspx/gmt-8" Value="-8" />
                    <asp:ListItem TextId="/pageText/settings/systemPreferences.aspx/gmt-7" Value="-7" />
                    <asp:ListItem TextId="/pageText/settings/systemPreferences.aspx/gmt-6" Value="-6" />
                    <asp:ListItem TextId="/pageText/settings/systemPreferences.aspx/gmt-5" Value="-5" />
                    <asp:ListItem TextId="/pageText/settings/systemPreferences.aspx/gmt-4.5" Value="-4.5" />
                    <asp:ListItem TextId="/pageText/settings/systemPreferences.aspx/gmt-4" Value="-4" />
                    <asp:ListItem TextId="/pageText/settings/systemPreferences.aspx/gmt-3.5" Value="-3.5" />
                    <asp:ListItem TextId="/pageText/settings/systemPreferences.aspx/gmt-3" Value="-3" />
                    <asp:ListItem TextId="/pageText/settings/systemPreferences.aspx/gmt-2" Value="-2" />
                    <asp:ListItem TextId="/pageText/settings/systemPreferences.aspx/gmt-1" Value="-1" />
                    <asp:ListItem TextId="/pageText/settings/systemPreferences.aspx/gmt+0" Value="0" />
                    <asp:ListItem TextId="/pageText/settings/systemPreferences.aspx/gmt+1" Value="1" />
                    <asp:ListItem TextId="/pageText/settings/systemPreferences.aspx/gmt+2" Value="2" />
                    <asp:ListItem TextId="/pageText/settings/systemPreferences.aspx/gmt+3" Value="3" />
                    <asp:ListItem TextId="/pageText/settings/systemPreferences.aspx/gmt+3.5" Value="3.5" />
                    <asp:ListItem TextId="/pageText/settings/systemPreferences.aspx/gmt+4" Value="4" />
                    <asp:ListItem TextId="/pageText/settings/systemPreferences.aspx/gmt+4.5" Value="4.5" />
                    <asp:ListItem TextId="/pageText/settings/systemPreferences.aspx/gmt+5" Value="5" />
                    <asp:ListItem TextId="/pageText/settings/systemPreferences.aspx/gmt+5.5" Value="5.5" />
                    <asp:ListItem TextId="/pageText/settings/systemPreferences.aspx/gmt+5.75" Value="5.75" />
                    <asp:ListItem TextId="/pageText/settings/systemPreferences.aspx/gmt+6" Value="6" />
                    <asp:ListItem TextId="/pageText/settings/systemPreferences.aspx/gmt+6.5" Value="6.5" />
                    <asp:ListItem TextId="/pageText/settings/systemPreferences.aspx/gmt+7" Value="7" />
                    <asp:ListItem TextId="/pageText/settings/systemPreferences.aspx/gmt+8" Value="8" />
                    <asp:ListItem TextId="/pageText/settings/systemPreferences.aspx/gmt+9" Value="9" />
                    <asp:ListItem TextId="/pageText/settings/systemPreferences.aspx/gmt+9.5" Value="9.5" />
                    <asp:ListItem TextId="/pageText/settings/systemPreferences.aspx/gmt+10" Value="10" />
                    <asp:ListItem TextId="/pageText/settings/systemPreferences.aspx/gmt+11" Value="11" />
                    <asp:ListItem TextId="/pageText/settings/systemPreferences.aspx/gmt+12" Value="12" />
                    <asp:ListItem TextId="/pageText/settings/systemPreferences.aspx/gmt+13" Value="13" />
                </ckbx:MultiLanguageDropDownList>
            </div>
        </div>
    </div>

     <div class="dashStatsWrapper border999 shadow999">
        <div class="dashStatsHeader">
            <span class="mainStats left">Notifications</span>
        </div>
        <div class="dashStatsContent allMenu" id="customerUpdateHeaderContainer">
            <div class="input">
                <ckbx:MultiLanguageCheckBox ID="_customerUpdateHeader" runat="server" TextId="/pageText/settings/systemPreferences.aspx/showCustomerUpdateHeader" />
            </div>
        </div>
     </div>

    <div class="dialogFormPush">&nbsp;</div>
</asp:Content>