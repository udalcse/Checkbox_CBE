<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="DisplayAnalysis.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemEditors.DisplayAnalysis" %>
<%@ Import Namespace="Checkbox.Web" %>

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

            <%if(HidePreview) { %>
                $('#itemEditorTabs').ckbxTabs('hideTab', 0);
            <% } %>
            
            <% if (EditMode != Checkbox.Forms.EditMode.Survey) { %>
                $('#itemEditorTabs').ckbxTabs('hideTab', 3);
            <%} %>
        });
    </script>

    <div style="display:none;">
        <asp:TextBox ID="_currentTabIndex" runat="server" Text="1" />
    </div>

    <ul id="itemEditorTabs" class="tabContainer">
        <li><%=WebTextManager.GetText("/pageText/formEditor.aspx/preview")%></li>
        <li><%=WebTextManager.GetText("/pageText/formEditor.aspx/edit")%></li>
        <li><%=WebTextManager.GetText("/controlText/itemEditor/appearance")%></li>
        <li><%=WebTextManager.GetText("/controlText/templatePageEditor/conditions")%></li>
        <div class="clear"></div>
    </ul>
     <div class="clear"></div>

    <div class="tabContentContainer">
        <div class="padding10" id="itemEditorTabs-0-tabContent">
            <ckbx:ActiveDisplay ID="ActiveDisplay1" runat="server" />

            <asp:PlaceHolder ID="_previewPlace" runat="server"></asp:PlaceHolder>

            <div class="clear"></div>
            <hr size="1" />
            <ckbx:RuleDisplay ID="_ruleDisplay" runat="server" />
            <div class="clear"></div>
        </div>

        <div id="itemEditorTabs-1-tabContent" class="padding10">
            <ckbx:ActiveDisplay ID="_activeDisplay" runat="server" />

            <!-- Report -->
            <div class="field_100">
                <ckbx:MultiLanguageLabel ID="_reportLbl" runat="server" TextId="/controlText/analysisItemEditor/analysis" />
            </div>

            <div class="input" style="float:left;">
                <asp:DropDownList ID="_reportList" runat="server" />                
            </div>
            
            <asp:Panel ID="_reportRequiredErrorPanel" runat="server" Visible="false" CssClass="Error">
                    &nbsp&nbsp&nbsp<ckbx:MultiLanguageLabel ID="_reportRequired" runat="server" TextId="/controlText/displayAnalysisItemEditor/analysisRequired" />
            </asp:Panel>

            <div class="clear"></div>

            <!-- Link Option -->
            <div class="field_100">
                <ckbx:MultiLanguageLabel ID="_optionLbl" runat="server" TextId="/controlText/redirectItemEditor/linkOption" />
            </div>

            <div class="input">
                <asp:DropDownList ID="_optionList" runat="server" AutoPostBack="true" />
            </div>

            <div class="clear"></div>

            <!-- Link Text -->
            <div class="field_100">
                <ckbx:MultiLanguageLabel ID="_linkTxtLbl" runat="server" TextId="/controlText/redirectItemEditor/linkText" />
            </div>

            <div class="input">
                <asp:TextBox ID="_linkTxt" runat="server" />
            </div>

            <!-- Link Text -->
            <div class="field_100">
                <asp:CheckBox ID="_newTabChk" runat="server" />
                <ckbx:MultiLanguageLabel ID="_newTabLbl" runat="server" TextId="/controlText/redirectItemEditor/newTabOptionText" />
            </div>


            <div class="clear"></div>
        </div>
        
        <div id="itemEditorTabs-2-tabContent" class="padding10">
            <asp:PlaceHolder ID="_appearancePlace" runat="server" />
        </div>

        <div id="itemEditorTabs-3-tabContent">
            <ckbx:ConditionEditor ID="_conditionEditor" runat="server" />
        </div>
    </div>

