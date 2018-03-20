<%@ Page Title="" Language="C#" MasterPageFile="~/Dialog.Master" CodeBehind="InvitationText.aspx.cs" Inherits="CheckboxWeb.Settings.InvitationText" AutoEventWireup="false" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>
<%@ Register src="~/Controls/Piping/PipeControl.ascx" tagname="PipeSelector" tagprefix="pipe" %>

<asp:Content ID="settingContent" ContentPlaceHolderID="_pageContent" runat="server" uframeignore="true">
    
     <script type="text/javascript">
        $(document).ready(function () {
            $("#accordion").accordion({ icons: false, active: false });

            $('.invitation-html-editor').tinymce({
                // Location of TinyMCE script
                script_url: '<%=ResolveUrl("~/Resources/tinymce_4.5.3/tinymce.min.js")%>',
                height: 325,
                relative_urls: false,
                remove_script_host: false,
                remove_redundant_brs: true,
                plugins: [
                 "advlist autolink link upload image lists charmap print preview hr anchor pagebreak spellchecker",
                 "searchreplace visualblocks visualchars code fullscreen insertdatetime media nonbreaking",
                 "save table contextmenu directionality emoticons template paste textcolor imagetools"
                ],
                toolbar: "insertfile undo redo | styleselect | bold italic | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | link | print preview fullpage | forecolor backcolor emoticons |   surveyLink",
                menubar: false,
                external_link_list_url: '<%=ResolveUrl("~/ContentList.aspx?contentType=documents")%>',
                external_image_list_url: '<%=ResolveUrl("~/ContentList.aspx?contentType=images")%>',
                media_external_list_url: '<%=ResolveUrl("~/ContentList.aspx?contentType=video")%>',
                //file_picker_types: 'image',
                //file_browser_callback_types: 'image',
                //apply_source_formatting: true,
                template_replace_values: {
                    username: '<%=HttpContext.Current.User.Identity.Name %>'
                },
                onchange_callback: function (ed) { ed.save(); },
                gecko_spellcheck: true,
                fontsize_formats: "8px 10px 12px 14px 16px 18px 20px 24px 36px",
                init_instance_callback: "tinyMCEInitialized_<%=ClientID%>",
                setup: function (editor) {
                    editor.addButton('surveyLink', {
                        text: 'Insert survey link',
                        icon: false,
                        onclick: function () {
                            var surveyUrlPlaceholder = "###@@SURVEY_URL_PLACEHOLDER__DO_NOT_ERASE";
                            editor.insertContent("<a href=\"" + surveyUrlPlaceholder + "\">Click here</a>");
                        }
                    });

                }
            });

            $("div[id*='_termsBinding']").parent().hide();
        });
       
        function tinyMCEInitialized_<%=ClientID%>() {
            /*buttons in tinymce editor should ignore uframe*/
            $('#<%=_customTextPanel.ClientID%> *').find(':image,:submit,:button').attr('uframeignore', 'true');
        }
    </script>  
    
     <asp:Panel runat="server" ID="_customTextPanel">
    <asp:Panel id="_multiLanguageNotAllowedWarningPanel" runat="server" CssClass="warning message" Visible="false">
        <ckbx:MultiLanguageLabel ID="_multiLanguageNotAllowedWarning" runat="server" TextId="/pageText/settings/welcomeText.aspx/multiLanguageNotAllowed" Text="MultiLanguage support is not allowed by your license."/><br />
    </asp:Panel>

    <h3><%= WebTextManager.GetText("/pageText/settings/navigation.ascx/invitationText")%></h3>
    
    <div class="dashStatsWrapper border999 shadow999">
        <div class="dashStatsHeader">
            <span class="mainStats left">Invitation settings</span>
        </div>
        <div class="dashStatsContent">
            <div class="dialogInstructions">
                  <asp:CheckBox id="_invitationTextEnabled" CssClass="trigger" runat="server" />
                  <ckbx:MultiLanguageLabel ID="dynamicSettings" runat="server" Text="Changing this option will enable the invitation settings below " />
            </div>
        </div>
    </div>

    <div class="dashStatsWrapper border999 shadow999">
        <div class="dashStatsHeader">
            <span class="mainStats left"><%= WebTextManager.GetText("/pageText/settings/invitationText.aspx/editLanguage")%></span>
        </div>
        <div class="dashStatsContent allMenu">
            <div class="fixed_125 left input"><ckbx:MultiLanguageLabel ID="MultiLanguageLabel1" runat="server" TextId="/pageText/settings/invitationText.aspx/editLanguage" /></div>
            <div class="left input">
                <asp:DropDownList ID="_miscTextLanguageList" runat="server" AutoPostBack="true" />
            </div>
            <br class="clear" />
        </div>
    </div> 

    <div class="dashStatsWrapper border999 shadow999" style="height:466px; margin-bottom:40px;">
        <div class="dashStatsHeader">
            <span class="mainStats left"><%= WebTextManager.GetText("/pageText/settings/navigation.ascx/invitationText")%></span>
        </div>
        <div id="accordion">            
            <h5 style="padding:5px;font-weight:bold;">
                <%= WebTextManager.GetText("/pageText/settings/invitationText.aspx/invitationMessage/htmlMessage")%>
            </h5>
            <div style="padding:5px;border-style:solid;border-color:Gray;margin-bottom:5px;">
                <asp:TextBox ID="_invitationHtmlMessage" CssClass="invitation-html-editor" runat="server" Rows="15" Columns="80" TextMode="MultiLine" tinyMCE="true" style="width:645px;height:320px" uframeignore="true" />
              
                  <div class="clear"></div>
                <div id="htmlPipeConteiner" class="right" style="margin-right:10px;" >
                    <pipe:PipeSelector ID="_htmlPipeSelector" runat="server" CausesValidation="false" uframeignore="true" />
                </div>
            </div>

            <h5 style="padding:5px;font-weight:bold;">
                <%= WebTextManager.GetText("/pageText/settings/invitationText.aspx/invitationMessage/textMessage")%>
            </h5>
            <div style="padding:5px;border-style:solid;border-color:Gray;margin-bottom:5px;">
                <asp:TextBox ID="_invitationTextMessage" runat="server" Rows="15" Columns="80" TextMode="MultiLine" style="width:645px;height:320px" uframeignore="true" />
                <div class="clear"></div>
                <div id="textPipeContainer" class="right" style="margin-right:10px;" >
                    <pipe:PipeSelector ID="_textPipeSelector" runat="server" CausesValidation="false" uframeignore="true" />
                </div>
            </div>
            
        </div>    
    </div>
    <div class="dialogFormPush">&nbsp;</div>
      
           
   </asp:Panel>
    
</asp:Content>
