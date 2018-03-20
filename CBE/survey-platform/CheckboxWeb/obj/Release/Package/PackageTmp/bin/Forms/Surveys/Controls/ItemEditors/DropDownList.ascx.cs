using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Web.Forms.UI.Editing;
using Checkbox.Forms;
using Checkbox.Web.Forms.UI.Rendering;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemEditors
{
    /// <summary>
    /// Item editor for user controls
    /// </summary>
    public partial class DropDownList : UserControlItemEditorBase
    {
        private ResponseTemplate _responseTemplate;
        private EditMode _editMode;

        /// <summary>
        /// Editor supports embedded editors
        /// </summary>
        public override bool SupportsEmbeddedAppearanceEditor { get { return true; } }
        
        /// <summary>
        /// 
        /// </summary>
        public override IRuleEditor RuleEditor { get { return _conditionEditor; } }

        /// <summary>
        /// 
        /// </summary>
        public override IItemRuleDisplay RuleDisplay { get { return _ruleDisplay; } }

        /// <summary>
        /// 
        /// </summary>
        protected override Control PreviewContainer
        {
            get { return _previewPlace; }
        }

        /// <summary>
        /// Add appearance editor to item control hierarchy.
        /// </summary>
        /// <param name="appearanceEditor"></param>
        protected override void AddAppearanceEditorToControl(IAppearanceEditor appearanceEditor)
        {
            _appearanceEditorPlace.Controls.Clear();
            _appearanceEditorPlace.Controls.Add((Control)appearanceEditor);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            //Load tab script
            RegisterClientScriptInclude(
                "jquery.ckbxtab.js",
                ResolveUrl("~/Resources/jquery.ckbxtab.js"));
        }

        /// <summary>
        /// Get response template
        /// </summary>
        private ResponseTemplate ResponseTemplate
        {
            get 
            {
                if (_editMode != Checkbox.Forms.EditMode.Survey)
                    return null;
                return _responseTemplate ??
                       (_responseTemplate = ResponseTemplateManager.GetResponseTemplate(TemplateId));
            }
        }


        /// <summary>
        /// Get SelectItemData
        /// </summary>
        private SelectItemData SelectItemData
        {
            get
            {
                if (HttpContext.Current.Session[ID + "_SelectItemData"] == null)
                {

                    HttpContext.Current.Session[ID + "_SelectItemData"] = ItemData as SelectItemData;

                    if (HttpContext.Current.Session[ID + "_SelectItemData"] == null)
                    {
                        throw new Exception("Unable to load select item data for item with id: " + ItemData.ID);
                    }
                }

                return HttpContext.Current.Session[ID + "_SelectItemData"] as SelectItemData;
            }
            set { HttpContext.Current.Session[ID + "_SelectItemData"] = value; }
        }


        /// <summary>
        /// Initialize item editor
        /// </summary>
        /// <param name="templateId"></param>
        /// <param name="pagePosition"></param>
        /// <param name="data"></param>
        /// <param name="currentLanguage"></param>
        /// <param name="surveyLanguages"></param>
        /// <param name="isPagePostBack"></param>
        /// <param name="editMode"></param>
        /// <param name="hidePreview"></param>
        public override void Initialize(int templateId, int pagePosition, ItemData data, string currentLanguage, List<string> surveyLanguages, bool isPagePostBack, EditMode editMode, bool hidePreview)
        {
            _editMode = editMode;

            // this item changes choices. if user clicks "Cancel", this data will be saved in cache. So 
            // this object must work with the clone of the data.
            if (!isPagePostBack)
            {
                SelectItemData = ((ICloneable) data).Clone() as SelectItemData;
            }

            base.Initialize(templateId, pagePosition, SelectItemData, currentLanguage, surveyLanguages, isPagePostBack, editMode, hidePreview);

            _activeDisplay.Initialize(data);

            _currentTabIndex.Text = HidePreview ? "1" : "0";

            //Initialize child editors
            if (TextDecorator is SelectItemTextDecorator)
            {
                _questionTextEditor.Initialize(((SelectItemTextDecorator)TextDecorator).Text, isPagePostBack, currentLanguage, templateId, pagePosition, editMode);
                _descriptionTextEditor.Initialize(((SelectItemTextDecorator)TextDecorator).SubText, isPagePostBack, currentLanguage, templateId, pagePosition, editMode);
                _selectOptionsEditor.Initialize((SelectItemTextDecorator)TextDecorator, isPagePostBack, ResponseTemplate, PagePosition, false, editMode);
                _behaviorEditor.Initialize(((SelectItemTextDecorator)TextDecorator), true, true, templateId, pagePosition, editMode);
            }
        }

        /// <summary>
        /// Update item data
        /// </summary>
        public override void UpdateData()
        {
            base.UpdateData();

            Update(true);
        }

        /// <summary>
        /// Update item data
        /// </summary>
        public override void UpdateData(bool updateTextDecoratorOptions)
        {
            Update(updateTextDecoratorOptions);
        }

        /// <summary>
        /// Update item data
        /// </summary>
        public void Update(bool updateTextDecoratorOptions)
        {
            base.UpdateData();

            if (TextDecorator is SelectItemTextDecorator)
            {
                ((SelectItemTextDecorator)TextDecorator).Text = _questionTextEditor.Text;
                ((SelectItemTextDecorator)TextDecorator).SubText = _descriptionTextEditor.Text;

                if (updateTextDecoratorOptions)
                    _selectOptionsEditor.UpdateData((SelectItemTextDecorator)TextDecorator);
                else
                    _selectOptionsEditor.UpdateOptionsInMemory();
                
                _behaviorEditor.UpdateData(((SelectItemTextDecorator)TextDecorator));
            }
        }

        /// <summary>
        /// Save item data
        /// </summary>
        /// <returns></returns>
        public override int SaveData()
        {
            int result = base.SaveData();

            //
            _currentTabIndex.Text = HidePreview ? "1" : "0";

            //Initialize selectOptionsEditor with updated values.
            _selectOptionsEditor.Initialize((SelectItemTextDecorator)TextDecorator, false, ResponseTemplate, PagePosition, false, _editMode);

            return result;
        }
    }
}