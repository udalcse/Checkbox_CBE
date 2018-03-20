<%@ Page Title="" Language="C#" MasterPageFile="~/Dialog.Master" AutoEventWireup="false" CodeBehind="Email.aspx.cs" Inherits="CheckboxWeb.Settings.Email" %>
<%@ Register Src="~/Install/Controls/DatabaseSelector.ascx" TagName="DatabaseSelector" TagPrefix="ckbx" %>
<%@ Register Src="~/Install/Controls/SMTPConfigurator.ascx" TagName="SMTPConfigurator" TagPrefix="ckbx" %>
<%@ Register Src="~/Install/Controls/EMailOptions.ascx" TagName="EMailOptions" TagPrefix="ckbx" %>
<%@ Import Namespace="Checkbox.Web"%>
<%@ Import Namespace="Checkbox.Management"%>
<%@ MasterType VirtualPath="~/Dialog.Master" %>

<asp:Content ID="settingContent" ContentPlaceHolderID="_pageContent" runat="server">
    <ckbx:ResolvingScriptElement ID="_tabsInclude" runat="server" Source="~/Resources/jquery.ckbxtab.js" />

    <script language="javascript" type="text/javascript">
        function showEmailDBConfirmDialog() {
            showDialog('dbEmailModal', 130, 570);
        }

        function OnEMailDBInstallDialogClose(arg) {
            $('#<%=_emailDBOperationConfirmed.ClientID%>').val(arg);
            if (typeof (UFrameManager) == 'undefined') {
                $("form").submit();
            }
            else {
                UFrameManager.executeASPNETPostback($('#<%=Master.OkClientID %>'), $('#<%=Master.OkClientID %>').attr('href'));
            }
        }
    </script>

    <asp:HiddenField ID="_emailDBOperationConfirmed" runat="server" />
    
    <div id="dbEmailModal" style="display:none;">
        <div class="dialogInstructions">
            <asp:Label ID="_instructionsLabel" runat="server" meta:resourcekey="_instructionsLabel">
		        <%=EMailDBConfirmation%>
		    </asp:Label>
        </div>
        <div>&nbsp;</div>
        <div class="confirmButtonsContainer">
            <div class="buttonWrapperAlignCenter">
                <a class="ckbxButton roundedCorners border999 shadow999 orangeButton" href="javascript:closeWindow(OnEMailDBInstallDialogClose, 'yes');"><%=WebTextManager.GetText("/common/yes") %></a>
                <a class="ckbxButton roundedCorners border999 shadow999 orangeButton" href="javascript:closeWindow(OnEMailDBInstallDialogClose, 'no');"><%=WebTextManager.GetText("/common/no") %></a>
            </div>            
        </div>
    </div> 

