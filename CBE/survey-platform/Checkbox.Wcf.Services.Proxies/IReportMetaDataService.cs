using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Interface definition for data service allowing fetching of listing survey reports and getting basic information about the
    /// report. 
    /// </summary>
    [ServiceKnownType(typeof(SurveyItemMetaData))]
    [ServiceKnownType(typeof(ItemMetaData))]
    [ServiceKnownType(typeof(ReportItemMetaData))]
    [ServiceContract]
    public interface IReportMetaDataService
    {
        /// <summary>
        /// Get a list of report data objects for a given survey.
        /// </summary>
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="responseTemplateId">Id of response template to list reports for.</param>
        /// <param name="resultsPerPage">Number of results to be listed per page.</param>
        /// <param name="nameFilter">Name value to search for.  Pass string.Empty for no filtering.</param>
        /// <param name="pageNumber">Number of the page of report in which to get objects from.</param>
        /// <param name="sortField">Field in which to sort the results.</param>
        /// <param name="sortAscending">Order in which to sort results. Pass true to sort results in ascending order.</param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<PagedListResult<ReportMetaData[]>> ListReportsForSurvey(
            string authTicket,
            int responseTemplateId,
            int pageNumber, 
            int resultsPerPage, 
            string nameFilter, 
            string sortField, 
            bool sortAscending);

        /// <summary>
        /// Get a list of report data objects that were created/modified during the period.
        /// </summary>
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="period">Period: 2 -- last day, 3 -- last week, 4 -- last month</param>
        /// <param name="dateFieldName">Date field criteria (CreatedDate or ModifiedDate)</param>
        /// <param name="resultsPerPage">Number of results to be listed per page.</param>
        /// <param name="nameFilter">Name value to search for.  Pass string.Empty for no filtering.</param>
        /// <param name="pageNumber">Number of the page of report in which to get objects from.</param>
        /// <param name="sortField">Field in which to sort the results.</param>
        /// <param name="sortAscending">Order in which to sort results. Pass true to sort results in ascending order.</param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<PagedListResult<ReportMetaData[]>> ListReportsByPeriod(
            string authTicket,
            int period,
            string dateFieldName,
            int pageNumber,
            int resultsPerPage,
            string nameFilter,
            string sortField,
            bool sortAscending);

        /// <summary>
        /// Get a list of report filter data objects for a given survey.
        /// </summary>
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="responseTemplateId">Id of response template to list reports for.</param>
        /// <param name="resultsPerPage">Number of results to be returned per page.</param>
        /// <param name="nameFilter">Name value to search for.  Pass string.Empty for no filtering.</param>
        /// <param name="pageNumber">Number of the page of report in which to get objects from.</param>
        /// <param name="sortField">Field in which to sort the results.</param>
        /// <param name="sortAscending">Order in which to sort results. Pass true to sort results in ascending order.</param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<PagedListResult<ReportFilterData[]>> ListReportFiltersForSurvey(
            string authTicket,
            int responseTemplateId,
            int pageNumber,
            int resultsPerPage,
            string nameFilter,
            string sortField,
            bool sortAscending);

        /// <summary>
        /// Delete the specified report filters.
        /// </summary>
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="surveyId">ID of the survey in which to delete filters from. </param>
        /// <param name="Ids">List of report filter Ids to delete.</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<bool> DeleteFilters(string authTicket, int surveyId, int[] Ids);

        /// <summary>
        /// Get a list of recently created reports for a given survey.
        /// </summary>
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="responseTemplateId">Id of response template to list reports for.</param>
        /// <param name="count">Number of items to return.</param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<ReportMetaData[]> ListRecentReportsForSurvey(
            string authTicket,
            int responseTemplateId,
            int count);

        /// <summary>
        /// Get count of reports associated with a survey.
        /// </summary>
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="responseTemplateId">Id of response template.</param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<int> GetReportCountForSurvey(string authTicket, int responseTemplateId);

        /// <summary>
        /// Get data for the report with the specified id
        /// </summary>
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="reportId">ID of report.</param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<ReportMetaData> GetReportWithId(string authTicket, int reportId);

        /// <summary>
        /// Get data for the report with the specified (unlocalized) name.
        /// </summary>
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="responseTemplateId">Id of survey to get named report for.</param>
        /// <param name="reportName">Name of report to load.</param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<ReportMetaData> GetReportWithName(string authTicket, int responseTemplateId, string reportName);

        /// <summary>
        /// Get data for the report with the specified guid.
        /// </summary>
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="reportGuid">GUID of report to load.</param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<ReportMetaData> GetReportWithGuid(string authTicket, Guid reportGuid);

        /// <summary>
        /// Get data for a report page with the specified Id
        /// </summary>
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="reportId"></param>
        /// <param name="templatePageId"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<ReportPageMetaData> GetReportPageWithId(string authTicket, int reportId, int templatePageId);

        /// <summary>
        /// Get configuration data for the item with the specified id
        /// </summary>
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="reportId"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<IItemMetadata> GetItemMetaData(string authTicket, int reportId, int itemId);

        /// <summary>
        /// Get configuration data for the items of the page with the specified id
        /// </summary>
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="reportId"></param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<IItemMetadata[]> ListPageItemsData(string authTicket, int reportId, int pageId);

        /// <summary>
        /// Search for reports matching the specific criteria
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="searchTerm"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<GroupedResult<ReportMetaData>[]> Search(string authTicket, string searchTerm);


        /// <summary>
        /// Deletes a report by it's ID
        /// </summary>
        /// <param name="authTicket">Authentication Token</param>
        /// <param name="reportID">Report ID</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<bool> DeleteReport(string authTicket, int reportID);

        /// <summary>
        /// Copies a report under the given name
        /// </summary>
        /// <param name="authTicket">Authentication Token</param>
        /// <param name="reportID">Report ID</param>
        /// <param name="newReportName">New Report Name</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<int> CopyReport(string authTicket, int reportID, string newReportName);

        /// <summary>
        ///  Remove an item from a report.
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="reportId"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> DeleteItem(string authTicket, int reportId, int itemId);

        /// <summary>
        ///  Remove a page from a report.
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="reportId"></param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> DeletePage(string authTicket, int reportId, int pageId);

        ///<summary>
        /// Move report item
        ///</summary>
        ///<param name="authTicket"></param>
        ///<param name="newPageId"></param>
        ///<param name="position"></param>
        ///<param name="reportId"></param>
        ///<param name="itemId"></param>
        ///<returns></returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> MoveReportItem(string authTicket, int reportId, int itemId, int? newPageId, int position);

        ///<summary>
        /// Move report page
        ///</summary>
        ///<param name="authTicket"></param>
        ///<param name="pageId"></param>
        ///<param name="position"></param>
        ///<param name="reportId"></param>
        ///<returns></returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> MoveReportPage(string authTicket, int reportId, int pageId, int position);

        ///<summary>
        /// Add new report page
        ///</summary>
        ///<param name="authTicket"></param>
        ///<param name="reportId"></param>
        ///<returns></returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> AddReportPage(string authTicket, int reportId);

        //TODO: Filter Data

        /// <summary>
        /// List all available reports for the user
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="filterField"></param>
        /// <param name="filterValue"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<PagedListResult<ReportMetaData[]>> ListAvailableReports(string authTicket, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue);


    }
}
