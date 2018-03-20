using Checkbox.Analytics;
using Checkbox.Common;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Forms.Security.Principal;
using Checkbox.Globalization.Text;
using Checkbox.Management;
using Checkbox.Pagination;
using Checkbox.Security;
using Checkbox.Security.Principal;
using Prezza.Framework.Caching;
using Prezza.Framework.Data;
using Prezza.Framework.Data.Sprocs;
using Prezza.Framework.Security;
using Prezza.Framework.Security.Principal;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Checkbox.Analytics.Data;
using Checkbox.Forms.Data;
using Checkbox.Wcf.Services.Proxies;

namespace Checkbox.Forms
{
    /// <summary>
    /// Provides CRUD operations for ResponseTemplates
    /// </summary>
    public static class ResponseTemplateManager
    {
        private static readonly CacheManager _templateCache;
        private static readonly CacheManager _templateModificationDateCache;
        private static readonly CacheManager _defaultCache;

        private static readonly Dictionary<string, string> _surveyTextKeyMap;

        /// <summary>
        /// The number of texts associated with ancillary survey activities, such as login,
        /// language select, etc. are spread out and are configurable through Settings pages.
        /// This dictionary maps those text ids, which were not previously configurable on a
        /// per-survey basis to survey-specific values.  The map excludes values already
        /// configurable on a per-survey basis, such as survey button texts and title.
        /// </summary>
        public static Dictionary<string, string> SurveyTextKeyMap
        {
            get
            {
                return _surveyTextKeyMap;
            }
        }

        static ResponseTemplateManager()
        {
            lock (typeof(ResponseTemplateManager))
            {
                _templateCache = CacheFactory.GetCacheManager("templateCacheManager");
                _templateModificationDateCache = CacheFactory.GetCacheManager("templateModificationDate");
                _defaultCache = CacheFactory.GetCacheManager("defaultCacheManager");

                //Create and populate dictionary.
                _surveyTextKeyMap = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
                _surveyTextKeyMap["TITLE"] = "/";
                _surveyTextKeyMap["CONTINUE"] = "/pageText/responseTemplate.cs/nextDefaultText";
                _surveyTextKeyMap["BACK"] = "/pageText/responseTemplate.cs/backDefaultText";
                _surveyTextKeyMap["FINISH"] = "/pageText/responseTemplate.cs/finishDefaultText";
                _surveyTextKeyMap["SAVEPROGRESS"] = "/pageText/responseTemplate.cs/saveDefaultText";
                _surveyTextKeyMap["FORM_RESET"] = "/pageText/responseTemplate.cs/formResetDefaultText";
                _surveyTextKeyMap["ACTIVATION_NOTACTIVE"] = "/pageText/survey.aspx/surveyNotActive";
                _surveyTextKeyMap["ACTIVATION_DELETED"] = "/pageText/survey.aspx/surveyDeleted";
                _surveyTextKeyMap["ACTIVATION_NOTYETACTIVE"] = "/pageText/survey.aspx/surveyNotYetActive";
                _surveyTextKeyMap["ACTIVATION_NOLONGERACTIVE"] = "/pageText/survey.aspx/surveyActivationEnded";
                _surveyTextKeyMap["SOCIAL_MEDIA_DESCRIPTION"] = "/controlText/survey.aspx/metaDescription";
                _surveyTextKeyMap["ACTIVATION_MAXRESPONSES"] = "/pageText/survey.aspx/MaxResponses";
                _surveyTextKeyMap["ACTIVATION_NOINVITATION"] = "/pageText/survey.aspx/noInvitation";
                _surveyTextKeyMap["MISC_CONTINUE"] = "/pageText/takeSurvey.aspx/continue";
                _surveyTextKeyMap["LOGIN_ENTERPASSWORD"] = "/pageText/survey.aspx/enterPassword";
                _surveyTextKeyMap["LOGIN_INVALIDPASSWORD"] = "/pageText/survey.aspx/incorrectPassword";
                _surveyTextKeyMap["LOGIN_SELECTLANGUAGE"] = "/pageText/takeSurvey.aspx/selectLanguage";
                _surveyTextKeyMap["LOGIN_LOGINFAILED"] = "/pageText/login.aspx/loginFailed";
                _surveyTextKeyMap["LOGIN_USERNAME"] = "/pageText/login.aspx/username";
                _surveyTextKeyMap["LOGIN_PASSWORD"] = "/pageText/login.aspx/password";
                _surveyTextKeyMap["LOGIN_RESETPASSWORD"] = "/pageText/Login.aspx/pwreset";
                _surveyTextKeyMap["LOGIN_NEWUSER"] = "/pageText/Login.aspx/newUser";
                _surveyTextKeyMap["LOGIN_LOGINBUTTON"] = "/pageText/login.aspx/loginButton";
                _surveyTextKeyMap["LOGIN_LOGINTEXT"] = "/pageText/login.aspx/loginText";
                _surveyTextKeyMap["SAVEPROGRESS_PROGRESSSAVED"] = "/controlText/responseView/progressSaved";
                _surveyTextKeyMap["SAVEPROGRESS_TORESUME"] = "/controlText/responseView/toResume";
                _surveyTextKeyMap["SAVEPROGRESS_MOBILERESUME"] = "/controlText/responseView/mobileResume";
                _surveyTextKeyMap["SAVEPROGRESS_CLICKHERE"] = "/controlText/responseView/clickHere";
                _surveyTextKeyMap["SAVEPROGRESS_TORETURN"] = "/controlText/responseView/toReturn";
                _surveyTextKeyMap["SAVEPROGRESS_TOEMAIL"] = "/controlText/responseView/toEmail";
                _surveyTextKeyMap["SAVEPROGRESS_EMAILADDRESS"] = "/controlText/responseView/emailAddress";
                _surveyTextKeyMap["SAVEPROGRESS_SENDEMAIL"] = "/controlText/responseView/sendEmail";
               
            }
        }

        /// <summary>
        /// Get list of active survey languages
        /// </summary>
        public static string[] ActiveSurveyLanguages
        {
            get
            {
                if (!_defaultCache.Contains("ActiveSurveyLanguages"))
                {
                    lock (typeof(ResponseTemplateManager))
                    {
                        if (!_defaultCache.Contains("ActiveSurveyLanguages"))
                        {
                            _defaultCache.Add("ActiveSurveyLanguages", new string[] { });
                        }
                    }
                }

                return (string[])_defaultCache["ActiveSurveyLanguages"];
            }
            set
            {
                _defaultCache.Add("ActiveSurveyLanguages", value);
            }
        }

        /// <summary>
        /// Get list of "in-use" survey languages.  Also include default app language
        /// and current user's language
        /// </summary>
        /// <returns></returns>
        public static void EnsureActiveLanguage(string langaugeCode)
        {
            MergeActiveSurveyLanguages(new[] { langaugeCode });
        }

        /// <summary>
        /// Ensure
        /// </summary>
        private static void MergeActiveSurveyLanguages(IEnumerable<string> languages)
        {
            var languagesToAdd = new List<string>();
            string[] activeLanguages = ActiveSurveyLanguages;

            foreach (string language in languages)
            {
                if (!activeLanguages.Contains(language, StringComparer.InvariantCultureIgnoreCase)
                    && !languagesToAdd.Contains(language))
                {
                    languagesToAdd.Add(language);
                }
            }

            if (languagesToAdd.Count == 0)
            {
                return;
            }

            languagesToAdd.AddRange(activeLanguages);

            lock (typeof(ResponseTemplateManager))
            {
                ActiveSurveyLanguages = languagesToAdd.ToArray();
            }
        }

