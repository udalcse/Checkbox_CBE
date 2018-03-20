<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="EditMessageControl.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Invitations.Controls.EditMessageControl" %>
<%@ Register TagPrefix="pipe" TagName="PipeSelector" Src="~/Controls/Piping/PipeControl.ascx" %>
<%@ Register TagPrefix="status" TagName="statuscontrol" Src="~/Controls/Status/StatusControl.ascx" %>
<%@ Import Namespace="Checkbox.Web" %>


<script type="text/javascript">
    //Show message about empty item before the continuation of the wizard
    function ShowConfirmMessage(message, successCallback, closeCallback) {
        showConfirmDialogWithCallback(message, successCallback, 400, 250, null, null, closeCallback);
    }

    function makeDialogAction(next) {
        $("#<%= _confirmed.ClientID %>").val('true');
        var button = $(next ? '[id*=_nextButton]' : '[id*=_previousButton]');
        if (button.length > 0)
            window.location = button.attr('href');
        else
            window.location = $('[id*=_okBtn]').attr('href');
    }

    //Confirmation approved handler
    function confirmationApproved(next) {
        makeDialogAction(next);
    }

    function moveNext() {
        confirmationApproved(true);
    }

    function moveBack() {
        confirmationApproved(false);
    }

    function stopProcessing() {
        //just do nothing        
    }

    function injectDefaultFooter() {
        insertText($('#<%= _defaultFooter.ClientID %>').val(), false);
        $('#<%=_isFooterInjected.ClientID%>').val('true');
    }

    function injectDefaultUnsubscriptionLink() {
        insertText($('#<%= _optoutLink.ClientID %>').val(), true);
        $('#<%=_isFooterInjected.ClientID%>').val('true');
    }

    function insertText(text, isHtml) {
        var targetField = $("#<%=_messageBodyText.ClientID %>");

        if (targetField.attr('tinyMCE')) {
            var ed = tinymce.get("<%=_messageBodyText.ClientID %>");

            //move caret to the end
            var endId = tinymce.DOM.uniqueId();
            ed.dom.add(ed.getBody(), 'br', {}, '');
            ed.dom.add(ed.getBody(), 'br', {}, '');
            ed.dom.add(ed.getBody(), 'span', { 'id': endId }, '');

            var lastnode = ed.dom.select('span#' + endId);
            ed.selection.select(lastnode[0]);
           // ed.selection.collapse(false);

            ed.execCommand(isHtml ? "mceInsertRawHTML" : "mceInsertContent", false, text);
        } else {
            targetField.val(targetField.val() + '\n' + text);
        }
    }

    function onCompanyProfileClosed(params) {
        if (typeof (params) != 'undefined' && params.success) {
            makeDialogAction(params.moveNext);
        }
    }

    function redirectToCompanyProfileSettings(next, companyProfileId) {
        setTimeout(function () {
            showDialog('<%=ResolveUrl("~/Settings/CompanyProfile.aspx?IsModal=true&onClose=onCompanyProfileClosed&moveNext=") %>' + next + '&profileId=' + companyProfileId, 500, 650, null, '<%= WebTextManager.GetText("/pageText/settings/navigation.ascx/companyProfile")%>');
        }, 500);
    }

    function ShowFooterAndCompanyDialogs(message, next) {
        ShowConfirmMessage(message, function() {
            injectDefaultFooter();
            setTimeout(function() {
                ShowCompanyDialog('<%=WebTextManager.GetText("/controlText/forms/surveys/invitations/controls/InvitationMessageValidator.ascx/companyFieldsAreEmpty") %>', next);
            }, 500);
        }, stopProcessing);
    }

    function ShowCompanyDialog(message, next) {
        ShowConfirmMessage(message, function(){ redirectToCompanyProfileSettings(next); }, stopProcessing);
    }

    $(document).ready(function () {
        $("div[id*='_termsBinding']").parent().hide();
        if ($('#<%=_formatTxt.ClientID %>').val() == 'Html') {
            $('#<%=_messageBodyText.ClientID %>').attr('tinyMCE', 'true');
            $('#<%=_messageBodyText.ClientID %>').tinymce({
                // Location of TinyMCE script
                script_url: '<%=ResolveUrl("~/Resources/tiny_mce/tinymce.min.js") %>',
                relative_urls: true,
                document_base_url: "<%=Page.AppRelativeTemplateSourceDirectory%>",
                remove_script_host: false,
                remove_redundant_brs: true,
                forced_root_block: false,
                height: 200,

                // Example content CSS (should be your site CSS)
                //TODO: Load survey CSS
                //content_css: '<%=ResolveUrl("~/ViewContent.aspx?st=1004") %>',

                // Drop lists for link/image/media/template dialogs
                //template_external_list_url: 'lists/template_list.js',
                external_link_list_url: '<%=ResolveUrl("~/ContentList.aspx?contentType=documents")%>',
                external_image_list_url: '<%=ResolveUrl("~/ContentList.aspx?contentType=images")%>',
                media_external_list_url: '<%=ResolveUrl("~/ContentList.aspx?contentType=video")%>',

                // Replace values for the template plugin
                template_replace_values: {
                    username: '<%=WebUtilities.GetCurrentUserEncodedName() %>'
                },
                gecko_spellcheck: true,
                plugins: [
                "image charmap textcolor code upload link"
                ],
                toolbar1: "bold italic underline strikethrough superscript subscript | bullist numlist link | image upload | charmap code  | forecolor backcolor  | styleselect fontselect fontsizeselect ",
                default_link_target: "_blank",
                menubar: false,
                fontsize_formats: "8px 10px 12px 14px 16px 18px 20px 24px 36px"
            });
        }
    });

    //
    function togglePipeSelector() {
        $('#bodyPipeContainer').toggle();
    }

    //
    function onHtmlClick() {
        $('#<%=_messageBodyText.ClientID %>').tinymce().show();
        $('#<%=_messageBodyText.ClientID %>').attr('tinyMCE', 'true');
        $('#<%=_formatTxt.ClientID %>').val('Html');

    }

    //
    function onTextClick() {
        $('#<%=_messageBodyText.ClientID %>').tinymce().hide();
        $('#<%=_messageBodyText.ClientID %>').removeAttr('tinyMCE');
        $('#<%=_formatTxt.ClientID %>').val('Text');
    }

    function copyTextFromInvitation() {
        if ($('#<%=_formatTxt.ClientID %>').val() == 'Html') {
            $('#<%=_messageBodyText.ClientID %>').tinymce().setContent($('#<%=_invitationText.ClientID %>').html(), { format: 'raw' }); 
        }
        else {
            $('#<%=_messageBodyText.ClientID %>').val($('#<%=_invitationText.ClientID %>').html()); 
        }
    }

