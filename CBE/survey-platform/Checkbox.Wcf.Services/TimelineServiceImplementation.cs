using System;
using System.Collections.Generic;
using System.Data;
using Checkbox.Progress;
using Checkbox.Wcf.Services.Proxies;
using Checkbox.Timeline;
using Checkbox.Web;
using System.Configuration;
using Checkbox.Management;
using Checkbox.Security.Principal;
using Prezza.Framework.Data;

namespace Checkbox.Wcf.Services
{

    /// <summary>
    /// Underlying implementation class for progress reporting service.
    /// </summary>
    public static class TimelineServiceImplementation
    {
        /// <summary>
        /// Get progress status
        /// </summary>
        /// <param name="progressKey"></param>
        /// <returns></returns>
        public static TimelineAnswer GetTimeline(CheckboxPrincipal callingPrincipal, string manager, long RequestID, long parentObjectID, string parentObjectType)
        {
            string message = null;
            TimelineRequestStatus status = RequestID == 0 ? TimelineRequestStatus.None : TimelineManager.GetRequestStatus(callingPrincipal, RequestID, out message);
            TimelineAggregatedResult[] results = null;
            
            if (status == TimelineRequestStatus.None)
            {
                //create a new request
                RequestID = TimelineManager.GetRequest(callingPrincipal, manager, ApplicationManager.AppSettings.TimelineRequestExpiration);
                status = TimelineManager.GetRequestStatus(callingPrincipal, RequestID, out message);
            }

            if (status == TimelineRequestStatus.Succeeded)
            {
                //aggregate results
                results = TimelineManager.GetResults(callingPrincipal, RequestID, parentObjectID, parentObjectType);
            }

            if (results != null)
            {
                //take into account the time zone
                foreach (var r in results)
                {
                    if (r.Date.HasValue)
                    {
                        r.Date = WebUtilities.ConvertToClientTimeZone(r.Date);
                    }
                }
            }

            return new TimelineAnswer() { RequestID = RequestID, Status = status, Results = results, Message = message,
                CacheTimeoutSeconds = ApplicationManager.AppSettings.TimelineRequestExpiration,
                RecordsPerPage = ApplicationManager.AppSettings.TimelineRecordsPerPage};
        }

        /// <summary>
        /// Get timeline settings for all events of the specific manager
        /// </summary>
        /// <param name="callingPrincipal"> </param>
        /// <param name="manager"></param>
        /// <returns></returns>
        public static List<TimelineSettings> GetTimelineSettings(CheckboxPrincipal callingPrincipal, string manager)
        {
            return TimelineManager.GetTimelineSettings(callingPrincipal, manager);
        }

        /// <summary>
        /// Update period value for specified manager and event
        /// </summary>
        /// <param name="callingPrincipal"> </param>
        /// <param name="manager"></param>
        /// <param name="eventName"></param>
        /// <param name="periodName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool UpdateTimelineEventPeriod(CheckboxPrincipal callingPrincipal, string manager, string eventName, string periodName, bool value)
        {
            return TimelineManager.UpdateTimelineEventPeriod(callingPrincipal, manager, eventName, periodName, value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callingPrincipal"></param>
        /// <param name="manager"></param>
        /// <param name="eventName"></param>
        /// <param name="eventOrder"></param>
        /// <returns></returns>
        public static bool UpdateTimelineEventOrder(CheckboxPrincipal callingPrincipal, string manager, string eventName, int eventOrder)
        {
            return TimelineManager.UpdateTimelineEventOrder(callingPrincipal, manager, eventName, eventOrder);
        }
    }
}
