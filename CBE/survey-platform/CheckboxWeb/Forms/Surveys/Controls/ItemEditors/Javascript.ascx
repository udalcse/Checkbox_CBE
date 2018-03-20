<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Javascript.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemEditors.Javascript" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Register TagPrefix="ckbx" TagName="ActiveDisplay" Src="~/Forms/Surveys/Controls/ItemActiveDisplay.ascx" %>
<%@ Register TagPrefix="ckbx" TagName="ConditionEditor" Src="~/Forms/Surveys/Controls/ItemConditionEditor.ascx" %>
<%@ Register TagPrefix="ckbx" TagName="ScriptEditor" src="~/Forms/Surveys/Controls/ItemEditors/JavascriptItemScriptEditor.ascx"  %>
<%@ Register TagPrefix="ckbx" TagName="RuleDisplay" Src="~/Forms/Surveys/Controls/ItemConditionsRuleDislpay.ascx" %>

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
                $('#itemEditorTabs').ckbxTabs('hideTab', 2);
            <%} %>
        });
</script>

<div style="display:none;">
    <asp:TextBox ID="_currentTabIndex" runat="server" Text="0" />
</div>

<ul id="itemEditorTabs" class="tabContainer">
    <li><%=WebTextManager.GetText("/pageText/formEditor.aspx/preview")%></li>
    <li><%=WebTextManager.GetText("/pageText/formEditor.aspx/edit")%></li>
    <li><%=WebTextManager.GetText("/controlText/templatePageEditor/conditions")%></li>
</ul>
<div class="clear">
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
    <div id="itemEditorTabs-1-tabContent">
        <ckbx:ScriptEditor ID="_scriptEditor" runat="server" />
    </div>
    <div id="itemEditorTabs-2-tabContent">
        <ckbx:ConditionEditor ID="_conditionEditor" runat="server" />
    </div>
</div>
