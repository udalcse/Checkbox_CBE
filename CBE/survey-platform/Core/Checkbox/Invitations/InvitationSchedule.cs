//===============================================================================
// Checkbox Application Source Code
// Copyright © Checkbox Survey Inc  All rights reserved.
//===============================================================================
using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using Checkbox.Common;
using Checkbox.Pagination;
using Checkbox.Security.Principal;
using Prezza.Framework.ExceptionHandling;
using Prezza.Framework.Data;
using Checkbox.Panels;
using Checkbox.Messaging.Email;
using Checkbox.Users;
using Prezza.Framework.Security.Principal;

namespace Checkbox.Invitations
{
    /// <summary>
    /// Represents an invitation to take a survey.
    /// </summary>
    [Serializable]
    public class InvitationSchedule
    {
        /// <summary>
        /// Gets or sets the unique database id of the Invitation
        /// </summary>
        public int? InvitationScheduleID { get; set; }

        /// <summary>
        /// Gets or sets the unique database id of the Invitation
        /// </summary>
        public int? InvitationID { get; set; }

        /// <summary>
        /// Get the user name of the creator
        /// </summary>
        public InvitationActivityType InvitationActivityType { get; set; }

        /// <summary>
        /// Get the scheduled date
        /// </summary>
        public DateTime? Scheduled { get; set; }

        /// <summary>
        /// Get the scheduled date
        /// </summary>
        public DateTime? ProcessingStarted { get; set; }

        /// <summary>
        /// Get the date of actual sending end
        /// </summary>
        public DateTime? ProcessingFinished { get; set; }

        /// <summary>
        /// Error message
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// User name that scheduled this activity
        /// </summary>
        public string Creator { get; set; }

        /// <summary>
        /// Gets or sets the batch ID
        /// </summary>
        public long? BatchID { get; set; }

        /// <summary>
        /// Persists an Invitation
        /// </summary>
        public void Save(CheckboxPrincipal principal, Database db = null)
        {            
            if (InvitationScheduleID == null || InvitationScheduleID.Value <= 0)
            {
                Create(principal, db);
            }
            else
            {
                Update(principal, db);
            }
        }


        string _recentBatchErrorText;
        /// <summary>
        /// Error text of the batch
        /// </summary>
        public string RecentBatchErrorText
        {
            get
            {
                if (!InvitationScheduleID.HasValue)
                    return string.Empty;
                if (_recentBatchErrorText == null)
                {
                    _recentBatchErrorText = EmailGateway.GetBatchErrorText(InvitationScheduleID.Value);
                }
                return _recentBatchErrorText;
            }
        }


        string[] _bouncedEmails;
        /// <summary>
        /// Error text of the batch
        /// </summary>
        public string[] BouncedEmails
        {
            get
            {
                if (!InvitationScheduleID.HasValue)
                    return new string[]{};
                if (_bouncedEmails == null)
                {
                    _bouncedEmails = EmailGateway.GetBouncedEmails(InvitationScheduleID.Value);
                }
                return _bouncedEmails;
            }
        }        
        /// <summary>
        /// Inserts a new invitation record into the database
        /// </summary>
        protected virtual void Create(CheckboxPrincipal principal, Database db)
        {
            if (db == null)
                db = DatabaseFactory.CreateDatabase();

            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Invitation_Schedule_Create");
            command.AddOutParameter("InvitationScheduleID", DbType.Int32, 4);

            command.AddInParameter("InvitationID", DbType.Int32, InvitationID);
            command.AddInParameter("InvitationActivityType", DbType.Int32, InvitationActivityType);
            command.AddInParameter("DateScheduled", DbType.DateTime, Scheduled);
            command.AddInParameter("Creator", DbType.String, principal.Identity.Name);
            command.AddInParameter("BatchID", DbType.Int32, BatchID);

            try
            {
                using (IDbConnection connection = db.GetConnection())
                {
                    connection.Open();
                    IDbTransaction transaction = connection.BeginTransaction();

                    try
                    {
                        db.ExecuteNonQuery(command, transaction);
                        InvitationScheduleID = (Int32)command.GetParameterValue("InvitationScheduleID");

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new ApplicationException("Unable to save Invitation", ex);
                    }
                    finally
                    {
                        connection.Close();
                    }

                }

            }
            catch (Exception e)
            {
                bool rethrow = ExceptionPolicy.HandleException(e, "BusinessPublic");
                if (rethrow)
                    throw;
            }
        }

