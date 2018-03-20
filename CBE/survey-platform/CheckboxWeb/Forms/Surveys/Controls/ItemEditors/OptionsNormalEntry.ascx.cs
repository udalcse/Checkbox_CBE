using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using Checkbox.Common;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Forms;
using Checkbox.Web;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemEditors
{
    /// <summary>
    /// User control editor for select options
    /// </summary>
    public partial class OptionsNormalEntry : Checkbox.Web.Common.UserControlBase
    {
        [QueryParameter("isNew")]
        public bool IsNew { get; set; }

        [QueryParameter("row")]
        public int? RowNumber { get; set; }

        [QueryParameter("i")]
        public int? ItemId { get; set; }

        [QueryParameter("lid")]
        public int? LibraryTemplateId { get; set; }

        public string Html { get; set; }

        protected int PagePosition { get; set; }

		static char[] separator = new[] { ',' };

        private EditMode _editMode;

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
        public string LanguageCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected ResponseTemplate ResponseTemplate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected bool AllowMultiDefaultSelect { get; private set; }

        /// <summary>
        /// Determine if columns should be categorized
        /// </summary>
        protected bool AreColumnsCategorized { get; set; }

        /// <summary>
        /// Fires before redirect to html options editor
        /// </summary>
        public event EventHandler OnHtmlEditorRedirect;

        /// <summary>
        /// 
        /// </summary>
        protected bool HasPoints { 
            get
            {
                return ResponseTemplate == null || ResponseTemplate.BehaviorSettings.EnableScoring;
            }
        }

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
                if (HttpContext.Current.Session[ItemId + "_OptionTexts_c=" + ColumnNumber] == null)
                {
                    HttpContext.Current.Session[ItemId + "_OptionTexts_c=" + ColumnNumber] = new Dictionary<int, string>();
                }

                return (Dictionary<int, string>)HttpContext.Current.Session[ItemId + "_OptionTexts_c=" + ColumnNumber];
            }
            set
            {
                HttpContext.Current.Session[ItemId + "_OptionTexts_c=" + ColumnNumber] = value;
            }
        }

        //Get/set options
        protected List<ListOptionData> Options
        {
            get
            {
                if (HttpContext.Current.Session[ItemId + "_Options_c=" + ColumnNumber] == null)
                {
                    HttpContext.Current.Session[ItemId + "_Options_c=" + ColumnNumber] = new List<ListOptionData>();
                }

                return (List<ListOptionData>)HttpContext.Current.Session[ItemId + "_Options_c=" + ColumnNumber];
            }
            set
            {
                HttpContext.Current.Session[ItemId + "_Options_c=" + ColumnNumber] = value;
            }
        }

        /// <summary>
        /// List of deleted option ids
        /// </summary>
        private List<int> DeletedOptionIds
        {
            get
            {
                if (HttpContext.Current.Session[ItemId + "_DeletedOptionIds_c=" + ColumnNumber] == null)
                {
                    HttpContext.Current.Session[ItemId + "_DeletedOptionIds_c=" + ColumnNumber] = new List<int>();
                }

                return (List<int>)HttpContext.Current.Session[ItemId + "_DeletedOptionIds_c=" + ColumnNumber];
            }
            set
            {
                HttpContext.Current.Session[ItemId + "_DeletedOptionIds_c=" + ColumnNumber] = value;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
           
            //Helper for loading pages into divs
            RegisterClientScriptInclude(
                "UFrame.js",
                ResolveUrl("~/Resources/UFrame.js"));

            _htmlEditorBtn.Visible = !RestrictHtmlOptions;

            _postOptions.Click += PostOptionsOnClick;
        }

        /// <summary>
        /// Initialize entry control.  Values are not persisted in session, so must be initialized on every load/postback.
        /// </summary>
        public void Initialize(List<ListOptionData> listOptions, Dictionary<int, string> optionTexts, bool allowMultiDefaultSelect, bool isPostBack, ResponseTemplate responseTemplate, int pagePosition, bool areColumnsCategorized, string languageCode, EditMode editMode)
        {
            //get from query, doesn't work as an attribute for some reason
            IsNew = Convert.ToBoolean(HttpContext.Current.Request.Params["isNew"]);
            ItemId = Convert.ToInt32(HttpContext.Current.Request.Params["i"]);
            RowNumber = Convert.ToInt32(HttpContext.Current.Request.Params["row"]);
            bool fromHtmlRedactor = Convert.ToBoolean(HttpContext.Current.Request.Params["fromHtmlRedactor"]);
            bool cancel = Convert.ToBoolean(HttpContext.Current.Request.Params["cancel"]);

            int lid;
            LibraryTemplateId = null;
            if (int.TryParse(HttpContext.Current.Request.Params["lid"], out lid))
                LibraryTemplateId = lid;

            _editMode = editMode;
            LanguageCode = languageCode;
            ResponseTemplate = responseTemplate;
            PagePosition = pagePosition;
            AreColumnsCategorized = areColumnsCategorized;

            if (!isPostBack && !fromHtmlRedactor)
            {
                Options = listOptions;
                OptionTexts = optionTexts;
                DeletedOptionIds = new List<int>();
            }

            AllowMultiDefaultSelect = allowMultiDefaultSelect;

            if (responseTemplate != null)
                _pipeSelector.Initialize(responseTemplate.ID, pagePosition, languageCode, "newOptionTxt");

            if(fromHtmlRedactor && !cancel)
                OptionTexts[RowNumber.Value] = HttpContext.Current.Session["temporary_html_" + ItemId + "_c=" + ColumnNumber] as string;
        }

        private void PostOptionsOnClick(object sender, EventArgs eventArgs)
        {
            ProcessOptionsPost(false);

            if (OnHtmlEditorRedirect != null)
                OnHtmlEditorRedirect(this, new EventArgs());

            var row = _currentrow.Value;
            var html = _currenthtml.Value;

            var uri = ResolveUrl("~/Forms/Surveys/HtmlEditor.aspx") +
                      "?s=" + (ResponseTemplate != null ? ResponseTemplate.ID.Value.ToString() : "0") +
                      "&p=" + PagePosition +
                      "&i=" + ItemId +
                      "&l=" + LanguageCode +
                      "&row=" + row +
                      "&isNew=" + IsNew +
                      "&isMatrix=" + MatrixColumnItem +
                      "&lid=" + (LibraryTemplateId.HasValue ? LibraryTemplateId.ToString() : string.Empty) +
                        ((Request == null || string.IsNullOrEmpty(Request["w"])) ? "" : ("&w=" + Request["w"]));

            if (ColumnNumber.HasValue)
                uri += "&c=" + ColumnNumber.Value;

            uri += "&html=" + html;

            Response.Redirect(uri);
        }

        private int? ColumnNumber
        {
            get { return HttpContext.Current.Session["matrixColumn_r=" + (ResponseTemplateId ?? LibraryTemplateId) + "_i=" + ItemId] as int?; }
        }

        /// <summary>
        /// Normal entry for options uses non-ASP.NET inputs, so form post must be handled manually.
        /// </summary>
        public void ProcessOptionsPost(bool updateTextDecorator = true)
        {
            //Process new/modified/deleted options

            //Get csv list of options, in order
            var optionOrderCsv = HttpContext.Current.Request["normalEntryOptionOrder"];
			var optionOrderArray = optionOrderCsv.Split(separator, StringSplitOptions.RemoveEmptyEntries);

            //Loop through
            for (var i = 0; i < optionOrderArray.Length; i++)
            {
                var optionId = Convert.ToInt32(optionOrderArray[i]);

                ListOptionData newOption = Options.FirstOrDefault(option => option.OptionID == optionId);

                //If option does not already exist, add placeholder to array
                if (optionId < 0 && newOption == null)
                {
                    newOption = new ListOptionData { OptionID = optionId };
                    Options.Add(newOption);
                }

                int? imageID = null;
                
                int temp;
                if (int.TryParse(HttpContext.Current.Request["OptionImageIDInput_" + optionId], out temp))
                    imageID = temp;

                //
                var isDefault = AllowMultiDefaultSelect
                    ? !string.IsNullOrEmpty(HttpContext.Current.Request["defaultSelect_" + optionId])
                    : HttpContext.Current.Request["defaultSelect"] == optionId.ToString();

                newOption.Category = HttpContext.Current.Request["OptionAliasInput_" + optionId];
                newOption.Alias = HttpContext.Current.Request["OptionAliasInput_" + optionId];
                newOption.Points = Utilities.AsDouble(HttpContext.Current.Request["OptionPointsInput_" + optionId]) ?? 0d;
                newOption.IsDefault = isDefault;
                newOption.Position = i + 1;
                newOption.ContentID = imageID;

                string text = HttpContext.Current.Request["OptionTextInput_" + optionId];
                if (Utilities.IsHtmlFormattedText(text))
                    text = Utilities.AdvancedHtmlDecode(text);

                OptionTexts[optionId] = text;

                if (updateTextDecorator)
                {
                    //Call update
                    UpdateOption(newOption, OptionTexts[optionId]);
                }
            }

            //Remove any deleted items
            foreach (var option in Options)
            {
                if (!option.IsOther && !option.IsNoneOfAbove && !optionOrderArray.Contains(option.OptionID.ToString())
                    && !DeletedOptionIds.Contains(option.OptionID))
                {
                    DeletedOptionIds.Add(option.OptionID);
                }
            }

            if (IsNew)
            {
                Options = Options.Where(o => !DeletedOptionIds.Contains(o.OptionID)).ToList();
                DeletedOptionIds.Clear();
            }
        }

        /// <summary>
        /// Web method for updating option points
        /// </summary>
        /// <param name="id"></param>
        /// <param name="text"></param>
        /// <param name="alias"></param>
        /// <param name="category"></param>
        /// <param name="points"></param>
        /// <param name="isDefault"></param>
        /// <param name="newPosition"></param>
        /// <param name="contentID"></param>
        public void UpdateOption(int id, string text, string alias, string category, double? points, bool isDefault, int newPosition, int? contentID)
        {
            //Consider "blank" option text to be indication option should be removed
            if (string.IsNullOrEmpty(text))
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
            optionToEdit.Category = category;
            optionToEdit.Points = points.HasValue ? points.Value : 0;
            optionToEdit.IsDefault = isDefault;
            optionToEdit.Position = newPosition;
            optionToEdit.ContentID = contentID;

            //Re-Add updated option to dictionary
            Options[optionIndex] = optionToEdit;
        }

        /// <summary>
        /// Web method for updating option points
        /// </summary>
        public void UpdateOption(ListOptionData optionData, string text)
        {
            UpdateOption(optionData.OptionID, text, optionData.Alias, optionData.Category,
                optionData.Points, optionData.IsDefault, optionData.Position, optionData.ContentID);
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

            var currentOptions = textDecorator.Data.Options;

            //Update existing, create new and update existing);)
            for (int i = 0; i < Options.Count; i++)
            {
                ListOptionData optionData = Options[i];  //Index is 0-based, but position is 1-based
                string optionText = OptionTexts[optionData.OptionID];

                int optionPosition = i + 1;

                if (currentOptions.Any(o => o.OptionID == optionData.OptionID))
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
                textDecorator.SetOptionText(optionPosition, optionText);
            }
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
            {
                DeletedOptionIds.Add(optionId);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Validate()
        {
            return true;
        }
    }
}