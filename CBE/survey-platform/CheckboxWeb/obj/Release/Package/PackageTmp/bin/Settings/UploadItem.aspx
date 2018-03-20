<%@ Page Title="" Language="C#" MasterPageFile="~/Dialog.Master" CodeBehind="UploadItem.aspx.cs" Inherits="CheckboxWeb.Settings.UploadItem" %>
<%@ Import Namespace="Checkbox.Web"%>
<%@ MasterType VirtualPath="~/Dialog.Master" %>

<asp:Content ID="settingContent" ContentPlaceHolderID="_pageContent" runat="server">        <h3><%= WebTextManager.GetText("/pageText/settings/navigation.ascx/uploadItem")%></h3>
    <div class="dashStatsWrapper border999 shadow999">
        <div class="dashStatsHeader">
            <span class="mainStats left"><%= WebTextManager.GetText("/pageText/settings/uploadItem.aspx/fileUploadTitle")%></span>
            <br class="clear" />
        </div>
        <div class="dashStatsContent">
            <div class="dialogInstructions">
                <ckbx:MultiLanguageLabel ID="dynamicSettings" runat="server" TextId="/pageText/settings/uploadItem.aspx/dynamicSettings" />
            </div>
            <div class="input">
                <ckbx:MultiLanguageCheckBox id="_enableFileUpload" runat="server" AutoPostBack="true" OnCheckedChanged="EnableFileUpload_ClickEvent" TextId="/pageText/settings/UploadItem.aspx/enableFileUploadItem" /><br />
            </div>
        </div>
    </div>

    <div class="dashStatsWrapper border999 shadow999">
        <div class="dashStatsHeader" >
            <span class="mainStats left"><%= WebTextManager.GetText("/pageText/settings/uploadItem.aspx/fileTypeOptions")%></span>
            <br class="clear" />
        </div>
        <div class="dashStatsContent">
            <div class="dashStatsContentHeader">
                <ckbx:MultiLanguageLabel ID="MultiLanguageLabel1" runat="server" AssociatedControlID="_allowedFileTypes" TextId="/pageText/settings/UploadItem.aspx/allowedFileTypes" />
            </div>
            <div class="left input">
                <div class="padding10">
                    <asp:ListBox ID="_allowedFileTypes" runat="server" Height="150" Width="150" SelectionMode="Multiple" />
                </div>
                <div>
                    <ckbx:MultiLanguageButton ID="_deleteFileType" runat="server" CssClass="ckbxButton roundedCorners border999 shadow999 silverButton" Style="font-size:11px;" OnClick="DeleteFileType" TextId="/pageText/settings/UploadItem.aspx/deleteFileType" />
                </div>
            </div>
            <div class="fixed_50 left">&nbsp;</div>
            <div class="left input">
                <ckbx:MultiLanguageLabel ID="MultiLanguageLabel2" runat="server" AssociatedControlID="_fileType" TextId="/pageText/settings/UploadItem.aspx/fileType" >File Type</ckbx:MultiLanguageLabel>
                <asp:TextBox ID="_fileType" runat="server" Width="143" />
                <ckbx:MultiLanguageButton ID="_addFileType" runat="server" CssClass="ckbxButton roundedCorners border999 shadow999 silverButton" Style="font-size:11px;" OnClick="Add_ClickEvent" ToolTipTextId="/pageText/settings/UploadItem.aspx/addFileType" TextId="/pageText/settings/UploadItem.aspx/addFileType" />
            </div>
            <div class="left input">
                <ckbx:MultiLanguageLabel ID="_fileTypeError" runat="server" Visible="false" CssClass="error message"/>
            </div>
            <br class="clear" />
        </div>
    </div>

    <div class="dashStatsWrapper border999 shadow999">
        <div class="dashStatsHeader">
            <span class="mainStats left"><%= WebTextManager.GetText("/pageText/settings/uploadItem.aspx/exportOptions")%></span>
            <br class="clear" />
        </div>
        <div class="dashStatsContent">
            <ckbx:MultiLanguageCheckbox ID="_restrictExport" runat="server" TextId="/pageText/settings/UploadItem.aspx/restrictExport"/>
        </div>
    </div>
    <div class="dialogFormPush">&nbsp;</div>
</asp:Content>