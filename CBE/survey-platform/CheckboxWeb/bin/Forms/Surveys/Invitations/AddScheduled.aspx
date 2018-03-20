<%@ Page Title="" Language="C#" MasterPageFile="~/Dialog.Master" AutoEventWireup="false" CodeBehind="AddScheduled.aspx.cs" Inherits="CheckboxWeb.Forms.Surveys.Invitations.AddScheduled" EnableEventValidation="false" ValidateRequest="false" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>
<%@ Register src="../../../Controls/Wizard/WizardNavigator.ascx" tagname="WizardNavigator" tagprefix="nav" %>
<%@ Register src="../../../Controls/Wizard/WizardButtons.ascx" tagname="WizardButtons" tagprefix="btn" %>
<%@ Register TagName="RecipientEditor" TagPrefix="ckbx" Src="~/Forms/Surveys/Invitations/Controls/Recipients.ascx" %>
<%@ Register TagName="AddRecipients" TagPrefix="ckbx" Src="~/Forms/Surveys/Invitations/Controls/AddRecipients.ascx" %>
<%@ Register Src="~/Forms/Surveys/Invitations/Controls/EditMessageControl.ascx" TagPrefix="ckbx" TagName="EditMessage" %>
<%@ Import Namespace="Checkbox.Web" %>

