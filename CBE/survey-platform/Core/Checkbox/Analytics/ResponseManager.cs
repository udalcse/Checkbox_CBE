using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Checkbox.Common;
using Checkbox.Forms.Security.Principal;
using Checkbox.Management;
using Checkbox.Pagination;
using Checkbox.Security;
using Checkbox.Security.Principal;
using Checkbox.Wcf.Services.Proxies;
using Prezza.Framework.Data;

namespace Checkbox.Analytics
{
    /// <summary>
    /// Utility class for managing survey responses, including getting paged/filtered lists of responses for a survey, deleting responses, and counting responses.
    /// </summary>
    public static class ResponseManager
    {
        ///  <summary>
        ///  Return a <see cref="DataTable"/> listing all survey responses that match the specified criteria.
        ///  </summary>
        ///  <param name="responseTemplateID">ID of the <see cref="Checkbox.Forms.ResponseTemplate"/> to list responses floor.</param>
        /// <param name="includeComplete"></param>
        /// <param name="includeIncomplete">Flag indicating if incomplete responses should be included in the list.</param>
        /// <param name="includeTest"></param>
        /// <param name="context"></param>
        /// <param name="minResponseCompletedDate"></param>
        /// <param name="maxResponseCompletedDate"></param>
        /// <param name="profileFieldId"></param>
        /// <returns>The following columns are contained the <see cref="DataTable"/> returned by this query:
        ///      ResponseID              -- bigint               -- ID of the response
        ///      GUID                    -- uniqueidentifier     -- GUID associated with the response.
        ///      ResponseTemplateID      -- int                  -- Survey response is assciated with.
        ///      IsComplete              -- bit                  -- Flag indicating if the response is complete.
        ///      LastPageViewed          -- int                  -- Database ID of last page viewed by respondent.
        ///      Started                 -- datetime             -- Timestamp response was started.
        ///      Ended                   -- datetime             -- Timestamp response was completed.
        ///      IP                      -- varchar              -- IP Address of the respondent.
        ///      LastEdit                -- datetime             -- Timestamp marking when response was last saved.
        ///      NetworkUser             -- varchar              -- AD login of the network user principal 
        ///      Language                -- varchar              -- Language code for the response
        ///      UniqueIdentifier        -- varchar              -- Unique identifier of user taking the survey.
        ///      IsTest                  -- bit                  -- Flag indicating if the response was made in test mode.
        /// </returns>
        /// <remarks>Filtering is done using a LIKE %FilterValue% text comparison, so attempting to filter on numeric fields may not work as expected.</remarks>
        public static List<ResponseData> GetResponseList(Int32? responseTemplateID, bool includeIncomplete, bool includeComplete, bool includeTest,
            PaginationContext context, DateTime? minResponseCompletedDate = null, DateTime? maxResponseCompletedDate = null, int profileFieldId = 0)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Response_List");
            command.AddInParameter("ResponseTemplateId", DbType.Int32, responseTemplateID);
            command.AddInParameter("IncludeComplete", DbType.Boolean, includeComplete);
            command.AddInParameter("IncludeIncomplete", DbType.Boolean, includeIncomplete);
            command.AddInParameter("IncludeTest", DbType.Boolean, includeTest);
            command.AddInParameter("MinResponseCompletedDate", DbType.DateTime, minResponseCompletedDate);
            command.AddInParameter("MaxResponseCompletedDate", DbType.DateTime, maxResponseCompletedDate);
            command.AddInParameter("ProfileFieldId", DbType.Int32, profileFieldId);

            QueryHelper.AddPagingAndFilteringToCommandWrapper(command, context);

