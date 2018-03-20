<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Analysis_Average_Score.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.Controls.AppearanceEditors.Analysis_Average_Score" %>
<%@ Register TagPrefix="ckbx" TagName="Margins" Src="~/Styles/Charts/AverageScore/EditorControls/AverageScoreMargins.ascx" %>
<%@ Register TagPrefix="ckbx" TagName="Border" Src="~/Styles/Charts/AverageScore/EditorControls/Border.ascx" %>
<%@ Register TagPrefix="ckbx" TagName="Legend" Src="~/Styles/Charts/AverageScore/EditorControls/Legend.ascx" %>
<%@ Register TagPrefix="ckbx" TagName="Other" Src="~/Styles/Charts/AverageScore/EditorControls/Other.ascx" %>
<%@ Register TagPrefix="ckbx" TagName="Options" Src="~/Styles/Charts/AverageScore/EditorControls/Options.ascx" %>
<%@ Register TagPrefix="ckbx" TagName="Text" Src="~/Styles/Charts/AverageScore/EditorControls/Text.ascx" %>
<%@ Import Namespace="Checkbox.Web"%>

    <script type="text/javascript">
        $(document).ready(function () {
            $('#averageScoreAppearanceTabs').ckbxTabs({ 
                tabName: 'averageScoreAppearanceTabs',
                tabStyle: 'inverted',
                initialTabIndex: <%=_currentTabIndex.Text %>,
                onTabClick: function(index){$('#<%=_currentTabIndex.ClientID %>').val(index)},
                onTabsLoaded: function(){$('.tabContainer').show();$('.tabContentContainer').show();}
            });

            $('#averageScoreAppearanceTabs').ckbxTabs('hideTab', 0);
        });
    </script>

    <div style="display:none;">
        <asp:TextBox ID="_currentTabIndex" runat="server" Text="1" />
    </div>

    <ul id="averageScoreAppearanceTabs" class="tabContainer">
        <li><%=WebTextManager.GetText("/pageText/formEditor.aspx/preview")%></li>
         <li><%=WebTextManager.GetText("/pageText/styles/charts/editor.aspx/graphType")%></li>
        <li><%=WebTextManager.GetText("/pageText/styles/charts/editor.aspx/margins")%></li>
        <li><%=WebTextManager.GetText("/pageText/styles/charts/editor.aspx/border")%></li>
        <li><%=WebTextManager.GetText("/pageText/styles/charts/editor.aspx/legend")%></li>
        <li><%=WebTextManager.GetText("/pageText/styles/charts/editor.aspx/other")%></li>
        <li><%=WebTextManager.GetText("/pageText/styles/charts/editor.aspx/text")%></li>
    </ul>
    <div class="clear"></div>

    <div class="tabContentContainer padding10">
        <div id="averageScoreAppearanceTabs-0-tabContent">
            <asp:PlaceHolder ID="_previewPlace" runat="server" />
        </div>
        <div id="averageScoreAppearanceTabs-1-tabContent">
            <ckbx:Options ID="_options" runat="server"  />
        </div>
        <div id="averageScoreAppearanceTabs-2-tabContent">
            <ckbx:Margins ID="_margins" runat="server" />
        </div>
        <div id="averageScoreAppearanceTabs-3-tabContent">
            <ckbx:Border ID="_border" runat="server" />
        </div>
         <div id="averageScoreAppearanceTabs-4-tabContent">
            <ckbx:Legend ID="_legend" runat="server"  />
        </div>
        <div id="averageScoreAppearanceTabs-5-tabContent">
            <ckbx:Other ID="_other" runat="server"  />
        </div>
        <div id="averageScoreAppearanceTabs-6-tabContent">
            <ckbx:Text ID="_text" runat="server"  />
        </div>
    </div>