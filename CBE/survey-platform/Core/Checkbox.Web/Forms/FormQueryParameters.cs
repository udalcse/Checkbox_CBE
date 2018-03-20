using System;
using System.Web;
using Checkbox.Analytics;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Invitations;
using Checkbox.Management;
using Checkbox.Users;
using Checkbox.Wcf.Services.Proxies;

namespace Checkbox.Web.Forms
{
    /// <summary>
    /// Simple container for reading variables from query string.
    /// </summary>
    public class FormQueryParameters
    {
        ///<summary>
        ///</summary>
        public const string TEST_SESSION_EXTERNAL_UID_KEY = "SurveyTestExternalUser";
        /// <summary>
        /// Query string survey guid.
        /// </summary>
        [QueryParameter("s")]
        public Guid? SurveyGuid { get; set; }

        /// <summary>
        /// Querystring response session guid
        /// </summary>
        [QueryParameter("r", null, IsDefaultUsedForInvalid = true)]
        public Guid? ResponseGuid { get; set; }

        /// <summary>
        /// Querystring response session guid
        /// </summary>
        [QueryParameter("u", null, IsDefaultUsedForInvalid=true)]
        public Guid? UserGuid { get; set; }

        /// <summary>
        /// Print/Export mode
        /// </summary>
        [QueryParameter("print")]
        public string Print { get; set; }

        
        [QueryParameter("restoreUser")]
        public string RestoreUser { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ExportMode ExportMode
        {
            get
            {
                if (Print == null)
                    return ExportMode.None;

                switch (Print.ToLower())
                {
                    case "pdf":
                        return ExportMode.Pdf;
                    case "clientpdf":
                        return ExportMode.ClientPdf;
                    case "default":
                        return ExportMode.Default;
                    default:
                        return ExportMode.None;
                }
            }
        }

        /// <summary>
        /// Querystring response session guid
        /// </summary>
        public string ExternalUid
        {
            get
            {
                if (IsTest 
                        && !UserGuid.HasValue 
                        && HttpContext.Current != null 
                        && HttpContext.Current.Session != null
                        && HttpContext.Current.Session[TEST_SESSION_EXTERNAL_UID_KEY] != null)
                {
                    return HttpContext.Current.Session[TEST_SESSION_EXTERNAL_UID_KEY] as string;
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string AuthenticatedUid
        {
            get
            {
                if (UserGuid.HasValue)
                {
                    var user = UserManager.GetUserByGuid(UserGuid.Value);

                    return user != null
                               ? user.Identity.Name
                               : string.Empty;
                }


                return ExternalUid ?? string.Empty;
            }
        }


        /// <summary>
        /// Instance id for resuming
        /// </summary>
        [QueryParameter("iid", null, IsDefaultUsedForInvalid = true)]
        public Guid? ResumeInstanceId { get; set; }

        /// <summary>
        /// Query string direct user invitation guid
        /// </summary>
        [QueryParameter("directInvitation", null, IsDefaultUsedForInvalid = true)]
        public Guid? DirectInvitation { get; set; }

        /// <summary>
        /// Query string invitation guid
        /// </summary>
        [QueryParameter("i", null, IsDefaultUsedForInvalid = true)]
        public Guid? InvitationGuid { get; set; }

        /// <summary>
        /// Numeric survey id.
        /// </summary>
        [QueryParameter("surveyId", null, IsDefaultUsedForInvalid = true)]
        public int? LegacySurveyId { get; set; }

        /// <summary>
        /// Numeric invitation id
        /// </summary>
        [QueryParameter("invitationId", null, IsDefaultUsedForInvalid = true)]
        public int? LegacyInvitationId { get; set; }

        ///<summary>
        ///Indicates response is a test response
        ///</summary>
        [QueryParameter("test")]
        public bool IsTest { get; set; }

        ///<summary>
        ///Indicates response is a test response
        ///</summary>
        [QueryParameter("edit")]
        public bool IsEdit { get; set; }

        ///<summary>
        ///Indicates response is a test response
        ///</summary>
        [QueryParameter("forceNew")]
        public bool ForceNew { get; set; }

        /// <summary>
        /// Load values
        /// </summary>
        public void LoadValues()
        {
            WebParameterAttribute.SetValues(this, HttpContext.Current);
        }

        /// <summary>
        /// Validate that required information is present in query string.
        /// </summary>
        /// <returns></returns>
        public bool ValidateRequiredParameters()
        {
            return SurveyGuid.HasValue
                || (ApplicationManager.AppSettings.AllowResponseTemplateIDLookup && LegacySurveyId.HasValue)
                || InvitationGuid.HasValue
                || LegacyInvitationId.HasValue
                || ResponseGuid.HasValue
                || ResumeInstanceId.HasValue;
        }

        /// <summary>
        /// Initialize a response data object based on data in the query string.
        /// </summary>
        public void InitializeResponseData(ResponseSessionData responseData)
        {
            //Determine template id based on invitation guid
            if (InvitationGuid.HasValue)
            {
                //Store invitation guid
                responseData.InvitationRecipientGuid = InvitationGuid;

                SurveyGuid = InvitationManager.GetResponseTemplateGuidForInvitation(InvitationGuid.Value);

                if (!SurveyGuid.HasValue)
                {
                    throw new Exception("Unable to locate survey for invitation id: " + InvitationGuid);
                }

                responseData.ResponseTemplateGuid = SurveyGuid;
                int? templateId = ResponseTemplateManager.GetResponseTemplateIdFromGuid(SurveyGuid.Value);

                if (!templateId.HasValue)
                {
                    throw new Exception("Unable to locate survey with specified id: " + SurveyGuid);
                }

                responseData.ResponseTemplateId = templateId.Value;
            }
            else if (DirectInvitation.HasValue)
            {
                responseData.DirectInvitationRecipientGuid = DirectInvitation;

                SurveyGuid = InvitationManager.GetResponseTemplateGuidForUsersInvitation(DirectInvitation.Value);

                if (!SurveyGuid.HasValue)
                {
                    throw new Exception("Unable to locate survey for direcet invitation id: " + InvitationGuid);
                }

                responseData.ResponseTemplateGuid = SurveyGuid;
                int? templateId = ResponseTemplateManager.GetResponseTemplateIdFromGuid(SurveyGuid.Value);

                if (!templateId.HasValue)
                {
                    throw new Exception("Unable to locate survey with specified id: " + SurveyGuid);
                }

                responseData.ResponseTemplateId = templateId.Value;
            }
            else if (ApplicationManager.AppSettings.AllowResponseTemplateIDLookup && LegacySurveyId.HasValue)
            {
                //Set response template id
                responseData.ResponseTemplateId = LegacySurveyId.Value;
            }
            //Determine template id based on survey guid
            else if (SurveyGuid.HasValue)
            {
                responseData.ResponseTemplateGuid = SurveyGuid;
                int? templateId = ResponseTemplateManager.GetResponseTemplateIdFromGuid(SurveyGuid.Value);

                if (!templateId.HasValue)
                {
                    throw new Exception("Unable to locate survey with specified id: " + SurveyGuid);
                }

                responseData.ResponseTemplateId = templateId.Value;
            }

            //Determine anonymous respondent guid
            if (InvitationGuid.HasValue)
            {
                responseData.AnonymousRespondentGuid = InvitationGuid;
            }
            else if(DirectInvitation.HasValue)
            {
                responseData.AnonymousRespondentGuid = DirectInvitation;
            }
            else
            {
                //Attempt to get cookie value
                if (ApplicationManager.AppSettings.SessionMode == AppSettings.SessionType.Cookies
                    && Utilities.IsNotNullOrEmpty(ApplicationManager.AppSettings.CookieName)
                    && HttpContext.Current != null
                    && HttpContext.Current.Request != null
                    && HttpContext.Current.Request.Cookies != null)
                {
                    HttpCookie respondentCookie = HttpContext.Current.Request.Cookies[Management.ApplicationManager.AppSettings.CookieName];

                    if (respondentCookie != null)
                    {
                        try
                        {
                            //Get value as guid
                            responseData.AnonymousRespondentGuid = new Guid(respondentCookie.Value);
                        }
                        catch
                        {
                        }
                    }
                }

                //No respondent info in cookie or was not a valid guid, generate new guid
                if (!responseData.AnonymousRespondentGuid.HasValue)
                {
                    responseData.AnonymousRespondentGuid = Guid.NewGuid();
                }

                //Update cookie
                if (ApplicationManager.AppSettings.SessionMode == AppSettings.SessionType.Cookies
                    && Utilities.IsNotNullOrEmpty(ApplicationManager.AppSettings.CookieName)
                    && HttpContext.Current != null
                    && HttpContext.Current.Request != null
                    && HttpContext.Current.Response != null)
                {
                    var cookie = new HttpCookie(ApplicationManager.AppSettings.CookieName, responseData.AnonymousRespondentGuid.ToString());
                    cookie.Expires = DateTime.Now.AddMonths(6);

                    if (HttpContext.Current.Response.Cookies[ApplicationManager.AppSettings.CookieName] != null)
                    {
                        HttpContext.Current.Response.Cookies.Add(cookie);
                    }
                    else
                    {
                        HttpContext.Current.Response.Cookies.Set(cookie);
                    }
                }
            }

            //User
            responseData.AuthenticatedRespondentUid = AuthenticatedUid;
          
            //Set test/edit flags flag
            responseData.IsTest = IsTest;
            responseData.IsEdit = IsEdit;
            responseData.ForceNew = ForceNew;

            //Response Guid or resume instance id, if present
            responseData.ResponseGuid = ResponseGuid;
            responseData.ResumeInstanceId = ResumeInstanceId;
        }
    }
}
