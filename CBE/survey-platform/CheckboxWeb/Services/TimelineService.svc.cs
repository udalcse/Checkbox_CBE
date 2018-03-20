using System;
using System.Collections.Generic;
using System.ServiceModel.Activation;
using Checkbox.Wcf.Services;
using Checkbox.Wcf.Services.Proxies;
using Prezza.Framework.ExceptionHandling;

namespace CheckboxWeb.Services
{
    /// <summary>
    /// 
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class TimelineService : ITimelineService
    {
        #region ITimelineService Members

        /// <summary>
        /// Get timeline
        /// </summary>
        /// <param name="progressKey"></param>
        /// <returns></returns>
        public ServiceOperationResult<TimelineAnswer> GetTimeline(string authToken, string manager, long RequestID, long parentObjectID, string parentObjectType)
        {
            try
            {
                return new ServiceOperationResult<TimelineAnswer>
                {
                    CallSuccess = true,
                    ResultData = TimelineServiceImplementation.GetTimeline(AuthenticationService.GetCurrentPrincipal(authToken), manager, RequestID, parentObjectID, parentObjectType)
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<TimelineAnswer>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Get timeline settings for all events of the specific manager
        /// </summary>
        /// <param name="authTicket"> </param>
        /// <param name="manager"></param>
        /// <returns></returns>
        public ServiceOperationResult<List<TimelineSettings>> GetTimelineSettings(string authTicket, string manager)
        {
            try
            {
                return new ServiceOperationResult<List<TimelineSettings>>
                {
                    CallSuccess = true,
                    ResultData = TimelineServiceImplementation.GetTimelineSettings(AuthenticationService.GetCurrentPrincipal(authTicket), manager)
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<List<TimelineSettings>>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        public ServiceOperationResult<bool> UpdateTimelineEventPeriod(string authTicket, string manager, string eventName, string periodName, bool value)
        {
            try
            {
                return new ServiceOperationResult<bool>
                {
                    CallSuccess = true,
                    ResultData = TimelineServiceImplementation.UpdateTimelineEventPeriod(AuthenticationService.GetCurrentPrincipal(authTicket), manager, eventName, periodName, value)
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<bool>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        public ServiceOperationResult<bool> UpdateTimelineEventOrder(string authTicket, string manager, string eventName, int eventOrder)
        {
            try
            {
                return new ServiceOperationResult<bool>
                {
                    CallSuccess = true,
                    ResultData = TimelineServiceImplementation.UpdateTimelineEventOrder(AuthenticationService.GetCurrentPrincipal(authTicket), manager, eventName, eventOrder)
                };
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");

                return new ServiceOperationResult<bool>
                {
                    CallSuccess = false,
                    FailureExceptionType = ex.GetType().ToString(),
                    FailureMessage = ex.Message
                };
            }
        }

        #endregion
    }
}
