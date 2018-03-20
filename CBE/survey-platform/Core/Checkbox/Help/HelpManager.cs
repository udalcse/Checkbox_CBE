using System;
using System.Collections;
using System.Data;
using System.Security.Principal;
using System.Text;

using Checkbox.Management;
using Checkbox.Users;
using Prezza.Framework.Data;

namespace Checkbox.Help
{
    /// <summary>
    /// Static class for providing context-sensitive help links.
    /// </summary>
    public static class HelpManager
    {
        private static Hashtable _helpContext = new Hashtable();

        /// <summary>
        /// 
        /// </summary>
        private static Hashtable HelpContext
        {
            get { return _helpContext; }
            set { _helpContext = value; }
        }

        /// <summary>
        /// Retrieve the help URL for a specified page context.
        /// If an associated help file can not be found an empty string is returned.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetHelpContext(string path)
        {
            const int DEFAULT_PUBLIC_HELP_ID = 1;
            const int DEFAULT_ADMIN_HELP_ID = 5;

            IPrincipal principal = UserManager.GetCurrentPrincipal();
            string skinName = principal == null ? "Public" : "Admin";
            string pageContext = path.Substring(ApplicationManager.ApplicationRoot.Length + 1);
            string key = string.Format("{0}/{1}", skinName, pageContext.ToLower());

            if (HelpContext[key] != null)
            {
                return HelpContext[key].ToString();
            }

            int fileId = GetContextId(pageContext, skinName);

            if (fileId == 0)
            {
                fileId = principal == null ? DEFAULT_PUBLIC_HELP_ID : DEFAULT_ADMIN_HELP_ID;
            }

            StringBuilder helpPath = new StringBuilder();
            helpPath.AppendFormat("{0}/Help/{1}/Default_CSH.htm#{2}", ApplicationManager.ApplicationRoot, skinName, fileId);

            if (pageContext != string.Empty && helpPath.Length > 0)
            {
                lock (HelpContext.SyncRoot)
                {
                    HelpContext[key] = helpPath.ToString();
                }
            }

            return helpPath.ToString();
        }

        /// <summary>
        /// Retrieve the id of the help page associated with a given page and skin type.
        /// </summary>
        /// <param name="pageContext">The page.</param>
        /// <param name="skinName">The skin type.</param>
        /// <returns></returns>
        private static int GetContextId(string pageContext, string skinName)
        {
            object results;

            try
            {
                Database db = DatabaseFactory.CreateDatabase();
                DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Help_GetContextLink");
                command.AddInParameter("@SourceFilePath", DbType.String, pageContext);
                command.AddInParameter("@SkinName", DbType.String, skinName);
                results = db.ExecuteScalar(command);
            }
            catch (Exception)
            {
                return 0;
            }

            if (results != null)
            {
                int fileId;
                if (int.TryParse(results.ToString(), out fileId))
                {
                    return fileId;
                }
            }

            return 0;
        }
    }
}