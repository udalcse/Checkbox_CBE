using System.Collections.Generic;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Forms.Items;
using Checkbox.Forms.PageLayout;
using Checkbox.Forms.Security.Principal;
using Checkbox.Management;
using Checkbox.Security;
using Checkbox.Users;
using Checkbox.Wcf.Services;
using Checkbox.Wcf.Services.Proxies;
using Checkbox.Web;
using Checkbox.Web.Forms.UI.Rendering;
using Checkbox.Web.Forms.UI.Templates;
using CheckboxWeb.Services;

namespace CheckboxWeb.Forms.Surveys.Controls.TakeSurvey
{
    public delegate IItemProxyObject[] GetResponsePageItemsCallback(int pageId);

    /// <summary>
    /// Page view control
    /// </summary>
    public partial class PageView : Checkbox.Web.Common.UserControlBase
    {
        #region Private Fields

        /// <summary>
        /// 
        /// </summary>
        private int FirstItemNumber { get; set; }

        /// <summary>
        /// 
        /// </summary>
        private ProgressBarOrientation ProgressBarOrientation { set; get; }

        /// <summary>
        /// Callback for loading items, if necessary
        /// </summary>
        private GetResponsePageItemsCallback GetPageItemsCallback { get; set; }

        /// <summary>
        /// List of item renderers
        /// </summary>
        private List<IItemRenderer> ItemRenderers { get; set; }

        #endregion

        public SurveyResponsePage ResponsePage { get; private set; }

        /// <summary>
        /// Get/set layout template
        /// </summary>
        public UserControlLayoutTemplate LayoutTemplate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool HasVisibleRenderers { private set; get; }

        /// <summary>
        /// Get page type
        /// </summary>
        public TemplatePageType PageType
        {
            get { return (TemplatePageType)Enum.Parse(typeof(TemplatePageType), ResponsePage.PageType); }
        }

        /// <summary>
        /// 
        /// </summary>
        public int CurrentPageNumber { set; get; }

        public int TotalPageNumbers { set; get; }

