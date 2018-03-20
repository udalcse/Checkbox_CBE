using System;
using Checkbox.Progress;
using Checkbox.Progress.DatabaseProvider;
using Checkbox.Wcf.Services.Proxies;

namespace Checkbox.Wcf.Services
{

    /// <summary>
    /// Underlying implementation class for progress reporting service.
    /// </summary>
    public static class ProgressReportingServiceImplementation
    {
        /// <summary>
        /// Get progress status
        /// </summary>
        /// <param name="progressKey"></param>
        /// <param name="provider"> </param>
        /// <returns></returns>
        public static GetProgressResult GetProgressStatus(string progressKey, string provider)
        {
            if (string.IsNullOrEmpty(progressKey))
            {
                return new GetProgressResult
                {
                    Success = false,
                    ErrorMessage =  "No progress key specified."
                };
            }

            IProgressProvider progressProvider = GetProgressProvider(provider);
            var progressData = progressProvider.GetProgress(progressKey);

            if (progressData == null)
            {
                return new GetProgressResult 
                { 
                    Success = false,
                    ErrorMessage = "No progress data found for specified key: " + progressKey
                };
            }

            return new GetProgressResult 
            { 
                Success = true,
                ProgressData =  new Proxies.ProgressData
                {
                    StatusMessage = progressData.Message,
                    ErrorMessage = progressData.ErrorMessage,
                    TotalItemCount = progressData.TotalItemCount,
                    CurrentItem = progressData.CurrentItem,
                    ProgressKey = progressKey,
                    Result = progressData.Result,
                    Status = progressData.Status.ToString(),
                    DownloadUrl = progressData.AdditionalData
                    
                }
            };
        }

        /// <summary>
        /// Set progress status
        /// </summary>
        /// <param name="progressData"></param>
        /// <param name="provider"> </param>
        public static void SetProgressStatus(Proxies.ProgressData progressData, string provider)
        {
            IProgressProvider progressProvider = GetProgressProvider(provider);

            progressProvider.SetProgress(
                progressData.ProgressKey,
                progressData.StatusMessage,
                progressData.ErrorMessage,
                (ProgressStatus)Enum.Parse(typeof(ProgressStatus), progressData.Status),
                progressData.CurrentItem,
                progressData.TotalItemCount);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="proviterType"></param>
        /// <returns></returns>
        private static IProgressProvider GetProgressProvider(string proviterType)
        {
            IProgressProvider progressProvider;
            
            switch (proviterType)
            {
                case "database":
                    progressProvider = new DatabaseProgressProvider();
                    break;
                default:
                    progressProvider = new CacheableProgressProvider();
                    break;
            }

            return progressProvider;
        }
    }
}
