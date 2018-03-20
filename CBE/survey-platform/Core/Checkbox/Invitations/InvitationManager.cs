//===============================================================================
// Checkbox Application Source Code
// Copyright © Checkbox Survey Inc  All rights reserved.
//===============================================================================
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using Checkbox.Forms;
using Checkbox.Common;
using Checkbox.Globalization.Text;
using Checkbox.Management;
using Checkbox.Messaging.Email;
using Checkbox.Pagination;
using Checkbox.Security;
using Checkbox.Security.Principal;
using Checkbox.Wcf.Services.Proxies;
using Newtonsoft.Json;
using Prezza.Framework.Data;
using Prezza.Framework.ExceptionHandling;
using Prezza.Framework.Security;
using Prezza.Framework.Security.Principal;
using Checkbox.Users;
using System.Configuration;
using System.IO;

namespace Checkbox.Invitations
{
    /// <summary>
    /// Handles factory methods associated with Retrieving and Deleting Invitations
    /// </summary>
    public static class InvitationManager
    {
        /// <summary>
        /// Default placeholder for substituting the customized URL on a per-invitee basis.
        /// </summary>
        public static string SURVEY_URL_PLACEHOLDER = ApplicationManager.AppSettings.PipePrefix + "SURVEY_URL_PLACEHOLDER__DO_NOT_ERASE";
        /// <summary>
        /// Default placeholder for substituting the customized URL on a per-invitee basis.
        /// </summary>
        public static string OPT_OUT_URL_PLACEHOLDER = ApplicationManager.AppSettings.PipePrefix + "OPT_OUT_PLACEHOLDER__DO_NOT_ERASE";

        /// <summary>
        /// The custom invitation link prefix
        /// </summary>
        private static string CUSTOM_INVITATION_LINK_PREFIX = "###";

        /// <summary>
        /// Gets a loaded instance of an Invitation object
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Invitation GetInvitation(int id)
        {
            var invitation = new Invitation(id);

            try
            {
                if (invitation.Load())
                {
                    return invitation;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Could not get Invitation from database", ex);
            }

            return null;
        }

        /// <summary>
        /// Create an invitation for a survey and initialize it with default invitation text.
        /// </summary>
        /// <param name="surveyId"></param>
        /// <param name="messageFormat"></param>
        /// <param name="creatorPrincipal"></param>
        /// <returns></returns>
        public static Invitation CreateInvitationForSurvey(LightweightResponseTemplate lightweightRt, CheckboxPrincipal creatorPrincipal)
        {
            //Ensure survey and or default language exist
            if (lightweightRt == null || Utilities.IsNullOrEmpty(lightweightRt.DefaultLanguage))
            {
                return null;
            }
            var newInvitation = new Invitation { ParentID = lightweightRt.ID};

            newInvitation.Save(creatorPrincipal);

            return newInvitation;
        }

        /// <summary>
        /// Sets all the fields for the invitation by default
        /// </summary>
        /// <param name="messageFormat"></param>
        /// <param name="lightweightRt"></param>
        /// <param name="newInvitation"></param>
        public static void PopulateInvitationWithDefaults(MailFormat messageFormat, LightweightResponseTemplate lightweightRt, Invitation newInvitation)
        {
            //Get the title
            string surveyTitle = TextManager.GetText(lightweightRt.TitleTextId, lightweightRt.DefaultLanguage);

            if (surveyTitle == null || surveyTitle.Trim() == string.Empty)
            {
                surveyTitle = lightweightRt.Name;
            }

            if (surveyTitle == null || surveyTitle.Trim() == string.Empty)
            {
                surveyTitle = TextManager.GetText("/pageText/newInvitation.aspx/survey", lightweightRt.DefaultLanguage, "survey", TextManager.DefaultLanguage);
            }

            //Set the subject
            newInvitation.Template.Subject = TextManager.GetText("/pageText/newInvitation.aspx/invitationToTakeSurvey", lightweightRt.DefaultLanguage, "Invitation to take a survey", TextManager.DefaultLanguage);
            String siteName = TextManager.GetText("/siteText/siteName", TextManager.DefaultLanguage);

            if (siteName != null && siteName.Trim() != string.Empty)
            {
                newInvitation.Template.Subject += TextManager.GetText("/pageText/newInvitation.aspx/at", lightweightRt.DefaultLanguage, " at ", TextManager.DefaultLanguage);
                newInvitation.Template.Subject += siteName;
            }

            //Set the "You have been invited..." 
            string invited = TextManager.GetText("/pageText/newInvitation.aspx/youHaveBeenInvited", lightweightRt.DefaultLanguage, "You have been invited to take the survey: ", TextManager.DefaultLanguage);
            invited += surveyTitle + ".";

            if (messageFormat == MailFormat.Html)
            {
                newInvitation.Template.Format = MailFormat.Html;

                if (ApplicationManager.AppSettings.IsInvitationTextEnabled)
                {
                    var text = TextManager.GetText("/siteText/invitationHtmlDefaultText");
                    if (!string.IsNullOrEmpty(text))
                        newInvitation.Template.Body = text.Replace(CUSTOM_INVITATION_LINK_PREFIX, string.Empty);
                  
                }
                else
                {
                    newInvitation.Template.Body = invited;
                    newInvitation.Template.Body += "<br /><br />";
                    newInvitation.Template.Body += "<a href=\"" + SURVEY_URL_PLACEHOLDER + "\">";
                    newInvitation.Template.Body += TextManager.GetText("/pageText/newInvitation.aspx/clickHere", lightweightRt.DefaultLanguage, "Click here", TextManager.DefaultLanguage) + "</a>&nbsp;";
                    newInvitation.Template.Body += TextManager.GetText("/pageText/newInvitation.aspx/toTakeTheSurvey", lightweightRt.DefaultLanguage, " to take the survey", TextManager.DefaultLanguage) + ".";
                   
                }

                if (ApplicationManager.AppSettings.FooterEnabled)
                {
                    newInvitation.Template.Body += "<br /><br />";
                    newInvitation.Template.Body += ApplicationManager.AppSettings.FooterText;
                    newInvitation.Template.Body += "<br />";
                }

            }
            else
            {
                if (ApplicationManager.AppSettings.IsInvitationTextEnabled)
                {
                    var text  = TextManager.GetText("/siteText/invitationTextDefaultText");
                    if (!string.IsNullOrEmpty(text))
                    {
                        newInvitation.Template.Body = text.Replace(CUSTOM_INVITATION_LINK_PREFIX, string.Empty);
                        newInvitation.Template.Format = MailFormat.Text;
                    }
                }
                else
                {
                    newInvitation.Template.Format = MailFormat.Text;

                    newInvitation.Template.Body = invited;
                    newInvitation.Template.Body += Environment.NewLine + Environment.NewLine;
                    newInvitation.Template.Body += SURVEY_URL_PLACEHOLDER;
                  
                }

                if (ApplicationManager.AppSettings.FooterEnabled)
                {
                    var footer = ApplicationManager.AppSettings.FooterText.Replace("<br>", Environment.NewLine).Replace("<br />", Environment.NewLine).Replace("<br/>", Environment.NewLine);
                    var regex = new Regex("<a\\s([^>]*\\s)?href=\"" + OPT_OUT_URL_PLACEHOLDER + "\"(.*?)>(.*?)</a>");
                    footer = regex.Replace(footer, OPT_OUT_URL_PLACEHOLDER);

                    newInvitation.Template.Body += Environment.NewLine + Environment.NewLine;
                    newInvitation.Template.Body += Utilities.StripHtml(footer);
                    newInvitation.Template.Body += Environment.NewLine;
                }
            }

            newInvitation.Template.ReminderBody = newInvitation.Template.Body;
            newInvitation.Template.ReminderSubject = newInvitation.Template.Subject;
        }

        /// <summary>
        /// Get the invitation for a recipient
        /// </summary>
        /// <param name="recipientGuid"></param>
        /// <returns></returns>
        public static Invitation GetInvitationForRecipient(Guid recipientGuid)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Invitation_GetIdFromRecipientGuid");
            command.AddInParameter("RecipientGuid", DbType.Guid, recipientGuid);


            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    if (reader.Read())
                    {
                        return GetInvitation((int)reader["InvitationID"]);
                    }
                }
                catch
                {
                    //Suppress errors to prevent alarm during take survey
                }
                finally
                {
                    reader.Close();
                }
            }