        /// <summary>
        /// Initialize page view without a response view.  Used for displaying survey messages such as no
        /// responses allowed, etc.
        /// </summary>
        public void InitializeForNoResponsePage(string languageCode)
        {
            AddLayoutTemplate(null, ResponseViewDisplayFlags.None, languageCode);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="control"></param>
        public void AddControlToPageView(Control control)
        {
            if (LayoutTemplate == null || control == null)
            {
                return;
            }

            LayoutTemplate.AddControlToZone("Default", control);
        }

        protected RenderMode RenderMode { set; get; }

        public ResponseTemplate ResponseTemplate { set; get; }

        /// <summary>
        /// Initialize view with response page.
        /// </summary>
        /// <param name="responsePage">The response page.</param>
        /// <param name="getItemsCallback">The get items callback.</param>
        /// <param name="renderMode">The render mode.</param>
        /// <param name="displayFlags">The display flags.</param>
        /// <param name="languageSettings">The language settings.</param>
        /// <param name="languageCode">The language code.</param>
        /// <param name="responseTemplate">The response template.</param>
        /// <param name="pageNumbers">The page numbers.</param>
        /// <param name="exportMode">The export mode.</param>
        /// <param name="items">The items.</param>
        /// <param name="state">The state.</param>
        /// <param name="completeEventItems">The complete event items.</param>
        /// <param name="responseSessionData">The response session data.</param>
        /// <param name="rGuid">The r unique identifier.</param>
        public void Initialize(
            SurveyResponsePage responsePage,
            GetResponsePageItemsCallback getItemsCallback,
            RenderMode renderMode,
            ResponseViewDisplayFlags displayFlags,
            SurveyLanguageSettings languageSettings,
            string languageCode,
            ResponseTemplate responseTemplate,
            PageNumberInfo pageNumbers,
            ExportMode exportMode, IEnumerable<IItemProxyObject> items = null,
            ResponseSessionState state = ResponseSessionState.None,
           IEnumerable<IItemProxyObject> completeEventItems = null, ResponseSessionData responseSessionData = null, Guid? rGuid = null)
        {
            RenderMode = renderMode;
            ResponsePage = responsePage;
            GetPageItemsCallback = getItemsCallback;
            FirstItemNumber = pageNumbers.FirstItemNumber;
            CurrentPageNumber = pageNumbers.CurrentPageNumber;
            ProgressBarOrientation = responseTemplate.StyleSettings.ProgressBarOrientation.Value;
            ResponseTemplate = responseTemplate;
            TotalPageNumbers = pageNumbers.TotalPageCount;
            //Update display flags accordingly for last page
            if (responsePage.IsLastContentPage)
            {
                displayFlags &= ~ResponseViewDisplayFlags.NextButton;
                displayFlags |= ResponseViewDisplayFlags.FinishButton;
            }

            if (!responsePage.IsFirstContentPage
                && responsePage.PageType != "Completion"
                && exportMode != ExportMode.Pdf
                && exportMode != ExportMode.ClientPdf)
                displayFlags |= ResponseViewDisplayFlags.BackButton;

            if (ApplicationManager.AppSettings.UseMandatoryCheckboxFooter)
            {
                displayFlags |= ResponseViewDisplayFlags.CheckboxFooter;
            }

            if (responsePage.PageType.Equals("Completion", StringComparison.InvariantCultureIgnoreCase))
            {
                displayFlags &= ~ResponseViewDisplayFlags.ProgressBar;
                displayFlags &= ~ResponseViewDisplayFlags.PageNumbers;

                if (responseTemplate.BehaviorSettings.DisplayPDFDownloadButton)
                {
                    if (_printButton != null && exportMode == ExportMode.None &&
                        (renderMode == RenderMode.Survey || renderMode == RenderMode.SurveyMobile))
                    {

                        // show only if session data is not null and user is taking the survey
                        if (responseSessionData != null)
                        {
                            var respondent = !string.IsNullOrWhiteSpace(responseSessionData.AuthenticatedRespondentUid)
                                ? UserManager.GetUserPrincipal(responseSessionData.AuthenticatedRespondentUid)
                                : new AnonymousRespondent(responseSessionData.AnonymousRespondentGuid ?? Guid.Empty);

                            // show warn message if users are reached the rimit of taking surveys 
                            if (!ResponseTemplateManager.MoreResponsesAllowed(responseTemplate.ID.Value,
                                responseTemplate.BehaviorSettings.MaxTotalResponses,
                                responseTemplate.BehaviorSettings.MaxResponsesPerUser,
                                respondent,
                                responseTemplate.BehaviorSettings.AnonymizeResponses))
                            {
                                _limitReachedWarnMessage.Visible = true;
                            }
                        }
                        //If user finishes  the survey we need to put print btn on the page.

                        //since url is wrong decoded, amp;i is uses if i values not found , should be found solution how to decode url 

                        var printButtonUrl =
                            $"Forms/Surveys/Export.aspx?s={responseTemplate.ID}&loc=EN-US&surveyGuid={responseTemplate.GUID}&printClientPdf={true}";

                        if (responseSessionData != null && responseSessionData.ResumeInstanceId != null)
                        {
                            printButtonUrl += $"&iid={responseSessionData.ResumeInstanceId}";
                        }

                        var invitationId = string.Empty;

                        var request = HttpContext.Current?.Request;

                        if (request != null)
                        {
                            //since url is wrong decoded, amp;i is uses if i values not found , should be found solution how to decode url 
                            invitationId = request.QueryString["i"] ?? request.QueryString["amp;i"];

                            if (!string.IsNullOrWhiteSpace(invitationId))
                                printButtonUrl += $"&i={invitationId}";
                        }

                        _printButton.Attributes.Add("href",
                            $"javascript:showDialog('{printButtonUrl}','properties', '', null, '#_surveyForm')");
                        _printButton.Visible = true;

                        if (RenderMode == RenderMode.SurveyPreview || RenderMode == RenderMode.SurveyMobilePreview)
                        {
                            //since it is a link it does not have disabled attr 
                            _printButton.OnClientClick = "return false";
                            _printButton.Style.Add("cursor", "not-allowed");
                        }
                    }
                }
            }


            //Add layout template to control hierarchy
            AddLayoutTemplate(responsePage.LayoutTemplateId, displayFlags, languageCode);

            if (exportMode == ExportMode.ClientPdf)
            {
                if (items != null)
                    AddAllItemsToThePage(items.ToList(), completeEventItems, renderMode, ResponseViewDisplayFlags.None,
                        exportMode, state , rGuid);
            }
            else
            {
                AddItems(responsePage.PageId, renderMode, displayFlags, exportMode, rGuid);
            }

            SetDefaultButton();

            //Disable back,forward, next, etc. buttons if render mode is not take survey
            if (renderMode != RenderMode.Survey && renderMode != RenderMode.SurveyMobile)
                _resetBtn.OnClientClick = "return false;";

            //Set button texts
            _finishBtn.Text = SurveyEditorServiceImplementation.GetSurveyText("FINISH", responseTemplate.ID.Value,
                languageCode, languageSettings.SupportedLanguages.ToArray());
            _nextBtn.Text = SurveyEditorServiceImplementation.GetSurveyText("CONTINUE", responseTemplate.ID.Value,
                languageCode, languageSettings.SupportedLanguages.ToArray());
            _saveBtn.Text = SurveyEditorServiceImplementation.GetSurveyText("SAVEPROGRESS",
                responseTemplate.ID.Value, languageCode, languageSettings.SupportedLanguages.ToArray());
            _resetBtn.Text = SurveyEditorServiceImplementation.GetSurveyText("FORM_RESET", responseTemplate.ID.Value,
                languageCode, languageSettings.SupportedLanguages.ToArray());
            _resetLnk.Text = SurveyEditorServiceImplementation.GetSurveyText("FORM_RESET", responseTemplate.ID.Value,
                languageCode, languageSettings.SupportedLanguages.ToArray());
            _prevBtn.Text = SurveyEditorServiceImplementation.GetSurveyText("BACK", responseTemplate.ID.Value,
                languageCode, languageSettings.SupportedLanguages.ToArray());
            _titleLbl.Text = SurveyEditorServiceImplementation.GetSurveyText("TITLE", responseTemplate.ID.Value,
                languageCode, languageSettings.SupportedLanguages.ToArray());
            _logoText.Text = GetCheckboxFooter();



            _pageNumberLbl.Text = pageNumbers.CurrentPageNumber > 0
                ? languageSettings.GetPageNumber(languageCode, pageNumbers.CurrentPageNumber, pageNumbers.TotalPageCount)
                : "";

            _hiddenCurrentPage.Value = CurrentPageNumber.ToString();

            //set attributes
            _resetLnk.Attributes["data-role"] = "button";
            _resetLnk.Attributes["href"] = "#";

            _finishBtn.Attributes["data-action"] = "finish";
            _nextBtn.Attributes["data-action"] = "next";
            _saveBtn.Attributes["data-action"] = "save";
            _prevBtn.Attributes["data-action"] = "back";

            if (WebUtilities.IsAjaxifyingSupported(HttpContext.Current.Request))
            {
                const string onClientClick = "";

                _finishBtn.OnClientClick = onClientClick;
                _nextBtn.OnClientClick = onClientClick;
                _saveBtn.OnClientClick = onClientClick;
                _prevBtn.OnClientClick = onClientClick;
            }

            FirstItemNumber = pageNumbers.FirstItemNumber;

            SetVisibility(_progressBar, ResponseViewDisplayFlags.ProgressBar, displayFlags);
            SetVisibility(_progressPanel, ResponseViewDisplayFlags.ProgressBar, displayFlags);
            SetVisibility(_ckbxLogoFooterPanel, ResponseViewDisplayFlags.CheckboxFooter, displayFlags);


            //remove finish button if it is pdf mode for printing 
            if (exportMode == ExportMode.ClientPdf)
                _finishBtn.Visible = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="headerHtml"></param>
        /// <param name="footerHtml"></param>
        public void ApplyHeaderAndFooter(string headerHtml, string footerHtml)
        {
            _headerTxt.Text = Utilities.ReplaceHtmlAttributes(headerHtml, false);
            _footerTxt.Text = Utilities.ReplaceHtmlAttributes(footerHtml, false);
        }

        /// <summary>
        /// Get checkbox logo
        /// </summary>
        /// <returns></returns>
        public string GetCheckboxFooter()
        {
            return
                "<p align=\"center\"><a target=\"_blank\" href=\"http://www.checkbox.com\" style=\"color: #808080; font-size: 10px; font-family: Arial; text-decoration: none\">This survey is powered by CHECKBOX&reg; " +
                "Survey Software - &copy;" + DateTime.Now.Year + " Checkbox Survey</a></p>";
        }

        /// <summary>
        /// Bind renderers to model data
        /// </summary>
        public void BindRenderers()
        {
            HasVisibleRenderers = false;

            if (ItemRenderers != null)
            {
                foreach (IItemRenderer itemRenderer in ItemRenderers)
                {
                    itemRenderer.BindModel();

                    if (itemRenderer.Visible)
                        HasVisibleRenderers = true;
                }
            }

            //If no visible renderers, fire MoveNext event.
            if (PageType == TemplatePageType.Completion)
                HasVisibleRenderers = true;
        }

        /// <summary>
        /// Update items with input state
        /// </summary>
        public void UpdateItems()
        {
            if (ItemRenderers != null)
            {
                foreach (IItemRenderer itemRenderer in ItemRenderers)
                {
                    itemRenderer.UpdateModel();
                }
            }
        }

        /// <summary>
        /// List survey response items.  List can be used when posting response data back to server
        /// after calling UpdateItems()
        /// </summary>
        /// <returns></returns>
        public List<IItemProxyObject> ListItems()
        {
            var responseItems = new List<IItemProxyObject>();

            if (ItemRenderers != null)
            {
                responseItems.AddRange(ItemRenderers.Select(itemRenderer => itemRenderer.DataTransferObject));
            }

            return responseItems;
        }

        /// <summary>
        /// Set default button
        /// </summary>
        private void SetDefaultButton()
        {
            //Set default button, if desired
            if (ApplicationManager.AppSettings.SetSurveyDefaultButton)
            {
                //TODO: Verify no crash when default button set and no items exist on
                // completion page!

                if (Page == null || Page.Form == null)
                    return;

                if (_finishBtn != null && _finishBtn.Visible)
                    Page.Form.DefaultButton = _finishBtn.UniqueID;
            }
        }

        /// <summary>
        /// Add layout template
        /// </summary>
        /// <param name="layoutTemplateId"></param>
        /// <param name="displayFlags"></param>
        /// <param name="languageCode"></param>
        private void AddLayoutTemplate(int? layoutTemplateId, ResponseViewDisplayFlags displayFlags, string languageCode)
        {
            //Get template for the page
            LayoutTemplate = GetLayoutTemplate(layoutTemplateId, languageCode);

            if (LayoutTemplate == null)
                return;

            LayoutTemplate.ID = "Layout_" + LayoutTemplate.ID;

            //Clear layout place
            _pageLayoutPlace.Controls.Clear();

            //Add template to controls collection
            _pageLayoutPlace.Controls.Add(LayoutTemplate);

            //Set visibility of various controls
            SetVisibility(_titlePanel, ResponseViewDisplayFlags.Title, displayFlags);
            SetVisibility(_pageNumbersPanel, ResponseViewDisplayFlags.PageNumbers, displayFlags);
            SetVisibility(_progressPanel, ResponseViewDisplayFlags.ProgressBar, displayFlags);
            SetVisibility(_saveBtn, ResponseViewDisplayFlags.SaveButton, displayFlags);
            SetVisibility(_prevBtn, ResponseViewDisplayFlags.BackButton, displayFlags);
            SetVisibility(_nextBtn, ResponseViewDisplayFlags.NextButton, displayFlags);
            SetVisibility(_finishBtn, ResponseViewDisplayFlags.FinishButton, displayFlags);

            //Add controls to layout
            AddControlToLayout(_titlePanel, LayoutTemplate.TitleZone, LayoutTemplate);
            AddControlToLayout(_pageNumbersPanel, LayoutTemplate.PageNumberZone, LayoutTemplate);
            AddControlToLayout(_prevBtn, LayoutTemplate.PreviousButtonZone, LayoutTemplate);
            AddControlToLayout(_nextBtn, LayoutTemplate.NextButtonZone, LayoutTemplate);
            AddControlToLayout(_finishBtn, LayoutTemplate.FinishButtonZone, LayoutTemplate);
            AddControlToLayout(_saveBtn, LayoutTemplate.SaveAndQuitButtonZone, LayoutTemplate);
            AddControlToLayout(_headerPanel, LayoutTemplate.HeaderZone, LayoutTemplate);
            AddControlToLayout(_footerPanel, LayoutTemplate.FooterZone, LayoutTemplate);

            ILayoutZone layoutZone = LayoutTemplate.ProgressBarTopZone;
            switch (ProgressBarOrientation)
            {
                case ProgressBarOrientation.Top_Left:
                    _progressPanel.CssClass = "progressWrapper";
                    break;
                case ProgressBarOrientation.Top_Center:
                    _progressPanel.CssClass = "progressWrapper centered";
                    break;
                case ProgressBarOrientation.Bottom_Left:
                    _progressPanel.CssClass = "progressWrapper bottom";
                    layoutZone = LayoutTemplate.ProgressBarBottomZone;
                    break;
                case ProgressBarOrientation.Bottom_Center:
                    _progressPanel.CssClass = "progressWrapper bottom centered";
                    layoutZone = LayoutTemplate.ProgressBarBottomZone;
                    break;
            }
            AddControlToLayout(_progressPanel, layoutZone, LayoutTemplate);

            _resetLnk.Visible = false;
            _resetBtn.Visible = false;
            if (RenderMode == RenderMode.SurveyMobile || RenderMode == RenderMode.SurveyMobilePreview)
            {
                AddControlToLayout(_resetLnk, LayoutTemplate.FormResetZone, LayoutTemplate);
                SetVisibility(_resetLnk, ResponseViewDisplayFlags.FormResetButton, displayFlags);
            }
            else
            {
                SetVisibility(_resetBtn, ResponseViewDisplayFlags.FormResetButton, displayFlags);
                AddControlToLayout(_resetBtn, LayoutTemplate.FormResetZone, LayoutTemplate);
            }
        }

        /// <summary>
        /// Add a control to the layout
        /// </summary>
        /// <param name="c"></param>
        /// <param name="zone"></param>
        /// <param name="layoutTemplate"></param>
        private static void AddControlToLayout(Control c, ILayoutZone zone, IWebLayoutTemplate layoutTemplate)
        {
            if (c != null && zone != null)
            {
                layoutTemplate.AddControlToZone(zone.ZoneName, c);
            }
        }

        /// <summary>
        /// Set visibility of controls if the current control state matches the required state
        /// </summary>
        /// <param name="c"></param>
        /// <param name="requiredState"></param>
        /// <param name="currentState"></param>
        private static void SetVisibility(Control c, ResponseViewDisplayFlags requiredState, ResponseViewDisplayFlags currentState)
        {
            if (c != null)
            {
                c.Visible = ((requiredState & currentState) == requiredState);
            }
        }

        /// <summary>
        /// Get layout template to use for the page.
        /// </summary>
        /// <param name="layoutTemplateId"></param>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        private static UserControlLayoutTemplate GetLayoutTemplate(int? layoutTemplateId, string languageCode)
        {
            if (layoutTemplateId.HasValue && layoutTemplateId.Value > 0)
            {
                var layoutTemplateData =
                    PageLayoutTemplateManager.GetPageLayoutTemplateData(layoutTemplateId.Value);

                var layoutTemplate = layoutTemplateData.CreateTemplate(languageCode);

                if (layoutTemplate is UserControlLayoutTemplate)
                {
                    return (UserControlLayoutTemplate)layoutTemplate;
                }
            }


            //Fallback to default template
            if (HttpContext.Current != null
                && HttpContext.Current.Handler is System.Web.UI.Page)
            {
                if (
                    File.Exists(
                        HttpContext.Current.Server.MapPath(
                            "~/Forms/Surveys/Controls/TakeSurvey/Templates/DefaultTemplate.ascx")))
                {
                    Control defaultTemplate =
                        ((System.Web.UI.Page)HttpContext.Current.Handler).LoadControl(
                            "~/Forms/Surveys/Controls/TakeSurvey/Templates/DefaultTemplate.ascx");

                    if (defaultTemplate is UserControlLayoutTemplate)
                    {
                        return (UserControlLayoutTemplate)defaultTemplate;
                    }
                }
            }


            return null;
        }


        /// <summary>
        /// Add list of items to page
        /// </summary>
        /// <param name="pageId"></param>
        /// <param name="renderMode"></param>
        /// <param name="displayFlags"></param>
        /// <param name="exportMode"></param>
        private void AddItems(int pageId, RenderMode renderMode, ResponseViewDisplayFlags displayFlags, ExportMode exportMode , Guid? rGuid)
        {
            IEnumerable<IItemProxyObject> items = GetPageItemsCallback(pageId);

            ItemRenderers = new List<IItemRenderer>();

            var currentItemNum = 0;
            var bypassedItemCount = 0;
            var isFirstItemZone = true;

            foreach (var item in items)
            {
                if (!ShouldRender(item)) continue;
                //fill showAsterisks field from the survey
                item.Metadata["showAsterisks"] = ((displayFlags & ResponseViewDisplayFlags.Asterisks) == ResponseViewDisplayFlags.Asterisks).ToString();

                //Add item number, if present and item numbers should be shown
                int? currentItemPosition = null;
                if ((displayFlags & ResponseViewDisplayFlags.ItemNumbers) == ResponseViewDisplayFlags.ItemNumbers
                    && item is SurveyResponseItem
                    && ((SurveyResponseItem)item).IsAnswerable
                    && !"HiddenItem".Equals(item.TypeName, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (ResponsePage.ItemNumbers != null)
                    {
                        currentItemPosition = FirstItemNumber + ResponsePage.ItemNumbers[currentItemNum] -
                                              bypassedItemCount;
                    }
                    else
                    {
                        currentItemPosition = FirstItemNumber++;
                    }
                }
                else
                {
                    bypassedItemCount++;
                }
                ++currentItemNum;

                if (item.TypeName == "Matrix")
                    MatrixField.BindMatrixProfielFieldToItem((ItemProxyObject)item, renderMode, rGuid);
                else if (item.TypeName == "RadioButtons")
                    RadioButtonField.BindRadioProfileFieldToItem((ItemProxyObject)item, renderMode, rGuid);

                //Get a renderer for the item
                var renderer = WebItemRendererManager.GetItemRenderer(item, renderMode, currentItemPosition, exportMode);

                if (renderer != null)
                {
                    renderer.ID = "Renderer_" + item.ItemId;

                    //Add to layout or to page directly
                    if (LayoutTemplate != null)
                    {
                        ILayoutZone itemZone = LayoutTemplate.GetItemZone(item.ItemId);

                        if (itemZone != null)
                        {
                            //Add spacer & renderer
                            if (!isFirstItemZone)
                                LayoutTemplate.AddControlToZone(itemZone.ZoneName, new Panel { CssClass = "Page", ID = "Spacer" + item.ItemId });       //CSS class name is a legacy of pre-5.0
                            LayoutTemplate.AddControlToZone(itemZone.ZoneName, GetWrappedRenderer(renderer));
                            isFirstItemZone = false;
                        }
                    }

                    var itemRenderer = renderer as IItemRenderer;
                    if (itemRenderer != null)
                    {
                        ItemRenderers.Add(itemRenderer);
                    }
                }
            }
        }

        private bool MatrixHasAnswers(ItemProxyObject item)
        {
            var additionalData = (MatrixAdditionalData)item?.AdditionalData;

            if (additionalData?.ChildItems != null)
            {
                var childItems = additionalData.ChildItems.Select(childItem => childItem.Cast<SurveyResponseItem>());

                return (from childItem in childItems from answer in childItem.Select(answer => answer.Answers) from currentAnswer in answer select currentAnswer)
                    .Any(currentAnswer => !string.IsNullOrWhiteSpace(currentAnswer.AnswerText) || currentAnswer.AnswerId != 0);
            }

            return false;
        }

        private bool RadioBtnHasAnswers(ItemProxyObject item)
        {
            var responseItem = item as SurveyResponseItem;
            if (responseItem?.Options != null)
            {
                return responseItem.Options.Any(option => option.IsSelected);
            }

            return false;
        }

        /// <summary>
        /// Add list of items to page
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="completeEventsItems">The complete events items.</param>
        /// <param name="renderMode">The render mode.</param>
        /// <param name="displayFlags">The display flags.</param>
        /// <param name="exportMode">The export mode.</param>
        /// <param name="state">The state.</param>
        private void AddAllItemsToThePage(List<IItemProxyObject> items, IEnumerable<IItemProxyObject> completeEventsItems, RenderMode renderMode, ResponseViewDisplayFlags displayFlags, ExportMode exportMode, ResponseSessionState state, Guid? rGuid = null)
        {
            ItemRenderers = new List<IItemRenderer>();

            var currentItemNum = 0;
            var bypassedItemCount = 0;
            var isFirstItemZone = true;


            if (state == ResponseSessionState.Completed)
            {
                //filter matrices 
                foreach (var item in items)
                {
                    if (item.TypeName == "Matrix")
                        MatrixField.BindMatrixProfielFieldToItem((ItemProxyObject)item, renderMode, rGuid);
                    else if (item.TypeName == "RadioButtons")
                        RadioButtonField.BindRadioProfileFieldToItem((ItemProxyObject)item, renderMode, rGuid);
                }

                items =
                    items.Where(
                        item => DoesItemHaveAnswers((SurveyResponseItem)item)
                            || !string.IsNullOrEmpty(((SurveyResponseItem)item).Metadata["defaultText"])
                            || ((SurveyResponseItem)item).TypeName == "Message").ToList();   // leave only answered questions and questions with default text and message items
            }

            if (completeEventsItems != null && completeEventsItems.Any())
                items.AddRange(completeEventsItems);

            Dictionary<int?, bool?> pageBreaks = ResponseTemplate.TemplatePages
                        .ToDictionary(d => d.Value.ID, d => d.Value.ShouldForceBreak);

            foreach (var item in items)
            {
                if (!ShouldRender(item)) continue;
                //fill showAsterisks field from the survey
                item.Metadata["showAsterisks"] = ((displayFlags & ResponseViewDisplayFlags.Asterisks) == ResponseViewDisplayFlags.Asterisks).ToString();

                //Add item number, if present and item numbers should be shown
                int? currentItemPosition = null;
                if ((displayFlags & ResponseViewDisplayFlags.ItemNumbers) == ResponseViewDisplayFlags.ItemNumbers
                    && item is SurveyResponseItem
                    && ((SurveyResponseItem)item).IsAnswerable
                    && !"HiddenItem".Equals(item.TypeName, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (ResponsePage.ItemNumbers != null)
                    {
                        currentItemPosition = FirstItemNumber + ResponsePage.ItemNumbers[currentItemNum] -
                                              bypassedItemCount;
                    }
                    else
                    {
                        currentItemPosition = FirstItemNumber++;
                    }
                }
                else
                {
                    bypassedItemCount++;
                }
                ++currentItemNum;

                if (item.TypeName == "Matrix")
                    MatrixField.BindMatrixProfielFieldToItem((ItemProxyObject)item, renderMode, rGuid);
                else if (item.TypeName == "RadioButtons")
                    RadioButtonField.BindRadioProfileFieldToItem((ItemProxyObject)item, renderMode, rGuid);

                //Get a renderer for the item
                var renderer = WebItemRendererManager.GetItemRenderer(item, renderMode, currentItemPosition, exportMode);

                if (renderer != null)
                {
                    renderer.ID = "Renderer_" + item.ItemId;

                    //Add to layout or to page directly
                    if (LayoutTemplate != null)
                    {
                        ILayoutZone itemZone = LayoutTemplate.GetItemZone(item.ItemId);

                        if (itemZone != null)
                        {
                            //Add spacer & renderer
                            if (!isFirstItemZone)
                                LayoutTemplate.AddControlToZone(itemZone.ZoneName, new Panel { CssClass = "Page", ID = "Spacer" + item.ItemId });       //CSS class name is a legacy of pre-5.0
                            LayoutTemplate.AddControlToZone(itemZone.ZoneName, GetWrappedRenderer(renderer));
                            isFirstItemZone = false;
                        }
                    }

                    int pageId = ((ItemProxyObject)item).PageId;
                    var itemProxy = (ItemProxyObject)item;
                    if (ShouldBreakPageAfterItem(itemProxy, pageId, pageBreaks))
                    {
                        var printPanel = GetPageBreakPanel(itemProxy);
                        renderer.Controls.Add(printPanel);
                    }

                    var itemRenderer = renderer as IItemRenderer;
                    if (itemRenderer != null)
                    {
                        ItemRenderers.Add(itemRenderer);
                    }
                }
            }
        }

        private bool DoesItemHaveAnswers(SurveyResponseItem item)
        {
            bool result = true;

            switch (item.TypeName)
            {
                case "Matrix":
                    result = MatrixHasAnswers(item);
                    break;
                case "RadioButtonScale":
                    result = RadioBtnHasAnswers(item);
                    break;
                case "RadioButtons":
                    result = RadioBtnHasAnswers(item);
                    break;
                case "MultiLineText":
                    var answer = item.Answers.FirstOrDefault();
                    result = !string.IsNullOrWhiteSpace(answer?.AnswerText);
                    break;
                default:
                    if (item.Answers == null || !item.Answers.Any())
                        result = false;
                    break;
            }

            return result;

        }

        /// <summary>
        /// Returns a panel that indicates pdf writer that it should make a page preak after panel
        /// </summary>
        /// <param name="item">Item after which page should break</param>
        /// <returns></returns>
        private static Panel GetPageBreakPanel(ItemProxyObject item)
        {
            var printPanel = new Panel();
            if (item.TypeName == "Matrix") printPanel.Attributes.Add("data-matrix-break", "true");
            else printPanel.Style["page-break-after"] = "always";
            printPanel.Style["padding-bottom"] = "30px";

            return printPanel;
        }

        /// <summary>
        /// Determines whether there is a page break after current item or not
        /// </summary>
        /// <param name="item"></param>
        /// <param name="pageId"></param>
        /// <param name="pageBreaks"></param>
        /// <returns></returns>
        private bool ShouldBreakPageAfterItem(ItemProxyObject item, int pageId, Dictionary<int?, bool?> pageBreaks)
        {
            return item.IsLastItemOnPage.HasValue
                   && item.IsLastItemOnPage.Value
                   && pageBreaks.Keys.Contains(pageId) &&
                   pageBreaks[pageId] == true;
        }

        /// <summary>
        /// Prevents from rendering radio buttons items without options
        /// </summary>
        /// <param name="proxyObject"></param>
        /// <returns></returns>
        private static bool ShouldRender(IItemProxyObject proxyObject)
        {
            if (proxyObject.TypeName == "RadioButtons" && proxyObject is SurveyResponseItem)
            {
                var surveyResponseItem = proxyObject as SurveyResponseItem;

                if ((surveyResponseItem.Options.Length == 0 ||
                    surveyResponseItem.Options.Select(o => o.Text).All(string.IsNullOrWhiteSpace)) && !PropertyBindingManager.IsBinded(proxyObject.ItemId))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Wrap renderer in a panel
        /// </summary>
        /// <param name="renderer"></param>
        /// <returns></returns>
        private static Control GetWrappedRenderer(Control renderer)
        {
            var container = new Panel { ID = renderer.ID + "_Wrapper" };

            container.Style["clear"] = "both";

            container.Controls.Add(renderer);

            return container;
        }

        /// <summary>
        /// Sets the percentage completed 
        /// </summary>
        /// <param name="percent"></param>
        public void SetProgressPercent(double percent)
        {
            _progressBar.Progress = percent;
        }
    }
}