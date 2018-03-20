<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="LibraryUploader.ascx.cs" Inherits="CheckboxWeb.Libraries.Controls.LibraryUploader" %>
<%@ Register tagPrefix="ckbx" tagName="Uploader" src="~/Controls/Uploader.ascx" %>
<%@ Import Namespace="Checkbox.Web"%>
<%@ Register Assembly="CheckboxWeb" Namespace="CheckboxWeb.Controls.Button" TagPrefix="btn" %>

    <script type="text/javascript" language="javascript">
        //Handle file selection.
        function libraryFileUploaded(fileData) {
            if (fileData != null) {
                $('#<%=_uploadedFileNameTxt.ClientID %>').val(fileData.name);
                $('#<%=_uploadedFilePathTxt.ClientID %>').val(fileData.TempName);

                //Hide error divs
                $('#<%=_extensionValidationError.ClientID %>').hide();
                $('#<%=_formatValidationError.ClientID %>').hide();
                $('#<%=_invalidXmlError.ClientID %>').hide();
            }
        }
    </script>

<div style="display:none;">
    <asp:TextBox ID="_uploadedFileNameTxt" runat="server" />
    <asp:TextBox ID="_uploadedFilePathTxt" runat="server" />
    <asp:HiddenField id="_parsedNameTxt" runat="server" />
</div>
<asp:Panel ID="_containerPanel" runat="server">

    <ckbx:Uploader ID="_uploader" runat="server" UploadedCallback="libraryFileUploaded" SelectFilePromptTextId="/controlText/fileUploader/selectFile" />
    
    <asp:Panel runat="server" id="_extensionValidationError" class="error message">
        <%=WebTextManager.GetText("/controlText/forms/libraries/libraryUploader.ascx/pleaseSelectXmlFile")%>
    </asp:Panel>

    <asp:Panel runat="server" id="_invalidXmlError" class="error message">
        <%= WebTextManager.GetText("/controlText/forms/libraries/libraryUploader.ascx/fileNotValidXml")%>
    </asp:Panel>

    <asp:Panel runat="server" id="_formatValidationError" class="error message">
        <%=WebTextManager.GetText("/controlText/forms/libraries/libraryUploader.ascx/fileNotCorrectFormat")%>
    </asp:Panel>
</asp:Panel>    
