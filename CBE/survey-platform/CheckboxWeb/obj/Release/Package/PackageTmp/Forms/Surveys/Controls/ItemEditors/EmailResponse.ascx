<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="EmailResponse.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemEditors.EmailResponse" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemEditors/QuestionTextEditor.ascx" TagPrefix="qte" TagName="MessageEditor" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemEditors/EmailOptions.ascx" TagPrefix="opt" TagName="EmailOptions" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemConditionEditor.ascx" TagPrefix="ckbx" TagName="ConditionEditor" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemConditionsRuleDislpay.ascx" TagPrefix="ckbx" TagName="RuleDisplay" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemActiveDisplay.ascx" TagPrefix="ckbx" TagName="ActiveDisplay" %>
<%@ Register Src="~/Controls/Piping/PipeControl.ascx" TagName="PipeSelector" TagPrefix="pipe" %>

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
                $('#itemEditorTabs').ckbxTabs('hideTab', 4);
            <%} %>
        });
    </script>

    <div style="display:none;">
        <asp:TextBox ID="_currentTabIndex" runat="server" Text="0" />
    </div>

    <ul id="itemEditorTabs" class="tabContainer">
        <li><%=WebTextManager.GetText("/pageText/formEditor.aspx/preview")%></li>
        <li><%=WebTextManager.GetText("/controlText/itemEditor/options")%></li>
        <li><%=WebTextManager.GetText("/controlText/itemEditor/behavior")%></li>
        <li><%=WebTextManager.GetText("/controlText/emailItemEditor/message")%></li>
        <li><%=WebTextManager.GetText("/controlText/templatePageEditor/conditions")%></li>
    </ul>
    <div class="clear"></div>

    <div class="tabContentContainer">
        <div id="itemEditorTabs-0-tabContent" class="padding10">
            <ckbx:ActiveDisplay ID="_activeDisplay" runat="server" />
            
            <asp:PlaceHolder ID="_previewPlace" runat="server"></asp:PlaceHolder>

            <div class="clear"></div>
            <hr size="1" />
            <ckbx:RuleDisplay ID="_ruleDisplay" runat="server" />
            <div class="clear"></div>
        </div>

        <div id="itemEditorTabs-1-tabContent">
            <opt:EmailOptions ID="_emailOptions" runat="server" />
        </div>

        <div id="itemEditorTabs-2-tabContent" class="padding15">
            <div class="formInput">
                <p>
                    <asp:CheckBox ID="_includeResponseDetailsChk" runat="server" />
                    <label><%=WebTextManager.GetText("/controlText/emailResponseItemEditor/includeResponseDetails")%></label>
                </p>
            
                <p>
                    <asp:CheckBox ID="_showPageNumbersChk" runat="server" />
                    <label><%=WebTextManager.GetText("/controlText/emailResponseItemEditor/showPageNumbers")%></label>
                </p>

                <p>
                    <asp:CheckBox ID="_showQuestionNumbersChk" runat="server" />
                    <label><%=WebTextManager.GetText("/controlText/emailResponseItemEditor/showQuestionNumbers")%></label>
                </p>

                <p>
                    <asp:CheckBox ID="_includeMessageItemsChk" runat="server" />
                    <label><%=WebTextManager.GetText("/controlText/emailResponseItemEditor/includeMessageItems")%></label>
                </p>
            
                <p>
                    <asp:CheckBox ID="_showHiddenItemsChk" runat="server" />
                    <label><%=WebTextManager.GetText("/controlText/emailResponseItemEditor/showHiddenItems")%></label>
                </p>
            </div>
        </div>
        
        <div id="itemEditorTabs-3-tabContent">
            <qte:MessageEditor ID="_messageEditor" runat="server" EditorHeight="400" EditorWidth="500"  />
            
            <asp:Panel ID="_textMessageEditor" runat="server" CssClass="padding10">
                <asp:TextBox ID="_messageTextTxt" runat="server" TextMode="MultiLine" Rows="27" Columns="50" />
                <pipe:PipeSelector ID="_pipeSelector_messageEditor" runat="server" AllowProfilePipes="true" AllowResponseInfoPipes="true" AllowResponseTemplatePipes="true" AllowSurveyItemPipes="true" />
            </asp:Panel>
        </div>

        <div id="itemEditorTabs-4-tabContent">
            <ckbx:ConditionEditor ID="_conditionEditor" runat="server" />
        </div>

        <% if (EditMode == Checkbox.Forms.EditMode.Library) %>
        <%{ %>
        <div class="formInput fixed_75 padding10" style="padding-bottom:0px">
            <p><ckbx:MultiLanguageLabel AssociatedControlID="_aliasText" ID="MultiLanguageLabel1" runat="server" TextId="/controlText/listEditor/alias" /></p>
        </div>
        <div class="formInput padding10" style="padding-top:0px">
            <asp:TextBox ID="_aliasText" runat="server" />
        </div>
        <%} %>
    </div>