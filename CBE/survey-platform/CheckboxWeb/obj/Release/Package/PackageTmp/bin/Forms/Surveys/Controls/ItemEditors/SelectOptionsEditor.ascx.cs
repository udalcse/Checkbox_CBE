using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Web.UI;
using Checkbox.Common;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Management;
using Checkbox.Forms;
using Checkbox.Security;
using Checkbox.Users;
using System.Web.Script.Services;
using System.Web.Services;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemEditors
{
    /// <summary>
    /// Select options editor.
    /// </summary>
    public partial class SelectOptionsEditor : Checkbox.Web.Common.UserControlBase
    {
        private EditMode _editMode;

        /// <summary>
        /// 
        /// </summary>
        public int CurrentTab
        {
            get { return Utilities.AsInt(_optionTabIndex.Text) ?? 0; }
            set { _optionTabIndex.Text = value.ToString(); }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool RestrictHtmlOptions { set; get; }

        /// <summary>
        /// 
        /// </summary>
        public bool MatrixColumnItem { set; get; }

        /// <summary>
        /// 
        /// </summary>
        private bool IsPagePostback { get; set; }

        /// <summary>
        /// 
        /// </summary>
        private ResponseTemplate ResponseTemplate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        private SelectItemTextDecorator TextDecorator { get; set; }

        /// <summary>
        /// 
        /// </summary>
        private int PagePosition { get; set; }

        /// <summary>
        /// Determine if columns should be categorized
        /// </summary>
        protected bool AreColumnsCategorized { get; set; }

        /// <summary>
        /// Holds the field name the item bound to
        /// </summary>
        protected string BindedFieldName { get; set; }

        /// <summary>
        /// Bind tab change button click
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            _tabChangeBtn.Click += _tabChangeBtn_Click;

            _optionsEntry.RestrictHtmlOptions = RestrictHtmlOptions;
            _optionsEntry.MatrixColumnItem = MatrixColumnItem;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            //Set initial tab
            RegisterClientScriptInclude(
            "jquery.ckbxtab.js",
            ResolveUrl("~/Resources/jquery.ckbxtab.js"));
        }

        /// <summary>
        /// Initialize options editor with text decorator for select item
        /// </summary>
        /// <param name="textDecorator"></param>
        /// <param name="isPagePostBack"></param>
        /// <param name="responseTemplate"></param>
        /// <param name="pagePosition"></param>
        /// <param name="areColumnsCategorized"></param>
        /// <param name="editMode"> </param>
        public void Initialize(SelectItemTextDecorator textDecorator, bool isPagePostBack, ResponseTemplate responseTemplate, int pagePosition, bool areColumnsCategorized, EditMode editMode)
        {
            IsPagePostback = isPagePostBack;
            ResponseTemplate = responseTemplate;
            PagePosition = pagePosition;
            TextDecorator = textDecorator;
            AreColumnsCategorized = areColumnsCategorized;

            InitializeOptionEditors(textDecorator, AreColumnsCategorized, editMode);

            if (!isPagePostBack)
            {
                if (ApplicationManager.AppSettings.DefaultOptionEntryType == AppSettings.OptionEntryType.Normal)
                {
                    CurrentTab = 0;
                }
                if (ApplicationManager.AppSettings.DefaultOptionEntryType == AppSettings.OptionEntryType.QuickEntry)
                {
                    CurrentTab = 1;
                }

                if (ApplicationManager.AppSettings.DefaultOptionEntryType == AppSettings.OptionEntryType.XMLImport)
                {
                    CurrentTab = 2;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private void InitializeOptionEditors(SelectItemTextDecorator textDecorator, bool areColumnsCategorized, EditMode editMode)
        {
            //Store options and such
            ReadOnlyCollection<ListOptionData> listOptions = textDecorator.Data.Options;
            var tempOptions = new List<ListOptionData>();
            var optionTexts = new Dictionary<int, string>();

            foreach (ListOptionData listOptionData in listOptions)
            {
                tempOptions.Add(new ListOptionData
                {
                    Alias = listOptionData.Alias,
                    Category = listOptionData.Category,
                    Disabled = listOptionData.Disabled,
                    IsDefault = listOptionData.IsDefault,
                    IsOther = listOptionData.IsOther,
                    IsNoneOfAbove = listOptionData.IsNoneOfAbove,
                    OptionID = listOptionData.OptionID,
                    Points = listOptionData.Points,
                    Position = listOptionData.Position
                });

                optionTexts[listOptionData.OptionID] = textDecorator.GetOptionText(listOptionData.Position);
            }

            //Initialize children
            _optionsEntry.Initialize(
                tempOptions,
                optionTexts,
                textDecorator.Data is SelectManyData,
                IsPagePostback,
                ResponseTemplate,
                PagePosition,
                areColumnsCategorized,
                textDecorator.Language, 
                editMode);

            _quickEntry.Initialize(
                tempOptions,
                optionTexts,
                textDecorator.Data is SelectManyData,
                IsPagePostback,
                ResponseTemplate == null || ResponseTemplate.BehaviorSettings.EnableScoring,
                areColumnsCategorized);
        }


        /// <summary>
        /// Return a boolean indicating if item is valid.
        /// </summary>
        /// <returns></returns>
        public bool Validate()
        {
            if (CurrentTab == 0)
            {
                return _optionsEntry.Validate();
            }

            if (CurrentTab == 1)
            {
                return _quickEntry.Validate();
            }

            return true;
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
                IsPagePostback = false; // Set PostBack false to reinitialize option editors.
                UpdateData(TextDecorator);
                CurrentTab = Utilities.AsInt(_newTabIndex.Text) ?? 0;
            }
        }

        /// <summary>
        /// Update data with options, etc.
        /// </summary>
        public void UpdateData(SelectItemTextDecorator decorator)
        {
            if (CurrentTab == 0)
            {
                _optionsEntry.UpdateData(decorator);
            }
            else if (CurrentTab == 1)
            {
                _quickEntry.UpdateData(decorator);
            }

            InitializeOptionEditors(decorator, AreColumnsCategorized, _editMode);
        }

        /// <summary>
        /// Update options
        /// </summary>
        public void UpdateOptionsInMemory()
        {
            _optionsEntry.UpdateOptionsInMemory();
        }

        /// <summary>
        /// Returns an options count
        /// </summary>
        public int OptionsCount
        {
            get
            {
                return _optionsEntry.TextsCount;
            }
        }
    }
}