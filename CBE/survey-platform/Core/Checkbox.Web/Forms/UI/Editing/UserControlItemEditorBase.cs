using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using Checkbox.Forms;
using Checkbox.Forms.Items;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Globalization.Text;
using Checkbox.Web.Forms.UI.Rendering;
using Checkbox.Security.Principal;
using Checkbox.Users;
using Checkbox.Wcf.Services.Proxies;

namespace Checkbox.Web.Forms.UI.Editing
{
    /// <summary>
    /// Base class for user control item editor
    /// </summary>
    public abstract class UserControlItemEditorBase : Checkbox.Web.Common.UserControlBase, IItemEditor
    {
        private ItemTextDecorator _textDecorator;
        
        /// <summary>
        /// Session key for in-memory object being edited
        /// </summary>
        public const string InMemoryItemEditorSessionKey = "ItemEditorTextDecorator";

        /// <summary>
        /// Get data being edited
        /// </summary>
        public ItemData ItemData { get; protected set; }

        /// <summary>
        /// Get survey id
        /// </summary>
        public int TemplateId { get; private set; }

        /// <summary>
        /// Get/set survey page position
        /// </summary>
        public int PagePosition { get; set; }

        /// <summary>
        /// Get/set list of supported survey languages
        /// </summary>
        public List<string> SurveyLanguages { get; private set; }

        /// <summary>
        /// Get/set current language
        /// </summary>
        public string CurrentLanguage { get; private set; }

        /// <summary>
        /// Get/set whether this is a postback or not. Can be used when Page object is not always
        /// available, such as after a control is instantiated but not yet added to the page
        /// control hierarchy.
        /// </summary>
        public bool IsPagePostBack { get; set; }

        /// <summary>
        /// Return a boolean indicating whether this type of item editor may also
        /// contain an appearance editor.
        /// </summary>
        public virtual bool SupportsEmbeddedAppearanceEditor { get { return false; } }

        /// <summary>
        /// Return reference to rule editor, if any, that will be used to edit rules when edit mode is survey editor.
        /// </summary>
        public virtual IRuleEditor RuleEditor { get { return null; } }

        /// <summary>
        /// Return reference to control to display rule data
        /// </summary>
        public virtual IItemRuleDisplay RuleDisplay { get { return null; } }

        /// <summary>
        /// Get edit mode for item.
        /// </summary>
        public EditMode EditMode { get; private set; }

        /// <summary>
        /// Get control to contain preview, if any
        /// </summary>
        protected virtual Control PreviewContainer { get { return null; } }

        /// <summary>
        /// Get/set whether preview field should be shown
        /// </summary>
        public bool HidePreview { get; set; }

        /// <summary>
        /// Load the preview control
        /// </summary>
        /// <returns></returns>
        protected  virtual Control LoadPreviewControl()
        {
            RenderMode renderMode = RenderMode.SurveyEditor;

            if(EditMode == EditMode.Library)
            {
                renderMode = RenderMode.LibraryEditor;
            }

            if(EditMode == EditMode.Report)
            {
                renderMode = RenderMode.ReportEditor;
            }

            int? responseTemplate = EditMode == EditMode.Report ? (int?)null : TemplateId;
            int? analysisTemplate = EditMode == EditMode.Report ? TemplateId : (int?)null;

            var item = ItemData.CreateItem(CurrentLanguage, responseTemplate, analysisTemplate).GetDataTransferObject();

            if (item.TypeName == "Matrix")
                MatrixField.BindMatrixProfielFieldToItem((ItemProxyObject)item, renderMode, null);
            else if (item.TypeName == "RadioButtons")
                RadioButtonField.BindRadioProfileFieldToItem((ItemProxyObject)item, renderMode, null);

            return WebItemRendererManager.GetItemRenderer(item, renderMode, null);
        }


