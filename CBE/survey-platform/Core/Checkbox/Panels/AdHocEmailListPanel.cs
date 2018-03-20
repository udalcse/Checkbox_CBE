using System;
using System.Collections.Generic;
using System.Data;

using Prezza.Framework.Data;
using Checkbox.Common;

namespace Checkbox.Panels
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class AdHocEmailListPanel : Panel
    {

        /// <summary>
        /// Panel constructor
        /// </summary>
        public AdHocEmailListPanel()
            : base(new string[] { }, new string[] { })
        {

        }

        /// <summary>
        /// Get name of object
        /// </summary>
        public override string ObjectTypeName { get { return "AdHocEmailListPanel"; } }

        /// <summary>
        /// Panel type name
        /// </summary>
        public override string PanelTypeName { get { return "Checkbox.Panels.AdHocEmailListPanel"; } }

        /// <summary>
        /// Add an email address
        /// </summary>
        /// <param name="email"></param>
        public void AddEmailAddress(string email)
        {
            //Do nothing for no email
            if (Utilities.IsNullOrEmpty(email))
            {
                return;
            }

			//string encodedEmail = email;//.Replace("'", string.Empty); Utilities.SqlEncode(email);

            if (Panelists.Find(p => p.Email.Equals(email, StringComparison.InvariantCultureIgnoreCase)) == null)
            {
				Panelists.Add(new Panelist { Email = email });
				AddedPanelists.Add(new Panelist { Email = email });
            }
        }

        /// <summary>
        /// Add email addresses
        /// </summary>
        /// <param name="emailAddresses"></param>
        public void AddEmailAddresses(IEnumerable<string> emailAddresses)
        {
            foreach (string emailAddress in emailAddresses)
            {
                AddEmailAddress(emailAddress);
            }
        }

        /// <summary>
        /// Remove an email address from a panel
        /// </summary>
        /// <param name="email"></param>
        public void RemoveEmailAddress(string email)
        {
            if (Utilities.IsNullOrEmpty(email))
            {
                return;
            }

            int panelistIndex = Panelists.FindIndex(p => p.Email.Equals(email, StringComparison.InvariantCultureIgnoreCase));

            if (panelistIndex >= 0)
            {
                RemovedPanelists.Add(Panelists[panelistIndex]);
                Panelists.RemoveAt(panelistIndex);
            }
        }

        /// <summary>
        /// Remove email addresses from a panel
        /// </summary>
        /// <param name="emailAddresses"></param>
        public void RemoveEmailAddresses(IEnumerable<string> emailAddresses)
        {
            foreach (string email in emailAddresses)
            {
                RemoveEmailAddress(email);
            }
        }

        /// <summary>
        /// Get a command to insert a panelist
        /// </summary>
        /// <param name="db"></param>
        /// <param name="panelistEmail"></param>
        /// <returns></returns>
        protected DBCommandWrapper GetInsertPanelistCommand(Database db, string panelistEmail)
        {
            DBCommandWrapper insert = db.GetStoredProcCommandWrapper("ckbx_sp_Panel_InsertAdHoc");
            insert.AddInParameter("PanelID", DbType.Int32, ID.Value);
			insert.AddInParameter("EmailAddress", DbType.String, panelistEmail);

            return insert;
        }

        /// <summary>
        /// Get a command to update a panelist
        /// </summary>
        /// <param name="db"></param>
        /// <param name="panelistEmail"></param>
        /// <returns></returns>
        protected DBCommandWrapper GetDeletePanelistCommand(Database db, string panelistEmail)
        {
            DBCommandWrapper delete = db.GetStoredProcCommandWrapper("ckbx_sp_Panel_DeleteAdHoc");
            delete.AddInParameter("PanelID", DbType.Int32, ID.Value);
			delete.AddInParameter("EmailAddress", DbType.String, panelistEmail);

            return delete;
        }

        /// <summary>
        /// Get a command to update a panelist
        /// Use this function to delete old-formatted names with apostrophe
        /// As example, there is in the database could be stored an address like o''neal@domen.com instead of o'neal@domen.com
        /// </summary>
        /// <param name="db"></param>
        /// <param name="panelistEmail"></param>
        /// <returns></returns>
        [Obsolete]
        protected DBCommandWrapper GetDeletePanelistCommand_Old(Database db, string panelistEmail)
        {
            DBCommandWrapper delete = db.GetStoredProcCommandWrapper("ckbx_sp_Panel_DeleteAdHoc");
            delete.AddInParameter("PanelID", DbType.Int32, ID.Value);
            delete.AddInParameter("EmailAddress", DbType.String, panelistEmail.Replace("'", "''"));

            return delete;
        }

        /// <summary>
        /// Update the list of panelists
        /// </summary>
        /// <param name="t"></param>
        protected override void UpdatePanelists(IDbTransaction t)
        {
            Database db = DatabaseFactory.CreateDatabase();

            foreach (Panelist panelist in RemovedPanelists)
            {
                DBCommandWrapper command = GetDeletePanelistCommand(db, panelist.Email);
                db.ExecuteNonQuery(command, t);

                command = GetDeletePanelistCommand_Old(db, panelist.Email);
                db.ExecuteNonQuery(command, t);
            }

            foreach (Panelist panelist in AddedPanelists)
            {
                DBCommandWrapper command = GetInsertPanelistCommand(db, panelist.Email);
                db.ExecuteNonQuery(command, t);
            }

            //Clear added/removed collections
            AddedPanelists.Clear();
            RemovedPanelists.Clear();
        }

        /// <summary>
        /// Get list of panelists
        /// </summary>
        /// <returns></returns>
        protected override List<Panelist> GetPanelists()
        {
            List<Panelist> panelists = new List<Panelist>();

            Database db = DatabaseFactory.CreateDatabase();

            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Panel_GetAdHocPanelists");
            command.AddInParameter("PanelID", DbType.Int32, ID);

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    while (reader.Read())
                    {
                        string panelistEmail = DbUtility.GetValueFromDataReader(reader, "EmailAddress", string.Empty);

                        if (Utilities.IsNotNullOrEmpty(panelistEmail))
                        {
                            panelists.Add(new Panelist { Email = panelistEmail.Replace("''", "'") });
                        }
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            return panelists;
        }
    }
}
