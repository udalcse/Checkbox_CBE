<%@ Control Language="C#" CodeBehind="ScoreMessage.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.ScoreMessage" %>
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
    /// Initialize child user controls to set repeat columns and other properties
    /// </summary>
    protected override void InlineInitialize()
    {
        SetItemPosition();
        SetFont();
        
        if (Model != null)
            _messageText.Text = Model.Text;        
    }

    /// <summary>
    /// Set item position.
    /// </summary>
    protected void SetItemPosition()
    {
        _containerPanel.CssClass = "itemContainer itemPosition" + (Utilities.IsNotNullOrEmpty(Appearance["ItemPosition"]) ? Appearance["ItemPosition"] : "Left") + " ScoreMessage";

        if ("center".Equals(Appearance["ItemPosition"], StringComparison.InvariantCultureIgnoreCase))
        {
            _contentPanel.Style[HtmlTextWriterStyle.Display] = "inline-block";
        }
    }

    /// <summary>
    /// Set item position.
    /// </summary>
    protected void SetFont()
    {
        int fontSize;
        if (int.TryParse(Appearance["FontSize"], out fontSize))
            _contentPanel.Style[HtmlTextWriterStyle.FontSize] = fontSize + "px";
        
        if (!string.IsNullOrEmpty(Appearance["FontColor"]))
            _contentPanel.Style[HtmlTextWriterStyle.Color] = Appearance["FontColor"];            
    }
</script>

