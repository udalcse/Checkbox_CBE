var advancedExpressionEditor = new advancedExpressionEditorObj();

////////////////////////////////////////////////////////////////////////////////
// Container for advanced expression editor routines.  Requires jQuery.       //
////////////////////////////////////////////////////////////////////////////////
function advancedExpressionEditorObj() {
    this.initialize = function (authToken, appRoot, id, params) {
        advancedExpressionEditor = $.extend({}, expressionEditor, advancedExpressionEditor);
        params.Manager = advancedExpressionEditor;
        expressionEditor.initialize(authToken, appRoot, id, params);
    };

    this.getEditorTemplateFileName = function () {
        return 'advancedExpressionEditorTemplate.html';
    };


}













