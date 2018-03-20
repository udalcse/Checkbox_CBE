<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="AddItem.aspx.cs" Inherits="CheckboxWeb.Forms.Surveys.AddItem" MasterPageFile="~/Dialog.Master" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>
<%@ Register Src="Controls/ItemList.ascx" TagName="ItemList" TagPrefix="ckbx" %>
<%@ Register src="../../Controls/Wizard/WizardNavigator.ascx" tagname="WizardNavigator" tagprefix="nav" %>
<%@ Register src="../../Controls/Wizard/WizardButtons.ascx" tagname="WizardButtons" tagprefix="btn" %>
<%@ Register Src="~/Controls/AddItems/ItemListFromLibrary.ascx" TagName="ItemListFromLibrary" TagPrefix="ckbx" %>
<%@ Register Src="~/Controls/AddItems/ItemImport.ascx" TagName="ItemImport" TagPrefix="ckbx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="_headContent" runat="server">
    <ckbx:ResolvingScriptElement runat="server" Source="~/Resources/WizardHighlight.js" />
    <ckbx:ResolvingScriptElement runat="server" Source="~/Resources/jquery.ckbxTab.js" />
    
    <script type="text/javascript" language="javascript">
        $(document).ready(function () {
            $('#itemSourceTabs').ckbxTabs({
                tabName: 'itemSourceTabs',
                initialTabIndex: <%=_currentTabTxt.Text %>,
                onTabsLoaded: function(){$('.tabContainer').show();$('.tabContentContainer').show();},
                onTabClick: function(index){$('#<%=_currentTabTxt.ClientID %>').val(index)}
            });

            //prevent double click on Next            
            $('a[id$=_nextButton]').click(function(e){
                $('a[id$=_nextButton]').click(function(e){e.preventDefault();});
            });
            
            <% if (!AllowLibraries) {%>
            $('.libraryTab').detach();
            <% } %>
        });

        function closeAndOpenItemEditor() {
            closeWindowAndRedirectParentPage(null, null, 'Edit.aspx?s=<%=ResponseTemplateId%>&i=<%=ItemID%>');
		}
    </script>
</asp:Content>

<asp:Content ID="_page" ContentPlaceHolderID="_pageContent" runat="server">
    <div style="display:none;">
        <asp:TextBox ID="_currentTabTxt" runat="server" Text="0" />
    </div>
    
    <asp:Wizard ID="_wizard" runat="server" DisplaySideBar="false" Height="500" DisplayCancelButton="true" OnFinishButtonClick="_wizard_FinishButtonClick" OnCancelButtonClick="_wizard_CancelButtonClick" > 
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
            <asp:WizardStep ID="_stepStart" runat="server" title="Select Item" StepType="Finish" AllowReturn="true">
                <ul id="itemSourceTabs" class="tabContainer">
                    <li>Add New Item</li>
                    <li class="libraryTab">Copy From Library</li>
                    <li>Import From XML</li>
                    <div class="clear"></div>
                </ul>
                <div class="tabContentContainer padding10">
                    <div id="itemSourceTabs-0-tabContent">
                        <div id="addSurveyItemList">
                            <ckbx:ItemList ID="_itemList" runat="server" IncludeSurveyCompatible="true" />                    
                        </div>
                    </div>
                    <div id="itemSourceTabs-1-tabContent" class="libraryTab" style="display:none;"><ckbx:ItemListFromLibrary ID="_itemListFromLibrary" runat="server" ItemCountPerPage="7" /></div>
                    <div id="itemSourceTabs-2-tabContent" style="display:none;"><ckbx:ItemImport ID="_itemImport" runat="server" /></div>
                </div>
            </asp:WizardStep>
                      
            <asp:WizardStep ID="_completedStep" runat="server" Title="Edit Item" StepType="Complete">
                <div class="padding10">
                    <div class="dialogInstructions">
                        <asp:Label ID="_addedItemSummaryLabel" runat="server" />
                    </div>
                </div>
                <div class="DialogButtonsContainer">
                    <hr />
                    <btn:CheckboxButton ID="_closeWizardButton" runat="server" CssClass="cancelButton left" TextID="/pageText/forms/surveys/addItem.aspx/exitButton" OnClientClick="closeWindowAndRefreshParentPage();return false;" />
                    <btn:CheckboxButton ID="_restartButton" runat="server" CssClass="ckbxButton roundedCorners border999 shadow999 silverButton right" TextID="/pageText/forms/surveys/addItem.aspx/restartButton" />
                    <btn:CheckboxButton ID="_editItemButton" runat="server" CssClass="ckbxButton roundedCorners border999 shadow999 silverButton right" TextID="/pageText/forms/surveys/addItem.aspx/editItemButton" />
                </div>
            </asp:WizardStep>
        </WizardSteps>
    </asp:Wizard>
</asp:Content>