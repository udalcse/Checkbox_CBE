<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="TotalScore.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.Controls.ItemEditors.TotalScore" %>
<%@ Register Src="~/Forms/Surveys/Reports/Controls/ItemEditors/SourceItemSelector.ascx" TagPrefix="ckbx" TagName="SourceItemSelector" %>
<%@ Register Src="~/Forms/Surveys/Reports/Controls/ItemEditors/TotalScoreOptions.ascx" TagPrefix="ckbx" TagName="TotalScoreOptions" %>
<%@ Import Namespace="Checkbox.Web"%>

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
    </ul>
    <div class="clear"></div>

    <div class="tabContentContainer padding10">
        <div id="itemEditorTabs-0-tabContent">
            <asp:PlaceHolder ID="_previewPlace" runat="server" />
        </div>

        <div id="itemEditorTabs-1-tabContent">
            <ckbx:SourceItemSelector runat="server" ID="_sourceItemSelector" />
        </div>
        
        <div id="itemEditorTabs-2-tabContent">
            <ckbx:TotalScoreOptions id="_scoreOptions" runat="server" />
        </div>
        
        <div id="itemEditorTabs-3-tabContent">
            <asp:PlaceHolder ID="_appearanceEditorPlace" runat="server" />
        </div>
    </div>
