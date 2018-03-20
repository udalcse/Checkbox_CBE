<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="Editor.aspx.cs" Inherits="CheckboxWeb.Styles.Charts.Editor" MasterPageFile="~/Admin.Master" %>
<%@ Register TagPrefix="ckbx" TagName="ChartPreview" Src="~/Styles/Controls/ChartPreview.ascx" %>
<%@ Register TagPrefix="ckbx" TagName="StatusControl" Src="~/Controls/Status/StatusControl.ascx" %>
<%@ Register Assembly="CheckboxWeb" Namespace="CheckboxWeb.Controls.Button" TagPrefix="btn" %>

<asp:Content ContentPlaceHolderID="_headContent" ID="headContent" runat="server" >
    <script type="text/javascript">
        function OnClientItemClicking(sender, args) {
            var multipage = $find("<%=_multiPage.ClientID%>");
            var id = args.get_item().get_index();
            multipage.set_selectedIndex(id);
        }
    </script>
</asp:Content>

<asp:Content ID="pageContent" runat="server" ContentPlaceHolderID="_pageContent" >
    <%--
    <div class="grid_12">
        <ckbx:StatusControl ID="_editorStatus" runat="server" />
        
        <div style="float:left;">
            <telerik:RadMultiPage ID="_multiPage" runat="server"
                SelectedIndex="0"
                OnPageViewCreated="_multiPage_PageViewCreated"
                Height="350px"
                Width="320px"
                BackColor="White"
                BorderColor="Silver"
                BorderStyle="Solid"
                BorderWidth="1px" />
            <telerik:RadPanelBar ID="_panelBar"
                                 runat="server"
                                 OnClientItemClicked="OnClientItemClicking"
                                 Width="322px" />
        </div>
        <div style="float:right;">
            <ckbx:ChartPreview ID="_chartPreview" runat="server" />
        </div>
         <div style="float:right">
        <btn:CheckboxButton ID="_save" runat="server" TextID="/common/save" CssClass="ckbxButton roundedCorners border999 shadow999 orangeButton" OnClick="_save_Click" />
        <btn:CheckboxButton ID="_cancel" runat="server" TextID="/common/cancel" CssClass="ckbxButton roundedCorners border999 shadow999 redButton" OnClick="_cancel_Click" />
        </div>
    </div> --%>
</asp:Content>