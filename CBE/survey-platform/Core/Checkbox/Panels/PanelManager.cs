using System;
using System.Collections.Generic;
using System.Data;
using Checkbox.Globalization.Text;
using Checkbox.Security.Principal;
using Checkbox.Users;
using Prezza.Framework.Data;
using Prezza.Framework.Data.Sprocs;
using Prezza.Framework.Security;
using Checkbox.Pagination;
using System.Linq;

namespace Checkbox.Panels
{
    /// <summary>
    /// Class to provide command panel management functionality.
    /// </summary>
    public class PanelManager
    {
        /// <summary>
        /// Create a panel of the specified type.
        /// </summary>
        /// <param name="panelTypeID"></param>
        /// <returns></returns>
        public static Panel CreatePanel(Int32 panelTypeID)
        {
            Panel p = new PanelFactory().CreatePanel(panelTypeID);

            if (p != null
                && UserManager.GetCurrentPrincipal() != null)
            {
                p.CreatedBy = UserManager.GetCurrentPrincipal().Identity.Name;
            }

            return p;
        }

        /// <summary>
        /// Get panel type id from name
        /// </summary>
        /// <param name="panelTypeName"></param>
        /// <returns></returns>
        public static int? GetPanelTypeIdFromName(string panelTypeName)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Panel_GetTypeInfoFromName");
            command.AddInParameter("PanelTypeName", DbType.String, panelTypeName);

            int? panelTypeID = null;
            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    while (reader.Read())
                    {
                        panelTypeID = (int)reader[0];
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            return panelTypeID;
        }

        /// <summary>
        /// Gets a panel type name from the type ID
        /// </summary>
        /// <param name="panelTypeID"></param>
        /// <returns></returns>
        public static string GetPanelTypeNameFromId(int panelTypeID)
        {
            PanelFactory.PanelTypeInfo info = PanelFactory.GetPanelTypeInfo(panelTypeID);
            return info.ClassTypeName;
        }

        /// <summary>
        /// Save the specified panel
        /// </summary>
        /// <param name="p"></param>
        /// <param name="currentPrincipal"></param>
        public static void SavePanel(Panel p, CheckboxPrincipal currentPrincipal)
        {
            if (p is EmailListPanel)
            {
                ((EmailListPanel)p).Save(currentPrincipal);
            }
            else
            {
                p.Save();
            }
        }

        /// <summary>
        /// Create an email list panel.
        /// </summary>
        /// <returns></returns>
        public static EmailListPanel CreateEmailListPanel()
        {
            return (EmailListPanel)CreatePanel("Checkbox.Panels.EmailListPanel");
        }

        /// <summary>
        /// Create a panel of the specified type
        /// </summary>
        /// <param name="panelTypeName"></param>
        /// <returns></returns>
        public static Panel CreatePanel(string panelTypeName)
        {
            int? panelTypeId = GetPanelTypeIdFromName(panelTypeName);

            if (panelTypeId.HasValue)
            {
                return CreatePanel(panelTypeId.Value);
            }

            return null;
        }

        /// <summary>
        /// Get the panel with the specified id.
        /// </summary>
        /// <param name="panelID"></param>
        /// <returns></returns>
        public static Panel GetPanel(Int32 panelID)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Panel_GetTypeID");
            command.AddInParameter("PanelID", DbType.Int32, panelID);

            int? panelTypeID = null;
            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    while (reader.Read())
                    {
                        panelTypeID = (int)reader[0];
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            if (panelTypeID == null)
            {
                return null;
            }

            Panel panel = CreatePanel(panelTypeID.Value);
            panel.Load(panelID);

            return panel;
        }

        /// <summary>
        /// Applies filtering, sorting and pagination to a raw list of users
        /// </summary>
        /// <param name="rawPanelistList">A list of panelists</param>
        /// <param name="paginationContext"></param>
        /// <returns>A list of user unique identifiers meeting the filter, sort, and page criteria of the pagination context</returns>
        /// <remarks>This method allows lists of user unique identifiers to be filtered and sorted based on profile properties, which may not be known to the call that generates the raw list</remarks>
        public static List<Panelist> FilterPageAndSortPanelists(List<Panelist> rawPanelistList, PaginationContext paginationContext)
        {
            List<Panelist> panelistList = new List<Panelist>();

            //If there is a sort or search specified, we're going to have to load all the profile properties in order to perform the sort/search
            if (
                (paginationContext.IsFiltered /*&& String.Compare(paginationContext.FilterField, "UniqueIdentifier", StringComparison.InvariantCultureIgnoreCase) != 0*/)
                || (paginationContext.IsSorted /* && String.Compare(paginationContext.FilterField, "UniqueIdentifier", StringComparison.InvariantCultureIgnoreCase) != 0*/)
                )
            {
                //Do filtering if applicable 
                //NB: Assumes that only the email field is filterable
                if (paginationContext.IsFiltered && paginationContext.FilterField == "Email")
                {
                    panelistList = rawPanelistList.Where(p => p.Email == paginationContext.FilterValue) as List<Panelist>;
                }
                else
                {
                    panelistList = rawPanelistList;
                }

                //Do sorting (again, only applies to email field)
                if (paginationContext.IsSorted && paginationContext.SortField == "Email")
                {
                    panelistList.Sort();
                    if (!paginationContext.SortAscending)
                    {
                        panelistList.Reverse();
                    }
                }
                
            }
            else
            {
                panelistList = rawPanelistList;
                panelistList.Sort();
            }

            //Finally, select page of results and set item count
            paginationContext.ItemCount = panelistList.Count;

            int startIndex = paginationContext.GetStartIndex();
            int endIndex = paginationContext.GetEndIndex(paginationContext.PageSize);

            if (startIndex > panelistList.Count)
            {
                return new List<Panelist>();
            }

            if (endIndex >= panelistList.Count)
            {
                endIndex = panelistList.Count - 1;
            }

            //Return range
            return panelistList.GetRange(startIndex, endIndex - startIndex + 1);
        }

