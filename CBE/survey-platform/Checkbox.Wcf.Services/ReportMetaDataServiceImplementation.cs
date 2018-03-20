using System;
using System.Collections.Generic;
using System.Linq;
using Checkbox.Analytics;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Globalization.Text;
using Checkbox.Pagination;
using Checkbox.Security;
using Checkbox.Security.Principal;
using Checkbox.Wcf.Services.Proxies;
using Checkbox.Analytics.Filters.Configuration;
using Checkbox.Web;
using Checkbox.Timeline;

namespace Checkbox.Wcf.Services
{
    /// <summary>
    /// Non-service level specific implementation of survey report meta data operations.
    /// </summary>
    public static class ReportMetaDataServiceImplementation
    {
        #region Public Methods 

        /// <summary>
        /// List report filters associated with a survey
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="responseTemplateId"></param>
        /// <param name="resultsPerPage"></param>
        /// <param name="nameFilter"></param>
        /// <param name="pageNumber"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <returns></returns>
        public static PagedListResult<ReportMetaData[]> ListReportsForSurvey(
            CheckboxPrincipal userPrincipal,
            int responseTemplateId,
            int pageNumber,
            int resultsPerPage,
            string nameFilter,
            string sortField,
            bool sortAscending)
        {
            Security.AuthorizeUserContext(userPrincipal, "Analysis.Run");

            var paginationContext = CreatePaginationContext("AnalysisName", nameFilter, sortField, sortAscending,
                pageNumber, resultsPerPage);
            paginationContext.Permissions = new List<string>(new[] { "Analysis.Run" });
            paginationContext.PermissionJoin = PermissionJoin.Any;

            var templateList = AnalysisTemplateManager.ListAnalysisTemplatesForSurvey(
                userPrincipal,
                responseTemplateId,
                paginationContext
               );

            return new PagedListResult<ReportMetaData[]>
            {
                TotalItemCount = paginationContext.ItemCount,
                ResultPage = (from t in templateList select BuildReportMetaData(t, userPrincipal)).ToArray()
            };
        }

        /// <summary>
        /// List recently changed(created) reports
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="responseTemplateId"></param>
        /// <param name="resultsPerPage"></param>
        /// <param name="nameFilter"></param>
        /// <param name="pageNumber"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <returns></returns>
        public static PagedListResult<ReportMetaData[]> ListReportsByPeriod(
            CheckboxPrincipal userPrincipal,
            int period, 
            string dateFieldName,
            int pageNumber,
            int resultsPerPage,
            string nameFilter,
            string sortField,
            bool sortAscending)
        {
            Security.AuthorizeUserContext(userPrincipal, "Analysis.Run");

            var paginationContext = new PaginationContext
            {
                SortField = sortField,
                SortAscending = sortAscending,
                PageSize = resultsPerPage,
                CurrentPage = pageNumber,
                FilterField = "AnalysisName",
                FilterValue = nameFilter,
                StartDate = TimelineManager.GetStartFilterDate(period),
                DateFieldName = TimelineManager.ProtectFieldNameFromSQLInjections(dateFieldName),
                PermissionJoin = PermissionJoin.Any,
                Permissions = new List<string>(new[] { "Analysis.Run" })
            };

            var templateList = AnalysisTemplateManager.ListReportsByPeriod(
                userPrincipal,
                paginationContext
               );

            return new PagedListResult<ReportMetaData[]>
            {
                TotalItemCount = paginationContext.ItemCount,
                ResultPage = (from t in templateList select BuildReportMetaData(t, userPrincipal)).ToArray()
            };
        }


