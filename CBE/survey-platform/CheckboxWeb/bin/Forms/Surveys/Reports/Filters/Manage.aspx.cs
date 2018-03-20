using System;
using Checkbox.Web;
using Checkbox.Web.Page;

namespace CheckboxWeb.Forms.Surveys.Reports.Filters
{
    public partial class Manage : SecuredPage
    {
        /// <summary>
        /// Survey editor access
        /// </summary>
        protected override string PageRequiredRolePermission
        {
            get { return "Analysis.Create"; }
        }

        /// <summary>
        /// Id of default selected library
        /// </summary>
        [QueryParameter("f")]
        public int? InitialFilterId { get; set; }

        /// <summary>
        /// Response template id
        /// </summary>
        [QueryParameter("s")]
        public int? SurveyId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageInit()
        {
            base.OnPageInit();


            var title = WebTextManager.GetText("/pageText/forms/surveys/reports/filters/manage.aspx/title");

            if (SurveyId.HasValue)
            {
                _buttonAddFilter.HRef = "javascript:showDialog('Add.aspx?s=" + SurveyId + "&onClose=onDialogClosed', 600, 800);";
                _filterList.ResponseTemplateId = SurveyId.Value;
            }
            
            Master.SetTitle(title);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!SurveyId.HasValue)
                return;

            string returnUrl;
            
            switch (Request.Params["back"])
            {
                case "report" :
                    returnUrl = ResolveUrl("~/Forms/Surveys/Reports/Manage.aspx?s=" + SurveyId);
                    break;
                case "reports" :
                    returnUrl = ResolveUrl("~/Forms/Surveys/Reports/Manage.aspx?s=" + SurveyId);
                    break;
                default:
                    returnUrl = ResolveUrl("~/Forms/Manage.aspx?s=" + SurveyId);
                    break;
            }

            Master.ShowBackButton(returnUrl, true);
        }
    }
}
