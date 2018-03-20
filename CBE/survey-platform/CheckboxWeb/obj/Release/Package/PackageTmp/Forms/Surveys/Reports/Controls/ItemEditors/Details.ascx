<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Details.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.Controls.ItemEditors.Details" %>
<%@ Register Src="~/Forms/Surveys/Reports/Controls/ItemEditors/SourceItemSelector.ascx" TagPrefix="ckbx" TagName="SourceItemSelector" %>
<%@ Register Src="~/Forms/Surveys/Reports/Controls/ItemEditors/DetailsItemOptions.ascx" TagPrefix="ckbx" TagName="DetailsOptions" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemConditionEditor.ascx" TagPrefix="ckbx" TagName="ConditionEditor" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemConditionsRuleDislpay.ascx" TagPrefix="ckbx" TagName="RuleDisplay" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemActiveDisplay.ascx" TagPrefix="ckbx" TagName="ActiveDisplay" %>
<%@ Register TagPrefix="ckbx" TagName="FilterSelector" Src="~/Forms/Surveys/Reports/Controls/FilterSelector.ascx" %>

<%@ Import Namespace="Checkbox.Web"%>

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
                $('#itemEditorTabs').ckbxTabs('hideTab', 4);
                $('#ruleDisplay').hide();
            <%} %>
        });
    </script>

    <div style="display:none;">
        <asp:TextBox ID="_currentTabIndex" runat="server" Text="0" />
    </div>

    <ul id="itemEditorTabs" class="tabContainer">
        <li><%=WebTextManager.GetText("/pageText/formEditor.aspx/preview")%></li>
        <li><%=WebTextManager.GetText("/controlText/chartEditor/sourceItems")%></li>
        <li><%=WebTextManager.GetText("/controlText/chartEditor/behavior")%></li>
        <li><%=WebTextManager.GetText("/controlText/chartEditor/appearance")%></li>
        <li><%=WebTextManager.GetText("/controlText/templatePageEditor/conditions")%></li>
        <li><%=WebTextManager.GetText("/pageText/editAnalysis.aspx/filters")%></li>
    </ul>
    <div class="clear"></div>

    <div class="tabContentContainer">
        <div id="itemEditorTabs-0-tabContent" class="padding10">
            <ckbx:ActiveDisplay ID="_activeDisplay" runat="server" />

            <asp:PlaceHolder ID="_previewPlace" runat="server" />

            <div class="clear"></div>

            <div id="ruleDisplay">
                <hr size="1" />
                <ckbx:RuleDisplay ID="_ruleDisplay" runat="server" />
                <div class="clear"></div>
            </div>
        </div>

        <div id="itemEditorTabs-1-tabContent" class="padding10">
            <ckbx:SourceItemSelector runat="server" ID="_sourceItemSelector" />
        </div>
        
        <div id="itemEditorTabs-2-tabContent" class="padding10">
            <ckbx:DetailsOptions ID="_detailsOptions" runat="server" />
        </div>
        
        <div id="itemEditorTabs-3-tabContent" class="padding10">
            <asp:PlaceHolder ID="_appearanceEditorPlace" runat="server"></asp:PlaceHolder>
        </div>

        <div id="itemEditorTabs-4-tabContent">
            <ckbx:ConditionEditor ID="_conditionEditor" runat="server" />
        </div>

         <div id="itemEditorTabs-5-tabContent">
            <ckbx:FilterSelector ID="_filterSelector" runat="server" />
        </div>
    </div>
