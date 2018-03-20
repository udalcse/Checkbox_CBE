using System.Xml;
using Prezza.Framework.Common;
using Prezza.Framework.Configuration;
using Prezza.Framework.Logging.Sinks;

namespace Prezza.Framework.Logging.Distributor.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    public class SystemEventLogSinkData : SinkData, IXmlConfigurationBase
    {
        /// <summary>
        /// 
        /// </summary>
        public string Source { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public string Log { get; private set; }
       
        /// <summary>
		/// Constructor.
		/// </summary>
		public SystemEventLogSinkData() : this(string.Empty)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name">Name of the log sink.</param>
        public SystemEventLogSinkData(string name)
            : this(name, typeof(SystemEventLogSink).AssemblyQualifiedName)
        {
        }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name">Name of the log sink.</param>
		/// <param name="typeName">Type name of the log sink.</param>
        public SystemEventLogSinkData(string name, string typeName)
            : base(name, typeName)
		{
		}
        
        /// <summary>
        /// Load configuration from xml
        /// </summary>
        /// <param name="node"></param>
        public void LoadFromXml(XmlNode node)
        {
            Source = XmlUtility.GetNodeText(node.SelectSingleNode("/systemEventLogSinkConfiguration/source"), true);
            Log = XmlUtility.GetNodeText(node.SelectSingleNode("/systemEventLogSinkConfiguration/log"), true);
        }
    }
}
