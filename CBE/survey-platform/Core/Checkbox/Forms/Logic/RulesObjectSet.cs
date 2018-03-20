using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Prezza.Framework.Data;
using Prezza.Framework.ExceptionHandling;

namespace Checkbox.Forms.Logic
{
    /// <summary>
    /// Data container for template rules
    /// </summary>
    [Serializable]
    public class RulesObjectSet
    {
        private readonly IList<ItemEntity> _items = new List<ItemEntity>();
        private readonly IList<PageEntity> _pages = new List<PageEntity>();

        private readonly IList<ExpressionEntity> _voidExpressions = new List<ExpressionEntity>();
        private readonly IList<OperandEntity> _voidOperands = new List<OperandEntity>();
        private readonly IList<RuleEntity> _voidRules = new List<RuleEntity>();
        private readonly IList<ActionEntity> _voidActions = new List<ActionEntity>();
        private readonly IList<int> _deletedRules = new List<int>();

        public IList<int> DeletedRules
        {
            get
            {
                return _deletedRules;
            }
        }
            
        /// <summary>
        /// Load rule data
        /// </summary>
        /// <param name="responseTemplateId"></param>
        public void Load(int responseTemplateId)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_RT_GetResponseTemplate_Rules");
            command.AddInParameter("ResponseTemplateId", DbType.Int32, responseTemplateId);

            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    _items.Clear();
                    _pages.Clear();

                    _voidExpressions.Clear();
                    _voidOperands.Clear();
                    _voidRules.Clear();
                    _voidActions.Clear();
                    
                    //read operands
                    while (reader.Read())
                    {
                        int operandId = DbUtility.GetValueFromDataReader(reader, "OperandID", -1);
                        string typeName = DbUtility.GetValueFromDataReader(reader, "TypeName", default(string));
                        string assembly = DbUtility.GetValueFromDataReader(reader, "TypeAssembly", default(string));
                        _voidOperands.Add(new OperandEntity(operandId, typeName, assembly));
                    }

                    //read expressions
                    reader.NextResult();
                    while (reader.Read())
                    {
                        int expressionId = DbUtility.GetValueFromDataReader(reader, "ExpressionID", -1);
                        int exprOperator = DbUtility.GetValueFromDataReader(reader, "Operator", 0);
                        int? left = DbUtility.GetValueFromDataReader(reader, "LeftOperand", default(int?));
                        int? right = DbUtility.GetValueFromDataReader(reader, "RightOperand", default(int?));
                        int? parent = DbUtility.GetValueFromDataReader(reader, "Parent", default(int?));
                        int depth = DbUtility.GetValueFromDataReader(reader, "Depth", -1);
                        string lineage = DbUtility.GetValueFromDataReader(reader, "Lineage", default(string));
                        int? root = DbUtility.GetValueFromDataReader(reader, "Root", default(int?));
                        string child = DbUtility.GetValueFromDataReader(reader, "ChildRelation", default(string));

                        _voidExpressions.Add(new ExpressionEntity(expressionId)
                                             {
                                                 Id = expressionId,
                                                 Operator = exprOperator,
                                                 LeftOperand = _voidOperands.FirstOrDefault(o => o.Id == left),
                                                 RightOperand = _voidOperands.FirstOrDefault(o => o.Id == right),
                                                 ParentId = parent,
                                                 Depth = depth,
                                                 Lineage = lineage,
                                                 RootId = root,
                                                 ChildRelation = child
                                             });
                    }
                    //set relations
                    foreach (var expression in _voidExpressions)
                    {
                        if (expression.ParentId.HasValue)
                            expression.Parent = _voidExpressions.FirstOrDefault(e => e.Id == expression.ParentId);
                        if (expression.RootId.HasValue)
                            expression.Root = _voidExpressions.FirstOrDefault(e => e.Id == expression.RootId);

                        expression.ResetState();
                    }

                    //read rules-expressions relations
                    reader.NextResult();
                    while (reader.Read())
                    {
                        int ruleId = DbUtility.GetValueFromDataReader(reader, "RuleID", -1);
                        int expressionId = DbUtility.GetValueFromDataReader(reader, "ExpressionID", -1);

                        var rule = new RuleEntity(ruleId)
                                       {
                                           Expression = _voidExpressions.FirstOrDefault(e => e.Id == expressionId)
                                       };

                        rule.ResetState();
                        _voidRules.Add(rule);
                    }

                    //read operands data
                    reader.NextResult();
                    while (reader.Read())
                    {
                        int operandId = DbUtility.GetValueFromDataReader(reader, "OperandID", -1);
                        int? itemId = DbUtility.GetValueFromDataReader(reader, "ItemID", default(int?));
                        int? parentItemId = DbUtility.GetValueFromDataReader(reader, "ParentItemID", default(int?));
                        int? column = DbUtility.GetValueFromDataReader(reader, "ColumnNumber", default(int?));
                        string category = DbUtility.GetValueFromDataReader(reader, "Category", default(string));

                        var operand = _voidOperands.FirstOrDefault(o => o.Id == operandId);
                        if(operand != null)
                        {
                            operand.ItemId = itemId;
                            operand.ParentItemId = parentItemId;
                            operand.ColumnNumber = column;
                            operand.Category = category;
                            
                            operand.ResetState();
                        }
                    }

