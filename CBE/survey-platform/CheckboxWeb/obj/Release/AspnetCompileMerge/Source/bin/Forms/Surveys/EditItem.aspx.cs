using System;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Web;
using Amazon.SimpleDB.Model;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Forms.Items.UI;
using Checkbox.Management;
using Checkbox.Web;
using Checkbox.Web.Page;
using Checkbox.Security;
using Checkbox.Wcf.Services;
using Checkbox.Users;
using Checkbox.Web.Forms.UI.Editing;
using Matrix = CheckboxWeb.Forms.Surveys.Controls.AppearanceEditors.Matrix;

namespace CheckboxWeb.Forms.Surveys
{
    /// <summary>
    /// Dialog for editing items.
    /// </summary>
    public partial class EditItem : SecuredPage
    {
        /// <summary>
        /// Control name that represents binding drop down list 
        /// </summary>
        private const string BindingControlName = "questionBindingSelector";

        private ResponseTemplate _responseTemplate;

        [QueryParameter("p", IsRequired = false)]
        public int? PageId { get; set; }

        [QueryParameter("i", IsRequired = true)]
        public int ItemId { get; set; }

        [QueryParameter("s", IsRequired = true)]
        public int ResponseTemplateId { get; set; }

        [QueryParameter("isNew")]
        public bool? IsNew { get; set; }

        [QueryParameter("l")]
        public string LanguageCode { get; set; }

        [QueryParameter("ro", "false")]
        public bool IsReadOnly { get; set; }

        [QueryParameter("mc")]
        public bool IsMatrixColumn { get; set; }

        [QueryParameter("added", IsRequired = false)]
        public bool IsJustAdded { get; set; }

        [QueryParameter("fromHtmlRedactor", IsRequired = false)]
        public bool IsFromHtmlRedactor { get; set; }

        [QueryParameter("itemId", IsRequired = false)]
        public int ItemIdtoBind { get; set; }

        [QueryParameter("fieldName", IsRequired = false)]
        public string ProfileFieldName { get; set; }

        [QueryParameter("command", IsRequired = false)]
        public string BindCommand { get; set; }

        private bool HasBindingControl => WebUtilities.HasRequestFormValue(BindingControlName);

        private int BindedProfileProperty {
            get
            {
                if (HasBindingControl)
                    return Convert.ToInt32(WebUtilities.GetRequestFormValueBySubstring(BindingControlName));

                return -1;
            }
        }

        /// <summary>
        /// Get page position
        /// </summary>
        public int PagePosition
        {
            get { return ResponseTemplate.GetPagePositionForItem(ItemId).Value; }
        }

        /// <summary>
        /// Get edit language
        /// </summary>
        public string EditLanguage
        {
            get
            {
                return Utilities.IsNotNullOrEmpty(LanguageCode)
                           ? LanguageCode
                           : ResponseTemplate.LanguageSettings.DefaultLanguage;
            }
        }

        /// <summary>
        /// Get response template being edited
        /// </summary>
        public ResponseTemplate ResponseTemplate
        {
            get
            {
                if (_responseTemplate == null)
                {
                    _responseTemplate = ResponseTemplateManager.GetResponseTemplate(ResponseTemplateId);

                    if (_responseTemplate == null)
                    {
                        throw new Exception(string.Format("Unable to load survey with id {0}.", ResponseTemplateId));
                    }
                }

                return _responseTemplate;
            }
        }

