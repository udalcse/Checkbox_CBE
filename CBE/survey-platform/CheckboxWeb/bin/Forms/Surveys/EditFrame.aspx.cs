using Checkbox.Web.Page;

namespace CheckboxWeb.Forms.Surveys
{
    public partial class EditFrame : ResponseTemplatePage
    {
        /// <summary>
        /// 
        /// </summary>
        protected override string PageSpecificTitle
        {
            get { return string.Empty; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageLoad()
        {
            base.OnPageLoad();

            Master.HideDialogButtons();
        }
    }
}