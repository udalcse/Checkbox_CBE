<%@ Page Language="C#" MasterPageFile="~/Install/Install.Master" AutoEventWireup="false" CodeBehind="Default.aspx.cs" Inherits="CheckboxWeb.Install.Default" %>
<%@ Register src="../Controls/Wizard/WizardNavigator.ascx" tagname="WizardNavigator" tagprefix="nav" %>
<%@ Register src="../Controls/Wizard/WizardButtons.ascx" tagname="WizardButtons" tagprefix="btn" %>
<%@ Register Src="~/Install/Controls/DatabaseSelector.ascx" TagName="DatabaseSelector" TagPrefix="ckbx" %>
<%@ Register Src="~/Install/Controls/SMTPConfigurator.ascx" TagName="SMTPConfigurator" TagPrefix="ckbx" %>
<%@ Register Src="~/Install/Controls/EMailOptions.ascx" TagName="EMailOptions" TagPrefix="ckbx" %>
<%@ Register Src="~/Install/Controls/SLA.ascx" TagName="SLA" TagPrefix="ckbx" %>

<asp:Content ID="script" runat="server" ContentPlaceHolderID="scriptContent">
    <script type="text/javascript">
        <% if (String.Compare(_installWizard.WizardSteps[_installWizard.ActiveStepIndex].ID, "EMailDatabaseStep", StringComparison.InvariantCultureIgnoreCase) == 0) { %>
        function checkEmailMode() {
            var sesChecked = $('#<%=_MSSmodeSES.ClientID%>').is(':checked');
            var smtpChecked = $('#<%=_MSSmodeSMTP.ClientID%>').is(':checked');

            $('#dbDiv').css("display", sesChecked ? "block" : "none");
            $('#smtpDiv').css("display", smtpChecked ? "block" : "none");
            $('#emailDiv').css("display", smtpChecked || sesChecked ? "block" : "none");
            $('#emptyDiv').css("display", !(smtpChecked || sesChecked) ? "block" : "none");

            EnableValidators<%=_emailDBSelector.ClientID %>(sesChecked);
            EnableValidators<%=_SMTPConfigurator.ClientID %>(smtpChecked);
            EnableValidators<%=_EMailOptions.ClientID %>(smtpChecked || sesChecked);
        }


        $(document).ready(function () {
            $('#<%=_MSSNone.ClientID%>').click(checkEmailMode);
            $('#<%=_MSSmodeSMTP.ClientID%>').click(checkEmailMode);
            $('#<%=_MSSmodeSES.ClientID%>').click(checkEmailMode);
            checkEmailMode();
        });
        <% } %>

        <% if (!AppInstaller.IsSlaAccepted && String.Compare(_installWizard.WizardSteps[_installWizard.ActiveStepIndex].ID, "WelcomeStep", StringComparison.InvariantCultureIgnoreCase) == 0) { %>
            $(document).ready(function () {
                showDialog('slaModal', 220, 620);
            });

            function OnSlaDialogClose() {
                $('#<%=_isSlaConfirmed.ClientID%>').val(validateSLA());
                $('form').submit();
            }
        <% } %>


        var nextClientId = 'next';
       
        //submitting on enter key
        $(document).keypress(function(e){
            if(e.keyCode == 13) {
                var existingDbDialog = $('#dbExistsModal a');
                if(existingDbDialog.length > 0) {
                    if(existingDbDialog.is(':visible')) {
                        location.href = existingDbDialog.attr('href');
                    } else {
                        location.href =  $('a[id$=_nextButton]').attr('href');
                    }
                } else {
                    location.href = $('a[id$=_nextButton]').attr('href');
               }
            }
        });

        function resize() {
            $('.rightPanel').height($(window).height() - 57 - 35);
            $('.rightPanel').width($(window).width() - 1);
        }

        function OnDBExistsDialogClose() {
            //we have to turn off all validators in order to enable postback
            ValidatorEnable(document.getElementById('<%=_checkboxDBSelector.ClientID %>__serverTxtValidator'), false);
            ValidatorEnable(document.getElementById('<%=_checkboxDBSelector.ClientID %>__dbNameTxtValidator'), false);
            ValidatorEnable(document.getElementById('<%=_checkboxDBSelector.ClientID %>__usernameTxtValidator'), false);
            ValidatorEnable(document.getElementById('<%=_checkboxDBSelector.ClientID %>__passwordTxtValidator'), false);
            ValidatorEnable(document.getElementById('<%=_checkboxDBSelector.ClientID %>__freeformConnectionStringValidator'), false);
            ValidatorEnable(document.getElementById('<%=_checkboxDBSelector.ClientID %>__trustedServerValidator'), false);
            ValidatorEnable(document.getElementById('<%=_checkboxDBSelector.ClientID %>__trustedDbNameValidator'), false);

            var dbExistValue = $('input[name$="dbExistsOptions"]').filter(':checked').val();
            $('#<%= _dbExistsDialogResult.ClientID%>').val(dbExistValue);

            eval($('#' + nextClientId).attr('href'));
        }

        function OnMailDBExistsDialogClose() {
            //we have to turn off all validators in order to enable postback
            ValidatorEnable(document.getElementById('<%=_emailDBSelector.ClientID %>__serverTxtValidator'), false);
            ValidatorEnable(document.getElementById('<%=_emailDBSelector.ClientID %>__dbNameTxtValidator'), false);
            ValidatorEnable(document.getElementById('<%=_emailDBSelector.ClientID %>__usernameTxtValidator'), false);
            ValidatorEnable(document.getElementById('<%=_emailDBSelector.ClientID %>__passwordTxtValidator'), false);
            ValidatorEnable(document.getElementById('<%=_emailDBSelector.ClientID %>__freeformConnectionStringValidator'), false);
            ValidatorEnable(document.getElementById('<%=_emailDBSelector.ClientID %>__trustedServerValidator'), false);
            ValidatorEnable(document.getElementById('<%=_emailDBSelector.ClientID %>__trustedDbNameValidator'), false);

            var dbExistValue = $('input[name$="emailDbExistsOptions"]').filter(':checked').val();
            $('#<%= _emailDbExistsDialogResult.ClientID%>').val(dbExistValue);

            eval($('#' + nextClientId).attr('href'));
        }

        function openDbExistsDialog() {
            $('#').modal({
                modal: true,
                close: false,
                minHeight: 180,
                maxWidth: 550,
                appendTo: 'form'
            });
        }
    </script>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlace" runat="server">
