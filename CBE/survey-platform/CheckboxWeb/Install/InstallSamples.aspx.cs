using System;
using System.Web.Script.Serialization;
using Checkbox.Common;
using Checkbox.Web.Page;

namespace CheckboxWeb.Install
{
    public partial class InstallSamples : CheckboxServerProtectedPage
    {
        /// <summary>
        /// Progress ke
        /// </summary>
        private string ProgressKey
        {
            get { return Session["InstallProgressKey"] as string; }
            set { Session["InstallProgressKey"] = value; }
        }

        /// <summary>
        /// Get worker url
        /// </summary>
        /// <returns></returns>
        private string GetWorkerUrl()
        {
            return "InstallWorker.aspx?m=s&i=" + Request.QueryString["i"];
        }

        /// <summary>
        /// Generate the key for storing/retrieving progress
        /// </summary>
        protected virtual string GenerateProgressKey()
        {
            return string.Format("{0}_{1}", Session.SessionID, "Install");
        }

        /// <summary>
        /// Register included scripts
        /// </summary>
        protected virtual void RegisterIncludes()
        {
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
                    contentType: 'application/json; charset=utf-8',
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
                    {5})";

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
                onSuccess ?? "null");

            ClientScript.RegisterStartupScript(
                GetType(),
                "StartProgress",
                string.Format(scriptWrapperTemplate, startWorkScript, startPollingScript),
                true);
        }

        /// <summary>
        /// Register scripts
        /// </summary>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            //Set the progress key, which is a concatenation of current session id
            // and URL by default.
            ProgressKey = GenerateProgressKey();

            if (Utilities.IsNotNullOrEmpty(GetWorkerUrl()))
            {
                RegisterIncludes();
            }

            string redirectUrl = ResolveUrl("~/Login.aspx");

            ClientScript.RegisterStartupScript(
                GetType(),
                "SuccessRedirect",
                "setRedirectUrl('" + redirectUrl + "');",
                true);
        }

        /// <summary>
        /// Handle page load
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (Utilities.IsNotNullOrEmpty(GetWorkerUrl()))
            {
                RegisterStartProgressScript(
                    ProgressKey,
                    GetWorkerUrl(),
                    string.Empty,
                    string.Empty,
                    "onProgressError",
                    "onProgressUpdate",
                    "onProgressComplete");
            }
        }
    }
}
