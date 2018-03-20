<%@ Page Title="" Language="C#" MasterPageFile="~/Dialog.Master" AutoEventWireup="false" CodeBehind="AddAddresses.aspx.cs" Inherits="CheckboxWeb.Users.EmailLists.AddAddresses" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Register src="../../Controls/Wizard/WizardNavigator.ascx" tagname="WizardNavigator" tagprefix="nav" %>
<%@ Register src="../../Controls/Wizard/WizardButtons.ascx" tagname="WizardButtons" tagprefix="btn" %>
<%@ Register src="~/Controls/Uploader.ascx" tagPrefix="ckbx" tagName="Uploader" %>

<asp:Content ID="Content1" ContentPlaceHolderID="_headContent" runat="server">
    <ckbx:ResolvingScriptElement runat="server" Source="~/Resources/WizardHighlight.js" />

    <script type="text/javascript" language="javascript">
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
            $('#progressText').empty();//.html('');
            $('#completedDiv').show();
            $('#resultsDiv').html(progressData.StatusMessage);
            $('#importingDiv').hide();
        }

    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="_pageContent" runat="server">
    <asp:Wizard ID="_addAddressesWizard" runat="server" DisplaySideBar="false" DisplayCancelButton="true" OnNextButtonClick="AddAddressesWizard_NextButtonClick" OnCancelButtonClick="AddAddressesWizard_CancelButtonClick">
        <WizardSteps>
            <asp:WizardStep ID="DataSourceStep" runat="server">
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
                                <asp:TextBox ID="_importTxt" runat="server" Rows="15" Columns="80" TextMode="MultiLine" />

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
                <asp:Panel ID="Panel2" runat="server" CssClass="padding10">
                    <div class="dialogSubTitle">
                        
                    </div>
                    <div class="dialogInstructions">
                        <asp:Label ID="_fileReviewSuccess" runat="server" />
                    </div>
                    <div class="dashStatsWrapper border999 shadow999" style="overflow:scroll;height:200px;">
                        <!--<div class="dashStatsHeader"><span class="mainStats left"></span></div>-->
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
                            <div class="dashStatsWrapper border999 shadow999" style="overflow:scroll; height:200px;">
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
                </asp:Panel>
            </asp:WizardStep>
            <asp:WizardStep ID="CompleteStep" runat="server" StepType="Complete">
                <asp:Panel ID="Panel4" runat="server" CssClass="padding10">
                    <div class="dialogSubTitle">
                        <ckbx:MultiLanguageLabel ID="_completionTitle" runat="server" CssClass="panelTitle" TextId="/pageText/users/emailLists/addAddresses.aspx/addressesAdded" />
                    </div>
                    <div class="dialogSubContainer">
                        <div class="dashStatsWrapper border999 shadow999">
                            <div id="importingDiv">
                                <div class="dashStatsHeader" style="padding:5px;">
                                    <span class="mainStats left"><%=WebTextManager.GetText("/pageText/users/emailLists/addAddresses.aspx/addingAddresses")%></span>
                                </div>
                                <div class="padding10">
                                    <ckbx:MultiLanguageLabel ID="_importingLbl" runat="server" CssClass="label" TextId="/pageText/users/emailLists/addAddresses.aspx/importingInProgress" />
                                    <br class="clear" />
                                    <div id="progressDiv"></div>
                                    <div id="progressText"></div>
                                </div>
                            </div>
                            <div id="completedDiv" style="display:none;">
                                <div class="dashStatsHeader"><span class="mainStats left"><%=WebTextManager.GetText("/pageText/users/emailLists/addAddresses.aspx/completedHeader")%></span></div>
                                <div id="resultsDiv" style="margin:5px;">
                                </div>
                                <div class="dialogInstructions">
                                    <ckbx:MultiLanguageLabel ID="_completionInstructions" runat="server" CssClass="" TextId="/pageText/users/emailLists/addAddresses.aspx/completionInstructions" />
                                </div>
                            </div>
                        </div>
                    </div>
                </asp:Panel>
                <div class="WizardNavContainer">
                    <hr />                                                                                                                                                       
                    <btn:CheckboxButton ID="_closeWizardButton" runat="server" CssClass="ckbxButton roundedCorners shadow999 border999 redButton left" OnClientClick="closeWindow(window.top.onDialogClosed, {page: 'addAddresses'}); return false;" TextID="/pageText/users/emailLists/addAddresses.aspx/exitButton" />
                    <btn:CheckboxButton ID="_restartButton" runat="server" CssClass="ckbxButton roundedCorners shadow999 border999 orangeButton right" OnClick="RestartButton_OnClick" TextID="/pageText/users/emailLists/addAddresses.aspx/restartButton" />
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

