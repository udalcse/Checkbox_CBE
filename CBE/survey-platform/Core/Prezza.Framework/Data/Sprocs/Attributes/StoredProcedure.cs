using System;

namespace Prezza.Framework.Data.Sprocs
{
    /// <summary>
    /// Stored procedure attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public abstract class StoredProcedure : Attribute
    {
        private readonly string _name;

        /// <summary>
        /// Constructor that accepts the name
        /// </summary>
        /// <param name="name"></param>
        public StoredProcedure(string name)
        {
            _name = name;
        }

        /// <summary>
        /// Get the stored procedure name
        /// </summary>
        public string Name
        {
            get { return _name; }
        }
    }
}
