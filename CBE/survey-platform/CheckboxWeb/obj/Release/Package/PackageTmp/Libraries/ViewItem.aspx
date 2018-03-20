<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="ViewItem.aspx.cs" Inherits="CheckboxWeb.Libraries.ViewItem" MasterPageFile="~/Dialog.Master" %>
<%@ MasterType VirtualPath="~/Dialog.master" %>

<asp:Content ID="head" runat="server" ContentPlaceHolderID="_headContent">   
    <ckbx:ResolvingCssElement runat="server" Source="~/GlobalSurveyStyles.css" media="screen" />
    <ckbx:ResolvingCssElement runat="server" Source="~/ScreenSurveyStyles.css" media="screen" />
    <ckbx:ResolvingCssElement runat="server" Source="~/CustomSurveyStyles.css" media="screen" />
    
    <script type="text/javascript">
        $(document).ready(function () {
            $(".itemPreview").find($('.inputContainer')).css({
                overflow: ''
            });
        });
    </script>
</asp:Content>

<asp:Content ID="content" runat="server" ContentPlaceHolderID="_pageContent">
    <div class="padding15 itemPreview" style="overflow: auto; height: 340px;">
        <asp:PlaceHolder id="_itemPreviewPlace" runat="server"></asp:PlaceHolder>
    </div>
</asp:Content>

