<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="Edit.aspx.cs" Inherits="CheckboxWeb.Forms.Surveys.Edit" MasterPageFile="~/DetailList.Master" ValidateRequest="false" IncludeJsLocalization="true" %>
<%@ Import Namespace="Checkbox.Common" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Register Src="~/Forms/Surveys/Controls/SurveySettings.ascx" TagName="SurveySettings" TagPrefix="ckbx" %>
<%@ MasterType VirtualPath="~/DetailList.Master" %>
<%-- Script Content --%>
<asp:Content ID="_scriptContent" runat="server" ContentPlaceHolderID="_head">
<ckbx:ResolvingScriptElement runat="server" Source="../../Resources/jquery.slickswitch.js"/>
<ckbx:ResolvingScriptElement runat="server" Source="../../Resources/surveys/edit.js"/>
<ckbx:ResolvingScriptElement runat="server" Source="../../Resources/surveys/settings.js"/>
<ckbx:ResolvingScriptElement runat="server" Source="../../Resources/tooltip.js"/>
<ckbx:ResolvingScriptElement runat="server" Source="../../Resources/jquery.truncate.js"/>
<ckbx:ResolvingScriptElement runat="server" Source="../../Resources/StatusControl.js"/>
<ckbx:ResolvingScriptElement runat="server" Source="../../Resources/globalHelper.js"/>

<%-- Global Survey Stylesheets --%>
<ckbx:ResolvingCssElement runat="server" media="screen" Source="~/GlobalSurveyStyles.css"/>
<ckbx:ResolvingCssElement runat="server" media="screen" Source="~/ScreenSurveyStyles.css"/>
<ckbx:ResolvingCssElement runat="server" media="screen" Source="../../Resources/css/jquery.lwMultiSelect.css" />

<%--<ckbx:ResolvingCssElement runat="server" media="screen" Source="../../Resources/css/listswap.css" />--%>
<!--[if lte IE 7]>
    <link rel="Stylesheet" type="text/css" media="screen" href="<%= ResolveUrl("~/GlobalSurveyStyles_IE.css") %>"/>
<![endif]-->