</script>
<asp:HiddenField ID="_confirmed" EnableViewState="True" runat="server" />
<asp:HiddenField ID="_defaultFooter" EnableViewState="False" runat="server" />
<asp:HiddenField ID="_optoutLink" EnableViewState="False" runat="server" />

<status:statuscontrol id="_statusControl" runat="server" />

<div style="display:none;">
    <asp:TextBox ID="_formatTxt" runat="server" Text="Text" />
    <asp:HiddenField ID="_isFooterInjected" Value="false" runat="server"/>
</div>
<% if (!HideFromFields)
   {%>


<div class="formInput">
    <div class="fixed_125 left">
        <p><ckbx:MultiLanguageLabel id="_fromLbl" runat="server" AssociatedControlId="_fromTxt" TextId="/controlText/forms/surveys/invitations/controls/messageViewControl.ascx/fromLabel" /></p>
     </div>
    <div class="left"><asp:TextBox ID="_fromTxt" runat="server" Width="275" /></div>
    <div class="left" style="margin-top:8px;">
        <asp:RequiredFieldValidator ID="_fromValidator" runat="server" Display="Dynamic" ControlToValidate="_fromTxt"  CssClass="error message"><%= WebTextManager.GetText("/controlText/forms/surveys/invitations/controls/messageViewControl.ascx/fromMissing")%></asp:RequiredFieldValidator>
        <ckbx:MultilanguageLabel CssClass="error message" runat="server"  ID="_emailFormatErrorLbl" TextId="/pageText/forms/surveys/invitations/add.aspx/fromEmailFormatError" />
    </div>
    <br class="clear" />
</div>

<div class="formInput">
    <div class="fixed_125 left">
        <p><ckbx:MultiLanguageLabel id="_fromNameLbl" runat="server" AssociatedControlId="_fromNameTxt" TextId="/controlText/forms/surveys/invitations/controls/messageViewControl.ascx/fromName" /></p>
     </div>
    <div class="left"><asp:TextBox ID="_fromNameTxt" runat="server" Width="450" /></div>
    <br class="clear" />
</div>

<% }%>

<div class="formInput">
    <div class="fixed_125 left"><p><ckbx:MultiLanguageLabel id="_subjectLbl" runat="server" AssociatedControlId="_subjectTxt" TextId="/controlText/forms/surveys/invitations/controls/messageViewControl.ascx/subjectLabel" /></p></div>
    <div class="left"><asp:TextBox ID="_subjectTxt" runat="server" Width="450" /></div>
    <div class="left"><asp:RequiredFieldValidator ID="_subjectValidator" runat="server" Display="Dynamic" ControlToValidate="_subjectTxt" CssClass="error message"><%= WebTextManager.GetText("/controlText/forms/surveys/invitations/controls/messageViewControl.ascx/subjectMissing")%></asp:RequiredFieldValidator></div>
    <br class="clear" />
</div>

<div class="fixed_115 left">&nbsp;</div>
<div class="left">
    <pipe:PipeSelector ID="_subjectPipeSelector" runat="server" CausesValidation="false" />
</div>

<br class="clear" />
<br class="clear" />

<div class="centerContent">                  
    <div style="width:660px;margin:auto;">
        <asp:TextBox ID="_messageBodyText" runat="server" Rows="12" Columns="80" TextMode="MultiLine"  style="margin-left: 5px;"/>
        
        <div class="left"><a class="ckbxButton orangeButton saveButton" id="_setInvitationText" runat="server" style="margin-top: 2px !important;" href="javascript:copyTextFromInvitation();"><%= WebTextManager.GetText("/controlText/forms/surveys/invitations/controls/messageViewControl.ascx/copyTextFromInvitation")%></a></div>

        <div id="bodyPipeContainer">
            <pipe:PipeSelector ID="_bodyPipeSelector" runat="server" CausesValidation="false" />
        </div>
    </div>
    <br class="clear" />
    <div style="display:none" id="_invitationText" runat="server">
    </div>    
 </div>