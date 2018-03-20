<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="CurrentScore.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemEditors.CurrentScore" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemEditors/QuestionTextEditor.ascx" TagPrefix="qte" TagName="QuestionTextEditor" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemEditors/CurrentScoreBehavior.ascx" TagPrefix="cs" TagName="CurrentScoreBehavior" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemConditionEditor.ascx" TagPrefix="ckbx" TagName="ConditionEditor" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemConditionsRuleDislpay.ascx" TagPrefix="ckbx" TagName="RuleDisplay" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemActiveDisplay.ascx" TagPrefix="ckbx" TagName="ActiveDisplay" %>


 <script type="text/javascript">
        $(document).ready(function () {
            $('#itemEditorTabs').ckbxTabs({ 
                tabName: 'itemEditorTabs',
                initialTabIndex:<%=_currentTabIndex.Text %>,
                onTabClick: function(index){$('#<%=_currentTabIndex.ClientID %>').val(index)},
                onTabsLoaded: function(){$('.tabContainer').show();$('.tabContentContainer').show();}
            });

            //Hide appearance as there are no options and hide behavior since only option is all pages.
            $('#itemEditorTabs').ckbxTabs('hideTab', 3);
     //       $('#itemEditorTabs').ckbxTabs('hideTab', 2);

            <%if(HidePreview) { %>
                $('#itemEditorTabs').ckbxTabs('hideTab', 0);
            <% } %>

            <% if (EditMode != Checkbox.Forms.EditMode.Survey) { %>
                $('#itemEditorTabs').ckbxTabs('hideTab', 4);
             <%} %>
        });
    </script>

    <div style="display:none;">
        <asp:TextBox ID="_currentTabIndex" runat="server" Text="0" />
    </div>

    <ul id="itemEditorTabs" class="tabContainer">
        <li><%=WebTextManager.GetText("/pageText/formEditor.aspx/preview")%></li>
        <li><%=WebTextManager.GetText("/controlText/scoreMessageItemEditor/scoreMessage")%></li>
        <li><%=WebTextManager.GetText("/controlText/itemEditor/behavior")%></li>
        <li><%=WebTextManager.GetText("/controlText/itemEditor/appearance")%></li>
        <li><%=WebTextManager.GetText("/controlText/templatePageEditor/conditions")%></li>
    </ul>
    <div class="clear"></div>

    <div class="tabContentContainer">
        <div id="itemEditorTabs-0-tabContent" class="padding10">
            <ckbx:ActiveDisplay ID="_activeDisplay" runat="server" />

            <asp:PlaceHolder ID="_previewPlace" runat="server" />

            <div class="clear"></div>
            <hr size="1" />
            <ckbx:RuleDisplay ID="_ruleDisplay" runat="server" />
            <div class="clear"></div>
        </div>
        <div id="itemEditorTabs-1-tabContent">
            <qte:QuestionTextEditor ID="_questionTextEditor" runat="server" EditorHeight="375" EditorWidth="600" />
            <div class="padding10" style="font-weight:bold;"">
                <ckbx:MultiLanguageLabel ID="_messageText" runat="server" TextId="/controlText/currentScoreItemEditor/message" />
            </div>
        </div>
        
        <div id="itemEditorTabs-2-tabContent" class="padding10">
            <cs:CurrentScoreBehavior ID="_currentScoreBehavior" runat="server" />
        </div>
        
        <div id="itemEditorTabs-3-tabContent" class="padding10"> 
            <asp:PlaceHolder ID="_appearanceEditorPlace" runat="server" />
        </div>

        <div id="itemEditorTabs-4-tabContent"> 
            <ckbx:ConditionEditor ID="_conditionEditor" runat="server" />
        </div>
    </div>
    <div style="clear:both;"></div>
   