        /// <summary>
        /// Get text decorator for item being edited
        /// </summary>
        public ItemTextDecorator TextDecorator
        {
            get
            {
                if (_textDecorator == null
                    && ItemData != null)
                {
                    if (System.Web.HttpContext.Current.Session[InMemoryItemEditorSessionKey + EditMode + "_" + ItemData.ID] != null)
                    {
                        _textDecorator = (ItemTextDecorator)System.Web.HttpContext.Current.Session[InMemoryItemEditorSessionKey + EditMode + "_" + ItemData.ID];
                    }

                    if (_textDecorator == null
                        || !_textDecorator.Language.Equals(CurrentLanguage, StringComparison.InvariantCultureIgnoreCase))
                    {
                        _textDecorator = ItemData.CreateTextDecorator(CurrentLanguage);
                    }

                    if (_textDecorator != null)
                    {
                        _textDecorator.AddAlternateLanguages(SurveyLanguages);
                    }

                    System.Web.HttpContext.Current.Session[InMemoryItemEditorSessionKey + EditMode + "_" + ItemData.ID] = _textDecorator;
                }

                return _textDecorator;
            }

            protected set
            {
                System.Web.HttpContext.Current.Session[InMemoryItemEditorSessionKey + EditMode + "_" + ItemData.ID] = value;
                _textDecorator = value;
            }
        }

     
        /// <summary>
        /// 
        /// </summary>
        /// <param name="appearanceEditor"></param>
        private void ValidateAppearanceEditor(IAppearanceEditor appearanceEditor)
        {
            if (!SupportsEmbeddedAppearanceEditor)
            {
                throw new Exception("Control does not support embedded appearance editors.");
            }

            if (!(appearanceEditor is Control))
            {
                throw new Exception("Appearance editor to embed must be a Control.");
            }
        }

        /// <summary>
        /// Embed the specified appearance editor
        /// </summary>
        /// <param name="appearanceEditor"></param>
        public void EmbedAppearanceEditor(IAppearanceEditor appearanceEditor)
        {
            ValidateAppearanceEditor(appearanceEditor);

            if (appearanceEditor != null)
            {
                AddAppearanceEditorToControl(appearanceEditor);
            }
        }

        /// <summary>
        /// Add appearance editor to item editor control hierarchy.
        /// </summary>
        protected virtual void AddAppearanceEditorToControl(IAppearanceEditor appearanceEditor) { }
     
        /// <summary>
        /// Initialize the control with data to edit.
        /// </summary>
        /// <param name="templateId"></param>
        /// <param name="pagePosition"></param>
        /// <param name="data"></param>
        /// <param name="currentLanguage"></param>
        /// <param name="surveyLanguages"></param>
        /// <param name="isPagePostBack"></param>
        /// <param name="editMode"></param>
        /// <param name="hidePreview"></param>
        /// <param name="preventTextDecoratorReuse"></param>
        public virtual void Initialize(int templateId, int pagePosition, ItemData data, string currentLanguage, List<string> surveyLanguages, bool isPagePostBack, EditMode editMode, bool hidePreview, bool preventTextDecoratorReuse)
        {
            TemplateId = templateId;
            PagePosition = pagePosition;
            ItemData = data;
            CurrentLanguage = currentLanguage;
            SurveyLanguages = surveyLanguages;
            IsPagePostBack = isPagePostBack;
            EditMode = editMode;
            HidePreview = hidePreview;

            bool fromHtmlRedactor = Convert.ToBoolean(HttpContext.Current.Request.Params["fromHtmlRedactor"]);;
            bool isMatrix = Convert.ToBoolean(HttpContext.Current.Request.Params["isMatrix"]);

            //If posting back, attempt to use item from in-memory text decorator
            if (IsPagePostBack && TextDecorator != null)
            {
                ItemData = TextDecorator.Data;
            }

            //If not posting back, clear text decorator in memory to prevent accidental
            // reuse
            if (!IsPagePostBack && preventTextDecoratorReuse && !fromHtmlRedactor && !isMatrix)
            {
                TextDecorator = null;
            }

            //Initialize rule editor
            if (EditMode == EditMode.Survey)
            {
                if (RuleEditor != null)
                {
                    RuleEditor.Initialize(IsPagePostBack, ItemData.ID.Value, PagePosition, TemplateId, CurrentLanguage);
                }

                if (RuleDisplay != null)
                {
                    RuleDisplay.InitializeAndBind(ItemData.ID.Value, TemplateId, CurrentLanguage);
                }
            }

            UpdatePreview();
        }

        #region IDataEditor<ItemData> Members

