<%@ Page Title="" Language="C#" MasterPageFile="~/Dialog.Master" AutoEventWireup="false" CodeBehind="Footer.aspx.cs" Inherits="CheckboxWeb.Settings.Footer" %>
<%@ Import Namespace="Checkbox.Web"%>
<%@ Register src="~/Controls/Piping/PipeControl.ascx" tagname="PipeSelector" tagprefix="pipe" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>


<asp:Content ID="settingContent" ContentPlaceHolderID="_pageContent" runat="server">
    <h3><%= WebTextManager.GetText("/pageText/settings/navigation.ascx/footerSettings")%></h3>

    <ckbx:MultiLanguageCheckBox ID="_enableFooter" TextId="/pageText/settings/footer.aspx/enableFooter" runat="server" />
    
    <script type="text/javascript">
        function checkFooterAvailability() {
            <% if (_enableFooter.Visible) { %>
                if ($('#<%=_enableFooter.ClientID%>').is(':checked'))
                    $('#footerEditorContainer').show();
                else
                    $('#footerEditorContainer').hide();
            <% } else { %>
                $('#footerEditorContainer').show();
            <% } %>
        }

        function tinyMCEInitialized_<%=ClientID%>() {
            /*buttons in tinymce editor should ignore uframe*/
            $('#footerEditorContainer *').find(':image,:submit,:button').attr('uframeignore', 'true');
        }

        $(document).ready(function () {
            $('#<%=_enableFooter.ClientID%>').change(checkFooterAvailability);

            //Firefox hotfix
            try { tinyMCE.remove(); } catch (e) {}

            $('#<%=_footerText.ClientID%>').tinymce({
                // Location of TinyMCE script
                script_url: '<%=ResolveUrl("~/Resources/tiny_mce/tinymce.min.js") %>',
                remove_redundant_brs: true,
                forced_root_block: false,
                relative_urls: false,
                remove_script_host: false,
                width: "600px",
                height: "300px",
                //Cause contents to be written back to base textbox on change
                onchange_callback: function (ed) { ed.save(); },
                gecko_spellcheck: true,
                plugins: [
                    "image charmap textcolor code upload link"
                ],
                toolbar1: "bold italic underline strikethrough superscript subscript | bullist numlist link | image upload | charmap code  | forecolor backcolor  | styleselect fontselect fontsizeselect ",
                menubar: false,
                init_instance_callback: "tinyMCEInitialized_<%=ClientID%>",
                fontsize_formats: "8px 10px 12px 14px 16px 18px 20px 24px 36px"
            });

            checkFooterAvailability();

        });
    </script>
    <div id="footerEditorContainer" style="display:none;">
        <asp:CustomValidator ValidateEmptyText="True" OnServerValidate="Validate" Display="Dynamic" ID="_footerTextValidator" runat="server" ControlToValidate="_footerText" />            
        <asp:TextBox ID="_footerText" CssClass="footerText" runat="server" Rows="20" TextMode="MultiLine" tinyMCE="true"></asp:TextBox>
        <pipe:PipeSelector ID="_pipeSelector" runat="server" />
    </div>
    <div style="padding-bottom:40px;">
    </div>
</asp:Content>