            using (var reader = db.ExecuteReader(command))
            {
                try
                {
                    var responseList = BuildResponseList(reader);

                    //Attempt to get total count
                    if (reader.NextResult() && reader.Read())
                    {
                        context.ItemCount = DbUtility.GetValueFromDataReader(reader, "TotalItemCount", -1);
                    }

                    return responseList;
                }
                finally
                {
                    reader.Close();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="responseTemplateId"></param>
        /// <param name="anonymizeResponses"></param>
        /// <returns></returns>
        public static List<ResponseData> ListResponsesForRespondent(CheckboxPrincipal userPrincipal, int responseTemplateId, bool anonymizeResponses)
        {
            return
                GetResponseList(
                    responseTemplateId,
                    true,
                    true,
                    true,
                    GetPaginationContextForRespondent(userPrincipal, anonymizeResponses),
                    null,
                    null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="anonymizeResponses"></param>
        /// <returns></returns>
        private static PaginationContext GetPaginationContextForRespondent(CheckboxPrincipal userPrincipal, bool anonymizeResponses)
        {
            var paginationContext = new PaginationContext();

            if (userPrincipal is AnonymousRespondent)
            {
                paginationContext.FilterField = "RespondentGuid";
                paginationContext.FilterValue = userPrincipal.UserGuid.ToString();
            }
            else
            {
                if (anonymizeResponses)
                {
                    paginationContext.FilterField = "ResumeKey";
                    paginationContext.FilterValue = Utilities.GetSaltedMd5Hash(userPrincipal.Identity.Name);
                }
                else
                {
                    paginationContext.FilterField = "UniqueIdentifier";
                    paginationContext.FilterValue = userPrincipal.Identity.Name;
                }
            }

            return paginationContext;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="responseTemplateID"></param>
        /// <param name="includeIncomplete"></param>
        /// <param name="includeComplete"></param>
        /// <param name="context"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static List<ResponseData> GetTestResponseList(Int32 responseTemplateID, bool includeIncomplete, bool includeComplete, PaginationContext context, DateTime? startDate, DateTime? endDate)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Response_ListTest");
            command.AddInParameter("ResponseTemplateId", DbType.Int32, responseTemplateID);
            command.AddInParameter("IncludeComplete", DbType.Boolean, includeComplete);
            command.AddInParameter("IncludeIncomplete", DbType.Boolean, includeIncomplete);
            command.AddInParameter("MinResponseCompletedDate", DbType.DateTime, startDate);
            command.AddInParameter("MaxResponseCompletedDate", DbType.DateTime, endDate);

            QueryHelper.AddPagingAndFilteringToCommandWrapper(command, context);

            using (var reader = db.ExecuteReader(command))
            {
                try
                {
                    var responseList = BuildResponseList(reader);

                    //Attempt to get total count
                    if (reader.NextResult() && reader.Read())
                    {
                        context.ItemCount = DbUtility.GetValueFromDataReader(reader, "TotalItemCount", -1);
                    }

                    return responseList;
                }
                finally
                {
                    reader.Close();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static int GetPageNumberByResponseId(long responseId, Int32 responseTemplateID, 
            bool includeIncomplete, DateTime? startDate, DateTime? endDate, int resultsPerPage, 
            string sortField, bool sortAscending, string filterField, string filterValue)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Response_GetPageByResponseId");
            command.AddInParameter("ResponseId", DbType.Int64, responseId);
            command.AddInParameter("ResponseTemplateId", DbType.Int32, responseTemplateID);
            command.AddInParameter("IncludeIncomplete", DbType.Boolean, includeIncomplete);
            command.AddInParameter("MinResponseCompletedDate", DbType.DateTime, startDate);
            command.AddInParameter("MaxResponseCompletedDate", DbType.DateTime, endDate);
            command.AddInParameter("ResultsPerPage", DbType.Int32, resultsPerPage);
            command.AddInParameter("SortField", DbType.String, sortField);
            command.AddInParameter("SortAscending", DbType.Boolean, sortAscending);
            command.AddInParameter("FilterField", DbType.String, filterField);
            command.AddInParameter("FilterValue", DbType.String, filterValue);

            var result = db.ExecuteScalar(command);
            if (result is int)
                return (int)result;

            return 1;
        }

        /// <summary>
        /// 
        /// </summary>
        public static int GetPageNumberByResponseId(long responseId, bool includeIncomplete, 
            DateTime? startDate, DateTime? endDate, int resultsPerPage, string sortField,
            bool sortAscending, string filterField, string filterValue)
        {
            var surveyId = GetSurveyIdFromResponseId(responseId);

            if (surveyId.HasValue)
            {
                return GetPageNumberByResponseId(responseId, surveyId.Value, includeIncomplete, startDate, endDate,
                    resultsPerPage, sortField, sortAscending, filterField, filterValue);
            }

            return 1;
        }

        /// <summary>
        /// Get a list of responses for a specific survey invitation.
        /// </summary>
        /// <param name="invitationId"></param>
        /// <param name="includeIncomplete"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static List<ResponseData> GetResponseListForInvitation(Int32 invitationId, bool includeIncomplete, PaginationContext context)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Response_ListForInvitation");
            command.AddInParameter("InvitationId", DbType.Int32, invitationId);
            command.AddInParameter("IncludeIncomplete", DbType.Boolean, includeIncomplete);

            QueryHelper.AddPagingAndFilteringToCommandWrapper(command, context);

            using (var reader = db.ExecuteReader(command))
            {
                try
                {
                    var responseList = BuildResponseList(reader);

                    //Attempt to get total count
                    if (reader.NextResult() && reader.Read())
                    {
                        context.ItemCount = DbUtility.GetValueFromDataReader(reader, "TotalItemCount", -1);
                    }

                    return responseList;
                }
                finally
                {
                    reader.Close();
                }
            }
        }

        /// <summary>
        /// Build response list from data reader
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static List<ResponseData> BuildResponseList(IDataReader reader)
        {
            var responseList = new List<ResponseData>();

            while (reader.Read())
            {
                responseList.Add(new ResponseData
                {
                    RespondentIp = ApplicationManager.AppSettings.LogIpAddresses ? DbUtility.GetValueFromDataReader(reader, "IP", string.Empty) : string.Empty,
                    AnonymousRespondentGuid = DbUtility.GetValueFromDataReader(reader, "RespondentGUID", Guid.Empty),
                    CompletionDate = DbUtility.GetValueFromDataReader(reader, "Ended", (DateTime?)null),
                    Guid = DbUtility.GetValueFromDataReader(reader, "GUID", Guid.Empty),
                    Id = DbUtility.GetValueFromDataReader(reader, "ResponseId", (long)0),
                    LastEditDate = DbUtility.GetValueFromDataReader(reader, "LastEdit", DateTime.MinValue),
                    LastPageViewed = DbUtility.GetValueFromDataReader(reader, "LastPageViewed", -1),
                    NetworkUser = ApplicationManager.AppSettings.LogNetworkUser ? DbUtility.GetValueFromDataReader(reader, "NetworkUser", string.Empty) : string.Empty,
                    ResponseLanguage = DbUtility.GetValueFromDataReader(reader, "Language", string.Empty),
                    Started = DbUtility.GetValueFromDataReader(reader, "Started", DateTime.MinValue),
                    UserIdentifier =  DbUtility.GetValueFromDataReader(reader, "UniqueIdentifier", string.Empty),
                    IsTest =  DbUtility.GetValueFromDataReader(reader, "IsTest", DbUtility.GetValueFromDataReader(reader, "IsTest", false)),
                    Invitee = DbUtility.GetValueFromDataReader(reader, "Invitee", string.Empty),
                    IsAnonymized = DbUtility.GetValueFromDataReader(reader, "IsAnonymized", false),
                    WorkflowSessionId = DbUtility.GetValueFromDataReader(reader, "SessionId", (Guid?)null),
                    SurveyName = DbUtility.GetValueFromDataReader(reader, "TemplateName", string.Empty),
                });
            }

            return responseList;
        }

        /// <summary>
        /// Return a boolean indicating if the response template has any non-test (complete or not) 
        /// responses.
        /// </summary>
        /// <param name="responseTemplateId"></param>
        /// <returns></returns>
        public static bool ResponseTemplateHasNonTestResponses(int responseTemplateId)
        {
            return (GetResponseCount(responseTemplateId, true, true) - GetResponseCount(responseTemplateId, true, false)) > 0;
        }

        /// <summary>
        /// Count the number of responses matching the specified filter information.
        /// </summary>
        /// <param name="responseTemplateID">ID of the <see cref="Checkbox.Forms.ResponseTemplate"/> to list responses floor.</param>
        /// <param name="includeIncomplete">Flag indicating if incomplete responses should be included in the list.</param>
        /// <param name="includeTest">Indicate whether to include "Test" responses in count.</param>
        /// <returns>Count of the number of responses matching the filter.</returns>
        /// <remarks>This method is mostly useful for user intefaces that display paged lists of respones where the number of matching
        /// responses is needed to calculate the total number of pages of results.</remarks>
        public static Int32 GetResponseCount(Int32 responseTemplateID, bool includeIncomplete, bool includeTest)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Response_Count");
            command.AddInParameter("ResponseTemplateId", DbType.Int32, responseTemplateID);
            command.AddInParameter("IncludeIncomplete", DbType.Boolean, includeIncomplete);
            command.AddInParameter("IncludeTest", DbType.Boolean, includeTest);
            command.AddOutParameter("TotalCount", DbType.Int32, 4);

            db.ExecuteNonQuery(command);

            object value = command.GetParameterValue("TotalCount");

            if (value != null
                && value != DBNull.Value)
            {
                return (int)value;
            }

            return 0;
        }

        /// <summary>
        /// Delete the responses specified in the input list.
        /// </summary>
        /// <param name="responseIDs">IDs corresponding to responses to delete.</param>
        /// <remarks>Responses are "soft deleted" from the database, meaning they are flagged as deleted, but rows are not removed from the ckbx_Response
        /// or ckbx_ResponseAnswers tables.</remarks>
        public static void DeleteResponses(List<long> responseIDs)
        {
            Database db = DatabaseFactory.CreateDatabase();

            foreach (Int32 responseID in responseIDs)
            {
                DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Response_Delete");
                command.AddInParameter("ResponseID", DbType.Int64, responseID);

                db.ExecuteNonQuery(command);
            }
        }

        /// <summary>
        /// Delete all responses for the specified survey.
        /// </summary>
        /// <param name="responseTemplateID">ID of survey to delete responses for.</param>
        /// <remarks>Responses are "soft deleted" from the database, meaning they are flagged as deleted, but rows are not removed from the ckbx_Response
        /// or ckbx_ResponseAnswers tables.</remarks>
        public static void DeleteAllResponses(Int32 responseTemplateID)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Response_DeleteForRT");
            command.AddInParameter("ResponseTemplateID", DbType.Int32, responseTemplateID);

            db.ExecuteNonQuery(command);
        }

        /// <summary>
        /// Delete all test responses for the specified survey.
        /// </summary>
        /// <param name="responseTemplateID">ID of survey to delete responses for.</param>
        /// <remarks>Responses are "soft deleted" from the database, meaning they are flagged as deleted, but rows are not removed from the ckbx_Response
        /// or ckbx_ResponseAnswers tables.</remarks>
        public static void DeleteAllTestResponses(Int32 responseTemplateID)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_RT_DeleteTestResponses");
            command.AddInParameter("ResponseTemplateID", DbType.Int32, responseTemplateID);

            db.ExecuteNonQuery(command);
        }

        /// <summary>
        /// Delete all responses for the specified user.
        /// </summary>
        /// <param name="uniqueIdentifier">Unique identifier of the user.</param>
        /// <remarks>Responses are "soft deleted" from the database, meaning they are flagged as deleted, but rows are not removed from the ckbx_Response
        /// or ckbx_ResponseAnswers tables.</remarks>
        public static void DeleteUserResponses(string uniqueIdentifier)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_RT_DeleteUserResponses");
            command.AddInParameter("UserUniqueIdentifier", DbType.String, uniqueIdentifier);

            db.ExecuteNonQuery(command);
        }

        ///<summary>
        /// Check if the answer belongs to the response
        ///</summary>
        ///<param name="responseId">Response Id</param>
        ///<param name="answerId">Answer Id</param>
        ///<returns></returns>
        public static bool DoesAnswerBelongToResponse(long responseId, long answerId)
        {
            var answers = ListResponseAnswers(responseId);
            return answers.Tables[0].Rows.Cast<DataRow>().Any(row => (long) row["AnswerID"] == answerId);
        }

        /// <summary>
        /// List all answers for a given response.
        /// </summary>
        /// <param name="responseId">Id of response to list answers for.</param>
        public static DataSet ListResponseAnswers(long responseId)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Response_ListAnswersForId");
            command.AddInParameter("ResponseId", DbType.Int64, responseId);

            return db.ExecuteDataSet(command);
        }

