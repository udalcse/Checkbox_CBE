using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;
using System;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Interface definition for Search Service
    /// </summary>
    [ServiceContract]
    public interface ISearchService
    {
        /// <summary>
        /// Prepares cached structures to perform fast search
        /// </summary>
        /// <param name="userAuthID"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<bool> Initialize(string userAuthID);

        /// <summary>
        /// Runs the search processes or returns search results
        /// </summary>
        /// <param name="userAuthID"></param>
        /// <param name="Term"></param>
        /// <param name="RequestID"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<SearchAnswer> Search(string userAuthID, string Term, string RequestID);

        /// <summary>
        /// Returns search settings
        /// </summary>
        /// <param name="userAuthID"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<SearchSettingsInfo[]> GetSearchSettings(string userAuthID);
        
        /// <summary>
        /// Changes the order of the search results
        /// </summary>
        /// <param name="userAuthID"></param>
        /// <param name="objectType"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<object> UpdateSearchResultsOrder(string userAuthID, string objectType, int order);

        /// <summary>
        /// Includes or excludes these objects from search
        /// </summary>
        /// <param name="userAuthID"></param>
        /// <param name="objectType"></param>
        /// <param name="included"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<object> ToggleSearchObjectType(string userAuthID, string objectType, bool included);

        /// <summary>
        /// Changes the roles set which may search for the objects of given type
        /// </summary>
        /// <param name="userAuthID"></param>
        /// <param name="objectType"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<object> UpdateObjectsRoles(string userAuthID, string objectType, string roles);
    }
}


