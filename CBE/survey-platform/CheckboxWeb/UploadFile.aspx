<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="UploadFile.aspx.cs" Inherits="CheckboxWeb.UploadFile" %>
<%@ Import Namespace="Checkbox.Web" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
    <head runat="server">
        <title>Upload File</title>
        <link id="_favicon" runat="server" rel="icon" href="~/favicon.png" type="image/ico" />
        <ckbx:ResolvingCssElement runat="server" Source="~/Resources/css/smoothness/jquery-ui-1.10.2.custom.css" />
        
        <asp:Literal ID="_jsInclude" runat="server"></asp:Literal>

        <script type="text/javascript">
            $(function () {
                'use strict';
                var theUrl = _appRoot + '/Upload.ashx';

                // Initialize the jQuery File Upload widget:

                // Load existing files:
                $.getJSON($('#fileupload form').attr('action'), function (files) {
                    var fu = $('#fileupload').data('blueimpFileupload');
                    fu._adjustMaxNumberOfFiles(-files.length);
                    fu._renderDownload(files)
                    .appendTo($('#fileupload .files'))
                    .fadeIn(function () {
                        // Fix for IE7 and lower:
                        $(this).show();
                    });
                });

                // Open download dialogs via iframes,
                // to prevent aborting current uploads:
                $(document).on('click', '#fileupload .files a:not([target^=_blank])', function (e) {
                    e.preventDefault();
                    $('<iframe style="display:none;"></iframe>')
                    .attr('src', this.href)
                    .appendTo('body');
                });
            });
        </script>
        
        <%-- Specified script include placeholder --%>
        <asp:PlaceHolder ID="_scriptIncludesPlace" runat="server" />
    </head>
    <body>
        <div id="fileupload">
            <form action="<%=ResolveUrl("~/Upload.ashx")%>" method="POST" enctype="multipart/form-data">
                <div class="fileupload-buttonbar">
                    <label class="fileinput-button">
                        <span>Add files...</span>
                        <input type="file" name="files[]" multiple="multiple" />
                    </label>
                <%--<button type="submit" class="start">Start upload</button>
                    <button type="reset" class="cancel">Cancel upload</button> --%>
                    <button type="button" class="delete">Delete all files</button>
			        <div class="fileupload-progressbar"></div>
                </div>
            </form>
            <div class="fileupload-content">
                <table class="files"></table>
            </div>
        </div>

        <br class="clear" />

        <script id="template-upload" type="text/x-jquery-tmpl">
            <tr class="template-upload{{if error}} ui-state-error{{/if}}">
                <td class="preview"></td>
                <td class="name">${name}</td>
                <td class="size">${sizef}</td>
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
                <td class="cancel"><button>Cancel</button></td>
            </tr>
        </script>
        <script id="template-download" type="text/x-jquery-tmpl">
            <tr class="template-download{{if error}} ui-state-error{{/if}}">
                {{if error}}
                    <td></td>
                    <td class="name">${name}</td>
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
                            <a href="${url}" target="_blank"><img src="${thumbnail_url}"></a>
                        {{/if}}
                    </td>
                    <td class="name">
                        <a href="${url}"{{if thumbnail_url}} target="_blank"{{/if}}>${name}</a>
                    </td>
                    <td class="size">${sizef}</td>
                    <td colspan="2"></td>
                {{/if}}
                <td class="delete">
                    <button data-type="${delete_type}" data-url="${delete_url}">Delete</button>
                </td>
            </tr>
        </script>
        <form id="form1" runat="server">
            <asp:Panel ID="_selectFilePanel" runat="server">
                <div>
                    <asp:FileUpload ID="_fileUpload" runat="server" Width="300" />
                </div>
                <div>
                    <btn:CheckboxButton ID="_uploadBtn" runat="server" CssClass="ckbxButton roundedCorners border999 shadow999 orangeButton" TextID="/controlText/imageItemEditor/upload" />
                </div>
            </asp:Panel>
            <asp:Panel ID="_uploadedPanel" runat="server">
                <div>
                    <%=WebTextManager.GetText("/controlText/imageItemEditor/selectedImage")%>:
                    &nbsp;
                    <asp:Label ID="_fileNameLbl" runat="server" />
                </div>
                <div>
                   <btn:CheckboxButton ID="_uploadNewImageBtn" runat="server" CssClass="ckbxButton roundedCorners border999 shadow999 redButton" TextID="/controlText/imageItemEditor/uploadNewImage" />
                </div>
            </asp:Panel>
        </form>
    </body>
</html>
