<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="ItemImport.ascx.cs" Inherits="CheckboxWeb.Controls.AddItems.ItemImport" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Register tagPrefix="ckbx" tagName="Uploader" src="~/Controls/Uploader.ascx" %>

    <script type="text/javascript" language="javascript">
        //Handle file selection.
        function itemFileUploaded(fileData) {
            if (fileData != null) {
                $('#<%=_uploadedFileNameTxt.ClientID %>').val(fileData.name);
                $('#<%=_uploadedFilePathTxt.ClientID %>').val(fileData.TempName);
            }
        }
    </script>
<div style="padding:10px;">
    
    <div style="display:none;">
        <asp:TextBox ID="_uploadedFileNameTxt" runat="server" />
        <asp:TextBox ID="_uploadedFilePathTxt" runat="server" />
        <asp:HiddenField id="_parsedNameTxt" runat="server" />
    </div>

    <div class="dialogInstructions">
        <%=WebTextManager.GetText("/controlText/itemImport.ascx/importTitle") %>
    </div>
    <div>&nbsp;</div>

    <ckbx:Uploader ID="_uploader" runat="server" UploadedCallback="itemFileUploaded" SelectFilePromptTextId="/controlText/fileUploader/selectFile" />
    
    <asp:Panel runat="server" id="_extensionValidationError" class="error message" Visible="false">
        <%=WebTextManager.GetText("/controlText/itemImport.ascx/notValidExtension")%>
    </asp:Panel>
</div>