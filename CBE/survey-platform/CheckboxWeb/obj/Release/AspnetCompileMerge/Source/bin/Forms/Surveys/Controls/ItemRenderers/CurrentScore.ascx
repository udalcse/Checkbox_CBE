<%@ Control Language="C#" CodeBehind="CurrentScore.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.CurrentScore" %>
<%@ Import Namespace="Checkbox.Common"%>


<asp:Panel ID="_containerPanel" runat="server" CssClass="itemContainer">
    <asp:Panel ID="_contentPanel" runat="server" CssClass="itemContent textContainer">
        <asp:Literal 
            ID="_messageText" 
            runat="server" 
            />
    </asp:Panel>
</asp:Panel>



<script language="C#" runat="server">
    /// <summary>
    /// 
    /// </summary>
    protected override void InlineBindModel()
    {
        base.InlineBindModel();

        _messageText.Text = Model.Text;
    }
    
    /// <summary>
    /// Initialize child user controls to set repeat columns and other properties
    /// </summary>
    protected override void InlineInitialize()
    {
        SetItemPosition();
    }

    /// <summary>
    /// Set item position.
    /// </summary>
    protected void SetItemPosition()
    {
        _containerPanel.CssClass = "itemContainer itemPosition" + (Utilities.IsNotNullOrEmpty(Appearance["ItemPosition"]) ? Appearance["ItemPosition"] : "Left");

        if ("center".Equals(Appearance["ItemPosition"], StringComparison.InvariantCultureIgnoreCase))
        {
            _contentPanel.Style[HtmlTextWriterStyle.Display] = "inline-block";
        }
    }
</script>