            return null;
        }

        /// <summary>
        /// Get the user invitation for a recipient
        /// </summary>
        /// <param name="recipientGuid"></param>
        /// <returns></returns>
        public static string GetUserInvitationForRecipient(Guid directInvitation)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Invitation_GetUserFromDirectInvite");
            command.AddInParameter("directInvitation", DbType.Guid, directInvitation);


            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    if (reader.Read())
                    {
                        return (string)reader["UserName"];
                    }
                }
                catch
                {
                    //Suppress errors to prevent alarm during take survey
                }
                finally
                {
                    reader.Close();
                }
            }

            return null;
        }

        /// <summary>
        /// Get the invitation for a recipient by partial guid string.  If multple matches are found,
        /// only the first match is returned.
        /// </summary>
        /// <param name="partialRecipientGuid"></param>
        /// <returns></returns>
        public static Invitation GetInvitationForRecipient(string partialRecipientGuid)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Invitation_GetIdFromPartialRecipientGuid");
            command.AddInParameter("PartialRecipientGuid", DbType.String, partialRecipientGuid);


            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    if (reader.Read())
                    {
                        return GetInvitation((int)reader["InvitationID"]);
                    }
                }
                catch
                {
                    //Suppress errors to prevent alarm during take survey
                }
                finally
                {
                    reader.Close();
                }
            }

            return null;
        }

        /// <summary>
        /// Get the invitation for a recipient
        /// </summary>
        /// <param name="recipientID"></param>
        /// <returns></returns>
        public static Invitation GetInvitationForRecipient(long recipientID)
        {
            int? invitationId = GetInvitationIdForRecipient(recipientID);

            if (!invitationId.HasValue)
            {
                return null;
            }

            var invitation = new Invitation(invitationId.Value);

            try
            {
                if (invitation.Load())
                {
                    return invitation;
                }
            }
            catch
            {
                return null;
            }

            return null;
        }

        /// <summary>
        /// Get the invitation for a recipient
        /// </summary>
        /// <param name="recipientID"></param>
        /// <returns></returns>
        public static int? GetInvitationIdForRecipient(long recipientID)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Invitation_GetID");
            command.AddInParameter("RecipientID", DbType.Int64, recipientID);

            int? invitationID = null;

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    if (reader.Read())
                    {
                        invitationID = (int)reader["InvitationID"];
                    }
                }
                catch
                {
                    //Suppress errors to prevent alarm during take survey
                }
                finally
                {
                    reader.Close();
                }
            }

            return invitationID;
        }

        /// <summary>
        /// Based on the provided guid, get the user name of a recipient.  If the recipient is not a registered user,
        /// a null value is returned.
        /// </summary>
        /// <param name="recipientGuid"></param>
        /// <returns></returns>
        public static string GetRecipientUniqueIdentifier(Guid recipientGuid)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Invitation_GetRecipientUniqueIdentifier");
            command.AddInParameter("RecipientGuid", DbType.Guid, recipientGuid);

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    if (reader.Read())
                    {
                        return DbUtility.GetValueFromDataReader<string>(reader, "UniqueIdentifier", null);
                    }
                }
                catch
                {
                    //Suppress errors to prevent alarm during take survey
                }
                finally
                {
                    reader.Close();
                }
            }

            return null;
        }

        public static void GenerateLinksForUsers(CheckboxPrincipal callingPrincipal, int surveyId, List<UserInvitation> userList)
        {
            Database db = DatabaseFactory.CreateDatabase();

            Guid? surveyGuid = ResponseTemplateManager.GetResponseTemplateGUID(surveyId);
            var baseSurveyUrl = InvitationPipeMediator.GetBaseSurveyUrl(surveyGuid);
            string prefix = baseSurveyUrl.Contains("?") ? "&" : "?";

            List<Object> generatedLinkInfo = new List<Object>();
            foreach (var user in userList)
            {
                var directInvitationGuid = Guid.NewGuid();
                string recipientGuidString = prefix + "directInvitation=" + directInvitationGuid.ToString().Replace("-", string.Empty);
                string fullLink = baseSurveyUrl + recipientGuidString;

                DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_GenerateUserInvitation");
                command.AddInParameter("SurveyId", DbType.Int32, surveyId);
                command.AddInParameter("UserName", DbType.String, user.UserName);
                command.AddInParameter("DirectInvitation", DbType.Guid, directInvitationGuid);
                command.AddInParameter("Url", DbType.String, fullLink);

                db.ExecuteScalar(command);

                command = db.GetStoredProcCommandWrapper("ckbx_sp_GetUserShareLink");
                command.AddInParameter("SurveyId", DbType.Int32, surveyId);
                command.AddInParameter("UserName", DbType.String, user.UserName);

                using (IDataReader reader = db.ExecuteReader(command))
                {
                    try
                    {
                        if (reader.Read())
                        {
                            if (reader[0] != null && reader[0] != DBNull.Value && reader[0] is string)
                            {
                                fullLink = (string)reader[0];
                            }
                        }
                    }
                    finally
                    {
                        reader.Close();
                    }
                }

                CheckboxPrincipal userPrincipal = UserManager.GetUserPrincipal(user.UserName);
                Dictionary<string, string> userProfileProperties = ProfileManager.GetProfile(userPrincipal.Identity.Name);

                if (userProfileProperties.ContainsKey("CompanyId") && userProfileProperties["CompanyId"] != null && !String.IsNullOrEmpty(userProfileProperties["CompanyId"]) &&
                    userProfileProperties.ContainsKey("ExternalUserId") && userProfileProperties["ExternalUserId"] != null && !String.IsNullOrEmpty(userProfileProperties["ExternalUserId"]))
                {
                    generatedLinkInfo.Add(new
                    {
                        PartnerUserId = userProfileProperties["ExternalUserId"],
                        PartnerSystemId = "DirectorsDesk",
                        PartnerCompanyId = userProfileProperties["CompanyId"],
                        PartnerUsername = userProfileProperties["TenantId"],
                        EngaugeUsername = user.UserName,
                        PartnerSurveyId = userProfileProperties["ExternalSurveyId"],
                        EngaugeSurveyId = surveyId + "_" + ConfigurationManager.AppSettings["EnvironmentName"],
                        InvitationUrl = fullLink
                    });
                }
            }

            string partnerLinkApiRoot = ConfigurationManager.AppSettings["PartnerLinkApiRoot"];
            if (String.IsNullOrEmpty(partnerLinkApiRoot) || partnerLinkApiRoot.Equals("UNSET"))
            {
                partnerLinkApiRoot = "http://localhost:5000";
            }
            HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create(partnerLinkApiRoot + "/api/linkedusers/bulk");
            webRequest.Method = "POST";
            webRequest.ContentType = "application/json";
            webRequest.SendChunked = true;
            string jsonPayload = JsonConvert.SerializeObject(generatedLinkInfo);
            byte[] postData = System.Text.ASCIIEncoding.UTF8.GetBytes(jsonPayload);
            webRequest.ContentLength = postData.Length;
            Stream postDataStream = webRequest.GetRequestStream();
            postDataStream.Write(postData, 0, postData.Length);
            postDataStream.Close();

            HttpWebResponse webResponse = null;

            try {
                webResponse = (HttpWebResponse)webRequest.GetResponse();

                Stream stream = webResponse.GetResponseStream();
                StreamReader streamReader = new StreamReader(stream);
                Char[] readBuffer = new Char[256];
                int count = streamReader.Read(readBuffer, 0, 256);
                while (count > 0)
                {
                    string outputData = new String(readBuffer, 0, count);
                    count = streamReader.Read(readBuffer, 0, 256);
                }
                // Release the response object resources.
                streamReader.Close();
                stream.Close();
            }
            catch (Exception e)
            {
                // Do nothing
            }
            finally
            {
                if (webResponse != null)
                {
                    webResponse.Close();
                }
            }
        }

        /// <summary>
        /// Based on the provided guid, get the email address a recipient.  
        /// </summary>
        /// <param name="recipientGuid"></param>
        /// <returns></returns>
        public static string GetRecipientEmailAddress(Guid recipientGuid)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Invitation_GetRecipientEmail");
            command.AddInParameter("RecipientGuid", DbType.Guid, recipientGuid);

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    if (reader.Read())
                    {
                        return DbUtility.GetValueFromDataReader<string>(reader, "EmailAddress", null);
                    }
                }
                catch
                {
                    //Suppress errors to prevent alarm during take survey
                }
                finally
                {
                    reader.Close();
                }
            }

            return null;
        }

        /// <summary>
        /// Inserts a new opt out rule into database
        /// </summary>
        /// <param name="recipient"> </param>
        /// <param name="responseTemplateId"> </param>
        /// <param name="optOutType"> </param>
        /// <param name="comment"> </param>
        /// <returns></returns>
        public static void OptOutRecipient(Recipient recipient, int? responseTemplateId, InvitationOptOutType optOutType, string comment)
        {
            OptOutRecipient(recipient.EmailToAddress, responseTemplateId, optOutType, comment, recipient.InvitationID);

            recipient.OptedOut = true;
            recipient.Save();
        }

        /// <summary>
        /// Inserts a new opt out rule into database
        /// </summary>
        /// <param name="email"> </param>
        /// <param name="responseTemplateId"> </param>
        /// <param name="optOutType"> </param>
        /// <param name="comment"> </param>
        /// <param name="invitationId"> </param>
        /// <returns></returns>
        public static void OptOutRecipient(string email, int? responseTemplateId, InvitationOptOutType optOutType, string comment, int invitationId)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Invitation_OptOutEmailAddress");
            command.AddInParameter("EmailAddress", DbType.String, email);
            command.AddInParameter("ResponseTemplateId", DbType.Int32, responseTemplateId);
            command.AddInParameter("OptOutType", DbType.Int32, (int)optOutType);
            command.AddInParameter("UserComment", DbType.String, comment);
            command.AddInParameter("InvitationId", DbType.Int32, invitationId);

            try
            {
                db.ExecuteNonQuery(command);
            }
            catch
            {
                //Suppress errors to prevent alarm during take survey
            }
        }


        /// <summary>
        /// Inserts a new opt out rule into database
        /// </summary>
        /// <param name="responseTemplateId"> </param>
        /// <returns></returns>
        public static List<string> GetOptedOutEmailsBySurveyId(int responseTemplateId)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Invitation_GetOptedOutEmails");
            command.AddInParameter("ResponseTemplateId", DbType.Int32, responseTemplateId);

            var result = new List<string>();

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    while (reader.Read())
                    {
                        result.Add(DbUtility.GetValueFromDataReader(reader, "EmailAddress", string.Empty).ToLower().Trim());
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
        /// Inserts a new opt out rule into database
        /// </summary>
        /// <param name="responseTemplateId"> </param>
        /// <param name="email"> </param>
        /// <param name="invitationId"> </param>
        /// <returns></returns>
        public static OptedOutInvitationData GetOptedOutEmailsDataBySurveyId(int responseTemplateId, string email, int invitationId)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Invitation_GetOptedOutEmailData");
            command.AddInParameter("ResponseTemplateId", DbType.Int32, responseTemplateId);
            command.AddInParameter("EmailAddress", DbType.String, email);
            command.AddInParameter("InvitationId", DbType.Int32, invitationId);

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    if (reader.Read())
                    {
                        var d = new OptedOutInvitationData
                            {
                                UserComment = DbUtility.GetValueFromDataReader<string>(reader, "UserComment", null),
                                ResponseTemplateId = responseTemplateId,
                                Date = DbUtility.GetValueFromDataReader(reader, "DateOccur", default(DateTime)),
                                ResponseTemplateName = DbUtility.GetValueFromDataReader(reader, "TemplateName", string.Empty),
                                Email = email,
                                Type = DbUtility.GetValueFromDataReader(reader, "OptOutType", 0)
                            };

                        return d;
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        public static OptedOutInvitationData GetDataIfUserHasOptedOutFromCurrentAccount(string email)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Invitation_GetDataIfUserHasOptedOutFromAccount");
            command.AddInParameter("EmailAddress", DbType.String, email);

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    if (reader.Read())
                    {
                        return new OptedOutInvitationData
                            {
                                UserComment = DbUtility.GetValueFromDataReader<string>(reader, "UserComment", null),
                                Email = email,
                                Type = DbUtility.GetValueFromDataReader(reader, "OptOutType", 0)
                            };
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            return null;
        }

        /// <summary>
        /// Get a guid for an invitation recipient.  If no matching response template is found for the guid,
        /// null is returned.
        /// </summary>
        /// <param name="recipientGuid"></param>
        /// <returns></returns>
        public static Guid? GetResponseTemplateGuidForInvitation(Guid recipientGuid)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Invitation_GetResponseTemplateGuid");
            command.AddInParameter("RecipientGuid", DbType.Guid, recipientGuid);

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    if (reader.Read())
                    {
                        return DbUtility.GetValueFromDataReader<Guid?>(reader, "Guid", null);
                    }
                }
                catch
                {
                    //Suppress errors to prevent alarm during take survey
                }
                finally
                {
                    reader.Close();
                }
            }

            return null;
        }

        /// <summary>
        /// Get a guid for an invitation recipient.  If no matching response template is found for the guid,
        /// null is returned.
        /// </summary>
        /// <param name="recipientGuid"></param>
        /// <returns></returns>
        public static Guid? GetResponseTemplateGuidForUsersInvitation(Guid recipientGuid)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("[ckbx_sp_Invitation_GetDirectResponseTemplateGuid]");
            command.AddInParameter("DirectInvitation", DbType.Guid, recipientGuid);

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    if (reader.Read())
                    {
                        return DbUtility.GetValueFromDataReader<Guid?>(reader, "Guid", null);
                    }
                }
                catch
                {
                    //Suppress errors to prevent alarm during take survey
                }
                finally
                {
                    reader.Close();
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <returns></returns>
        public static List<OptedOutInvitationData> GetOptedOutSurveyListByEmail(string emailAddress)
        {
            var result = new List<OptedOutInvitationData>();

            if (!string.IsNullOrEmpty(emailAddress))
            {
                Database db = DatabaseFactory.CreateDatabase();
                DBCommandWrapper command =
                    db.GetStoredProcCommandWrapper("ckbx_sp_Invitation_GetOptedOutSurveyListByEmail");
                command.AddInParameter("EmailAddress", DbType.String, emailAddress);

                using (IDataReader reader = db.ExecuteReader(command))
                {
                    try
                    {
                        while (reader.Read())
                        {
                            result.Add(new OptedOutInvitationData
                            {
                                Email = emailAddress,
                                ResponseTemplateId = DbUtility.GetValueFromDataReader(reader, "ResponseTemplateId", -1),
                                ResponseTemplateName = DbUtility.GetValueFromDataReader(reader, "TemplateName", string.Empty),
                                UserComment = DbUtility.GetValueFromDataReader<string>(reader, "UserComment", null),
                                Type = DbUtility.GetValueFromDataReader(reader, "OptOutType", 0)
                            });
                        }
                    }
                    finally
                    {
                        reader.Close();
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Given a recipient's numeric db id, get the corresponding guid.  If the recipient has been
        /// deleted, or the id can't be found, not value will be returned.
        /// </summary>
        /// <param name="recipientId"></param>
        /// <returns></returns>
        public static Guid? GetRecipientGuid(long recipientId)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Invitation_GetRecipientGuid");
            command.AddInParameter("RecipientID", DbType.Int64, recipientId);

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    if (reader.Read())
                    {
                        return DbUtility.GetValueFromDataReader<Guid?>(reader, "Guid", null);
                    }
                }
                catch
                {
                    //Suppress errors to prevent alarm during take survey
                }
                finally
                {
                    reader.Close();
                }
            }

            return null;
        }

        /// <summary>
        /// Get the recipient by given GUID
        /// </summary>
        /// <param name="recipientGuid"></param>
        /// <returns></returns>
        public static Recipient GetRecipientByGuid(Guid recipientGuid)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Invitation_GetRecipientByGuid");
            command.AddInParameter("RecipientGuid", DbType.Guid, recipientGuid);

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    if (reader.Read())
                    {
                        return new Recipient(reader);
                    }
                }
                catch
                {
                    //Suppress errors to prevent alarm during take survey
                }
                finally
                {
                    reader.Close();
                }
            }

            return null;
        }

        /// <summary>
        /// Given a recipient's numeric db id, get the corresponding guid.  If the recipient has been
        /// deleted, or the id can't be found, not value will be returned.
        /// </summary>
        /// <param name="recipientGuid"></param>
        /// <returns></returns>
        public static long? GetRecipientId(Guid recipientGuid)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Invitation_GetRecipientId");
            command.AddInParameter("RecipientGuid", DbType.Guid, recipientGuid);

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    if (reader.Read())
                    {
                        return DbUtility.GetValueFromDataReader<long?>(reader, "RecipientId", null);
                    }
                }
                catch
                {
                    //Suppress errors to prevent alarm during take survey
                }
                finally
                {
                    reader.Close();
                }
            }

            return null;
        }

        /// <summary>
        /// Count the number of times a recipient has responded to an invitation
        /// </summary>
        /// <param name="recipientGuid">Recipient guid.</param>
        /// <returns></returns>
        public static int GetRecipientResponseCount(Guid recipientGuid)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Invitation_GetRecipientResponseCount");
            command.AddInParameter("RecipientGuid", DbType.Guid, recipientGuid);

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    if (reader.Read())
                    {
                        return DbUtility.GetValueFromDataReader(reader, "ResponseCount", 0);
                    }
                }
                catch
                {
                    //Suppress errors to prevent alarm during take survey
                }
                finally
                {
                    reader.Close();
                }
            }

            return 0;
        }

        /// <summary>
        /// Deletes a databound Invitation from the database
        /// </summary>
        /// <param name="invitation"><see cref="Invitation"/></param>
        public static void DeleteInvitation(Invitation invitation)
        {
            if (invitation.ID != null)
                DeleteInvitation(invitation.ID.Value);
        }

        /// <summary>
        /// Delete the invitation with the specified id
        /// </summary>
        /// <param name="invitationID"></param>
        public static void DeleteInvitation(int invitationID)
        {
            //Delete invitation emails
            DeleteAllInvitationEmailBatches(invitationID);

            Database db = DatabaseFactory.CreateDatabase();

            using (IDbConnection connection = db.GetConnection())
            {
                connection.Open();
                IDbTransaction t = connection.BeginTransaction();

                try
                {
                    DBCommandWrapper delete = db.GetStoredProcCommandWrapper("ckbx_sp_Invitation_Delete");
                    delete.AddInParameter("InvitationID", DbType.Int32, invitationID);
                    db.ExecuteNonQuery(delete, t);

                    t.Commit();
                }
                catch
                {
                    t.Rollback();
                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        /// <summary>
        /// Returns a list of Invitation IDs available to a specified principal
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="paginationContext"></param>
        /// <param name="responseTemplateId"></param>
        /// <returns></returns>
        public static List<int> ListInvitationIDsForSurvey(ExtendedPrincipal principal, int responseTemplateId, PaginationContext paginationContext)
        {
            var invitationIds = new List<int>();

            //Ensure principal can view invitations for survey
            LightweightResponseTemplate rt = ResponseTemplateManager.GetLightweightResponseTemplate(responseTemplateId);

            if (rt == null)
            {
                return invitationIds;
            }

            if (!AuthorizationFactory.GetAuthorizationProvider().Authorize(principal, rt, "Form.Administer"))
            {
                return invitationIds;
            }

            //Get list of invitations for the survey
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Invitation_ListForSurvey");
            command.AddInParameter("ResponseTemplateId", DbType.Int32, responseTemplateId);
            command.AddInParameter("PageNumber", DbType.Int32, paginationContext.CurrentPage);
            command.AddInParameter("ResultsPerPage", DbType.Int32, paginationContext.PageSize);
            command.AddInParameter("SortField", DbType.String, paginationContext.SortField ?? string.Empty);
            command.AddInParameter("SortAscending", DbType.Boolean, paginationContext.SortAscending);
            command.AddInParameter("FilterField", DbType.String, paginationContext.FilterField ?? string.Empty);
            command.AddInParameter("FilterValue", DbType.String, paginationContext.FilterValue ?? string.Empty);

            
            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    while (reader.Read())
                    {
                        int invitationId = DbUtility.GetValueFromDataReader(reader, "InvitationId", -1);

                        if (invitationId > 0
                            && !invitationIds.Contains(invitationId))
                        {
                            invitationIds.Add(invitationId);
                        }
                    }
                    
                    if (reader.NextResult() && reader.Read())
                    {
                        paginationContext.ItemCount = DbUtility.GetValueFromDataReader(reader, "TotalItemCount", -1);
                    }
                }
                finally
                {
                    

                    reader.Close();
                }
            }

            return invitationIds;
        }

        /// <summary>
        /// Returns a list of users available to a specified principal
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="paginationContext"></param>
        /// <param name="responseTemplateId"></param>
        /// <returns></returns>
        public static List<InvitationData> ListUsersInvitationForSurvey(ExtendedPrincipal principal, int responseTemplateId, PaginationContext paginationContext)
        {
            var userInvitation = new List<InvitationData>();

            //Ensure principal can view invitations for survey
            LightweightResponseTemplate rt = ResponseTemplateManager.GetLightweightResponseTemplate(responseTemplateId);

            if (rt == null)
            {
                return userInvitation;
            }

            if (!AuthorizationFactory.GetAuthorizationProvider().Authorize(principal, rt, "Form.Administer"))
            {
                return userInvitation;
            }

            //Get list of invitations for the survey
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Invitation_ListUsersForSurvey");
            command.AddInParameter("ResponseTemplateId", DbType.Int32, responseTemplateId);
            command.AddInParameter("PageNumber", DbType.Int32, paginationContext.CurrentPage);
            command.AddInParameter("ResultsPerPage", DbType.Int32, paginationContext.PageSize);
            command.AddInParameter("SortField", DbType.String, paginationContext.SortField ?? string.Empty);
            command.AddInParameter("SortAscending", DbType.Boolean, paginationContext.SortAscending);

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    while (reader.Read())
                    {
                        userInvitation.Add(new InvitationData
                        {
                            Name = DbUtility.GetValueFromDataReader(reader, "UserName", string.Empty),
                            LinkText = DbUtility.GetValueFromDataReader(reader, "Url", string.Empty),
                            CreatedDate = DbUtility.GetValueFromDataReader(reader, "DateGenerated", DateTime.Now),
                            LastSent = DbUtility.GetValueFromDataReader<DateTime?>(reader, "DatePublished", null),
                        });
                    }

                    if (reader.NextResult() && reader.Read())
                    {
                        paginationContext.ItemCount = DbUtility.GetValueFromDataReader(reader, "TotalItemCount", -1);
                    }
                }
                finally
                {


                    reader.Close();
                }
            }

            return userInvitation;
        }

        /// <summary>
        /// Returns a table of Invitation details available to a specified principal
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="responseTemplateId"></param>
        /// <param name="paginationContext"></param>
        /// <returns></returns>
        public static List<Invitation> ListInvitationDetailsForSurvey(ExtendedPrincipal principal, int responseTemplateId, PaginationContext paginationContext)
        {

            return ListInvitationIDsForSurvey(principal, responseTemplateId, paginationContext)
                .Select(GetInvitation)
                .ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="responseTemplateId"></param>
        /// <param name="paginationContext"></param>
        /// <returns></returns>
        public static List<InvitationData> ListLightWeightInvitationDetailsForSurvey(ExtendedPrincipal principal, int responseTemplateId, PaginationContext paginationContext)
        {

            return ListInvitationIDsForSurvey(principal, responseTemplateId, paginationContext)
                .Select(GetLightweightInvitation)
                .ToList();
        }

        /// <summary>
        /// Sets an invitations last sent field to the current date
        /// </summary>
        /// <param name="invitationId">The invitations unique id.</param>
        public static void UpdateInvitationSentDate(int invitationId)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Invitation_UpdateLastSent");
            command.AddInParameter("InvitationId", DbType.Int32, invitationId);

            db.ExecuteNonQuery(command);
        }

        /// <summary>
        /// Reset 'ProcessingBatchId' column to NULL in ckbx_InvitationRecipients for requested invitation
        /// </summary>
        /// <param name="sheduleId">The invitations unique id.</param>
        public static void ResetProcessingBatchForRecipients(int sheduleId)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_Invitation_ResetProcessingBatchForRecipients");
            command.AddInParameter("SheduleId", DbType.Int32, sheduleId);

            db.ExecuteNonQuery(command);
        }

        /// <summary>
        /// Opts a recipient out of an invitation.
        /// </summary>
        /// <param name="recipientId">The unique id of the invitation sent to a particular recipient.</param>
        public static void OptOutRecipient(long recipientId)
        {
            Guid? guid = GetRecipientGuid(recipientId);

            if (guid.HasValue)
            {
                OptOutRecipient(guid.Value);
            }
        }

        /// <summary>
        /// Opts a recipient out of an invitation.
        /// </summary>
        /// <param name="recipientGuid">The guid of the invitation sent to a particular recipient.</param>
        public static void OptOutRecipient(Guid recipientGuid)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Invitation_OptOut");
            command.AddInParameter("RecipientGuid", DbType.Guid, recipientGuid);

            db.ExecuteNonQuery(command);
        }

        #region Batch Management

        /// <summary>
        /// Create a new email batch for an invitation.
        /// </summary>
        /// <param name="invitationId"></param>
        /// <param name="invitationMode"></param>
        /// <param name="recipientCount"></param>
        /// <param name="createdBy"></param>
        /// <param name="earliestSendDate"></param>
        /// <returns></returns>
        public static long? CreateInvitationEmailBatch(int invitationId, InvitationActivityType invitationMode, int recipientCount, string createdBy, DateTime? earliestSendDate, int scheduleID)
        {
            long? batchId = null;

            if (EmailGateway.ProviderSupportsBatches)
            {
                string batchDescription = string.Format("Invitation {0} {1} batch of {2} for {3} recipients", invitationId, invitationMode, DateTime.Now, recipientCount);

                //Create an email batch
                batchId = EmailGateway.CreateEmailBatch(
                    batchDescription,
                    createdBy,
                    earliestSendDate,
                    scheduleID);

                //Add mapping in checkbox
                if (batchId.HasValue)
                {
                    Database db = DatabaseFactory.CreateDatabase();
                    DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Invitation_RecordBatch");
                    command.AddInParameter("InvitationId", DbType.Int32, invitationId);
                    command.AddInParameter("BatchId", DbType.Int64, batchId.Value);
                    command.AddInParameter("BatchDescription", DbType.String, batchDescription);

                    db.ExecuteNonQuery(command);
                }
            }

            return batchId;
        }

        /// <summary>
        /// Mark an email batch as ready to send.
        /// </summary>
        /// <param name="batchId"></param>
        public static void CloseInvitationEmailBatch(long batchId)
        {
            EmailGateway.MarkEmailBatchReady(batchId);
        }

        /// <summary>
        /// Delete invitation email batch from the invitations/batches mapping.  This method DOES NOT
        /// remove associated messages.
        /// </summary>
        /// <param name="batchId"></param>
        public static void DeleteInvitationEmailBatch(long batchId)
        {
            //Remove from queue
            EmailGateway.DeleteMessageBatch(batchId);

            //Remove mapping from checkbox
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Invitation_RemoveBatch");
            command.AddInParameter("BatchId", DbType.Int64, batchId);
            command.AddInParameter("InvitationId", DbType.Int32, DBNull.Value);

            db.ExecuteNonQuery(command);
        }

        /// <summary>
        /// Set successful status for all non-opted out recipients for given invitation
        /// </summary>
        /// <param name="invitationID"></param>
        public static void SetSuccessfulSentStatusForRecipients(int invitationID)
        {
            //Remove mapping from checkbox
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_Invitation_SetSuccessfulSentStatusForRecipients");
            command.AddInParameter("InvitationID", DbType.Int32, invitationID);

            db.ExecuteNonQuery(command);
        }

        /// <summary>
        /// Delete all batches from the invitations/messages mapping.  This method DOES NOT
        /// remove associated messages.
        /// </summary>
        /// <param name="invitationId"></param>
        public static void DeleteAllInvitationEmailBatches(int invitationId)
        {
            //Remove from queue
            if (EmailGateway.ProviderSupportsBatches)
            {
                List<long> batchIds = ListInvitationBatchIds(invitationId);

                foreach (long batchId in batchIds)
                {
                    EmailGateway.DeleteMessageBatch(batchId);
                }
            }

            //Remove email mappings from checkbox. This can be done in one shot by passing invitation id to remove messages method
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Invitation_RemoveMessage");
            command.AddInParameter("InvitationId", DbType.Int32, invitationId);
            command.AddInParameter("RecipientId", DbType.Int64, DBNull.Value);
            command.AddInParameter("MessageId", DbType.Int64, DBNull.Value);


            //Remove mappings from checkbox.  This can be done in one shot by passing invitation id to remove batch method
            command = db.GetStoredProcCommandWrapper("ckbx_sp_Invitation_RemoveBatch");
            command.AddInParameter("BatchId", DbType.Int64, DBNull.Value);
            command.AddInParameter("InvitationId", DbType.Int32, invitationId);

            db.ExecuteNonQuery(command);
        }

        /// <summary>
        /// Get a list of queue message ids for a given recipient.
        /// </summary>
        /// <param name="recipientId"></param>
        /// <returns></returns>
        public static List<long> ListRecipientQueueMessages(long recipientId)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Invitation_ListRecipientMessages");
            command.AddInParameter("RecipientId", DbType.Int64, recipientId);

            //List messages for the recpient
            var messageIds = new List<long>();

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    while (reader.Read())
                    {
                        if (reader["MessageId"] != DBNull.Value)
                        {
                            messageIds.Add((long)reader["MessageId"]);
                        }
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            return messageIds;
        }

        /// <summary>
        /// Delete any queued invitation messages for a recipient.  Does not check if current email provider supports batches, so
        /// this check should be made by any calling code.
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="recipientId"></param>
        public static void RemoveRecipientMessagesFromEmailQueue(long recipientId, IDbTransaction transaction)
        {
            List<long> messageIds = ListRecipientQueueMessages(recipientId);

            //Delete the messages
            foreach (long messageId in messageIds)
            {
                EmailGateway.DeleteMessage(messageId);

                //Remove mapping from ckbx tables.
                Database db = DatabaseFactory.CreateDatabase();
                DBCommandWrapper batchOpCommand = db.GetStoredProcCommandWrapper("ckbx_sp_Invitation_RemoveMessage");
                batchOpCommand.AddInParameter("InvitationId", DbType.Int32, DBNull.Value);
                batchOpCommand.AddInParameter("RecipientId", DbType.Int64, recipientId);
                batchOpCommand.AddInParameter("MessageId", DbType.Int64, DBNull.Value);

                if (transaction != null)
                {
                    db.ExecuteNonQuery(batchOpCommand, transaction);
                }
                else
                {
                    db.ExecuteNonQuery(batchOpCommand);
                }
            }
        }

        /// <summary>
        /// Get a list of batch ids associated with an invitation
        /// </summary>
        /// <param name="invitationId"></param>
        /// <returns></returns>
        public static List<long> ListInvitationBatchIds(int invitationId)
        {
            var batchIds = new List<long>();

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Invitation_ListBatches");
            command.AddInParameter("InvitationId", DbType.Int32, invitationId);

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    while (reader.Read())
                    {
                        var batchId = DbUtility.GetValueFromDataReader<long?>(reader, "BatchId", null);

                        if (batchId.HasValue)
                        {
                            batchIds.Add(batchId.Value);
                        }
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            return batchIds;
        }

        /// <summary>
        /// Returns invitation count for the specified response template
        /// </summary>
        /// <param name="currentPrincipal"></param>
        /// <param name="responseTemplateId"></param>
        /// <returns></returns>
        public static int GetInvitationCountForSurvey(CheckboxPrincipal currentPrincipal, int responseTemplateId)
        {
            var db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command;

            if (currentPrincipal.IsInRole("System Administrator"))
            {
                command = db.GetStoredProcCommandWrapper("ckbx_sp_Invitation_CountForRT");
            }
            else
            {
                command = db.GetStoredProcCommandWrapper("ckbx_sp_Security_CountAccessibleInvitationsForSurvey");
                command.AddInParameter("UniqueIdentifier", DbType.String, currentPrincipal.Identity.Name);
                command.AddInParameter("UseAclExclusion", DbType.Boolean, ApplicationManager.AppSettings.AllowExclusionaryAclEntries);
            }

            command.AddInParameter("ResponseTemplateId", DbType.Int32, responseTemplateId);
            command.AddOutParameter("TotalCount", DbType.Int32, 4);

            try
            {
                db.ExecuteNonQuery(command);
                return (int)command.GetParameterValue("TotalCount");
            }
            catch (Exception ex)
            {
                bool rethrow = ExceptionPolicy.HandleException(ex, "BusinessPublic");

                if (rethrow)
                {
                    throw;
                }
            }

            return 0;
        }


        /// <summary>
        /// Returns invitation count for the specified response template
        /// </summary>
        /// <param name="currentPrincipal"></param>
        /// <param name="responseTemplateId"></param>
        /// <returns></returns>
        public static int GetInvitationCountForSurveyByType(CheckboxPrincipal currentPrincipal, int responseTemplateId, bool isSent)
        {
            var db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command;

            if (currentPrincipal.IsInRole("System Administrator"))
            {
                command = db.GetStoredProcCommandWrapper("ckbx_sp_Invitation_CountForRTByType");
            }
            else
            {
                command = db.GetStoredProcCommandWrapper("ckbx_sp_Security_CountAccessibleInvitationsForSurveyByType");
                command.AddInParameter("UniqueIdentifier", DbType.String, currentPrincipal.Identity.Name);
                command.AddInParameter("UseAclExclusion", DbType.Boolean, ApplicationManager.AppSettings.AllowExclusionaryAclEntries);
            }

            command.AddInParameter("ResponseTemplateId", DbType.Int32, responseTemplateId);
            command.AddInParameter("IsSent", DbType.Boolean, isSent);
            command.AddOutParameter("TotalCount", DbType.Int32, 4);

            try
            {
                db.ExecuteNonQuery(command);
                return (int)command.GetParameterValue("TotalCount");
            }
            catch (Exception ex)
            {
                bool rethrow = ExceptionPolicy.HandleException(ex, "BusinessPublic");

                if (rethrow)
                {
                    throw;
                }
            }

            return 0;
        }

        /// <summary>
        /// List all invitations specified user has access to
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="paginationContext"></param>
        /// <returns></returns>
        public static List<int> ListAccessibleInvitationIds(CheckboxPrincipal userPrincipal, PaginationContext paginationContext)
        {
            //Return no results if username not specified
            if (userPrincipal == null)
            {
                return new List<int>();
            }
            
            var db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command;

            if (userPrincipal.IsInRole("System Administrator"))
            {
                command = db.GetStoredProcCommandWrapper("ckbx_sp_Security_ListAccessibleInvitationsAdmin");
            }
            else
            {
                command = db.GetStoredProcCommandWrapper("ckbx_sp_Security_ListAccessibleInvitations");
                command.AddInParameter("UniqueIdentifier", DbType.String, userPrincipal.Identity.Name);
                command.AddInParameter("UseAclExclusion", DbType.Boolean, ApplicationManager.AppSettings.AllowExclusionaryAclEntries);
            }

            QueryHelper.AddPagingAndFilteringToCommandWrapper(command, paginationContext);


            var results = new List<int>();

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    while (reader.Read())
                    {
                        var invitationId = DbUtility.GetValueFromDataReader(reader, "InvitationId", -1);

                        if(invitationId > 0)
                        {
                            results.Add(invitationId);
                        }
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            return results;
        }

        #endregion

        /// <summary>
        /// Copy invitation
        /// </summary>
        /// <param name="InvitationId">Base invitation ID</param>
        /// <param name="Name">Invitation Name</param>
        /// <param name="useDefaultText">Use default texts</param>
        /// <param name="copyRecipients">Copy recipients</param>
        public static Invitation CopyInvitation(int InvitationId, string Name, bool useDefaultText, bool copyRecipients, CheckboxPrincipal creator)
        {
            try
            {
                Invitation invitation = GetInvitation(InvitationId);
                var survey = ResponseTemplateManager.GetLightweightResponseTemplate(invitation.ParentID);
                Invitation newInvitation = CreateInvitationForSurvey(survey, creator);
                newInvitation.Name = Name;
                //copy options
                newInvitation.Template.IncludeOptOutLink = invitation.Template.IncludeOptOutLink;
                newInvitation.Template.LoginOption = invitation.Template.LoginOption;

                //copy texts if needed
                if (!useDefaultText)
                {
                    newInvitation.Template.Body = invitation.Template.Body;
                    newInvitation.Template.Subject = invitation.Template.Subject;
                    newInvitation.Template.Format = invitation.Template.Format;
                    newInvitation.Template.FromAddress = invitation.Template.FromAddress;
                    newInvitation.Template.FromName = invitation.Template.FromName;
                    newInvitation.Template.LinkText = invitation.Template.LinkText;
                    newInvitation.Template.OptOutText = invitation.Template.OptOutText;
                    newInvitation.Template.ReminderBody = invitation.Template.ReminderBody;
                    newInvitation.Template.ReminderSubject = invitation.Template.ReminderSubject;
                }
                else
                    PopulateInvitationWithDefaults(invitation.Template.Format, survey, newInvitation);

                //copy recipients
                if (copyRecipients)
                {
                    List<string> recipients = (from r in invitation.GetRecipients(RecipientFilter.All) select r.EmailToAddress).ToList();
                    newInvitation.AddPanel(recipients);
                }

                newInvitation.Save(creator);
                return newInvitation;
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "BusinessPublic");
                throw new Exception("Failed to copy the invitation", ex);
            }            
        }

        /// <summary>
        /// Returns invitation recipient GUID by 
        /// </summary>
        /// <returns></returns>
        public static Guid? GetInvitationIDForAuthenticatedRespondent(string userUid, int responseTemplateID)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Invitation_GetRecipientGuidIDForAuthenticatedRespondent");
            command.AddInParameter("UserUid", DbType.String, userUid);
            command.AddInParameter("ResponseTemplateID", DbType.Int32, responseTemplateID);

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    if (reader.Read())
                    {
                        return DbUtility.GetValueFromDataReader<Guid?>(reader, "RecipientGUID", null);
                    }
                }
                catch
                {
                    //Suppress errors to prevent alarm during take survey
                }
                finally
                {
                    reader.Close();
                }
            }

            return null;
        }

        /// <summary>
        /// Returns invitation recipient GUID by 
        /// </summary>
        /// <returns></returns>
        public static Guid? GetInvitationIDForAuthenticatedRespondent(string userUid, Guid responseTemplateGuid)
        {
            var id =  ResponseTemplateManager.GetResponseTemplateIdFromGuid(responseTemplateGuid);
            if (id.HasValue)
                GetInvitationIDForAuthenticatedRespondent(userUid, id.Value);

            return null;
        }

        /// <summary>
        /// Returns invitation recipient GUID by 
        /// </summary>
        /// <returns></returns>
        public static Guid? GetInvitationIDForAuthenticatedRespondentByResponseGuid(string userUid, Guid responseGuid)
        {
            var id = ResponseTemplateManager.GetResponseTemplateFromResponseGUID(responseGuid).ID;
            if (id.HasValue)
                GetInvitationIDForAuthenticatedRespondent(userUid, id.Value);

            return null;
        }

        /// <summary>
        /// Returns all available invitations filtered by one or all fields
        /// </summary>
        /// <param name="userPrincipal"></param>
        /// <param name="context"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public static InvitationData[] ListInvitations(CheckboxPrincipal userPrincipal, PaginationContext context, out int totalCount)
        {
            var invitationIDs = ListAccessibleInvitationIds(userPrincipal, context);
            totalCount = invitationIDs.Count;
            
            if (context.CurrentPage > 1)
                invitationIDs = invitationIDs.Skip(context.PageSize * (context.CurrentPage - 1)).ToList();

            if (context.PageSize > 0)
            {
                invitationIDs = invitationIDs.Take(context.PageSize).ToList();
            }

            return (from i in invitationIDs select GetLightweightInvitation(i)).ToArray();            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        private static InvitationData GetLightweightInvitation(int invitationID)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Invitation_Get_Lightweight");
            command.AddInParameter("InvitationID", DbType.String, invitationID);

            InvitationData res = new InvitationData();
            res.DatabaseId = invitationID;
            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    if (reader.Read())
                    {
                        res.DatabaseId = DbUtility.GetValueFromDataReader<int>(reader, "InvitationID", 0);
                        res.Name = DbUtility.GetValueFromDataReader<string>(reader, "Name", "");
                        res.CreatedBy = DbUtility.GetValueFromDataReader<string>(reader, "CreatedBy", "");
                        res.LastSent = DbUtility.GetValueFromDataReader<DateTime?>(reader, "LastSentOn", null);
                        res.ResponseTemplateId = DbUtility.GetValueFromDataReader<int>(reader, "ResponseTemplateID", -1);
                    }
                }
                catch
                {
                    //Suppress errors to prevent alarm during take survey
                }
                finally
                {
                    reader.Close();
                }
            }

            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public static InvitationData GetInvitationData(int invitationID)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Invitation_GetInvitationData");
            command.AddInParameter("InvitationID", DbType.String, invitationID);

            InvitationData res = new InvitationData();
            res.DatabaseId = invitationID;
            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    if (reader.Read())
                    {
                        res.DatabaseId = DbUtility.GetValueFromDataReader<int>(reader, "InvitationID", 0);
                        res.CreatedDate = DbUtility.GetValueFromDataReader(reader, "DateCreated", DateTime.Now);
                        res.CreatedBy = DbUtility.GetValueFromDataReader(reader, "CreatedBy", string.Empty);
                        res.Name = DbUtility.GetValueFromDataReader(reader, "Name", string.Empty);
                        res.LastSent = DbUtility.GetValueFromDataReader<DateTime?>(reader, "LastSentOn", null);
                        res.ResponseTemplateId = DbUtility.GetValueFromDataReader<int>(reader, "ResponseTemplateID", -1);
                        res.Guid = (Guid)reader["GUID"];
                        res.MailFormat = ((MailFormat)Enum.Parse(typeof(MailFormat), DbUtility.GetValueFromDataReader(reader, "EmailFormat", "Text"))).ToString();
                        res.FromAddress = DbUtility.GetValueFromDataReader(reader, "FromAddress", string.Empty);
                        res.FromName = DbUtility.GetValueFromDataReader(reader, "FromName", string.Empty);
                        res.Subject = DbUtility.GetValueFromDataReader(reader, "Subject", string.Empty);
                        res.Body = DbUtility.GetValueFromDataReader(reader, "Body", string.Empty);
                        res.OptOutText = DbUtility.GetValueFromDataReader(reader, "OptOutText", string.Empty);
                        res.IncludeOptOut = DbUtility.GetValueFromDataReader(reader, "IncludeOptOutLink", false);
                        res.LinkText = DbUtility.GetValueFromDataReader(reader, "LinkText", string.Empty);


                        if (reader["LoginOption"] != DBNull.Value && ((string)reader["LoginOption"]).ToLower() == "auto")
                        {
                            res.LoginOption = LoginOption.Auto.ToString();
                        }
                        else
                        {
                            res.LoginOption = LoginOption.None.ToString();
                        }
                    }
                }
                catch
                {
                    //Suppress errors to prevent alarm during take survey
                }
                finally
                {
                    reader.Close();
                }

                var scheduleList = GetScheduleByInvitationId(invitationID);
                var processedItem = (from s in scheduleList where s.ProcessingFinished.HasValue orderby s.ProcessingFinished descending select s).FirstOrDefault();
                var scheduledItem = (from s in scheduleList where s.Scheduled.HasValue && s.Scheduled.Value > DateTime.Now orderby s.Scheduled select s).FirstOrDefault();

                if (processedItem != null)
                {
                    res.LastSent = processedItem.ProcessingFinished;
                    res.StatusDescription = processedItem.ErrorMessage;
                    res.ErrorMessage = processedItem.ErrorMessage;
                }

                if (scheduledItem != null)
                {
                    res.Scheduled = scheduledItem.Scheduled;
                    res.ScheduleID = scheduledItem.InvitationScheduleID;
                    res.InvitationActivityType = scheduledItem.InvitationActivityType.ToString();
                    if (scheduledItem.ProcessingStarted.HasValue)
                    {
                        if (scheduledItem.ProcessingFinished.HasValue)
                            res.Status = "InvitationStatusCompleted";
                        else
                        {
                            res.StatusDescription = "Sending...";
                            res.Status = "InvitationStatusSending";
                        }
                    }
                }
            }
            return res;
        }

        /// <summary>
        /// Get schedule if invitation by its id
        /// </summary>
        /// <param name="invitationId"></param>
        /// <returns></returns>
        public static List<InvitationSchedule> GetScheduleByInvitationId(int invitationId)
        {
            Database db = DatabaseFactory.CreateDatabase();

            DBCommandWrapper invitationCommand = db.GetStoredProcCommandWrapper("ckbx_sp_Invitation_Get_Schedule");
            invitationCommand.AddInParameter("InvitationID", DbType.Int32, invitationId);
            List<InvitationSchedule> _schedule = new List<InvitationSchedule>();
            using (IDataReader reader = db.ExecuteReader(invitationCommand))
            {
                try
                {
                    while (reader.Read())
                    {
                        var scheduleItem = new InvitationSchedule();
                        scheduleItem.LoadFromReader(reader);
                        _schedule.Add(scheduleItem);
                    }
                }
                finally
                {
                    reader.Close();
                }
            }
            _schedule.Sort((i1, i2) => i1.Scheduled.HasValue && i2.Scheduled.HasValue ? (int)Math.Round(i1.Scheduled.Value.Subtract(i2.Scheduled.Value).TotalSeconds) :
                (i1.Scheduled.HasValue ? int.MinValue : int.MaxValue));

            return _schedule;
        }

        /// <summary>
        /// Get number of recipients by invitation ID
        /// </summary>
        public static int GetRecipientsCount(int invitationId)
        {
            int recipientsCount = 0;
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Invitation_GetRecipientsCount");
            command.AddInParameter("InvitationID", DbType.Int32, invitationId);
            command.AddInParameter("Filter", DbType.String, "All");

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    while (reader.Read())
                    {
                        recipientsCount = DbUtility.GetValueFromDataReader(reader, "RecipientsNumber", 0);
                    }
                }
                finally
                {
                    reader.Close();
                }
            }
            return recipientsCount;
        }

        /// <summary>
        /// Get related invitation batch ID
        /// </summary>
        public static int GetRelatedInvitationBatchId(int scheduleID)
        {
            int batchID = -1;
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Invitation_GetInvitationBatchId");
            command.AddInParameter("ScheduleId", DbType.Int32, scheduleID);

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    while (reader.Read())
                    {
                        batchID = DbUtility.GetValueFromDataReader(reader, "BatchID", -1);
                    }
                }
                finally
                {
                    reader.Close();
                }
            }
            return batchID;
        }

        internal static void MarkRecipientAsProcessed(string toBeMarkedAsProcessed, long messageBatchId)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Invitation_MarkRecipientAsProcessed");
            command.AddInParameter("RecipientIDs", DbType.String, toBeMarkedAsProcessed);
            command.AddInParameter("BatchID", DbType.Int64, messageBatchId);

            db.ExecuteNonQuery(command);
        }
    }
}
