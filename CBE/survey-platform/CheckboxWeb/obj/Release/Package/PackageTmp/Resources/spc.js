var SPCManager = new SPCManagerObj();

function SPCManagerObj() {
    this._url = "Survey.aspx";
    this._root = "";
    this._xScroll = 0;
    this._yScroll = 0;

    this.enabled = false;

    //initialize Same Page Condition Manager
    this.initSPC = function (url, root) {
        SPCManager._url = url;
        SPCManager._root = root;
        SPCManager.subscribeHandlers();
    },

    this.subscribeHandlers = function (url) {
        $(document).on("change", '[spcMarker="true"] input', function () { SPCManager.postSurveyForm(); });
        $(document).on("change", '[spcMarker="true"] select', function () { SPCManager.postSurveyForm(); });
        $(document).on("change", 'select[spcMarker="true"]', function () { SPCManager.postSurveyForm(); });
    },

    /*
    this.unsubscribeHandlers = function (url) {
        $(document).off("change", '[spcMarker="true"] input');
        $(document).off("change", '[spcMarker="true"] select');
        $(document).off("change", 'select[spcMarker="true"]');
    },*/

    this.saveTinyMCE = function () {
        if (typeof (tinymce) != 'undefined') {
            $(tinymce.editors).each(function (index, inst) {
                try {
                    if (!($('#' + inst.id.replace(new RegExp('_questionText$'), '_currentEditMode')).val().toLowerCase() == 'textarea')) {
                        inst.save();
                    }
                }
                catch (ee) //suppress strange exception from jquery
                {
                }
            });
        } 
    },

    //post data back to server
    this.postSurveyForm = function () {
        if (!SPCManager.enabled)
            return;

        var formData = new Object();

        //mark request as SPC
        formData["action"] = "spc";

        SPCManager.saveTinyMCE();

        //save scroll positions
        SPCManager.saveScroll();

        $('form')
	        .find("input:checked, input[type='text'], input[type='hidden'], input[type='password'], input[type='number'], input[type='tel'], input[type='date'], input[type='url'], option:selected, textarea")
            .filter(":not(:disabled)")
	        .each(function () {
	            formData[this.name || this.id || this.parentNode.name || this.parentNode.id || this.parentNode.parentNode.name || this.parentNode.parentNode.id] = this.value;
	        });

        $('.surveyContentFrame').empty();
        $('.surveyContentFrame').html("<div class='surveyButtonsContainer'><img src=\"" + SPCManager._root + "App_Themes/CheckboxTheme/Images/ProgressSpinner.gif\" Width=\"31\" Height=\"31\"></img></div>");

        $.ajax({
            url: SPCManager._url,
            type: 'POST',
            dataType: 'html',
            data: formData,
            success: function (data) {

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
                        $('form').hide().empty().append(el.innerHTML);
                        if (typeof (applyUniform) == 'function') {
                            applyUniform();
                        }
                        $('form').show();
                        if (typeof (fixSelectWidth) == 'function') {
                            fixSelectWidth();
                        }
                        if (typeof (fixIFrames) == 'function') {
                            fixIFrames();
                        }
                        if (typeof (fixTableProperties) == 'function') {
                            fixTableProperties();
                        }
                        if (typeof (fixForBrokenSliderMarginsWithSPC) == 'function') {
                            fixForBrokenSliderMarginsWithSPC();
                        }
                        if (typeof (fixSliderHeight) == 'function') {
                            fixSliderHeight($('.slider-value-list-container'));
                        }
                    }
                });

                SPCManager.restoreScroll();
            },
            error: function (data) {
                alert('Failed to post form data');
            }
        });
    };

    this.saveScroll = function () {
        var d = document.documentElement;
        SPCManager._xScroll = (window.pageXOffset || d.scrollLeft) - (d.clientLeft || 0);
        SPCManager._yScroll = (window.pageYOffset || d.scrollTop) - (d.clientTop || 0);        
    };

    this.restoreScroll = function () {
        setTimeout(function(){ window.scrollTo(SPCManager._xScroll, SPCManager._yScroll);}, 50);
    };
}