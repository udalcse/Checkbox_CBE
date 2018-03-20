using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Globalization.Text;
using Checkbox.Management;
using Checkbox.PdfExport;
using Checkbox.Styles;
using Checkbox.Users;
using Checkbox.Wcf.Services.Proxies;
using Checkbox.Web;
using Checkbox.Web.Analytics;
using Checkbox.Web.Page;

using Checkbox.Wcf.Services.LocalProxies;

namespace CheckboxWeb
{
    /// <summary>
    /// Run report main page
    /// </summary>
    public partial class RunAnalysis : PersistedStatePage
    {
        //Service used to get information about the structure of the report
        private IReportMetaDataService _metaDataService;

        //Service used to get actual result data for the report
        private IReportDataService _dataService;

        //Query parameters associated with analysis
        private AnalysisQueryParameters _params;

        //Report data
        protected ReportMetaData _reportData;

        protected ExportMode _exportMode;

        protected PdfExportOrientation _orientation;

        //User ticket
        private string AuthTicket
        {
            get { return string.Empty; }
        }

        protected int GetReportId()
        {
            return _reportData.ReportId;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        protected override void ShowError(string message, Exception ex)
        {
            //We must report the error to the user here, otherwise just a blank page is shown
            _errorPanel.Visible = true;
            _errorMessage.Text = message;
            _errorSubMessage.Text = ex.Message;
        }

        /// <summary>
        /// Perform page initialization
        /// </summary>
        protected override void OnPageInit()
        {
            //TODO: Anonymous user
            base.OnPageInit();

            //Validate query string
            ValidateQueryParameters();

            //WCF client for eventual use w/distributed survey front-end servers and central 
            // reporting server
            //_metaDataService = new ReportMetaDataServiceProxy("PoxReportMetaDataServiceClient");
            //_dataService = new ReportDataServiceProxy("PoxReportDataServiceClient");

            _metaDataService = new LocalReportMetaDataServiceProxy();
            _dataService = new LocalReportDataServiceProxy();

            //Get report data
            var getReportResult = _params.AnalysisId > 0
                ? _metaDataService.GetReportWithId(AuthTicket, _params.AnalysisId)
                : _metaDataService.GetReportWithGuid(AuthTicket, _params.AnalysisGuid.HasValue ? _params.AnalysisGuid.Value : _params.AnalysisGuid_4_7.Value);

            if (!getReportResult.CallSuccess)
            {
                if (typeof(ServiceAuthorizationException).ToString().Equals(getReportResult.FailureExceptionType))
                {
                    FormsAuthentication.RedirectToLoginPage();
                    return;
                }

                throw new Exception("Unable to load report.  " + getReportResult.FailureMessage);
            }

            _reportData = getReportResult.ResultData;

            var responseTemplate = ResponseTemplateManager.GetResponseTemplate(_reportData.ResponseTemplateId);
            if (responseTemplate.IsDeleted)
            {
                throw new Exception("The report is no longer accessible.");
            }

            if (_reportData.PageIds.Length <= 0)
            {
                throw new Exception("Report was loaded, but it has no pages.");
            }

            //Ensure language set and marked "active" by response template manager. "Active" flag affects
            // how metadata proxy gets data.
            _params.LanguageCode = string.IsNullOrEmpty(_params.LanguageCode)
                ? _reportData.DefaultSurveyLanguage
                : _params.LanguageCode;

            ResponseTemplateManager.EnsureActiveLanguage(_params.LanguageCode);

            Page.Title = _reportData.Name;
            _pageTitle.Text = _reportData.Name;
            _pdfExportButton.Visible = _reportData.DisplayPdfExportButton;

            _exportMode = _params.ExportMode;
            _orientation = _params.Orientation;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageLoad()
        {
            base.OnPageLoad();

            var pageIds = new List<int>();

            //Determine if all pages or single page
            if (_params.IsAllPages)
            {
                pageIds.AddRange(_reportData.PageIds);
            }
            else
            {
                if (_params.PageNumber > 0 && _reportData.PageIds.Length >= _params.PageNumber)
                {
                    pageIds.Add(_reportData.PageIds[_params.PageNumber - 1]);
                }
                else
                {
                    pageIds.Add(_reportData.PageIds[0]);
                }
            }

            //Initialize view
            _reportView.Initialize(_reportData, pageIds.ToArray(), _params.LanguageCode, GetReportPage, GetReportItem, _params.ProgressKey, _params.ExportMode);

            //Style template
            ApplyStyleTemplate(_reportData.StyleTemplateId);
        }

        /// <summary>
        /// Get page
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public IItemProxyObject GetReportItem(int itemId)
        {
            var result = _dataService.GetItem(
                AuthTicket,
                _reportData.ReportId,
                itemId,
                _reportData.IncludeIncompleteResponses,
                _params.LanguageCode);

            if (!result.CallSuccess)
            {
                throw new Exception("An error occurred getting a report item: " + result.FailureMessage);
            }

            return result.ResultData;
        }

        /// <summary>
        /// Get page
        /// </summary>
        /// <param name="pageId"></param>
        /// <returns></returns>
        public ReportPageMetaData GetReportPage(int pageId)
        {
            var result = _metaDataService.GetReportPageWithId(AuthTicket, _reportData.ReportId, pageId);

            if (!result.CallSuccess)
            {
                throw new Exception("An error occurred getting a report page: " + result.FailureMessage);
            }

            return result.ResultData;
        }

        /// <summary>
        /// Ensure query parameters are valid
        /// </summary>
        private void ValidateQueryParameters()
        {
            _params = new AnalysisQueryParameters();

            //Load values
            WebParameterAttribute.SetValues(_params, HttpContext.Current);

            if (!_params.ValidateQueryParamters())
            {
                throw new Exception("Analysis URL appears to be invalid.  A valid URL must identify the analysis to run.");
            }
        }

        /// <summary>
        /// Set the user context for reporting on admin screens
        /// </summary>
        protected override void SetUserContext()
        {
            var currentUser = UserManager.GetCurrentPrincipal();

            var userName = currentUser != null
                ? currentUser.Identity.Name
                : "Anonymous";

            //Set the caller's context
            UserManager.SetUserContext(
                userName,
                HttpContext.Current.Request.ServerVariables["HTTP_HOST"],
                HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"],
                HttpContext.Current.Request.ServerVariables["HTTP_USER_AGENT"],
                HttpContext.Current.Request.ServerVariables["URL"]);
        }


        /// <summary>
        /// Apply the survey's style template.
        /// </summary>
        /// <param name="styleTemplateId"></param>
        private void ApplyStyleTemplate(int? styleTemplateId)
        {
            if (!styleTemplateId.HasValue)
            {
                return;
            }

            StyleTemplate st = StyleTemplateManager.GetStyleTemplate(styleTemplateId.Value);

            if (st == null)
            {
                return;
            }

            _reportStyle.Controls.Clear();
            _reportStyle.Controls.Add(new LiteralControl("<style type=\"text/css\">" + st.GetCss() + "</style>"));

            //Hardcode for english until ml style editing is funcitonal
            _reportView.ApplyHeaderAndFooter(
                TextManager.GetText(st.HeaderTextID, _params.LanguageCode, string.Empty, TextManager.DefaultLanguage),
                TextManager.GetText(st.FooterTextID, _params.LanguageCode, string.Empty, TextManager.DefaultLanguage)
            );
        }

        protected bool ShowPdfExportLink
        {
            get { return _params.ExportMode == ExportMode.None; }
        }
    }
}