<asp:Content ID="Content1" ContentPlaceHolderID="_headContent" runat="server">
    <ckbx:ResolvingScriptElement runat="server" Source="~/Resources/DialogHandler.js" />
    <ckbx:ResolvingScriptElement runat="server" Source="~/Resources/WizardHighlight.js" />
    <ckbx:ResolvingScriptElement runat="server" Source="~/Resources/jquery-ui-timepicker-addon.js" />
    <ckbx:ResolvingScriptElement runat="server" Source="~/Resources/AjaxProgress.js" />
    <ckbx:ResolvingScriptElement runat="server" Source="~/Resources/jquery.progressbar.min.js" />    
    <ckbx:ResolvingScriptElement ID="_datePickerLocaleResolver" runat="server" />

     <script type="text/javascript">
         var _liveSearchForScheduled;
         var _pendingUserCurrentView = '#<%=(HasRecipients ? "pendingRecipientsContainer" : "addRecipientsContainer") %>';

         $(document).ready(function() {
             $(_pendingUserCurrentView).show();

             initRecipientsTabs();

             $('select, input:checkbox, input:radio, input:file, input:text').filter(':not([uniformIgnore])').uniform();

             $('#<%=_name.ClientID %>').attr('title', '<%=WebTextManager.GetText("/pageText/forms/surveys/invitations/add.aspx/nameTip") %>');
             $('#<%=_name.ClientID %>').watermark();

             initScheduleStep();

             $('[name$="MessageSource"]').on('change', function (){ checkMessageOptionsVisibility(this);});
             checkMessageOptionsVisibility($('[name$="MessageSource"]:checked'));
             
             _liveSearchForScheduled = new LiveSearchObj('innerSearchDiv');
         });

         function checkMessageOptionsVisibility(elem) {
             if (!$(elem).length || $(elem).val() == '_newInvitationRad') {
                 $('#emailFormat').show();
                 $('#messageSourceDropDown').hide();      
             } else {
                 $('#emailFormat').hide();                     
                 $('#messageSourceDropDown').show();      
             }  
         }

         //
         function initRecipientsTabs() {
              $('#addRecipientTabs').ckbxTabs({ 
                tabName: 'addRecipientTabs',
                initialTabIndex: <%=(HasRecipients ? "-1" : "0") %>,
                onTabsLoaded: function(){$('.tabContainer').show();$('.tabContentContainer').show();},
                onTabClick: function(index) {
                    togglePendingRecipientView(index);
                }
            });
        }

         //
        function showPendingRecipientsView() {
            $('#addRecipientTabs').ckbxTabs('deSelect');
             togglePendingRecipientView(-1);
         }
         ///
         function togglePendingRecipientView(index) {
             var newView = index < 0 ? '#pendingRecipientsContainer' : '#addRecipientsContainer';

             if (index == 0) {
                 $('#searchDiv').hide();
             } else {
                 $('#searchDiv').show();
                 _liveSearchForScheduled.changeFilterName('searchTerm' + index);
                 _liveSearchForScheduled.doSearch();
             }
                          
             if(newView != _pendingUserCurrentView) {
                 $(_pendingUserCurrentView).hide('fold', null, 500, function() {
                     $(newView).show('fold', null, 500);
                 });
             }

             _pendingUserCurrentView = newView;
         }

         /************** TEST INVITATION *************************/
         function setEmailStatus(type, text)
         {
            $('#_testInvitationEmail' + type).html(text);
            $('#_testInvitationEmail' + type).show();
            setTimeout(function(){
                $('#_testInvitationEmailError').hide('slow');
                $('#_testInvitationEmailInfo').hide('slow');
                }, 5000);
         }

         var lastTestScheduleID = 0;
         function askForScheduleStatus(scheduleId)
         {
            lastTestScheduleID = scheduleId;
            svcInvitationManagement.getScheduleStatus(_at, scheduleId, 
                onScheduleStatusRecieved);            
         }

         function onScheduleStatusRecieved(res)
         {
            if (res != null && typeof(res) != 'undefined' && res != '')
            {
                setEmailStatus('Info', '<%=WebTextManager.GetText("/pageText/forms/surveys/invitations/add.aspx/serviceReplied") %>' + res);
            }
            else
            {
                setTimeout(function() { askForScheduleStatus(lastTestScheduleID)}, 1000);
            }
         }

         function refreshEmailStatus(scheduleId)
         {
            setTimeout(function() { askForScheduleStatus(scheduleId)}, 1000);
         }

         /************** PROGRESS *************************/
         var progressStatusSuccess = false;

         //Start progress monitoring
         function startProgress(progressKey, mode) {             
             //Start send process
             $.ajax({
                 type: "GET",
                 url: '<%=ResolveUrl("~/Forms/Surveys/Invitations/Worker.aspx")%>',
                 async: true,
                 contentType: "application/json; charset=utf-8",
                 data: { mode: mode, i: '<%=InvitationId%>' },
                 dataType: "json",
                 timeout: 100
             });         //Set a short timeout to work around issues where load balancers and the like
             // may timeout after 60 seconds, etc.  This essentially makes the call fire and
             // forget instead of waiting for error/success error to return.

             //Start monitoring progress
             ajaxProgress.startProgress(
                progressKey,
                'progressDiv',
                '<%=ResolveUrl("~/") %>',
                onProgressUpdate,
                onProgressError,
                onProgressComplete);
         }

         //Show message on error
         function onProgressError(errorMessage) {
             $('#progressDiv').hide();
             $('#progressText').addClass('error');
             $('#progressText').html('<div class="ErrorMessage">An error occurred while updating progress: <span style="color:black;font-weight:normal;">&nbsp;&nbsp;' + errorMessage + '</span></div>');
         }

         //Update status
         function onProgressUpdate(progressData) {             
             $('#progressDiv').show();
             $('#progressText').html(progressData.StatusMessage);
         }

         //Do something on progress complete
         function onProgressComplete(progressData) {
             $('#progressText').hide();
         } 
         
         //
         function onRecipientsAdded() {
             $('#addRecipientTabs').ckbxTabs('deSelect');
             reloadRecipientsGrid();
             togglePendingRecipientView(-1);
         }

         function checkDateVisibility() {
                if ($('#<%=_schedule.ClientID%>').prop("checked"))
                    $('#<%=_scheduledDate.ClientID%>').show();
                else
                    $('#<%=_scheduledDate.ClientID%>').hide();
         }

         function initScheduleStep() {
            //$('#<%=_scheduledDate.ClientID%>').datetimepicker("setDate", new Date().addHours(1));
            checkDateVisibility();
            $('#<%=_immediate.ClientID%>').change(function (){ checkDateVisibility();});
            $('#<%=_schedule.ClientID%>').change(function (){ checkDateVisibility();});
            $('#<%=_postponed.ClientID%>').change(function (){ checkDateVisibility();});
         }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="_pageContent" runat="server">
    <asp:Wizard ID="_invitationWizard" runat="server" 
        DisplaySideBar="false" 
        DisplayCancelButton="false" >
        <WizardSteps>
            <asp:WizardStep ID="StartStep" runat="server">
                <div class="padding10">
                    <div class="dialogInstructions">
                        <ckbx:MultiLanguageLabel ID="_nameInstructions" runat="server" CssClass="" TextId="/pageText/forms/surveys/invitations/add.aspx/nameInstructions"/>
                    </div>
                    
                    <div class="formInput large">
                        <p><ckbx:MultiLanguageLabel ID="_nameTextLabel" runat="server" TextId="/pageText/forms/surveys/invitations/add.aspx/nameLabel" AssociatedControlID="_name" /></p>
                        <asp:TextBox ID="_name" runat="server" Width="500" />
                        <asp:RequiredFieldValidator ID="_nameRequired" runat="server" ControlToValidate="_name" CssClass="error message"><%= WebTextManager.GetText("/pageText/forms/surveys/invitations/add.aspx/nameError")%></asp:RequiredFieldValidator>
                        <br class="clear" />
                    </div>
                    
                    <div class="formInput">
                        <fieldset>
                            <legend><ckbx:MultiLanguageLabel  ID="_formatLabel" runat="server" CssClass="" TextId="/pageText/forms/surveys/invitations/add.aspx/optionsLabel" AssociatedControlID="_existingInvitationSource" /></legend>

                            <asp:PlaceHolder ID="_invitationSourcePlace" runat="server">
                                <p><label><ckbx:MultiLanguageLiteral ID="_sourceLiteral" runat="server" TextId="/pageText/forms/surveys/invitations/add.aspx/sourceLabel" /></label></p>
                                <div class="left fixed_25">&nbsp;</div>
                                <div class="left radioButton small">
                                    <asp:RadioButton ID="_newInvitationRad" runat="server" GroupName="MessageSource" Checked="True" AutoPostBack="False" />
                                </div>
                                <div class="left small">
                                    <p><ckbx:MultiLanguageLabel ID="_createNewLbl" runat="server" TextId="/pageText/forms/surveys/invitations/add.aspx/createNew" AssociatedControlID="_newInvitationRad" /></p>
                                </div>

                                <br class="clear" />

                                <div class="left fixed_25">&nbsp;</div>
                                <div class="left radioButton small">
                                    <asp:RadioButton ID="_copyInvitationRad" runat="server" GroupName="MessageSource" Checked="False" AutoPostBack="False" />
                                </div>
                                <div class="left small">
                                    <p><ckbx:MultiLanguageLabel ID="_copyInvitationLbl" runat="server" TextId="/pageText/forms/surveys/invitations/add.aspx/copyExisting" AssociatedControlID="_copyInvitationRad" /></p>
                                </div>
                            
                                <br class="clear" />
                                
                                <div id="messageSourceDropDown">
                                    <div class="left fixed_25">&nbsp;</div>
                                    <div class="left dropDown">
                                        <asp:DropDownList ID="_existingInvitationSource" runat="server" />
                                    </div>
                                </div>
                                <br class="clear" />
                            </asp:PlaceHolder>

                            <asp:PlaceHolder ID ="_messageFormatPanel" runat ="server">
                                <div id="emailFormat">
                                    <p><label for="messageFormat"><ckbx:MultiLanguageLiteral ID="_formatLiteral" runat="server" TextId="/pageText/reviewInvitation.aspx/format" /></label></p>

                                    <div class="left fixed_25">&nbsp;</div>
                                    <div class="left radioButton small">
                                        <asp:RadioButton ID="_formatHtmlRad" runat="server" Checked="true" GroupName="MessageFormatGroup" />
                                    </div>
                                    <div class="left small">
                                        <p><ckbx:MultiLanguageLabel ID="_formatHtmlLbl" runat="server" AssociatedControlID="_formatHtmlRad" TextId="/controlText/htmlItemEditor/htmlMode" /></p>
                                    </div>
                                    <br class="clear" />

                                    <div class="left fixed_25">&nbsp;</div>
                                    <div class="left radioButton small">
                                        <asp:RadioButton ID="_formatTextRad" runat="server" Checked="false" GroupName="MessageFormatGroup" />
                                    </div>
                                    <div class="left small">
                                        <p><ckbx:MultiLanguageLabel ID="_formatTxtLbl" runat="server" AssociatedControlID="_formatTextRad" TextId="/controlText/htmlItemEditor/textMode" /></p>
                                    </div>
                                </div>
                                <br class="clear" />
                            </asp:PlaceHolder>

                            <p><label for="messageFormat"><ckbx:MultiLanguageLiteral ID="_otherOptionsLiteral" runat="server" TextId="/pageText/forms/surveys/invitations/add.aspx/otherOptionsLabel" /></label></p>

                            <div class="left fixed_25">&nbsp;</div>
                            <div class="left checkBox small">
                                <asp:CheckBox ID="_autoLoginChk" runat="server" Checked="true" />
                            </div>
                            <div class="left small">
                                <p><ckbx:MultiLanguageLabel ID="_autoLoginLbl" runat="server" AssociatedControlID="_formatHtmlRad" TextId="/pageText/forms/surveys/invitations/add.aspx/autoLogin" /></p>
                            </div>
                            <br class="clear" />
                            
                            <div class="left fixed_25">&nbsp;</div>
                            <div class="left checkBox small">
                                <asp:CheckBox ID="_optOutChk" runat="server" Checked="false" />
                            </div>
                            <div class="left small">
                                <p><ckbx:MultiLanguageLabel ID="_optOutLbl" runat="server" AssociatedControlID="_optOutChk" TextId="/pageText/forms/surveys/invitations/add.aspx/optOut" /></p>
                            </div>

                            <br class="clear" />
                            
                            <asp:Panel runat="server" ID="_selectProfilePanel" Visible="False">
                                <p><label for="messageFormat"><ckbx:MultiLanguageLiteral runat="server" TextId="/pageText/forms/surveys/invitations/add.aspx/companyProfileLabel" /></label></p>

                                <div class="left fixed_25">&nbsp;</div>
                                <div class="left radioButton small">
                                    <asp:DropDownList ID="_companyProfileList" runat="server" AutoPostBack="False" />
                                </div>
                                <br class="clear" />
                            </asp:Panel>

                        </fieldset>
                    </div>
                </div>
            </asp:WizardStep>
            <asp:WizardStep ID="TextStep" runat="server">
                <div class="padding10 overflowPanel_468">
                    <ckbx:EditMessage ID="_editMessage" runat="server" />
                </div>
            </asp:WizardStep>
            <asp:WizardStep ID="RecipientStep" runat="server">
                <div class="padding10">
                    <div class="dialogInstructions">
                        <ckbx:MultiLanguageLabel id="_descriptionLabel" runat="server" TextId="/pageText/forms/surveys/invitations/add.aspx/recipientSourceInstructions"/>
                    </div>
                    <div class="recipient-tabs-wrapper clearfix">
                        <h4>Add:</h4>
                        <ul id="addRecipientTabs" class="tabContainer">
                            <li><ckbx:MultiLanguageLiteral ID="MultiLanguageLiteral2" runat="server" TextID="/pageText/forms/surveys/invitations/add.aspx/addEmail" /></li>
                            <li><ckbx:MultiLanguageLiteral ID="_addUsersTxt" runat="server" TextID="/pageText/forms/surveys/invitations/add.aspx/addUsers" /></li>
                            <li><ckbx:MultiLanguageLiteral ID="MultiLanguageLiteral1" runat="server" TextID="/pageText/forms/surveys/invitations/add.aspx/addGroups" /></li>
                            <li><ckbx:MultiLanguageLiteral ID="MultiLanguageLiteral3" runat="server" TextID="/pageText/forms/surveys/invitations/add.aspx/addEmailLists" /></li>
                        </ul>
                    </div>
                    <div class="recipient-list-wrapper">

                        <div id="pendingRecipientsContainer" class="dashStatsWrapper border999" style='width:100%;display:none;'>
                            <div class="dashStatsHeader">
                                <div class="mainStats">
                                    <ckbx:MultiLanguageLabel ID="_selectedRecipientsTitle" runat="server" TextId="/pageText/forms/surveys/invitations/add.aspx/selectedRecipientsTitle" />
                                </div>
                            </div>
                            
                            <ckbx:RecipientEditor ID="_recipientEditor" runat="server" PendingInvitationMode="True" GridCssClass="ckbxInvitationWizardRecipientsGrid"  />
                        </div>

                        <div id="addRecipientsContainer" class="dashStatsWrapper border999 shadow999" style='width:100%;display:none;'>
                            <div class="dashStatsHeader">
                                <div class="mainStats">
                                    <ckbx:MultiLanguageLabel ID="_addRecipientsLbl" runat="server" TextId="/pageText/forms/surveys/invitations/add.aspx/recipientSourceTitle" />
                                </div>
                            </div>

                            <ckbx:AddRecipients ID="_addRecipients" runat="server" OnRecipientsAdded="onRecipientsAdded" PendingInvitationMode="True" GridCssClass="ckbxInvitationWizardRecipientsGrid" />
                        </div>
                    </div>
                    <br class="clear" />
                </div>
            </asp:WizardStep>
            <asp:WizardStep ID="ScheduleStep" runat="server" StepType="Finish">
                <div class="dialogSubTitle">
                    <ckbx:MultiLanguageLabel ID="_scheduleTitle" runat="server" TextId="/pageText/forms/surveys/invitations/add.aspx/scheduleTitle" />
                </div>
                <div class="padding10">
                    <div>
                        <div class="padding10">
                            <ckbx:MultiLanguageRadioButton ID="_immediate" runat="server" Checked="true" GroupName="mode" TextId="/pageText/forms/surveys/invitations/add.aspx/immediateOption"/>
                        </div>
                        <div class="padding10">
                            <ckbx:MultiLanguageRadioButton ID="_schedule" runat="server" GroupName="mode" TextID="/pageText/forms/surveys/invitations/add.aspx/scheduleOption" /> <ckbx:DateTimePicker NumberOfMonths="2" ID="_scheduledDate" runat="server" style="display:none;"/>
                        </div>                        
                        <div class="padding10">
                            <ckbx:MultiLanguageRadioButton ID="_postponed" runat="server" Checked="false" GroupName="mode" TextId="/pageText/forms/surveys/invitations/add.aspx/postponedOption"/>
                        </div>
                    </div>
                </div>
                <div class="dialogSubTitle">
                    <ckbx:MultiLanguageLabel ID="_testEmailTitle" runat="server" TextId="/pageText/forms/surveys/invitations/add.aspx/sendTestInvitation" />
                </div>
                <div class="padding10">
                    <div ID="_testInvitationEmailError" style="display:none;" class="error message" style="font-size:14px;">                        
                    </div>
                    <div ID="_testInvitationEmailInfo" style="display:none;" class="warning message" style="font-size:14px;">                        
                    </div>
                    <div class="formInput large">
                        <p><ckbx:MultiLanguageLabel ID="_testEmailLabel" runat="server" TextId="/pageText/forms/surveys/invitations/add.aspx/emaillabel" AssociatedControlID="_name" /></p>
                        <asp:TextBox ID="_testEmail" runat="server" Width="500" />
                        <btn:CheckboxButton ID="_sendTestEmail" runat="server" CssClass="ckbxButton roundedCorners border999 shadow999 silverButton" OnClick="SendTestEmailButton_Click" TextID="/pageText/forms/surveys/invitations/add.aspx/sendTestEmailButton" />
                      
                    </div>
                </div>
            </asp:WizardStep>
            <asp:WizardStep ID="FinishStep" runat="server" StepType="Complete" AllowReturn="false">
                <asp:Panel ID="_activationWarning" runat="server" Visible="false" CssClass="StatusPanel warning" style="font-size:14px;">
                    <ckbx:MultiLanguageLabel ID="_activationWarningLbl" runat="server" />
                </asp:Panel>
                <div style="height: 495px;">
                    <div class="padding10">
                        <asp:Panel ID="_invitationWarningPanel" runat="server" Visible="false" CssClass="StatusPanel warning">
                            <ckbx:MultiLanguageLabel ID="_invitationWarning" runat="server" />
                        </asp:Panel>
                        <asp:Panel ID="_noRecipientsPanel" runat="server" CssClass="StatusPanel warning" Visible="false">
                            <ckbx:MultiLanguageLabel ID="_noRecipientsLabel" runat="server" TextId="/pageText/forms/surveys/invitations/add.aspx/noRecipients" />
                        </asp:Panel>
                        <ckbx:MultiLanguageLabel ID="_createInvitationError" runat="server" CssClass="error message" Visible="false" />
                        <asp:Panel id="_emailsNotEnoughToInviteWarningPanel" runat="server" CssClass="warning message" Visible="false">
                            <asp:Label ID="_emailsNotEnoughToInviteWarning" runat="server" />
                        </asp:Panel>
                    </div>

                    <div class="padding10" style="text-align: center">
                            <div>
                                <div style="font-size:18px; font-weight: bold; padding-bottom: 20px;">
                                    <ckbx:MultiLanguageLabel ID="_reviewInstructions" runat="server" TextId="/pageText/forms/surveys/invitations/addscheduled.aspx/reviewInstructions" />
                                </div>
                                <div style="font-size:14px; font-weight:bold; padding-bottom:20px;">
                                    <ckbx:MultiLanguageLabel ID="_invitationTypeMessage" runat="server"/>
                                </div>
                                <div>
                                    <ckbx:MultiLanguageLabel ID="_editInvitation" runat="server" />
                                </div>
                            </div>
                    </div>
                    <div class="WizardNavContainer">
                        <hr />
                        <btn:CheckboxButton ID="_editInvitationButton" runat="server" CssClass="ckbxButton roundedCorners border999 shadow999 silverButton right" OnClick="EditInvitationButton_Click" TextID="/pageText/forms/surveys/invitations/add.aspx/editInvitationButton" />
                        <btn:CheckboxButton ID="_closeWizardButton" runat="server" CssClass="cancelButton left" OnClick="CloseWindowButton_Click" TextID="/pageText/forms/surveys/invitations/add.aspx/closeButton" />
                    </div>
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