<!-- Controls whether or not Checkbox is able to send emails. When this option is disabled it is not possible to send invitations or use survey items which send emails. -->
    <h3><%= WebTextManager.GetText("/pageText/settings/navigation.ascx/email")%></h3>
   
    <div class="dashStatsWrapper border999 shadow999" id="emailEnabledOption" style="display: none;">
        <div class="dashStatsHeader">
            <span class="mainStats left"><%= WebTextManager.GetText("/pageText/settings/email.aspx/emailEnabledTitle")%></span>
        </div>
        <div class="dashStatsContent allMenu">
            <div class="dialogInstructions">
                <ckbx:MultiLanguageLabel ID="dynamicSettings" runat="server" TextId="/pageText/settings/email.aspx/dynamicSettings" />
            </div>
            <div class="input">
                <ckbx:MultiLanguageCheckBox id="_emailEnabled" runat="server" TextId="/pageText/settings/email.aspx/emailEnabled" AutoPostBack="true" />
            </div>
        </div>
    </div>


    <div class="dashStatsWrapper border999 shadow999" id="emailServiceOptions" style="display:none;">
        <div class="dashStatsHeader">
            <span class="mainStats left"><%= WebTextManager.GetText("/pageText/settings/email.aspx/emailServiceOptions")%></span>
        </div>
        <div class="dashStatsContent">
            <div style="width:100%" class="warning" id="_emailServiceUnavailableOption" runat="server">
                <%= WebTextManager.GetText("/pageText/settings/email.aspx/mssNotAvailableWarning")%>
            </div>
            
            <div class="left input">
                <ckbx:MultiLanguageRadioButton id="_MSSmodeSMTP" runat="server" GroupName="mssmode" TextId="/pageText/settings/email.aspx/mssmodeSMTP"/>
            </div>
            <br class="clear" />
           <div class="left input">
                <ckbx:MultiLanguageRadioButton id="_MSSmodeSES" runat="server" GroupName="mssmode" TextId="/pageText/settings/email.aspx/mssmodeSES"/>
            </div>
            <br class="clear" />
        </div>
    </div>

    <div id="smtpSettings" style="display:none">
        <div class="dashStatsWrapper border999 shadow999">
            <div class="dashStatsHeader">
                <span class="left mainStats"><%= WebTextManager.GetText("/pageText/settings/email.aspx/smtpOptions")%></span>
            </div>
            <!-- The address of the mail server that will be delivering the emails sent by Checkbox. -->
            <div class="dashStatsContent allMenu">
                <ckbx:SMTPConfigurator id="_SMTPConfigurator" runat="server"/>
            </div>
        </div> 

    </div>


    <div id="sesSettings"  style="display:none">
        <div class="dashStatsWrapper border999 shadow999">
            <div class="dashStatsHeader">
                <span class="mainStats left"><%= WebTextManager.GetText("/pageText/settings/email.aspx/emailRelayDBConnectionString")%></span>
            </div>
            <!-- The default from address used by Checkbox when sending emails. -->
            <div class="dashStatsContent">
                    <asp:Panel CssClass="error message" runat="server" ID="_emailDBConnectionFailed" Visible="false">
                        <%= WebTextManager.GetText("/pageText/settings/email.aspx/emailDBConnectionFailed")%> <%=MailDBConnectError%>
                    </asp:Panel>
                    <ckbx:DatabaseSelector id="_emailDBSelector" runat="server" HorizontalTabs="true" />
            </div>
        </div>
    </div>

    <div id="commonEmailOptions"  style="display:none">
        <div class="dashStatsWrapper border999 shadow999">
            <div class="dashStatsHeader">
                <span class="mainStats left"><%= WebTextManager.GetText("/pageText/settings/email.aspx/checkboxOptions")%></span>
            </div>
            <!-- The default from address used by Checkbox when sending emails. -->
            <div class="dashStatsContent">
                    <ckbx:EMailOptions id="_emailOptions" runat="server" HorizontalTabs="true" />
            </div>
        </div>
    </div>

    <div class="dialogFormPush">&nbsp;</div>

    <script type="text/javascript" language="javascript">
        var emailEnabled = false;

        function checkMode()
        {
            var smtpEnabled = $("#<%=_MSSmodeSMTP.ClientID%>").prop('checked');
            var sesEnabled = $("#<%=_MSSmodeSES.ClientID%>").prop('checked');
            $("#smtpSettings").css("display", emailEnabled && smtpEnabled ? "block" : "none");
            $("#sesSettings").css("display", emailEnabled && sesEnabled ? "block" : "none");
            EnableValidators<%=_emailDBSelector.ClientID%>(emailEnabled && sesEnabled);
            EnableValidators<%=_SMTPConfigurator.ClientID%>(emailEnabled && smtpEnabled);
        }

        function enablePanels()
        {
            emailEnabled = $("#<%=_emailEnabled.ClientID%>").prop('checked');
            var smtpEnabled = $("#<%=_MSSmodeSMTP.ClientID%>").prop('checked');
            var sesEnabled = $("#<%=_MSSmodeSES.ClientID%>").prop('checked');
            <%if (!ApplicationManager.AppSettings.EnableMultiDatabase) {%>
                $("#emailServiceOptions").css("display", emailEnabled ? "block" : "none");
                $("#emailEnabledOption").css("display", "block");
                $("#smtpSettings").css("display", emailEnabled && smtpEnabled ? "block" : "none");
                $("#sesSettings").css("display", emailEnabled && sesEnabled ? "block" : "none");
                EnableValidators<%=_emailDBSelector.ClientID%>(emailEnabled && sesEnabled);
            <%} else {%>
                EnableValidators<%=_emailDBSelector.ClientID%>(false);
            <%} %>
            $("#commonEmailOptions").css("display", emailEnabled ? "block" : "none");
            $("#<%=_MSSmodeSMTP.ClientID%>").click(function(){checkMode();});
            $("#<%=_MSSmodeSES.ClientID%>").click(function(){checkMode();});
            resizePanels();
        }

        function hideMessages()
        {
            $('#<%=_emailDBConnectionFailed.ClientID%>').hide("slow");
            $('.StatusPanel').hide("slow");
            resizePanels();
        }

        $(document).ready(function () {
            setTimeout(enablePanels, 500);
            setTimeout(hideMessages, 5000);
        });
    </script>
</asp:Content>
