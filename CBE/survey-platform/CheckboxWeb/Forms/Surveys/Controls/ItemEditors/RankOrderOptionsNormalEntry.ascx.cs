using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Web;

namespace CheckboxWeb.Forms.Surveys.Controls.ItemEditors
{
    /// <summary>
    /// 
    /// </summary>
    public partial class RankOrderOptionsNormalEntry : Checkbox.Web.Common.UserControlBase
    {
        [QueryParameter("isNew")]
        public bool IsNew { get; set; }

        [QueryParameter("row")]
        public int? RowNumber { get; set; }

        [QueryParameter("i")]
        public int? ItemId { get; set; }

        public string Html { get; set; }

        protected int PagePosition { get; set; }

        [QueryParameter("lid")]
        public int? LibraryTemplateId { get; set; }

        public string LanguageCode { get; set; }


        static char[] separator = new char[] { ',' };

        /// <summary>
        /// 
        /// </summary>
        protected ResponseTemplate ResponseTemplate { get; set; }

        /// <summary>
        /// Determine if options are with image or with text.
        /// </summary>
        protected bool AreOptionsWithImage
        {
            get { return RankOrderOptionType == RankOrderOptionType.Image; }
        }

        /// <summary>
        /// Determine Rank Order type
        /// </summary>
        public RankOrderType RankOrderType { get; set; }

        /// <summary>
        /// Get/Set Rank Order option type
        /// </summary>
        public RankOrderOptionType RankOrderOptionType { get; set; }

        /// <summary>
        /// Fires before redirect to html options editor
        /// </summary>
        public event EventHandler OnHtmlEditorRedirect;

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
                if (OptionTexts.Keys.Count == 0 && !String.IsNullOrEmpty(HttpContext.Current.Request["normalEntryOptionOrder"]))
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
                if (HttpContext.Current.Session[ID + "_RankOrderOptionTexts"] == null)
                {
                    HttpContext.Current.Session[ID + "_RankOrderOptionTexts"] = new Dictionary<int, string>();
                }

