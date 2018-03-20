using System;
using System.Data;

using Prezza.Framework.Data;

using Checkbox.Panels;

namespace Checkbox.Invitations
{
    ///<summary>
    ///</summary>
    [Serializable]
    public class UserPanelistRecipient : Recipient
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public UserPanelistRecipient()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="panelist"></param>
        public UserPanelistRecipient(Panelist panelist)
            : base(panelist)
        {
        }

        /// <summary>
        /// Insert recipient
        /// </summary>
        protected override void InsertRecipient()
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Invitation_InsertRecipient");
            command.AddInParameter("InvitationID", DbType.Int32, InvitationID);
            command.AddInParameter("PanelID", DbType.Int32, PanelID);
            command.AddInParameter("EmailToAddress", DbType.String, EmailToAddress);
            command.AddInParameter("UniqueIdentifier", DbType.String, UniqueIdentifier);
            command.AddInParameter("GUID", DbType.Guid, GUID);
            command.AddInParameter("LastSent", DbType.DateTime, LastSent);
            command.AddInParameter("Success", DbType.Boolean, SuccessfullySent);
            command.AddInParameter("Error", DbType.String, Error);
            command.AddInParameter("LastBatchMessageId", DbType.Int64, BatchMessageId);
            command.AddOutParameter("RecipientID", DbType.Int64, 8);
            command.AddInParameter("OptOut", DbType.Boolean, OptedOut);
            command.AddInParameter("ProcessingBatchId", DbType.Int64, ProcessingBatchId);

            db.ExecuteNonQuery(command);

            object idValue = command.GetParameterValue("RecipientID");

            if (idValue != DBNull.Value)
            {
                ID = Convert.ToInt32(idValue);
            }
            else
            {
                throw new Exception("Unable to insert recipient into database.");
            }
        }
    }
}
