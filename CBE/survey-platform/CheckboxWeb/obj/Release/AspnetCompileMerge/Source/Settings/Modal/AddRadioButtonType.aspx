<%@ Page Language="C#" MasterPageFile="~/Dialog.Master" AutoEventWireup="true" CodeBehind="AddRadioButtonType.aspx.cs" Inherits="CheckboxWeb.Settings.Modal.AddRadioButtonType" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemEditors/SelectOptionsEditor.ascx" TagPrefix="sle" TagName="SelectOptionsEditor" %>
<%@ Import namespace="Checkbox.Users" %>

<%@ MasterType VirtualPath="~/Dialog.Master" %>


<asp:Content ID="_content" ContentPlaceHolderID="_pageContent" runat="server">
    <style>
         .DialogButtonsContainer{ position: absolute;bottom: 25px;left: 30px;}
         .DialogButtonsContainer .buttonWrapper  { margin: 0;}
    </style>
    <div class="addRadioFieldContainer">
     <br />
     <asp:Repeater ID="_optionsRepeater" runat="server">
          <ItemTemplate>
                 <div>
                  <asp:RadioButton ID="_radioFieldOptionSelect" name="_radioFieldOptionSelect" runat="server" data-index="<%# Container.ItemIndex%>" 
                      Checked="<%# ((RadioButtonFieldOption)Container.DataItem).IsSelected %>" OnCheckedChanged="_radioFieldOptionSelect_CheckChanged" AutoPostBack="True"/>
                  <asp:TextBox ID="_radioFieldOption" runat="server" Text="<%# ((RadioButtonFieldOption)Container.DataItem).Name %>" Enabled="False" />
                  <asp:LinkButton runat="server" ID="_removeOption" data-index="<%# Container.ItemIndex%>"  Text="Remove" OnClick="_removeOption_Click"></asp:LinkButton>
                <div>
          </ItemTemplate>
      </asp:Repeater>
         <asp:RadioButton ID="_newRadioBtn"  runat="server" OnCheckedChanged="_newRadioBtn_CheckChanged" AutoPostBack="True"/>
         <asp:TextBox ID="_newRadioBtnOption" runat="server" ValidationGroup="RadioButtonTitle"  />
         <asp:Button ValidationGroup="RadioButtonTitle" runat="server" ID="_addRowBtn" Text="New choice" class="ckbxButton silverButton"/>
        <asp:RequiredFieldValidator ValidationGroup="RadioButtonTitle" id="_newRadioBtnOptionValidator" runat="server" ControlToValidate="_newRadioBtnOption" ErrorMessage="Option text is a required field." ForeColor="Red"></asp:RequiredFieldValidator>
    </div>
    <div style="position:absolute;bottom:80px;left:30px">
        <asp:CheckBox ID="_applyToAllCkbx" runat="server" />
        <asp:Label AssociatedControlID="_applyToAllCkbx" runat="server">Apply default selection to all users</asp:Label>
    </div>
</asp:Content>
