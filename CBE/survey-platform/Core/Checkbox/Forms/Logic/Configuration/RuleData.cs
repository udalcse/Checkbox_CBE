using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Prezza.Framework.Common;
using Checkbox.Globalization.Text;

namespace Checkbox.Forms.Logic.Configuration
{
    ///<summary>
    ///</summary>
    [Serializable]
    public sealed class RuleData
    {
        ///<summary>
        ///</summary>
        public RuleEventTrigger Trigger { get; set; }

        /// <summary>
        /// Get rule id
        /// </summary>
        public int RuleId { get; private set; }

        ///<summary>
        ///</summary>
        public List<ActionData> Actions { get; set; }

        /// <summary>
        /// Get/set Expression for the rule
        /// </summary>
        public ExpressionData Expression { get; set; }

        ///<summary>
        ///</summary>
        ///<param name="expression"></param>
        ///<param name="actions"></param>
        public RuleData(ExpressionData expression, List<ActionData> actions)
        {
            Expression = expression;
            Actions = actions;
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		public RuleData(int id)
		{
			RuleId = id;
		}

        /// <summary>
        /// Load the rule from a rules data set.
        /// </summary>
        public void Load(int ruleId, RulesObjectSet rulesData)
        {
            RuleId = ruleId;
			Trigger = rulesData.GetRuleEventTrigger(ruleId);

            //Load the rule data.
            var rule = rulesData.GetRule(ruleId);

            if (rule == null)
            {
                throw new Exception(string.Format("RuleData for this RuleData [{0}] was not found", ruleId));
            }

            int? expressionId = rule.Expression.Id;

            if (!expressionId.HasValue)
            {
                throw new Exception(string.Format("Expression for this RuleData [{0}] had a NULL id.", ruleId));
            }

            Expression.Load(expressionId.Value, rulesData);

            //Load action data
            Actions.Clear();

            var ruleAction = rulesData.GetRule(ruleId).Action;
            var actionDataFactory = new ActionDataFactory();

            int? actionId = ruleAction.Id;

            string actionTypeName = string.Format("{0},{1}", ruleAction.TypeName, ruleAction.AssemblyName);

            ActionData actionData = actionDataFactory.CreateActionData(actionTypeName);

            if (actionData != null)
            {
                actionData.Load(actionId.Value, rulesData);
                Actions.Add(actionData);
            }
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="writer"></param>
		public void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement(GetType().Name);
			writer.WriteAttributeString("Trigger", Trigger.ToString());

			Expression.WriteXml(writer);

			writer.WriteStartElement("Actions");
			writer.WriteAttributeString("Count", Actions.Count.ToString());

			foreach (ActionData action in Actions)
				action.WriteXml(writer);

			writer.WriteEndElement();// actions

			writer.WriteEndElement();
		}

		/// <summary>
		/// 
		/// </summary>
        /// <param name="xmlNode"></param>
		public void Load(XmlNode xmlNode)
		{
		    Trigger = (RuleEventTrigger) XmlUtility.GetAttributeEnum(xmlNode, "Trigger", typeof (RuleEventTrigger));
			
            //Ensure child node exists
            if(!xmlNode.HasChildNodes)
            {
                throw new Exception("RuleData was not valid.   Expression Data node was not found.");
            }

		    var expressionNode = xmlNode.FirstChild;

            if (Expression == null)
            {
                if (expressionNode.Name == typeof(ExpressionData).Name)
                {
                    Expression = new ExpressionData();
                }
                else if (expressionNode.Name == typeof(CompositeExpressionData).Name)
                {
                    Expression = new CompositeExpressionData();
                }
            }

            if (Expression != null)
            {
                Expression.Load(expressionNode);
            }


		    var actionsNode = xmlNode.SelectSingleNode("Actions");

            if (actionsNode == null)
            {
                throw new Exception("RuleData had no actions.");
            }

            for (int i = 0; i < actionsNode.ChildNodes.Count; i++)
            {
                ActionData action = null;

                if (Actions == null || Actions.Count <= i)
                {
                    action = ActionDataFactory.CreateActionDataObject(actionsNode.ChildNodes[i].Name);

                    if (Actions == null)
                        Actions = new List<ActionData>();

                    Actions.Add(action);
                }

                if (Actions != null && Actions.Count > i)
                {
                    if (action == null)
                        action = Actions[i];
                }

                if (action != null)
                {
                    action.Load(actionsNode.ChildNodes[i]);
                }
            }
		}
      

        /// <summary>
        /// Provides a human-readable description of this Rule by combining its Expression and Actions ToString() evaluates
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
			StringBuilder s = new StringBuilder();
			s.Append(TextManager.GetText("/logic/if"));
			s.Append(" ");

			s.Append(Expression.ToString());

			if (Expression is CompositeExpressionData && ((CompositeExpressionData)Expression).Children.Count > 0)
			{
				s.Append(Environment.NewLine);
			}
			else
			{
				s.Append(" ");
			}

			s.Append(TextManager.GetText("/logic/then"));
			s.Append(" ");

			for (int x = 0; x < Actions.Count; x++)
			{
				s.Append(Actions[x].ToString());
				if (x < (Actions.Count - 1))
				{
					s.Append(" ");
					s.Append(TextManager.GetText("/logic/and"));
					s.Append(" ");
				}
			}

			return s.ToString();
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <returns></returns>
        //public override DataSet GetConfigurationDataSet()
        //{
        //    if (_context == null)
        //    {
        //        if (ID == null)
        //        {
        //            throw new ApplicationException("No Identity specified.");
        //        }

        //        try
        //        {
        //            string[] tableNames = new string[4];
        //            tableNames[0] = "Rule";
        //            tableNames[1] = "Expression";
        //            tableNames[2] = "Operand";
        //            tableNames[3] = "Action";

        //            Database db = DatabaseFactory.CreateDatabase();
        //            DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_Rules_GetData");
        //            command.AddInParameter("RuleID", DbType.Int32, ID.Value);

        //            DataSet ds = new DataSet();
        //            db.LoadDataSet(command, ds, tableNames);

        //            return ds;
        //        }
        //        catch (Exception ex)
        //        {
        //            bool rethrow = ExceptionPolicy.HandleException(ex, "BusinessProtected");

        //            if (rethrow)
        //            {
        //                throw;
        //            }

        //            return null;
        //        }
        //    }
        //    return new DataSet();
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="transaction"></param>
        //public override void Save(IDbTransaction transaction)
        //{
        //    // Expressions are autonomously saved
        //    Expression.Save(transaction);

        //    base.Save(transaction);

        //    // now add the actions
        //    foreach (ActionData action in Actions)
        //    {
        //        // save the action
        //        action.Save(transaction);

        //        DataRow[] drs = _context.RuleActionsTable.Select("RuleID=" + ID + " AND ActionID=" + action.ID);
        //        if (drs.Length == 0)
        //        {
        //            // add the actiondata
        //            DataRow dr = _context.RuleActionsTable.NewRow();
        //            dr["RuleID"] = ID.Value;
        //            dr["ActionID"] = action.ID.Value;

        //            _context.RuleActionsTable.Rows.Add(dr);
        //        }
        //    }
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="transaction"></param>
        //protected override void Create(IDbTransaction transaction)
        //{
        //    DataRow dr = _context.RuleTable.NewRow();
        //    dr["ExpressionID"] = Expression.ID.Value;
        //    _context.RuleTable.Rows.Add(dr);
        //    ID = (int)dr["RuleID"];
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="t"></param>
        //protected override void Update(IDbTransaction t)
        //{

        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="t"></param>
        //public override void Delete(IDbTransaction t)
        //{
        //    if (ID != null)
        //    {
        //        Expression.Delete(t);

        //        foreach (ActionData action in Actions)
        //        {
        //            action.Delete(t);
        //        }

        //        DataRow[] rows = _context.RuleTable.Select(IdentityColumnName + "=" + ID.Value);
        //        if (rows.Length > 0)
        //        {
        //            rows[0].Delete();
        //        }
        //    }
        //}

        //private void LoadRuleExpression(DataSet data)
        //{
           
        //}

        //private void LoadRuleActions(DataSet data)
        //{
            
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="data"></param>
        //public override void Load(DataSet data)
        //{
        //    try
        //    {
        //        if (!data.Tables.Contains(DataTableName))
        //        {
        //            throw new ApplicationException("Cannot find DataTable for RuleData");
        //        }

        //        LoadRuleExpression(data);
        //        LoadRuleActions(data);

        //    }
        //    catch (Exception ex)
        //    {
        //        bool rethrow = ExceptionPolicy.HandleException(ex, "BusinessPublic");

        //        if (rethrow)
        //        {
        //            throw;
        //        }
        //    }
        //}

        /// <summary>
        /// 
        /// </summary>
        public bool Validate()
        {
            return Expression.Validate();
        }

        /// <summary>
        /// Total conditions count
        /// </summary>
        public int TotalConditionsCount
        {
            get
            {
                return Expression == null ? 0 : Expression.ConditionsCount;
            }
        }

	}
}
