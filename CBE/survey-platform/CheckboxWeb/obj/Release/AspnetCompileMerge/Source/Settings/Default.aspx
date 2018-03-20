<%@ Page  Language="C#" MasterPageFile="~/DetailList.master" AutoEventWireup="false" CodeBehind="Default.aspx.cs" Inherits="CheckboxWeb.Settings.Default" IncludeJsLocalization="true" %>
<%@ Import Namespace="Checkbox.Management" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ MasterType VirtualPath="~/DetailList.master" %>

<asp:Content ID="_head" runat="server" ContentPlaceHolderID="_head">
    <meta http-equiv='Pragma' content='no-cache'/>
    <meta http-equiv='Expires' content='-1'/>

    <style type="text/css">
        .groupContent {display:none;}
    </style>

    <script type="text/javascript">
        $(document).ready(function () {
            $('.groupHeader').click(function () {
                $('.groupContent :visible').parent().hide('fast');

                if (!$(this).hasClass('active')) {
                    var header = $(this);

                    header
                        .siblings('.folderContainer')
                        .show('fast',
                            function () {
                                header
                                    .siblings('.folderContainer')
                                    .children('.groupContent')
                                    .show('fast', function () {
                                        if (typeof (resizePanels) == 'function') {
                                            resizePanels();
                                        }
                                    });
                            });

                    $('.groupHeader').removeClass('active');
                    $(this).addClass('active');
                }
                else
                    $('.groupHeader').removeClass('active');
            });

            $('.groupContent').click(function () {
                showSettingsPage($(this).attr('settingsPage'));
                $('.groupContent').removeClass('activeContent');
                $(this).addClass('activeContent');
            });
            
            //uncertain fix for IE9
            var timeoutID = window.setTimeout(function () {
                $('.leftPanel').show();
                window.clearTimeout(timeoutID);
            }, 100);
        });

        //
        function showSettingsPage(settingsPage, isRefresh, message) {
            if (settingsPage == null
                || settingsPage == '') {
                return;
            }

            var url = '<%=ResolveUrl("~/Settings/")%>' + settingsPage;

            if (isRefresh != null && isRefresh) {
                url += '?isRefresh=true';
            }

            if (message != null) {
                if (url.indexOf('?') > 0)
                    url += '&message=' + message;
                else
                    url += '?message=' + message;
            }

            if (typeof (tinymce) != 'undefined')
                tinymce.remove('textarea');

            UFrameManager.init({id: '_editFrame',loadFrom: url, progressTemplate: $('#detailProgressContainer').html(),showProgress: true});

            //Update preview
            //$('#_editFrame').attr('src', '<%=ResolveUrl("~/Settings/")%>' + settingsPage);
        }

        //
        function onDialogClosed(arg) {
            if (arg == null || arg == 'cancel') {
                return;
            }

            if (arg.dialog != null) {
                showSettingsPage(arg.dialog, true, arg.message);
            }
        }

        function doTextExport(target, argument) {
            UFrameManager.prepareOuterFormSubmit();

            var url = '<%=ResolveUrl("~/Settings/Default.aspx") + "?target=" %>' + target + '&argument=' + argument;

            $.fileDownload(url)
                .fail(function () { alert('File download error!'); });
        }


       var prevFieldType;
       var selectedField;

        $("body").on("focus", ".custom-field-dd", function () {
            prevFieldType = this.value;
        }).on("change", ".custom-field-dd", function () {
            var fieldName = $(this).closest('tr').find('td').eq(0).text().trim();
            selectedField = fieldName;

            if (this.value === "Matrix") {
                var url = "Modal/AddMatrixType.aspx?fieldName=" + fieldName;
                showDialog(url, "xlargeDialog")

            }
            else if (this.value === "RadioButton") {
                var url = "Modal/AddRadioButtonType.aspx?fieldName=" + fieldName;
                showDialog(url, "xlargeDialog")

            } else {
                var fieldType = this.value;
                var data = { 'fieldName': fieldName, 'fieldType': fieldType };
                var url = "CustomUserFields.aspx/UpdateCustomFieldType";
                if (fieldName && fieldType) {
                    $.ajax({
                        type: "POST",
                        contentType: "application/json; charset=utf-8",
                        url: url,
                        data: JSON.stringify(data),
                        datatype: "json",
                        success: function () {
                            if (prevFieldType !== fieldType) {
                                showMessage(fieldName);
                            }
                            prevFieldType = fieldType;
                        },
                        error: function (xmlhttprequest, textstatus, errorthrown) {
                            console.log("error: " + errorthrown);
                        }
                    });
                } else {
                    Console.log("field name or field type has wrong value");
                }
            }
        });

        function showMessage(fieldName) {
            var message = fieldName + ' field type has been updated.';
            var statusPanel = $('.dialogFormContainer .StatusPanel');
            if (statusPanel.length === 0) {
                $('.dialogFormContainer').prepend('<div class="StatusPanel success fixedPosition" style="display: none;">' +
                    '<span>' + message + '</span><div class="clear"> </div></div>');
                $('.StatusPanel.success').fadeIn(200);
            } else {
                $('.dialogFormContainer .StatusPanel span').text(message);
                $('.dialogFormContainer .StatusPanel').fadeIn(200);
            }
            setTimeout(function () {
                $('.dialogFormContainer .StatusPanel.success').fadeOut(500);
            }, 3000);
        }

    </script>
