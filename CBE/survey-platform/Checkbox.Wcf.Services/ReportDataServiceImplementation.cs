using System;
using Checkbox.Analytics;
using Checkbox.Analytics.Items;
using Checkbox.Analytics.Items.Configuration;
using Checkbox.Forms;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Security.Principal;
using Checkbox.Wcf.Services.Proxies;

namespace Checkbox.Wcf.Services
{
    /// <summary>
    /// Non-service specific implementation methods for getting report data
    /// </summary>
    public static class ReportDataServiceImplementation
    {
        #region Public Methods

        /// <summary>
        /// Get data for an item in a report.
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="reportId"></param>
        /// <param name="itemId"></param>
        /// <param name="languageCode"></param>
        /// <param name="includeIncomplete"></param>
        /// <param name="loadSingleItemOnly">By default, when one item in a report is loaded, all are loaded to prime item result cache
        /// and to prevent contstant loading of results from database for each item access.  (Only computed results and not 
        /// raw response data is cached for scalability reasons.</param>
        /// <returns></returns>
        public static IItemProxyObject GetItem(CheckboxPrincipal userPrincipal, int reportId, int itemId, string languageCode, bool includeIncomplete, bool loadSingleItemOnly)
        {
            return GetItemWorker(userPrincipal, reportId, itemId, languageCode, includeIncomplete, loadSingleItemOnly);
        }

        /// <summary>
        /// Get data for an item in a report.
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="reportId"></param>
        /// <param name="itemId"></param>
        /// <param name="languageCode"></param>
        /// <param name="includeIncomplete"></param>
        /// <param name="loadSingleItemOnly">By default, when one item in a report is loaded, all are loaded to prime item result cache
        /// and to prevent contstant loading of results from database for each item access.  (Only computed results and not 
        /// raw response data is cached for scalability reasons.</param>
        /// <param name="progressKey"> </param>
        /// <returns></returns>
        private static IItemProxyObject GetItemWorker(CheckboxPrincipal userPrincipal, int reportId, int itemId, string languageCode, bool includeIncomplete, bool loadSingleItemOnly)
        {
            //Ensure access.  Only run access required to view report details.  Also allow success on edit
            // permission.
            var report = GetAnalysis(userPrincipal, reportId, includeIncomplete, languageCode);

            //Ensure analysis contains item
            var item = report.GetItem(itemId);

            if (item == null)
            {
                throw new Exception("Item with id [" + itemId + "] not found in report with id: " + reportId);
            }

            //If preloading, cause items to load data
            if (!loadSingleItemOnly)
            {
                report.Items.ForEach(childItem => childItem.GetDataTransferObject());
            }

            //return data
            return item.GetDataTransferObject();
        }

        /// <summary>
        /// Get report results for a single report item
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="surveyId"></param>
        /// <param name="itemId"></param>
        /// <param name="includeIncomplete"></param>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public static IItemProxyObject GetResultsForSurveyItem(CheckboxPrincipal userPrincipal, int surveyId, int itemId, bool includeIncomplete, string languageCode)
        {
            if (surveyId <= 0)
            {
                throw new Exception("Survey id not specified.");
            }

            if (itemId <= 0)
            {
                throw new Exception("Survey item id not specified.");
            }

            var responseTemplate = ResponseTemplateManager.GetResponseTemplate(surveyId);

            if (responseTemplate == null)
            {
                throw new Exception("Unable to load response template with id [" + surveyId + "].");
            }
            
            //Authorize
            Security.AuthorizeUserContext(userPrincipal, ResponseTemplateManager.GetResponseTemplate(surveyId), "Analysis.Run");

            var frequencyItemData = ItemConfigurationManager.CreateConfigurationData("SummaryChart") as AnalysisItemData;

            if (frequencyItemData == null)
            {
                throw new Exception("Unable to create summary item data.");
            }

            frequencyItemData.AddResponseTemplate(surveyId);
            frequencyItemData.AddSourceItem(itemId);

            var frequencyItem = frequencyItemData.CreateItem(languageCode, null) as AnalysisItem;
            frequencyItem.RunMode = true;
            

            return frequencyItem.GetDataTransferObject();
        }


        #endregion

        #region Utility

        /// <summary>
        /// Check access and return loaded analysis
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="reportId"></param>
        /// <param name="includeIncomplete"></param>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        private static Analysis GetAnalysis(CheckboxPrincipal userPrincipal, int reportId, bool includeIncomplete, string languageCode)
        {
            //Get analysis template
            var analysisTemplate = AnalysisTemplateManager.GetAnalysisTemplate(reportId, true);

            if (analysisTemplate == null)
            {
                throw new Exception("Unable to load analysis template with id: " + reportId);
            }

            //Check access
            Security.AuthorizeUserContext(userPrincipal, analysisTemplate, "Analysis.Run");
            
            //Generate analysis
            return analysisTemplate.CreateAnalysis(languageCode, includeIncomplete, analysisTemplate.IncludeTestResponses);
        }

        #endregion
    }
}
