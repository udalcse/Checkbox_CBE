<%@ Page Title="" Language="C#" MasterPageFile="~/Dialog.Master"  AutoEventWireup="false" CodeBehind="CompanySignature.aspx.cs" Inherits="CheckboxWeb.Settings.CompanySignature" %>
<%@ Register TagPrefix="styles" Namespace="Checkbox.Web.UI.Controls.Styles" Assembly="Checkbox.Web" %>
<%@ Register Src="~/Forms/Surveys/Controls/ItemEditors/ImageSelector.ascx" TagPrefix="ckbx" TagName="ImageSelector" %>
<%@ Import Namespace="Checkbox.Web"%>
<%@ MasterType VirtualPath="~/Dialog.Master" %>

<asp:Content ID="settingContent" ContentPlaceHolderID="_pageContent" runat="server" uframeignore="true">

<div class="dashStatsWrapper border999 shadow999">
      
     <div class="dashStatsHeader">
            <span class="mainStats left">Invitation settings</span>
        </div>
        <div class="dashStatsContent">
            <div class="dialogInstructions">
                  <asp:CheckBox id="_companySignatureEnabled" CssClass="trigger" runat="server" />
                  <ckbx:MultiLanguageLabel ID="dynamicSettings" runat="server" Text="Changing this option will enable the invitation settings below " />
            </div>
        </div>
    </div>
    
     <asp:Panel ID="_companySignature" runat="server" CssClass="dashStatsWrapper border999 shadow999">
        <div class="dashStatsHeader">
            <span class="mainStats left">Company signature</span>
        </div>
   
         <div class="dashStatsContent" style="padding-top:0px;border-top:1px solid gray;">
            <ckbx:ImageSelector ID="_companySignatureImageSelector" runat="server" />
        </div>
    </asp:Panel>
    

</asp:Content>
