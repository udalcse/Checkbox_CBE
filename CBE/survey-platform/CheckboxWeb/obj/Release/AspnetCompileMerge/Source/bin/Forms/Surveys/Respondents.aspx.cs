using System;
using System.Collections.Generic;
using Checkbox.Wcf.Services.Proxies;
using Checkbox.Web;
using Checkbox.Web.Page;

namespace CheckboxWeb.Forms.Surveys
{
    /// <summary>
    /// 
    /// </summary>
    public partial class Respondents : ResponseTemplateSecurityEditorPage
    {
        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageInit()
        {
            base.OnPageInit();

            _grantSurveyAccess.Initialize(SecuredResourceType.Survey, ResponseTemplate.ID.Value, "Form.Fill");

            Master.SetTitle(WebTextManager.GetText("/pageText/surveySecuritySettings.aspx/respondentsList"));
            Master.OkVisible = false;
            Master.CancelTextId = "/controlText/AccessListEditor.ascx/closeEditor";
            Master.CancelClick += CloseWindowButton_Click;
        }

        /// <summary>
        /// Handles the 'Close Window' event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CloseWindowButton_Click(object sender, EventArgs e)
        {
            var args = new Dictionary<string, string>
                           {
                               {"op", "refresh"},
                           };

            Master.CloseDialog(args);
        }
    }
}