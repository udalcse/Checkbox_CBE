using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Forms;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Web.Forms.UI.Editing;
using Checkbox.Web.Forms.UI.Rendering;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemEditors
{
    public partial class Slider : UserControlItemEditorBase
    {
        private ResponseTemplate _responseTemplate;
        private EditMode _editMode;


        /// <summary>
        /// 
        /// </summary>
        protected override Control PreviewContainer
        {
            get { return _previewPlace; }
        }

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
        /// Get response template being edited
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
        /// Override onInit to handle events
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            _tabChangeBtn.Click += _tabChangeBtn_Click;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);

            //Load tab script
            RegisterClientScriptInclude(
                "jquery.ckbxtab.js",
                ResolveUrl("~/Resources/jquery.ckbxtab.js"));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _tabChangeBtn_Click(object sender, EventArgs e)
        {
            if (Validate())
            {
                UpdateData();
                _selectOptionsEditor.Initialize((SelectItemTextDecorator)TextDecorator, false, ResponseTemplate, PagePosition);
            }
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
                SelectItemData = ((SliderItemData) data).Clone() as SelectItemData;
            }

            base.Initialize(templateId, pagePosition, SelectItemData, currentLanguage, surveyLanguages, isPagePostBack, editMode, hidePreview);

            _activeDisplay.Initialize(data);

            _currentTabIndex.Text = HidePreview ? "1" : "0";

            if (Context.Request.QueryString["showOptionsTab"] == "true")
            {
                _currentTabIndex.Text = "3";
            }

            //Initialize child editors
            if (TextDecorator != null
                && TextDecorator is SelectItemTextDecorator)
            {
                _questionTextEditor.Initialize(((SelectItemTextDecorator)TextDecorator).Text, isPagePostBack, currentLanguage, templateId, pagePosition, editMode);
                _descriptionTextEditor.Initialize(((SelectItemTextDecorator)TextDecorator).SubText, isPagePostBack, currentLanguage, templateId, pagePosition, editMode);
                _selectOptionsEditor.Initialize((SelectItemTextDecorator)TextDecorator, isPagePostBack, ResponseTemplate, PagePosition);
                _behaviorEditor.Initialize(((SelectItemTextDecorator)TextDecorator), false);
            }

            _selectOptionsEditor.OnBeforeRedirect += _selectOptionsEditor_OnBeforeRedirect;
        }

        /// <summary>
        /// 
        /// </summary>
        private void _selectOptionsEditor_OnBeforeRedirect()
        {
            SaveData();
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
            if (TextDecorator is SelectItemTextDecorator)
            {
                ((SelectItemTextDecorator)TextDecorator).Text = _questionTextEditor.Text;
                ((SelectItemTextDecorator)TextDecorator).SubText = _descriptionTextEditor.Text;

                _behaviorEditor.UpdateData(((SelectItemTextDecorator)TextDecorator));

                SliderItemData itemData = TextDecorator.Data as SliderItemData;

                //Update value list option type because it can influence on update result.
                _selectOptionsEditor.SetSliderOptionType(itemData.ValueType, itemData.ValueListOptionType);
                if (updateTextDecoratorOptions)
                    _selectOptionsEditor.UpdateData((SelectItemTextDecorator)TextDecorator);
                else
                    _selectOptionsEditor.UpdateOptionsInMemory();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int SaveData()
        {
            int result = base.SaveData();

            _currentTabIndex.Text = HidePreview ? "1" : "0";

            //Initialize selectOptionsEditor with updated values.
            _selectOptionsEditor.Initialize((SelectItemTextDecorator)TextDecorator, false, ResponseTemplate, PagePosition);
            
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool Validate()
        {
            return _selectOptionsEditor.Validate() && _behaviorEditor.Validate();
        }


        /// <summary>
        /// Determine if user entered enough information.
        /// </summary>
        /// <param name="message">Contains a warning message, which information should be added to the item</param>
        /// <returns></returns>
        public override bool DoesItemContainEnoughInformation(out string message)
        {
            message = string.Empty;
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        protected override bool DoesItemContainEnoughOptions(out string message)
        {
            message = null;

            var sliderItem = TextDecorator.Data as SliderItemData;
            if (sliderItem != null && sliderItem.ValueType == SliderValueType.NumberRange)
                return true;

            return base.DoesItemContainEnoughOptions(out message);
        }
    }
}