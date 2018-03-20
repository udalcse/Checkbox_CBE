<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GradientColorDirectorSkillsMatrix.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.Controls.ItemEditors.GradientColorDirectorSkillsMatrix" %>

<%@ Import Namespace="Checkbox.Web"%>
<%@ Register Src="~/Forms/Surveys/Reports/Controls/ItemEditors/SourceItemSelector.ascx" TagPrefix="ckbx" TagName="SourceItemSelector" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemActiveDisplay.ascx" TagPrefix="ckbx" TagName="ActiveDisplay" %>
<%@ Register Src="~/Forms/Surveys/Reports/Controls/ItemEditors/GradientColorDirectorSkillsMatrixOptions.ascx" TagPrefix="ckbx" TagName="GradientColorDirectorSkillsMatrixOptions" %>
<%@ Register TagPrefix="ckbx" TagName="ConditionEditor" Src="~/Forms/Surveys/Controls/ItemConditionEditor.ascx" %>


<%@ Register TagPrefix="ckbx" TagName="FilterSelector" Src="~/Forms/Surveys/Reports/Controls/FilterSelector.ascx" %>

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
        <li><%=WebTextManager.GetText("/controlText/chartEditor/behavior")%></li>
        <li><%=WebTextManager.GetText("/controlText/chartEditor/appearance")%></li>
        <li><%=WebTextManager.GetText("/pageText/editAnalysis.aspx/filters")%></li>
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
            <ckbx:GradientColorDirectorSkillsMatrixOptions ID="_itemBehavior" runat="server" />
        </div>
        <div id="itemEditorTabs-3-tabContent">
            <asp:PlaceHolder ID="_appearanceEditorPlace" runat="server" />
        </div>

         <div id="itemEditorTabs-4-tabContent">
            <ckbx:FilterSelector ID="_filterSelector" runat="server" />
        </div>
      

    </div>
 


