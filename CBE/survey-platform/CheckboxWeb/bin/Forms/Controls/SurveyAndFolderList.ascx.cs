namespace CheckboxWeb.Forms.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public partial class SurveyAndFolderList : Checkbox.Web.Common.UserControlBase
    {
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
    }
}