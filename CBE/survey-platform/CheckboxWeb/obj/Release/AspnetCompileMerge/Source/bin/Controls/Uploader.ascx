<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Uploader.ascx.cs" Inherits="CheckboxWeb.Controls.Uploader" %>
<%@ Import Namespace="Checkbox.Web" %>

<ckbx:ResolvingScriptElement runat="server" Source="~/Resources/jquery.iframe-transport.js" />
<ckbx:ResolvingScriptElement runat="server" Source="~/Resources/jquery.fileupload.js" />
<ckbx:ResolvingScriptElement runat="server" Source="~/Resources/jquery.tmpl.min.js" />
<ckbx:ResolvingScriptElement runat="server" Source="~/Resources/globalHelper.js" />
<ckbx:ResolvingScriptElement runat="server" Source="~/Resources/textHelper.js" />



<div id="fileupload_<%= ClientID %>" class="fileupload_<%= ClientID %>">
    <div class="fileupload-buttonbar" style="border-width:0;border-style:none;background-color:transparent;">
        <label <%= LabelRoleForMobile %> id="fileinput-button_<%= ClientID %>" class="silverButton ckbxButton" for="file-upload-input_<%= ClientID %>">
            <span><%= SelectFilePrompt %></span>
        </label>
        <input id="file-upload-input_<%= ClientID %>" type="file" style="display: none; visibility:hidden; width:0;" name="files[]" />
		<!--<div class="fileupload-progressbar"></div>-->
    </div>
    <div class="fileupload-content" style="display:none;">
        <table class="files"></table>
    </div>
    <br class="clear"/>
</div>

<script id="template-upload" type="text/x-jquery-tmpl">
    <tr class="template-upload{{if error}} ui-state-error{{/if}}">
        <td class="preview"></td>
            <td class="name"><%=WebTextManager.GetText("/controlText/fileUploader/uploading") %><b>${textHelper.truncateText(name, <%=MaxUploadingLength%>)}</b></td>
            <td class="size">(${sizef})</td>
        {{if error}}
            <td class="error" colspan="2">Error:
                {{if error === 'maxFileSize'}}File is too big
                {{else error === 'minFileSize'}}File is too small
                {{else error === 'acceptFileTypes'}}Filetype not allowed
                {{else error === 'maxNumberOfFiles'}}Max number of files exceeded
                {{else}}${error}
                {{/if}}
            </td>
        {{else}}
            <td class="progress"><div></div></td>
            <td class="start"><button>Start</button></td>
        {{/if}}
        <!--<td class="cancel"><button>Cancel</button></td>-->
    </tr>
</script>
<script id="template-download" type="text/x-jquery-tmpl">
    <tr class="template-download{{if error}} ui-state-error{{/if}}">
        {{if error}}
            <td></td>
            <td class="name">${textHelper.truncateText(name, <%=MaxDowloadErrorLength%>)}</td>
            <td class="size">${sizef}</td>
            <td class="error" colspan="2">Error:
                {{if error === 1}}File exceeds upload_max_filesize (php.ini directive)
                {{else error === 2}}File exceeds MAX_FILE_SIZE (HTML form directive)
                {{else error === 3}}File was only partially uploaded
                {{else error === 4}}No File was uploaded
                {{else error === 5}}Missing a temporary folder
                {{else error === 6}}Failed to write file to disk
                {{else error === 7}}File upload stopped by extension
                {{else error === 'maxFileSize'}}File is too big
                {{else error === 'minFileSize'}}File is too small
                {{else error === 'acceptFileTypes'}}Filetype not allowed
                {{else error === 'maxNumberOfFiles'}}Max number of files exceeded
                {{else error === 'uploadedBytes'}}Uploaded bytes exceed file size
                {{else error === 'emptyResult'}}Empty file upload result
                {{else}}${error}
                {{/if}}
            </td>
        {{else}}
            <td class="preview">
                {{if thumbnail_url}}
                    <!--<a href="${url}" target="_blank"><img src="${thumbnail_url}"></a>-->
                {{/if}}
            </td>
            <td class="name">
                <a href="${url}"{{if thumbnail_url}} target="_blank"{{/if}}>${textHelper.truncateText(name, <%=MaxDowloadedLength %>)}</a>
            </td>
            <td class="size">(${size})</td>
            <td colspan="2"></td>
        {{/if}}
        <td class="delete">
            <button data-type="${delete_type}" data-url="${delete_url}">Delete</button>
        </td>
    </tr>
</script>



<script type="text/javascript">
    ///
    function onFileUploaded_<%= ClientID %>(fileData) {
       <% if(!string.IsNullOrEmpty(UploadedCallback)) { %>
        <%=UploadedCallback %> (fileData);
        <%} %>
    }

    function onSuccess_<%= ClientID %>(data) {
        if (data != null && data.result != null && data.result.length > 0) {
            if (typeof data.result === "string")
                data.result = $.parseJSON(data.result);

            var dataToShow;
            //IE9 returns in data.result the whole page, so we should parse it
            if (getIeVersion() == 9)
                dataToShow = $.parseJSON(data.result[0].body.innerText)[0];
            else
                dataToShow = data.result;

            <% if (!SurveyMode) { %>
            $('#template-download').tmpl(dataToShow).appendTo('#fileupload_<%= ClientID %> .files').fadeIn(function () {
                // Fix for IE7 and lower:
                $(this).show();
            });
            <% } %>

            if (getIeVersion() == 9) 
                onFileUploaded_<%= ClientID %>(dataToShow);
            else 
                onFileUploaded_<%= ClientID %>(dataToShow[0]);
        }
    }

    $(function () {
        'use strict';

        var theUrl = '<%=UploadHandlerUrl %>';
       
        // Initialize the jQuery File Upload widget:
        $('#fileupload_<%= ClientID %>').fileupload({
            url: theUrl,
            autoUpload: true,
            maxNumberOfFiles: 1,
            async: true,
            cache: false
        })
        .on('fileuploadstart', document, function (e, data) {
            $('#fileupload_<%= ClientID %> .fileupload-buttonbar').hide();
            $('#fileupload_<%= ClientID %> .fileupload-content').show();
        })
        .on('fileuploaddone', document, function (e, data) {
            onSuccess_<%= ClientID %>(data);
        })
        .on('done', document, function (e, data) {
            onSuccess_<%= ClientID %>(data);
        })
        .on('fileuploaddestroy', document, function (e, data) {
            $('#fileupload_<%= ClientID %> .fileupload-content').hide();
            $('#fileupload_<%= ClientID %> .fileupload-buttonbar').show();
        });

        // Open download dialogs via iframes,
        // to prevent aborting current uploads:
        $(document).on('click', '#fileupload_<%= ClientID %> .files a:not([target^=_blank])', function (e) {
            e.preventDefault();
            $('<iframe style="display:none;"></iframe>')
            .attr('src', this.href)
            .appendTo('body');
        });

        $("#fileinput-button_<%= ClientID %>").on('click', function () {
            //Fix for IE9
            if (getIeVersion() != '9') {
                $("#file-upload-input_<%= ClientID %>").click();
                return false;
            }
        });
    });
</script>