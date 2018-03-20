using System;
using System.Web;
using System.Web.Security;
using Checkbox.Security.Principal;
using Checkbox.Users;
using Checkbox.Wcf.Services.Proxies;
using Prezza.Framework.ExceptionHandling;

namespace Checkbox.Wcf.Services.LocalProxies
{
    /// <summary>
    /// 
    /// </summary>
    public class LocalReportMetaDataServiceProxy : IReportMetaDataService
    {
        /// <summary>
        /// Get guid associated with current user.
        /// </summary>
        /// <returns></returns>
        private CheckboxPrincipal GetCurrentPrincipal(string encryptedTicket)
        {
            //Use ticket if provided
            if (!string.IsNullOrEmpty(encryptedTicket))
            {
                var decryptedTicket = FormsAuthentication.Decrypt(encryptedTicket);

                //Check user data for user guid
                if (string.IsNullOrEmpty(decryptedTicket.UserData))
                {
                    return null;
                }

                Guid userGuid;

                if (!Guid.TryParse(decryptedTicket.UserData, out userGuid))
                {
                    return null;
                }

                return UserManager.GetUserByGuid(userGuid);
            }

            //Fallback to current user
            if (HttpContext.Current != null)
            {
                return HttpContext.Current.User as CheckboxPrincipal;
            }

            return null;
        }


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
                    ResultData = ReportMetaDataServiceImplementation.ListReportsForSurvey(GetCurrentPrincipal(authTicket), responseTemplateId, pageNumber, resultsPerPage, nameFilter, sortField, sortAscending)
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
                    ResultData = ReportMetaDataServiceImplementation.ListReportsByPeriod(GetCurrentPrincipal(authTicket), period, dateFieldName, pageNumber, resultsPerPage, nameFilter, sortField, sortAscending)
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
                    ResultData = ReportMetaDataServiceImplementation.DeleteFilters(GetCurrentPrincipal(authTicket), surveyId, filterIds)
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
                    ResultData = ReportMetaDataServiceImplementation.ListReportFiltersForSurvey(GetCurrentPrincipal(authTicket), responseTemplateId, pageNumber, resultsPerPage, nameFilter, sortField, sortAscending)
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
                    ResultData = ReportMetaDataServiceImplementation.GetReportCountForSurvey(GetCurrentPrincipal(authTicket), responseTemplateId)
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
                    ResultData = ReportMetaDataServiceImplementation.ListRecentReportsForSurvey(GetCurrentPrincipal(authTicket), responseTemplateId, count)
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
        /// <param name="reportId"></param>
        /// <returns></returns>
        public ServiceOperationResult<ReportMetaData> GetReportWithId(string authTicket, int reportId)
        {
            try
            {
                return new ServiceOperationResult<ReportMetaData>
                {
                    CallSuccess = true,
                    ResultData = ReportMetaDataServiceImplementation.GetReportWithId(GetCurrentPrincipal(authTicket), reportId)
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
                    ResultData = ReportMetaDataServiceImplementation.GetReportWithGuid(GetCurrentPrincipal(authTicket), reportGuid)
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
                    ResultData = ReportMetaDataServiceImplementation.GetReportWithName(GetCurrentPrincipal(authTicket), responseTemplateId, reportName)
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
                    ResultData = ReportMetaDataServiceImplementation.GetReportPageWithId(GetCurrentPrincipal(authTicket), reportId, templatePageId)
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
                    ResultData = ReportMetaDataServiceImplementation.GetItemMetaData(GetCurrentPrincipal(authTicket), reportId, itemId)
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="reportId"></param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        public ServiceOperationResult<IItemMetadata[]> ListPageItemsData(string authTicket, int reportId, int pageId)
        {
            try
            {
                return new ServiceOperationResult<IItemMetadata[]>
                {
                    CallSuccess = true,
                    ResultData = ReportMetaDataServiceImplementation.ListPageItemsData(GetCurrentPrincipal(authTicket), reportId, pageId)
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
                    ResultData = ReportMetaDataServiceImplementation.Search(GetCurrentPrincipal(authTicket), searchTerm)
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
                ReportMetaDataServiceImplementation.DeleteReport(GetCurrentPrincipal(authTicket), reportID);

                return new ServiceOperationResult<bool>
                {
                    CallSuccess = true,
                    ResultData = true
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
                    ResultData = ReportMetaDataServiceImplementation.CopyReport(GetCurrentPrincipal(authToken), reportID, newReportName)
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
                ReportMetaDataServiceImplementation.DeleteItem(GetCurrentPrincipal(authTicket), reportId, itemId);

                return new ServiceOperationResult<object>
                {
                    CallSuccess = true
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
                ReportMetaDataServiceImplementation.DeletePage(GetCurrentPrincipal(authTicket), reportId, pageId);

                return new ServiceOperationResult<object>
                {
                    CallSuccess = true
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
                ReportMetaDataServiceImplementation.MoveReportItem(GetCurrentPrincipal(authTicket), reportId, itemId, newPageId, position);

                return new ServiceOperationResult<object>
                {
                    CallSuccess = true
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
                ReportMetaDataServiceImplementation.MoveReportPage(GetCurrentPrincipal(authTicket), reportId, pageId, position);

                return new ServiceOperationResult<object>
                {
                    CallSuccess = true
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
                ReportMetaDataServiceImplementation.AddReportPage(GetCurrentPrincipal(authTicket), reportId);

                return new ServiceOperationResult<object>
                {
                    CallSuccess = true
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
                return new ServiceOperationResult<PagedListResult<ReportMetaData[]>>
                {
                    CallSuccess = true,
                    ResultData = ReportMetaDataServiceImplementation.ListAvailableReports(GetCurrentPrincipal(authTicket), pageNumber, pageSize, sortField, sortAscending, filterField, filterValue)
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
