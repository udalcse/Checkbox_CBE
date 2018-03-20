<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="MatrixSlider.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemEditors.MatrixSlider" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemEditors/QuestionTextEditor.ascx" TagPrefix="qte"TagName="QuestionTextEditor" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemEditors/SliderOptionsEditor.ascx" TagPrefix="sle" TagName="SliderOptionsEditor" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemEditors/SliderBehavior.ascx" TagPrefix="be" TagName="Behavior" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemEditors/ColumnOptionsEditor.ascx"TagPrefix="cle" TagName="ColumnOptionsEditor" %>

    <script type="text/javascript">
        $(document).ready(function () {
            
            $('#itemEditorTabs').ckbxTabs({ 
                tabName: 'itemEditorTabs',
                initialTabIndex:<%=_currentTabIndex.Text %>,
                onTabClick: function(index){onMainTabChange(index)},
                onTabsLoaded: function(){$('.tabContainer').show();$('.tabContentContainer').show();}
            });
        });

     //
     function onMainTabChange(newTabIndex) {
         //Set value of current tab and cause postback, so options entry dialogs can be updated
         $('#<%=_currentTabIndex.ClientID%>').val(newTabIndex);

         //Simulate button click. _choiceTabIsOld is defined in SliderBehavior tab
         if (typeof _choiceTabIsOld != "undefined" && _choiceTabIsOld){
             if (typeof (UFrameManager) == 'undefined') {
                eval($('#<%=_tabChangeBtn.ClientID %>').attr('href'));
             }
             else {
                 UFrameManager.executeASPNETPostback($('#<%=_tabChangeBtn.ClientID %>'), $('#<%=_tabChangeBtn.ClientID %>').attr('href'));
             }
             _choiceTabIsOld = false;
         }
     }
    </script>
    
    <div style="display:none;">
        <asp:TextBox ID="_currentTabIndex" runat="server" Text="0" />
        <asp:LinkButton ID="_tabChangeBtn" runat="server" />
    </div>

    <ul id="itemEditorTabs" class="tabContainer">
        <li><%=WebTextManager.GetText("/controlText/itemEditor/question")%></li>
        <li><%=WebTextManager.GetText("/controlText/itemEditor/behavior")%></li>
        <li><%=WebTextManager.GetText("/controlText/itemEditor/choices")%></li>
        <li><%=WebTextManager.GetText("/pageText/matrixItemEditor.aspx/otherOptions")%></li>
        <li><%=WebTextManager.GetText("/controlText/itemEditor/appearance")%></li>
    </ul>

    <div class="clear"></div>

    <div class="tabContentContainer">
        <div id="itemEditorTabs-0-tabContent">
            <qte:QuestionTextEditor ID="_questionTextEditor" runat="server" EditorHeight="425" EditorWidth="600" />
        </div>
        <div id="itemEditorTabs-1-tabContent" class="padding10">
            <be:Behavior ID="_behaviorEditor" runat="server" />
        </div>
        <div id="itemEditorTabs-2-tabContent">
            <sle:SliderOptionsEditor ID="_selectOptionsEditor" runat="server" />
        </div>
        <div id="itemEditorTabs-3-tabContent" class="padding10">
            <cle:ColumnOptionsEditor ID="_columnOptionsEditor" runat="server" />
        </div>
        <div id="itemEditorTabs-4-tabContent" class="padding10">
            <div>
                <asp:PlaceHolder ID="_appearanceEditorPlace" runat="server" />
            </div>            
        </div>
    </div>
