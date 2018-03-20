<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="SurveyUploader.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.SurveyUploader" %>
<%@ Register tagPrefix="ckbx" tagName="Uploader" src="~/Controls/Uploader.ascx" %>
<%@ Import Namespace="Checkbox.Web"%>
<%@ Register Assembly="CheckboxWeb" Namespace="CheckboxWeb.Controls.Button" TagPrefix="btn" %>

    <script type="text/javascript" language="javascript">
        //Handle file selection.
        function surveyFileUploaded(fileData) {
            if (fileData != null) {
                $('#<%=_uploadedFileNameTxt.ClientID %>').val(fileData.name);
                $('#<%=_uploadedFilePathTxt.ClientID %>').val(fileData.TempName);

                //Hide error divs
                $('#<%=_extensionValidationError.ClientID %>').hide();
                $('#<%=_formatValidationError.ClientID %>').hide();
                $('#<%=_invalidXmlError.ClientID %>').hide();
                $('#<%=_xmlfromPreviousVersion.ClientID %>').hide();
            }
        }
    </script>

<div style="display:none;">
    <asp:TextBox ID="_uploadedFileNameTxt" runat="server" />
    <asp:TextBox ID="_uploadedFilePathTxt" runat="server" />
    <asp:HiddenField id="_parsedNameTxt" runat="server" />
</div>
<asp:Panel ID="_containerPanel" runat="server">
    <div class="dialogInstructions">
        <ckbx:MultiLanguageLabel ID="_selectFileLbl" runat="server" TextId="/controlText/forms/surveys/surveyUploader.ascx/selectSurveyToUpload" />
    </div>

    <ckbx:Uploader ID="_uploader" runat="server" UploadedCallback="surveyFileUploaded" SelectFilePromptTextId="/controlText/fileUploader/selectFile"
                   MaxUploadingLength="30" MaxDowloadErrorLength="35" MaxDowloadedLength="50"/>
    
    <asp:Panel runat="server" id="_extensionValidationError" class="error message">
        <%=WebTextManager.GetText("/controlText/forms/surveys/surveyUploader.ascx/pleaseSelectXmlFile") %>
    </asp:Panel>

    <asp:Panel runat="server" id="_invalidXmlError" class="error message">
        <%= WebTextManager.GetText("/controlText/forms/surveys/surveyUploader.ascx/fileNotValidXml") %>
    </asp:Panel>

    <asp:Panel runat="server" id="_formatValidationError" class="error message">
        <%=WebTextManager.GetText("/controlText/forms/surveys/surveyUploader.ascx/fileNotCorrectFormat") %>
    </asp:Panel>
    
    <asp:Panel runat="server" id="_xmlfromPreviousVersion" class="error message">
        <%=WebTextManager.GetText("/controlText/forms/surveys/surveyUploader.ascx/xmlPreviousVersion")%>
    </asp:Panel>
</asp:Panel>    
