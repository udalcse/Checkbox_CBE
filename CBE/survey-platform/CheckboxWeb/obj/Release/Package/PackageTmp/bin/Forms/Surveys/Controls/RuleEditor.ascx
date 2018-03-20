<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="RuleEditor.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.RuleEditor" %>
<%@ Register Src="~/Forms/Surveys/Controls/BasicExpressionEditor.ascx" TagName="BasicExpressionEditor" TagPrefix="ckbx" %>
<%@ Register Src="~/Forms/Surveys/Controls/AdvancedExpressionEditor.ascx" TagName="AdvancedExpressionEditor" TagPrefix="ckbx" %>
<%@ Import Namespace="Checkbox.Web"%>
<%@ Import Namespace="Checkbox.Forms.Logic.Configuration"%>

<script type="text/javascript">
    $(document).ready(function () {
        $('#ruleEditorTabs').ckbxTabs({
            tabName: 'ruleEditorTabs',
            tabStyle: '<%=TabStyle %>',
            initialTabIndex: <%=_currentTabIndex.Text %>,
            onTabClick: function(index){$('#<%=_currentTabIndex.ClientID %>').val(index); if (typeof (resizePanels) == 'function') resizePanels(); expressionEditor.reloadInnerEditors(index);},
            onTabsLoaded: function(){$('#ruleEditorTabs').show();$('#ruleEditorTabContent').show();}
        });
        
        //Sometimes list of rules can be too large. We need to resize panels to show a tinyScrollbar.
        //This method is defined in DetailedList.master
        if (typeof (resizePanels) == 'function')
            resizePanels();
    });
</script>

<div style="display:none;">
    <asp:TextBox ID="_currentTabIndex" runat="server" Text="0" />
</div>

<asp:Panel ID="_tabPanel" runat="server">
    <ul id="ruleEditorTabs" class="tabContainer">
        <li><%=WebTextManager.GetText("/pageText/conditionEditor.aspx/basicView")%></li>
        <li><%=WebTextManager.GetText("/pageText/conditionEditor.aspx/advancedView")%></li>
    </ul>
</asp:Panel>

<div class="tabContentContainer" id="ruleEditorTabContent" style="height: 450px; overflow-y: auto;">
    <div id="ruleEditorTabs-0-tabContent">
        <asp:Repeater ID="_basicRuleRepeater" runat="server">
            <ItemTemplate>
                <ckbx:BasicExpressionEditor ID="_basicEditor" runat="server" />
            </ItemTemplate>
        </asp:Repeater>
    </div>
    <div id="ruleEditorTabs-1-tabContent">
        <asp:Repeater ID="_advancedRuleRepeater" runat="server">
            <ItemTemplate>  
                <div class="ruleEditorWrapper" ruleId="<%#((RuleData)(Container.DataItem)).RuleId%>">
                    <asp:Panel ID="_goToPagePanel" runat="server" CssClass="formInput">
                        <div class="left">
                            <p><label for=""><%= WebTextManager.GetText("/pageBranch/goToPage") %></label></p>
                        </div>
                        <div class="left dropDown" style="margin-left:5px;">
                            <asp:DropDownList ID="_goToPageList" runat="server" EnableViewState="true" AutoPostBack="false" CssClass="branchTargetSelector" uframeignore="true"/>
                        </div>
                        <br class="clear"/>
                    </asp:Panel>
                    <ckbx:AdvancedExpressionEditor ID="_advancedEditor" runat="server"/>
                </div>
            </ItemTemplate>
        </asp:Repeater>
        
        <div class="detailZebra" style="margin-top:15px;">
            <asp:Panel ID="_newBranchPanel" runat="server" CssClass="formInput ruleEditorWrapper">
                <div class="left">
                    <p><label for=""><%= WebTextManager.GetText("/pageBranch/goToPage") %></label></p>
                </div>
                <div class="left dropDown" style="margin-left:5px;">
                    <asp:DropDownList ID="_newBranchPageList" runat="server" CssClass="branchTargetSelector"/>
                </div>
                <br class="clear"/>
                <ckbx:AdvancedExpressionEditor ID="_newBranchEditor" AddExpressionMode="true" runat="server" />
            </asp:Panel>
        </div>
    </div>
</div>




