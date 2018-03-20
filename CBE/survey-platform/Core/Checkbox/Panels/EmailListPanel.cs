using System;
using System.Collections.Generic;
using System.Data;
using Checkbox.Pagination;
using Checkbox.Panels.Security;
using Checkbox.Security;
using Checkbox.Security.Principal;
using Prezza.Framework.Data;
using Prezza.Framework.ExceptionHandling;
using Prezza.Framework.Security;
using Prezza.Framework.Security.Principal;
using Checkbox.Common;

namespace Checkbox.Panels
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class EmailListPanel : Panel //, IAccessControllable
    {
        //private int? _aclId;
        //private int? _defaultPolicyId;

        //private AccessControlList _acl;
        //private Policy _defaultPolicy;

        ///// <summary>
        ///// Get list of permissions supported by the panel
        ///// </summary>
        //public string[] SupportedPermissionMasks { get; private set; }

        ///// <summary>
        ///// Get list of permissions supported by the panel
        ///// </summary>
        //public string[] SupportedPermissions { get; private set; }

        /// <summary>
        /// Return email list panel
        /// </summary>
        public override string PanelTypeName { get { return "Checkbox.Panels.EmailListPanel"; } }

        /// <summary>
        /// Email address data column.
        /// </summary>
        public const string EMAIL_ADDRESS_KEY = "EmailAddress";

        /// <summary>
        /// First name data column.
        /// </summary>
        public const string FIRST_NAME_KEY = "FName";

        /// <summary>
        /// Last name data column.
        /// </summary>
        public const string LAST_NAME_KEY = "LName";

        ///// <summary>
        ///// Get acl for object
        ///// </summary>
        //public IAccessControlList ACL
        //{
        //    get
        //    {
        //        if (_acl == null
        //            && _aclId.HasValue)
        //        {
        //            _acl = new AccessControlList(_aclId.Value);
        //        }

        //        return _acl;
        //    }
        //}

        ///// <summary>
        ///// Get default policy for object
        ///// </summary>
        //public Policy DefaultPolicy
        //{
        //    get
        //    {
        //        if (_defaultPolicy == null
        //            && _defaultPolicyId.HasValue)
        //        {
        //            _defaultPolicy = Policy.GetPolicy(_defaultPolicyId.Value);
        //        }

        //        return _defaultPolicy;
        //    }
        //}

        ///// <summary>
        ///// Set access to the object
        ///// </summary>
        ///// <param name="defaultPolicy"></param>
        ///// <param name="acl"></param>
        //public void SetAccess(Policy defaultPolicy, AccessControlList acl)
        //{
        //    if (defaultPolicy != null)
        //    {
        //        _defaultPolicyId = defaultPolicy.Persist();
        //        _defaultPolicy = defaultPolicy;
        //    }

        //    if (acl != null)
        //    {
        //        _aclId = Convert.ToInt32(acl.ID);
        //        _acl = acl;
        //    }
        //}

        ///// <summary>
        ///// Create a policy for the object
        ///// </summary>
        ///// <param name="permissions"></param>
        ///// <returns></returns>
        //public Policy CreatePolicy(string[] permissions)
        //{
        //    return new Policy(permissions);
        //}

        /// <summary>
        /// Constructor.
        /// </summary>
        public EmailListPanel() : base(new[] { "EmailList.View", "EmailList.Edit" }, new[] { "EmailList.Edit", "EmailList.View", "EmailList.Delete"})
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        public void RemoveAllAddresses()
        {
            //Add all panelists to the "deleted" list.
            RemovedPanelists.Clear();
            RemovedPanelists.AddRange(Panelists);
            Panelists.Clear();
        }

        /// <summary>
        /// Get list of panelists
        /// </summary>
        /// <returns></returns>
        protected override List<Panelist> GetPanelists()
        {
            List<Panelist> panelists = new List<Panelist>();

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Panel_GetEmailAddressData");
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
                            Panelist panelist = new Panelist { Email = panelistEmail };
                            panelist.SetProperty("FName", DbUtility.GetValueFromDataReader(reader, "FName", string.Empty));
                            panelist.SetProperty("LName", DbUtility.GetValueFromDataReader(reader, "LName", string.Empty));

                            panelists.Add(panelist);
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
        /// 
        /// </summary>
        /// <param name="emailAddress"></param>
        public void AddPanelist(string emailAddress)
        {
            AddPanelist(emailAddress, string.Empty, string.Empty);
        }

        /// <summary>
        /// Add a panelist
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <param name="fName"></param>
        /// <param name="lName"></param>
        public void AddPanelist(string emailAddress, string fName, string lName)
        {
            //Do nothing for no email
            if (Utilities.IsNullOrEmpty(emailAddress))
            {
                return;
            }			

            if (Panelists.Find(p => p.Email.Equals(emailAddress, StringComparison.InvariantCultureIgnoreCase)) == null)
            {
                Panelist newPanelist = new Panelist { Email = emailAddress };
                newPanelist.SetProperty("FName", fName);
                newPanelist.SetProperty("LName", lName);

                AddedPanelists.Add(newPanelist);
                Panelists.Add(newPanelist);
            }
        }

        /// <summary>
        /// Remove a panelist
        /// </summary>
        /// <param name="emailAddresses"></param>
        public void RemovePanelists(IEnumerable<string> emailAddresses)
        {
            //ensure that we have all these address in the list
            foreach (string emailAddress in emailAddresses)
            {
                if (!PanelistExist(emailAddress))
                {
                    throw new Exception(string.Format("E-mail Address {0} doesn't belong to this list.", emailAddress));
                }
            }
            foreach (string emailAddress in emailAddresses)
            {
                RemovePanelist(emailAddress);
            }
        }

        /// <summary>
        /// Checks if the address exists in the list
        /// </summary>
        /// <param name="emailAddress"></param>
        public bool PanelistExist(string emailAddress)
        {
            if (Utilities.IsNullOrEmpty(emailAddress))
            {
                return false;
            }

            return Panelists.FindIndex(p => p.Email.Equals(emailAddress, StringComparison.InvariantCultureIgnoreCase)) >= 0;
        }

        /// <summary>
        /// Removes an email address from the list
        /// </summary>
        /// <param name="emailAddress"></param>
        public void RemovePanelist(string emailAddress)
        {
            if (Utilities.IsNullOrEmpty(emailAddress))
            {
                return;
            }

            int panelistIndex = Panelists.FindIndex(p => p.Email.Equals(emailAddress, StringComparison.InvariantCultureIgnoreCase));

            if (panelistIndex >= 0)
            {
                RemovedPanelists.Add(Panelists[panelistIndex]);
                Panelists.RemoveAt(panelistIndex);
            }
        }

        /// <summary>
        /// Gets the count of Panelists in this Panel
        /// </summary>
        public Int32 Count { get { return Panelists.Count; } }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        protected override void UpdatePanelists(IDbTransaction t)
        {
            Database db = DatabaseFactory.CreateDatabase();

            foreach (Panelist panelist in RemovedPanelists)
            {
                DBCommandWrapper command = GetDeletePanelistCommand(db, panelist.Email);
                db.ExecuteNonQuery(command, t);
            }

            foreach (Panelist panelist in AddedPanelists)
            {
                DBCommandWrapper command = GetInsertPanelistCommand(db, panelist.Email, panelist.GetProperty("FName"), panelist.GetProperty("LName"));
                db.ExecuteNonQuery(command, t);
            }

            AddedPanelists.Clear();
            RemovedPanelists.Clear();
        }

        /// <summary>
        /// Get a command to insert panelists
        /// </summary>
        /// <param name="db"></param>
        /// <param name="panelistEmail"></param>
        /// <param name="fName"></param>
        /// <param name="lName"></param>
        /// <returns></returns>
        protected DBCommandWrapper GetInsertPanelistCommand(Database db, string panelistEmail, string fName, string lName)
        {
            DBCommandWrapper insert = db.GetStoredProcCommandWrapper("ckbx_sp_Panel_InsertEmail");
            insert.AddInParameter("PanelID", DbType.Int32, ID.Value);
            insert.AddInParameter("EmailAddress", DbType.String, panelistEmail);
            insert.AddInParameter("FName", DbType.String, fName);
            insert.AddInParameter("LName", DbType.String, lName);

            return insert;
        }

        /// <summary>
        /// Get a command to delete panelists
        /// </summary>
        /// <param name="db"></param>
        /// <param name="panelistEmail"></param>
        /// <returns></returns>
        protected DBCommandWrapper GetDeletePanelistCommand(Database db, string panelistEmail)
        {
            DBCommandWrapper delete = db.GetStoredProcCommandWrapper("ckbx_sp_Panel_DeleteEmail");
            delete.AddInParameter("PanelID", DbType.Int32, ID.Value);
            delete.AddInParameter("EmailAddress", DbType.String, panelistEmail);

            return delete;
        }

        /// <summary>
        /// Determines if a EmailListPanel can be deleted. An EmailListPanel is eligible to be
        /// deleted if it is not in use by an invitation.
        /// </summary>
        /// <returns>True if the panel can be deleted. False if the panel can not be deleted.</returns>
        public bool CanDelete()
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Panel_CanDeleteELEntries");
            command.AddInParameter("PanelID", DbType.Int32, ID.Value);

            object result = db.ExecuteScalar(command);
            int count;
            Int32.TryParse(result.ToString(), out count);

            return count < 1;
        }

        /// <summary>
        /// Delete the email list
        /// </summary>
        /// <param name="transaction"></param>
        public override void Delete(IDbTransaction transaction)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Panel_DeleteELEntries");
            command.AddInParameter("PanelID", DbType.Int32, ID.Value);
            db.ExecuteNonQuery(command, transaction);

            base.Delete(transaction);
        }

        /// <summary>
        /// Load panel data
        /// </summary>
        /// <param name="data"></param>
        protected override void LoadBaseObjectData(DataRow data)
        {
            base.LoadBaseObjectData(data);

            AclID = DbUtility.GetValueFromDataRow<int?>(data, "AclId", null);
            DefaultPolicyID = DbUtility.GetValueFromDataRow<int?>(data, "DefaultPolicy", null);
        }


        /// <summary>
        /// Save panel
        /// </summary>
        /// <param name="p"></param>
        public void Save(CheckboxPrincipal p)
        {
            try
            {
                ModifiedBy = p.Identity.Name;

                Database db = DatabaseFactory.CreateDatabase();

                using (IDbConnection connection = db.GetConnection())
                {
                    connection.Open();
                    IDbTransaction transaction = connection.BeginTransaction();
                    DateTime? previousModified = LastModified;

                    try
                    {
                        LastModified = DateTime.Now;

                        Save(p, transaction);

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        ExceptionPolicy.HandleException(ex, "BusinessProtected");

                        transaction.Rollback();
                        LastModified = previousModified;
                        OnAbort(this, EventArgs.Empty);
                        throw new Exception("Unable to save data.");
                    }
                    finally
                    {
                        connection.Close();
                    }
                }

                //Call on commit only on success
                OnCommit(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                bool rethrow = ExceptionPolicy.HandleException(ex, "BusinessPublic");

                if (rethrow)
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Save panel
        /// </summary>
        /// <param name="p"></param>
        /// <param name="t"></param>
        public void Save(CheckboxPrincipal p, IDbTransaction t)
        {
            string permissionToCheck = ID.HasValue ? "EmailList.Edit" : "EmailList.Create";

            if (!AuthorizationFactory.GetAuthorizationProvider().Authorize(p, permissionToCheck))
            {
                throw new AuthorizationException();
            }

            base.Save(t);
        }

        /// <summary>
        /// Create the panel
        /// </summary>
        /// <param name="t"></param>
        protected override void Create(IDbTransaction t)
        {
            // Ensure no name conflict
            if (PanelManager.PanelWithNameExists(Name, PanelTypeName, null))
                throw new Exception("An email panel list with this name already exists");

            base.Create(t);

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Panel_CreateEmailListACL");
            command.AddInParameter("PanelID", DbType.Int32, ID.Value);
            command.AddInParameter("CreatedBy", DbType.String, CreatedBy);
            
            command.AddOutParameter("AclID", DbType.Int32, 4);
            command.AddOutParameter("DefaultPolicyID", DbType.Int32, 4);

            db.ExecuteNonQuery(command, t);

            AclID = (int)command.GetParameterValue("AclID");
            DefaultPolicyID = (int)command.GetParameterValue("DefaultPolicyID");
        }

        // <summary>
        // Gets a SecurityEditor for EmailListPanel
        // </summary>
        // <returns></returns>
        public override SecurityEditor GetEditor()
        {
            return new EmailListSecurityEditor(this);
        }

    }
}
