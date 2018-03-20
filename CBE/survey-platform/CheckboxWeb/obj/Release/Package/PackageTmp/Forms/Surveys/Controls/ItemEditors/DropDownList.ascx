<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="DropDownList.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemEditors.DropDownList" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemEditors/QuestionTextEditor.ascx" TagPrefix="qte" TagName="QuestionTextEditor" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemEditors/SelectOptionsEditor.ascx" TagPrefix="sle" TagName="SelectOptionsEditor" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemEditors/Select1Behavior.ascx" TagPrefix="be" TagName="Behavior" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemConditionEditor.ascx" TagPrefix="ckbx" TagName="ConditionEditor" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemConditionsRuleDislpay.ascx" TagPrefix="ckbx" TagName="RuleDisplay" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemActiveDisplay.ascx" TagPrefix="ckbx" TagName="ActiveDisplay" %>

 <script type="text/javascript">     
     $(document).ready(function () {
         $("div[id^='selectEditorTabs']").find("div[id*='_termsBinding']").parent().hide();

        $('#itemEditorTabs').ckbxTabs({ 
            tabName: 'itemEditorTabs',
            initialTabIndex: <%=_currentTabIndex.Text %>,
            onTabClick: function(index) { $('#<%=_currentTabIndex.ClientID %>').val(index); },
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
</script>

    <ul id="itemEditorTabs" class="tabContainer">
        <li><%=WebTextManager.GetText("/pageText/formEditor.aspx/preview")%></li>
        <li><%=WebTextManager.GetText("/controlText/itemEditor/question")%></li>
        <li><%=WebTextManager.GetText("/controlText/itemEditor/choices")%></li>
        <li><%=WebTextManager.GetText("/controlText/itemEditor/behavior")%></li>
        <li><%=WebTextManager.GetText("/controlText/itemEditor/appearance")%></li>
        <li><%=WebTextManager.GetText("/controlText/templatePageEditor/conditions")%></li>
        <div class="clear"></div>
    </ul>
    <div class="clear"></div>

    <div style="display:none;">
        <asp:TextBox ID="_currentTabIndex" runat="server" Text="0" />
    </div>

     <div class="tabContentContainer">
        <div class="padding10" id="itemEditorTabs-0-tabContent">
            <ckbx:ActiveDisplay ID="_activeDisplay" runat="server" />

            <div class="item-preview-container">
                <asp:PlaceHolder ID="_previewPlace" runat="server"></asp:PlaceHolder>
            </div>

            <div class="clear"></div>
            <ckbx:RuleDisplay ID="_ruleDisplay" runat="server" />
            <div class="clear"></div>
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
         
        <div id="itemEditorTabs-2-tabContent">
            <sle:SelectOptionsEditor ID="_selectOptionsEditor" runat="server" RestrictHtmlOptions="True" />
        </div>
        
        <div class="padding10" id="itemEditorTabs-3-tabContent">
            <be:Behavior DisableHtmlFormattedOtherOption="True" ID="_behaviorEditor" runat="server" />
        </div>


        <div class="padding10" id="itemEditorTabs-4-tabContent">
            <asp:PlaceHolder ID="_appearanceEditorPlace" runat="server" />
        </div>

         <div id="itemEditorTabs-5-tabContent">
            <ckbx:ConditionEditor ID="_conditionEditor" runat="server" />
        </div>
    </div>
   