        /// <summary>
        /// List reports associated with a survey
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="responseTemplateId"></param>
        /// <param name="resultsPerPage"></param>
        /// <param name="nameFilter"></param>
        /// <param name="pageNumber"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <returns></returns>
        public static PagedListResult<ReportFilterData[]> ListReportFiltersForSurvey(
            CheckboxPrincipal userPrincipal,
            int responseTemplateId,
            int pageNumber,
            int resultsPerPage,
            string nameFilter,
            string sortField,
            bool sortAscending)
        {
            Security.AuthorizeUserContext(userPrincipal, "Analysis.Run");

            ResponseTemplate survey = ResponseTemplateManager.GetResponseTemplate(responseTemplateId);            
            List<FilterData> filters = survey.GetFilterDataObjects();
            var languageCode = survey.LanguageSettings.DefaultLanguage;

            var filterList =
                filters.Select(
                    filterData =>
                    new ReportFilterData
                        {
                            Comparison = filterData.Operator.ToString(),
                            FilterId = filterData.ID.Value,
                            FilterText = filterData.ToString(languageCode),
                            SourceType = filterData.FilterTypeName,
                            ValueAsString = filterData.Value != null ? filterData.Value.ToString() : null,
                            ShortText = Utilities.StripHtml(filterData.ToString(languageCode), 60)
                        });

            var totalFilterCount = filterList.Count();

            filterList = pageNumber > 0 && resultsPerPage > 0
                                ? filterList.Skip((pageNumber - 1) * resultsPerPage).Take(resultsPerPage)
                                : filterList;

            switch (sortField)
            {
                case "Type":
                    filterList = sortAscending
                                     ? filterList.OrderBy(f => f.SourceType)
                                     : filterList.OrderByDescending(f => f.SourceType);
                    break;
                case "Filter":
                    filterList = sortAscending
                                     ? filterList.OrderBy(f => f.FilterText)
                                     : filterList.OrderByDescending(f => f.FilterText);
                    break;
                default: //sort by ID
                    filterList = sortAscending
                                     ? filterList.OrderBy(f => f.FilterId)
                                     : filterList.OrderByDescending(f => f.FilterId);
                    break;
            }

 
            return new PagedListResult<ReportFilterData[]>
            {
                TotalItemCount = totalFilterCount,
                ResultPage = filterList.ToArray()
            };
        }

