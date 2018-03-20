<%@ Page Title="" Language="C#" MasterPageFile="~/Dialog.Master" AutoEventWireup="false" CodeBehind="Properties.aspx.cs" Inherits="CheckboxWeb.Users.Properties" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Register src="Controls/RoleSelector.ascx" tagname="RoleSelector" tagprefix="role" %>
<%@ Register src="Controls/ProfilePropertyEditor.ascx" tagname="ProfileEditor" tagprefix="prf" %>
<%@ Register src="Controls/GroupSelector.ascx" tagname="GroupSelector" tagprefix="grp" %>
<%@ Register Assembly="CheckboxWeb" Namespace="CheckboxWeb.Controls.Button" TagPrefix="btn" %>
<%@ Register src="../Controls/Status/StatusControl.ascx" tagname="StatusControl" tagprefix="status" %>

<asp:Content ID="head" runat="server" ContentPlaceHolderID="_headContent">
    <script type="text/javascript">
        $(document).ready(function () {
            $('#propertiesTabs').ckbxTabs({
                tabName: 'propertiesTabs',
                initialTabIndex: <%=_currentTabIndex.Text %>,
                onTabClick: function(index){$('#<%=_currentTabIndex.ClientID %>').val(index)},
                onTabsLoaded: function(){$('.tabContainer').show();$('.tabContentContainer').show();}
            });
        });
    </script>
</asp:Content>

