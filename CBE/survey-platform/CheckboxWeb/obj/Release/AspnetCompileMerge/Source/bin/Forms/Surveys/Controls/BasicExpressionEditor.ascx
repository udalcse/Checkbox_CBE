<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="BasicExpressionEditor.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.BasicExpressionEditor" %>
<%@ Import Namespace="Checkbox.Globalization" %>

<div class="dialogSubContainer">
    <div class="formInput radioButton">
        <p><asp:RadioButton runat="server" id="_anyView" GroupName="anyAll" /></p>
    </div>

    <div class="formInput radioButton">
        <p><asp:RadioButton runat="server" id="_allView" GroupName="anyAll" /></p>
    </div>
</div>



    <div id="_basicExpressionEditor" runat="server" class="expressionEditorBase">
    </div>
    <script type="text/javascript" language="javascript">
        $(document).ready(function () {
            $('#<%=_anyView.ClientID %>').click(function () { basicExpressionEditor.reorganizeExpressions('<%=_basicExpressionEditor.ClientID%>', 'OR'); });
            $('#<%=_allView.ClientID %>').click(function () { basicExpressionEditor.reorganizeExpressions('<%=_basicExpressionEditor.ClientID%>', 'AND'); });

            templateHelper.loadAndCompileTemplate('<%=ResolveUrl("~/Forms/jqtmpl/basicExpressionEditorTemplate.html") %>', 'basicExpressionEditorTemplate.html');
            templateHelper.loadAndCompileTemplate('<%=ResolveUrl("~/Forms/jqtmpl/basicLeafExpressionTemplate.html") %>', 'basicLeafExpressionTemplate.html');
            templateHelper.loadAndCompileTemplate('<%=ResolveUrl("~/Forms/jqtmpl/addExpressionTemplate.html") %>', 'addExpressionTemplate.html');
            templateHelper.loadAndCompileTemplate('<%=ResolveUrl("~/Forms/jqtmpl/editExpressionTemplate.html") %>', 'editExpressionTemplate.html');

            basicExpressionEditor.initialize(_at, '<%=ResolveUrl("~/") %>', '<%=_basicExpressionEditor.ClientID%>',
                {
                    RuleType: '<%=Params.RuleType %>',
                    ResponseTemplateId: '<%=Params.ResponseTemplateId %>',
                    MaxSourceQuestionPagePosition: '<%=Params.MaxSourceQuestionPagePosition %>',
                    DependentItemId: '<%=Params.DependentItemId %>',
                    DependentPageId: '<%=Params.DependentPageId %>',
                    LanguageCode: '<%=Params.LanguageCode %>',
                    DateFormat: '<%=GlobalizationManager.GetDateFormat()%>',
                    Connector: '<%= GetBasicModeConnector() %>'
                });
        });
    </script>

