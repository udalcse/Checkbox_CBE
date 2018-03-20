<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="LibraryEditor.ascx.cs" Inherits="CheckboxWeb.Libraries.LibraryEditor" %>
<%@ Register Assembly="CheckboxWeb" Namespace="CheckboxWeb.Controls.Button" TagPrefix="btn" %>
<%@ Import Namespace="Checkbox.Web" %>

<script type="text/javascript" language="javascript">
    function openEditorWindow(itemId, pageNumber) {
        var oWnd = $find("<%=_editItemWindow.ClientID%>");
        oWnd.setUrl('EditItem.aspx?i=' + itemId + '&p=' + pageNumber + '&id=<%# Request.QueryString["id"] %>');
        oWnd.show();
      }
</script>

<div class="padding10">
    <asp:Repeater ID="_pageRepeater" runat="server">
        <ItemTemplate>
            <div class="pageContainer">
                <asp:Repeater ID="_itemRepeater" runat="server" OnItemCommand="_itemRepeater_ItemCommand" >
                    <ItemTemplate>
                        <div class="surveyItemContainer">
                            <div class="itemTitle">
                                Item <%# Container.ItemIndex + 1 %>:  <%# DataBinder.Eval(Container.DataItem, "ItemTypeName") %>
                            </div>
                            <div class="menu" style="height:100px;">
                                <a href="javascript:openEditorWindow('<%# DataBinder.Eval(Container.DataItem, "ItemId") %>', '<%# DataBinder.Eval(Container.DataItem, "PageNumber") %>');">Edit</a>
                                <br />
                                <ckbx:MultiLanguageLinkButton ID="_copyItem" runat="server" TextId="/controlText/libraries/libraryEditor.ascx/copyItem" CommandName="CopyItem" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ItemId") %>' />
                                <br />
                                <ckbx:MultiLanguageLinkButton ID="_deleteItem" runat="server" TextId="/controlText/libraries/libraryEditor.ascx/deleteItem" CommandName="DeleteItem" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ItemId") %>' />
                            </div>
                            <div>
                                <asp:Placeholder runat="server" ID="_itemRendererPlace" />
                            </div>
                            <div style="clear:both;"></div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </ItemTemplate>
    </asp:Repeater>
</div>
