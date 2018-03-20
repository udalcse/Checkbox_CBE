<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="OtherOptionEntry.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemEditors.OtherOptionEntry" %>
<%@ Register TagPrefix="pipe" TagName="PipeSelector" Src="~/Controls/Piping/PipeControl.ascx" %>

<script type="text/javascript">
    function isHtml(text) {
        return text.indexOf('html-wrapper') > 0 || text.indexOf('<p>') == 0;
    }
    function isEncoded(text) {
        return text.indexOf("&lt;") >= 0 || text.indexOf("&gt") >= 0;
    }

    $(function () {
        $('#otherLabelPreview').bind('click', onPreviewEditClick).html('<%= !string.IsNullOrEmpty(OtherLabelContent) ? OtherLabelContent.Replace("'", "&#39;") : string.Empty %>');
        $('#editOtherHtmlLink').bind('click', onOtherOptionEditHtmlClick);

        $('#<%=_updateOptionButton.ClientID %>').attr('href', 'javascript:void(0);');
        $('#<%=_cancelOptionButton.ClientID %>').attr('href', 'javascript:void(0);');

        if (isOtherOptionsEnabled)
            $('#otherLabelPreview').addClass('simple');

        $('#<%=_updateOptionButton.ClientID %>').on('click', function(e) {
            e.preventDefault();
            e.stopPropagation();
            
            var optionTxt = $("#<%= _otherLabelTxt.ClientID %>").val();
            if (isHtml(optionTxt) && optionTxt.indexOf("&lt;") == 0) {
                optionTxt = htmlDecode(optionTxt);
            }

            optionTxt = stripScripts(optionTxt);
            $('#<%= _otherLabelTxt.ClientID %>').val(optionTxt);
            
            if (!isHtml(optionTxt)) {
                $('#otherLabelPreview').html(htmlEncode(optionTxt));
            }

            hideEditControls(true);
            return;
        });

        $('#<%=_cancelOptionButton.ClientID %>').on('click', function(e) {
            e.preventDefault();
            e.stopPropagation();

            hideEditControls(true);
            return;
        });

        if ('<%= AllowOtherButton.ToString().ToLower() %>' == 'false') {
            $('#editOtherHtmlLink').hide();
        }
        
        <%if(DisableHtmlFormattedOtherOption) {%>
            $('#editOtherHtmlLink').detach();
        <% } %>
    });

    function allowOtherCheckedChanged(isChecked) {
        if (isChecked) {
            $('#editOtherHtmlLink').show();
            $('#otherLabelPreview').addClass('simple');
        } else {
            $('#editOtherHtmlLink').hide();
            $('#otherLabelPreview').removeClass('simple');
            hideEditControls(false);
        }
    }

    //
    function hideEditControls(showHtmlLink){
        //Replace the buttons
        var updateButton = $('#<%=_updateOptionButton.ClientID %>').detach();
        var cancelButton = $('#<%=_cancelOptionButton.ClientID %>').detach();
        
        updateButton.appendTo('#otherOptionhiddenControls');
        cancelButton.appendTo('#otherOptionhiddenControls');

        $('#pipeSelectorContainer').hide();
            
        $('#<%= _otherLabelTxt.ClientID %>, #pipeSelectorContainer').hide();
        $('#otherLabelPreview').show();
        
        if(showHtmlLink) {
            $('#editOtherHtmlLink').show();
        }
    }
    
    function onOtherOptionHtmlDialogClosed(args) {
        $('#otherLabelPreview').html(htmlDecode(args.html));
        $('#<%= _otherLabelTxt.ClientID %>').val(args.html);
    }

    function onPreviewEditClick(e) {
        e.stopPropagation();

        if(!isOtherOptionsEnabled)
            return;

        var content = $('#otherLabelPreview').html();

        if (isHtml(content)) {
            onOtherOptionEditHtmlClick(e);
        } else {
            var updateButton = $('#<%=_updateOptionButton.ClientID %>').detach();
            var cancelButton = $('#<%=_cancelOptionButton.ClientID %>').detach();
            
            updateButton.appendTo('#editOtherHtmlDiv');
            cancelButton.appendTo('#editOtherHtmlDiv');

            $('#otherLabelPreview, #editOtherHtmlLink').hide();
            $('#<%= _otherLabelTxt.ClientID %>, #pipeSelectorContainer').show();
        }
    }

    function onOtherOptionEditHtmlClick(e) {
        e.stopPropagation();

        var html = $('#otherLabelPreview').html().replace(/#/g, '%23').replace(/&/g, '%26');
        <% if (IsNew) { %>
            $('#<%= _currentOtherHtml.ClientID %>').val(html);
            window.location = "javascript:__doPostBack('HTML_EDITOR_CALLED_FOR_OTHER', '')";
            return;
        <% } %>
        var params = new Array({ name: 'html', value: html }, {name: 'callback', value: 'onOtherOptionHtmlDialogClosed'});
        templateEditor.openChildWindow(<%=ItemId %>, <%=PagePosition %>, 'HtmlEditor.aspx', params);
    }
</script>

<%-- Hidden input to store option order  --%>
<div id="otherOptionhiddenControls" style="display: none;">
    <asp:HiddenField ID="_currentOtherHtml" runat="server" />
    <btn:CheckboxButton ID="_updateOptionButton" CssClass="ckbxButton roundedCorners border999 shadow999 orangeButton smallButton OptionEditorButton" runat="server" TextId="/pageText/forms/surveys/itemEditors/optionsNormalEntry.ascx/Update" uframeignore="true" />
    <btn:CheckboxButton ID="_cancelOptionButton" CssClass="ckbxButton roundedCorners border999 shadow999 redButton smallButton OptionEditorButton" runat="server" TextId="/pageText/forms/surveys/itemEditors/optionsNormalEntry.ascx/Cancel" uframeignore="true" />
</div>


    <div class="formInput left fixed_250">
            <p><ckbx:MultiLanguageLabel ID="_otherOptionsLbl" AssociatedControlID="_otherLabelTxt" runat="server" TextId="/controlText/allowOtherControl/defaultOtherText" /></p>
        </div>
       <div class="formInput left">
            <div>
                <asp:TextBox ID="_otherLabelTxt" runat="server" Style="display: none; width: 150px;" />
                <div id="otherLabelPreview" class="additional-option-preview"></div>
            </div>
            <% if (!RestrictHtmlOptions) { %>
            <div class="left" id="editOtherHtmlDiv">
                <a id="editOtherHtmlLink" class="ckbxButton roundedCorners border999 shadow999 orangeButton OptionEditorButton" uframeignore="true">HTML</a>
            </div>
            <% } %>
            <br />
            <div class="left" style="display: none;" id="pipeSelectorContainer">
                <pipe:PipeSelector ID="_pipeSelectorForOther" runat="server" />
            </div>
        </div>

        <br class="clear" />


