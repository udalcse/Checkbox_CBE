using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
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
    /// Slider options editor
    /// </summary>
    public partial class SliderOptionsEditor : Checkbox.Web.Common.UserControlBase
    {
        public event BeforeRedirectDelegate OnBeforeRedirect;

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

            _optionsEntry.UploadImageClick += _optionsEntry_UploadImageClick;
        }

        void _optionsEntry_UploadImageClick(int? editingOptionId, int? editingImageId)
        {
            if (OnBeforeRedirect != null)
                OnBeforeRedirect();
            string url = ResolveUrl("~/Forms/Surveys/UploadImage.aspx") + "?" + Request.QueryString;

            int ind1 = url.IndexOf("redirect=");
            if (HttpContext.Current.Request.Url.AbsolutePath.Contains("Libraries"))
            {
                if (ind1 >= 0)
                {
                    int ind2 = url.IndexOf("&", ind1);
                    url = url.Remove(ind1, ind2 - ind1);
                    url = url.Insert(ind1, "redirect=library");
                }
                else
                {
                    url += "&redirect=library";
                }
            }
             
            
            //add option id to the url
            ind1 = url.IndexOf("optionId=");
            if (ind1 >= 0)
            {
                int ind2 = url.IndexOf("&", ind1);
                url = url.Remove(ind1, ind2 - ind1);
                url = url.Insert(ind1, "optionId=" + (editingOptionId != null ? editingOptionId.ToString() : string.Empty));
            }
            else if (editingOptionId != null)
                url += "&optionId=" + editingOptionId;

            //add image id to the url
            ind1 = url.IndexOf("imageId=");
            if (ind1 >= 0)
            {
                int ind2 = url.IndexOf("&", ind1);
                url = url.Remove(ind1, ind2 - ind1);
                url = url.Insert(ind1, "imageId=" + (editingImageId != null ? editingImageId.ToString() : string.Empty));
            }
            else if (editingImageId != null)
                url += "&imageId=" + editingImageId;

            //save options values to session before redirect
            _optionsEntry.SaveOptionsInStorage();

            Response.Redirect(url);
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
        /// Determine slider option type
        /// </summary>
        protected SliderValueListOptionType SliderOptionType { get; set; }

        /// <summary>
        /// Determine if options are with image or with text.
        /// </summary>
        protected bool AreOptionsWithImage
        {
            get { return SliderOptionType == SliderValueListOptionType.Image; }
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

            SliderItemData itemData = textDecorator.Data as SliderItemData;

            SliderOptionType = itemData.ValueListOptionType;

            if (itemData.ValueType == SliderValueType.NumberRange)
            {
                _optionsEditorPanel.Style["display"] = "none";
                _numberRangeWarningPanel.Style.Remove("display");
            }
            else
            {
                _numberRangeWarningPanel.Style["display"] = "none";
                _optionsEditorPanel.Style.Remove("display");

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

            //we need to initialize option editors even though they won't be visible to ensure that they will contain the options from the proper slider
            InitializeOptionEditors(textDecorator);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private void InitializeOptionEditors(SelectItemTextDecorator textDecorator)
        {
            SliderItemData itemData = textDecorator.Data as SliderItemData;

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
                itemData.ValueListOptionType,
                itemData.ValueType);

            _quickEntry.Initialize(
                tempOptions,
                optionTexts,
                textDecorator.Data is SelectManyData,
                IsPagePostback,
                ResponseTemplate == null || ResponseTemplate.BehaviorSettings.EnableScoring,
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
        /// Set Slider Option Type
        /// </summary>
        /// <param name="sliderValueType"></param>
        /// <param name="optionType"></param>
        public void SetSliderOptionType(SliderValueType sliderValueType, SliderValueListOptionType optionType)
        {
            _optionsEntry.SliderValueType = sliderValueType;
            _optionsEntry.SliderOptionType = optionType;            
        }

        /// <summary>
        /// Returns an options count
        /// </summary>
        public int OptionsCount
        {
            get
            {
                return _optionsEntry.TextsCount == 0 ? _quickEntry.OptionLinesCount : _optionsEntry.TextsCount;
            }
        }
    }
}