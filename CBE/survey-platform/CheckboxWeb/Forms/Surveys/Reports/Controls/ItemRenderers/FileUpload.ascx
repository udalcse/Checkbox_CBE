<%@ Control Language="C#" CodeBehind="FileUpload.ascx.cs" Inherits="CheckboxWeb.Forms.Surveys.Reports.Controls.ItemRenderers.FileUpload" %>
<%@ Import Namespace="System.Collections.Generic"%>
<%@ Import Namespace="Checkbox.Wcf.Services.Proxies"%>
<%@ Import Namespace="Checkbox.Common"%>
<%@ Import Namespace="Checkbox.Web"%>

<telerik:RadScriptBlock ID="_scriptBlock" runat="server">
    <script type="text/javascript" language="javascript">
//        //Disable upload button
//        function disableUploadButton() {
//            $(document).ready(function() {
//                $('#<%=_uploadBtn.ClientID %>').attr('disabled', 'disabled');
//            });
//        }        
//        
//        //Handle file selection.  Requires that RadUploadHandler.js is included in page
//        function fileSelected(radUpload, eventArgs) {
//            if (validateSelectedFileExtension(radUpload, eventArgs)) {
//                hideValidationErrors();
//                $('#<%=_uploadBtn.ClientID %>').removeAttr('disabled');
//            }
//            else {
//                showValidationError('_extensionValidationError_<%=Model.ItemId %>');
//            }
//        }

//        //Show a particular validation error and hide others
//        function showValidationError(divToShow) {
//            //Hide rest of errors
//            hideValidationErrors();
//            
//            //Show particular error
//            $(document).ready(function() {
//                $('#' + divToShow).show();
//            });
//        }

//        //Hide all validation errors
//        function hideValidationErrors() {
//            $(document).ready(function(){
//                $('.validationError').hide();
//            });
//        }
    </script>
</telerik:RadScriptBlock>

