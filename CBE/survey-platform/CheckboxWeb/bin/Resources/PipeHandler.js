/****************************************************************************
 * PipeHandler.js                                                            *
 *   Client controller for piping functionality.  Relies on other components *
 *   such as svcSurveyManagement.js, jquery.a-tools-1.5.2.min.js             *
 ****************************************************************************/
var pipeHandler = new pipeHandlerObj();

function pipeHandlerObj() {

    this.responseTemplateId = null;
    this.pipePrefix = null;
    this.boundButton = null;
    this.initialized = false;

    //Initialize the control, load pipes, and bind data
    this.initialize = function(responseTemplateId, maxPagePosition, languageCode, pipePrefix) {

        //Only initialize once
        if (pipeHandler.initialized) {
            return;
        }

        pipeHandler.initialized = true;
        pipeHandler.responseTemplateId = responseTemplateId;
        pipeHandler.pipePrefix = pipePrefix;

        //Ensure pipe inputs exist before loading
        if ($('select[pipeTarget]').length > 0) {
            $('select[pipeTarget] option[value="__NONE__"]').text('Loading...');

            svcSurveyEditor.listPipeSourcesD(_at, responseTemplateId, maxPagePosition, languageCode).then(pipeHandler.onPipesLoaded);
        }
    };

    //Handle pipes loaded
    this.onPipesLoaded = function(pipeList) {
        var optGroups = new Array();
        var defaultOption = '';
        var currentType = '';
        var currentOptGroup = null;

        //Build opt groups
        for (var i = 0; i < pipeList.length; i++) {
            if (i == 0) {
                defaultOption = pipeList[i].DisplayText;
            }

            if (pipeList[i].SourceType != currentType) {
                if (currentOptGroup != null) {
                    optGroups.push(currentOptGroup);
                }

                currentType = pipeList[i].SourceType;
                currentOptGroup = $('<optgroup />');
                currentOptGroup.attr('label', pipeList[i].SourceType);
            }

            //Add option to opt group
            $('<option/>').val(pipeList[i].PipeToken).text(pipeList[i].DisplayText).appendTo(currentOptGroup);
        }

        //HadleMatrixChange();
        //Ensure last opt group added to array
        if (currentOptGroup != null) {
            optGroups.push(currentOptGroup);
        }

        //Now add opt groups to each select
        $('select[pipeTarget]').each(function() {

            //Add options and bind
            for (var j = 0; j < optGroups.length; j++) {
                optGroups[j].clone().appendTo($(this));
            }

            //Find merge button
            var selectId = $(this).attr('id');
            var mergeButton = $("a[mergefor='" + selectId + "']");

            if (mergeButton.length > 0) {
                pipeHandler.bind($(this).attr('pipeTarget'), selectId, mergeButton.attr('id'));
            }

            $("#uniform-" + $(this).attr('id') + " span").html(defaultOption);
        });

        $('select[pipeTarget] option[value="__NONE__"]').remove();
    };

    //Bind target with dropDown
    this.bind = function(targetId, dropDownId, buttonId) {
        //alert('unbind');

        //Unbind previous handlers
        $('#' + buttonId).unbind('click');

        //Bind to new
        $('#' + buttonId).bind('click', function() {
            //if target text input is disabled, then return.
            if ($("#" + targetId).attr('disabled'))
                return false;
            pipeHandler.handle(targetId, dropDownId);
            return false;
        });
    };

    //Handle piping operation
    this.handle = function(targetId, dropDownId) {
        $("#" + dropDownId + " option:selected").each(function() {
            var value = $(this).attr('value');
            value = pipeHandler.createPipeText(value);

            if (value != '__NONE__') {
                //Insert
                pipeHandler.insertAtCursor(targetId, value);

                //Fire change event
                $('#' + targetId).change();
            }
        });
    };

    //Create pipe text. If it is necessary, add response pipe to the survey.
    this.createPipeText = function(value) {
        //Check if the value starts with [I]
        var regex = new RegExp("^(\\[I\\])");
        if (value.match(regex)) {
            value = value.substr(3);
            //Remove pipePrefix and trim starting 0.
            var trimmedValue = value.replace(pipeHandler.pipePrefix, '').replace(/^0+/, '');
            svcSurveyManagement.addResponsePipeToSurvey(_at, pipeHandler.responseTemplateId, trimmedValue, value);
        }
        return value;
    };

    //Insert text at cursor position of the target. If it is necessary, replace the selection with this text.
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