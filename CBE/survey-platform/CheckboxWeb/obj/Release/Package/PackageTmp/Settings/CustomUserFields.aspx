<%@ Page Title="" Language="C#" MasterPageFile="~/Dialog.Master" CodeBehind="CustomUserFields.aspx.cs" Inherits="CheckboxWeb.Settings.CustomUserFields" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>

<asp:Content ID="messageContent" ContentPlaceHolderID="_headContent" runat="server">
    <script type="text/javascript">
        //Show remove field confirm dialog.
        function showRemoveFieldConfirmDialog(confirmMessage, button) {
            if (confirm(confirmMessage))
                UFrameManager.submitInput(button);
            return false;        
        }

        function restoreDropDownValue(fieldName) {
            debugger;
            var dropDownId = "dd_" + fieldName;
            $("select[id*='" + dropDownId + "']").val(prevFieldType).change();
        }
        

    </script>
</asp:Content>

<asp:Content ID="settingContent" ContentPlaceHolderID="_pageContent" runat="server">
    <h3><%= WebTextManager.GetText("/pageText/settings/navigation.ascx/customUserFields")%>     
        <a ID="_exportProfile" href="javascript:showDialog('CustomFieldsExport.aspx',700, 800)" class="header-button ckbxButton silverButton right main-font" runat="server">Export</a>
        <a ID="_importProfile" href="javascript:showDialog('CustomFieldsImport.aspx',700, 800)" class="header-button ckbxButton silverButton right main-font" style="margin-right: 5px;" runat="server">Import</a>
    </h3>
    <div class="warning message" runat="server" id="StatusMessage" visible="false"></div>

    <div class="dashStatsWrapper border999 shadow999">
        <div class="dashStatsHeader"><span class="mainStats left">&nbsp;</span></div>
        <div class="dashStatsContent">
            <div class="warning message">
                <ckbx:MultiLanguageLabel ID="_descriptionLbl" runat="server" TextId="/pageText/settings/customUserFields.aspx/description"  />
            </div>
                <asp:datagrid id="_properties" runat="server" class="customFieldsGrid">
                    <HeaderStyle HorizontalAlign="Center" CssClass="dashStatsContentHeader"></HeaderStyle>
                    <ItemStyle HorizontalAlign="Center" CssClass="zebra"></ItemStyle>
                    <AlternatingItemStyle HorizontalAlign="Center" CssClass="detailZebra"></AlternatingItemStyle>
                </asp:datagrid>
            <div style="color: Red">
                <ckbx:MultiLanguageLabel ID="_newFieldError" Visible="false" runat="server" TextId="/pageText/settings/customUserFields.aspx/invalidName"  />
                
            </div>
            <div class="input">
                <ckbx:MultiLanguageCheckBox id="_isHidden" runat="server" TextId="/pageText/settings/customUserFields.aspx/hideDescription" />
            </div>
            <div class="warning message">
            The following values are reserved and can not be used as profile properties <span style="font-weight: bold">Language, email, domain, username, and uniqueidentifier</span>
                </div>
        </div>
    </div>
    <div class="dialogFormPush">&nbsp;</div>
</asp:Content>
