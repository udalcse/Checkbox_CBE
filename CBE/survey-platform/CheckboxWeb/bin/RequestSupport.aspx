<%@ Page Title="" Language="C#" MasterPageFile="~/Admin.Master" AutoEventWireup="false" CodeBehind="RequestSupport.aspx.cs" Inherits="CheckboxWeb.RequestSupport" %>
<%@ MasterType VirtualPath="~/Admin.Master" %>

<asp:Content ID="page" runat="server" ContentPlaceHolderID="_pageContent">
    <asp:Panel ID="_warningPanel" runat="server" Visible="false" CssClass="padding10">
        <div class="warning message">
            <p style="margin-top:10px;margin-bottom:10px;">
                In order to use our support area you must first add an email address to your account.  
                
            </p>

            <p style="margin-top:10px;margin-bottom:10px;">
                For help, please contact our telephone support line at <b>+1 617-715-9600</b>.
            </p>
            <p style="margin-top:10px;margin-bottom:10px;">
                Our office hours are: <br />
                Monday - Friday, 8:00AM - 6:00PM <br />
                (Eastern Time Zone)
            </p>
        </div>
    </asp:Panel>
</asp:Content>
