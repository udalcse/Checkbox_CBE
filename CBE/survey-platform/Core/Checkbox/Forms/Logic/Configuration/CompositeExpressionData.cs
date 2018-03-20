using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Checkbox.Globalization.Text;
using Prezza.Framework.Common;
using Prezza.Framework.Data;


namespace Checkbox.Forms.Logic.Configuration
{
    ///<summary>
    ///</summary>
    [Serializable]
    public class CompositeExpressionData : ExpressionData
    {
        private List<ExpressionData> _children;
        private List<ExpressionData> _deletedChildren;

        ///<summary>
        ///</summary>
        public CompositeExpressionData()
            : this(new List<ExpressionData>(), LogicalConnector.AND)
        {
        }

        ///<summary>
        ///</summary>
        ///<param name="children"></param>
        ///<param name="connector"></param>
        public CompositeExpressionData(List<ExpressionData> children, LogicalConnector connector)
        {
            _children = children ?? new List<ExpressionData>();
            Connector = connector;
        }

        /// <summary>
        /// Gets a value indicating the logical relationship between all children of this Expression
        /// </summary>
        public LogicalConnector Connector { get; set; }

        ///<summary>
        ///</summary>
        public List<ExpressionData> Children
        {
            get { return _children ?? (_children = new List<ExpressionData>()); }
        }

        /// <summary>
        /// Adds a child <see cref="ExpressionData"/> to this CompositeExpression
        /// </summary>
        /// <param name="child">the child Expression</param>
        public void Add(ExpressionData child)
        {
            //TODO: Update if reference to parent data is still required
            child.Parent = this;
            child.ParentExpressionId = ExpressionId;
            Children.Add(child);
        }

        ///<summary>
        ///</summary>
        ///<param name="child"></param>
        public void RemoveChild(ExpressionData child)
        {
            Children.Remove(child);
            DeletedChildren.Add(child);
        }

