<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="FileColumnSelector.ascx.cs" Inherits="CheckboxWeb.Users.Controls.FileColumnSelector" %>
<%@ Register Assembly="CheckboxWeb" Namespace="CheckboxWeb.Controls.Button" TagPrefix="btn" %>

<asp:Panel ID="_columnSelectorPanel" runat="server" Visible="true">
    <div class="centerContent" style="width:800px;">
        <div class="left">
            <asp:listbox ID="_availableColumnList" runat="server" Width="350" Height="200" SelectionMode="Multiple"></asp:listbox>
        </div>
        <div class="left" style="margin-top:57px;">
            <btn:CheckboxButton ID="_moveRightButton" runat="server" TextID="/controlText/users/controls/groupSelector/moveRight" CssClass="ckbxButton roundedCorners border999 shadow999 orangeButton"  OnClick="MoveRightButton_Click"/><br /><br />
            <btn:CheckboxButton ID="_moveLeftButton" runat="server" TextID="/controlText/users/controls/groupSelector/moveLeft" CssClass="ckbxButton roundedCorners border999 shadow999 orangeButton" OnClick="MoveLeftButton_Click"/>
        </div>
        <div class="left">
            <asp:ListBox ID="_selectedColumnList" runat="server" Width="350" Height="200" SelectionMode="Multiple"></asp:listbox>
            <asp:Button  runat="server" ID="_clearFieldsStructure" CssClass="clear-field-structure-btn"  Text="Clear current configuration" OnClick="ClearCurrentConfiguration_Click"></asp:Button>
            <asp:CheckBox runat="server" ID="_saveFieldsConfiguration" CssClass="save-fields-configuration-ckbx" Text="Save current configuration for next import" Enabled="False" />        
        </div>
        <ckbx:MultiLanguageImageButton ID="_moveFieldUpBtn" runat="server" SkinID="FileFieldMoveUpButton" OnClick="MoveFieldUpBtn_Click"></ckbx:MultiLanguageImageButton>
        <ckbx:MultiLanguageImageButton ID="_moveFieldDownBtn" runat="server" SkinID="FileFieldMoveDownButton" OnClick="MoveFieldDownBtn_Click"></ckbx:MultiLanguageImageButton>
        <br class="clear" />
    </div>
</asp:Panel>