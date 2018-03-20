using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using Checkbox.Analytics;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Forms.Items;
using Checkbox.Invitations;
using Checkbox.Pagination;
using Checkbox.Security;
using Checkbox.Security.Principal;
using Checkbox.Wcf.Services.Proxies;
using Checkbox.Web;
using Checkbox.Web.UI.Controls;
using Checkbox.Timeline;
using Checkbox.Forms.Items.Configuration;

namespace Checkbox.Wcf.Services
{
    /// <summary>
    /// Non service-specific implementation of response data service.
    /// </summary>
    public static class ResponseDataServiceImplementation
    {
        /// <summary>
        /// Get summary data for the specified survey
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="surveyId"></param>
        /// <returns></returns>
        public static ResponseSummaryData GetResponseSummary(CheckboxPrincipal userPrincipal, int surveyId)
        {
            var lightweightRt = ResponseTemplateManager.GetLightweightResponseTemplate(surveyId);

            if (lightweightRt == null)
            {
                return null;
            }

            Security.AuthorizeUserContext(userPrincipal, lightweightRt, "Analysis.Responses.View", "Analysis.Responses.Export", "Analysis.Create", "Form.Administer");

            var completeNonTestResponseCount = ResponseManager.GetResponseCount(surveyId, false, false);
            var totalNonTestResponseCount = ResponseManager.GetResponseCount(surveyId, true, false);
            var totalResponseCount = ResponseManager.GetResponseCount(surveyId, true, true);

            return new ResponseSummaryData
            {
                CompletedResponseCount = completeNonTestResponseCount,
                IncompleteResponseCount = totalNonTestResponseCount - completeNonTestResponseCount,
                TestResponseCount = totalResponseCount - totalNonTestResponseCount
            };
        }

