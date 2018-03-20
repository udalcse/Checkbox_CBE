<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="DisplayResponse.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemEditors.DisplayResponse" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Register Assembly="Checkbox.Web" Namespace="Checkbox.Web.UI.Controls" TagPrefix="ls" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemConditionEditor.ascx" TagPrefix="ckbx" TagName="ConditionEditor" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemConditionsRuleDislpay.ascx" TagPrefix="ckbx" TagName="RuleDisplay" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemActiveDisplay.ascx" TagPrefix="ckbx" TagName="ActiveDisplay" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemList.ascx" TagPrefix="ckbx" TagName="ItemList" %>

 <script type="text/javascript">
        $(document).ready(function () {
            $('#itemEditorTabs').ckbxTabs({ 
                tabName: 'itemEditorTabs',
                initialTabIndex: <%=_currentTabIndex.Text %>,
                onTabClick: function(index){$('#<%=_currentTabIndex.ClientID %>').val(index)},
                onTabsLoaded: function(){$('.tabContainer').show();$('.tabContentContainer').show();}
            });

            <%if(HidePreview) { %>
                $('#itemEditorTabs').ckbxTabs('hideTab', 0);
            <% } %>

             <% if (EditMode != Checkbox.Forms.EditMode.Survey) { %>
                $('#itemEditorTabs').ckbxTabs('hideTab', 3);
            <%} %>
        });
    </script>

    <ul id="itemEditorTabs" class="tabContainer">
        <li><%=WebTextManager.GetText("/pageText/formEditor.aspx/preview")%></li>
        <li><%=WebTextManager.GetText("/controlText/itemEditor/options")%></li>
        <li><%=WebTextManager.GetText("/controlText/itemEditor/appearance")%></li>
        <li><%=WebTextManager.GetText("/controlText/templatePageEditor/conditions")%></li>
        <div class="clear"></div>
    </ul>
    <div class="clear"></div>

    <div style="display:none;">
        <asp:TextBox ID="_currentTabIndex" runat="server" Text="0" />
    </div>

     <div class="tabContentContainer">
        <div id="itemEditorTabs-0-tabContent" class="padding10">
            <ckbx:ActiveDisplay ID="_activeDisplay" runat="server" />
            
            <asp:PlaceHolder ID="_previewPlace" runat="server"></asp:PlaceHolder> 

            <div class="clear"></div>
            <hr size="1" />
            <ckbx:RuleDisplay ID="_ruleDisplay" runat="server" />
            <div class="clear"></div>
        </div>

        <div id="itemEditorTabs-1-tabContent" class="padding10">
            <div class="formInput">
                <p>
                    <ckbx:MultiLanguageLabel AssociatedControlID="_optionsList" ID="_optionLbl" runat="server" TextId="/controlText/displayResponseItemEditor/displayOption" />
                </p>
                <asp:DropDownList ID="_optionsList" runat="server" AutoPostBack="true" />

                <p>
                    <ckbx:MultiLanguageLabel ID="_linkTxtLbl" runat="server" TextId="/controlText/displayResponseItemEditor/linkText" AssociatedControlID="_linkTxt" />
                </p>
                <asp:TextBox ID="_linkTxt" runat="server" />
            
                <br class="clear" />

                <div class="left checkBox"><asp:CheckBox ID="_includeResponseDetailsChk" runat="server" /></div>
                <div class="left"><p><label><%=WebTextManager.GetText("/controlText/displayResponseItemEditor/includeResponseDetails")%></label></p></div>
                <br class="clear" />
    
                <div class="left checkBox"><asp:CheckBox ID="_showPageNumbersChk" runat="server" /></div>
                <div class="left"><p><label><%=WebTextManager.GetText("/controlText/displayResponseItemEditor/showPageNumbers")%></label></p></div>
                <br class="clear" />    

                <div class="left checkBox"><asp:CheckBox ID="_showQuestionNumbersChk" runat="server" /></div>
                <div class="left"><p><label><%=WebTextManager.GetText("/controlText/displayResponseItemEditor/showQuestionNumbers")%></label></p></div>
                <br class="clear" />    

                <div class="left checkBox"><asp:CheckBox ID="_includeMessageItemsChk" runat="server" /></div>
                <div class="left"><p><label><%=WebTextManager.GetText("/controlText/displayResponseItemEditor/includeMessageItems")%></label></p></div>
                <br class="clear" />

                <div class="left checkBox"><asp:CheckBox ID="_showHiddenItemsChk" runat="server" /></div>
                <div class="left"><p><label><%=WebTextManager.GetText("/controlText/displayResponseItemEditor/showHiddenItems")%></label></p></div>
                <br class="clear" />
            </div>
        </div>
        
        <div class="padding10" id="itemEditorTabs-2-tabContent" >
            <asp:PlaceHolder ID="_appearanceEditorPlace" runat="server" />
        </div>

         <div id="itemEditorTabs-3-tabContent">
            <ckbx:ConditionEditor ID="_conditionEditor" runat="server" />
        </div>
    </div>


