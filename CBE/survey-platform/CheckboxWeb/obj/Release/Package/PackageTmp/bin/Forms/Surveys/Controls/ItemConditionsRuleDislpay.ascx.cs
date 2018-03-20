using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Forms;
using Checkbox.Forms.Logic.Configuration;
using Checkbox.Web.Forms.UI.Rendering;
using Prezza.Framework.ExceptionHandling;

namespace CheckboxWeb.Forms.Surveys.Controls
{
    public partial class ItemConditionsRuleDislpay : Checkbox.Web.Common.UserControlBase, IItemRuleDisplay
    {
		static List<ExpressionData> emptyList;

		static ItemConditionsRuleDislpay()
		{
			emptyList = new List<ExpressionData>(1);
		}

        private RuleData RuleData { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected string LanguageCode { get; set; }

        /// <summary>
        /// Initialize control
        /// </summary>
        public void InitializeAndBind(int itemId, int responseTemplateId, string languageCode)
        {
            try
            {
                LanguageCode = languageCode;

                RuleDataService rds = ResponseTemplate.CreateRuleDataService(responseTemplateId);

                if (rds == null)
                {
                    throw new Exception("Unable to load rule service for survey with id " + responseTemplateId);
                }

                RuleData = rds.GetConditionForItem(itemId);
                
                _errorPnl.Visible = false;

                _orLevelRepeater.DataSource = OrExpresssionDatas;
                _orLevelRepeater.DataBind();

                _orLevelRepeater.Visible = _orLevelRepeater.Items.Count > 0;
                _pageWillBeDisplayedLbl.Visible = _orLevelRepeater.Visible;
                _noConditionsLbl.Visible = !_orLevelRepeater.Visible;
            }
            catch(Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "UIProcess");

                _orLevelRepeater.Visible = false;
                _noConditionsLbl.Visible = false;
                _pageWillBeDisplayedLbl.Visible = false;

                _errorPnl.Visible = true;
            }
        }

        /// <summary>
        /// Bind events and data
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            //Bind data
            _orLevelRepeater.ItemDataBound += RuleDisplay_ItemDataBound;
        }

        /// <summary>
        /// Handle item data bound to bind inner repeater
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RuleDisplay_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            RepeaterItem item = e.Item;

            if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
            {
                var orExpression = e.Item.DataItem as CompositeExpressionData;
                var innerRepeater = item.FindControl("_andLevelRepeater") as Repeater;

                if (orExpression != null && innerRepeater != null)
                {
                    innerRepeater.ID = orExpression.ExpressionId.ToString();
                    innerRepeater.DataSource = orExpression.Children;
                    innerRepeater.DataBind();
                }
            }
        }

        /// <summary>
        /// Get a list of or expressions.
        /// </summary>
        protected List<ExpressionData> OrExpresssionDatas
        {
            get
            {
                if (RuleData == null)
					return emptyList;

				CompositeExpressionData ced = RuleData.Expression as CompositeExpressionData;

                if(ced != null)
                {
                    return ced.Children;
                }

				return emptyList;
            }
        }
    }
}