        /// <summary>
        /// Get a AccessControllableResource which has the same ACL and DefaultPolicy as a given panel.
        /// </summary>
        /// <param name="panelID">ID of the panel to clone ACL and DefaultPolicy from.</param>
        /// <returns>A <see cref="LightweightAccessControllable"/> object initialized with a
        /// a given Panel's ACL and DefaultPolicy.</returns>
        public static LightweightAccessControllable GetLightWeightPanel(int panelID)
        {
            LightweightPanel lightweightTemplate = new LightweightPanel(panelID);
            StoredProcedureCommandExtractor.ExecuteProcedure(ProcedureType.Select, lightweightTemplate);

            return lightweightTemplate;
        }

        ///<summary>
        /// Get the count of email addresses associated with a given <see cref="EmailListPanel"/>.
        ///</summary>
        ///<param name="panelID">ID of the <see cref="EmailListPanel"/> to get count from.</param>
        ///<returns>int count of associated email addresses.</returns>
        public static int GetEmailListPanelAddressCount(int panelID)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_EmailListPanel_GetEmailAddressCount");
            command.AddInParameter("PanelID", DbType.Int32, panelID);

            object results = db.ExecuteScalar(command);
            int count;

            if (results != null && Int32.TryParse(results.ToString(), out count))
            {
                return count;
            }
            
            return 0;
        }

        /// <summary>
        /// Copy the panel
        /// </summary>
        /// <returns></returns>
        public static EmailListPanel CopyEmailListPanel(EmailListPanel panel, CheckboxPrincipal currentPrincipal, string languageCode)
        {
            //Determine the new name
            Int32 copyVersion = 1;
            string proposedName = panel.Name + " " + TextManager.GetText("/common/duplicate", languageCode) + " " + copyVersion;
            while (PanelWithNameExists(proposedName, panel.PanelTypeName, null))
            {
                copyVersion++;
                proposedName = panel.Name + " " + TextManager.GetText("/common/duplicate", languageCode) + " " + copyVersion;
            }

            EmailListPanel copy = new EmailListPanel
            {
                Name = proposedName,
                Description = panel.Description,
                CreatedBy = currentPrincipal.Identity.Name
            };

            //copy panelists
            List<Panelist> panelistsToCopy = panel.Panelists;

            foreach (Panelist p in panelistsToCopy)
            {
                copy.AddPanelist(
                    p.Email,
                    p.GetProperty("FName"),
                    p.GetProperty("LName"));
            }

            //Create an access list
            //AccessControlList acl = new AccessControlList();
            Policy creatorAccessPolicy = copy.CreatePolicy(panel.SupportedPermissions);
           // acl.Add(currentPrincipal, creatorAccessPolicy);
           // acl.Save();

            //Policy defaultPolicy = copy.CreatePolicy(new string[] { });

            //copy.SetAccess(defaultPolicy, acl);

            copy.Save(currentPrincipal);

            copy.ACL.Add(currentPrincipal, creatorAccessPolicy);
            copy.ACL.Save();

            return copy;
        }


        /// <summary>
        /// Checks whether or not a proposed list name is in use
        /// </summary>
        /// <param name="panelName">Name to validate</param>
        /// <param name="panelTypeName"></param>
        /// <param name="idToIgnore">Existing panel id to ignore when  checking for duplicates</param>
        /// <returns></returns>
        public static bool PanelWithNameExists(string panelName, string panelTypeName, int? idToIgnore)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper duplicateCommand = db.GetStoredProcCommandWrapper("ckbx_sp_Panel_CheckNameExists");
            duplicateCommand.AddInParameter("Name", DbType.String, panelName);
            duplicateCommand.AddInParameter("PanelTypeName", DbType.String, panelTypeName);
            duplicateCommand.AddInParameter("IDToIgnore", DbType.Int32, idToIgnore);

            using (IDataReader duplicateReader = db.ExecuteReader(duplicateCommand))
            {
                try
                {
                    if (duplicateReader.Read())
                    {
                        if (duplicateReader.GetInt32(0) > 0)
                        {
                            return true;
                        }
                    }
                }
                finally
                {
                    duplicateReader.Close();
                }
            }

            return false;

        }
    }

}
