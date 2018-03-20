<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="DefaultButton.ascx.cs" Inherits="CheckboxWeb.Controls.Button.DefaultButton" %>
<%@ Import Namespace="Checkbox.Management" %>
<%@ Import Namespace="Checkbox.Web" %>

<asp:LinkButton ID="_button" runat="server">
    <asp:Label ID="_buttonText" runat="server" />
</asp:LinkButton>

<script type="text/C#" runat="server">
    
    /// <summary>
    /// Get/set text id for button
    /// </summary>
    public string TextId { get; set; }

    /// <summary>
    /// Get/set tooltip text id for button
    /// </summary>
    public string ToolTipTextId { get; set; }

    /// <summary>
    /// Get/set width
    /// </summary>
    public Unit Width { get { return _button.Width; } set { _button.Width = value; } }

    /// <summary>
    /// Get/set height
    /// </summary>
    public Unit Height { get { return _button.Height; } set { _button.Height = value; } }

    /// <summary>
    /// Get/set client click event handler
    /// </summary>
    public string OnClientClick { get { return _button.OnClientClick; } set { _button.OnClientClick = value; } }

    /// <summary>
    /// Get/set css class for button
    /// </summary>
    public string CssClass { get { return _button.CssClass; } set { _button.CssClass = value; } }
    
    /// <summary>
    /// Configure button settings
    /// </summary>
    /// <param name="e"></param>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (!string.IsNullOrEmpty(TextId))
        {
            _buttonText.Text = WebTextManager.GetText(TextId);

            if (ApplicationManager.AppSettings.LanguageDebugMode && string.IsNullOrEmpty(_buttonText.Text))
            {
                _buttonText.Text = TextId;
            }
        }

        if (!string.IsNullOrEmpty(ToolTipTextId))
        {
            _button.ToolTip = WebTextManager.GetText(ToolTipTextId);
        }
    }
    
</script>
