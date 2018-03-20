<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="ReportEditor.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.Controls.ReportEditor" %>
<%@ Import Namespace="Checkbox.Forms" %>
<%@ Import Namespace="Checkbox.Management" %>
<%@ Import Namespace="Checkbox.Web"%>

<script type="text/javascript" language="javascript" src="<%=ResolveUrl("~/Resources/templateEditor.js") %>"></script>

<script type="text/javascript">
    //Initialize editor
    $(document).ready(function () {
        //Initialize editor JS
        templateEditor.initialize('<%=ApplicationManager.ApplicationRoot %>');
    });

    //Open child window
    function openChildWindow(itemId, pageId, baseUrl) {
        var additionalParams = new Array();
        
        additionalParams.push(
            { name: 'r', value: '<%=Request.QueryString["r"] %>' }
        );

        templateEditor.openChildWindow(
            itemId,
            pageId,
            baseUrl,
            additionalParams,
            '<%=_childWindow.ClientID %>'
            );
    }

    //Queue up item renderer loading on page ready
    function queueLoadItemHtml(itemId, rendererPanelId, loadingPanelId) {
        templateEditor.queueLoadItemHtml(itemId, rendererPanelId, loadingPanelId, '<%=RenderMode.ReportEditor %>');
    }
</script>



<div class="reportFilters">
    <ckbx:MultiLanguageLabel ID="_appliedFiltersLbl" runat="server" CssClass="label" TextId="/controlText/forms/surveys/reports/reportEditor.ascx/reportFilters" />
    <br />
    <ckbx:MultiLanguageLabel ID="_noFiltersLbl" runat="server" TextId="/controlText/forms/surveys/reports/reportEditor.ascx/noFilters" />
    <asp:Label ID="_filterTxtLbl" runat="server" />
    <br /><br />
    <ckbx:MultiLanguageHyperLink ID="_selectReportFiltersLink" runat="server" TextId="/controlText/forms/surveys/reports/reportEditor.ascx/selectFilters" NavigateUrl='<%# "~/Forms/Surveys/Reports/ReportFilters.aspx?r=" + ReportTemplate.ID %>' />
</div>


<asp:Repeater ID="_pageRepeater" runat="server">
    <ItemTemplate>
        <div class="pageContainer">
            <div class="pageTitle">
                <ckbx:MultiLanguageLabel ID="_pageLbl" runat="server" TextId="/controlText/forms/surveys/reports/reportEditor.ascx/page">Page</ckbx:MultiLanguageLabel>
                &nbsp;
                <asp:Label ID="_pagePositionLbl" runat="server" />
            </div>
            <asp:Repeater ID="_itemRepeater" runat="server" OnItemCommand="itemRepeater_ItemCommand">
                <ItemTemplate>
                    <div class="itemContainer">
                        <div class="itemTitle">
                            Item <%# Container.ItemIndex + 1 %>:  <%# DataBinder.Eval(Container.DataItem, "ItemTypeName") %>
                        </div>
                        <div class="menu" style="height:100px;">
                            <a href="javascript:openChildWindow('<%# DataBinder.Eval(Container.DataItem, "ItemId") %>', '<%# DataBinder.Eval(Container.DataItem, "PageNumber") %>', 'EditItem.aspx');">Edit</a>
                            <br />
                            <a href="javascript:openChildWindow('<%# DataBinder.Eval(Container.DataItem, "ItemId") %>', '<%# DataBinder.Eval(Container.DataItem, "PageNumber") %>', 'CopyMoveItem.aspx');">Move/Copy</a>
                            <%--<a href="javascript:void(0);">Move/Copy</a>--%>
                            <br />
                            <ckbx:MultiLanguageLinkButton ID="_deleteItem" runat="server" TextId="/controlText/forms/surveys/surveyEditor.ascx/deleteItem" CommandName="DeleteItem" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ItemId") + "," + DataBinder.Eval(Container.DataItem, "PageNumber") %>' />
                        </div>
                        <div style="float:left;width:600px;">
                            <asp:Panel ID="_itemLoadingPanel" runat="server">
                                <asp:Image ID="_spinner" runat="server" SkinID="ProgressSpinner" />
                            </asp:Panel>
                            <asp:Panel runat="server" ID="_itemRendererPlace" />
                        </div>
                        <div style="clear:both;"></div>
                        
                        <div class="itemFilters">
                            <asp:Label ID="_itemFiltersLbl" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "FilterText") %>' />
                            <br />
                            <a href="javascript:openChildWindow('<%# DataBinder.Eval(Container.DataItem, "ItemId") %>', '<%# DataBinder.Eval(Container.DataItem, "PageNumber") %>', 'ItemFilters.aspx');"><%=WebTextManager.GetText("/controlText/forms/surveys/reports/reportEditor.ascx/selectFilters") %></a>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </ItemTemplate>
</asp:Repeater>

<telerik:RadWindowManager ID="_windowMgr" runat="server">
    <Windows>
        <telerik:RadWindow ID="_childWindow" runat="server" SkinID="LargeDialog" OnClientClose="templateEditor.handleItemClose"  />
        <telerik:RadWindow ID="_selectReportFilterWindow" runat="server" SkinID="LargeDialog" ReloadOnShow="true" NavigateUrl='<%# "~/Forms/Surveys/Reports/ReportFilters.aspx?r=" + ReportTemplate.ID %>' OpenerElementID='<%# _selectReportFiltersLink.ClientID %>' />
    </Windows>
</telerik:RadWindowManager>
