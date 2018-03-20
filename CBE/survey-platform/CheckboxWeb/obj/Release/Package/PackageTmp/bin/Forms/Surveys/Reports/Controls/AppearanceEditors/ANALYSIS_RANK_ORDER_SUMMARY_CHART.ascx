<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ANALYSIS_RANK_ORDER_SUMMARY_CHART.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.Controls.AppearanceEditors.ANALYSIS_RANK_ORDER_SUMMARY_CHART" %>
<%@ Register TagPrefix="ckbx" TagName="GraphOptions" Src="~/Styles/Charts/EditorControls/GraphOptions.ascx" %>
<%@ Register TagPrefix="ckbx" TagName="Border" Src="~/Styles/Charts/EditorControls/Border.ascx" %>
<%@ Register TagPrefix="ckbx" TagName="Text" Src="~/Styles/Charts/EditorControls/Text.ascx" %>
<%@ Register TagPrefix="ckbx" TagName="Other" Src="~/Styles/Charts/EditorControls/Other.ascx" %>
<%@ Import Namespace="Checkbox.Web"%>

    <script type="text/javascript">
        $(document).ready(function () {
            $('#summaryAppearanceTabs').ckbxTabs({ 
                tabName: 'summaryAppearanceTabs',
                tabStyle: 'inverted',
                initialTabIndex: <%=_currentTabIndex.Text %>,
                onTabClick: function(index){$('#<%=_currentTabIndex.ClientID %>').val(index)},
                onTabsLoaded: function(){$('.tabContainer').show();$('.tabContentContainer').show();}
            });

            $('#summaryAppearanceTabs').ckbxTabs('hideTab', 0);
        });
    </script>

    <div style="display:none;">
        <asp:TextBox ID="_currentTabIndex" runat="server" Text="1" />
    </div>

    <ul id="summaryAppearanceTabs" class="tabContainer">
        <li><%=WebTextManager.GetText("/pageText/formEditor.aspx/preview")%></li>
        <li><%=WebTextManager.GetText("/pageText/styles/charts/editor.aspx/graphType")%></li>
        <li><%=WebTextManager.GetText("/pageText/styles/charts/editor.aspx/border")%></li>
        <li><%=WebTextManager.GetText("/pageText/styles/charts/editor.aspx/text")%></li>
        <li><%=WebTextManager.GetText("/pageText/styles/charts/editor.aspx/other")%></li>
    </ul>
    <div class="clear"></div>

    <div class="tabContentContainer padding10">
        <div id="summaryAppearanceTabs-0-tabContent">
            <asp:PlaceHolder ID="_previewPlace" runat="server" />
        </div>
        <div id="summaryAppearanceTabs-1-tabContent">
            <ckbx:GraphOptions ID="_graphOptions" runat="server" Force3DSettingsEnabled="false" ShowGraphTypeList="true" />
        </div>
        <div id="summaryAppearanceTabs-2-tabContent">
            <ckbx:Border ID="_border" runat="server" Force3DSettingsEnabled="false" ShowGraphTypeList="true"  />
        </div>
        <div id="summaryAppearanceTabs-3-tabContent">
            <ckbx:Text ID="_text" runat="server" Force3DSettingsEnabled="false" ShowGraphTypeList="true"  />
        </div>
        <div id="summaryAppearanceTabs-4-tabContent">
            <ckbx:Other ID="_other" runat="server" Force3DSettingsEnabled="false" ShowGraphTypeList="true"  />
        </div>
    </div>

