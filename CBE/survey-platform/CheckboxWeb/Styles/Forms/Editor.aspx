<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="Editor.aspx.cs" Inherits="CheckboxWeb.Styles.Forms.Editor" MasterPageFile="~/Admin.Master" %>
<%@ Register TagPrefix="ckbx" TagName="FormPreview" Src="~/Styles/Controls/StylePreview.ascx" %>
<%@ Register TagPrefix="ckbx" TagName="StatusControl" Src="~/Controls/Status/StatusControl.ascx" %>
<%@ Register Assembly="CheckboxWeb" Namespace="CheckboxWeb.Controls.Button" TagPrefix="btn" %>

<asp:Content ContentPlaceHolderID="_headContent" ID="_head" runat="server" >
    <script type="text/javascript">
        function OnClientItemClicking(sender, args) {
            var multipage = $find("<%=_styleEditor.ClientID%>");
            var id = args.get_item().get_index();
            multipage.set_selectedIndex(id);
        }
    </script>
</asp:Content>

<asp:Content ContentPlaceHolderID="_pageContent" ID="_content" runat="server" >
 
    <div class="grid_12">
        <ckbx:StatusControl ID="_editorStatus" runat="server" />
        
        <div style="float:left;">
            <telerik:RadMultiPage ID="_styleEditor"
                                  runat="server"
                                  SelectedIndex="0"
                                  Height="580px"
                                  Width="469px"
                                  BackColor="White"
                                  BorderColor="Silver"
                                  BorderStyle="Solid"
                                  BorderWidth="1px"
                                  OnPageViewCreated="_multiPage_PageViewCreated" />
            
            <telerik:RadPanelBar ID="_stylePanelBar"
                                 runat="server"
                                 Skin="Telerik"
                                 ExpandMode="SingleExpandedItem"
                                 OnClientItemClicking="OnClientItemClicking"
                                 Width="470px" />
      </div>
      <div style="float:right;">
        <ckbx:FormPreview ID="_stylePreview" runat="server" />
      </div>
	  <div style="float:right;">
        <btn:CheckboxButton ID="_save" runat="server" TextID="/common/save" CssClass="SubmitButton" OnClick="_save_Click" />
        <btn:CheckboxButton ID="_cancel" runat="server" TextID="/common/cancel" CssClass="CancelButton" OnClick="_cancel_Click" />
      </div>
    </div>
</asp:Content>