</asp:Content>

<asp:Content ID="_left" runat="server" ContentPlaceHolderID="_leftContent">
    <%-- Survey Settings --%>
    <div class="groupWrapper surveyWrapper pageWrapper settingWrapper">
        <div class="groupHeader">
            <div class="groupName left"><%= WebTextManager.GetText("/pageText/settings/navigation.ascx/surveySettings")%></div>
            <br class="clear" />
        </div>

        <div class="folderContainer">
            <div class="groupContent border999" settingsPage="SurveyPreferences.aspx">
                <div class="groupContentName left"><%= WebTextManager.GetText("/pageText/settings/navigation.ascx/surveyPreferences")%></div>
                <br class="clear" />
            </div>

        <% if (!ApplicationManager.AppSettings.EnableMultiDatabase){  %>
            <div class="groupContent border999" settingsPage="UploadItem.aspx">
                <div class="groupContentName left"><%= WebTextManager.GetText("/pageText/settings/navigation.ascx/uploadItem")%></div>
                <br class="clear" />
            </div>            
        <%} %>
            
        <% if (ApplicationManager.AppSettings.AllowJavascriptItem){  %>
            <div class="groupContent border999" settingsPage="JavascriptItem.aspx">
                <div class="groupContentName left"><%= WebTextManager.GetText("/pageText/settings/navigation.ascx/javascriptItem")%></div>
                <br class="clear" />
            </div>            
        <%} %>
        </div>
    </div>

     <%-- Report Settings --%>
    <div class="groupWrapper surveyWrapper pageWrapper settingWrapper">
        <div class="groupHeader">
            <div class="groupName left"><%= WebTextManager.GetText("/pageText/settings/navigation.ascx/reportSettings")%></div>
            <br class="clear" />
        </div>

        <div class="folderContainer">
            <div class="groupContent border999" settingsPage="ReportPreferences.aspx">
                <div class="groupContentName left"><%= WebTextManager.GetText("/pageText/settings/navigation.ascx/reportPreferences")%></div>
                <br class="clear" />
            </div>

            <div class="groupContent border999" settingsPage="ResponseDetails.aspx">
                <div class="groupContentName left"><%= WebTextManager.GetText("/pageText/settings/navigation.ascx/responseDetails")%></div>
                <br class="clear" />
            </div>

            <div class="groupContent border999" settingsPage="ResponseExport.aspx">
                <div class="groupContentName left"><%= WebTextManager.GetText("/pageText/settings/navigation.ascx/responseExport")%></div>
                <br class="clear" />
            </div>
        </div>
    </div>

     <%-- User Settings --%>
    <div class="groupWrapper surveyWrapper pageWrapper settingWrapper">
        <div class="groupHeader">
            <div class="groupName left"><%= WebTextManager.GetText("/pageText/settings/navigation.ascx/userSettings")%></div>
            <br class="clear" />
        </div>

        <div class="folderContainer">
            <div class="groupContent border999" settingsPage="Users.aspx">
                <div class="groupContentName left"><%= WebTextManager.GetText("/pageText/settings/navigation.ascx/users")%></div>
                <br class="clear" />
            </div>

            <div class="groupContent border999" settingsPage="CustomUserFields.aspx">
                <div class="groupContentName left"><%= WebTextManager.GetText("/pageText/settings/navigation.ascx/customUserFields")%></div>
                <br class="clear" />
            </div>
             <div class="groupContent border999" settingsPage="UsersSecurity.aspx">
                <div class="groupContentName left"><%= WebTextManager.GetText("/pageText/settings/navigation.ascx/userssecurity")%></div>
                <br class="clear" />
            </div>
        </div>
    </div>

     <%-- System Settings --%>
    <div class="groupWrapper surveyWrapper pageWrapper settingWrapper">
        <div class="groupHeader">
            <div class="groupName left"><%= WebTextManager.GetText("/pageText/settings/navigation.ascx/systemSettings")%></div>
            <br class="clear" />
        </div>

        <div class="folderContainer">
        <% if (true){  %> <!-- Here need to add some precondoitions to render or not-->
            <div class="groupContent border999" settingsPage="SystemMode.aspx">
                <div class="groupContentName left">Mode<%--<%= WebTextManager.GetText("/pageText/settings/navigation.ascx/branding")%>--%></div> <!-- Get text from db -->
                <br class="clear" />
            </div>
        <%} %>
       <% if (!ApplicationManager.AppSettings.EnableMultiDatabase){  %>
            <div class="groupContent border999" settingsPage="Branding.aspx">
                <div class="groupContentName left"><%= WebTextManager.GetText("/pageText/settings/navigation.ascx/branding")%></div>
                <br class="clear" />
            </div>
        <%} %>
        <% if (!ApplicationManager.AppSettings.EnableMultiDatabase){  %>
        <div class="groupContent border999" settingsPage="Performance.aspx">
                <div class="groupContentName left"><%= WebTextManager.GetText("/pageText/settings/navigation.ascx/performance")%></div>
                <br class="clear" />
            </div>
        <div class="groupContent border999" settingsPage="SearchSettings.aspx">
                <div class="groupContentName left"><%= WebTextManager.GetText("/pageText/settings/navigation.ascx/search")%></div>
                <br class="clear" />
            </div>
        <%} %>
            <div class="groupContent border999" settingsPage="Security.aspx">
                <div class="groupContentName left"><%= WebTextManager.GetText("/pageText/settings/navigation.ascx/security")%></div>
                <br class="clear" />
            </div>
            <div class="groupContent border999" settingsPage="SystemPreferences.aspx">
                <div class="groupContentName left"><%= WebTextManager.GetText("/pageText/settings/navigation.ascx/systemPreferences")%></div>
                <br class="clear" />
            </div>
        </div>
    </div>

    <div class="groupWrapper surveyWrapper pageWrapper settingWrapper">
        <div class="groupHeader">
            <div class="groupName left"><%= WebTextManager.GetText("/pageText/settings/navigation.ascx/emailSettings")%></div>
            <br class="clear" />
        </div>

        <div class="folderContainer">
       
            <div class="groupContent border999" settingsPage="Email.aspx">
                <div class="groupContentName left"><%= WebTextManager.GetText("/pageText/settings/navigation.ascx/commonSettings")%></div>
                <br class="clear" />
            </div>
        <% if (!ApplicationManager.AppSettings.EnableMultiDatabase){  %>
            <div class="groupContent border999" settingsPage="OptOutPreferences.aspx">
                <div class="groupContentName left"><%= WebTextManager.GetText("/pageText/settings/navigation.ascx/optScreen")%></div>
                <br class="clear" />
            </div>
        <%} %>
            <div class="groupContent border999" settingsPage="Footer.aspx">
                <div class="groupContentName left"><%= WebTextManager.GetText("/pageText/settings/navigation.ascx/footerSettings")%></div>
                <br class="clear" />
            </div>
             <div class="groupContent border999" settingsPage="InvitationText.aspx">
                <div class="groupContentName left">Invitation text</div>
                <br class="clear" />
            </div>

              <div class="groupContent border999" settingsPage="CompanySignature.aspx">
                <div class="groupContentName left">Company Signature</div>
                <br class="clear" />
            </div>
       
            <div class="groupContent border999" settingsPage="CompanyProfile.aspx">
                <div class="groupContentName left"><%= WebTextManager.GetText("/pageText/settings/navigation.ascx/companyProfile")%></div>
                <br class="clear" />
            </div>
        </div>
    </div>

     <%-- Application Text --%>
    <div class="groupWrapper surveyWrapper pageWrapper settingWrapper">
        <div class="groupHeader">
            <div class="groupName left"><%= WebTextManager.GetText("/pageText/settings/navigation.ascx/textSettings")%></div>
            <br class="clear" />
        </div>

        <div class="folderContainer">
        <% if (!ApplicationManager.AppSettings.EnableMultiDatabase &&
               ApplicationManager.AppSettings.AllowMultiLanguage) { 
        %>
            <div class="groupContent border999" settingsPage="Languages.aspx">
                <div class="groupContentName left"><%= WebTextManager.GetText("/pageText/settings/navigation.ascx/languages")%></div>
                <br class="clear" />
            </div>
        <% } %>
            <% if (ApplicationManager.AppSettings.AllowMultiLanguage){  %>
            <div class="groupContent border999" settingsPage="LanguageNames.aspx">
                <div class="groupContentName left"><%= WebTextManager.GetText("/pageText/settings/navigation.ascx/languageNames")%></div>
                <br class="clear" />
            </div>
            <% } %>

            <div class="groupContent border999" settingsPage="SurveyText.aspx">
                <div class="groupContentName left"><%= WebTextManager.GetText("/pageText/settings/navigation.ascx/surveyText")%></div>
                <br class="clear" />
            </div>
             <div class="groupContent border999" settingsPage="EmailText.aspx">
                <div class="groupContentName left">Email text</div>
                <br class="clear" />
            </div>
            <div class="groupContent border999" settingsPage="ValidationText.aspx">
                <div class="groupContentName left"><%= WebTextManager.GetText("/pageText/settings/navigation.ascx/validationText")%></div>
                <br class="clear" />
            </div>
            <div class="groupContent border999" settingsPage="SelfRegistrationText.aspx">
                <div class="groupContentName left"><%= WebTextManager.GetText("/pageText/settings/navigation.ascx/selfRegistrationText")%></div>
                <br class="clear" />
            </div>
            <div class="groupContent border999" settingsPage="ImportText.aspx">
                <div class="groupContentName left"><%= WebTextManager.GetText("/pageText/settings/navigation.ascx/importText")%></div>
                <br class="clear" />
            </div>
        </div>
    </div>

    <% if (!ApplicationManager.AppSettings.EnableMultiDatabase){  %>
         <%-- System Status --%>
        <div class="groupWrapper surveyWrapper pageWrapper settingWrapper">
            <div class="groupHeader">
                <div class="groupName left"><%= WebTextManager.GetText("/pageText/settings/navigation.ascx/systemStatus")%></div>
                <br class="clear" />
            </div>
            <div class="folderContainer">
                <div class="groupContent border999" settingsPage="LoggedInUsers.aspx">
                    <div class="groupContentName left"><%= WebTextManager.GetText("/pageText/settings/navigation.ascx/currentUsers")%></div>
                    <br class="clear" />
                </div>
                <div class="groupContent border999" settingsPage="Exceptions.aspx">
                    <div class="groupContentName left"><%= WebTextManager.GetText("/pageText/settings/navigation.ascx/exceptionLog")%></div>
                    <br class="clear" />
                </div>
                 <div class="groupContent border999" settingsPage="<%=CacheManagementPage%>">
                    <div class="groupContentName left"><%= WebTextManager.GetText("/pageText/settings/navigation.ascx/cache")%></div>
                    <br class="clear" />
                </div>
            </div>
        </div>
    <%} %>
    <div class="groupWrapper surveyWrapper pageWrapper settingWrapper">
            <div class="groupHeader">
                <div class="groupName left">Timeline Settings</div>
                <br class="clear" />
            </div>
            <div class="folderContainer">
                <div class="groupContent border999" settingsPage="TimelineCommonSettings.aspx">
                    <div class="groupContentName left"><%= WebTextManager.GetText("/timeline/eventsettings/caption/common_settings")%></div>
                    <br class="clear" />
                </div>
                <div class="groupContent border999" settingsPage="TimelineSurveyManagerSettings.aspx">
                    <div class="groupContentName left"><%= WebTextManager.GetText("/timeline/eventsettings/caption/survey_manager_settings") %></div>
                    <br class="clear" />
                </div>
                <div class="groupContent border999" settingsPage="TimelineUserManagerSettings.aspx">
                    <div class="groupContentName left"><%= WebTextManager.GetText("/timeline/eventsettings/caption/user_manager_settings") %></div>
                    <br class="clear" />
                </div>
            </div>
        </div>
