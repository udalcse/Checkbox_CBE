using System;
using Checkbox.Common;
using Checkbox.Forms;
using Checkbox.Forms.Logic;
using Checkbox.Forms.Logic.Configuration;
using Checkbox.Web;
using Checkbox.Web.Page;

namespace CheckboxWeb.Forms.Surveys
{
    public partial class PageBranches : SecuredPage
    {
        /// <summary>
        /// ID of page
        /// </summary>
        [QueryParameter("p")]
        public int PageId { get; set; }

        /// <summary>
        /// Id of response template
        /// </summary>
        [QueryParameter("s")]
        public int ResponseTemplateId { get; set; }

        [QueryParameter("l")]
        public string EditLanguage { get; set; }

        /// <summary>
        /// Initialize rule editor control
        /// </summary>
        /// <param name="e"></param>
        protected override void  OnPageInit()
        {
 	        base.OnPageInit();

            Master.IsDialog = false;

            Master.OkVisible = false;
            Master.CancelVisible = false;
            

            if (!Page.IsPostBack)
            {
                RuleDataService rds = ResponseTemplate.CreateRuleDataService(ResponseTemplateId);
                ResponseTemplate rt = ResponseTemplateManager.GetResponseTemplate(ResponseTemplateId);
                TemplatePage tp = rt.GetPage(PageId);

                if (rds == null || tp == null)
                {
                    throw new Exception("Unable to load survey or page to edit.");
                }

                if (Utilities.IsNullOrEmpty(EditLanguage))
                {
                    EditLanguage = rt.LanguageSettings.DefaultLanguage;
                }

                _ruleEditor.Initialize(rds, RuleType.PageBranchCondition, ResponseTemplateId, tp.Position, PageId, null, EditLanguage);
            }

            //Always bind, which is required for postback. Any additional bindings to handle changes in rule data
            // made during event handlers will be performed by rule editor control.
            _ruleEditor.Bind();
        }

        /// <summary>
        /// Bind service
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _okButton_Click(object sender, EventArgs e)
        {
            _ruleEditor.SaveChanges();

            ClientScript.RegisterClientScriptBlock(GetType(), "closeDialog", "okClick(" + PageId + ");", true);
        }
    }
}
