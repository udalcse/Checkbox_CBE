using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Interface definition for Timeline Service
    /// </summary>
    [ServiceContract]
    public interface ITimelineService
    {
        #region GetTimeline

        /// <summary>
        /// Returs timeline aggregated data or the timeline request processing status
        /// </summary>
        /// <param name="userAuthID"></param>
        /// <param name="manager"></param>
        /// <param name="RequestID"></param>
        /// <param name="parentObjectID"></param>
        /// <param name="parentObjectType"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<TimelineAnswer> GetTimeline(string userAuthID, string manager, long RequestID, long parentObjectID, string parentObjectType);

        #endregion

        #region GetTimelineSettings

        /// <summary>
        /// Get timeline settings for all events of the specific manager
        /// </summary>
        /// <param name="authTicket"> </param>
        /// <param name="manager"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<List<TimelineSettings>> GetTimelineSettings(string authTicket, string manager);

        /// <summary>
        /// Update period value for specified manager and event
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="manager"></param>
        /// <param name="eventName"></param>
        /// <param name="periodName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<bool> UpdateTimelineEventPeriod(string authTicket, string manager, string eventName, string periodName, bool value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authTicket"></param>
        /// <param name="manager"></param>
        /// <param name="dateFieldName"></param>
        /// <param name="eventOrder"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet]
        ServiceOperationResult<bool> UpdateTimelineEventOrder(string authTicket, string manager, string eventName, int eventOrder);


        #endregion
    }
}
