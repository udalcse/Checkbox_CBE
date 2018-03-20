<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="Import.aspx.cs" Inherits="CheckboxWeb.Forms.Surveys.Responses.Import" MasterPageFile="~/Dialog.master"%>
<%@ Register TagPrefix="cc1" Namespace="Checkbox.Web.UI.Controls" Assembly="Checkbox.Web" %>
<%@ Register Assembly="CheckboxWeb" Namespace="CheckboxWeb.Controls.Button" TagPrefix="btn" %>
<%@ MasterType VirtualPath="~/Dialog.master" %>


<asp:Content ID="script" ContentPlaceHolderID="_headContent" runat="server">
    <script type="text/javascript">
        function OnFileChoose(okButton, fileUploadText) {
            okButton.disabled = (fileUploadText.value == '');
            if (!okButton.disabled) {
                //manually subscribe the button
                okButton.href = "javascript: __doPostBack(null, null);";
            }

            if (!okButton.disabled && typeof ctl00_pageContentPlace_InfoPanel != "undefined") 
            {
                ctl00_pageContentPlace_InfoPanel.style.display = "none";
            }
        }
    </script>
</asp:Content>

<asp:Content ID="content" runat="server" ContentPlaceHolderID="_pageContent">
    <div class="padding10">
        <asp:Image ID="warningImg" runat="server" AlternateText="Warning Image" ImageUrl="~/App_Themes/CheckboxTheme/Images/warning.gif" />
        <cc1:MultiLanguageLabel ID="v3NoticeLbl" runat="server" CssClass="PrezzaNormal" TextId="/pageText/surveyImport.aspx/v3NoticeLbl" />
        <br />
        <cc1:MultiLanguageLabel ID="importInstructionsLbl" runat="server" CssClass="PrezzaNormal" TextId="/pageText/surveyImport.aspx/importSurveyResponsesInstructions" />
        <br />
        <br />
        <asp:Panel ID="inputPanel" runat="server">
            <asp:FileUpload ID="_inputFile" runat="server"/>
        </asp:Panel>
            <br />
        <asp:Panel ID="InfoPanel" runat="server">
            <asp:Label ID="WarningLabel" runat="server" ForeColor="Red" />
        </asp:Panel>
        <br />
    </div>
</asp:Content>
