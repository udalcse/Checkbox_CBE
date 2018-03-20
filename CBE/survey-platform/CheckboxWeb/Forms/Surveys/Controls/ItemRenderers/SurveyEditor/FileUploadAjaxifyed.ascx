<%@ Control Language="C#" AutoEventWireup="false"  CodeBehind="FileUploadAjaxifyed.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.SurveyEditor.FileUploadAjaxifyed" %>
<%@ Register TagPrefix="ckbx" TagName="QuestionText" Src="~/Forms/Surveys/Controls/ItemRenderers/QuestionText.ascx" %>
<%@ Register TagPrefix="ckbx" TagName="FakeUploader" Src="~/Controls/FakeUploader.ascx" %>
<%@ Import Namespace="Checkbox.Globalization.Text" %>
<%@ Import Namespace="Checkbox.Wcf.Services.Proxies" %>
<%@ Import Namespace="Checkbox.Common" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Import Namespace="Checkbox.Web.Forms.UI.Rendering" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="CheckboxWeb" %>

<asp:Panel ID="_containerPanel" runat="server" CssClass="itemContainer">
    <script type="text/javascript">
        function clear_<%=Model.ItemId %>() {
            $('#<%=_uploadedFileNameTxt.ClientID %>').val('');
            $('#<%=_uploadedFilePathTxt.ClientID %>').val('');

            $('#<%= _uploadedFileName.ClientID %>').empty();
            $('#<%= _uploadedFilePanel.ClientID %>').hide();
            $("#<%= _inputPanel.ClientID %>").show();
            $("#<%= _inputPanel.ClientID %> .fileupload-buttonbar").show();
        }

        function onRespondentFileUploaded_<%=Model.ItemId %>(filename) {
            $('#<%=_uploadedFileNameTxt.ClientID %>').val(filename);

            $('#<%= _uploadedFileName.ClientID %>').text(filename);
            $('#<%= _uploadedFilePanel.ClientID %>').show();
            $("#<%= _inputPanel.ClientID %>").hide();
            
            $('#clearButton_<%=Model.ItemId %>').off('click').on('click', function () {
                clear_<%=Model.ItemId %>();
                return false;
            });
        }
    </script>
    
    <div id="_containerPanel_<%=Model.ItemId %>">
        <asp:Panel ID="_contentPanel" runat="server" CssClass="itemContent">
            <asp:Panel ID="_topAndOrLeftPanel" runat="server" CssClass="topAndOrLeftContainer">
                <asp:Panel ID="_textContainer" runat="server" CssClass="textContainer">
                    <ckbx:QuestionText ID="_questionText" runat="server" />
                </asp:Panel>
            </asp:Panel>
            <asp:Panel ID="_bottomAndOrRightPanel" runat="server" CssClass="bottomAndOrRightContainer">
                <div class="hidden" >
                    <asp:TextBox ID="_uploadedFileNameTxt" runat="server" />
                    <asp:TextBox ID="_uploadedFilePathTxt" runat="server" />
                </div>
                <asp:Panel ID="_uploadedFilePanel" runat="server" style="display: none;">
                    <asp:Panel ID="_uploadedFileName" runat="server" />
                    <a id="clearButton_<%=Model.ItemId %>" href="#" class="ckbxButton roundedCorners border999 shadow999 orangeButton" ><%= TextManager.GetText("/controlText/UploadItemRenderer/clear") %></a>
                </asp:Panel>
                <asp:Panel ID="_inputPanel" runat="server" CssClass="inputContainer" style="display: none">
                    <div id="_extensionValidationError_<%=Model.ItemId%>" class="error message" style="display: none">
                        <%=WebTextManager.GetText("/controlText/forms/surveys/surveyUploader.ascx/fileNotValidType")%>
                    </div>
                    <ckbx:FakeUploader ID="_uploader" runat="server" Width="600" SelectFilePromptTextId="/controlText/fileUploader/selectFile" />
                    <div>
                        <ckbx:MultiLanguageLabel ID="_allowedTypesLbl" runat="server" TextId="/controlText/UploadItemRenderer/allowedFileTypesDescription" />
                        <asp:Label ID="_fileTypesLbl" runat="server" />
                    </div>
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
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        _uploader.UploadedCallback = "onRespondentFileUploaded_"+ Model.ItemId;
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

        _fileTypesLbl.Text = Model.Metadata["AllowedFileTypesCSV"];

        if (Model.Answers.Length > 0)
        {
            _uploadedFileName.Controls.Add(new Literal {Text = Model.Answers[0].AnswerText});
            _uploadedFilePanel.Attributes.Remove("style");

            _uploadedFileNameTxt.Text = Model.Answers[0].AnswerText;
        }
        else
            _inputPanel.Attributes.Remove("style");
    }

    /// <summary>
    /// Update model with upload data
    /// </summary>
    protected override void InlineUpdateModel()
    {
        base.InlineUpdateModel();

        var fileName = Server.HtmlEncode(Request[_uploadedFileNameTxt.UniqueID]);
        var tempFilePath = Server.HtmlEncode(Request[_uploadedFilePathTxt.UniqueID]);

        if (!string.IsNullOrEmpty(fileName))
        {
            if(!string.IsNullOrEmpty(tempFilePath))
            {
                var bytes = Upload.GetDataForFile(HttpContext.Current, tempFilePath);

                Model.AdditionalData = new UploadItemAdditionalData { FileBytes = bytes };
                Model.InstanceData["FileName"] = fileName;
                Model.InstanceData["FileType"] = Path.GetExtension(fileName);                
            }
        }
        else
        {
            Model.AdditionalData = null;
            Model.Answers = new SurveyResponseItemAnswer[0];
        }
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
            _contentPanel.Style[HtmlTextWriterStyle.Display] = "inline-block";
    }
</script>