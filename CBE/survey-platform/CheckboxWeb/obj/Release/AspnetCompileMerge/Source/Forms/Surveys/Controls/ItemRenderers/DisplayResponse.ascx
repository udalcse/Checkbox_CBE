<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="DisplayResponse.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.DisplayResponse" %>
<%@ Import Namespace="Checkbox.Common" %>

<asp:Panel ID="_containerPanel" runat="server" CssClass="itemContainer">
     <asp:Panel ID="_inlineResponseContainer" runat="server">
        <asp:Literal ID="_htmlPlace" runat="server" />
    </asp:Panel>

    <asp:Panel ID="_linkContainer" runat="server">
        <asp:HyperLink ID="_responseLink" runat="server" Target="_blank"  />
    </asp:Panel>
</asp:Panel>

<script type="text/javascript">
    $(function () {
        $('#<%= _inlineResponseContainer.ClientID %> .Question, #<%= _inlineResponseContainer.ClientID %> .Answer').removeClass('Question Answer');
    });
</script>


<script type="text/C#" runat="server">
        /// <summary>
        /// Initialize child user controls to set repeat columns and other properties
        /// </summary>
        protected override void InlineInitialize()
        {
            SetItemPosition();

            string w = Model.AppearanceData["Width"];
            string h = Model.AppearanceData["Height"];

            int wi = 0;
            int hi = 0;

            if (int.TryParse(w, out wi))
                _containerPanel.Width = wi;

            if (int.TryParse(h, out hi))
                _containerPanel.Height = hi;
        }

    /// <summary>
    /// 
    /// </summary>
    protected override void InlineBindModel()
    {
        base.InlineBindModel();

        _htmlPlace.Controls.Clear();
        
        Visible = Model.Visible;

        _inlineResponseContainer.Visible = Utilities.AsBool(Model.Metadata["DisplayInForm"], false);
        _linkContainer.Visible = !_inlineResponseContainer.Visible;
        
        _htmlPlace.Text = Utilities.EncodeTagsInHtmlContent(Model.InstanceData["ResponseHtml"]);
        _responseLink.Text = Model.InstanceData["LinkText"];
        if (Page == null)
            _responseLink.NavigateUrl = Utilities.AdvancedHtmlDecode(Model.InstanceData["LinkUrl"]);
        else
            _responseLink.NavigateUrl = Server.HtmlDecode(Model.InstanceData["LinkUrl"]);
        
    }
    
    /// <summary>
    /// Set item position.
    /// </summary>
    protected void SetItemPosition()
    {
        _containerPanel.CssClass = "itemContainer itemPosition" + (Utilities.IsNotNullOrEmpty(Appearance["ItemPosition"]) ? Appearance["ItemPosition"] : "Left");
    }
</script>
