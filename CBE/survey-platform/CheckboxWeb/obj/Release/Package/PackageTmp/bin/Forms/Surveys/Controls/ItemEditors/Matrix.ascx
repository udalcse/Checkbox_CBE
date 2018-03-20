﻿<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Matrix.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemEditors.Matrix" EnableViewState="false"  %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemEditors/QuestionTextEditor.ascx" TagPrefix="qte" TagName="QuestionTextEditor" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemEditors/MatrixRowEditor.ascx" TagPrefix="mre" TagName="MatrixRowEditor" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemEditors/MatrixColumnEditor.ascx" TagPrefix="mce" TagName="MatrixColumnEditor" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemConditionEditor.ascx" TagPrefix="ckbx" TagName="ConditionEditor" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemConditionsRuleDislpay.ascx" TagPrefix="ckbx" TagName="RuleDisplay" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemActiveDisplay.ascx" TagPrefix="ckbx" TagName="ActiveDisplay" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemEditors/MatrixBehavior.ascx" TagPrefix="mb" TagName="MatrixBehavior" %>


 <script type="text/javascript">
        $(document).ready(function () {
            
            $('#itemEditorTabs').ckbxTabs({ 
                tabName: 'itemEditorTabs',
                initialTabIndex:<%=_currentTabIndex.Text %>,
                onTabClick: function(index){$('#<%=_currentTabIndex.ClientID %>').val(index)},
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
                $('#itemEditorTabs').ckbxTabs('hideTab', 6);
            <%} %>

            <% if (IsBinded) { %>
                $('#itemEditorTabs').ckbxTabs('hideTab', 3);
                $('#itemEditorTabs').ckbxTabs('hideTab', 4);
            <%} %>
        });
    </script>

    <div style="display:none;">
        <asp:TextBox ID="_currentTabIndex" runat="server" Text="0" />
    </div>

    <ul id="itemEditorTabs" class="tabContainer">
        <li><%=WebTextManager.GetText("/pageText/formEditor.aspx/preview")%></li>
        <li><%=WebTextManager.GetText("/controlText/itemEditor/question")%></li>
        <li><%=WebTextManager.GetText("/controlText/itemEditor/behavior")%></li>
        <li><%=WebTextManager.GetText("/controlText/matrixItemEditor/pkRowHeaderLbl")%></li>
        <li><%=WebTextManager.GetText("/controlText/matrixItemEditor/columns")%></li>
        <li><%=WebTextManager.GetText("/controlText/itemEditor/appearance")%></li>
        <li><%=WebTextManager.GetText("/controlText/templatePageEditor/conditions")%></li>
    </ul>

    <div class="clear"></div>

    <div class="tabContentContainer">
        <div class="padding10" id="itemEditorTabs-0-tabContent">
            <ckbx:ActiveDisplay ID="_activeDisplay" runat="server" />

            <asp:PlaceHolder ID="_previewPlace" runat="server"></asp:PlaceHolder>

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
            <mb:MatrixBehavior ID="_matrixBehavior" runat="server"/>
        </div>
        
        <div class="padding10" id="itemEditorTabs-3-tabContent">
            <mre:MatrixRowEditor ID="_matrixRowEditor" runat="server" />
        </div>
                 
        <div class="padding10" id="itemEditorTabs-4-tabContent">
            <mce:MatrixColumnEditor ID="_columnEditor" runat="server" />
        </div>
                
        <div class="padding10" id="itemEditorTabs-5-tabContent">
            <asp:PlaceHolder ID="_appearanceEditorPlace" runat="server"></asp:PlaceHolder>
        </div>

         <div id="itemEditorTabs-6-tabContent">
            <ckbx:ConditionEditor ID="_conditionEditor" runat="server" />
        </div>
    </div>
   

<script type="text/javascript">
    $(document).ready(function () {
        //Sometimes matrix can be too large. We need to resize panels to show a tinyScrollbar.
        //This method is defined in DetailedList.master
        if (typeof (resizePanels) == 'function')
            resizePanels();
    });
</script>