        /// <summary>
        /// Initialize the control with data to edit.
        /// </summary>
        /// <param name="templateId"></param>
        /// <param name="pagePosition"></param>
        /// <param name="data"></param>
        /// <param name="currentLanguage"></param>
        /// <param name="surveyLanguages"></param>
        /// <param name="isPagePostBack"></param>
        /// <param name="editMode"></param>
        /// <param name="hidePreview"></param>
        public virtual void Initialize(int templateId, int pagePosition, ItemData data, string currentLanguage, List<string> surveyLanguages, bool isPagePostBack, EditMode editMode, bool hidePreview)
        {
            Initialize(templateId, pagePosition, data, currentLanguage, surveyLanguages, isPagePostBack, editMode, hidePreview, true);   
        }

        /// <summary>
        /// Update the preview
        /// </summary>
        protected virtual void UpdatePreview()
        {
            if (PreviewContainer != null)
            {
                if (HidePreview)
                    PreviewContainer.Visible = false;
                else
                {
                    var renderer = LoadPreviewControl();

                    if (renderer != null)
                    {
                        PreviewContainer.Controls.Clear();
                        PreviewContainer.Controls.Add(renderer);

                        //Databind for databinding expressions
                        renderer.DataBind();

                        //Bind control to model
                        if (renderer is IItemRenderer)
                        {
                            ((IItemRenderer) renderer).BindModel();
                        }
                    }
                }
            }

			bool mode = EditMode == EditMode.Survey || EditMode == EditMode.Report;

            //Update rule display
            if (mode && RuleDisplay != null)
            {
                RuleDisplay.InitializeAndBind(ItemData.ID.Value, TemplateId, CurrentLanguage);
            }
        }

        /// <summary>
        /// Validate the control
        /// </summary>
        /// <returns></returns>
        public virtual bool Validate()
        {
            return true;
        }

        public virtual void UpdateData(bool updateTextDecoratorOptions)
        {
            UpdateData();
        }

        /// <summary>
        /// Determine if user entered enough information.
        /// </summary>
        /// <param name="message">Contains a warning message, which information should be added to the item</param>
        /// <returns></returns>
        public virtual bool DoesItemContainEnoughInformation(out string message)
        {
            return DoesItemContainQuestionText(out message);// && DoesItemContainEnoughOptions(out message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        protected virtual bool DoesItemContainQuestionText(out string message)
        {
            message = null;

            var ltd = TextDecorator as LabelledItemTextDecorator;
            if (ltd != null && string.IsNullOrWhiteSpace(ltd.Text))
            {
                message = EmptyQuestionTextErrorText;
                return false;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        protected virtual bool DoesItemContainEnoughOptions(out string message)
        {
            message = null;

            var std = TextDecorator as SelectItemTextDecorator;
            if (std != null && std.Data.Options.Count == 0)
            {
                message = NoOptionsErrorText;
                return false;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual string EmptyQuestionTextErrorText
        {
            get
            {
                return TextManager.GetText("/pageText/forms/surveys/userControlItemEditor/confirmEmptyText");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual string NoOptionsErrorText
        {
            get
            {
                return TextManager.GetText("/pageText/forms/surveys/userControlItemEditor/confirmWithoutChoices");
            }
        }

        /// <summary>
        /// Update the control's data.
        /// </summary>
        public virtual void UpdateData()
        {
        }

        /// <summary>
        /// Save item data
        /// </summary>
        public virtual int SaveData()
        {            
            if (TextDecorator != null)
            {
                //Text decorator should save item data too, so no need for
                // separate call.
                TextDecorator.Save();

                //Update preview
                UpdatePreview();
                
                //Save rules
                if (EditMode == EditMode.Survey && RuleEditor != null)
                {
                    RuleEditor.SaveChanges();
                }

            }

            string modifiedBy = null;
            if (Page != null && Page.User != null && Page.User.Identity != null)
                modifiedBy = Page.User.Identity.Name;

            ItemData.ModifiedBy = modifiedBy;
            ItemData.Save();

            //Update preview
            UpdatePreview();

            //Save rules
            if (EditMode == EditMode.Survey && RuleEditor != null)
            {
                RuleEditor.SaveChanges();
            }

            return ItemData.ID.Value;

        }


        #endregion
    }
}
