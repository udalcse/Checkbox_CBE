using System;
using System.Web.UI;
using Checkbox.Forms;
using Checkbox.Forms.Logic;
using Checkbox.Forms.Logic.Configuration;
using Checkbox.Web.Forms.UI.Editing;

namespace CheckboxWeb.Forms.Surveys.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public partial class ItemConditionEditor : Checkbox.Web.Common.UserControlBase, IRuleEditor
    {
        /// <summary>
        /// Item id to edit conditions for
        /// </summary>
        protected int ItemId { get; set; }

        /// <summary>
        /// Position of page containing item
        /// </summary>
        public int PagePosition { get; set; }

        /// <summary>
        /// ID of response template
        /// </summary>
        public int ResponseTemplateId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isPagePostback"></param>
        /// <param name="itemId"></param>
        /// <param name="pagePosition"></param>
        /// <param name="responseTemplateId"></param>
        /// <param name="languageCode"></param>
        public void Initialize(bool isPagePostback, int itemId, int pagePosition, int responseTemplateId, string languageCode)
        {
            if (languageCode == null) throw new ArgumentNullException("languageCode");
            ItemId = itemId;
            PagePosition = pagePosition;
            ResponseTemplateId = responseTemplateId;

            _ruleEditor.ConfigureBrachPanelVisibility(RuleType.ItemCondition);

            if (!isPagePostback)
            {
                RuleDataService rds = ResponseTemplate.CreateRuleDataService(ResponseTemplateId);

                if (rds == null)
                {
                    throw new Exception("Unable to load rule service for survey with id " + ResponseTemplateId);
                }
                _ruleEditor.Initialize(rds, RuleType.ItemCondition, ResponseTemplateId, PagePosition, null, ItemId, languageCode);
            }
            
            //Always bind, which is required for postback. Any additional bindings to handle changes in rule data
            // made during event handlers will be performed by rule editor control.
            _ruleEditor.Bind();
        }

        /// <summary>
        /// Update rule data with selected values
        /// </summary>
        public void SaveChanges()
        {
            _ruleEditor.SaveChanges();
        }
    }
}