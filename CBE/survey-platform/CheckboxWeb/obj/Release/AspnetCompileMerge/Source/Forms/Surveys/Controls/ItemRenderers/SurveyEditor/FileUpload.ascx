<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="FileUpload.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.SurveyEditor.FileUpload" %>
<%@ Register TagPrefix="ckbx" TagName="QuestionText" Src="~/Forms/Surveys/Controls/ItemRenderers/SurveyEditor/QuestionText.ascx" %>
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
            <asp:Panel ID="_inputPanel" runat="server">
                <asp:Panel ID="_uploadedFilePlace" runat="server">
                    <asp:Label ID="_uploadedFileNameLbl" runat="server" />
                    <br />
                    <ckbx:MultiLanguageButton ID="_clearBtn" runat="server" CssClass="ckbxButton roundedCorners border999 shadow999 orangeButton" TextID="/controlText/UploadItemRenderer/clear" />
                </asp:Panel>
                
                <asp:Panel ID="_selectFilePlace" runat="server">
                    <input type="file" />

                    <!--<ckbx:MultiLanguageButton ID="_uploadBtn" runat="server" TextID="/controlText/UploadItemRenderer/upload" CssClass="ckbxButton roundedCorners border999 shadow999 orangeButton" OnClientClick="return false;" />-->
                    
                    <div>
                        <ckbx:MultiLanguageLabel ID="_allowedTypesLbl" runat="server" TextId="/controlText/UploadItemRenderer/allowedFileTypesDescription" />
                        <asp:Label ID="_fileTypesLbl" runat="server" />
                    </div>
                </asp:Panel>
            </asp:Panel>
        </asp:Panel>
    </asp:Panel>
</asp:Panel>

<script type="text/C#" runat="server">
    
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
    /// Bind to model
    /// </summary>
    protected override void InlineBindModel()
    {
        base.InlineBindModel();

        //Show/hide inputs
        _uploadedFilePlace.Visible = Model.Answers.Length > 0;
        _selectFilePlace.Visible = !_uploadedFilePlace.Visible;
        
        //Set allowed file types
        _fileTypesLbl.Text = Model.Metadata["AllowedFileTypesCSV"];

        string[] fileTypesArray = Model.Metadata["AllowedFileTypesCSV"].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        var fileTypesList = new List<string>();


        foreach (string fileType in fileTypesArray)
        {
            fileTypesList.Add(fileType.Trim());
        }

        _uploadedFileNameLbl.Text = Model.Answers.Length > 0
            ? Model.Answers[0].AnswerText
            : string.Empty;
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
