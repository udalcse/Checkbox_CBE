<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="QuestionTextEditor.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemEditors.QuestionTextEditor" %>
<%@ Import Namespace="Checkbox.Management" %>
<%@ Register src="~/Controls/Piping/PipeControl.ascx" tagname="PipeSelector" tagprefix="pipe" %>

<script type="text/javascript">
<%if (ApplicationManager.AppSettings.UseHTMLEditor)
  {%>
    $(document).ready(function () {

            $('#<%=_questionText.ClientID%>').tinymce({
            // Location of TinyMCE script
                script_url: '<%=ResolveUrl("~/Resources/tiny_mce/tinymce.min.js")%>',
                height: 246,
                //cleanup_on_startup:true,
                remove_redundant_brs : true,
                forced_root_block : false,
                width: 1035,
                relative_urls: false,
                remove_script_host: false,
                // Drop lists for link/image/media/template dialogs
                //template_external_list_url: 'lists/template_list.js',
                external_link_list_url: '<%=ResolveUrl("~/ContentList.aspx?contentType=documents")%>',
                external_image_list_url: '<%=ResolveUrl("~/ContentList.aspx?contentType=images")%>',
                media_external_list_url: '<%=ResolveUrl("~/ContentList.aspx?contentType=video")%>',
                plugins: [
                    "image charmap textcolor code upload advlist link <%= (IsHorizontalRuleEnabled? "hr" : "") %> table paste"
                ],
                toolbar1: "bold italic underline strikethrough superscript subscript | bullist numlist indent outdent link | <%= (IsHorizontalRuleEnabled? "hr" : "") %> image upload table | charmap code | forecolor backcolor  | styleselect fontselect fontsizeselect ",
                table_default_attributes: {
                    cellpadding: '5px'
                },
                table_default_styles: {
                    width: '50%'
                },
                paste_as_text: true,
                menubar: false,
                init_instance_callback: "tinyMCEInitialized_<%=ClientID%>",
                default_link_target: "_blank",
                //Cause contents to be written back to base textbox on change
                onchange_callback: function(ed) { ed.save(); },
                gecko_spellcheck: true,
                fontsize_formats: "8px 10px 12px 14px 16px 18px 20px 24px 36px"
            });

        //fix for incorrect user input html
        var ok = $('a[id$=_okBtn]');
        ok.attr('data-href', ok.attr('href'));
        ok.removeAttr('href');
        ok.attr('onclick', 'okButtonClick()');
    });
    
    function okButtonClick() {
        var ok = $('a[id$=_okBtn]');        
        ok.click(function(e){
            $('a[id$=_okBtn]').click(function(e){e.preventDefault();});
        });            
                  
        $.each($('.questionText'), function(index, value){
            if($(value).attr('tinymce') != "true") {
                $(value).tinymce().show();
                $(value).attr('tinyMCE', 'true');
                $(value).tinymce().hide();
                $(value).removeAttr('tinyMCE');
            }
        });

        ok.attr('href', ok.attr('data-href'));

        //for radio buttons we need to save option aliases on click
        if ($(".customFieldMode").length > 0) { // if editing item is of RadioButton type

            var optionAliases = {};
            var tableRows = $("table.radioFieldAliases tbody tr");
            for (var i = 0; i < tableRows.length; i++) {
                var optionName = $(tableRows[i]).find(".optionName").text();
                var alias = $(tableRows[i]).find("input[type='text']").val();
                optionAliases[optionName] = alias;
            }

            $.ajax({
                type: "POST",
                url: "Edit.aspx/AddRadioButtonFieldOptionAliases",
                data: JSON.stringify({
                    fieldName: $("select[id*='questionBinding']").find("option:selected")[0].label,
                    itemId: $("#currentItemId").val(),
                    optionAliases: optionAliases
                }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    var radioButtonField = response.d;
                    var radioOptionAliasesTable = $(".customFieldMode .radioFieldAliases tbody");
                    $(radioOptionAliasesTable).html('');
                    for (var i = 0; i < radioButtonField.Options.length; i++) {
                        var optionName = radioButtonField.Options[i].Name
                        $(radioOptionAliasesTable).append('<tr><td id="' + optionName + '">' + optionName + '</td><td><input type="text" class=" left uniform-input text"/></td></tr>');
                    }
                    $(".customFieldMode").css("display", "block");
                    $(".normalMode").css("display", "none");

                },
                error: function (xhr) {
                    console.log(xhr);
                }
            });
        }
    }

    // Show/Hide tinyMCE editor - hack for IE9
    function changeTinymceVisibility(textareaId, visible) {
        var editorsCount = 0;
        $.each(tinymce.editors, function(i, ed) {
                if (ed.id == textareaId && editorsCount < 1) {
                    if (visible) {
                        ed.show();
                    }
                    else {
                        ed.hide();
                    }
                    editorsCount++;
                }
            });
    }
    
    //TinyMCE loaded callback
    function tinyMCEInitialized_<%=ClientID%>(){ 
        $('.mce-tinymce').find(':image,:submit,:button').attr('uframeignore', 'true');
        if ($("#<%=_currentEditMode.ClientID%>").val() == "HTML"){
            $("#<%=_questionText.ClientID%>").unbind('change');
            changeTinymceVisibility('<%= _questionText.ClientID%>', true);
            $('#<%=_questionText.ClientID%>').attr('tinyMCE', 'true');
        }            
        else{
            changeTinymceVisibility('<%= _questionText.ClientID%>', false);
            $('#<%=_questionText.ClientID%>').removeAttr('tinyMCE');
            $("#<%=_questionText.ClientID%>").unbind('change');
            $("#<%=_questionText.ClientID%>").change(function(e){     
                    if (typeof(e.srcElement) == 'undefined')
                        $('#<%=_questionText.ClientID%>').val(e.target.value);
                    else
                        $('#<%=_questionText.ClientID%>').val(e.srcElement.value);
            });
        
        }
    }

    //
    function onHtmlClick_<%=ClientID%>() {
        if ($("#<%=_currentEditMode.ClientID%>").val() == "Textarea")
        {
            $("#<%=_questionText.ClientID%>").unbind('change');
            changeTinymceVisibility('<%= _questionText.ClientID%>', true);
            $('#<%=_questionText.ClientID%>').attr('tinyMCE', 'true');
            $("#<%=_currentEditMode.ClientID%>").val("HTML");
        }
    }

    //
    function onTextClick_<%=ClientID%>() {
        if ($("#<%=_currentEditMode.ClientID%>").val() == "HTML")
        {
            changeTinymceVisibility('<%= _questionText.ClientID%>', false);
            $('#<%=_questionText.ClientID%>').removeAttr('tinyMCE');
            $("#<%=_currentEditMode.ClientID%>").val("Textarea");
            $("#<%=_questionText.ClientID%>").unbind('change');
            $("#<%=_questionText.ClientID%>").change(function(e){     
                    if (typeof(e.srcElement) == 'undefined')
                        $('#<%=_questionText.ClientID%>').val(e.target.value);
                    else
                        $('#<%=_questionText.ClientID%>').val(e.srcElement.value);
            });
        }
    }

    <%
  }%>
</script>

<div style="display:none">
    <asp:TextBox ID="_currentEditMode" runat="server"></asp:TextBox>
</div>

<div class="padding10" style="overflow-x: auto;">
    <asp:TextBox ID="_questionText" CssClass="questionText" runat="server" Rows="15" Columns="80" TextMode="MultiLine" tinyMCE="true"></asp:TextBox>
    <pipe:PipeSelector ID="_pipeSelector" runat="server" />
</div>