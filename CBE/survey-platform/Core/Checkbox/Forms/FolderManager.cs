using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Principal;
using System.Text;
using Checkbox.Pagination;
using Checkbox.Security;
using Checkbox.Security.Principal;
using Prezza.Framework.Data;
using Prezza.Framework.Security;
using Prezza.Framework.Security.Principal;
using Prezza.Framework.ExceptionHandling;

namespace Checkbox.Forms
{
    /// <summary>
    /// Static manager used to access folders.
    /// </summary>
    public static class FolderManager
    {
        /// <summary>
        /// Get a folder with the specified id.
        /// </summary>
        /// <param name="folderId"></param>
        /// <returns></returns>
        public static FormFolder GetFolder(int folderId)
        {
            FormFolder folder = new FormFolder();
            folder.Load(folderId);

            return folder;
        }

        /// <summary>
        /// Get a "lightweight" folder class.
        /// </summary>
        /// <param name="folderId"></param>
        /// <returns></returns>
        public static LightweightAccessControllable GetLightweightFolder(int folderId)
        {
            //Get the folder directly.  Folders themselves are lightweight so it's ok to load them here.  It may
            // seem redundant, but there are cases where having same type of base data structure in a list, such as
            // a list of surveys and folders, makes work at the UI level easier.
            FormFolder folder = GetFolder(folderId);

            if (folder != null)
            {
                return new LightweightAccessControllable { Name = folder.Name, ID = folderId, EntityType = "Folder", Owner = folder.CreatedBy, ChildrenCount = folder.ChildrenCount };
            }

            return null;
        }

        /// <summary>
        /// Return a boolean indicating if a folder with the specified name already exists.
        /// </summary>
        /// <param name="folderId"></param>
        /// <param name="folderName"></param>
        /// <param name="currentPrincipal"></param>
        /// <returns></returns>
        public static bool FolderExists(int? folderId, string folderName, IPrincipal currentPrincipal)
        {
            //white space is ignored when comparing the names of entities
            if (folderName != null)
            {
                folderName = folderName.Trim();
            }

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_RT_CheckFolderExists");
            command.AddInParameter("FolderName", DbType.String, folderName);
            command.AddInParameter("ItemId", DbType.Int32, folderId.GetValueOrDefault());

            if (currentPrincipal != null)
            {
                command.AddInParameter("CreatedBy", DbType.String, currentPrincipal.Identity.Name);
            }
            else
            {
                command.AddInParameter("CreatedBy", DbType.String, string.Empty);
            }

            //True if value is not zero; otherwise, false.
            return Convert.ToBoolean(Convert.ToInt32(db.ExecuteScalar(command)));
        }

        /// <summary>
        /// Get a list of accessible folders, sorted in ascending order by name
        /// </summary>
        /// <param name="currentPrincipal"></param>
        /// <param name="permissionToCheck"></param>
        /// <returns></returns>
        public static List<LightweightAccessControllable> ListAccessibleFolders(
            CheckboxPrincipal currentPrincipal,
            string permissionToCheck)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command;

            if (currentPrincipal.IsInRole("System Administrator"))
            {
                command = db.GetStoredProcCommandWrapper("ckbx_sp_Security_ListAllFolders");
                QueryHelper.AddPagingAndFilteringToCommandWrapper(command, new PaginationContext { SortField = "Name" });
            }
            else
            {
                command = QueryHelper.CreateListAccessibleCommand(
                    "ckbx_sp_Security_ListAccessibleFolders",
                    currentPrincipal,
                    new PaginationContext
                    {
                        SortField = "Name",
                        Permissions = new List<string> { permissionToCheck }
                    });
            }

            List<LightweightAccessControllable> folderList = new List<LightweightAccessControllable>();

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    while (reader.Read())
                    {
                        int folderId = DbUtility.GetValueFromDataReader(reader, "FolderId", -1);

                        if (folderId > 0)
                        {
                            LightweightAccessControllable folder = GetLightweightFolder(folderId);

                            if (folder != null)
                            {
                                folderList.Add(folder);
                            }
                        }
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            return folderList;
        }

        /// <summary>
        /// List all folders
        /// </summary>
        /// <param name="currentPrincipal"></param>
        /// <param name="sortProperty">Folder sort property.</param>
        /// <param name="sortAscending"></param>
        /// <returns></returns>
        public static DataSet GetAllFolders(ExtendedPrincipal currentPrincipal, string sortProperty, bool sortAscending)
        {
            //TODO: List All Folders - NWC 12/14/2010 - Only used by web.services so no need to implement just yet
            //Database db = DatabaseFactory.CreateDatabase();
            //DBCommandWrapper command = db.GetSqlStringCommandWrapper(QueryFactory.GetSelectAvailableFormFoldersQuery(currentPrincipal, sortProperty, sortAscending).ToString());
            //return db.ExecuteDataSet(command);

            return null;
        }

        /// <summary>
        /// Get all folders the current permission has the specified permission on.
        /// </summary>
        /// <param name="currentPrincipal">Current principal.</param>
        /// <param name="permissionToCheck">Permission to check.</param>
        /// <returns>Array of folders.</returns>
        public static List<FormFolder> GetAllFolders(ExtendedPrincipal currentPrincipal, string permissionToCheck)
        {
            List<FormFolder> folders = new List<FormFolder>();

            DataSet ds = GetAllFolders(currentPrincipal, "Name", true);

            if (ds != null && ds.Tables.Count > 0)
            {
                DataRow[] folderRows = ds.Tables[0].Select();

                IAuthorizationProvider authorizationProvider = AuthorizationFactory.GetAuthorizationProvider();

                foreach (DataRow folderRow in folderRows)
                {
                    int folderID = (int)folderRow["FolderID"];

                    FormFolder folder = new FormFolder();
                    folder.Load(folderID);


                    if (authorizationProvider.Authorize(currentPrincipal, folder, permissionToCheck))
                    {
                        folders.Add(folder);
                    }
                }
            }

            return folders;
        }


        /// <summary>
        /// Get the root folder
        /// </summary>
        /// <returns></returns>
        public static RootFormFolder GetRoot()
        {
            return new RootFormFolder();
        }

		/// <summary>
		/// ListFolderItems
		/// </summary>
		/// <returns></returns>
		public static List<int> ListFolderItems(int folderId)
		{
			string sql = "Select ItemID from ckbx_TemplatesAndFoldersView WHERE AncestorID = " + folderId;

			Database db = DatabaseFactory.CreateDatabase();
			DBCommandWrapper command = db.GetSqlStringCommandWrapper(sql);

			List<int> result = new List<int>();

			using (IDataReader reader = db.ExecuteReader(command))
			{
				try
				{
					while (reader.Read())
					{
						int? itemId = DbUtility.GetValueFromDataReader<int?>(reader, "ItemID", null);

						if (itemId.HasValue)
							result.Add(itemId.Value);
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

			return result;
		}
    }
}
