<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="CalculatorEditor.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemEditors.CalculatorEditor" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemEditors/QuestionTextEditor.ascx" TagPrefix="qte"
    TagName="QuestionTextEditor" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemEditors/CalculatorBehavior.ascx" TagPrefix="be"
    TagName="Behavior" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemConditionEditor.ascx" TagPrefix="ckbx"
    TagName="ConditionEditor" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemConditionsRuleDislpay.ascx" TagPrefix="ckbx"
    TagName="RuleDisplay" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemActiveDisplay.ascx" TagPrefix="ckbx"
    TagName="ActiveDisplay" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemEditors/SliderOptionsEditor.ascx"
    TagPrefix="sle" TagName="SliderOptionsEditor" %>
<script type="text/javascript">
        $(document).ready(function () {
            $('#itemEditorTabs').ckbxTabs({ 
                tabName: 'itemEditorTabs',
                initialTabIndex: <%=_currentTabIndex.Text %>,
                onTabClick: function(index){onMainTabChange(index)},
                onTabsLoaded: function(){$('.tabContainer').show();$('.tabContentContainer').show();}
            });

            <%if(HidePreview) { %>
                $('#itemEditorTabs').ckbxTabs('hideTab', 0);
            <% } %>

            
            <% if (EditMode != Checkbox.Forms.EditMode.Survey) { %>
                $('#itemEditorTabs').ckbxTabs('hideTab', 4);
            <%} %>
        });

     //
     function onMainTabChange(newTabIndex) {
     }
</script>
<div style="display: none;">
    <asp:TextBox ID="_currentTabIndex" runat="server" Text="0" />
    <asp:LinkButton ID="_tabChangeBtn" runat="server" />
</div>
<ul id="itemEditorTabs" class="tabContainer">
    <li>
        <%=WebTextManager.GetText("/pageText/formEditor.aspx/preview")%></li>
    <li>
        <%=WebTextManager.GetText("/controlText/itemEditor/questionText")%></li>
    <li>
        <%=WebTextManager.GetText("/controlText/itemEditor/description")%></li>
    <li>
        <%=WebTextManager.GetText("/controlText/itemEditor/behavior")%></li>
    <li>
        <%=WebTextManager.GetText("/controlText/itemEditor/appearance")%></li>
    <li>
        <%=WebTextManager.GetText("/controlText/templatePageEditor/conditions")%></li>
</ul>
<div class="clear">
</div>
<div class="tabContentContainer">
    <div id="itemEditorTabs-0-tabContent" class="padding10">
        <ckbx:ActiveDisplay ID="_activeDisplay" runat="server" />
        <asp:PlaceHolder ID="_previewPlace" runat="server"></asp:PlaceHolder>
        <div class="clear">
        </div>
        <hr size="1" />
        <ckbx:RuleDisplay ID="_ruleDisplay" runat="server" />
        <div class="clear">
        </div>
    </div>
    <div id="itemEditorTabs-1-tabContent">
        <qte:QuestionTextEditor ID="_questionTextEditor" runat="server" EditorHeight="425"
            EditorWidth="600" />
    </div>
    <div id="itemEditorTabs-2-tabContent">
        <qte:QuestionTextEditor ID="_descriptionTextEditor" runat="server" EditorHeight="425"
            EditorWidth="600" />
    </div>
    <div id="itemEditorTabs-3-tabContent" class="padding10">
        <be:Behavior ID="_behaviorEditor" runat="server" />
    </div>
    <div id="itemEditorTabs-4-tabContent" class="padding10">
        <asp:PlaceHolder ID="_appearanceEditorPlace" runat="server" />
    </div>
    <div id="itemEditorTabs-5-tabContent">
        <ckbx:ConditionEditor ID="_conditionEditor" runat="server" />
    </div>
</div>
