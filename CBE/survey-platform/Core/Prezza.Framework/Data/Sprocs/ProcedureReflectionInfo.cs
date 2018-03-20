using System;
using System.Collections.Generic;
using System.Text;

namespace Prezza.Framework.Data.Sprocs
{
    /// <summary>
    /// Store for procedure information gleaned through reflection
    /// </summary>
    internal class ProcedureReflectionInfo
    {
        private Dictionary<string, ParameterReflectionInfo> _parameters;
        private string _name;

        /// <summary>
        /// Constructor -- Intializes parameter dictionary
        /// </summary>
        public ProcedureReflectionInfo()
        {
            _parameters = new Dictionary<string, ParameterReflectionInfo>();
        }

        /// <summary>
        /// Get/set the procedure name
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Get the parameters
        /// </summary>
        public Dictionary<string, ParameterReflectionInfo> Parameters
        {
            get { return _parameters; }
        }

        /// <summary>
        /// Add a parameter to the collection
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="info"></param>
        public void AddParameter(string parameterName, ParameterReflectionInfo info)
        {
            _parameters[parameterName] = info;
        }
    }
}
