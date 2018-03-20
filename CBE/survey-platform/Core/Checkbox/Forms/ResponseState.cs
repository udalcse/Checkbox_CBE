using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Runtime.Serialization;
using Checkbox.Analytics.Data;
using Checkbox.Common;
using Checkbox.Forms.ResponseStateEntities;
using Checkbox.Wcf.Services.Proxies;
using Prezza.Framework.Data;

namespace Checkbox.Forms
{
    /// <summary>
    /// Encapsulates the state of a Response for a specific Respondent
    /// </summary>
    [Serializable]
    public class ResponseState : IAnswerData, IDisposable
    {
        private readonly List<ResponseAnswerEntity> _answers;
        private readonly List<ResponseLogEntity> _logs;
        private readonly List<ResponsePageItemOrderEntity> _itemOrder;
        private readonly List<ResponseItemOptionOrderEntity> _optionOrder;

        private readonly object _answersLock;
        private readonly object _logsLock;
        private readonly object _itemOrderLoc;
        private readonly object _optionOrderLoc;

        /// <summary>
        /// 
        /// </summary>
        private long? _id;

        /// <summary>
        /// GUID associated with the response
        /// </summary>
        private Guid _guid;

        /// <summary>
        /// ID of response template response is associated with
        /// </summary>
        private int? _responseTemplateID;

        /// <summary>
        /// ID of last page viewed in response.
        /// </summary>
        private int? _lastPageViewed;

        /// <summary>
        /// IP address of respondent
        /// </summary>
        private string _ip;

        /// <summary>
        /// Unique identifier of respondent.
        /// </summary>
        private string _uniqueIdentifier;

        /// <summary>
        /// AD user name associated with respondent.
        /// </summary>
        private string _networkUser;

        /// <summary>
        /// Language code of the response.
        /// </summary>
        private string _languageCode;

        /// <summary>
        /// GUID associated with anonymous respondents.
        /// </summary>
        private Guid? _respondentGuid;

        /// <summary>
        /// Determines if the response is a test or not.
        /// </summary>
        private bool _isTest;

        /// <summary>
        /// 
        /// </summary>
        private bool _isAnonymized;

        /// <summary>
        /// 
        /// </summary>
        private string _invitee;

        /// <summary>
        /// 
        /// </summary>
        private Guid? _sessionGuid;

        /// <summary>
        /// 
        /// </summary>
        private DateTime? _startDate;

        /// <summary>
        /// 
        /// </summary>
        private DateTime? _endDate;

        /// <summary>
        /// 
        /// </summary>
        private DateTime? _lastEdit;

        /// <summary>
        /// 
        /// </summary>
        private bool _isComplete;

        /// <summary>
        /// 
        /// </summary>
        private string _resumeKey;
        
        /// <summary>
        /// 
        /// </summary>
        public ResponseState()
        {
            _answers = new List<ResponseAnswerEntity>();
            _logs = new List<ResponseLogEntity>();
            _itemOrder = new List<ResponsePageItemOrderEntity>();
            _optionOrder = new List<ResponseItemOptionOrderEntity>();

            _answersLock = new object();
            _logsLock = new object();
            _itemOrderLoc = new object();
            _optionOrderLoc = new object();
        }

        /// <summary>
        /// Clear the contents of the page log table.
        /// </summary>
        internal void ClearPageLog()
        {
            foreach (var log in _logs)
            {
                log.Delete();
            }
        }

        /// <summary>
        /// Add a page to the page log stack.
        /// </summary>
        /// <param name="pageID">ID of page to add to stack.</param>
        internal void PushPageLog(int pageID)
        {
            ResponseLogEntity log = EntityBase.Create<ResponseLogEntity>();
            log.ResponseID = ResponseID.Value;
            log.PageID = pageID;
            log.PageStartTime = DateTime.Now;

            ResponseLogEntity referringPage;
            lock (_logsLock)
            {
                referringPage = _logs.LastOrDefault(l => l.State != EntityState.Deleted);
            }

            if (referringPage != null)
            {
                referringPage.PageEndTime = DateTime.Now;
                referringPage.Update();
            }

            lock (_logsLock)
            {
                _logs.Add(log);
            }
        }

        /// <summary>
        /// Pop a page from the page log stack.
        /// </summary>
        internal void PopPageLog()
        {
            ResponseLogEntity referringPage;
            lock (_logsLock)
            {
                referringPage = _logs.LastOrDefault(l => l.State != EntityState.Deleted);
            }
            
            if (referringPage != null)
                referringPage.Delete();
        }

        /// <summary>
        /// Return a list of visited pages without popping the stack.
        /// </summary>
        internal int[] VisitedPages
        {
            get { return _logs.Select(l => l.PageID).ToArray(); }
        }

