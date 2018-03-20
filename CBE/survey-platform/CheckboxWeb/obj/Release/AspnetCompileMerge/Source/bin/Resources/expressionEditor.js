var expressionEditor = new expressionEditorObj();

////////////////////////////////////////////////////////////////////////////////
// Container for expression editor routines.  Requires jQuery.                //
////////////////////////////////////////////////////////////////////////////////
function expressionEditorObj() {

    this.appRoot = '/';
    this.authToken = '';
    this.expressionEditors = new Array();

    ///////////////////////////////////////////////////////////////////////////
    // Initialize with application root url of child action window.          //                                                    
    ///////////////////////////////////////////////////////////////////////////
    this.initialize = function (authToken, appRoot, id, params) {
        params.ID = id;
        expressionEditor.appRoot = appRoot;
        expressionEditor.authToken = authToken;
        expressionEditor.expressionEditors[id] = params;
        expressionEditor.load(id);
        $(document).off('change', '.branchTargetSelector');
        $(document).on('change', '.branchTargetSelector', function (event) {
            expressionEditor.onBranchTargetPageChanged(event);
        });
    };

    this.load = function (id) {
        $('#' + id).empty();

        var params = expressionEditor.expressionEditors[id];

        if (params.AddExpressionMode) {
            templateHelper.loadAndApplyTemplate(
            'addExpressionTemplate.html',
            expressionEditor.appRoot + 'Forms/jqtmpl/addExpressionTemplate.html',
            {},
            {},
            $('#' + id),
            true,
            expressionEditor.onExpressionRendered,
            id);
        }
        else {
            //reduce number of calls to service via delayed rendering of blocks that consumes the same data
            var requestedEditor = expressionEditor.getEditorWithActiveRequest(params);
            if (!requestedEditor) {
                //do request
                params.Requested = true;

                if (params.RuleType == 'PageCondition') {
                    svcSurveyManagement.getPageConditions(expressionEditor.authToken, params.ResponseTemplateId, params.DependentPageId, expressionEditor.onExpressionReceived, id);
                } else if (params.RuleType == 'PageBranchCondition') {
                    svcSurveyManagement.getPageBranches(expressionEditor.authToken, params.ResponseTemplateId, params.DependentPageId, expressionEditor.onExpressionReceived, id);
                } else if (params.RuleType == 'ItemCondition') {
                    svcSurveyManagement.getItemConditions(expressionEditor.authToken, params.ResponseTemplateId, params.DependentItemId, expressionEditor.onExpressionReceived, id);
                }
                params.EditorWithActiveRequest = params;
            }
            else
            {
                params.EditorWithActiveRequest = requestedEditor;
            }
        }

    };

    this.getEditorWithActiveRequest = function (editor) {
        for (var i in expressionEditor.expressionEditors) {
            var e = expressionEditor.expressionEditors[i];
            if (e.Requested) {
                if (editor.RuleType == e.RuleType && editor.ID != e.ID
                    && editor.ResponseTemplateId == e.ResponseTemplateId
                    && editor.MaxSourceQuestionPagePosition == e.MaxSourceQuestionPagePosition
                    && editor.DependentItemId == e.DependentItemId
                    && editor.DependentPageId == e.DependentPageId
                    )
                    return e;
            }
        }
    };

    this.getEditorTemplateFileName = function () {
        return 'expressionEditorTemplate.html';
    };

    this.onExpressionReceived = function (data, id) {
        if (data == null)
            data = {};

        var editor = expressionEditor.expressionEditors[id];

        for (var i in expressionEditor.expressionEditors) {
            if (expressionEditor.expressionEditors[i].EditorWithActiveRequest === editor) {
                var currentEditor = expressionEditor.expressionEditors[i];

                currentEditor.EditorWithActiveRequest = null;

                
                var realData = data;
                
                //filter rules
                var ruleId = $('#' + currentEditor.ID).closest('.ruleEditorWrapper').attr('ruleId');
                var rule = null;
                if (typeof (ruleId) != 'undefined' && ruleId) {
                    for (var i = 0; i < data.length; i++) {
                        if (data[i].RuleId == ruleId) {
                            rule = data[i];
                        }
                    }
                }
                if (rule) {
                    realData = [rule];
                }

                templateHelper.loadAndApplyTemplate(
                currentEditor.Manager.getEditorTemplateFileName(),
                expressionEditor.appRoot + 'Forms/jqtmpl/' + currentEditor.Manager.getEditorTemplateFileName(),
                realData,
                {},
                $('#' + currentEditor.ID),
                true,
                expressionEditor.onExpressionRendered,
                currentEditor.ID);

                //save root expression id
                currentEditor.rootExpressionId = data.RootExpressionId;
            }
        }

        if (typeof editor != "undefined")
            editor.Requested = false;
    };

    this.onExpressionRendered = function (id) {
        $('.expressionRightParamDate').datepicker();
        $('.expressionRightParamDate').datepicker("option", "onSelect", function (dateText, inst) {
            expressionEditor.onRightParamDateChange(dateText, inst);
        });        
    };

    /*--------------------------------------------------------------------------------------------------------*/
    /*---------------------------     Add expression ...                     ---------------------------------*/
    /*--------------------------------------------------------------------------------------------------------*/


    this.onAddExpressionClick = function (event) {
        event = event || window.event;
        $(event.target).siblings('.expressionEditorPanel').find('.expressionLeftParam').empty().hide();
        $(event.target).siblings('.expressionEditorPanel').find('.expressionOperator').empty().hide();
        $(event.target).siblings('.expressionEditorPanel').find('.expressionRightParamSelect').empty().hide();
        $(event.target).siblings('.expressionEditorPanel').find('.addNewExpressionButton').hide();

        $(event.target).hide();
        $(event.target).next().show();
    };

    this.onLeftParamTypeChange = function (event, expressionId) {
        event = event || window.event;
        if (expressionId == '')
            expressionId = 0;

        var containerId = $(event.target).closest('.expressionEditorBase').attr('id');
        var editor = expressionEditor.expressionEditors[containerId];
        if (typeof (editor) == 'undefined') {
            return;
        }

        var container = $(event.target).closest('.expressionEditorPanel');
        container.find('.expressionOperator').hide();
        container.find('.expressionRightParamSelect').hide();
        container.find('.expressionRightParamText').hide();
        container.find('.expressionRightParamDate').hide();
        container.find('.addNewExpressionButton').hide();
        container.find('.expressionLeftParam').hide();

        if ('SourceTypeNotSpecified' == $(event.target).children().filter(":selected").val())
            return;

        svcSurveyManagement.getExpressionLeftParamByTypeAndRuleType(expressionEditor.authToken, editor.ResponseTemplateId, editor.DependentItemId == "" ? 0 : editor.DependentItemId, $(event.target).children().filter(":selected").val(),
            editor.RuleType, editor.MaxSourceQuestionPagePosition, editor.LanguageCode, expressionEditor.onLeftParamByTypeReceived, { editor: editor, container: container });
    };

    this.onLeftParamByTypeReceived = function (data, callbackArgs) {
        callbackArgs.container.find('.expressionLeftParam').empty();
        callbackArgs.container.find('.expressionLeftParam').populate(data.NameValueList, 'SOURCE', textHelper.getTextValue("/expressionEditor/noSource", "No Source Available"));
        callbackArgs.container.find('.expressionLeftParam').show();
    };

    this.onLeftParamChange = function (event) {
        event = event || window.event;
        var containerId = $(event.target).closest('.expressionEditorBase').attr('id');
        var editor = expressionEditor.expressionEditors[containerId];
        if (typeof (editor) == 'undefined') {
            return;
        }

        var container = $(event.target).closest('.expressionEditorPanel');

        container.find('.expressionOperator').hide();
        container.find('.expressionRightParamSelect').hide();
        container.find('.expressionRightParamText').hide();
        container.find('.expressionRightParamDate').hide();
        container.find('.addNewExpressionButton').hide();

        if ('NotSelected' == $(event.target).children().filter(":selected").val())
            return;

        svcSurveyManagement.getExpressionOperators(expressionEditor.authToken, editor.ResponseTemplateId,
            container.find('.expressionLeftParamType').val(),
            $(event.target).children().filter(":selected").val(),
            editor.MaxSourceQuestionPagePosition, editor.LanguageCode, expressionEditor.onExpressionOperatorsReceived, { editor: editor, container: container });
    };

    this.onExpressionOperatorsReceived = function (data, callbackArgs) {
        callbackArgs.container.find('.expressionOperator').empty();
        callbackArgs.container.find('.expressionOperator').populate(data.NameValueList, 'OPERATOR', textHelper.getTextValue("/expressionEditor/noOperators", "No Operators Available"));
        callbackArgs.container.find('.expressionOperator').show();
    };

    this.onOperatorChange = function (event) {
        event = event || window.event;
        var containerId = $(event.target).closest('.expressionEditorBase').attr('id');
        var editor = expressionEditor.expressionEditors[containerId];
        if (typeof (editor) == 'undefined') {
            return;
        }

        var container = $(event.target).closest('.expressionEditorPanel');

        container.find('.expressionRightParamSelect').hide();
        container.find('.expressionRightParamText').hide();
        container.find('.expressionRightParamDate').hide();
        container.find('.addNewExpressionButton').hide();

       
        if (container.find('.expressionOperator').val() == 'OPERATOR')
            return;

        svcSurveyManagement.getExpressionRightParams(expressionEditor.authToken, editor.ResponseTemplateId,
            container.find('.expressionLeftParamType').val(),
            container.find('.expressionLeftParam').val(),
            container.find('.expressionOperator').val(),
            editor.MaxSourceQuestionPagePosition, editor.LanguageCode, expressionEditor.onExpressionRightParamsReceived, { editor: editor, container: container });

    };

    this.onExpressionRightParamsReceived = function (data, callbackArgs) {
        callbackArgs.container.find('.expressionRightParamSelect').empty();
        callbackArgs.container.find('.expressionRightParamText').empty();
        callbackArgs.container.find('.expressionRightParamDate').empty();
        callbackArgs.container.find('.expressionRightParamSelect').hide();
        callbackArgs.container.find('.expressionRightParamText').hide();
        callbackArgs.container.find('.expressionRightParamDate').hide();
        if (data.Type == 'Select') {
            callbackArgs.container.find('.expressionRightParamSelect').populate(data.Options.NameValueList, 'VALUE', textHelper.getTextValue("/expressionEditor/value", "Value"));
            callbackArgs.container.find('.expressionRightParamSelect').show();
        } else if (data.Type == 'Date') {
            var containerId = callbackArgs.container.closest('.expressionEditorBase').attr('id');
            var editor = expressionEditor.expressionEditors[containerId];
            if (typeof (editor) == 'undefined') {
                return;
            }

            var format = editor.DateFormat.replace('yyyy', 'yy').toLowerCase();
            var leftParam = callbackArgs.container.find('.expressionLeftParam').find(':selected').val();
            if (leftParam == 'CurrentDateROTW')
                format = "dd/mm/yy";
            else if (leftParam == 'CurrentDateUS')
                format = "mm/dd/yy";

            callbackArgs.container.find('.expressionRightParamDate').datepicker("option", "dateFormat", format);
            callbackArgs.container.find('.expressionRightParamDate').show();
        } else if (data.Type == 'Text') {
            callbackArgs.container.find('.expressionRightParamText').show();
            setTimeout(function () {
                callbackArgs.container.find('.expressionRightParamText').focus().val('');
                callbackArgs.container.find('.addNewExpressionButton').show();
            }, 500);
        } else {
            if (data.Type == 'None') {
                callbackArgs.container.find('.addNewExpressionButton').show();
            } else {
                alert('Bad value type!');
            }
        }
    };

    this.onRightParamSelectChange = function (event) {
        event = event || window.event;
        var containerId = $(event.target).closest('.expressionEditorBase').attr('id');
        var editor = expressionEditor.expressionEditors[containerId];
        if (typeof (editor) == 'undefined') {
            return;
        }

        var container = $(event.target).closest('.expressionEditorPanel');

        var selectedValue = container.find('.expressionRightParamSelect').find(':selected').val();
        if (typeof (selectedValue) != 'undefined' && selectedValue != 'VALUE') {
            container.find('.addNewExpressionButton').prop('data', selectedValue);
            container.find('.addNewExpressionButton').show();
        }
        else
            container.find('.addNewExpressionButton').hide();
    };

    this.onRightParamTextChange = function (event) {
        event = event || window.event;
        var containerId = $(event.target).closest('.expressionEditorBase').attr('id');
        var editor = expressionEditor.expressionEditors[containerId];
        if (typeof (editor) == 'undefined') {
            return;
        }

        var container = $(event.target).closest('.expressionEditorPanel');

        var selectedValue = container.find('.expressionRightParamText').val();

        if (typeof (selectedValue) != 'undefined' && selectedValue.length > 0) {
            container.find('.addNewExpressionButton').prop('data', selectedValue);
            container.find('.addNewExpressionButton').show();
        }
        //else
        //    container.find('.addNewExpressionButton').hide();
    };

    this.onRightParamDateChange = function (dateText, inst) {
        if ((typeof (inst) == 'undefined') && (typeof (event) == 'undefined'))
            return;
        var containerId = $(typeof (inst) == 'undefined' ? event.target : inst.input).closest('.expressionEditorBase').attr('id');
        var editor = expressionEditor.expressionEditors[containerId];
        if (typeof (editor) == 'undefined') {
            return;
        }
        var container = $(typeof (inst) == 'undefined' ? event.target : inst.input).closest('.expressionEditorPanel');

        var selectedValue = container.find('.expressionRightParamDate').val();

        if (typeof (selectedValue) != 'undefined' && selectedValue.length > 0) {
            try {
                var d = new Date(selectedValue);
                container.find('.addNewExpressionButton').prop('data', selectedValue);
                container.find('.addNewExpressionButton').show();
            }
            catch (e) {
                container.find('.addNewExpressionButton').hide();
            }
        }
        else
            container.find('.addNewExpressionButton').hide();
    };

    this.getParentExpressionId = function(event, editor) {
        event = event || window.event;
        var expressionId = $(event.target).attr('expressionId');

        if (typeof(expressionId) == 'undefined' || !expressionId.length)
            expressionId = -1;

        return expressionId;
    };

    this.onAddNewExpressionClick = function (event) {
        event = event || window.event;
        var containerId = $(event.target).closest('.expressionEditorBase').attr('id');
        var editor = expressionEditor.expressionEditors[containerId];
        if (typeof (editor) == 'undefined') {
            return;
        }

        var expressionId = editor.Manager.getParentExpressionId(event, editor);

        var container = $(event.target).closest('.expressionEditorPanel');

        container.find('.addNewExpressionButton').hide();

        var targetPageID = $('#' + containerId).closest('.ruleEditorWrapper').find('.branchTargetSelector').find(':selected').val();
        if (typeof (targetPageID) == 'undefined' || !targetPageID) {
            targetPageID = 0;
        }

        svcSurveyManagement.addExpression(expressionEditor.authToken,
            editor.RuleType,
            editor.DependentItemId == '' ? 0 : editor.DependentItemId,
            editor.DependentPageId == '' ? 0 : editor.DependentPageId,
            targetPageID,
            editor.ResponseTemplateId,
            editor.rootExpressionId,
            expressionId,
            container.find('.expressionLeftParamType').val(),
            container.find('.expressionLeftParam').val(),
            container.find('.expressionOperator').val(),
            $(event.target).prop('data'),
            editor.MaxSourceQuestionPagePosition, editor.LanguageCode, expressionEditor.onExpressionAdded, { editor: editor, expressionId: expressionId, container: container });
    };

    this.onExpressionAdded = function (data, callbackArgs) {
        if (callbackArgs.editor.AddExpressionMode) {
            //a new branching added -- reload all page            
            UFrameManager.getUFrame('itemEditorContainer').load();
        }
        else {
            //regular case
            expressionEditor.showMessage(callbackArgs.editor.ID, 'success', textHelper.getTextValue("/expressionEditor/addSuccess", "Expression added. Changes has been saved. Reloading..."));
            expressionEditor.load(callbackArgs.editor.ID);
        }
    };



    /*--------------------------------------------------------------------------------------------------------*/
    /*---------------------------     ... Add expression                     ---------------------------------*/
    /*--------------------------------------------------------------------------------------------------------*/

    /*--------------------------------------------------------------------------------------------------------*/
    /*---------------------------     Remove expression ...                  ---------------------------------*/
    /*--------------------------------------------------------------------------------------------------------*/
    this.onRemoveExpressionClick = function (event) {
        event = event || window.event;
        var containerId = $(event.target).closest('.expressionEditorBase').attr('id');
        var editor = expressionEditor.expressionEditors[containerId];
        if (typeof (editor) == 'undefined') {
            return;
        }

        var expressionId = $(event.target).closest('.leafExpression').attr('expressionId');

        svcSurveyManagement.removeExpression(expressionEditor.authToken, editor.ResponseTemplateId, expressionId,
            expressionEditor.onExpressionRemoved, { editor: editor, expressionId: expressionId });
    };

    this.onExpressionRemoved = function (data, callbackArgs) {
        if (data) {
            var expression = $('.leafExpression[expressionId="' + callbackArgs.expressionId + '"]');

            var p = expression.parent();
            expression.detach();
            //if there are no more leafs we can remove the root
            if (p.find('.leafExpression').length <= 0) {
                p.parent().empty();
                callbackArgs.editor.rootExpressionId = undefined;
            }

            expressionEditor.showMessage(callbackArgs.editor.ID, 'success', textHelper.getTextValue("/expressionEditor/deleteSuccess", "Expression deleted. Changes has been saved. Reloading..."));
            if (typeof (data.length) != 'undefined' && data.length > 0) {
               /* setTimeout(function () {
                    for (var i = 0; i < data.length; i++) {
                        $('.ruleEditorWrapper[ruleid="' + data[i] + '"]').remove();
                    }
                }, 1000);*/
            }
            else {
                expressionEditor.load(callbackArgs.editor.ID);
            }
        }
    };

    this.showMessage = function (containerID, messageType, text) {
        if (typeof (statusControl) != 'undefined') {
            statusControl.showStatusMessage(text, StatusMessageType[messageType]);
        }
        else {
            var messageContainer = $('<div></div>').insertBefore($('#' + containerID));
            messageContainer.attr("class", messageType).attr("id", "expressionOperationStatus").hide().html(text).fadeIn(1000);
            $('#expressionOperationStatus').fadeOut(2000);
            setTimeout(function () { $('#expressionOperationStatus').detach(); }, 2000);
        }
    };
    /*--------------------------------------------------------------------------------------------------------*/
    /*---------------------------     ... Remove expression                  ---------------------------------*/
    /*--------------------------------------------------------------------------------------------------------*/


    /*--------------------------------------------------------------------------------------------------------*/
    /*---------------------------     Edit expression ...                  ---------------------------------*/
    /*--------------------------------------------------------------------------------------------------------*/
    this.onEditExpressionClick = function (event) {
        event = event || window.event;

        var containerId = $(event.target).closest('.expressionEditorBase').attr('id');
        var editor = expressionEditor.expressionEditors[containerId];
        if (typeof (editor) == 'undefined') {
            return;
        }

        $(event.target).closest('.leafExpression').find('.leafView').find('.valueDisplay').hide();
        $(event.target).closest('.leafExpression').find('.expressionEditorPanel').show();
        $(event.target).closest('.leafExpression').find('.leafButtons').hide();
        $(event.target).closest('.leafExpression').find('.expressionEditorPanel img').hide();

        var container = $(event.target).closest('.leafExpression');
        svcSurveyManagement.getExistingExpressionRightParams(expressionEditor.authToken, editor.ResponseTemplateId,
            $(event.target).closest('.leafExpression').attr('expressionId'),
            editor.MaxSourceQuestionPagePosition, editor.LanguageCode, expressionEditor.onExpressionRightParamsReceived, { editor: editor, container: container});
    };

    this.onSaveEditedExpressionClick = function (event) {
        event = event || window.event;
        var containerId = $(event.target).closest('.expressionEditorBase').attr('id');
        var editor = expressionEditor.expressionEditors[containerId];
        if (typeof (editor) == 'undefined') {
            return;
        }

        var expressionId = $(event.target).attr('expressionId');

        if (typeof (expressionId) == 'undefined' || !expressionId.length)
            expressionId = -1;

        var container = $(event.target).closest('.expressionEditorPanel');

        container.find('.addNewExpressionButton').hide();

        var targetPageID = $('#' + containerId).closest('.ruleEditorWrapper').find('.branchTargetSelector').find(':selected').val();
        if (typeof (targetPageID) == 'undefined' || !targetPageID) {
            targetPageID = 0;
        }

        svcSurveyManagement.editExpression(expressionEditor.authToken,
            editor.RuleType,
            editor.DependentItemId == '' ? 0 : editor.DependentItemId,
            editor.DependentPageId == '' ? 0 : editor.DependentPageId,
            targetPageID,
            editor.ResponseTemplateId,
            editor.rootExpressionId,
            expressionId,
            container.find('.expressionLeftParamType').val(),
            container.find('.expressionLeftParam').val(),
            container.find('.expressionOperator').val(),
            $(event.target).prop('data'),
            editor.MaxSourceQuestionPagePosition, editor.LanguageCode, expressionEditor.onExpressionEdited, { editor: editor, expressionId: expressionId, container: container });
    };


    this.onCancelEditedExpressionClick = function (event) {
        event = event || window.event;
        $(event.target).closest('.leafExpression').find('.leafView').find('.valueDisplay').show();
        $(event.target).closest('.leafExpression').find('.leafButtons').show();
        $(event.target).closest('.expressionEditorPanel').hide();
    };

    this.onExpressionEdited = function (data, callbackArgs) {
        expressionEditor.showMessage(callbackArgs.editor.ID, 'success', textHelper.getTextValue("/expressionEditor/editSuccess", "Expression modified. Changes has been saved. Reloading..."));
        expressionEditor.load(callbackArgs.editor.ID);
    };

    /*--------------------------------------------------------------------------------------------------------*/
    /*---------------------------     ... Remove expression                  ---------------------------------*/
    /*--------------------------------------------------------------------------------------------------------*/


    /*--------------------------------------------------------------------------------------------------------*/
    /*---------------------------     Change target page ...                 ---------------------------------*/
    /*--------------------------------------------------------------------------------------------------------*/
    this.onBranchTargetPageChanged = function (event) {
        event = event || window.event;
        var id = $(event.target).closest('.ruleEditorWrapper').find('.expressionEditorBase').attr('id');
        var ruleId = $(event.target).closest('.ruleEditorWrapper').attr('ruleId');
        if (ruleId) {
            expressionEditor.showMessage(id, 'success', textHelper.getTextValue("/expressionEditor/saving", "Saving..."));

            var editor = expressionEditor.expressionEditors[id];
            svcSurveyManagement.setPageBranchTargetPage(expressionEditor.authToken, editor.ResponseTemplateId,
            $(event.target).closest('.ruleEditorWrapper').attr('ruleId'),
            $(event.target).find(':selected').val(), expressionEditor.onBranchTargetPageSaved, id);
        }
    };

    this.onBranchTargetPageSaved = function (data, id) {
        expressionEditor.showMessage(id, 'success', textHelper.getTextValue("/expressionEditor/targetSetSuccess", "Target page has been set successfully.Reloading..."));
    };
    /*--------------------------------------------------------------------------------------------------------*/
    /*---------------------------     ... Change target page                 ---------------------------------*/
    /*--------------------------------------------------------------------------------------------------------*/


    this.reloadInnerEditors = function (index) {
        var container = $("#ruleEditorTabs-" + index + "-tabContent");
        container.find('.expressionEditorBase').each(function f(idx, el) {
            expressionEditor.load($(el).attr('id'));
        });
    };
}















$.fn.populate = function (nvc, emptyValue, emptyName) {
    var select = $(this);
    if (typeof (nvc) == 'undefined' || !nvc.length) {
        select.append('<option value="' + emptyValue + '">' + emptyName + '</option>');
    };
    $(nvc).each(function (idx, elem) {
        select.append('<option value="' + elem.Name + '">' + elem.Value + '</option>');
    });
}