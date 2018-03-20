using System;
using System.Data;

namespace Prezza.Framework.Data.Sprocs
{
    /// <summary>
    /// Stored procedure parameter
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public abstract class SprocParameter : Attribute
    {
        ///<summary>
        ///</summary>
        protected SprocParameter()
        {
            ConvertDBNullToNull = true;
        }

        /// <summary>
        /// Get set parameter size
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// Get/set whether null should be set
        /// </summary>
        public bool AllowNull { get; set; }

        /// <summary>
        /// Get/set parameter name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Get/set db type
        /// </summary>
        public DbType DbType { get; set; }

        /// <summary>
        /// Parameter direction
        /// </summary>
        public ParameterDirection Direction { get; set; }

        /// <summary>
        /// Get/set whether to convert DBNullToNull for output parameters.  Default is true.
        /// </summary>
        public bool ConvertDBNullToNull { get; set; }
    }
}