<div class="rightPanel">
    <asp:Wizard ID="_installWizard" runat="server" DisplaySideBar="false" DisplayCancelButton="false" OnNextButtonClick="InstallWizard_NextButtonClick" OnFinishButtonClick="InstallWizard_FinishButtonClick">
        <StartNavigationTemplate>
            <btn:WizardButtons  NonLocalized="true" ID="_startNavigationButtons" runat="server" />
        </StartNavigationTemplate>
        <WizardSteps>
            <asp:WizardStep ID="WelcomeStep" StepType="Start" runat="server" title="Welcome" meta:resourcekey="WelcomeStep">
                <div class="padding10">
                    <div class="dialogSubTitle sectionTitle">Welcome to Checkbox&reg; 6</div>

                    <div class="label">
                        This installation wizard will walk you through the steps required to <span style="color:#FF5336;">Install Checkbox&reg;Survey 6</span> <!-- or to <span style="color:#f38800;">Upgrade Checkbox&reg; Survey 4</span> -->.  
                    </div>
                    <div class="introPage">
                        <p class="label">To continue, you will need the following information:</p>
                        <p> - File system access to the web server running Checkbox&reg;</p>
                        <p> - Name and server address of database server that will store Checkbox&reg; data.</p>
                        <p> - Administrative credentials to access database server which sufficient permission to create tables, stored procedures, and views.</p>
                        <p> - Internet address (URL) through which users will reach this Checkbox&reg; installation.</p>
                    </div>
                </div>
                <div class="padding10">
                    <div class="dialogSubTitle sectionTitle">Prerequisites</div>
                    <div class="dialogInstructions">
                    In order to install and run Checkbox, the Application Pool hosting Checkbox&reg; must be configured to 
                    run ASP.NET 4.0 and Checkbox&reg; must be able to modify the web.config file.  If the ASP.NET Version and
                    File Permissions checks below report success, click "Next" to continue.
                    </div>
                    <div>&nbsp;</div>
                    <div class="border999 left">
                        <div class="dashStatsContentLarge zebra">
                            <div class="left fixed_175 label">User Context</div>
                            <div class="left fixed_150"><asp:Literal ID="_userContextLabel" runat="server" /></div>
                            <div class="left fixed_75"><div class="success message condensed">Success</div></div>
                            <br class="clear" />
                        </div>

                        <div class="dashStatsContentLarge detailZebra">
                            <div class="left fixed_175 label">ASP.NET Version</div>
                            <div class="left fixed_150"><asp:Literal ID="_aspNetVersionLabel" runat="server" /></div>
                            <div class="left fixed_75">
                                <asp:Panel ID="_successAspNetPanel" runat="server" CssClass="success message condensed" Visible="false">Success</asp:Panel>
                                <asp:Panel ID="_failAspNetPanel" runat="server" CssClass="error message condensed" Visible="false">Checkbox&reg; 6 requires ASP.NET version 4.0 or higher.</asp:Panel>
                            </div>
                            <br class="clear" />
                        </div>
                            
                        <div class="dashStatsContentLarge zebra">
                            <div class="left fixed_175 label">File Permissions</div>
                            <div class="left fixed_150">Check Complete</div>
                            <div class="left fixed_75">
                                <asp:Panel ID="_filePermissionsSuccess" runat="server" CssClass="success message condensed" Visible="false">Success</asp:Panel>
                                <asp:Panel ID="_filePermissionsFail" runat="server" CssClass="error message condensed" Visible="false">Error</asp:Panel>
                            </div>
                            <br class="clear" />
                        </div>
                        <div class="dashStatsContentLarge detailZebra">
                            <div class="left fixed_175 label">Non-Secure Connection</div>
                            <div class="left fixed_150">Check Complete</div>
                            <div class="left fixed_75">
                                <asp:Panel ID="_secureConnectionSuccess" runat="server" CssClass="success message condensed" Visible ="false" >Success</asp:Panel>
                                <asp:Panel ID="_secureConnectionFail" runat="server" CssClass="error message condensed" Visible ="false">Error</asp:Panel>
                            </div>
                            <br class="clear" />
                        </div>
                    </div>
                    <div class="padding5 left">&nbsp;</div>
                    <div class="left fixed_400"><asp:Panel ID="_filePermissionsErrorContainer" runat="server" Visible="false" /></div>
                    <br class="clear" />
                </div>
                <div id="slaModal" style="display:none;">
                    <ckbx:SLA ID="_sla" HideCloseButtonInHeader="True" runat="server" />
                    <asp:HiddenField ID="_isSlaConfirmed" runat="server"/>
                    <a class="ckbxButton roundedCorners border999 shadow999 orangeButton sla-ok" href="javascript:closeWindow(OnSlaDialogClose, null);" style="margin-left:15px;display: none;">Agree and Continue</a>                        
                </div>                
            </asp:WizardStep>
            <asp:WizardStep ID="InstallUpgradeStep" StepType="Step" runat="server" Title="Install Type" AllowReturn="true">
                <div class="padding10 clearfix">
                    <div class="dialogSubTitle sectionTitle">Installation Type</div>
                    <div class="dialogInstructions" style="padding-bottom: 20px;">
                        Please choose whether you are creating a new Checkbox&reg; 6 installation or upgrading an existing Checkbox&reg; 4 installation.
                    </div>
                    <div class="left fixed_200">
                        <p class="formInput condensed label">
                            <asp:RadioButton GroupName="installType" runat="server" ID="_installRad" Text="New Installation" Checked="true" />
                        </p>
                        <p class="formInput condensed label">
                            <asp:RadioButton GroupName="installType" runat="server" ID="_upgradeRad" Text="Upgrade" />
                        </p>
                    </div>
                    <div class="message warning left padding10 fixed_350">
                        <p style="margin: 0; padding: 0 0 10px; border-bottom: 1px solid #ffffff;">
                            Choose <span class="label">New Installation</span> if you would like to create a new Checkbox&reg; installation.
                        </p>
                        <p style="margin: 0; padding: 10px 0 0;">
                            Choose <span class="label">Upgrade</span> if you have an existing Checkbox&reg; 4 installation and wish to upgrade it to version 6.
                        </p>
                    </div>
                </div>
            </asp:WizardStep>
            <asp:WizardStep ID="DatabaseStep" StepType="Step" runat="server" title="Database Info" meta:resourcekey="DatabaseStep" AllowReturn="false">
                <div class="padding10">
                    <div class="dialogSubTitle sectionTitle">Database Connection</div>
                    <div class="dialogInstructions">
                        <asp:Label ID="_adminDBInstructions" runat="server" meta:resourcekey="_adminDBInstructions">This connection information is used by Checkbox Survey to install the required database 
                            objects to the specified database.  The database must exist and be reachable from the web server running Checkbox Survey. The user credentials
                            supplied must have permision to create tables, views, stored procedures, and users.  These user credentials will not be stored by Checkbox and are used
                            only for installation.
                        </asp:Label>
                    </div>
                    <asp:HiddenField ID="_dbExistsDialogResult" runat="server" Value="NotShown" />    
                </div>
                <div class="padding10 clearfix">
                    <ckbx:DatabaseSelector id="_checkboxDBSelector" runat="server" />
                </div>
                <div>
                    <asp:Panel ID="_connectionErrorPanel" runat="server" Visible="false" CssClass="error padding10 message" Style="margin: 20px 10px 0;">
                        <asp:Label ID="_connectionErrorMessage" runat="server"></asp:Label>
                    </asp:Panel>
                    <br class="clear" />
                </div>
                <div id="dbExistsModal" style="display:none;">
                    <div class="dialogInstructions">
                        <asp:Label ID="_instructionsLabel" runat="server" meta:resourcekey="_instructionsLabel">
		                    It appears that Checkbox&reg; Survey has already been installed in the specified database.  Please select an option below
		                    and click the continue button to proceed.
		                </asp:Label>
                    </div>
                    <div class="input">
		                <asp:RadioButtonList ID="_dbExistsOptions" runat="server">
		                    <asp:ListItem Selected="True" Text="Use this database as it is, preserving existing data" Value="UseDB" />
		                    <asp:ListItem Text="Use this database, but overwrite all Checkbox&reg; data" Value="Overwrite" />
		                    <asp:ListItem Text="Change the database server settings" Value="ChangeSettings" />
		                </asp:RadioButtonList>
                    </div>
                    <div>&nbsp;</div>
                    <a class="ckbxButton roundedCorners border999 shadow999 orangeButton" href="javascript:closeWindow(OnDBExistsDialogClose, null);" style="margin-left:75px;">OK</a>
                </div>                
            </asp:WizardStep>
            <asp:WizardStep ID="EMailDatabaseStep" StepType="Step" runat="server" title="Messaging Settings" meta:resourcekey="EMailDatabaseStep" AllowReturn="false">
                <div class="padding10">
                    <div class="left formInput condensed">
                        <asp:RadioButton id="_MSSNone" runat="server" GroupName="mssmode" Text="Set up e-mail system later on the Settings -> Email tab" Checked="true"/>
                    </div>
                    <br class="clear" />
                    <div class="left formInput condensed">
                        <asp:RadioButton id="_MSSmodeSMTP" runat="server" GroupName="mssmode" Text="Send e-mail messages via SMTP Server"/>
                    </div>
                    <br class="clear" />
                    <div class="left formInput condensed">
                        <asp:RadioButton id="_MSSmodeSES" runat="server" GroupName="mssmode" Text="Send e-mail messages via Checkbox Messaging Service"/>
                    </div>
                </div>
                <div>
                    <asp:Panel ID="_connectionEmailErrorPanel" runat="server" Visible="false" CssClass="error padding10 message" Style="margin:20px 10px 0;">
                        <asp:Label ID="_connectionEmailErrorMessage" runat="server"></asp:Label>
                    </asp:Panel>
                    <div class="clear"></div>
                </div>
                <div class="padding10" id="dbDiv" style="display:none">
                    <div class="dialogSubTitle sectionTitle">Checkbox Messaging Service Database Connection</div>
                    <div class="dialogInstructions">
                        <asp:Label ID="Label1" runat="server" meta:resourcekey="_adminEmailDBInstructions">This connection information is used by Checkbox Survey to transfer messages to 
                            Checkbox Messaging Service.  The database must exist and be reachable from the web server running Checkbox Survey. The user credentials
                            supplied must have permision to run stored procedures.  These user credentials will not be stored by Checkbox and are used
                            only for installation.
                        </asp:Label>
                    </div>
                    <asp:HiddenField ID="_emailDbExistsDialogResult" runat="server" Value="NotShown" />    
                    <ckbx:DatabaseSelector id="_emailDBSelector" runat="server" />
                    <div class="clear"></div>
                </div>
                <div class="padding10" id="emailDiv" style="display:none">
                    <div class="dialogSubTitle sectionTitle">Email options</div>
                    <div class="padding10">
                        <ckbx:EMailOptions id="_EMailOptions" runat="server" />
                    </div>
                    <div class="clear"></div>
                </div>
                <div class="padding10" id="smtpDiv" style="display:none">
                    <div class="dialogSubTitle sectionTitle">SMTP Settings</div>
                    <ckbx:SMTPConfigurator id="_SMTPConfigurator" runat="server" />
                    <div class="clear"></div>
                </div>
                <div id="mailDbExistsModal" style="display:none;">
                    <div class="dialogInstructions">
                        <asp:Label runat="server" meta:resourcekey="_instructionsLabel">
		                    It appears that Checkbox&reg; Survey has already been installed in the specified database.  Please select an option below
		                    and click the continue button to proceed.
		                </asp:Label>
                    </div>
                    <div class="input">
		                <asp:RadioButtonList ID="_emailDbExistsOptions" runat="server">
		                    <asp:ListItem Selected="True" Text="Use this database as it is, preserving existing data" Value="UseDB" />
		                    <asp:ListItem Text="Use this database, but overwrite all Checkbox&reg; data" Value="Overwrite" />
		                    <asp:ListItem Text="Change the database server settings" Value="ChangeSettings" />
		                </asp:RadioButtonList>
                    </div>
                    <div>&nbsp;</div>
                    <a class="ckbxButton roundedCorners border999 shadow999 orangeButton" href="javascript:closeWindow(OnMailDBExistsDialogClose, null);" style="margin-left:75px;">OK</a>
                </div>                
            </asp:WizardStep>
            <asp:WizardStep ID="AdditionalInfo" StepType="Step" runat="server" title="Additional Information" meta:resourcekey="EMailDatabaseStep" AllowReturn="false">
                <div class="padding10">
                    <div class="dialogSubTitle sectionTitle">
                        <asp:Label ID="_applicationAddressTitle" runat="server" meta:resourcekey="_applicationAddressTitle">Application Web Address</asp:Label>
                    </div>
                    <div class="dialogInstructions">
                        <asp:Label ID="_urlInstructions" runat="server" meta:resourcekey="_urlInstructions">
                            Please verify that the application URL and application root values are correct.<br class="clear" />
                            For example, if you will access Checkbox&reg; Survey through this URL: http://myserver.com/CheckboxSurvey, the application
                            URL would be <b>http://myserver.com</b> and the application root would be <b>/CheckboxSurvey</b>.
                        </asp:Label>
                    </div>
                    <div class="formInput">
                        <p><asp:Label ID="_applicationURLLabel" runat="server" AssociatedControlID="_applicationURL" meta:resourcekey="_applicationURLLabel">Application URL</asp:Label></p>
                        <asp:TextBox runat="server" ID="_applicationURL" Size="40"></asp:TextBox>

                        <p><asp:Label ID="_applicationRootLabel" runat="server" AssociatedControlID="_applicationRoot" meta:resourcekey="_applicationRootLabel">Application Root</asp:Label></p>
                        <asp:TextBox runat="server" ID="_applicationRoot" Size="40"></asp:TextBox>
                    </div>
                    <div class="clear"></div>
                </div>
                
                <div class="padding10">
                    <div class="dialogSubTitle sectionTitle">Application Timezone</div>
                    <div class="dialogInstructions">
                        <asp:Label ID="_timezoneTxtLabel" runat="server" meta:resourcekey="_TimezoneTxtLabel" AssociatedControlID="_timeZone">
                            Please select a timezone for your installation. 
                        </asp:Label>
                    </div>
                    <div class="formInput">
                        <ckbx:MultiLanguageDropDownList ID="_timeZone" runat="server">
                            <asp:ListItem Text="Not Specified" Value="None"></asp:ListItem>
                            <asp:ListItem Text="(GMT-12:00) International Date Line West" Value="-12" />
                            <asp:ListItem Text="(GMT-11:00) Midway Islands, Samoan Islands" Value="-11" />
                            <asp:ListItem Text="(GMT-10:00) Hawaii" Value="-10" />
                            <asp:ListItem Text="(GMT-09:00) Alaska" Value="-9" />
                            <asp:ListItem Text="(GMT-08:00) Pacific Time (USA, Canada)" Value="-8" />
                            <asp:ListItem Text="(GMT-07:00) Mountain Time (USA, Canada)" Value="-7" />
                            <asp:ListItem Text="(GMT-06:00) Central time (USA, Canada)" Value="-6" />
                            <asp:ListItem Text="(GMT-05:00) Eastern time (USA, Canada)" Value="-5" />
                            <asp:ListItem Text="(GMT-04:30) Caracas" Value="-4.5" />
                            <asp:ListItem Text="(GMT-04:00) Atlantic Time (Canada)" Value="-4" />
                            <asp:ListItem Text="(GMT-03:30) Newfoundland" Value="-3.5" />
                            <asp:ListItem Text="(GMT-03:00) Brasilia" Value="-3" />
                            <asp:ListItem Text="(GMT-02:00) Mid-Atlantic" Value="-2" />
                            <asp:ListItem Text="(GMT-01:00) Azores" Value="-1" />
                            <asp:ListItem Text="(GMT) Greenwich Mean Time: Dublin, Edinburgh, Lisbon, London" Value="0" />
                            <asp:ListItem Text="(GMT+01:00) Amsterdam, Berlin, Bern, Rom, Stockholm, Vienna" Value="1" />
                            <asp:ListItem Text="(GMT+02:00) Athens, Istanbul, Minsk" Value="2" />
                            <asp:ListItem Text="(GMT+03:00) Kaliningrad, Kuwait, Nairobi" Value="3" />
                            <asp:ListItem Text="(GMT+03:30) Teheran" Value="3.5" />
                            <asp:ListItem Text="(GMT+04:00) Moscow, St. Petersburg, Abu Dhabi, Muscat" Value="4" />
                            <asp:ListItem Text="(GMT+04:30) Kabul" Value="4.5" />
                            <asp:ListItem Text="(GMT+05:00) Islamabad, Karachi, Tasjkent" Value="5" />
                            <asp:ListItem Text="(GMT+05:30) Kolkata, Chennai, Mumbai, New Delhi" Value="5.5" />
                            <asp:ListItem Text="(GMT+05:45) Kathmandu" Value="5.75" />
                            <asp:ListItem Text="(GMT+06:00) Astana, Dhaka" Value="6" />
                            <asp:ListItem Text="(GMT+06:30) Rangoon" Value="6.5" />
                            <asp:ListItem Text="(GMT+07:00) Bangkok, Hanoi, Jakarta" Value="7" />
                            <asp:ListItem Text="(GMT+08:00) Beijing, Chongqing, Hong Kong, Urumqi" Value="8" />
                            <asp:ListItem Text="(GMT+09:00) Osaka, Sapporo, Tokyo" Value="9" />
                            <asp:ListItem Text="(GMT+09:30) Adelaide" Value="9.5" />
                            <asp:ListItem Text="(GMT+10:00) Canberra, Melbourne, Sydney" Value="10" />
                            <asp:ListItem Text="(GMT+11:00) Solomon Islands, New Caledonia" Value="11" />
                            <asp:ListItem Text="(GMT+12:00) Fiji, Magadan, Wellington" Value="12" />
                            <asp:ListItem Text="(GMT+13:00) Nuku'alofa" Value="13" />
                        </ckbx:MultiLanguageDropDownList>
                    </div>
                </div>

                <div class="padding10" id="_adminUserProfileDiv" runat="server">
                    <div class="dialogSubTitle sectionTitle">Admin User Profile</div>
                    <div class="dialogInstructions">
                        <asp:Label ID="_adminUserlabel" runat="server" meta:resourcekey="_adminUserProfile">
                            Please set a user name and password for the System Administrator.
                        </asp:Label>
                    </div>
                    <div class="formInput">
                        <p><asp:Label ID="_adminNameTxtLabel" runat="server" meta:resourcekey="_adminNameTxtLabel" AssociatedControlID="_adminUsernameTxt">Username</asp:Label></p>
                        <asp:TextBox id="_adminUsernameTxt" runat="server" Width="400px" CssClass="left"></asp:TextBox>
                        <asp:RequiredFieldValidator id="_adminUsernameTxtValidator" runat="server" meta:resourcekey="_adminUsernameTxtValidator" CssClass="error left message" Display="Dynamic" ControlToValidate="_adminUsernameTxt" ErrorMessage="Required Field"></asp:RequiredFieldValidator>
                        <br class="clear" />

                        <p><asp:Label ID="_adminPasswordTxtLabel" runat="server" meta:resourcekey="_adminPasswordTxtLabel" AssociatedControlID="_adminPasswordTxt">Password</asp:Label></p>
                        <asp:TextBox id="_adminPasswordTxt" runat="server" Width="400px" CssClass="left" TextMode="Password"></asp:TextBox>
                        <asp:RequiredFieldValidator id="_adminPasswordTxtValidator" runat="server" meta:resourcekey="_adminPasswordTxtValidator" CssClass="error left message" Display="Dynamic" ControlToValidate="_adminPasswordTxt" ErrorMessage="Required Field"></asp:RequiredFieldValidator>
                        <br class="clear" />
                        
                        
                        <p><asp:Label ID="_confirmPasswordTxtLabel" runat="server" meta:resourcekey="_PasswordTxtLabel" AssociatedControlID="_adminPasswordTxt">Re-enter Password</asp:Label></p>
                        <asp:TextBox id="_confirmPasswordTxt" runat="server" Width="400px" CssClass="left" TextMode="Password"></asp:TextBox>
                        <asp:RequiredFieldValidator id="_confirmPasswordTxtValidator" runat="server" meta:resourcekey="_confirmPasswordTxtValidator" CssClass="error left message" Display="Dynamic" ControlToValidate="_confirmPasswordTxt" ErrorMessage="Required Field"></asp:RequiredFieldValidator>
                        <asp:CompareValidator runat="server" ID="_passwordCompareValidator" ControlToValidate="_adminPasswordTxt" CssClass="error left message" ControlToCompare="_confirmPasswordTxt" Text="Passwords don't match" />                        
                        <br class="clear" />
                    </div>
                    <div class="clear"></div>
                </div>    
            </asp:WizardStep>

            <%--
            <asp:WizardStep ID="ConfigStep" StepType="Step" runat="server" Title="Configuration" meta:resourcekey="ConfigStep" AllowReturn="false">
                <asp:Panel ID="_samplePanel" CssClass="InstallPanel" runat="server">
                    <div class="InstallControlPanel">
                        <asp:Label ID="_sampleTitle" runat="server" meta:resourcekey="_sampleTitle">Sample Items</asp:Label><br />
                        <asp:CheckBoxList ID="_sampleList" runat="server" RepeatDirection="Vertical">
                            <asp:ListItem Selected="True" Text="Surveys" Value="Surveys" />
                            <asp:ListItem Selected="True" Text="Style Templates" Value="Styles" />
                            <asp:ListItem Selected="True" Text="Item Libraries" Value="Libraries" />                                        
                        </asp:CheckBoxList>
                    </div>
                    <div class="InstallInstructionsPanel">
                        <asp:Label ID="_sampleInstructions" runat="server" meta:resourcekey="_sampleInstructions">
                        These sample surveys, style templates, and item libraries will help you get started using Checkbox Survey right away
                        </asp:Label>
                    </div>
                    <div class="clear"></div>    
                </asp:Panel>
            </asp:WizardStep> --%>

            <asp:WizardStep ID="FinishStep" StepType="Finish" runat="server" Title="Finish" meta:resourcekey="FinishStep">
                <div class="padding10">
                    <div class="dialogSubTitle sectionTitle">Ready to Install</div>
                    <asp:Panel ID="_upgradeWarningMessage" runat="server" CssClass="error message" Visible="false" Style="font-size:1.3em;margin-bottom:10px;">
                        If you have not yet made a full backup of your database you MUST do so now.<br />
                        The upgrade makes irreversible changes to the Checkbox database.<br /><br />
                        Please make a backup of your data before clicking "Finish".
                    </asp:Panel>
                    <asp:Panel runat="server" ID="_finalInstructionsPnl" CssClass="warning message">
                        <asp:Label ID="_finalInstructionsTxt" runat="server" />
                    </asp:Panel>
                    <div class="dialogInstructions">
                        Checkbox&reg; has gathered all the information it needs.  Click the "Finish" button to <asp:Label runat="server" ID="_upgradeLiteral" CssClass="label" Style="color:#000;text-shadow: 1px 1px 0 #fff;"></asp:Label>.
                    </div>
                    <div class="padding15 fixed_600">
                        For information about additional features of Checkbox, such as Email or Custom Survey Urls, please see the installation
                        documentation for steps to take once the install process is complete.
                    </div>
                </div>
            </asp:WizardStep>
        </WizardSteps>
        <FinishNavigationTemplate>
            <btn:WizardButtons NonLocalized="true" ID="_finishNavigationButtons" runat="server" />
        </FinishNavigationTemplate>
        <HeaderTemplate>
            <nav:WizardNavigator ID="_wizardNavigator" runat="server" AllowForwardNavigation="false" />
        </HeaderTemplate>
        <StepNavigationTemplate>
            <btn:WizardButtons NonLocalized="true" ID="_stepNavigationButtons" runat="server" />
        </StepNavigationTemplate>
    </asp:Wizard>
</div>

<!--Need to be returned to the "install wizard"
<asp:Panel runat="server" Visible="false">
    <asp:Wizard runat="server">
        <WizardSteps>
        </WizardSteps>
    </asp:Wizard>
</asp:Panel>
-->

</asp:Content>