        /// <summary>
        /// Create a new response template
        /// </summary>
        /// <returns>An empty <see cref="ResponseTemplate"/> object.</returns>
        public static ResponseTemplate CreateResponseTemplate(CheckboxPrincipal principal)
        {
            //There are several pieces to creating a template
            //  1) Create template object
            //  2) Create default policy (public access)
            //  3) Create an ACL entry for the owner
            //

            //Create the template object
            var template = new ResponseTemplate(principal.AclEntryIdentifier);

            //Create a default policy w/no permissions
            Policy defaultPolicy = template.CreatePolicy(new string[] { });

            //Create an access list
            var acl = new AccessControlList();
            Policy creatorAccessPolicy = template.CreatePolicy(template.SupportedPermissions);
            acl.Add(principal, creatorAccessPolicy);
            acl.Save();

            //Set the default policy & acl
            template.InitializeAccess(defaultPolicy, acl);

            //Add default pages

            //Now save the template, but do so within the context of a transaction
            // to be used by item data objects and other components in the template.
            Database db = DatabaseFactory.CreateDatabase();

            using (IDbConnection connection = db.GetConnection())
            {
                connection.Open();
                IDbTransaction transaction = connection.BeginTransaction();

                try
                {
                    //Save the template
                    template.Save(transaction);

                    //Commit
                    transaction.Commit();
                }
                catch
                {
                    //Rollback on error
                    transaction.Rollback();
                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }

            template.NotifyCommit(null, new EventArgs());

            //Update template ACL and policy for default settings
            template.BehaviorSettings.ReportSecurityType = ReportSecurityType.SummaryPrivate;
            template.BehaviorSettings.SecurityType = ApplicationManager.AppSettings.DefaultSurveySecurityType;
            InitializeResponseTemplateAclAndPolicy(template, principal);

            //Reload
            template.Load();

            return template;
        }

        /// <summary>
        /// Create a new response template
        /// </summary>
        /// <returns>An empty <see cref="ResponseTemplate"/> object.</returns>
        public static ResponseTemplate CreateResponseTemplate(ResponseTemplate template, CheckboxPrincipal principal)
        {
            //There are several pieces to creating a template
            //  1) Create template object
            //  2) Create default policy (public access)
            //  3) Create an ACL entry for the owner
            //

            //Create the template object
            //ResponseTemplate template = new ResponseTemplate(principal.AclEntryIdentifier);
            //template.CreatedBy = principal.AclEntryIdentifier;
            //Create a default policy w/no permissions
            Policy defaultPolicy = template.CreatePolicy(new string[] { });

            //Create an access list
            var acl = new AccessControlList();
            Policy creatorAccessPolicy = template.CreatePolicy(template.SupportedPermissions);
            acl.Add(principal, creatorAccessPolicy);
            acl.Save();

            //Set the default policy & acl
            template.InitializeAccess(defaultPolicy, acl);

            //Add default pages

            //Now save the template, but do so within the context of a transaction
            // to be used by item data objects and other components in the template.
            Database db = DatabaseFactory.CreateDatabase();

            using (IDbConnection connection = db.GetConnection())
            {
                connection.Open();
                IDbTransaction transaction = connection.BeginTransaction();

                try
                {
                    //Save the template
                    template.Save(transaction);

                    //Commit
                    transaction.Commit();
                }
                catch
                {
                    //Rollback on error
                    transaction.Rollback();
                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }

            template.NotifyCommit(null, new EventArgs());

            //Update template ACL and policy for default settings
            template.BehaviorSettings.ReportSecurityType = ReportSecurityType.SummaryPrivate;
            template.BehaviorSettings.SecurityType = ApplicationManager.AppSettings.DefaultSurveySecurityType;
            InitializeResponseTemplateAclAndPolicy(template, principal);

            //Reload
            template.Load();

            return template;
        }

        /// <summary>
        /// Initialize a template by creating pages, setting default languages, applying
        /// default security settings, etc.
        /// </summary>
        /// <param name="rt"></param>
        /// <param name="name"></param>
        public static void InitializeTemplate(ResponseTemplate rt, string name)
        {
            rt.LanguageSettings.DefaultLanguage = TextManager.DefaultLanguage;
            rt.LanguageSettings.SupportedLanguages = new List<string> { rt.LanguageSettings.DefaultLanguage };
            rt.LanguageSettings.SurveyId = rt.ID != null ? rt.ID.Value : -1;
            rt.Name = name;

            rt.BehaviorSettings.ReportSecurityType = ApplicationManager.AppSettings.DefaultReportSecurityType;
            rt.BehaviorSettings.SecurityType = ApplicationManager.AppSettings.DefaultSurveySecurityType;
            rt.BehaviorSettings.AllowContinue = ApplicationManager.AppSettings.DefaultAllowResumeResponse;
            rt.BehaviorSettings.AllowEdit = ApplicationManager.AppSettings.DefaultAllowEditResponse;

            if (ApplicationManager.AppSettings.DefaultTotalResponseLimit > 0)
            {
                rt.BehaviorSettings.MaxTotalResponses = ApplicationManager.AppSettings.DefaultTotalResponseLimit;
            }

            if (ApplicationManager.AppSettings.DefaultUserResponseLimit > 0)
            {
                rt.BehaviorSettings.MaxResponsesPerUser = ApplicationManager.AppSettings.DefaultUserResponseLimit;
            }

            if (ApplicationManager.AppSettings.DefaultStyleTemplate > 0)
            {
                rt.StyleSettings.StyleTemplateId = ApplicationManager.AppSettings.DefaultStyleTemplate;
            }

            //Set the completion type to message upon creation so it will be set if the survey is now
            // mobile-compatible or will be made mobile compatible
            rt.BehaviorSettings.CompletionType = 1; //1 = Show message (added below)

            //Ensure at least one content page.  Hidden items and completion pages are handled automatically by
            // rt when a page is added.
            rt.AddPageToTemplate(2, true);

            TemplatePage completionPage = rt.GetCompletionPage();

            if (completionPage != null)
            {
                //Add a message item completion event by default
                var messageItemData = (MessageItemData)ItemConfigurationManager.CreateConfigurationData("Message");

                if (messageItemData != null)
                {
                    var decorator = new MessageItemTextDecorator(messageItemData, rt.LanguageSettings.DefaultLanguage)
                    {
                        Message = TextManager.GetText("/pageText/surveyProperties.aspx/defaultCompletionText", TextManager.DefaultLanguage)
                    };

                    messageItemData.CreatedBy = rt.CreatedBy;

                    decorator.Save();

                    var appearance = (Items.UI.Message)Items.UI.AppearanceDataManager.GetDefaultAppearanceDataForType(decorator.Data.ItemTypeID);

                    if (appearance != null)
                    {
                        appearance.Save(decorator.Data.ID.Value);

                        rt.AddItemToPage(completionPage.ID.Value, decorator.Data.ID.Value, null);
                    }
                }
            }

            rt.ModifiedBy = rt.CreatedBy;
            rt.Save();
        }

        /// <summary>
        /// Gets the ConfigurationDataSet for a given <see cref="ResponseTemplate"/>
        /// </summary>
        /// <param name="templateID">the data ID of the template</param>
        /// <returns></returns>
        public static DataSet GetResponseTemplateData(int templateID)
        {
            //To avoid having each page, item, etc. call to the database, we use a system
            //where item data is retrieved all at once.  However, to support plugging new items
            //in, it will not be required that every item's data be in the retrieved dataset.  One issue
            //is that we will not, in code, know the names of each table returned by ckbx_GetResponseTemplate,
            //so what we'll do is look that up.

            //Step 1:  Look up the table names; tablenames must be ordered by position
            var tableNames = new List<string>();
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper dbNameCommand = db.GetStoredProcCommandWrapper("ckbx_sp_RT_GetTableNames");

            using (IDataReader reader = db.ExecuteReader(dbNameCommand))
            {
                try
                {
                    while (reader.Read())
                    {
                        tableNames.Add((string)reader["TableName"]);
                    }
                }
                catch
                {
                }
                finally
                {
                    reader.Close();
                }
            }

            //Step 2:  Get the data from the database, using the table names we just retrieved
            var data = new DataSet();
            DBCommandWrapper dbDataCommand = db.GetStoredProcCommandWrapper("ckbx_sp_RT_GetResponseTemplate");
            dbDataCommand.AddInParameter("ResponseTemplateID", DbType.Int32, templateID);
            db.LoadDataSet(dbDataCommand, data, tableNames.ToArray());

            if (data.Tables.Count == 0 || !data.Tables.Contains(ResponseTemplate.TEMPLATE_DATA_TABLE_NAME) || data.Tables[ResponseTemplate.TEMPLATE_DATA_TABLE_NAME].Rows.Count <= 0)
            {
                throw new TemplateDoesNotExist(templateID);
            }
            return data;
        }

        /// <summary>
        /// Get the GUID for a response template
        /// </summary>
        /// <param name="templateID"></param>
        /// <returns></returns>
        public static Guid? GetResponseTemplateGUID(Int32 templateID)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_RT_GetGUID");
            command.AddInParameter("ResponseTemplateID", DbType.Int32, templateID);

            Guid? guid = null;

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    while (reader.Read())
                    {
                        guid = (Guid)reader["GUID"];
                    }
                }
                catch
                {
                }
                finally
                {
                    reader.Close();
                }
            }

            return guid;
        }

        /// <summary>
        /// Determines if a response template can be deleted.
        /// </summary>
        /// <param name="templateID">The id of the template being evaluated.</param>
        /// <param name="principal">The <see cref="ExtendedPrincipal"/> of the user performing the evaluation.</param>
        /// <returns></returns>
        public static bool CanBeDeleted(int templateID, ExtendedPrincipal principal)
        {
            LightweightResponseTemplate template = GetLightweightResponseTemplate(templateID);
            bool authorized = AuthorizationFactory.GetAuthorizationProvider().Authorize(principal, template, "Form.Delete");
            bool canEditActiveSurvey = ApplicationManager.AppSettings.AllowEditActiveSurvey && template.AllowEditWhileActive;

            return (authorized && template.IsActive && canEditActiveSurvey) || (authorized && !template.IsActive);
        }

        /// <summary>
        /// Determines if a response template can be edited.  Does NOT check permissions, but does check app/survey
        /// settings regarding editing active surveys.
        /// </summary>
        /// <param name="templateId"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public static bool CanBeEdited(int templateId, out string reason)
        {
            ResponseTemplate rt = GetResponseTemplate(templateId);

            if (rt == null)
            {
                reason = string.Empty;
                return false;
            }

            //If survey is not active or has no responses, allow edit
            string dummy;
            if (!rt.BehaviorSettings.GetIsActiveOnDate(DateTime.Now, out dummy)
                || ResponseManager.ResponseTemplateHasNonTestResponses(templateId))
            {
                reason = string.Empty;
                return true;
            }

            //Otherwise, set appropriate error message and return false
            if (ApplicationManager.AppSettings.AllowEditActiveSurvey)
            {
                reason = "appSetting";
                return false;
            }

            reason = "surveySetting";
            return false;
        }

        /// <summary>
        /// Delete a response template
        /// </summary>
        /// <param name="templateID"></param>
        public static void DeleteResponseTemplate(int templateID)
        {
            DeleteResponseTemplate(templateID, null);
        }

        /// <summary>
        /// Delete a response template within the context of the specified transaction.
        /// </summary>
        /// <param name="templateID"></param>
        /// <param name="t"></param>
        public static void DeleteResponseTemplate(int templateID, IDbTransaction t)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_RT_Delete");
            command.AddInParameter("ResponseTemplateID", DbType.Int32, templateID);
            command.AddInParameter("ModifiedDate", DbType.DateTime, DateTime.Now);

            if (t != null)
            {
                db.ExecuteNonQuery(command, t);
            }
            else
            {
                db.ExecuteNonQuery(command);
            }

