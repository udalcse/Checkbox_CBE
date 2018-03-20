<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="CustomColors.aspx.cs" Inherits="CheckboxWeb.Styles.Charts.CustomColors" MasterPageFile="~/Dialog.Master" %>
<%@ Register Assembly="CheckboxWeb" Namespace="CheckboxWeb.Controls.Button" TagPrefix="btn" %>
<%@ Import Namespace="Checkbox.Web" %>

<asp:Content ID="content" runat="server" ContentPlaceHolderID="_pageContent">
    <%--

        <div style="float:left;width:120px;"><ckbx:MultiLanguageLabel ID="colorListLbl" runat="server" TextId="/pageText/styles/charts/customColors.aspx/colorList" /></div>
        <div style="float:left;width:555px;clear:both;">
            <asp:Repeater ID="_colorRepeater" runat="server" >
                <ItemTemplate>
                    <div style="padding:4px;clear:both;">
                        <div style="float:left;width:120px;"><asp:Label ID="_colorLbl" runat="server" SkinID="NormalWeightLabel" Text='<%# Container.DataItem.ToString() %>' /></div>
                        <div style="float:left;width:120px;"><telerik:RadColorPicker ID="_color" runat="server" PaletteModes="WebPalette,RGBSliders" ShowIcon="true" SelectedColor='<%# Checkbox.Common.Utilities.HexToColor(Container.DataItem.ToString()) %>' /></div>
                        <div style="float:left;"><ckbx:MultiLanguageImageButton ID="_deleteColor" runat="server" CommandName="DeleteColor" CommandArgument='<%# Container.DataItem.ToString() %>' SkinID="StyleEditorDeleteImage" ToolTipTextId="/pageText/styles/charts/cusotmColors.aspx/deleteColor" CausesValidation="false" /></div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
            <div style="clear:both;">
                <div style="float:left;width:600px;padding:4px;">
                    <div style="float:left;width:120px;"><asp:TextBox ID="_newColorTxt" runat="server" Width="80px" /></div>
                    <div style="float:left;width:120px;"><telerik:RadColorPicker ID="_newColor" runat="server" PaletteModes="WebPalette,RGBSliders" ShowIcon="true" OnColorChanged="NewColorChanged" AutoPostBack="true" /></div>
                    <div style="float:left;"><ckbx:MultiLanguageImageButton ID="_addColorBtn" runat="server" SkinID="StyleEditorAddProperty" TextId="/pageText/styles/charts/customColors.aspx/addNewColor" ToolTipTextId="/pageText/styles/charts/customColors.aspx/addNewColor" /></div>
                    <div style="float:left;"><asp:Panel ID="_colorRequired" runat="server" ><asp:RequiredFieldValidator ID="_colorRequiredValidator" runat="server" ControlToValidate="_newColorTxt" EnableClientScript="false" CssClass="error" ><%= WebTextManager.GetText("/pageText/styles/forms/styleBuilder.aspx/nameRequired") %></asp:RequiredFieldValidator></asp:Panel></div>
                </div>
            </div>
          </div>
      <div style="clear:both;">
        <btn:CheckboxButton ID="_save" runat="server" CssClass="ckbxButton roundedCorners border999 shadow999 orangeButton" TextId="/common/save" CausesValidation="false" />
        &nbsp;&nbsp;&nbsp;
        <btn:CheckboxButton ID="_close" runat="server" CssClass="ckbxButton roundedCorners border999 shadow999 redButton" TextId="/common/cancel" CausesValidation="false" OnClientClick="Close();return false;" />
      </div> --%>
</asp:Content>