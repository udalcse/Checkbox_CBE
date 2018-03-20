using System;
using System.Data;
using Checkbox.Common;
using Checkbox.Management;
using Checkbox.Pagination;
using Prezza.Framework.Data;
using Prezza.Framework.Security.Principal;

namespace Checkbox.Security
{
    /// <summary>
    /// Simple helper for creating commands for accessing Checkbox security sprocs.
    /// </summary>
    public static class QueryHelper
    {
        //TODO: Figure out how to handle cases where non-checkbox users are placed in System Administrator role by 
        // mapping in web.config.

        /// <summary>
        /// Return a query command wrapper populated with common inputs for listing securable resources
        /// </summary>
        /// <param name="procName"></param>
        /// <param name="currentPrincipal"></param>
        /// <param name="paginationContext"></param>
        /// <returns></returns>
        public static DBCommandWrapper CreateListAccessibleCommand(
            string procName,
            ExtendedPrincipal currentPrincipal,
            PaginationContext paginationContext)
        {
            if(paginationContext.Permissions.Count == 0
                || paginationContext.Permissions.Count > 3)
            {
                throw new Exception("QueryHelper.CreateListAccessibleCommand requires that one, two or three permissions are specified.");
            }

            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper(procName);
             
            command.AddInParameter("UniqueIdentifier", DbType.String, Utilities.AdvancedHtmlEncode(currentPrincipal.Identity.Name));
            command.AddInParameter("FirstPermissionName", DbType.String, paginationContext.Permissions.Count > 0 ? paginationContext.Permissions[0] : string.Empty);
            command.AddInParameter("SecondPermissionName", DbType.String,paginationContext.Permissions.Count > 1 ? paginationContext.Permissions[1] : string.Empty);
            command.AddInParameter("RequireBothPermissions", DbType.Boolean, paginationContext.PermissionJoin == PermissionJoin.All);
            command.AddInParameter("UseAclExclusion", DbType.Boolean, ApplicationManager.AppSettings.AllowExclusionaryAclEntries);
            if (paginationContext.Permissions.Count > 2)
            {
                command.AddInParameter("ThirdPermissionName", DbType.String, paginationContext.Permissions[2]);
            }

            AddPagingAndFilteringToCommandWrapper(command, paginationContext);

            return command;
        }

        /// <summary>
        /// Add paging and filtering parameters to sproc.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="paginationContext"></param>
        public static void AddPagingAndFilteringToCommandWrapper(DBCommandWrapper command, PaginationContext paginationContext)
        {
            command.AddInParameter("PageNumber", DbType.Int32, paginationContext.CurrentPage);
            command.AddInParameter("ResultsPerPage", DbType.Int32, paginationContext.PageSize);
            command.AddInParameter("SortField", DbType.String, paginationContext.SortField ?? string.Empty);
            command.AddInParameter("SortAscending", DbType.Boolean, paginationContext.SortAscending);
            command.AddInParameter("FilterField", DbType.String, paginationContext.FilterField ?? string.Empty);
            command.AddInParameter("FilterValue", DbType.String, paginationContext.FilterValue ?? string.Empty);
            
            if (paginationContext.StartDate != null)
                command.AddInParameter("StartDate", DbType.DateTime, paginationContext.StartDate);
            if (paginationContext.EndDate != null)
                command.AddInParameter("EndDate", DbType.DateTime, paginationContext.EndDate);
            if (!string.IsNullOrEmpty(paginationContext.DateFieldName))
                command.AddInParameter("DateFieldName", DbType.String, paginationContext.DateFieldName);
        }
    }
}
