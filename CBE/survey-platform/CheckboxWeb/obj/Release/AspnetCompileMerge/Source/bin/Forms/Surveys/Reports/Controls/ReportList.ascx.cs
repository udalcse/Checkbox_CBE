using System.Web.UI;

namespace CheckboxWeb.Forms.Surveys.Reports.Controls
{
    public partial class ReportList : Checkbox.Web.Common.UserControlBase
    {
        /// <summary>
        /// 
        /// </summary>
        protected string Period
        {
            get
            {
                return string.IsNullOrEmpty(Request["period"]) ? "0" : Request["period"]; 
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected string DateFieldName
        {
            get
            {
                return string.IsNullOrEmpty(Request["dateFieldName"]) ? "" : Request["dateFieldName"];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected string SearchTerm
        {
            get
            {
                return string.IsNullOrEmpty(Request["term"]) ? "" : Request["term"];
            }
        }


        /// <summary>
        /// Id of survey to list responses for
        /// </summary>
        public int SurveyId { get; set; }

        /// <summary>
        /// Callback for response selected
        /// </summary>
        public string ReportSelectedClientCallback { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool AllowDelete { get { return true; } }
    }
}