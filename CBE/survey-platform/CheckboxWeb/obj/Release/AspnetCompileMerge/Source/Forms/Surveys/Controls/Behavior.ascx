<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Behavior.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.Behavior" %>
<%@ Import Namespace="Checkbox.Web" %>

<script type="text/javascript">
    function toggleResumeOptions(checkboxElem) {
        if (checkboxElem.checked) {
            $('#<%=_resumeOptionsPanel.ClientID %>').removeAttr('disabled');            
            $('#<%=_resumeOptionsLbl.ClientID %>').removeAttr('disabled');
            $('#<%=_showSaveAndExitButton.ClientID %>').removeAttr('disabled');
            $('#<%=_showEmailResumeLinkDialog.ClientID %>').removeAttr('disabled');

            if ($('#<%=_showEmailResumeLinkDialog.ClientID %>').is(':checked')) {
                $('#<%=_resumeLinkFromEmail.ClientID %>').removeAttr('disabled');
                $('#<%=_resumeLinkFromEmailLbl.ClientID %>').removeAttr('disabled');
            }

            //Remove attr 'disabled' from covering <span> tags. It needs for correct handling in IE 
            $('#<%=_resumeOptionsPanel.ClientID %>').find('span').not('#<%=_resumeLinkFromEmailLbl.ClientID %>').removeAttr('disabled');
        }
        else {
            $('#<%=_resumeOptionsPanel.ClientID %>').attr('disabled', 'disabled');            
            $('#<%=_resumeOptionsLbl.ClientID %>').attr('disabled', 'disabled');
            $('#<%=_showSaveAndExitButton.ClientID %>').attr('disabled', 'disabled');
            $('#<%=_showEmailResumeLinkDialog.ClientID %>').attr('disabled', 'disabled');
            $('#<%=_resumeLinkFromEmail.ClientID %>').attr('disabled', 'disabled');
            $('#<%=_resumeLinkFromEmailLbl.ClientID %>').attr('disabled', 'disabled');

            //Add attr 'disabled' to covering <span> tags. It needs for correct handling in IE.
            $('#<%=_resumeOptionsPanel.ClientID %>').find('span').not('#<%=_resumeLinkFromEmailLbl.ClientID %>').attr('disabled', 'disabled');
        }
    }

    function toggleEmailAddress(checkboxElem) {
        if (checkboxElem.checked) {
            $('#<%=_resumeLinkFromEmail.ClientID %>').removeAttr('disabled');
            $('#<%=_resumeLinkFromEmailLbl.ClientID %>').removeAttr('disabled');
        }
        else {
            $('#<%=_resumeLinkFromEmail.ClientID %>').attr('disabled', 'disabled');
            $('#<%=_resumeLinkFromEmailLbl.ClientID %>').attr('disabled', 'disabled');
        }
    }
</script>

<div class="dialogSubTitle"><%=WebTextManager.GetText("/controlText/forms/surveys/behavior.ascx/behaviorOptions") %></div>

<div class="dialogSubContainer">
    <div class="dialogInstructions">
            <ckbx:MultiLanguageLabel ID="_instructionsLbl" runat="server" TextId="/controlText/forms/surveys/behavior.ascx/behaviorOptionsInstructions">
            Use the following options to determine whether respondents are allowed to return to already visited survey pages, leave the survey
            and resume it at a later date, or edit their completed responses to the survey.
        </ckbx:MultiLanguageLabel>
    </div>

    <div class="formInput">
        <div class="left fixed_25 dropDown"><asp:Checkbox ID="_enableBackButton" runat="server" /></div>
        <div class="left">
            <p>
                <label for="<%=_enableBackButton.ClientID %>"><%=WebTextManager.GetText("/controlText/forms/surveys/behavior.ascx/allowBackButton") %></label>
            </p>
        </div>
    </div>
    <br class="clear"/>
    <div class="formInput">
        <div class="left fixed_25 dropDown"><asp:Checkbox ID="_enableEdit" runat="server" /></div>
        <div class="left">
            <p>
                <label for="<%=_enableEdit.ClientID %>"><%=WebTextManager.GetText("/controlText/forms/surveys/behavior.ascx/allowEdit")%></label>
            </p>
        </div>
    </div>
    <br class="clear"/>
    <div class="formInput">
        <div class="left fixed_25 dropDown"><asp:Checkbox ID="_allowResume" runat="server" AutoPostBack="true"/></div>
        <div class="left">
            <p>
                <label for="<%=_allowResume.ClientID %>"><%=WebTextManager.GetText("/controlText/forms/surveys/behavior.ascx/allowResume")%></label>
            </p>
        </div>
    </div>
    <br class="clear"/>
    <div class="formInput">
        <div class="left fixed_25 dropDown"><asp:Checkbox ID="_anonymizeResponses" runat="server" /></div>
        <div class="left">
            <p>
                <label for="<%=_anonymizeResponses.ClientID %>"><%=WebTextManager.GetText("/controlText/forms/surveys/behavior.ascx/anonymizeResponses")%></label>
            </p>
        </div>
    </div>
    <br class="clear"/>
    <asp:Panel ID="_resumeOptionsPanel" runat="server">
        <div class="formInput">
            <div class="left fixed_25 dropDown"><asp:Checkbox ID="_showSaveAndExitButton" runat="server" /></div>
            <div class="left">
                <p>
                    <label for="<%=_showSaveAndExitButton.ClientID %>"><%=WebTextManager.GetText("/controlText/forms/surveys/behavior.ascx/showSaveAndExit")%></label>
                </p>
            </div>
        </div>
        <br class="clear"/>
    </asp:Panel>

    <!--<fieldset style="border-width:2px;width:400px;padding:3px;">
        <legend><ckbx:MultiLanguageLabel ID="_resumeOptionsLbl" runat="server" TextId="/controlText/forms/surveys/behavior.ascx/resumeOptions" Text="Resume Options"></ckbx:MultiLanguageLabel></legend>
        
        <br />
        <ckbx:MultiLanguageCheckBox ID="_showEmailResumeLinkDialog" runat="server" TextId="/controlText/forms/surveys/behavior.ascx/showEmailResumeLink" Text="Send an email with resume survey link when respondent uses &quot;Save and Exit&quot; Button"  OnClick="toggleEmailAddress(this);" />
        <br /><br />
        <ckbx:MultiLanguageLabel ID="_resumeLinkFromEmailLbl" runat="server"  TextId="/controlText/forms/surveys/behavior.ascx/resumeSurveyLinkFromEmail">From address to use for email message containing resume survey link</ckbx:MultiLanguageLabel>
        <br />
        <asp:TextBox ID="_resumeLinkFromEmail" runat="server" Width="300" />
    </fieldset>-->
</div>