using System;
using System.Collections.Generic;

namespace Checkbox.Forms.Logic
{
    ///<summary>
    ///</summary>
    public enum EntityState
    {
        Normal,
        Added,
        Deleted,
        Updated
    }
    
    ///<summary>
    ///</summary>
    [Serializable]
    public abstract class EntityBase
    {
        protected static int _id;

        public int Id { set; get; }
        public EntityState State { protected set; get; }

        protected EntityBase()
        {
            State = EntityState.Normal;
        }

        public void Update()
        {
            if (State != EntityState.Deleted && State != EntityState.Added)
                State = EntityState.Updated;
        }

        public virtual void Delete()
        {
            State = EntityState.Deleted;
        }

        public void ResetState()
        {
            State = EntityState.Normal;
        }

        public static T Create<T>() where T : EntityBase
        {
            var instance = (T)Activator.CreateInstance(typeof(T), --_id);
            instance.State = EntityState.Added;
            return instance;
        }
    }

    ///<summary>
    ///</summary>
    [Serializable]
    public class RuleEntity : EntityBase
    {
        private ActionEntity _action;
        private ExpressionEntity _expression;
        private string _eventTrigger;

        public RuleEntity(int id)
        {
            Id = id;
        }

        public ActionEntity Action
        {
            set 
            {
                if (_action != null && (value == null || _action.Id != value.Id))
                    _action.Delete();

                _action = value;
                
                if (_action != null)
                    _action.ParentRuleId = Id;

                Update(); 
            }
            get { return _action; }
        }

        public ExpressionEntity Expression
        {
            set 
            {
                if (_expression != null && (value == null || _expression.Id != value.Id))
                    _expression.Delete();
                
                _expression = value;
                Update(); 
            }
            get { return _expression; }
        }

        public string EventTrigger
        {
            set { _eventTrigger = value; Update(); }
            get { return _eventTrigger; }
        }

        public override void Delete()
        {
            base.Delete();
            if (Expression != null)
                Expression.Delete();
            if (Action != null)
                Action.Delete();
        }
    }

    ///<summary>
    ///</summary>
    [Serializable]
    public class ItemEntity : EntityBase
    {
        private RuleEntity _rule;

        public RuleEntity Rule
        {
            set
            {
                if (_rule != null)
                {
                    if (_rule.Id > 0)
                        DeletedRule = _rule.Id;

                    _rule.Delete();
                }

                _rule = value; 
                Update();
            }
            get { return _rule; }
        }

        public int? DeletedRule { set; get; }

        public int PageID { get; set; }

        public ItemEntity(int id)
        {
            Id = id;
        }
    }
    
    ///<summary>
    ///</summary>
    [Serializable]
    public class PageEntity : EntityBase
    {
        public IList<RuleEntity> Rules { set; get; }
        public IList<int> RemovedRules { set; get; }

        public PageEntity(int id)
        {
            Id = id;
            Rules = new List<RuleEntity>();
            RemovedRules = new List<int>();
        }
   }

    ///<summary>
    ///</summary>
    [Serializable]
    public class ExpressionEntity : EntityBase
    {
        private int _operator;
        private OperandEntity _leftOperand;
        private OperandEntity _rightOperand;
        private ExpressionEntity _parent;
        private int _depth;
        private string _lineage;
        private ExpressionEntity _root;
        private string _childRelation;

        public ExpressionEntity(int id)
        {
            Id = id;
        }

        public int Operator
        {
            set { _operator = value; Update(); }
            get { return _operator; }
        }

        public OperandEntity LeftOperand
        {
            set
            {
                if (_leftOperand != null && (value == null || _leftOperand.Id != value.Id))
                    _leftOperand.Delete();

                _leftOperand = value; 
                Update();
            }
            get { return _leftOperand; }
        }

        public OperandEntity RightOperand
        {
            set
            {
                if (_rightOperand != null && (value == null || _rightOperand.Id != value.Id))
                    _rightOperand.Delete();

                _rightOperand = value; 
                Update();
            }
            get { return _rightOperand; }
        }

        public ExpressionEntity Parent
        {
            set { _parent = value; Update(); }
            get { return _parent; }
        }

        public int Depth
        {
            set { _depth = value; Update(); }
            get { return _depth; }
        }

        public string Lineage
        {
            set { _lineage = value; Update(); }
            get { return _lineage; }
        }

        public ExpressionEntity Root
        {
            set { _root = value; Update(); }
            get { return _root; }
        }

        public string ChildRelation
        {
            set { _childRelation = value; Update(); }
            get { return _childRelation; }
        }

        public int? ParentId { set; get; }
        public int? RootId { set; get; }
    }

    ///<summary>
    ///</summary>
    [Serializable]
    public class OperandEntity : EntityBase
    {
        public OperandEntity(int id, string typeName, string assembly)
        {
            Id = id;
            TypeName = typeName;
            AssemblyName = assembly;
        }

        public static OperandEntity Create(Type type)
        {
            return new OperandEntity(--_id, type.FullName, type.Assembly.GetName().Name)
                              {State = EntityState.Added};
        }

        private string _typeName;
        public string TypeName
        {
            private set { _typeName = value; }
            get { return _typeName; }
        }

        private string _assemblyName;
        public string AssemblyName
        {
            private set { _assemblyName = value; }
            get { return _assemblyName; }
        }

        //item data
        private int? _itemId;
        public int? ItemId
        {
            set { _itemId = value; Update(); }
            get { return _itemId; }
        }

        private int? _parentItemId;
        public int? ParentItemId
        {
            set { _parentItemId = value; Update(); }
            get { return _parentItemId; }
        }

        //matrix data
        private int? _columnNumber;
        public int? ColumnNumber
        {
            set { _columnNumber = value; Update(); }
            get { return _columnNumber; }
        }

        private string _category;
        public string Category
        {
            set { _category = value; Update(); }
            get { return _category; }
        }

        //answer data
        private int? _optionId;
        public int? OptionId
        {
            set { _optionId = value; Update(); }
            get { return _optionId; }
        }

        private string _answerValue;
        public string AnswerValue
        {
            set { _answerValue = value; Update(); }
            get { return _answerValue; }
        }

        //profile data
        private string _profileKey;
        public string ProfileKey
        {
            set { _profileKey = value; Update(); }
            get { return _profileKey; }
        }

        //response data
        private string _responseKey;
        public string ResponseKey
        {
            set { _responseKey = value; Update(); }
            get { return _responseKey; }
        }
    }

    ///<summary>
    ///</summary>
    [Serializable]
    public class ActionEntity : EntityBase
    {
        private string _typeName;
        private string _assemblyName;
        private int? _goToPageId;

        public ActionEntity(int id, string typeName, string assemblyName)
        {
            Id = id;
            TypeName = typeName;
            AssemblyName = assemblyName;
        }

        public int? ParentRuleId { set; get; }

        public static ActionEntity Create(Type type)
        {
            return new ActionEntity(--_id, type.FullName, type.Assembly.GetName().Name) { State = EntityState.Added };
        }

        public string TypeName
        {
            private set { _typeName = value; Update(); }
            get { return _typeName; }
        }

        public string AssemblyName
        {
            private set { _assemblyName = value; Update(); }
            get { return _assemblyName; }
        }

        public int? GoToPageId
        {
            set { _goToPageId = value; Update(); }
            get { return _goToPageId; }
        }
    }
}
