using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Prezza.Framework.Data;
using Checkbox.Security;
using System.Data;
using Checkbox.Pagination;
using Prezza.Framework.Security.Principal;
using Checkbox.Security.Principal;

namespace Checkbox.Panels
{
    public static class EmailListManager
    {

        /// <summary>
        /// Get an <see cref="Array"/> of EmailList objects where the principal has
        /// the specified permissions.
        /// </summary>
        /// <param name="currentPrincipal">Principal accessing list</param>
        /// <param name="paginationContext"></param>
        /// the caller to specify whether all or any of the permissions must be met.</param>
        /// <returns><see cref="Array"/> of email list data objects.</returns>
        public static List<EmailListPanel> GetAccessibleEmailLists(ExtendedPrincipal currentPrincipal, PaginationContext paginationContext)
        {
            return
                ListAvailableEmailLists(currentPrincipal, paginationContext).Select(GetEmailListPanel).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="panelId"></param>
        /// <returns></returns>
        public static EmailListPanel GetEmailListPanel(int panelId)
        {
            var panel = new EmailListPanel();
            panel.Load(panelId);

            return panel;
        }

        /// <summary>
        /// Determines if a proposed name is already used by an existing email list.
        /// </summary>
        /// <param name="emailListID">ID of an email list to ignore when checking for duplicates.</param>
        /// <param name="proposedName">Name to check.</param>
        /// <returns>True if an email list other than the one specified by emailListID exists with the specified name; 
        /// False in all other cases. 
        /// </returns>
        public static bool IsDuplicateName(int? emailListID, string proposedName)
        {
            //white space is ignored when comparing the names of entities
            if (proposedName != null)
            {
                proposedName = proposedName.Trim();
            }

            return PanelManager.PanelWithNameExists(proposedName, "Checkbox.Panels.EmailListPanel", emailListID);
        }

        /// <summary>
        /// Gets all EmailLists available for use by the logged-in user.
        /// </summary>
        private static List<int> ListEmailLists(ExtendedPrincipal principal, PaginationContext paginationContext)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command;

            //Shortcut to list all for system admins
            if (principal.IsInRole("System Administrator"))
            {
                command = db.GetStoredProcCommandWrapper("ckbx_sp_Security_ListAccessibleEmailListsAdmin");
                QueryHelper.AddPagingAndFilteringToCommandWrapper(command, paginationContext);
            }
            else
            {
                command = QueryHelper.CreateListAccessibleCommand(
                    "ckbx_sp_Security_ListAccessibleEmailLists",
                    principal,
                    paginationContext);
            }

            List<int> emailListPanelIds = new List<int>();

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    while (reader.Read())
                    {
                        int panelId = DbUtility.GetValueFromDataReader(reader, "PanelId", -1);

                        if (panelId > 0 &&
                            !emailListPanelIds.Contains(panelId))
                        {
                            emailListPanelIds.Add(panelId);
                        }
                    }

                    if (reader.NextResult() && reader.Read())
                    {
                        paginationContext.ItemCount = DbUtility.GetValueFromDataReader(reader, "RecordCount", 0);
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            return emailListPanelIds;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentPrincipal">Principal accessing lists</param>
        /// <param name="paginationContext"></param>
        /// <returns></returns>
        public static List<int> ListAvailableEmailLists(ExtendedPrincipal currentPrincipal, PaginationContext paginationContext)
        {
            //If no pagination context set, provide it
            if (paginationContext == null)
            {
                paginationContext = new PaginationContext
                    {
                        Permissions = new List<string> {"EmailList.View"}
                    };
            }

            return ListEmailLists(currentPrincipal, paginationContext);
        }


        /// <summary>
        /// Gets a list of EmailLists available for editing by the logged-in user
        /// </summary>
        /// <param name="principal">Principal to make request on behalf of.</param>
        /// <param name="paginationContext"></param>
        public static List<int> ListEditableEmailLists(ExtendedPrincipal principal, PaginationContext paginationContext)
        {
            //If no pagination context set, provide it
            if (paginationContext == null)
            {
                paginationContext = new PaginationContext();
            }

            //Set permissions
            paginationContext.Permissions = new List<string> { "EmailList.Edit" };

            return ListEmailLists(principal, paginationContext);
        }

        /// <summary>
        /// Create an email list panel.
        /// </summary>
        /// <returns></returns>
        public static EmailListPanel CreateEmailList(string listName, string listDescription, ExtendedPrincipal principal)
        {
            string cleanName = listName.Trim();
            string cleanDescription = listDescription.Trim();

            if (!Management.ApplicationManager.AppSettings.AllowHTMLNames)
            {
                cleanName = WebUtility.HtmlEncode(cleanName);
                cleanDescription = WebUtility.HtmlEncode(cleanDescription);
            }

            EmailListPanel panel = PanelManager.CreateEmailListPanel();
            panel.Name = cleanName;
            panel.Description = cleanDescription;
            panel.CreatedBy = principal.Identity.Name;
            panel.Save();

            if (Management.ApplicationManager.UseSimpleSecurity)
            {
                AccessManager.AddPermissionsToDefaultPolicy(panel, principal, "EmailList.Edit", "EmailList.View", "EmailList.Delete");
            }

            return panel;
        }

        /// <summary>
        /// Copy the list
        /// </summary>
        /// <returns></returns>
        public static EmailListPanel CopyEmailList(EmailListPanel panel, CheckboxPrincipal currentPrincipal, string languageCode)
        {
            return PanelManager.CopyEmailListPanel(panel, currentPrincipal, languageCode);
        }

        ///<summary>
        /// Get the count of email addresses associated with a given <see cref="EmailListPanel"/>.
        ///</summary>
        ///<param name="emailListID">ID of the <see cref="EmailListPanel"/> to get count from.</param>
        ///<returns>int count of associated email addresses.</returns>
        public static int GetEmailListAddressCount(int emailListID)
        {
            return PanelManager.GetEmailListPanelAddressCount(emailListID);
        }
    }
}