<script type="text/javascript">
    // Id of language select container 
    var languageListId = '#<%= _languageList.ClientID %>';
    var defaultLanguage = '<%= ResponseTemplate.LanguageSettings.DefaultLanguage %>';
    var currentPreviewType = 'pc';

    <%-- Init --%>
    $(document).ready(function() {
        $(document).on('click', '.ckbx-closedialog', function() {
            reloadPermissionsAcl();
        });

        $('#survey_actions_menu .pdfExportLink').on('click', function() {
            showDialog('Export.aspx?s=<%= ResponseTemplateId %>&loc=' + templateEditor.currentLanguage, 'properties');
        });

        <%-- Initialize Survey Editor Obj --%>
        surveyEditor.initialize(
            _at,
            '<%= ResolveUrl("~/") %>',
            <%= ResponseTemplateId %>,
            '<%= LanguageCode %>',
            'SurveyEditor',
            <%= IsFormReadOnly.ToString().ToLower() %>,
            new Array({ name: 's', value: <%= ResponseTemplateId %> }),
            'itemEditorContainer',
            'itemTitle',
            'itemType',
            'itemId',
            'detailContent',
            'detailWelcome',
            'detailProgressContainer',
            'detailMenuContainer',
            '<%= ShowConfirmation.ToString() %>',
            'surveySettings'
        );

        //Ensure that all the templates are loaded, and only after this navigation can be loaded.
        var intervalID;
        intervalID = window.setInterval(function() {
            if (surveyEditor.checkIfTemplatesLoaded()) {
                surveyEditor.loadNavigation('surveyNavigatorPlace');
                <% if (Request.Params["item"] != null)
                   { %>
                templateEditor.loadItemEditor(<%= Request.Params["item"] %>);
                $('#surveySettings').hide();
                $('#detailWelcome').hide();
                $('#detailContent').show();
                <% } %>
                clearInterval(intervalID);
            }
        }, 100);

        //Bind language change
        $(languageListId).val(defaultLanguage).change(onLanguageChange).uniform();

        //
        $('#activateItemLink').on('click', function() {
            svcSurveyEditor.toggleItemActiveStatusD(_at, <%= ResponseTemplateId %>, templateEditor.selectedItemId)
                .then(updateItemIsActiveStatus(_at, <%= ResponseTemplateId %>, templateEditor.selectedItemId));
        });

        //Enable to edit survey name
        <% if (ResponseTemplate != null)
           { %>
        var text = "<%= Utilities.AdvancedHtmlDecode(Utilities.AdvancedHtmlEncode(ResponseTemplate.Name)).Replace("\"", "\\\"").Replace("\n", string.Empty) %>";
        $('#surveyName').text(htmlDecode(text));
        <% } %>
        $('#surveyName')
            .addClass('hand')
            .ckbxEditable({
                inputCssClass: 'surveyEditorNameInput',
                onSave: updateSurveyName
            });

        $("#survey_favorite_button").click(function(event) { onFavoriteButtonClick($(this), event); });

        //check if current survey is favorite;
        svcSurveyManagement.IsFavoriteSurvey('', <%= ResponseTemplateId %>, {})
            .then(function(result) {
                if (result) {
                    $("#survey_favorite_button").addClass("favorite");
                } else {
                    $("#survey_favorite_button").removeClass("favorite");
                }
            });
        statusControl.initialize('_statusPanel');

        securityHelper.protect('<%= WebUtilities.GetCurrentUserEncodedName() %>', svcAuthorization.RESOURCETYPE_SURVEY, <%= ResponseTemplateId %>, '.dashboard .header-content');
    });

    <%-- Redirect to created invitation --%>
    function openInvitation(params) {
        window.location.href = '<%= ResolveUrl("~/Forms/Surveys/Invitations/Manage.aspx") %>?s=' + params.surveyId + '&i=' + params.invitationId;
    }

    function onFavoriteButtonClick(target, event) {
        if (!$("#survey_favorite_button").hasClass("favorite")) {
            svcSurveyManagement.AddFavoriteSurvey('', <%= ResponseTemplateId %>, {})
                .then(function() {
                    $("#survey_favorite_button").addClass("favorite");
                });

        } else {
            svcSurveyManagement.RemoveFavoriteSurvey('', <%= ResponseTemplateId %>, {})
                .then(function() {
                    $("#survey_favorite_button").removeClass("favorite");
                });
        }
    }

    function updateItemIsActiveStatus(authToken, surveyId, itemId) {
        svcSurveyEditor.getItemIsActiveD(authToken, surveyId, itemId).then(function(result) {
            if (result) {
                $('.itemIsNotActiveWarning').hide();
                $('#activateItemLink').text('<%= WebTextManager.GetText("/pageText/forms/surveys/ItemActivation.aspx/deactivate") %>');
            } else {
                $('.itemIsNotActiveWarning').show();
                $('#activateItemLink').text('<%= WebTextManager.GetText("/pageText/forms/surveys/ItemActivation.aspx/activate") %>');
            }
        });
    }

    function copyActionIsAvailable(authToken, surveyId, itemId) {
        svcSurveyEditor.copyActionIsAvailableForItem(authToken, surveyId, itemId).then(function(result) {
            if (result) {
                $('#moveCopyItemLink').show();
            } else {
                $('#moveCopyItemLink').hide();
            }
        });
    }

    /////////////////////////////////////////////////////////////////////
    // Handle language change
    function onLanguageChange() {
        surveyEditor.onLanguageChange($(languageListId).val());
        refreshSurveyPreview();
    }

    //Handle dialog close
    function onDialogClosed(args) {
        if (args == null) {
            return;
        }
        if (typeof(args.op) != 'undefined' && args.op == 'refresh') {
            reloadPermissionsAcl();
        }
    }

    //reload survey permissions acl in settings
    function reloadPermissionsAcl() {
        $('.setting-container iframe').attr('src', $('.setting-container iframe').attr('src'));
    }

    //Update survey setting
    function updateSurveyName(settingElement, newValue, oldValue) {
        svcSurveyEditor.updateSurveySettingD(
                _at,
                <%= ResponseTemplate.ID %>,
                settingElement.attr('settingName'),
                escapeInjections(newValue))
            .then(function(result) {
                //Special case for survey name
                if (settingElement.attr('settingName') == 'Name') {
                    settingElement.html(result);
                }
            })
            .fail(function(result) {
                var errorDiv = $('<div class="error left message" style="margin-left:15px;">' + result.FailureMessage + '</div>');
                settingElement.html(oldValue);
                settingElement.after(errorDiv);
                setTimeout(function() { errorDiv.fadeOut('fast', errorDiv.remove); }, 5000);
            });
    }

    function refreshSurveyPreview(previewType) {
        if (typeof previewType == 'undefined' || previewType == '') {
            if (typeof currentPreviewType != 'undefined' && previewType != '')
                previewType = currentPreviewType;
            else
                previewType = 'pc';
        }

        $('#' + templateEditor.welcomeElemId).show();
        $('#' + templateEditor.detailElemId).hide();
        $('#' + templateEditor.settingsElemId).hide();

        templateEditor.selectedItemId = -1;
        $('#' + templateEditor.itemFrameId).attr('src', '');
        $('#' + templateEditor.itemTitleElemId).empty(); //.html('');
        $('#' + templateEditor.itemTypeElemId).empty(); //.html('');
        $('#' + templateEditor.itemIdElemId).empty(); //.html('');

        var prev = $('#survey_preview');
        //refresh survey preview iframe
        if (prev) {
            //set frame size
            prev.attr('style', '');
            prev.attr('class', 'preview-' + previewType);
            $('#survey-preview-wrapper').attr('class', 'preview-background-' + previewType);
            $(window).resize();

            var mode = previewType == 'pc' ? 'SurveyPreview' : 'SurveyMobilePreview';
            var src = prev.attr('src');

            //replace locale
            var ind = src.indexOf('&loc');
            var subStringToRep = src.substring(ind, ind + 10);
            src = src.replace(subStringToRep, '&loc=' + templateEditor.currentLanguage);

            //replace render mode
            ind = src.indexOf('&mode');
            if (ind >= 0) {
                subStringToRep = src.substring(ind, src.length);
                src = src.replace(subStringToRep, '&mode=' + mode);
            } else {
                src += '&mode=' + mode;
            }

            prev.attr('src', src);
            statusControl.showStatusMessage("The survey preview has been refreshed.", StatusMessageType.success);
        }

        currentPreviewType = previewType;
        resizePanels();
    };

    function onLaunchDialogClosed() {
        updateSurveySettings();
    }

    function clearCurrentItem() {
        var iframe = document.getElementById('templateEditorModal');
        var innerDoc = iframe.contentDocument || iframe.contentWindow.document;
        var form = innerDoc.getElementById("aspnetForm");
        $.ajax({
            type: "POST",
            url: $(form).attr("action") + "&cmd=clearCurrentItem",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function(msg) {
            }
        });

        closeWindow();

    }

    // Handle event on 
    $(function(){
        $("body").on("click", ".libraryItems li", function (e) {
            e.preventDefault();
            var pageId = $(this).parent().attr("data-pageId");
            if ($(this).text() === "New Custom Item") {
                surveyEditor.showAddItemDialog(pageId, 'addNewSurveyItemWizard', clearCurrentItem);
            }
            else{                
                var itemId = $(this).attr("data-itemId");
                var surveyId = $("#templateIdVal").val();  
                var libraryId = $(this).attr("data-libraryId");
                svcSurveyManagement.addSurveyItemFromLibrary(pageId, itemId, surveyId, libraryId, function (data) {
                }, null);
                history.go(0);
            }
        });
    });

