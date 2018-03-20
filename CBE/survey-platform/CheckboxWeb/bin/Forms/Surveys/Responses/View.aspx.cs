using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Forms.Items;
using Checkbox.Management;
using Checkbox.Security;
using Checkbox.Users;
using Checkbox.Web;
using Checkbox.Web.Page;
using Checkbox.Web.UI.Controls;
using CheckboxWeb.Users;
using Newtonsoft.Json;
using Prezza.Framework.Security;

namespace CheckboxWeb.Forms.Surveys.Responses
{
    public partial class View : SecuredPage
    {
        private ResponseTemplate _responseTemplate;
        private Response _response;
        private string _responseLanguage;
        private Guid? _responseGuid;
        private bool? _ticketValidated;
        private Dictionary<int, IItemFormatter> _formatterDictionary;

        /// <summary>
        /// 
        /// </summary>
        public ExportMode PrintMode
        {
            get
            {
                var print = Request.QueryString["print"];
                if (string.IsNullOrEmpty(print))
                    return ExportMode.None;

                switch (print.ToLower())
                {
                    case "pdf":
                        return ExportMode.Pdf;
                    case "default":
                        return ExportMode.Default;
                    default:
                        return ExportMode.None;
                }
            }
        }

        /// <summary>
        /// Get survey associated with response
        /// </summary>
        protected ResponseTemplate ResponseTemplate
        {
            get { return _responseTemplate ?? (_responseTemplate = GetResponseTemplate()); }
        }

