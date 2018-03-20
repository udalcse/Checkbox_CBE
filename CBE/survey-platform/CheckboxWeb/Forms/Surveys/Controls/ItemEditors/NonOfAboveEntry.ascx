<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="NonOfAboveEntry.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemEditors.NonOfAboveEntry" %>
<%@ Register TagPrefix="pipe" TagName="PipeSelector" Src="~/Controls/Piping/PipeControl.ascx" %>

<script type="text/javascript">
    function isHtml(text) {
        return text.indexOf('html-wrapper') > 0 || text.indexOf('<p>') == 0;
    }
    function isEncoded(text) {
        return text.indexOf("&lt;") >= 0 || text.indexOf("&gt") >= 0;
    }

    $(function () {
        $('#<%= _optionLabelPreview.ClientID %>').bind('click', onPreviewEditClick_<%=ClientID%>).html('<%= !string.IsNullOrEmpty(OptionLabelContent) ? OptionLabelContent.Replace("'", "&#39;") : string.Empty %>');
        $('#<%= EditOptionHtmlLinkClientId %>').bind('click', onOptionEditHtmlClick_<%= ClientID %>);

        $('#<%=_updateOptionButton.ClientID %>').attr('href', 'javascript:void(0);');
        $('#<%=_cancelOptionButton.ClientID %>').attr('href', 'javascript:void(0);');

        <% if (AllowOption) { %>
            $('#<%= _optionLabelPreview.ClientID %>').addClass('simple');
        <% } %>

        $('#<%=_updateOptionButton.ClientID %>').on('click', function(e) {
            e.preventDefault();
            e.stopPropagation();
            
            var optionTxt = $("#<%= _labelTxt.ClientID %>").val();
            if (isHtml(optionTxt) && optionTxt.indexOf("&lt;") == 0) {
                optionTxt = htmlDecode(optionTxt);
            }

            optionTxt = stripScripts(optionTxt);
            $('#<%= _labelTxt.ClientID %>').val(optionTxt);
            
            if (!isHtml(optionTxt)) {
                $('#<%= _optionLabelPreview.ClientID %>').html(htmlEncode(optionTxt));
            }

            hideEditControls_<%=ClientID%>(true);
            return;
        });

        $('#<%=_cancelOptionButton.ClientID %>').on('click', function(e) {
            e.preventDefault();
            e.stopPropagation();

            hideEditControls_<%=ClientID%>(true);
            return;
        });

        <% if (!AllowOption) { %>
            $('#<%= EditOptionHtmlLinkClientId %>').hide();
        <% } %>
        
        <%if(DisableHtmlFormattedOption) {%>
        $('#<%= EditOptionHtmlLinkClientId %>').detach();
        <% } %>
    });

    function <%= AllowOptionCheckedChangedHandlerName %>(isChecked) {
        if (isChecked) {
            $('#<%= EditOptionHtmlLinkClientId %>').show();
            $('#<%= _optionLabelPreview.ClientID %>').addClass('simple');
        } else {
            $('#<%= EditOptionHtmlLinkClientId %>').hide();
            $('#<%= _optionLabelPreview.ClientID %>').removeClass('simple');
            hideEditControls_<%=ClientID%>(false);
        }
    }

    //
    function hideEditControls_<%=ClientID%>(showHtmlLink){
        //Replace the buttons
        var updateButton = $('#<%=_updateOptionButton.ClientID %>').detach();
        var cancelButton = $('#<%=_cancelOptionButton.ClientID %>').detach();
        
        updateButton.appendTo('#<%=_otherOptionhiddenControls.ClientID%>');
        cancelButton.appendTo('#<%=_otherOptionhiddenControls.ClientID%>');

        $('#<%= _pipeSelectorContainer.ClientID %>').hide();
            
        $('#<%= _labelTxt.ClientID %>, #<%= _pipeSelectorContainer.ClientID %>').hide();
        $('#<%= _optionLabelPreview.ClientID %>').show();
        
        if(showHtmlLink) {
            $('#<%= EditOptionHtmlLinkClientId %>').show();
        }
    }
    
    function onOptionHtmlDialogClosed_<%=ClientID%>(args) {
        $('#<%= _optionLabelPreview.ClientID %>').html(htmlDecode(args.html));
        $('#<%= _labelTxt.ClientID %>').val(args.html);
    }

    function onPreviewEditClick_<%=ClientID%>(e) {
        e.stopPropagation();

        if($('#<%= ToggleOptionControlClientId %>:checked').length == 0)
            return;

        var content = $('#<%= _optionLabelPreview.ClientID %>').html();

        if (isHtml(content)) {
            onOptionEditHtmlClick_<%= ClientID %>(e);
        } else {
            var updateButton = $('#<%=_updateOptionButton.ClientID %>').detach();
            var cancelButton = $('#<%=_cancelOptionButton.ClientID %>').detach();
            
            updateButton.appendTo('#<%=_editOtherHtmlDiv.ClientID %>');
            cancelButton.appendTo('#<%=_editOtherHtmlDiv.ClientID %>');

            $('#<%= _optionLabelPreview.ClientID %>, #<%= EditOptionHtmlLinkClientId %>').hide();
            $('#<%= _labelTxt.ClientID %>, #<%= _pipeSelectorContainer.ClientID %>').show();
        }
    }

    function onOptionEditHtmlClick_<%= ClientID %>(e) {
        e.stopPropagation();

        var html = $('#<%= _optionLabelPreview.ClientID %>').html().replace(/#/g, '%23').replace(/&/g, '%26');
        <% if (IsNew || IsMatrix) { %>
            $('#<%= _currentOptionHtml.ClientID %>').val(html);
            window.location = $('#<%= _postForm.ClientID %>').attr('href');
            return;
        <% } %>
        var params = new Array({ name: 'html', value: html }, {name: 'callback', value: 'onOptionHtmlDialogClosed_<%=ClientID%>'});
        if (templateEditor)
            templateEditor.openChildWindow(<%=ItemId %>, <%=PagePosition %>, 'HtmlEditor.aspx', params);
    }
</script>

<%-- Hidden input to store option order  --%>
<asp:Panel CssClass="hidden" runat="server" ID="_otherOptionhiddenControls" >
    <asp:HiddenField ID="_currentOptionHtml" runat="server" />
    <btn:CheckboxButton ID="_updateOptionButton" CssClass="ckbxButton roundedCorners border999 shadow999 orangeButton smallButton OptionEditorButton" runat="server" TextId="/pageText/forms/surveys/itemEditors/optionsNormalEntry.ascx/Update" uframeignore="true" />
    <btn:CheckboxButton ID="_cancelOptionButton" CssClass="ckbxButton roundedCorners border999 shadow999 redButton smallButton OptionEditorButton" runat="server" TextId="/pageText/forms/surveys/itemEditors/optionsNormalEntry.ascx/Cancel" uframeignore="true" />
    <btn:CheckboxButton ID="_postForm" runat="server" OnClick="PostFromOnClick" style="display: none;" uframeignore="true" />
</asp:Panel>


    <div class="formInput left fixed_250">
            <p><ckbx:MultiLanguageLabel ID="_lbl" AssociatedControlID="_labelTxt" runat="server"  /></p>
        </div>
       <div class="formInput left">
            <div>
                <asp:TextBox ID="_labelTxt" runat="server" Style="display: none; width: 150px;" />
                <asp:Panel ID="_optionLabelPreview" CssClass="additional-option-preview" runat="server"></asp:Panel> 
            </div>
            <% if (!RestrictHtmlOptions) { %>
            <asp:Panel CssClass="left" id="_editOtherHtmlDiv" runat="server">
                <a id="<%= EditOptionHtmlLinkClientId %>" class="ckbxButton roundedCorners border999 shadow999 orangeButton OptionEditorButton" uframeignore="true">HTML</a>
            </asp:Panel>
            <% } %>
            <br />
            <asp:Panel runat="server" CssClass="left hidden" ID="_pipeSelectorContainer">
                <pipe:PipeSelector ID="_pipeSelectorForOption" runat="server" />
            </asp:Panel>
        </div>

        <br class="clear" />


