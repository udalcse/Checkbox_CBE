<%@ Page Title="" Language="C#" MasterPageFile="~/Dialog.Master" AutoEventWireup="false" CodeBehind="Import.aspx.cs" Inherits="CheckboxWeb.Users.Import" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Register src="../Controls/Wizard/WizardNavigator.ascx" tagname="WizardNavigator" tagprefix="nav" %>
<%@ Register src="../Controls/Wizard/WizardButtons.ascx" tagname="WizardButtons" tagprefix="btn" %>
<%@ Register src="Controls/FileColumnSelector.ascx" tagname="FileSelector" tagprefix="ckbx" %>
<%@ Register src="Controls/RoleSelector.ascx" tagname="RoleSelector" tagprefix="role" %>
<%@ Register src="Controls/GroupSelector.ascx" tagname="GroupSelector" tagprefix="grp" %>
<%@ Register TagPrefix="ckbx" TagName="Uploader" Src="~/Controls/Uploader.ascx" %>


<asp:Content ID="Content1" ContentPlaceHolderID="_headContent" runat="server">
    <ckbx:ResolvingScriptElement runat="server" Source="~/Resources/WizardHighlight.js" />

    <script type="text/javascript" language="javascript">
        //Handle file selection.
        function userFileUploaded(fileData) {
            if (fileData != null) {
                $('#<%=_uploadedFileNameTxt.ClientID %>').val(fileData.name);
                $('#<%=_uploadedFilePathTxt.ClientID %>').val(fileData.TempName);
            }
        }

        //
        $(document).ready(function() {
             $('#validationTab').ckbxTabs({ 
                tabName: 'validationTab',
                initialTabIndex: 0,
                onTabClick: function(index){},
                onTabsLoaded: function(){$('.tabContainer').show();$('.tabContentContainer').show();}
            });
        });

        //Show message on error
        function onProgressError(errorMessage) {
              $('#progressDiv').hide();
              $('#progressText').html('<div class="ErrorMessage">An error occurred: <span style="color:black;font-weight:normal;">&nbsp;&nbsp;' + errorMessage + '</span></div>');
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
    <asp:Wizard ID="_importUserWizard" runat="server" DisplaySideBar="false" DisplayCancelButton="true" OnNextButtonClick="ImportUserWizard_NextButtonClick" OnCancelButtonClick="ImportUserWizard_CancelButtonClick" >
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
            <asp:WizardStep ID="LayoutStep" runat="server">
                <asp:Panel ID="Panel" runat="server" CssClass="padding10">
                    <div class="dialogSubTitle">
                        <%=WebTextManager.GetText("/pageText/users/import.aspx/layoutTitle") %>
                    </div>
                    <div class="dialogInstructions">
                        <asp:Label ID="_layoutInstructions" runat="server" />
                    </div>
                    <div class="dialogSubContainer">
                        <div><asp:Label ID="_fieldSelectionError" runat="server" CssClass="error message" Visible="false" /></div>
                        <ckbx:FileSelector ID="_fileColumnSelector" runat="server" />
                    </div>
                </asp:Panel>
            </asp:WizardStep>
            <asp:WizardStep ID="UploadStep" runat="server">
                <asp:Panel ID="Panel1" runat="server" CssClass="padding10">
                    <div class="dialogSubTitle">
                        <ckbx:MultiLanguageLabel id="_userSourceLabel" runat="server" TextId="/pageText/users/import.aspx/userSource">Data source</ckbx:MultiLanguageLabel>
                    </div>
                    <div class="dialogSubContainer overflowPanel_400" style="width:850px">
                        <div class="left formInput radioList fixed_350">
                            <ckbx:MultiLanguageLabel id="label" runat="server" CssClass="panelTitle" TextId="/pageText/users/import.aspx/sourceQuestion"></ckbx:MultiLanguageLabel>
                            <ckbx:MultiLanguageRadioButtonList ID="_userSourceList" runat="server" RepeatDirection="Vertical" AutoPostBack="True" OnSelectedIndexChanged="UserSource_SelectedIndexChanged">
                                <asp:ListItem TextId="/pageText/users/import.aspx/csvText"  Value="Upload" Selected="True"/>
                                <asp:ListItem TextId="/pageText/users/import.aspx/textEntry" Value="Enter" />
                            </ckbx:MultiLanguageRadioButtonList>
                            <br />
                           
                        </div>
                        <div class="right fixed_400">
                            <ckbx:MultiLanguageLabel ID="_fieldOrderTitle" runat="server" CssClass="panelTitle" TextId="/pageText/users/import.aspx/dataFieldOrderTitle" /><br />
                            <ckbx:MultiLanguageLabel ID="_fieldOrderInstructions" runat="server" CssClass="" TextId="/pageText/users/import.aspx/dataFieldOrderInstructions" /><br />
                            <asp:Label ID="_fieldOrderExample" runat="server" CssClass="fieldOrderExample" />
                        </div>
                        <br class="clear" />
                         <ckbx:MultiLanguageLabel id="_descriptionLabel" runat="server" TextId="/pageText/users/import.aspx/csvText/explanation"/>
                        <div style="margin-top:5px;"></div>
                        
                        <asp:Panel ID="_fileUploadPanel" runat="server" Visible="true">
                            <fieldset>
                                <legend><ckbx:MultiLanguageLabel ID="_fileUploadTitle" runat="server" CssClass="panelTitle" TextId="/pageText/users/import.aspx/fileUploadTitle" /></legend>
                                <div style="display:none;">
                                    <asp:TextBox ID="_uploadedFileNameTxt" runat="server" />
                                    <asp:TextBox ID="_uploadedFilePathTxt" runat="server" />
                                </div>

                                <!--<ckbx:MultiLanguageLabel ID="_fileUploadInstructions" runat="server" CssClass="" TextId="/pageText/users/import.aspx/fileUploadInstructions" /><br />-->
                                <asp:Label ID="_fileUploadError" runat="server" CssClass="error message" Visible="false" /><br />
                            
                                <ckbx:Uploader ID="_uploader" runat="server" UploadedCallback="userFileUploaded" SelectFilePromptTextId="/controlText/fileUploader/selectFile" />
                            
                                <ckbx:MultiLanguageLabel ID="_fileEncodingLabel" runat="server" TextId="/pageText/users/import.aspx/fileUploadEncoding" />
                                <ckbx:MultiLanguageRadioButtonList ID="_uploadFileEncoding" runat="server">
                                    <asp:ListItem TextId="/pageText/users/import.aspx/ascii" Value="ASCII"/>
                                    <asp:ListItem TextId="/pageText/users/import.aspx/unicode" Value="UTF-8" Selected="True"/>
                                </ckbx:MultiLanguageRadioButtonList>
                            </fieldset>
                        </asp:Panel>
                        <asp:Panel ID="_textEntryPanel" runat="server" Visible="false">
                            <fieldset>
                                <legend><ckbx:MultiLanguageLabel ID="_textEntryTitle" runat="server" CssClass="panelTitle" TextId="/pageText/users/import.aspx/textEntryTitle" /></legend>
                                <ckbx:MultiLanguageLabel ID="_textEntryInstructions" runat="server" CssClass="" TextId="/pageText/users/import.aspx/textEntryInstructions" /><br />
                                <div><asp:Label ID="_textEntryError" runat="server" CssClass="error message" Visible="false" /></div>
                                <asp:TextBox ID="_importTxt" runat="server" Rows="7" Columns="85" TextMode="MultiLine" />

                                <div>
                                    <ckbx:MultiLanguageLabel ID="_textEntryEncodingLabel" runat="server" TextId="/pageText/users/import.aspx/textEntryEncoding" />
                                    <ckbx:MultiLanguageRadioButtonList ID="_textEntryEncoding" runat="server">
                                        <asp:ListItem TextId="/pageText/users/import.aspx/ascii" Value="ASCII" />
                                        <asp:ListItem TextId="/pageText/users/import.aspx/unicode" Value="UTF-8" Selected="True"/>
                                    </ckbx:MultiLanguageRadioButtonList>
                                </div>
                            </fieldset>
                        </asp:Panel>
                    </div>
                </asp:Panel>
            </asp:WizardStep>
            <asp:WizardStep ID="FileReviewStep" runat="server">
                <asp:Panel ID="Panel2" runat="server" CssClass="padding10">
                    <div class="dialogSubTitle">
                        <%=WebTextManager.GetText("/pageText/users/import.aspx/fileReviewTitle") %>
                    </div>
                    <div class="dialogInstructions">
                        <div class="left" style="margin-left:15px;">
                            <asp:Label ID="_fileReviewSuccess" runat="server" />
                        </div>
                        <div class="left"><asp:Label ID="_fileReviewError" runat="server" CssClass="error message" Visible="false" /></div>
                        <br class="clear"/>
                    </div>
                    <div class="dialogSubContainer">
                        <div class="left padding10">
                            <ul id="validationTab" class="tabContainer left border999 shadow999" style="width:150px;">
                                <li style="width:100%;border-bottom:1px solid #999;"><ckbx:MultiLanguageLiteral ID="_validTxt" runat="server" TextID="/pageText/users/import.aspx/validEntries" /></li>
                                <li style="width:100%;border-bottom:1px solid #999;"><ckbx:MultiLanguageLiteral ID="MultiLanguageLiteral1" runat="server" TextID="/pageText/users/import.aspx/invalidEntries" /></li>
                            </ul>
                        </div>
                        <div class="left" style="overflow:scroll; height:375px; width:670px;">
                            <div id="validationTab-0-tabContent">
                                <asp:Panel ID="_validUserPanel" runat="server" CssClass="border999 shadow999" style="min-height:355px; background-color:#F0F0F0;">
                                    <asp:ListView ID="_validUserFieldList" runat="server" ItemPlaceholderID="headerPlaceHolder">
                                        <LayoutTemplate>                                       
                                            <div class="dashStatsHeader"><asp:PlaceHolder ID="headerPlaceHolder" runat="server" /></div>
                                        </LayoutTemplate>
                                        <ItemTemplate>
                                            <span class="left mainStats fixed_190"><%# Container.DataItem %></span>
                                        </ItemTemplate>
                                    </asp:ListView>
                                    <asp:ListView ID="_validUserList" runat="server" ItemPlaceholderID="outerPlaceHolder">
                                        <LayoutTemplate>                                        
                                            <asp:PlaceHolder ID="outerPlaceHolder" runat="server" />
                                        </LayoutTemplate>
                                        <ItemTemplate>
                                            <div class="zebra">
                                                <asp:ListView ID="_validUserListColumnList" runat="server" DataSource="<%# Container.DataItem %>" ItemPlaceholderID="innerPlaceHolder">
                                                    <LayoutTemplate><asp:PlaceHolder ID="innerPlaceHolder" runat="server" /></LayoutTemplate>
                                                    <ItemTemplate>
                                                        <div class="fixed_180 left dashStatsContent"><%# Container.DataItem %></div>
                                                    </ItemTemplate>
                                                </asp:ListView>
                                                <br class="clear" />
                                            </div>
                                        </ItemTemplate>
                                        <AlternatingItemTemplate>
                                            <div class="detailZebra">
                                                <asp:ListView ID="_validUserListAlternateColumnList" runat="server" DataSource="<%# Container.DataItem %>" ItemPlaceholderID="alternateInnerPlaceHolder">
                                                    <LayoutTemplate><asp:PlaceHolder ID="alternateInnerPlaceHolder" runat="server" /></LayoutTemplate>
                                                    <ItemTemplate>
                                                        <div class="fixed_180 left dashStatsContent"><%# Container.DataItem %></div>
                                                    </ItemTemplate>
                                                </asp:ListView>
                                                <br class="clear" />
                                            </div>
                                        </AlternatingItemTemplate>
                                    </asp:ListView>
                                </asp:Panel>
                            </div>
                            <div id="validationTab-1-tabContent">
                                <asp:ListView ID="_invalidUserList" runat="server">
                                    <LayoutTemplate>
                                        <div class="border999 shadow999 centerContent" style="min-height:355px; background-color:#F0F0F0;">
                                            <asp:PlaceHolder ID="itemPlaceholder" runat="server" />
                                        </div>
                                    </LayoutTemplate>
                                    <ItemTemplate>
                                        <div class="zebra dashStatsContent"><%# Container.DataItem  %></div>
                                    </ItemTemplate>
                                    <AlternatingItemTemplate>
                                        <div class="detailZebra dashStatsContent"><%# Container.DataItem %></div>
                                    </AlternatingItemTemplate>
                                    <EmptyDataTemplate>
                                        <div class="border999 shadow999 centerContent" style="min-height:355px; background-color:#F0F0F0;">
                                            <ckbx:MultiLanguageLiteral ID="_noInvalidRowsLbl" runat="server" TextId="/pageText/users/import.aspx/noInvalidRows" />
                                        </div>
                                     </EmptyDataTemplate>
                                </asp:ListView>
                            </div>
                        </div>
                    </div>
                </asp:Panel>
            </asp:WizardStep>
            <asp:WizardStep ID="RoleStep" runat="server">
                <asp:Panel ID="Panel3" runat="server" CssClass="padding10">
                    <div class="dialogSubTitle">
                        <ckbx:MultiLanguageLabel ID="_roleTitle" runat="server" TextId="/pageText/users/import.aspx/roleTitle" />
                    </div>
                    <div class="dialogInstructions">
                        <ckbx:MultiLanguageLabel ID="_roleInstructions" runat="server" CssClass="" TextId="/pageText/users/import.aspx/roleInstructions" />
                    </div>
                    <div><ckbx:MultiLanguageLabel ID="_roleRequiredError" runat="server" CssClass="error message" Visible="false" TextId="/pageText/users/import.aspx/roleRequired" /></div>
                    <div class="padding15"><role:RoleSelector ID="_roleSelector" runat="server" /></div>
                </asp:Panel>
            </asp:WizardStep>
            <asp:WizardStep ID="GroupStep" runat="server">
                <asp:Panel ID="Panel4" runat="server" CssClass="padding10">
                    <div class="dialogSubTitle">
                        <ckbx:MultiLanguageLabel ID="_groupTitle" runat="server" CssClass="panelTitle" TextId="/pageText/users/import.aspx/groupTitle" />
                    </div>
                    <div class="dialogInstructions">
                        <ckbx:MultiLanguageLabel ID="_groupInstructions" runat="server" CssClass="" TextId="/pageText/users/import.aspx/groupInstructions" />
                    </div>
                    <div><ckbx:MultiLanguageLabel ID="_groupRequiredError" runat="server" CssClass="error message" Visible="false" TextId="/pageText/users/import.aspx/groupRequired" /></div>
                    <grp:GroupSelector ID="_groupSelector" runat="server"/>
                </asp:Panel>
            </asp:WizardStep>
            <asp:WizardStep ID="ReviewStep" runat="server" StepType="Finish">
                <asp:Panel ID="Panel5" runat="server" CssClass="padding10">
                    <div class="dialogSubTitle" style="width:250px !important;">
                        <ckbx:MultiLanguageLabel ID="_reviewTitle" runat="server" CssClass="panelTitle" TextId="/pageText/users/import.aspx/reviewTitle" />
                    </div>
                    <div class="dialogInstructions">
                        <ckbx:MultiLanguageLabel ID="_reviewInstructions" runat="server" CssClass="" TextId="/pageText/users/import.aspx/reviewInstructions" />
                    </div>
                    <asp:Panel ID="_importWaitWarningPanel" runat="server" Visible="false" CssClass="StatusPanel warning">
                        <ckbx:MultiLanguageLabel ID="_importWaitWarning" runat="server" TextId="/pageText/users/import.aspx/importConfirm" />
                    </asp:Panel>
                    <div class="dialogSubContainer">
                        <p class="selectorTitle">
                            <ckbx:MultiLanguageLabel id="_userInfoReviewTitle" runat="server" TextId="/pageText/users/import.aspx/userInfoReviewTitle">User information</ckbx:MultiLanguageLabel>
                        </p>
                        <div style="margin:10px;"><asp:Label ID="_userCountReview" runat="server"></asp:Label></div>
                    
                        <div class="formInput left">
                            <div class="left checkBox"><asp:Checkbox runat="server" ID="_updateUsers" /></div>
                            <div class="left"><p><label for="<%=_updateUsers.ClientID %>"><%=WebTextManager.GetText("/pageText/users/import.aspx/updateExisting")%></label></p></div>
                        </div>
                        <div class="left" style="width:500px;margin-left:10px;margin-top:8px;margin-bottom:5px;font-style:italic;">
                            <ckbx:MultiLanguageLabel AssociatedControlID="_updateUsers" ID="_updateUsersInstructions" runat="server" TextId="/pageText/users/import.aspx/updateDescription" />
                        </div>
                        <br class="clear"/>
                        <div class="centerContent" style="width:820px;">
                            <div class="dashStatsWrapper fixed_400 border999 shadow999 left" style="overflow-y:scroll;height:175px;">
                                <div class="dashStatsHeader">
                                    <ckbx:MultiLanguageLabel id="_rolesReviewTitle" runat="server" CssClass="left mainStats" TextId="/pageText/users/import.aspx/rolesReviewTitle">Selected roles</ckbx:MultiLanguageLabel>
                                </div>
                                <asp:ListView ID="_rolesReviewList" runat="server">
                                    <LayoutTemplate>
                                        <asp:PlaceHolder runat="server" ID="itemPlaceholder"></asp:PlaceHolder>
                                    </LayoutTemplate>
                                    <ItemTemplate>
                                        <div class="zebra dashStatsContent"><asp:Label ID="_roleReviewLabel" runat="server" Text="<%# Container.DataItem %>" /></div>
                                    </ItemTemplate>
                                    <AlternatingItemTemplate>
                                        <div class="detailZebra dashStatsContent"><asp:Label ID="_roleReviewLabel" runat="server" Text="<%# Container.DataItem %>" /></div>
                                    </AlternatingItemTemplate>
                                </asp:ListView>
                            </div>
                            <div class="dashStatsWrapper fixed_400 border999 shadow999 right" style="overflow-y:scroll;height:175px;">
                                <div class="dashStatsHeader">
                                    <ckbx:MultiLanguageLabel id="_groupsReviewTitle" runat="server" CssClass="left mainStats" TextId="/pageText/users/import.aspx/groupsReviewTitle">Selected groups</ckbx:MultiLanguageLabel>
                                </div>
                                <asp:ListView ID="_groupsReviewList" runat="server" OnItemCreated="GroupsReviewList_ItemCreated">
                                    <LayoutTemplate>
                                        <asp:PlaceHolder runat="server" ID="itemPlaceholder"></asp:PlaceHolder>
                                    </LayoutTemplate>
                                    <ItemTemplate>
                                        <div class="zebra dashStatsContent"><asp:Label ID="_groupReviewLabel" runat="server" /></div>
                                    </ItemTemplate>
                                    <AlternatingItemTemplate>
                                        <div class="detailZebra dashStatsContent"><asp:Label ID="_groupReviewLabel" runat="server" /></div>
                                    </AlternatingItemTemplate>
                                    <EmptyDataTemplate>
                                        <div class="zebra dashStatsContent"><ckbx:MultiLanguageLabel ID="_groupReviewNoGroupsLabel" runat="server" TextId="/pageText/users/import.aspx/groupsReviewNoGroups" /></div>
                                    </EmptyDataTemplate>
                                </asp:ListView>
                            </div>
                        </div>
                    </div>
                </asp:Panel>
            </asp:WizardStep>
            <asp:WizardStep ID="CompleteStep" runat="server" StepType="Complete">
                <asp:Panel ID="Panel6" runat="server" CssClass="padding10">
                    <div class="dialogSubContainer">
                        <div class="dashStatsWrapper border999 shadow999">
                            <div id="importingDiv">
                                <div class="dashStatsHeader" style="padding:5px;">
                                    <span class="mainStats left"><%=WebTextManager.GetText("/pageText/users/import.aspx/importingUsers") %></span>
                                </div>
                                <div class="padding10">
                                    <ckbx:MultiLanguageLabel ID="_importingLbl" runat="server" CssClass="label" TextId="/pageText/users/import.aspx/importingInProgress" />
                                    <br class="clear" />
                                    <div id="progressDiv">&nbsp;</div>
                                    <div id="progressText">&nbsp;</div>
                                </div>
                            </div>
                            <div id="completedDiv" style="display:none;">
                                <div class="dashStatsHeader"><span class="mainStats left"><%=WebTextManager.GetText("/pageText/users/import.aspx/completedHeader") %></span></div>
                                <div id="resultsDiv" style="margin:5px;">
                                </div>
                                <div class="dialogInstructions">
                                    <ckbx:MultiLanguageLabel ID="_completionInstructions" runat="server" CssClass="" TextId="/pageText/users/import.aspx/completionInstructions" />
                                </div>
                            </div>
                        </div>
                    </div>
                </asp:Panel>
                <div class="WizardNavContainer">
                    <hr />
                    <btn:CheckboxButton ID="_closeWizardButton" runat="server" CssClass="cancelButton left"  OnClick="_closeWizardButton_Click" TextID="/pageText/users/import.aspx/exitButton" />
                    <btn:CheckboxButton ID="_restartButton" runat="server" CssClass="ckbxButton roundedCorners shadow999 border999 silverButton right" OnClick="RestartButton_OnClick" TextID="/pageText/users/import.aspx/restartButton" />
                </div>
            </asp:WizardStep>
        </WizardSteps>
    </asp:Wizard>                
</asp:Content>
