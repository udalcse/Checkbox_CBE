var basicExpressionEditor = new basicExpressionEditorObj();

////////////////////////////////////////////////////////////////////////////////
// Container for advanced expression editor routines.  Requires jQuery.       //
////////////////////////////////////////////////////////////////////////////////
function basicExpressionEditorObj() {
    this.initialize = function (authToken, appRoot, id, params) {
        basicExpressionEditor = $.extend({}, expressionEditor, basicExpressionEditor);
        params.Manager = basicExpressionEditor;
        expressionEditor.initialize(authToken, appRoot, id, params);
    };

    this.getEditorTemplateFileName = function () {
        return 'basicExpressionEditorTemplate.html';
    };


    this.getParentExpressionId = function(event, editor) {
        event = event || window.event;

        var expressionId = -1;

        if (editor.Connector == 'AND') {
            expressionId = $(event.target).closest('.expressionEditorBase').find('.expressionContainer').attr('expressionId');
        }

        return expressionId;
    };

    this.reorganizeExpressions = function(id, connector) {
        var editor = expressionEditor.expressionEditors[id];
        if (typeof(editor) == 'undefined') {
            return;
        }

        svcSurveyManagement.reorganizeExpressions(expressionEditor.authToken,
            editor.RuleType,
            editor.DependentItemId == '' ? 0 : editor.DependentItemId,
            editor.DependentPageId == '' ? 0 : editor.DependentPageId,
            0,
            editor.ResponseTemplateId,
            editor.rootExpressionId,
            connector,
            basicExpressionEditor.onExpressionsReorganized, { editor: editor });
    };

    this.onExpressionsReorganized = function (data, callbackArgs) {
        callbackArgs.editor.Connector = data;
        expressionEditor.showMessage(callbackArgs.editor.ID, 'success', textHelper.getTextValue("/expressionEditor/rebuildSuccess", "Expressions rebuild. Changes has been saved. Reloading..."));
        expressionEditor.load(callbackArgs.editor.ID);
    };

}














