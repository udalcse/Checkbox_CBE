using System;
using System.Collections.Generic;
using Checkbox.Common;
using Checkbox.Forms.Logic;
using Checkbox.Forms.Logic.Configuration;
using Checkbox.Web;

namespace CheckboxWeb.Forms.Surveys.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public partial class BasicExpressionEditor : ExpressionEditorControl
    {
        /// <summary>
        /// Fired when expressions are updated.
        /// </summary>
        public event EventHandler ExpressionsUpdated;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            RegisterClientScriptInclude(
                "basicExpressionEditor.js",
                ResolveUrl("~/Resources/basicExpressionEditor.js"));

            base.OnLoad(e);
        }

        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="editorParams"></param>
        public void Initialize(ExpressionEditorParams editorParams)
        {
            //Initialize control
            Params = editorParams;
            PopulateAnyAllText(editorParams);
            SetBasicModeConnectorSelection();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="editorParams"></param>
        private void PopulateAnyAllText(ExpressionEditorParams editorParams)
        {
            //Set any/all text
            string baseText = WebTextManager.GetText("/pageText/conditionEditor.aspx/show");

            if (Utilities.IsNotNullOrEmpty(baseText)
                && baseText.Contains("{0}")
                && baseText.Contains("{1}"))
            {
                string typeText = string.Empty;

                if (editorParams.RuleType == RuleType.ItemCondition)
                {
                    typeText = WebTextManager.GetText("/pageText/conditionEditor.aspx/surveyItem");
                }

                if (editorParams.RuleType == RuleType.PageBranchCondition)
                {
                }

                if (editorParams.RuleType == RuleType.PageCondition)
                {
                    typeText = WebTextManager.GetText("/pageText/conditionEditor.aspx/surveyPage");
                }

                _anyView.Text = string.Format(
                    baseText,
                    typeText,
                    WebTextManager.GetText("/pageText/conditionEditor.aspx/any"));

                _allView.Text = string.Format(
                    baseText,
                    typeText,
                    WebTextManager.GetText("/pageText/conditionEditor.aspx/all"));
            }
        }

        /// <summary>
        /// Set selected radio for basic mode connector
        /// </summary>
        private void SetBasicModeConnectorSelection()
        {
            if (GetBasicModeConnector() == LogicalConnector.OR)
            {
                _allView.Checked = false;
                _anyView.Checked = true;
            }
            else
            {
                _anyView.Checked = false;
                _allView.Checked = true;
            }
        }



        /// <summary>
        /// Get the connector for basic mode, which is determined by the structure of the conditions.
        /// If there are more than one OR-level conditions, the connector is OR...if there are zero or 1 
        /// OR-level conditions, the connector is AND.
        /// </summary>
        /// <returns></returns>
        protected LogicalConnector GetBasicModeConnector()
        {
            //If there are no rules, or the top-level rule is a single rule, connector is AND
            if (ConditionRuleData == null || !(ConditionRuleData.Expression is CompositeExpressionData))
            {
                return LogicalConnector.AND;
            }

            //Get the children, which are the "OR-level" conditions
            List<ExpressionData> childExpressions = ((CompositeExpressionData)ConditionRuleData.Expression).Children;

            //If there are multiple top-level conditions, we have OR mode otherwise it's AND mode
            if (childExpressions.Count > 1)
            {
                return LogicalConnector.OR;
            }

            return LogicalConnector.AND;
        }
    }
}