using System.Web;
using Checkbox.Common;
using Checkbox.Progress;
using System.Web.Script.Serialization;

namespace Checkbox.Web.Page
{
    /// <summary>
    /// Extension of basic application page to add support for recording and reporting progress information.
    /// </summary>
    public abstract class ProgressReportingEnabledPage : ApplicationPage
    {
        /// <summary>
        /// Get URL to call to start work
        /// </summary>
        /// <returns></returns>
        protected virtual string GetWorkerUrl()
        {
            return string.Empty;
        }

        /// <summary>
        /// Get additional data to pass to worker url. 
        /// </summary>
        /// <returns></returns>
        protected virtual object GetWorkerUrlData()
        {
            return null;
        }

        /// <summary>
        /// Get id of progress container.  This value is case sensitive.
        /// </summary>
        protected virtual string ProgressContainerId { get { return "progressDiv"; } }

        //
        protected virtual string OnCompleteMethod { get { return "onProgressComplete"; } }
        protected virtual string OnUpdateMethod { get { return "onProgressUpdate"; } }
        protected virtual string OnErrorMethod { get { return "onProgressError"; } }

        /// <summary>
        /// Get key to use for progress tracking with progress provider.
        /// </summary>
        protected virtual string ProgressKey { get; set; }

        /// <summary>
        /// Set the current page progress
        /// </summary>
        /// <param name="progressData"></param>
        protected void SetProgress(ProgressData progressData)
        {
            ProgressProvider.SetProgress(ProgressKey, progressData);
        }

        /// <summary>
        /// Get progress
        /// </summary>
        /// <param name="totalItems"></param>
        /// <param name="status"></param>
        /// <param name="currentItem"></param>
        /// <param name="message"></param>
        protected void SetProgressStatus(ProgressStatus status, int currentItem, int totalItems, string message)
        {
            SetProgress(new ProgressData
            {
                Status = status,
                CurrentItem = currentItem,
                TotalItemCount = totalItems,
                Message = message
            });
        }

        /// <summary>
        /// Generate the key for storing/retrieving progress
        /// </summary>
        protected virtual string GenerateProgressKey()
        {
            return Session.SessionID + Request.Url;
        }

        /// <summary>
        /// Register included scripts
        /// </summary>
        protected virtual void RegisterIncludes()
        {
            //Script for progress bar
            RegisterClientScriptInclude("ProgressBar", ResolveUrl("~/Resources/jquery.progressbar.min.js"));

            //Script for ajax polling of progress
            RegisterClientScriptInclude("AjaxProgress", ResolveUrl("~/Resources/AjaxProgress.js"));
        }

        /// <summary>
        /// Start progress tracking.  Names of JS functions called are passsed as arguments.
        /// </summary>
        protected virtual void RegisterStartProgressScript(string progressKey, string workerUrl, object workerUrlParams, string progressContainerId, string onError, string onUpdate, string onSuccess)
        {
            const string scriptWrapperTemplate = "$(document).ready(function(){{{0};{1};}});";

            const string startWorkScriptTemplate =
                @"$.ajax({{
                    type: 'GET',
                    url: '{0}',
                    async: true,
                    contentType: 'application/x-www-form-urlencoded; charset=utf-8',
                    data: {1},
                    dataType: 'json',
                    timeout: 100}})";

            const string startPollingScriptTemplate =
                @"ajaxProgress.startProgress(
                    '{0}',
                    '{1}',
                    '{2}',
                    {3},
                    {4},
                    {5},
                    {6})";

            string workerParamsJson = "{}";

            if (workerUrlParams != null)
            {
                var serializer = new JavaScriptSerializer();
                workerParamsJson = serializer.Serialize(workerUrlParams);
            }

            string startWorkScript = string.Format(
                startWorkScriptTemplate,
                workerUrl,
                workerParamsJson);

            string startPollingScript = string.Format(
                startPollingScriptTemplate,
                progressKey,
                progressContainerId,
                ResolveUrl("~/"),
                onUpdate ?? "null",
                onError ?? "null",
                onSuccess ?? "null",
                GetProgressProviderType());

            ClientScript.RegisterStartupScript(
                GetType(),
                "StartProgress",
                string.Format(scriptWrapperTemplate, startWorkScript, startPollingScript),
                true);
        }

        /// <summary>
        /// Get URL to call to start work
        /// </summary>
        /// <returns></returns>
        protected virtual string GetProgressProviderType()
        {
            return "''";
        }

        /// <summary>
        /// Register scripts
        /// </summary>
        protected override void OnPageInit()
        {
            base.OnPageInit();

            //Set the progress key, which is a concatenation of current session id
            // and URL by default.
            ProgressKey = GenerateProgressKey();
        }

        /// <summary>
        /// Handle page load
        /// </summary>
        protected override void OnPageLoad()
        {
            base.OnPageLoad();
            RegisterStartProgressScript();
        }

        /// <summary>
        /// 
        /// </summary>
        protected void RegisterStartProgressScript()
        {
            if (Utilities.IsNotNullOrEmpty(GetWorkerUrl()))
            {
                RegisterIncludes();
                RegisterStartProgressScript(
                    ProgressKey,
                    GetWorkerUrl(),
                    GetWorkerUrlData(),
                    ProgressContainerId,
                    OnErrorMethod,
                    OnUpdateMethod,
                    OnCompleteMethod);
            }
        }

        /// <summary>
        /// Return result as JSON
        /// </summary>
        /// <param name="result"></param>
        /// <param name="response"> </param>
        protected void WriteResult(object result, HttpResponse response)
        {
            if (response != null)
            {
                var serializer = new JavaScriptSerializer();

                response.ContentType = "application/json";
                response.Write(serializer.Serialize(result));                
            }
        }

        /// <summary>
        /// Return result as JSON
        /// </summary>
        /// <param name="result"></param>
        protected void WriteResult(object result)
        {
            var serializer = new JavaScriptSerializer();

            Response.ContentType = "application/json";
            Response.Write(serializer.Serialize(result));
        }
    }
}