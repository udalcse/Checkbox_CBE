<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="Calculator.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.Calculator" %>
<%@ Register TagPrefix="ckbx" TagName="QuestionText" Src="~/Forms/Surveys/Controls/ItemRenderers/QuestionText.ascx" %>
<%@ Import Namespace="Checkbox.Management"%>
<%@ Import Namespace="System.Globalization"%>
<%@ Import Namespace="Checkbox.Forms.Items"%>
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
                <asp:Label ID="_calculationResult" runat="server"></asp:Label>
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
    /// Initialize child user controls to set repeat columns and other appearance properties
    /// </summary>
    protected override void InlineInitialize()
    {
        //Item and label position
        SetLabelPosition();
        SetItemPosition();
    }


    /// <summary>
    /// Bind controls to survey item.
    /// </summary>
    protected override void InlineBindModel()
    {
        base.InlineBindModel();

        //Input properties, such as showing/hiding proper input as well
        // as width and any restrictions based on answer format
        SetInputProperties();
        if (Model.Answers.Length > 0)
        {
            _calculationResult.Text = Model.Answers[0].AnswerText;
        }
    }

    /// <summary>
    /// Update model
    /// </summary>
    protected override void InlineUpdateModel()
    {
        base.InlineUpdateModel();

        //Set text as answer
        //Get answer from whatever input is visible
        UpsertTextAnswer(_calculationResult.Text);
    }

    /// <summary>
    /// Get answer format from model
    /// </summary>
    /// <returns></returns>
    private AnswerFormat GetAnswerFormat()
    {
        AnswerFormat answerFormat = AnswerFormat.None;

        if (Utilities.IsNotNullOrEmpty(Model.Metadata["AnswerFormat"]))
        {
            try
            {
                answerFormat = (AnswerFormat)Enum.Parse(typeof(AnswerFormat), Model.Metadata["AnswerFormat"]);
            }
            catch
            {
            }
        }

        return answerFormat;
    }

    /// <summary>
    /// Set accepted value for inputs, based on format
    /// </summary>
    protected void SetInputProperties()
    {
        //By default, text input is visible and others are hidden
    }

    /// <summary>
    /// Set associated control id for 508 input
    /// </summary>
    /// <param name="e"></param>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        _questionText.SetAssociatedInputId(_calculationResult.ClientID);
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
   
</script>