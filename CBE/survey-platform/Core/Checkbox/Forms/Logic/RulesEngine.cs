using System;
using System.Collections.Generic;
using System.Linq;
using Checkbox.Common;
using Checkbox.Forms.Items.Configuration;
using Prezza.Framework.ExceptionHandling;
using Checkbox.Forms.Items;

namespace Checkbox.Forms.Logic
{
    /// <summary>
    /// Engine for running/evalulating survey rules.
    /// </summary>
    [Serializable]
    public class RulesEngine
    {
        private Dictionary<int, List<Rule>> _pageRules;
        private Dictionary<int, Rule> _itemRules;
        private List<string> _ignoreConditionTypes = new List<string>(); 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ruleData"></param>
        /// <param name="pageIds"></param>
        /// <param name="itemIds"></param>
        public void Initialize(RulesObjectSet ruleData, List<int> pageIds, List<int> itemIds)
        {
            //Populate page & item rules
            BuildRules(ruleData, pageIds, itemIds);
        }

        /// <summary>
        /// Build page rules
        /// </summary>
        private void BuildRules(RulesObjectSet ruleData, IEnumerable<int> pageIds, IEnumerable<int> itemids)
        {
            _pageRules = new Dictionary<int, List<Rule>>();
            _itemRules = new Dictionary<int, Rule>();

            foreach (int pageId in pageIds)
            {
                _pageRules[pageId] = GetRulesForPage(pageId, ruleData);
            }

            foreach (int itemId in itemids)
            {
                Rule itemRule = GetRuleForItem(itemId, ruleData);

                if (itemRule != null)
                {
                    _itemRules[itemId] = itemRule;
                }
            }
        }

        /// <summary>
        /// Adds the ignore rule types.
        /// </summary>
        /// <param name="types">The types.</param>
        public void AddIgnoreRuleTypes(List<string> types )
        {
            if (types.Any())
                _ignoreConditionTypes.AddRange(types);
        }

        /// <summary>
        /// Removes the type of the ignore rule.
        /// </summary>
        /// <param name="type">The type.</param>
        public void RemoveIgnoreRuleType(string type)
        {
            if (!string.IsNullOrEmpty(type))
                _ignoreConditionTypes.RemoveAll(item => item.Equals(type));
        }

        /// <summary>
        /// Clears the ignore type rules.
        /// </summary>
        public void ClearIgnoreTypeRules()
        {
            _ignoreConditionTypes.Clear();
        }


        public Dictionary<int, Rule> FilterItemRules(Dictionary<int, Rule> itemRules)
        {
            Dictionary<int, Rule> result = null;

            if (_ignoreConditionTypes.Any())
            {
                result = new Dictionary<int, Rule>();

                foreach (var key in itemRules.Keys)
                {
                    var expressions = CollectExpressionTreeNodes(itemRules[key].Expression);

                    if (expressions.Any() && !expressions.Any(HasIgnoreTypes))
                        result.Add(key, itemRules[key]);
                }

            }

            return result;
        }


