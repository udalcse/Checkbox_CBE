<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="SurveyDashboard.ascx.cs" Inherits="CheckboxWeb.Forms.Controls.SurveyDashboard" %>
<%@ Import Namespace="Checkbox.Common" %>
<%@ Import Namespace="Checkbox.Messaging.Email" %>
<%@ Import Namespace="Checkbox.Management" %>
<%@ Import Namespace="Checkbox.Users" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Import Namespace="Prezza.Framework.Security" %>

<script type="text/javascript">
	var _dashSurveyId = null;
	var _dashSurveyName = null;
    var _loadedTemplateCount = 0;
    var _templateCount;
    var _dashSurveyLanguages = new Array();
    
	// Ensure services initialized
	$(document).ready(function () {
		_loadedTemplateCount = 0;
        _templateCount = 2;

        //Precompile templates used
		templateHelper.loadAndCompileTemplateD('<%=ResolveUrl("~/Forms/jqtmpl/surveyListSurveyTemplate.html") %>', 'surveyListSurveyTemplate.html')
            .then(onTemplateLoadAndCompiled);
		templateHelper.loadAndCompileTemplateD('<%=ResolveUrl("~/Forms/jqtmpl/surveyDashboardTemplate.html") %>', 'surveyDashboardTemplate.html')
            .then(onTemplateLoadAndCompiled);

        $(document).on('click', '.stopEvent', function(event){event.stopPropagation();});
   	});

    //TemplateCompiled handler
    function onTemplateLoadAndCompiled() {
        _loadedTemplateCount++;
    }

	// Expose Method to Load Survey
	function loadSurveyData(surveyId, reloadListItem) {
        //Ensure all templates loaded and compiled       
        if(_loadedTemplateCount != _templateCount)
        {
            //Try to load Survey data after 100 ms.
            setTimeout(function() { loadSurveyData(surveyId, reloadListItem); }, 100);
            return null;
        }

		$('#detailProgressContainer_<%=ID %>').show();
		CleanDashboard();

		if(surveyId == null || surveyId == ''){
			return null;
		}
		
		_dashSurveyId = surveyId;
        _dashSurveyName = $('ul[surveyid="' + surveyId + '"]>li.groupContentName').html();

		// Start by loading survey meta data
	    return svcSurveyManagement.getSurveyMetaDataD(_at, surveyId, { reloadListItem: reloadListItem, surveyId: surveyId, at: _at })
	        .then(onSurveyMetaDataLoaded, onSurveyMetaDataLoadingFailed)
	        .then(applySurveyTemplate)
    	    .then(bindClickToEditEvents)
	        .then(bindActionClickEvents)
	        .then(bindFavoriteSurveyClickEvent)
	        .then(bindLocalizedTextEditors)
	        .then(doSecurityPruning)
	        .then(setViewAllLinkUrls)
	        .then(bindDefaultSurveyText)
	        .then(onMetaDataComplete);
	}

    function OnDefaultLanguageChanged(e) {
        svcSurveyEditor.setDefaultLanguage(
            _at,
	        _dashSurveyId,
	        $("#surveyDefaultLanguageList").val());
    }

    //	
	function bindDefaultSurveyText() {        
	    $("#surveyDefaultLanguageList").val(surveyData.DefaultLanguage);
	    $("#surveyDefaultLanguageList").change(function(e){ OnDefaultLanguageChanged(e);});
	}

	//
	function bindLocalizedTextEditors() {        
        var languageCodes = Array();
        $(surveyData.SupportedLanguages.NameValueList).each(function (i, o)
        {
            languageCodes.push(o.Name);
        });
	    _dashSurveyLanguages = languageCodes;
	}
	
	//
	function bindSurveyTextTemplate(textData, supportedLanguages, selectedLanguage) {
	    //Bind to template
    	var textTemplateData = { Languages: supportedLanguages, Texts: textData };

	    templateHelper.loadAndApplyTemplateD(
	        'surveyTextTemplate.html',
	        '<%=ResolveUrl("~/Forms/jqtmpl/surveyTextTemplate.html") %>',
	        textTemplateData,
	        null,
	        'localizableTextPlace',
	        true)
    	    .then(bindSurveyTextClick)
    	    .then($('#surveyTextLanguageSelect').val(selectedLanguage));
	}
	
	//
	function bindSurveyTextClick() {
	      //Check to see whether titles should be set or edit buttons enabled
	    svcAuthorization
    	    .authorizeAccessD('<%= WebUtilities.GetCurrentUserEncodedName() %>', svcAuthorization.RESOURCETYPE_SURVEY, _dashSurveyId, 'Form.Administer')
    	    .then(function(result) {
    	        //false result = no access, so do nothing
    	        if (!result) {
    	            return;
    	        }
    	        //Set titles & click events
    	        var editTitle = textHelper.getTextValue("/controlText/surveyDashboard/clickToEdit", "Click to edit");

    	        $('#localizableTextPlace > div > div > div[textid]')
        	        .attr('title', editTitle)
        	        .addClass('hand')
        	        .addClass('clickToEdit')
        	        .ckbxEditable({ onSave: updateSurveyTextValue });

    	        $('#surveyTextLanguageSelect').change(onSurveyLanguageChanged);
    	    });
	}
	
	//
	function onSurveyLanguageChanged() {
	    loadCustomTexts(_dashSurveyLanguages, $('#surveyTextLanguageSelect').val());
	}

	//
	function updateSurveyTextValue(textElement, newValue, oldValue) {
	    var spinner = $('<div class="spinner16" style="margin-top:2px;margin-left:3px;">&nbsp;</div>');
	    
	    //Start spinner
	    textElement.after(spinner);

	    svcSurveyEditor.updateSurveyTextD(
	        _at,
	        _dashSurveyId,
	        textElement.attr('textid'),
	        newValue,
	        $('#surveyTextLanguageSelect').val())
    	    .then(function(result) {
    	        //Result is new value of setting reported by server
    	        textElement.html(newValue);
    	        spinner.remove();
    	    })
    	    .fail(function(result) {
    	        spinner.remove();

    	        var errorDiv = $('<div class="error left message" style="margin-left:15px;">' + result.FailureMessage + '</div>');

    	        textElement.html(oldValue);
                textElement.after(errorDiv);
    	        
    	        setTimeout(function() { errorDiv.fadeOut('fast', errorDiv.remove); }, 5000);
    	    });
	}
	
    //
    function buildPreviewLink(styleId, styleType)
    {
        //remember pure link
        if ($("#preview" + styleType).attr("pureHref") == null || $("#preview" + styleType).attr("pureHref") == '')
            $("#preview" + styleType).attr("pureHref", $("#preview" + styleType).attr("href")); 
        $("#preview" + styleType).attr("href", $("#preview" + styleType).attr("pureHref") + "&st=" + styleId);
    }

	//
	function bindClickToEditEvents() {
        var surveyMetaData = surveyData;
	    //Check to see whether titles should be set or edit buttons enabled
	    svcAuthorization
    	    .authorizeAccessD('<%= WebUtilities.GetCurrentUserEncodedName() %>', svcAuthorization.RESOURCETYPE_SURVEY, _dashSurveyId, 'Form.Edit')
    	    .then(function(result) {
    	        //false result = no access, so do nothing
    	        if (!result) {
    	            return;
    	        }

    	        var toggleTitle = textHelper.getTextValue("/controlText/surveyDashboard/clickToToggle", "Click to toggle");
    	        var editTitle = textHelper.getTextValue("/controlText/surveyDashboard/clickToEdit", "Click to edit");

    	        //Set titles
    	        $('div[toggleSettingName]')
        	        .attr('title', toggleTitle)
        	        .addClass('hand')
        	        .unbind('click')
        	        .bind('click', toggleSurveySetting);
    	        
    	        $('div[settingName]')
        	        .attr('title', editTitle)
        	        .addClass('hand')
        	        .addClass('clickToEdit');
                    
    	        $('#surveyName')
        	        .attr('title', editTitle)
        	        .addClass('hand')
        	        .ckbxEditable({
        	                inputCssClass: 'surveyNameInput',
        	                onSave: updateSurveySetting
        	            });

    	    
    	        $('.tabContentContainer [settingName]').ckbxEditable({ onSave: updateSurveySetting, onCancel: updateSurveySettingCancelled });
    	        
                <% if (ApplicationManager.AppSettings.AllowSurveyUrlRewriting) { %>
    	        //Edit alt survey url
    	        if(surveyMetaData.SurveyUrls.length <= 1) {
    	            $('#altSurveyUrlPlace').hide();
    	            $('#addCustomUrlBtn').show();
    	            $('#cancelNewAltUrlBtn').hide();
    	            
    	            $('#addCustomUrlBtn').unbind('click');
    	            $('#addCustomUrlBtn').bind('click', onEditSurveyUrl);
    	        }
    	        else {
                	$('#altSurveyUrlPlace a').attr('href', surveyMetaData.SurveyUrls[1]);

    	            $('#addCustomUrlBtn').hide();
    	            $('#altSurveyUrlPlace').show();
    	        }
                <%} else {%>
    	            $('#altSurveyUrlPlace').hide();
    	            $('#addCustomUrlBtn').hide();
    	            $('#cancelNewAltUrlBtn').hide();
                <%}%>

    	        $('#cancelNewAltUrlBtn')
        	        .show()
    	            .unbind('click')
    	            .bind('click', onCancelUpdateSurveyUrl);

    	        $('#saveNewAltUrlBtn')
        	        .unbind('click')
    	            .bind('click', onUpdateSurveyUrl);

    	        $('#newAltUrl')
        	        .unbind('keyup')
        	        .bind('keyup', function(e) {
        	            if (e.which == 13) {
        	                $('#saveNewAltUrlBtn').trigger('click');
        	            }

        	            if (e.which == 27) {
        	                $('#cancelNewAltUrlBtn').trigger('click');
        	            }
        	        });
    	         
                $('.editUrlLink')            
    	            .unbind('click')
    	            .bind('click', onEditSurveyUrl);   
    	    });
	}

	//
	function onEditSurveyUrl() {
	    //Figure out URL extension and file name
	    var altUrl = $('#altSurveyUrlPlace input[type="text"]').val();
	    var slashIndex = altUrl.lastIndexOf('/');
	    var dotIndex = altUrl.lastIndexOf('.');

	    var extension = '';
	    var fileName = '';

	    if(slashIndex > 0 && dotIndex > 0) {
	    	//Get extension
	        extension = altUrl.substring(dotIndex);
	        fileName = altUrl.substring(slashIndex + 1, dotIndex);
	    }

	    $('#newAltUrlExtension').val(extension);
	    $('#newAltUrl').val(fileName);
    
        $('#altSurveyUrlPlace').hide();
	    $('#editAltSurveyUrlPlace').show();
	    $('#cancelNewAltUrlBtn').show();
	    $('#saveNewAltUrlBtn').show();
	    $('#addCustomUrlBtn').hide();
        $('#newAltUrl').focus();

        return false;
	}
	
	//
	function onCancelUpdateSurveyUrl() {
        $('#editAltSurveyUrlPlace div.error').hide();
	    $('#saveNewAltUrlBtn').hide();
	    $('#cancelNewAltUrlBtn').hide();
        $('#editAltSurveyUrlPlace').hide();
	    
        var altUrl = $('#altSurveyUrlPlace input[type="text"]').val();
	    
	    if(altUrl == null || altUrl == '') {
	        $('#altSurveyUrlPlace').hide();
	        $('#addCustomUrlBtn').show();
	    }
	    else {
	        $('#altSurveyUrlPlace').show();
	        $('#addCustomUrlBtn').hide();
	    }
	}
	
	//
	function onUpdateSurveyUrl() {
	    //Hide errors
	    $('#editAltSurveyUrlPlace div.error').hide();
	    $('#saveNewAltUrlBtn').hide();
	    $('#cancelNewAltUrlBtn').hide();
	    $('#newAltUrlSpinner').show();
	    
	    //Get new Url.  Blank url = remove mapping
	    var newUrl = $('#editAltSurveyUrlPlace input[type="text"]').val();
	    
	    if(newUrl != '') {
	        newUrl = newUrl + $('#editAltSurveyUrlPlace select').val();
	    }

	    //Attempt to update the URL -
	    svcSurveyEditor.setAlternateUrlD(
            _at,
            _dashSurveyId,
            newUrl)
	    .then(function (result) {
	        $('#saveNewAltUrlBtn').show();
	        $('#cancelNewAltUrlBtn').show();
	        $('#newAltUrlSpinner').hide();
	        
	        if(result == "URL_UPDATE_SUCCESS") {
	            //Update alt url & hide edit
                $('#editAltSurveyUrlPlace').hide();
	            
	            if(newUrl == '') {
	                $('#altSurveyUrlPlace').hide();
	                $('#addCustomUrlBtn').show();
	            }
	            else {
	                newUrl = '<%=ApplicationManager.ApplicationPath %>/' + newUrl;
	                $('#altSurveyUrlPlace').show();
	                $('#addCustomUrlBtn').hide();
	            }
	            
	            $('#altSurveyUrlPlace input[type="text"]')
    	            .val(newUrl)
	                .attr('size', newUrl.length);
	            
	            $('#altSurveyUrlPlace a').attr('href', newUrl);
	        }
	        else {
	            //Show the error
	            $('#' + result + '_Error').show();
	        }
	    });
	}

	//
    function updateSurveySettingCancelled(settingElement)
    {
        if (settingElement.attr('editMode') == 'Date' && $.trim(settingElement.html()) == 'No Restriction')
            $('[settingName="' + settingElement.attr('settingName') + '"][editMode="Time"]').hide();
    }

    //
	function updateSurveySetting(settingElement, newValue, oldValue) {
	    var spinner = $('<div class="spinner16" style="margin-top:2px;margin-left:3px;">&nbsp;</div>');

	    //Start spinner
	    settingElement.after(spinner);

	    svcSurveyEditor.updateSurveySettingD(
	        _at,
	        _dashSurveyId,
	        settingElement.attr('settingName'),
	         escapeInjections(newValue))
    	    .then(function(result) {
    	        //Result is new value of setting reported by server
    	        spinner.remove();
    	        
    	        //Special case for survey name
    	        if(settingElement.attr('settingName') == 'Name') {
    	            reloadSurveyListItem({ Id: _dashSurveyId }, {reloadListItem:true});
        	        settingElement.html(result);
	        } else
        	        settingElement.html(result);
    	    })
    	    .fail(function(result) {
    	        spinner.remove();

    	        var errorDiv = $('<div class="error left message" style="margin-left:15px;">' + result.FailureMessage + '</div>');

    	        settingElement.html(oldValue);
                settingElement.after(errorDiv);
    	        
    	        setTimeout(function() { errorDiv.fadeOut('fast', errorDiv.remove); }, 5000);
    	    });
	    clearAndReload();
	}

	//
	function toggleSurveySetting() {
	    var settingName = $(this).attr('toggleSettingName');

	    if(settingName == null || settingName == '') {
	        return;
	    }
	    
	    $(this).children('div')
    	    .removeClass('checkChecked')
            .removeClass('checkUnchecked')
	        .addClass('spinner16');

	    svcSurveyEditor.toggleSurveySettingD(
	        _at,
	        _dashSurveyId,
	        settingName,
	        $(this))
    	    .then(function(result, buttonContainer) {
    	        buttonContainer.children('div').removeClass('spinner16');

    	        //Negate for allow survey edit or disable back button since they are displayed as "Locked" and "Allow" in ui
    	        if (buttonContainer.attr('toggleSettingName') == 'AllowSurveyEditWhileActive'
    	            || buttonContainer.attr('toggleSettingName') == 'DisableBackButton') {
    	            result = !result;
    	        }

                //Reflect on the left panel activation of the survey
    	        if (buttonContainer.attr('toggleSettingName') == 'IsActive') {
                    $('#survey_' + _dashSurveyId + ' > div').attr('class', result ? 'activeSurvey' : '');
    	        }

    	        if (result) {
    	            buttonContainer.children('div').addClass('checkChecked');
    	        }
    	        else {
    	            buttonContainer.children('div').addClass('checkUnchecked');
    	        }

    	        //Special case for activation
    	        if (buttonContainer.attr('toggleSettingName') == 'IsActive') {
    	            svcSurveyManagement.getSurveyMetaDataD(_at, _dashSurveyId).then(updateActivationStatusAndReason);
    	        }
    	        
    	        //Special case for allow resume
    	        if(buttonContainer.attr('toggleSettingName') == 'AllowResume') {
    	            if(result) {
    	                $('#saveAndQuitContainer').slideDown(250);
    	            }
    	            else {
    	                $('#saveAndQuitContainer').slideUp(250);
    	            }
    	        }
    	    })
    	    .fail(function(result) {
    	        
    	    });
	}
	
	//
	function setViewAllLinkUrls() {
	    $('#dashViewAllResponsesLink').attr('href', 'Surveys/Responses/Manage.aspx?s=' + _dashSurveyId);
	    $('#dashViewAllReportsLink').attr('href', 'Surveys/Reports/Manage.aspx?s=' + _dashSurveyId);
	    $('#dashViewAllReportFiltersLink').attr('href', 'Surveys/Reports/Filters/Manage.aspx?s=' + _dashSurveyId);
	    $('#dashViewAllInvitationsLink').attr('href', 'Surveys/Invitations/Manage.aspx?s=' + _dashSurveyId);

	    bindTooltips();
	}

	//
	function doSecurityPruning() {
	    securityHelper
    	    .protect('<%= WebUtilities.GetCurrentUserEncodedName() %>', svcAuthorization.RESOURCETYPE_SURVEY, _dashSurveyId, '#infoPlace')
    	    .then(
	        function() {
                //hide menu button if there are not any elements
	            $.each($('.itemActionMenu'), function(ind, elem){
                    if($(elem).find('li a').length == 0) {
                        $('#' + $(elem).attr('data-assosiatedbutton')).hide();
                    }
                });
                //show the panel
                $('#detailProgressContainer_<%=ID %>').hide();
	            $('#infoPlace').show();
	        });
	}

	//Make dashboard empty
	function CleanDashboard(){
		$('#infoPlace').empty();
        $('#infoPlace').hide();
	}


    var surveyData = null;
    var resultDataArgs = null;

	// Apply template to loaded metadata and then apply child templates
	function onSurveyMetaDataLoaded(resultData, args) {
        if (resultData == null) {
			$('#infoPlace').html('<div class="error message" style="margin:15px;padding:5px;">Unable to load survey information for survey with id: ' + args.surveyData.Id + '.</div>');
            return;
		}
        surveyData = resultData;
        resultDataArgs = args;
	}

    function onSurveyMetaDataLoadingFailed(resultData, args) {
        surveyData = null;
        resultDataArgs = null;
        templateHelper.loadAndApplyTemplateD
            (
                'surveyDashboardTemplate.html',
                '<%=ResolveUrl("~/Forms/jqtmpl/surveyDashboardTemplate.html") %>',
                {Id : _dashSurveyId, Name : _dashSurveyName, Description : '~~chopped template~~'},
                null,
                'infoPlace',
                true
            ).
            then(setViewAllLinkUrls).
            then(doSecurityPruning).
            then(bindActionClickEvents);
    }

    //
    function applySurveyTemplate(){
        var resultData = surveyData;
        var args = resultDataArgs;

        // Load/Compile/Run Dashboard Template
        // Add some extra data to result data to be used by dashboard template
        resultData.ReportIncompleteResponses = <%= ApplicationManager.AppSettings.ReportIncompleteResponses.ToString().ToLower() %> ;
        resultData.AppBaseUrl = '<%=ApplicationManager.ApplicationURL %>';
        
        //Figure out alt url extension, if any
        if(resultData.SurveyUrls.length > 1) {
            var altUrl = resultData.SurveyUrls[1];
            var dotIndex = altUrl.lastIndexOf(altUrl, 0);
            
            if(dotIndex > 0) {
                resultData.AlternateUrlExtension = altUrl.substring(dotIndex + 1);
            }
        }

        resultData.AllowedUrlRewriteExtensions = new Array();
        
        <%
            if(ApplicationManager.AppSettings.AllowSurveyUrlRewriting)
            {
                foreach(var extension in ApplicationManager.AppSettings.AllowedUrlRewriteExtensions)
                {
                 %> resultData.AllowedUrlRewriteExtensions.push('<%=extension %>');     <%   
                }
            }
        %>
        
        return templateHelper.loadAndApplyTemplateD
        (
            'surveyDashboardTemplate.html',
            '<%=ResolveUrl("~/Forms/jqtmpl/surveyDashboardTemplate.html") %>',
            resultData,
            null,
            'infoPlace',
            true
        );
    }
    

	//Update surveyListItem
	function reloadSurveyListItem(resultData, args){
        // Update List Item, if necessary
	    if(args.reloadListItem){
			$('#survey_' + resultData.Id).effect(
			'shake', 
			{
				times:2,
				distance:10,
				duration:250
			},
			function(){
				//Required to remove jagged text left behind in IE for some reason
				// when pulsate is called.
				if (typeof(this.style.removeAttribute) != 'undefined' && this.style.removeAttribute != null)
					this.style.removeAttribute('filter');
		        }
			);
	        
	        $('#favoriteSurvey_' + resultData.Id).effect(
			'shake', 
			{
				times:2,
				distance:10,
				duration:250
			},
			function(){
				//Required to remove jagged text left behind in IE for some reason
				// when pulsate is called.
				if (typeof(this.style.removeAttribute) != 'undefined' && this.style.removeAttribute != null)
					this.style.removeAttribute('filter');

				svcSurveyManagement.getSurveyListItem(
		            _at,
		            resultData.Id,
		            function(resultData){
		                //Finally reload list of surveys
		                //Apply template to common list of surveys
		                templateHelper.loadAndApplyTemplate(
				            'surveyListSurveyTemplate.html',
				            '<%=ResolveUrl("~/Forms/jqtmpl/surveyListSurveyTemplate.html") %>',
				            resultData,
				            null,
				            'survey_' + resultData.ID,
				            true);
		                //Apply template to list of favorite surveys
		                templateHelper.loadAndApplyTemplate(
				            'surveyFavoriteListTemplate.html',
				            '<%=ResolveUrl("~/Forms/jqtmpl/surveyListSurveyTemplate.html") %>',
				            resultData,
				            null,
				            'favoriteSurvey_' + resultData.ID,
				            true);
		        });
			});
		}
	}

    //
    function onMetaDataComplete() {
        
	    svcSurveyEditor.getStatus(
	        _at,
	        surveyData.Id)
	        .then(function(result) {
	            $('#surveyStatusDates').text(result);
	        });

        if(surveyData.FormEditPermission) {
            $('#surveyName').hover(function(){
                $(this).addClass('hover');
            }, function(){
                $(this).removeClass('hover');
            });
        }
    }

    //
    function bindActionClickEvents(){
        //Delete Link
		$('#deleteSurveyLink').click(function() { onDeleteSurveyClick(); });
		//Export Link
		$('#exportSurveyLink').click(function() { onExportSurveyClick(); });
		//Bind dash link clicks
		$('a[surveyDashLink]').click(function(event){onDashLinkClick($(this), event);});
    }

	//Handle dialog close and reload survey dashboard
	function onDialogClosed(args){
		if(args == null){
				return;
		}

		if (args.op == "refreshInvitations"){
			refreshInvitations();
			return;
		}

		if (args.op == "addReport" && typeof(args.url) != 'undefined' && args.url != null){
			//Redirect to new page
			location = args.url;
			return;
		}               
		
		if (args.op == "moveOrCopySurvey"){
			<% if (!String.IsNullOrEmpty(OnSurveyMoved))
			   {%>
			   <%=OnSurveyMoved %>();
			<%
			   }%>
			return;
		}

	    //No reload
	    if(typeof(args.noReload) != 'undefined' && args.noReload == true) {
	        return;
	    }

		//Reload dash
		loadSurveyData(_dashSurveyId, true)
        .then(function(){
            //Select "Reports" tab if needed
            if (args.op == "addReport" && args.showReportsTab == "true"){
               $("#reportsTab a").click();
            }
        });
	}

	//
	function onDashLinkClick(target, event){
		showDialog(getSurveyLink(target), target.attr('windowName'));
		event.stopPropagation();
	}

	//
	function getSurveyLink(target){
	    var link = target.attr('surveyDashLink').replace('${Id}', _dashSurveyId);
	    
	    if(link.indexOf('?') > 0) {
	        link = link + '&onClose=onDialogClosed';
	    }
	    else {
	        link = link + '?onClose=onDialogClosed';
	    }

	    return link;
	}

	function onExportSurveyClick(){
		if(_dashSurveyId == null || _dashSurveyId == ''){
			return;
		}

		__doPostBack('export', _dashSurveyId);
	}

    //Show confirm dialog about deleting a survey
	function onDeleteSurveyClick() {
		if(_dashSurveyId == null || _dashSurveyId == ''){
			return;
		}
		showConfirmDialogWithCallback(
		'<%=WebTextManager.GetText("/pageText/forms/manage.aspx/deleteSurveyConfirm") %>',
		 deleteSurveyConfirmCallback,
		 350,
		 200,
		 '<%=WebTextManager.GetText("/pageText/forms/manage.aspx/confirmDeleteSurvey") %>');
	}

	//Callback for confirm dialog about deleting a survey
	function deleteSurveyConfirmCallback(args){
        if(args.success){
			svcSurveyManagement.deleteSurvey(_at, _dashSurveyId, surveyDeletedCallback, null);
		}
	}

	//Handle survey deleted
	function surveyDeletedCallback(resultData){
		var message;
		if (resultData)
		{		
			message = '<%=WebTextManager.GetText("/pageText/forms/manage.aspx/surveyDeleted") %>';

			//Bind survey delete event, if any specified
			<% if(!string.IsNullOrEmpty(OnSurveyDeleted)) { %>
			   <%=OnSurveyDeleted %>(_dashSurveyId); 
			<%} %>  
            if (typeof(timeline) != 'undefined')
            {
                timeline.setParentObject(0, 'SurveyManager', 'SURVEY');
            }
		}
		else
		{
			message = '<%=WebTextManager.GetText("/pageText/forms/manage.aspx/oneSurveyCouldNotDelete") %>';
		}

		<% if (!String.IsNullOrEmpty(ShowStatusMessageHandler))
		   {%>
		   <%=ShowStatusMessageHandler%>(message, resultData);
		<%
		   }%>              
	}
	
     function bindFavoriteSurveyClickEvent() {
	     $("#survey_favorite_button").click(function(event) { onFavoriteButtonClick($(this), event); });
	     
	     //check if current survey is favorite;
	     svcSurveyManagement.IsFavoriteSurvey(_at, _dashSurveyId, {  })
    	     .then(function(result) {
    	         if (result) {
    	             $("#survey_favorite_button").addClass("favorite");
    	         } else {
    	             $("#survey_favorite_button").removeClass("favorite");
    	         }
    	     });
	 }

	 function onFavoriteButtonClick(target, event) {
         if (!$("#survey_favorite_button").hasClass("favorite")) {
             svcSurveyManagement.AddFavoriteSurvey(_at, _dashSurveyId,  { })
                 .then(function (){$("#survey_favorite_button").addClass("favorite");
                      $("#favoriteSurveys").empty();
                     //reload list of favorite surveys
                      getFavoriteSurveysFromServer();
             });
              
         } else {
            svcSurveyManagement.RemoveFavoriteSurvey(_at, _dashSurveyId,  { })
                .then(function () {
                    $("#survey_favorite_button").removeClass("favorite");
                    $("#favoriteSurveys").empty();
                     //reload list of favorite surveys
                       getFavoriteSurveysFromServer();
                });
         }
    }
