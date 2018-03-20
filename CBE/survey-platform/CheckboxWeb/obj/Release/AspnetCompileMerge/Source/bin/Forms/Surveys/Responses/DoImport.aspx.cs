using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Script.Serialization;
using System.IO;
using System.Xml;

using Checkbox.Web;
using Checkbox.Web.Page;
using Checkbox.Management;
using Checkbox.Common;
using Checkbox.Globalization.Text;
using Checkbox.Forms;
using Checkbox.Progress;

using Prezza.Framework.Caching;
using Prezza.Framework.ExceptionHandling;

using Checkbox.Analytics.Import;

namespace CheckboxWeb.Forms.Surveys.Responses
{
    public partial class DoImport : ImportPage
    {
        [QueryParameter]
        public int SurveyId { get; set; }

        ResponseTemplate _rt;

        ResponseTemplate Survey
        {
            get 
            {
                if (_rt == null)
                    _rt = ResponseTemplateManager.GetResponseTemplate(SurveyId);

                return _rt;
            }
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
                StateImportInfo.IsCanceled = false;
				return ResolveUrl("~/Forms/Surveys/Responses/Manage.aspx?s=" + SurveyId);
            }
        }

        /// <summary>
        /// Start worker, if necessary
        /// </summary>
        protected override void OnPageLoad()
        {
            ProgressKey = (string)Session["ImportResultsProgressKey"];

            base.OnPageLoad();

            List<ItemInfo> srcItems = Session["ItemInfos"] as List<ItemInfo>;
            List<ResponseInfo> srcResponses = Session["ResponseInfos"] as List<ResponseInfo>;

            try
            {
                //Check for data
                if (srcResponses == null)
                {
                    throw new Exception("Unable to do import: A required data, such as XML file. was not set.");
                }
                StateImportInfo.IsCanceled = false;
                XmlDataImporter.Import(Survey, srcItems, srcResponses, ProgressKey, StateImportInfo);

                WriteResult(new { success = true });

                //Set progress to 100%
                ProgressProvider.SetProgress(
                    ProgressKey,
                    new ProgressData
                    {
                        CurrentItem = 100,
                        Status = ProgressStatus.Completed,
                        Message = TextManager.GetText("/controlText/doExport.aspx/complete", Survey.LanguageSettings.DefaultLanguage),
                        TotalItemCount = 100
                    }
                );
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "UIProcess");

                //Set progress to errr
                ProgressProvider.SetProgress(
                    ProgressKey,
                    new ProgressData
                    {
                        CurrentItem = 100,
                        Status = ProgressStatus.Error,
                        Message = "An error occurred while exporting results data.",
                        ErrorMessage = ex.Message,
                        TotalItemCount = 100
                    }
                );
            }
        }
    }
}
