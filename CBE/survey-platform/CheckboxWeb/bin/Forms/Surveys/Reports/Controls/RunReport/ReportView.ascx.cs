using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Amazon.SimpleDB.Model;
using Checkbox.Analytics;
using Checkbox.Common;
using Checkbox.Globalization.Text;
using Checkbox.Progress.DatabaseProvider;
using Checkbox.Wcf.Services;
using Checkbox.Web;
using Checkbox.Wcf.Services.Proxies;
using Checkbox.Forms;

namespace CheckboxWeb.Forms.Surveys.Reports.Controls.RunReport
{
    /// <summary>
    /// Callback to load data for a report page
    /// </summary>
    /// <param name="pageId"></param>
    /// <returns></returns>
    public delegate ReportPageMetaData GetReportPageCallback(int pageId);

    /// <summary>
    /// 
    /// </summary>
    public partial class ReportView : Checkbox.Web.Common.UserControlBase
    {
        private GetReportPageCallback GetPageCallback { get; set; }
        private GetReportItemCallback GetItemCallback { get; set; }
        protected int ReportId { get; set; }
        private int[] AllPageIds { get; set; }
        private string LanguageCode { get; set; }
        private string ProgressKey { get; set; }
        protected DropDownList _pageNumberList;
        private ExportMode _exportMode { get; set; }
        private bool ShowPageDropdown { get { return _exportMode == ExportMode.None; } }

        /// <summary>
        /// Initialize view
        /// </summary>
        /// <param name="reportData"></param>
        /// <param name="languageCode"></param>
        /// <param name="getReportPageCallback"></param>
        /// <param name="getReportItemCallback">'</param>
        /// <param name="pageIds"></param>
        /// <param name="progressKey"> </param>
        /// <param name="exportMode"> </param>
        public void Initialize(ReportMetaData reportData, int[] pageIds, string languageCode, GetReportPageCallback getReportPageCallback, 
            GetReportItemCallback getReportItemCallback, string progressKey, ExportMode exportMode)
        {
            if (getReportPageCallback == null)
            {
                throw new Exception("Unable to bind report view with NULL page callback.");
            }

            if (getReportItemCallback == null)
            {
                throw new Exception("Unable to bind report view with NULL item callback.");
            }

            ReportId = reportData.ReportId;
            GetPageCallback = getReportPageCallback;
            GetItemCallback = getReportItemCallback;
            AllPageIds = reportData.PageIds;
            LanguageCode = languageCode;
            ProgressKey = progressKey;
            _exportMode = exportMode;

            if (reportData.DisplaySurveyTitle)
            {
                _surveyTitlePanel.Visible = true;

                var responseTemplate = ResponseTemplateManager.GetResponseTemplate(reportData.ResponseTemplateId);
                _surveyTitle.InnerText = Utilities.AdvancedHtmlDecode(WebTextManager.GetText(responseTemplate.TitleTextID));
            }

            if (ShowPageDropdown)
                PopulatePageNumbers(reportData.PageIds, pageIds.Length == 1 ? pageIds[0] : (int?)null);

            LoadPages(pageIds);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="headerHtml"></param>
        /// <param name="footerHtml"></param>
        public void ApplyHeaderAndFooter(string headerHtml, string footerHtml)
        {
            _headerTxt.Text = headerHtml;
            _footerTxt.Text = footerHtml;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="allPageIds"></param>
        /// <param name="currentPageId"></param>
        private void PopulatePageNumbers(int[] allPageIds, int? currentPageId)
        {
            _pageNumberList = new DropDownList();

            var pageIndex = currentPageId.HasValue
                ? Array.IndexOf(allPageIds, currentPageId.Value)
                : -1;

            //Convert from index to page number
            pageIndex++;

            for (int pageNumber = 1; pageNumber <= allPageIds.Length; pageNumber++)
            {
                _pageNumberList.Items.Add(
                    new ListItem(
                        pageNumber.ToString(),
                        pageNumber.ToString()
                    )
                );
            }

            _pageNumberList.Items.Add(
                    new ListItem(
                        WebTextManager.GetText("/pageText/runAnalysis.aspx/allpages", WebTextManager.GetUserLanguage(), "All Pages"),
                        "ALL_PAGES"
                    )
                );

            if (pageIndex > 0
                && _pageNumberList.Items.FindByValue(pageIndex.ToString()) != null)
            {
                _pageNumberList.SelectedValue = pageIndex.ToString();
            }
            else
            {
                _pageNumberList.SelectedValue = "ALL_PAGES";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void LoadPages(IList<int> pageIds)
        {
            var pageContainer = _pageViewPlaceHolder;

            var pageView = LoadControl("~/Forms/Surveys/Reports/Controls/RunReport/PageView.ascx") as PageView;

            if (pageView == null)
            {
                pageContainer.Controls.Add(new LiteralControl("Unable to render page with NULL Id."));
                return;
            }

            pageContainer.Controls.Add(pageView);

            if (pageIds == null || pageIds.Count < 1)
            {
                pageContainer.Controls.Add(new LiteralControl("Unable to render page with NULL Id."));
                return;
            }

            var pageId = pageIds[0];

            var pageData = GetPageCallback(pageId);

            if (pageData == null)
            {
                pageContainer.Controls.Add(new LiteralControl("Unable to get data for page with id: " + pageId));
                return;
            }

            //get item ids
            IList<int> itemIds;
            if (pageIds.Count == 1)
                itemIds = pageData.ItemIds;
            else
            {
                var temp = AnalysisTemplateManager.GetAnalysisTemplate(ReportId, true);
                itemIds = temp.ListTemplateItemIds();
            }

            pageView.Initialize(ReportId, pageData.LayoutTemplateId, GetItemCallback, LanguageCode, ProgressKey, _exportMode, itemIds);
            pageView.BindRenderers();

            if (ShowPageDropdown && AllPageIds.Length > 1)
            {
                _pagingPlace.Visible = true;
                pageView.AddPageNumberControl(CreatePageNumberPanel());                
            }

            if (_exportMode == ExportMode.Pdf)
            {
                ProgressProvider.SetProgress(ProgressKey, TextManager.GetText(
                    "/controlText/analysisData/pdfExport/generatePdf"), 99, null);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Control CreatePageNumberPanel()
        {
            var pageNumberPanel = new Panel {CssClass = "pageNumbersContainer"};

            pageNumberPanel.Controls.Add(new LiteralControl("<div class=\"pageNumber\">" + WebTextManager.GetText("/pageText/runAnalysis.aspx/page", WebTextManager.GetUserLanguage(), "Page") + "&nbsp;"));
            pageNumberPanel.Controls.Add(_pageNumberList);
            pageNumberPanel.Controls.Add(new LiteralControl("</div>"));

            return pageNumberPanel;
        }

        //use to avoid page background cropping on pdf export
        protected bool DisableStyleBackground
        {
            get { return _exportMode == ExportMode.Pdf; }
        }
    }
}