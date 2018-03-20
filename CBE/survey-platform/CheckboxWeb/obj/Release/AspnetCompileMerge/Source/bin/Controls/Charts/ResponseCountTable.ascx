<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="ResponseCountTable.ascx.cs" Inherits="CheckboxWeb.Controls.Charts.ResponseCountTable" %>
<%@ Import Namespace="Checkbox.Web" %>

<div style="text-align:center;">
    <asp:GridView EnableViewState="false" ID="_gridView" runat="server" CellPadding="2" AutoGenerateColumns="false" CssClass="matrix Matrix" GridLines="Both" HeaderStyle-CssClass="header" RowStyle-CssClass="Item" AlternatingRowStyle-CssClass="AlternatingItem" style="margin-left:auto;margin-right:auto;width:600px;">
    </asp:GridView>
</div>

<script type="text/C#" runat="server">
    protected int ResponseCount
    {
        get
        {
            if (Model.SourceItems.Any())
                return  (from si in Model.SourceItems select si.ResponseCount).Max();
            
            return 0;
        }
    }

    /// <summary>
    /// Initialize grid/text
    /// </summary>
    protected override void InlineInitialize()
    {
        base.InlineInitialize();
        BoundField col = null;
        col = new BoundField() { HeaderText = WebTextManager.GetText("/itemType/responseCountTable/totalResponseCount")};
        col.HeaderStyle.HorizontalAlign=HorizontalAlign.Left;        
        _gridView.Columns.Add(col);
        col = new BoundField() { HeaderText = ResponseCount.ToString() };
        col.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
        _gridView.Columns.Add(col);
        _gridView.DataSource = new object[] { };
        _gridView.ShowHeaderWhenEmpty = true;
        _gridView.DataBind();            
    }
    
</script>