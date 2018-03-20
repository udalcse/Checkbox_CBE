<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="AddMatrixColumn.aspx.cs"  Inherits="CheckboxWeb.Forms.Surveys.AddMatrixColumn" MasterPageFile="~/Dialog.Master" EnableEventValidation="false" ValidateRequest="false"  %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>
<%@ Register Src="../../Controls/Wizard/WizardNavigator.ascx" TagName="WizardNavigator"  TagPrefix="nav" %>
<%@ Register Src="../../Controls/Wizard/WizardButtons.ascx" TagName="WizardButtons"  TagPrefix="btn" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemList.ascx" TagName="ItemList" TagPrefix="ckbx" %>
<%@ Register Src="~/Forms/Surveys/Controls/SurveyItemEditor.ascx" TagPrefix="ckbx"  TagName="ItemEditor" %>

<asp:Content ContentPlaceHolderID="_headContent" ID="_headContent" runat="server">
    
    <script type="text/javascript">

        //Show message about empty item before the continuation of the wizard
        function ShowConfirmMessage(message) {
            showConfirmDialogWithCallback(message, confirmationApproved);
        }

        //Confirmation approved handler
        function confirmationApproved() {
            $("#<%= _isConfirmed.ClientID %>").val('true');

            eval($('#<%= NavigationButtons.NextButton.ClientID %>').attr('href'));            
        }

        //
        function closeAddMatrixColumnDialog() {
            var inDialogWindow = (window.self != window.top);

            var args = { op: 'addMatrixColumn', result: 'ok', itemId: <%=MatrixItemId %> };

            if (inDialogWindow) {
                closeWindow(window.top.templateEditor.onDialogClosed, args);
            }
            else if (parent && parent.templateEditor) {
                parent.templateEditor.onDialogClosed(args);
            }
        }
    </script>

    <ckbx:ResolvingScriptElement runat="server" Source="~/Resources/WizardHighlight.js" />
</asp:Content>

<asp:Content ContentPlaceHolderID="_pageContent" ID="_pageContent" runat="server">
    <div style="display:none;">
        <asp:TextBox ID="_isConfirmed" runat="server" Text="false" />
    </div>

    <asp:Wizard ID="_wizard" runat="server" DisplaySideBar="false" Height="615" DisplayCancelButton="true">
        <HeaderTemplate>
            <nav:WizardNavigator ID="_wizardNavigator" runat="server" AllowForwardNavigation="false" />
        </HeaderTemplate>
        <StartNavigationTemplate>
            <btn:WizardButtons ID="_startNavigationButtons" runat="server" CloseDialogOnCancel="false" />
        </StartNavigationTemplate>
        <FinishNavigationTemplate>
            <btn:WizardButtons ID="_finishNavigationButtons" runat="server" CloseDialogOnCancel="false" />
        </FinishNavigationTemplate>
        <StepNavigationTemplate>
            <btn:WizardButtons ID="_stepNavigationButtons" runat="server" CloseDialogOnCancel="false" />
        </StepNavigationTemplate>
        <WizardSteps>
            <asp:WizardStep ID="_typeStep" runat="server" Title="Select Column Type" StepType="Step" AllowReturn="true">
                <asp:Panel ID="_errorPanel" runat="server" CssClass="error message" Visible="false">
                    <asp:Label ID="_errorMsgLbl" runat="server" />
                </asp:Panel>
                <ckbx:ItemList ID="_itemList" runat="server" TypesToInclude="SingleLineText,RadioButtons,DropdownList,Checkboxes,Slider,MatrixSumTotal,RadioButtonScale" />
            </asp:WizardStep>
            <asp:WizardStep ID="_propertiesStep" runat="server" Title="Properties" StepType="Finish"  AllowReturn="true">
                <div style="height: 495px; overflow: auto">
                    <ckbx:ItemEditor ID="_itemEditor" runat="server" />
                </div>
                <div style="clear: both;">
                </div>
            </asp:WizardStep>
        </WizardSteps>
    </asp:Wizard>
</asp:Content>
