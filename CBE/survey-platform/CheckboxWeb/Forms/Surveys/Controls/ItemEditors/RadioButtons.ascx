<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="RadioButtons.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemEditors.RadioButtons" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemEditors/QuestionTextEditor.ascx" TagPrefix="qte" TagName="QuestionTextEditor" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemEditors/SelectOptionsEditor.ascx" TagPrefix="sle" TagName="SelectOptionsEditor" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemEditors/Select1Behavior.ascx" TagPrefix="be" TagName="Behavior" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemConditionEditor.ascx" TagPrefix="ckbx" TagName="ConditionEditor" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemConditionsRuleDislpay.ascx" TagPrefix="ckbx" TagName="RuleDisplay" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemActiveDisplay.ascx" TagPrefix="ckbx" TagName="ActiveDisplay" %>


<script type="text/javascript">
    $(document).ready(function () {
        $('#itemEditorTabs').ckbxTabs({ 
            tabName: 'itemEditorTabs',
            initialTabIndex:<%=_currentTabIndex.Text %>,
            onTabClick: function(index) { $('#<%=_currentTabIndex.ClientID %>').val(index); },
            onTabsLoaded: function(){$('.tabContainer').show();$('.tabContentContainer').show();}
        });

        $('#questionTextTabs_<%=ID %>').ckbxTabs({ 
            tabName: 'questionTextTabs_<%=ID %>',
            tabStyle: 'inverted',
            initialTabIndex: 0
        });

        <% if(HidePreview) { %>
        $('#itemEditorTabs').ckbxTabs('hideTab', 0);
        <% } %>
            
        <% if (EditMode != Checkbox.Forms.EditMode.Survey) { %>
        $('#itemEditorTabs').ckbxTabs('hideTab', 6);
        <%} %>

        //on initialize check if item is bound to custom field and if so then initialize customFieldMode panel
        var boundItem = $("select[id*='questionBinding']").find("option:selected")[0].label;
        if(boundItem !== "None"){
            initializeCustomFieldPanel($("select[id*='questionBinding']"), false);
        }

        //set value from two way binding drop-down to default text element
        $(document).on("change", "select[id*='questionBinding']", function() {
            var defaultText =  $("#newOptionTxt");
            if ($(this).find("option:selected")[0].label === "None") {
                $(defaultText).val("");
                // need to make a call to delete all aliases from previous bound field
                deleteAliasesOfBoundField();
                $(".customFieldMode").css("display", "none");
                $(".normalMode").css("display", "block");
                $(".customFieldMode .radioFieldAliases tbody").html('');
            } else {
                $(defaultText).val("@@" + $(this).find("option:selected").text());
                // make request to server to get the data for bound field
                initializeCustomFieldPanel($("select[id*='questionBinding']"), true);
            }
        });

        function deleteAliasesOfBoundField(){
            $.ajax({ 
                type: "POST",
                url: "Edit.aspx/DeleteRadioButtonFieldOptionAliases",
                data: JSON.stringify(
                    {
                        itemId: $("#currentItemId").val()
                    }),
                contentType: "application/json; charset=utf-8",
                dataType: "json"
            });
        }

        function initializeCustomFieldPanel(questionBindingSelector, shouldCleanUp){
            $.ajax({ 
                type: "POST",
                url: "Edit.aspx/GetBoundRadioField",
                data: JSON.stringify(
                    {
                        fieldName: $(questionBindingSelector).find("option:selected")[0].label,
                        itemId: $("#currentItemId").val()
                    }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function(response){
                    console.log(response);
                    var radioButtonField = response.d;
                    var radioOptionAliasesTable = $(".customFieldMode .radioFieldAliases tbody");
                    radioOptionAliasesTable.html('');
                    for(var  i = 0; i < radioButtonField.Options.length; i++){
                        var optionName = radioButtonField.Options[i].Name;
                        var optionAlias = radioButtonField.Options[i].Alias;
                        radioOptionAliasesTable.append('<tr><td class="optionName">' + optionName + '</td>' + 
                            '<td><input type="text" class="left uniform-input text alias" value="' + optionAlias  + '"/></td></tr>');
                    }
                    $(".customFieldMode").css("display", "block");
                    $(".normalMode").css("display", "none");

                },
                error: function(xhr){
                    console.log(xhr);
                }
            });
        }
    });
</script>

    <div style="display:none;">
        <asp:TextBox ID="_currentTabIndex" runat="server" Text="0" />
    </div>

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
            <sle:SelectOptionsEditor ID="_selectOptionsEditor" runat="server" />
        </div>

        <div class="padding10" id="itemEditorTabs-3-tabContent">
            <be:Behavior ID="_behaviorEditor" runat="server" />
        </div>

        <div class="padding10" id="itemEditorTabs-4-tabContent">
            <asp:PlaceHolder ID="_appearanceEditorPlace" runat="server" />
        </div>

        <div id="itemEditorTabs-5-tabContent">
            <ckbx:ConditionEditor ID="_conditionEditor" runat="server" />
        </div>
    </div>

    <input type="hidden" id="itemType" value="radiobutton"/>
   