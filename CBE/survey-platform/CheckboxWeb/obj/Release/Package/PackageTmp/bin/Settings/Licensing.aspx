<%@ Page Title="" Language="C#" MasterPageFile="~/Dialog.Master" CodeBehind="Licensing.aspx.cs" Inherits="CheckboxWeb.Settings.Licensing" %>
<%@ Import Namespace="Checkbox.Web"%>
<%@ MasterType VirtualPath="~/Dialog.Master" %>

<asp:Content ID="settingContent" ContentPlaceHolderID="_pageContent" runat="server">
    <h3><%= WebTextManager.GetText("/pageText/settings/navigation.ascx/licensing")%></h3>
                <div>
            <span class="mainStats"><%=WebTextManager.GetText("/pageText/settings/licensing.aspx/descriptionText") %></span>
        </div>
    <div class="dashStatsWrapper border999 shadow999">

    
        <div class="dashStatsHeader">
            <span class="mainStats left"><%= WebTextManager.GetText("/pageText/settings/licensing.aspx/licenseDetails")%></span>
        </div>
        <div class="dashStatsContent zebra">
            <div class="fixed_150 left">
                <ckbx:MultiLanguageLabel ID="MultiLanguageLabel1" AssociatedControlID="_serialNumber" runat="server" TextId="/pageText/settings/licensing.aspx/serialNumber">Serial Number:</ckbx:MultiLanguageLabel>
            </div>
            <div class="left">
                <asp:Label id="_serialNumber" runat="server" />
            </div>
            <br class="clear" />
        </div>
        <div class="dashStatsContent detailZebra">
            <div class="fixed_150 left"><ckbx:MultiLanguageLabel ID="MultiLanguageLabel2" AssociatedControlID="_licenseType" runat="server" TextId="/pageText/settings/licensing.aspx/licenseType">License Type:</ckbx:MultiLanguageLabel></div>
            <div class="left">
                <asp:Label id="_licenseType" runat="server" />
            </div>
            <br class="clear" />
        </div>
        <div class="dashStatsContent zebra">
            <div class="fixed_150 left"><ckbx:MultiLanguageLabel ID="MultiLanguageLabel3" AssociatedControlID="_editorLimit" runat="server" TextId="/pageText/settings/licensing.aspx/editorLimit">Editor Limit:</ckbx:MultiLanguageLabel></div>
            <div class="left">
                <asp:Label id="_editorLimit" runat="server" />
            </div>
            <br class="clear" />
        </div>
        <ul class="dashStatsContent detailZebra">
            <li class="fixed_150 left"><ckbx:MultiLanguageLabel ID="MultiLanguageLabel6" AssociatedControlID="_allowInvitations" runat="server" TextId="/pageText/settings/licensing.aspx/allowInvitations">Allow invitations:</ckbx:MultiLanguageLabel></li>
            <li>
                <asp:Label id="_allowInvitations" runat="server" />
            </li>
            <div class="clear"></div>
        </ul>
        <ul class="dashStatsContent zebra">
            <li class="fixed_150 left"><ckbx:MultiLanguageLabel ID="MultiLanguageLabel7" AssociatedControlID="_allowMultilanguageSupport" runat="server" TextId="/pageText/settings/licensing.aspx/allowMultilanguageSupport">Allow multilanguage support:</ckbx:MultiLanguageLabel></li>
            <li>
                <asp:Label id="_allowMultilanguageSupport" runat="server" />
            </li>
            <div class="clear"></div>
        </ul>
        <ul class="dashStatsContent detailZebra">
            <li class="fixed_150 left"><ckbx:MultiLanguageLabel ID="MultiLanguageLabel8" AssociatedControlID="_allowNativeSpssExport" runat="server" TextId="/pageText/settings/licensing.aspx/allowNativeSpssExport">Allow native Spss export:</ckbx:MultiLanguageLabel></li>
            <li>
                <asp:Label id="_allowNativeSpssExport" runat="server" />
            </li>
            <div class="clear"></div>
        </ul>
    </div>

<asp:PlaceHolder ID="_adminsPlace" runat="server">
    <div class="dashStatsWrapper border999 shadow999">
        <div class="dashStatsHeader">
            <span class="mainStats left"><%= WebTextManager.GetText("/pageText/settings/licensing.aspx/administrators")%></span>
        </div>
        <div class="dashStatsContentHeader">
            <ckbx:MultiLanguageLabel ID="MultiLanguageLabel4" runat="server" Font-Bold="true" TextId="/pageText/settings/licensing.aspx/username" />
        </div>
        <asp:Repeater runat="server" ID="_users">
            <ItemTemplate>
                <div class="dashStatsContent zebra"><%# Container.ItemIndex + 1 %>. <%#Container.DataItem %></div>
            </ItemTemplate>
            <AlternatingItemTemplate>
                <div class="dashStatsContent detailZebra"><%# Container.ItemIndex + 1 %>. <%#Container.DataItem %></div>
            </AlternatingItemTemplate>
        </asp:Repeater>
    </div>
</asp:PlaceHolder>
</asp:Content>