using System.Data;
using System.Reflection;
using System.Collections.Generic;

using Prezza.Framework.Common;

namespace Prezza.Framework.Data.Sprocs
{
    /// <summary>
    /// Container
    /// </summary>
    internal class ParameterReflectionInfo
    {
        private int _size;
        private string _name;
        private DbType _dbType;
        private PropertyInfo _property;
        private ParameterDirection _direction;
        private bool _convertDBNullToNull;
        private bool _allowNull;

        /// <summary>
        /// Construct a new parameter info
        /// </summary>
        /// <param name="size">Size of the parameter's data.</param>
        /// <param name="name">Name of the parameter.</param>
        /// <param name="dbType">DBType of the parameter.</param>
        /// <param name="direction">Parameter direction.</param>
        /// <param name="convertDBNullToNull">Specify DBNull should be converted to null for output.</param>
        /// <param name="allowNull">Specify whether a null value is allowed for the parameter.</param>
        /// <param name="property">Associated property.</param>
        public ParameterReflectionInfo(int size, string name, DbType dbType, ParameterDirection direction, bool convertDBNullToNull, bool allowNull, PropertyInfo property)
        {
            _size = size;
            _name = name;
            _direction = direction;
            _dbType = dbType;
            _property = property;
            _convertDBNullToNull = convertDBNullToNull;
            _allowNull = allowNull;
        }

        /// <summary>
        /// Get/set whether null allowed
        /// </summary>
        public bool AllowNull
        {
            get { return _allowNull; }
        }

        /// <summary>
        /// Get parameter data size
        /// </summary>
        public int Size
        {
            get { return _size; }
        }

        /// <summary>
        /// Get parameter name
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Get the parameter direction
        /// </summary>
        public ParameterDirection Direction
        {
            get { return _direction; }
        }

        /// <summary>
        /// Get the DB Type
        /// </summary>
        public DbType DbType
        {
            get { return _dbType; }
        }

        /// <summary>
        /// Get the field info
        /// </summary>
        public PropertyInfo Property
        {
            get { return _property; }
        }

        /// <summary>
        /// Get/set whether to convert DBNull to null for out parameters or return values.
        /// </summary>
        public bool ConvertDBNullToNull
        {
            get { return _convertDBNullToNull; }
            set { _convertDBNullToNull = value; }
        }
    }
}
