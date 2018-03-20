using System;
using System.Data;
using System.Text;
using System.Collections.Generic;
using Prezza.Framework.Data;
using Prezza.Framework.Security.Principal;

namespace Checkbox.Content
{
    /// <summary>
    /// Content manager for database content
    /// </summary>
    public static class DBContentManager
    {
        /// <summary>
        /// Load a folder
        /// </summary>
        /// <param name="contentFolderID"></param>
        /// <param name="currentPrincipal"></param>
        /// <returns></returns>
        public static DBContentFolder GetFolder(int contentFolderID, ExtendedPrincipal currentPrincipal)
        {
            var folder = new DBContentFolder(contentFolderID, null, currentPrincipal);
            folder.Load();

            return folder;
        }

        /// <summary>
        /// Load a folder
        /// </summary>
        /// <param name="folderPath"></param>
        /// <param name="currentPrincipal"></param>
        /// <returns></returns>
        public static DBContentFolder GetFolder(string folderPath, ExtendedPrincipal currentPrincipal)
        {
            var folder = new DBContentFolder(null, folderPath, currentPrincipal);
            folder.Load();
            return folder;
        }

        /// <summary>
        /// Get the db content item
        /// </summary>
        /// <param name="itemID"></param>
        /// <returns></returns>
        public static DBContentItem GetItem(int itemID)
        {
            var item = new DBContentItem(itemID);
            item.Load();

            return item;
        }

        /// <summary>
        /// List content items in a folder
        /// </summary>
        /// <param name="folderPath"></param>
        /// <param name="currentUser"></param>
        /// <param name="contentFilters"></param>
        /// <returns></returns>
        public static Dictionary<string, ContentItem> ListItems(string folderPath, ExtendedPrincipal currentUser, params string[] contentFilters)
        {
            var items = new Dictionary<string, ContentItem>(StringComparer.InvariantCultureIgnoreCase);

            var parentFolder = GetFolder(folderPath, currentUser);

            //If the path is invalid, folder id will be null.  Check for null to prevent listing of all folders.  All accessible
            //folders should be children of pre-defined top-level folders
            if (parentFolder.FolderID.HasValue)
            {

                var sb = new StringBuilder();

                foreach (string filter in contentFilters)
                {
                    sb.Append("'");
                    sb.Append(filter.Replace("'", "''"));
                    sb.Append("'");
                }

                var db = DatabaseFactory.CreateDatabase();
                var command = db.GetStoredProcCommandWrapper("ckbx_sp_Content_ListFolderContent");

                command.AddInParameter("ParentFolderID", DbType.Int32, parentFolder.FolderID);
                command.AddInParameter("CreatedBy", DbType.String, currentUser.Identity.Name);
                command.AddInParameter("MimeContentTypes", DbType.String, sb.ToString());
                command.AddInParameter("ForceListAll", DbType.Boolean, currentUser.IsInRole("System Administrator"));

                using (var reader = db.ExecuteReader(command))
                {
                    try
                    {
                        while (reader.Read())
                        {
                            var itemID = (int)reader["ItemID"];

                            //Load the item data
                            var item = new DBContentItem(itemID);
                            item.Load();

                            var itemName = item.ItemName;

                            if (string.IsNullOrEmpty(itemName) || items.ContainsKey(item.ItemName))
                            {
                                item.ItemName = item.ItemID.ToString();
                            }

                            items[item.ItemName] = item;
                        }
                    }
                    finally
                    {
                        reader.Close();
                    }
                }
            }

            return items;
        }

        /// <summary>
        /// Get a list of all children of the parent folder
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="folderPath"></param>
        /// <returns></returns>
        public static Dictionary<string, ContentFolder> ListFolders(string folderPath, ExtendedPrincipal currentUser)
        {
            var folders = new Dictionary<string, ContentFolder>();

            var parentFolder = GetFolder(folderPath, currentUser);

            //If the path is invalid, folder id will be null.  Check for null to prevent listing of all folders.  All accessible
            //folders should be children of pre-defined top-level folders
            if (parentFolder.FolderID.HasValue)
            {
                var db = DatabaseFactory.CreateDatabase();
                var command = db.GetStoredProcCommandWrapper("ckbx_sp_Content_ListFolders");

                command.AddInParameter("ParentFolderID", DbType.Int32, parentFolder.FolderID);
                command.AddInParameter("CreatedBy", DbType.String, currentUser.Identity.Name);
                command.AddInParameter("ForceListAll", DbType.Boolean, currentUser.IsInRole("System Administrator"));


                using (var reader = db.ExecuteReader(command))
                {
                    try
                    {
                        while (reader.Read())
                        {
                            var folderID = (int)reader["FolderID"];

                            //Load the item data
                            var folder = new DBContentFolder(folderID, string.Empty, currentUser);
                            folder.Load();

                            folders[folder.FolderName] = folder;
                        }
                    }
                    finally
                    {
                        reader.Close();
                    }
                }
            }

            return folders;
        }
    }
}
