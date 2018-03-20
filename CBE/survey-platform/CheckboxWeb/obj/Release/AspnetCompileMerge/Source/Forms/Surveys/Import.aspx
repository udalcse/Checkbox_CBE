<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="Import.aspx.cs" Inherits="CheckboxWeb.Forms.Surveys.Import" MasterPageFile="~/Dialog.Master" %>
<%@ Import Namespace="Checkbox.Web" %>


<%@ MasterType VirtualPath="~/Dialog.Master" %>
<%@ Register Src="../../Controls/Wizard/WizardNavigator.ascx" TagName="WizardNavigator" TagPrefix="nav" %>
<%@ Register Src="../../Controls/Wizard/WizardButtons.ascx" TagName="WizardButtons" TagPrefix="btn" %>
<%@ Register Src="~/Forms/Surveys/Controls/SurveyUploader.ascx" TagName="SurveyUploader" TagPrefix="ckbx" %>

<asp:Content ID="_script" ContentPlaceHolderID="_headContent" runat="server">
    <script type="text/javascript">
        $(function () {
            $(".SelectedBody").prev().children("a").addClass("WizardLinkSelectedTail");
            $(".SelectedBody").prev().children("span").addClass("WizardLinkSelectedTail");
        });

        //Show message on error
        function onProgressError(errorMessage) {
            $('#progressDiv').hide();
            $('#progressText').html('<div class="ErrorMessage">An error occurred: <span style="color:black;font-weight:normal;">&nbsp;&nbsp;' + errorMessage + '</span></div>');
            $('#<%= _closeBtn.ClientID %>').fadeIn(300);
        }

        //Update status
        function onProgressUpdate(progressData) {
            $('#progressDiv').show();
            $('#progressText').html(progressData.StatusMessage);
        }

        //Do something on progress complete
        function onProgressComplete(progressData) {
            $('#progressText').empty(); //.html('');
            $('#completedDiv').show();
            $('#resultsDiv').html(progressData.StatusMessage);
            $('#importingDiv').hide();
            $('#<%= _closeBtn.ClientID %>').fadeIn(300);
            __doPostBack('ctl00$_pageContent$_wizard$_closeBtn$ctl00', '');
        }
    </script>
</asp:Content>

<asp:Content ID="_page" ContentPlaceHolderID="_pageContent" runat="server">
    <asp:HiddenField ID="_newTemplateId" runat="server" />

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
            <asp:WizardStep ID="_stepStart" runat="server" Title="Upload File" StepType="Step" AllowReturn="true">
                <div class="padding10">
                    <ckbx:SurveyUploader ID="_uploader" runat="server" />
                </div>
            </asp:WizardStep>

            <asp:WizardStep ID="_options" runat="server" Title="Options" StepType="Finish" AllowReturn="false">
                <div class="dialogSubContainer">
                    <div class="formInput condensed">
                        <p class="left fixed_150">
                            <ckbx:MultiLanguageLabel ID="MultiLanguageLabel1" runat="server" TextId="/pageText/forms/surveys/import.aspx/newName" AssociatedControlID="_nameTxt" />
                        </p>
                        <div class="left" style="margin-right: 10px;">
                            <asp:TextBox ID="_nameTxt" runat="server" Width="300" />
                        </div>
                    </div>

                    <div class="clear"></div>
                    
                    <div class="dialogSubTitle">Style Templates</div> 
                    
                    <div class="dialogSubContainer">
                        <p class="left fixed_150">
                            <ckbx:MultiLanguageLabel runat="server" TextId="/pageText/forms/surveystyle.aspx/stylePC" AssociatedControlID="_styleListPC" />
                        </p>
                        <div class="left" style="margin-right: 10px;">
                            <asp:DropDownList ID="_styleListPC" runat="server" DataTextField="Value" DataValueField="Key" Width="150px"/>
                        </div>
                        <div class="clear"></div>
                        <p class="left fixed_150">
                            <ckbx:MultiLanguageLabel runat="server" TextId ="/pageText/forms/surveystyle.aspx/mobileStyle" AssociatedControlID="_styleListMobile" />
                        </p>
                        <div class="left" style="margin-right:10px;">
                            <asp:DropDownList runat="server" ID="_styleListMobile" DataTextField="Name" DataValueField="StyleId" Width="150px"/>
                        </div>
                    </div>

                    <div class="clear"></div>
                    
                     
                    <div class="dialogSubTitle">Other Options</div>

                     <div class="dialogSubContainer">
                        <p class="left fixed_150">
                            <ckbx:MultiLanguageLabel ID="_folderLbl" runat="server" TextId="/pageText/forms/surveys/import.aspx/folder" AssociatedControlID="_folderList" />
                        </p>
                        <div class="left" style="margin-right: 10px;">
                            <asp:DropDownList ID="_folderList" runat="server" Width="150px" />
                        </div>
                    </div>
                </div>
            </asp:WizardStep>

            <asp:WizardStep ID="_completion" runat="server" Title="Complete" StepType="Complete">
                <asp:Panel ID="_completionContainer" runat="server" CssClass="padding10">
                    <div class="dialogSubContainer">
                        <div class="dashStatsWrapper border999 shadow999">
                            <div id="importingDiv">
                                <div class="dashStatsHeader" style="padding: 5px;">
                                    <span class="mainStats"><%=WebTextManager.GetText("/pageText/forms/surveys/import.aspx/importingSurvey")%></span>
                                </div>
                                <div class="padding10">
                                    <ckbx:MultiLanguageLabel ID="_importingLbl" runat="server" CssClass="label" TextId="/pageText/forms/surveys/import.aspx/importingInProgress" />
                                    <br class="clear" />
                                    <div id="progressDiv"></div>
                                    <div id="progressText"></div>
                                </div>
                            </div>
                            <div id="completedDiv" style="display: none;">
                                <div class="dashStatsHeader"><span class="mainStats left"><%=WebTextManager.GetText("/pageText/forms/surveys/import.aspx/completedHeader")%></span></div>
                                <div id="resultsDiv" style="margin: 5px;">
                                </div>
                                <div class="dialogInstructions">
                                    <ckbx:MultiLanguageLabel ID="_completionInstructions" runat="server" CssClass="" TextId="/pageText/forms/surveys/import.aspx/completionInstructions" />
                                </div>
                            </div>
                        </div>
                    </div>
                </asp:Panel>
                <div class="WizardNavContainer">
                    <hr />
                    <btn:CheckboxButton runat="server" Style="display: none;" ID="_closeBtn" TextId="/common/close" CausesValidation="false" CssClass="cancelButton left" />
                    <br class="clear" />
                </div>
            </asp:WizardStep>
        </WizardSteps>
    </asp:Wizard>
</asp:Content>