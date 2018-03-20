using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Checkbox.Common;
using Checkbox.Globalization.Text;
using Checkbox.Progress;
using Prezza.Framework.ExceptionHandling;

namespace Checkbox.Invitations.Export
{
    /// <summary>
    /// Abstract base class for data exporters (CSV, SPSS CSV, SPSS Native, etc.) for survey
    /// data.  This base class assumes the use of an analysis to load answer data, generate column
    /// headers, etc.  In the future this may not be the case and some refactoring will be done as
    /// necessary.
    /// </summary>
    public abstract class InvitationDataExporter
    {
        /// <summary>
        /// Max export columns per file when splitting files
        /// </summary>
        public const int MAX_COLUMN_COUNT_PER_FILE = 255;

        /// <summary>
        /// Backing field for analysis used by exporter
        /// </summary>
        private Invitation _invitation;

        /// <summary>
        /// name of export data export
        /// </summary>
        private List<string> _headers;

        /// <summary>
        /// Get/set progress key
        /// </summary>
        protected string ProgressKey { get; set; }

        /// <summary>
        /// Language code for export
        /// </summary>
        protected string LanguageCode { get; set; }

        /// <summary>
        /// Get/set list of all data field names
        /// </summary>
        protected List<string> AllFieldNames { get; set; }

        /// <summary>
        /// Exporting invtation id
        /// </summary>
        protected int InvitationId { set; get; }

        /// <summary>
        /// List all field names for the item.
        /// </summary>
        /// <returns></returns>
        public ReadOnlyCollection<string> ListAllFieldNames()
        {
            if (AllFieldNames == null || AllFieldNames.Count == 0)
            {
                PopulateFieldNames(false);
            }

            return new ReadOnlyCollection<string>(AllFieldNames);
        }

        /// <summary>
        /// Get the analysis used as the basis for the export.
        /// </summary>
        protected Invitation Invitation
        {
            get { return _invitation ?? (_invitation = InvitationManager.GetInvitation(InvitationId)); }
        }

        /// <summary>
        /// Initialize the exporter class.
        /// </summary>
        /// <param name="invitationId"></param>
        /// <param name="languageCode">Language code to use for quesiton and option texts.</param>
        /// <param name="progressKey">Key to use for tracking progress when working asynchronously.</param>
        /// <remarks>When using progress tracking, the export will only set progress to 99%.  It's up to the caller
        /// to mark the progress as complete.</remarks>
        public virtual void Initialize(int invitationId, string languageCode, string progressKey)
        {
            InvitationId = invitationId;
            ProgressKey = progressKey;
            LanguageCode = languageCode;
        }

        /// <summary>
        /// Write the export data to the specified file path.
        /// </summary>
        /// <param name="filePath">File path to write export to.</param>
        public virtual void WriteToFile(string filePath)
        {
            using (StreamWriter writer = File.CreateText(filePath))
            {
                try
                {
                    WriteToTextWriter(writer);
                }
                catch (Exception ex)
                {
                    ExceptionPolicy.HandleException(ex, "UIProcess");
                    throw;
                }
                finally
                {
                    writer.Close();
                }
            }
        }

        /// <summary>
        /// Write export to text writer.
        /// </summary>
        /// <param name="writer">Text writer to write export data to.</param>
        public virtual void WriteToTextWriter(TextWriter writer)
        {
            //Set initial progress
            if (Utilities.IsNotNullOrEmpty(ProgressKey))
            {
                ProgressProvider.SetProgress(
                    ProgressKey,
                    new ProgressData
                    {
                        TotalItemCount = 100,
                        CurrentItem = 0,
                        Status = ProgressStatus.Pending,
                        Message = TextManager.GetText("/controlText/exportManager/analyzingSurveyStructure", LanguageCode)
                    }
                );
            }

            //Get field names
            PopulateFieldNames(true);

            //Pre-Load profile information in bulk if necessary.  Greatly speeds export
            // when user options are included.
            ProgressProvider.SetProgress(
                ProgressKey,
                new ProgressData
                {
                    TotalItemCount = 100,
                    CurrentItem = 0,
                    Status = ProgressStatus.Pending,
                    Message = TextManager.GetText("/controlText/exportManager/loadingUserProfileData", LanguageCode)
                }
            );

            //Write export
            WriteExportData(writer);

            //Set completed progress
            if (Utilities.IsNotNullOrEmpty(ProgressKey))
            {
                //Set progress to 99% so caller can finally
                // mark progress complete.
                ProgressProvider.SetProgress(
                    ProgressKey,
                    new ProgressData
                    {
                        CurrentItem = 99,
                        Status = ProgressStatus.Pending,
                        Message = TextManager.GetText("/controlText/exportManager/completed", LanguageCode),
                        TotalItemCount = 100
                    }
                );
            }
        }

