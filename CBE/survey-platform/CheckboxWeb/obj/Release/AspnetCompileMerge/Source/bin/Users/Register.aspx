<%@ Page Language="C#" CodeBehind="Register.aspx.cs" Inherits="CheckboxWeb.Users.Register" MasterPageFile="~/Admin.Master" %>
<%@ Import Namespace="Checkbox.Globalization.Text" %>
<%@ MasterType VirtualPath="~/Admin.Master" %>

<asp:Content ID="_styleContent" runat="server" ContentPlaceHolderID="_styleContent">
    <style type="text/css">
    <!--
      <asp:PlaceHolder id="_styleCssPlace" runat="server" />
    -->
    
    .error{display:block;margin-bottom:25px;}
    
  </style>
</asp:Content>

<asp:Content ID="_pageContent" runat="server" ContentPlaceHolderID="_pageContent">
    
    <asp:PlaceHolder runat="server" ID="_styleHeaderPlace" Visible="false" />
    <br />
  
    <asp:PlaceHolder ID="_createUserPlace" runat="server">
        
        <div class="formInput padding10" style="margin-left:20px;">
            <div> <%= TextManager.GetText("/selfRegistrationScreen/registrationText", LanguageCode) %> </div>
            <br />
            <asp:Label ID="_registerInstructionsLbl" runat="server" Text="You may register by entering the information below. Required fields are marked with a *" />
                
            <div class="clear"></div>
        
            <asp:Panel ID="_errorPanel" runat="server" CssClass="grid_12">
                <div class="error message" runat="server" style="padding:5px;">
                    <asp:Label ID="_topErrorLbl" runat="server" />
                    <asp:Repeater ID="_errorRepeater" runat="server">
                        <HeaderTemplate><br /></HeaderTemplate>
                        <ItemTemplate>
                            * <%#Container.DataItem %>
                        </ItemTemplate>
                        <SeparatorTemplate>
                            <br />
                        </SeparatorTemplate>
                        <FooterTemplate><br /></FooterTemplate>
                    </asp:Repeater>
                </div>
            </asp:Panel>
        
            <div class="clear"></div>
            <br />

            <div class="left fixed_300">
                <div class="dialogSubTitle">
                    <label class="registerSubSection"><%= TextManager.GetText("/selfRegistrationScreen/loginInformation", LanguageCode) %></label>
                </div>
      
                <div class="clear"></div>

                <p><label for="<%= _userName.ClientID %>"><%= TextManager.GetText("/selfRegistrationScreen/desiredUsername", LanguageCode) %></label></p>
                <asp:textbox id="_userName" runat="server" CssClass="registerTextBox" />
                <asp:Label id="_userNameRequiredLbl" runat="server" CssClass="registerRequired">*</asp:Label>&nbsp;
                <asp:Label ID="_userNameErrorLbl" runat="server" CssClass="error message" EnableViewState="false" Visible="false" />
      
                <div class="clear"></div>
                <p><label for="<%= _password.ClientID %>"><%= TextManager.GetText("/selfRegistrationScreen/password", LanguageCode) %></label></p>
                <asp:textbox id="_password" runat="server" CssClass="registerTextBox" TextMode="Password"></asp:textbox>
                <asp:Label id="_passwordRequiredLbl" runat="server" CssClass="registerRequired">*</asp:Label>&nbsp;
                <asp:Label ID="_passwordErrorLbl" runat="server" CssClass="error message" EnableViewState="false" Visible="false" />

                <div class="clear"></div>
                <p><label for="<%= _confirmPassword.ClientID %>"><%= TextManager.GetText("/selfRegistrationScreen/confirmPassword", LanguageCode) %></label></p>
                <asp:textbox id="_confirmPassword" runat="server" CssClass="registerTextBox" TextMode="Password"></asp:textbox>
                <asp:Label id="_confirmRequiredLbl" runat="server" CssClass="registerRequired">*</asp:Label>&nbsp; 
                <ckbx:MultiLanguageLabel ID="_confirmPasswordRequiredError" runat="server" CssClass="error message" EnableViewState="false" Visible="false" />
                <asp:Label ID="_confirmErrorLbl" runat="server" CssClass="error message" EnableViewState="false" Visible="false" />
           
                <div class="dialogSubTitle">
                    <label class="registerSubSection"><%= TextManager.GetText("/selfRegistrationScreen/userInformation", LanguageCode) %></label>
                </div>
      
                <div class="clear"></div>
                <p><label for="<%= _email.ClientID %>"><%= TextManager.GetText("/selfRegistrationScreen/emailAddress", LanguageCode) %></label></p>
                <asp:textbox id="_email" runat="server" CssClass="registerTextBox"></asp:textbox>
                <asp:Label id="_emailRequiredLbl" runat="server" CssClass="registerRequired">*</asp:Label>
                <ckbx:MultiLanguageLabel ID="_emailErrorLbl" runat="server" CssClass="error message" EnableViewState="false" Visible="false" />
                  
                <div class="clear"></div>
      
                <asp:Repeater ID="_customFieldsRepeater" runat="server">
                    <SeparatorTemplate>
                        <div class="clear"></div>
                    </SeparatorTemplate>
                    <ItemTemplate>
                        <p><label><%# Container.DataItem %></label></p>
                        <asp:textbox id="_valueTxt" runat="server" CssClass="registerTextBox"></asp:textbox>
                    </ItemTemplate>
                </asp:Repeater>
           
       </div>
            <div class="clear"></div>

            <div style="text-align:center;margin-top:25px;">
                <div class="left" style="margin-right:25px;padding-top:2px;">
                <btn:CheckboxButton ID="_cancelBtn" runat="server" CssClass="cancelButton"  />
                </div>
                <div class="left">
                    <btn:CheckboxButton ID="_okBtn" runat="server" CssClass="ckbxButton roundedCorners border999 shadow999 silverButton" />
                </div>
                <div class="clear"></div>
            </div>
            <div class="clear"></div>
        </div>
    
    </div>
        </asp:PlaceHolder>
    
        <asp:PlaceHolder ID="_userCreatedPlace" runat="server">
            <div class="padding10">
                <asp:Label ID="_userCreatedLbl" runat="server" />
            
                <div class="clear"></div>
                <br /><br />
        
                <btn:CheckboxButton ID="_returnBtn" runat="server" CssClass="ckbxButton roundedCorners border999 shadow999 silverButton" />
            </div>
        </asp:PlaceHolder>
    
        <br /><br />
    
    <asp:PlaceHolder runat="server" ID="_styleFooterPlace" Visible="false" />
</asp:Content>