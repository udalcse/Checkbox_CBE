<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="AppliedFilterDisplay.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.Controls.AppliedFilterDisplay" %>
<%@ Import Namespace="Checkbox.Analytics.Filters.Configuration" %>
<%@ Import Namespace="Checkbox.Web" %>


<div style="font-weight:bold;">
    <%=WebTextManager.GetText("/pageText/filterSelector.aspx/associatedFilters")%>
</div>
<asp:Panel ID="_viewPanel" runat="server" style="margin-left:25px;">
    <asp:ListView ID="_filterListView" runat="server" >
        <LayoutTemplate>
            <ul>
                <asp:PlaceHolder ID="itemPlaceholder" runat="server" />
            </ul>
        </LayoutTemplate>
        <ItemTemplate>
            <li>
                <%# GetFilterText(Container.DataItem)%>
            </li>
        </ItemTemplate>
        <EmptyDataTemplate><%=WebTextManager.GetText("/pageText/filterSelector.aspx/noFiltersApplied")%></EmptyDataTemplate>
    </asp:ListView>
</asp:Panel>


<script type="text/C#" runat="server">
    /// <summary>
    /// 
    /// </summary>
    /// <param name="dataItem"></param>
    /// <returns></returns>
    protected string GetFilterText(object dataItem)
    {
        if (dataItem is FilterData)
        {
            return ((FilterData) dataItem).ToString(LanguageCode);
        }
        
        return dataItem.ToString();
    }
</script>    

