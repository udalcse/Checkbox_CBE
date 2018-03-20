<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HtmlEditor.aspx.cs" MasterPageFile="~/Dialog.Master" Inherits="CheckboxWeb.Forms.Surveys.HtmlEditor" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>
<%@ Register src="~/Controls/Piping/PipeControl.ascx" tagname="PipeSelector" tagprefix="pipe" %>
<%@ Import Namespace="Checkbox.Web"%>

<asp:Content ContentPlaceHolderID="_headContent" ID="_headContent" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            //fix for hanging font dropdown in tinemce
            $('.mceListBoxMenu').remove();

            $('#<%=_htmlTextbox.ClientID%>').tinymce({
                // Location of TinyMCE script
                script_url: '<%=ResolveUrl("~/Resources/tiny_mce/tinymce.min.js") %>',
                height: 350,
                relative_urls: false,
                remove_redundant_brs: true,
                forced_root_block: false,
                remove_script_host: false,

                // General options
                //encoding: 'xml',
                oninit: function () {
                    <% if(!IsNew && !IsMatrix) {%>
                            $('.mceIframeContainer iframe').height(200);
                    <% } %>
                },
                plugins: [
                    "image charmap textcolor upload link"
                ],
                toolbar1: "bold italic underline strikethrough superscript subscript | bullist numlist link | image upload | charmap  | forecolor backcolor  | styleselect fontselect fontsizeselect ",
                default_link_target: "_blank",
                menubar: false,

                //Cause contents to be written back to base textbox on change
                onchange_callback: function (ed) { ed.save(); },
                gecko_spellcheck: true,
                fontsize_formats: "8px 10px 12px 14px 16px 18px 20px 24px 36px"
            });

            $('#<%=Master.OkClientID %>').attr('oldhref', $('#<%=Master.OkClientID %>').attr('href'));
            $('#<%=Master.OkClientID %>').attr('href', '#');

            $('#<%=Master.OkClientID %>').on('click', function(e)
            {
                $('<div>' + $('#<%=_htmlTextbox.ClientID%>[tinymce="true"]').val() + '</div>').appendTo($('#textHelper'));
                var text = $.trim(($('#textHelper').text()));

                if (!text)
                {   
                    e.stopPropagation();                 
                    $('#emptyTextWarning').show();
                    setTimeout(function(){$('#emptyTextWarning').hide('slow');}, 3000);                
                    return false;
                }
                eval($('#<%=Master.OkClientID %>').attr('oldhref'));
                return true;
            });
        });

        
    </script>
</asp:Content>

<asp:Content ContentPlaceHolderID="_pageContent" ID="_pageContent" runat="server">
    <div style="display:none;" id="textHelper"></div>
    <div class="error centerContent" id="emptyTextWarning" style="display:none"><%=WebTextManager.GetText("/pageText/editMatrixRows.aspx/rowTextIsRequired") %></div>
    <div class="padding10" style="overflow-x: auto;">
        <asp:TextBox ID="_htmlTextbox" runat="server" Rows="15" Columns="75" TextMode="MultiLine" tinyMCE="true"></asp:TextBox>
        <pipe:PipeSelector ID="_pipeSelector" runat="server" />
    </div>
</asp:Content>
