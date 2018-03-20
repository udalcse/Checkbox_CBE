using System;
using System.Data;
using Prezza.Framework.Security;
using Prezza.Framework.Security.Principal;
using Prezza.Framework.ExceptionHandling;
using Prezza.Framework.Data;
using Prezza.Framework.Common;

using Checkbox.Common;
using Checkbox.Forms.Security;

using Checkbox.Globalization.Text;

namespace Checkbox.Forms
{
    /// <summary>
    /// Summary description for FormFolder.
    /// </summary>
    [Serializable]
    public class FormFolder : Folder
    {
        /// <summary>
        /// Construct a new form folder
        /// </summary>
        public FormFolder()
            : this(new[] { "FormFolder.FullControl", "FormFolder.Read" }, new[] { "FormFolder.FullControl", "FormFolder.Read" })
        {
        }

        /// <summary>
        /// Construct a new form folder
        /// </summary>
        /// <param name="supportedPermissionMasks"></param>
        /// <param name="supportedPermissions"></param>
        public FormFolder(string[] supportedPermissionMasks, string[] supportedPermissions)
            : base(supportedPermissionMasks, supportedPermissions)
        {

        }

        /// <summary>
        /// Create a form folder policy.
        /// </summary>
        /// <param name="permissions"></param>
        /// <returns></returns>
        public override Policy CreatePolicy(string[] permissions)
        {
            return new FormFolderPolicy(permissions);
        }

        /// <summary>
        /// Add a survey to a folder
        /// </summary>
        /// <param name="form"></param>
        public override void Add(object form)
        {
            ArgumentValidation.CheckExpectedType(form, typeof(ResponseTemplate));
            MoveToFolder(this, ((ResponseTemplate)form).ID.Value);
        }

        /// <summary>
        /// Add a survey to a folder.
        /// </summary>
        /// <param name="id"></param>
        public override void Add(int id)
        {
            MoveToFolder(this, id);
        }

        /// <summary>
        /// Remove a survey from a folder.  Does nothing.
        /// </summary>
        /// <param name="form"></param>
        public override void Remove(object form)
        {
            ArgumentValidation.CheckExpectedType(form, typeof(ResponseTemplate));
        }

        /// <summary>
        /// Remove a survey from a folder.  Does nothing
        /// </summary>
        /// <param name="id"></param>
        public override void Remove(int id)
        {
        }

