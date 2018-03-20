<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="FakeUploader.ascx.cs" Inherits="CheckboxWeb.Controls.FakeUploader" %>

<script type="text/javascript">
    ///
    function onFileUploaded_<%= ClientID %>(fileData) {
        <% if(!string.IsNullOrEmpty(UploadedCallback)) { %>
        <%=UploadedCallback %>(fileData);
        <%} %>
    }

    $(function () {
        $("#file-upload-input_<%= ClientID %>").on('change', function (e) {
            var filename = $(this).val().split('/').pop().split('\\').pop();
            onFileUploaded_<%= ClientID %>(filename);
        });

        //Fix for IE9
        if (getIeVersion() != '9') {
            $("#fileinput-button_<%= ClientID %>").on('click', function () {
                $("#file-upload-input_<%= ClientID %>").click();
                return false;
            });
        }
    });
</script>

<div id="fileupload_<%= ClientID %>">
    <div class="fileupload-buttonbar" style="border-width:0;border-style:none;background-color:transparent;">
        <label id="fileinput-button_<%= ClientID %>" class="silverButton ckbxButton" for="file-upload-input_<%= ClientID %>">
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
