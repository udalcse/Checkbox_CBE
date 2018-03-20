<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="Languages.aspx.cs" Inherits="CheckboxWeb.Forms.Surveys.Languages" MasterPageFile="~/Dialog.Master" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>
<%@ Register Src="~/Controls/Status/StatusControl.ascx" TagPrefix="ckbx" TagName="StatusControl" %>
<%@ Register tagPrefix="ckbx" tagName="Uploader" src="~/Controls/Uploader.ascx" %>
<%@ Import Namespace="Checkbox.Web"%>

<asp:Content ID="_headContent" runat="server" ContentPlaceHolderID="_headContent">
    <ckbx:ResolvingScriptElement runat="server" Source="../../Resources/jquery.ckbxtab.js" />

    <script type="text/javascript" language="javascript">
        var oldHref = "#";
        $(document).ready(function () {

            oldHref = $('#<%=_import.ClientID %>').attr("href");
            $('#<%=_import.ClientID %>').attr("href", "#");

            $('.saveCancelButton').css('margin-top', '30px');
        });

         //Handle file selection.
        function onXmlFileUploaded(fileData) {
            if (fileData != null) {
                $('#<%=_uploadedFileNameTxt.ClientID %>').val(fileData.name);
                $('#<%=_uploadedFilePathTxt.ClientID %>').val(fileData.TempName);
                $('#<%=_import.ClientID %>').attr("href", oldHref);
            }
        }
    </script>
</asp:Content>

<asp:Content ID="_pageContent" runat="server" ContentPlaceHolderID="_pageContent">
    <div style="display:none;">
        <asp:TextBox ID="_refreshRequired" runat="server" Text="no"  />
    </div>
    <div class="padding10">
        <ckbx:StatusControl ID="_importExportStatus" runat="server" />
        <br />
        <div style="display:none">
            <asp:TextBox ID="_uploadedFileNameTxt" runat="server" />
            <asp:TextBox ID="_uploadedFilePathTxt" runat="server" />
        </div>
            
        <ckbx:Uploader ID="_uploader" runat="server" UploadedCallback="onXmlFileUploaded" SelectFilePromptTextId="/controlText/fileUploader/selectFile" />            

        <asp:Panel runat="server" id="_extensionValidationError" class="error message">
            <%=WebTextManager.GetText("/pageText/surveyLanguage.aspx/fileNotXml")%>
        </asp:Panel>

        <asp:Panel runat="server" id="_invalidXmlError" class="error message">
            <%= WebTextManager.GetText("/pageText/settings/importText.aspx/loadFileError")%>
        </asp:Panel>

        <asp:Panel runat="server" id="_formatValidationError" class="error message">
            <%=WebTextManager.GetText("/pageText/surveyLanguage.aspx/noTextFound")%>
        </asp:Panel>

        <btn:CheckboxButton ID="_import" runat="server" TextID="/pageText/surveyLanguage.aspx/import" CssClass="ckbxButton roundedCorners border999 shadow999 orangeButton" />
        <div class="clear"></div>
    </div>
</asp:Content>