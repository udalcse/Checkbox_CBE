using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using Checkbox.Forms;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Security;
using Checkbox.Users;
using Checkbox.Web;
using Checkbox.Web.Forms.UI.Editing;
using Checkbox.Web.Forms.UI.Rendering;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemEditors
{
    public partial class Matrix : UserControlItemEditorBase
    {
        [QueryParameter("lid", IsRequired = true)]
        public int LibraryTemplateId { get; set; }

        public bool IsBinded { get; set; }

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
        /// In memory key for matrix text decorator.  Use separate key from base user control item editor class
        ///  to prevent from being overwritten when editors for column prototypes are loaded
        /// </summary>
        public const string InMemoryMatrixItemEditorSessionKey = "CurrentMatrixItem";

        /// <summary>
        /// 
        /// </summary>
        public const string InMemoryMatrixColumnsEditorSessionKey = "MatrixColumns";

        /// <summary>
        /// 
        /// </summary>
        public MatrixItemTextDecorator TextDecoratorClone
        {
            get { return HttpContext.Current.Session[InMemoryMatrixItemEditorSessionKey] as MatrixItemTextDecorator; }
            set { HttpContext.Current.Session[InMemoryMatrixItemEditorSessionKey] = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<int, ItemTextDecorator> ColumnPrototypesWorkingCopies
        {
            get { return HttpContext.Current.Session[InMemoryMatrixColumnsEditorSessionKey] as Dictionary<int, ItemTextDecorator>; }
            set { HttpContext.Current.Session[InMemoryMatrixColumnsEditorSessionKey] = value; }
        }

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
        /// Get the textDecorator for the matrix
        /// </summary>
        private new MatrixItemTextDecorator TextDecorator
        {
            get { return base.TextDecorator as MatrixItemTextDecorator; }
            set { base.TextDecorator = value; }
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
        /// Bind event handlers
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            _columnEditor.OnColumnOrderChanged += new ColumnOrderChangedDelegate(_columnEditor_OnColumnOrderChanged);
            _columnEditor.OnBeforeRedirect += new BeforeRedirectDelegate(_columnEditor_OnBeforeRedirect);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        protected override bool DoesItemContainQuestionText(out string message)
        {
            message = null;

            var ltd = TextDecoratorClone as LabelledItemTextDecorator;
            if (ltd != null && string.IsNullOrWhiteSpace(ltd.Text))
            {
                message = EmptyQuestionTextErrorText;
                return false;
            }

            return true;
        }

        /// <summary>
        /// OnColumnOrderChanged handler
        /// </summary>
        /// <param name="columnsToPrototypeIds"></param>
        void _columnEditor_OnColumnOrderChanged(Dictionary<int, int> columnsToPrototypeIds)
        {
            Dictionary<int, ItemTextDecorator> idsToTextDecorator =
                ColumnPrototypesWorkingCopies.ToDictionary(p => p.Value.Data.ID.Value, p => p.Value);

            Dictionary<int, ItemTextDecorator> tempColumnProperties = new Dictionary<int, ItemTextDecorator>();

            foreach (KeyValuePair<int, int> pair in columnsToPrototypeIds)
            {
                tempColumnProperties.Add(pair.Key, idsToTextDecorator[pair.Value]);
            }

            ColumnPrototypesWorkingCopies = tempColumnProperties;
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
            base.Initialize(templateId, pagePosition, data, currentLanguage, surveyLanguages, isPagePostBack, editMode, hidePreview);
            IsBinded = ProfileManager.GetPropertiesList().Where(x => x.BindedItemId.Any(i => i == data.ID)).Count() > 0;
            _activeDisplay.Initialize(data);

            //Since the editor changes properties of the MatrixItem data object BEFORE saving, we need to operate
            // on a clone, so that any changes are not propagated due to byRef to MatrixItemData object stored in
            // response template.
            if (!IsPagePostBack)
            {
                HttpContext.Current.Session["matrixColumn_r=" + (templateId > 0 ? templateId : LibraryTemplateId) + "_i=" + data.ID] = null;

                _currentTabIndex.Text = HidePreview ? "1" : "0";

                //If a column is added or edited while item is editing in DIALOG window (just after creation)
                //current tab index should be set as "4" to show columns tab. TextDecoratorClone and 
                //ColumnPrototypesWorkingCopies have been already initialized. So another initialization isn't allowed.
                if (HttpContext.Current.Request.QueryString["showColumnsTab"] == "true")
                {
                    _currentTabIndex.Text = "4";
                }
                else if (HttpContext.Current.Request.QueryString["showRowsTab"] == "true")
                {
                    _currentTabIndex.Text = "3";
                }
                else
                {
                    var matrixClone = ((ICloneable) data).Clone() as MatrixItemData;
                    var textDecoratorClone = matrixClone.CreateTextDecorator(currentLanguage) as MatrixItemTextDecorator;

                    TextDecoratorClone = textDecoratorClone;

                    //Store working copies of column prototypes
                    var columnPrototypeDictionary = new Dictionary<int, ItemTextDecorator>();

                    for (var columnNumber = 1; columnNumber <= matrixClone.ColumnCount; columnNumber++)
                    {
                        if (columnNumber != matrixClone.PrimaryKeyColumnIndex)
                        {
                            var columnItemData =
                                ItemConfigurationManager.GetConfigurationData(matrixClone.GetColumnPrototypeId(columnNumber));

                            if (columnItemData != null)
                            {
                                columnPrototypeDictionary[columnNumber] =
                                    columnItemData.CreateTextDecorator(currentLanguage);
                            }
                        }
                    }

                    ColumnPrototypesWorkingCopies = columnPrototypeDictionary;
                }
            }

            //Initialize child editors
            if (TextDecoratorClone != null)
            {
                _questionTextEditor.Initialize(TextDecoratorClone.Text, isPagePostBack, currentLanguage, templateId, pagePosition, editMode, CustomFieldType.Matrix);
                _descriptionTextEditor.Initialize(TextDecoratorClone.SubText, isPagePostBack, currentLanguage, templateId, pagePosition, editMode);
                _matrixBehavior.Initialize(TextDecoratorClone);

                _matrixRowEditor.Initialize(templateId, pagePosition, TextDecoratorClone, editMode, TextDecoratorClone.Language);
                _matrixRowEditor.OnHtmlEditorRedirect += new EventHandler(_matrixRowEditor_OnHtmlEditorRedirect);
                _columnEditor.Initialize(templateId, pagePosition, TextDecoratorClone, false);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _matrixRowEditor_OnHtmlEditorRedirect(object sender, EventArgs e)
        {
            UpdateData();
        }

        /// <summary>
        /// Update item data
        /// </summary>
        public override void UpdateData()
        {
            base.UpdateData();

            if (TextDecoratorClone != null)
            {
                TextDecoratorClone.Text = _questionTextEditor.Text;
                TextDecoratorClone.SubText = _descriptionTextEditor.Text;

                _matrixRowEditor.UpdateData(TextDecoratorClone);
                _columnEditor.UpdateData(TextDecoratorClone);
                _matrixBehavior.UpdateData(TextDecoratorClone);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int SaveData()
        {
            //Reset tab
            _currentTabIndex.Text = HidePreview ? "1" : "0";

            //Save column prototypes
            if (ColumnPrototypesWorkingCopies != null)
            {
                foreach (var columnPrototype in ColumnPrototypesWorkingCopies.Values)
                {
                    columnPrototype.Save();
                }
            }

            //Reset current session item
            TextDecorator = TextDecoratorClone;
            ItemData = TextDecoratorClone.Data;

            int res = base.SaveData();
            
            //remove expressions which depends on the deleted items
            if (ItemData is MatrixItemData)
                (ItemData as MatrixItemData).DeleteDependentExpressions();

            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool Validate()
        {
            return _matrixRowEditor.Validate();
        }

        #region Text Saving and Restoring
        /// <summary>
        /// Handler for the event that column editor control fires before redirecting to another page
        /// </summary>
        void _columnEditor_OnBeforeRedirect()
        {
            UpdateData();
        }

        #endregion Text Saving and Restoring
    }
}