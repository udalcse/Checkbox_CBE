<%@ Page Language="C#" AutoEventWireup="false" MasterPageFile="~/Dialog.Master" CodeBehind="SelfRegistrationText.aspx.cs" Inherits="CheckboxWeb.Settings.SelfRegistrationText" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="_pageContent" runat="server">
    <asp:Panel id="_multiLanguageNotAllowedWarningPanel" runat="server" CssClass="warning message" Visible="false">
        <ckbx:MultiLanguageLabel ID="_multiLanguageNotAllowedWarning" runat="server" TextId="/pageText/settings/selfRegistrationText.aspx/multiLanguageNotAllowed" Text="MultiLanguage support is not allowed by your license."/><br />
    </asp:Panel>

        <h3><%= WebTextManager.GetText("/pageText/settings/navigation.ascx/selfRegistrationText")%></h3>
        
        <div class="dashStatsWrapper border999 shadow999">
            <div class="dashStatsHeader">
                <span class="mainStats left"><%= WebTextManager.GetText("/pageText/settings/selfRegistrationText.aspx/editLanguage")%></span>
            </div>
            <div class="dashStatsContent">
                <div class="fixed_125 left"><ckbx:MultiLanguageLabel ID="_validationPageLanguageLbl" runat="server" TextId="/pageText/settings/selfRegistrationText.aspx/editLanguage" /></div>
                <div class="left input">
                    <asp:DropDownList ID="_validationPageLanguageList" runat="server" AutoPostBack="true" />
                </div>
                <br class="clear" />
             </div>             
        </div>

        <div class="dashStatsWrapper border999 shadow999">
            <div class="dashStatsHeader">
                <span class="mainStats fixed_150 left"><%= WebTextManager.GetText("/pageText/settings/selfRegistrationText.aspx/selfRegistrationText")%></span>                    
                <a class="ckbxButton roundedCorners border999 shadow999 silverButton smallButton" href="javascript:doTextExport('_exportSelfRegistrationMsgsBtn', '/selfRegistrationScreen/')" runat="server" id="_exportSelfRegistrationMsgsBtn"><%= WebTextManager.GetText("/pageText/settings/selfRegistrationText.aspx/exportToXml")%></a>
                <br class="clear" />
            </div>
            <asp:Repeater ID="_validationMessageRepeater" runat="server">
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
