using System.Xml;
using Prezza.Framework.Common;
using Prezza.Framework.Configuration;
using Prezza.Framework.Logging.Sinks;

namespace Prezza.Framework.Logging.Distributor.Configuration
{
    ///<summary>
    ///</summary>
    public class DatabaseSinkData : SinkData , IXmlConfigurationBase
    {
        /// <summary>
		/// Constructor.
		/// </summary>
		public DatabaseSinkData() : this(string.Empty)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name">Name of the log sink.</param>
        public DatabaseSinkData(string name)
            : this(name, typeof(DatabaseLogSink).AssemblyQualifiedName)
        {
        }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name">Name of the log sink.</param>
		/// <param name="typeName">Type name of the log sink.</param>
        public DatabaseSinkData(string name, string typeName)
            : base(name, typeName)
		{
		}

        /// <summary>
        /// Get name of database instance to use for logging data
        /// </summary>
        public string DbInstanceName{get;private set;}

        /// <summary>
        /// Load configuration from xml
        /// </summary>
        /// <param name="node"></param>
        public void LoadFromXml(XmlNode node)
        {
            DbInstanceName = XmlUtility.GetNodeText(node.SelectSingleNode("/databaseSinkConfiguration/dbInstanceName"), true);
        }
    }
}