<asp:Content ID="page" ContentPlaceHolderID="_pageContent" runat="server">
     <div style="display:none;">
        <asp:TextBox ID="_currentTabIndex" runat="server" Text="0" />
    </div>

    <ul id="propertiesTabs" class="tabContainer">
        <li><%=WebTextManager.GetText("/pageText/users/properties.aspx/basicInfoTab")%></li>
        <li><%=WebTextManager.GetText("/pageText/users/properties.aspx/profileTab")%></li>
        <li><%=WebTextManager.GetText("/pageText/users/properties.aspx/rolesTab")%></li>
        <li><%=WebTextManager.GetText("/pageText/users/properties.aspx/groupsTab")%></li>
    </ul>
    
    <div class="clear"></div>

    <div class="padding10">
        <status:StatusControl ID="_statusControl" runat="server" />
    </div>    
    <div class="clear"></div>

    <div class="tabContentContainer padding10">
        <div class="wizardContainer" id="propertiesTabs-0-tabContent">
            <asp:Panel ID="_loginTypePanel" runat="server" Visible="false" CssClass="leftColumn"> 
                <p class="selectorTitle">
                    <ckbx:MultiLanguageLabel id="_loginTypeLbl" runat="server" TextId="/pageText/users/properties.aspx/loginType">Login Type:</ckbx:MultiLanguageLabel>
                </p>
                <ckbx:MultiLanguageRadioButtonList ID="_loginTypeList" runat="server" RepeatDirection="Vertical" AutoPostBack="True" OnSelectedIndexChanged="LoginType_SelectedIndexChanged">
                    <asp:ListItem TextId="/pageText/users/properties.aspx/loginType/checkbox" Value="Checkbox"  Selected="True"/>
                    <asp:ListItem TextId="/pageText/users/properties.aspx/loginType/external" Value="External" />
                </ckbx:MultiLanguageRadioButtonList>
                <br />
                <ckbx:MultiLanguageLabel id="_descriptionLabel" runat="server"/>
            </asp:Panel>
            <asp:Panel ID="_loginInfoPanel" runat="server">

                <asp:Panel ID="_readOnlyUserWarningPanelLoginInfo" runat="server" Visible="false">
                    <div class="warning message">
                        <%=WebTextManager.GetText("/pageText/users/properties.aspx/readOnlyUserWarning")%>
                    </div>
                </asp:Panel>

                <asp:Panel ID="_externalUserWarningPanel" runat="server" Visible="false">
                    <div class="warning message" style="padding:10px;">
                        <ckbx:MultiLanguageLabel ID="_warningLbl" runat="server" TextId="/pageText/users/properties.aspx/externalUserWarning" />
                    </div>
                    <br />
                </asp:Panel>

                <ckbx:MultiLanguageLabel ID="_loginInfoTitle" runat="server" CssClass="panelTitle" TextId="/pageText/users/properties.aspx/loginInfoTitle" /><br />
                <ckbx:MultiLanguageLabel ID="_loginInfoInstructions" runat="server" CssClass="" TextId="/pageText/users/properties.aspx/loginInfoInstructions" /><br /><br />
              
                            
                <ckbx:MultiLanguageLabel ID="_usernameLabel" runat="server" AssociatedControlID="_username" TextId="/pageText/users/properties.aspx/username" CssClass="loginInfoLabel" /><asp:TextBox ID="_username" runat="server" CssClass="loginInfo" MaxLength="32" />
                <asp:RequiredFieldValidator ID="_usernameRequired" runat="server" ControlToValidate="_username" Display="Dynamic" CssClass="error message" ValidationGroup="UserInfo" ><%= WebTextManager.GetText("/pageText/users/properties.aspx/usernameRequired") %></asp:RequiredFieldValidator><asp:CustomValidator ID="_usernameFormat" runat="server" ControlToValidate="_username" Display="Dynamic" CssClass="error"  OnServerValidate="Username_ValidateFormat" EnableClientScript="false"><%= WebTextManager.GetText("/pageText/users/properties.aspx/usernameInvalidFormat") %></asp:CustomValidator><ckbx:MultiLanguageLabel ID="_usernameInUseLabel" runat="server" CssClass="error" Visible="false" TextId="/pageText/users/properties.aspx/usernameInUse" /><asp:RegularExpressionValidator ID="_usernameLength" runat="server" Display="Dynamic" ControlToValidate="_username" CssClass="error"  ValidationExpression="[\w\s]{1,255}"><%= WebTextManager.GetText("/pageText/users/properties.aspx/usernameLength") %></asp:RegularExpressionValidator><br />
                <ckbx:MultiLanguageLabel ID="_emailLabel" runat="server" AssociatedControlID="_email" TextId="/pageText/users/properties.aspx/email" CssClass="loginInfoLabel" /><asp:TextBox ID="_email" runat="server" CssClass="loginInfo" />
                <ckbx:MultiLanguageLabel ID="_emailFormatInvalidLabel" runat="server" CssClass="error message" Visible="false" TextId="/pageText/users/properties.aspx/emailInvalid" /><br />
                <asp:Placeholder runat="server" id="_passwordPlace">
                    <ckbx:MultiLanguageLabel ID="_passwordLabel" runat="server" AssociatedControlID="_password" TextId="/pageText/users/properties.aspx/password" CssClass="loginInfoLabel" />
                    <asp:TextBox ID="_password" runat="server" TextMode="Password" CssClass="loginInfo" />
                    <ckbx:MultiLanguageLabel ID="_passwordError" runat="server" CssClass="error message" Visible="false" />
                    <asp:RegularExpressionValidator ValidationGroup="UserInfo"  ID="_passwordLength" runat="server" Display="Dynamic" ControlToValidate="_password" CssClass="error message"  ValidationExpression="[\*\w\s]{1,255}"><%= WebTextManager.GetText("/pageText/users/properties.aspx/passwordLength") %></asp:RegularExpressionValidator><br />

                    <ckbx:MultiLanguageLabel ID="_passwordConfirmLabel" runat="server" AssociatedControlID="_passwordConfirm" TextId="/pageText/users/properties.aspx/confirmPassword" CssClass="loginInfoLabel" />
                    <asp:TextBox ID="_passwordConfirm" runat="server" TextMode="Password" CssClass="loginInfo" />
                    <ckbx:MultiLanguageLabel ID="_confirmPasswordError" runat="server" CssClass="error message" Visible="false" /><br />
                </asp:Placeholder>
                <asp:Placeholder runat="server" id="_domainPlace" Visible="false">
                    <ckbx:MultiLanguageLabel ID="_domainLabel" runat="server" AssociatedControlID="_domain" TextId="/pageText/users/properties.aspx/domain" CssClass="loginInfoLabel" />
                    <asp:TextBox ID="_domain" runat="server"  CssClass="loginInfo" /><ckbx:MultiLanguageLabel ID="_domainError" runat="server" CssClass="error message" Visible="false" TextId="/pageText/users/properties.aspx/domainRequired" /><br />
                </asp:Placeholder>
            <div class="clear"></div>
            </asp:Panel>
        </div>         
        <div class="wizardContainer" id="propertiesTabs-1-tabContent">
            <asp:Panel ID="_readOnlyUserWarningPanelProfile" runat="server" Visible="false">
                <div class="warning message">
                    <%=WebTextManager.GetText("/pageText/users/properties.aspx/readOnlyUserWarning")%>
                </div>
            </asp:Panel>

            <ckbx:MultiLanguageLabel ID="_profileTitle" runat="server" CssClass="panelTitle" TextId="/pageText/users/properties.aspx/profileTitle" /><br />
            <ckbx:MultiLanguageLabel ID="_profileInstructions" runat="server" CssClass="" TextId="/pageText/users/properties.aspx/profileInstructions" /><br /><br />
            <prf:ProfileEditor ID="_profileEditor" runat="server" />
            <div class="clear"></div>
        </div>
        <div class="wizardContainer" id="propertiesTabs-2-tabContent">
            <asp:Panel ID="_readOnlyUserWarningPanelRoles" runat="server" Visible="false">
                <div class="warning message">
                    <%=WebTextManager.GetText("/pageText/users/properties.aspx/readOnlyUserWarning")%>
                </div>
            </asp:Panel>

            <ckbx:MultiLanguageLabel ID="_roleTitle" runat="server" CssClass="panelTitle" TextId="/pageText/users/properties.aspx/roleTitle" /><br />
            <ckbx:MultiLanguageLabel ID="_roleInstructions" runat="server" CssClass="" TextId="/pageText/users/properties.aspx/roleInstructions" /><br /><br />
            <ckbx:MultiLanguageLabel ID="_roleRequiredError" runat="server" CssClass="error message" Visible="false" TextId="/pageText/users/properties.aspx/roleRequired" /><br />
            <role:RoleSelector ID="_roleSelector" runat="server" />
            <div class="clear"></div>
        </div>
        <div class="wizardContainer" id="propertiesTabs-3-tabContent">
            <asp:Panel ID="_readOnlyUserWarningPanelGroups" runat="server" Visible="false">
                <div class="warning message">
                    <%=WebTextManager.GetText("/pageText/users/properties.aspx/readOnlyUserWarning")%>
                </div>
            </asp:Panel>

            <ckbx:MultiLanguageLabel ID="_groupTitle" runat="server" CssClass="panelTitle" TextId="/pageText/users/properties.aspx/groupTitle" /><br />
            <ckbx:MultiLanguageLabel ID="_groupInstructions" runat="server" CssClass="" TextId="/pageText/users/properties.aspx/groupInstructions" /><br /><br />
            <ckbx:MultiLanguageLabel ID="_groupRequiredError" runat="server" CssClass="error message" Visible="false" TextId="/pageText/users/properties.aspx/groupRequired" /><br />
            <grp:GroupSelector ID="_groupSelector" runat="server" />
            <div class="clear"></div>
        </div>
    </div>
</asp:Content>