        /// <summary>
        /// Delete specified filters
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="surveyId"></param>
        /// <param name="filterIds"></param>
        public static bool DeleteFilters(CheckboxPrincipal userPrincipal, int surveyId, int[] filterIds)
        {
            try
            {
                Security.AuthorizeUserContext(userPrincipal, "Filter.Delete");

                ResponseTemplate survey = ResponseTemplateManager.GetResponseTemplate(surveyId);

                List<FilterData> filters = survey.GetFilterDataObjects();
                List<FilterData> filtersForDelete = (from f in filters where filterIds.Contains(f.ID.Value) select f).ToList();
                foreach (FilterData f in filtersForDelete)
                {
                    survey.DeleteFilter(f); 
                }

                //remove analysis templates from cache if exist
                AnalysisTemplateManager.CleanupAnalysisTemplatesCacheForSurvey(userPrincipal, surveyId);
                

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Get count of reports associated with a survey
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="responseTemplateId"></param>
        /// <returns></returns>
        public static int GetReportCountForSurvey(CheckboxPrincipal userPrincipal, int responseTemplateId)
        {
            Security.AuthorizeUserContext(userPrincipal, "Analysis.Run");
            var permissions = new List<string>(new[] {"Analysis.Run"});

            return AnalysisTemplateManager.GetAnalysisCountForSurvey(userPrincipal, responseTemplateId, permissions,
                                                                     PermissionJoin.Any);
        }

        /// <summary>
        /// List reports associated with a survey
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="responseTemplateId"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static ReportMetaData[] ListRecentReportsForSurvey(CheckboxPrincipal userPrincipal, int responseTemplateId, int count)
        {
            Security.AuthorizeUserContext(userPrincipal, "Analysis.Run");

            var templateList = AnalysisTemplateManager.ListAnalysisTemplatesForSurvey(
                userPrincipal,
                responseTemplateId,
                new PaginationContext
                    {
                        SortField = "CreatedDate",
                        SortAscending = false,
                        CurrentPage = 1,
                        PageSize = count
                    });

            return (from t in templateList select BuildReportMetaData(t, userPrincipal)).ToArray();
        }

        /// <summary>
        /// Get data for a report with the specified id
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="reportId"></param>
        /// <returns></returns>
        public static ReportMetaData GetReportWithId(CheckboxPrincipal userPrincipal, int reportId)
        {
            //Ensure access.  Only run access required to view report details.  Also allow success on edit
            // permission.
            AuthorizeAccess(userPrincipal, reportId, "Analysis.Run", "Analysis.Edit");

            return BuildReportMetaData(AnalysisTemplateManager.GetAnalysisTemplate(reportId, true));
        }

        /// <summary>
        /// Get data for a report with the specified id
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="reportGuid"></param>
        /// <returns></returns>
        public static ReportMetaData GetReportWithGuid(CheckboxPrincipal userPrincipal, Guid reportGuid)
        {
            //Ensure access.  Only run access required to view report details.  Also allow success on edit
            // permission.
            var analysisTemplate = AnalysisTemplateManager.GetAnalysisTemplate(reportGuid);

            if (analysisTemplate == null)
            {
                throw new Exception("Unable to load report with GUID.");
            }

            AuthorizeAccess(userPrincipal, analysisTemplate.ID.Value, "Analysis.Run", "Analysis.Edit");

            return BuildReportMetaData(analysisTemplate);
        }


        /// <summary>
        /// Get data for a report with the specified name
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="responseTemplateId"></param>
        /// <param name="reportName"></param>
        /// <returns></returns>
        public static ReportMetaData GetReportWithName(CheckboxPrincipal userPrincipal, int responseTemplateId, string reportName)
        {
            //Get id of template
            var reportId = AnalysisTemplateManager.GetAnalysisTemplateId(responseTemplateId, reportName);

            if (reportId == null)
            {
                throw new Exception("Unable to determine id of report with name: " + reportName);
            }

            //Return template
            return GetReportWithId(userPrincipal, reportId.Value);
        }

        /// <summary>
        /// Get data for a report page with the specified id
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="reportId"></param>
        /// <param name="templatePageId"></param>
        /// <returns></returns>
        public static ReportPageMetaData GetReportPageWithId(CheckboxPrincipal userPrincipal, int reportId, int templatePageId)
        {
            //Check access
            AuthorizeAccess(userPrincipal, reportId, "Analysis.Run", "Analysis.Edit");

            //Get template page
            var template = AnalysisTemplateManager.GetAnalysisTemplate(reportId, true);

            if (template == null)
            {
                throw new Exception("Unable to load report with id: " + reportId);
            }

            return BuildReportPageMetaData(template.GetPage(templatePageId));
        }

        /// <summary>
        /// Get data for an item in a report with the specified id
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="reportId"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public static IItemMetadata GetItemMetaData(CheckboxPrincipal userPrincipal, int reportId, int itemId)
        {
            //Check access
            AuthorizeAccess(userPrincipal, reportId, "Analysis.Run", "Analysis.Edit");

            //Ensure item is in report
            //Get template page
            var template = AnalysisTemplateManager.GetAnalysisTemplate(reportId);

            if (template == null)
            {
                throw new Exception("Unable to load report with id: " + reportId);
            }

            if (!template.ListTemplateItemIds().Contains(itemId))
            {
                throw new Exception("Item with id [" + itemId + "] not found in report with id: "  + reportId);
            }

            //Get data
            var itemData = ItemConfigurationManager.GetConfigurationData(itemId);

            if (itemData == null)
            {
                throw new Exception("Unable to load item with id: " + itemId);
            }

            //Return data
            return itemData.GetDataTransferObject(template);
        }

        /// <summary>
        /// Get data for items of the specified id in a report 
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="reportId"></param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        public static IItemMetadata[] ListPageItemsData(CheckboxPrincipal userPrincipal, int reportId, int pageId)
        {
            //Check access
            AuthorizeAccess(userPrincipal, reportId, "Analysis.Run", "Analysis.Edit");

            //Ensure item is in report
            //Get template page
            var template = AnalysisTemplateManager.GetAnalysisTemplate(reportId);

            if (template == null)
            {
                throw new Exception("Unable to load report with id: " + reportId);
            }
            
            if (!template.ListTemplatePageIds().Contains(pageId))
            {
                throw new Exception("Page with id [" + pageId + "] not found in report with id: " + reportId);
            }

            //Get data
            var result = new List<IItemMetadata>();

            var page = template.GetPage(pageId);
            foreach (int itemId in page.ListItemIds())
            {
                result.Add(ItemConfigurationManager.GetConfigurationData(itemId).GetDataTransferObject(template));
            }

            return result.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="searchTerm"></param>
        /// <returns></returns>
        public static GroupedResult<ReportMetaData>[] Search(CheckboxPrincipal userPrincipal, string searchTerm)
        {
            Security.AuthorizeUserContext(userPrincipal, "Analysis.Run");

            var paginationContext = new PaginationContext
            {
                SortField = "AnalysisName",
                SortAscending = true,
                FilterValue = searchTerm,
                PermissionJoin = PermissionJoin.Any,
                Permissions = new List<string>(new[] { "Analysis.Run", "Analysis.Edit" })
            };

            paginationContext.FilterField = "AnalysisName";
            var resultsByName = AnalysisTemplateManager.ListViewableAnalysisTemplates(userPrincipal, paginationContext);

            paginationContext.FilterField = "CreatedBy";
            var resultsByOwner = AnalysisTemplateManager.ListViewableAnalysisTemplates(userPrincipal, paginationContext);

            var result = new List<GroupedResult<ReportMetaData>>();

            result.Add(new GroupedResult<ReportMetaData>
            {
                GroupKey = "matchingName",
                GroupResults = (from t in resultsByName select BuildReportMetaData(t, userPrincipal)).ToArray()
            });

            result.Add(new GroupedResult<ReportMetaData>
            {
                GroupKey = "matchingOwner",
                GroupResults = (from t in resultsByOwner select BuildReportMetaData(t, userPrincipal)).ToArray() 
            });

            return result.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="reportId"></param>
        public static void DeleteReport(CheckboxPrincipal userPrincipal, int reportId)
        {
            var report = AnalysisTemplateManager.GetAnalysisTemplate(reportId);

            if (report == null)
            {
                return;
            }

            //Authorize
            Security.AuthorizeUserContext(userPrincipal, report, "Analysis.Delete");

            AnalysisTemplateManager.DeleteAnalysisTemplate(userPrincipal, reportId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="reportId"></param>
        /// <param name="newReportName"></param>
        /// <returns></returns>
        public static int CopyReport(CheckboxPrincipal userPrincipal, int reportId, string newReportName)
        {
            return 0;
        }

        /// <summary>
        /// Delete a page from a report.
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="reportId"></param>
        /// <param name="pageId"></param>
        public static void DeletePage(CheckboxPrincipal userPrincipal, int reportId, int pageId)
        {
            var report = AnalysisTemplateManager.GetAnalysisTemplate(reportId);

            if (report == null)
            {
                return;
            }

            //Authorize
            Security.AuthorizeUserContext(userPrincipal, report, "Analysis.Edit");

            //Remove page
            report.DeletePage(pageId);

            //clean up the cache
            AnalysisTemplateManager.CleanupAnalysisTemplatesCache(reportId);

            //Save
            report.Save();
        }

        /// <summary>
        /// Delete a page from a report.
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="reportId"></param>
        /// <param name="itemId"></param>
        public static void DeleteItem(CheckboxPrincipal userPrincipal, int reportId, int itemId)
        {
            var report = AnalysisTemplateManager.GetAnalysisTemplate(reportId);

            if (report == null)
            {
                throw new Exception("Unable to load data for report with id: " + reportId);
            }

            //Authorize
            Security.AuthorizeUserContext(userPrincipal, report, "Analysis.Edit");

            var pagePosition = report.GetPagePositionForItem(itemId);

            if (!pagePosition.HasValue)
            {
                throw new Exception(String.Format("The item (ID={0}) isn't contained in the report (ID={1})", itemId, reportId));
            }

            //Remove page
            report.DeleteItemFromPage(report.GetPageAtPosition(pagePosition.Value).ID.Value, itemId);

            //Save
            report.Save();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="parentItemId"></param>
        /// <param name="includeReportResponseCount"></param>
        public static PagedListResult<ReportMetaData[]> ListAvailableReports(CheckboxPrincipal userPrincipal, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue)
        {
            //Authorize user
            //Security.AuthorizeUserContext(userPrincipal, "Form.Fill");

            var paginationContext = new PaginationContext
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                Permissions = new List<string> { "Analysis.Run" },
                PermissionJoin = PermissionJoin.Any,
                SortField = "AnalysisName",
                SortAscending = sortAscending,
                FilterField = filterField,
                FilterValue = filterValue
            };

            PagedListResult<ReportMetaData[]> res = new PagedListResult<ReportMetaData[]>();
            res.ResultPage = ToReportMetaDataList(AnalysisTemplateManager.ListViewableAnalysisTemplates(userPrincipal, paginationContext), userPrincipal).ToArray();
            res.TotalItemCount = paginationContext.ItemCount;
            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="accessControllables"></param>
        /// <param name="includeSurveyResponseCount"></param>
        /// <returns></returns>
        private static List<ReportMetaData> ToReportMetaDataList(List<LightweightAnalysisTemplate> list, CheckboxPrincipal p)
        {
            return list.Select(r => ToReportMetaData(r, p)).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="accessControllable"></param>
        /// <param name="includeResponseCount"></param>
        /// <returns></returns>
        private static ReportMetaData ToReportMetaData(LightweightAnalysisTemplate report, CheckboxPrincipal p)
        {
            var reportMetaData = new ReportMetaData
            {
                ReportId = report.ID,
                Name = report.Name,
                ReportGuid = report.Guid,
                Editable = AnalysisTemplateManager.CanBeEdited(report, p),
                Deletable = AnalysisTemplateManager.CanBeDeleted(report, p)

                /*if more fields are need -- add below*/
            };

            return reportMetaData;
        }

        /// <summary>
        /// Move report item
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="reportId"></param>
        /// <param name="itemId"></param>
        /// <param name="newPageId"></param>
        /// <param name="position"></param>
        public static void MoveReportItem(CheckboxPrincipal userPrincipal, int reportId, int itemId, int? newPageId, int position)
        {
            var report = AnalysisTemplateManager.GetAnalysisTemplate(reportId);

            if (report == null)
            {
                throw new Exception("Unable to load data for report with id: " + reportId);
            }

            //Authorize
            Security.AuthorizeUserContext(userPrincipal, report, "Analysis.Edit");

            var oldPagePosition = report.GetPagePositionForItem(itemId);

            TemplatePage oldPage = null;
            if (oldPagePosition.HasValue)
                oldPage = report.GetPageAtPosition(oldPagePosition.Value);

            if (oldPage != null
                && oldPage.ID.HasValue
                && newPageId != null)
            {
                report.MoveItemToPage(itemId, oldPage.ID.Value, newPageId.Value, position);
                ResponseTemplateManager.MarkTemplateUpdated(reportId);
                report.Save();
            }
        }

        /// <summary>
        /// Move report item
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="reportId"></param>
        /// <param name="pageId"></param>
        /// <param name="position"></param>
        public static void MoveReportPage(CheckboxPrincipal userPrincipal, int reportId, int pageId, int position)
        {
            var report = AnalysisTemplateManager.GetAnalysisTemplate(reportId);

            if (report == null)
            {
                throw new Exception("Unable to load data for report with id: " + reportId);
            }

            //Authorize
            Security.AuthorizeUserContext(userPrincipal, report, "Analysis.Edit");

            report.MovePage(pageId, position);
            report.Save();

            ResponseTemplateManager.MarkTemplateUpdated(reportId);
        }

        /// <summary>
        /// Add new page
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="reportId"></param>
        public static void AddReportPage(CheckboxPrincipal userPrincipal, int reportId)
        {
            var report = AnalysisTemplateManager.GetAnalysisTemplate(reportId);

            if (report == null)
            {
                throw new Exception("Unable to load data for report with id: " + reportId);
            }

            //Authorize
            Security.AuthorizeUserContext(userPrincipal, report, "Analysis.Edit");

            report.AddPageToTemplate(null, true);
            report.Save();

            //clean up the cache
            AnalysisTemplateManager.CleanupAnalysisTemplatesCache(reportId);

            ResponseTemplateManager.MarkTemplateUpdated(reportId);
        }

        #endregion


        #region Utility Methods

        /// <summary>
        /// Authorize access to report. Throws exception on failure.
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="reportId"></param>
        /// <param name="permissions"></param>
        private static void AuthorizeAccess(CheckboxPrincipal userPrincipal, int reportId, params string[] permissions)
        {
            //Load report
            var ltReport = AnalysisTemplateManager.GetLightweightAnalysisTemplate(reportId);

            if (ltReport == null)
            {
                throw new Exception("Unable to load report with id: " + reportId);
            }

            Security.AuthorizeUserContext(userPrincipal, ltReport, permissions);
        }


        /// <summary>
        /// Build report meta data from analysis template
        /// </summary>
        /// <param name="at"></param>
        /// <returns></returns>
        private static ReportMetaData BuildReportMetaData(AnalysisTemplate at)
        {
            if (at == null
                || at.ID == null)
            {
                return null;
            }

            var data = new ReportMetaData
            {
                MaxCompletionDateFilter = at.FilterEndDate,
                MinCompletionDateFilter = at.FilterStartDate,
                Name = at.Name,
                ReportGuid = at.Guid,
                ReportId = at.ID.Value,
                ResponseTemplateId = at.ResponseTemplateID,
                ResponseTemplateName = ResponseTemplateManager.GetLightweightResponseTemplate(at.ResponseTemplateID).Name,
                CreatedBy = at.CreatedBy,
                LastModified = WebUtilities.ConvertToClientTimeZone(at.LastModified.Value),
                DateCreated = WebUtilities.ConvertToClientTimeZone(at.CreatedDate.Value),
                PageIds = at.ListTemplatePageIds().ToArray(),
                StyleTemplateId = at.StyleTemplateID,
                DisplaySurveyTitle = at.DisplaySurveyTitle,
                DisplayPdfExportButton = at.DisplayPdfExportButton,
                IncludeIncompleteResponses = at.IncludeIncompleteResponses,
                IncludeTestResponses = at.IncludeTestResponses,
                FilterEndDate = at.FilterEndDate,
                FilterStartDate = at.FilterStartDate 
            };

            var responseTemplate = ResponseTemplateManager.GetResponseTemplate(at.ResponseTemplateID);

            data.DefaultSurveyLanguage = responseTemplate == null ? TextManager.DefaultLanguage : responseTemplate.LanguageSettings.DefaultLanguage;

            return data;
        }

        /// <summary>
        /// Build report meta data from analysis template
        /// </summary>
        /// <param name="at"></param>
        /// <returns></returns>
        private static ReportMetaData BuildReportMetaData(LightweightAnalysisTemplate at, CheckboxPrincipal p)
        {
            if (at == null)
            {
                return null;
            }

            return new ReportMetaData
            {
                MaxCompletionDateFilter = at.DateFilterEnd,
                MinCompletionDateFilter = at.DateFilterEnd,
                Name = at.Name,
                ReportGuid = at.Guid,
                ReportId = at.ID,
                ResponseTemplateId = at.ResponseTemplateId.Value,
                ResponseTemplateName = ResponseTemplateManager.GetLightweightResponseTemplate(at.ResponseTemplateId.Value).Name,
                LastModified = WebUtilities.ConvertToClientTimeZone(at.DateModified),
                DateCreated = WebUtilities.ConvertToClientTimeZone(at.DateCreated),
                CreatedBy = at.CreatedBy,
                StyleTemplateId = at.StyleTemplateId,
                FilterEndDate = WebUtilities.ConvertToClientTimeZone(at.DateFilterEnd),
                FilterStartDate = WebUtilities.ConvertToClientTimeZone(at.DateFilterStart),
                Editable = AnalysisTemplateManager.CanBeEdited(at, p),
                Deletable = AnalysisTemplateManager.CanBeDeleted(at, p)
            };
        }

        /// <summary>
        /// Build report meta data from analysis template
        /// </summary>
        /// <param name="at"></param>
        /// <returns></returns>
        private static ReportFilterData BuildReportFilterMetaData(FilterData at)
        {
            if (at == null)
            {
                return null;
            }

            at.Load();

            string filterText = "";

            return new ReportFilterData
            {
                FilterId = at.ID.Value,
                SourceType = at.FilterTypeName.ToString(),
                Comparison = at.Operator.ToString(),
                ValueAsString = at.Value.ToString(),
                FilterText = filterText
            };
        }


        /// <summary>
        /// Build meta data for report page
        /// </summary>
        /// <param name="tp"></param>
        /// <returns></returns>
        private static ReportPageMetaData BuildReportPageMetaData(TemplatePage tp)
        {
            if (tp == null
                || tp.ID == null)
            {
                return null;
            }

            return new ReportPageMetaData
            {
                Position = tp.Position,
                ItemIds = tp.ListItemIds(),
                Id = tp.ID.Value,
                LayoutTemplateId = tp.LayoutTemplateId
            };
        }


        #endregion

        private static PaginationContext CreatePaginationContext(string filterField = "", string filterValue = "",
            string sortField = "", bool sortAscending = true, int pageNumber = -1, int pageSize = -1)
        {
            //check params
            switch (sortField)
            {
                case "ResponseTemplateID":
                case "AnalysisTemplateID":
                    break;
                default:
                    sortField = "AnalysisName";
                    break;
            }
            return new PaginationContext
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                SortField = sortField,
                SortAscending = sortAscending,
                FilterField = filterField,
                FilterValue = Utilities.ProtectFilterFieldFromSQLInjections(filterValue)
            };
        }
    }
}
