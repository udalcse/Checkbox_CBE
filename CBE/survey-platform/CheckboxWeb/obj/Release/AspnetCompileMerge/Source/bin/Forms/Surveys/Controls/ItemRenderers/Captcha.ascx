<%@ Control Language="C#" CodeBehind="Captcha.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.Captcha" %>
<%@ Register TagPrefix="ckbx" TagName="QuestionText" Src="~/Forms/Surveys/Controls/ItemRenderers/QuestionText.ascx" %>
<%@ Import Namespace="System.IO"%>
<%@ Import Namespace="Checkbox.Common.Captcha.Sound"%>
<%@ Import Namespace="Checkbox.Web"%>
<%@ Import Namespace="Checkbox.Common"%>
<%@ Import Namespace="Checkbox.Web.Forms.UI.Rendering" %>

<asp:Panel ID="_containerPanel" runat="server" CssClass="itemContainer">
    <asp:Panel ID="_contentPanel" runat="server" CssClass="itemContent">
        <asp:Panel ID="_topAndOrLeftPanel" runat="server" CssClass="topAndOrLeftContainer">
            <asp:Panel ID="_textContainer" runat="server" CssClass="textContainer">
                <ckbx:QuestionText ID="_questionText" runat="server" />
            </asp:Panel>
        </asp:Panel>

        <asp:Panel ID="_bottomAndOrRightPanel" runat="server" CssClass="bottomAndOrRightContainer">
            <asp:Panel ID="_inputPanel" runat="server" CssClass="inputContainer">
                <asp:Panel ID="_imageContainer" runat="server">
                    <asp:Image 
                        ID="_captchaImage" 
                        runat="server" 
                        ToolTip='<%# WebTextManager.GetText("/controlText/captchaControl/imageTip", Model.LanguageCode) %>' />

                    <asp:ImageButton 
                        ID="_playBtn"
                        runat="server" 
                        ToolTip='<%#WebTextManager.GetText("/controlText/captchaControl/soundTip", Model.LanguageCode) %>'
                        ImageUrl="Images/speaker.gif" />
                </asp:Panel>
                <br />
                
                <%-- Answer Input --%>
                <asp:TextBox ID="_inputTxt" runat="server" />
                
                <%-- Validation Message --%>
                <asp:Panel ID="_errorPanel" CssClass="Error" runat="server">
                    <asp:Label ID="_errorMsgLbl" runat="server" CssClass="Error"/>
                </asp:Panel>
            </asp:Panel>
        </asp:Panel>
    </asp:Panel>
</asp:Panel>



<script language="C#" runat="server">
    
    /// <summary>
    /// 
    /// </summary>
    public override List<UserControlItemRendererBase> ChildUserControls
    {
        get
        {
            var childControls = base.ChildUserControls;
            childControls.Add(_questionText);
            return childControls;
        }
    }
    
    /// <summary>
    /// Initialize child user controls to set repeat columns and other properties
    /// </summary>
    protected override void InlineInitialize()
    {
        SetLabelPosition();
        SetItemPosition();
    }

    /// <summary>
    /// Set size of container for item
    /// </summary>
    protected void SetContainerSize()
    {
    }

    /// <summary>
    /// Reorganize controls and/or apply specific styles depending
    /// on item's label position setting.
    /// </summary>
    protected void SetLabelPosition()
    {
        //When label is set to bottom, we need to move controls from the top panel
        // to the bottom panel.  Otherwise, position changes are managed by setting
        // CSS class.
        if ("Bottom".Equals(Appearance["LabelPosition"], StringComparison.InvariantCultureIgnoreCase))
        {
            //Move text controls to bottom
            _bottomAndOrRightPanel.Controls.Add(_textContainer);

            //Move input to top
            _topAndOrLeftPanel.Controls.Add(_inputPanel);
        }
        
        //Set css classes
        _topAndOrLeftPanel.CssClass = "topAndOrLeftContainer label" + (Utilities.IsNotNullOrEmpty(Appearance["LabelPosition"]) ? Appearance["LabelPosition"] : "Top");
        _bottomAndOrRightPanel.CssClass = "bottomAndOrRightContainer inputForLabel" + (Utilities.IsNotNullOrEmpty(Appearance["ItemPosition"]) ? Appearance["ItemPosition"] : "Top");
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

    /// <summary>
    /// Bind event for playing sound
    /// </summary>
    /// <param name="e"></param>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        _playBtn.Click += _playBtn_Click;
    }

    /// <summary>
    /// Play captcha sound
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void _playBtn_Click(object sender, ImageClickEventArgs e)
    {
        HttpContext context = HttpContext.Current;

        if (context == null || context.Response == null)
        {
            return;
        }

        context.Response.Clear();

        context.Response.AddHeader("content-disposition", "attachment; filename=" + "captcha.wav");
        context.Response.AddHeader("content-transfer-encoding", "binary");

        context.Response.ContentType = "audio/wav";

        SoundGenerator soundGenerator = new SoundGenerator(Model.InstanceData["Code"]);

        MemoryStream sound = new MemoryStream();

        // Write the sound to the response stream 
        soundGenerator.Sound.Save(sound, SoundFormatEnum.Wav);

        sound.WriteTo(context.Response.OutputStream);
    }

    /// <summary>
    /// Bind controls to model, which is the radiobuttonsitem
    /// </summary>
    protected override void InlineBindModel()
    {
        base.InlineBindModel();

        _captchaImage.ImageUrl = Model.InstanceData["ImageUrl"];

        bool enableSound;
        int imgHeight;
        int imgWidth;

        bool.TryParse(Model.Metadata["EnableSound"], out enableSound);
        int.TryParse(Model.Metadata["ImageHeight"], out imgHeight);
        int.TryParse(Model.Metadata["ImageWidth"], out imgWidth);
        
        
        _playBtn.Visible = enableSound;

        if (imgHeight > 0)
        {
            _imageContainer.Height = Unit.Pixel(imgHeight);
        }

        if (imgWidth > 0)
        {
            _imageContainer.Width = Unit.Pixel(imgWidth);
        }


        //Answer text
        _inputTxt.Text = GetTextAnswer();
        
        //Error messages
        _errorPanel.Visible = !Model.IsValid;
        _errorMsgLbl.Text = GetErrorMessageText();
    }

    /// <summary>
    /// Update model
    /// </summary>
    protected override void InlineUpdateModel()
    {
        base.InlineUpdateModel();

        UpsertTextAnswer(Request[_inputTxt.UniqueID] ?? _inputTxt.Text.Trim());
    }
   
</script>