        /// <summary>
        /// Insert a row into the in-memory response row table.
        /// </summary>
        /// <param name="guid">GUID associated with the response</param>
        /// <param name="responseTemplateID">ID of response template response is associated with.</param>
        /// <param name="lastPageViewed">ID of last page viewed in response.</param>
        /// <param name="ip">IP address of respondent.</param>
        /// <param name="uniqueIdentifier">Unique identifier of respondent.</param>
        /// <param name="networkUser">AD user name associated with respondent.</param>
        /// <param name="languageCode">Language code of the response.</param>
        /// <param name="respondentGuid">GUID associated with anonymous respondents.</param>
        /// <param name="isTest">Determines if the response is a test or not.</param>
        /// <param name="invitee"></param>
        /// <param name="sessionGuid"></param>
        /// <param name="isAnonymized"></param>
        /// <param name="startDate"> </param>
        internal void InsertResponseData(
            Guid guid, 
            int responseTemplateID,
            int lastPageViewed,
            string ip,
            string uniqueIdentifier,
            string networkUser,
            string languageCode,
            Guid? respondentGuid,
            bool isTest,
            bool isAnonymized,
            string invitee,
            Guid? sessionGuid,
            DateTime? startDate = null
            )
        {
            string uniqueIdentifierHash = string.Empty;
            if (isAnonymized)
            {
                uniqueIdentifierHash = Utilities.GetSaltedMd5Hash(uniqueIdentifier);
                uniqueIdentifier = "AnonymizedRespondent";
                networkUser = string.Empty;
                invitee = string.Empty;
                ip = string.Empty;
            }
            
            _guid = guid;
            _responseTemplateID = responseTemplateID;
            _lastPageViewed = lastPageViewed;
            _ip = ip;

            if (sessionGuid.HasValue)
                _sessionGuid = sessionGuid;

            //If network user, set the unique identifier to be the network user as well
            if ((uniqueIdentifier == null || uniqueIdentifier.Trim() == string.Empty) && networkUser != null)
                _uniqueIdentifier = networkUser;
            else
                _uniqueIdentifier = uniqueIdentifier;
             
            _networkUser = networkUser;
            _languageCode = languageCode;
            _startDate = startDate ?? DateTime.Now;
            _isComplete = false;

            if (respondentGuid != null)
                _respondentGuid = respondentGuid;

            _isTest = isTest;
            _isAnonymized = isAnonymized;
            _resumeKey = uniqueIdentifierHash;
            _invitee = invitee;
        }

        /// <summary>
        /// Get the ID of the response from the response data table.
        /// </summary>
        public long? ResponseID 
        { 
            get { return _id; } 
        }

        /// <summary>
        /// 
        /// </summary>
        public int? ResponseTemplateId
        {
            get { return _responseTemplateID; }
        }

        /// <summary>
        /// Get the response created date from the response data table.
        /// </summary>
        internal DateTime? DateCreated 
        { 
            get { return _startDate; }
        }

        /// <summary>
        /// Get/(internal)set the last modified date for the response from the response data table.
        /// </summary>
        internal DateTime? LastModified
        {
            get { return _lastEdit; }
            set { _lastEdit = value; }
        }

        /// <summary>
        /// ID of related workflow session.  This is key to reloading workflow from
        /// persistent store.  For responses created prior to 5.0, this value will be null.
        /// </summary>
        internal Guid? SessionId
        {
            get { return _sessionGuid; }
            set { _sessionGuid = value; }
        }

        /// <summary>
        /// Get/set the completion date for the response in the response data table.
        /// </summary>
        internal DateTime? DateCompleted
        {
            get { return _endDate; }
            set { _endDate = value; }
        }

        /// <summary>
        /// Get respondent IP address from the response data table.
        /// </summary>
        internal string IpAddress
        {
            get { return _ip; }
        }

        /// <summary>
        /// 
        /// </summary>
        internal string Invitee
        {
            get { return _invitee; }
        }

        /// <summary>
        /// Get respondent uniqueidentifier from the response data table.
        /// </summary>
        internal string UniqueIdentifier
        {
            get { return _uniqueIdentifier; }
        }

        internal string NetworkUser
        {
            get { return _networkUser; }
        }

        /// <summary>
        /// Get/set response language code in/from the response data table.
        /// </summary>
        public string LanguageCode
        {
            get { return _languageCode; }
            set { _languageCode = value; }
        }

        /// <summary>
        /// Get/set id of last page view from/in the response data table.
        /// </summary>
        public int? LastPageViewed
        {
            get { return _lastPageViewed; }
            internal set { _lastPageViewed = value; }
        }

        /// <summary>
        /// Get GUID associated with respondent from the response data table.
        /// </summary>
        internal Guid? RespondentGuid 
        {
            get { return _respondentGuid; }
        }

        /// <summary>
        /// Get indicator of whether survey response is completed
        /// </summary>
        internal bool IsComplete
        {
            get { return _isComplete; }
            set { _isComplete = value; }
        }

