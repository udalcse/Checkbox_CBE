<%@ Page Language="C#" MasterPageFile="~/Dialog.Master" AutoEventWireup="false" CodeBehind="OptOutPreferences.aspx.cs" Inherits="CheckboxWeb.Settings.OptOutPreferences" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>

<asp:Content ID="settingContent" ContentPlaceHolderID="_pageContent" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            $('textarea.tyneMceEditor').tinymce({
            // Location of TinyMCE script
                script_url: '<%=ResolveUrl("~/Resources/tiny_mce/tinymce.min.js")%>',
                height: 175,
                relative_urls: false,
                remove_script_host: false,
                remove_redundant_brs: true,

                plugins: [
                    "image charmap textcolor code upload link"
                ],
                toolbar1: "bold italic underline strikethrough superscript subscript | bullist numlist link | image upload | charmap code  | forecolor backcolor  | styleselect fontselect fontsizeselect ",
                menubar: false,

                // Example content CSS (should be your site CSS)
                //content_css: '',

                // Drop lists for link/image/media/template dialogs
                //template_external_list_url: 'lists/template_list.js',
                external_link_list_url: '<%=ResolveUrl("~/ContentList.aspx?contentType=documents")%>',
                external_image_list_url: '<%=ResolveUrl("~/ContentList.aspx?contentType=images")%>',
                media_external_list_url: '<%=ResolveUrl("~/ContentList.aspx?contentType=video")%>',
                //Cause contents to be written back to base textbox on change
                onchange_callback: function(ed) { ed.save(); },
                gecko_spellcheck: true,
                //avoid <p></p> wrapping text for good layout on OptOut.aspx, where these texts will be used
                force_p_newlines : false,
                forced_root_block : false,
                init_instance_callback: "tinyMCEInitialized_<%=ClientID%>",
                fontsize_formats: "8px 10px 12px 14px 16px 18px 20px 24px 36px"
            });
        });

        function tinyMCEInitialized_<%=ClientID%>() {
            /*buttons in tinymce editor should ignore uframe*/
            $('#<%=_customTextPanel.ClientID%> *').find(':image,:submit,:button').attr('uframeignore', 'true');
        }
    </script>
    

    <h3><%= WebTextManager.GetText("/pageText/settings/navigation.ascx/optScreen")%></h3>
    
    <asp:Panel runat="server" ID="_customTextPanel">
        <div class="dashStatsWrapper border999 shadow999">
            <div class="dashStatsHeader">
                <span class="mainStats left"><%= WebTextManager.GetText("/pageText/settings/optOutPreferences.aspx/optOutGreetingsTitle")%></span>
            </div>
            <div class="dashStatsContent allMenu">
                <asp:TextBox ID="_greetingsTextbox" runat="server" TextMode="MultiLine" Rows="15" Columns="80" CssClass="tyneMceEditor" tinyMCE="true"></asp:TextBox>
            </div>
        </div>
        <div class="dashStatsWrapper border999 shadow999">
            <div class="dashStatsHeader">
                <span class="mainStats left"><%= WebTextManager.GetText("/pageText/settings/optOutPreferences.aspx/optOutUserReasonsBlockSender")%></span>
            </div>
            <div class="dashStatsContent allMenu">
                <asp:TextBox ID="_blockSenderTextbox" runat="server" TextMode="MultiLine" Rows="15" Columns="80" CssClass="tyneMceEditor" tinyMCE="true"></asp:TextBox>
            </div>
        </div>
        <div class="dashStatsWrapper border999 shadow999">
            <div class="dashStatsHeader">
                <span class="mainStats left"><%= WebTextManager.GetText("/pageText/settings/optOutPreferences.aspx/optOutUserReasonsBlockSurvey")%></span>
            </div>
            <div class="dashStatsContent allMenu">
                <asp:TextBox ID="_blockSurveyTextbox" runat="server" TextMode="MultiLine" Rows="15" Columns="80" CssClass="tyneMceEditor" tinyMCE="true"></asp:TextBox>
            </div>
        </div>
        <div class="dashStatsWrapper border999 shadow999">
            <div class="dashStatsHeader">
                <span class="mainStats left"><%= WebTextManager.GetText("/pageText/settings/optOutPreferences.aspx/optOutUserReasonsIsSpam")%></span>
            </div>
            <div class="dashStatsContent allMenu">
                <asp:TextBox ID="_spamTextbox" runat="server" TextMode="MultiLine" Rows="15" Columns="80" CssClass="tyneMceEditor" tinyMCE="true"></asp:TextBox>
            </div>
        </div>
    
        <div class="dashStatsWrapper border999 shadow999">
            <div class="dashStatsHeader">
                <span class="mainStats left"><%= WebTextManager.GetText("/pageText/settings/optOutPreferences.aspx/optOutThankYouTitle")%></span>
            </div>
            <div class="dashStatsContent allMenu">
                <asp:TextBox ID="_thankYouTextbox" runat="server" TextMode="MultiLine" Rows="15" Columns="80" CssClass="tyneMceEditor" tinyMCE="true"></asp:TextBox>
            </div>
        </div>
    </asp:Panel>

</asp:Content>