        /// <summary>
        /// List all answers for a given response.
        /// </summary>
        /// <param name="responseGuid">Guid of response to list answers for.</param>
        public static DataSet ListResponseAnswers(Guid responseGuid)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Response_ListAnswersForGuid");
            command.AddInParameter("ResponseGuid", DbType.Guid, responseGuid);

            return db.ExecuteDataSet(command);
        }

        /// <summary>
        /// Updates response answers
        /// </summary>
        /// <param name="answerId"></param>
        /// <param name="answerText"></param>
        /// <param name="optionID"></param>
        /// <param name="dateCreated"></param>
        /// <returns></returns>
        public static void UpdateResponseAnswer(long answerId, string answerText, int? optionID, DateTime? dateCreated)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper updateAnswer = db.GetStoredProcCommandWrapper("ckbx_sp_Response_UpdateAnswer");
            updateAnswer.AddInParameter("AnswerID", DbType.Int64, answerId);
            updateAnswer.AddInParameter("AnswerText", DbType.String, answerText);
            updateAnswer.AddInParameter("OptionID", DbType.Int32, optionID);
            updateAnswer.AddInParameter("DateCreated", DbType.DateTime, dateCreated);

            db.ExecuteNonQuery(updateAnswer);
        }

        /// <summary>
        /// Get a row of data for a specific response.
        /// </summary>
        /// <param name="responseId">Id of response to get data row for.</param>
        /// <returns></returns>
        public static DataRow GetResponseDataRow(long responseId)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Response_GetDataRowForId");
            command.AddInParameter("ResponseId", DbType.Int64, responseId);

            DataSet ds = db.ExecuteDataSet(command);

            if (ds != null && ds.Tables.Count == 1 && ds.Tables[0].Rows.Count == 1)
            {
                return ds.Tables[0].Rows[0];
            }

            return null;
        }

        /// <summary>
        /// Get a row of data for a specific response.
        /// </summary>
        /// <param name="responseGuid">Guid of response to get data row for.</param>
        /// <returns></returns>
        public static DataRow GetResponseDataRow(Guid responseGuid)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Response_GetDataRowForGuid");
            command.AddInParameter("ResponseGuid", DbType.Guid, responseGuid);

            DataSet ds = db.ExecuteDataSet(command);

            if (ds != null && ds.Tables.Count == 1 && ds.Tables[0].Rows.Count == 1)
            {
                return ds.Tables[0].Rows[0];
            }

            return null;
        }

        /// <summary>
        /// Get the id of a survey based on the id of a response.
        /// </summary>
        /// <param name="responseId"></param>
        /// <returns></returns>
        public static int? GetSurveyIdFromResponseId(long responseId)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Response_GetSurveyIdFromResponseId");
            command.AddInParameter("ResponseId", DbType.Int64, responseId);
            command.AddOutParameter("SurveyId", DbType.Int32, 4);

            db.ExecuteNonQuery(command);

            object outVal = command.GetParameterValue("SurveyId");

            if (outVal != null && outVal != DBNull.Value)
            {
                return (int)outVal;
            }

            return null;

        }

        /// <summary>
        /// Get the id of a survey based on the id of a response.
        /// </summary>
        /// <param name="responseGuid"></param>
        /// <returns></returns>
        public static int? GetSurveyIdFromResponseGuid(Guid responseGuid)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Response_GetSurveyIdFromResponseGuid");
            command.AddInParameter("ResponseGuid", DbType.Guid, responseGuid);
            command.AddOutParameter("SurveyId", DbType.Int32, 4);

            db.ExecuteNonQuery(command);

            object outVal = command.GetParameterValue("SurveyId");

            if (outVal != null && outVal != DBNull.Value)
            {
                return (int)outVal;
            }

            return null;
        }

        /// <summary>
        /// Get the id of a survey based on the id of a response.
        /// </summary>
        /// <param name="responseGuid"></param>
        /// <returns></returns>
        public static int? GetSurveyIdFromWorkflowSessionGuid(Guid responseGuid)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Response_GetSurveyIdFromWorkflowSessionGuid");
            command.AddInParameter("SessionGuid", DbType.Guid, responseGuid);
            command.AddOutParameter("SurveyId", DbType.Int32, 4);

            db.ExecuteNonQuery(command);

            object outVal = command.GetParameterValue("SurveyId");

            if (outVal != null && outVal != DBNull.Value)
            {
                return (int)outVal;
            }

            return null;
        }

        /// <summary>
        /// Get min/max response dates for a survey.
        /// </summary>
        /// <param name="responseTemplateId"></param>
        /// <param name="invitationId"></param>
        /// <param name="minDate"></param>
        /// <param name="maxDate"></param>
        public static void GetMinMaxResponseDates(int? responseTemplateId, int? invitationId, out DateTime? minDate, out DateTime? maxDate)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ResponseTemplate_GetMinMaxResponseDate");
            command.AddInParameter("ResponseTemplateId", DbType.Int32, responseTemplateId);
            command.AddInParameter("InvitationId", DbType.Int32, invitationId);

            minDate = null;
            maxDate = null;

            using (IDataReader reader = db.ExecuteReader(command))
            {
                if (reader.Read())
                {
                    minDate = DbUtility.GetValueFromDataReader<DateTime?>(reader, "MinStartDate", null);
                    DateTime? maxStartDate = DbUtility.GetValueFromDataReader<DateTime?>(reader, "MaxStartDate", null);
                    DateTime? maxEndDate = DbUtility.GetValueFromDataReader<DateTime?>(reader, "MaxEndDate", null);

                    if (maxEndDate.HasValue)
                    {
                        //Get the greater of max start/end date
                        maxDate = maxEndDate > maxStartDate ? maxEndDate : maxStartDate;
                    }
                    else
                    {
                        maxDate = maxStartDate;
                    }
                }
            }
        }

        /// <summary>
        /// Get a data set containing a response count historam.
        /// </summary>
        /// <param name="responseTemplateId"></param>
        /// <param name="invitationId"></param>
        /// <param name="minDate"></param>
        /// <param name="maxDate"></param>
        /// <param name="granularity"></param>
        /// <returns></returns>
        public static DataSet GetResponseHistogram(int? responseTemplateId, int? invitationId, DateTime minDate, DateTime maxDate, string granularity)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ResponseTemplate_GenerateActivityHistogram");
            command.AddInParameter("ResponseTemplateId", DbType.Int32, responseTemplateId);
            command.AddInParameter("InvitationId", DbType.Int32, invitationId);
            command.AddInParameter("StartDate", DbType.DateTime, minDate);
            command.AddInParameter("EndDate", DbType.DateTime, maxDate);
            command.AddInParameter("Granularity", DbType.String, granularity);

            return db.ExecuteDataSet(command);
        }

        /// <summary>
        /// Get a response guid by session guid
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static Guid? GetResponseGuidBySessionGuid(Guid guid)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Response_GetResponseGuidBySessionGuid");
            command.AddInParameter("SessionGuid", DbType.Guid, guid);
            command.AddOutParameter("ResponseGuid", DbType.Guid, 0);

            db.ExecuteNonQuery(command);

            object outVal = command.GetParameterValue("ResponseGuid");

            if (outVal != null && outVal != DBNull.Value)
            {
                return (Guid)outVal;
            }

            return null;
        }

        /// <summary>
        /// Get a response guid by session guid
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static Guid? GetSessionIdByResponseGuid(Guid guid)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Response_GetSessionIdByResponseGuid");
            command.AddInParameter("ResponseGuid", DbType.Guid, guid);

            Guid? sessionGuid = null;

            using (IDataReader reader = db.ExecuteReader(command))
            {
                if (reader.Read())
                    sessionGuid = DbUtility.GetValueFromDataReader(reader, "SessionId", (Guid?)null);
            }

            return sessionGuid;
        }

        /// <summary>
        /// Get a response data using the response GUID
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static ResponseData GetResponseData(Guid guid)
        {
            DataRow dr = GetResponseDataRow(guid);
            return dr == null ? null : GetResponseData(dr);
        }

        /// <summary>
        /// Get a response data using the response GUID
        /// </summary>
        /// <param name="responseId"></param>
        /// <returns></returns>
        public static ResponseData GetResponseData(long responseId)
        {
            DataRow dr = GetResponseDataRow(responseId);
            return dr == null ? null : GetResponseData(dr);
        }

        private static ResponseData GetResponseData(DataRow dr)
        {
            return new ResponseData
            {
                RespondentIp = ApplicationManager.AppSettings.LogIpAddresses ? DbUtility.GetValueFromDataRow(dr, "IP", string.Empty) : string.Empty,
                AnonymousRespondentGuid = DbUtility.GetValueFromDataRow(dr, "RespondentGUID", (Guid?)null),
                CompletionDate = DbUtility.GetValueFromDataRow(dr, "Ended", (DateTime?)null),
                Guid = DbUtility.GetValueFromDataRow(dr, "GUID", Guid.Empty),
                Id = DbUtility.GetValueFromDataRow(dr, "ResponseId", (long)0),
                LastEditDate = DbUtility.GetValueFromDataRow(dr, "LastEdit", DateTime.MinValue),
                LastPageViewed = DbUtility.GetValueFromDataRow(dr, "LastPageViewed", -1),
                NetworkUser = ApplicationManager.AppSettings.LogNetworkUser ? DbUtility.GetValueFromDataRow(dr, "NetworkUser", string.Empty) : string.Empty,
                ResponseLanguage = DbUtility.GetValueFromDataRow(dr, "Language", string.Empty),
                Started = DbUtility.GetValueFromDataRow(dr, "Started", DateTime.MinValue),
                UserIdentifier = DbUtility.GetValueFromDataRow(dr, "UniqueIdentifier", string.Empty),
                IsTest = DbUtility.GetValueFromDataRow(dr, "IsTest", DbUtility.GetValueFromDataRow(dr, "IsTest", false)),
                Invitee = DbUtility.GetValueFromDataRow(dr, "Invitee", string.Empty),
                IsAnonymized = DbUtility.GetValueFromDataRow(dr, "IsAnonymized", false),
                WorkflowSessionId = DbUtility.GetValueFromDataRow(dr, "SessionId", (Guid?)null)
            };
        }
    }
}
