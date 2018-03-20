<%@ Page Language="C#" MasterPageFile="~/Dialog.Master" CodeBehind="Exceptions.aspx.cs" Inherits="CheckboxWeb.Settings.Exceptions"%>
<%@ Import Namespace="Checkbox.Web" %>
<%@ MasterType VirtualPath="~/Dialog.Master" %>

<asp:Content ID="settingContent" ContentPlaceHolderID="_pageContent" runat="server">
    <h3><ckbx:MultiLanguageLabel id="MultiLanguageLabel2" runat="server" CssClass="PrezzaBold" TextId="/pageText/settings/navigation.ascx/exceptionLog">Exception Log</ckbx:MultiLanguageLabel></h3>
           
        <div class="itemActionMenu">
            <div style="float:right;margin-top:2px;margin-left:10px;">
                <btn:CheckboxButton CssClass="ckbxButton roundedCorners border999 shadow999 redButton" ID="_deleteSelected" runat="server" TextId="/pageText/settings/exceptions.aspx/deleteSelected" OnClick="OnDeleteSelectedClick" />
            </div>
            <div style="float:right;margin-top:2px;margin-left:10px;">
                <btn:CheckboxButton CssClass="ckbxButton roundedCorners border999 shadow999 redButton" ID="_deleteAllBtn" runat="server" TextId="/pageText/settings/exceptions.aspx/deleteAll" OnClick="OnDeleteAllClick" />
            </div>
        </div>

        <br class="clear" />

        <script type="text/javascript">
            $(document).ready(
            function () {
                $(".HeaderRow input[name*='_exceptionsGrid']").attr("uframeignore", "true").change(
                    function () {
                        if ($(this).attr("checked")) {
                            $('.<%=CustomCheckBoxField.CheckboxesCss%>').find('input').attr('checked', 'checked');
                        }
                        else {
                            $('.<%=CustomCheckBoxField.CheckboxesCss%>').find('input').removeAttr('checked');
                        }
                        $.uniform.update($('.<%=CustomCheckBoxField.CheckboxesCss%>').find('input'));
                    }
                );
                $(".itemActionMenu a[name*='_deleteSelected']").attr("uframeignore", "true").click(function () { OnDeleteSelected(); return false; });
                $(".itemActionMenu a[name*='_deleteAllBtn']").attr("uframeignore", "true").click(function () { OnDeleteAll(); return false; });
            });

            function OnDeleteSelected() {
                if ($('.<%=CustomCheckBoxField.CheckboxesCss%> .checked').length > 0) {
                    showConfirmDialogWithCallback(
                        '<%=WebTextManager.GetText("/pageText/settings/exceptions.aspx/deleteSelectedConfirm") %>',
                        function () { UFrameManager.executeASPNETPostback($('#<%=_deleteSelected.ClientID %>'), $('#<%=_deleteSelected.ClientID %>').attr('href')); },
                        337,
                        200,
                        '<%=WebTextManager.GetText("/pageText/settings/exceptions.aspx/deleteSelectedTitle") %>'
                    );
                }
            }
            function OnDeleteAll() {
                if ($('.<%=CustomCheckBoxField.CheckboxesCss%>').length > 0) {
                    showConfirmDialogWithCallback(
                        '<%=WebTextManager.GetText("/pageText/settings/exceptions.aspx/deleteAllConfirm") %>',
                        function () { UFrameManager.executeASPNETPostback($('#<%=_deleteAllBtn.ClientID %>'), $('#<%=_deleteAllBtn.ClientID %>').attr('href')); },
                        337,
                        200,
                        '<%=WebTextManager.GetText("/pageText/settings/exceptions.aspx/deleteAllTitle") %>'
                    );
                }
            }
        </script>

        <div class="dashStatsContent">
            <asp:GridView ID="_exceptionsGrid" runat="server" Width="700"  AutoGenerateColumns="False" PageSize="25" AllowPaging="true" OnPageIndexChanging="OnExceptionsGridPageIndexChanging">
                <EmptyDataTemplate>
                    <ckbx:MultiLanguageLabel ID="_noEntriesLbl" runat="server" TextId="/pageText/settings/exceptions.aspx/noEntries" />
                </EmptyDataTemplate>
                <Columns>
                    <grd:LocalizedHeaderTemplateField HeaderTextID="/pageText/settings/exceptions.aspx/id">
                        <ItemTemplate>
                            <a href="ExceptionEntry.aspx?EntryID=<%# DataBinder.Eval(Container.DataItem, "EntryId") %>&PageIndex=<%#_exceptionsGrid.PageIndex %>&PageSize=<%#_exceptionsGrid.PageSize %>&uframe=1"><%# DataBinder.Eval(Container.DataItem, "EntryId") %></a>
                        </ItemTemplate>
                    </grd:LocalizedHeaderTemplateField>
                    <grd:CustomCheckBoxField HeaderTextID="/pageText/settings/exceptions.aspx/select" AllowSelectAll="true" DataField="Checked" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="25px" />
                    <grd:LocalizedHeaderBoundField HeaderTextID="/pageText/settings/exceptions.aspx/serverName" DataField="MachineName" />
                    <grd:LocalizedHeaderBoundField HeaderTextID="/pageText/settings/exceptions.aspx/message" DataField="Title" />
                    <grd:LocalizedHeaderBoundField HeaderTextID="/pageText/settings/exceptions.aspx/timestamp" DataField="DateTimeStamp" />
                </Columns>
            </asp:GridView>
        </div>
    <div class="dialogFormPush">&nbsp;</div>
</asp:Content>