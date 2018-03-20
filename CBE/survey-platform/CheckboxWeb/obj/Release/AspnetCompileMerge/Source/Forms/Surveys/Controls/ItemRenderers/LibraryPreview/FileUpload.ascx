<%@ Control Language="C#" AutoEventWireup="false" CodeBehind="FileUpload.ascx.cs"
    Inherits="CheckboxWeb.Forms.Surveys.Controls.ItemRenderers.LibraryEditor.FileUpload" %>
<%@ Register TagPrefix="ckbx" TagName="QuestionText" Src="~/Forms/Surveys/Controls/ItemRenderers/QuestionText.ascx" %>
<%@ Import Namespace="Checkbox.Wcf.Services.Proxies" %>
<%@ Import Namespace="Checkbox.Common" %>
<%@ Import Namespace="Checkbox.Web" %>
<%@ Import Namespace="Checkbox.Web.Forms.UI.Rendering" %>
<%@ Import Namespace="System.IO" %>
<script type="text/javascript" language="javascript">
          //Array of valid extensions
        var _validExtensions_<%=Model.ItemId %> = <%=GetArrayOfValidExtensions() %>;
        
        //Handle file selection.  Requires that RadUploadHandler.js is included in page
        function fileSelected_<%=Model.ItemId %>(radUpload) {
            if (validateSelectedFileExtension(radUpload, _validExtensions_<%=Model.ItemId %>)) {
                hideValidationErrors_<%=Model.ItemId %>();
                $('#<%=_uploadBtn.ClientID %>', "#_containerPanel_<%=Model.ItemId %>").removeAttr('disabled');
            }
            else {
                showValidationError('_extensionValidationError_<%=Model.ItemId %>');
                $('#<%=_uploadBtn.ClientID %>', "#_containerPanel_<%=Model.ItemId %>").attr('disabled', 'disabled');
            }
        }

        //Show a particular validation error and hide others
        function showValidationError(divToShow) {
            //Hide rest of errors
            hideValidationErrors_<%=Model.ItemId %>();
            
            //Show particular error
            $(document).ready(function() {
                $('#' + divToShow).show();
            });
        }

        //Hide all validation errors
        function hideValidationErrors_<%=Model.ItemId %>() {
            $(document).ready(function(){
                $('#_extensionValidationError_<%=Model.ItemId %>').hide();
            });
        }
</script>
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
                        <input type="file" style="width: 600px" onchange='<%# "fileSelected_" + Model.ItemId + "(this);"%>' />
                        <ckbx:MultiLanguageButton ID="_uploadBtn" runat="server" TextId="/controlText/UploadItemRenderer/upload"
                            CssClass="ckbxButton roundedCorners border999 shadow999 orangeButton" OnClientClick="return false;" uframeignore="true" />
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
    /// Override on load to ensure necessary script present.
    /// </summary>
    /// <param name="e"></param>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (Page != null)
        {
            RegisterClientScriptInclude("UploadHandler", ResolveUrl("~/Resources/FileUploadHandler.js"));
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
    /// Get jScript array of valid extensions
    /// </summary>
    /// <returns></returns>
    protected string GetArrayOfValidExtensions()
    {
        string[] fileTypes = Model.Metadata["AllowedFileTypesCSV"].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        string result = String.Empty;
        foreach (string fileType in fileTypes)
        {
            result += String.Format("'{0}', ", fileType.Trim());
        }

        if (fileTypes.Length > 0)
        {
            result = result.Substring(0, result.Length - 2);
        }

        return "[" + result + "]";
    }
</script>
