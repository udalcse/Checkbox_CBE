<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FileUpload.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.FileUpload" %>
<%@ Register TagPrefix="ckbx" TagName="QuestionText" Src="~/Forms/Surveys/Controls/ItemRenderers/QuestionText.ascx" %>
<%@ Import Namespace="Checkbox.Wcf.Services.Proxies" %>
<%@ Import Namespace="Checkbox.Common" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Import Namespace="Checkbox.Web.Forms.UI.Rendering" %>
<%@ Import Namespace="System.IO" %>

<asp:Panel ID="_containerPanel" runat="server" CssClass="itemContainer">
    <div id="_containerPanel_<%=Model.ItemId %>">
        <asp:Panel ID="_contentPanel" runat="server" CssClass="itemContent">
            <asp:Panel ID="_topAndOrLeftPanel" runat="server" CssClass="topAndOrLeftContainer">
                <asp:Panel ID="_textContainer" runat="server" CssClass="textContainer">
                    <ckbx:QuestionText ID="_questionText" runat="server" />
                </asp:Panel>
            </asp:Panel>
            <asp:Panel ID="_bottomAndOrRightPanel" runat="server" CssClass="bottomAndOrRightContainer">
                <asp:Panel ID="_inputPanel" runat="server" CssClass="inputContainer">
                    <asp:Panel ID="_uploadedFilePlace" runat="server">
                        <asp:Label ID="_uploadedFileNameLbl" runat="server" />
                        <br />
                        <ckbx:MultiLanguageButton ID="_clearBtn" runat="server" CssClass="ckbxButton roundedCorners border999 shadow999 orangeButton" TextId="/controlText/UploadItemRenderer/clear" />
                    </asp:Panel>
                    <asp:Panel ID="_selectFilePlace" runat="server">
                        <div id="_extensionValidationError_<%=Model.ItemId%>" class="error message" style="display: none">
                            <%=WebTextManager.GetText("/controlText/forms/surveys/surveyUploader.ascx/fileNotValidType")%>
                        </div>
                        <asp:FileUpload ID="_upload" runat="server" Width="600" />
                        <!--<ckbx:MultiLanguageButton ID="_uploadBtn" runat="server" TextId="/controlText/UploadItemRenderer/upload" CssClass="ckbxButton roundedCorners border999 shadow999 orangeButton" OnClientClick="return false;" uframeignore="true" />-->
                        <div>
                            <ckbx:MultiLanguageLabel ID="_allowedTypesLbl" runat="server" TextId="/controlText/UploadItemRenderer/allowedFileTypesDescription" />
                            <asp:Label ID="_fileTypesLbl" runat="server" />
                        </div>
                    </asp:Panel>
                </asp:Panel>
            </asp:Panel>
        </asp:Panel>
    </div>
</asp:Panel>

<script type="text/C#" runat="server">
    /// <summary>
    /// 
    /// </summary>
    /// <param name="e"></param>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        _clearBtn.Click += _clearBtn_Click;
    }

    /// <summary>
    /// 
    /// </summary>
    private bool IsFileCleared
    {
        set { Session[Model.ItemId + "_uploadFileCleared"] = value; }
        get { return (bool)(Session[Model.ItemId + "_uploadFileCleared"] ?? false); }
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void _clearBtn_Click(object sender, EventArgs e)
    {
        IsFileCleared = true;

        _uploadedFilePlace.Visible = false;
        _selectFilePlace.Visible = true;
    }
        
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

        _uploadedFileNameLbl.Text = Model.Answers.Length > 0
            ? Model.Answers[0].AnswerText
            : string.Empty;
    }

    /// <summary>
    /// Update model with upload data
    /// </summary>
    protected override void InlineUpdateModel()
    {
        base.InlineUpdateModel();

        if (_upload.HasFile)
        {
            Model.AdditionalData = new UploadItemAdditionalData { FileBytes = _upload.FileBytes };
            String fileName = Server.HtmlEncode(_upload.FileName);
            Model.InstanceData["FileName"] = fileName;
            Model.InstanceData["FileType"] = Path.GetExtension(fileName);
        }
        else if (IsFileCleared)
        {
            Model.AdditionalData = null;
            Model.Answers = new SurveyResponseItemAnswer[0];
        }

        IsFileCleared = false;
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