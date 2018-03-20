<%@ Page Title="" Language="C#" MasterPageFile="~/Dialog.Master" AutoEventWireup="false" CodeBehind="Add.aspx.cs" Inherits="CheckboxWeb.Users.EmailLists.Add" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ MasterType VirtualPath="~/Dialog.Master"%>
<%@ Register src="../../Controls/Wizard/WizardNavigator.ascx" tagname="WizardNavigator" tagprefix="nav" %>
<%@ Register src="../../Controls/Wizard/WizardButtons.ascx" tagname="WizardButtons" tagprefix="btn" %>
<%@ Register TagPrefix="ckbx" TagName="Uploader" Src="~/Controls/Uploader.ascx" %>

<asp:Content ID="_head" runat="server" ContentPlaceHolderID="_headContent">
    <ckbx:ResolvingScriptElement runat="server" Source="~/Resources/WizardHighlight.js" />
    
    <script type="text/javascript" language="javascript">
        $(document).ready(
            function() {
                $('#<%=_emailListName.ClientID%>').focus();
            });

        //Handle file selection.
        function onEmailListUploaded(fileData) {
            if (fileData != null) {
                $('#<%=_uploadedFileNameTxt.ClientID %>').val(fileData.name);
                $('#<%=_uploadedFilePathTxt.ClientID %>').val(fileData.TempName);
            }
        }

        //Show message on error
        function onProgressError(errorMessage) {
            $('#progressDiv').hide();
            $('#progressText').html('<div class="ErrorMessage">An error occurred while updating progress: <span style="color:black;font-weight:normal;">&nbsp;&nbsp;' + errorMessage + '</span></div>');
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
        }

    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="_pageContent" runat="server">
    
    <div style="width:100%; height:100%; margin:0">
        <asp:Wizard ID="_addEmailListWizard" runat="server" DisplaySideBar="false" DisplayCancelButton="true" OnNextButtonClick="AddEmailListWizard_NextButtonClick" OnFinishButtonClick="AddEmailListWizard_FinishButtonClick" OnCancelButtonClick="AddEmailListWizard_CancelButtonClick">
            <WizardSteps>
                <asp:WizardStep ID="ListNameStep" runat="server">
                    <div class="padding10">
                        <div class="dialogSubTitle">
                            <ckbx:MultiLanguageLabel ID="_profileTitle" runat="server" TextId="/pageText/users/emailLists/add.aspx/nameTitle" Text="Email list name"/>
                        </div>           
                        <div class="dialogSubContainer">
                            <div class="dialogInstructions">
                                <ckbx:MultiLanguageLabel ID="_profileInstructions" runat="server" CssClass="" TextId="/pageText/users/emailLists/add.aspx/nameInstructions" Text="Enter a name and optional description for this email list"/>
                            </div>
                        
                            <div style="margin:10px 0px 10px 25px;">
                                <asp:ValidationSummary ID="_validationSummary" runat="server" ValidationGroup="emailListValidationGroup" />
                            </div>  
                                            
                            <div class="formInput">
                                <p><ckbx:MultiLanguageLabel ID="_emailListNameLabel" runat="server" AssociatedControlID="_emailListName" TextId="/pageText/users/emailLists/add.aspx/listName" /></p>
                                <asp:TextBox ID="_emailListName" runat="server" style="width:498px;" MaxLength="32" />
                                <asp:RequiredFieldValidator ID="_emailListNameRequired" runat="server" ControlToValidate="_emailListName" Display="Dynamic"  ValidationGroup="emailListValidationGroup">*</asp:RequiredFieldValidator>                        
                                <asp:RegularExpressionValidator ID="_listNameLength" runat="server" Display="Dynamic" ControlToValidate="_emailListName"  ValidationExpression="[\w\s]{1,255}" ValidationGroup="emailListValidationGroup">*</asp:RegularExpressionValidator>
                                <asp:CustomValidator ID="_emailListExistValidator" runat="server" ControlToValidate="_emailListName"  OnServerValidate="_emailListExistValidator_ServerValidate" ValidationGroup="emailListValidationGroup">*</asp:CustomValidator>
                                <br class="clear"/>
                            </div>  

                            <div class="formInput">
                                <p><ckbx:MultiLanguageLabel ID="_emailListDescriptionLabel" runat="server" AssociatedControlID="_emailListDescription" TextId="/pageText/users/emailLists/add.aspx/listDescription" /></p>
                                <asp:TextBox ID="_emailListDescription" runat="server" TextMode="MultiLine" Rows="7" Columns="60" />
                                <asp:RegularExpressionValidator ID="_descriptionLength" runat="server" Display="Dynamic" ControlToValidate="_emailListDescription" ValidationExpression="[\S\s]{0,510}" ValidationGroup="emailListValidationGroup">*</asp:RegularExpressionValidator>
                                <br class="clear"/>
                            </div>
                        </div>
                    </div>
                </asp:WizardStep>
                <asp:WizardStep ID="AddAddresesStep" runat="server">
                    <asp:Panel ID="Panel1" runat="server" CssClass="padding10">
                        <div class="dialogSubTitle">
                            <%=WebTextManager.GetText("/pageText/users/emailLists/addAddresses.aspx/userSource") %>
                        </div>
                        <div class="dialogSubContainer">
                            <div class="formInput radioList">
                                <ckbx:MultiLanguageRadioButtonList ID="_userSourceList" runat="server" RepeatDirection="Vertical" AutoPostBack="True">
                                    <asp:ListItem Text="Upload CSV/Text file" TextId="/pageText/users/emailLists/addAddresses.aspx/csvText" Value="Upload" Selected="True"/>
                                    <asp:ListItem Text="Copy/Paste text" TextId="/pageText/users/emailLists/addAddresses.aspx/textEntry" Value="Enter" />
                                </ckbx:MultiLanguageRadioButtonList>
                            </div>
                            <asp:Panel ID="_textPanel" runat="server">
                                <fieldset>
                                    <legend><%=WebTextManager.GetText("/pageText/users/emailLists/addAddresses.aspx/textEntryTitle") %></legend>
                                    <div>
                                        <%=WebTextManager.GetText("/pageText/users/emailLists/addAddresses.aspx/textEntryInstructions") %>
                                    </div>
                                    <asp:Label ID="_textEntryError" runat="server" CssClass="error message" Visible="false" style="margin-left: 0; margin-bottom: 5px;" />
                                    <asp:TextBox ID="_importTxt" runat="server" Rows="5" Columns="60" TextMode="MultiLine" />

                                    <div style="margin-top:10px;">
                                        <ckbx:MultiLanguageLabel ID="_textEntryEncodingLabel" runat="server" TextId="/pageText/users/emailLists/addAddresses.aspx/textEntryEncoding" Text="File encoding"/>
                                        <ckbx:MultiLanguageRadioButtonList ID="_textEntryEncoding" runat="server">
                                            <asp:ListItem Text="ASCII" TextId="/pageText/users/emailLists/addAddresses.aspx/ascii" Value="ASCII" Selected="True"/>
                                            <asp:ListItem Text="Unicode (UTF-8)" TextId="/pageText/users/emailLists/addAddresses.aspx/unicode" Value="UTF-8" />
                                        </ckbx:MultiLanguageRadioButtonList>
                                    </div>
                                </fieldset>
                            </asp:Panel>

                            <asp:Panel ID="_filePanel" runat="server">
                                <fieldset>
                                    <legend><%=WebTextManager.GetText("/pageText/users/emailLists/addAddresses.aspx/fileUploadTitle") %></legend>
                                    <div><%=WebTextManager.GetText("/pageText/users/emailLists/addAddresses.aspx/fileUploadInstructions") %></div>

                                    <asp:Label ID="_fileUploadError" runat="server" CssClass="error message" Visible="false" style="margin-left: 0; margin-bottom: 5px;" />

                                    <ckbx:Uploader ID="_uploader" runat="server" UploadedCallback="onEmailListUploaded" SelectFilePromptTextId="/controlText/fileUploader/selectFile" />

                                    <div style="display:none;">
                                        <asp:TextBox ID="_uploadedFileNameTxt" runat="server" />
                                        <asp:TextBox ID="_uploadedFilePathTxt" runat="server" />
                                    </div>

                                    <div style="margin-top:10px;">
                                        <ckbx:MultiLanguageLabel ID="_fileEncodingLabel" runat="server" TextId="/pageText/users/emailLists/addAddresses.aspx/fileUploadEncoding" Text="File encoding"/>
                                        <ckbx:MultiLanguageRadioButtonList ID="_uploadFileEncoding" runat="server">
                                            <asp:ListItem Text="ASCII" TextId="/pageText/users/emailLists/addAddresses.aspx/ascii" Value="ASCII" Selected="True"/>
                                            <asp:ListItem Text="Unicode (UTF-8)" TextId="/pageText/users/emailLists/addAddresses.aspx/unicode" Value="UTF-8" />
                                        </ckbx:MultiLanguageRadioButtonList>
                                    </div>
                                </fieldset>
                            </asp:Panel>
                        </div>
                    </asp:Panel>
            </asp:WizardStep>
                <asp:WizardStep ID="ReviewStep" runat="server" StepType="Finish">
                    <asp:Panel runat="server">
                    <div class="padding10">
                        <div class="dialogSubTitle" style="width:220px;">
                            <ckbx:MultiLanguageLabel ID="_reviewTitle" runat="server" CssClass="panelTitle" TextId="/pageText/users/emailLists/add.aspx/reviewTitle" Text="Ready to create email list"/><br />
                        </div>
                        <div class="dialogInstructions">
                            <ckbx:MultiLanguageLabel ID="_reviewInstructions" runat="server" CssClass="" TextId="/pageText/users/emailLists/add.aspx/reviewInstructions" Text="Please review the information below to ensure it is correct"/>
                        </div>
                        <div class="dashStatsWrapper shadow999 border999">
                            <div class="dashStatsContent zebra">
                                <ckbx:MultiLanguageLabel ID="_emailListNameReviewLabel" runat="server" TextId="/pageText/users/emailLists/add.aspx/listName" CssClass="left fixed_125" />
                                <strong><ckbx:MultiLanguageLabel ID="_emailListNameReview" runat="server" CssClass="left" /></strong>
                                <br class="clear"/>
                            </div>
                            <div class="dashStatsContent detailZebra">
                                <ckbx:MultiLanguageLabel ID="_emailListDescriptionReviewLabel" runat="server" TextId="/pageText/users/emailLists/add.aspx/listDescription" CssClass="left fixed_125" />
                                <strong><ckbx:MultiLanguageLabel ID="_emailListDescriptionReview" runat="server" CssClass="left" /></strong>
                                <ckbx:MultiLanguageLabel runat="server"></ckbx:MultiLanguageLabel>
                                <br class="clear"/>
                            </div>
                        </div>
                    
                    <div class="dialogInstructions">
                            <ckbx:MultiLanguageLabel ID="_reviewAddreses" runat="server" CssClass="" Text="Please review the addreses:"/>
                        </div>
                    <div class="dialogInstructions">
                        <asp:Label ID="_fileReviewSuccess" runat="server" />
                    </div>
                    <div class="dashStatsWrapper border999 shadow999" style="overflow-y:scroll;height:70px;">
                        <asp:ListView ID="_validUserList" runat="server" ItemPlaceholderID="outerPlaceHolder">
                            <LayoutTemplate>                                        
                                <asp:PlaceHolder ID="outerPlaceHolder" runat="server" />
                            </LayoutTemplate>
                            <ItemTemplate>
                                    <div class="dashStatsContent zebra"><%# Container.DataItem %></div>
                            </ItemTemplate>
                            <AlternatingItemTemplate>
                                    <div class="dashStatsContent detailZebra"><%# Container.DataItem %></div>
                            </AlternatingItemTemplate>
                        </asp:ListView>
                    </div>                                                
                    <asp:Label ID="_fileReviewError" runat="server" CssClass="error message" Visible="false" />
                    <asp:ListView ID="_invalidUserList" runat="server">
                        <LayoutTemplate>
                            <div class="dashStatsWrapper border999 shadow999" style="overflow-y:scroll; height:70px;">
                                <asp:PlaceHolder ID="itemPlaceholder" runat="server" />
                            </div>
                        </LayoutTemplate>
                        <ItemTemplate>
                            <div class="dashStatsContent zebra"><%# Container.DataItem %></div>
                        </ItemTemplate>
                        <AlternatingItemTemplate>
                            <div class="dashStatsContent detailZebra"><%# Container.DataItem %></div>
                        </AlternatingItemTemplate>
                    </asp:ListView>
                    </div>
                    </asp:Panel>
                </asp:WizardStep>
                <asp:WizardStep ID="FinishStep" runat="server" StepType="Complete">
                    <div class="padding10">
                        <ckbx:MultiLanguageLabel ID="_completionTitle" runat="server" CssClass="panelTitle" TextId="/pageText/users/emailLists/add.aspx/completionTitle" Text="Email list created"/><br />
                        <ckbx:MultiLanguageLabel ID="_completionInstructions" runat="server" CssClass="" TextId="/pageText/users/emailLists/add.aspx/completionInstructions" Text="You may add another email list or continue to the email list editor to add addresses to this list"/><br />
                        
                        <ckbx:MultiLanguageLabel ID="_createEmailListError" runat="server" CssClass="error message" Visible="false" /><br /><br />
                        </div>
                        <div class="WizardNavContainer">
                        <hr />
                            <btn:CheckboxButton ID="_closeButton" runat="server" CssClass="ckbxButton roundedCorners border999 shadow999 redButton" OnClick="_close_OnClick" TextID="/pageText/users/emailLists/add.aspx/closeButton" />
                            <btn:CheckboxButton ID="_editEmailListButton" runat="server" CssClass="ckbxButton roundedCorners border999 shadow999 orangeButton right" OnClick="_editEmailListButton_OnClick" TextID="/pageText/users/emailLists/add.aspx/exitButton" />
                        </div>
                </asp:WizardStep>
            </WizardSteps>
            <StartNavigationTemplate>
                <btn:WizardButtons ID="_startNavigationButtons" runat="server" NextButtonValidationGroup="emailListValidationGroup" CloseDialogOnCancel="false"/>
            </StartNavigationTemplate>
            <FinishNavigationTemplate>
                <btn:WizardButtons ID="_finishNavigationButtons" runat="server" CloseDialogOnCancel="false" />
            </FinishNavigationTemplate>
            <HeaderTemplate>
                <nav:WizardNavigator ID="_wizardNavigator" runat="server" AllowForwardNavigation="false" />
            </HeaderTemplate>
            <StepNavigationTemplate>
                <btn:WizardButtons ID="_stepNavigationButtons" runat="server"/>
            </StepNavigationTemplate>
        </asp:Wizard>                
    </div>
</asp:Content>