</script>

<%-- Container for Results --%>
<div id="detailProgressContainer_<%=ID %>" style="display: none;margin-top:10px;">
    <div id="detailProgress_<%=ID %>" style="text-align: center;">
        <p><%=WebTextManager.GetText("/common/loading")%></p>
        <p><asp:Image ID="_progressSpinner" runat="server" SkinID="ProgressSpinner" /></p>
    </div>
</div>
<div id="infoPlace">
    <div class="introPage">
    </div>
</div>

<script type="text/C#" runat="server">
    /// <summary>
    /// Get/set callback for handling survey delete event
    /// </summary>
    public string OnSurveyDeleted { get; set; }

    /// <summary>
    /// Get/set callback for handling survey moved/copied event
    /// </summary>
    public string OnSurveyMoved { get; set; }

    /// <summary>
    /// Get/set handler for showing status message.
    /// The first parameter must be a message.
    /// The second parameter must determine if an operation was succeeded or not.
    /// </summary>
    public string ShowStatusMessageHandler { get; set; }

    /// <summary>
    /// Does Provider support batches
    /// </summary>
    public bool ProviderSupportsBatches
    {
        get { return EmailGateway.ProviderSupportsBatches; }
    }
    
    /// <summary>
    /// 
    /// </summary>
    public bool AllowInvitations
    {
        get
        {
            return //ApplicationManager.AppSettings.AllowInvitations &&  -- Limits not in yet, update when added
                   ApplicationManager.AppSettings.EmailEnabled;
        }
    }

    /// <summary>
    /// Override OnLoad to ensure necessary scripts are loaded.
    /// </summary>
    /// <param name="e"></param>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        //RegisterClientScriptInclude(
        //    "serviceHelper.js",
        //    ResolveUrl("~/Services/js/serviceHelper.js"));

        //RegisterClientScriptInclude(
        //    "svcSurveyManagement.js",
        //    ResolveUrl("~/Services/js/svcSurveyManagement.js"));

        //RegisterClientScriptInclude(
        //   "svcAuthorization.js",
        //   ResolveUrl("~/Services/js/svcAuthorization.js"));

        //RegisterClientScriptInclude(
        //   "svcResponseData.js",
        //   ResolveUrl("~/Services/js/svcResponseData.js"));

        //RegisterClientScriptInclude(
        //   "svcReportManagement.js",
        //   ResolveUrl("~/Services/js/svcReportManagement.js"));

        //RegisterClientScriptInclude(
        //  "svcInvitationManagement.js",
        //  ResolveUrl("~/Services/js/svcInvitationManagement.js"));

        //RegisterClientScriptInclude(
        //    "templateHelper.js",
        //    ResolveUrl("~/Resources/templateHelper.js"));

        //RegisterClientScriptInclude(
        //    "dateUtils.js",
        //    ResolveUrl("~/Resources/dateUtils.js"));

        //RegisterClientScriptInclude(
        //    "jquery.ckbxProtect.js",
        //    ResolveUrl("~/Resources/jquery.ckbxProtect.js"));

        //RegisterClientScriptInclude(
        //  "securityHelper.js",
        //  ResolveUrl("~/Resources/securityHelper.js"));

        RegisterClientScriptInclude(
         "svcSurveyEditor.js",
         ResolveUrl("~/Services/js/svcSurveyEditor.js"));

        //RegisterClientScriptInclude(
        //    "svcStyleManagement.js",
        //    ResolveUrl("~/Services/js/svcStyleManagement.js"));

        RegisterClientScriptInclude(
         "jquery.ckbxEditable.js",
         ResolveUrl("~/Resources/jquery.ckbxEditable.js"));

        RegisterClientScriptInclude(
            "tooltip.js",
            ResolveUrl("~/Resources/tooltip.js"));
        //RegisterClientScriptInclude(
        //    "jquery.numeric.js",
        //    ResolveUrl("~/Resources/jquery.numeric.js"));
    }
</script>
