<%@ Page Title="" Language="C#" MasterPageFile="~/Dialog.Master" CodeBehind="SurveyText.aspx.cs" Inherits="CheckboxWeb.Settings.SurveyText" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="_pageContent" runat="server">
    <asp:Panel id="_multiLanguageNotAllowedWarningPanel" runat="server" CssClass="warning message" Visible="false">
        <ckbx:MultiLanguageLabel ID="_multiLanguageNotAllowedWarning" runat="server" TextId="/pageText/settings/surveyText.aspx/multiLanguageNotAllowed" Text="MultiLanguage support is not allowed by your license."/><br />
    </asp:Panel>

    <h3><%= WebTextManager.GetText("/pageText/settings/navigation.ascx/surveyText")%></h3>

    <div class="dashStatsWrapper border999 shadow999">
        <div class="dashStatsHeader">
            <span class="mainStats left"><%= WebTextManager.GetText("/pageText/settings/surveyText.aspx/editLanguage")%></span>
        </div>
        <div class="dashStatsContent allMenu">
            <div class="fixed_125 left input"><ckbx:MultiLanguageLabel ID="MultiLanguageLabel1" runat="server" TextId="/pageText/settings/surveyText.aspx/editLanguage" /></div>
            <div class="left input">
                <asp:DropDownList ID="_miscTextLanguageList" runat="server" AutoPostBack="true" />
            </div>
            <br class="clear" />
        </div>
    </div> 

    <div class="dashStatsWrapper border999 shadow999">
        <div class="dashStatsHeader">
            <span class="mainStats fixed_125 left"><%= WebTextManager.GetText("/pageText/settings/navigation.ascx/surveyText")%></span>
            <a class="ckbxButton roundedCorners border999 shadow999 silverButton smallButton" href="javascript:doTextExport('_exportMiscTextBtn', 'void');" runat="server" id="_exportMiscTextBtn"><%= WebTextManager.GetText("/pageText/settings/surveyText.aspx/exportToXml")%></a>
            <br class="clear" />
        </div>
        <asp:Repeater ID="_miscTextRepeater" runat="server">
            <ItemTemplate>
                <div class="dashStatsContent zebra">
                    <div class="fixed_150 left input"><ckbx:MultiLanguageLabel ID="_textLbl" runat="server" TextId='<%# DataBinder.Eval(Container.DataItem, "TextID") + "/description" %>' /></div>
                    <div class="left input">
                        <asp:TextBox TextId='<%# DataBinder.Eval(Container.DataItem, "TextID") %>' ID="_textValue" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "TextValue") %>' Columns="60" />
                    </div>
                    <br class="clear" />
                </div>
            </ItemTemplate>
            <AlternatingItemTemplate>
                <div class="dashStatsContent detailZebra">
                    <div class="fixed_150 left input"><ckbx:MultiLanguageLabel ID="_textLbl" runat="server" TextId='<%# DataBinder.Eval(Container.DataItem, "TextID") + "/description" %>' /></div>
                    <div class="left input">
                        <asp:TextBox TextId='<%# DataBinder.Eval(Container.DataItem, "TextID") %>' ID="_textValue" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "TextValue") %>' Columns="60" />
                    </div>
                    <br class="clear" />
                </div>
            </AlternatingItemTemplate>
        </asp:Repeater>
    </div>
    <div class="dialogFormPush">&nbsp;</div>
</asp:Content>