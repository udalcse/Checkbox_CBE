﻿<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="AverageScoreByPage.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.Controls.ItemEditors.AverageScoreByPage" %>
<%@ Register Src="~/Forms/Surveys/Reports/Controls/ItemEditors/SourcePageSelector.ascx" TagPrefix="ckbx" TagName="SourcePageSelector" %>
<%@ Register Src="~/Forms/Surveys/Reports/Controls/ItemEditors/AverageScoreItemOptions.ascx" TagPrefix="ckbx" TagName="AverageScoreOptions" %>
<%@ Register Src="~/Forms/Surveys/Reports/Controls/FilterSelector.ascx" TagPrefix="ckbx" TagName="FilterSelector" %>
<%@ Import Namespace="Checkbox.Web"%>

<script type="text/javascript">
        $(document).ready(function () {

            $('#itemEditorTabs').ckbxTabs({ 
                tabName: 'itemEditorTabs',
                initialTabIndex: <%=_currentTabIndex.Text %>,
                onTabClick: function(index){$('#<%=_currentTabIndex.ClientID %>').val(index)},
                onTabsLoaded: function(){$('.tabContainer').show();$('.tabContentContainer').show();}
            });

            $('#itemEditorTabs').ckbxTabs('hideTab', 2);

            <%if(HidePreview) { %>
                $('#itemEditorTabs').ckbxTabs('hideTab', 0);
            <% } %>
        });
    </script>

    <div style="display:none;">
        <asp:TextBox ID="_currentTabIndex" runat="server" Text="0" />
    </div>

    <ul id="itemEditorTabs" class="tabContainer">
        <li><%=WebTextManager.GetText("/pageText/formEditor.aspx/preview")%></li>
        <li><%=WebTextManager.GetText("/controlText/chartEditor/sourcePages")%></li>
        <li><%=WebTextManager.GetText("/controlText/chartEditor/behavior")%></li>
        <li><%=WebTextManager.GetText("/controlText/chartEditor/appearance")%></li>
        <li><%=WebTextManager.GetText("/pageText/editAnalysis.aspx/filters")%></li>
    </ul>
    <div class="clear"></div>

    <div class="tabContentContainer padding10">
        <div id="itemEditorTabs-0-tabContent">
            <asp:PlaceHolder ID="_previewPlace" runat="server" />
        </div>

        <div id="itemEditorTabs-1-tabContent">
            <ckbx:SourcePageSelector runat="server" ID="_sourcePageSelector" />
        </div>
        
        <div id="itemEditorTabs-2-tabContent">
            <ckbx:AverageScoreOptions id="_scoreOptions" runat="server" />
        </div>
        
        <div id="itemEditorTabs-3-tabContent">
            <asp:PlaceHolder ID="_appearanceEditorPlace" runat="server" />
        </div>

        <div id="itemEditorTabs-4-tabContent">
            <ckbx:FilterSelector ID="_filterSelector" runat="server" />
        </div>

    </div>

