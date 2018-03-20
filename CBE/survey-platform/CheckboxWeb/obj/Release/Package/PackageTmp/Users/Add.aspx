<%@ Page Title="" Language="C#" MasterPageFile="~/Dialog.Master" AutoEventWireup="false" CodeBehind="Add.aspx.cs" Inherits="CheckboxWeb.Users.Add" %>
<%@ MasterType VirtualPath="~/Dialog.Master"%>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Register src="../Controls/Wizard/WizardNavigator.ascx" tagname="WizardNavigator" tagprefix="nav" %>
<%@ Register src="../Controls/Wizard/WizardButtons.ascx" tagname="WizardButtons" tagprefix="btn" %>
<%@ Register src="Controls/RoleSelector.ascx" tagname="RoleSelector" tagprefix="role" %>
<%@ Register src="Controls/ProfilePropertyEditor.ascx" tagname="ProfileEditor" tagprefix="prf" %>
<%@ Register src="Controls/GroupSelector.ascx" tagname="GroupSelector" tagprefix="grp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="_headContent" runat="server">
    <ckbx:ResolvingScriptElement runat="server" Source="~/Resources/WizardHighlight.js" />
    
    <script>
        $(document).ready(
            function () {
                $('#<%=_username.ClientID%>').focus();
            });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="_pageContent" runat="server">
    <asp:Wizard ID="_addUserWizard" runat="server" DisplaySideBar="false" DisplayCancelButton="true" OnNextButtonClick="AddUserWizard_NextButtonClick" OnFinishButtonClick="AddUserWizard_FinishButtonClick" OnCancelButtonClick="AddUserWizard_CancelButtonClick" >
        <WizardSteps>
            <asp:WizardStep ID="UsernameStep" runat="server" Title="Welcome">
                <asp:Panel ID="Panel1" runat="server" CssClass="padding10">
                    <asp:Panel ID="_loginTypePanel" runat="server" Visible="false" CssClass="left fixed_350"> 
                        <div class="dialogSubTitle">
                            <%=WebTextManager.GetText("/pageText/users/add.aspx/loginType") %>
                        </div>
                        <div class="dialogSubContainer">
                            <ckbx:MultiLanguageRadioButtonList ID="_loginTypeList" runat="server" RepeatDirection="Vertical" AutoPostBack="True" OnSelectedIndexChanged="LoginType_SelectedIndexChanged">
                                <asp:ListItem Text="Checkbox authenticated" TextId="/pageText/users/add.aspx/loginType/checkbox" Value="Checkbox"  Selected="True"/>
                                <asp:ListItem Text="Password Protected" TextId="/pageText/users/add.aspx/loginType/external" Value="External" />
                            </ckbx:MultiLanguageRadioButtonList>
                            <div class="dialogInstructions">
                                <ckbx:MultiLanguageLabel id="_descriptionLabel" runat="server" />
                            </div>
                        </div>
                    </asp:Panel>
                    <asp:Panel ID="_loginInfoPanel" runat="server" CssClass="left">
                        <div class="dialogSubTitle">
                            <%=WebTextManager.GetText("/pageText/users/add.aspx/loginInfoTitle") %>
                        </div>
                        <div class="dialogSubContainer">
                            <div class="dialogInstructions">
                                <asp:Label ID="_loginInfoInstructions" runat="server" />
                            </div>
                            <div class="formInput">
                                <p><ckbx:MultiLanguageLabel ID="_usernameLabel" runat="server" AssociatedControlID="_username" TextId="/pageText/users/add.aspx/desiredUsername" CssClass="fixed_150" /></p>
                                <asp:TextBox ID="_username" runat="server" Width="300" />
                                <asp:RequiredFieldValidator ID="_usernameRequired" runat="server" ControlToValidate="_username" Display="Dynamic" CssClass="error message"><%= WebTextManager.GetText("/pageText/users/add.aspx/usernameRequired") %></asp:RequiredFieldValidator>
                                <asp:CustomValidator ID="_usernameFormat" runat="server"  ControlToValidate="_username" Display="Dynamic" CssClass="error message" OnServerValidate="Username_ValidateFormat"><%= WebTextManager.GetText("/pageText/users/add.aspx/usernameInvalidFormat") %></asp:CustomValidator>
                                <asp:CustomValidator ID="_usernameEmailFormat" runat="server"  ControlToValidate="_username" Display="Dynamic" CssClass="error message" OnServerValidate="Username_ValidateEmailFormat"><%= WebTextManager.GetText("/pageText/users/properties.aspx/userNameEmailRulesViolated")%></asp:CustomValidator>
                                <ckbx:MultiLanguageLabel ID="_usernameInUseLabel" runat="server" CssClass="error message" Visible="false" TextId="/pageText/users/add.aspx/usernameInUse" />
                                <asp:CustomValidator ID="_usernameLength" runat="server" Display="Dynamic" ControlToValidate="_username" CssClass="error message" OnServerValidate="Username_ValidateLength"><%= WebTextManager.GetText("/pageText/users/add.aspx/usernameLength") %></asp:CustomValidator>
                            </div>
                            <br class="clear" />
                            <div class="formInput">
                                <p><ckbx:MultiLanguageLabel ID="_emailLabel" runat="server" AssociatedControlID="_email" TextId="/pageText/users/add.aspx/email" CssClass="fixed_150" /></p>
                                <asp:TextBox ID="_email" runat="server"  Width="300" MaxLength="255" />
                                <asp:RequiredFieldValidator ID="_emailRequired" runat="server" ControlToValidate="_email" Display="Dynamic" CssClass="error message" Enabled="false"><%= WebTextManager.GetText("/pageText/users/add.aspx/emailRequired")%></asp:RequiredFieldValidator>
                                <ckbx:MultiLanguageLabel ID="_emailFormatInvalidLabel" runat="server" CssClass="error message" Visible="false" TextId="/pageText/users/add.aspx/emailInvalid" />
                            </div>
                            <br class="clear" />
                            <asp:Placeholder runat="server" id="_passwordPlace">
                                <div class="formInput">
                                    <p><ckbx:MultiLanguageLabel ID="_passwordLabel" runat="server" AssociatedControlID="_password" TextId="/pageText/users/add.aspx/password" CssClass="fixed_150" /></p>
                                    <asp:TextBox ID="_password" runat="server" TextMode="Password" MaxLength="255" />
                                    <ckbx:MultiLanguageLabel ID="_passwordValidationErrorLabel" runat="server" CssClass="error message" Visible="false" />
                                    <asp:RequiredFieldValidator ID="_passwordRequired" runat="server" ControlToValidate="_password" Display="Dynamic" CssClass="error message"><%= WebTextManager.GetText("/pageText/users/add.aspx/passwordRequired") %></asp:RequiredFieldValidator><br />                                
                                </div>
                                <br class="clear" />
                                <div class="formInput">
                                    <p><ckbx:MultiLanguageLabel ID="_passwordConfirmLabel" runat="server" AssociatedControlID="_passwordConfirm" TextId="/pageText/users/add.aspx/confirmPassword" CssClass="fixed_150" /></p>
                                    <asp:TextBox ID="_passwordConfirm" runat="server" TextMode="Password" MaxLength="255" />
                                    <asp:RequiredFieldValidator ID="_passwordConfirmRequried" runat="server" ControlToValidate="_passwordConfirm" Display="Dynamic" CssClass="error message"><%= WebTextManager.GetText("/pageText/users/add.aspx/passwordConfirmRequired") %></asp:RequiredFieldValidator>
                                    <asp:CompareValidator ID="_passwordMatchValidator" runat="server" ControlToValidate="_password" EnableClientScript="false" ControlToCompare="_passwordConfirm" CssClass="error message" Display="Dynamic"><%= WebTextManager.GetText("/pageText/users/add.aspx/passwordMatch")%></asp:CompareValidator>
                                </div>
                            </asp:Placeholder>
                            <asp:Placeholder runat="server" id="_domainPlace" Visible="false">
                                <div class="formInput">
                                    <p><ckbx:MultiLanguageLabel ID="_domainLabel" runat="server" AssociatedControlID="_domain" TextId="/pageText/users/add.aspx/domain" CssClass="fixed_150" /></p>
                                    <asp:TextBox ID="_domain" runat="server" />
                                </div>
                                <br class="clear" />
                            </asp:Placeholder>
                        </div>
                    </asp:Panel>
                    <br class="clear" />
                </asp:Panel>
            </asp:WizardStep>
            <asp:WizardStep ID="ProfileStep" runat="server">
                <asp:Panel ID="Panel2" runat="server" class="padding10">
                    <div class="dialogSubTitle">
                        <%=WebTextManager.GetText("/pageText/users/add.aspx/profileTitle") %>
                    </div>
                    <div class="dialogInstructions">
                        <ckbx:MultiLanguageLabel ID="_profileInstructions" runat="server" TextId="/pageText/users/add.aspx/profileInstructions" Text="Enter the user profile properties.  All fields are optional."/>
                    </div>
                    <div class="dialogSubContainer overflowPanel_350">
                        <prf:ProfileEditor ID="_profileEditor" runat="server" />
                    </div>
                </asp:Panel>
            </asp:WizardStep>
            <asp:WizardStep ID="RoleStep" runat="server">
                <asp:Panel ID="Panel3" runat="server" class="padding10">
                    <div class="dialogSubTitle">
                        <%=WebTextManager.GetText("/pageText/users/add.aspx/roleTitle") %>
                    </div>
                    <div class="dialogInstructions">
                        <%=WebTextManager.GetText("/pageText/users/add.aspx/roleInstructions") %>
                    </div>
                    <div class="dialogSubContainer">
                        <ckbx:MultiLanguageLabel ID="_roleRequiredError" runat="server" CssClass="error message" Visible="false" TextId="/pageText/users/add.aspx/roleRequired" />
                        <role:RoleSelector ID="_roleSelector" runat="server" />
                    </div>
                </asp:Panel>
            </asp:WizardStep>
            <asp:WizardStep ID="GroupStep" runat="server">
                <asp:Panel ID="Panel4" runat="server" class="padding10">
                    <div class="dialogSubTitle">
                        <%=WebTextManager.GetText("/pageText/users/add.aspx/groupTitle") %>
                    </div>
                    <div class="dialogInstructions">
                        <%=WebTextManager.GetText("/pageText/users/add.aspx/groupInstructions") %>
                    </div>
                    <ckbx:MultiLanguageLabel ID="_groupRequiredError" runat="server" CssClass="error message" Visible="false" TextId="/pageText/users/add.aspx/groupRequired" />
                    <grp:GroupSelector ID="_groupSelector" runat="server" />
                </asp:Panel>
            </asp:WizardStep>
            <asp:WizardStep ID="ReviewStep" runat="server" StepType="Finish">
                <asp:Panel ID="Panel5" runat="server" class="padding10">
                    <div class="dialogSubTitle">
                        <%=WebTextManager.GetText("/pageText/users/add.aspx/reviewTitle") %>
                    </div>
                    <div class="dialogInstructions">
                        <%=WebTextManager.GetText("/pageText/users/add.aspx/reviewInstructions") %>
                    </div>
                    <div class="centerContent" style="width:800px;">
                        <div class="dashStatsWrapper fixed_275 border999 shadow999 left overflowPanel_390">
                            <div class="dashStatsHeader">
                                <ckbx:MultiLanguageLabel id="_userInfoReviewTitle" runat="server" CssClass="left mainStats" TextId="/pageText/users/add.aspx/userInfoReviewTitle">User information</ckbx:MultiLanguageLabel>
                            </div>
                            <div class="zebra dashStatsContent">
                                <ckbx:MultiLanguageLabel ID="_usernameReviewLabel" runat="server" CssClass="left fixed_100" TextId="/pageText/users/add.aspx/username" /><asp:Label ID="_usernameReview" runat="server" CssClass="left fixed_100 truncate"/><br class="clear" />
                            </div>
                            <div class="detailZebra dashStatsContent">
                                <ckbx:MultiLanguageLabel ID="_emailReviewLabel" runat="server" CssClass="left fixed_100" TextId="/pageText/users/add.aspx/email" /><asp:Label ID="_emailReview" runat="server" CssClass="left fixed_100 truncate" /><br class="clear" />
                            </div>
                            <div class="zebra dashStatsContent">
                                <ckbx:MultiLanguageLabel ID="_domainReviewLabel" runat="server" TextId="/pageText/users/add.aspx/domain" CssClass="left fixed_100" /><asp:Label ID="_domainReview" runat="server" CssClass="left fixed_100 truncate" /><br class="clear" />
                            </div>
                            <asp:ListView ID="_profileReviewList" runat="server" OnItemCreated="ProfileReviewList_ItemCreated">
                                <LayoutTemplate>
                                    <asp:PlaceHolder runat="server" ID="itemPlaceholder"></asp:PlaceHolder>
                                </LayoutTemplate>
                                <ItemTemplate>
                                        <div class="detailZebra dashStatsContent"><asp:Label ID="_profileReviewLabel" runat="server" CssClass="left fixed_100 truncate" Text="<%# Container.DataItem %>" /><asp:Label ID="_profileReview" runat="server" CssClass="left fixed_100 truncate" /><br class="clear" /></div>
                                </ItemTemplate>
                                <AlternatingItemTemplate>
                                        <div class="zebra dashStatsContent"><asp:Label ID="_profileReviewLabel" runat="server" CssClass="left fixed_100 truncate" Text="<%# Container.DataItem %>" /><asp:Label ID="_profileReview" runat="server" CssClass="left fixed_100 truncate" /><br class="clear" /></div>
                                </AlternatingItemTemplate>
                            </asp:ListView>
                        </div>
                        <div class="dashStatsWrapper fixed_250 border999 shadow999 left overflowPanel_390" style="margin-left:5px;">
                            <div class="dashStatsHeader">
                                <ckbx:MultiLanguageLabel id="_rolesReviewTitle" runat="server" CssClass="left mainStats" TextId="/pageText/users/add.aspx/rolesReviewTitle">Selected roles</ckbx:MultiLanguageLabel>
                            </div>
                            <asp:ListView ID="_rolesReviewList" runat="server">
                                <LayoutTemplate>
                                    <asp:PlaceHolder runat="server" ID="itemPlaceholder"></asp:PlaceHolder>
                                </LayoutTemplate>
                                <ItemTemplate>
                                    <div class="zebra dashStatsContent"><asp:Label ID="_roleReviewLabel" runat="server" Text="<%# Container.DataItem %>" /></div>
                                </ItemTemplate>
                                <AlternatingItemTemplate>
                                    <div class="detailZebra dashStatsContent"><asp:Label ID="_roleReviewLabel" runat="server" Text="<%# Container.DataItem %>" /></div>
                                </AlternatingItemTemplate>
                            </asp:ListView>
                        </div>
                        <div class="dashStatsWrapper fixed_250 border999 shadow999 left overflowPanel_390" style="margin-left:5px;">
                            <div class="dashStatsHeader">
                                <ckbx:MultiLanguageLabel id="_groupsReviewTitle" runat="server" CssClass="left mainStats" TextId="/pageText/users/add.aspx/groupsReviewTitle">Selected groups</ckbx:MultiLanguageLabel>
                            </div>
                            <asp:ListView ID="_groupsReviewList" runat="server" OnItemCreated="GroupsReviewList_ItemCreated">
                                <LayoutTemplate>
                                    <asp:PlaceHolder runat="server" ID="itemPlaceholder"></asp:PlaceHolder>
                                </LayoutTemplate>
                                <ItemTemplate>
                                    <div class="dashStatsContent zebra"><asp:Label ID="_groupReviewLabel" runat="server" /></div>
                                </ItemTemplate>
                                <AlternatingItemTemplate>
                                    <div class="dashStatsContent detailZebra"><asp:Label ID="_groupReviewLabel" runat="server" /></div>
                                </AlternatingItemTemplate>
                                <EmptyDataTemplate>
                                    <div class="dashStatsContent zebra"><ckbx:MultiLanguageLabel ID="_groupReviewNoGroupsLabel" runat="server" TextId="/pageText/users/add.aspx/groupsReviewNoGroups" /></div>
                                </EmptyDataTemplate>
                            </asp:ListView>
                        </div>
                        <br class="clear" />
                    </div>
                </asp:Panel>
            </asp:WizardStep>
            <asp:WizardStep ID="CompleteStep" runat="server" StepType="Complete">
                <asp:Panel ID="Panel6" runat="server" CssClass="padding10">
                    <div class="dialogSubTitle">
                        <ckbx:MultiLanguageLabel ID="_completionTitle" runat="server" CssClass="panelTitle" TextId="/pageText/users/add.aspx/completionTitle" Text="User created"/>
                    </div>
                    <div class="dialogInstructions">
                        <ckbx:MultiLanguageLabel ID="_completionInstructions" runat="server" TextId="/pageText/users/add.aspx/completionInstructions" Text="You may create another user or return to the user manager"/>
                    </div>
                    <ckbx:MultiLanguageLabel ID="_createUserError" runat="server" CssClass="error message" Visible="false" />
                </asp:Panel>
                <div class="WizardNavContainer">
                    <hr />
                    <btn:CheckboxButton ID="_closeWizardButton" runat="server" CssClass="cancelButton left" OnClick="_closeWizardButton_Click" TextID="/pageText/users/add.aspx/exitButton" />
                    <btn:CheckboxButton ID="_restartButton" runat="server" CssClass="ckbxButton roundedCorners shadow999 border999 silverButton right" OnClick="RestartButton_OnClick" TextID="/pageText/users/add.aspx/restartButton" />
                </div>
            </asp:WizardStep>
        </WizardSteps>
        <StartNavigationTemplate>
            <btn:WizardButtons ID="_startNavigationButtons" runat="server" CloseDialogOnCancel="false" />
        </StartNavigationTemplate>
        <FinishNavigationTemplate>
            <btn:WizardButtons ID="_finishNavigationButtons" runat="server" CloseDialogOnCancel="false" />
        </FinishNavigationTemplate>
        <HeaderTemplate>
            <nav:WizardNavigator ID="_wizardNavigator" runat="server" AllowForwardNavigation="false" />
        </HeaderTemplate>
        <StepNavigationTemplate>
            <btn:WizardButtons ID="_stepNavigationButtons" runat="server" CloseDialogOnCancel="false" />
        </StepNavigationTemplate>
    </asp:Wizard>                
</asp:Content>
