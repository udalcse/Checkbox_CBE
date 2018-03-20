var surveyWorkflow = new SurveyWorkflowObj();

function SurveyWorkflowObj() {
    //private members
    var _self = this;

    var _url;
    var _root;
    var _isMobile;

    var _asyncWorkerInProgress = false;

    //public member
    this.IsSpcEnabled = false;

    //public method
    this.init = function (url, root, isMobile) {
        _url = url;
        _root = root;
        _isMobile = isMobile;
        
        subscribeNavigationHandlers();
    };

    //public method
    this.makePostAction = function (action, resetScroll) {
        if (typeof action != "undefined" && action != null && action != '')
            postFormData(action, resetScroll);

        return false;
    };

    //public method
    this.makeGetAction = function (action, resetScroll) {
        if (typeof action != "undefined" && action != null && action != '')
            getData(action, resetScroll);

        return false;
    };

    //private method
    var saveTinyMCE = function () {
        if (typeof(tinymce) != 'undefined') {
            $(tinymce.editors).each(function(index, inst) {
                try {
                    if (!($('#' + inst.id.replace(new RegExp('_questionText$'),
                        '_currentEditMode')).val().toLowerCase() == 'textarea')) 
                        inst.save();
                } catch (ee) { } //suppress strange exception from jquery
            });
        }
    };

    //private method
    var subscribeNavigationHandlers = function () {
        $(document).on('click', 'input.workflowAjaxPostAction', function (e) {
            e.preventDefault();

            var action = $(this).attr('data-action');
            //validate only on next and finish - disable validation on save && back buttons
            var shouldValidate = action === "next" || action === "finish";

            if (shouldValidate) {
                if (validateControls()) {
                    _self.makePostAction(action, true);

                    //move user screen to errors
                    var errors = $("[id*='errorPanel']");

                    if (errors.length) {
                        $('html, body').animate({
                            scrollTop: errors.first().offset().top
                        }, 0);
                    }

                    return false;
                }
            } else {
                _self.makePostAction(action, true); return false;
            }

        });
        $(document).on('change', 'select.workflowAjaxPostAction', function (e) { e.preventDefault(); _self.makePostAction($(this).attr('data-action'), true); return false; });

        $(document).on('click', '.workflowAjaxGetAction', function (e) { e.preventDefault(); _self.makeGetAction($(this).attr('href'), true); return false; });

        //subscribe spc
        $(document).on("change", '[spcMarker="true"] input, [spcMarker="true"] select, select[spcMarker="true"]', 
            function (e) { e.preventDefault(); _self.makePostAction('spc'); return false; });
    };

    //private method
    var getData = function (url, resetScroll) {
        if (_asyncWorkerInProgress)
            return false;

        //set flag to avoid multiple postbacks
        _asyncWorkerInProgress = true;
        runProgressSpinner(true);

        $.ajax({
            url: url,
            type: 'GET',
            headers: { "cache-control": "no-cache" }, //instruction for iOS
            success: function (data) {
                onSuccess(data, resetScroll);
                _url = url;
            },
            error: function (statusCode) {
                if (statusCode.status == 0 || !window.navigator.onLine)
                    alert("You have lost internet connection and cannot submit this page, please try again");
                else
                    alert('Failed to get page data');
            },
            complete: function() {
                _asyncWorkerInProgress = false;
                stopProgressSpinner();
            }
        });

        return false;
    };

    //private method
    //post data back to server
    var postFormData = function (action, resetScroll) {
        if (_asyncWorkerInProgress)
            return false;

        //set flag to avoid multiple postbacks
        _asyncWorkerInProgress = true;

        //don't run spinner for spc actions
        runProgressSpinner(action != 'spc');

        var formData = new Object();
        
        saveTinyMCE();

        //set action
        formData["action"] = action;

        $('form')
            .find("input:checked, input[type='text'], input[type='hidden'], input[type='password'], input[type='number'], input[type='tel'], input[type='date'], input[type='url'], option:selected, textarea")
            .filter(":not(:disabled)")
            .each(function () {
                formData[this.name || this.id || this.parentNode.name || this.parentNode.id || this.parentNode.parentNode.name || this.parentNode.parentNode.id] = this.value;
            });

        $.ajax({
            url: _url,
            type: 'POST',
            headers: { "cache-control": "no-cache" }, //instruction for iOS
            dataType: 'html',
            data: formData,
            success: function (data) {
                onSuccess(data, resetScroll);
            },
            error: function (statusCode) {
                if (statusCode.status == 0 || !window.navigator.onLine)
                    alert("You have lost internet connection and cannot submit this page, please try again");
                else
                    alert('Failed to post form data');
            },
            complete: function () {
                _asyncWorkerInProgress = false;
                stopProgressSpinner();
            }
        });

        return false;
    };

    //private method
    var runProgressSpinner = function (showSpinner) {
        var spinnerMarkup = '<img src="' + _root + 'App_Themes/CheckboxTheme/Images/ProgressSpinner.gif" >';
        $('form').append('<div class="progressScreen"><div />' + (showSpinner? spinnerMarkup : '') + '</div>');
    };

    //private method
    var stopProgressSpinner = function () {
        $('form .progressScreen').remove();
    };

    //private method
    var onSuccess = function (data, resetScroll) {
        //clear all redirect timeouts
        var ids = window.redirectTimeoutIds;
        if (ids) {
            for (var i = 0; i < ids.length; i++) {
                clearTimeout(ids[i]);
            }
        }

        //remove previously initialized editors
        if (typeof (tinymce) != 'undefined') {
            var i = 0;
            tinymce.editors = [];
            while (tinymce.editors.length > 0 && i <= tinymce.editors.length) {
                tinyMCE.execCommand("mceRemoveEditor", false, tinymce.editors[tinymce.editors.length - 1].id);
                i++;
            }
        }
        
        //parse recieved document
        var result = $.parseHTML(data, document, true);
        $.each(result, function (i, el) {
            //replace form content with the fresh one
            if (typeof (el.tagName) != 'undefined' && el.tagName.toLowerCase() == 'form') {
                if (isRedirect(el))
                    return;
                var currentPosition = $(window).scrollTop();
                handleHtml(el.innerHTML, resetScroll);

                if (typeof (fixSelectWidth) == 'function') 
                    fixSelectWidth();
                if (typeof (fixIFrames) == 'function') 
                    fixIFrames();
                if (typeof (fixTableProperties) == 'function') 
                {
                    fixTableProperties();
                    //$(window).scrollTop(currentPosition + 32);
                }
                if (typeof (fixSliderHeight) == 'function') 
                    fixSliderHeight($('.slider-value-list-container'));
            }
        });
    };

    //private method
    var handleHtml = function (html, resetScroll) {
        if (typeof resetScroll != "undefined" && resetScroll)
            window.scrollTo(0, 0);

        var currentPosition = $(window).scrollTop();

        if (_isMobile) {
            $('form').empty();
            $('form').hide().html(html);

            if (typeof (applyUniform) == 'function')
                applyUniform();
            $('form').show();

            $(window).scrollTop(currentPosition);
        } else {
            //replace old html
            $('form').html(html);

            //apply jquery mobile
            if (typeof (applyUniform) == 'function')
                applyUniform();

            $(window).scrollTop(currentPosition);
        }
    };

    //private method
    var isRedirect = function (form) {
        var redirect = $(form).find('.automatic-redirect');
        if (typeof redirect != "undefined" && redirect != null && typeof redirect.attr('href') != "undefined") {
            window.location.href = redirect.attr('href');
            return true;
        }
        return false;
    };


    function validateMultiLineFields() {
        var isValid = true;
   
        var textAreas = $(".inputContainer textarea[bindedfield='true']");

        if (textAreas.length === 0) {
            return isValid;
        }

        textAreas.each(function (index, value) {

            var errorMessage;
            var inputValue;

            if (_isMobile) {
                errorMessage = $(value).parents(".inputContainer").find(".validationError");
            } else {
                errorMessage = $(value).siblings('.validationError');
            }

            if ($(value).attr("tinymce") === "true") {
                inputValue = $(value).siblings(".mce-container").find("iframe").contents().find("body").text();
            } else {
                inputValue = $(value).val();
            }

            $(errorMessage).css("display", "none");

            if (inputValue.length < 2) {
                $(errorMessage).css("display", "block");
                isValid = false;
            }
        });

        return isValid;
    }

    function validateSingleLineFields() {
        var isValid = true;
        var singleLineElements = $("input[type='text'][id*='textInput'][binded-field='true']");
        if (singleLineElements.length == 0) return isValid;

        singleLineElements.each(function (index, value) {
            var errorMessage = $(value).siblings('.validationError');

            if (_isMobile) {
                errorMessage = $(value).parent().siblings('.validationError');
            }

            $(errorMessage).css("display", "none");
            if ($(value).val().length == 0) {
                $(errorMessage).css("display", "block");
                isValid = false;
            }
        });
        return isValid;
    }

    function validateRadioButtonFields() {
        var isValid = true;
        var radioButtonsPanel = $(".radioButtonList[bindedfield='true']");
        radioButtonsPanel.each(function (index, value) {
            var errorMessage = $(value).find(".validationError");
            $(errorMessage).css("display", "none");

            var checkedRadioButton = $(value).find("input[type='radio']").parent(".checked");

            if (_isMobile) {
                checkedRadioButton = $(value).find(".ui-icon-radio-on");
            }

            if (checkedRadioButton.length == 0) {
                $(errorMessage).css("display", "block");
                isValid = false;
            }
        });
        return isValid;
    }
   
    function validateControls() {
        if (!validateSingleLineFields() || !validateMultiLineFields() || !validateRadioButtonFields()) {

            // focus user on errror
            if ($(".validationError").length) {
                var validationMessages = $(".validationError").filter(function () {
                    return this.style.display === 'block';
                });

                if (validationMessages.length) {

                    $('html, body').animate({
                        scrollTop: validationMessages.first().offset().top
                    }, 0);
                }
            }

            return false;
        }

        return true;
    }
}