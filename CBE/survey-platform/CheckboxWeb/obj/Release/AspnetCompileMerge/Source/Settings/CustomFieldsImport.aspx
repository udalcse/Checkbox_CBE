<%@ Page Title="" Language="C#" MasterPageFile="~/Dialog.Master" CodeBehind="CustomFieldsImport.aspx.cs" Inherits="CheckboxWeb.Settings.CustomFieldsImport" %>
<%@ Register TagPrefix="ckbx" TagName="Uploader" Src="~/Controls/Uploader.ascx" %>
<%@ Register TagPrefix="nav" TagName="WizardNavigator" Src="~/Controls/Wizard/WizardNavigator.ascx" %>
<%@ Register TagPrefix="btn" TagName="WizardButtons_1" Src="~/Controls/Wizard/WizardButtons.ascx" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>
<%@ Register src="~/Controls/Status/StatusControl.ascx" tagname="StatusControl" tagprefix="status" %>

<asp:Content ID="settingContent" ContentPlaceHolderID="_pageContent" runat="server">
    
    <script type="text/javascript" language="javascript">
        $(function() {
            $("[id$='_finishNavigationButtons__nextButton']")
                .click(function() {
                    closeWindowAndRefreshParentPage();
                    return false;
                });

            toggleNextButton();
        });

        //Handle file selection.
        function itemFileUploaded(fileData) {
            if (fileData != null) {
                $('#<%=_uploadedFileNameTxt.ClientID %>').val(fileData.name);
                $('#<%=_uploadedFilePathTxt.ClientID %>').val(fileData.TempName);
                $("[id$='_importProperties']").css("display", "block");

                __doPostBack("customFieldsImport", '');
                toggleNextButton();
            }
        }
        __doPostBack = function (eventTarget, eventArgument) {
            var theForm = document.getElementById('aspnetForm');
            if (!theForm) {
                theForm = document.aspnetForm;
            }

            if (!theForm.onsubmit || (theForm.onsubmit() != false)) {
                theForm.__EVENTTARGET.value = eventTarget;
                theForm.__EVENTARGUMENT.value = eventArgument;
                theForm.submit();
            }
        };

        function toggleNextButton() {
            var button = $("a[id$='__nextButton']:contains('Next')");

            if ($('#<%=_uploadedFileNameTxt.ClientID %>').val() && !($('#<%=_errorFileUpload.ClientID %>').val())) {
                $(button).css("pointer-events", "auto").removeClass("disabled");
            } else {
                $(button).css("pointer-events", "none").addClass("disabled");
            }
        }

    </script>
    
    <asp:Wizard ID="_wizard" runat="server" DisplaySideBar="false" Height="500" DisplayCancelButton="true" 
        OnNextButtonClick="_wizard_NextButtonClick"
        OnPreviousButtonClick="_wizard_PreviousButtonClick"> 
        <HeaderTemplate>
            <nav:WizardNavigator ID="_wizardNavigator" runat="server" AllowForwardNavigation="false" />
        </HeaderTemplate>
        <StartNavigationTemplate>
            <btn:WizardButtons_1 ID="_startNavigationButtons" runat="server" />
        </StartNavigationTemplate>
        <FinishNavigationTemplate>
            <btn:WizardButtons_1 ID="_finishNavigationButtons" runat="server" />
        </FinishNavigationTemplate>
        <StepNavigationTemplate>
            <btn:WizardButtons_1 ID="_stepNavigationButtons" runat="server" />
        </StepNavigationTemplate>
        <WizardSteps> 
            <asp:WizardStep ID="_stepStart" runat="server" title="Select File to Upload" StepType="Step" AllowReturn="true">
                <div style="margin:0 10px 0 10px">
                    <h3 class="dialogInstructions" runat="server" style="padding-top: 10px;padding-bottom:30px"><%= WebTextManager.GetText("/pageText/settings/customFieldsImport.aspx/title")%></h3>
                    <div class="warning message" runat="server" id="StatusMessage" visible="false"></div>
        
                    <asp:Label CssClass="dialogInstructions" runat="server">
                        <%=WebTextManager.GetText("/controlText/itemImport.ascx/importTitle") %>
                    </asp:Label>
                    <div>&nbsp;</div>
        
                    <div style="">
                        <asp:TextBox ID="_uploadedFileNameTxt" runat="server" BorderStyle="None" Width="100%"/>
                        <asp:TextBox style="display: none" ID="_uploadedFilePathTxt" runat="server" BorderStyle="None" Width="100%"/>
                        <asp:HiddenField id="_parsedNameTxt" runat="server" />
                        <asp:HiddenField ID="_errorFileUpload" runat="server" />
                    </div>

                    <ckbx:Uploader ID="_uploader" runat="server" UploadedCallback="itemFileUploaded" SelectFilePromptTextId="/controlText/fileUploader/selectFile" />
                 </div>
            </asp:WizardStep>
                      
            <asp:WizardStep ID="_completedStep" runat="server" Title="Upload Profile Fields" StepType="Finish" AllowReturn="True">
                <div class="padding10">
                    <asp:Panel ID="_errorPanel" runat="server" CssClass="MasterErrorPanel">
                        <div class="left">
                            <asp:Image ID="_warningImage" SkinID="Warning" runat="server" />
                        </div>
                        <div class="left" style="margin-left:15px;">
                            <div style="font-weight:bold;"><asp:Literal ID="_errorMessageLiteral" runat="server" /></div>
                            <div style="color:#333333;"><asp:Literal ID="_errorDescriptionLiteral" runat="server" /></div>
                        </div>
                        <br class="clear" />
                    </asp:Panel>
                    
                    <asp:Panel ID="_sucessPanel" runat="server" CssClass="MasterSuccessPanel">
                        <div class="center" style="margin-left:15px;">
                            <div style="font-weight:bold;"><asp:Literal ID="_successMessageLiteral" runat="server" /></div>
                            <div style="color:#333333;"><asp:Literal ID="_successDescriptionLiteral" runat="server" /></div>
                        </div>
                        <span class="clear"></span>
                    </asp:Panel>

                </div>
            </asp:WizardStep>
        </WizardSteps>
    </asp:Wizard>
</asp:Content>

