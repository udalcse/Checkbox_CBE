<%@ Page Title="" Language="C#" MasterPageFile="~/Dialog.Master" AutoEventWireup="false" CodeBehind="Add.aspx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.Add" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>
<%@ Register src="../../../Controls/Wizard/WizardNavigator.ascx" tagname="WizardNavigator" tagprefix="nav" %>
<%@ Register src="../../../Controls/Wizard/WizardButtons.ascx" tagname="WizardButtons" tagprefix="btn" %>
<%@ Register src="Controls/ReportProperties.ascx" tagname="Properties" tagprefix="report" %>
<%@ Register src="Controls/ReportItems.ascx" tagname="Items" tagprefix="report" %>
<%@ Register src="Controls/ReportWizardOptions.ascx" tagname="Options" tagprefix="report" %>

<asp:Content ID="Content1" ContentPlaceHolderID="_headContent" runat="server">
    <ckbx:ResolvingScriptElement runat="server" Source="../../../Resources/DialogHandler.js" />
    <ckbx:ResolvingScriptElement runat="server" Source="../../../Resources/WizardHighlight.js" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="_pageContent" runat="server">
    <script language="javascript" type="text/javascript">
        function runReport() {
            window.open('<%=ResolveUrl("~/RunAnalysis.aspx?aid=" + ReportID)%>');
        };

        $(document).ready(function () {            
                $('#<%=_runButton.ClientID%>').attr("href", "#");
                }
        );

    </script>
    
    
    <style>
        #aspnetForm {
            overflow: hidden;
        }
    </style>

        <asp:Wizard ID="_reportWizard" runat="server" 
            DisplaySideBar="false" 
            DisplayCancelButton="true" 
            OnNextButtonClick="AnalysisWizard_NextButtonClick" 
            OnFinishButtonClick="AnalysisWizard_FinishButtonClick" 
            OnCancelButtonClick="AnalysisWizard_CancelButtonClick" >
            <WizardSteps>
                <asp:WizardStep ID="StartStep" runat="server">
                    <div class="padding10">
                        <div class="dialogSubTitle">
                            <ckbx:MultiLanguageLabel ID="_propertiesTitle" runat="server" CssClass="panelTitle" TextId="/pageText/forms/surveys/reports/add.aspx/propertiesTitle" />
                        </div>
                        <div class="dialogSubContainer">
                            <ckbx:MultiLanguageLabel ID="_propertiesInstructions" runat="server"  TextId="/pageText/forms/surveys/reports/add.aspx/propertiesInstructions"/><br /><br />
                        </div>
                        <report:Properties id="_properties" runat="server"></report:Properties>
                    </div>
                </asp:WizardStep>
                <asp:WizardStep ID="QuestionsStep" runat="server">
                    <div class="padding10">
                        <div class="dialogSubTitle extended">
                            <ckbx:MultiLanguageLabel ID="_itemsTitle" runat="server" CssClass="panelTitle" TextId="/pageText/forms/surveys/reports/add.aspx/itemsTitle" />
                        </div>
                        <div class="dialogSubContainer">
                            <asp:Panel ID="_reportWizardPanel" runat="server">
                                <ckbx:MultiLanguageLabel runat="server" TextId="/pageText/forms/surveys/reports/add.aspx/itemsInstructions"/><br /><br />
                                <report:Items id="_items" runat="server" />
                            </asp:Panel>
                            <asp:Panel ID="_blankWizardPanel" runat="server">
                                <ckbx:MultiLanguageLabel runat="server" TextId="/pageText/forms/surveys/reports/add.aspx/noItemsInstructions" /><br />
                            </asp:Panel>
                        </div>
                    </div>
                </asp:WizardStep>
                <asp:WizardStep ID="OptionsStep" runat="server">
                    <div class="padding10">
                        <div class="dialogSubTitle extended">
                            <ckbx:MultiLanguageLabel ID="MultiLanguageLabel1" runat="server" CssClass="panelTitle" TextId="/pageText/forms/surveys/reports/add.aspx/optionsTitle" />
                        </div>
                        <div class="dialogSubContainer">
                            <asp:Panel ID="_optionsPanel" runat="server">
                                <ckbx:MultiLanguageLabel runat="server" TextId="/pageText/forms/surveys/reports/add.aspx/optionsInstructions"/><br /><br />
                                <report:Options id="_options" runat="server"/>
                            </asp:Panel>
                            <asp:Panel ID="_blankOptionsPanel" runat="server">
                                <ckbx:MultiLanguageLabel ID="MultiLanguageLabel2" runat="server" TextId="/pageText/forms/surveys/reports/add.aspx/noOptionsInstructions"/><br /><br />
                            </asp:Panel>
                        </div>
                    </div>
                </asp:WizardStep>
                <asp:WizardStep ID="ReviewStep" runat="server" StepType="Finish">
                    <div class="padding10">
                        <div class="dialogSubTitle extended">
                            <ckbx:MultiLanguageLabel ID="_reviewTitle" runat="server" CssClass="panelTitle" TextId="/pageText/forms/surveys/reports/add.aspx/reviewTitle" /><br />
                        </div>
                        <div class="dialogSubContainer overflowPanel_400">
                            <div>
                                <ckbx:MultiLanguageLabel ID="_reviewInstructions" runat="server" TextId="/pageText/forms/surveys/reports/add.aspx/reviewInstructions"/>
                            </div>

                            <div style="float:left; width:400px">
                                <fieldset>
                                    <legend><ckbx:MultiLanguageLiteral id="_userInfoReviewTitle" runat="server" TextId="/pageText/forms/surveys/reports/add.aspx/reportPropertiesReviewTitle" /></legend>

                                    <div>
                                        <ckbx:MultiLanguageLabel runat="server" CssClass="label" TextId="/pageText/reportProperties.ascx/analysisName" />
                                        <div style="margin-left:5px;margin-bottom:5px;"><asp:Label ID="_reportNameReview" runat="server" /></div>
                                    </div>

                                    <div>
                                        <ckbx:MultiLanguageLabel ID="MultiLanguageLabel3" CssClass="label"  runat="server" TextId="/pageText/reportProperties.ascx/reportType" />
                                        <div style="margin-left:5px;margin-bottom:5px;">
                                            <asp:Label ID="_reportTypeReview" runat="server" />
                                        </div>
                                    </div>

                                    <div>
                                        <ckbx:MultiLanguageLabel runat="server" CssClass="label" TextId="/pageText/reportProperties.ascx/styleTemplate" />
                                        <div style="margin-left:5px;">
                                            <asp:Label ID="_styleTemplateReview" runat="server" />
                                        </div>
                                    </div>
                                   
                                </fieldset>                                    
            

                                <fieldset class="nameValueList">
                                    <legend><ckbx:MultiLanguageLiteral id="_groupsReviewTitle" runat="server" TextId="/pageText/forms/surveys/reports/add.aspx/reportOptionsTitle" /></legend>

                                    <asp:PlaceHolder ID="_optionsReviewPanel" runat="server">
                                        <div>
                                            <ckbx:MultiLanguageLabel runat="server" CssClass="left fixed_250" TextId="/pageText/AutogenerateReport.aspx/useAliasLbl" />
                                            <asp:Label ID="_useAliaseReview" runat="server" />
                                            <br class="clear"/>
                                        </div>

                                        <div>
                                            <ckbx:MultiLanguageLabel runat="server" CssClass="left fixed_250" TextId="/pageText/AutogenerateReport.aspx/multiPageLbl" />
                                            <asp:Label ID="_multiPageReview" runat="server" />
                                            <br class="clear"/>
                                        </div>

                                        <div>
                                            <ckbx:MultiLanguageLabel runat="server" CssClass="left fixed_250" TextId="/pageText/AutogenerateReport.aspx/itemPositionLbl" />
                                            <asp:Label ID="_itemPositionReview" runat="server" />
                                            <br class="clear"/>
                                        </div>

                                        <div>
                                            <ckbx:MultiLanguageLabel runat="server" CssClass="left fixed_250" TextId="/pageText/AutogenerateReport.aspx/maxOptionsLbl" />
                                            <asp:Label ID="_maxOptionsReview" runat="server" />
                                            <br class="clear"/>
                                        </div>
                                    </asp:PlaceHolder>
                                    <asp:PlaceHolder ID="_optionsReviewNAPanel" runat="server">
                                        <ckbx:MultiLanguageLabel runat="server" TextId="/pageText/surveys/add.aspx/na" />
                                    </asp:PlaceHolder>
                                </fieldset>
                            </div>

                            <div class="fullSpanReviewPanel" style="float:right; width:340px; margin-right:100px;">
                                <fieldset class="nameValueList">
                                    <legend><ckbx:MultiLanguageLiteral id="_rolesReviewTitle" runat="server" TextId="/pageText/forms/surveys/reports/add.aspx/reportQuestionsTitle" /></legend>
                                
                                <asp:PlaceHolder ID="_questionsReviewPanel" runat="server">
                                    <div>
                                        <ckbx:MultiLanguageLabel runat="server" CssClass="left fixed_150" TextId="/pageText/reportItems.ascx/radioButtons" />
                                        <asp:Label ID="_radioButtonsReview" runat="server" />
                                        <br class="clear"/>
                                    </div>

                                    <div>
                                        <ckbx:MultiLanguageLabel runat="server" CssClass="left fixed_150" TextId="/pageText/reportItems.ascx/checkboxes" />
                                        <asp:Label ID="_checkboxesReview" runat="server" />
                                        <br class="clear"/>
                                    </div>
    
                                      
                                        <div>
                                            <ckbx:MultiLanguageLabel runat="server" CssClass="left fixed_150" TextId="/pageText/reportItems.ascx/slt" />
                                            <asp:Label ID="_sltReview" runat="server" />
                                            <br class="clear"/>
                                        </div>

                                        <div>
                                            <ckbx:MultiLanguageLabel runat="server" CssClass="left fixed_150" TextId="/pageText/reportItems.ascx/mlt" />
                                            <asp:Label ID="_mltReview" runat="server" />
                                            <br class="clear"/>
                                        </div>
                                    

                                    <div>
                                        <ckbx:MultiLanguageLabel runat="server" CssClass="left fixed_150" TextId="/pageText/reportItems.ascx/netPromoterScore" />
                                        <asp:Label ID="_netPromoterScoreReview" runat="server" />
                                        <br class="clear"/>
                                    </div>

                                    <div>
                                        <ckbx:MultiLanguageLabel runat="server" CssClass="left fixed_150" TextId="/pageText/reportItems.ascx/slider" />
                                        <asp:Label ID="_sliderReview" runat="server" />
                                        <br class="clear"/>
                                    </div>

                                    <div>
                                        <ckbx:MultiLanguageLabel ID="MultiLanguageLabel4" runat="server" CssClass="left fixed_150" TextId="/pageText/reportItems.ascx/rankOrder" />
                                        <asp:Label ID="_rankOrderReview" runat="server" />
                                        <br class="clear"/>
                                    </div>

                                    <div>
                                        <ckbx:MultiLanguageLabel runat="server" CssClass="left fixed_150" TextId="/pageText/reportItems.ascx/ratingScale" />
                                        <asp:Label ID="_ratingScaleReview" runat="server" />
                                        <br class="clear"/>
                                    </div>

                                    <div>
                                        <ckbx:MultiLanguageLabel runat="server" CssClass="left fixed_150" TextId="/pageText/reportItems.ascx/dropDownList" />
                                        <asp:Label ID="_dropDownListReview" runat="server" />
                                        <br class="clear"/>
                                    </div>

                                    <div>
                                        <ckbx:MultiLanguageLabel runat="server" CssClass="left fixed_150" TextId="/pageText/reportItems.ascx/matrix" />
                                        <asp:Label ID="_matrixReview" runat="server" />
                                        <br class="clear"/>
                                    </div>

                                    <div>
                                        <ckbx:MultiLanguageLabel runat="server" CssClass="left fixed_150" TextId="/pageText/reportItems.ascx/hiddenItems" />
                                        <asp:Label ID="_hiddenItemsReview" runat="server" />
                                        <br class="clear"/>
                                    </div>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="_questionsReviewNAPanel" runat="server">
                                    <ckbx:MultiLanguageLabel runat="server" TextId="/pageText/surveys/add.aspx/na" />
                                </asp:PlaceHolder>
                                </fieldset>
                            </div>
                        
                            <div class="clear"></div>
                        </div>
                    </div>
                </asp:WizardStep>
                <asp:WizardStep ID="FinishStep" runat="server" StepType="Complete">
                    <div class="padding10">
                        <ckbx:MultiLanguageLabel ID="_completionTitle" runat="server" CssClass="panelTitle" TextId="/pageText/forms/surveys/reports/add.aspx/completionTitle" Text="Report created"/><br />
                        <ckbx:MultiLanguageLabel ID="_completionInstructions" runat="server" CssClass="" TextId="/pageText/forms/surveys/reports/add.aspx/completionInstructions" Text="You may create another report or return to the report manager"/><br />
                        <ckbx:MultiLanguageLabel ID="_createReportError" runat="server" CssClass="error message" Visible="false" /><br /><br />
                    </div>
                    <div class="WizardNavContainer" style="bottom:15px;">
                        <hr />
                       
                            <btn:CheckboxButton ID="_closeWizardButton" runat="server" CssClass="cancelButton left" OnClick="ExitButton_OnClick" TextID="/pageText/forms/surveys/reports/add.aspx/exitButton" />
                            <btn:CheckboxButton ID="_runButton" runat="server" CssClass="ckbxButton roundedCorners border999 shadow999 silverButton right" OnClientClick="runReport()" TextID="/pageText/forms/surveys/reports/add.aspx/runButton"/>
                            <btn:CheckboxButton ID="_editButton" runat="server" CssClass="ckbxButton roundedCorners border999 shadow999 silverButton right" OnClick="EditButton_OnClick" TextID="/pageText/forms/surveys/reports/add.aspx/editButton" />
                            <btn:CheckboxButton ID="_restartButton" runat="server" CssClass="ckbxButton roundedCorners border999 shadow999 silverButton right" OnClick="RestartButton_OnClick" TextID="/pageText/forms/surveys/reports/add.aspx/restartButton" />               
                    </div>
                    
                </asp:WizardStep>
             </WizardSteps>
            <StartNavigationTemplate>
                <btn:WizardButtons ID="_startNavigationButtons" runat="server" />
            </StartNavigationTemplate>
            <FinishNavigationTemplate>
                <btn:WizardButtons ID="_finishNavigationButtons" runat="server" />
            </FinishNavigationTemplate>
            <HeaderTemplate>
                <nav:WizardNavigator ID="_wizardNavigator" runat="server" AllowForwardNavigation="false" />
            </HeaderTemplate>
            <StepNavigationTemplate>
                <btn:WizardButtons ID="_stepNavigationButtons" runat="server" />
            </StepNavigationTemplate>
        </asp:Wizard>                
</asp:Content>
