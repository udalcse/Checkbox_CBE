<%@ Page Language="C#" AutoEventWireup="False" CodeBehind="Properties.aspx.cs" Inherits="CheckboxWeb.Forms.Surveys.Properties" MasterPageFile="~/Dialog.Master" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>
<%@ Register Src="~/Forms/Surveys/Controls/Properties.ascx" TagPrefix="ckbx" TagName="SurveyProperties" %>

<asp:Content ID="_pageContent" runat="server" ContentPlaceHolderID="_pageContent">
    <div style="height:150px;">
        <ckbx:SurveyProperties ID="_properties" runat="server" />
        <div class="clear"></div>
    </div>
</asp:Content>