<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="MultiLineText.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.MultiLineText" %>
<%@ Import Namespace="Checkbox.Common" %>
<%@ Register TagPrefix="ckbx" TagName="QuestionText" Src="~/Forms/Surveys/Controls/ItemRenderers/QuestionText.ascx" %>

    <script type="text/javascript">
     
            
      
        $(function() {
            if ($('#<%= _topAndOrLeftPanel.ClientID %>').hasClass('labelRight')) {

                var question = $('#<%= _textContainer.ClientID %>').find('.Question.itemNumber');

                if (question.length > 0) {
                    var margin = -$('#<%= _inputPanel.ClientID %>').width() + 5 + parseInt(question.css('margin-left').replace('px', ''));
                    question.css('margin-left', margin + 'px');
                }
            }
            
            <% if (ExportMode != ExportMode.Pdf)
               { %>

            var intervalId = setInterval(function() {
                var styles = "";
                if (typeof globalTinyMceStyle !== 'undefined') {
                    styles = globalTinyMceStyle;
                }

                var initialWidth = $('textarea#<%= _textInput.ClientID %>').width();

                var toolBarValue = ('<%= Model.Metadata["isHtmlFormattedData"] %>' == 'True')
                    ? "bold italic underline strikethrough superscript subscript | bullist numlist indent outdent link | styleselect fontselect fontsizeselect | table"
                    : false;

                $('textarea#<%= _textInput.ClientID %>').tinymce({
                    // Location of TinyMCE script
                    script_url: '<%= ResolveUrl("~/Resources/tiny_mce/tinymce.min.js") %>',
                    //height: 325,
                    content_style: styles,
                    remove_redundant_brs: true,
                    forced_root_block: false,
                    width: initialWidth + "px",
                    relative_urls: false,
                    remove_script_host: false,

                    //Cause contents to be written back to base textbox on change
                    onchange_callback: function(ed) { ed.save(); },
                    gecko_spellcheck: true,
                    plugins: [
                        "image charmap textcolor code upload link table"
                    ],
                    toolbar: toolBarValue,
                    table_default_attributes: {
                        cellpadding: '5px',
                        border: '1px'
                    },
                    default_link_target: "_blank",
                    <% if (Model.IsSPCArgument)
                       { %>
                    setup: function(ed) {
                        ed.on("blur", function(e) {
                            e.target.save();
                            surveyWorkflow.makePostAction('spc');
                        });
                    },
                    <% } %>
                    menubar: false,
                    fontsize_formats: "8px 10px 12px 14px 16px 18px 20px 24px 36px"
                });

                clearInterval(intervalId);
            }, 1);

            <% }  %>
            
        });
       
    </script>
   

<asp:Panel ID="_containerPanel" runat="server" CssClass="itemContainer">
    <asp:Panel ID="_contentPanel" runat="server" CssClass="itemContent">
        <asp:Panel ID="_topAndOrLeftPanel" runat="server" CssClass="topAndOrLeftContainer">
            <asp:Panel ID="_textContainer" runat="server" CssClass="textContainer">
                <ckbx:QuestionText ID="_questionText" runat="server" />
            </asp:Panel>
        </asp:Panel>

        <asp:Panel ID="_bottomAndOrRightPanel" runat="server" CssClass="bottomAndOrRightContainer">
            <asp:Panel ID="_inputPanel" runat="server" CssClass="inputContainer">
                <div class="validationError" style="color: red; display:none">Answer must contain 2 or more characters.</div>
               <textarea
                    ID="_textInput" 
                    runat="server"  />
            </asp:Panel>
        </asp:Panel>
    </asp:Panel>
    
</asp:Panel>


