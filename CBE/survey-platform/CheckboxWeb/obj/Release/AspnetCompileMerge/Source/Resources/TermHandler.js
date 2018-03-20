/****************************************************************************
 * termHandler.js                                                            *
 *   Client controller for piping functionality.  Relies on other components *
 *   such as svcSurveyManagement.js, jquery.a-tools-1.5.2.min.js             *
 ****************************************************************************/
var termHandler = new termHandlerObj();

function termHandlerObj() {
    this.responseTemplateId = null;
    this.termPrefix = null;
    this.boundButton = null;
    this.initialized = false;

    this.initialize = function(responseTemplateId, maxPagePosition, languageCode, termPrefix) {
        //Only initialize once
        if (termHandler.initialized) {
            return;
        }

        termHandler.initialized = true;
        termHandler.responseTemplateId = responseTemplateId;
        termHandler.termPrefix = termPrefix;

        $('select[termTarget]').each(function () {
            //Find merge button
            var selectId = $(this).attr('id');
            var mergeButton = $("a[mergefor='" + selectId + "']");

            if (mergeButton.length > 0) {
                termHandler.bind($(this).attr('termTarget'), selectId, mergeButton.attr('id'));
            }
        });
    };

    ////Bind target with dropDown
    this.bind = function(targetId, dropDownId, buttonId) {
        //alert('unbind');

        //Unbind previous handlers
        $('#' + buttonId).unbind('click');

        //Bind to new
        $('#' + buttonId).bind('click', function() {
            //if target text input is disabled, then return.
            if ($("#" + targetId).attr('disabled'))
                return false;
            termHandler.handle(targetId, dropDownId);
            return false;
        });
    };

    //Handle piping operation
    this.handle = function (targetId, dropDownId) {
        $("#" + dropDownId + " option:selected").each(function() {
            var value = $(this).attr('value');

            if (value != '__NONE__' && value != '0') {
                //Insert
                value = termHandler.createTermText(value);
                termHandler.insertAtCursor(targetId, value);

                //Fire change event
                $('#' + targetId).change();
            }
        });
    };

    ////Create term text. If it is necessary, add response term to the survey.
    this.createTermText = function(value) {
        //Check if the value starts with [I]
        var regex = new RegExp("^(\\[I\\])");
        if (value.match(regex)) {
            value = value.substr(3);
            //Remove termPrefix and trim starting 0.
            var trimmedValue = value.replace(termHandler.termPrefix, '').replace(/^0+/, '');
            //svcSurveyManagement.addResponsetermToSurvey(_at, termHandler.responseTemplateId, trimmedValue, value);
        }
        return termHandler.termPrefix + value;
    };

    ////Insert text at cursor position of the target. If it is necessary, replace the selection with this text.
    this.insertAtCursor = function(targetId, textValue) {
        var targetField = $('[id$="'+ targetId +'"]');

        if (targetField.attr('tinyMCE')) {
            tinymce.get(targetField.attr("Id")).execCommand("mceInsertContent", false, textValue);
        } else {
            if (targetField.getSelection().length > 0)
                targetField.replaceSelection(textValue);
            else
                targetField.insertAtCaretPos(textValue);
        }
    };
 
};