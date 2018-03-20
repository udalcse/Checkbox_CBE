using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Management;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemEditors
{
    /// <summary>
    /// Rank Order options editor
    /// </summary>
    public partial class RankOrderOptionsEditor : Checkbox.Web.Common.UserControlBase
    {
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
        /// Bind tab change button click
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
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        /// <summary>
        /// Determine Rank Order option type
        /// </summary>
        protected RankOrderOptionType RankOrderOptionType { get; set; }

        /// <summary>
        /// Determine if options are with image or with text.
        /// </summary>
        protected bool AreOptionsWithImage
        {
            get { return RankOrderOptionType == RankOrderOptionType.Image; }
        }

        /// <summary>
        /// Initialize options editor with text decorator for select item
        /// </summary>
        /// <param name="textDecorator"></param>
        /// <param name="isPagePostBack"></param>
        /// <param name="responseTemplate"></param>
        /// <param name="pagePosition"></param>
        public void Initialize(SelectItemTextDecorator textDecorator, bool isPagePostBack, ResponseTemplate responseTemplate, int pagePosition)
        {
            IsPagePostback = isPagePostBack;
            ResponseTemplate = responseTemplate;
            PagePosition = pagePosition;
            TextDecorator = textDecorator;

            RankOrderItemData itemData = textDecorator.Data as RankOrderItemData;

            RankOrderOptionType = itemData.RankOrderOptionType;

            InitializeOptionEditors(textDecorator);

            if (!isPagePostBack)
            {
                if (ApplicationManager.AppSettings.DefaultOptionEntryType == AppSettings.OptionEntryType.Normal || AreOptionsWithImage)
                {
                    CurrentTab = 0;
                }
                if (ApplicationManager.AppSettings.DefaultOptionEntryType == AppSettings.OptionEntryType.QuickEntry)
                {
                    CurrentTab = 1;
                }
            }                         
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private void InitializeOptionEditors(SelectItemTextDecorator textDecorator)
        {
            RankOrderItemData itemData = textDecorator.Data as RankOrderItemData;

            if (itemData == null)
                return;

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
                    OptionID = listOptionData.OptionID,
                    Points = listOptionData.Points,
                    Position = listOptionData.Position,
                    ContentID = listOptionData.ContentID
                });

                optionTexts[listOptionData.OptionID] = textDecorator.GetOptionText(listOptionData.Position);
            }
            
            //Initialize children
            _optionsEntry.Initialize(
                tempOptions,
                optionTexts,
                IsPagePostback,
                ResponseTemplate,
                PagePosition,
                itemData.RankOrderOptionType,
                itemData.RankOrderType,
                textDecorator.Language);

            _quickEntry.Initialize(
                tempOptions,
                optionTexts,
                textDecorator.Data is SelectManyData,
                IsPagePostback,
                false,
                false);
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

            InitializeOptionEditors(decorator);
        }

        /// <summary>
        /// Update options
        /// </summary>
        public void UpdateOptionsInMemory()
        {
            _optionsEntry.UpdateOptionsInMemory();
        }

        /// <summary>
        /// Set Rank Order Option Type
        /// </summary>
        /// <param name="RankOrderValueType"></param>
        /// <param name="optionType"></param>
        public void SetRankOrderOptionType(RankOrderOptionType optionType)
        {            
            _optionsEntry.RankOrderOptionType = optionType;            
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