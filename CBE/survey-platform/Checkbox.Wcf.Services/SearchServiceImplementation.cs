using System;
using System.Collections.Generic;
using System.Data;
using Checkbox.Progress;
using Checkbox.Wcf.Services.Proxies;
using Checkbox.Search;
using System.Configuration;
using Checkbox.Management;
using Checkbox.Security.Principal;
using Prezza.Framework.Data;
using System.Threading;
using Checkbox.Common;

namespace Checkbox.Wcf.Services
{

    /// <summary>
    /// Underlying implementation class for universal search service.
    /// </summary>
    public static class SearchServiceImplementation
    {
        /// <summary>
        /// Get progress status
        /// </summary>
        /// <param name="progressKey"></param>
        /// <returns></returns>
        public static bool Initialize(CheckboxPrincipal callingPrincipal, int expPeriodSeconds)
        {
            if (callingPrincipal.IsInRole("System Administrator"))
                return true;

            try
            {
                Guid requestID = SearchManager.InitializeAvailableObjects(callingPrincipal, expPeriodSeconds);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Run a search or return search results if any
        /// </summary>
        /// <param name="callingPrincipal"></param>
        /// <param name="Term"></param>
        /// <param name="RequestID"></param>
        /// <returns></returns>
        public static SearchAnswer Search(CheckboxPrincipal callingPrincipal, string Term, Guid RequestID)
        {
            SearchAnswer result = new SearchAnswer();
            int cnt = 0;

            Term = Utilities.AdvancedHtmlDecode(Term);

            do
            {
                string status = SearchManager.GetStatus(callingPrincipal, Term, ref RequestID);

                switch (status)
                {
                    case "Succeeded":
                        result.Completed = true;
                        //get all results
                        result.Results = SearchManager.CollectResults(callingPrincipal, RequestID, Term);
                        cnt = 2;
                        break;
                    case "Pending":
                        result.Pending = true;
                        //get all available results
                        result.Results = SearchManager.CollectResults(callingPrincipal, RequestID, Term);
                        cnt = 2;
                        break;
                    case "CollectingObjects":
                        result.CollectingObjects = true;
                        cnt = 2;
                        break;
                    case "Created":
                        SearchManager.RunSearch(callingPrincipal, RequestID, Term);
                        Thread.Sleep(100);
                        break;
                    default:
                        return null;
                }
                cnt++;
            } while (cnt < 2);

            result.RequestID = RequestID;
            return result;
        }

        /// <summary>
        /// Returns search settings
        /// </summary>
        /// <param name="callingPrincipal"></param>
        /// <returns></returns>
        public static SearchSettingsInfo[] GetSearchSettings(CheckboxPrincipal callingPrincipal)
        {
            if (!callingPrincipal.IsInRole("System Administrator"))
                return null;

            return SearchManager.GetSearchSettings();
        }

        /// <summary>
        /// Changes the order of the search results
        /// </summary>
        /// <param name="callingPrincipal"></param>
        /// <param name="objectType"></param>
        /// <param name="order"></param>
        public static void UpdateSearchResultsOrder(CheckboxPrincipal callingPrincipal, string objectType, int order)
        {
            if (!callingPrincipal.IsInRole("System Administrator"))
                return;

            SearchManager.UpdateSearchResultsOrder(objectType, order);
        }

        /// <summary>
        /// Includes or excludes these objects from search
        /// </summary>
        /// <param name="callingPrincipal"></param>
        /// <param name="objectType"></param>
        /// <param name="included"></param>
        public static void ToggleSearchObjectType(CheckboxPrincipal callingPrincipal, string objectType, bool included)
        {
            if (!callingPrincipal.IsInRole("System Administrator"))
                return;

            SearchManager.ToggleSearchObjectType(objectType, included);
        }

        /// <summary>
        /// Changes the roles set which may search for the objects of given type
        /// </summary>
        /// <param name="callingPrincipal"></param>
        /// <param name="objectType"></param>
        /// <param name="roles"></param>
        public static void UpdateObjectsRoles(CheckboxPrincipal callingPrincipal, string objectType, string roles)
        {
            if (!callingPrincipal.IsInRole("System Administrator"))
                return;

            SearchManager.UpdateObjectsRoles(objectType, roles);
        }
    }
}