                    //read operands answer data
                    reader.NextResult();
                    while (reader.Read())
                    {
                        int operandId = DbUtility.GetValueFromDataReader(reader, "OperandID", -1);
                        int? itemId = DbUtility.GetValueFromDataReader(reader, "ItemID", default(int?));
                        int? optionId = DbUtility.GetValueFromDataReader(reader, "OptionID", default(int?));
                        string answer = DbUtility.GetValueFromDataReader(reader, "AnswerValue", default(string));

                        var operand = _voidOperands.FirstOrDefault(o => o.Id == operandId);
                        if(operand != null)
                        {
                            operand.ItemId = itemId;
                            operand.OptionId = optionId;
                            operand.AnswerValue = answer;

                            operand.ResetState();
                        }
                    }

                    //read operand profile data
                    reader.NextResult();
                    while (reader.Read())
                    {
                        int operandId = DbUtility.GetValueFromDataReader(reader, "OperandID", -1);
                        string key = DbUtility.GetValueFromDataReader(reader, "ProfileKey", default(string));

                        var operand = _voidOperands.FirstOrDefault(o => o.Id == operandId);
                        if (operand != null)
                        {
                            operand.ProfileKey = key;
                            
                            operand.ResetState();
                        }
                    }

                    //read operand response data
                    reader.NextResult();
                    while (reader.Read())
                    {
                        int operandId = DbUtility.GetValueFromDataReader(reader, "OperandID", -1);
                        string key = DbUtility.GetValueFromDataReader(reader, "ResponseKey", default(string));

                        var operand = _voidOperands.FirstOrDefault(o => o.Id == operandId);
                        if (operand != null)
                        {
                            operand.ResponseKey = key;

                            operand.ResetState();
                        }
                    }

                    //read actions
                    reader.NextResult();
                    while (reader.Read())
                    {
                        int actionId = DbUtility.GetValueFromDataReader(reader, "ActionID", -1);
                        string type = DbUtility.GetValueFromDataReader(reader, "ActionTypeName", default(string));
                        string assembly = DbUtility.GetValueFromDataReader(reader, "ActionAssembly", default(string));

                        _voidActions.Add(new ActionEntity(actionId, type, assembly));
                    }

                    //set rules-actions relations
                    reader.NextResult();
                    while (reader.Read())
                    {
                        int ruleId = DbUtility.GetValueFromDataReader(reader, "RuleID", -1);
                        int actionId = DbUtility.GetValueFromDataReader(reader, "ActionID", -1);

                        var rule = _voidRules.FirstOrDefault(r => r.Id == ruleId);
                        var action = _voidActions.FirstOrDefault(a => a.Id == actionId);
                        if (rule != null)
                        {
                            rule.Action = action;
                            
                            rule.ResetState();
                        }
                    }

                    //set item-rules relations
                    reader.NextResult();
                    while (reader.Read())
                    {
                        int ruleId = DbUtility.GetValueFromDataReader(reader, "RuleID", -1);
                        int itemId = DbUtility.GetValueFromDataReader(reader, "ItemID", -1);
                        int pageId = DbUtility.GetValueFromDataReader(reader, "PageID", -1);

                        var item = new ItemEntity(itemId) {Rule = _voidRules.FirstOrDefault(r => r.Id == ruleId), PageID = pageId};
                        
                        item.ResetState();
                        
                        _items.Add(item);
                    }

                    //set page-rules relations
                    reader.NextResult();
                    while (reader.Read())
                    {
                        int ruleId = DbUtility.GetValueFromDataReader(reader, "RuleID", -1);
                        int pageId = DbUtility.GetValueFromDataReader(reader, "PageID", -1);
                        string trigger = DbUtility.GetValueFromDataReader(reader, "EventTrigger", default(string));

                        PageEntity page = _pages.FirstOrDefault(p => p.Id == pageId);
                        if (page == null)
                        {
                            page = new PageEntity(pageId);
                            _pages.Add(page);
                        }

                        var rule = _voidRules.FirstOrDefault(r => r.Id == ruleId);
                        if (rule != null)
                        {
                            rule.EventTrigger = trigger;
                            rule.ResetState();
                            page.Rules.Add(rule);
                        }
                    }

                    //read action data
                    reader.NextResult();
                    while (reader.Read())
                    {
                        int actionId = DbUtility.GetValueFromDataReader(reader, "ActionID", -1);
                        int? goToPageId = DbUtility.GetValueFromDataReader(reader, "GoToPageID", default(int?));

                        var action = _voidActions.FirstOrDefault(a => a.Id == actionId);
                        if (action != null)
                        {
                            action.GoToPageId = goToPageId;
                            action.ResetState();
                        }
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

        }

        #region Access to Rule Data

