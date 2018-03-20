<%@ Page Title="" Language="C#" MasterPageFile="~/Dialog.Master" AutoEventWireup="false" CodeBehind="Credentials.aspx.cs" Inherits="CheckboxWeb.Users.Credentials" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Register src="../Controls/Status/StatusControl.ascx" tagname="StatusControl" tagprefix="status" %>
<asp:Content ID="Content2" ContentPlaceHolderID="_pageContent" runat="server">
    <status:statuscontrol id="_statusControl" runat="server" />
    <div class="padding10" style="overflow:hidden">
        <asp:Panel ID="_loginTypePanel" runat="server" Visible="false" CssClass="leftColumn">
            <div class="dialogSubTitle">
                <ckbx:MultiLanguageLabel ID="_loginTypeLbl" runat="server" TextId="/pageText/users/properties.aspx/loginType">Login Type:</ckbx:MultiLanguageLabel>
            </div>
            <div class="left radioList">
                <ckbx:MultiLanguageRadioButtonList ID="_loginTypeList" runat="server" RepeatDirection="Vertical" AutoPostBack="True" CssClass="loginTypeList">
                    <asp:ListItem Text="Checkbox authenticated" TextId="/pageText/users/properties.aspx/loginType/checkbox" Value="Checkbox" Selected="True" />
                    <asp:ListItem Text="Password Protected" TextId="/pageText/users/properties.aspx/loginType/external" Value="External" />
                </ckbx:MultiLanguageRadioButtonList>
            </div>
            <div class="left" style="margin-left:15px;width:375px;font-style:italic">
                <ckbx:MultiLanguageLabel ID="_descriptionLabel" runat="server" />
            </div>
            <br class="clear"/>
        </asp:Panel>
        <asp:Panel ID="_loginInfoPanel" runat="server" Style="width: 690px;margin-top:15px;">
            <div class="dialogSubTitle">
                <ckbx:MultiLanguageLabel ID="_loginInfoTitle" runat="server" CssClass="panelTitle" TextId="/pageText/users/properties.aspx/loginInfoTitle" Text="Login information" /> 
               
            </div>
            <asp:Panel ID="_externalUserWarningPanel" runat="server" Visible="false">
                <div class="warning message">
                    <ckbx:MultiLanguageLabel ID="_warningLbl" runat="server" TextId="/pageText/users/properties.aspx/externalUserWarning" />
                </div>
            </asp:Panel>
            <asp:Panel ID="_readOnlyUserWarningPanel" runat="server" Visible="false">
                <div class="warning message">
                    <ckbx:MultiLanguageLabel ID="MultiLanguageLabel1" runat="server" TextId="/pageText/users/properties.aspx/readOnlyUserWarning" />
                </div>
            </asp:Panel>

            <div class="padding10">
                <div class="formInput">
                    <div class="left fixed_150">
                        <p><ckbx:MultiLanguageLabel ID="_usernameLabel" runat="server" AssociatedControlID="_username" TextId="/pageText/users/properties.aspx/username"/></p>
                    </div>
                    <div class="left">
                        <asp:TextBox ID="_username" runat="server" AutoEventWireup="false" MaxLength="255" Width="250" />
                    </div>
                     <div class="left">
                        <asp:RequiredFieldValidator ID="_usernameRequired" runat="server" ControlToValidate="_username" Display="Dynamic" CssClass="error message" ValidationGroup="UserInfo"><%= WebTextManager.GetText("/pageText/users/properties.aspx/usernameRequired") %></asp:RequiredFieldValidator>
                        <asp:CustomValidator ID="_usernameFormat" runat="server" ControlToValidate="_username" Display="Dynamic" CssClass="error message" OnServerValidate="Username_ValidateFormat" EnableClientScript="false"><%= WebTextManager.GetText("/pageText/users/properties.aspx/usernameInvalidFormat") %></asp:CustomValidator>
                        <ckbx:MultiLanguageLabel ID="_usernameInUseLabel" runat="server" CssClass="error message" Visible="false" TextId="/pageText/users/properties.aspx/usernameInUse" />
                        <asp:RegularExpressionValidator ID="_usernameLength" runat="server" Display="Dynamic" ControlToValidate="_username" CssClass="error message" ValidationExpression="[\w\s\W]{1,255}"><%= WebTextManager.GetText("/pageText/users/properties.aspx/usernameLength") %></asp:RegularExpressionValidator>
                    </div>                   
                    <br class="clear"/>
                </div>

                <div class="formInput">
                    <div class="left fixed_150">
                        <p><ckbx:MultiLanguageLabel ID="_emailLabel" runat="server" AssociatedControlID="_email" TextId="/pageText/users/properties.aspx/email" /></p>
                    </div>
                    <div class="left">
                        <asp:TextBox ID="_email" runat="server" AutoEventWireup="false" MaxLength="255" Width="250" />
                    </div>
                    <div class="left">
                        <asp:RequiredFieldValidator ID="_emailRequired" runat="server" ControlToValidate="_email" Display="Dynamic" CssClass="error message" Enabled="false" ValidationGroup="UserInfo"><%= WebTextManager.GetText("/pageText/users/properties.aspx/emailRequired") %></asp:RequiredFieldValidator>
                        <ckbx:MultiLanguageLabel ID="_emailFormatInvalidLabel" runat="server" CssClass="error message" Visible="false" TextId="/pageText/users/properties.aspx/emailInvalid" />
                    </div>
                    <br class="clear" />
                </div>
                
                <asp:PlaceHolder runat="server" ID="_passwordPlace">
                    <div class="formInput">
                        <div class="left fixed_150">
                            <p><ckbx:MultiLanguageLabel ID="_passwordLabel" runat="server" AssociatedControlID="_password" TextId="/pageText/users/properties.aspx/password" /></p>
                        </div>
                        <div class="left">
                            <asp:TextBox ID="_password" runat="server" TextMode="Password" Width="150" AutoEventWireup="false" />
                        </div>
                        <div class="left">
                            <ckbx:MultiLanguageLabel ID="_passwordError" runat="server" CssClass="error message" Visible="false" />
                            <asp:RegularExpressionValidator ValidationGroup="UserInfo" ID="_passwordLength" runat="server" Display="Dynamic" ControlToValidate="_password" CssClass="error message" ValidationExpression="[\*\w\s]{1,255}"><%= WebTextManager.GetText("/pageText/users/properties.aspx/passwordLength") %></asp:RegularExpressionValidator><br />
                        </div>
                        <br class="clear" />
                    </div>

                    <div class="formInput">
                        <div class="left fixed_150">
                            <p><ckbx:MultiLanguageLabel ID="_passwordConfirmLabel" runat="server" AssociatedControlID="_passwordConfirm" TextId="/pageText/users/properties.aspx/confirmPassword" /></p>
                        </div>
                        <div class="left">
                            <asp:TextBox ID="_passwordConfirm" runat="server" TextMode="Password"  Width="150" AutoEventWireup="false" />
                        </div>
                        <div class="left">
                            <ckbx:MultiLanguageLabel ID="_confirmPasswordError" runat="server" CssClass="error message" Visible="false" />
                        </div>
                    </div>
                    <br class="clear" />
                </asp:PlaceHolder>
                <asp:PlaceHolder runat="server" ID="_domainPlace" Visible="false">
                    <div class="formInput">
                        <div class="left fixed_150">
                            <p><ckbx:MultiLanguageLabel ID="_domainLabel" runat="server" AssociatedControlID="_domain" TextId="/pageText/users/properties.aspx/domain" /></p>
                        </div>
                        <div class="left">
                            <asp:TextBox ID="_domain" runat="server" AutoEventWireup="false" MaxLength="255" Width="250" />
                        </div>
                        <div class="left">
                            <ckbx:MultiLanguageLabel ID="_domainError" runat="server" CssClass="error message" Visible="false" TextId="/pageText/users/properties.aspx/domainRequired" /><br />
                        </div>
                        <br class="clear"/>
                    </div>
                </asp:PlaceHolder>
            </div>
        </asp:Panel>
    </div>
</asp:Content>