                return (Dictionary<int, string>)HttpContext.Current.Session[ID + "_RankOrderOptionTexts"];
            }
            set
            {
                HttpContext.Current.Session[ID + "_RankOrderOptionTexts"] = value;
            }
        }

        //Get/set options
        protected List<ListOptionData> Options
        {
            get
            {
                if (HttpContext.Current.Session[ID + "_RankOrderOptions"] == null)
                {
                    HttpContext.Current.Session[ID + "_RankOrderOptions"] = new List<ListOptionData>();
                }

                return (List<ListOptionData>)HttpContext.Current.Session[ID + "_RankOrderOptions"];
            }
            set
            {
                HttpContext.Current.Session[ID + "_RankOrderOptions"] = value;
            }
        }

        /// <summary>
        /// List of deleted option ids
        /// </summary>
        private List<int> DeletedOptionIds
        {
            get
            {
                if (HttpContext.Current.Session[ID + "_RankOrderDeletedOptionIds"] == null)
                {
                    HttpContext.Current.Session[ID + "_RankOrderDeletedOptionIds"] = new List<int>();
                }

                return (List<int>)HttpContext.Current.Session[ID + "_RankOrderDeletedOptionIds"];
            }
            set
            {
                HttpContext.Current.Session[ID + "_RankOrderDeletedOptionIds"] = value;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _postOptions.Click += PostOptionsOnClick;
        }

        /// <summary>
        /// Initialize entry control.  Values are not persisted in session, so must be initialized on every load/postback.
        /// </summary>
        public void Initialize(List<ListOptionData> listOptions, Dictionary<int, string> optionTexts, bool isPostBack, ResponseTemplate responseTemplate, int pagePosition, RankOrderOptionType rankOrderOptionType, RankOrderType rankOrderType, string languageCode)
        {
            //get from query, doesn't work as an attribute for some reason
            IsNew = Convert.ToBoolean(HttpContext.Current.Request.Params["isNew"]);
            ItemId = Convert.ToInt32(HttpContext.Current.Request.Params["i"]);
            RowNumber = Convert.ToInt32(HttpContext.Current.Request.Params["row"]);
            LanguageCode = languageCode;
            bool fromHtmlRedactor = Convert.ToBoolean(HttpContext.Current.Request.Params["fromHtmlRedactor"]);
            bool cancel = Convert.ToBoolean(HttpContext.Current.Request.Params["cancel"]);

            int lid;
            LibraryTemplateId = null;
            if (int.TryParse(HttpContext.Current.Request.Params["lid"], out lid))
                LibraryTemplateId = lid;

            ResponseTemplate = responseTemplate;
            PagePosition = pagePosition;
            RankOrderOptionType = rankOrderOptionType;
            RankOrderType= rankOrderType;

            if (!isPostBack && !fromHtmlRedactor)
            {
                Options = listOptions;
                OptionTexts = optionTexts;
                DeletedOptionIds = new List<int>();
            }

            if (fromHtmlRedactor && !cancel)
                OptionTexts[RowNumber.Value] = HttpContext.Current.Session["temporary_html_" + ItemId + "_c="] as string;
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
                      "&isMatrix=" + false +
                      "&lid=" + (LibraryTemplateId.HasValue ? LibraryTemplateId.ToString() : string.Empty) +
                  ((Request == null || string.IsNullOrEmpty(Request["w"])) ? "" : ("&w=" + Request["w"]));

            uri += "&html=" + html;

            Response.Redirect(uri);
        }

        /// <summary>
        /// Normal entry for options uses non-ASP.NET inputs, so form post must be handled manually.
        /// </summary>
        private void ProcessOptionsPost(bool updateTextDecorator = true)
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
                if (newOption == null)
                {
                    if (optionId < 0)
                    {
                        newOption = new ListOptionData {OptionID = optionId};
                        Options.Add(newOption);
                    }
                    else
                        return;
                }

                int? imageID = null;

                int temp;
                if (int.TryParse(HttpContext.Current.Request["OptionImageInput_" + optionId], out temp))
                    imageID = temp;

                newOption.Category = HttpContext.Current.Request["OptionAliasInput_" + optionId];
                newOption.Alias = HttpContext.Current.Request["OptionAliasInput_" + optionId];
                newOption.Points = Utilities.AsDouble(HttpContext.Current.Request["OptionPointsInput_" + optionId]) ?? 0d;
                newOption.IsDefault = HttpContext.Current.Request["defaultSelect"] == optionId.ToString(); 
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
                if (!option.IsOther && !optionOrderArray.Contains(option.OptionID.ToString())
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

            /*
             *                 var optionId = Convert.ToInt32(optionOrderArray[i]);

                //If option does not already exist, add placeholder to array
                if (optionId < 0 && Options.FirstOrDefault(option => option.OptionID == optionId) == null)
                {
                    var newOption = new ListOptionData { OptionID = optionId };
                    Options.Add(newOption);
                }

                //
                var isDefault = Request["defaultSelect"] == optionId.ToString();

                //Call update
                UpdateOption(
                    optionId,
                    Request["OptionTextInput_" + optionId] ?? string.Empty,
                    Request["OptionAliasInput_" + optionId],
                    0,
                    isDefault,
                    i + 1,
                    Utilities.AsInt(Request["OptionImageInput_" + optionId]));

             * 
             * */
        }

        /// <summary>
        /// Web method for updating option points
        /// </summary>
        public void UpdateOption(ListOptionData optionData, string text)
        {
            UpdateOption(optionData.OptionID, text, optionData.Alias, 
                optionData.Points, optionData.IsDefault, optionData.Position, optionData.ContentID);
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
            //Delete the unnecessary options
            for (int i = Options.Count() - 1; i >= 0; i--)
            {
                if ((AreOptionsWithImage && !Options[i].ContentID.HasValue) || (!AreOptionsWithImage && String.IsNullOrEmpty(OptionTexts[Options[i].OptionID])))
                    DeleteSpecifiedOption(Options[i].OptionID);
            }

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