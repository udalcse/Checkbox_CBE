using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Checkbox.Forms.Logic.Configuration;
using Prezza.Framework.ExceptionHandling;

namespace CheckboxWeb.Forms.Surveys.Controls
{
    public partial class PageConditionsRuleDisplay : Checkbox.Web.Common.UserControlBase
    {
        private RuleData RuleData { get; set; }

        /// <summary>
        /// Initialize control
        /// </summary>
        /// <param name="ruleData"></param>
        public void InitializeAndBind(RuleData ruleData)
        {
            try
            {
                _errorPnl.Visible = false;

                RuleData = ruleData;

                _orLevelRepeater.DataSource = OrExpresssionDatas;
                _orLevelRepeater.DataBind();

                _orLevelRepeater.Visible = _orLevelRepeater.Items.Count > 0;
                _pageWillBeDisplayedLbl.Visible = _orLevelRepeater.Visible;
                _noConditionsLbl.Visible = !_orLevelRepeater.Visible;
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "UIProcess");

                _noConditionsLbl.Visible = false;
                _orLevelRepeater.Visible = false;
                _pageWillBeDisplayedLbl.Visible = false;
                _ruleLbl.Visible = false;
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
                CompositeExpressionData orExpression = e.Item.DataItem as CompositeExpressionData;
                Repeater innerRepeater = item.FindControl("_andLevelRepeater") as Repeater;

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
                if (RuleData != null
                    && RuleData.Expression is CompositeExpressionData)
                {
                    return ((CompositeExpressionData)RuleData.Expression).Children;
                }
                
                return new List<ExpressionData>();
            }
        }
    }
}