        /// <summary>
        /// Get data rows for a rule.
        /// </summary>
        /// <param name="ruleId"></param>
        /// <returns></returns>
        public RuleEntity GetRule(int ruleId)
        {
            return ListRules().FirstOrDefault(r => r.Id == ruleId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private IEnumerable<RuleEntity> ListRules()
        {
            var rules = _pages.SelectMany(p => p.Rules).ToList();
            rules.AddRange(_items.Where(i => i.Rule != null).Select(i => i.Rule));
            rules.AddRange(_voidRules);
            return rules.GroupBy(r => r.Id).Select(g => g.First());
        }

        /// <summary>
        /// Get page 
        /// </summary>
        /// <param name="ruleId"></param>
        /// <returns></returns>
        public PageEntity GetPageByRule(int ruleId)
        {
            return _pages.FirstOrDefault(p => p.Rules.Any(r => r.Id == ruleId));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageId"></param>
        /// <param name="eventTrigger"></param>
        /// <returns></returns>
        public IEnumerable<RuleEntity> GetPageRules(int pageId, string eventTrigger)
        {
            return GetPageRules(pageId).Where(r => r.EventTrigger == eventTrigger);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageId"></param>
        /// <returns></returns>
        public IEnumerable<RuleEntity> GetPageRules(int pageId)
        {
            var page = _pages.FirstOrDefault(p => p.Id == pageId);
            return page != null ? page.Rules : new List<RuleEntity>();
        }

        /// <summary>
        /// Get Item Rule
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public RuleEntity GetItemRule(int itemId)
        {
            var item = _items.FirstOrDefault(i => i.Id == itemId);
            return item != null ? item.Rule : null;
        }

        /// <summary>
        /// Get Item 
        /// </summary>
        /// <param name="ruleId"></param>
        /// <returns></returns>
        public ItemEntity GetItemByRule(int ruleId)
        {
            return _items.FirstOrDefault(i =>i.Rule !=null && i.Rule.Id == ruleId);
        }

        /// <summary>
		/// 
		/// </summary>
		/// <param name="ruleId"></param>
		/// <returns></returns>
		public RuleEventTrigger GetRuleEventTrigger(int ruleId)
		{
			RuleEventTrigger result = RuleEventTrigger.Load;

			var rule = GetRule(ruleId);

            if (rule != null)
            {
                object trigger = rule.EventTrigger;

                if (trigger == null)
                    return result;

                Enum.TryParse(trigger.ToString(), out result);
            }

			return result;
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expressionID"></param>
        /// <returns></returns>
        public RuleEntity GetRuleByExpression(int expressionID)
        {
            return ListRules().FirstOrDefault(r => r.Expression.Id == expressionID);
        }

        /// <summary>
        /// Get data for an expression
        /// </summary>
        /// <param name="expressionId"></param>
        /// <returns></returns>
        public ExpressionEntity GetExpression(int expressionId)
        {
            return ListExpressions(true).FirstOrDefault(e => e.Id == expressionId);
        }

        private IEnumerable<ExpressionEntity> ListExpressions(bool includeDeleted)
        {
            List<ExpressionEntity> expressions = new List<ExpressionEntity>();
            foreach (var expression in ListRules().Select(r => r.Expression))
            {
                expressions.Add(expression);
                var parent = expression.Parent;
                while (parent != null)
                {
                    expressions.Add(parent);
                    parent = parent.Parent;
                }
            }

            expressions.AddRange(_voidExpressions);

            if (!includeDeleted)
                expressions = expressions.Where(e => e.State != EntityState.Deleted).ToList();

            return expressions.GroupBy(e => e.Id).Select(g => g.First());
        }


        ///<summary>
        ///</summary>
        ///<returns></returns>
        public IEnumerable<OperandEntity> ListOperands()
        {
            List<OperandEntity> operands = new List<OperandEntity>();

            foreach (var expression in ListExpressions(true))
            {
                if (expression.LeftOperand != null)
                    operands.Add(expression.LeftOperand);
                if (expression.RightOperand != null)
                    operands.Add(expression.RightOperand);
            }

            operands.AddRange(_voidOperands);

            return operands.GroupBy(o => o.Id).Select(g => g.First());
        }

        ///<summary>
        ///</summary>
        ///<param name="itemId"></param>
        ///<returns></returns>
        public IEnumerable<OperandEntity> GetOperandsForItem(int itemId)
        {
            return ListOperands().Where(o => o.ItemId == itemId);
        }

        /// <summary>
        /// Get data for an operand
        /// </summary>
        /// <param name="operandId"></param>
        /// <returns></returns>
        public OperandEntity GetOperand(int operandId)
        {
            return ListOperands().FirstOrDefault(o => o.Id == operandId);
        }

        /// <summary>
        /// Gets a <see cref="DataRow"/>[] of all Expressions subscribing to a given item.
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public IEnumerable<ExpressionEntity> FindItemOperandSubscribers(int itemId)
        {
            var itemOperands = GetOperandsForItem(itemId);

            //Now, for each item operand, get the "left" operand.
            List<ExpressionEntity> subscriberOperandRows = new List<ExpressionEntity>();

            foreach (var operand in itemOperands)
            {
                subscriberOperandRows.AddRange(ListExpressions(false).
                    Where(e => e != null && e.LeftOperand != null && e.LeftOperand.Id == operand.Id));
            }

            return subscriberOperandRows;
        }

        /// <summary>
        /// Gets a <see cref="DataRow"/>[] of all Expressions related to the specified rootExpression.
        /// </summary>
        /// <param name="rootExpressionId"></param>
        /// <returns></returns>
        public IEnumerable<ExpressionEntity> FindItemOperandSubscribersByRootExpression(int rootExpressionId)
        {
            return ListExpressions(false).Where(e => e.Root != null && e.Root.Id == rootExpressionId && e.LeftOperand != null && e.LeftOperand.ItemId != null);
        }

        private IEnumerable<ActionEntity> ListActions()
        {
            var actions = ListRules().Select(r => r.Action).ToList();
            actions.AddRange(_voidActions);
            return actions.GroupBy(a => a.Id).Select(g => g.First());
        }

        /// <summary>
        /// Get action data
        /// </summary>
        /// <param name="actionId"></param>
        /// <returns></returns>
        public ActionEntity GetAction(int actionId)
        {
            return ListActions().FirstOrDefault(a => a.Id == actionId);
        }

        /// <summary>
        /// Get branch action data
        /// </summary>
        /// <param name="actionId"></param>
        /// <returns></returns>
        public ActionEntity GetBranchAction(int actionId)
        {
            return _pages.SelectMany(p => p.Rules)
                .Select(r => r.Action).FirstOrDefault(a => a.Id == actionId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentExpressionId"></param>
        /// <returns></returns>
        public IEnumerable<ExpressionEntity> GetChildExpressions(int parentExpressionId)
        {
            return ListExpressions(false).Where(e => 
                (e.Parent != null && e.Parent.Id == parentExpressionId) || (e.ParentId == parentExpressionId));
        }

        /// <summary>
        /// Gets an array of <see cref="DataRow"/>s containing all expressions that have a page action 
        /// referencing the given page 
        /// </summary>
        /// <param name="pageId"></param>
        /// <returns></returns>
        public IEnumerable<RuleEntity> FindPageActionExpressions(int pageId)
        {
            return ListRules().Where(r => r.Action.GoToPageId == pageId);
        }

        #endregion

        #region Modify Rule Data

        /// <summary>
        /// Add a rule to an item
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="rule"></param>
        public void AddRuleToItem(int itemId, RuleEntity rule)
        {
            var item = _items.FirstOrDefault(i => i.Id == itemId); 

            if(item == null)
            {
                item = EntityBase.Create<ItemEntity>();
                item.Id = itemId;
                _items.Add(item);
            }

            item.Rule = rule;
        }

        /// <summary>
        /// Add a mapping between a rule and a page
        /// </summary>
        /// <param name="pageId"></param>
        /// <param name="rule"></param>
        public void AddRuleToPage(int pageId, RuleEntity rule)
        {
            var page = _pages.FirstOrDefault(p => p.Id == pageId);

            if (page == null)
            {
                page = EntityBase.Create<PageEntity>();
                page.Id = pageId;
                _pages.Add(page);
            }

            if (!page.Rules.Any(r => r.Id == rule.Id))
            {
                page.Rules.Add(rule);
            }
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expressionId"></param>
        public void DeleteExpression(int expressionId)
        {
            var expression = GetExpression(expressionId);

            //Do nothing if expression is not found
            if (expression == null)
            {
                return;
            }

            //If no parent id, this expression likely is a parent and children should be removed
            if (expression.Parent == null)
            {
                //delete the rule if it is root
                var rule = GetRuleByExpression(expressionId);
                if (rule != null)
                {
                    var item = GetItemByRule(rule.Id);
                    if (item != null)
                    {
                        item.Rule = null;
                    }
                    else
                    {
                        var page = GetPageByRule(rule.Id);
                        if (page != null)
                        {
                            page.RemovedRules.Add(rule.Id);
                            page.Rules.Remove(rule);
                        }
                    }
                    _deletedRules.Add(rule.Id);
                    rule.Delete();
                }

                DeleteExpressionTree(expression.Id);
            }
            else //If there is a parent id, this expression could have sibling or be an only child
            {
                var siblingRows = GetChildExpressions(expression.Parent.Id);

                //If parent has children in addition to the expression we want to delete, call
                // delete expression tree
                if (siblingRows.Count() > 1)
                {
                    DeleteExpressionTree(expressionId);
                }
                else
                {
                    //delete parent directly
                    DeleteExpression(expression.Parent.Id);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expressionId"></param>
        private void DeleteExpressionTree(int expressionId)
        {
            // recursively delete children
            var children = GetChildExpressions(expressionId);
            
            foreach (var child in children)
            {
                DeleteExpressionTree(child.Id);
            }

            //Now delete the expression itself
            var expression = GetExpression(expressionId);

            if(expression != null)
                expression.Delete();
        }
        
        #region Rule CRUD

        ///<summary>
        ///</summary>
        public OperandEntity InsertVoidOperand(Type type)
        {
            var operand = OperandEntity.Create(type);
            _voidOperands.Add(operand);

            return operand;
        }

        ///<summary>
        ///</summary>
        public ExpressionEntity InsertVoidExpression()
        {
            var expression = EntityBase.Create<ExpressionEntity>();
            _voidExpressions.Add(expression);

            return expression;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ruleId"></param>
        public void DeleteRule(int ruleId)
        {
            var item = GetItemByRule(ruleId);
            var rule = GetRule(ruleId);
            if (item != null)
            {
                item.Rule = null;
            }
            else
            {
                var page = GetPageByRule(ruleId);
                if (page != null)
                {
                    page.RemovedRules.Add(rule.Id);
                    page.Rules.Remove(rule);
                }
            }

            if (rule.Expression != null)
            {
                DeleteExpression(rule.Expression.Id);
            }

            _deletedRules.Add(rule.Id);

            rule.Delete();
        }

        /// <summary>
        /// Delete all rules associated with a page.
        /// </summary>
        /// <param name="pageId"></param>
        public void DeletePageRules(int pageId)
        {
            var page = _pages.FirstOrDefault(p => p.Id == pageId);
            if (page != null)
            {
                //as collection page.Rules is going to be modified inside 'foreach' let's make it's local copy
                var rulesCollection = new List<RuleEntity>();
                rulesCollection.AddRange(page.Rules);
                foreach (var rule in rulesCollection)
                {
                    DeleteRule(rule.Id);
                }
                page.Rules.Clear();
            }
        }

        #endregion  

        
        #region Save
        ///<summary>
        ///</summary>
        ///<exception cref="Exception"></exception>
        public void Save()
        {
            Database db = DatabaseFactory.CreateDatabase();

            using (IDbConnection connection = db.GetConnection())
            {
                connection.Open();
                IDbTransaction transaction = connection.BeginTransaction();

                try
                {
                    Save(transaction);
                    transaction.Commit();

                    //Mark the template as updated
                    //TODO: Ensure template updated on save
                }
                catch (Exception ex)
                {
                    ExceptionPolicy.HandleException(ex, "BusinessProtected");

                    transaction.Rollback();
                    throw new Exception("Unable to save data.");
                }
                finally
                {
                    _deletedRules.Clear();
                    connection.Close();
                }
            }
        }

        ///<summary>
        ///</summary>
        ///<param name="t"></param>
        public void Save(IDbTransaction t)
        {
            DeleteObjects(t);

            //save operands
            foreach (var operand in _voidOperands)
            {
                if (operand.State == EntityState.Added || operand.State == EntityState.Updated)
                    SaveOperand(operand, t);
            }

            SaveItems(t);
            SavePages(t);

            //save expressions
            foreach (var expression in _voidExpressions)
            {
                if (expression.State == EntityState.Added || expression.State == EntityState.Updated)
                    SaveExpression(expression, t);
            }
        }

        private void DeleteObjects(IDbTransaction t)
        {
            //deleted operands
            var operands = ListOperands().Where(o => o.Id >= 0 && o.State == EntityState.Deleted);
            foreach (var operand in operands)
            {
                DeleteItemOperandCommand(operand.Id, t);
                DeleteValueOperandCommand(operand.Id, t);
                DeleteProfileOperandCommand(operand.Id, t);
                DeleteResponseOperandCommand(operand.Id, t);
                DeleteOperandCommand(operand.Id, t);
            }
            //delete actions
            var actions = ListActions().Where(a => a.Id >= 0 && a.State == EntityState.Deleted);
            foreach (var action in actions)
            {
                if (action.ParentRuleId.HasValue)
                    DeleteActionFromRuleCommand(action.Id, action.ParentRuleId.Value, t);

                if (action.GoToPageId != null)
                    DeleteBranchActionCommand(action.Id, t);

                DeleteActionCommand(action.Id, t);
            }
            //delete rules from items
            foreach (var item in _items)
            {
                if (item.Id > 0 && item.DeletedRule.HasValue)
                    DeleteRuleFromItemCommand(item.Id, t);
            }
            //delete rules from pages
            foreach (var page in _pages)
            {
                if (page.Id > 0 && page.RemovedRules.Any())
                {
                    foreach (var ruleId in page.RemovedRules)
                    {
                        if (ruleId > 0)
                            DeleteRuleFromPageCommand(page.Id, ruleId, t);
                    }
                }
            }
            //deleted rules
            var rules = ListRules().Where(o => o.Id >= 0 && o.State == EntityState.Deleted);
            foreach (var rule in rules)
            {
                DeleteRuleCommand(rule.Id, t);
            }
            //deleted expressions
            var expressions = ListExpressions(true).Where(o => o.Id >= 0 && o.State == EntityState.Deleted);
            foreach (var expression in expressions)
            {
                DeleteExpressionCommand(expression.Id, t);
            }
        }

        private void SaveItems(IDbTransaction t)
        {
            foreach (var item in _items)
            {
                var rule = item.Rule;
                if (rule != null)
                {
                    var expression = rule.Expression;
                    if (expression != null)
                    {
                        //insert/update expression
                        SaveExpression(expression, t);

                        //save rule
                        if (rule.State == EntityState.Added)
                        {
                            rule.Id = InsertRuleCommand(expression.Id, t);
                            rule.ResetState();
                        }

                        //save action
                        var action = rule.Action;
                        if (action != null)
                        {
                            if (action.State == EntityState.Added)
                                action.Id = InsertActionCommand(action.TypeName, action.AssemblyName, t);

                            AddActionToRuleCommand(action.Id, rule.Id, t);
                        }
                    }

                    AddRuleToItemCommand(item.Id, rule.Id, t);
                }
            }
        }

        Dictionary<int, int> _expressionIDMap;
        /// <summary>
        /// This dictionary maps old Ids (negative) to the real ones
        /// </summary>
        public Dictionary<int, int> ExpressionIDMap
        {
            get
            {
                if (_expressionIDMap == null)
                    _expressionIDMap = new Dictionary<int, int>();
                return _expressionIDMap;
            }
        }

        private void SaveExpression(ExpressionEntity expression, IDbTransaction t)
        {
            var left = expression.LeftOperand;
            var right = expression.RightOperand;
            
            SaveOperand(left, t);
            SaveOperand(right, t);

            if (expression.State == EntityState.Updated)
                UpdateExpressionCommand(expression.Id, expression.Operator,
                                        left != null ? left.Id : default(int?),
                                        right != null ? right.Id : default(int?),
                                        expression.Parent != null ? expression.Parent.Id : default(int?),
                                        expression.Depth, expression.Lineage,
                                        expression.Root != null ? expression.Root.Id : default(int?),
                                        expression.ChildRelation, t);
            else if (expression.State == EntityState.Added)
            {
                int oldId = expression.Id;
                expression.Id = InsertExpressionCommand(expression.Operator,
                                                        left != null ? left.Id : default(int?),
                                                        right != null ? right.Id : default(int?),
                                                        expression.Parent != null ? expression.Parent.Id : default(int?),
                                                        expression.Depth, expression.Lineage,
                                                        expression.Root != null ? expression.Root.Id : default(int?),
                                                        expression.ChildRelation, t);
                
                ExpressionIDMap[oldId] = expression.Id;

                //update children
                foreach (var child in GetChildExpressions(oldId))
                {
                    child.ParentId = expression.Id;
                    child.Parent = expression;
                    child.Lineage = child.Lineage.Replace("/" + oldId.ToString() + "/", "/" + expression.Id.ToString() + "/");
                    child.Update();
                    SaveExpression(child, t);
                }
            }
            expression.ResetState();
        }

        private void SavePages(IDbTransaction t)
        {
            foreach (var page in _pages)
            {
                foreach (var rule in page.Rules)
                {
                    var expression = rule.Expression;
                    if (expression != null)
                    {
                        //insert/update expression
                        SaveExpression(expression, t);

                        //save rule
                        if (rule.State == EntityState.Added)
                        {
                            rule.Id = InsertRuleCommand(expression.Id, t);
                            rule.ResetState();
                        }

                        //save action
                        var action = rule.Action;
                        if (action != null)
                        {
                            if (action.State == EntityState.Added)
                            {
                                action.Id = InsertActionCommand(action.TypeName, action.AssemblyName, t);
                                action.ResetState();

                                if(action.GoToPageId.HasValue)
                                    InsertBranchActionCommand(action.Id, action.GoToPageId, t);
                            }
                            else if (action.State == EntityState.Updated && action.GoToPageId.HasValue)
                                UpdateBranchActionCommand(action.Id, action.GoToPageId, t);

                            action.ResetState();

                            AddActionToRuleCommand(action.Id, rule.Id, t);
                        }
                    }

                    AddRuleToPageCommand(page.Id, rule.Id, rule.EventTrigger, t);
                }
            }
        }

        private void SaveOperand(OperandEntity operand, IDbTransaction t)
        {
            if (operand != null && (operand.State == EntityState.Added || operand.State == EntityState.Updated))
            {
                if (operand.State == EntityState.Added)
                    operand.Id = InsertOperandCommand(operand.TypeName, operand.AssemblyName, t);

                //check for value operand
                if (!string.IsNullOrEmpty(operand.AnswerValue) || operand.OptionId.HasValue)
                {
                    if (operand.State == EntityState.Updated)
                        UpdateValueOperandCommand(operand.Id, operand.ItemId, operand.OptionId, operand.AnswerValue, t);
                    else if (operand.State == EntityState.Added)
                        InsertValueOperandCommand(operand.Id, operand.ItemId, operand.OptionId, operand.AnswerValue, t);
                }
                else
                {
                    // check for item operand
                    if (operand.ItemId != null)
                    {
                        if (operand.State == EntityState.Updated)
                            UpdateItemOperandCommand(operand.Id, operand.ItemId, operand.ParentItemId, operand.ColumnNumber, operand.Category, t);
                        else if (operand.State == EntityState.Added)
                            InsertItemOperandCommand(operand.Id, operand.ItemId, operand.ParentItemId, operand.ColumnNumber, operand.Category, t);
                    }
                    else
                    {
                        //check for response key
                        if (!string.IsNullOrEmpty(operand.ResponseKey))
                            InsertResponseOperandCommand(operand.Id, operand.ResponseKey, t);
                            //check for profile key
                        else if (!string.IsNullOrEmpty(operand.ProfileKey))
                            InsertProfileOperandCommand(operand.Id, operand.ProfileKey, t);
                    }
                }
                operand.ResetState();
            }
        }

        #endregion

        #region DBCommandWrappers

        /// <summary>
        /// Get a command to add a rule to a page
        /// </summary>
        /// <returns></returns>
        private static void AddRuleToPageCommand(int pageID, int ruleID, string eventTrigger, IDbTransaction transaction)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_TemplatePage_AddRule");
            command.AddInParameter("PageID", DbType.Int32, pageID);
            command.AddInParameter("RuleID", DbType.Int32, ruleID);
            command.AddInParameter("EventTrigger", DbType.String, eventTrigger);
            db.ExecuteNonQuery(command, transaction);
        }

        /// <summary>
        /// Get a command to remove a rule from a page
        /// </summary>
        /// <returns></returns>
        private static void DeleteRuleFromPageCommand(int pageID, int ruleID, IDbTransaction transaction)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_TemplatePage_DeleteRule");
            command.AddInParameter("PageID", DbType.Int32, pageID);
            command.AddInParameter("RuleID", DbType.Int32, ruleID);
            db.ExecuteNonQuery(command, transaction);
        }

        private static void AddRuleToItemCommand(int itemID, int ruleID, IDbTransaction transaction)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_AddRule");
            command.AddInParameter("ItemID", DbType.Int32, itemID);
            command.AddInParameter("RuleID", DbType.Int32, ruleID);
            db.ExecuteNonQuery(command, transaction);
        }


        private static int InsertExpressionCommand(int exprOperator, int? left, int? right, int? parent, int depth,
            string lineage, int? root, string childRelation, IDbTransaction transaction)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Rules_CreateExpression");
            command.AddInParameter("Operator", DbType.Int32, exprOperator);
            command.AddInParameter("Left", DbType.Int32, left);
            command.AddInParameter("Right", DbType.Int32, right);
            command.AddInParameter("Parent", DbType.Int32, parent);
            command.AddInParameter("Depth", DbType.Int32, depth);
            command.AddInParameter("Lineage", DbType.String, lineage);
            command.AddInParameter("Root", DbType.Int32, root);
            command.AddInParameter("LogicalConnector", DbType.String, childRelation);
            command.AddOutParameter("ExpressionID", DbType.Int32, 4);
            db.ExecuteNonQuery(command, transaction);

            return (int)command.GetParameterValue("ExpressionID");
        }

        private static void UpdateExpressionCommand(int expressionId, int exprOperator, int? left, int? right, int? parent,
            int depth, string lineage, int? root, string childRelation, IDbTransaction transaction)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Rules_UpdateComposite");
            command.AddInParameter("ExpressionID", DbType.Int32, expressionId);
            command.AddInParameter("Operator", DbType.Int32, exprOperator);
            command.AddInParameter("Left", DbType.Int32, left);
            command.AddInParameter("Right", DbType.Int32, right);
            command.AddInParameter("Parent", DbType.Int32, parent);
            command.AddInParameter("Depth", DbType.Int32, depth);
            command.AddInParameter("Lineage", DbType.String, lineage);
            command.AddInParameter("Root", DbType.Int32, root);
            command.AddInParameter("LogicalConnector", DbType.String, childRelation);
            db.ExecuteNonQuery(command, transaction);
        }

        private static void DeleteExpressionCommand(int expressionID, IDbTransaction transaction)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Rules_DeleteExpression");
            command.AddInParameter("ExpressionID", DbType.Int32, expressionID);
            db.ExecuteNonQuery(command, transaction);
        }

        private static void DeleteRuleFromItemCommand(int itemID, IDbTransaction transaction)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemData_RemoveRule");
            command.AddInParameter("ItemID", DbType.Int32, itemID);
            db.ExecuteNonQuery(command, transaction); 
        }

        private static void AddActionToRuleCommand(int actionID, int ruleID, IDbTransaction transaction)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Rules_AddAction");
            command.AddInParameter("RuleID", DbType.Int32, ruleID);
            command.AddInParameter("ActionID", DbType.Int32, actionID);
            db.ExecuteNonQuery(command, transaction);
        }

        private static void DeleteActionFromRuleCommand(int actionID, int ruleID, IDbTransaction transaction)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Rules_RemoveAction");
            command.AddInParameter("RuleID", DbType.Int32, ruleID);
            command.AddInParameter("ActionID", DbType.Int32, actionID);
            db.ExecuteNonQuery(command, transaction);    
        }

        private static int InsertOperandCommand(string typeName, string assembly, IDbTransaction transaction)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Rules_InsertOperand");
            command.AddInParameter("TypeName", DbType.String, typeName);
            command.AddInParameter("TypeAssembly", DbType.String, assembly);
            command.AddOutParameter("OperandID", DbType.Int32, 4);
            db.ExecuteNonQuery(command, transaction);

            return (int)command.GetParameterValue("OperandID");
        }

        private static void InsertItemOperandCommand(int operandID, int? itemID, int? parentItemID, int? columnNumber, string category, IDbTransaction transaction)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemOperand_Create");
            command.AddInParameter("OperandID", DbType.Int32, operandID);
            command.AddInParameter("ItemID", DbType.Int32, itemID);
            command.AddInParameter("ParentItemID", DbType.Int32, parentItemID);
            command.AddInParameter("ColumnNumber", DbType.Int32, columnNumber);
            command.AddInParameter("Category", DbType.String, category);
            db.ExecuteNonQuery(command, transaction);
        }

        private static void UpdateItemOperandCommand(int operandID, int? itemID, int? parentItemID, int? columnNumber, string category, IDbTransaction transaction)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemOperand_Update");
            command.AddInParameter("OperandID", DbType.Int32, operandID);
            command.AddInParameter("ItemID", DbType.Int32, itemID);
            command.AddInParameter("ParentItemID", DbType.Int32, parentItemID);
            command.AddInParameter("ColumnNumber", DbType.Int32, columnNumber);
            command.AddInParameter("Category", DbType.String, category);
            db.ExecuteNonQuery(command, transaction);
        }

