<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="OthersTable.ascx.cs" Inherits="CheckboxWeb.Controls.Charts.OthersTable" %>
<%@ Import Namespace="Checkbox.Common" %>
<%@ Import Namespace="Checkbox.Globalization.Text" %>

<div style="text-align:center;">
    <asp:GridView EnableViewState="false" ID="_gridView" runat="server" CellPadding="2" AutoGenerateColumns="false" CssClass="matrix Matrix" GridLines="Both" HeaderStyle-CssClass="header" RowStyle-CssClass="Item" AlternatingRowStyle-CssClass="AlternatingItem" style="margin-left:auto;margin-right:auto;">
        <Columns>
            <grd:LocalizedHeaderBoundField DataField="ResultText" HeaderTextID="/controlText/frequencyItemRenderer/otherAnswers" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left" />
        </Columns>
    </asp:GridView>
</div>

<script type="text/C#" runat="server">
    /// <summary>
    /// Initialize grid/text
    /// </summary>
    protected override void InlineInitialize()
    {
        base.InlineInitialize();

        var width = Utilities.AsInt(Appearance["Width"]);

        if (width.HasValue)
        {
            _gridView.Width = Unit.Pixel(width.Value);
        }
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected string GetGridViewWidth()
    {
        if (string.IsNullOrEmpty(Appearance["Width"]))
        {
            return "width:600px;";
        }

        var width = Utilities.AsInt(Appearance["Width"]);

        if (!width.HasValue)
        {
            return "width:600px;";
        }
        
        return string.Format("width:{0};", Unit.Pixel(Math.Max(width.Value, 200)));
        
    }
    
    /// <summary>
    /// Bind grid to model
    /// </summary>
    protected override void InlineBindModel()
    {
        base.InlineBindModel();
            
        _gridView.DataSource = Model.DetailResults;
        _gridView.DataBind();       
        
    }


    /// <summary>
    /// Get language code for text
    /// </summary>
    public string LanguageCode
    {
        get
        {
            return string.IsNullOrEmpty(Model.InstanceData["LanguageCode"])
                ? TextManager.DefaultLanguage
                : Model.InstanceData["LanguageCode"];
        }
    }
    
</script>