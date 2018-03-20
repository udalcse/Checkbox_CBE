using System;
using Checkbox.Forms;
using Checkbox.Forms.Logic;
using Checkbox.Forms.Logic.Configuration;
using Checkbox.Web;
using Checkbox.Web.Page;
using Checkbox.Forms.Items.Configuration;

namespace CheckboxWeb.Forms.Surveys
{
    /// <summary>
    /// Edit item conditions
    /// </summary>
    public partial class ItemConditions : SecuredPage
    {
        /// <summary>
        /// ID of item to edit condition for
        /// </summary>
        [QueryParameter("i")]
        public int ItemId { get; set; }

        /// <summary>
        /// ID of page containing item
        /// </summary>
        [QueryParameter("p")]
        public int PagePosition { get; set; }

        /// <summary>
        /// Id of response template
        /// </summary>
        [QueryParameter("s")]
        public int ResponseTemplateId { get; set; }

        /// <summary>
        /// Initialize rule editor control
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            IncludeJsLocalization = true;

            Master.CancelClick += new EventHandler(Master_CancelClick);
            Master.CancelTextId = "/common/close";
            Master.OkVisible = false;

            if (!Page.IsPostBack)
            {
                RuleDataService rds = ResponseTemplate.CreateRuleDataService(ResponseTemplateId);

                if (rds == null)
                {
                    throw new Exception("Unable to load rule service for survey with id " + ResponseTemplateId);
                }

                //TODO: Language code
                _ruleEditor.Initialize(rds, RuleType.ItemCondition, ResponseTemplateId, PagePosition, null, ItemId, "en-US");
            }

            //Always bind, which is required for postback. Any additional bindings to handle changes in rule data
            // made during event handlers will be performed by rule editor control.
            _ruleEditor.Bind();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Master_CancelClick(object sender, EventArgs e)
        {
            RuleDataService rds = ResponseTemplate.CreateRuleDataService(ResponseTemplateId);

            ClientScript.RegisterClientScriptBlock(GetType(), "closeDialog", string.Format("okClick({0});", _ruleEditor.HasConditions.ToString().ToLower()), true);
        }
    }
}
