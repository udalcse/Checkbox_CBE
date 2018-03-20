<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="Create.aspx.cs" Inherits="CheckboxWeb.Forms.Surveys.Create" MasterPageFile="~/Dialog.Master" %>

<%@ Import Namespace="Checkbox.Web" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>
<%@ Register Src="~/Forms/Surveys/Controls/Properties.ascx" TagPrefix="ckbx" TagName="SurveyProperties" %>

<asp:Content ID="_content" ContentPlaceHolderID="_pageContent" runat="server">
    <div class="padding10 detailedCreateSurveyDialog">
        <ckbx:SurveyProperties IsDetailedForm="True" ID="_properties" runat="server" />

        <div class="clear"></div>
        <% if (CanAssignStyles)
           { %>

        <div class="dialogSubTitle"><%=WebTextManager.GetText("/pageText/forms/surveys/import.aspx/styleTemplate")%></div>
        <div class="dialogSubContainer">
            <div class="formInput condensed">
                <p class="left fixed_150">
                    <label for="<%=_styleListPC.ClientID %>"><%=WebTextManager.GetText("/pageText/forms/surveystyle.aspx/stylePC")%></label></p>
                <div class="left" style="margin-right: 10px;">
                    <asp:DropDownList ID="_styleListPC" runat="server" DataTextField="Value" DataValueField="Key" Width="150px" />
                </div>
            </div>
            <div class="clear"></div>
            <div class="formInput condensed">
                <p class="left fixed_150">
                    <label for="<%=_styleListMobile.ClientID %>"><%=WebTextManager.GetText("/pageText/forms/surveystyle.aspx/mobileStyle")%></label></p>
                <div class="left" style="margin-right: 10px;">
                    <asp:DropDownList ID="_styleListMobile" runat="server" DataTextField="Name" DataValueField="StyleId" Width="150px" />
                </div>
            </div>
        </div>

        <div class="clear"></div>
        <%} %>

        <div class="dialogSubTitle"><%=WebTextManager.GetText("/pageText/forms/surveys/create.aspx/otherOptions")%></div>
        <div class="dialogSubContainer">
            <div class="formInput condensed">
                <p class="left fixed_150">
                    <label for="<%=_folderList.ClientID %>"><%=WebTextManager.GetText("/pagetext/forms/surveys/create.aspx/folder")%></label></p>
                <div class="left" style="margin-right: 10px;">
                    <asp:DropDownList ID="_folderList" runat="server" />
                </div>
            </div>
        </div>
        <div class="clear"></div>
    </div>
</asp:Content>