        /// <summary>
        /// 
        /// </summary>
        protected string ResponseLanguage
        {
            get
            {
                //Get the current response in the viewer's language
                if (string.IsNullOrEmpty(_responseLanguage))
                {

                    _responseLanguage =
                       ResponseTemplate.LanguageSettings.SupportedLanguages.Contains(WebTextManager.GetUserLanguage())
                            ? WebTextManager.GetUserLanguage()
                            : ResponseTemplate.LanguageSettings.DefaultLanguage;
                }

                return _responseLanguage;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected Response CurrentResponse
        {
            get { return _response ?? (_response = GetResponse()); }
        }

        /// <summary>
        /// Get a response template based on a responseGUID
        /// </summary>
        /// <returns></returns>
        private ResponseTemplate GetResponseTemplate()
        {
            return ResponseTemplateManager.GetResponseTemplateFromResponseGUID(ResponseGuid);
        }

        /// <summary>
        /// Get response object
        /// </summary>
        /// <returns></returns>
        private Response GetResponse()
        {
            if (_response == null)
            {
                _response = ResponseTemplate.CreateResponse(ResponseLanguage, ResponseGuid);
                _response.Restore(ResponseGuid);
            }

            return _response;
        }

        /// <summary>
        /// Convert response creation date to the client's timeZone and get it.
        /// </summary>
        public DateTime? ResponseDateCreatedOnClientSide
        {
            get { return WebUtilities.ConvertToClientTimeZone(CurrentResponse.DateCreated); }
        }

        /// <summary>
        /// Convert response completion date to the client's timeZone and get it.
        /// </summary>
        public DateTime? ResponseDateCompletedOnClientSide
        {
            get { return WebUtilities.ConvertToClientTimeZone(CurrentResponse.DateCompleted); }
        }

        /// <summary>
        /// Convert response last modified date to the client's timeZone and get it.
        /// </summary>
        public DateTime? ResponseDateLastModifiedOnClientSide  
        {
            get { return WebUtilities.ConvertToClientTimeZone(CurrentResponse.LastModified); }
        }


        /// <summary>
        /// Get the controlled entity for this page
        /// </summary>
        protected override IAccessControllable GetControllableEntity()
        {
            //Special case for users linked via a view response item in a survey.  If there is a ticket,
             //return null, so that auth check is bypassed
            if (HasValidTicket)
            {
                return null;
            }

            return ResponseTemplate;
        }

        /// <summary>
        /// Get a boolean indicating if a valid ticket exists for this response, such as when a
        /// user is linked directly from the view response summary item in a survye.
        /// </summary>
        private bool HasValidTicket
        {
            get
            {
                if (!_ticketValidated.HasValue)
                {
                    _ticketValidated = Ticketing.ValidateTicket(ResponseGuid);
                }

                return _ticketValidated.Value;
            }
        }
         

        /// <summary>
        /// Get the response GUID from the command line
        /// </summary>
        /// <returns></returns>
        private Guid ResponseGuid
        {
            get
            {
                if (!_responseGuid.HasValue)
                {
                    Guid g;
                    if (Guid.TryParse(Request.QueryString["responseGuid"], out g))
                        _responseGuid = g;
                    else
                        throw new Exception("No response GUID specified.");
                }

                return _responseGuid.Value;
            }
        }

        /// <summary>
        /// Get whether to hide page elements
        /// </summary>
        private bool NoChrome
        {
            get
            {
                return Utilities.IsNotNullOrEmpty(Request.QueryString["NoChrome"]);
            }
        }

        /// <summary>
        /// Get whether Include Response Details option is enabled or not
        /// </summary>
        protected bool IncludeResponseDetails
        {
            get { return !"false".Equals(Request.QueryString["includeDetails"], StringComparison.InvariantCultureIgnoreCase); }
        }

        /// <summary>
        /// Get whether Show Page Numbers option is enabled or not
        /// </summary>
        protected bool ShowPageNumbers
        {
            get { return !"false".Equals(Request.QueryString["showPageNumbers"], StringComparison.InvariantCultureIgnoreCase); }
        }

        /// <summary>
        /// Get whether Include Message/HTML Items option is enabled or not
        /// </summary>
        protected bool IncludeMessageItems
        {
            get { return "true".Equals(Request.QueryString["showMessages"], StringComparison.InvariantCultureIgnoreCase); }
        }

        /// <summary>
        /// Get whether Show Page Numbers option is enabled or not
        /// </summary>
        protected bool ShowHiddens
        {
            get { return "true".Equals(Request.QueryString["showHiddens"], StringComparison.InvariantCultureIgnoreCase); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
            {
                string[] targets = Request.Params.Get("__EVENTTARGET").Split(',');
                string[] args = Request.Params.Get("__EVENTARGUMENT").Split(',');

                for (int i = 0; i < targets.Length; i++)
                {
                    long answerId;
                    if (targets[i] == "downloadFile" && i < args.Length && long.TryParse(args[i], out answerId))
                    {
                        UploadItemManager.DownloadFile(Response, answerId);
                    }
                }
            }

            LoadDatePickerLocalized();

            //Helper for uframe
            RegisterClientScriptInclude(
                "UFrame.js",
                ResolveUrl("~/Resources/UFrame.js"));

            RegisterClientScriptInclude(
                "globalHelper.js",
                ResolveUrl("~/Resources/globalHelper.js"));
        }

        /// <summary>
        /// Check for values when initializing
        /// </summary>
        protected override void OnPageInit()
        {
            base.OnPageInit();

            if (ResponseTemplate == null || Response == null)
            {
                throw new Exception("Unable to locate specified response.");
            }

            Title = string.Format("{0} - {1} [{2}]", WebTextManager.GetText("/pageText/reporting.aspx/responseDetails"), CurrentResponse.ID, CurrentResponse.UniqueIdentifier);

            Master.HideDialogButtons();

            //
            //ApplyStyleTemplate(ResponseTemplate.StyleSettings.StyleTemplateId);
            
            //Authorize the current principal by checking permissions or a ticket
            if (!AuthorizationProvider.Authorize(CurrentPrincipal, _responseTemplate, "Analysis.Responses.View")
                && !Ticketing.ValidateTicket(_response.GUID.Value))
            {
                if(NoChrome)
                {
                    _responseContent.Visible = false;
                    throw new Exception("You do not have permission to view the details for this response.");
                }

                Response.Redirect("~/Login.aspx");
            }

            _scorePlace.Visible = ResponseTemplate.BehaviorSettings.EnableScoring;
            _userPropertiesPlace.Visible = Utilities.IsNotNullOrEmpty(CurrentResponse.UniqueIdentifier)
                                            && !"AnonymousRespondent".Equals(CurrentResponse.UniqueIdentifier, StringComparison.InvariantCultureIgnoreCase) 
                                            && IncludeResponseDetails;
            _networkUserPlace.Visible = ApplicationManager.AppSettings.LogNetworkUser;
            _responseInfoPanel.Visible = IncludeResponseDetails;


            var userGuid = UserManager.GetUserGuid(CurrentResponse.UniqueIdentifier);

            var profileValues = ProfileManager.GetProfileProperties(CurrentResponse.UniqueIdentifier, true, true, userGuid);
      
            //Bind data
            if (_userPropertiesPlace.Visible)
            {
                _userPropertiesRepeater.DataSource = profileValues;
                _userPropertiesRepeater.DataBind();
            }

            _pageRepeater.DataSource = 
                CurrentResponse
                    .GetResponsePages()     
                    //.VistedPageStack -- look for the answers in the unvisited pages as well: survey editor may move item to the new page: CHECKBOX-1752
                    .Where(
                        page => page.PageType != TemplatePageType.Completion &&
                           page.GetItems().Any(pi => (pi is IAnswerable && ((IAnswerable)pi).HasAnswer) || PropertyBindingManager.IsBinded(pi.ID) || IsItemInUserMatrixResponse(pi)))
                    .OrderBy(page => page.Position);

            _pageRepeater.DataBind();
            
            //add Edit button if a user has permission Analysis.Responses.Edit
            _editResponsePlace.Visible = AuthorizationProvider.Authorize(CurrentPrincipal, _responseTemplate, "Analysis.Responses.Edit") && PrintMode == ExportMode.None;
            if (_editResponsePlace.Visible)
            {
                _editLink.Visible = true;
                _editLink.HRef = ResolveUrl(string.Format("~/Survey.aspx?edit=true&u={0}&r={1}&respondentGuid={2}", CurrentPrincipal.UserGuid, ResponseGuid, CurrentResponse.Respondent.UserGuid));
                _printLink.HRef =
                    ResolveUrl(string.Format("~/Forms/Surveys/Responses/View.aspx?responseGuid={0}&print=default",
                                             ResponseGuid));
            }
        }


        ///// <summary>
        ///// Apply the survey's style template.
        ///// </summary>
        ///// <param name="styleTemplateId"></param>
        //private void ApplyStyleTemplate(int? styleTemplateId)
        //{
        //    if (!styleTemplateId.HasValue)
        //    {
        //        return;
        //    }

        //    var st = StyleTemplateManager.GetStyleTemplate(styleTemplateId.Value);

        //    if (st == null)
        //    {
        //        return;
        //    }
        //}
       
        /// <summary>
        /// Get the temporary item formatter dictionary
        /// </summary>
        protected Dictionary<int, IItemFormatter> FormatterDictionary
        {
            get { return _formatterDictionary ?? (_formatterDictionary = new Dictionary<int, IItemFormatter>()); }
        }


        /// <summary>
        /// Get the html formatter for a given item
        /// </summary>
        /// <param name="itemTypeId"></param>
        protected IItemFormatter GetItemFormatter(int itemTypeId)
        {
            if (!FormatterDictionary.ContainsKey(itemTypeId))
            {
                var formatter = ItemFormatterFactory.GetItemFormatter(itemTypeId, "html");

                if (formatter == null)
                {
                    return null;
                }

                FormatterDictionary[itemTypeId] = formatter;
            }

            return FormatterDictionary[itemTypeId];
        }

        protected bool IsItemInUserMatrixResponse(Item it)
        {
            return MatrixField.IsMatrixInResponces(it.ID, ResponseGuid, User.Identity.Name);
        }

        protected IEnumerable<Item> ItemsToRender(RepeaterItemEventArgs e)
        {
            return ((ResponsePage) e.Item.DataItem)
                .Items
                .Where(it => (it is IAnswerable && ((IAnswerable)it).HasAnswer)
                             || (IncludeMessageItems && it.ShouldRender)
                             || PropertyBindingManager.IsBinded(it.ID)
                             || IsItemInUserMatrixResponse(it));
        }

        protected static bool IsItemBinded(Item item)
        {
            return PropertyBindingManager.IsBinded(item.ID);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        protected string GetPageTitle(ResponsePage page)
        {
            if (page == null)
            {
                return string.Empty;
            }

            if (page.PageType == TemplatePageType.HiddenItems)
            {
                return WebTextManager.GetText("/pageText/viewResponseDetails.aspx/hiddenItems");
            }

            var pageScoreString = string.Empty;

            if (ResponseTemplate.BehaviorSettings.EnableScoring && page.PageType == TemplatePageType.ContentPage)
            {
                var pageScore = page.Items.Where(item => item is IScored).Sum(item => ((IScored)item).GetScore());

                pageScoreString = string.Format("&nbsp;({0})", pageScore);
            }

            //Subtract 1 from page position to account for hidden items at page 1
            return string.Format("{0} {1}{2}", WebTextManager.GetText("/pageText/viewResponseDetails.aspx/page"), page.Position - 1, pageScoreString);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected string GetItemNumber(Item item)
        {
            var answerableItem = item as IAnswerable;

            if ((answerableItem == null
                 || !answerableItem.HasAnswer)
                && !IsItemBinded(item)
                && !IsItemInUserMatrixResponse(item))
            {
                return string.Empty;
            }

            var itemNumber = CurrentResponse.GetItemNumber(item.ID);

            return itemNumber.HasValue ? itemNumber.ToString() : string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected string GetItemHtml(Item item)
        {
            var answerableItem = item as IAnswerable;

            if ((answerableItem == null && !item.ShouldRender)
                || (answerableItem != null && !answerableItem.HasAnswer && !IsItemBinded(item) && !IsItemInUserMatrixResponse(item)))
            {
                return string.Empty;
            }

            //Add HTML for the item                                                
            IItemFormatter itemFormatter = GetItemFormatter(item.TypeID);

            if (itemFormatter == null)
                return string.Empty;


            // Only Matrix items
            if (item.ItemTypeName == "Matrix")
            {
                var json = PropertyBindingManager.GetResponseFieldState(item.ID, (Guid)CurrentResponse.GUID, CurrentResponse.UniqueIdentifier);

                if (!string.IsNullOrEmpty(json))
                {
                    var matrix = MatrixField.JsonToMatrix(json);

                    return Utilities.EncodeTagsInHtmlContent(
                        string.Format("<div>{0}</div><div class=\"profileAnswerFormatter Matrix\">{1}</div>", (item as MatrixItem).Text, MatrixField.MatrixRowsToJson(matrix)));
                }
            }
            //map options from binded response to the select1 obj
            else if (item.ItemTypeName == "RadioButtons")
            {
                var json = PropertyBindingManager.GetResponseFieldState(item.ID, (Guid)CurrentResponse.GUID, CurrentResponse.UniqueIdentifier);

                if (!string.IsNullOrEmpty(json))
                {
                    var radioButton = JsonConvert.DeserializeObject<RadioButtonField>(json);

                    var radioItem = item as Select1;
                    if (radioItem != null)
                    {
                        var radioButtonFieldOption = radioButton.Options.FirstOrDefault(radioOption => radioOption.IsSelected);
                        if (radioButtonFieldOption != null)
                        {
                            radioItem.Options.Clear();

                            foreach (var option in radioButton.Options)
                                radioItem.Options.Add(new ListOption() {Text = string.IsNullOrEmpty(option.Alias) ? option.Name : option.Alias, IsSelected = option.IsSelected});
                        }
                        
                    }
                }
            }


            return Utilities.EncodeTagsInHtmlContent(itemFormatter.Format(item, "html", ResponseTemplate.BehaviorSettings.EnableScoring));
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void pageRepeaterItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item == null || e.Item.DataItem == null)
            {
                return;
            }

            //Create a panel that will serve as a page break
            var responsePage = (ResponsePage)e.Item.DataItem;
            var templatePage = ResponseTemplate.GetPage(responsePage.PageId);
            if (templatePage.ShouldForceBreak.HasValue
                && templatePage.ShouldForceBreak.Value
                && templatePage.Position != ResponseTemplate.PageCount - 1)
            {
                var printPanel = new Panel();
                printPanel.Style["page-break-after"] = "always";
                e.Item.Controls.Add(printPanel);
            }

            var itemRepeater = e.Item.FindControl("_itemRepeater") as Repeater;

            if (itemRepeater == null)
            {
                return;
            }

            itemRepeater.DataSource =
                ItemsToRender(e);
            itemRepeater.DataBind();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void itemRepeaterItemCreated(object sender, RepeaterItemEventArgs e)
        {
            //Handle upload item file downloads, if necessary
            if (e.Item == null || e.Item.DataItem == null || e.Item.DataItem as UploadItem == null)
            {
                return;
            }

            var uploadItemPlace = e.Item.FindControl("_uploadedItemsPlace") as Panel;

            if (uploadItemPlace == null)
            {
                return;
            }

            var uploadItem = e.Item.DataItem as UploadItem;

            //<btn:CheckboxButton ID="_exportLanguageNamesBtn" runat="server" returnfile="true" CssClass="ckbxButton roundedCorners border999 shadow999 orangeButton" TextId="/pageText/settings/languageNames.aspx/exportToXml"/>
            foreach (long answerId in uploadItem.GetAllAnswerIds())
            {
                var downloadFile = new MultiLanguageLinkButton
                {
                    ID = "DownloadFile" + answerId,
                    CommandName = "AnswerID",
                    TextId = "/pageText/viewResponseDetails.aspx/downloadFile",
                    Text = "Download File",
                    CommandArgument = answerId.ToString()
                };
                downloadFile.Attributes["href"] = "javascript:doDownload(" + answerId + ")";
                uploadItemPlace.Controls.Add(downloadFile);
            }
        }
    }
}