<%@ Page Language="C#" CodeBehind="Launch.aspx.cs" MasterPageFile="~/Dialog.Master"  Inherits="CheckboxWeb.Forms.Surveys.Launch"%>
<%@ MasterType VirtualPath="~/Dialog.Master" %>
<%@ Register src="../../Controls/Wizard/WizardNavigator.ascx" tagname="WizardNavigator" tagprefix="nav" %>
<%@ Register src="../../Controls/Wizard/WizardButtons.ascx" tagname="WizardButtons" tagprefix="btn" %>
<%@ Register Src="Controls/SecuritySelector.ascx" TagName="SecuritySelector" TagPrefix="ckbx" %>
<%@ Register Src="Controls/Behavior.ascx" TagName="Behavior" TagPrefix="ckbx" %>
<%@ Register Src="Controls/ResponseLimits.ascx" TagName="ResponseLimits" TagPrefix="ckbx" %>
<%@ Register Src="Controls/TimeLimits.ascx" TagName="TimeLimits" TagPrefix="ckbx" %>
<%@ Register Src="Controls/SettingsSummary.ascx" TagName="SettingsSummary" TagPrefix="ckbx" %>
<%@ Register Src="Controls/Share.ascx" TagName="Share" TagPrefix="ckbx" %>

<asp:Content ID="_headContent" ContentPlaceHolderID="_headContent" runat="server">
    <ckbx:ResolvingScriptElement runat="server" Source="~/Resources/WizardHighlight.js" />
</asp:Content>

<asp:Content ID="_pageContent" runat="server" ContentPlaceHolderID="_pageContent">
    <asp:Wizard ID="_wizard" runat="server" DisplaySideBar="false" DisplayCancelButton="true"> 
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
            <asp:WizardStep ID="_stepPermissions" runat="server" Title="Permissions" StepType="Step" AllowReturn="true">
                <div class="padding10">
                    <ckbx:SecuritySelector ID="_securitySelector" runat="server" />
                </div>
            </asp:WizardStep>
                
            <asp:WizardStep ID="_stepOptions" runat="server" Title="Options" StepType="Step" AllowReturn="true">
                <div class="padding10">
                    <ckbx:Behavior runat="server" ID="_behaviorOptions" />
                </div>
            </asp:WizardStep>
                
            <asp:WizardStep ID="_stepLimits" runat="server" Title="Limits" StepType="Step" AllowReturn="true">
                <div class="padding10">
                    <ckbx:ResponseLimits ID="_responseLimits" runat="server" />
                    <ckbx:TimeLimits ID="_timeLimits" runat="server" />
                </div>
            </asp:WizardStep>
                
            <asp:WizardStep ID="_stepConfirm" runat="server" title="Confirm" StepType="Finish" AllowReturn="true">
                <div class="padding10">
                    <ckbx:SettingsSummary ID="_summary" runat="server" />
                </div>              
            </asp:WizardStep>
                
            <asp:WizardStep ID="_stepComplete" runat="server" title="Share" StepType="Complete">
                <div class="padding10">
                    <ckbx:Share ID="_share" runat="server" />
                </div>
                <div class="padding10 left" id="_container"  runat="server">
                </div>                
                <div class="WizardNavContainer">
                    <hr />
                    <btn:CheckboxButton ID="_closeWizardButton" runat="server" CssClass="cancelButton left" TextID="/pageText/forms/surveys/launch.aspx/exitButton" />
                </div>
            </asp:WizardStep>
        </WizardSteps>
    </asp:Wizard>
</asp:Content>