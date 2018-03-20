<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="MatrixSummary.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.Controls.ItemEditors.MatrixSummary" %>
<%@ Register Src="~/Forms/Surveys/Reports/Controls/ItemEditors/SourceItemSelector.ascx" TagPrefix="ckbx" TagName="SourceItemSelector" %>
<%@ Register Src="~/Forms/Surveys/Reports/Controls/ItemEditors/MatrixSummaryOptions.ascx" TagPrefix="ckbx" TagName="MatrixSummaryOptions" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemConditionEditor.ascx" TagPrefix="ckbx" TagName="ConditionEditor" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemConditionsRuleDislpay.ascx" TagPrefix="ckbx" TagName="RuleDisplay" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemActiveDisplay.ascx" TagPrefix="ckbx" TagName="ActiveDisplay" %>
<%@ Register Src="~/Forms/Surveys/Reports/Controls/FilterSelector.ascx" TagPrefix="ckbx" TagName="FilterSelector" %>

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
            <p><%=WebTextManager.GetText("/controlText/matrixSummaryItemEditor/sourceItems")%></p>
            <asp:DropDownList ID="_selectedMatrix" runat ="server">
            </asp:DropDownList>
        </div>
        
        <div id="itemEditorTabs-2-tabContent">
            <ckbx:MatrixSummaryOptions ID="_options" runat="server" />
        </div>

         <div id="itemEditorTabs-3-tabContent">
            <ckbx:FilterSelector ID="_filterSelector" runat="server" />
        </div>
    </div>
