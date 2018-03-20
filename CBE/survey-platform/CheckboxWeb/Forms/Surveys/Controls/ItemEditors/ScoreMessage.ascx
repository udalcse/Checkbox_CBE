<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="ScoreMessage.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemEditors.ScoreMessage" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemEditors/ScoreMessagesEditor.ascx" TagPrefix="edt" TagName="ScoreMessagesEditor" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemEditors/ScoreMessageBehavior.ascx" TagPrefix="cs" TagName="ScoreMessageBehavior" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemConditionEditor.ascx" TagPrefix="ckbx" TagName="ConditionEditor" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemConditionsRuleDislpay.ascx" TagPrefix="ckbx" TagName="RuleDisplay" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemActiveDisplay.ascx" TagPrefix="ckbx" TagName="ActiveDisplay" %>

<script type="text/javascript">
    $(document).ready(function () {
            
        $('#itemEditorTabs').ckbxTabs({ 
            tabName: 'itemEditorTabs',
            initialTabIndex:<%=_currentTabIndex.Text %>,
            onTabClick: function(index){ $('#<%=_currentTabIndex.ClientID %>').val(index);},
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
    <asp:TextBox ID="_currentTabIndex" runat="server" Text="0" />
</div>

<ul id="itemEditorTabs" class="tabContainer">
    <li><%=WebTextManager.GetText("/pageText/formEditor.aspx/preview")%></li>
    <li><%=WebTextManager.GetText("/controlText/scoreMessageItemEditor/messages")%></li>
    <li><%=WebTextManager.GetText("/controlText/itemEditor/behavior")%></li>
    <li><%=WebTextManager.GetText("/controlText/itemEditor/appearance")%></li>
    <li><%=WebTextManager.GetText("/controlText/templatePageEditor/conditions")%></li>
</ul>

<div class="tabContentContainer">
    <div class="padding10" id="itemEditorTabs-0-tabContent">
        <ckbx:ActiveDisplay ID="_activeDisplay" runat="server" />
        
        <div class="ScoreMessage">
                    <asp:PlaceHolder ID="_previewPlace" runat="server"></asp:PlaceHolder>
        </div>

        <div class="clear"></div>
        <hr size="1" />
        <ckbx:RuleDisplay ID="_ruleDisplay" runat="server" />
        <div class="clear"></div>
    </div>

    <div class="padding10" id="itemEditorTabs-1-tabContent">
        <edt:ScoreMessagesEditor ID="_scoreMessagesEditor" runat="server" />
    </div>

    <div class="padding10" id="itemEditorTabs-2-tabContent">
        <cs:ScoreMessageBehavior ID="_scoreMessageBehavior" runat="server" />
    </div>
        
    <div class="padding10" id="itemEditorTabs-3-tabContent">
        <asp:PlaceHolder ID="_appearancePlace" runat="server" />
    </div>

    <div id="itemEditorTabs-4-tabContent">
        <ckbx:ConditionEditor ID="_conditionEditor" runat="server" />
    </div>
</div>