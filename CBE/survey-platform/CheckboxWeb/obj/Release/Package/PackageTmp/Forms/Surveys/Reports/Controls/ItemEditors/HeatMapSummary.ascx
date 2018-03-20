<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="HeatMapSummary.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.Controls.ItemEditors.HeatMapSummary" %>

<%@ Import Namespace="Checkbox.Web"%>
<%@ Register TagPrefix="ckbx" TagName="ChartPreview" Src="~/Styles/Controls/ChartPreview.ascx" %>
<%@ Register Src="~/Forms/Surveys/Reports/Controls/ItemEditors/SourceItemSelector.ascx" TagPrefix="ckbx" TagName="SourceItemSelector" %>
<%@ Register Src="~/Forms/Surveys/Reports/Controls/ItemEditors/HeatMapBehavior.ascx" TagPrefix="ckbx" TagName="HeatMapBehavior" %>
<%@ Register Src="~/Forms/Surveys/Reports/Controls/ItemEditors/HeatMapDataTab.ascx" TagPrefix="ckbx" TagName="HeatMapDataTab" %>
<%@ Register Assembly="CheckboxWeb" Namespace="CheckboxWeb.Controls.Button" TagPrefix="btn" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemActiveDisplay.ascx" TagPrefix="ckbx" TagName="ActiveDisplay" %>


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
                $('#itemEditorTabs').ckbxTabs('hideTab', 5);
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
        <li><%=WebTextManager.GetText("/controlText/chartEditor/data")%></li>
        <li><%=WebTextManager.GetText("/controlText/chartEditor/behavior")%></li>
        <li><%=WebTextManager.GetText("/controlText/chartEditor/appearance")%></li>
      
        <div class="clear"></div>
    </ul>
    <div class="clear"></div>

    <div class="tabContentContainer">
        <div id="itemEditorTabs-0-tabContent" class="padding10">
            <ckbx:ActiveDisplay ID="_activeDisplay" runat="server" />

            <asp:PlaceHolder ID="_previewPlace" runat="server" />

            <div class="clear"></div>

        </div>

        <div id="itemEditorTabs-1-tabContent" class="padding10">
            <ckbx:SourceItemSelector runat="server" ID="_sourceItemSelector" />
        </div>

        <div id="itemEditorTabs-2-tabContent" class="padding10">
            <ckbx:HeatMapDataTab runat="server" ID="_heatMapData" />
        </div>
        
        <div id="itemEditorTabs-3-tabContent" class="padding10">
            <ckbx:HeatMapBehavior ID="_behavior" runat="server" />
        </div>
        
        <div id="itemEditorTabs-4-tabContent">
            <asp:PlaceHolder ID="_appearanceEditorPlace" runat="server" />
        </div>
    </div>
 
