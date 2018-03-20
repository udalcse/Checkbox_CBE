<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="MatrixColumnsEditor.ascx.cs"  Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemEditors.MatrixColumnsEditor" %>
<%@ Import Namespace="Checkbox.Wcf.Services.Proxies" %>

<asp:Panel ID="_noColumnsPanel" runat="server" Visible="<%#MatrixItem.Columns.Length<=1%>">
    There are no columns.
</asp:Panel>
<asp:Panel ID="_inputPanel" runat="server" Visible="<%#MatrixItem.Columns.Length>1%>">
    <table id='<%=ClientID + "_matrix" %>' class="matrix" <%#String.Format("style='border: 1px solid gray; {0}'",String.IsNullOrEmpty(MatrixAppearance["Width"])? String.Empty : "width:"+MatrixAppearance["Width"]+"px" ) %>>
        <!--<thead>-->
        <tr id="_orderColumnHeaderWithEditing">
            <asp:Repeater ID="_editColumnsRepeater" runat="server" DataSource="<%#MatrixItem.Columns%>"
                OnItemDataBound="EditColumnRepeater_OnItemDataBound" OnItemCommand="EditColumnRepeater_ItemCommand">
                <ItemTemplate>
                    <asp:PlaceHolder ID="_controlsPlace" runat="server" />
                </ItemTemplate>
            </asp:Repeater>
        </tr>
        <%-- Column Text --%>
        <tr>
            <asp:Repeater ID="_columnHeaderRepeater" runat="server" DataSource='<%# MatrixItem.Columns %>'
                OnItemDataBound='ColumnHeaderRepeater_ItemDataBound'>
                <ItemTemplate>
                    <%-- DataItem is of type Checkbox.Wcf.Services.Proxies.MatrixItemColumn --%>
                    <asp:PlaceHolder ID="_controlsPlace" runat="server" />
                </ItemTemplate>
            </asp:Repeater>
        </tr>
        <%-- Scale Texts --%>
        <asp:PlaceHolder ID="_scalePlaceholder" runat="server" Visible="<%# DoesConsistRatingScaleColumn %>">
            <tr valign="bottom">
                <asp:Repeater ID="_columnScaleTextsRepeater" runat="server" DataSource='<%# MatrixItem.Columns %>'>
                    <ItemTemplate>
                        <%-- DataItem is of type Checkbox.Wcf.Services.Proxies.MatrixItemColumn --%>
                        <asp:PlaceHolder ID="_columnTextsPlace" runat="server" Visible='<%#((MatrixItemColumn)Container.DataItem).ColumnType.Equals("RadioButtonScale",StringComparison.InvariantCultureIgnoreCase)%>'>
                            <td colspan='<%#Math.Max(1,((MatrixItemColumn)Container.DataItem).ColumnType.Equals("DropDownList",StringComparison.InvariantCultureIgnoreCase) ? 1 : ((MatrixItemColumn)Container.DataItem).OptionTexts.Length) %>'
                                class='<%#GridLineClassForHeader%>'>
                                <table width="100%" style="border: 0 none black;">
                                    <tr>
                                        <td class='columnHeader' style="text-align:left">
                                            <%# ((MatrixItemColumn)Container.DataItem).ScaleStartText.Length==0 ? "&nbsp":((MatrixItemColumn)Container.DataItem).ScaleStartText%>
                                        </td>
                                        <td class='columnHeader' style="text-align:center">
                                            <%# ((MatrixItemColumn)Container.DataItem).ScaleMidText.Length == 0 ? "&nbsp" : ((MatrixItemColumn)Container.DataItem).ScaleMidText%>
                                        </td>
                                        <td class='columnHeader' style="text-align:right">
                                            <%# ((MatrixItemColumn)Container.DataItem).ScaleEndText.Length == 0 ? "&nbsp" : ((MatrixItemColumn)Container.DataItem).ScaleEndText%>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </asp:PlaceHolder>
                    </ItemTemplate>
                </asp:Repeater>
            </tr>
        </asp:PlaceHolder>
        <%-- Column Options --%>
        <asp:PlaceHolder ID="_columnOptionsPlaceholder" runat="server" Visible="<%#DoesConsistRatingScaleColumn || DoesConsistRadioButtonsColumn || DoesConsistCheckBoxesColumn %>">
            <tr>
                <asp:Repeater ID="_columnOptionsRepeater" runat="server" DataSource='<%# MatrixItem.Columns %>'
                    OnItemDataBound="ColumnOptionsRepeater_OnItemDataBound">
                    <ItemTemplate>
                        <%-- DataItem is of type Checkbox.Wcf.Services.Proxies.MatrixItemColumn --%>
                        <asp:PlaceHolder ID="_controlsPlace" runat="server" />
                    </ItemTemplate>
                </asp:Repeater>
            </tr>
        </asp:PlaceHolder>
        <%-- Empty row --%>
        <asp:PlaceHolder ID="_emptyRow" runat="server" Visible="<%#MatrixItem.Rows.Length==0%>">
            <tr>
                <asp:Repeater ID="_emptyRowRepeater" runat="server" DataSource="<%#MatrixItem.Columns %>"
                    OnItemDataBound="EmptryRowRepeater_OnItemDataBound">
                    <ItemTemplate>
                        <asp:PlaceHolder ID="_controlsPlace" runat="server" />
                    </ItemTemplate>
                </asp:Repeater>
            </tr>
        </asp:PlaceHolder>
        <%-- Rows --%>
        <asp:Repeater ID="_rowRepeater" runat="server" DataSource='<%#MatrixItem.Rows %>'>
            <ItemTemplate>
                <tr class='<%# GetRowClass(((MatrixItemRow) Container.DataItem).RowNumber)%>'>
                    <asp:Repeater ID="_columnRepeater" runat="server" DataSource='<%# MatrixItem.Columns %>'
                        OnItemDataBound="MatrixChildItemCreated">
                        <ItemTemplate>
                            <asp:PlaceHolder ID="_childControlsPlace" runat="server" />
                        </ItemTemplate>
                    </asp:Repeater>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
        <!--</tbody> -->
    </table>
</asp:Panel>
