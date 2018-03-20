<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Image.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.Image" %>
<%@ Import Namespace="Checkbox.Common" %>

<asp:Panel ID="_containerPanel" runat="server">
    <img src="<%= Model.InstanceData["ImagePath"] %>" alt="<%= Model.InstanceData["AlternateText"] %>" title="<%= Model.InstanceData["AlternateText"] %>" />
</asp:Panel>

<script type="text/C#" runat="server">

    /// <summary>
    /// 
    /// </summary>
    protected override void InlineInitialize()
    {
        base.InlineInitialize();

        SetItemPosition();
    }
    
    /// <summary>
    /// Set item position.
    /// </summary>
    protected void SetItemPosition()
    {
        _containerPanel.CssClass = "itemContainer itemPosition" + (Utilities.IsNotNullOrEmpty(Appearance["ItemPosition"]) ? Appearance["ItemPosition"] : "Left");

        //if ("center".Equals(Appearance["ItemPosition"], StringComparison.InvariantCultureIgnoreCase))
        //{
        //    _contentPanel.Style[HtmlTextWriterStyle.Display] = "inline-block";
        //}
    }
</script>