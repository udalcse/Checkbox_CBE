using System;
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
    public class ProgressReportingService : IProgressReportingService
    {
        #region IProgressReportingService Members

        /// <summary>
        /// Get status data for for a progress-enabled operation.
        /// </summary>
        /// <param name="progressKey"></param>
        /// <param name="provider"> </param>
        /// <returns></returns>
        public GetProgressResult GetProgressStatus(string progressKey, string provider)
        {
            try
            {
                return ProgressReportingServiceImplementation.GetProgressStatus(progressKey, provider);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");
                throw;
            }
        }

        /// <summary>
        /// Set status for a progress-enabled operation.
        /// </summary>
        /// <param name="progressData"></param>
        /// <param name="provider"> </param>
        public void SetProgressStatus(ProgressData progressData, string provider)
        {
            try
            {
                ProgressReportingServiceImplementation.SetProgressStatus(progressData, provider);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "Service");
                throw;
            }
        }

        #endregion
    }
}
