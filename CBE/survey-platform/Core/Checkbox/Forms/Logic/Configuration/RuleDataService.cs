using System;
using System.Linq;
using System.Data;
using System.Collections.Generic;
using Checkbox.Forms.Data;
using Checkbox.Forms.Items.Configuration;

namespace Checkbox.Forms.Logic.Configuration
{
    /// <summary>
    /// Provides methods to access <see cref="RuleData"/> and associated classes within the context of a given <see cref="ResponseTemplate"/>.
    /// Also provides a data-oriented service layer around the <see cref="ResponseTemplate"/> rules configuration.
    /// <remarks>
    /// Rule configuration operations and queries are exposed using a RuleDataService wrapper around the <see cref="ResponseTemplate"/>.
    /// </remarks>
    /// </summary>
    [Serializable]
    public class RuleDataService
    {
        private int TemplateId { get; set; }
        private RulesObjectSet RulesObjectSet { get; set; }
        private ResponseTemplate _responseTemplate;

        /// <summary>
        /// Get response template being edited
        /// </summary>
        public ResponseTemplate ResponseTemplate
        {
            get
            {
                if (_responseTemplate == null)
                {
                    _responseTemplate = ResponseTemplateManager.GetResponseTemplate(TemplateId);

                    if (_responseTemplate == null)
                    {
                        throw new Exception(string.Format("Unable to load survey with id {0}.", TemplateId));
                    }
                }

                return _responseTemplate;
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public RuleDataService()
        {
            RulesObjectSet = new RulesObjectSet();
        }

        /// <summary>
        /// Initialize rules data for a template.
        /// </summary>
        /// <param name="templateId"></param>
        public void Initialize(int templateId)
        {
            TemplateId = templateId;

            RulesObjectSet.Load(templateId);
        }

        /// <summary>
        /// Save rule data.
        /// </summary>
        public void SaveRuleData()
        {
            RulesObjectSet.Save();    
        
            //cleanup cache
            SurveyMetaDataProxy.RemoveRulesEngineFromCache(TemplateId);
            SurveyMetaDataProxy.AddRuleDataServiceToCache(TemplateId, this);
        }

        /// <summary>
        /// Determine if RulesDataSet has unsaved changes.
        /// </summary>
        /// <returns></returns>
        public bool AreThereUnsavedChanges
        {
            get { return true; }
        }        

        /// <summary>
        /// Save rule data in the context of hte provided transaction.
        /// </summary>
        /// <param name="transaction"></param>
        public void SaveRuleData(IDbTransaction transaction)
        {
            RulesObjectSet.Save(transaction);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ruleId"></param>
        /// <returns></returns>
        public int GetTargetPageId(int ruleId)
        {
            RuleData rule = GetRuleData(ruleId);

            if (rule == null || !rule.Actions.Any() || !(rule.Actions.First() is BranchPageActionData))
            {
                return -1;
            }

            return (rule.Actions.First() as BranchPageActionData).GoToPageID.Value;
        }

        /// <summary>
        /// Gets a boolean indicating whether a given <see cref="ItemData"/> has associated conditions
        /// </summary>
        /// <param name="itemId">the <see cref="ItemData"/></param>
        /// <returns>true, if has conditions; otherwise, false</returns>
        public bool ItemHasCondition(int itemId)
        {
            var rule = RulesObjectSet.GetItemRule(itemId);

            if (rule == null || rule.Expression == null)
                return false;

            var expr = RulesObjectSet.GetChildExpressions(rule.Expression.Id);

            return expr.Count() > 0;
        }

        /// <summary>
        /// Gets a boolean indicating whether a given <see cref="TemplatePage"/> has associated conditions
        /// </summary>
        /// <param name="pageId">the <see cref="TemplatePage"/></param>
        /// <returns>true, if has conditions; otherwise, false</returns>
        public bool PageHasCondition(int pageId)
        {
            return RulesObjectSet.GetPageRules(pageId, RuleEventTrigger.Load.ToString()).Any();
        }

        /// <summary>
        /// Gets a boolean indicating whether a given <see cref="TemplatePage"/> has associated branches
        /// </summary>
        /// <param name="pageId">the <see cref="TemplatePage"/></param>
        /// <returns>true, if has conditions; otherwise, false</returns>
        public bool PageHasBranches(int pageId)
        {
            return RulesObjectSet.GetPageRules(pageId, RuleEventTrigger.UnLoad.ToString()).Any();
        }

        /// <summary>
        /// Gets a <see cref="RuleData"/> for an <see cref="ItemData"/>, if any; otherwise, creates a new one and returns it
        /// <remarks>
        /// The Rule for an Item will always have a IncludeExcludedActionData associated with it
        /// </remarks>
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public RuleData GetConditionForItem(int itemId)
        {
            RuleData ruleData = null;

            var rule = RulesObjectSet.GetItemRule(itemId);

            if (rule != null)
            {
                ruleData = GetRuleData(rule.Id);
            }

            return ruleData;
        }

        /// <summary>
        /// Get a condition for a given page
        /// </summary>
        /// <param name="pageId"></param>
        /// <returns></returns>
        public RuleData GetConditionForPage(int pageId)
        {
            RuleData ruleData = null;

            var ruleDataRows = RulesObjectSet.GetPageRules(pageId, RuleEventTrigger.Load.ToString());

            if (ruleDataRows.Any())
            {
                ruleData = GetRuleData(ruleDataRows.First().Id);
            }

            return ruleData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageId"></param>
        /// <returns></returns>
        public RuleData[] GetBranchRulesForPage(int pageId)
        {
            var pageRules = new List<RuleData>();

            var rules = RulesObjectSet.GetPageRules(pageId, RuleEventTrigger.UnLoad.ToString());

            if (rules.Any())
            {
                foreach (var rule in rules)
                {
                    RuleData ruleData = GetRuleData(rule.Id);

                    if (ruleData != null)
                    {
                        pageRules.Add(ruleData);
                    }
                }
            }

            return pageRules.ToArray();
        }

        /// <summary>
        /// Get data for a specific rule.
        /// </summary>
        /// <param name="ruleId"></param>
        /// <returns></returns>
        public RuleData GetRuleData(int ruleId)
        {
            RuleData rule = CreateRuleData();

            rule.Load(ruleId, RulesObjectSet);

            return rule;
        }

        /// <summary>
        /// Create an empty rule data object with an empty top-level expression
        /// </summary>
        /// <returns></returns>
        private static RuleData CreateRuleData()
        {
            return new RuleData(
                new CompositeExpressionData(),
                new List<ActionData>());
        }

        /// <summary>
        /// Deletes all Rules associated with a given <see cref="TemplatePage"/>
        /// </summary>
        /// <param name="pageId"></param>
        public void DeletePageRules(int pageId)
        {
            RulesObjectSet.DeletePageRules(pageId);
        }

        /// <summary>
        /// When the publisher <see cref="ItemData" /> is moved within the survey, any dependent subscribers must verify that the
        /// expressions are still valid
        /// </summary>
        /// <param name="itemId">The item identifier.</param>
        /// <param name="newPagePosition">The new page position.</param>
        /// <param name="page">The page.</param>
        public void NotifySubscribingExpressionsOfPublisherItemMoved(int itemId, int newPagePosition, TemplatePage page)
        {
            ItemData item = ResponseTemplate.GetItem(itemId);
            if (item is ICompositeItemData)
            {
                foreach (int childId in ((ICompositeItemData)item).GetChildItemDataIDs())
                {
                    NotifySubscribingExpressionsOfPublisherItemMoved(childId, newPagePosition, page);
                }
            }

            var subscribers = RulesObjectSet.FindItemOperandSubscribers(itemId);

            foreach (var subscriber in subscribers)
            {
                int? expressionId = subscriber.Id;
                int? rootExpressionId = subscriber.Root.Id;

                RuleEventTrigger eventTrigger;
                int dependentPagePosition = DetermineContainingPagePosition(rootExpressionId.Value,
                                                                                out eventTrigger);

                //remove rules only if item does not belong to moved page
                if (!page.ContainsItem(itemId))
                {
                    // test for branch rule, which allows the publisher to be on the same page
                    if (eventTrigger == RuleEventTrigger.UnLoad)
                    {
                        if (dependentPagePosition < newPagePosition)
                            RulesObjectSet.DeleteExpression(expressionId.Value);
                    }
                    else
                    {
                        if (dependentPagePosition <= newPagePosition)
                            RulesObjectSet.DeleteExpression(expressionId.Value);
                    }
                }
            }
        }

        /// <summary>
        /// Deletes invalid Expressions that are dependent (subscribers) of ItemOperands
        /// <remarks>
        /// This operation compares the position of the subscriber's owner to the that of the publisher.
        /// </remarks>
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="newPagePosition"></param>
        internal void HandleItemMoved(int itemId, int newPagePosition)
        {
            HandleForItem(itemId, newPagePosition, false,  null);
        }



        /// <summary>
        /// Set new page target for branch rule.
        /// </summary>
        /// <param name="ruleId"></param>
        /// <param name="targetId"></param>
        public void SetPageBranchTargetPageId(int ruleId, int targetId)
        {
            RuleData ruleData = GetRuleData(ruleId);

            if (ruleData == null)
            {
                return;
            }

            foreach (ActionData actionData in ruleData.Actions)
            {
                var action = actionData as BranchPageActionData;

                if (action != null)
                {
                    action.SetTargetPageId(RulesObjectSet, targetId);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageId"></param>
        /// <param name="newPagePosition"></param>
        internal void HandlePageMoved(int pageId, int newPagePosition)
        {
            TemplatePage page = ResponseTemplate.GetPage(pageId);

            foreach (int itemId in page.ListItemIds())
            {
                HandleForItem(itemId, newPagePosition, true, page);
            }

            var rules = RulesObjectSet.GetPageRules(pageId).ToArray();
            foreach (var rule in rules)
            {
                int ruleID = rule.Id;
                int rootExpressionID = RulesObjectSet.GetRule(ruleID).Expression.Id;
                bool isBranchRule = (rule.EventTrigger == "UnLoad") ? true : false;

                if (isBranchRule)
                {
                    // validate the target page is still valid
                    // since we flattened the ActionTable, we can select by the RuleID                            
                    int actionID = rule.Action.Id;
                    int? targetPageID = RulesObjectSet.GetBranchAction(actionID).GoToPageId;

                    if (targetPageID.HasValue && ResponseTemplate.GetPage(targetPageID.Value).Position <= newPagePosition)
                    {
                        RulesObjectSet.DeleteRule(ruleID);
                        continue; // keep evaluating the rules
                    }
                }

                var subscriberExpressions = RulesObjectSet.FindItemOperandSubscribersByRootExpression(rootExpressionID);

                foreach (var expression in subscriberExpressions)
                {
                    int publicherItemID = expression.LeftOperand.ItemId.Value;

                    int publisherPosition = ResponseTemplate.GetItemPositionWithinPage(publicherItemID) ??
                                            int.MaxValue;
                    if (isBranchRule)
                    {
                        // treat branch rule
                        if (newPagePosition - 1 < publisherPosition)
                        {
                            DeleteExpression(expression.Id);
                        }
                    }
                    else
                    {
                        if (newPagePosition - 1 <= publisherPosition)
                        {
                            DeleteExpression(expression.Id);
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Given a expressionRootID, determines what the related Rule observer is (<see cref="ItemData"/>, <see cref="TemplatePage"/>) and what the position in the Form of 
        /// that observer is.
        /// </summary>
        /// <param name="expressionRootID"></param>
        /// <param name="eventTrigger"></param>
        /// <returns></returns>
        private int DetermineContainingPagePosition(int expressionRootID, out RuleEventTrigger eventTrigger)
        {
            eventTrigger = RuleEventTrigger.Load;
            var rule = RulesObjectSet.GetRuleByExpression(expressionRootID);
            
            if (rule != null)
            {
                Enum.TryParse(rule.EventTrigger, out eventTrigger);
                var page = RulesObjectSet.GetPageByRule(rule.Id);
                if (page != null)
                    return ResponseTemplate.GetPage(page.Id).Position;

                var item = RulesObjectSet.GetItemByRule(rule.Id);
                if (item != null)
                    return ResponseTemplate.GetPagePositionForItem(item.Id) ?? -1;
            }

            return -1;
        }

        /// <summary>
        /// Recursively calls the HandleSubscriberItemMoved method
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="newPagePosition"></param>
        /// <param name="isPageMoved"> </param>
        private void HandleForItem(int itemId, int newPagePosition, bool isPageMoved, TemplatePage page)
        {
            ItemData item = ResponseTemplate.GetItem(itemId);
            if (item is ICompositeItemData)
            {
                foreach (int childId in ((ICompositeItemData)item).GetChildItemDataIDs())
                {
                    HandleForItem(childId, newPagePosition, isPageMoved, page);
                }
            }

            // notification only required if Item is in an operand
            if (item.ID.HasValue)
            {
                var rule = RulesObjectSet.GetItemRule(itemId);

                if (rule != null)
                {
                    int rootExpressionID = rule.Expression.Id;

                    var subscriberExpressions = RulesObjectSet.FindItemOperandSubscribersByRootExpression(rootExpressionID);
                    foreach (var expression in subscriberExpressions)
                    {
                        var publisherItemID = expression.LeftOperand.ItemId;

                        //check if items belong to different pages 
                        if (publisherItemID.HasValue && !IsSourceAndTargetBelongToPage(publisherItemID.Value, itemId, page))
                        {
                            int? publisherItemposition = ResponseTemplate.GetPagePositionForItem(publisherItemID.Value);

                            if (publisherItemposition.HasValue && newPagePosition < publisherItemposition)
                                DeleteExpression(expression.Id);
                        }
                    }                    
                }
            }
        }

        private bool IsSourceAndTargetBelongToPage(int sourceId , int targetId, TemplatePage page)
        {
            return page != null && (page.ListItemIds().Contains(sourceId) && page.ListItemIds().Contains(targetId));
        }

        /// <summary>
        /// Delete all subscriber expressions with source items not in the valid ID list
        /// </summary>
        /// <param name="validItemIds"></param>
        /// <returns></returns>
        public bool DeleteInvalidSubscriberExpressions(List<int> validItemIds)
        {
            bool isDirty = false;

            var itemOperands = RulesObjectSet.ListOperands();

            foreach (var itemOperand in itemOperands)
            {
                var itemId = itemOperand.ItemId;

                if (itemId.HasValue && !validItemIds.Contains(itemId.Value))
                {
                    isDirty = true;
                    DeleteSubscriberExpressions(itemId.Value);
                }
            }

            //2: Delete operands & expressions that are invalid due to item changes
            foreach (int itemId in validItemIds)
            {
                isDirty = isDirty || DeleteInvalidSubscriberExpressions(itemId);
            }

            return isDirty;
        }

        /// <summary>
        /// Retrieves the subscribers to an <see cref="ItemData"/> and examines whether the expression is still valid for the ItemData's
        /// present state
        /// </summary>
        /// <param name="itemId">the publisher Item, on which subscriber expressions depend</param>
        /// <returns>true, if invalid dependents were found and deleted.  This signals the calling method to 
        /// the changed dataset, which it can then save</returns>
        public bool DeleteInvalidSubscriberExpressions(int itemId)
        {
            bool isDirty = false;

            //TODO: Item Children & Specific Item Types
            /*
            if (item is ICompositeItemData)
            {
                foreach (ItemData child in ((ICompositeItemData)item).GetChildItemDatas())
                {
                    bool dirty = DeleteInvalidSubscriberExpressions(child);
                    if (!isDirty)
                        isDirty = dirty;
                }
            }

            if (!ItemHasSubscriberExpressions(item))
                return isDirty;

            // SelectItemDatas may be invalid because their option list has changed
            // Therefore, compare the option ID for validity
            if (item is SelectItemData && item.ID.HasValue)
            {
                DataRow[] itemOperands = ItemOperandTable.Select("ItemID=" + item.ID);

                foreach (DataRow dr in itemOperands)
                {
                    DataRow[] containingExpressionRows = ExpressionTable.Select("LeftOperand=" + (int)dr["OperandID"]);

                    if (containingExpressionRows.Length > 0)
                    {
                        DataRow containingExpressionDR = containingExpressionRows[0];

                        int rightOperand = (int)containingExpressionDR["RightOperand"];

                        DataRow[] valueOperandRows = ValueOperandTable.Select("OperandID=" + rightOperand);

                        if (valueOperandRows.Length > 0)
                        {
                            DataRow valueOperandDR = valueOperandRows[0];

                            if (valueOperandDR["OptionID"] != DBNull.Value)
                            {
                                int optionIDToCompare = (int)valueOperandDR["OptionID"];

                                ReadOnlyCollection<ListOptionData> options = ((SelectItemData)item).Options;
                                for (int x = 0; x < options.Count; x++)
                                {
                                    if (options[x].OptionID == optionIDToCompare)
                                        break;

                                    if ((x + 1) == options.Count) // option no longer exists in this Item, delete the expression
                                    {
                                        DeleteExpression((int)containingExpressionDR["ExpressionID"]);
                                        isDirty = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }*/
            return isDirty;
        }


        /// <summary>
        /// Deletes any rule dependencies for an item data
        /// </summary>
        /// <param name="itemId"></param>
        public void DeleteSubscriberExpressions(int itemId)
        {
            var subscribers = RulesObjectSet.FindItemOperandSubscribers(itemId);

            foreach (var subscriber in subscribers)
            {
                if (subscriber != null)
                {
                    RulesObjectSet.DeleteExpression(subscriber.Id);
                }
            }
        }

        /// <summary>
        /// Delete any expressions where this page is the action target
        /// </summary>
        /// <param name="pageId"></param>
        internal void DeletePageTargetExpressions(int pageId)
        {
            var pageTargetRules = RulesObjectSet.FindPageActionExpressions(pageId);

            foreach (var rule in pageTargetRules)
            {
                if (rule != null)
                {
                    RulesObjectSet.DeleteRule(rule.Id);
                }
            }            
        }


        #region Service Code

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootExpressionId"></param>
        /// <returns></returns>
        public Int32 CreateANDCompositeExpression(int rootExpressionId)
        {
            var expression = RulesObjectSet.InsertVoidExpression();
            expression.Root = RulesObjectSet.GetExpression(rootExpressionId);
            expression.Parent = expression.Root;
            expression.Depth = 1;
            expression.Operator = (int)LogicalOperator.OperatorNotSpecified;
            expression.ChildRelation = LogicalConnector.AND.ToString();

            return expression.Id;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public int CreateOperand<T>() where T : OperandData
        {
            return RulesObjectSet.InsertVoidOperand(typeof (T)).Id;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public RuleData CreateEmptyConditionRuleForItem(int itemId)
        {
            var action = ActionEntity.Create(typeof(IncludeExcludeActionData));
            var rule = CreateRootLevelRule();
            rule.Action = action;

            RulesObjectSet.AddRuleToItem(itemId, rule);

            return GetRuleData(rule.Id);
        }

        ///<summary>
        ///</summary>
        ///<param name="pageId"></param>
        ///<returns></returns>
        public RuleData CreateEmptyConditionRuleForPage(int pageId)
        {
            var action = ActionEntity.Create(typeof(IncludeExcludeActionData));
            var rule = CreateRootLevelRule();
            rule.EventTrigger = RuleEventTrigger.Load.ToString();
            rule.Action = action;
            
            RulesObjectSet.AddRuleToPage(pageId, rule);

            return GetRuleData(rule.Id);
        }

        /// <summary>
        /// Create a branch rule
        /// </summary>
        /// <param name="dependentPageId"></param>
        /// <param name="targetPageId"></param>
        /// <returns></returns>
        public RuleData CreateBranchRule(int dependentPageId, int? targetPageId)
        {
            var action = ActionEntity.Create(typeof (BranchPageActionData));
            action.GoToPageId = targetPageId;
            var rule = CreateRootLevelRule();
            rule.EventTrigger = RuleEventTrigger.UnLoad.ToString();
            rule.Action = action;

            RulesObjectSet.AddRuleToPage(dependentPageId, rule);

            return GetRuleData(rule.Id);
        }

        /// <summary>
        /// </summary>
        /// <param name="pageId"></param>
        /// <returns></returns>
        public RuleData CreateEmptyBranchRule(int pageId)
        {
            return CreateBranchRule(pageId, default(int?));
        }

        /// <summary>
        /// Create an item expression
        /// </summary>
        /// <param name="parentExpressionId"></param>
        /// <param name="itemId"></param>
        /// <param name="condition"></param>
        /// <param name="valueToCompare"></param>
        /// <returns></returns>
        public int CreateItemExpression(int parentExpressionId, int itemId, LogicalOperator condition, string valueToCompare)
        {
            return CreateItemExpression(parentExpressionId, itemId, condition, null, valueToCompare);
        }

        /// <summary>
        /// Replace the existing expression with an item expression
        /// </summary>
        /// <param name="expressionId"></param>
        /// <param name="itemId"></param>
        /// <param name="parentItemId"></param>
        /// <param name="condition"></param>
        /// <param name="valueToCompare"></param>
        /// <param name="answerText"></param>
        public void ReplaceExpressionWithItemExpression(int expressionId, int itemId, int? parentItemId, LogicalOperator condition, int? valueToCompare, string answerText)
        {
            //Get the parent of the existing expression
            int? parentExpressionId = GetExpressionParentID(expressionId);

            if (parentExpressionId.HasValue)
            {
                //Create a new expression
                if (parentItemId.HasValue)
                {
                    if (valueToCompare.HasValue)
                    {
                        CreateMatrixItemExpression(parentExpressionId.Value, itemId, parentItemId.Value, condition, valueToCompare);
                    }
                    else
                    {
                        CreateMatrixItemExpression(parentExpressionId.Value, parentItemId.Value, itemId, condition, answerText);
                    }
                }
                else
                {
                    if (valueToCompare.HasValue)
                    {
                        CreateItemExpression(parentExpressionId.Value, itemId, condition, valueToCompare, answerText);
                    }
                    else
                    {
                        CreateItemExpression(parentExpressionId.Value, itemId, condition, answerText);
                    }
                }

                RulesObjectSet.DeleteExpression(expressionId);
            }
        }

        /// <summary>
        /// Replace the existing expression with a profile expression
        /// </summary>
        /// <param name="expressionId"></param>
        /// <param name="profileKey"></param>
        /// <param name="condition"></param>
        /// <param name="valueToCompare"></param>
        public void ReplaceExpressionWithProfileExpression(int expressionId, string profileKey, LogicalOperator condition, string valueToCompare)
        {
            //Get the parent of the existing expression
            int? parentExpressionId = GetExpressionParentID(expressionId);

            if (parentExpressionId.HasValue)
            {
                //Create the new expression
                CreateProfileExpression(parentExpressionId.Value, profileKey, condition, valueToCompare);

                //Delete the old expression
                RulesObjectSet.DeleteExpression(expressionId);
            }
        }


        /// <summary>
        /// Replace the existing expression with a categorized expression
        /// </summary>
        /// <param name="expressionId"></param>
        /// <param name="itemId"></param>
        /// <param name="columnNumber"></param>
        /// <param name="category"></param>
        /// <param name="op"></param>
        /// <param name="answerText"></param>
        public void ReplaceExpressionWithCategorizedExpression(int expressionId, int itemId, int columnNumber,
                                                     string category, LogicalOperator op, String answerText)
        {
            //Get the parent of the existing expression
            int? parentExpressionId = GetExpressionParentID(expressionId);

            if (parentExpressionId.HasValue)
            {
                //Create the new expression
                CreateCategorizedMatrixExpression(parentExpressionId.Value, itemId, columnNumber, category, op, answerText);

                //Delete the old expression
                RulesObjectSet.DeleteExpression(expressionId);
            }
        }

        /// <summary>
        /// Replace an expression with a response expression
        /// </summary>
        /// <param name="expressionId"></param>
        /// <param name="responseKey"></param>
        /// <param name="condition"></param>
        /// <param name="valueToCompare"></param>
        public void ReplaceExpressionWithResponseExpression(int expressionId, string responseKey, LogicalOperator condition, string valueToCompare)
        {
            //Find expression parent
            int? parentExpressionId = GetExpressionParentID(expressionId);

            if (parentExpressionId.HasValue)
            {
                //Create new expression
                CreateResponseExpression(parentExpressionId.Value, responseKey, condition, valueToCompare);

                //Delete the old
                RulesObjectSet.DeleteExpression(expressionId);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expressionId"></param>
        /// <returns></returns>
        public ExpressionEntity GetExpression(int expressionId)
        {
            return RulesObjectSet.GetExpression(expressionId);
        }

        /// <summary>
        /// Get the ID of the parent of specified expression
        /// </summary>
        /// <param name="expressionId"></param>
        /// <returns></returns>
        public int? GetExpressionParentID(int expressionId)
        {
            var expression = RulesObjectSet.GetExpression(expressionId);

            if (expression == null)
            {
                return null;
            }

            return expression.Parent.Id;
        }

        ///<summary>
        ///</summary>
        ///<param name="parentExpressionId"></param>
        ///<param name="itemId"></param>
        ///<param name="condition"></param>
        ///<param name="valueToCompare"></param>
        ///<param name="answerText"></param>
        ///<returns></returns>
        public int CreateItemExpression(int parentExpressionId, int itemId, LogicalOperator condition, int? valueToCompare, string answerText)
        {
            return CreateItemExpression(parentExpressionId, null, itemId, condition, valueToCompare, null, null, answerText, typeof(ItemOperandData));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentExpressionId"></param>
        /// <param name="parentItemId"></param>
        /// <param name="itemId"></param>
        /// <param name="condition"></param>
        /// <param name="valueToCompare"></param>
        /// <param name="category"></param>
        /// <param name="columnNumber"></param>
        /// <param name="answerText"></param>
        /// <param name="leftOperandType"></param>
        /// <returns></returns>
        private int CreateItemExpression(int parentExpressionId, int? parentItemId, int itemId, LogicalOperator condition,
            int? valueToCompare, string category, int? columnNumber, string answerText, Type leftOperandType)
        {
            //Get expression
            var parentExpression = RulesObjectSet.GetExpression(parentExpressionId);

            if (parentExpression == null || parentExpression.Root == null)
            {
                throw new Exception(string.Format("Root of expression [{0}] had no value.", parentExpressionId));
            }

            var rootExpression = parentExpression.Root;

            //Create left operand
            var leftOperand = RulesObjectSet.InsertVoidOperand(leftOperandType);
            leftOperand.ItemId = itemId;
            leftOperand.ParentItemId = parentItemId;
            leftOperand.ColumnNumber = columnNumber;
            leftOperand.Category = category;

            var rightOperand = RulesObjectSet.InsertVoidOperand(valueToCompare.HasValue
                                                        ? typeof (OptionOperandData)
                                                        : typeof (StringOperandData));
            rightOperand.ItemId = itemId;
            rightOperand.AnswerValue = answerText;
            rightOperand.OptionId = valueToCompare;

            //create expression
            var expression = RulesObjectSet.InsertVoidExpression();
            expression.LeftOperand = leftOperand;
            expression.RightOperand = rightOperand;
            expression.Root = rootExpression;
            expression.Parent = RulesObjectSet.GetExpression(parentExpressionId);
            expression.Depth = 2;
            expression.Operator = (int)condition;

            return expression.Id;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentExpressionId"></param>
        /// <param name="itemId"></param>
        /// <param name="columnNumber"></param>
        /// <param name="category"></param>
        /// <param name="op"></param>
        /// <param name="answerText"></param>
        /// <returns></returns>
        public int CreateCategorizedMatrixExpression(int parentExpressionId, int itemId, int columnNumber,
                                                     string category, LogicalOperator op, String answerText)
        {
            return CreateItemExpression(parentExpressionId, null, itemId, op, null, category, columnNumber, answerText, typeof(CategorizedMatrixItemOperandData));
        }

        ///<summary>
        ///</summary>
        ///<param name="parentExpressionId"></param>
        ///<param name="parentItemId"></param>
        ///<param name="itemId"></param>
        ///<param name="condition"></param>
        ///<param name="valueToCompare"></param>
        ///<returns></returns>
        public int CreateMatrixItemExpression(int parentExpressionId, int parentItemId, int itemId, LogicalOperator condition, string valueToCompare)
        {
            return CreateItemExpression(parentExpressionId, parentItemId, itemId, condition, null, null, null, valueToCompare, typeof(MatrixItemOperandData));
        }

        ///<summary>
        ///</summary>
        ///<param name="parentExpressionId"></param>
        ///<param name="parentItemId"></param>
        ///<param name="itemId"></param>
        ///<param name="condition"></param>
        ///<param name="valueToCompare"></param>
        ///<returns></returns>
        public int CreateMatrixItemExpression(int parentExpressionId, int parentItemId, int itemId, LogicalOperator condition, int? valueToCompare)
        {
            return CreateItemExpression(parentExpressionId, parentItemId, itemId, condition, valueToCompare, null, null, null, typeof(MatrixItemOperandData));
        }

        /// <summary>
        /// Create an expression for comparing responses
        /// </summary>
        /// <param name="parentExpressionId"></param>
        /// <param name="key"></param>
        /// <param name="condition"></param>
        /// <param name="valueToCompare"></param>
        /// <returns></returns>
        public int CreateResponseExpression(int parentExpressionId, string key, LogicalOperator condition, string valueToCompare)
        {
            //Get expression
            var rootExpression = RulesObjectSet.GetExpression(parentExpressionId).Root;

            if (rootExpression == null)
            {
                throw new Exception(string.Format("Root of expression [{0}] had no value.", parentExpressionId));
            }

            //Create operand
            var leftOperand = RulesObjectSet.InsertVoidOperand(typeof(ResponseOperandData));
            leftOperand.ResponseKey = key;

            var rightOperand = RulesObjectSet.InsertVoidOperand(typeof(StringOperandData));
            rightOperand.AnswerValue = valueToCompare;

            //create expression
            var expression = RulesObjectSet.InsertVoidExpression();
            expression.LeftOperand = leftOperand;
            expression.RightOperand = rightOperand;
            expression.Root = rootExpression;
            expression.Parent = RulesObjectSet.GetExpression(parentExpressionId);
            expression.Depth = 2;
            expression.Operator = (int)condition;

            return expression.Id;
        }

        /// <summary>
        /// Create a user profile expression
        /// </summary>
        /// <param name="parentExpressionId"></param>
        /// <param name="key"></param>
        /// <param name="condition"></param>
        /// <param name="valueToCompare"></param>
        /// <returns></returns>
        public int CreateProfileExpression(int parentExpressionId, string key, LogicalOperator condition, string valueToCompare)
        {
            //Get expression
            var rootExpression = RulesObjectSet.GetExpression(parentExpressionId).Root;

            if (rootExpression == null)
            {
                throw new Exception(string.Format("Root of expression [{0}] had no value.", parentExpressionId));
            }

            //Create operand
            var leftOperand = RulesObjectSet.InsertVoidOperand(typeof(ProfileOperandData));
            leftOperand.ProfileKey = key;

            var rightOperand = RulesObjectSet.InsertVoidOperand(typeof(StringOperandData));
            rightOperand.AnswerValue = valueToCompare;

            //create expression
            var expression = RulesObjectSet.InsertVoidExpression();
            expression.LeftOperand = leftOperand;
            expression.RightOperand = rightOperand;
            expression.Root = rootExpression;
            expression.Parent = RulesObjectSet.GetExpression(parentExpressionId);
            expression.Depth = 2;
            expression.Operator = (int)condition;

            return expression.Id;
        }

        private RuleEntity CreateRootLevelRule()
        {
            var expression = RulesObjectSet.InsertVoidExpression();
            expression.Operator = (int)LogicalOperator.OperatorNotSpecified;
            expression.Depth = 0;
            expression.Lineage = "/";
            expression.ChildRelation = LogicalConnector.OR.ToString();
            expression.Root = null;
            expression.Parent = null;

            var rule = EntityBase.Create<RuleEntity>();
            rule.Expression = expression;

            return rule;
        }

        /// <summary>
        /// Returns real expression ID by old ID
        /// </summary>
        /// <param name="newExpressionId"></param>
        /// <returns></returns>
        public int? GetRealExpressionId(int newExpressionId)
        {
            if (RulesObjectSet != null && RulesObjectSet.ExpressionIDMap.ContainsKey(newExpressionId))
            {
                return (int?)RulesObjectSet.ExpressionIDMap[newExpressionId];
            }
            return null;
        }

        #endregion

        #region Update/Delete

        /// <summary>
        /// Update the RulesDataSet with the expression's data.
        /// </summary>
        /// <param name="dataToUpdate"></param>
        public void UpdateExpressionData(ExpressionData dataToUpdate)
        {
            if (dataToUpdate == null)
            {
                return;
            }

            dataToUpdate.UpdateRuleData(RulesObjectSet);
        }

        /// <summary>
        /// Delete the expression with the specified id
        /// </summary>
        /// <param name="expressionId"></param>
        public void DeleteExpression(int expressionId)
        {
            RulesObjectSet.DeleteExpression(expressionId);
        }

        public int[] DeletedRules
        {
            get
            {
                return RulesObjectSet.DeletedRules.ToArray();
            }
        }
        #endregion

        #region Import

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rules"></param>
        /// <param name="pageIds"></param>
        public void UpdateBranchTargets(RuleData[] rules, Dictionary<int, int> pageIds)
        {
            foreach (var ruleData in rules)
            {
                if (ruleData.Actions == null)
                {
                    continue;
                }

                var branchActions = ruleData.Actions.Where(action => action is BranchPageActionData);

                foreach (var branchAction in branchActions)
                {
                    var goToPageId = ((BranchPageActionData)branchAction).GoToPageID;

                    if (goToPageId.HasValue && pageIds.ContainsKey(goToPageId.Value))
                    {
                        ((BranchPageActionData)branchAction).SetTargetPageId(RulesObjectSet, pageIds[goToPageId.Value]);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rules"></param>
        /// <param name="optionPositionMap"></param>
        /// <param name="itemIdMap"></param>
        public void UpdateItemAndOptionIds(RuleData[] rules, Dictionary<int, int> itemIdMap, Dictionary<int, int> optionPositionMap, Dictionary<int, int> prototypesMap)
        {
            foreach (var ruleData in rules)
            {
                if (ruleData.Expression != null)
                {
                    //if the expression can not be updated, that contain mapping on deleted items or options
                    if (ruleData.Expression.UpdateItemAndOptionIds(itemIdMap, optionPositionMap, prototypesMap))
                        UpdateExpressionData(ruleData.Expression);
                    else ruleData.Expression = null;
                }
            }
        }

        #endregion


        /// <summary>
        /// Get Expresion Data
        /// </summary>
        /// <param name="expressionId"></param>
        /// <returns></returns>
        public ExpressionData GetExpressionData(int expressionId)
        {
            ExpressionData expressionData = new ExpressionData();
            expressionData.Load(expressionId, RulesObjectSet);
            return expressionData;
        }
    }
}
