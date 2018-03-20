<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="ItemListFromLibrary.ascx.cs" Inherits="CheckboxWeb.Controls.AddItems.ItemListFromLibrary" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Register TagPrefix="grid" Namespace="Checkbox.Web.UI.Controls.GridTemplates" Assembly="Checkbox.Web" %>

<script type="text/javascript">
    //Select/Deselect all checkboxes
    function selectAllChangedFromLibrary(selectAllItem) {
        if ($(selectAllItem).prop("checked")) {
            $('.<%=CustomCheckBoxField.CheckboxesCss%>').find('input').prop('checked', true);
        }
        else {
            $('.<%=CustomCheckBoxField.CheckboxesCss%>').find('input').prop('checked', false);
        }

        $.uniform.update($('.<%=CustomCheckBoxField.CheckboxesCss%>').find('input'));
    }

    //Click Preview handle
    function onPreviewClick(itemId) {
        showDialog('<%=ResolveUrl("~/") %>Libraries/ViewItem.aspx?i='+itemId + '&l=<%=Server.UrlEncode(LanguageCode)%>', 'properties');
    }
</script>

<div class="padding10">
    <asp:Panel ID="_panelLibrariesNotFound" runat="server" CssClass="warning message" align="center">
        <%=WebTextManager.GetText("/controlText/ItemListFromLibrary.ascx/noLibrariesFound")%>
    </asp:Panel>

    <asp:Panel ID="_panelLibraries" CssClass="importToLibrariesSourcePanel" runat="server" >
        <div class="left">
            <h3><ckbx:MultiLanguageLabel ID="_chooseLibrary" runat="server" TextId="/controlText/ItemListFromLibrary.ascx/addItemFromLibrary" /></h3>
        </div>
        <div class="left librariesList" style="margin-left:15px;">
            <ckbx:MultiLanguageDropDownList ID="_libraryList" runat="server" AutoPostBack="true" OnSelectedIndexChanged="_libraryList_SelectedIndexChanged"/>
        </div>
        <br class="clear" />
    </asp:Panel>
    <div>&nbsp;</div>

    <asp:Panel ID="_panelEmptyItemList" runat="server" CssClass="warning message">
        <%=WebTextManager.GetText("/controlText/ItemListFromLibrary.ascx/noItemsFound")%>
    </asp:Panel>

    <asp:Panel ID="_panelItemList" runat="server">
        <div style="overflow-y: auto; overflow-x: hidden; height:330px;">
            <asp:GridView ID="_itemList" runat="server" AutoGenerateColumns="false" CssClass="DefaultGrid border999 shadow999" Width="100%">
                <HeaderStyle CssClass="HeaderRow" />
                <RowStyle CssClass="EvenRow" />
                <AlternatingRowStyle CssClass="OddRow" />
                <Columns>
                    <asp:BoundField DataField="ItemID" Visible="false"/>
                    <grid:CustomCheckboxField HeaderTextID="/asdfcontrolText/ItemListFromLibrary.ascx/select" AllowSelectAll="true" OnClientClick="selectAllChangedFromLibrary" DataField="Checked" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="25px" />
                    <grid:ItemTextField DataField="ItemID" HeaderTextID="/controlText/ItemListFromLibrary.ascx/itemText" ItemStyle-Width="570px"/>
                    <grid:LocalizedHeaderTemplateField HeaderTextID="/controlText/ItemListFromLibrary.ascx/itemType" ItemStyle-Width="125px" >
                        <ItemTemplate>
                            <asp:Label
                                id="_typeLbl"
                                runat="server"
                                Text='<%# WebTextManager.GetText("/itemType/" + Eval("ItemType") + "/name") %>' />
                        </ItemTemplate>
                    </grid:LocalizedHeaderTemplateField>
                    <grid:LocalizedHeaderTemplateField HeaderTextID="/controlText/ItemListFromLibrary.ascx/view" ItemStyle-Width="70px" ItemStyle-HorizontalAlign="center" HeaderStyle-HorizontalAlign="center">
                        <ItemTemplate>
                            <asp:ImageButton 
                                id="_viewBtn"
                                runat="server"
                                ImageUrl="~/App_Themes/CheckboxTheme/Images/button-details-sq.gif"                        
                                OnClientClick='<%# "onPreviewClick(" +Eval("ItemID") +"); return false;"%>'
                                ToolTip='<%# WebTextManager.GetText("/common/newWindow") %>'
                            />
                        </ItemTemplate>
                    </grid:LocalizedHeaderTemplateField>
                </Columns>
            </asp:GridView>
        </div>
        <div class="centerContent" style="margin-bottom:5px;width:180px;">
            <ckbx:PagingControl ID="_pager" runat="server" OnOnPagingChanged="_pager_OnPagingChanged"/>
        </div>
    </asp:Panel>
</div>