<asp:Panel ID="_containerPanel" runat="server" CssClass="itemContainer">
    <asp:Panel ID="_contentPanel" runat="server" CssClass="itemContent">
        <asp:Panel ID="_topAndOrLeftPanel" runat="server" CssClass="topAndOrLeftContainer">
            <asp:Panel ID="_itemNumberContainer" runat="server" CssClass="itemNumberContainer" Visible='<%# ShowItemNumber %>'>
                <asp:Label ID="_itemNumberLbl" runat="server" CssClass="Question" Text='<% %>' />
            </asp:Panel>
            <asp:Panel ID="_textContainer" runat="server" CssClass="textContainer">
                <asp:Panel ID="_questionPanel" runat="server"  Visible='<%# Utilities.IsNotNullOrEmpty(Model.Text) ? true : false %>' CssClass="questionTextContainer Question" >
                    <asp:PlaceHolder 
                        runat="server" ID="_requiredIndicatorPlace"
                        Visible='<%# Model.AnswerRequired %>'>
                        <span class="requiredField"><%=WebTextManager.GetText("/common/surveyItemRequiredIndicator", Model.LanguageCode, "*")%></span>
                    </asp:PlaceHolder>
                    
                    <%= Model.Text %>
                </asp:Panel>

                <asp:Panel ID="_descriptionPanel" runat="server"  Visible='<%# Utilities.IsNotNullOrEmpty(Model.Description) ? true : false %>' CssClass="descriptionTextContainer Description">
                    <%= Model.Description %>
                </asp:Panel>
                
                <%-- Validation Message --%>
                <asp:Panel ID="_errorPanel" CssClass="Error" runat="server">
                    <asp:Label ID="_errorMsgLbl" runat="server" />
                </asp:Panel>
            </asp:Panel>
        </asp:Panel>

        <asp:Panel ID="_bottomAndOrRightPanel" runat="server" CssClass="bottomAndOrRightContainer">
            <asp:Panel ID="_inputPanel" runat="server">
                <asp:Panel ID="_uploadedFilePlace" runat="server">
                    <asp:Label ID="_uploadedFileNameLbl" runat="server" />
                    <br />
                    <ckbx:MultiLanguageButton ID="_clearBtn" runat="server" CssClass="SubmitButton" TextID="/controlText/UploadItemRenderer/clear" />
                </asp:Panel>
                
                <asp:Panel ID="_selectFilePlace" runat="server">
                    <telerik:RadUpload 
                        ID="_upload" 
                        runat="server" 
                        MaxFileInputsCount="1" 
                        Skin="Default" 
                        ControlObjectsVisibility="None"
                        InputSize="60" 
                        Width="600"
                        AllowedFileExtensions=".xml" />
                        
                        <% //Disable client side validation until making upload work w/jquery happens OnClientFileSelected="fileSelected" /> %>

                    <telerik:RadAjaxLoadingPanel ID="_loadingPanel" runat="server" Transparency="0" InitialDelayTime="200" CssClass="AjaxLoadingPanel" />

                    <div id="_extensionValidationError_<%=Model.ItemId%>" class="error" style="display:none">
                        <%=WebTextManager.GetText("/controlText/forms/surveys/surveyUploader.ascx/pleaseSelectXmlFile") %>
                    </div>

                    <div id="_invalidXmlError" class="error" style="display:none">
                        <%= WebTextManager.GetText("/controlText/forms/surveys/surveyUploader.ascx/fileNotValidXml") %>
                    </div>

                    <div id="_formatValidationError" class="error" style="display:none">
                        <%=WebTextManager.GetText("/controlText/forms/surveys/surveyUploader.ascx/fileNotCorrectFormat") %>
                    </div>
                
                    <ckbx:MultiLanguageButton ID="_uploadBtn" runat="server" TextID="/controlText/UploadItemRenderer/upload" CssClass="SubmitButton" />
                    
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
    /// Override on load to ensure necessary script present.
    /// </summary>
    /// <param name="e"></param>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (Page != null)
        {
            Page.ClientScript.RegisterClientScriptInclude("UploadHandler", ResolveUrl("~/Resources/RadUploadHandler.js"));
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

        //Error message
        _errorPanel.Visible = !Model.IsValid;
        _errorMsgLbl.Text = GetErrorMessageText();

        //Show/hide inputs
        _uploadedFilePlace.Visible = Model.Answers.Length > 0;
        _selectFilePlace.Visible = !_uploadedFilePlace.Visible;
        
        //Set allowed file types
        _fileTypesLbl.Text = Model.Metadata["AllowedFileTypesCSV"];

        string[] fileTypesArray = Model.Metadata["AllowedFileTypesCSV"].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        List<string> fileTypesList = new List<string>();


        foreach (string fileType in fileTypesArray)
        {
            fileTypesList.Add(fileType.Trim());
        }

        _upload.AllowedFileExtensions = fileTypesList.ToArray();

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

        if (Model is SurveyFileUploadItem
            && _upload.UploadedFiles.Count  > 0)
        {
            UploadedFile file = _upload.UploadedFiles[0];

            //Get uploaded file content as a byte array
            byte[] byteArray = new byte[file.InputStream.Length];
            file.InputStream.Read(byteArray, 0, byteArray.Length);

            ((SurveyFileUploadItem)Model).FileBytes = byteArray;
            Model.InstanceData["FileName"] = file.GetName();
            Model.InstanceData["FileType"] = file.GetExtension();
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
            _bottomAndOrRightPanel.Controls.Add(_itemNumberContainer);
            _bottomAndOrRightPanel.Controls.Add(_textContainer);

            //Move input to top
            _topAndOrLeftPanel.Controls.Add(_inputPanel);
        }

        //Set css classes
        _topAndOrLeftPanel.CssClass = "topAndOrLeftContainer label" + (Utilities.IsNotNullOrEmpty(Appearance["LabelPosition"]) ? Appearance["LabelPosition"] : "Top");
        _bottomAndOrRightPanel.CssClass = "bottomAndOrRightContainer inputForLabel" + (Utilities.IsNotNullOrEmpty(Appearance["LabelPosition"]) ? Appearance["LabelPosition"] : "Top");
    }

    /// <summary>
    /// Set item position.
    /// </summary>
    protected void SetItemPosition()
    {
        _containerPanel.CssClass = "itemContainer itemPosition" + (Utilities.IsNotNullOrEmpty(Appearance["ItemPosition"]) ? Appearance["ItemPosition"] : "Left");
    }
</script>