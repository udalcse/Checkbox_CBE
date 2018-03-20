using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Checkbox.Web.Page;
using Checkbox.Web;
using Checkbox.Forms;
using Checkbox.Progress;
using Checkbox.Analytics.Import;

namespace CheckboxWeb.Forms.Surveys.Responses
{
    public partial class ImportProgress : ImportPage
    {
        [QueryParameter]
        public int SurveyId { get; set; }

        [QueryParameter("ReturnPage")]
        public string ReturnPage;

        /// <summary>
        /// Get worker url
        /// </summary>
        /// <returns></returns>
        protected override string GetWorkerUrl()
        {
            return ResolveUrl("~/Forms/Surveys/Responses/DoImport.aspx");
        }

        StateImportInfo StateImportInfo
        {
            get
            {
                if (Session["StateImportInfo"] == null)
                    Session["StateImportInfo"] = new StateImportInfo { IsCanceled = false };
                return Session["StateImportInfo"] as StateImportInfo;
            }
        }


        protected string CancelRedirectUrl
        {
            get
            {
                StateImportInfo.IsCanceled = true;
                return ReturnPage;
            }
        }

        /// <summary>
        /// Get worker url params
        /// </summary>
        /// <returns></returns>
        protected override object GetWorkerUrlData()
        {
            return new { SurveyId = SurveyId };
        }

        /// <summary>
        /// Bind events
        /// </summary>
        protected override void OnPageInit()
        {
            base.OnPageInit();
            //TypedMaster.DisableSaveButton();
        }

        /// <summary>
        /// Start the process when page is loaded
        /// </summary>
        protected override void OnPageLoad()
        {
            Master.OkVisible = false;
            Master.CancelEnable = false;
            Master.CancelTextId = "/common/close";

            //Seed progress cache so it's not empty on first request to get progress
            ProgressKey = (string)Session["ImportResultsProgressKey"];// GenerateProgressKey();

            //Session["ProgressKey"] = ProgressKey;

            SetProgressStatus(
                ProgressStatus.Running,
                20,
                100,
                "Importing XML file: validation");

            //Base method starts progress
            base.OnPageLoad();
        }
    }
}
