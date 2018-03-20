<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="CloseWindow.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemEditors.CloseWindow" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemConditionEditor.ascx" TagPrefix="ckbx" TagName="ConditionEditor" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemConditionsRuleDislpay.ascx" TagPrefix="ckbx" TagName="RuleDisplay" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemActiveDisplay.ascx" TagPrefix="ckbx" TagName="ActiveDisplay" %>


    <script type="text/javascript">
         $(document).ready(function () {
            <%if(EditMode != Checkbox.Forms.EditMode.Survey) { %>
                $('#itemEditorTabs').hide();
                $('#itemEditorTabs-1-tabContent').hide();

            <% } else { %>
                $('#itemEditorTabs').ckbxTabs({ 
                    tabName: 'itemEditorTabs',
                    onTabClick: function (index) { $('#<%=_currentTabIndex.ClientID %>').val(index) },
                    onTabsLoaded: function(){$('.tabContainer').show();$('.tabContentContainer').show();}
                });
            <% } %>
        });
    </script>


    <div style="display:none;">
        <asp:TextBox ID="_currentTabIndex" runat="server" Text="0" />
    </div>
    
    <ul id="itemEditorTabs" class="tabContainer">
        <li><%=WebTextManager.GetText("/pageText/formEditor.aspx/edit")%></li>
        <li><%=WebTextManager.GetText("/controlText/templatePageEditor/conditions")%></li>
    </ul>

    <div class="clear"></div>

    <div class="tabContentContainer">
        <div id="itemEditorTabs-0-tabContent" class="padding10">
            <ckbx:ActiveDisplay ID="_activeDisplay" runat="server" />

            <ckbx:MultiLanguageLabel ID="_noPropertiesLbl" runat="server" TextId="/controlText/closeWindowItemEditor/noPropertiesEdit" />

            <div class="clear"></div>
            <hr size="1" />
            <ckbx:RuleDisplay ID="_ruleDisplay" runat="server" />
            <div class="clear"></div>
        </div>

        <div id="itemEditorTabs-1-tabContent">
            <ckbx:ConditionEditor ID="_conditionEditor" runat="server" />
        </div>
    </div>
    <div class="clear"></div>
