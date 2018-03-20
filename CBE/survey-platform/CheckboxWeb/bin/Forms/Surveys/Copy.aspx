<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="Copy.aspx.cs" Inherits="CheckboxWeb.Forms.Surveys.Copy" MasterPageFile="~/Dialog.Master" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>
<%@ Register Assembly="CheckboxWeb" Namespace="CheckboxWeb.Controls.Button" TagPrefix="btn" %>
<%@ Import Namespace="Checkbox.Web"%>

<asp:Content runat="server" ID="_content" ContentPlaceHolderID="_pageContent">
    <div class="padding10">
        <div class="dialogSubTitle extended"><%=WebTextManager.GetText("/pageText/forms/surveys/copy.aspx/chooseAction") %></div>
        <div class="dialogSubContainer">
            <div class="formInput">
                <div class="left fixed_25 radioButton"><asp:RadioButton ID="_radMove" runat="server" AutoPostBack="true" GroupName="action" /></div>
                <p><label for="<%=_radMove.ClientID %>"><%=WebTextManager.GetText("/pageText/forms/surveys/copy.aspx/move") %></label></p>
            </div>
            
            <div class="formInput">
                <div class="left fixed_25 radioButton"><asp:RadioButton ID="_radCopy" runat="server" AutoPostBack="true" GroupName="action" /></div>
                <p><label for="<%=_radCopy.ClientID %>"><%=WebTextManager.GetText("/pageText/forms/surveys/copy.aspx/copy") %></label></p>
            </div>
            
            <asp:Panel ID="_copyNamePanel" runat="server" style="margin-left:25px;">
                <div class="formInput">
                    <p><label for="<%=_nameText.ClientID %>"><%=WebTextManager.GetText("/pageText/forms/surveys/copy.aspx/enterName", null, "Survey Name: ") %></label></p>
                    <asp:TextBox ID="_nameText" Width="300" runat="server" />
                </div>
                
            </asp:Panel>
        </div>
        
        <div class="dialogSubTitle extended"><%=WebTextManager.GetText("/pageText/forms/surveys/copy.aspx/chooseDestination") %></div>
        <div class="dialogSubContainer" style="margin-top:-10px;">
            <asp:DropDownList ID="_destinationList" runat="server"></asp:DropDownList>
        </div>
    </div>
</asp:Content>
