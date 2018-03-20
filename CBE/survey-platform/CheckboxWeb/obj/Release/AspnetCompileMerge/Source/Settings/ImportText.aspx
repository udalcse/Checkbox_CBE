<%@ Page Title="" Language="C#" MasterPageFile="~/Dialog.Master" CodeBehind="ImportText.aspx.cs" Inherits="CheckboxWeb.Settings.ImportText" AutoEventWireup="false" %>
<%@ Import Namespace="Checkbox.Web"%>
<%@ MasterType VirtualPath="~/Dialog.Master" %>

<asp:Content ContentPlaceHolderID="_headContent" runat="server"></asp:Content>

<asp:Content ContentPlaceHolderID="_pageContent" runat="server">
    <script type="text/javascript">
        function doTextExportExt() {
            doTextExport('_textExportLink', $('[name="<%=_exportOptions.UniqueID%>"]:checked').val());
        }
    </script>

    <h3><%= WebTextManager.GetText("/pageText/settings/navigation.ascx/importText")%></h3>
    <div id="importErrorMessage" class="error message" style="display:none">Import file failed</div>
    <div class="dashStatsWrapper border999 shadow999">
        <div class="dashStatsHeader">
            <span class="mainStats left"><%= WebTextManager.GetText("/pageText/settings/importText.aspx/exportOptions")%></span>
        </div>
        <div class="dashStatsContent">
            <div class="input">
                <ckbx:MultiLanguageRadioButtonList ID="_exportOptions" runat="server" >
                    <asp:ListItem TextId="/pageText/settings/importText.aspx/applicationText" Value="ApplicationText"/>
                    <asp:ListItem Selected="True" TextId="/pageText/settings/importText.aspx/allText" Value="AllText"/>
                </ckbx:MultiLanguageRadioButtonList>
            </div>
            <div class="left input spacing">
                <a class="ckbxButton roundedCorners border999 shadow999 silverButton smallButton" href="javascript:doTextExportExt()" runat="server" id="_textExportLink"><%= WebTextManager.GetText("/pageText/settings/importText.aspx/export")%></a>
            </div>
            <br class="clear" />
        </div>
    </div> 

    <div class="dashStatsWrapper border999 shadow999">
        <div class="dashStatsHeader">
            <span class="mainStats left"><%= WebTextManager.GetText("/pageText/settings/importText.aspx/importOptions")%></span>
        </div>
        <div class="dashStatsContent">
            <div class="dashStatsContentHeader"><ckbx:MultiLanguageLabel ID="MultiLanguageLabel2" runat="server" TextId="/pageText/settings/importText.aspx/importTextFromXml">Import Text from XML</ckbx:MultiLanguageLabel></div>
            <div class="spacing">
                <ckbx:UFrameFileUploadControl ID="_fileUploader" ButtonTextID="/controlText/xmlListImport/import" ButtonCssClass="ckbxButton roundedCorners border999 shadow999 silverButton smallButton" runat="server" />
            </div>
        </div>
        <div class="clear"></div>
    </div> 
    <div class="dialogFormPush">&nbsp;</div>
</asp:Content>
