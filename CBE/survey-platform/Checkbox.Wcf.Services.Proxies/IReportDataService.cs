using System;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Interface definition for getting report data
    /// </summary>
    [ServiceKnownType(typeof(ReportItemInstanceData))]
    [ServiceKnownType(typeof(SurveyResponseItem))]
    [ServiceContract]
    public interface IReportDataService
    {
        /// <summary>
        /// Get data for item in a report.  Depending on configuration settings, other items in the same report may be preloaded with data to enhance performance
        /// in cases where items are iteratively loaded, such as when running a report.
        /// </summary>
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="reportId"></param>
        /// <param name="itemId"></param>
        /// <param name="includeIncompleteResponses"></param>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<IItemProxyObject> GetItem(string authTicket, int reportId, int itemId, bool includeIncompleteResponses, string languageCode);

        /// <summary>
        /// Get data for an item in a report without preloading data for other items.  This should generally be used when only one report item is loaded
        /// and it is not likely other items from the same report will be loaded too.
        /// </summary>
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="reportId"></param>
        /// <param name="itemId"></param>
        /// <param name="includeIncompleteResponses"></param>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<IItemProxyObject> GetItemNoPreload(string authTicket, int reportId, int itemId, bool includeIncompleteResponses, string languageCode);

        /// <summary>
        /// Get summary data for a single survey item.
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="surveyId"></param>
        /// <param name="surveyItemId"></param>
        /// <param name="includeIncompleteResponses"></param>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<IItemProxyObject> GetResultsForSurveyItem(string authTicket, int surveyId, int surveyItemId, bool includeIncompleteResponses, string languageCode);
    }
}