        private static void InsertValueOperandCommand(int operandID, int? itemID, int? optionID, string answerValue, IDbTransaction transaction)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ValueOperand_Create");
            command.AddInParameter("OperandID", DbType.Int32, operandID);
            command.AddInParameter("ItemID", DbType.Int32, itemID);
            command.AddInParameter("OptionID", DbType.Int32, optionID);
            command.AddInParameter("AnswerValue", DbType.String, answerValue);
            db.ExecuteNonQuery(command, transaction);
        }

        private static void UpdateValueOperandCommand(int operandID, int? itemID, int? optionID, string answerValue, IDbTransaction transaction)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ValueOperand_Update");
            command.AddInParameter("OperandID", DbType.Int32, operandID);
            command.AddInParameter("ItemID", DbType.Int32, itemID);
            command.AddInParameter("OptionID", DbType.Int32, optionID);
            command.AddInParameter("AnswerValue", DbType.String, answerValue);
            db.ExecuteNonQuery(command, transaction);
        }

        private static void InsertProfileOperandCommand(int operandID, string profileKey, IDbTransaction transaction)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ProfileOperand_Create");
            command.AddInParameter("OperandID", DbType.Int32, operandID);
            command.AddInParameter("ProfileKey", DbType.String, profileKey);
            db.ExecuteNonQuery(command, transaction);
        }

        private static void InsertResponseOperandCommand(int operandID, string responseKey, IDbTransaction transaction)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ResponseOperand_Create");
            command.AddInParameter("OperandID", DbType.Int32, operandID);
            command.AddInParameter("ResponseKey", DbType.String, responseKey);
            db.ExecuteNonQuery(command, transaction);
        }

        private static void DeleteOperandCommand(int operandID, IDbTransaction transaction)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Rules_DeleteOperand");
            command.AddInParameter("OperandID", DbType.Int32, operandID);
            db.ExecuteNonQuery(command, transaction);
        }

        private static void DeleteItemOperandCommand(int operandID, IDbTransaction transaction)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ItemOperand_Delete");
            command.AddInParameter("OperandID", DbType.Int32, operandID);
            db.ExecuteNonQuery(command, transaction);
        }

        private static void DeleteValueOperandCommand(int operandID, IDbTransaction transaction)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ValueOperand_Delete");
            command.AddInParameter("OperandID", DbType.Int32, operandID);
            db.ExecuteNonQuery(command, transaction);
        }

        private static void DeleteProfileOperandCommand(int operandID, IDbTransaction transaction)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ProfileOperand_Delete");
            command.AddInParameter("OperandID", DbType.Int32, operandID);
            db.ExecuteNonQuery(command, transaction);
        }

        private static void DeleteResponseOperandCommand(int operandID, IDbTransaction transaction)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_ResponseOperand_Delete");
            command.AddInParameter("OperandID", DbType.Int32, operandID);
            db.ExecuteNonQuery(command, transaction);
        }


        private static int InsertRuleCommand(int expressionID, IDbTransaction transaction)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Rules_Create");
            command.AddInParameter("ExpressionID", DbType.Int32, expressionID);
            command.AddOutParameter("RuleID", DbType.Int32, 4);
            db.ExecuteNonQuery(command, transaction);
            
            return (int)command.GetParameterValue("RuleID");
        }


        private static int InsertActionCommand(string actionTypeName, string actionAssembly, IDbTransaction transaction)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Rules_CreateAction");
            command.AddInParameter("ActionTypeName", DbType.String, actionTypeName);
            command.AddInParameter("ActionTypeAssembly", DbType.String, actionAssembly);
            command.AddOutParameter("ActionID", DbType.Int32, 4);
            db.ExecuteNonQuery(command, transaction);
            
            return (int)command.GetParameterValue("ActionID");
        }

        private static void InsertBranchActionCommand(int actionID, int? goToPageID, IDbTransaction transaction)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_BranchAction_Create");
            command.AddInParameter("ActionID", DbType.String, actionID);
            command.AddInParameter("GoToPageID", DbType.String, goToPageID);
            db.ExecuteNonQuery(command, transaction);
        }

        private static void UpdateBranchActionCommand(int actionID, int? goToPageID, IDbTransaction transaction)
        {
            Database db = DatabaseFactory.CreateDatabase();

            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_BranchAction_Update");
            command.AddInParameter("ActionID", DbType.String, actionID);
            command.AddInParameter("GoToPageID", DbType.String, goToPageID);
            db.ExecuteNonQuery(command, transaction);
        }

        private static void DeleteActionCommand(int actionID, IDbTransaction transaction)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Rules_DeleteAction");
            command.AddInParameter("ActionID", DbType.Int32, actionID);
            db.ExecuteNonQuery(command, transaction);
        }

        private static void DeleteBranchActionCommand(int actionID, IDbTransaction transaction)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_BranchAction_Delete");
            command.AddInParameter("ActionID", DbType.Int32, actionID);
            db.ExecuteNonQuery(command, transaction);
        }

        private static void DeleteRuleCommand(int ruleID, IDbTransaction transaction)
        {
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Rules_Delete");
            command.AddInParameter("RuleID", DbType.Int32, ruleID);
            db.ExecuteNonQuery(command, transaction);
        }

        #endregion
    }
}