        /// <summary>
        /// Move a child expression to a new parent
        /// </summary>
        /// <param name="child"></param>
        /// <param name="newParent"></param>
        public void MoveChild(ExpressionData child, CompositeExpressionData newParent)
        {
            if (child != null && newParent != null)
            {
                //Remove and unbind
                Children.Remove(child);

                //Add to the new parent
                newParent.Add(child);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private List<ExpressionData> DeletedChildren
        {
            get { return _deletedChildren ?? (_deletedChildren = new List<ExpressionData>()); }
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool Validate()
        {
            return Children.Aggregate(true, (current, child) => current && child.Validate());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            try
            {
                var s = new StringBuilder();
                //s.Append("(");
                for (int i = 0; i < Children.Count; i++)
                {
                    ExpressionData child = Children[i];

                    if (i > 0)
                    {
                        //TODO: Language Code 
                        s.Append(Environment.NewLine);
                        s.Append(TextManager.GetText("/logic/" + Connector, "en-US"));
                        s.Append(" ");
                    }

                    if (Children.Count > 1)
                    {
                        s.Append("(");
                    }

                    s.Append(child.ToString());

                    if (Children.Count > 1)
                    {
                        s.Append(")");
                    }

                }
                //s.Append(")");

                return s.ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ruleData"></param>
        /// <returns></returns>
        protected override int InsertExpression(RulesObjectSet ruleData)
        {
            return ruleData.InsertVoidExpression().Id;
        }

        /// <summary>
        /// Update rule data with expression configuration
        /// </summary>
        /// <param name="ruleData"></param>
        public override void UpdateRuleData(RulesObjectSet ruleData)
        {
            //Update expression id, left, right
            base.UpdateRuleData(ruleData);
            
            //Update children

            //Ensure deleted children removed
            foreach (ExpressionData deletingChild in DeletedChildren)
            {
                var expression = ruleData.GetExpression(deletingChild.ExpressionId.Value);
                ruleData.DeleteExpression(expression.Id);
            }

            //Update children
            foreach(var expressionChild in Children)
            {
                expressionChild.ParentExpressionId = ExpressionId;
                expressionChild.Parent = this;
                expressionChild.UpdateRuleData(ruleData);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="ruleData"></param>
        protected override void UpdateExpression(ExpressionEntity expression, RulesObjectSet ruleData)
        {
			base.UpdateExpression(expression, ruleData);

            expression.ChildRelation = Connector.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
		/// <param name="expressionId"></param>
		/// <param name="rulesData"></param>
		public override bool Load(int expressionId, RulesObjectSet rulesData)
        {
            bool success = true;

            ExpressionId = expressionId;

            var parentExpression = rulesData.GetExpression(expressionId);

            if (parentExpression == null)
            {
                throw new Exception(string.Format("Could not find data for CompositeExpression [{0}]", expressionId));
            }

            //Get child relationship
            string childRelation = parentExpression.ChildRelation;

            if (string.IsNullOrEmpty(childRelation))
            {
                throw new Exception(string.Format("No child relation specified for CompositeExpression [{0}]", expressionId));
            }

            //Set connector
            Connector = (LogicalConnector)Enum.Parse(typeof(LogicalConnector), childRelation);

            //Now find child of relation
            var childExpressions = rulesData.GetChildExpressions(expressionId);

            foreach (var child in childExpressions)
            {
                //Note: Conditions back end supports infinite nesting of expressions, though this is not currently supported
                // by the UI.  As a result, we need to handle this case.

                var childExpressionId = child.Id;
                childRelation = child.ChildRelation;

                //If no child relation, then the child is not a composite
                if (string.IsNullOrEmpty(childRelation))
                {
                    //Create and load
                    var childExpression = new ExpressionData();
                    success = childExpression.Load(childExpressionId, rulesData);

                    //Add to children collection
                    if (success)
                        Add(childExpression);
                }
                else
                {
                    //Create and load child
                    var childComposite = new CompositeExpressionData(
                        new List<ExpressionData>(),
                        (LogicalConnector)Enum.Parse(typeof(LogicalConnector), childRelation));

                    success = childComposite.Load(childExpressionId, rulesData);

                    //Add to list of children
                    if (success)
                        Add(childComposite);
                }
            }
            return success;
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="writer"></param>
		public override void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement(GetType().Name);
			writer.WriteAttributeString("Operator", Operator.ToString());
			writer.WriteAttributeString("Connector", Connector.ToString());

			writer.WriteStartElement("Left");
			if(Left != null) Left.WriteXml(writer);
			//else writer.WriteNull();
			writer.WriteEndElement();

			writer.WriteStartElement("Right");
			if(Right != null) Right.WriteXml(writer);
			//else writer.WriteNull();
			writer.WriteEndElement();

			writer.WriteStartElement("Children");
			writer.WriteAttributeString("Count", Children.Count.ToString());

            foreach (ExpressionData child in Children)
            {
                child.WriteXml(writer);
            }

		    writer.WriteEndElement(); // children


			writer.WriteEndElement();
		}

		/// <summary>
		/// 
		/// </summary>
        /// <param name="xmlNode"></param>
        public override void Load(XmlNode xmlNode)
		{
		    var lOperator = XmlUtility.GetAttributeText(xmlNode, "Operator");

            if(string.IsNullOrEmpty(lOperator))
            {
                lOperator = LogicalOperator.OperatorNotSpecified.ToString();
            }

		    var lConnector = XmlUtility.GetAttributeText(xmlNode, "Connector");

            if(string.IsNullOrEmpty(lConnector))
            {
                lConnector = LogicalConnector.OR.ToString();
            }

		    //Operator and Connector
		    Operator = (LogicalOperator) Enum.Parse(typeof (LogicalOperator), lOperator);
		    Connector = (LogicalConnector) Enum.Parse(typeof (LogicalConnector), lConnector);

		    //Right and left operand
		    var leftOperandNode = xmlNode.SelectSingleNode("Left");
		    var rightOperandNode = xmlNode.SelectSingleNode("Right");

		    if (leftOperandNode != null && leftOperandNode.HasChildNodes)
		    {
		        Left = OperandDataFactory.CreateOperandDataObject(leftOperandNode.FirstChild.Name);

		        if (Left != null)
		        {
		            Left.Load(leftOperandNode.FirstChild);
		        }
		    }

		    if (rightOperandNode != null && rightOperandNode.HasChildNodes)
		    {
		        Right = OperandDataFactory.CreateOperandDataObject(rightOperandNode.FirstChild.Name);

		        if (Right != null)
		        {
		            Right.Load(rightOperandNode.FirstChild);
		        }
		    }

		    //Children
		    var childrenNode = xmlNode.SelectSingleNode("Children");

		    if (childrenNode != null)
		    {
		        foreach (XmlNode childNode in childrenNode)
		        {

		            ExpressionData child = null;

		            if (childNode.Name == typeof (ExpressionData).Name)
		            {
		                child = new ExpressionData();
		            }
		            else if (childNode.Name == typeof (CompositeExpressionData).Name)
		            {
		                child = new CompositeExpressionData();
		            }

		            if (child != null)
		            {
		                child.Load(childNode);

		                Add(child);
		            }
		        }
		    }
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemIdMap"></param>
        /// <param name="optionPositionMap"></param>
        public override bool UpdateItemAndOptionIds(Dictionary<int, int> itemIdMap, Dictionary<int, int> optionPositionMap, Dictionary<int, int> prototypesMap)
        {
            base.UpdateItemAndOptionIds(itemIdMap, optionPositionMap, prototypesMap);

            int childrenCount = Children.Count;
            for(int i=0; i < childrenCount; i++)
            {
                var child = Children[i];
                //if child wasn't updated, then items or options can not be mapped
                if (!child.UpdateItemAndOptionIds(itemIdMap, optionPositionMap, prototypesMap))
                {
                    Children.Remove(child);
                    childrenCount = Children.Count;
                    i--;
                }
            }

            return childrenCount > 0;
        }

        /// <summary>
        /// Count of conditions
        /// </summary>
        public override int ConditionsCount
        {
            get
            {
                int s = 0;
                foreach (ExpressionData child in Children)
                    s += child.ConditionsCount;
                return s;
            }
        }
    }
}
