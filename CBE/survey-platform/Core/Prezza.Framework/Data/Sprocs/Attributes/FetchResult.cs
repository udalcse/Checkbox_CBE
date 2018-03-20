using System;

namespace Prezza.Framework.Data.Sprocs
{
    /// <summary>
    /// Mark a field as accepting a value from a fetch operation
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class FetchResult : Attribute
    {
        private readonly string _sourceFieldName;

        /// <summary>
        /// Construct a new fetch result with the source field
        /// </summary>
        /// <param name="sourceFieldName"></param>
        public FetchResult(string sourceFieldName)
        {
            _sourceFieldName = sourceFieldName;
        }

        /// <summary>
        /// Get the name of the source field
        /// </summary>
        public string SourceField
        {
            get { return _sourceFieldName; }
        }
    }
}
