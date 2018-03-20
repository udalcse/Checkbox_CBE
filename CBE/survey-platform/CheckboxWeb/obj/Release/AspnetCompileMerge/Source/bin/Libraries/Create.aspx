<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="Create.aspx.cs" Inherits="CheckboxWeb.Libraries.Create" MasterPageFile="~/Dialog.Master" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>
<%@ Register Assembly="CheckboxWeb" Namespace="CheckboxWeb.Controls.Button" TagPrefix="btn" %>
 

<asp:Content ContentPlaceHolderID="_pageContent" ID="content" runat="server" >
 <script type="text/javascript">
     $(document).ready(function () {
         $("img").each(function (idx, elem) {
             elem.src = elem.src.replace('Images/warning.gif', 'App_Themes/CheckboxTheme/Images/warning.gif');
             elem.src = elem.src.replace('Images/close.gif', 'App_Themes/CheckboxTheme/Images/close.png');
         });
     });
 </script>
    <div class="padding10">
        <div class="left input fixed_125">
            <ckbx:MultiLanguageLabel ID="libraryNameLbl" runat="server" TextId="/pageText/libraries/create.aspx/name" CssClass="itemLabel"/>
        </div>
        <div class="left input">
            <asp:TextBox ID="_libraryName" runat="server" Width="250" SetFocusOnError="true" />
        </div>
        <div class="left input">
            <asp:RequiredFieldValidator ID="_nameRequired" runat="server" ControlToValidate="_libraryName" Display="Dynamic" />
            <ckbx:CalloutLibraryNameInUseValidator ID="_duplicateName" runat="server" TextID="/pageText/libraries/create.aspx/nameInUse" ControlToValidate="_libraryName" Display="Dynamic" />
        </div>
        <br class="clear" />
        <div class="left input fixed_125">
            <ckbx:MultiLanguageLabel ID="libraryDescLbl" runat="server" TextId="/pageText/libraries/create.aspx/description" CssClass="itemLabel" />
        </div>
        <div class="left input">
            <asp:TextBox ID="_description" runat="server" TextMode="MultiLine" Width="250"  Rows="4" />
        </div>
        <br class="clear" />
    </div>
</asp:Content>