</asp:Content>

<asp:Content ID="_right" runat="server" ContentPlaceHolderID="_rightContent">
    <div id="detailProgressContainer" style="display:none;">
        <div id="detailProgress" style="text-align:center;">
            <p>Loading...</p>
            <p>
                <asp:Image ID="_progressSpinner" runat="server" SkinId="ProgressSpinner" />
            </p>
        </div>
    </div>

    <div id="_editFrame" class="padding10 dashboard">
        <div class="introPage">
            <h3>Welcome to Engauge Settings!</h3>

            <p>
                    The Settings area is the place to set Survey, Report, User, System and Application settings
                    defaults. Any changes you make in the Settings area will be applied across your
                    entire Engauge account for all users. If you have any questions on the impact
                    of Settings changes, please 
                    <a href="http://www.checkbox.com/support/ticket.aspx" target="_blank">contact support</a> prior to making the change.
            </p>

            <p>
                    To view and edit survey, report, user, system and application settings, click on the
                    appropriate category on the left to expand the details and options.
            </p>

            <p>
                    For more help on Settings, check out our online 
                    <a href="http://www.checkbox.com/resources/training_videos.aspx" target="_blank">Training Videos</a> and 
                    <a href="http://www.checkbox.com/resources/support_videos.aspx" target="_blank">Support Videos</a> or 
                    <a href="http://www.checkbox.com/support/ticket.aspx" target="_blank">contact support</a>.
            </p> 
        </div>
    </div>
</asp:Content>