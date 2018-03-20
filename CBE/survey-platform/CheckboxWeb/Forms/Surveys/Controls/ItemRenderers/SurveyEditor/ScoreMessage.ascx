<%@ Control Language="C#" CodeBehind="ScoreMessage.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.SurveyEditor.ScoreMessage" %>
<%@ Import Namespace="Checkbox.Common" %>
<%@ Import Namespace="Checkbox.Web" %>

<asp:Panel ID="_containerPanel" runat="server">
    <asp:Panel ID="_contentPanel" runat="server">
        <%=GetMessagesText() %>
    </asp:Panel>
</asp:Panel>

<div class="clear;"></div>

<script type="text/C#" runat="server">
    /// <summary>
    /// Initialize child user controls to set repeat columns and other properties
    /// </summary>
    protected override void InlineInitialize()
    {
        _containerPanel.Style[HtmlTextWriterStyle.Padding] = "5px";
        
        SetItemPosition();
        SetFont();
    }    

    protected string GetMessagesText()
    {
        if (string.IsNullOrEmpty(Model.Metadata["ScoreMessages"]))
        {
            return WebTextManager.GetText("/controlText/scoreMessageItemEditor/noMessages");
        }

        return Model.Metadata["ScoreMessages"];
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