        /// <summary>
        /// Updates and invitation record in the database
        /// </summary>
        protected virtual void Update(CheckboxPrincipal principal, Database db)
        {
            if (db == null)
                db = DatabaseFactory.CreateDatabase();

            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Invitation_Schedule_Update");
            command.AddInParameter("InvitationScheduleID", DbType.Int32, InvitationScheduleID);
            command.AddInParameter("InvitationActivityType", DbType.Int32, InvitationActivityType);
            command.AddInParameter("DateScheduled", DbType.DateTime, Scheduled);
            command.AddInParameter("ProcessingStarted", DbType.DateTime, ProcessingStarted);
            command.AddInParameter("ProcessingFinished", DbType.DateTime, ProcessingFinished);
            command.AddInParameter("ErrorMessage", DbType.String, ErrorMessage);
            command.AddInParameter("BatchID", DbType.Int32, BatchID);

            using (IDbConnection connection = db.GetConnection())
            {
                connection.Open();
                IDbTransaction transaction = connection.BeginTransaction();

                try
                {
                    db.ExecuteNonQuery(command);
                    transaction.Commit();

                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public virtual void Load()
        {
            Database db = DatabaseFactory.CreateDatabase();

            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Invitation_Schedule_Get");
            command.AddInParameter("InvitationScheduleID", DbType.Int32, InvitationScheduleID);

            using (IDbConnection connection = db.GetConnection())
            {
                connection.Open();

                try
                {
                    IDataReader reader = db.ExecuteReader(command);
                    var hasInvitation = false;
                    if (hasInvitation = reader.Read())
                    {
                        LoadFromReader(reader);
                    }
                    reader.Close();
                    if (!hasInvitation)
                    {
                        throw new Exception(string.Format("Invitation with schedule ID = {0} has been deleted.", InvitationScheduleID));
                    }

                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        /// <summary>
        /// Loads instances data from the opened reader
        /// </summary>
        public void LoadFromReader(IDataReader reader)
        {
            InvitationScheduleID = DbUtility.GetValueFromDataReader(reader, "InvitationScheduleID", 0);
            InvitationActivityType = (InvitationActivityType)Enum.Parse(typeof(InvitationActivityType), DbUtility.GetValueFromDataReader(reader, "InvitationActivityType", "Invitation"));
            Creator = DbUtility.GetValueFromDataReader(reader, "Creator", "");
            ErrorMessage = DbUtility.GetValueFromDataReader(reader, "ErrorMessage", "");
            this.InvitationID = DbUtility.GetValueFromDataReader(reader, "InvitationID", 0);
            this.BatchID = DbUtility.GetValueFromDataReader<long?>(reader, "BatchID", null);
            DateTime tmp;
            if (DateTime.TryParse(reader["ProcessingStarted"].ToString(), out tmp))
            {
                ProcessingStarted = tmp;
            }
            else
            {
                ProcessingStarted = null;
            }
            if (DateTime.TryParse(reader["ProcessingFinished"].ToString(), out tmp))
            {
                ProcessingFinished = tmp;
            }
            else
            {
                ProcessingFinished = null;
            }
            this.Scheduled = DbUtility.GetValueFromDataReader(reader, "DateScheduled", default(DateTime?));
        }

        /// <summary>
        /// Removes schedule record permanently
        /// </summary>
        public void Delete()
        {
            if (!InvitationScheduleID.HasValue)
                throw new Exception("Cannot delete uninatialized invitation");
            Database db = DatabaseFactory.CreateDatabase();

            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Invitation_Schedule_Delete");
            command.AddInParameter("InvitationScheduleID", DbType.Int32, InvitationScheduleID);

            using (IDbConnection connection = db.GetConnection())
            {
                connection.Open();
                IDbTransaction transaction = connection.BeginTransaction();

                try
                {
                    db.ExecuteNonQuery(command);
                    transaction.Commit();

                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

    }
}
