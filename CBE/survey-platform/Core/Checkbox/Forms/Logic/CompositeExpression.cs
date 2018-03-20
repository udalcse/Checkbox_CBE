using System;
using System.Collections.Generic;

namespace Checkbox.Forms.Logic
{
    /// <summary>
    /// Respresents the Composite of Expressions used to create Expression trees.
    /// </summary>
    [Serializable]
    public class CompositeExpression : Expression
    {
        private readonly List<Expression> _children;
        private readonly LogicalConnector _connector;

        /// <summary>
        /// Constructor.  
        /// </summary>
        /// <param name="children">a List of child Expressions and CompositeExpressions</param>
        /// <param name="connector">the <see cref="LogicalConnector"/> relationship among children</param>
        public CompositeExpression(List<Expression> children, LogicalConnector connector)
            : base(null, null, LogicalOperator.OperatorNotSpecified)
        {
            _children = children;
            _connector = connector;
        }

        /// <summary>
        /// Gets a value indicating the logical relationship between all children of this Expression
        /// </summary>
        public LogicalConnector Connector
        {
            get { return _connector; }
        }

        /// <summary>
        /// Overridden.  Evaluates each child Expression in turn and returns the output of the child Expression's Evaluate()
        /// method across all children related via the ChildConnector (AND, OR)
        /// </summary>
        /// <returns>true or false</returns>
        internal override bool Evaluate(Response response)
        {
            bool result = false;

            foreach (Expression child in _children)
            {
                result = child.Evaluate(response);

                if (result)
                {
                    if (Connector == LogicalConnector.AND)
                        continue;
                    break;
                }
                
                if (Connector == LogicalConnector.OR)
                    continue;
                
                break;
            }
            return result;
        }

        /// <summary>
        /// Adds a child <see cref="Expression"/> to this CompositeExpression
        /// </summary>
        /// <param name="child">the child Expression</param>
        internal void Add(Expression child)
        {
            child.Parent = this;
            _children.Add(child);
        }

        internal void Remove(Expression child)
        {
            _children.Remove(child);
        }

        /// <summary>
        /// 
        /// </summary>
        public List<Expression> Children
        {
            get { return _children; }
        }
    }
}
