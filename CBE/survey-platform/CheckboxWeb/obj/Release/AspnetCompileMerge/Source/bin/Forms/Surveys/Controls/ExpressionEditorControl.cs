using System;
using System.Collections.Generic;
using Checkbox.Forms.Logic;
using Checkbox.Forms.Logic.Configuration;
using System.Web;

namespace CheckboxWeb.Forms.Surveys.Controls
{
    /// <summary>
    /// Expression editor control
    /// </summary>
    public abstract class ExpressionEditorControl : Checkbox.Web.Common.UserControlBase
    {
        private List<RuleData> _ruleData;
        private ExpressionEditorParams _params;
        /// <summary>
        /// Max length for item text to display
        /// </summary>
        protected const int MAX_TEXT_LENGTH = 64;

        /// <summary>
        /// Editor params
        /// </summary>
        protected ExpressionEditorParams Params
        {
            get
            {
                return _params ??
                       (_params = HttpContext.Current.Session["_EditorParams"] as ExpressionEditorParams);
            }
            set
            {
                _params = value;
                HttpContext.Current.Session["_EditorParams"] = value;
            }
        }

        /// <summary>
        /// ID of parent rule
        /// </summary>
        protected int RuleID { get; set; }


        /// <summary>
        /// Target page for page branching
        /// </summary>
        public int? TargetPageId { get; set; }

        /// <summary>
        /// Get whether basic editing mode is allowed
        /// </summary>
        public bool BasicModeAllowed
        {
            get { return GetBasicModeAllowed(); }
        }

        /// <summary>
        /// Get rule data service
        /// </summary>
        private RuleDataService RuleDataService
        {
            get
            {
                if (Params == null)
                {
                    throw new Exception("Params collection is null for editor.");
                }

                if (Params.RuleDataService == null)
                {
                    throw new Exception("RuleDataService of expression editor params collection is null.");
                }

                return Params.RuleDataService;
            }
        }

        ///<summary>
        /// Get the rule data to edit.  Due to the way it works, we want to get a new instance
        /// every time the property is accessed.
        /// </summary>
        protected List<RuleData> RuleData
        {
            get { return _ruleData ?? (_ruleData = GetRuleData(Params)); }
            set { _ruleData = value; }
        }

        /// <summary>
        /// Get the root expression for page or item conditions
        /// </summary>
        protected RuleData ConditionRuleData
        {
            get
            {
                if (RuleData != null && RuleData.Count == 1)
                {
                    return RuleData[0];
                }

                return null;
            }
        }

