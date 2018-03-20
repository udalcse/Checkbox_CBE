<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="MultiLineTextBehavior.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemEditors.MultiLineTextBehavior" %>
<%@ Register src="~/Controls/Piping/PipeControl.ascx" tagname="PipeSelector" tagprefix="pipe" %>

    <script type="text/javascript">
        $(document).ready(function () {    
            $("#<%=_isHtmlFormattedDataChk.ClientID %>").click(function () {
                onIsHtmlFormattedDataChkClick_<%=ID %>();
            });

            $('#<%=_minCharLimit.ClientID %>').numeric({ decimal: false, negative: false });
            $('#<%=_maxCharLimit.ClientID %>').numeric({ decimal: false, negative: false });

            var initialWidth = $('textarea#<%=_defaultHtml.ClientID%>').width();
            var isDefaultReadOnly = 0;
            if(setDefaultReadOnly)
                isDefaultReadOnly = 1;
            
            $('#<%=_defaultHtml.ClientID %>').tinymce({
                // Location of TinyMCE script
                script_url: '<%=ResolveUrl("~/Resources/tiny_mce/tinymce.min.js") %>',
                //height: 325,
                remove_redundant_brs: true,
                forced_root_block: false,
                relative_urls: false,
                remove_script_host: false,
                width: initialWidth + "px",
                //Cause contents to be written back to base textbox on change
                onchange_callback: function (ed) { ed.save(); },
                gecko_spellcheck: true,
                plugins: [
                    "image charmap textcolor code upload link"
                ],
                toolbar1: "bold italic underline strikethrough superscript subscript | bullist numlist link | image upload | charmap code  | forecolor backcolor  | styleselect fontselect fontsizeselect ",
                default_link_target: "_blank",
                menubar: false,
                fontsize_formats: "8px 10px 12px 14px 16px 18px 20px 24px 36px",
                readonly: isDefaultReadOnly
            });

            onIsHtmlFormattedDataChkClick_<%=ID %>();
        });

        function onIsHtmlFormattedDataChkClick_<%=ID %>() {
            if ($("#<%=_isHtmlFormattedDataChk.ClientID %>").parent().hasClass('checked') == false) {
                $('#htmlModeArea').hide();
                $('#textModeArea').show();
            }
            else {
                $('#textModeArea').hide();
                $('#htmlModeArea').show();
            }
        }
        
    </script>


<!-- Required -->
<div class="formInput fixed_250 left">
    <p><ckbx:MultiLanguageLabel AssociatedControlID="_requiredChk" ID="_requiredLbl" runat="server" TextId="/controlText/multiLineTextEditor/answerRequired" /></p>
</div>
<div class="left input">
     <asp:CheckBox ID="_requiredChk" runat="server" />
</div>
<br class="clear"/>

<!-- HTML Formatted Data -->

<div class="formInput fixed_250 left">
    <p><ckbx:MultiLanguageLabel AssociatedControlID="_isHtmlFormattedDataChk" ID="_isHtmlFormattedDataLbl" runat="server" TextId="/controlText/multiLineTextEditor/htmlFormattedData" /></p>
</div>
<div class="left">
    <asp:CheckBox ID="_isHtmlFormattedDataChk" runat="server"/>
</div>
<br class="clear"/>

<!-- Alias -->
<div class="formInput fixed_250 left">
    <p><ckbx:MultiLanguageLabel AssociatedControlID="_aliasText" ID="MultiLanguageLabel1" runat="server" TextId="/controlText/listEditor/alias" /></p>
</div>
<div class="left">
    <asp:TextBox ID="_aliasText" runat="server" />
</div>
<br class="clear" />

<!-- Min char limit -->
<div class="formInput fixed_250 left">
    <p><ckbx:MultiLanguageLabel AssociatedControlID="_aliasText" ID="MultiLanguageLabel2" runat="server" TextId="/controlText/multiLineTextEditor/minCharLimit" /></p>
</div>
<div class="left">
    <asp:TextBox ID="_minCharLimit" MaxLength="9" runat="server" CssClass="fixed_50" />
</div>
<br class="clear" />

<!-- Max char limit -->
<div class="formInput fixed_250 left">
    <p><ckbx:MultiLanguageLabel AssociatedControlID="_aliasText" ID="MultiLanguageLabel3" runat="server" TextId="/controlText/multiLineTextEditor/maxCharLimit" /></p>
</div>
<div class="left">
    <asp:TextBox ID="_maxCharLimit" MaxLength="9" runat="server" CssClass="fixed_50" />
</div>
<br class="clear" />

<!-- Default Text -->
<div class="formInput" >
    <p><ckbx:MultiLanguageLabel AssociatedControlID="_defaultHtml" ID="_defaultValueLbl" runat="server" TextId="/controlText/multiLineTextEditor/defaultText" /></p>
    <div id="htmlModeArea">
        <asp:TextBox ID="_defaultHtml" runat="server" Rows="15" Columns="80" TextMode="MultiLine" tinyMCE="true" Width="500"></asp:TextBox>
        <pipe:PipeSelector ID="_pipeSelectorHtml" runat="server"  uframeignore="true" />
    </div>
    <div id="textModeArea">
        <asp:TextBox ID="_defaultText" runat="server" Rows="15" Columns="80" TextMode="MultiLine" ></asp:TextBox>
        <pipe:PipeSelector ID="_pipeSelectorText" runat="server"  uframeignore="true" />
    </div>
</div>

<br class="clear" />
