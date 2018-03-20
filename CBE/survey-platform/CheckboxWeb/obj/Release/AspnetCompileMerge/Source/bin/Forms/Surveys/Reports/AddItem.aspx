<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="AddItem.aspx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.AddItem" MasterPageFile="~/Dialog.Master"  %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemList.ascx" TagName="ItemList" TagPrefix="ckbx" %>
<%@ Register src="~/Controls/Wizard/WizardNavigator.ascx" tagname="WizardNavigator" tagprefix="nav" %>
<%@ Register src="~/Controls/Wizard/WizardButtons.ascx" tagname="WizardButtons" tagprefix="btn" %>

<asp:Content ID="_head" runat="server" ContentPlaceHolderID="_headContent">
    <ckbx:ResolvingScriptElement runat="server" Source="~/Resources/WizardHighlight.js" />
    
    <script type="text/javascript">
        $(function () {
            <% if( _wizard.ActiveStepIndex == 0) {%>
                $next = $('[id$="_nextButton"]').hide();
                $('#itemList .itemTypeList p').click(function () { $next.show(); });
            <% } %>
        });
    </script>
</asp:Content>

<asp:Content ID="_page" ContentPlaceHolderID="_pageContent" runat="server">
    <asp:Wizard ID="_wizard" runat="server" DisplaySideBar="false" Height="500" DisplayCancelButton="true"> 
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
            <asp:WizardStep ID="_stepStart" runat="server" title="Select Item" StepType="Step" AllowReturn="true">
                <div id="addReportItemList">
                    <ckbx:ItemList ID="_itemList" runat="server" IncludeReportCompatible="true" DefaultCategory="Report"/>                    
                </div>
            </asp:WizardStep>
            
            <asp:WizardStep ID="_locationStep" runat="server" Title="Location" StepType="Finish" AllowReturn="true">
                <div style="margin: 15px;">
                    <asp:Panel runat="server" ID="_maxResponseCountWarningPanel" CssClass="StatusPanel warning">
                        <span style="margin-bottom: 15px;"><%=WarningMessage%></span>
                    </asp:Panel>
                    <p>
                        <asp:Label ID="_itemTypeSelectedLbl" runat="server" />
                    </p>

                    <p>
                        <ckbx:MultiLanguageLabel ID="_positionLbl" runat="server" TextId="/pageText/forms/surveys/reports/addItem.aspx/newItemPosition" />
                        <br />
                        <asp:DropDownList ID="_itemPositionList" runat="server" />                    
                    </p>
                </div>
            </asp:WizardStep>
            
            <asp:WizardStep ID="_completedStep" runat="server" Title="Complete" StepType="Complete">
                <div style="margin-left:15px; margin-top:15px; height:200px;">
                    <p>
                        <asp:Label ID="_addedItemSummaryLabel" runat="server" />
                    </p>
                    <p>
                        <btn:CheckboxButton ID="_restartButton" runat="server" CssClass="ckbxButton roundedCorners border999 shadow999 orangeButton" TextID="/pageText/forms/surveys/reports/addItem.aspx/restartButton" />
                        <btn:CheckboxButton ID="_editItemButton" runat="server" CssClass="ckbxButton roundedCorners border999 shadow999 orangeButton" TextID="/pageText/forms/surveys/reports/addItem.aspx/editItemButton" />
                        <btn:CheckboxButton ID="_closeWizardButton" runat="server" CssClass="ckbxButton roundedCorners border999 shadow999 orangeButton" TextID="/pageText/forms/surveys/reports/addItem.aspx/exitButton" />
                    </p>
                </div>
            </asp:WizardStep>
        </WizardSteps>
    </asp:Wizard>
</asp:Content>
