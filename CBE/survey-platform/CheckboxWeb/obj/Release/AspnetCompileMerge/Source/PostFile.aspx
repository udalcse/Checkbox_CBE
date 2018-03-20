<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="PostFile.aspx.cs" Inherits="CheckboxWeb.PostFile" MasterPageFile="~/Dialog.Master" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>
<%@ Register tagPrefix="ckbx" tagName="Uploader" src="~/Controls/Uploader.ascx" %>

<asp:Content ID="head" runat="server" ContentPlaceHolderID="_headContent">
    <script type="text/javascript" language="javascript">
        //Handle file selection.
        function onFilePosted(fileData) {
            if (fileData != null) {
                $('#<%=_uploadedFileNameTxt.ClientID %>').val(fileData.name);
                $('#<%=_uploadedFilePathTxt.ClientID %>').val(fileData.TempName);
                $('#<%=_fileUploadedBtn.ClientID %>').click();
            }
        }
        
        ///
        function returnToEditor(fileType, contentItemId) {
            parent.ClosePopup($('#previewPlace').html(), fileType, contentItemId);   
        }
    </script>
</asp:Content>
<asp:Content ID="content" runat="server" ContentPlaceHolderID="_pageContent">            
    <div style="display:none">
        <asp:Button ID="_fileUploadedBtn" runat="server" Text="Uploaded" />
        <asp:TextBox ID="_uploadedFileNameTxt" runat="server" />
        <asp:TextBox ID="_uploadedFilePathTxt" runat="server" />
    </div>

    <div>
        <ckbx:Uploader ID="_uploader" runat="server" UploadedCallback="onFilePosted" SelectFilePromptTextId="/controlText/fileUploader/selectFile" />
    </div>
    <div id="previewPlace" style="display:none;">
        <asp:Literal ID="_previewPlace" runat="server" />
    </div>
</asp:Content>
