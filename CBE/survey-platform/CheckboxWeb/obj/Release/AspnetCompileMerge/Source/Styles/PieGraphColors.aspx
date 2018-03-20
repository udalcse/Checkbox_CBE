<%@ Page Title="" Language="C#" MasterPageFile="~/Dialog.Master" AutoEventWireup="false" CodeBehind="PieGraphColors.aspx.cs" Inherits="CheckboxWeb.Styles.PieGraphColors" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="_headContent" runat="server">
<style type="text/css">
    table tr, table td { vertical-align:top; }
</style>

<script type="text/javascript">
    $(document).ready(function () {
        $("#<%=_pieGraphColorsTxtList.ClientID %>").keypress(function (e) {
            //Stop propagation of keypress event because in this case it'll be handled be Dialog.Master js-code.
            e.stopPropagation();
        });
    });
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="_pageContent" runat="server">
<div style="margin:5px">
    <table cellspacing="1" cellpadding="1">
        <tr valign="top">
            <td><asp:PlaceHolder ID="_selectedColors" runat="server"></asp:PlaceHolder></td>
            <td><asp:TextBox ID="_pieGraphColorsTxtList" runat="server" CssClass="PrezzaNormal" style="overflow:hidden; " TextMode="MultiLine" Wrap="true" Rows="29" Columns="12" Width="89px"></asp:TextBox> </td>
            <td><asp:PlaceHolder ID="paletteTable" runat="server"></asp:PlaceHolder></td>
        </tr>
    </table>
    <table>
        <tr valign="top" style="vertical-align: top;">
            <td valign="middle"><asp:ImageButton ID="refreshColorListBtn" runat="server" ImageUrl="~/App_Themes/CheckboxTheme/Images/ref_16.gif" /></td>
            <td valign="middle"><ckbx:MultiLanguageLabel runat="server" ID="_refreshBtn" TextId="/pageText/PieGraphColors.aspx/refresh">Refresh the color list</ckbx:MultiLanguageLabel></td>
        </tr>
    </table>
</div>    
</asp:Content>
