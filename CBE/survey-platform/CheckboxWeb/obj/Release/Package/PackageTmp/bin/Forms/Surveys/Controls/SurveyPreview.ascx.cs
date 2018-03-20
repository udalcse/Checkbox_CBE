using System;
using System.Collections.Generic;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Globalization.Text;

namespace CheckboxWeb.Forms.Surveys.Controls
{
    /// <summary>
    /// Master control for survey preview.
    /// </summary>
    public partial class SurveyPreview : Checkbox.Web.Common.UserControlBase
    {
        /// <summary>
        /// Get/set language code
        /// </summary>
        protected string LanguageCode { get; set; }

        /// <summary>
        /// Get/set response template
        /// </summary>
        protected ResponseTemplate ResponseTemplate { get; set; }

        /// <summary>
        /// Get/set print mode
        /// </summary>
        protected ExportMode ExportMode { get; private set; }

        /// <summary>
        /// Get/set print mode
        /// </summary>
        protected RenderMode RenderMode { get; private set; }

        /// <summary>
        ///  
        /// </summary>
        protected string HeaderHtml { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected string FooterHtml { get; set; }

        /// <summary>
        /// Bind events
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            _previewNavigation.SelectedPageChanged += _previewNavigation_SelectedPageChanged;
        }

        /// <summary>
        /// Set preview placeholder visibility.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _previewPlaceHolder.Visible = ExportMode == ExportMode.None;
        }

        /// <summary>
        /// Initialize page preview control with specified pages.
        /// </summary>
        /// <param name="responseTemplate"></param>
        /// <param name="exportMode"></param>
        /// <param name="languageCode"></param>
        /// <param name="styleTemplateId"> </param>
        /// <param name="mode"> </param>
        public void Initialize(ResponseTemplate responseTemplate, ExportMode exportMode, string languageCode, int? styleTemplateId, RenderMode? mode)
        {
            //Store language code
            RenderMode = mode.HasValue ? mode.Value : RenderMode.SurveyPreview;
            LanguageCode = languageCode;
            ResponseTemplate = responseTemplate;
            ExportMode = exportMode;

            if (styleTemplateId.HasValue)
            {
                //Hardcode for english until ml style editing complete
                HeaderHtml = TextManager.GetText(string.Format("/styleTemplate/{0}/header", styleTemplateId), "en-US");
                FooterHtml = TextManager.GetText(string.Format("/styleTemplate/{0}/footer", styleTemplateId), "en-US");

                //HeaderHtml = TextManager.GetText(string.Format("/styleTemplate/{0}/header", styleTemplateId), languageCode);
                //FooterHtml = TextManager.GetText(string.Format("/styleTemplate/{0}/footer", styleTemplateId), languageCode);
            }

            //Initialize preview controls
            _previewNavigation.Initialize(responseTemplate);

            //Bind repeater
            if (!Page.IsPostBack)
            {
                //Get first content page, which is page two in all surveys.
                TemplatePage page = ResponseTemplate.GetPageAtPosition(2);

                string selectedPage = page != null && ExportMode == ExportMode.None
                    ? page.ID.ToString()
                    : "ALL_PAGES";

                //Set selected page
                _previewNavigation.SetSelectedPage(selectedPage);

                //Pages are 1 indexed.  1st page is hidden items, so default to first content page.
                BindView(selectedPage);
            }
        }

        /// <summary>
        /// Bind repeater to page list
        /// </summary>
        private void BindView(string selectedPage)
        {
            //Hide page repeater to start
            _pageRepeater.Visible = false;

            List<TemplatePage> pageList = new List<TemplatePage>();

            //All pages
            if ("ALL_PAGES".Equals(selectedPage, StringComparison.InvariantCultureIgnoreCase))
            {
                _pageRepeater.Visible = true;

                int[] pageIds = ResponseTemplate.ListTemplatePageIds();

                foreach (int pageId in pageIds)
                {
                    TemplatePage page = ResponseTemplate.GetPage(pageId);

                    if (page != null && page.PageType != TemplatePageType.HiddenItems)
                    {
                        pageList.Add(page);
                    }
                }
                _pageRepeater.DataSource = pageList;
                _pageRepeater.DataBind();
            }

            //Set visibility of special controls.
            _login.Visible = "LOGIN".Equals(selectedPage, StringComparison.InvariantCultureIgnoreCase);
            _password.Visible = "PASSWORD".Equals(selectedPage, StringComparison.InvariantCultureIgnoreCase);
            _progressSaved.Visible = "SAVE_AND_EXIT".Equals(selectedPage, StringComparison.InvariantCultureIgnoreCase);
            _responseSelect.Visible = "EDIT_RESPONSE".Equals(selectedPage, StringComparison.InvariantCultureIgnoreCase);
            _language.Visible = "LANGUAGE_SELECT".Equals(selectedPage, StringComparison.InvariantCultureIgnoreCase);

            //Initialize special controls
            _login.Initialize(LanguageCode, ResponseTemplate.ID.Value);

            //Completion page
            if ("COMPLETION".Equals(selectedPage, StringComparison.InvariantCultureIgnoreCase))
            {
                TemplatePage completionPage = ResponseTemplate.GetCompletionPage();

                if (completionPage != null)
                {
                    _pageRepeater.Visible = true;
                    _pageRepeater.DataSource = new List<TemplatePage> { completionPage };
                    _pageRepeater.DataBind();
                }
            }

            //Individual pages
            int selectedPageId;

            if (int.TryParse(selectedPage, out selectedPageId))
            {
                _pageRepeater.Visible = true;

                TemplatePage page = ResponseTemplate.GetPage(selectedPageId);

                if (page != null && page.PageType != TemplatePageType.HiddenItems)
                {
                    pageList.Add(page);
                }

                _pageRepeater.DataSource = pageList;
                _pageRepeater.DataBind();
            }
        }

        /// <summary>
        /// Handle selection of page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _previewNavigation_SelectedPageChanged(object sender, PreviewControlEventArgs e)
        {
            BindView(e.SelectedPage);
        }
    }
}