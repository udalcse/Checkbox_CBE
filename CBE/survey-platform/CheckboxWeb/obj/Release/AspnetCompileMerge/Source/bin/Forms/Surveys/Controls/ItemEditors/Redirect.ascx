<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Redirect.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemEditors.Redirect" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemEditors/RedirectOptions.ascx" TagPrefix="opt" TagName="RedirectOptions" %>
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
                $('#itemEditorTabs').ckbxTabs('hideTab', 3);
            <%} %>
        });
    </script>

    <div style="display:none;">
        <asp:TextBox ID="_currentTabIndex" runat="server" Text="0" />
    </div>

    <ul id="itemEditorTabs" class="tabContainer">
        <li><%=WebTextManager.GetText("/pageText/formEditor.aspx/preview")%></li>
        <li><%=WebTextManager.GetText("/controlText/redirectItemEditor/linkOptions")%></li>
        <li><%=WebTextManager.GetText("/controlText/itemEditor/appearance")%></li>
        <li><%=WebTextManager.GetText("/controlText/templatePageEditor/conditions")%></li>
    </ul>
    <div class="clear"></div>

    <div class="tabContentContainer">
        <div id="itemEditorTabs-0-tabContent" class="padding10">
            <ckbx:ActiveDisplay ID="_activeDisplay" runat="server" />

            <asp:PlaceHolder ID="_previewPlace" runat="server"></asp:PlaceHolder>

            <div class="clear"></div>
            <hr size="1" />
            <ckbx:RuleDisplay ID="_ruleDisplay" runat="server" />
            <div class="clear"></div>
        </div>

        <div id="itemEditorTabs-1-tabContent" class="padding10">
            <opt:RedirectOptions ID="_redirectOptions" runat="server" />

            <div class="formInput fixed_75 padding10" style="padding-bottom:0px">
                <p><ckbx:MultiLanguageLabel AssociatedControlID="_aliasText" ID="MultiLanguageLabel1" runat="server" TextId="/controlText/listEditor/alias" /></p>
            </div>
            <div class="formInput padding10" style="padding-top:0px">
                <asp:TextBox ID="_aliasText" runat="server" />
            </div>

        </div>
        
        <div id="itemEditorTabs-2-tabContent" class="padding10">
            <asp:PlaceHolder ID="_appearanceEditorPlace" runat="server" />
        </div>

          
        <div id="itemEditorTabs-3-tabContent">
            <ckbx:ConditionEditor ID="_conditionEditor" runat="server" />
        </div>
    </div>