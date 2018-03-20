using System.ServiceModel;
using System.ServiceModel.Web;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Interface definition for style management service, which allows basic style
    /// management functionality, including listing styles, copying styles, etc.
    /// </summary>
    [ServiceContract]
    public interface IStyleManagementService
    {
        /// <summary>
        /// List all form styles
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="filterField"></param>
        /// <param name="filterValue"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<PagedListResult<StyleListItem[]>> ListFormStyles(string authToken, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue);

        /// <summary>
        /// List all chart styles
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="sortAscending"></param>
        /// <param name="filterField"></param>
        /// <param name="filterValue"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<StyleListItem[]> ListChartStyles(string authToken, int pageNumber, int pageSize, string sortField, bool sortAscending, string filterField, string filterValue);

        /// <summary>
        /// Get simple list item associated with a single style.
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="styleId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<StyleListItem> GetStyleListItem(string authTicket, int styleId, string type);

        /// <summary>
        /// Delete the specified form style
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="styleId"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<bool> DeleteFormStyle(string authTicket, int styleId);

        /// <summary>
        /// Delete the spelified form styles
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="styleIds"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<bool> DeleteFormStyles(string authTicket, int[] styleIds);

        /// <summary>
        /// Delete the specified chart style
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="styleId"></param>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<bool> DeleteChartStyle(string authTicket, int styleId);

        /// <summary>
        /// Copy the specific style.
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="styleId"></param>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        ServiceOperationResult<object> CopyFormStyle(string authTicket, int styleId, string languageCode);    
    }
}
