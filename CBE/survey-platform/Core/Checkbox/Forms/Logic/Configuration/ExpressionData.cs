using System;
using System.Text;
using System.Data;
using System.Xml;
using Prezza.Framework.Common;
using Prezza.Framework.Data;

using Checkbox.Globalization.Text;
using System.Collections.Generic;

namespace Checkbox.Forms.Logic.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class ExpressionData
    {
         /// <summary>
        /// 
        /// </summary>
        public virtual CompositeExpressionData Parent { get; set; }

        /// <summary>
        /// Get/set parent expression
        /// </summary>
        public int? ParentExpressionId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public OperandData Left { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public OperandData Right { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public LogicalOperator Operator { get; protected set; }

        /// <summary>
        /// Get id of expression
        /// </summary>
        public int? ExpressionId{get;protected set;}

        /// <summary>
        /// 
        /// </summary>
        public ExpressionData()
            : this(null, LogicalOperator.OperatorNotSpecified, null)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="operation"></param>
        /// <param name="right"></param>
        public ExpressionData(OperandData left, LogicalOperator operation, OperandData right)
        {
            Operator = operation;
            Left = left;
            Right = right;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual string ToString(string languageCode)
        {
            var s = new StringBuilder();
            s.Append("'");
            s.Append(Left.ToString(languageCode));
            s.Append("' ");
            s.Append(TextManager.GetText("/enum/logicalOperator/" + Operator, TextManager.DefaultLanguage));
            if (Operator != LogicalOperator.Answered && Operator != LogicalOperator.NotAnswered)
            {
                s.Append(" '");
                s.Append(Right.ToString(languageCode));
            }
            s.Append("'");
            return s.ToString();
        }

        /// <summary>
        /// Get the expression operator
        /// </summary>
        /// <returns></returns>
        public string OperatorText
        {
            get
            {
                return TextManager.GetText("/enum/logicalOperator/" + Operator, TextManager.DefaultLanguage);
            }
        }

        /// <summary>
        /// Get the right operand text
        /// </summary>
        /// <returns></returns>
        public string RightOperandText(string languageCode)
        {
            return Right != null ? Right.ToString(languageCode) : string.Empty;
        }

        /// <summary>
        /// Get the right operand text
        /// </summary>
        /// <returns></returns>
        public string LeftOperandText(string languageCode)
        {
            return Left != null ? Left.ToString(languageCode) : string.Empty;
        }


        #region Rule Data Update

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ruleData"></param>
        /// <returns></returns>
        protected virtual int InsertExpression(RulesObjectSet ruleData)
        {
            return ruleData.InsertVoidExpression().Id;
        }

        /// <summary>
        /// Update the specified rule data set with updated rule data
        /// </summary>
        /// <param name="ruleData"></param>
        public virtual void UpdateRuleData(RulesObjectSet ruleData)
        {
            if(!ExpressionId.HasValue)
            {
                ExpressionId = InsertExpression(ruleData);
            }

            var expression = ruleData.GetExpression(ExpressionId.Value);

            //If no rows found, something wrong
            if (expression == null)
            {
                throw new Exception("CompositionExpression inserted, but has no valus.");
            }
			//Save operands
            if (Left != null)
            {
                Left.UpdateRuleData(ruleData);
            }

            if (Right != null)
            {
                Right.UpdateRuleData(ruleData);
            }

            //Update expression row
            UpdateExpression(expression, ruleData);
        }

        /// <summary>
        /// Update the expression data row with current values
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="ruleData"></param>
        protected virtual void UpdateExpression(ExpressionEntity expression, RulesObjectSet ruleData)
        {
            expression.Operator = (int)Operator;
            expression.LeftOperand = Left != null && Left.OperandId.HasValue ? ruleData.GetOperand(Left.OperandId.Value) : null;
            expression.RightOperand = Right != null && Right.OperandId.HasValue ? ruleData.GetOperand(Right.OperandId.Value) : null;
            expression.Parent = ParentExpressionId.HasValue ? ruleData.GetExpression(ParentExpressionId.Value) : null;
            expression.ParentId = ParentExpressionId;

            var lineage = new StringBuilder();
            lineage.Append("/");
            CompositeExpressionData parent = Parent;
            CompositeExpressionData root = null;
            int depth = 0;

            while (parent != null)
            {
                depth++;
                lineage.Insert(0, "/" + parent.ExpressionId);
                if (parent.Parent == null)
                {
                    // found the root expression
                    root = parent;
                }
                parent = parent.Parent;  // up a level
            }

            if (Parent != null)
                expression.Parent = Parent.ExpressionId.HasValue ? ruleData.GetExpression(Parent.ExpressionId.Value) : null;

            expression.Depth = depth;
            expression.Lineage = lineage.ToString();

            if (root != null)
                expression.Root = root.ExpressionId.HasValue ? ruleData.GetExpression(root.ExpressionId.Value) : null;
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
		/// <param name="expressionId"></param>
		/// <param name="rulesData"></param>
		public virtual bool Load(int expressionId, RulesObjectSet rulesData)
        {
            ExpressionId = expressionId;

            var expression = rulesData.GetExpression(expressionId);

            if (expression == null)
            {
                throw new Exception(string.Format("Expression data set did not contain a row for the ExpressionData [{0}].", expressionId));
            }

            var operatorEnum = expression.Operator;
            var leftOperand = expression.LeftOperand;
            var rightOperand = expression.RightOperand;

            if (leftOperand == null || rightOperand == null)
                return false;

            ParentExpressionId = expression.Parent == null ? default(int?) : expression.Parent.Id;
            Operator = (LogicalOperator)Enum.Parse(typeof(LogicalOperator), operatorEnum.ToString());

            //Load operands
            string rightOperandType = string.Format( "{0},{1}", rightOperand.TypeName, rightOperand.AssemblyName);
            string leftOperandType = string.Format("{0},{1}", leftOperand.TypeName, leftOperand.AssemblyName);
            
            var operandFactory = new OperandDataFactory();

            Right = operandFactory.CreateOperandData(rightOperandType);
            Left = operandFactory.CreateOperandData(leftOperandType);

            Right.Load(rightOperand.Id, rulesData);
            Left.Load(leftOperand.Id, rulesData);

            return true;
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="writer"></param>
		public virtual void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement(GetType().Name);
			writer.WriteAttributeString("Operator", Operator.ToString());

			writer.WriteStartElement("Left");
			if (Left != null) Left.WriteXml(writer);
			//else writer.WriteNull();
			writer.WriteEndElement();

			writer.WriteStartElement("Right");
			if(Right != null) Right.WriteXml(writer);
			//else writer.WriteNull();
			writer.WriteEndElement();

			writer.WriteEndElement();
		}

		/// <summary>
		/// 
		/// </summary>
        /// <param name="expressionNode"></param>
		public virtual void Load(XmlNode expressionNode)
		{
		    Operator = (LogicalOperator) XmlUtility.GetAttributeEnum(expressionNode, "Operator", typeof (LogicalOperator));

		    var leftExpressionNode = expressionNode.SelectSingleNode("Left");

			if(leftExpressionNode != null && leftExpressionNode.HasChildNodes)
			{
			    var operandNode = leftExpressionNode.FirstChild;
                
                Left = OperandDataFactory.CreateOperandDataObject(operandNode.Name);

                if (Left != null)
                {
                    Left.Load(operandNode);
                }
			}
            
            var rightExpressionNode = expressionNode.SelectSingleNode("Right");

            if (rightExpressionNode != null && rightExpressionNode.HasChildNodes)
            {
                var operandNode = rightExpressionNode.FirstChild;

				Right = OperandDataFactory.CreateOperandDataObject(operandNode.Name);

                if (Right != null)
                {
                    Right.Load(operandNode);
                }
            }
		}

        /// <summary>
        /// Validate expression
        /// </summary>
        /// <returns></returns>
        public virtual bool Validate()
        {
            return
                Right != null
                && Left != null
                && Right.Validate() 
                && Left.Validate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemIdMap"></param>
        /// <param name="optionPositionMap"></param>
        public virtual bool UpdateItemAndOptionIds(Dictionary<int, int> itemIdMap, Dictionary<int, int> optionPositionMap, Dictionary<int, int> prototypesMap)
        {
            if (Left != null && !Left.UpdateItemAndOptionIds(itemIdMap, optionPositionMap, prototypesMap))
            {
                return false;
            }

            if (Right != null && !Right.UpdateItemAndOptionIds(itemIdMap, optionPositionMap, prototypesMap))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Count of conditions
        /// </summary>
        public virtual int ConditionsCount
        {
            get
            {
                return 1;
            }
        }
	}
}
