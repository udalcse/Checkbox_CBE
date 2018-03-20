<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="AdvancedExpressionEditor.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.AdvancedExpressionEditor" %>
<%@ Import Namespace="Checkbox.Globalization" %>

<div class="padding10">
    <div id="_advancedExpressionEditor" runat="server" class="expressionEditorBase">
    </div>
    <script type="text/javascript" language="javascript">
        $(document).ready(function () {
            templateHelper.loadAndCompileTemplate('<%=ResolveUrl("~/Forms/jqtmpl/advancedExpressionEditorTemplate.html") %>', 'advancedExpressionEditorTemplate.html');
            templateHelper.loadAndCompileTemplate('<%=ResolveUrl("~/Forms/jqtmpl/leafExpressionTemplate.html") %>', 'leafExpressionTemplate.html');
            templateHelper.loadAndCompileTemplate('<%=ResolveUrl("~/Forms/jqtmpl/addExpressionTemplate.html") %>', 'addExpressionTemplate.html');
            templateHelper.loadAndCompileTemplate('<%=ResolveUrl("~/Forms/jqtmpl/editExpressionTemplate.html") %>', 'editExpressionTemplate.html');

            <%if (Params != null) {%>
            advancedExpressionEditor.initialize(_at, '<%= ResolveUrl("~/") %>', '<%= _advancedExpressionEditor.ClientID %>',
                {
                    RuleType: '<%= Params.RuleType %>',
                    ResponseTemplateId: '<%= Params.ResponseTemplateId %>',
                    MaxSourceQuestionPagePosition: '<%= Params.MaxSourceQuestionPagePosition %>',
                    DependentItemId: '<%= Params.DependentItemId %>',
                    DependentPageId: '<%= Params.DependentPageId %>',
                    LanguageCode: '<%= Params.LanguageCode %>',
                    DateFormat: '<%= GlobalizationManager.GetDateFormat() %>',
                    AddExpressionMode : <%= AddExpressionMode.ToString().ToLower() %>
                    });
            <% } %>
        });
    </script>
</div>
