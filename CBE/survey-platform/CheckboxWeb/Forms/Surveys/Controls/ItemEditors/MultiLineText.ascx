<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="MultiLineText.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemEditors.MultiLineText" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemEditors/QuestionTextEditor.ascx" TagPrefix="qte" TagName="QuestionTextEditor" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemEditors/MultiLineTextBehavior.ascx" TagPrefix="be" TagName="Behavior" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemConditionEditor.ascx" TagPrefix="ckbx" TagName="ConditionEditor" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemConditionsRuleDislpay.ascx" TagPrefix="ckbx" TagName="RuleDisplay" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemActiveDisplay.ascx" TagPrefix="ckbx" TagName="ActiveDisplay" %>



<script type="text/javascript">
        $(document).ready(function () {
            $('#itemEditorTabs').ckbxTabs({ 
                tabName: 'itemEditorTabs',
                initialTabIndex: <%=_currentTabIndex.Text %>,
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
                $('#itemEditorTabs').ckbxTabs('hideTab', 4);
            <%} %>
            
            //set value from two way binding drop-down to default text element
            $(document).on("change", "select[id*='questionBinding']", function() {
                var defaultText;
                var isHtmlFormattable = $("[id*='isHtmlFormattedDataChk']").is(':checked');

                if (isHtmlFormattable) {
                    var tinyMceId = $("textarea[id*='_defaultHtml']").attr('id');
                    defaultText = tinyMCE.get(tinyMceId);
                } else {
                    defaultText = $("textarea[id*='defaultText']");
                }

                if ($(this).find("option:selected")[0].label === "None") {
                    if (isHtmlFormattable) {
                        defaultText.setContent("");
                    } else {
                        $(defaultText).text("");
                    }
                } else {
                    var value = "@@" + $(this).find("option:selected")[0].label;
                    if (isHtmlFormattable) {
                        defaultText.setContent(value);
                    } else {
                        $(defaultText).text(value);
                    }
                }
            });
        });

    </script>

    <div style="display:none;">
        <asp:TextBox ID="_currentTabIndex" runat="server" Text="0" />
    </div>

    <ul id="itemEditorTabs" class="tabContainer">
        <li><%=WebTextManager.GetText("/pageText/formEditor.aspx/preview")%></li>
        <li><%=WebTextManager.GetText("/controlText/itemEditor/question")%></li>
        <li><%=WebTextManager.GetText("/controlText/itemEditor/behavior")%></li>
        <li><%=WebTextManager.GetText("/controlText/itemEditor/appearance")%></li>
        <li><%=WebTextManager.GetText("/controlText/templatePageEditor/conditions")%></li>
    </ul>
    <div class="clear"></div>

    <div class="tabContentContainer">
        <div id="itemEditorTabs-0-tabContent" class="padding10">
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
        
        <div id="itemEditorTabs-2-tabContent" class="padding10 overflowPanel_445">
            <be:Behavior ID="_behaviorEditor" runat="server" />
        </div>
        
        <div id="itemEditorTabs-3-tabContent" class="padding10">
            <asp:PlaceHolder ID="_appearanceEditorPlace" runat="server" />
        </div>

         <div id="itemEditorTabs-4-tabContent">
            <ckbx:ConditionEditor ID="_conditionEditor" runat="server" />
        </div>
    </div>
 
   <input type="hidden" id="itemType" value="multiline"/>