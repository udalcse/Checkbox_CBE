<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="RankOrder.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemEditors.RankOrder" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemEditors/QuestionTextEditor.ascx" TagPrefix="qte" TagName="QuestionTextEditor" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemEditors/RankOrderBehavior.ascx" TagPrefix="be" TagName="Behavior" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemConditionEditor.ascx" TagPrefix="ckbx" TagName="ConditionEditor" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemConditionsRuleDislpay.ascx" TagPrefix="ckbx" TagName="RuleDisplay" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemActiveDisplay.ascx" TagPrefix="ckbx" TagName="ActiveDisplay" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemEditors/RankOrderOptionsEditor.ascx" TagPrefix="sle" TagName="RankOrderOptionsEditor" %>

<script type="text/javascript">
        $(document).ready(function () {
            $('#itemEditorTabs').ckbxTabs({ 
                tabName: 'itemEditorTabs',
                initialTabIndex: <%=_currentTabIndex.Text %>,
                onTabClick: function(index){onMainTabChange(index)},
                onTabsLoaded: function(){$('.tabContainer').show();$('.tabContentContainer').show();}
            });

            $('#questionTextTabs_<%=ID %>').ckbxTabs({ 
                tabName: 'questionTextTabs_<%=ID %>',
                tabStyle: 'inverted',
                initialTabIndex: 0
            });        

            <%if(HidePreview) { %>
                $('#itemEditorTabs').ckbxTabs('hideTab', 0);
            <% } %>

            
            <% if (EditMode != Checkbox.Forms.EditMode.Survey) { %>
                $('#itemEditorTabs').ckbxTabs('hideTab', 5);
            <%} %>
        });

     //
     function onMainTabChange(newTabIndex) {
         //Set value of current tab and cause postback, so options entry dialogs can be updated
         $('#<%=_currentTabIndex.ClientID%>').val(newTabIndex);

         //Simulate button click. _choiceTabIsOld is defined in RankOrderBehavior tab
         if (typeof _choiceTabIsOld != "undefined" && _choiceTabIsOld){
             if (typeof (UFrameManager) == 'undefined') {
                eval($('#<%=_tabChangeBtn.ClientID %>').attr('href'));
             }
             else {
                 UFrameManager.executeASPNETPostback($('#<%=_tabChangeBtn.ClientID %>'), $('#<%=_tabChangeBtn.ClientID %>').attr('href'));
             }
             _choiceTabIsOld = false;
        }
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
        <%=WebTextManager.GetText("/controlText/itemEditor/question")%></li>
    <li>
        <%=WebTextManager.GetText("/controlText/itemEditor/behavior")%></li>
    <li>
        <%=WebTextManager.GetText("/controlText/itemEditor/choices")%></li>
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
        <ckbx:RuleDisplay ID="_ruleDisplay" runat="server" />
        <div class="clear">
        </div>
    </div>

    <div id="itemEditorTabs-1-tabContent">
        <ul id="questionTextTabs_<%=ID %>" class="tabContainer">
            <li><%=WebTextManager.GetText("/controlText/itemEditor/questionText")%></li>
            <li><%=WebTextManager.GetText("/controlText/itemEditor/subText")%></li>
        </ul>
        <div id="questionTextTabs_<%=ID %>-0-tabContent">
            <qte:QuestionTextEditor ID="_questionTextEditor" runat="server" EditorHeight="425" EditorWidth="600" />
        </div>
        <div id="questionTextTabs_<%=ID %>-1-tabContent">
            <qte:QuestionTextEditor ID="_descriptionTextEditor" runat="server" EditorHeight="425" EditorWidth="600" />
        </div>            
    </div>        
    
    <div id="itemEditorTabs-2-tabContent" class="padding10">
        <be:Behavior ID="_behaviorEditor" runat="server" />
    </div>

    <div id="itemEditorTabs-3-tabContent">
        <sle:RankOrderOptionsEditor ID="_selectOptionsEditor" runat="server" />
    </div>
    
    <div id="itemEditorTabs-4-tabContent" class="padding10">
        <asp:PlaceHolder ID="_appearanceEditorPlace" runat="server" />
    </div>
    <div id="itemEditorTabs-5-tabContent">
        <ckbx:ConditionEditor ID="_conditionEditor" runat="server" />
    </div>
</div>