        /// <summary>
        /// Get summary data for the specified survey
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="surveyId"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static ResponseData[] ListRecentResponses(CheckboxPrincipal userPrincipal, int surveyId, int count)
        {
            var lightweightRt = ResponseTemplateManager.GetLightweightResponseTemplate(surveyId);

            if (lightweightRt == null)
            {
                return null;
            }

            Security.AuthorizeUserContext(userPrincipal, lightweightRt, "Analysis.Responses.View");

            var responseArray = ResponseManager.GetResponseList(
                surveyId, 
                true,
                true,
                true,
                new PaginationContext
                    {
                      PageSize   = count,
                      CurrentPage = 1,
                      SortField = "Started",
                      SortAscending = false
                    }
                ).ToArray();

            //Convert dates to client's time zone.
            foreach (ResponseData responseData in responseArray)
            {
                responseData.CompletionDate = WebUtilities.ConvertToClientTimeZone(responseData.CompletionDate);
                responseData.Started = WebUtilities.ConvertToClientTimeZone(responseData.Started);
                responseData.LastEditDate = WebUtilities.ConvertToClientTimeZone(responseData.LastEditDate);
            }

            return responseArray;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="responseId"></param>
        /// <param name="surveyId"></param>
        /// <param name="answerId"></param>
        /// <param name="answerText"></param>
        /// <param name="optionID"></param>
        /// <param name="dateCreated"></param>
        public static void UpdateResponseAnswer(CheckboxPrincipal userPrincipal, int surveyId, long responseId, long answerId, string answerText, int? optionID, DateTime? dateCreated)
        {
            var lightweightRt = ResponseTemplateManager.GetLightweightResponseTemplate(surveyId);

            if (lightweightRt == null)
                throw new ResponseTemplateDoesNotExistException(surveyId);

            Security.AuthorizeUserContext(userPrincipal, lightweightRt, "Analysis.Responses.Edit");

            if(ResponseManager.GetSurveyIdFromResponseId(responseId) != surveyId)
                throw new ResponseDoesNotExistException(responseId);

            if(!ResponseManager.DoesAnswerBelongToResponse(responseId, answerId))
                throw new ResponseAnswerDoesNotExistException(answerId);

            ResponseManager.UpdateResponseAnswer(answerId, answerText, optionID, dateCreated);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="surveyId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="resultsPerPage"></param>
        /// <param name="filterField"></param>
        /// <param name="filterValue"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="period">0 - don't filter by period. 2 -- daily, 3 - weekly, 4 -- monthly created/modified entities</param>
        /// <param name="dateFieldName">Specifies what field of datetime type should be used when filtering by period. Null or empty -- don't filter by period.</param>
        /// <param name="filterKey"></param>
        /// <returns></returns>
        public static PagedListResult<ResponseData[]> ListSurveyResponses(
            CheckboxPrincipal userPrincipal,
            int surveyId,
            int pageNumber,
            int resultsPerPage,
            string filterField,
            string filterValue,
            string sortField,
            bool sortAscending,
            int period,
            string dateFieldName,
            string filterKey,
            int profileFieldId = 0)
        {
            if (surveyId > 0)
            {
                var lightweightRt = ResponseTemplateManager.GetLightweightResponseTemplate(surveyId);

                if (lightweightRt == null)
                {
                    return null;
                }

                Security.AuthorizeUserContext(userPrincipal, lightweightRt, "Analysis.Responses.View");
            }

            var includeComplete = true;
            var includeIncomplete = true;
            var includeTestResponses = true;
            var onlyTestResponses = false;

            string[] filters;
            if (!string.IsNullOrEmpty(filterKey) && (filters =  filterKey.Split(',')).Any())
            {
                includeComplete = false;
                includeIncomplete = false;
                includeTestResponses = false;

                foreach (var filter in filters)
                {
                    switch (filter.ToLower())
                    {
                        case "complete":
                            includeComplete = true;
                            break;
                        case "incomplete":
                            includeIncomplete = true;
                            break;
                        case "test":
                            includeTestResponses = true;
                            break;
                    }
                }

                if (!includeComplete && !includeIncomplete && includeTestResponses)
                {
                    onlyTestResponses = true;
                    includeComplete = true;
                    includeIncomplete = true;
                }
            }

            return ListSurveyResponses(
                surveyId,
                pageNumber,
                resultsPerPage,
                filterField,
                filterValue,
                sortField,
                sortAscending,
                period,
                dateFieldName,
                includeComplete,
                includeIncomplete,
                onlyTestResponses, 
                includeTestResponses,
                profileFieldId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="surveyId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="resultsPerPage"></param>
        /// <param name="filterField"></param>
        /// <param name="filterValue"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="period"></param>
        /// <param name="dateFieldName"></param>
        /// <param name="includeIncomplete"></param>
        /// <param name="includeComplete"></param>
        /// <param name="onlyTestResponses"></param>
        /// <param name="includeTestResponses"></param>
        /// <returns></returns>
        private static PagedListResult<ResponseData[]> ListSurveyResponses(
            int surveyId,
            int pageNumber,
            int resultsPerPage,
            string filterField,
            string filterValue,
            string sortField,
            bool sortAscending,
            int period,
            string dateFieldName,
            bool includeComplete,
            bool includeIncomplete,
            bool onlyTestResponses,
            bool includeTestResponses,
            int profileFieldId = 0)
        {
            var paginationContext = new PaginationContext
            {
                PageSize = resultsPerPage,
                CurrentPage = pageNumber,
                FilterField = filterField,
                FilterValue = filterValue,
                SortField = ValidateSortField(sortField),
                SortAscending = sortAscending,
            };

            DateTime? start = null, end = null;
            if (!string.IsNullOrEmpty(dateFieldName))
            {
                TimelineManager.GetPeriodDates(period, out start, out end);
                paginationContext.DateFieldName = TimelineManager.ProtectFieldNameFromSQLInjections(dateFieldName);
            }

            var results = onlyTestResponses
                ? ResponseManager.GetTestResponseList(surveyId, includeIncomplete, includeComplete, paginationContext, start, end)
                : ResponseManager.GetResponseList(surveyId, includeIncomplete, includeComplete, includeTestResponses, paginationContext, start, end, profileFieldId);

            //Convert dates to the client's time zone
            foreach (ResponseData responseData in results)
            {
                responseData.Started = WebUtilities.ConvertToClientTimeZone(responseData.Started);
                responseData.CompletionDate = WebUtilities.ConvertToClientTimeZone(responseData.CompletionDate);
                responseData.LastEditDate = WebUtilities.ConvertToClientTimeZone(responseData.LastEditDate);
            }

            return new PagedListResult<ResponseData[]> { ResultPage = results.ToArray(), TotalItemCount = paginationContext.ItemCount };
        }

        private static string ValidateSortField(string sortField)
        {
            switch (sortField.ToLower())
            {
                case "responseid":
                case "guid":
                case "responseTemplateid":
                case "iscomplete":
                case "lastpageviewed":
                case "started":
                case "ended":
                case "ip":
                case "lastedit":
                case "networkuser":
                case "language":
                case "uniqueidentifier":
                case "deleted":
                case "respondentguid":
                case "istest":
                case "invitee":
                case "resumekey":
                case "sessionid":
                case "isanonymized":
                case "templatename":
                    break;
                default:
                    sortField = "ResponseID";
                    break;
            }

            return sortField;
        }

        /// <summary>
        /// Export responses in tabular form
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="surveyId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="resultsPerPage"></param>
        /// <param name="filterField"></param>
        /// <param name="filterValue"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="period"></param>
        /// <param name="dtStart"></param>
        /// <param name="dtEnd"></param>
        /// <param name="detailedResponseInfo"></param>
        /// <param name="detailedUserInfo"></param>
        /// <param name="includeOpenEndedResults"></param>
        /// <param name="includeAliases"></param>
        /// <param name="includeHiddenItems"></param>
        /// <param name="includeIncompleteResponses"></param>
        /// <param name="stripHTMLTagsFromAnswers"></param>
        /// <param name="stripHTMLTagsFromQuestions"></param>
        /// <param name="mergeAnswersForSelectMany"></param>
        /// <param name="includeScoreData"></param>
        /// <returns></returns>
        public static PagedListResult<TabularResponseExportData> ExportResponsesTabular(
            CheckboxPrincipal userPrincipal,
            int surveyId,
            int pageNumber,
            int resultsPerPage,
            string filterField,
            string filterValue,
            string sortField,
            bool sortAscending,
            int period,
            DateTime dtStart,
            DateTime dtEnd,
            bool detailedResponseInfo,
            bool detailedUserInfo,
            bool includeOpenEndedResults,
            bool includeAliases,
            bool includeHiddenItems,
            bool includeIncompleteResponses,
            bool stripHTMLTagsFromAnswers,
            bool stripHTMLTagsFromQuestions,
            bool mergeAnswersForSelectMany,
            bool includeScoreData)
        {
            if (surveyId > 0)
            {
                var lightweightRt = ResponseTemplateManager.GetLightweightResponseTemplate(surveyId);

                if (lightweightRt == null)
                {
                    return null;
                }

                Security.AuthorizeUserContext(userPrincipal, lightweightRt, "Analysis.Responses.View");
            }

            var paginationContext = new PaginationContext
            {
                PageSize = resultsPerPage,
                CurrentPage = pageNumber,
                FilterField = filterField,
                FilterValue = filterValue,
                SortField = Utilities.IsNotNullOrEmpty(sortField) ? sortField : "ResponseID",
                SortAscending = sortAscending,
            };

            var result = new TabularResponseExportData();

            var responses = ResponseManager.GetResponseList(surveyId, includeIncompleteResponses, true, true, paginationContext, dtStart, dtEnd);

            var respondents = new List<UserData>();

            //these caches help to improve performance 
            Dictionary<int, ItemData> itemDatas = null;
            //necessary due to matrix complexity
            Dictionary<int, ItemData> itemPrototypes = null;
            //necessary due to matrix complexity
            Dictionary<int, int> itemRows = null;

            var tabulatedResponses = new List<SimpleNameValueCollection>();

            foreach (var responseData in responses)
            {
                var responseFieldCollection = new SimpleNameValueCollection();

                //Convert dates to the client's time zone
                convertResponseDatesToClientTimeZone(responseData);

                //collect detailed response data
                if (detailedResponseInfo)
                    insertResponseDataToNVC(responseFieldCollection, responseData);                
                else
                    responseFieldCollection["Id"] = responseData.Id.ToString();

                var rt = ResponseTemplateManager.GetResponseTemplate(surveyId);
                var answers = GetAnswersForResponseByGuid(userPrincipal, rt, responseData.ResponseLanguage, responseData.Guid, false, includeOpenEndedResults, includeAliases, stripHTMLTagsFromQuestions);

                //add score
                if (rt.BehaviorSettings.EnableScoring && includeScoreData)
                {
                    for (int i=2; i<=rt.PageCount-1; i++)
                    {
                        var page = rt.GetPageAtPosition(i);
                        if (page != null && page.ID.HasValue)
                        {
                            var score = answers.Where(a => a.PageId == page.ID.Value).SelectMany(a => a.Answers).Where(a => a.IsScored && a.Points.HasValue).Sum(a => a.Points.Value);
                            responseFieldCollection[string.Format("Page {0} Score", page.Position-1)] = score.ToString();
                        }
                    }
                    //add total score
                    var sum = answers.Sum(a => (from aa in a.Answers where aa.IsScored && aa.Points.HasValue select aa.Points.Value).Sum());
                    responseFieldCollection["Score"] = sum.ToString();
                }

                if (detailedResponseInfo)
                {
                    var totalTime = string.Empty;
                    if (responseData.Started != null && responseData.CompletionDate != null)
                    {
                        var start = (DateTime)responseData.Started;
                        var end = (DateTime)responseData.CompletionDate;

                        var time = end.Subtract(start);

                        totalTime = time.ToString();
                    }

                    responseFieldCollection["TotalTime"] = totalTime;
                }

                //collect metadata if needed
                UnsureItemMetadataCollected(ref itemDatas, ref itemPrototypes, ref itemRows, surveyId);

                //collect answer data and build proper keys 
                InsertAnswerDataToNVC(responseFieldCollection, answers, itemDatas, itemPrototypes, itemRows,
                    stripHTMLTagsFromQuestions, stripHTMLTagsFromAnswers, includeAliases, mergeAnswersForSelectMany, responseData.ResponseLanguage, surveyId, includeOpenEndedResults);                

                tabulatedResponses.Add(responseFieldCollection);
            }

            //collect detailed respondent data
            if (detailedUserInfo)
            {
                var uids = (from r in responses select r.UserIdentifier).Distinct();
                respondents.AddRange(uids.Select(uid => UserManagementServiceImplementation.GetUserData(userPrincipal, uid)));
                result.Respondents = respondents.ToArray();
            }

            result.Responses = tabulatedResponses.ToArray();
            return new PagedListResult<TabularResponseExportData> { ResultPage = result, TotalItemCount = paginationContext.ItemCount };
        }

        /// <summary>
        /// Export responses
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="surveyId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="resultsPerPage"></param>
        /// <param name="filterField"></param>
        /// <param name="filterValue"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="period"></param>
        /// <param name="dtStart"></param>
        /// <param name="dtEnd"></param>
        /// <param name="DetailedResponseInfo"></param>
        /// <param name="DetailedUserInfo"></param>
        /// <param name="IncludeOpenEndedResults"></param>
        /// <param name="IncludeAliases"></param>
        /// <param name="IncludeHiddenItems"></param>
        /// <param name="IncludeIncompleteResponses"></param>
        /// <param name="StripHTMLTagsFromAnswers"></param>
        /// <returns></returns>
        public static PagedListResult<ResponseExportData> ExportResponses(
            CheckboxPrincipal userPrincipal,
            int surveyId,
            int pageNumber,
            int resultsPerPage,
            string filterField,
            string filterValue,
            string sortField,
            bool sortAscending,
            int period,
            DateTime dtStart,
            DateTime dtEnd,
            bool DetailedResponseInfo,
            bool DetailedUserInfo,
            bool IncludeOpenEndedResults,
            bool IncludeAliases,
            bool IncludeHiddenItems,
            bool IncludeIncompleteResponses,
            bool StripHTMLTagsFromAnswers)
        {
            if (surveyId > 0)
            {
                var lightweightRt = ResponseTemplateManager.GetLightweightResponseTemplate(surveyId);

                if (lightweightRt == null)
                {
                    return null;
                }

                Security.AuthorizeUserContext(userPrincipal, lightweightRt, "Analysis.Responses.View");
            }

            var paginationContext = new PaginationContext
            {
                PageSize = resultsPerPage,
                CurrentPage = pageNumber,
                FilterField = filterField,
                FilterValue = filterValue,
                SortField = Utilities.IsNotNullOrEmpty(sortField) ? sortField : "ResponseID",
                SortAscending = sortAscending,
            };

            var result = new ResponseExportData();

            var responses = ResponseManager.GetResponseList(surveyId, IncludeIncompleteResponses, true, true, paginationContext, dtStart, dtEnd);

            //Convert dates to the client's time zone
            foreach (var responseData in responses)
            {
                convertResponseDatesToClientTimeZone(responseData);
            }            

            result.Responses = responses.ToArray();

            //collect detailed response data
            if (DetailedResponseInfo)
            {
                var itemIds = new List<int>();

                foreach (var responseData in responses)
                {
                    var rt = ResponseTemplateManager.GetResponseTemplate(surveyId);
                    responseData.Answers = GetAnswersForResponseByGuid(userPrincipal, rt, responseData.ResponseLanguage, responseData.Guid);
                    
                    //get items
                    var items = (from a in responseData.Answers where !itemIds.Contains(a.ItemId) select a.ItemId).Distinct().ToArray();

                    if (IncludeAliases)
                        ApplyAliasOnOptions(responseData.Answers);

                    /*if (!IncludeAliases)
                    {
                        //delete aliases
                        foreach (var a in responseData.Answers)
                        {
                            a.Alias = null;
                            foreach (var ia in a.Answers)
                            {
                                ia.Alias = null;
                            }
                        }
                    }
                    else
                    {
                        //delete item texts and description
                        foreach (var a in responseData.Answers)
                        {
                            a.Text = null;
                            a.Description = null;
                        }
                    }*/

                    itemIds.AddRange(items);
                }

                //collect item data
                var itemDatas = new List<SurveyItemMetaData>();
                foreach (var itemId in itemIds)
                {
                    var itemData = SurveyManagementServiceImplementation.GetSurveyItemMetaData(userPrincipal, surveyId, itemId);
                    SurveyItemMetaData surveyItemMetaData = itemData as SurveyItemMetaData;
                    if (surveyItemMetaData == null)
                        continue;
                    if (IncludeHiddenItems || itemData.TypeName != "HiddenItem")
                    {
                        itemDatas.Add(surveyItemMetaData);
                    }
                    else
                    {
                        //remove answers for the hidden item
                        foreach (var responseData in responses)
                        {
                            responseData.Answers = (from a in responseData.Answers where a.ItemId != itemId select a).ToArray();
                        }
                    }
                }
                //remove open-ended results
                if (!IncludeOpenEndedResults)
                {
                    //remove answers for the hidden item
                    foreach (var responseData in responses)
                    {
                        var radsToRemove = new List<ResponseItemAnswerData>();
                        foreach (var rad in responseData.Answers)
                        {
                            var imd = itemDatas.FirstOrDefault(i => i.ItemId == rad.ItemId);
                            if (imd != null && isOpenEndedItem(imd.TypeName))
                            {
                                radsToRemove.Add(rad);
                                continue;
                            }
                            foreach (var ria in rad.Answers)
                            {                                
                                if (!string.IsNullOrEmpty(ria.AnswerText))
                                {
                                    radsToRemove.Add(rad);
                                    break;
                                }
                            }
                        }
                        responseData.Answers = (from a in responseData.Answers where !radsToRemove.Contains(a) select a).ToArray();
                    }
                }

                if (StripHTMLTagsFromAnswers)
                {
                    //remove answers for the hidden item
                    foreach (var responseData in responses)
                    {
                        foreach (var rad in responseData.Answers)
                        {
                            foreach (var ria in rad.Answers)
                            {                                
                                if (!string.IsNullOrEmpty(ria.AnswerText))
                                {
                                    ria.AnswerText = Utilities.StripHtmlTags(ria.AnswerText);
                                }
                            }
                        }
                    }
                    
                }
            }
            
            //collect detailed respondent data
            if (DetailedUserInfo)
            {
                var respondents = new List<UserData>();
                var uids = (from r in result.Responses select r.UserIdentifier).Distinct();
                foreach (var uid in uids)
                {
                    respondents.Add(UserManagementServiceImplementation.GetUserData(userPrincipal, uid));
                }
                result.Respondents = respondents.ToArray();
            }

            return new PagedListResult<ResponseExportData>{ResultPage = result, TotalItemCount = paginationContext.ItemCount };
        }


        public static readonly string[] openEndedTypes = { "SingleLineText", "MultiLineText", "AddressVerifier" };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="surveyId"></param>
        /// <param name="numberOfRecentMonths"></param>
        /// <returns></returns>
        public static string GetLifecycleResponseDataInMonths(CheckboxPrincipal userPrincipal, int surveyId, int numberOfRecentMonths)
        {
            var lightweightRt = ResponseTemplateManager.GetLightweightResponseTemplate(surveyId);

            if (lightweightRt == null)
            {
                return null;
            }

            Security.AuthorizeUserContext(userPrincipal, lightweightRt, "Analysis.Responses.View", "Analysis.Responses.Export", "Analysis.Create", "Form.Administer");

            return ResponseLifecycleChartControlHelper.GetResponseLifecycleChartImageUrl(surveyId, numberOfRecentMonths);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="surveyId"></param>
        /// <param name="periodLengthInDays"></param>
        /// <param name="numberOfPeriods"></param>
        /// <returns></returns>
        public static string GetLifecycleResponseDataInDays(CheckboxPrincipal userPrincipal, int surveyId, int periodLengthInDays, int numberOfPeriods)
        {
            var lightweightRt = ResponseTemplateManager.GetLightweightResponseTemplate(surveyId);

            if (lightweightRt == null)
            {
                return null;
            }

            Security.AuthorizeUserContext(userPrincipal, lightweightRt, "Analysis.Responses.View", "Analysis.Responses.Export", "Analysis.Create", "Form.Administer");

            return ResponseLifecycleChartControlHelper.GetResponseLifecycleChartImageUrl(surveyId, periodLengthInDays, numberOfPeriods);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="surveyId"></param>
        /// <param name="periodLengthInDays"></param>
        /// <param name="numberOfPeriods"></param>
        /// <returns></returns>
        public static ResponseAggregatedData GetLifecycleAggregatedResponseDataInDays(CheckboxPrincipal userPrincipal, int surveyId, int periodLengthInDays, int numberOfPeriods)
        {
            if (surveyId == -1)
            {
                if (userPrincipal.IsInRole("System Administrator"))
                    return ResponseLifecycleChartControlHelper.GetResponseLifecycle(surveyId, periodLengthInDays, numberOfPeriods);
                
                return null;
            }

            var lightweightRt = ResponseTemplateManager.GetLightweightResponseTemplate(surveyId);

            Security.AuthorizeUserContext(userPrincipal, lightweightRt, "Analysis.Responses.View", "Analysis.Responses.Export", "Analysis.Create", "Form.Edit");

            return ResponseLifecycleChartControlHelper.GetResponseLifecycle(surveyId, periodLengthInDays, numberOfPeriods);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="surveyId"></param>
        /// <param name="responseIds"></param>
        public static void DeleteResponses(CheckboxPrincipal userPrincipal, int surveyId, long[] responseIds)
        {
            var lightweightRt = ResponseTemplateManager.GetLightweightResponseTemplate(surveyId);

            if (lightweightRt == null)
            {
                return;
            }

            Security.AuthorizeUserContext(userPrincipal, lightweightRt, "Analysis.Responses.Edit");


            ResponseManager.DeleteResponses(responseIds.ToList());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="surveyId"></param>
        public static void DeleteAllSurveyResponses(CheckboxPrincipal userPrincipal, int surveyId)
        {
            var lightweightRt = ResponseTemplateManager.GetLightweightResponseTemplate(surveyId);

            if (lightweightRt == null)
            {
                return;
            }

            Security.AuthorizeUserContext(userPrincipal, lightweightRt, "Analysis.Responses.Edit");


            ResponseManager.DeleteAllResponses(surveyId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="surveyId"></param>
        public static void DeleteTestSurveyResponses(CheckboxPrincipal userPrincipal, int surveyId)
        {
            var lightweightRt = ResponseTemplateManager.GetLightweightResponseTemplate(surveyId);

            if (lightweightRt == null)
            {
                return;
            }

            Security.AuthorizeUserContext(userPrincipal, lightweightRt, "Analysis.Responses.Edit");

            //Delete test responses
            ResponseManager.DeleteAllTestResponses(surveyId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="searchTerm"></param>
        /// <returns></returns>
        public static GroupedResult<ResponseData>[] Search(CheckboxPrincipal userPrincipal,  string searchTerm)
        {
            Security.AuthorizeUserContext(userPrincipal, "Analysis.Responses.View");

            var responseTemplateIdList = new List<int?>();

            if (userPrincipal != null && userPrincipal.IsInRole("System Administrator"))
            {
                responseTemplateIdList.Add(null);
            }
            else
            {
                responseTemplateIdList.AddRange(
                        ResponseTemplateManager.ListAccessibleTemplates(
                            userPrincipal,
                            null,
                            new PaginationContext
                            {
                                CurrentPage = -1,
                                FilterField = string.Empty,
                                FilterValue = string.Empty,
                                PermissionJoin = PermissionJoin.Any,
                                PageSize = -1,
                                Permissions = new List<string>(new[] {"Analysis.Responses.View"})
                            }, true, true
                        )
                        .Select(lightweightRt => new int?(lightweightRt.ID))
                );
            }

            //Results by user
            var resultByUserList = new List<ResponseData>();

            foreach (int? responseTemplateId in responseTemplateIdList)
            {
                resultByUserList.AddRange(ResponseManager.GetResponseList(
                    responseTemplateId,
                    true,
                    true,
                    true,
                    new PaginationContext
                        {
                            FilterField = "UniqueIdentifier",
                            FilterValue = searchTerm
                        }));
            }

            //Results by response id
            var resultsByIdList = new List<ResponseData>();

            if (Utilities.IsLongInt(searchTerm))
            {
                foreach (int? responseTemplateId in responseTemplateIdList)
                {
                    resultsByIdList.AddRange(ResponseManager.GetResponseList(
                        responseTemplateId,
                        true,
                        true,
                        true,
                        new PaginationContext
                            {
                                FilterField = "ResponseId",
                                FilterValue = searchTerm
                            }));
                }
            }


            //Results by invitee
            var resultByInviteeList = new List<ResponseData>();

            foreach (int? responseTemplateId in responseTemplateIdList)
            {
                resultByInviteeList.AddRange(ResponseManager.GetResponseList(
                    responseTemplateId,
                    true,
                    true,
                    true,
                    new PaginationContext
                        {
                            FilterField = "Invitee",
                            FilterValue = searchTerm
                        }));
            }

            //Results by GUID
            var resultByGuidList = new List<ResponseData>();

            foreach (int? responseTemplateId in responseTemplateIdList)
            {
                resultByGuidList.AddRange(ResponseManager.GetResponseList(
                    responseTemplateId,
                    true,
                    true,
                    true,
                    new PaginationContext
                    {
                        FilterField = "GUID",
                        FilterValue = searchTerm
                    }));
            }

            //Convert date times to the client's time zone.
            ConvertResponsesToClientTimeZone(resultByUserList);
            ConvertResponsesToClientTimeZone(resultsByIdList);
            ConvertResponsesToClientTimeZone(resultByInviteeList);
            ConvertResponsesToClientTimeZone(resultByGuidList);

            //Assemble results
            var result = new List<GroupedResult<ResponseData>>
                             {
                                 new GroupedResult<ResponseData>
                                     {
                                         GroupKey = "matchingUser",
                                         GroupResults = resultByUserList.ToArray()
                                     },
                                 new GroupedResult<ResponseData>
                                     {
                                         GroupKey = "matchingId",
                                         GroupResults = resultsByIdList.ToArray()
                                     },
                                 new GroupedResult<ResponseData>
                                     {
                                         GroupKey = "matchingInvitee",
                                         GroupResults = resultByInviteeList.ToArray()
                                     },
                                 new GroupedResult<ResponseData>
                                     {
                                         GroupKey = "matchingGuid",
                                         GroupResults = resultByGuidList.ToArray()
                                     }
                             };

            return result.ToArray();
        }

        #region Private Methods

        /// <summary>
        /// Convert all dateTime properties of responses to the client's time zone.
        /// </summary>
        /// <param name="responses"></param>
        private static void ConvertResponsesToClientTimeZone(List<ResponseData> responses)
        {
            foreach (ResponseData responseData in responses)
            {
                responseData.Started = WebUtilities.ConvertToClientTimeZone(responseData.Started);
                responseData.LastEditDate = WebUtilities.ConvertToClientTimeZone(responseData.LastEditDate);
                responseData.CompletionDate = WebUtilities.ConvertToClientTimeZone(responseData.CompletionDate);                
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="checkboxPrincipal"></param>
        /// <param name="rt"></param>
        /// <param name="languageCode"></param>
        /// <param name="responseGuid"></param>
        /// <returns></returns>
        public static ResponseItemAnswerData[] GetAnswersForResponseByGuid(CheckboxPrincipal checkboxPrincipal, ResponseTemplate rt, string languageCode, Guid responseGuid)
        {
            return GetAnswersForResponseByGuid(checkboxPrincipal, rt, languageCode, responseGuid, true, true, false, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="checkboxPrincipal"></param>
        /// <param name="surveyId"></param>
        /// <param name="languageCode"></param>
        /// <param name="responseGuid"></param>
        /// <returns></returns>
        public static ResponseItemAnswerData[] GetAnswersForResponseByGuid(CheckboxPrincipal checkboxPrincipal, int surveyId, string languageCode, Guid responseGuid)
        {
            return GetAnswersForResponseByGuid(checkboxPrincipal, ResponseTemplateManager.GetResponseTemplate(surveyId), languageCode, responseGuid, true, true, false, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        private static Dictionary<int, Item[]> GetItemAnsweredData(Response response)
        {
            var responsePages = response
                            .VistedPageStack
                            .Where(page => page.PageType != TemplatePageType.Completion &&
                                page.GetItems().Any(pi => pi is IAnswerable && ((IAnswerable)pi).HasAnswer))
                            .OrderBy(page => page.Position);

            return 
                responsePages.GroupBy(p => p.PageId).ToDictionary(
                    p => p.Key, p => (p.Any() ? p.Last().GetItems() : new List<Item>()).Where(
                        item => item is IAnswerable && ((IAnswerable)item).HasAnswer).ToArray());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="checkboxPrincipal"></param>
        /// <param name="rt"></param>
        /// <param name="languageCode"></param>
        /// <param name="responseGuid"></param>
        /// <param name="onlyAnswered"></param>
        /// <param name="includeOpenEndedResults"></param>
        /// <param name="includeAliases"></param>
        /// <param name="stripHTMLTagsFromQuestions"></param>
        /// <returns></returns>
        private static ResponseItemAnswerData[] GetAnswersForResponseByGuid(CheckboxPrincipal checkboxPrincipal, ResponseTemplate rt, string languageCode, Guid responseGuid, bool onlyAnswered, bool includeOpenEndedResults, bool includeAliases, bool stripHTMLTagsFromQuestions)
        {
            Security.AuthorizeUserContext(checkboxPrincipal, "Analysis.Responses.View");

            var response = rt.CreateResponse(languageCode, responseGuid);
            response.Restore(responseGuid);

            Dictionary<int, Item[]> pageData;
            Dictionary<int, Item[]> answeredPageData = GetItemAnsweredData(response);

            //in case of including all items (answered and non-aswered), 
            //we need get their data from response template  
            if (!onlyAnswered)
            {
                var templatePages = rt.ListTemplatePageIds().Select(rt.GetPage);
                pageData = templatePages.Where(p => p.ID.HasValue).ToDictionary(p => p.ID.Value, p => p.ListItemIds().
                    Select(rt.GetItem).Select(i => i.CreateItem(languageCode, rt.ID)).ToArray());
            }
            else
                pageData = answeredPageData;


            var itemAnswers = new List<ResponseItemAnswerData>();

            foreach (var page in pageData)
            {
                foreach (var i in page.Value)
                {
                    var item = i;

                    if (!includeOpenEndedResults)
                    {
                        //skip open-ended
                        if (item != null && isOpenEndedItem(item.ItemTypeName))
                            continue;
                    }

                    //check if the item was answered, get it response data, not template data
                    if (!onlyAnswered)
                    {
                        var answeredItem = answeredPageData.SelectMany(p => p.Value).FirstOrDefault(p => p.ID == i.ID);
                        if (answeredItem != null)
                            item = answeredItem;
                    }

                    var surveyItem = item.GetDataTransferObject() as SurveyResponseItem;

                    if (surveyItem == null)
                        continue;

                    if (surveyItem.AdditionalData is MatrixAdditionalData)
                    {
                        var matrixData = (MatrixAdditionalData)surveyItem.AdditionalData;

                        foreach (var row in matrixData.Rows)
                        {
                            itemAnswers.AddRange(from column in matrixData.Columns
                                                 let child = matrixData.ChildItems[row.RowNumber - 1][column.ColumnNumber - 1] as SurveyResponseItem
                                                 where child != null/* && child.IsAnswerable*/
                                                 select new ResponseItemAnswerData
                                                 {
                                                     ItemId = child.ItemId,
                                                     PageId = page.Key,
                                                     Alias = GetResponseMatrixItemAnswerDataAlias((MatrixItem)item, row, column),
                                                     Text = GetResponseItemAnswerDataText(surveyItem, row, column, stripHTMLTagsFromQuestions),
                                                     Description = Utilities.StripHtml(surveyItem.Description),
                                                     Answers = GetResponseItemAnswers(child.Answers, child, true, includeAliases)
                                                 });
                        }
                    }
                    else
                    {
                        var newAnswer = new ResponseItemAnswerData
                        {
                            ItemId = item.ID,
                            PageId = page.Key,
                            Alias = string.IsNullOrEmpty(item.Alias) ? Utilities.StripHtml(surveyItem.Text) : item.Alias,
                            Text = stripHTMLTagsFromQuestions ? Utilities.StripHtml(surveyItem.Text) : surveyItem.Text,
                            Description = Utilities.StripHtml(surveyItem.Description),
                            Answers = GetResponseItemAnswers(surveyItem.Answers, surveyItem, item is ScoredItem, includeAliases)
                        };

                        itemAnswers.Add(newAnswer);
                    }
                }
            }

            return itemAnswers.ToArray();
        }

        private static string GetResponseMatrixItemAnswerDataAlias(MatrixItem matrixItem, MatrixRowInfo row, MatrixColumnInfo column)
        {
            return GetResponseMatrixItemAnswerDataAlias(matrixItem, row.Text, row.Alias, column.ColumnNumber, column.Text, column.Alias);
        }

        private static string GetResponseMatrixItemAnswerDataAlias(MatrixItem matrixItem, MatrixItemRow row, MatrixItemColumn column)
        {
            return GetResponseMatrixItemAnswerDataAlias(matrixItem, row.Text, row.Alias, column.ColumnNumber, column.HeaderText, column.Alias);
        }

        private static string GetResponseMatrixItemAnswerDataAlias(MatrixItem matrixItem, 
            string rowText, string rowAlias, int columnNumber, string columnText, string columnAlias)
        {
            //Get the row text
            string result = matrixItem.Alias;

            if (string.IsNullOrEmpty(result))
                result = matrixItem.Text;

            if (!string.IsNullOrEmpty(rowAlias))
                result += "_" + rowAlias;
            else if (!string.IsNullOrEmpty(rowText))
                result += "_" + rowText;

            if (!string.IsNullOrEmpty(columnAlias))
                result += "_" + columnAlias;
            else if (!string.IsNullOrEmpty(columnText))
                result += "_" + columnText;
            else
                result += "_Column" + columnNumber;

            return Utilities.StripHtml(result);
        }

        private static string GetResponseItemAnswerDataText(SurveyResponseItem surveyItem, MatrixItemRow row, MatrixItemColumn column, bool stripHTML)
        {
            //Get the row text
            string result = surveyItem.Text;

            if (!string.IsNullOrEmpty(row.Text))
                result += "_" + row.Text;
            else if (!string.IsNullOrEmpty(row.Alias))
                result += "_" + row.Alias;

            if (!string.IsNullOrEmpty(column.HeaderText))
                result += "_" + column.HeaderText;
            else if (!string.IsNullOrEmpty(column.Alias))
                result += "_" + column.Alias;
            else
                result += "_Column" + column.ColumnNumber;

            return stripHTML ? Utilities.StripHtml(result) : result;
        }

        private static ResponseItemAnswer[] GetResponseItemAnswers(IEnumerable<SurveyResponseItemAnswer> surveyResponseItemAnswer, SurveyResponseItem item, bool isScored, bool includeAliases)
        {
            return (from answer in surveyResponseItemAnswer
                    select new ResponseItemAnswer
                               {
                                   AnswerId = answer.AnswerId, 
                                   Alias = answer.Alias, 
                                   OptionId = answer.OptionId, 
                                   AnswerText = answer.AnswerText, 
                                   Points = GetPoints(answer, item), 
                                   OptionText = GetOptionText(answer, item, includeAliases),
                                   IsScored = isScored
                    }).ToArray();
        }

        private static string GetOptionText(SurveyResponseItemAnswer answer, SurveyResponseItem item, bool includeAliases)
        {
            var text = string.Empty;
            var option = item.Options.FirstOrDefault(opt => opt.OptionId == answer.OptionId);

            if (option != null)
            {
                switch (item.TypeName)
                {
                    case "Slider":
                        text = "Image".Equals(item.InstanceData["ValueType"], StringComparison.InvariantCultureIgnoreCase)
                            ? option.Alias
                            : option.Text;
                        break;
                    case "RadioButtonScale":
                        text = option.Text;
                        break;
                    default:
                        text = includeAliases && !string.IsNullOrEmpty(option.Alias) ? option.Alias : option.Text;
                        break;
                }
            }

            return text;
        }

        private static double GetPoints(SurveyResponseItemAnswer answer, SurveyResponseItem item)
        {
            var points = 0d;
            var option = item.Options.FirstOrDefault(opt => opt.OptionId == answer.OptionId);

            if (option != null)
                points = option.Points;
            else if (item.TypeName.Equals("Slider", StringComparison.InvariantCultureIgnoreCase) &&
                "NumberRange".Equals(item.InstanceData["ValueType"], StringComparison.InvariantCultureIgnoreCase))
                double.TryParse(answer.AnswerText, out points);

            return points;
        }

        /// <summary>
        /// Collect answer data
        /// </summary>
        /// <param name="responseFieldCollection"></param>
        /// <param name="answers"></param>
        /// <param name="itemDatas"></param>
        /// <param name="itemPrototypes"></param>
        /// <param name="itemRows"></param>
        /// <param name="stripHTMLTagsFromQuestions"></param>
        /// <param name="stripHTMLTagsFromAnswers"></param>
        /// <param name="includeAliases"></param>
        /// <param name="mergeAnswersForSelectMany"></param>
        /// <param name="language"></param>
        /// <param name="surveyId"></param>
        /// <param name="includeOpenEndedResults"></param>
        private static void InsertAnswerDataToNVC(SimpleNameValueCollection responseFieldCollection, ResponseItemAnswerData[] answers,
            Dictionary<int, ItemData> itemDatas, Dictionary<int, ItemData> itemPrototypes,
            Dictionary<int, int> itemRows,
            bool stripHTMLTagsFromQuestions, bool stripHTMLTagsFromAnswers,
            bool includeAliases, bool mergeAnswersForSelectMany, string language, int surveyId, bool includeOpenEndedResults)
        {
            foreach (var key in itemDatas.Keys)
            {
                var itemAnswers = (from a in answers where a.ItemId == key select a).FirstOrDefault();

                var itemData = itemDatas[key];

                if (itemData == null || !itemData.ItemIsIAnswerable || itemAnswers == null)
                    continue;

                //cut off open-ended
                if (!includeOpenEndedResults && isOpenEndedItem(itemData.ItemTypeName))
                    continue;

                var answerData = answers.FirstOrDefault(a => a.ItemId == itemData.ID);

                string itemText;
                if (answerData != null)
                    itemText = includeAliases ? answerData.Alias : answerData.Text;
                else
                    itemText = getItemText(itemData, itemPrototypes.ContainsKey(itemData.ID.Value) ? itemPrototypes[itemData.ID.Value] : null,
                        itemRows.ContainsKey(itemData.ID.Value) ? itemRows[itemData.ID.Value] : 0, language, includeAliases, stripHTMLTagsFromQuestions, surveyId);

                // that's better to use polymorphism here    
                if (itemData is SelectManyData)
                {
                    SelectManyData sid = (SelectManyData)itemData;
                    if (mergeAnswersForSelectMany)
                    {
                        var answer = itemAnswers == null ? string.Empty : string.Join(",", 
                            (from a in itemAnswers.Answers select string.IsNullOrEmpty(a.AnswerText)
                                ? getOptionText(a, includeAliases, stripHTMLTagsFromAnswers) 
                                : a.AnswerText).ToArray());

                        responseFieldCollection.Add(itemText, answer);
                    }
                    else
                    {
                        var options = sid.Options;
                        if (itemPrototypes.ContainsKey(itemData.ID.Value))
                            options = ((SelectManyData)itemPrototypes[itemData.ID.Value]).Options;

                        foreach (var o in options)
                        {
                            var answer = itemAnswers == null ? null : (from a in itemAnswers.Answers where a.OptionId == o.OptionID select a).FirstOrDefault();
                            string optionText = answer == null 
                                ? getOptionText(o, language, includeAliases, stripHTMLTagsFromAnswers) 
                                : getOptionText(answer, includeAliases, stripHTMLTagsFromAnswers);
                            string answerText = (o.IsOther && answer != null) ? answer.AnswerText : answer != null ? "1" : "0";

                            responseFieldCollection.Add(string.Format("{0}_{1}", itemText, optionText), answer == null ? "0" : answerText);
                        }
                    }
                }
                else if (itemData is RankOrderItemData)
                {
                    RankOrderItemData sid = (RankOrderItemData)itemData;
                    var options = sid.Options;
                    if (itemPrototypes.ContainsKey(itemData.ID.Value))
                    {
                        options = ((SelectManyData)itemPrototypes[itemData.ID.Value]).Options;
                    }

                    foreach (var o in options)
                    {
                        string optionText = null;
                        var answer = answers.SelectMany(a => a.Answers).FirstOrDefault(op => op.OptionId == o.OptionID);
                                // (a => a).FirstOrDefault(o => o.) //= itemAnswers == null ? null : (from a in itemAnswers.Answers where a.OptionId == o.OptionID select a).FirstOrDefault();
                            
                        var optionPoints = "";
                        if (answer == null)
                        {
                            optionText = getOptionText(o, language, includeAliases, stripHTMLTagsFromAnswers);
                        }
                        else
                        {
                            optionPoints = ((int)answer.Points).ToString();
                            optionText = stripHTMLTagsFromAnswers ? Utilities.StripHtmlTags(answer.OptionText) : answer.OptionText;
                        }
                        responseFieldCollection.Add(string.Format("{0}_{1}", itemText, optionText), optionPoints);
                    }
                }
                else if (itemData is SliderItemData)
                {
                    var answerText = string.Empty;
                    if (itemAnswers != null && itemAnswers.Answers.Length > 0)
                    {
                        var answer = itemAnswers.Answers[0];
                        answerText = answer.OptionId.HasValue ? getOptionText(answer, includeAliases, stripHTMLTagsFromAnswers) : answer.AnswerText;
                    }
                    responseFieldCollection.Add(itemText, answerText);
                }
                else if (itemData is RatingScaleItemData)
                {
                    var answer = itemAnswers == null ? null : itemAnswers.Answers.FirstOrDefault();
                    var optionText = answer != null ? answer.OptionText : string.Empty;

                    responseFieldCollection.Add(itemText, optionText);
                }
                else if (itemData is SelectItemData)
                {
                    var answer = itemAnswers == null ? null : itemAnswers.Answers.FirstOrDefault();
                    var optionText = answer != null
                        ? getOptionText(answer, includeAliases, stripHTMLTagsFromAnswers)
                        : string.Empty;

                    var options = (itemData as SelectItemData).Options;
                    var selectedOption = answer != null ? options.FirstOrDefault(o => o.OptionID == answer.OptionId) : null;
                    string answerText = (selectedOption != null && selectedOption.IsOther) ? answer.AnswerText : optionText;
                    responseFieldCollection.Add(itemText, answerText);
                }
                else
                {
                    string answerText = itemAnswers.Answers.Length > 0 ? itemAnswers.Answers[0].AnswerText : string.Empty;

                    responseFieldCollection.Add(itemText, itemAnswers == null ? null :
                        (stripHTMLTagsFromAnswers ? Utilities.StripHtmlTags(answerText) : answerText));
                }
            }
        }
        /// <summary>
        /// Get text for the option
        /// </summary>
        /// <param name="answer"></param>
        /// <param name="IncludeAliases"></param>
        /// <param name="StripHTMLTagsFromAnswers"></param>
        /// <returns></returns>
        private static string getOptionText(ResponseItemAnswer answer, bool IncludeAliases, bool StripHTMLTagsFromAnswers)
        {
            string optionText = string.Empty;

            if (IncludeAliases)
                optionText = answer.Alias;

            if (string.IsNullOrEmpty(optionText))
                optionText = answer.OptionText;

            if (string.IsNullOrEmpty(optionText))
                optionText = string.Format("Option_{0}", answer.OptionId);

            return StripHTMLTagsFromAnswers ? Utilities.StripHtmlTags(optionText) : optionText;
        }

        /// <summary>
        /// Get text for the option
        /// </summary>
        /// <param name="optionData"></param>
        /// <param name="Language"></param>
        /// <param name="IncludeAliases"></param>
        /// <param name="StripHTMLTagsFromAnswers"></param>
        /// <returns></returns>
        private static string getOptionText(ListOptionData optionData, string Language, bool IncludeAliases, bool StripHTMLTagsFromAnswers)
        {
            string optionText = string.Empty;

            if (IncludeAliases)
                optionText = optionData.Alias;

            if (string.IsNullOrEmpty(optionText))
                optionText = WebTextManager.GetText(optionData.TextID, Language);

            if (string.IsNullOrEmpty(optionText))
                optionText = string.Format("Option_{0}", optionData.OptionID);

            return StripHTMLTagsFromAnswers ? Utilities.StripHtmlTags(optionText) : optionText;
        }

        /// <summary>
        /// Get text for the item
        /// </summary>
        /// <returns></returns>
        private static string getItemText(ItemData itemData, ItemData prototype, int row, 
            string language, bool includeAliases, bool stripHTMLTagsFromQuestions, int surveyId)
        {
            if (includeAliases)
            {
                //if this is a matrix child
                if (itemData.ParentItemId.HasValue && itemData.ID.HasValue)
                {
                    var matrixData = ItemConfigurationManager.GetConfigurationData(itemData.ParentItemId.Value) as MatrixItemData;
                    if (matrixData != null)
                    {
                        var matrix = matrixData.CreateItem(language, surveyId) as MatrixItem;
                        if (matrix != null)
                        {
                            var col = matrixData.GetItemCoordinate(itemData.ID.Value).Column;

                            return GetResponseMatrixItemAnswerDataAlias(matrix, matrix.GetRowInfo(row), matrix.GetColumnInfo(col));                            
                        }
                    }
                }

                if (prototype != null && !string.IsNullOrEmpty(prototype.Alias))
                    return prototype.Alias + (row > 0 ? string.Format("_{0}", row) : "");

                if (!string.IsNullOrEmpty(itemData.Alias))
                    return itemData.Alias;
            }

            string text = itemData is LabelledItemData ? WebTextManager.GetText(
                prototype == null ? ((LabelledItemData)itemData).TextID : ((LabelledItemData)prototype).TextID, language) : "";

            if (string.IsNullOrEmpty(text))
                text = string.Format("Item_{0}", itemData.ID);

            if (row > 0)
                text += string.Format("_{0}", row);

            return stripHTMLTagsFromQuestions ? Utilities.StripHtmlTags(text) : text;
        }

        /// <summary>
        /// Collect metadata fro items if not collected before
        /// </summary>
        private static void UnsureItemMetadataCollected(ref Dictionary<int, ItemData> itemDatas, ref Dictionary<int, ItemData> itemPrototypes, 
            ref Dictionary<int, int> itemRows, int surveyId)
        {
            if (itemDatas != null)
                return;

            itemDatas = new Dictionary<int, ItemData>();
            itemPrototypes = new Dictionary<int, ItemData>();
            itemRows = new Dictionary<int, int>();

            var rt = ResponseTemplateManager.GetResponseTemplate(surveyId);

            //Add all items in the response template
            foreach (var templatePageId in rt.ListTemplatePageIds())
            {
                var templatePage = rt.GetPage(templatePageId);

                foreach (var itemId in templatePage.ListItemIds())
                {
                    var itemData = ItemConfigurationManager.GetConfigurationData(itemId, true);

                    if (itemData is MatrixItemData)
                    {
                        MatrixItemData matrixItemData = (MatrixItemData)itemData;

                        //Iterate matrix columns
                        for (int column = 1; column <= matrixItemData.ColumnCount; column++)
                        {
                            var protoId = matrixItemData.GetColumnPrototypeId(column);
                            var proto = ItemConfigurationManager.GetConfigurationData(protoId, true);

                            //Iterate matrix rows
                            for (int row = 1; row <= matrixItemData.RowCount; row++)
                            {
                                var childItemId = matrixItemData.GetItemIdAt(row, column);
                                if (!childItemId.HasValue)
                                    continue;

                                var i = ItemConfigurationManager.GetConfigurationData(childItemId.Value, true);
                                if (i == null)
                                    continue;

                                //save matrix id to child
                                i.ParentItemId = matrixItemData.ID;

                                itemDatas[childItemId.Value] = i;
                                itemPrototypes[childItemId.Value] = proto;
                                itemRows[childItemId.Value] = row;
                            }
                        }

                    }
                    else
                    {
                        itemDatas[itemId] = itemData;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="responseFieldCollection"></param>
        /// <param name="responseData"></param>
        private static void insertResponseDataToNVC(SimpleNameValueCollection responseFieldCollection, ResponseData responseData)
        {
            responseFieldCollection["Guid"] = responseData.Guid.ToString();
            responseFieldCollection["UserIdentifier"] = responseData.UserIdentifier;
            responseFieldCollection["CompletionDate"] = responseData.CompletionDate.HasValue ? responseData.CompletionDate.ToString() : "";
            responseFieldCollection["RespondentIp"] = responseData.RespondentIp;
            responseFieldCollection["ResponseLanguage"] = responseData.ResponseLanguage;
            responseFieldCollection["NetworkUser"] = responseData.NetworkUser;
            responseFieldCollection["LastEditDate"] = responseData.LastEditDate.HasValue ? responseData.LastEditDate.ToString() : "";
            responseFieldCollection["Started"] = responseData.Started.HasValue ? responseData.Started.ToString() : "";
            responseFieldCollection["Id"] = responseData.Id.ToString();
            responseFieldCollection["LastPageViewed"] = responseData.Started.HasValue ? responseData.LastPageViewed.ToString() : "";
            responseFieldCollection["AnonymousRespondentGuid"] = responseData.AnonymousRespondentGuid.HasValue ? responseData.AnonymousRespondentGuid.ToString() : "";
            responseFieldCollection["Invitee"] = responseData.Invitee;
            responseFieldCollection["IsAnonymized"] = responseData.IsAnonymized.ToString();
            responseFieldCollection["IsTest"] = responseData.IsTest.ToString();
            responseFieldCollection["WorkflowSessionId"] = responseData.WorkflowSessionId.HasValue ? responseData.WorkflowSessionId.ToString() : "";
            responseFieldCollection["SurveyName"] = responseData.SurveyName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="responseData"></param>
        private static void convertResponseDatesToClientTimeZone(ResponseData responseData)
        {
            responseData.Started = WebUtilities.ConvertToClientTimeZone(responseData.Started);
            responseData.CompletionDate = WebUtilities.ConvertToClientTimeZone(responseData.CompletionDate);
            responseData.LastEditDate = WebUtilities.ConvertToClientTimeZone(responseData.LastEditDate);
        }


        private static void ApplyAliasOnOptions(IEnumerable<ResponseItemAnswerData> answers)
        {
            foreach (var a in answers)
            {
                if (string.IsNullOrEmpty(a.Alias))
                    a.Alias = a.Text;

                foreach (var ia in a.Answers)
                {
                    if (string.IsNullOrEmpty(ia.Alias))
                        ia.Alias = ia.OptionText;
                }
            }
        }

        /// <summary>
        /// Define that the item may have an open ended result
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        private static bool isOpenEndedItem(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
                return false;
            if (openEndedTypes.Contains(typeName))
                return true;

            return false;
        }

#endregion
    }
}
