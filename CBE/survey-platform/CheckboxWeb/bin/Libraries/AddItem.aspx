<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="AddItem.aspx.cs" Inherits="CheckboxWeb.Libraries.AddItem" MasterPageFile="~/Dialog.Master" %>
<%@ Import Namespace="Checkbox.Common" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemList.ascx" TagName="ItemList" TagPrefix="ckbx" %>
<%@ Register Src="~/Controls/Wizard/WizardNavigator.ascx" TagName="WizardNavigator" TagPrefix="nav" %>
<%@ Register Src="~/Controls/Wizard/WizardButtons.ascx" TagName="WizardButtons" TagPrefix="btn" %>
<%@ Register Src="~/Controls/AddItems/ItemListFromLibrary.ascx" TagName="ItemListFromLibrary" TagPrefix="ckbx" %>
<%@ Register Src="~/Libraries/Controls/ItemListFromSurvey.ascx" TagName="ItemListFromSurvey" TagPrefix="ckbx" %>
<%@ Register Src="~/Controls/AddItems/ItemImport.ascx" TagName="ItemImport" TagPrefix="ckbx" %>

<asp:Content ID="_head" runat="server" ContentPlaceHolderID="_headContent">
    <ckbx:ResolvingScriptElement runat="server" Source="~/Resources/WizardHighlight.js" />
    <ckbx:ResolvingScriptElement runat="server" Source="~/Resources/jquery.ckbxTab.js" />
        
    <script type="text/javascript" language="javascript">
        $(document).ready(function () {
            $('#itemSourceTabs').ckbxTabs({
                tabName: 'itemSourceTabs',
                initialTabIndex: <%= Utilities.AdvancedHtmlEncode(_currentTabTxt.Text) %>,
                onTabsLoaded: function(){$('.tabContainer').show();$('.tabContentContainer').show();},
                onTabClick: function(index){$('#<%=_currentTabTxt.ClientID %>').val(index)}
            });
        });
    </script>
</asp:Content>

<asp:Content ID="_page" ContentPlaceHolderID="_pageContent" runat="server">
    <div style="display:none;">
        <asp:TextBox ID="_currentTabTxt" runat="server" Text="0" />
    </div>

    <asp:Wizard ID="_wizard" runat="server" DisplaySideBar="false" Height="500" DisplayCancelButton = "true"> 
        <HeaderTemplate>
          <nav:WizardNavigator ID="_wizardNavigator" runat="server" AllowForwardNavigation="false" />
        </HeaderTemplate>
        <StartNavigationTemplate>
            <btn:WizardButtons ID="_startNavigationButtons" runat="server" />
        </StartNavigationTemplate>
        <FinishNavigationTemplate>
            <btn:WizardButtons ID="_finishNavigationButtons" runat="server" />
        </FinishNavigationTemplate>
        <StepNavigationTemplate>
            <btn:WizardButtons ID="_stepNavigationButtons" runat="server" />
        </StepNavigationTemplate>
        <WizardSteps> 
            <asp:WizardStep ID="_stepStart" runat="server" title="Select Item" StepType="Finish" AllowReturn="false">
                <ul id="itemSourceTabs" class="tabContainer">
                    <li>Add New Item</li>
                    <li>Copy From Library</li>
                    <li>Copy From Survey</li>
                    <li>Import From XML</li>
                </ul>
                <div class="tabContentContainer padding10">
                    <div id="itemSourceTabs-0-tabContent">
                        <ckbx:ItemList ID="_itemList" runat="server" IncludeLibraryCompatible="true" />
                    </div>
                    <div id="itemSourceTabs-1-tabContent" style="display:none;">
                        <ckbx:ItemListFromLibrary ID="_itemListFromLibrary" runat="server" ItemCountPerPage="7" />
                    </div>
                    <div id="itemSourceTabs-2-tabContent" style="display:none;">
                        <ckbx:ItemListFromSurvey ID="_itemListFromSurvey" runat="server" ItemCountPerPage="8" SurveyCountPerPage="8" />
                    </div>
                    <div id="itemSourceTabs-3-tabContent" style="display:none;">
                        <ckbx:ItemImport ID="_itemImport" runat="server" />
                    </div>
                </div>
            </asp:WizardStep>
            <asp:WizardStep ID="_completedStep" runat="server" Title="Complete" StepType="Complete">
                <div class="padding10">
                    <div class="dialogInstructions">
                        <asp:Label ID="_addedItemSummaryLabel" runat="server" />
                    </div>
                    <div class="dialogFormPush"></div>
                </div>
                <div class="DialogButtonsContainer">
                    <hr />
                    <btn:CheckboxButton ID="_closeWizardButton" runat="server" CssClass="ckbxButton roundedCorners border999 shadow999 redButton left" TextID="/pageText/libraries/addItem.aspx/exitButton" OnClientClick="closeWindow('onDialogClosed', {op: 'addItem',result:'ok'});return false;" />
                    <btn:CheckboxButton ID="_restartButton" runat="server" CssClass="ckbxButton roundedCorners border999 shadow999 orangeButton right" TextID="/pageText/libraries/addItem.aspx/restartButton" />
                    <btn:CheckboxButton ID="_editItemButton" runat="server" CssClass="ckbxButton roundedCorners border999 shadow999 orangeButton right" TextID="/pageText/libraries/addItem.aspx/editItemButton" />
                </div>
            </asp:WizardStep>
        </WizardSteps>
    </asp:Wizard>
</asp:Content>