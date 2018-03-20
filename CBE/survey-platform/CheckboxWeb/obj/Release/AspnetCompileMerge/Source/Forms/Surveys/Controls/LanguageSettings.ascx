<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="LanguageSettings.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.LanguageSettings" %>
<%@ Import Namespace="Checkbox.Web"%>

<%-- Supported Languages --%>
<div class="formInput">
    <p><label><%=WebTextManager.GetText("/pageText/surveyLanguage.aspx/supportedLanguages")%></label></p>
</div>

<div class="padding15">
    <asp:Repeater ID="_languagesRepeater" runat="server" OnItemCommand="LanguagesRepeater_ItemCommand">
        <ItemTemplate>
            <div style="float:left;width:150px;border-bottom:1px solid gray #AEAEAE;">
                <%# string.Format("{0} [{1}]", WebTextManager.GetText("/languageText/" + Container.DataItem), Container.DataItem) %>
            </div>
            
            <div style="float:left;padding-left:15px;">
                <asp:ImageButton ID="_deleteBtn" ToolTip="Delete" runat="server" CommandName="Delete" CommandArgument='<%# Container.DataItem %>' ImageUrl="~/App_Themes/CheckboxTheme/Images/delete16.gif" />
            </div>
            
            <div class="clear"></div>
        </ItemTemplate>
    </asp:Repeater>
</div>

<div class="clear" style="height:5px;"></div>

<%-- Available Languages --%>
<div class="formInput">
    <p><label><%=WebTextManager.GetText("/pageText/surveyLanguage.aspx/availableLanguages")%></label></p>
</div>

<asp:Panel ID="availablePanel" runat="server" Visible="true" CssClass="padding15">
    <div class="left">
        <asp:DropDownList ID="_availableLanguages" runat="server" />
    </div>

    <div class="left" style="margin-left:3px;margin-top:4px;">
        <asp:ImageButton ID="_addLanguageBtn" runat="server" ToolTip="Add Language" ImageUrl="~/App_Themes/CheckboxTheme/Images/Plus-blue.png" Width="17px" Height="17px" CommandName="Add" />
    </div>
</asp:Panel>

<asp:Panel ID="noneAvailablePanel" runat="server" Visible="false">
    <div class="warning message">
        <ckbx:MultiLanguageLabel ID="_noneAvailableLbl" runat="server" TextId="/pageText/surveyLanguage.aspx/noAvailableLanguages"></ckbx:MultiLanguageLabel>
    </div>
</asp:Panel>

<div class="clear" style="height:25px;"></div>

<%-- Default Language --%>
<div class="formInput">
    <p><ckbx:MultiLanguageLabel ID="_defaultLanguageLbl" AssociatedControlID="_defaultLanguage" runat="server" TextId="/pageText/surveyLanguage.aspx/defaultLanguage" /></p>
    <asp:DropDownList ID="_defaultLanguage" runat="server" />
</div>

<div class="clear" style="height:5px;"></div>

<%-- Language Select --%>
<div class="formInput">
    <p><ckbx:MultiLanguageLabel ID="_languageSelectionLbl" AssociatedControlID="_languageSelectList" runat="server" TextId="/pageText/surveyLanguage.aspx/languageSelection" /></p>
    <ckbx:MultiLanguageDropDownList ID="_languageSelectList" runat="server" AutoPostBack="true">
        <asp:ListItem Text="Prompt User" Value="Prompt" TextId="/enum/formLanguageSource/prompt" />
        <asp:ListItem Text="Query String" Value="QueryString" TextId="/enum/formLanguageSource/queryString" />
        <asp:ListItem Text="User Attribute" Value="User" TextId="/enum/formLanguageSource/user" />
        <asp:ListItem Text="Session Variable" Value="Session" TextId="/enum/formLanguageSource/session" />
        <asp:ListItem Text="Browser Detection" Value="Browser" TextId="/enum/formLanguageSource/browser" />
    </ckbx:MultiLanguageDropDownList>
</div>

<div class="clear" style="height:5px;"></div>

<asp:PlaceHolder ID="_languageSelectOptionPlace" runat="server">
    <div class="formInput">
        <p><asp:Label AssociatedControlID="_variableNameTxt" ID="_optionLbl" runat="server" /></p>
        <asp:TextBox ID="_variableNameTxt" runat="server" />
    </div>
</asp:PlaceHolder>

<asp:PlaceHolder ID="_browserDetectionUnsuccessOptionPlace" runat="server">
    <div class="formInput">
        <p><ckbx:MultiLanguageLabel AssociatedControlID="_browserDetectionUnsuccessOption" ID="_browserOptionLbl" TextId="/pageText/surveyLanguage.aspx/unsuccessBrowserDetection" runat="server" /></p>
        <ckbx:MultiLanguageDropDownList ID="_browserDetectionUnsuccessOption" runat="server" AutoPostBack="true">
            <asp:ListItem Text="Use The Default Language" Value="Default Language" TextId="/enum/formLanguageSource/unsuccessDetectionUseDefault" />
            <asp:ListItem Text="Promt User" Value="Prompt User" TextId="/enum/formLanguageSource/unsuccessDetectionPromt" />
        </ckbx:MultiLanguageDropDownList>
    </div>
</asp:PlaceHolder>