        /// <summary>
        /// Get indicator of whether survey response is test
        /// </summary>
        internal bool IsTest
        {
            get { return _isTest; }
            set { _isTest = value; }
        }

        /// <summary>
        /// Get response guid
        /// </summary>
        public Guid? Guid
        {
            get { return _guid; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="responseTemplateId"></param>
        /// <param name="properties"></param>
        /// <param name="answers"></param>
        public void Load(int responseTemplateId, ResponseProperties properties, List<ItemAnswer> answers)
        {
            _responseTemplateID = responseTemplateId;
            _id = properties["ResponseID"] as long?; 
            _guid = properties["ResponseGuid"] is Guid ? (Guid)properties["ResponseGuid"] : System.Guid.NewGuid();
            _lastPageViewed = properties["LastPageViewed"] as int?;
            _ip = properties["IP"] as string;
            _uniqueIdentifier = properties["UniqueIdentifier"] as string;
            _networkUser = properties["NetworkUser"] as string;
            _languageCode = properties["Language"] as string;
            _startDate = properties["Started"] as DateTime?;
            _endDate = properties["Ended"] as DateTime?;
            _lastEdit = properties["LastEdit"] as DateTime?;
            _isComplete = properties["IsComplete"] is bool ? (bool)properties["IsComplete"] : false;
            _respondentGuid = properties["RespondentGuid"] as Guid?;
            _invitee = properties["Invitee"] as string;

            lock (_answersLock)
            {
                _answers.Clear();
                foreach (var a in answers)
                {
                    var answer = new ResponseAnswerEntity
                    {
                        AnswerId = a.AnswerId,
                        ResponseId = a.ResponseId,
                        ItemId = a.ItemId,
                        AnswerText = a.AnswerText,
                        OptionId = a.OptionId,
                        Points = a.Points    
                    };
                    answer.ResetState();
                    _answers.Add(answer);
                }
            }
        }

        /// <summary>
        /// For the response with the specified GUID, load state information from the database.
        /// </summary>
        /// <param name="responseGUID">GUID of the response to load state information for.</param>
        internal void Load(Guid responseGUID)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Response_GetState");
            command.AddInParameter("GUID", DbType.Guid, responseGUID);

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    // read response data
                    if (reader.Read())
                    {
                        _id = DbUtility.GetValueFromDataReader(reader, "ResponseID", default(long?));
                        _guid = DbUtility.GetValueFromDataReader(reader, "GUID", default(Guid));
                        _responseTemplateID = DbUtility.GetValueFromDataReader(reader, "ResponseTemplateID", default(int?));
                        _isComplete = DbUtility.GetValueFromDataReader(reader, "IsComplete", false);
                        _lastPageViewed = DbUtility.GetValueFromDataReader(reader, "LastPageViewed",  default(int?));
                        _startDate = DbUtility.GetValueFromDataReader(reader, "Started", default(DateTime?));
                        _endDate = DbUtility.GetValueFromDataReader(reader, "Ended", default(DateTime?));
                        _ip = DbUtility.GetValueFromDataReader(reader, "IP", string.Empty);
                        _lastEdit = DbUtility.GetValueFromDataReader(reader, "LastEdit", default(DateTime?));
                        _networkUser = DbUtility.GetValueFromDataReader(reader, "NetworkUser", string.Empty);
                        _languageCode = DbUtility.GetValueFromDataReader(reader, "Language", string.Empty);
                        _respondentGuid = DbUtility.GetValueFromDataReader(reader, "RespondentGUID", default(Guid?));
                        _isTest = DbUtility.GetValueFromDataReader(reader, "IsTest", true);
                        _isAnonymized = DbUtility.GetValueFromDataReader(reader, "IsAnonymized", true);
                        _resumeKey = DbUtility.GetValueFromDataReader(reader, "ResumeKey", string.Empty);
                        _invitee = DbUtility.GetValueFromDataReader(reader, "Invitee", string.Empty);
                        _sessionGuid = DbUtility.GetValueFromDataReader(reader, "SessionId", default(Guid?));
                        _uniqueIdentifier = DbUtility.GetValueFromDataReader(reader, "UniqueIdentifier", String.Empty);
                    }

                    //read answers data
                    if (reader.NextResult())
                    {
                        lock (_answersLock)
                        {
                            _answers.Clear();

                            while (reader.Read())
                            {
                                ResponseAnswerEntity answer = new ResponseAnswerEntity
                                                                  {
                                                                      AnswerId = DbUtility.GetValueFromDataReader(reader, "AnswerID", 0L),
                                                                      ResponseId = DbUtility.GetValueFromDataReader(reader, "ResponseID", 0L),
                                                                      ItemId = DbUtility.GetValueFromDataReader(reader, "ItemID", 0),
                                                                      AnswerText = DbUtility.GetValueFromDataReader(reader, "AnswerText", default(string)),
                                                                      OptionId = DbUtility.GetValueFromDataReader(reader, "OptionID", default(int?)),
                                                                      DateCreated = DbUtility.GetValueFromDataReader(reader, "DateCreated", default(DateTime?)),
                                                                      Deleted = DbUtility.GetValueFromDataReader(reader, "Deleted", false),
                                                                      ModifiedDate = DbUtility.GetValueFromDataReader(reader, "ModifiedDate", default(DateTime?)),
                                                                      Points = DbUtility.GetValueFromDataReader(reader, "Points", default(double?))
                                                                  };
                                answer.ResetState();
                                _answers.Add(answer);
                            }
                        }
                    }

                    //read log data
                    if (reader.NextResult())
                    {
                        lock (_logsLock)
                        {
                            _logs.Clear();

                            while (reader.Read())
                            {
                                ResponseLogEntity log = new ResponseLogEntity
                                {
                                    PageLogID = DbUtility.GetValueFromDataReader(reader, "PageLogID", 0L),
                                    ResponseID = DbUtility.GetValueFromDataReader(reader, "ResponseID", 0L),
                                    PageID = DbUtility.GetValueFromDataReader(reader, "PageID", 0),
                                    PageStartTime = DbUtility.GetValueFromDataReader(reader, "PageStartTime", default(DateTime?)),
                                    PageEndTime = DbUtility.GetValueFromDataReader(reader, "PageEndTime", default(DateTime?)),
                                };
                                log.ResetState();
                                _logs.Add(log);
                            }
                        }
                    }

                    //read page item order
                    if (reader.NextResult())
                    {
                        lock (_itemOrderLoc)
                        {
                            _itemOrder.Clear();

                            while (reader.Read())
                            {
                                ResponsePageItemOrderEntity pageItemOrder = new ResponsePageItemOrderEntity
                                {
                                    ResponseID = DbUtility.GetValueFromDataReader(reader, "ResponseID", 0L),
                                    PageID = DbUtility.GetValueFromDataReader(reader, "PageID", default(int?)),
                                    ItemID = DbUtility.GetValueFromDataReader(reader, "ItemID", default(int?)),
                                    Position = DbUtility.GetValueFromDataReader(reader, "Position", default(int?)),
                                };
                                pageItemOrder.ResetState();
                                _itemOrder.Add(pageItemOrder);
                            }
                        }
                    }

                    //read item option order
                    if (reader.NextResult())
                    {
                        lock (_optionOrderLoc)
                        {
                            _optionOrder.Clear();

                            while (reader.Read())
                            {
                                ResponseItemOptionOrderEntity itemOptionOrder = new ResponseItemOptionOrderEntity
                                {
                                    ResponseID = DbUtility.GetValueFromDataReader(reader, "ResponseID", 0L),
                                    OptionID = DbUtility.GetValueFromDataReader(reader, "OptionID", default(int?)),
                                    ItemID = DbUtility.GetValueFromDataReader(reader, "ItemID", default(int?)),
                                    Position = DbUtility.GetValueFromDataReader(reader, "Position", default(int?)),
                                };
                                itemOptionOrder.ResetState();
                                _optionOrder.Add(itemOptionOrder);
                            }
                        }
                    }
                    
                }
                finally
                {
                    reader.Close();
                }
            }
        }

