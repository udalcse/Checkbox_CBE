<%@ Page Title="" Language="C#" MasterPageFile="~/Dialog.Master" CodeBehind="LanguageNames.aspx.cs" Inherits="CheckboxWeb.Settings.LanguageNames" %>
<%@ Import Namespace="Checkbox.Web"%>
<%@ Import Namespace="Checkbox.Globalization.Text"%>
<%@ Import Namespace="Checkbox.Common"%>
<%@ MasterType VirtualPath="~/Dialog.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="_pageContent" runat="server">
    <h3><%= WebTextManager.GetText("/pageText/settings/navigation.ascx/languageNames")%></h3>

    <asp:Panel id="_multiLanguageNotAllowedWarningPanel" runat="server" CssClass="warning message" Visible="false">
        <ckbx:MultiLanguageLabel ID="_multiLanguageNotAllowedWarning" runat="server" TextId="/pageText/settings/languageNames.aspx/multiLanguageNotAllowed" Text="MultiLanguage support is not allowed by your license."/><br />
    </asp:Panel>

    <asp:Panel runat="server" CssClass="dashStatsWrapper border999 shadow999" Visible="False">
        <div class="dashStatsHeader">
            <span class="mainStats left"><%= WebTextManager.GetText("/pageText/settings/languageNames.aspx/editLanguage")%></span>
        </div>
        <div class="dashStatsContent">
            <div class="fixed_125 input left"><ckbx:MultiLanguageLabel ID="_languageNamesLanguageSelectLbl" runat="server" TextId="/pageText/settings/languageNames.aspx/editLanguage" /></div>
            <div class="input left">
                <asp:DropDownList ID="_languageNamesLanguageList" runat="server" AutoPostBack="true" />
            </div>
            <br class="clear" />
        </div>
    </asp:Panel>

    <div class="dashStatsWrapper border999 shadow999">
        <div class="dashStatsHeader">
            <span class="mainStats fixed_125 left"><%= WebTextManager.GetText("/pageText/settings/navigation.ascx/languageNames")%></span>
            <a class="ckbxButton roundedCorners border999 shadow999 silverButton smallButton" href="javascript:doTextExport('_exportLanguageNamesBtn', '/languageText/')" runat="server" id="_exportLanguageNamesBtn"><%= WebTextManager.GetText("/pageText/settings/languageNames.aspx/exportToXml")%></a>
            <br class="clear" />
        </div>
        <asp:Repeater ID="_txtRepeater" runat="server" EnableViewState="false">
            <HeaderTemplate>
                <div class="dashStatsContentHeader">
                    <div class="fixed_150 left"><b><%=WebTextManager.GetText("/pageText/settings/languageNames.aspx/languageCode") %></b></div>
                    <div class="left"><b><%=WebTextManager.GetText("/pageText/settings/languageNames.aspx/localizedName") %></b></div>
                    <br class="clear" />
                </div>
            </HeaderTemplate>
            <ItemTemplate>
                <div class="dashStatsContent zebra">
                    <div class="fixed_150 left" style="line-height:25px;"><%# Container.DataItem %></div>
                    <div class="left input"><asp:TextBox LanguageCode='<%# Container.DataItem %>' EnableViewState="false" ID="_languageText" runat="server" Text='<%# GetText(Container.DataItem) %>' Columns="60" /></div>
                    <br class="clear" />
                </div>
            </ItemTemplate>
            <AlternatingItemTemplate>
                <div class="dashStatsContent detailZebra">
                    <div class="fixed_150 left" style="line-height:25px;"><%# Container.DataItem %></div>
                    <div class="left input"><asp:TextBox LanguageCode='<%# Container.DataItem %>' EnableViewState="false" ID="_languageText" runat="server" Text='<%# GetText(Container.DataItem) %>' Columns="60" /></div>
                    <br class="clear" />
                </div>
            </AlternatingItemTemplate>
        </asp:Repeater>
    </div>
    <div class="dialogFormPush">&nbsp;</div>
</asp:Content>

<script type="text/C#" runat="server">
    /// <summary>
    /// Get text for item
    /// </summary>
    /// <param name="languageCode"></param>
    /// <returns></returns>
    protected string GetText(object languageCode)
    {
        //Get the label for the specified text id
        if (languageCode == null
            || string.IsNullOrEmpty(languageCode.ToString())
            || Utilities.IsNullOrEmpty(_languageNamesLanguageList.SelectedValue))
        {
            return string.Empty;
        }

        return TextManager.GetText("/languageText/" + languageCode, _languageNamesLanguageList.SelectedValue, languageCode.ToString());
    }
</script>