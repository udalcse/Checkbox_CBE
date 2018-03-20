using System;

namespace Checkbox.Common
{
    ///<summary>
    ///</summary>
    public enum EntityState
    {
        Normal,
        Added,
        Deleted,
        Updated,

        //Use it to detect unchecked checkbox options. 
        //They could be reset to their default condition, if no answer was selected.
        //CHECKBOX-3322
        Empty 
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
}
