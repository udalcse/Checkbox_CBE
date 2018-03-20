<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Message.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemEditors.Message" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemEditors/QuestionTextEditor.ascx" TagPrefix="qte" TagName="QuestionTextEditor" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemConditionEditor.ascx" TagPrefix="ckbx" TagName="ConditionEditor" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemConditionsRuleDislpay.ascx" TagPrefix="ckbx" TagName="RuleDisplay" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemActiveDisplay.ascx" TagPrefix="ckbx" TagName="ActiveDisplay" %>
<%@ Register Src="~/Forms/Surveys/Controls/SectionEditor.ascx" TagPrefix="ckbx" TagName="SectionEditor" %>

<%@ Register TagPrefix="ckbx" Namespace="CheckboxWeb.Forms.Surveys.Controls" Assembly="CheckboxWeb" %>

<%@ Import Namespace="Checkbox.Web" %>
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
        <li>Sections</li>
        <div class="clear"></div>
    </ul>
    <div class="clear"></div>

    <div class="tabContentContainer">
        <div id="itemEditorTabs-0-tabContent" class="padding10">
            <ckbx:ActiveDisplay ID="_activeDisplay" runat="server" />
            
            <asp:PlaceHolder ID="_previewPlace" runat="server"></asp:PlaceHolder>

            <div class="clear"></div>
            <ckbx:RuleDisplay ID="_ruleDisplay" runat="server" />
            <div class="clear"></div>
            <br>
            <%--<asp:CheckBox runat="server" ID="_reportableSection" Text="Reportable section" CssClass="item-editor-content reportable-section"/>--%>
        </div>

        <div id="itemEditorTabs-1-tabContent">
            <qte:QuestionTextEditor ID="_textEditor" IsHorizontalRuleEnabled="True" runat="server" />
            <div class="clear"></div>
        </div>
        
         <div id="itemEditorTabs-2-tabContent">
            <ckbx:ConditionEditor ID="_conditionEditor" runat="server" />
        </div>

        <div id="itemEditorTabs-3-tabContent">
            <ckbx:SectionEditor ID="_sectionEditor" runat="server" />
        </div>

        <% if (EditMode == Checkbox.Forms.EditMode.Library) %>
        <%{ %>
        <div class="formInput left fixed_75 padding10">
            <p><ckbx:MultiLanguageLabel AssociatedControlID="_aliasText" ID="MultiLanguageLabel1" runat="server" TextId="/controlText/listEditor/alias" /></p>
        </div>
        <div class="formInput left padding10">
            <asp:TextBox ID="_aliasText" runat="server" />
        </div>
        <%} %>

    </div>
    <div class="clear"></div>