        /// <summary>
        /// Copy the specified folder
        /// </summary>
        /// <returns></returns>
        public override Folder Copy(ExtendedPrincipal principal, string languageCode)
        {
            FormFolder copy = new FormFolder();

            //Figure out the copied folder name
            int copyCount = 1;
            string newFolderName = TextManager.GetText("/pageText/manageSurveys.aspx/copy", languageCode) + " " + copyCount + " " + TextManager.GetText("/pageText/manageSurveys.aspx/of", languageCode) + " " + Name;

            while (FolderManager.FolderExists(null, newFolderName, principal))
            {
                copyCount++;
                newFolderName = TextManager.GetText("/pageText/manageSurveys.aspx/copy", languageCode) + " " + copyCount + " " + TextManager.GetText("/pageText/manageSurveys.aspx/of", languageCode) + " " + Name;
            }


            copy.Name = newFolderName;
            copy.Description = Description;
            copy.Save(principal);

            string sql = "Select ItemID from ckbx_TemplatesAndFoldersView WHERE AncestorID = " + ID + " order by itemtype, itemname";
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetSqlStringCommandWrapper(sql);

            IAuthorizationProvider authProvider = AuthorizationFactory.GetAuthorizationProvider();

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    // copy the contents of the folder
                    while (reader.Read())
                    {
                        int? itemId = DbUtility.GetValueFromDataReader<int?>(reader, "ItemID", null);

                        if(itemId.HasValue)
                        {
                            LightweightResponseTemplate toCopy = ResponseTemplateManager.GetLightweightResponseTemplate(itemId.Value);

                            if (authProvider.Authorize(principal, toCopy, "Form.Edit"))
                            {
                                ResponseTemplate templateCopy = ResponseTemplateManager.CopyTemplate(itemId.Value, null, principal, languageCode);
                                copy.Add(templateCopy);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    bool rethrow = ExceptionPolicy.HandleException(ex, "BusinessProtected");

                    if (rethrow)
                    {
                        throw;
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            return copy;
        }

        /// <summary>
        /// Save the current folder
        /// </summary>
        /// <param name="principal"></param>
        public void Save(ExtendedPrincipal principal)
        {
            Save(principal, false);
        }

        /// <summary>
        /// Save the current folder
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="SimpleSecurity"></param>
        public void Save(ExtendedPrincipal principal, bool SimpleSecurity)
        {
            if (!AuthorizationFactory.GetAuthorizationProvider().Authorize(principal, "FormFolder.FullControl"))
            {
                throw new AuthorizationException();
            }

            if (FolderManager.FolderExists(ID, Name, principal))
            {
                throw new ArgumentException("Folder name must be unique.  '" + Name + "' is currently in use by another folder.");
            }

            Database db = DatabaseFactory.CreateDatabase();

            using (IDbConnection connection = db.GetConnection())
            {
                connection.Open();

                IDbTransaction t = connection.BeginTransaction();
                try
                {
                    if (!ID.HasValue || ID <= 0)
                    {
                        Create(principal, t, SimpleSecurity);
                    }
                    else
                    {
                        Update(principal, t);
                    }

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
        /// Persist the folder in the database
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="t"></param>
        protected void Create(ExtendedPrincipal principal, IDbTransaction t, bool SimpleSecurity)
        {
            //Create a default policy
            string[] defaultPolicyPermissions = SimpleSecurity ? (new string[] { "FormFolder.FullControl", "FormFolder.Read" }) : (new string[0]);
            Policy defaultPolicy = CreatePolicy(defaultPolicyPermissions);

            //Create an access list
            AccessControlList acl = new AccessControlList();
            Policy creatorAccessPolicy = CreatePolicy(SupportedPermissions);
            acl.Add(principal, creatorAccessPolicy);
            acl.Save();

            SetAccess(defaultPolicy, acl);

            Database db = DatabaseFactory.CreateDatabase();

            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Folder_Create");
            command.AddInParameter("FolderName", DbType.String, Name);
            command.AddInParameter("FolderDescription", DbType.String, Description);
            command.AddInParameter("CreatedBy", DbType.String, principal.Identity.Name);
            command.AddInParameter("AclID", DbType.Int32, AclID);
            command.AddInParameter("DefaultPolicyID", DbType.Int32, DefaultPolicyID);
            command.AddOutParameter("FolderID", DbType.Int32, 4);

            db.ExecuteNonQuery(command, t);

            object folderID = command.GetParameterValue("FolderID");

            if (folderID != null && folderID != DBNull.Value)
            {
                ID = (int)folderID;
            }
            else
            {
                throw new Exception("Unable to save folder.");
            }

            DBCommandWrapper addToFormFolder = db.GetStoredProcCommandWrapper("ckbx_sp_Folder_CreateFormFolder");
            addToFormFolder.AddInParameter("FolderID", DbType.Int32, (int)command.GetParameterValue("FolderID"));

            db.ExecuteNonQuery(addToFormFolder, t);
        }

        /// <summary>
        /// Persist updates to the folder
        /// </summary>
        /// <param name="t"></param>
        /// <param name="principal"></param>
        protected void Update(ExtendedPrincipal principal, IDbTransaction t)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Folder_Update");
            command.AddInParameter("FolderID", DbType.Int32, ID.Value);
            command.AddInParameter("FolderName", DbType.String, Name);
            command.AddInParameter("FolderDescription", DbType.String, Description);
            command.AddInParameter("CreatedBy", DbType.String, CreatedBy);

            db.ExecuteNonQuery(command, t);
        }

        /// <summary>
        /// Delete the folder
        /// </summary>
        /// <param name="principal"></param>
        public void Delete(ExtendedPrincipal principal)
        {
            if (!AuthorizationFactory.GetAuthorizationProvider().Authorize(principal, this, "FormFolder.FullControl"))
            {
                throw new AuthorizationException();
            }

            Database db = DatabaseFactory.CreateDatabase();

            using (IDbConnection connection = db.GetConnection())
            {
                connection.Open();
                IDbTransaction transaction = connection.BeginTransaction();

                try
                {
                    DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Folder_GetTemplates");
                    command.AddInParameter("FolderID", DbType.Int32, ID.Value);

                    using (IDataReader reader = db.ExecuteReader(command))
                    {
                        try
                        {
                            // delete the contents of the folder
                            while (reader.Read())
                            {
                                int surveyId = DbUtility.GetValueFromDataReader(reader, "ItemID", -1);

                                if (ResponseTemplateManager.CanBeDeleted(surveyId, principal))
                                {
                                    ResponseTemplateManager.DeleteResponseTemplate(surveyId, transaction);
                                }
                                else
                                {
                                    throw new AuthorizationException();
                                }
                            }
                        }
                        finally
                        {
                            reader.Close();
                        }
                    }

                    // delete the folder
                    command = db.GetStoredProcCommandWrapper("ckbx_sp_Folder_Delete");
                    command.AddInParameter("FolderID", DbType.Int32, ID);
                    db.ExecuteNonQuery(command, transaction);

                    transaction.Commit();
                }
                catch
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
        
        /// <summary>
        /// Get a folder security editor
        /// </summary>
        /// <returns></returns>
        public override SecurityEditor GetEditor()
        {
            return new FormFolderSecurityEditor(this);
        }


        /// <summary>
        /// Override create
        /// </summary>
        /// <param name="t"></param>
        protected override void Create(IDbTransaction t)
        {
            throw new Exception("The method or operation is not implemented.  Call overloaded Create(...) instead.");
        }

        /// <summary>
        /// Override update.
        /// </summary>
        /// <param name="t"></param>
        protected override void Update(IDbTransaction t)
        {
            throw new Exception("The method or operation is not implemented.  Call overloaded Update(...) instead.");
        }
    }


    /// <summary>
    /// Special case for root folder
    /// </summary>
    public class RootFormFolder : FormFolder
    {
        /// <summary>
        /// Constructor
        /// </summary>
        internal RootFormFolder()
            : base(new string[0], new string[0])
        {
            Name = "ROOT FOLDER";
            Description = "The implicit root";
            CreatedBy = "admin";
        }

        /// <summary>
        /// Add an item to the folder
        /// </summary>
        /// <param name="id"></param>
        public override void Add(int id)
        {
            if (id <= 0)
            {
                throw new Exception("Invalid Response Template ID.");
            }

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_RT_MoveTemplateToRoot");
            command.AddInParameter("ResponseTemplateID", DbType.Int32, id);

            db.ExecuteNonQuery(command);
        }

        /// <summary>
        /// Add an item to the folder
        /// </summary>
        /// <param name="responseTemplate"></param>
        public override void Add(object responseTemplate)
        {
            ArgumentValidation.CheckExpectedType(responseTemplate, typeof(ResponseTemplate));
            // call the Add(int id) member
            Add(((ResponseTemplate)responseTemplate).ID.Value);
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        /// <param name="id"></param>
        public override void Remove(int id)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        /// <param name="item"></param>
        public override void Remove(object item)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        public override Policy DefaultPolicy
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        public override string[] SupportedPermissionMasks
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        public override string[] SupportedPermissions
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        /// <param name="permissions"></param>
        /// <returns></returns>
        public override Policy CreatePolicy(string[] permissions)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        /// <returns></returns>
        public override SecurityEditor GetEditor()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public override Folder Copy(ExtendedPrincipal owner, string languageCode)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