        /// <summary>
        /// Determines whether [has ignore types] [the specified expression].
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>
        ///   <c>true</c> if [has ignore types] [the specified expression]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasIgnoreTypes(Expression expression)
        {
            foreach (var ignoreType in _ignoreConditionTypes)
            {
                var leftOperant = expression.LeftOperand;
                if (leftOperant != null)
                {
                    var expressionType = leftOperant.GetType().ToString();
                    if (expressionType.Equals(ignoreType))
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        public void RunRules(int itemId, Response response)
        {
            var rulesToApply = _ignoreConditionTypes.Any() ? FilterItemRules(_itemRules) : _itemRules;

            if (rulesToApply.ContainsKey(itemId))
            {
                rulesToApply[itemId].Run(response);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageId"></param>
        /// <param name="response"></param>
        /// <param name="trigger"></param>
        public void RunRules(int pageId, Response response, RuleEventTrigger trigger)
        {
            if (!_pageRules.ContainsKey(pageId))
            {
                return;
            }

            //Find rules matching trigger
            var rules = _pageRules[pageId].Where(r => r != null && r.Trigger == trigger).ToList();

            // all rules are evaluated for a ResponsePage
            foreach (Rule t in rules)
            {
                //Run rule
                bool result = t.Run(response);

                //Big OO-Break, but if this is a branch action, avoid running additional rules if a branch
                // rule evaluated successfully.
                if (result
                    && trigger == RuleEventTrigger.UnLoad)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rp"></param>
        /// <param name="rt"></param>
        /// <returns></returns>
        public List<int> ListSourcePageIdsBoundOnSpecifiedPageConditions(ResponsePage rp, ResponseTemplate rt)
        {
            var allRulesOnPage = new List<Rule>();

            //collect all rules for page
            if (_pageRules.ContainsKey(rp.PageId))
                allRulesOnPage.AddRange(_pageRules[rp.PageId]);

            //collect all rules for items on the page
            foreach (var item in rp.Items)
            {
                if (_itemRules.ContainsKey(item.ID))
                    allRulesOnPage.Add(_itemRules[item.ID]);

                var tabular = item as TabularItem;
                if (tabular != null)
                {
                    foreach (var child in tabular.Items)
                    {
                        if (_itemRules.ContainsKey(child.ID))
                            allRulesOnPage.Add(_itemRules[child.ID]);
                    }
                }
            }

            var result = new List<int>();
            //list source items and find their parent page
            foreach (var rule in allRulesOnPage)
            {
                var expressions = CollectExpressionTreeNodes(rule.Expression);
                foreach (var expression in expressions)
                {
                    var operand = expression.LeftOperand as ItemDependentOperand;
                    if (operand != null)
                    {
                        foreach (var id in operand.SourceItemIds)
                        {
                            var pageContainingTheItem = rt.GetPageIdForItem(id);
                            if (pageContainingTheItem != rp.PageId && pageContainingTheItem.HasValue && !result.Contains(pageContainingTheItem.Value))
                                result.Add(pageContainingTheItem.Value);
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<int> ListTargetPageIdsBoundOnSpecifiedPageBranching(ResponsePage rp, ResponseTemplate rt)
        {
            var rules = _pageRules[rp.PageId].SelectMany(r => r.Actions).OfType<GoToPageAction>();
            return rules.Select(a => a.TargetPageId).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        private List<Expression> CollectExpressionTreeNodes(Expression expression)
        {
            var result = new List<Expression> { expression };
            var composite = expression as CompositeExpression;
            if (composite != null)
            {
                foreach (var child in composite.Children)
                {
                    result.AddRange(CollectExpressionTreeNodes(child));
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageId"></param>
        /// <param name="ruleData"></param>
        /// <returns></returns>
        private static List<Rule> GetRulesForPage(int pageId, RulesObjectSet ruleData)
        {
            var pageRules = ruleData.GetPageRules(pageId);

            var rules = new List<Rule>();

            foreach (var pageRule in pageRules)
            {
                var rootExpression = pageRule.Expression;

                string logicalConnectorString = rootExpression.ChildRelation;

                if (Utilities.IsNullOrEmpty(logicalConnectorString))
                {
                    continue;
                }

                var lc = (LogicalConnector)Enum.Parse(typeof(LogicalConnector), logicalConnectorString);
                var root = new CompositeExpression(new List<Expression>(), lc) { Identity = rootExpression.Id };

                BuildExpressionTree(root, ruleData);

                //Check for children.  If there is none, then this rule is not valid, since all root rules in Checkbox
                // should have at least one child
                if (root.Children.Count == 0)
                {
                    continue;
                }

                var actions = new Action[1];

                RuleEventTrigger trigger = RuleEventTrigger.Load;

                string eventTriggerString = pageRule.EventTrigger;

                if (Utilities.IsNullOrEmpty(eventTriggerString))
                {
                    throw new Exception("Unable to get trigger value.");
                }

                if (eventTriggerString.Equals("Load", StringComparison.InvariantCultureIgnoreCase))
                {
                    actions[0] = new IncludeExcludeAction(Action.ActionReceiverType.Page, pageId);
                }
                else
                {
                    var goToPageId = pageRule.Action.GoToPageId;

                    if (!goToPageId.HasValue)
                    {
                        continue;
                    }
                    actions[0] = new GoToPageAction(pageId, goToPageId.Value);
                    trigger = RuleEventTrigger.UnLoad;
                }

                var rule = new Rule(root, trigger, actions);

                rules.Add(rule);
            }
            return rules;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="ruleData"></param>
        /// <returns></returns>
        private static Rule GetRuleForItem(int itemId, RulesObjectSet ruleData)
        {
            var ruleEntity = ruleData.GetItemRule(itemId);

            if (ruleEntity == null)
            {
                return null;
            }

            var rootExpressionId = ruleEntity.Expression.Id;
            var logicalConnectorString = ruleEntity.Expression.ChildRelation;

            if (Utilities.IsNullOrEmpty(logicalConnectorString))
            {
                throw new Exception("Unable to get connector for expression: " + rootExpressionId);
            }

            var lc = (LogicalConnector)Enum.Parse(typeof(LogicalConnector), logicalConnectorString);
            var root = new CompositeExpression(new List<Expression>(), lc) { Identity = rootExpressionId };

            BuildExpressionTree(root, ruleData);

            //Ensure rule has children.  If no children, then rule is not valid and can be ignored
            if (root.Children.Count == 0)
            {
                return null;
            }
            var actions = new Action[1];
            actions[0] = new IncludeExcludeAction(Action.ActionReceiverType.Item, itemId);

            var rule = new Rule(root, RuleEventTrigger.Load, actions);

            return rule;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="composite"></param>
        /// <param name="ruleData"></param>
        private static void BuildExpressionTree(CompositeExpression composite, RulesObjectSet ruleData)
        {
            try
            {
                var children = ruleData.GetChildExpressions(composite.Identity.Value);

                foreach (var childData in children)
                {
                    string childRelation = childData.ChildRelation;
                    var expressionId = childData.Id;

                    if (Utilities.IsNotNullOrEmpty(childRelation))
                    {
                        // only Composites have ChildRelation
                        var lc = (LogicalConnector)Enum.Parse(typeof(LogicalConnector), childRelation);
                        var childComposite = new CompositeExpression(new List<Expression>(), lc) { Identity = expressionId };
                        BuildExpressionTree(childComposite, ruleData);
                        composite.Add(childComposite);
                    }
                    else
                    {
                        Expression child = CreateExpression(childData);
                        child.Identity = expressionId;
                        composite.Add(child);
                    }
                }
            }
            catch (Exception ex)
            {
                bool rethrow = ExceptionPolicy.HandleException(ex, "BusinessPublic");

                if (rethrow)
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        private static Expression CreateExpression(ExpressionEntity expression)
        {
            var operatorInt = expression.Operator;
            var leftOperand = expression.LeftOperand;
            var rightOperand = expression.RightOperand;

            if (operatorInt == -1)
            {
                throw new Exception("Operator did not have a value in expression data.");
            }

            if (leftOperand == null)
            {
                throw new Exception("Expression has no left operand id.");
            }

            if (rightOperand == null)
            {
                throw new Exception("Expression has no right operand id.");
            }

            var op = (LogicalOperator)Enum.Parse(typeof(LogicalOperator), operatorInt.ToString());

            string leftType = leftOperand.TypeName;
            string rightType = rightOperand.TypeName;

            if (string.IsNullOrWhiteSpace(leftType))
            {
                throw new Exception("Unable to find type of operand with id: " + leftOperand.Id);
            }

            if (string.IsNullOrWhiteSpace(rightType))
            {
                throw new Exception("Unable to find type of operand with id: " + rightOperand.Id);
            }

            Operand left = null;
            Operand right = null;

            // we only support these left types
            if (leftType == typeof(Configuration.ItemOperandData).FullName ||
                leftType == typeof(Configuration.MatrixItemOperandData).FullName)
            {
                var itemId = leftOperand.ItemId;

                if (itemId == null)
                {
                    throw new Exception("Unable to get ItemId for item operand with id: " + leftOperand.Id);
                }

                //get column prototype data
                var prototypeId = ItemConfigurationManager.GetPrototypeItemID(itemId.Value);
                ItemData itemData = ItemConfigurationManager.GetConfigurationData(
                    prototypeId.HasValue ? prototypeId.Value : itemId.Value, true);

                if(itemData is SliderItemData && (itemData as SliderItemData).ValueType == SliderValueType.NumberRange)
                {
                    left = new AnswerableOperand(itemId.Value);
                }
                else if (itemData is SelectItemData)
                {
                    left = new SelectItemOperand(itemId.Value);
                }
                else if (itemData.ItemIsIAnswerable)
                {
                    left = new AnswerableOperand(itemId.Value);
                }

            }
            else if (leftType == typeof(Configuration.CategorizedMatrixItemOperandData).FullName)
            {
                var itemId = leftOperand.ItemId;
                var columnNumber = leftOperand.ColumnNumber;
                var category = leftOperand.Category;

                if (!itemId.HasValue)
                    throw new Exception("Unable to get ItemId for item operand with id: " + leftOperand.Id);
                if (!columnNumber.HasValue)
                    throw new Exception("Unable to get ColumnNumber for item operand with id: " + leftOperand.Id);

                left = new CategorizedItemOperand(itemId.Value, columnNumber.Value, category);
            }
            else if (leftType == typeof(Configuration.ProfileOperandData).FullName)
            {
                string profileKey = leftOperand.ProfileKey;
                left = new ProfileOperand(profileKey);
            }
            else if (leftType == typeof(Configuration.ResponseOperandData).FullName)
            {
                string responseKey = leftOperand.ResponseKey;
                left = new ResponseOperand(responseKey);
            }

            if (rightType == typeof(Configuration.OptionOperandData).FullName && rightOperand.OptionId.HasValue)
            {
                int optId = rightOperand.OptionId.Value;
                right = new OptionOperand(optId);
            }
            else if (rightType == typeof(Configuration.StringOperandData).FullName)
            {
                string answerValue = rightOperand.AnswerValue;
                right = new StringOperand(answerValue);
            }

            return new Expression(left, right, op);
        }

        /// <summary>
        /// Checks whereas a item set contains references to this set
        /// </summary>
        /// <param name="Items"></param>
        internal bool HasSamePageConditions(List<Item> Items)
        {
            foreach (var i in Items)
            {
                var probablyDependentItems = (from iii in Items where iii != i select iii).ToList();
                if (ItemsDependOn(probablyDependentItems, i))
                {
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// Checks that list of items depends on the other item
        /// </summary>
        /// <param name="itemIDs"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        internal bool ItemsDependOn(List<Item> dependentItems, Item i)
        {
            var l = new List<Item>();
            l.Add(i);

            foreach (var id in dependentItems)
            {
                if (id is MatrixItem)
                {
                    //check all matrix children to make working Row Conditions for SPC
                    var mi = (MatrixItem)id;
                    foreach (var childItem in mi.Items)
                    {
                        if (_itemRules.ContainsKey(childItem.ID))
                        {
                            Rule r = _itemRules[childItem.ID];
                            if (r != null)
                            {
                                if (expressionDependsOnItems(r.Expression, l))
                                    return true;
                            }
                        }
                    }
                }

                //check the item itself
                if (_itemRules.ContainsKey(id.ID))
                {
                    Rule r = _itemRules[id.ID];
                    if (r != null)
                    {
                        if (expressionDependsOnItems(r.Expression, l))
                            return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Check that expression depends on one of given items
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="ItemIDs"></param>
        /// <returns></returns>
        private bool expressionDependsOnItems(Expression expression, List<Item> Items)
        {
            int? itemID = null;
            
            var sio = expression.LeftOperand as SelectItemOperand;
            if (sio != null)
                itemID = sio.ItemID;

            var aio = expression.LeftOperand as AnswerableOperand;
            if (aio != null)
                itemID = aio.ItemID;

            if (itemID.HasValue && Items.Exists(i => i.ID == itemID.Value))
                return true;

            if (itemID.HasValue)
            {
                foreach (var i in Items)
                {
                    if (i is MatrixItem)
                    {
                        if ((i as MatrixItem).Items.ToList().Exists(ii => ii.ID == itemID.Value))
                            return true;
                    }
                }
            }

            if (expression is CompositeExpression)
            {
                var cex = expression as CompositeExpression;

                foreach (var c in cex.Children)
                {
                    if (expressionDependsOnItems(c, Items))
                        return true;
                }
            }

            return false;
        }


    }
}