</script>
</asp:Content>

<%-- Language Select --%>
<asp:Content ID="_titleButtons" runat="server" ContentPlaceHolderID="_titleLinks">
    <div class="surveys-edit-buttons">
        <% if (!IsFormReadOnly)
           { %>
            <a class="header-button ckbxButton blueButton statistics_AddPage" href="javascript:templateEditor.addPage();"><%= WebTextManager.GetText("/pageText/formEditor.aspx/newPage") %></a>
            <a class="header-button ckbxButton blueButton statistics_ExpandPages expand-up-angle-arrow">Collapse All</a>
        <% } %>

        <asp:DropDownList ID="_languageList" runat="server" CssClass="right"></asp:DropDownList>
    </div>
</asp:Content>

<asp:Content ID="_warningContent" runat="server" ContentPlaceHolderID="_topContent">
    <div class="warning message isLocked" style="display: none;">
        <%= WebTextManager.GetText("/pageText/forms/surveys/edit.aspx/editNotAllowed") %>
    </div>
</asp:Content>

<%-- Left Content --%>
<asp:Content ID="_listContent" runat="server" ContentPlaceHolderID="_leftContent">
    <div id="surveyNavigatorPlace" class="leftPanelScrollFix"></div>
</asp:Content>

<%-- Right Content --%>
<asp:Content ID="_detailContent" runat="server" ContentPlaceHolderID="_rightContent">

    <!-- Survey Header -->
    <div class="survey-header-container-no-dash clearfix" style="margin: -10px 15px 0 5px;">
        <div class="header-content">
            <a id="survey_favorite_button" href="#" class="favorite-survey"></a>
            <h3 id="surveyName" settingName="Name" editMode="Text"></h3>
            <a protectPermission="Form.Administer" id="survey_settings_button" class="action-preview action-button ckbxButton silverButton" href="#" target="_blank">Configure</a>
            <a id="survey_actions_button" class="action-share action-menu-toggle action-button ckbxButton silverButton buttonWidth100" href="#"><span class="toggle-arrow"></span>Print/Export</a>
                        <a id="survey_preview_button" class="action-preview action-menu-toggle action-button ckbxButton silverButton" href="#">
                <span class="toggle-arrow"></span><%= WebTextManager.GetText("/pageMenu/survey_editor/_preview") %>
            </a>
            <a protectpermission="Analysis.Responses.View" id="survey_results_button" class="action-share action-menu-toggle action-button ckbxButton silverButton" href="#"><span class="toggle-arrow"></span>Analyze</a>
            <a protectPermission="Form.Administer" id="survey_sharing_button" class="action-share action-menu-toggle action-button ckbxButton silverButton" href="#"><span class="toggle-arrow"></span>Share</a>


            <div id="survey_sharing_menu" class="groupMenu glimpse-options-menu">
                <ul class="itemActionMenu">
                    <li>
                        <a class="ckbxButton" href="javascript:showDialog('Test.aspx?s=<%= ResponseTemplateId %>', 'wizard');"><%= WebTextManager.GetText("/pageMenu/survey_editor/_takeSurvey") %></a>
                    </li>
                    <li>
                        <a class="ckbxButton" href="javascript:showDialog('../Surveys/Launch.aspx?s=<%= ResponseTemplate.ID %>&onClose=onLaunchDialogClosed', 'wizard');">Launch</a>
                    </li>
                    <li protectPermission="Form.Administer" class="popup">
                        <a class="ckbxButton" href="#"><span class="toggle-arrow"></span>Social Sharing</a>
                        <ul class="itemActionMenu groupMenu " style="width: 7em !important; left: -7em !important;">
                            <li>
                                <a class="ckbxButton statistics_TwitterShare" href="<%= TwitterUrl %>" onclick=" javascript:window.open(this.href, '', 'menubar=no,toolbar=no,resizable=yes,scrollbars=yes,height=600,width=600');return false;"><%= WebTextManager.GetText("/pageMenu/survey_editor/Twitter") %></a>
                            </li>
                            <li>
                                <a class="ckbxButton" href="<%= FacebookUrl %>" onclick=" javascript:window.open(this.href, '', 'menubar=no,toolbar=no,resizable=yes,scrollbars=yes,height=600,width=600');return false; "><%= WebTextManager.GetText("/pageMenu/survey_editor/Facebook") %></a>
                            </li>
                            <li>
                                <a class="ckbxButton" href="<%= LinkedInUrl %>" onclick=" javascript:window.open(this.href, '', 'menubar=no,toolbar=no,resizable=yes,scrollbars=yes,height=600,width=600');return false;"><% = WebTextManager.GetText("/pageMenu/survey_editor/LinkedIn") %></a>
                            </li>
                            <li>
                                <a class="ckbxButton" href="<%= GplusUrl %>" onclick=" javascript:window.open(this.href, '', 'menubar=no,toolbar=no,resizable=yes,scrollbars=yes,height=600,width=600');return false;"><% = WebTextManager.GetText("/pageMenu/survey_editor/Google+") %></a>
                            </li>
                        </ul>
                    </li>
                    <li>
                        <a class="ckbxButton" href="javascript:showDialog('../Surveys/EmbedMenu.aspx?s=<%= ResponseTemplate.ID %>', 450, 650)">Link or Embed</a>
                    </li>
                    <% if (ShowInvitations)
                       { %>
                        <li class="popup">
                            <a class="ckbxButton" href="#">
                                <span class="toggle-arrow"></span><% = WebTextManager.GetText("/pageMenu/survey_editor/EmailInvitations") %>
                            </a>
                            <ul class="itemActionMenu groupMenu" style="top: 6em !important; left: -10em !important;">
                                <% if (SheduledInvitations)
                                   { %>
                                    <li>
                                        <a class="ckbxButton" href="javascript:showDialog('../Surveys/Invitations/AddScheduled.aspx?s=<%= ResponseTemplate.ID %>', 'wizard')">+Email Invitation</a>
                                    </li>
                                <% }
                                   else
                                   { %>
                                    <li>
                                        <a class="ckbxButton" href="javascript:showDialog('../Surveys/Invitations/Add.aspx?s=<%= ResponseTemplate.ID %>', 'wizard')">+Email Invitation</a>
                                    </li>
                                <% } %>
                                <li>
                                    <a class="ckbxButton" href="../Surveys/Invitations/Manage.aspx?s=<%= ResponseTemplate.ID %>">Manage Invitations</a>
                                </li>
                            </ul>
                        </li>
                        <li class="popup">
                            <a class="ckbxButton" href="#"><span class="toggle-arrow"></span> <% = WebTextManager.GetText("/pageMenu/survey_editor/UserLinks") %></a>
                            <ul class="itemActionMenu groupMenu" style="top: 6em !important; left: -10em !important;">
                                <li><a class="ckbxButton" href="javascript:showDialog('../Surveys/Invitations/AddUsers.aspx?s=<%= ResponseTemplate.ID %>', 'wizard')"><% = WebTextManager.GetText("/pageMenu/survey_editor/plusUserLinks") %></a></li>
                                <li><a class="ckbxButton" href="../Surveys/Invitations/UserManage.aspx?s=<%= ResponseTemplate.ID %>"> <% = WebTextManager.GetText("/pageMenu/survey_editor/manageUserLinks") %></a></li>
                            </ul>
                        </li>
                    <% } %>
                </ul>
            </div>

            <div id="survey_results_menu" class="groupMenu glimpse-options-menu">
                <ul class="itemActionMenu">
                    <li class="responses" protectpermission="Analysis.Responses.View">
                        <a class="ckbxButton" href="../Surveys/Responses/Manage.aspx?s=<%= ResponseTemplate.ID %>">View Responses</a>
                    </li>
                    <li class="reports" protectpermission="Analysis.Create">
                        <a class="ckbxButton" href="../Surveys/Reports/Manage.aspx?s=<%= ResponseTemplate.ID %>">Summary Reports</a>
                    </li>
                    <li class="exportdata" protectpermission="Analysis.Responses.View">
                        <a class="ckbxButton" href="javascript:showDialog('../Surveys/Responses/Export.aspx?s=<%= ResponseTemplate.ID %>', 'longlargeProperties');">Export Data</a>
                    </li>
                </ul>
            </div>

            <div id="survey_actions_menu" class="groupMenu glimpse-options-menu">
                <ul class="itemActionMenu">
                    
                    <li protectPermission="Form.Administer" >

                                <a class="ckbxButton" href="Edit.aspx?doexport=<%= ResponseTemplateId %>"><%= WebTextManager.GetText("/pageText/manageSurveys.aspx/export") %></a>

                                <a class="ckbxButton pdfExportLink" href="#"><%= WebTextManager.GetText("/pageMenu/survey_editor/_printPdf") %> </a>

                    <li>
                        <a class="ckbxButton" href="Preview.aspx?print=default&s=<%= ResponseTemplateId %>&loc=<%= ResponseTemplate.LanguageSettings.DefaultLanguage %>" target="_blank"><%= WebTextManager.GetText("/pageMenu/survey_editor/_print") %></a>
                    </li>
                </ul>
            </div>
            <div id="survey_preview_menu" class="groupMenu glimpse-options-menu">
                <ul class="itemActionMenu">
                    <li>
                        <a class="ckbxButton" onclick="refreshSurveyPreview('pc');" href="#"><%= WebTextManager.GetText("/pageMenu/survey_editor/_pc") %></a>
                    </li>
                    <li>
                        <a class="ckbxButton" onclick="refreshSurveyPreview('tablet');" href="#"><%= WebTextManager.GetText("/pageMenu/survey_editor/_tablet") %></a>
                    </li>
                    <li>
                        <a class="ckbxButton" onclick="refreshSurveyPreview('smartphone');" href="#"><%= WebTextManager.GetText("/pageMenu/survey_editor/_mobile") %></a>
                    </li>
                </ul>
            </div>
        </div>
    </div>

    <div id="detailProgressContainer" style="display: none;">
        <div id="detailProgress" style="text-align: center;">
            <p><%= WebTextManager.GetText("/common/loading") %></p>
            <p>
                <asp:Image ID="_progressSpinner" runat="server" SkinID="ProgressSpinner"/>
            </p>
        </div>
    </div>
    <div id="detailContentContainer" class="survey-header-container-no-dash">
        <div id="surveySettings" style="display: none;">
            <ckbx:SurveySettings ID="_settings" runat="server"/>
        </div>
        <div id="detailWelcome" class="introPage">
            <div id="survey-preview-wrapper">
                <iframe id="survey_preview" src="Preview.aspx?preview=true&s=<%= ResponseTemplateId %>&loc=<%= ResponseTemplate.LanguageSettings.DefaultLanguage %>&mode=SurveyPreview" class="preview-pc"></iframe>
            </div>
        </div>
        <div id="detailContent" style="display: none;">
            <div id="detailHeader"></div>
            <h3 class="clearfix">
                <span id="itemTitle"></span>
                <div id="detailMenuContainer">
                    <a class="action-menu-toggle action-button ckbxButton silverButton buttonWidth100" href="#" id="item_actions_button"><span class="toggle-arrow"></span>Item Actions</a>
                    <div id="item_actions_menu" class="groupMenu" style="margin-top: 30px !important; z-index: 9999">
                        <ul class="itemActionMenu allMenu">
                            <li>
                                <a class="ckbxButton silverButton statistics_ExportData" href="javascript:void(0);" id="exportItemLink"><%= WebTextManager.GetText("/pageText/formEditor.aspx/export") %></a>
                            </li>
                            <% if (AllowActiveEdit)
                               { %>
                                <li>
                                    <a class="ckbxButton silverButton" href="javascript:void(0);" id="activateItemLink"><%= WebTextManager.GetText("/pageText/formEditor.aspx/activation") %></a>
                                </li>
                            <% } %>
                            <% if (!IsFormReadOnly)
                               { %>
                                <li>
                                    <a class="ckbxButton silverButton" href="javascript:void(0);" id="moveCopyItemLink"><%= WebTextManager.GetText("/pageText/formEditor.aspx/moveCopy") %></a>
                                </li>
                                <li>
                                    <a class="ckbxButton redButton" href="javascript:void(0);" id="deleteItemLink"><%= WebTextManager.GetText("/pageText/formEditor.aspx/delete") %></a>
                                </li>
                            <% } %>
                        </ul>
                    </div>
                </div>
            </h3>
            <div id="itemEditorContainer"></div>
            <div id="detailFooter"></div>
        </div>
    </div>
</asp:Content>
