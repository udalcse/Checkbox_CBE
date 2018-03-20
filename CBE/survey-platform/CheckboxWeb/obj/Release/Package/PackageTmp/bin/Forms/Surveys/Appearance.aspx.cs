using System;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Web;
using Checkbox.Web.Page;

namespace CheckboxWeb.Forms.Surveys
{
    /// <summary>
    /// 
    /// </summary>
    public partial class Appearance : ResponseTemplatePage
    {
        /// <summary>
        /// Get page specific title
        /// </summary>
        protected override string PageSpecificTitle
        {
			get { return WebTextManager.GetText("/pageText/forms/appearance.aspx/title"); }
        }
        
        /// <summary>
        /// Handle page init
        /// </summary>
        protected override void OnPageInit()
        {
            base.OnPageInit();

            _appearanceEditor.Initialize(ResponseTemplate);

            Master.OkClick += _okBtn_Click;

			string title = PageSpecificTitle;

			if (string.IsNullOrEmpty(title))
				title = "Appearance";

            Master.Title = title + " - " + Utilities.StripHtml(ResponseTemplate.Name, 64);
        }

        /// <summary>
        /// Handle ok button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _okBtn_Click(object sender, EventArgs e)
        {
            _appearanceEditor.Update(ResponseTemplate);

            ResponseTemplate.ModifiedBy = User.Identity.Name;
            ResponseTemplate.Save();

            ResponseTemplateManager.MarkTemplateUpdated(ResponseTemplate.ID.Value);

            Master.CloseDialog("appearance", false);
        }
    }
}
