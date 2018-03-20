using System;
using System.ServiceModel.Activation;
using Checkbox.Wcf.Services;
using Checkbox.Wcf.Services.Proxies;
using Prezza.Framework.ExceptionHandling;

namespace CheckboxWeb.Services
{
    /// <summary>
    /// 
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class ReportMetaDataService : IReportMetaDataService
    {
        #region IReportMetaDataService Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="responseTemplateId"></param>
        /// <param name="resultsPerPage"></param>
        /// <param name="nameFilter"></param>
        /// <param name="pageNumber"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <returns></returns>
        public ServiceOperationResult<PagedListResult<ReportMetaData[]>> ListReportsForSurvey(string authTicket, int responseTemplateId, int pageNumber, int resultsPerPage, string nameFilter, string sortField, bool sortAscending)
        {
            try
            {
                return new ServiceOperationResult<PagedListResult<ReportMetaData[]>>
                {
                    CallSuccess = true,
                    ResultData = ReportMetaDataServiceImplementation.ListReportsForSurvey(AuthenticationService.GetCurrentPrincipal(authTicket), responseTemplateId, pageNumber, resultsPerPage, nameFilter, sortField, sortAscending)
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<PagedListResult<ReportMetaData[]>>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="period">0 - don't filter by period. 2 -- daily, 3 - weekly, 4 -- monthly created/modified entities</param>
        /// <param name="dateFieldName">Specifies what field of datetime type should be used when filtering by period. Null or empty -- don't filter by period.</param>
        /// <param name="pageNumber"></param>
        /// <param name="resultsPerPage"></param>
        /// <param name="nameFilter"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <returns></returns>
        public ServiceOperationResult<PagedListResult<ReportMetaData[]>> ListReportsByPeriod(string authTicket, int period, string dateFieldName, int pageNumber, int resultsPerPage, string nameFilter, string sortField, bool sortAscending)
        {
            try
            {
                return new ServiceOperationResult<PagedListResult<ReportMetaData[]>>
                {
                    CallSuccess = true,
                    ResultData = ReportMetaDataServiceImplementation.ListReportsByPeriod(AuthenticationService.GetCurrentPrincipal(authTicket), period, dateFieldName, pageNumber, resultsPerPage, nameFilter, sortField, sortAscending)
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<PagedListResult<ReportMetaData[]>>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="responseTemplateId"></param>
        /// <param name="resultsPerPage"></param>
        /// <param name="nameFilter"></param>
        /// <param name="pageNumber"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <returns></returns>
        public ServiceOperationResult<PagedListResult<ReportFilterData[]>> ListReportFiltersForSurvey(string authTicket, int responseTemplateId, int pageNumber, int resultsPerPage, string nameFilter, string sortField, bool sortAscending)
        {
            try
            {
                return new ServiceOperationResult<PagedListResult<ReportFilterData[]>>
                {
                    CallSuccess = true,
                    ResultData = ReportMetaDataServiceImplementation.ListReportFiltersForSurvey(AuthenticationService.GetCurrentPrincipal(authTicket), responseTemplateId, pageNumber, resultsPerPage, nameFilter, sortField, sortAscending)
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<PagedListResult<ReportFilterData[]>>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Delete the specified filters
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="surveyId"></param>
        /// <param name="filterIds"></param>
        /// <returns></returns>
        public ServiceOperationResult<bool> DeleteFilters(string authTicket, int surveyId, int[] filterIds)
        {
            try
            {
                return new ServiceOperationResult<bool>
                {
                    CallSuccess = true,
                    ResultData = ReportMetaDataServiceImplementation.DeleteFilters(AuthenticationService.GetCurrentPrincipal(authTicket), surveyId, filterIds)
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<bool>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="responseTemplateId"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public ServiceOperationResult<ReportMetaData[]> ListRecentReportsForSurvey(string authTicket, int responseTemplateId, int count)
        {
            try
            {
                return new ServiceOperationResult<ReportMetaData[]>
                {
                    CallSuccess = true,
                    ResultData = ReportMetaDataServiceImplementation.ListRecentReportsForSurvey(AuthenticationService.GetCurrentPrincipal(authTicket), responseTemplateId, count)
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<ReportMetaData[]>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="responseTemplateId"></param>
        /// <returns></returns>
        public ServiceOperationResult<int> GetReportCountForSurvey(string authTicket, int responseTemplateId)
        {
            try
            {
                return new ServiceOperationResult<int>
                {
                    CallSuccess = true,
                    ResultData = ReportMetaDataServiceImplementation.GetReportCountForSurvey(AuthenticationService.GetCurrentPrincipal(authTicket), responseTemplateId)
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<int>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="reportId"></param>
        /// <returns></returns>
        public ServiceOperationResult<ReportMetaData> GetReportWithId(string authTicket, int reportId)
        {
             try
            {
                return new ServiceOperationResult<ReportMetaData>
                {
                    CallSuccess = true,
                    ResultData = ReportMetaDataServiceImplementation.GetReportWithId(AuthenticationService.GetCurrentPrincipal(authTicket), reportId)
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<ReportMetaData>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="reportGuid"></param>
        /// <returns></returns>
        public ServiceOperationResult<ReportMetaData> GetReportWithGuid(string authTicket, Guid reportGuid)
        {
            try
            {
                return new ServiceOperationResult<ReportMetaData>
                {
                    CallSuccess = true,
                    ResultData = ReportMetaDataServiceImplementation.GetReportWithGuid(AuthenticationService.GetCurrentPrincipal(authTicket), reportGuid)
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<ReportMetaData>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="responseTemplateId"></param>
        /// <param name="reportName"></param>
        /// <returns></returns>
        public ServiceOperationResult<ReportMetaData> GetReportWithName(string authTicket, int responseTemplateId, string reportName)
        {
             try
            {
                return new ServiceOperationResult<ReportMetaData>
                {
                    CallSuccess = true,
                    ResultData = ReportMetaDataServiceImplementation.GetReportWithName(AuthenticationService.GetCurrentPrincipal(authTicket), responseTemplateId, reportName)
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<ReportMetaData>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="reportId"></param>
        /// <param name="templatePageId"></param>
        /// <returns></returns>
        public ServiceOperationResult<ReportPageMetaData> GetReportPageWithId(string authTicket, int reportId, int templatePageId)
        {
             try
            {
                return new ServiceOperationResult<ReportPageMetaData>
                {
                    CallSuccess = true,
                    ResultData = ReportMetaDataServiceImplementation.GetReportPageWithId(AuthenticationService.GetCurrentPrincipal(authTicket), reportId, templatePageId)
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<ReportPageMetaData>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authTicket">Authentication ticket identifying calling user.</param>
        /// <param name="reportId"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public ServiceOperationResult<IItemMetadata> GetItemMetaData(string authTicket, int reportId, int itemId)
        {
             try
            {
                return new ServiceOperationResult<IItemMetadata>
                {
                    CallSuccess = true,
                    ResultData = ReportMetaDataServiceImplementation.GetItemMetaData(AuthenticationService.GetCurrentPrincipal(authTicket), reportId, itemId)
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<IItemMetadata>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        public ServiceOperationResult<IItemMetadata[]> ListPageItemsData(string authTicket, int reportId, int pageId)
        {
            try
            {
                return new ServiceOperationResult<IItemMetadata[]>
                {
                    CallSuccess = true,
                    ResultData = ReportMetaDataServiceImplementation.ListPageItemsData(AuthenticationService.GetCurrentPrincipal(authTicket), reportId, pageId)
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<IItemMetadata[]>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="searchTerm"></param>
        /// <returns></returns>
        public ServiceOperationResult<GroupedResult<ReportMetaData>[]> Search(string authTicket, string searchTerm)
        {
            try
            {
                return new ServiceOperationResult<GroupedResult<ReportMetaData>[]>
                {
                    CallSuccess = true,
                    ResultData = ReportMetaDataServiceImplementation.Search(AuthenticationService.GetCurrentPrincipal(authTicket), searchTerm)
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<GroupedResult<ReportMetaData>[]>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        #endregion

        /// <summary>
        /// Deletes a report by it's ID
        /// </summary>
        /// <param name="authTicket">Authentication Token</param>
        /// <param name="reportID">Report ID</param>
        /// <returns></returns>
        public ServiceOperationResult<bool> DeleteReport(string authTicket, int reportID)
        {
            try
            {
                ReportMetaDataServiceImplementation.DeleteReport(AuthenticationService.GetCurrentPrincipal(authTicket), reportID);

                return new ServiceOperationResult<bool>
                {
                    CallSuccess = true,
                    ResultData = true
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<bool>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<bool>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Copies a report under the given name
        /// </summary>
        /// <param name="authToken">Authentication Token</param>
        /// <param name="reportID">Report ID</param>
        /// <param name="newReportName">New Report Name</param>
        /// <returns></returns>
        public ServiceOperationResult<int> CopyReport(string authToken, int reportID, string newReportName)
        {
            try
            {
                return new ServiceOperationResult<int>
                {
                    CallSuccess = true,
                    ResultData = ReportMetaDataServiceImplementation.CopyReport(AuthenticationService.GetCurrentPrincipal(authToken), reportID, newReportName)
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<int>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<int>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="reportId"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public ServiceOperationResult<object> DeleteItem(string authTicket, int reportId, int itemId)
        {
            try
            {
                ReportMetaDataServiceImplementation.DeleteItem(AuthenticationService.GetCurrentPrincipal(authTicket), reportId, itemId);

                return new ServiceOperationResult<object>
                {
                    CallSuccess = true
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<object>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<object>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="reportId"></param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        public ServiceOperationResult<object> DeletePage(string authTicket, int reportId, int pageId)
        {
            try
            {
                ReportMetaDataServiceImplementation.DeletePage(AuthenticationService.GetCurrentPrincipal(authTicket), reportId, pageId);

                return new ServiceOperationResult<object>
                {
                    CallSuccess = true
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<object>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<object>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="reportId"></param>
        /// <param name="itemId"></param>
        /// <param name="newPageId"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public ServiceOperationResult<object> MoveReportItem(string authTicket, int reportId, int itemId, int? newPageId, int position)
        {
            try
            {
                ReportMetaDataServiceImplementation.MoveReportItem(AuthenticationService.GetCurrentPrincipal(authTicket), reportId, itemId, newPageId, position);

                return new ServiceOperationResult<object>
                {
                    CallSuccess = true
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<object>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<object>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="reportId"></param>
        /// <param name="pageId"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public ServiceOperationResult<object> MoveReportPage(string authTicket, int reportId, int pageId, int position)
        {
            try
            {
                ReportMetaDataServiceImplementation.MoveReportPage(AuthenticationService.GetCurrentPrincipal(authTicket), reportId, pageId, position);

                return new ServiceOperationResult<object>
                {
                    CallSuccess = true
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<object>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<object>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="reportId"></param>
        /// <returns></returns>
        public ServiceOperationResult<object> AddReportPage(string authTicket, int reportId)
        {
            try
            {
                ReportMetaDataServiceImplementation.AddReportPage(AuthenticationService.GetCurrentPrincipal(authTicket), reportId);

                return new ServiceOperationResult<object>
                {
                    CallSuccess = true
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<object>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<object>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="reportId"></param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        public ServiceOperationResult<PagedListResult<ReportMetaData[]>> ListAvailableReports(string authTicket, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue)
        {
            try
            {
                PagedListResult<ReportMetaData[]> res = ReportMetaDataServiceImplementation.ListAvailableReports(AuthenticationService.GetCurrentPrincipal(authTicket), pageNumber, pageSize, sortField, sortAscending, filterField, filterValue);

                return new ServiceOperationResult<PagedListResult<ReportMetaData[]>>
                {
                    CallSuccess = true,
                    ResultData = res
                };
            }
            catch (NoUserException ex)
            {
                return new ServiceOperationResult<PagedListResult<ReportMetaData[]>>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = AuthenticationService.NoUserMessage
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<PagedListResult<ReportMetaData[]>>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

    }
}
