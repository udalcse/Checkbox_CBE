<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Image.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemEditors.Image" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemEditors/ImageSelector.ascx" TagPrefix="sel" TagName="ImageSelector" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemEditors/ImageOptions.ascx" TagPrefix="opt" TagName="ImageOptions" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemConditionEditor.ascx" TagPrefix="ckbx" TagName="ConditionEditor" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemConditionsRuleDislpay.ascx" TagPrefix="ckbx" TagName="RuleDisplay" %>
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
                $('#itemEditorTabs').ckbxTabs('hideTab', 4);
            <%} %>
        });
    </script>

    <div style="display:none;">
        <asp:TextBox ID="_currentTabIndex" runat="server" Text="0" />
    </div>

    <ul id="itemEditorTabs" class="tabContainer">
        <li><%=WebTextManager.GetText("/pageText/formEditor.aspx/preview")%></li>
        <li><%=WebTextManager.GetText("/controlText/imageItemEditor/image")%></li>
        <li><%=WebTextManager.GetText("/controlText/imageItemEditor/accessibility")%></li>
        <li><%=WebTextManager.GetText("/controlText/itemEditor/appearance")%></li>
        <li><%=WebTextManager.GetText("/controlText/templatePageEditor/conditions")%></li>
    </ul>
    <div class="clear"></div>

    <div class="tabContentContainer">
        <div id="itemEditorTabs-0-tabContent" class="padding10">
            <ckbx:ActiveDisplay ID="_activeDisplay" runat="server" />

            <asp:PlaceHolder ID="_previewPlace" runat="server" />
            
            <div class="clear"></div>
            <hr size="1" />
            <ckbx:RuleDisplay ID="_ruleDisplay" runat="server" />
            <div class="clear"></div>
        </div>

        <div id="itemEditorTabs-1-tabContent">
            <sel:ImageSelector ID="_imageSelector" runat="server" />
        </div>
        
        <div id="itemEditorTabs-2-tabContent" class="padding10">
            <opt:ImageOptions ID="_imageOptions" runat="server" />
        </div>
        
        <div id="itemEditorTabs-3-tabContent" class="padding10">
            <asp:PlaceHolder ID="_appearanceEditorPlace" runat="server" />
        </div>

         <div id="itemEditorTabs-4-tabContent">
            <ckbx:ConditionEditor ID="_conditionEditor" runat="server" />
        </div>
    </div>