        /// <summary>
        /// Save response information to the database.
        /// </summary>
        internal void Save()
        {
            Database db = DatabaseFactory.CreateDatabase();

            using (IDbConnection connection = db.GetConnection())
            {
                connection.Open();
                IDbTransaction transaction = connection.BeginTransaction();
                try
                {
                    //save/update the response
                    if (!ResponseID.HasValue)
                        Database_CreateResponse(db, transaction);
                    else
                        Database_UpdateResponse(db, transaction);

                    lock (_answersLock)
                    {
                        //update answers
                        for (int i=0; i < _answers.Count; i++)
                        {
                            var answer = _answers[i];
                            switch (answer.State)
                            {
                                case EntityState.Added:
                                    Database_CreateAnswer(db, transaction, answer);
                                    answer.ResetState();
                                    break;
                                case EntityState.Deleted:
                                    //if 'id' is null then this entity is not presented in the database, just skip it
                                    if (answer.AnswerId.HasValue)
                                        Database_DeleteAnswer(db, transaction, answer.AnswerId.Value);

                                    //remove the element
                                    _answers.RemoveAt(i--); 
                                    break;
                                case EntityState.Updated:
                                    Database_UpdateAnswer(db, transaction, answer);
                                    answer.ResetState();
                                    break;
                                default:
                                    break;
                            }
                        }
                    }

                    lock (_logsLock)
                    {
                        //update page logs
                        for (int i=0; i < _logs.Count; i++)
                        {
                            var log = _logs[i];
                            switch (log.State)
                            {
                                case EntityState.Added:
                                    Database_InsertPageLog(db, transaction, log);
                                    log.ResetState();
                                    break;
                                case EntityState.Deleted:
                                    //if 'id' is null then this entity is not presented in the database, just skip it
                                    if (log.PageLogID.HasValue)
                                        Database_DeletePageLog(db, transaction, log.PageLogID.Value);

                                    //remove the element
                                    _logs.RemoveAt(i--);
                                    break;
                                case EntityState.Updated:
                                    Database_UpdatePageLog(db, transaction, log);
                                    log.ResetState();
                                    break;
                                default:
                                    break;
                            }
                        }
                    }

                    lock (_itemOrderLoc)
                    {
                        //update items order
                        foreach (var order in _itemOrder)
                        {
                            switch (order.State)
                            {
                                case EntityState.Added:
                                    Database_InsertPageItemOrderCommand(db, transaction, order);
                                    break;
                                default:
                                    break;
                            }
                            order.ResetState();
                        }
                    }

                    lock (_optionOrderLoc)
                    {
                        
                        //update option order
                        foreach (var order in _optionOrder)
                        {
                            switch (order.State)
                            {
                                case EntityState.Added:
                                    Database_InsertItemOptionOrderCommand(db, transaction, order);
                                    break;
                                default:
                                    break;
                            }
                            order.ResetState();
                        }
                    }

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }

            //State saved
            if (Saved != null)
            {
                Saved(this, new EventArgs());
            }
        }

        #region Database SP wrappers

        private void Database_CreateResponse(Database db, IDbTransaction t)
        {
            DBCommandWrapper createResponseCommand = db.GetStoredProcCommandWrapper("ckbx_sp_Response_Create");
            createResponseCommand.AddInParameter("RespondentID", DbType.String, UniqueIdentifier);
            createResponseCommand.AddInParameter("GUID", DbType.Guid, Guid);
            createResponseCommand.AddInParameter("ResponseTemplateID", DbType.Int32, ResponseTemplateId);
            createResponseCommand.AddInParameter("LastPageViewed", DbType.Int32, LastPageViewed);
            createResponseCommand.AddInParameter("IP", DbType.String, IpAddress);
            createResponseCommand.AddInParameter("NetworkUser", DbType.String, NetworkUser);
            createResponseCommand.AddInParameter("StartDate", DbType.DateTime, DateCreated);
            createResponseCommand.AddInParameter("RespondentGuid", DbType.Guid, RespondentGuid);
            createResponseCommand.AddInParameter("LanguageCode", DbType.String, LanguageCode);
            createResponseCommand.AddInParameter("IsTest", DbType.Boolean, IsTest);
            createResponseCommand.AddInParameter("IsAnonymized", DbType.Boolean, _isAnonymized);
            createResponseCommand.AddInParameter("ResumeKey", DbType.String, _resumeKey);
            createResponseCommand.AddInParameter("Invitee", DbType.String, Invitee);
            createResponseCommand.AddInParameter("SessionId", DbType.Guid, SessionId);
            createResponseCommand.AddOutParameter("ResponseID", DbType.Int64, 8);

            db.ExecuteNonQuery(createResponseCommand, t);

            _id = (long)createResponseCommand.GetParameterValue("ResponseID");
        }

        private void Database_UpdateResponse(Database db, IDbTransaction t)
        {
            DBCommandWrapper updateResponseCommand = db.GetStoredProcCommandWrapper("ckbx_sp_Response_Update");
            updateResponseCommand.AddInParameter("GUID", DbType.Guid, Guid);
            updateResponseCommand.AddInParameter("LastEdit", DbType.DateTime, LastModified);
            updateResponseCommand.AddInParameter("IsComplete", DbType.Boolean, IsComplete);
            updateResponseCommand.AddInParameter("EndDate", DbType.DateTime, DateCompleted);
            updateResponseCommand.AddInParameter("LastPageViewed", DbType.Int32, LastPageViewed);
            updateResponseCommand.AddInParameter("Language", DbType.String, LanguageCode);
            updateResponseCommand.AddInParameter("IsTest", DbType.Boolean, IsTest);

            db.ExecuteNonQuery(updateResponseCommand, t);
        }

        private void Database_CreateAnswer(Database db, IDbTransaction t, ResponseAnswerEntity answer)
        {
            DBCommandWrapper insertAnswer = db.GetStoredProcCommandWrapper("ckbx_sp_Response_CreateAnswer");
            insertAnswer.AddInParameter("ResponseID", DbType.Int64, ResponseID);
            insertAnswer.AddInParameter("ItemID", DbType.Int32, answer.ItemId);
            insertAnswer.AddInParameter("AnswerText", DbType.String, answer.AnswerText);
            insertAnswer.AddInParameter("OptionID", DbType.Int32, answer.OptionId);
            insertAnswer.AddInParameter("Points", DbType.Decimal, answer.Points);
            insertAnswer.AddInParameter("DateCreated", DbType.DateTime, DateTime.Now);
            insertAnswer.AddOutParameter("AnswerID", DbType.Int64, 8);

            db.ExecuteNonQuery(insertAnswer, t);

            answer.AnswerId = (long)insertAnswer.GetParameterValue("AnswerID");
        }

        private void Database_UpdateAnswer(Database db, IDbTransaction t, ResponseAnswerEntity answer)
        {
            DBCommandWrapper updateAnswer = db.GetStoredProcCommandWrapper("ckbx_sp_Response_UpdateAnswer");
            updateAnswer.AddInParameter("AnswerID", DbType.Int64, answer.AnswerId);
            updateAnswer.AddInParameter("AnswerText", DbType.String, answer.AnswerText);
            updateAnswer.AddInParameter("Points", DbType.Decimal, answer.Points);
            updateAnswer.AddInParameter("OptionID", DbType.Int32, answer.OptionId);
            updateAnswer.AddInParameter("DateCreated", DbType.DateTime, DateTime.Now);

            db.ExecuteNonQuery(updateAnswer, t);
        }

        private void Database_DeleteAnswer(Database db, IDbTransaction t, long answerId)
        {
            DBCommandWrapper deleteAnswer = db.GetStoredProcCommandWrapper("ckbx_sp_Response_DeleteAnswer");
            deleteAnswer.AddInParameter("AnswerID", DbType.Int64, answerId);

            db.ExecuteNonQuery(deleteAnswer, t);
        }

        private void Database_InsertPageLog(Database db, IDbTransaction t, ResponseLogEntity log)
        {
            DBCommandWrapper insertPageLog = db.GetStoredProcCommandWrapper("ckbx_sp_Response_InsertPageLog");
            insertPageLog.AddInParameter("ResponseID", DbType.Int64, ResponseID);
            insertPageLog.AddInParameter("PageID", DbType.Int32, log.PageID);
            insertPageLog.AddInParameter("PageStartTime", DbType.DateTime, log.PageStartTime);
            insertPageLog.AddInParameter("PageEndTime", DbType.DateTime, log.PageEndTime);
            insertPageLog.AddOutParameter("PageLogID", DbType.Int64, 8);

            db.ExecuteNonQuery(insertPageLog, t);

            log.PageLogID = (long)insertPageLog.GetParameterValue("PageLogID");
        }

        private void Database_UpdatePageLog(Database db, IDbTransaction t, ResponseLogEntity log)
        {
            DBCommandWrapper updatePageLog = db.GetStoredProcCommandWrapper("ckbx_sp_Response_UpdatePageLog");
            updatePageLog.AddInParameter("PageLogID", DbType.Int64, log.PageLogID);
            updatePageLog.AddInParameter("PageEndTime", DbType.DateTime, log.PageEndTime);

            db.ExecuteNonQuery(updatePageLog, t);
        }

        private void Database_DeletePageLog(Database db, IDbTransaction t, long pageLogId)
        {
            DBCommandWrapper deletePageLog = db.GetStoredProcCommandWrapper("ckbx_sp_Response_DeletePageLog");
            deletePageLog.AddInParameter("PageLogID", DbType.Int64, pageLogId);

            db.ExecuteNonQuery(deletePageLog, t);
        }

        /// <summary>
        /// Get the command to insert page item order
        /// </summary>
        /// <param name="db">Database object.</param>
        /// <param name="t"> </param>
        /// <param name="order"> </param>
        /// <returns>Command wrapper with insert command defined.</returns>
        private void Database_InsertPageItemOrderCommand(Database db, IDbTransaction t, ResponsePageItemOrderEntity order)
        {
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Response_InsertPageItemOrder");
            command.AddInParameter("ResponseID", DbType.Int64, ResponseID);
            command.AddInParameter("PageID", DbType.Int32, order.PageID);
            command.AddInParameter("ItemID", DbType.Int32, order.ItemID);
            command.AddInParameter("Position", DbType.Int32, order.Position);

            db.ExecuteNonQuery(command, t);
        }

        /// <summary>
        /// Get the command to insert item option order
        /// </summary>
        /// <param name="db">Database object.</param>
        /// <param name="t"> </param>
        /// <param name="order"> </param>
        /// <returns>Command wrapper with insert command defined.</returns>
        private void Database_InsertItemOptionOrderCommand(Database db, IDbTransaction t, ResponseItemOptionOrderEntity order)
        {
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Response_InsertItemOptionOrder");
            command.AddInParameter("ResponseID", DbType.Int64, ResponseID);
            command.AddInParameter("ItemID", DbType.Int32, order.ItemID);
            command.AddInParameter("OptionID", DbType.Int32, order.OptionID);
            command.AddInParameter("Position", DbType.Int32, order.Position);

            db.ExecuteNonQuery(command, t);
        }

        #endregion

        /// <summary>
        /// Get the item order for the pages.  Returns empty list if information not present.
        /// </summary>
        /// <param name="pageID">ID of page to load item order for.</param>
        /// <returns>Ordered list of items on the page.</returns>
        /// <remarks>This method is used to maintain consistent ordering of items when item
        /// position randomization is enabled and a respondent is moving between pages or 
        /// resuming a survey.</remarks>
        internal List<int> GetPageItemOrder(int pageID)
        {
            List<ResponsePageItemOrderEntity> orderEntities;

            lock (_optionOrderLoc)
            {
                orderEntities = _itemOrder.Where(o => o.PageID == pageID && o.State != EntityState.Deleted).OrderBy(o => o.Position).ToList();
            }

            return orderEntities.Where(o => o.ItemID.HasValue).Select(o => o.ItemID.Value).ToList();
        }

        /// <summary>
        /// Persist the position of the item relative to other items on the specified page.
        /// </summary>
        /// <param name="pageID">ID of page containing item.</param>
        /// <param name="itemID">ID of item to persist order of.</param>
        /// <param name="position">Position of item relative to other items.</param>
        /// <remarks>This method is used to maintain consistent ordering of items when item
        /// position randomization is enabled and a respondent is moving between pages or 
        /// resuming a survey.</remarks>
        internal void SavePageItemOrder(int pageID, int itemID, int position)
        {
            ResponsePageItemOrderEntity order = EntityBase.Create<ResponsePageItemOrderEntity>();
            order.PageID = pageID;
            order.ItemID = itemID;
            order.Position = position;

            lock (_itemOrderLoc)
            {
                _itemOrder.Add(order);
            }
        }

        /// <summary>
        /// Get the option order for the item.  Returns empty list if information not present.
        /// </summary>
        /// <param name="itemID">ID of item to get option positions for.</param>
        /// <returns>Ordered list of options for an item.</returns>
        /// <remarks>This method is used to maintain consistent ordering of options when item
        /// option randomization is enabled and a respondent is moving between pages or 
        /// resuming a survey.</remarks>
        internal List<int> GetItemOptionOrder(int itemID)
        {
            List<ResponseItemOptionOrderEntity> orderEntities;

            lock (_optionOrderLoc)
            {
                orderEntities = _optionOrder.Where(o => o.ItemID == itemID && o.State != EntityState.Deleted).OrderBy(o => o.Position).ToList();
            }

            return orderEntities.Where(o => o.OptionID.HasValue).Select(o => o.OptionID.Value).ToList();
        }

        /// <summary>
        /// Store the position of an option relative to other options for the item.
        /// </summary>
        /// <param name="itemID">ID of item option is related to..</param>
        /// <param name="optionID">ID of option to store relative position of.</param>
        /// <param name="position">Position of the option.</param>
        /// <remarks>This method is used to maintain consistent ordering of options when item
        /// option randomization is enabled and a respondent is moving between pages or 
        /// resuming a survey.</remarks>
        internal void SaveItemOptionOrder(int itemID, int optionID, int position)
        {
            ResponseItemOptionOrderEntity order = EntityBase.Create<ResponseItemOptionOrderEntity>();
            order.ItemID = itemID;
            order.OptionID = optionID;
            order.Position = position;

            lock (_optionOrderLoc)
            {
                _optionOrder.Add(order);
            }
        }

        #region IAnswerData Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemId"></param>
        public void DeleteAllAnswersForItem(int itemId)
        {
            List<ResponseAnswerEntity> answers;
            lock (_answersLock)
            {
                answers = _answers.Where(a => a.ItemId == itemId && a.State != EntityState.Deleted).ToList();
            }

            answers.ForEach(a => a.Delete());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="optionId"> </param>
        public void DeleteOptionAnswerForItem(int itemId, int optionId)
        {
            List<ResponseAnswerEntity> answers;

            lock (_answersLock)
            {
                answers = _answers.Where(a => a.ItemId == itemId && a.State != EntityState.Deleted && a.OptionId == optionId).ToList();
            }

            answers.ForEach(a => a.Delete());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public bool IsAnswered(int itemId)
        {
            string answer = GetTextAnswerForItem(itemId);

            return !string.IsNullOrEmpty(answer) || GetOptionAnswersForItem(itemId).Any();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public List<long> GetAllAnswerIds(int itemId)
        {
            List<long> result;

            lock (_answersLock)
            {
                result = _answers.Where(a => a.ItemId == itemId && a.State != EntityState.Deleted && a.State != EntityState.Empty && a.AnswerId.HasValue)
                    .Select(a => a.AnswerId.Value).ToList();
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public List<SurveyResponseItemAnswer> BuildDataTransferObjectAnswerList(int itemId)
        {
            List<SurveyResponseItemAnswer> answerList = new List<SurveyResponseItemAnswer>();

            var answers = _answers.Where(a => a.ItemId == itemId && a.State != EntityState.Deleted && a.State != EntityState.Empty).ToList();
            foreach (var answer in answers)
            {
                answerList.Add(new SurveyResponseItemAnswer
                                   {
                                       AnswerText = answer.AnswerText,
                                       OptionId = answer.OptionId,
                                       Points = answer.Points,
                                       AnswerId = answer.AnswerId.HasValue ? answer.AnswerId.Value : -1L
                                   });
            }

            return answerList;
        }

        /// <summary>
        /// Get options for a specified item.
        /// </summary>
        /// <param name="itemID">ID of item to get answer rows for.</param>
        public Dictionary<int, double?> GetOptionAnswersForItem(int itemID)
        {
            Dictionary<int, double?> options;
            lock (_answersLock)
            {
                options = _answers.Where(a => a.ItemId == itemID && a.State != EntityState.Deleted && a.State != EntityState.Empty && a.OptionId.HasValue)
                    .Select(a => new { OptionId = a.OptionId.Value, a.Points }).Distinct().ToDictionary(a => a.OptionId, a => a.Points);
            }

            return options;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemID"></param>
        /// <returns></returns>
        public bool HasEmptyAnswer(int itemID)
        {
            lock (_answersLock)
            {
                return _answers.Any(a => a.ItemId == itemID && a.State == EntityState.Empty);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="optionId"> </param>
        /// <param name="points"></param>
        /// <param name="otherText"> </param>
        /// <param name="doNotSave"> Will answer be inserted into database on save </param>
        public void SetOptionAnswerForItem(int itemId, int optionId, double? points, string otherText, bool doNotSave = false)
        {
            ResponseAnswerEntity answerEntity;

            //if an answer already exist
            lock (_answersLock)
            {
                answerEntity = _answers.FirstOrDefault(a => a.ItemId == itemId && a.State != EntityState.Deleted && a.OptionId == optionId);
            }

            if (answerEntity != null)
            {
                if (otherText != null)
                    answerEntity.AnswerText = otherText;

                if (points != null)
                    answerEntity.Points = points;
            }
            else
                AddAnswer(itemId, otherText, optionId, points, doNotSave);
        }

        /// <summary>
        /// Get text answer for a specified item.
        /// </summary>
        /// <param name="itemID">ID of item to get answer rows for.</param>
        public string GetTextAnswerForItem(int itemID)
        {
            ResponseAnswerEntity answer;
            lock (_answersLock)
            {
                answer = _answers.FirstOrDefault(a => a.ItemId == itemID && a.State != EntityState.Deleted
                     && a.State != EntityState.Empty && !string.IsNullOrEmpty(a.AnswerText));
            }

            return answer != null ? answer.AnswerText : string.Empty;
        }

        /// <summary>
        /// Get the answer rows for the item with the specified id
        /// </summary>
        /// <param name="itemID">ID of item to get answer rows for.</param>
        /// <param name="answer"> </param>
        public void SetTextAnswersForItem(int itemID, string answer)
        {
            ResponseAnswerEntity answerEntity;
            lock (_answersLock)
            {
                answerEntity = _answers.FirstOrDefault(a => a.ItemId == itemID && a.State != EntityState.Deleted);
            }

            if (answerEntity != null)
                answerEntity.AnswerText = answer;
            else
                AddAnswer(itemID, answer, null, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="answerText"></param>
        /// <param name="optionId"></param>
        /// <param name="points"></param>
        /// <param name="doNotSave"> Will answer be inserted into database on save </param>
        private void AddAnswer(int itemId, string answerText, int? optionId, double? points, bool doNotSave = false)
        {
            var answer = EntityBase.Create<ResponseAnswerEntity>();
            answer.ItemId = itemId;
            answer.AnswerText = answerText;
            answer.OptionId = optionId;
            answer.Points = points;

            if (doNotSave)
            {
                answer.ResetState();
            }

            lock (_answersLock)
            {
                _answers.Add(answer);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemId"></param>
        public void SetEmptyAnswerForItem(int itemId)
        {
            bool alreadyExist;
            lock (_answersLock)
            {
                alreadyExist = _answers.Any(a => a.ItemId == itemId && a.State == EntityState.Empty);
            }

            if (!alreadyExist)
            {
                var answer = EntityBase.Create<ResponseAnswerEntity>();
                answer.ItemId = itemId;
                answer.Empty();

                lock (_answersLock)
                {
                    _answers.Add(answer);
                }
            }
        }

        #endregion

        #region IAnswerData Members

        /// <summary>
        /// Event fired when response state is saved.  Can be used as a trigger for items that store their data outside of the
        /// Response State to save their data as well.
        /// </summary>
        public event EventHandler Saved;

        #endregion

        public void Dispose()
        {
            _answers.Clear();
            _logs.Clear();
            _itemOrder.Clear();
            _optionOrder.Clear();
        }
    }
}
