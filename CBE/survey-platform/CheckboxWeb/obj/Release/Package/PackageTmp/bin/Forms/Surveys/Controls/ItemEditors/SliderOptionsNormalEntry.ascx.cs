using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Web;
using CheckboxWeb.Controls.Button;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemEditors
{
    public partial class SliderOptionsNormalEntry : Checkbox.Web.Common.UserControlBase
    {
        private SliderValueListOptionType _sliderOptionType;

        public delegate void UploadImageClickDelegate(int? editingOptionId, int? editingImageId);

        public event UploadImageClickDelegate UploadImageClick;

        protected int PagePosition { get; set; }

        static char[] separator = new char[] { ',' };

        /// <summary>
        /// 
        /// </summary>
        protected ResponseTemplate ResponseTemplate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected bool HasPoints
        {
            get
            {
                return ResponseTemplate == null || ResponseTemplate.BehaviorSettings.EnableScoring;
            }
        }

        /// <summary>
        /// Determine if options are with image or with text.
        /// </summary>
        protected bool AreOptionsWithImage
        {
            get { return SliderOptionType == SliderValueListOptionType.Image; }
        }

        /// <summary>
        /// Shows when options type is switched
        /// </summary>
        private bool IsOptionsTypeSwitched { set; get; }

        /// <summary>
        /// Determine slider option type
        /// </summary>
        public SliderValueListOptionType SliderOptionType
        {
            set
            {
                if (LastSliderValueListOptionType.HasValue && LastSliderValueListOptionType.Value != value)
                    IsOptionsTypeSwitched = true;
                
                LastSliderValueListOptionType = _sliderOptionType = value;
            }
            get { return _sliderOptionType; }
        }

        /// <summary>
        /// Get/Set Slider value type
        /// </summary>
        public SliderValueType SliderValueType { get; set; }

        /// <summary>
        /// Get response template id
        /// </summary>
        protected int? ResponseTemplateId
        {
            get
            {
                if (ResponseTemplate != null)
                {
                    return ResponseTemplate.ID;
                }

                return null;
            }
        }

        /// <summary>
        /// Returns a count of options inputted by a user
        /// </summary>
        public int TextsCount
        {
            get
            {
                if (OptionTexts.Keys.Count == 0 && !String.IsNullOrEmpty(Request["normalEntryOptionOrder"]))
                {
                    //there are some unprocessed options in the request
                    ProcessOptionsPost();
                }
                return OptionTexts.Keys.Count;
            }
        }

        //Get/set option texts
        protected Dictionary<int, string> OptionTexts
        {
            get
            {
                if (ViewState[ID + "_SliderOptionTexts"] == null)
                {
                    ViewState[ID + "_SliderOptionTexts"] = new Dictionary<int, string>();
                }

                return (Dictionary<int, string>)ViewState[ID + "_SliderOptionTexts"];
            }
            set
            {
                ViewState[ID + "_SliderOptionTexts"] = value;
            }
        }

        //Get/set option texts
        protected Dictionary<int, string> OptionTextsStorage
        {
            get
            {
                if (HttpContext.Current.Session[ID + "_SliderOptionTexts"] == null)
                {
                    HttpContext.Current.Session[ID + "_SliderOptionTexts"] = new Dictionary<int, string>();
                }

                return (Dictionary<int, string>)HttpContext.Current.Session[ID + "_SliderOptionTexts"];
            }
            set
            {
                HttpContext.Current.Session[ID + "_SliderOptionTexts"] = value;
            }
        }

        //Get/set options
        protected List<ListOptionData> Options
        {
            get
            {
                if (HttpContext.Current.Session[ID + "_SliderOptions"] == null)
                {
                    HttpContext.Current.Session[ID + "_SliderOptions"] = new List<ListOptionData>();
                }

                return (List<ListOptionData>)HttpContext.Current.Session[ID + "_SliderOptions"];
            }
            set
            {
                HttpContext.Current.Session[ID + "_SliderOptions"] = value;
            }
        }

        //Get/set options
        protected List<ListOptionData> OptionsStorage
        {
            get
            {
                if (HttpContext.Current.Session[ID + "_SliderOptionsStorage"] == null)
                {
                    HttpContext.Current.Session[ID + "_SliderOptionsStorage"] = new List<ListOptionData>();
                }

                return (List<ListOptionData>)HttpContext.Current.Session[ID + "_SliderOptionsStorage"];
            }
            set
            {
                HttpContext.Current.Session[ID + "_SliderOptionsStorage"] = value;
            }
        }

        /// <summary>
        /// List of deleted option ids
        /// </summary>
        private List<int> DeletedOptionIds
        {
            get
            {
                if (HttpContext.Current.Session[ID + "_DeletedSliderOptionIds"] == null)
                {
                    HttpContext.Current.Session[ID + "_DeletedSliderOptionIds"] = new List<int>();
                }

                return (List<int>)HttpContext.Current.Session[ID + "_DeletedSliderOptionIds"];
            }
            set
            {
                HttpContext.Current.Session[ID + "_DeletedSliderOptionIds"] = value;
            }
        }

        /// <summary>
        /// List of deleted option ids
        /// </summary>
        private List<int> DeletedTempOptionIds
        {
            get
            {
                if (ViewState[ID + "_DeletedSliderTempOptionIds"] == null)
                {
                    ViewState[ID + "_DeletedSliderTempOptionIds"] = new List<int>();
                }

                return (List<int>)ViewState[ID + "_DeletedSliderTempOptionIds"];
            }
            set
            {
                ViewState[ID + "_DeletedSliderTempOptionIds"] = value;
            }
        }

        private int? NewOptionContentId
        {
            get { return (int?)HttpContext.Current.Session[ID + "_SliderOptionContentId"]; }
            set { HttpContext.Current.Session[ID + "_SliderOptionContentId"] = value; }
        }

        private SliderValueListOptionType? LastSliderValueListOptionType
        {
            get { return (SliderValueListOptionType?)(HttpContext.Current.Session[ID + "_LastSliderValueListOptionType"]); }
            set { HttpContext.Current.Session[ID + "_LastSliderValueListOptionType"] = value; }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (string.Compare(HttpContext.Current.Request.QueryString["op"], "imageUploaded",
                StringComparison.InvariantCultureIgnoreCase) == 0 && SliderOptionType==SliderValueListOptionType.Image)
            {
                Dictionary<String, String> args = new Dictionary<string, string>();
                foreach (var param in HttpContext.Current.Request.Params)
                {
                    if (param.ToString() == "op" ||
                        param.ToString() == "optionId" ||
                        param.ToString() == "imageUrl" ||
                        param.ToString() == "imageName" ||
                        param.ToString() == "imageId")
                        args.Add(param.ToString(), HttpContext.Current.Request.QueryString[param.ToString()]);
                }
                var jScriptObject = args.Aggregate("{", (current, argument) => current + String.Format("{0}: '{1}', ", argument.Key, argument.Value));
                jScriptObject = jScriptObject.Substring(0, jScriptObject.Length - 2) + "}";

                //update last uploaded content
                int contentId;
                if (int.TryParse(HttpContext.Current.Request["imageId"], out contentId))
                {
                    NewOptionContentId = contentId;

                    //check option for update
                    int optionId;
                    if (int.TryParse(HttpContext.Current.Request["optionId"], out optionId))
                    {
                        var option = Options.FirstOrDefault(o => o.OptionID == optionId);
                        if (option != null)
                            option.ContentID = contentId;
                    }
                }

                Page.ClientScript.RegisterClientScriptBlock(GetType(), "UpdateSliderOption", "$(function(){imageUploaded(" + jScriptObject + ");});", true);
            }
        }


        /// <summary>
        /// Initialize entry control.  Values are not persisted in session, so must be initialized on every load/postback.
        /// </summary>
        public void Initialize(List<ListOptionData> listOptions, Dictionary<int, string> optionTexts, bool isPostBack, ResponseTemplate responseTemplate, int pagePosition, SliderValueListOptionType sliderOptionType, SliderValueType sliderValueType)
        {
            ResponseTemplate = responseTemplate;
            PagePosition = pagePosition;
            SliderOptionType = sliderOptionType;
            SliderValueType = sliderValueType;

            _uploadImageHiddenBtn.Click += _uploadImageBtn_Click;

            if (HttpContext.Current.Request.QueryString["isNew"] == "true")
                _uploadImageBtn.Click += _uploadImageBtn_Click;

            if (!isPostBack)
            {
                Options = listOptions;
                OptionTexts = optionTexts;
            }

            if (HttpContext.Current.Request.QueryString["op"] != null)
            {
                Options = OptionsStorage.Where(option => !DeletedOptionIds.Contains(option.OptionID)).ToList();
                OptionTexts = OptionTextsStorage;
            }
        } 

        private void _uploadImageBtn_Click(object sender, EventArgs e)
        {
            ProcessOptionsPost();

            if (UploadImageClick != null)
            {
                int optionId;
                int? editingOptionId = null;
                //check if click was on 'new opton button' or generating without text 
                int? editingImageId = (sender is CheckboxButton && string.IsNullOrEmpty((sender as CheckboxButton).TextId))? NewOptionContentId : null;
                if (int.TryParse(_updateOptionHidden.Value, out optionId))
                {
                    editingOptionId = optionId;
                    var option = Options.FirstOrDefault(o => o.OptionID == editingOptionId);
                    if (option != null)
                        editingImageId = option.ContentID;
                }
                UploadImageClick(editingOptionId, editingImageId);
            }
        }

        /// <summary>
        /// Normal entry for options uses non-ASP.NET inputs, so form post must be handled manually.
        /// </summary>
        private void ProcessOptionsPost(bool updateTextDecorator = true)
        {
            //Process new/modified/deleted options

            if(IsOptionsTypeSwitched)
            {
                OptionsStorage = null;
                OptionTextsStorage = null;
                Options = null;
                IsOptionsTypeSwitched = false;
                return;
            }

            //Get csv list of options, in order
            var optionOrderCsv = Request["normalEntryOptionOrder"];
            var optionOrderArray = optionOrderCsv.Split(separator, StringSplitOptions.RemoveEmptyEntries);

            List<ListOptionData> temporaryOptionsList = new List<ListOptionData>();

            //Loop through)
            for (var i = 0; i < optionOrderArray.Length; i++)
            {
                var optionId = Convert.ToInt32(optionOrderArray[i]);

                //If option does not already exist, add placeholder to array
                var currentOption = Options.FirstOrDefault(option => option.OptionID == optionId);
                if (optionId < 0 && currentOption == null)
                {
                    currentOption = new ListOptionData { OptionID = optionId };
                    Options.Add(currentOption);
                }
                temporaryOptionsList.Add(currentOption);

                //
                var isDefault = Request["defaultSelect"] == optionId.ToString();

                int? imageId = NewOptionContentId;
                if (currentOption != null && currentOption.ContentID.HasValue)
                    imageId = currentOption.ContentID.Value;

                if (updateTextDecorator)
                {
                    //Call update
                    UpdateOption(
                        optionId,
                        Request["OptionTextInput_" + optionId] ?? string.Empty,
                        Request["OptionAliasInput_" + optionId],
                        Utilities.AsDouble(Request["OptionPointsInput_" + optionId]),
                        isDefault,
                        i + 1,
                        HttpContext.Current.Request.QueryString["isNew"] == "true" ? imageId : Utilities.AsInt(Request["OptionImageInput_" + optionId]));
                }
            }

            //Remove any deleted items
            foreach (var option in Options)
            {
                if (!optionOrderArray.Contains(option.OptionID.ToString())
                    && !DeletedOptionIds.Contains(option.OptionID))
                {
                    DeletedOptionIds.Add(option.OptionID);
                }
            }
            Options = temporaryOptionsList.Where(o => o != null && !DeletedOptionIds.Contains(o.OptionID) && !DeletedTempOptionIds.Contains(o.OptionID)).ToList();


            OptionsStorage = Options;
            OptionTextsStorage = OptionTexts;
        }

        /// <summary>
        /// Web method for updating option points
        /// </summary>
        /// <param name="id"></param>
        /// <param name="text"></param>
        /// <param name="alias"></param>
        /// <param name="points"></param>
        /// <param name="isDefault"></param>
        /// <param name="newPosition"></param>
        /// <param name="contentID"></param>
        public void UpdateOption(int id, string text, string alias, double? points, bool isDefault, int newPosition, int? contentID)
        {           
            //Consider "blank" option text to be indication option should be removed
            if (!AreOptionsWithImage && string.IsNullOrEmpty(text))
            {
                DeleteSpecifiedOption(id);
                return;
            }

            //Consider empty image or empty alias to be indication option should be removed
            if (AreOptionsWithImage && (!contentID.HasValue || String.IsNullOrEmpty(alias)))
            {
                DeleteSpecifiedOption(id);
                return;
            }

            //Otherwise, update
            var optionToEdit = Options.FirstOrDefault(opt => opt.OptionID == id);

            if (optionToEdit == null)
            {
                return;
            }

            var optionIndex = Options.IndexOf(optionToEdit);

            OptionTexts[id] = text;
            optionToEdit.Alias = alias;
            optionToEdit.Points = points.HasValue ? points.Value : 0;
            optionToEdit.IsDefault = isDefault;
            optionToEdit.Position = newPosition;
            optionToEdit.ContentID = contentID;

            //Re-Add updated option to dictionary
            Options[optionIndex] = optionToEdit;
        }

        /// <summary>
        /// 
        /// </summary>
        public void UpdateOptionsInMemory()
        {
            ProcessOptionsPost(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="textDecorator"></param>
        public void UpdateData(SelectItemTextDecorator textDecorator)
        {
            //Update options
            ProcessOptionsPost();

            //Delete the unnecessary options
            for (int i = Options.Count() - 1; i >= 0; i--)
            {
                if (SliderValueType == SliderValueType.NumberRange || (AreOptionsWithImage && !Options[i].ContentID.HasValue) || (!AreOptionsWithImage && String.IsNullOrEmpty(OptionTexts[Options[i].OptionID])))
                    DeleteSpecifiedOption(Options[i].OptionID);
            }

            //Have child controls update data
            foreach (int optionId in DeletedOptionIds)
            {
                textDecorator.Data.RemoveOption(optionId);
            }

            //Reassign options and order by positions, excluding deleted options
            Options = Options
                .Where(option => !DeletedOptionIds.Contains(option.OptionID))
                .OrderBy(theOption => theOption.Position)
                .ToList();

            IEnumerable<int> currentOptions = textDecorator.Data.Options.Select(p => p.OptionID);

            //Update existing, create new and update existing);)
            for (int i = 0; i < Options.Count; i++)
            {
                ListOptionData optionData = Options[i];  //Index is 0-based, but position is 1-based

                int optionPosition = i + 1;
                string optionText = OptionTexts[optionData.OptionID];

                if (currentOptions.Contains(optionData.OptionID))
                {
                    textDecorator.Data.UpdateOption(
                        optionData.OptionID,
                        optionData.Alias,
                        optionData.Category,
                        optionData.IsDefault,
                        optionPosition,
                        optionData.Points,
                        optionData.IsOther,
                        optionData.IsNoneOfAbove,
                        optionData.ContentID);
                }
                else
                {
                    var updated = textDecorator.Data.AddOption(
                        optionData.Alias,
                        optionData.Category,
                        optionData.IsDefault,
                        optionPosition,
                        optionData.Points,
                        optionData.IsOther,
                        optionData.IsNoneOfAbove,
                        optionData.ContentID);

                    //remove option text
                    OptionTexts.Remove(optionData.OptionID);

                    //update OptionId to prevent option duplication
                    optionData.OptionID = updated.OptionID;

                    //update option text
                    OptionTexts[optionData.OptionID] = optionText;
                }

                //Now set text
                if (OptionTexts.ContainsKey(optionData.OptionID))
                    textDecorator.SetOptionText(optionPosition, optionText);
                else
                    DeletedOptionIds.Add(optionData.OptionID);
            }

            DeletedTempOptionIds.Clear();
            DeletedOptionIds.Clear();
            Options.Clear();
            OptionsStorage.Clear();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="optionId"></param>
        private void DeleteSpecifiedOption(int optionId)
        {
            //Remove any options
            if (OptionTexts.ContainsKey(optionId))
            {
                OptionTexts.Remove(optionId);
            }

            //Remove selected option
            for (int i = 0; i < Options.Count; i++)
            {
                if (Options[i].OptionID == optionId)
                {
                    Options.RemoveAt(i);
                    break;
                }
            }

            if (optionId > 0)
                DeletedOptionIds.Add(optionId);
            else
                DeletedTempOptionIds.Add(optionId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Validate()
        {
            return true;
        }

        /// <summary>
        /// Save options in storage
        /// </summary>
        public void SaveOptionsInStorage()
        {
            OptionsStorage = Options;
        }
    }
}