        /// <summary>
        /// Get/set data
        /// </summary>
        private ItemData Data { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is matrix.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is matrix; otherwise, <c>false</c>.
        /// </value>
        public bool IsMatrix => Data.ItemTypeName.Equals("Matrix");

        /// <summary>
        /// Gets item type name
        /// </summary>
        public string ItemTypeName => Data.ItemTypeName;

        /// <summary>
        /// Get/set appearance
        /// </summary>
        private AppearanceData Appearance { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            //remove current item on popup close event 
            if (Request.QueryString["cmd"] != null)
            {
                //Delete an empty item that has been added after choosing item type
                SurveyManagementServiceImplementation.DeleteSurveyItem(
                    UserManager.GetUserPrincipal(HttpContext.Current.User.Identity.Name),
                    ResponseTemplateId,
                    ItemId);
            }

            LoadDatePickerLocalized();
        }

        /// <summary>
        /// Bind item to editor.
        /// </summary>
        protected override void OnPageInit()
        {
            base.OnPageInit();

            IncludeJsLocalization = true;

            Page.Title = WebTextManager.GetText("/pageText/forms/surveys/editItem.aspx/editSurveyItem");
            
            Data = ResponseTemplate.GetItem(ItemId, true);

            if (Data == null)
            {
                throw new Exception(string.Format("Unable to load item with id {0}.", ItemId));
            }

            if (!IsPostBack)
                IsConfirmed = false;
    
            //Not all items have appearances, so it's ok for appearance data to be null
            Appearance = AppearanceDataManager.GetAppearanceDataForItem(ItemId);

            _itemEditor.IsMatrixColumn = IsMatrixColumn;

            //Initialize component on initial view of page.
            InitializeItemEditor();
            
            //Bind click events
            if (!IsReadOnly)
                Master.OkClick += _saveButton_Click;

            //Item already has been added -- refresh of the item list is needed
            if (IsNew.HasValue && IsNew.Value)
            {
                Master.CancelClick += _cancelButton_Click;

                //Update page/item view if just added item
                //If there is a matrix item, don't refresh item editor at this moment because it will clear the matrixClone in the Session.
                if (!IsPostBack && !(Data is MatrixItemData) && !IsFromHtmlRedactor && !IsJustAdded)
                {
                    Page.ClientScript.RegisterStartupScript(
                        GetType(),
                        "refreshEditor",
                        "$(document).ready(function () {onItemAdded(" + ItemId + "," + PageId + "); });",
                        true);
                }
            }


            Master.OkEnable = !IsReadOnly;

            //Prevent Enter-Key press handling
            Master.PreventEnterKeyBinding();
            Master.IsDialog = IsNew ?? false;
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitializeItemEditor()
        {
            _itemEditor.Initialize(
                ResponseTemplateId,
                PagePosition,
                Data,
                Appearance,
                EditLanguage,
                ResponseTemplate.LanguageSettings.SupportedLanguages,
                Page.IsPostBack,
                IsNew.HasValue && IsNew.Value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _cancelButton_Click(object sender, EventArgs e)
        {
            //Fire event on finish
            var isNewArg = IsNew.HasValue && IsNew.Value
                ? "true"
                : "false";

            var closeWindow = "closeWindow('templateEditor.onItemAdded', { op: 'addItem', result: 'ok', pageId: " + PageId + " });";

            ClientScript.RegisterClientScriptBlock(GetType(), "closeDialog", "okClick(" + ItemId + "," + ResponseTemplate.GetPageAtPosition(PagePosition).ID + ", " + isNewArg + "," + IsMatrixColumn.ToString().ToLower() + "); " + closeWindow + " ", true);
           
            //Delete an empty item that has been added after choosing item type
            SurveyManagementServiceImplementation.DeleteSurveyItem(
                UserManager.GetUserPrincipal(HttpContext.Current.User.Identity.Name),
                ResponseTemplateId,
                ItemId);
        }

        /// <summary>
        /// Determine if user confirm creating the empty item
        /// </summary>
        private bool IsConfirmed
        {
            get
            {
                bool temp;
                return bool.TryParse(_isConfirmed.Text, out temp) && temp;
            }
            set { _isConfirmed.Text = value.ToString(); }
        }

        /// <summary>
        /// Handle save click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _saveButton_Click(object sender, EventArgs e)
        {
            if (_itemEditor.Validate())
            {
                // if control has binding drop down list bind or unbind
                if (HasBindingControl)
                {
                    var isItemAlreadyBinded = ProfileManager.IsItemBindedTo(this.ItemId, BindedProfileProperty);
                    var connectedFieldName = ProfileManager.GetConnectedProfileFieldName(this.ItemId);
                    if (BindedProfileProperty > 0 && !isItemAlreadyBinded)
                    {
                        PropertyBindingManager.AddSurveyItemProfilePropertyMapping(this.ItemId, BindedProfileProperty);
                        if (this.Data.ItemTypeName.Equals("Matrix"))
                            UpdateMatrixAppearenceData(BindedProfileProperty, this.ItemId);
                      
                    }
                    else if (BindedProfileProperty == 0 && !string.IsNullOrWhiteSpace(connectedFieldName))
                    {
                        ProfileManager.UnbindItemFromProfileField(ItemId);
                    }
                }

                //Update data without post options (prevent duplicates)
                _itemEditor.UpdateData(false);
                String message;
                if (IsNew.HasValue && IsNew.Value && !IsConfirmed && !_itemEditor.DoesItemContainEnoughInformation(out message))
                {
                    ClientScript.RegisterClientScriptBlock(GetType(), "ShowConfirmMessage",
                                                                "$(function() { ShowConfirmMessage(\"" +
                                                                message + "\"); });",
                                                                true);
                    return;
                }

                //Update all data
                _itemEditor.UpdateData(true);

                IsConfirmed = false;

                //Cleanup appearance caches of the item
                AppearanceDataManager.CleanUpAppearanceDataCacheForItem(ItemId);

                //Save item & appearance data
                _itemEditor.SaveData();

                //Update template
                ResponseTemplate.SetItem(_itemEditor.ItemData, true);

                //Cleanup caches of the item
                ResponseTemplate.CleanupItemCaches(_itemEditor.ItemData.ID.Value);

                //Mark template updated
                ResponseTemplateManager.MarkTemplateUpdated(ResponseTemplate.ID.Value);


                if (ProfileManager.IsItemBindedTo(this.ItemId, BindedProfileProperty) && this.ItemTypeName.Equals("Matrix"))
                {
                    ProfileManager.ClearMatrixRowsAndColumns(this.ItemId);
                }


                //Fire event on finish
                var isNewArg = IsNew.HasValue && IsNew.Value
                    ? "true"
                    : "false";

                var closeWindow = "closeWindow('templateEditor.onItemAdded', { op: 'addItem', result: 'ok', pageId: " +
                                  PageId + " });";

                ClientScript.RegisterClientScriptBlock(GetType(), "closeDialog",
                    "okClick(" + ItemId + "," + ResponseTemplate.GetPageAtPosition(PagePosition).ID + ", " + isNewArg +
                    "," + IsMatrixColumn.ToString().ToLower() + "); " + closeWindow + " ", true);
            }
            else
            {
                var validationMessage = _itemEditor.GetValidationMessage();
                if (!string.IsNullOrEmpty(validationMessage))
                    Master.ShowStatusMessage(validationMessage, StatusMessageType.Error);
            }
        }

        private void UpdateMatrixAppearenceData(int profilePropertyId, int itemId)
        {
            var profileProperties = ProfileManager.GetPropertiesList();
            var bindedField = profileProperties.FirstOrDefault(prop => prop.FieldId == profilePropertyId && prop.BindedItemId.Any(item => item == itemId));

            if (bindedField != null)
            {
                var matrix = ProfileManager.GetMatrixField(bindedField.Name,
                    UserManager.GetCurrentPrincipal().UserGuid);
                
                var appearenceEditor = _itemEditor.AppearanceEditor as UserControlHostAppearanceEditor;
                var matrixAppearenceEditor = appearenceEditor?.Controls[0] as Matrix;
                matrixAppearenceEditor?.SetMatrixGridLines(matrix != null ? matrix.GridLines : "None");
              
            }
        }
    }
}
