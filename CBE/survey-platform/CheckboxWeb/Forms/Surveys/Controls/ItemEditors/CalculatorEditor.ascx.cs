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
    public partial class CalculatorEditor : UserControlItemEditorBase
    {
        private ResponseTemplate _responseTemplate;


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

        void _tabChangeBtn_Click(object sender, EventArgs e)
        {
            if (Validate())
            {
                UpdateData();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);

            //Load tab script
            Page.ClientScript.RegisterClientScriptInclude(
                "jquery.ckbxtab.js",
                ResolveUrl("~/Resources/jquery.ckbxtab.js"));
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
            // this item changes choices. if user clicks "Cancel", this data will be saved in cache. So 
            // this object must work with the clone of the data.
            if (!isPagePostBack)
            {
                SelectItemData = ((SliderItemData)data).Clone() as SelectItemData;
            }

            base.Initialize(templateId, pagePosition, SelectItemData, currentLanguage, surveyLanguages, isPagePostBack, editMode, hidePreview);

            _activeDisplay.Initialize(data);

            _currentTabIndex.Text = HidePreview ? "1" : "0";

            //Initialize child editors
            if (TextDecorator != null
                && TextDecorator is TextItemDecorator)
            {
                _questionTextEditor.Initialize(((TextItemDecorator)TextDecorator).Text, isPagePostBack, currentLanguage, templateId, pagePosition, editMode);
                _descriptionTextEditor.Initialize(((TextItemDecorator)TextDecorator).SubText, isPagePostBack, currentLanguage, templateId, pagePosition, editMode);
                _behaviorEditor.Initialize(((TextItemDecorator)TextDecorator));
            }
        }

        /// <summary>
        /// Update item data
        /// </summary>
        public override void UpdateData()
        {
            base.UpdateData();

            if (TextDecorator != null
                && TextDecorator is TextItemDecorator)
            {
                ((TextItemDecorator)TextDecorator).Text = _questionTextEditor.Text;
                ((TextItemDecorator)TextDecorator).SubText = _descriptionTextEditor.Text;

                _behaviorEditor.UpdateData(((TextItemDecorator)TextDecorator));

                //CalculatorItemData itemData = TextDecorator.Data as CalculatorItemData;

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

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool Validate()
        {
            return _behaviorEditor.Validate();
        }
    }
}