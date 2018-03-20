<%@ Page Title="" Language="C#" MasterPageFile="~/Dialog.Master" CodeBehind="Branding.aspx.cs" Inherits="CheckboxWeb.Settings.Branding" %>
<%@ Register TagPrefix="styles" Namespace="Checkbox.Web.UI.Controls.Styles" Assembly="Checkbox.Web" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemEditors/ImageSelector.ascx" TagPrefix="ckbx" TagName="ImageSelector" %>
<%@ Import Namespace="Checkbox.Web"%>
<%@ MasterType VirtualPath="~/Dialog.Master" %>

<asp:Content ID="head" ContentPlaceHolderID="_headContent" runat="server">
   <script type="text/javascript">
        $(document).ready(function () {
            $('#<%=ID %>_Color').mColorPicker({imageFolder:'<%=ResolveUrl("~/App_Themes/CheckboxTheme/images") %>/'});
        });

        function <%=ID %>UpdateSelectedColor(newColor){
            $('#<%=_selectedColor.ClientID %>').val(newColor);
        }
   </script>
</asp:Content>

<asp:Content ID="content" runat="server" ContentPlaceHolderID="_pageContent">

    <div style="display:none;">
        <asp:TextBox ID="_selectedColor" runat="server" />
    </div>


    <h3><%=WebTextManager.GetText("/pageText/settings/branding.aspx/title")%></h3>

    <div class="dashStatsWrapper border999 shadow999">
        <div class="dashStatsHeader">
            <span class="mainStats left"><%= WebTextManager.GetText("/pageText/settings/branding.aspx/headerType")%></span>
        </div>
        <div class="dashStatsContent">
            <div class="dialogInstructions">
                <ckbx:MultiLanguageLabel ID="dynamicSettings" runat="server" TextId="/pageText/settings/branding.aspx/dynamicSettings" />
            </div>
            <div class="input">
                <asp:RadioButtonList id="_headerType" CssClass="trigger" runat="server" RepeatDirection="Vertical" AutoPostBack="true"/>
            </div>
        </div>
    </div>

    <asp:Panel ID="_logoOptionsPanel" runat="server" CssClass="dashStatsWrapper border999 shadow999">
        <div class="dashStatsHeader">
            <span class="mainStats left"><%= WebTextManager.GetText("/pageText/settings/branding.aspx/logoOptions")%></span>
        </div>
        <div class="warning message">
            <ckbx:MultiLanguageLabel runat="server" TextId="/pageText/settings/branding.aspx/imageSizeWarning"  />
        </div>
        <div class="dashStatsContent" style="padding-top:0px;border-top:1px solid gray;">
            <ckbx:ImageSelector ID="_logoImageSelector" runat="server" />
        </div>
    </asp:Panel>

    <asp:Panel ID="_textPanel" runat="server" CssClass="dashStatsWrapper border999 shadow999">
        <div class="dashStatsHeader">
            <span class="mainStats left"><%= WebTextManager.GetText("/pageText/settings/branding.aspx/textOptions")%></span>
        </div>
        <div class="dashStatsContent">
            <div class="input"><asp:TextBox ID="_headerText" runat="server" Width="300" /></div>
            <div class="spacing">&nbsp;</div>
            <div class="input left">
                <styles:XmlBoundDropDownList
                        id="_fontName"
                        runat="server"
                        DataFile="~/Resources/CodeDependentResources.xml"
                        XPath="/CodeDependentResources/StyleFonts/FontName"
                        DataValueField="Value"
                        DataTextField="Text" />
                <styles:XmlBoundDropDownList
                        id="_fontStyle"
                        runat="server"
                        DataFile="~/Resources/CodeDependentResources.xml"
                        XPath="/CodeDependentResources/FontStyles/FontStyle"
                        DataValueField="Value"
                        DataTextField="Text" />
                <styles:XmlBoundDropDownList
                        id="_fontSize"
                        runat="server"
                        DataFile="~/Resources/CodeDependentResources.xml"
                        XPath="/CodeDependentResources/FontSizes/FontSize"
                        DataValueField="Value"
                        DataTextField="Text" />
            </div>
            <div class="left">
                <input name="<%=ID %>_Color" id="<%=ID %>_Color" type="color" hex="true" text="hidden" style="width:20px;height:20px;display:none;" value="<%=_selectedColor.Text %>" onchange="<%=ID %>UpdateSelectedColor(this.value);" />
            </div>
            <br class="clear" />
        </div>
    </asp:Panel>
    <div class="dialogFormPush">&nbsp;</div>
</asp:Content>