        /// <summary>
        /// Walk the rules to determine if basic mode is allowed.  Basic mode is allowed if the rule
        /// contains only OR-level conditions with one child each or only has one  or zero OR-level 
        /// conditions.
        /// </summary>
        /// <returns></returns>
        private bool GetBasicModeAllowed()
        {
            //If there are no rules, or the top-level rule is a single rule, allow basic mode
            if (ConditionRuleData == null || !(ConditionRuleData.Expression is CompositeExpressionData))
            {
                return true;
            }

            //Get the children, which are the "OR-level" conditions
            List<ExpressionData> childExpressions = ((CompositeExpressionData)ConditionRuleData.Expression).Children;

            //If there are none or only one OR-Level condition, basic mode is allowed
            if (childExpressions.Count < 2)
            {
                return true;
            }

            //If there are more than one OR-Level condition, allow basic mode if each only has 0 or 1 children
            foreach (ExpressionData childExpression in childExpressions)
            {
                if (childExpression is CompositeExpressionData && ((CompositeExpressionData)childExpression).Children.Count > 1)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Get the top-level rule data to edit
        /// </summary>
        /// <returns></returns>
        public static List<RuleData> GetRuleData(ExpressionEditorParams editorParams)
        {
            if (editorParams.RuleType == RuleType.ItemCondition) 
            {
                return new List<RuleData>
                           {
                               editorParams.RuleDataService.GetConditionForItem(editorParams.DependentItemId.Value)
                               ?? editorParams.RuleDataService.CreateEmptyConditionRuleForItem(editorParams.DependentItemId.Value)
                           };
            }
             
            if (editorParams.RuleType == RuleType.PageCondition)
            {
                return new List<RuleData>
                           {
                               editorParams.RuleDataService.GetConditionForPage(editorParams.DependentPageId.Value)
                               ?? editorParams.RuleDataService.CreateEmptyConditionRuleForPage(editorParams.DependentPageId.Value)
                           };
            }

            return new List<RuleData>(editorParams.RuleDataService.GetBranchRulesForPage(editorParams.DependentPageId.Value));
        }


        #region Rule Creation

        /// <summary>
        /// Create an expression for non-item operands, such as user attributes or response properties
        /// </summary>
        /// <param name="existingExpressionId"></param>
        /// <param name="operandType"></param>
        /// <param name="compositeExpressionID"></param>
        /// <param name="propertyName"></param>
        /// <param name="theOperator"></param>
        /// <param name="valueToCompare"></param>
        /// <returns></returns>
        protected int CreateExpression(int? existingExpressionId, ExpressionOperandType operandType, int compositeExpressionID, string propertyName, LogicalOperator theOperator, string valueToCompare)
        {
            if (operandType == ExpressionOperandType.Profile)
            {
                if (!existingExpressionId.HasValue)
                {
                    return RuleDataService.CreateProfileExpression(compositeExpressionID, propertyName, theOperator, valueToCompare);
                }

                RuleDataService.ReplaceExpressionWithProfileExpression(existingExpressionId.Value, propertyName, theOperator, valueToCompare);
                return existingExpressionId.Value;
            }

            if (!existingExpressionId.HasValue)
            {
                return RuleDataService.CreateResponseExpression(compositeExpressionID, propertyName, theOperator, valueToCompare);
            }

            RuleDataService.ReplaceExpressionWithResponseExpression(existingExpressionId.Value, propertyName, theOperator, valueToCompare);
            return existingExpressionId.Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="existingExpressionId"></param>
        /// <param name="compositeExpressionID"></param>
        /// <param name="sourceItemId"></param>
        /// <param name="theOperator"></param>
        /// <param name="category"></param>
        /// <param name="valueToCompare"></param>
        /// <param name="columnNumber"></param>
        /// <returns></returns>
        protected int CreateCategorizedItemExpression(int? existingExpressionId, int compositeExpressionID, int sourceItemId, LogicalOperator theOperator, int columnNumber, String category, string valueToCompare)
        {
            if (!existingExpressionId.HasValue)
            {
                return RuleDataService.CreateCategorizedMatrixExpression(compositeExpressionID, sourceItemId,
                                                                         columnNumber, category, theOperator,
                                                                         valueToCompare);
            }
            RuleDataService.ReplaceExpressionWithCategorizedExpression(existingExpressionId.Value, sourceItemId, columnNumber, category, theOperator, valueToCompare);
            return existingExpressionId.Value;
        }

        /// <summary>
        /// Create an expression that is the child of the specified composite expression id.  
        /// </summary>
        /// <param name="existingExpressionId"></param>
        /// <param name="compositeExpressionID"></param>
        /// <param name="sourceItemId"></param>
        /// <param name="theOperator"></param>
        /// <param name="valueToCompare"></param>
        /// <returns></returns>
        protected int CreateExpression(int? existingExpressionId, int compositeExpressionID, int sourceItemId, LogicalOperator theOperator, string valueToCompare)
        {
            if (!existingExpressionId.HasValue)
            {
                return RuleDataService.CreateItemExpression(compositeExpressionID, sourceItemId, theOperator, valueToCompare);
            }

            RuleDataService.ReplaceExpressionWithItemExpression(existingExpressionId.Value, sourceItemId, null, theOperator, null, valueToCompare);
            return existingExpressionId.Value;
        }

        /// <summary>
        /// Create an expression that is the child of the specified composite expression id.  If the
        /// id is null, an empty rule will be created.
        /// </summary>
        /// <param name="existingExpressionId"></param>
        /// <param name="compositeExpressionID"></param>
        /// <param name="sourceItemId"></param>
        /// <param name="theOperator"></param>
        /// <param name="optionID"></param>
        /// <param name="answerText"></param>
        /// <returns></returns>
        protected int CreateExpression(int? existingExpressionId, int compositeExpressionID, int sourceItemId, LogicalOperator theOperator, int? optionID, string answerText)
        {
            if (!existingExpressionId.HasValue)
            {
                return RuleDataService.CreateItemExpression(compositeExpressionID, sourceItemId, theOperator, optionID, answerText);
            }

            RuleDataService.ReplaceExpressionWithItemExpression(existingExpressionId.Value, sourceItemId, null, theOperator, optionID, answerText);
            return existingExpressionId.Value;
        }

        /// <summary>
        /// Create an expression that is the child of the specified composite expression id.  If the
        /// id is null, an empty rule will be created.
        /// </summary>
        /// <param name="existingExpressionId"></param>
        /// <param name="compositeExpressionID"></param>
        /// <param name="parentItemId"></param>
        /// <param name="theOperator"></param>
        /// <param name="optionID"></param>
        /// <param name="answerText"></param>
        /// <param name="sourceItemId"></param>
        /// <returns></returns>
        protected int CreateExpression(int? existingExpressionId, int compositeExpressionID, int sourceItemId, int? parentItemId, LogicalOperator theOperator, int? optionID, string answerText)
        {
            if (!existingExpressionId.HasValue)
            {
                if (parentItemId.HasValue)
                {
                    if (optionID.HasValue)
                    {
                        return RuleDataService.CreateMatrixItemExpression(compositeExpressionID, parentItemId.Value, sourceItemId, theOperator, optionID);
                    }

                    return RuleDataService.CreateMatrixItemExpression(compositeExpressionID, parentItemId.Value, sourceItemId, theOperator, answerText);
                }

                return RuleDataService.CreateItemExpression(compositeExpressionID, sourceItemId, theOperator, optionID, answerText);
            }

            RuleDataService.ReplaceExpressionWithItemExpression(existingExpressionId.Value, sourceItemId, parentItemId, theOperator, optionID, answerText);
            return existingExpressionId.Value;
        }


        ///<summary>
        /// Create a new rule and return the id of it's root expression
        /// </summary>
        /// <returns></returns>
        private int CreateNewRule()
        {
            RuleData ruleData;

            if (Params.RuleType == RuleType.PageBranchCondition)
            {
                ruleData = RuleDataService.CreateBranchRule(Params.DependentPageId.Value, TargetPageId.Value);
            }
            else
            {
                ruleData = Params.DependentItemId.HasValue
                    ? RuleDataService.CreateEmptyConditionRuleForItem(Params.DependentItemId.Value)
                    : RuleDataService.CreateEmptyConditionRuleForPage(Params.DependentPageId.Value);
            }

            if (ruleData != null
                && ruleData.Expression != null
                && ruleData.Expression.ExpressionId.HasValue)
            {
                return ruleData.Expression.ExpressionId.Value;
            }

            throw new Exception("Unable to create new rule.");
        }

        /// <summary>
        /// Get the root expression id, creating it if necessary
        /// </summary>
        /// <returns></returns>
        protected int GetRootExpressionID()
        {
            //Get the root expression id, creating it if necessary
            if (Params.RuleType==RuleType.PageBranchCondition)
            {
                //Find the rule
                foreach (RuleData ruleData in RuleData)
                {
                    if (ruleData.Expression != null
                        && ruleData.Expression.ExpressionId.HasValue
                        && Params.RuleDataService.GetTargetPageId(ruleData.RuleId) == TargetPageId)
                    {
                        return ruleData.Expression.ExpressionId.Value;
                    }
                }
            }
            else if (ConditionRuleData != null
                     && ConditionRuleData.Expression != null
                     && ConditionRuleData.Expression.ExpressionId.HasValue)
            {
                return ConditionRuleData.Expression.ExpressionId.Value;
            }

            return CreateNewRule();
        }


        /// <summary>
        /// Get the id of the parent of the expression to create, creating it if
        /// necessary.
        /// </summary>
        /// <returns></returns>
        protected int GetParentExpressionId(int? childExpressionId)
        {
            if (childExpressionId.HasValue)
            {
                int? parentExpressionID = RuleDataService.GetExpressionParentID(childExpressionId.Value);

                if (parentExpressionID.HasValue)
                {
                    return parentExpressionID.Value;
                }
            }

            //Create a new or expression with a child "AND" expression
            return RuleDataService.CreateANDCompositeExpression(GetRootExpressionID());
        }

        #endregion

    }
}
