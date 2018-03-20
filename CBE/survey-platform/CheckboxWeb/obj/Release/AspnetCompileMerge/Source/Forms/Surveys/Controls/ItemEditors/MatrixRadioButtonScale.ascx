<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="MatrixRadioButtonScale.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemEditors.MatrixRadioButtonScale" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemEditors/QuestionTextEditor.ascx" TagPrefix="qte" TagName="QuestionTextEditor" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemEditors/SelectOptionsEditor.ascx" TagPrefix="sle" TagName="SelectOptionsEditor" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemEditors/RatingScaleBehavior.ascx" TagPrefix="be" TagName="Behavior" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemEditors/ColumnOptionsEditor.ascx" TagPrefix="cle" TagName="ColumnOptionsEditor" %>

    <script type="text/javascript">
        $(document).ready(function () {
            
            $('#itemEditorTabs').ckbxTabs({ 
                tabName: 'itemEditorTabs',
                initialTabIndex:<%=_currentTabIndex.Text %>,
                onTabClick: function(index){$('#<%=_currentTabIndex.ClientID %>').val(index)},
                onTabsLoaded: function(){$('.tabContainer').show();$('.tabContentContainer').show();}
            });
        });
    </script>
    
    <div style="display:none;">
        <asp:TextBox ID="_currentTabIndex" runat="server" Text="0" />
    </div>

    <ul id="itemEditorTabs" class="tabContainer">
        <li><%=WebTextManager.GetText("/controlText/itemEditor/question")%></li>
        <li><%=WebTextManager.GetText("/controlText/itemEditor/behavior")%></li>
        <li><%=WebTextManager.GetText("/pageText/matrixItemEditor.aspx/otherOptions")%></li>
    </ul>
    
    <div class="clear"></div>
        
    <div class="tabContentContainer">
        <div id="itemEditorTabs-0-tabContent">
            <qte:QuestionTextEditor ID="_questionTextEditor" runat="server" EditorHeight="425" EditorWidth="600" />
        </div>
        <div id="itemEditorTabs-1-tabContent" class="padding10">
            <be:Behavior ID="_behaviorEditor" runat="server" />
        </div>
        <div id="itemEditorTabs-2-tabContent" class="padding10">
            <cle:ColumnOptionsEditor ID="_columnOptionsEditor" runat="server" />
        </div>
    </div>
