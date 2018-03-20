using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Principal;
using Prezza.Framework.Data;
using Checkbox.Common;

namespace Checkbox.Panels
{
    /// <summary>
    /// Panel representing users
    /// </summary>
    [Serializable]
    public class UserPanel : Panel
    {

        /// <summary>
        /// Constructor
        /// </summary>
        public UserPanel()
            : base(new string[] { }, new string[] { })
        {

        }

        /// <summary>
        /// Get panel type name
        /// </summary>
        public override string PanelTypeName { get { return "Checkbox.Panels.UserPanel"; } }

        /// <summary>
        /// Add an identity to the table
        /// </summary>
        /// <param name="identity"></param>
        public void AddIdentity(string identity)
        {
            if (Utilities.IsNullOrEmpty(identity))
            {
                return;
            }

            if (Panelists.Find(p => ((UserPanelist)p).UniqueIdentifier.Equals(identity, StringComparison.InvariantCultureIgnoreCase)) == null)
            {
                Panelists.Add(new UserPanelist { UniqueIdentifier = identity });
                AddedPanelists.Add(new UserPanelist { UniqueIdentifier = identity });
            }
        }

        /// <summary>
        /// Adds the recipient.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="email">The email.</param>
        /// <exception cref="System.ArgumentNullException">Name or email is null</exception>
        public void AddRecipient(string name, string email)
        {
            if (Utilities.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(email))
                throw new ArgumentNullException("Name or email is null");

            if (Panelists.Find(p => ((UserPanelist)p).Email.Equals(email, StringComparison.InvariantCultureIgnoreCase)) == null)
            {
                var userPanel = new UserPanelist {UniqueIdentifier = name, Email = email};

                Panelists.Add(userPanel);
                AddedPanelists.Add(userPanel);
            }
        }

        /// <summary>
        /// Add a list of identities
        /// </summary>
        /// <param name="identities"></param>
        public void AddIdentities(IList<IIdentity> identities)
        {
            foreach (IIdentity id in identities)
            {
                AddIdentity(id.Name);
            }
        }

        /// <summary>
        /// Remove an identity
        /// </summary>
        /// <param name="identity"></param>
        public void RemoveIdentity(string identity)
        {
            if (Utilities.IsNullOrEmpty(identity))
            {
                return;
            }

            int panelistIndex = Panelists.FindIndex(p => ((UserPanelist)p).UniqueIdentifier.Equals(identity, StringComparison.InvariantCultureIgnoreCase));

            if (panelistIndex >= 0)
            {
                RemovedPanelists.Add(Panelists[panelistIndex]);
                Panelists.RemoveAt(panelistIndex);
            }
        }

        /// <summary>
        /// Remove identities
        /// </summary>
        /// <param name="identities"></param>
        public void RemoveIdentities(IEnumerable<string> identities)
        {
            foreach (string id in identities)
            {
                RemoveIdentity(id);
            }
        }

        /// <summary>
        /// Update the panelist list
        /// </summary>
        /// <param name="t"></param>
        protected override void UpdatePanelists(IDbTransaction t)
        {

            Database db = DatabaseFactory.CreateDatabase();

            foreach (Panelist panelist in RemovedPanelists)
            {
                DBCommandWrapper command = GetDeletePanelistCommand(db, ((UserPanelist)panelist).UniqueIdentifier);
                db.ExecuteNonQuery(command, t);
            }

            foreach (Panelist panelist in AddedPanelists)
            {
                DBCommandWrapper command = GetInsertPanelistCommand(db, ((UserPanelist)panelist).UniqueIdentifier, ((UserPanelist)panelist).Email);
                db.ExecuteNonQuery(command, t);
            }

            RemovedPanelists.Clear();
            AddedPanelists.Clear();
        }

        /// <summary>
        /// Get the insert panelist command
        /// </summary>
        /// <param name="db">The database.</param>
        /// <param name="panelistIdentifier">The panelist identifier.</param>
        /// <param name="email">The email.</param>
        /// <returns></returns>
        protected DBCommandWrapper GetInsertPanelistCommand(Database db, string panelistIdentifier, string email)
        {
            DBCommandWrapper insert = db.GetStoredProcCommandWrapper("ckbx_sp_Panel_InsertUser");
            insert.AddInParameter("PanelID", DbType.Int32, ID.Value);
            insert.AddInParameter("UniqueIdentifier", DbType.String, panelistIdentifier);
            insert.AddInParameter("Email", DbType.String, email);

            return insert;
        }

        /// <summary>
        /// Get the delete panelist command
        /// </summary>
        /// <param name="db"></param>
        /// <param name="panelistIdentifier"></param>
        /// <returns></returns>
        protected DBCommandWrapper GetDeletePanelistCommand(Database db, string panelistIdentifier)
        {
            DBCommandWrapper delete = db.GetStoredProcCommandWrapper("ckbx_sp_Panel_DeleteUser");
            delete.AddInParameter("PanelID", DbType.Int32, ID.Value);
            delete.AddInParameter("UniqueIdentifier", DbType.String, panelistIdentifier);

            return delete;
        }

        /// <summary>
        /// Get list of panelists
        /// </summary>
        /// <returns></returns>
        protected override List<Panelist> GetPanelists()
        {
            List<Panelist> panelists = new List<Panelist>();

            Database db = DatabaseFactory.CreateDatabase();

            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Panel_GetUserPanelIDs");
            command.AddInParameter("PanelID", DbType.Int32, ID.Value);

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    while (reader.Read())
                    {
                        string panelistId = DbUtility.GetValueFromDataReader(reader, "UserIdentifier", string.Empty);
                        string email = DbUtility.GetValueFromDataReader(reader, "Email", string.Empty);

                        if (!string.IsNullOrWhiteSpace(panelistId))
                        {
                            var userPanel = new UserPanelist { UniqueIdentifier = panelistId };

                            if (!string.IsNullOrWhiteSpace(email))
                                userPanel.Email = email;


                            panelists.Add(userPanel);
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

        /// <summary>
        /// Get a particular panelist
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public override Panelist GetPanelist(string identifier)
        {
            return Panelists.Find(p => ((UserPanelist)p).UniqueIdentifier.Equals(identifier, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