            MarkTemplateUpdated(templateID);
        }

        /// <summary>
        /// Inserts page break indentifier to the database
        /// </summary>
        /// <param name="pageId"></param>
        /// <param name="shouldPageBreak"></param>
        /// <param name="templateId"></param>
        public static void InsertPageBreak(int pageId, bool shouldPageBreak, int templateId)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_TemplatePage_AddPageBreak");
            command.AddInParameter("@PageId", DbType.Int32, pageId);
            command.AddInParameter("@ShouldPageBreak", DbType.Boolean, shouldPageBreak);

            db.ExecuteNonQuery(command);
            MarkTemplateUpdated(templateId);
        }

        #region Retrieve

        /// <summary>
        /// Get a response template with the specified id.
        /// </summary>
        /// <param name="templateID">ID of the response template to get.</param>
        /// <returns>A <see cref="ResponseTemplate"/> object initialized with its data. A
        /// cached template will be returned if available.</returns>
        public static ResponseTemplate GetResponseTemplate(int templateID)
        {
            var template = (ResponseTemplate)_templateCache[templateID.ToString()];

            //Ensure template is "fresh".  If template has been updated since
            // the cached template's "Last Modified" date, ensure template
            // gets reloaded.
            if (template != null
                && CheckTemplateUpdated(templateID, template.LastModified))
            {
                template = null;
            }

            //Load template, if necessary
            if (template == null)
            {
                template = new ResponseTemplate();
                try
                {
                    template.Load(templateID);
                    CacheResponseTemplate(template);
                    MergeActiveSurveyLanguages(template.LanguageSettings.SupportedLanguages);
                }
                catch (TemplateDoesNotExist e)
                {
                    template = null;
                }
            }

            return template;
        }

        /// <summary>
        /// Get the ID of a response template given it's name.
        /// </summary>
        /// <param name="responseTemplateName">Name of response template to get id for.</param>
        /// <returns>ID of response template.</returns>
        /// <remarks>Returns null if no response template with name found.  If multiple templates with name exists, returns
        /// first matching name.</remarks>
        public static int? GetResponseTemplateId(string responseTemplateName)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_ResponseTemplate_GetIdFromName");

            command.AddInParameter("TemplateName", DbType.String, responseTemplateName);
            command.AddOutParameter("TemplateId", DbType.Int32, 4);

            db.ExecuteNonQuery(command);

            object outVal = command.GetParameterValue("TemplateId");

            if (outVal != null && outVal != DBNull.Value)
            {
                return (int)outVal;
            }

            return null;
        }

        /// <summary>
        /// Add rt to cache
        /// </summary>
        /// <param name="rt"></param>
        public static void CacheResponseTemplate(ResponseTemplate rt)
        {
            if (ApplicationManager.AppSettings.CacheResponseTemplates)
            {
                _templateCache.Add(rt.ID.ToString(), rt);

                _templateModificationDateCache.Add(rt.ID.ToString(), rt.LastModified);
            }
        }

        /// <summary>
        /// Get a AccessControllableResource which has the same ACL and DefaultPolicy as a given template.
        /// </summary>
        /// <param name="templateID">ID of the response template to clone ACL and DefaultPolicy from.</param>
        /// <returns>A <see cref="LightweightAccessControllable"/> object initialized with a
        /// a given Template's ACL and DefaultPolicy.</returns>
        public static LightweightResponseTemplate GetLightweightResponseTemplate(int templateID)
        {
            var lightweightTemplate = _templateCache.GetData("Lightweight" + templateID.ToString()) as LightweightResponseTemplate;

            if (lightweightTemplate != null)
                return lightweightTemplate;

            lightweightTemplate = new LightweightResponseTemplate(templateID);
            StoredProcedureCommandExtractor.ExecuteProcedure(ProcedureType.Select, lightweightTemplate);

            _templateCache.Add("Lightweight" + templateID.ToString(), lightweightTemplate);

            return lightweightTemplate;
        }

        /// <summary>
        /// Get a AccessControllableResource which has the same ACL and DefaultPolicy as a given template.
        /// </summary>
        /// <param name="templateGuid">ID of the response template to clone ACL and DefaultPolicy from.</param>
        /// <returns>A <see cref="LightweightAccessControllable"/> object initialized with a
        /// a given Template's ACL and DefaultPolicy.</returns>
        public static LightweightResponseTemplate GetLightweightResponseTemplate(Guid templateGuid)
        {
            int? templateId = GetResponseTemplateIdFromGuid(templateGuid);

            if (!templateId.HasValue)
            {
                return null;
            }

            var lightweightTemplate = new LightweightResponseTemplate(templateId.Value);
            StoredProcedureCommandExtractor.ExecuteProcedure(ProcedureType.Select, lightweightTemplate);

            return lightweightTemplate;
        }

        /// <summary>
        /// Get the Id of a response template from its guid.f
        /// </summary>
        /// <param name="responseTemplateGuid"></param>
        /// <returns></returns>
        public static int? GetResponseTemplateIdFromGuid(Guid responseTemplateGuid)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper dbDataCommand = db.GetStoredProcCommandWrapper("ckbx_sp_RT_GetIDByGUID");
            dbDataCommand.AddInParameter("GUID", DbType.String, responseTemplateGuid.ToString());

            object result = db.ExecuteScalar(dbDataCommand);

            if (result == null || result == DBNull.Value)
            {
                return null;
            }

            return (int)result;
        }

        /// <summary>
        /// Get the response template with the specified <see cref="Guid"/>.
        /// </summary>
        /// <param name="guid"><see cref="Guid"/> for the response template.</param>
        /// <returns><see cref="ResponseTemplate"/> object.</returns>
        public static ResponseTemplate GetResponseTemplate(Guid guid)
        {
            int? rtId = GetResponseTemplateIdFromGuid(guid);

            if (!rtId.HasValue)
            {
                throw new TemplateDoesNotExist(guid.ToString());
            }

            return GetResponseTemplate(rtId.Value);
        }

        /// <summary>
        /// Get the response template with the specified <see cref="Guid" />.
        /// </summary>
        /// <param name="templateId">The template identifier.</param>
        /// <returns>
        ///   <see cref="ResponseTemplate" /> object.
        /// </returns>
        /// <exception cref="System.ArgumentException">template id should be greater than 0 - templateId</exception>
        public static Dictionary<int, string> GetResponseTemplateSectionsIds(int templateId)
        {
            if (templateId <= 0)
                throw new ArgumentException("template id should be greater than 0", nameof(templateId));

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command =
                db.GetStoredProcCommandWrapper("ckbx_sp_Get_Item_Sections");

            command.AddInParameter("TemplateID", DbType.Int32, templateId);

            var result = new Dictionary<int,string>();

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {

                    while (reader.Read())
                    {
                        int itemId = DbUtility.GetValueFromDataReader(reader, "ItemID", 0);
                        string textValue = DbUtility.GetValueFromDataReader(reader, "TextValue", string.Empty);

                        if (!string.IsNullOrWhiteSpace(textValue))
                            textValue = Utilities.StripHtml(textValue);

                        result.Add(itemId,textValue);
                    }

                }
                finally
                {
                    reader.Close();
                }
            }

            return result;
        }


        /// <summary>
        /// Gets the response template reportable sections.
        /// </summary>
        /// <param name="templateId">The template identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">template id should be greater than 0 - templateId</exception>
        public static List<ReportableSection> GetResponseTemplateReportableSections(int templateId)
        {
            var reportableSectionIds = GetResponseTemplateSectionsIds(templateId);

            return reportableSectionIds?.Keys.Select(
                key => new ReportableSection() {Id = key, Title = reportableSectionIds[key]}).ToList() ?? new List<ReportableSection>();
        }

        /// <summary>
        /// Gets  e sigma values for heat map.
        /// </summary>
        /// <param name="templateId">The template identifier.</param>
        /// <param name="reportId">The report identifier.</param>
        /// <returns></returns>
        public static Dictionary<int, double> GetSectionsESigmaValues(int templateId, int reportId)
        {
            var eSigmaValues = GetResponseTemplateSectionsESigma(reportId, true);
          
            return eSigmaValues ?? new Dictionary<int, double>();
        }

        /// <summary>
        /// Get the eSgima values for sections. Null if report not allowed to use eSigma values
        /// </summary>
        /// <param name="reportId">The report identifier.</param>
        /// <param name="checkForEnable">if set to <c>true</c> [check for enable].</param>
        /// <returns>
        ///   <see cref="ResponseTemplate" /> object.
        /// </returns>
        /// <exception cref="System.ArgumentException">report id should be greater than 0</exception>
        public static Dictionary<int, double> GetResponseTemplateSectionsESigma(int reportId, bool checkForEnable)
        {
            if (reportId <= 0)
                throw new ArgumentException("template id should be greater than 0", nameof(reportId));

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command =
                db.GetStoredProcCommandWrapper("ckbx_sp_Get_Items_eSigma_Values");

            command.AddInParameter("ItemID", DbType.Int32, reportId);
            command.AddInParameter("CheckForEnable", DbType.Boolean, checkForEnable);

            var result = new Dictionary<int, double>();

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {

                    while (reader.Read())
                    {
                        int itemId = DbUtility.GetValueFromDataReader(reader, "SectionId", 0);
                        double eSigma = DbUtility.GetValueFromDataReader<double>(reader, "eSigmaValue", 1);
                        result.Add(itemId, eSigma);
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            return result;
        }



        /// <summary>
        /// Gets the response template sections.
        /// </summary>
        /// <param name="templateId">The template identifier.</param>
        /// <returns></returns>
        public static List<ReportableSection> GetResponseTemplateSections(int templateId)
        {
            var templateSections = GetResponseTemplateReportableSections(templateId);
            var template = GetResponseTemplate(templateId);

            var pagesWithSections = GetPagesWithSectionItems(template, templateSections);

            return GetPagesWithSections(pagesWithSections, templateSections);

        }

        public static List<ReportableSection> GetSurveySections(int templateId)
        {
            var templateSections = GetResponseTemplateReportableSections(templateId);

            foreach (var section in templateSections)
                section.Items.AddRange(SurveySectionManager.GetSectionItemIds(section.Id));

            return templateSections;
        }

        private static Dictionary<int, List<int>> GetPagesWithSectionItems(ResponseTemplate template, List<ReportableSection> reportableSections )
        {
            var pagesWithSections = new Dictionary<int, List<int>>();

            // filter pages with section items
            foreach (var key in template.TemplatePages.Keys)
            {
                var page = template.TemplatePages[key];

                // if page has secton item , add this page to filtered list of data
                if (reportableSections.Any(section => page.ContainsItem(section.Id) && page.PageType == TemplatePageType.ContentPage))
                {
                    pagesWithSections.Add(page.ID.Value, page.ListItemIds().ToList());
                }
            }

            //remove item that are before section header
            foreach (var key in pagesWithSections.Keys)
            {
                foreach (var itemId in pagesWithSections[key].ToList())
                {
                    if (!reportableSections.Any(item => item.Id == itemId))
                        pagesWithSections[key].Remove(itemId);
                    else
                        break;
                }
            }

            return pagesWithSections;
        }

        private static List<ReportableSection> GetPagesWithSections(Dictionary<int, List<int>> pages, List<ReportableSection> reportableSections)
        {
            //Dictionary<string, List<int>> result = new Dictionary<string, List<int>>();

            List<ReportableSection> result = new List<ReportableSection>();

            foreach (var page in pages)
            {
                foreach (var itemId in page.Value)
                {
                    var section = reportableSections.FirstOrDefault(item => item.Id == itemId);

                    //if it is section create section item and set up title property
                    if (section != null)
                    {
                        result.Add(new ReportableSection() { Id = itemId, Title = section.Title });
                    }
                    else
                    {
                        result.Last().Items.Add(itemId);
                    }
                }
            }

          
            return result;
        }

        /// <summary>
        /// Get the response template corresponding to the response with the specified GUID
        /// </summary>
        /// <param name="responseGUID"></param>
        /// <returns></returns>
        public static ResponseTemplate GetResponseTemplateFromResponseGUID(Guid responseGUID)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_RT_GetIDFromResponseGUID");
            command.AddInParameter("ResponseGUID", DbType.Guid, responseGUID);

            object result = db.ExecuteScalar(command);

            if (result == null || result == DBNull.Value)
            {
                return null;
            }

            return GetResponseTemplate((int)result);
        }

        /// <summary>
        /// Get the response template with the specified name.
        /// </summary>
        /// <param name="rtName">Name of the response template.</param>
        /// <param name="useCache">When false, always load a fresh copy of the response template.</param>
        /// <returns><see cref="ResponseTemplate"/> object.</returns>
        public static ResponseTemplate GetResponseTemplate(string rtName)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_RT_GetResponseTemplateIDFromName");
            command.AddInParameter("ResponseTemplateName", DbType.String, rtName);
            command.AddOutParameter("ResponseTemplateID", DbType.Int32, sizeof(int));

            db.ExecuteNonQuery(command);

            object result = command.GetParameterValue("ResponseTemplateID");

            if (result == null || result == DBNull.Value)
            {
                throw new TemplateDoesNotExist(rtName);
            }

            return GetResponseTemplate((int)result);
        }

        /// <summary>
        /// Gets a <see cref="DataSet"/> of ResponseTemplates available to the logged-in user.
        /// </summary>
        /// <param name="currentPrincipal">Principal making the request.</param>
        /// <param name="includeInactiveTemplates">Determines if inactive surveys should be included in the result set.</param>
        /// <param name="context"></param>
        /// <param name="totalCount">The count of available <see cref="ResponseTemplate"/>.</param>
        /// <returns>A <see cref="DataSet"/> containing one table.  </returns>
        public static DataSet GetAvailableResponseTemplates(ExtendedPrincipal currentPrincipal, bool includeInactiveTemplates, PaginationContext context, out int totalCount)
        {
            //TODO: Verify if this is needed
            totalCount = 0;
            return null;

            //            SelectQuery query = QueryFactory.GetAllResponseTemplatesQuery(includeInactiveTemplates, context);
            //            Database db = DatabaseFactory.CreateDatabase();
            //            DBCommandWrapper command = db.GetSqlStringCommandWrapper(query.ToString());
            //
            //            return FilterTemplatesAndFolders(
            //                db.ExecuteDataSet(command),
            //                currentPrincipal,
            //                context,
            //                out totalCount);
        }

        /// <summary>
        /// List templates and folders accessible to the specified user.
        /// </summary>
        /// <param name="currentPrincipal"></param>
        /// <param name="paginationContext"></param>
        /// <param name="includeActive"></param>
        /// <param name="includeInactive"></param>
        /// <returns></returns>
        /// <remarks>No checks are made for response limits and the like.</remarks>
        public static List<LightweightAccessControllable> ListAccessibleTemplatesAndFolders(
            CheckboxPrincipal currentPrincipal,
            PaginationContext paginationContext, bool includeActive, bool includeInactive)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command;

            //Shortcut to list all for system admins
            if (currentPrincipal.IsInRole("System Administrator"))
            {
                command = db.GetStoredProcCommandWrapper("ckbx_sp_Security_ListAccessibleSurveysAndFoldersAdmin");
                QueryHelper.AddPagingAndFilteringToCommandWrapper(command, paginationContext);
            }
            else
            {
                command = QueryHelper.CreateListAccessibleCommand(
                    "ckbx_sp_Security_ListAccessibleSurveysAndFolders",
                    currentPrincipal,
                    paginationContext);
            }

            command.AddInParameter("IncludeInactive", DbType.Boolean, includeInactive);
            command.AddInParameter("IncludeActive", DbType.Boolean, includeActive);

            var accessibleTemplatesAndFolders = new List<LightweightAccessControllable>();

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    //Check if the first table exists
                    bool doesTheFirstTableExist = true;
                    while (doesTheFirstTableExist && reader.Read())
                    {
                        doesTheFirstTableExist = reader.GetName(0) == "ResourceId";
                        if (doesTheFirstTableExist)
                        {
                            int resourceId = DbUtility.GetValueFromDataReader(reader, "ResourceId", -1);
                            string resourceType = DbUtility.GetValueFromDataReader(reader, "ResourceType", string.Empty);

                            if (resourceId > 0
                                && Utilities.IsNotNullOrEmpty(resourceType))
                            {
                                LightweightAccessControllable resource = resourceType.Equals("Form",
                                                                                             StringComparison.
                                                                                                 InvariantCultureIgnoreCase)
                                                                             ? GetLightweightResponseTemplate(resourceId)
                                                                             : FolderManager.GetLightweightFolder(
                                                                                 resourceId);

                                if (resource != null)
                                {
                                    accessibleTemplatesAndFolders.Add(resource);
                                }
                            }
                        }
                    }

                    //Now attempt to get page count, which will be present in second result set (if the first table exists)
                    if (!doesTheFirstTableExist || reader.NextResult()
                        && reader.Read())
                    {
                        paginationContext.ItemCount = DbUtility.GetValueFromDataReader(reader, "TotalItemCount", -1);
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            return accessibleTemplatesAndFolders;
        }

        /// <summary>
        /// List of favorite surveys accessible to the specified user.
        /// </summary>
        /// <param name="currentPrincipal"></param>
        /// <param name="paginationContext"></param>
        /// <returns></returns>
        public static List<LightweightAccessControllable> ListFavoriteSurveys(
            CheckboxPrincipal currentPrincipal,
            PaginationContext paginationContext)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command;

            if (currentPrincipal.IsInRole("System Administrator"))
            {
                command = QueryHelper.CreateListAccessibleCommand("ckbx_sp_FavoriteSurvey_GetListAdmin", currentPrincipal,
                    paginationContext);
            }
            else
            {
                command = QueryHelper.CreateListAccessibleCommand(
                    "ckbx_sp_FavoriteSurvey_GetList",
                    currentPrincipal,
                    paginationContext);
            }

            var listFavoriteTemplates = new List<LightweightAccessControllable>();

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    //Check if the first table exists
                    bool doesTheFirstTableExist = true;
                    while (doesTheFirstTableExist && reader.Read())
                    {
                        doesTheFirstTableExist = reader.GetName(0) == "ResourceId";
                        if (doesTheFirstTableExist)
                        {
                            int resourceId = DbUtility.GetValueFromDataReader(reader, "ResourceId", -1);
                            string resourceType = DbUtility.GetValueFromDataReader(reader, "ResourceType", string.Empty);

                            if (resourceId > 0
                                && Utilities.IsNotNullOrEmpty(resourceType))
                            {
                                LightweightAccessControllable resource = GetLightweightResponseTemplate(resourceId);
                                if (resource != null)
                                {
                                    listFavoriteTemplates.Add(resource);
                                }
                            }
                        }
                    }
                }
                finally
                {
                    reader.Close();
                }
            }
            return listFavoriteTemplates;
        }

        /// <summary>
        /// Add survey to list of favorite ones
        /// </summary>
        /// <param name="currentPrincipal"></param>
        /// <param name="surveyid"></param>
        /// <returns></returns>
        public static bool AddFavoriteSurvey(
            CheckboxPrincipal currentPrincipal,
            int surveyid, List<String> permissions)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command;
            command = db.GetStoredProcCommandWrapper("ckbx_sp_FavoriteSurvey_Add");
            command.AddInParameter("userID", DbType.String, currentPrincipal.Identity.Name);
            command.AddInParameter("responseTemplateID", DbType.String, surveyid);

            command.AddInParameter("FirstPermissionName", DbType.String, permissions.Count > 0 ? permissions[0] : string.Empty);
            command.AddInParameter("SecondPermissionName", DbType.String, permissions.Count > 1 ? permissions[1] : string.Empty);

            try
            {
                db.ExecuteNonQuery(command);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Remove survey from the list of favorite ones
        /// </summary>
        /// <param name="currentPrincipal"></param>
        /// <param name="surveyid"></param>
        /// <returns></returns>
        public static bool RemoveFavoriteSurvey(
            CheckboxPrincipal currentPrincipal,
            int surveyid)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command;
            command = db.GetStoredProcCommandWrapper("ckbx_sp_FavoriteSurvey_Remove");
            command.AddInParameter("userID", DbType.String, currentPrincipal.Identity.Name);
            command.AddInParameter("responseTemplateID", DbType.String, surveyid);
            try
            {
                db.ExecuteNonQuery(command);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Check if specified survey is favorite
        /// </summary>
        /// <param name="currentPrincipal"></param>
        /// <param name="surveyid"></param>
        /// <returns></returns>
        public static bool IsFavoriteSurvey(
            CheckboxPrincipal currentPrincipal,
            int surveyid)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command;
            command = db.GetStoredProcCommandWrapper("ckbx_sp_FavoriteSurvey_IsFavorite");
            command.AddInParameter("UniqueIdentifier", DbType.String, currentPrincipal.Identity.Name);
            command.AddInParameter("responseTemplateID", DbType.String, surveyid);
            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    //Check if the first table exists
                    bool doesTheFirstTableExist = true;
                    while (reader.Read())
                    {
                        doesTheFirstTableExist = reader.GetName(0) == "isFavorite";
                        if (doesTheFirstTableExist)
                        {
                            int isFavorite = DbUtility.GetValueFromDataReader(reader, "isFavorite", -1);
                            return (isFavorite > 0);
                        }
                    }
                    return false;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// List templates that are accessible to the specified user in a given folder.
        /// If no folder is specified it is assumed the listing is for the root folder.
        /// </summary>
        /// <param name="currentPrincipal"></param>
        /// <param name="ancestorFolderId"></param>
        /// <param name="paginationContext"></param>
        /// <param name="includeActive"></param>
        /// <param name="includeInactive"></param>
        /// <returns></returns>
        /// <remarks>No checks are made for response limits and the like.</remarks>
        public static List<LightweightAccessControllable> ListAccessibleTemplates(
            CheckboxPrincipal currentPrincipal,
            int? ancestorFolderId,
            PaginationContext paginationContext, bool includeActive, bool includeInactive)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command;

            if (currentPrincipal.IsInRole("System Administrator"))
            {
                command = db.GetStoredProcCommandWrapper("ckbx_sp_Security_ListAccessibleSurveysAdmin");
                QueryHelper.AddPagingAndFilteringToCommandWrapper(command, paginationContext);
            }
            else
            {
                //the case when we are looking for surveys only but the user can edit surveys and create reports - remove FormFolder.Read permission
                //because ckbx_sp_Security_ListAccessibleSurveys works for 2 permissions as a maximum
                if (paginationContext.Permissions.Count > 2)
                {
                    if (!currentPrincipal.IsInRole("Report Administrator") && !currentPrincipal.IsInRole("Report Editor"))
                        paginationContext.Permissions.Remove("Analysis.Create");
                    else
                        if (!currentPrincipal.IsInRole("Survey Administrator") && !currentPrincipal.IsInRole("Survey Editor"))
                            paginationContext.Permissions.Remove("Form.Edit");
                        else
                            paginationContext.Permissions.Remove("FormFolder.Read");
                }

                command = QueryHelper.CreateListAccessibleCommand(
                    "ckbx_sp_Security_ListAccessibleSurveys",
                    currentPrincipal,
                    paginationContext);
            }
            //Add ancestor folder id
            command.AddInParameter("AncestorFolder", DbType.Int32, ancestorFolderId);
            command.AddInParameter("IncludeInactive", DbType.Boolean, includeInactive);
            command.AddInParameter("IncludeActive", DbType.Boolean, includeActive);

            var accessibleTemplates = new List<LightweightAccessControllable>();

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    //Check if the first table exists
                    bool doesTheFirstTableExist = true;
                    while (doesTheFirstTableExist && reader.Read())
                    {
                        doesTheFirstTableExist = reader.GetName(0) == "ResponseTemplateID";
                        if (doesTheFirstTableExist)
                        {
                            int resourceId = DbUtility.GetValueFromDataReader(reader, "ResponseTemplateID", -1);

                            if (resourceId > 0)
                            {
                                LightweightAccessControllable resource = GetLightweightResponseTemplate(resourceId);

                                if (resource != null)
                                {
                                    accessibleTemplates.Add(resource);
                                }
                            }
                        }
                    }

                    //Now attempt to get page count, which will be present in second result set (if the first table exists)
                    if (!doesTheFirstTableExist || reader.NextResult()
                        && reader.Read())
                    {
                        paginationContext.ItemCount = DbUtility.GetValueFromDataReader(reader, "TotalItemCount", -1);
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            return accessibleTemplates;
        }

        /// <summary>
        /// List response templates accessible to the specified user.
        /// </summary>
        /// <param name="currentPrincipal"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static List<LightweightResponseTemplate> ListTakeableResponseTemplates(CheckboxPrincipal currentPrincipal, PaginationContext context)
        {
            //TODO: Handle system admin
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = null;

            string uniqueIdentifier = currentPrincipal == null ? string.Empty : Utilities.AdvancedHtmlEncode(currentPrincipal.Identity.Name);

            if (currentPrincipal != null && !(currentPrincipal is AnonymousRespondent))
            {
                command = db.GetStoredProcCommandWrapper("ckbx_sp_Security_ListAccessibleSurveys");
                //Add ancestor folder id

                command.AddInParameter("AncestorFolder", DbType.Int32, null);
            }
            else
            {
                command = db.GetStoredProcCommandWrapper("ckbx_sp_Security_ListTakeableSurveysAnonymous");
            }

            command.AddInParameter("UniqueIdentifier", DbType.String, uniqueIdentifier);
            command.AddInParameter("FirstPermissionName", DbType.String, "Form.Fill");
            command.AddInParameter("SecondPermissionName", DbType.String, context.Permissions.Count > 1 ? context.Permissions[1] : string.Empty);
            command.AddInParameter("RequireBothPermissions", DbType.Boolean, context.PermissionJoin == PermissionJoin.All);
            command.AddInParameter("UseAclExclusion", DbType.Boolean, ApplicationManager.AppSettings.AllowExclusionaryAclEntries);

            QueryHelper.AddPagingAndFilteringToCommandWrapper(command, context);

            var accessibleTemplates = new List<LightweightResponseTemplate>();

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    while (reader.Read())
                    {
                        int resourceId = DbUtility.GetValueFromDataReader(reader, "ResponseTemplateId", -1);

                        if (resourceId > 0)
                        {
                            LightweightResponseTemplate resource = GetLightweightResponseTemplate(resourceId);

                            if (resource != null)
                            {
                                accessibleTemplates.Add(resource);
                            }
                        }
                    }

                    if (reader.NextResult())
                    {
                        reader.Read();
                        int totalItemCount = DbUtility.GetValueFromDataReader(reader, "TotalItemCount", 0);
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            return accessibleTemplates;
        }

        /// <summary>
        /// Get ResponseTemplates and folders available to the logged-in user in a specific folder.
        /// </summary>
        /// <param name="currentPrincipal">Logged-in user.</param>
        /// <param name="parentFolderId">Folder id</param>
        /// <param name="context"></param>
        /// <param name="totalCount">Total items matching filter</param>
        /// <returns><see cref="DataSet"/> containing result.</returns>
        public static DataSet GetAvailableResponseTemplatesAndFolders(ExtendedPrincipal currentPrincipal, int? parentFolderId, PaginationContext context, out int totalCount)
        {
            //TODO: Verify if this is needed
            totalCount = 0;
            return null;

            //SelectQuery query = QueryFactory.GetAllTemplatesAndFoldersQuery(context, parentFolderId);
            //Database db = DatabaseFactory.CreateDatabase();
            //DBCommandWrapper command = db.GetSqlStringCommandWrapper(query.ToString());

            //return FilterTemplatesAndFolders(
            //    db.ExecuteDataSet(command),
            //    currentPrincipal,
            //    context,
            //    out totalCount);
        }

        //        /// <summary>
        //        /// Gets a <see cref="DataSet"/> of ResponseTemplates available to the logged-in user.
        //        /// </summary>
        //        /// <param name="currentPrincipal">Principal requesting access</param>
        //        /// <param name="context"></param>
        //        /// <param name="totalCount">The count of available <see cref="ResponseTemplate"/>.</param>
        //        /// <returns>A <see cref="DataSet"/> containing one table.</returns>
        //        public static DataSet GetTakeableResponseTemplates(ExtendedPrincipal currentPrincipal, PaginationContext context, out int totalCount)
        //        {
        //            //TODO: Verify if this is needed
        //            totalCount = 0;
        //            return null;
        //
        //            PaginationContext noPaging = new PaginationContext
        //            {
        //                FilterField = context.FilterField,
        //                FilterValue = context.FilterValue,
        //                SortField = context.SortField,
        //                SortAscending = context.SortAscending,
        //                Permissions = context.Permissions
        //            };
        //
        //            ////Get unpaged list of templates
        //            //DataSet permittedTemplatesData = GetAvailableResponseTemplates(currentPrincipal, false, noPaging, out totalCount);
        //
        //            ////Now filter & page and use activation status for filtering
        //            //return FilterTakeableResponseTemplates(currentPrincipal, permittedTemplatesData, context, out totalCount);
        //        }

        //TODO: Is this needed?
        ///// <summary>
        /////
        ///// </summary>
        ///// <param name="templateDataSet"></param>
        ///// <param name="currentPrincipal"></param>
        ///// <param name="context"></param>
        ///// <param name="totalCount"></param>
        ///// <returns></returns>
        ///// <remarks>Assumption is made that incoming data is already sorted.</remarks>
        //private static DataSet FilterTemplatesAndFolders(DataSet templateDataSet,
        //                                                 ExtendedPrincipal currentPrincipal,
        //                                                 PaginationContext context,
        //                                                 out int totalCount)
        //{
        //    int startIndex = context.GetStartIndex();
        //    int endIndex = context.GetEndIndex(templateDataSet.Tables[0].Rows.Count);

        //    //Now authorize based on permissions.  Include if any permissions match.
        //    var outputDs = new DataSet();
        //    DataTable outputTable = templateDataSet.Tables[0].Clone();
        //    outputDs.Tables.Add(outputTable);

        //    DataRow[] inputRows = templateDataSet.Tables[0].Select();

        //    //Total templates/folders with access
        //    int totalPermitted = 0;

        //    foreach (DataRow inputRow in inputRows)
        //    {
        //        var itemType = DbUtility.GetValueFromDataRow<string>(inputRow, "ItemType", null);
        //        int itemId = DbUtility.GetValueFromDataRow(inputRow, "ItemID", -1);

        //        if (itemId > 0 && Utilities.IsNotNullOrEmpty(itemType))
        //        {
        //            if (AuthorizeItem(currentPrincipal, itemType, itemId, context.Permissions))
        //            {
        //                if (totalPermitted >= startIndex && totalPermitted <= endIndex)
        //                {
        //                    outputTable.ImportRow(inputRow);
        //                }
        //                totalPermitted++;
        //            }
        //        }
        //    }

        //    totalCount = totalPermitted;

        //    return outputDs;
        //}

        //TODO: Needed?
        ///// <summary>
        ///// Authorize the survey item
        ///// </summary>
        ///// <param name="currentPrincipal"></param>
        ///// <param name="itemType"></param>
        ///// <param name="itemId"></param>
        ///// <param name="permissions"></param>
        ///// <returns></returns>
        //private static bool AuthorizeItem(ExtendedPrincipal currentPrincipal, string itemType, int itemId, IEnumerable<string> permissions)
        //{
        //    IAccessControllable controllableResource;

        //    if (itemType.Equals("Folder", StringComparison.InvariantCultureIgnoreCase))
        //    {
        //        var f = new FormFolder();
        //        f.Load(itemId);

        //        controllableResource = f;
        //    }
        //    else
        //    {
        //        controllableResource = GetLightweightResponseTemplate(itemId);
        //    }

        //    if (controllableResource == null)
        //    {
        //        return false;
        //    }

        //    IAuthorizationProvider authorizationProvider = AuthorizationFactory.GetAuthorizationProvider();

        //    return permissions.Any(permission => authorizationProvider.Authorize(currentPrincipal, controllableResource, permission));
        //}

        /// <summary>
        /// Filter the list of available response templates to remove those that the user is not allowed to take due
        /// to response count and/or activation date limitations
        /// </summary>
        /// <param name="currentPrincipal">Principal requesting access</param>
        /// <param name="takeableTemplates">A list of all active and accessible response templates</param>
        /// <param name="context">The paging context</param>
        /// <returns></returns>
        private static List<LightweightResponseTemplate> FilterTakeableResponseTemplates(
            CheckboxPrincipal currentPrincipal,
            List<LightweightResponseTemplate> takeableTemplates,
            PaginationContext context)
        {
            //For a survey to be included in the filtered list:
            //  1) Active flag must be set
            //  2) Current date must be in activation date range
            //  3) Current survey must be in proper page
            //  4) The survey security is not Invitation only
            //  5) The survey has not reached the maximum number of responses
            //  6) The user has not reached their maximum number of allowed responses

            var results = new List<LightweightResponseTemplate>();

            if (currentPrincipal != null && currentPrincipal.IsInRole("System Administrator"))
            {
                context.ItemCount = takeableTemplates.Count;
                return takeableTemplates;
            }

            //The id of the invitation only survey security type
            const int invitationSecurityTypeId = 5;

            int totalIncluded = 0;

            if (takeableTemplates != null && takeableTemplates.Count > 0)
            {
                //Figure out if a survey is on the page.  For counting, we only should count the
                // surveys that would be included.

                //Figure out how many surveys appear on pages before the current page
                int priorPageSurveyCount = context.GetStartIndex();
                int rowsAdded = 0;

                foreach (LightweightResponseTemplate template in takeableTemplates)
                {
                    int id = template.ID;
                    var securityTypeId = (int)template.SecurityType;
                    bool isActive = template.IsActive;

                    //Check active flag
                    if (isActive && securityTypeId != invitationSecurityTypeId)
                    {
                        DateTime? activationStart = template.ActivationStartDate;
                        DateTime? activationEnd = template.ActivationEndDate;

                        DateTime now = DateTime.Now;

                        //Check activation start & end
                        if ((!activationStart.HasValue || now >= activationStart.Value) && (!activationEnd.HasValue || now <= activationEnd.Value))
                        {
                            // Check if more responses allowed
                            //  Check total responses for survey & total responses for user
                            int? maxTotalResponses = template.MaxTotalResponses;
                            int? maxResponsesPerUser = template.MaxResponsesPerUser;

                            if (MoreResponsesAllowed(id, maxTotalResponses, maxResponsesPerUser, currentPrincipal, template.AnonymizeResponses))
                            {
                                totalIncluded++;

                                if (context.PageSize > 0
                                    && totalIncluded > priorPageSurveyCount
                                    && context.PageSize > rowsAdded)
                                {
                                    //Import the row if we haven't reached the total page count
                                    results.Add(template);
                                    rowsAdded++;
                                }
                            }
                        }
                    }
                }
            }

            context.ItemCount = totalIncluded;

            return results;
        }

        /// <summary>
        /// Determine if the specified user may submit more responses
        /// </summary>
        /// <param name="templateID"></param>
        /// <param name="maxResponsesPerUser"></param>
        /// <param name="maxTotalResponses"></param>
        /// <param name="principal"></param>
        /// <param name="isAnonymized"></param>
        /// <returns></returns>
        public static bool MoreResponsesAllowed(int templateID, int? maxTotalResponses, int? maxResponsesPerUser, ExtendedPrincipal principal, bool isAnonymized)
        {
            return ((MoreResponsesAllowedForTemplate(templateID, maxTotalResponses) && (MoreResponsesAllowedForUser(templateID, maxResponsesPerUser, principal, isAnonymized))));
        }

        /// <summary>
        /// Determine if the template allows more responses
        /// </summary>
        /// <param name="templateID"></param>
        /// <param name="maxResponseCount"></param>
        /// <returns></returns>
        private static bool MoreResponsesAllowedForTemplate(int templateID, int? maxResponseCount)
        {
            if (maxResponseCount.HasValue && GetTemplateResponseCount(templateID) >= maxResponseCount.Value)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Determine if more responses are allowed for a given user
        /// </summary>
        /// <param name="templateID"></param>
        /// <param name="maxResponsesPerUser"></param>
        /// <param name="currentPrincipal"></param>
        /// <param name="isAnonymized"></param>
        /// <returns></returns>
        private static bool MoreResponsesAllowedForUser(int templateID, int? maxResponsesPerUser, ExtendedPrincipal currentPrincipal, bool isAnonymized)
        {
            if (!maxResponsesPerUser.HasValue)
            {
                return true;
            }

            int userResponseCount = 0;

            if (currentPrincipal is AnonymousRespondent)
            {
                //Get the current response total for this user and in general
                Database db = DatabaseFactory.CreateDatabase();
                DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Response_CountForAnonRespondent");
                command.AddInParameter("RespondentGUID", DbType.Guid, ((AnonymousRespondent)currentPrincipal).UserGuid);
                command.AddInParameter("ResponseTemplateID", DbType.Int32, templateID);
                command.AddOutParameter("UserCount", DbType.Int32, 4);

                db.ExecuteNonQuery(command);

                object returnValue = command.GetParameterValue("UserCount");

                if (returnValue != DBNull.Value)
                {
                    userResponseCount = (int)returnValue;
                }
            }
            else if (isAnonymized)
            {
                //Get the current response total for this user and in general
                Database db = DatabaseFactory.CreateDatabase();
                DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Response_CountForAnonymizedUser");

                command.AddInParameter("ResumeKey", DbType.String, Utilities.GetSaltedMd5Hash(currentPrincipal.Identity.Name));
                command.AddInParameter("ResponseTemplateID", DbType.Int32, templateID);
                command.AddOutParameter("UserCount", DbType.Int32, 4);

                db.ExecuteNonQuery(command);

                object returnValue = command.GetParameterValue("UserCount");

                if (returnValue != DBNull.Value)
                {
                    userResponseCount = (int)returnValue;
                }
            }
            else
            {
                //Get the current response total for this user and in general
                Database db = DatabaseFactory.CreateDatabase();
                DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Response_CountForUser");

                command.AddInParameter("UniqueIdentifier", DbType.String, currentPrincipal.Identity.Name);
                command.AddInParameter("ResponseTemplateID", DbType.Int32, templateID);
                command.AddOutParameter("UserCount", DbType.Int32, 4);

                db.ExecuteNonQuery(command);

                object returnValue = command.GetParameterValue("UserCount");

                if (returnValue != DBNull.Value)
                {
                    userResponseCount = (int)returnValue;
                }
            }

            return (userResponseCount < maxResponsesPerUser.Value);
        }

        /// <summary>
        /// Get the number of responses for a response template
        /// </summary>
        /// <param name="templateID"></param>
        /// <returns></returns>
        private static int GetTemplateResponseCount(int templateID)
        {
            //Get the current response total

            return GetTemplateResponseCount(templateID, ApplicationManager.AppSettings.IncludeIncompleteResponsesToTotalAmount, false);
        }

        /// <summary>
        /// Get the number of responses for a response template
        /// </summary>
        /// <param name="templateID"></param>
        /// <param name="includeIncoplete></param>
        /// <param name="includeTest></param>
        /// <returns></returns>
        public static int GetTemplateResponseCount(int templateID, bool includeIncoplete, bool includeTest)
        {
            //Get the current response total
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Response_Count");
            command.AddInParameter("ResponseTemplateID", DbType.Int32, templateID);
            command.AddInParameter("IncludeIncomplete", DbType.Boolean, includeIncoplete);
            command.AddInParameter("IncludeTest", DbType.Boolean, includeTest);
            command.AddOutParameter("TotalCount", DbType.Int32, 4);

            db.ExecuteNonQuery(command);

            object responseCount = command.GetParameterValue("TotalCount");

            if (responseCount != DBNull.Value)
            {
                return (int)responseCount;
            }

            return 0;
        }

        /// <summary>
        /// Get the number of test responses for a response template
        /// </summary>
        /// <param name="templateID"></param>
        /// <returns></returns>
        public static int GetTestResponseCount(int templateID)
        {
            //Get the current response total
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_RT_TestResponseCount");
            command.AddInParameter("ResponseTemplateID", DbType.Int32, templateID);
            command.AddOutParameter("Count", DbType.Int32, 4);

            db.ExecuteNonQuery(command);

            object responseCount = command.GetParameterValue("Count");

            if (responseCount != DBNull.Value)
            {
                return (int)responseCount;
            }

            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageID"></param>
        /// <returns></returns>
        public static int GetPagePositionById(int pageID)
        {
            //Get the current response total
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_RT_GetPagePositionById");
            command.AddInParameter("PageID", DbType.Int32, pageID);

            return (int)db.ExecuteScalar(command);
        }

        /// <summary>
        /// Return a boolean indicating whether a response template with the specified name exists
        /// </summary>
        /// <param name="responseTemplateName"></param>
        /// <returns></returns>
        public static bool ResponseTemplateExists(string responseTemplateName)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_RT_GetResponseTemplateIDFromName");
            command.AddInParameter("ResponseTemplateName", DbType.String, responseTemplateName);
            command.AddOutParameter("ResponseTemplateID", DbType.Int32, 4);

            db.ExecuteNonQuery(command);

            object responseTemplateID = command.GetParameterValue("ResponseTemplateID");

            if (responseTemplateID != null && responseTemplateID != DBNull.Value)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Return a boolean indicating whether a response template has one or more response.
        /// A response is any entry that was not made in Test Mode or
        /// </summary>
        /// <param name="responseTemplateId"></param>
        /// <returns></returns>
        public static bool HasResponses(int responseTemplateId)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_RT_HasResponses");
            command.AddInParameter("ResponseTemplateID", DbType.String, responseTemplateId);
            command.AddOutParameter("Count", DbType.Int32, 4);

            db.ExecuteNonQuery(command);

            object responseCount = command.GetParameterValue("Count");

            if (responseCount != DBNull.Value)
            {
                return (int)responseCount > 0;
            }

            return false;
        }

        /// <summary>
        /// Mark a template as being updated
        /// </summary>
        /// <param name="templateID"></param>
        /// <returns></returns>
        public static void MarkTemplateUpdated(Int32 templateID)
        {
            //mark basic items data updated
            var rt = _templateCache[templateID.ToString()] as ResponseTemplate;
            if (rt != null)
                ItemConfigurationManager.MarkBasicItemsDataUpdated(rt);
            else
                ItemConfigurationManager.MarkBasicItemsDataUpdated(templateID);

            var now = DateTime.Now;
            //Remove the template from the cache
            _templateCache.Remove(templateID.ToString());
            _templateCache.Remove("Lightweight" + templateID);
            _templateModificationDateCache.Remove(templateID.ToString());
            _templateModificationDateCache.Add(templateID.ToString(), now);

            //clean up rules cache
            SurveyMetaDataProxy.RemoveRulesEngineFromCache(templateID);

            //Update the modified date of the template in the database
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Template_SetModifiedDate");
            command.AddInParameter("TemplateID", DbType.Int32, templateID);
            command.AddInParameter("ModifiedDate", DbType.DateTime, now);

            db.ExecuteNonQuery(command);
        }

        /// <summary>
        /// Return a boolean value if a response template has been updated since the
        /// refernece date.
        /// </summary>
        /// <param name="templateID"></param>
        /// <param name="referenceDate"></param>
        /// <returns></returns>
        public static bool CheckTemplateUpdated(Int32 templateID, DateTime? referenceDate)
        {
            var oUpdatedDate = _templateModificationDateCache[templateID.ToString()];
            if (oUpdatedDate != null && oUpdatedDate is DateTime && referenceDate != null)
            {
                DateTime updatedDate = (DateTime)oUpdatedDate;
                if (updatedDate <= referenceDate)
                    return false;
            }

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Template_CheckUpdated");
            command.AddInParameter("TemplateId", DbType.Int32, templateID);
            command.AddInParameter("ReferenceDate", DbType.DateTime, referenceDate);
            command.AddOutParameter("Updated", DbType.Boolean, 4);

            //Run query and get output value
            db.ExecuteNonQuery(command);

            object outVal = command.GetParameterValue("Updated");

            //Err on the side of caution and return template updated
            // to cause a reload.
            if (outVal == null || outVal == DBNull.Value)
            {
                return true;
            }

            return (bool)outVal;
        }

        /// <summary>
        /// Return a boolean value if a response template has been updated since the
        /// refernece date.
        /// </summary>
        /// <param name="templateID"></param>
        /// <param name="referenceDate"></param>
        /// <returns></returns>
        public static bool CheckTemplateResponsesUpdated(Int32 templateID, DateTime? referenceDate)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Template_CheckResponsesUpdated");
            command.AddInParameter("TemplateId", DbType.Int32, templateID);
            command.AddInParameter("ReferenceDate", DbType.DateTime, referenceDate);
            command.AddOutParameter("Updated", DbType.Boolean, 4);

            //Run query and get output value
            db.ExecuteNonQuery(command);

            object outVal = command.GetParameterValue("Updated");

            //Err on the side of caution and return template updated
            // to cause a reload.
            if (outVal == null || outVal == DBNull.Value)
            {
                return true;
            }

            return (bool)outVal;
        }

        /// <summary>
        /// Return a boolean value if an item has been updated since the
        /// reference date.
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="referenceDate"></param>
        /// <returns></returns>
        public static bool CheckItemDataUpdated(Int32 itemId, DateTime? referenceDate)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_CheckUpdated");
            command.AddInParameter("ItemId", DbType.Int32, itemId);
            command.AddInParameter("ReferenceDate", DbType.DateTime, referenceDate);
            command.AddOutParameter("Updated", DbType.Boolean, 4);

            //Run query and get output value
            db.ExecuteNonQuery(command);

            object outVal = command.GetParameterValue("Updated");

            //Err on the side of caution and return template updated
            // to cause a reload.
            if (outVal == null || outVal == DBNull.Value)
            {
                return true;
            }

            return (bool)outVal;
        }

        /// <summary>
        /// Copy a response template and return the copied template
        /// </summary>
        /// <param name="templateID">ID of the template to copy.</param>
        /// <param name="name">suggested name for new template, could be null</param>
        /// <param name="owner">Owner of the new template</param>
        /// <param name="languageCode">Language code to use for survey name</param>
        /// <returns>A copy of the specified response template.</returns>
        public static ResponseTemplate CopyTemplate(Int32 templateID, string name, ExtendedPrincipal owner, string languageCode)
        {
            //Get template
            var survey = GetResponseTemplate(templateID);
            
            if (string.IsNullOrEmpty(name))
                name = survey.Name;

            ////Copy the template using the same serialization / deserialization as the export
            var stream = new MemoryStream();
            var writer = new XmlTextWriter(stream, Encoding.UTF8);

            survey.Export(writer);
            writer.Flush();

            stream.Seek(0, SeekOrigin.Begin);

            var xmlDoc = new XmlDocument();
            xmlDoc.Load(stream);

            //Create a new survey
            ResponseTemplate newSurvey = CreateResponseTemplate(owner as CheckboxPrincipal);
            newSurvey.Import((CheckboxPrincipal)owner, xmlDoc.DocumentElement, null, null, null, true);

            newSurvey.Name = GetUniqueName(name, languageCode);

            newSurvey.BehaviorSettings.IsActive = survey.BehaviorSettings.IsActive;

            //Set default security type
            newSurvey.BehaviorSettings.SecurityType = survey.BehaviorSettings.SecurityType;

            ////Set the new survey's style template to be the default style template (if it exists)
            var defaultStyleTemplateId = ApplicationManager.AppSettings.DefaultStyleTemplate;

            newSurvey.StyleSettings.StyleTemplateId = survey.StyleSettings.StyleTemplateId;

            if (defaultStyleTemplateId > 0 && newSurvey.StyleSettings.StyleTemplateId != 0)
            {
                newSurvey.StyleSettings.StyleTemplateId = defaultStyleTemplateId;
            }

            newSurvey.StyleSettings.MobileStyleId = survey.StyleSettings.MobileStyleId;
            
            //Save survey
            newSurvey.Save();

            return newSurvey;
        }

        /// <summary>
        /// Determines if a survey name is already in use and returns the next available unique name.
        /// The format of the unique name is Survey_Name Copy #.
        /// Returns the provided survey name if it is unique.
        /// </summary>
        /// <param name="surveyName"></param>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public static string GetUniqueName(string surveyName, string languageCode)
        {
            int copyCount = 0;
            string newSurveyName = surveyName;

            if (string.IsNullOrEmpty(languageCode))
                languageCode = TextManager.DefaultLanguage;

            while (ResponseTemplateExists(newSurveyName))
            {
                copyCount++;

                string duplicateText = TextManager.GetText("/common/duplicate", languageCode);
                int duplicateSuffixLength = duplicateText.Length + copyCount.ToString().Length + 2;

                if (surveyName.Length + duplicateSuffixLength > 255)
                {
                    surveyName = surveyName.Remove(255 - duplicateSuffixLength);
                }

                newSurveyName = string.Format("{0} {1} {2}", surveyName, duplicateText, copyCount);
            }

            return newSurveyName;
        }

        /// <summary>
        /// Change the specified ReponseTemplate's CreatedBy field
        /// </summary>
        /// <param name="rtId">Id of the ResponseTemplate.</param>
        /// <param name="newOwner">Name of the new owner.</param>
        public static bool ChangeResponseTemplateOwner(int rtId, string newOwner)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_RT_ChangeOwner");
            command.AddInParameter("ResponseTemplateId", DbType.Int32, rtId);
            command.AddInParameter("CreatedBy", DbType.String, newOwner);

            int i = db.ExecuteNonQuery(command);

            MarkTemplateUpdated(rtId);

            return i > 0;
        }

        /// <summary>
        /// Initialize survey acl and policy depending on security type
        /// </summary>
        public static void InitializeResponseTemplateAclAndPolicy(ResponseTemplate responseTemplate, ExtendedPrincipal principal)
        {
            if (ApplicationManager.UseSimpleSecurity)
            {
                AccessManager.AddPermissionsToDefaultPolicy(responseTemplate, principal, "Form.Administer", "Form.Create", "Form.Delete", "Form.Edit", "Form.Fill", "Analysis.Create", "Analysis.Responses.View", "Analysis.Responses.Export", "Analysis.Responses.Edit", "Analysis.ManageFilters");
                AccessManager.AddPermissionsToAllAclEntries(responseTemplate, principal, "Form.Fill");
            }
            else
            {
                switch (responseTemplate.BehaviorSettings.SecurityType)
                {
                    case SecurityType.AllRegisteredUsers:
                        //Add the everyone group to the acl
                        AccessManager.AddEveryoneGroupWithPermissions(responseTemplate, principal, "Form.Fill");
                        AccessManager.AddPermissionsToAllAclEntries(responseTemplate, principal, "Form.Fill");
                        AccessManager.RemovePermissionsFromDefaultPolicy(responseTemplate, principal, "Form.Fill");
                        break;

                    case SecurityType.Public:
                    case SecurityType.PasswordProtected:
                    case SecurityType.InvitationOnly:
                        //Add Form.Fill to the default policy
                        AccessManager.AddPermissionsToDefaultPolicy(responseTemplate, principal, "Form.Fill");
                        AccessManager.AddPermissionsToAllAclEntries(responseTemplate, principal, "Form.Fill");
                        break;

                    case SecurityType.AccessControlList:
                        //Remove fill permission from the default policy
                        AccessManager.RemovePermissionsFromDefaultPolicy(responseTemplate, principal, "Form.Fill");
                        break;

                    default:
                        break;
                }

                switch (responseTemplate.BehaviorSettings.ReportSecurityType)
                {
                    case ReportSecurityType.DetailsPrivate:
                        //Remove all default policy and acl permissions
                        AccessManager.RemovePermissionsFromDefaultPolicy(responseTemplate, principal, "Analysis.Create", "Analysis.Delete", "Analysis.Edit", "Analysis.ManageFilters");
                        AccessManager.RemovePermissionsFromAllAclEntries(responseTemplate, principal, responseTemplate.CreatedBy, "Analysis.Create", "Analysis.Delete", "Analysis.Edit", "Analsyis.ManageFilters");

                        AccessManager.AddPermissionsToAllAclEntries(responseTemplate, principal, "Analysis.Run", "Analysis.Responses.View", "Analysis.Responses.Export");
                        AccessManager.AddPermissionsToDefaultPolicy(responseTemplate, principal, "Analysis.Run", "Analysis.Responses.View", "Analysis.Responses.Export");

                        break;

                    case ReportSecurityType.SummaryPrivate:
                        //Remove all default policy and acl permissions
                        AccessManager.RemovePermissionsFromDefaultPolicy(responseTemplate, principal, "Analysis.Create", "Analysis.Delete", "Analysis.Edit", "Analysis.ManageFilters", "Analysis.Responses.View", "Analysis.Responses.Export");
                        AccessManager.RemovePermissionsFromAllAclEntries(responseTemplate, principal, responseTemplate.CreatedBy, "Analysis.Create", "Analysis.Delete", "Analysis.Edit", "Analsyis.ManageFilters", "Analysis.Responses.View", "Analysis.Responses.Export");

                        break;

                    case ReportSecurityType.SummaryPublic:
                        //Make sure everyone has Analysis.Run, but not View Responses or Export Responses
                        AccessManager.AddPermissionsToDefaultPolicy(responseTemplate, principal, "Analysis.Run", "Analysis.Create", "Analysis.Delete", "Analysis.Edit", "Analysis.ManageFilters");
                        AccessManager.AddPermissionsToAllAclEntries(responseTemplate, principal, "Analysis.Run", "Analysis.Create", "Analysis.Delete", "Analysis.Edit", "Analysis.ManageFilters");

                        //Remove any extraneous entries, if any
                        AccessManager.RemovePermissionsFromDefaultPolicy(responseTemplate, principal, "Analysis.Responses.View", "Analysis.Responses.Export");
                        AccessManager.RemovePermissionsFromAllAclEntries(responseTemplate, principal, responseTemplate.CreatedBy, "Analysis.Responses.View", "Analysis.Responses.Export");

                        break;

                    case ReportSecurityType.DetailsPublic:
                        AccessManager.AddPermissionsToDefaultPolicy(responseTemplate, principal, "Analysis.Run", "Analysis.Create", "Analysis.Delete", "Analysis.Edit", "Analysis.ManageFilters", "Analysis.Responses.View", "Analysis.Responses.Export");
                        AccessManager.AddPermissionsToAllAclEntries(responseTemplate, principal, "Analysis.Run", "Analysis.Create", "Analysis.Delete", "Analysis.Edit", "Analysis.ManageFilters", "Analysis.Responses.View", "Analysis.Responses.Export");

                        break;

                    case ReportSecurityType.SummaryRegisteredUsers:
                        AccessManager.AddEveryoneGroupWithPermissions(responseTemplate, principal, "Analysis.Run", "Analysis.Create", "Analysis.Delete", "Analysis.Edit", "Analysis.ManageFilters");
                        AccessManager.AddPermissionsToAllAclEntries(responseTemplate, principal, "Analysis.Run", "Analysis.Create", "Analysis.Delete", "Analysis.Edit", "Analysis.ManageFilters");

                        AccessManager.RemovePermissionsFromDefaultPolicy(responseTemplate, principal, "Analysis.Responses.View", "Analysis.Responses.Export");
                        AccessManager.RemovePermissionsFromAllAclEntries(responseTemplate, principal, responseTemplate.CreatedBy, "Analysis.Responses.View", "Analysis.Responses.Export");

                        break;

                    case ReportSecurityType.DetailsRegisteredUsers:
                        AccessManager.AddEveryoneGroupWithPermissions(responseTemplate, principal, "Analysis.Run", "Analysis.Create", "Analysis.Delete", "Analysis.Edit", "Analysis.ManageFilters", "Analysis.Responses.View", "Analysis.Responses.Export");
                        AccessManager.AddPermissionsToAllAclEntries(responseTemplate, principal, "Analysis.Run", "Analysis.Create", "Analysis.Delete", "Analysis.Edit", "Analysis.ManageFilters", "Analysis.Responses.View", "Analysis.Responses.Export");
                        AccessManager.RemovePermissionsFromDefaultPolicy(responseTemplate, principal, "Analysis.Create", "Analysis.Delete", "Analysis.Edit", "Analysis.ManageFilters", "Analysis.Responses.View", "Analysis.Responses.Export");
                        break;

                    case ReportSecurityType.SummaryAcl:
                        //RUN/CREATE/DELETE/EDIT permission was added by FormViewerSecurityView.aspx
                        AccessManager.RemovePermissionsFromDefaultPolicy(responseTemplate, principal, "Analysis.Create", "Analysis.Delete", "Analysis.Edit", "Analysis.ManageFilters", "Analysis.Responses.View", "Analysis.Responses.Export");
                        AccessManager.RemovePermissionsFromAllAclEntries(responseTemplate, principal, responseTemplate.CreatedBy, "Analysis.Responses.View", "Analysis.Responses.Export");
                        break;

                    case ReportSecurityType.DetailsAcl:
                        //Remove permissions from default policy
                        AccessManager.RemovePermissionsFromDefaultPolicy(responseTemplate, principal, "Analysis.Create", "Analysis.Delete", "Analysis.Edit", "Analysis.ManageFilters", "Analysis.Responses.View", "Analysis.Responses.Export");

                        //Add details view to acl entries -- RUN/CREATE/DELETE/EDIT permission was added by FormViewerSecurityView.aspx
                        AccessManager.AddPermissionsToAllAclEntries(responseTemplate, principal, "Analysis.Responses.View", "Analysis.Responses.Export");
                        break;
                }
            }
        }

        /// <summary>
        /// Get the Id of the folder tha contains the specified response template.  If
        /// the template is not in a folder, a null value is returned.
        /// </summary>
        /// <param name="responseTemplateId">ID of response template to get folder parent for.</param>
        /// <returns>Database ID of folder, if any, that contains the response template.</returns>
        public static int? GetResponseTemplateParentFolderId(int responseTemplateId)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_RT_GetParent");
            command.AddInParameter("ResponseTemplateId", DbType.Int32, responseTemplateId);
            command.AddOutParameter("FolderId", DbType.Int32, 4);

            db.ExecuteNonQuery(command);

            object folderIdObject = command.GetParameterValue("FolderId");

            if (folderIdObject != null && folderIdObject != DBNull.Value)
            {
                return (int)folderIdObject;
            }

            return null;
        }

        /// <summary>
        /// Get the name of the folder tha contains the specified response template.  If
        /// the template is not in a folder, an empty string value is returned.
        /// </summary>
        /// <param name="responseTemplateId">ID of response template to get folder parent for.</param>
        /// <returns>Name of folder, if any, that contains the response template.</returns>
        public static string GetResponseTemplateParentFolderName(int responseTemplateId)
        {
            string parentName = string.Empty;
            int? folderId = GetResponseTemplateParentFolderId(responseTemplateId);

            if (folderId.HasValue)
            {
                var parentFolder = new FormFolder();
                parentFolder.Load(folderId.Value);

                parentName = parentFolder.Name ?? string.Empty;
            }

            return parentName;
        }

        /// <summary>
        /// Get a security editor for a response template but without the cost of loading a full survey.
        /// </summary>
        /// <param name="responseTemplateId"></param>
        /// <returns></returns>
        public static SecurityEditor GetLightweightResponseTemplateSecurityEditor(int responseTemplateId)
        {
            LightweightResponseTemplate lightweightRt = GetLightweightResponseTemplate(responseTemplateId);

            if (lightweightRt != null)
            {
                return lightweightRt.GetEditor();
            }

            return null;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <param name="name"></param>
        /// <param name="creator"></param>
        /// <param name="folder"></param>
        /// <param name="styleTemplateId"></param>
        /// <param name="mobileTemplateId"></param>
        /// <param name="progressKey"></param>
        /// <param name="progressLanguage"></param>
        /// <returns></returns>
        public static ResponseTemplate ImportResponseTemplate(XmlDocument xmlDoc, string name, CheckboxPrincipal creator, Folder folder, int? styleTemplateId, int? mobileTemplateId, string progressKey, string progressLanguage)
        {
            //Serialize the response template
            var template = CreateResponseTemplate(creator);

            template.ModifiedBy = creator.Identity.Name;
            template.Import(creator, xmlDoc.DocumentElement, progressKey, progressLanguage);
            template.Name = name;

            //Set default security type
            //template.SecurityType = ApplicationManager.AppSettings.DefaultSurveySecurityType;

            //Set the style template to the default style (if it exists)
            //Set the new survey's style template to be the default style template (if it exists)
            var surveyStyleId = styleTemplateId ?? ApplicationManager.AppSettings.DefaultStyleTemplate;

            if (surveyStyleId > 0)
            {
                template.StyleSettings.StyleTemplateId = surveyStyleId;
            }

            var surveyMobileStyleId = mobileTemplateId != null ? (int)mobileTemplateId : 0;

            if (surveyMobileStyleId > 0)
            {
                template.StyleSettings.MobileStyleId = surveyMobileStyleId;
            }

            template.Save();

            MarkTemplateUpdated(template.ID.Value);

            if (folder != null)
            {
                folder.Add(template);
            }

            return template;
        }

        #endregion Retrieve

        #region Settings

        ///<summary>
        ///</summary>
        ///<param name="responseTemplateId"></param>
        ///<returns></returns>
        public static bool GetShowAsterisksSetting(int responseTemplateId)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_RT_GetShowAsterisksSetting");
            command.AddInParameter("ResponseTemplateId", DbType.Int32, responseTemplateId);
            command.AddOutParameter("ShowAsterisks", DbType.Boolean, 4);

            //Run query and get output value
            db.ExecuteNonQuery(command);

            object outVal = command.GetParameterValue("ShowAsterisks");

            if (outVal == null || outVal == DBNull.Value)
                return true;

            return (bool)outVal;
        }

        #endregion Settings

        /// <summary>
        /// Returns a response template by item ID
        /// </summary>
        /// <param name="ItemID"></param>
        /// <returns></returns>
        public static ResponseTemplate GetResponseTemplateByItemID(int ItemID)
        {
            int ResponseTemplateID = GetResponseTemplateIDByItemID(ItemID);

            return GetResponseTemplate(ResponseTemplateID);
        }

        /// <summary>
        /// Returns a response template ID by item ID
        /// </summary>
        /// <param name="ItemID"></param>
        /// <returns></returns>
        public static int GetResponseTemplateIDByItemID(int ItemID)
        {
            int ResponseTemplateID = 0;
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_RT_GetByItem");
            command.AddInParameter("ItemID", DbType.Int32, ItemID);
            command.AddOutParameter("ResponseTemplateID", DbType.Int32, 4);

            db.ExecuteNonQuery(command);

            ResponseTemplateID = (int)command.GetParameterValue("ResponseTemplateID");

            return ResponseTemplateID;
        }
    }
}