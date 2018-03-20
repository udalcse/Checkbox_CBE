using System;
using Checkbox.Forms;
using Checkbox.Management;
using Checkbox.Users;
using Checkbox.Web;
using Checkbox.Web.Page;

namespace CheckboxWeb.Forms.Surveys
{
    public partial class CreateQuick : ApplicationPage
    {
        /// <summary>
        /// Bind events
        /// </summary>
        protected override void OnPageInit()
        {
            base.OnPageInit();

            Master.OkClick += OkBtn_Click;

            //Set page title
            Master.Title = WebTextManager.GetText("/pageText/forms/surveys/create.aspx/createSurvey");
        }

        /// <summary>
        /// Handle _okBtnClick
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OkBtn_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                var rt = ResponseTemplateManager.CreateResponseTemplate(UserManager.GetCurrentPrincipal());
                ResponseTemplateManager.InitializeTemplate(rt, _properties.SurveyName);
          //      rt.BehaviorSettings.EnableScoring = _properties.EnableScoring;
            //    rt.BehaviorSettings.IsActive = _properties.ActivateSurvey;

                rt.Save();

                if (rt.ID.HasValue)
                {
                    CloseAndRedirect(rt.ID.Value);
                }
            }
        }

        private void CloseAndRedirect(int id)
        {
            var folder = FolderManager.GetRoot();
            folder.Add(id);

            Page.ClientScript.RegisterClientScriptBlock(
                GetType(),
                "Redirect",
                string.Format("closeWindowAndRedirectParentPage('', null, 'Edit.aspx?f={0}&s={1}');", null, id),
                true);
        }
    }
}