        /// <summary>
        /// Perform the actual working of writing export data to the specified text
        /// writer.
        /// </summary>
        /// <param name="writer">Text writer to write dtata to.</param>
        protected abstract void WriteExportData(TextWriter writer);

        /// <summary>
        /// Populate names of fields
        /// </summary>
        /// <param name="forceColumnReload">Force reload of column names.</param>
        protected virtual void PopulateFieldNames(bool forceColumnReload)
        {
            AllFieldNames = new List<string>();
            AllFieldNames.AddRange(ListAllInvitationFieldNames());
        }

        /// <summary>
        /// Get list of all export fields that are included when "Include Detailed
        /// Response Information" option is used.
        /// </summary>
        /// <returns>List of response field names.</returns>
        protected virtual List<string> ListAllInvitationFieldNames()
        {
            return _headers ?? (_headers = new List<string>
                                               {
                                                   "RecipientID",
                                                   "RecipientGuid",
                                                   "InvitationID",
                                                   "PanelID",
                                                   "PanelTypeID",
                                                   "EmailAddress",
                                                   "UniqueIdentifier",
                                                   "SuccessfullySent",
                                                   "TotalSent",
                                                   "LastSent",
                                                   "ErrorMessage",
                                                   "HasResponded",
                                                   "OptedOut",
                                                   "OptedOutType",
                                                   "Deleted"
                                               });
        }

        /// <summary>
        /// Format a specific invitation property field value for the export.
        /// </summary>
        /// <param name="recipient"></param>
        /// <param name="headerValue">Invitation property to format.</param>
        /// <remarks>Checks for DBNull values and calculates TotalTime field.</remarks>
        protected string FormatInvitationData(Recipient recipient, string headerValue)
        {
            switch (headerValue)
            {
                case "RecipientID":
                    return recipient.ID == null ? string.Empty : recipient.ID.ToString();
                case "RecipientGuid":
                    return recipient.GUID.ToString();
                case "InvitationID":
                    return Invitation.ID == null ? string.Empty : Invitation.ID.ToString();
                case "PanelID":
                    return recipient.PanelID.ToString();
                case "PanelTypeID":
                    return recipient.PanelTypeId.ToString();
                case "EmailAddress":
                    return recipient.EmailToAddress;
                case "UniqueIdentifier":
                    return recipient.UniqueIdentifier;
                case "SuccessfullySent":
                    return recipient.Bounced ? "FALSE" : recipient.SuccessfullySent.ToString();
                case "TotalSent":
                    return Invitation.SentCount.ToString();
                case "LastSent":
                    return Invitation.LastSent == null ? string.Empty : Invitation.LastSent.ToString();
                case "ErrorMessage":
                    return recipient.Bounced ? "BOUNCED" : recipient.Error;
                case "HasResponded":
                    return recipient.HasResponded.ToString();
                case "OptedOut":
                    return recipient.OptedOut.ToString();
                case "OptedOutType":
                    return TextManager.GetText("/pageText/invitation/data/doexport.aspx/optOutTypes/" + recipient.OptedOutType.ToString().ToLower());
                case "Deleted":
                    return recipient.Deleted.ToString();
            }

            //Default value
            return string.Empty;
        }
    }
}
