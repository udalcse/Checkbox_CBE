//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================

using System.Xml;
using Prezza.Framework.Common;
using Prezza.Framework.Configuration;
using Prezza.Framework.Logging;

namespace Prezza.Framework.ExceptionHandling.Configuration
{
	/// <summary>
	/// Base class for exception handling configuration information.
	/// </summary>
	public abstract class ExceptionHandlerData : ProviderData, IXmlConfigurationBase
	{

        /// <summary>
        /// Get/Set the default category for entries written by the handler.
        /// </summary>
        public string DefaultLogCategory { get; set; }

        /// <summary>
        /// Get/Set the default event Id for entries written by the handler.
        /// </summary>
        public int DefaultEventId { get; set; }

        /// <summary>
        /// Get/Set the default severity for entries written by the handler.
        /// </summary>
        public Severity DefaultSeverity { get; set; }

        /// <summary>
        /// Get/Set the default title for entries written by the handler.
        /// </summary>
        public string DefaultTitle { get; set; }

        /// <summary>
        /// Get/Set the TypeName of the log formatter to use when writing entries.
        /// </summary>
        public string FormatterTypeName { get; set; }

        /// <summary>
        /// The minimum priority to set when writing entries.
        /// </summary>
        public int MinimumPriority { get; set; }

		/// <summary>
		/// Constructor.
		/// </summary>
		protected ExceptionHandlerData() : this(string.Empty, string.Empty)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name">Name of the exception handler.</param>
		/// <param name="typeName">TypeName of the exception handler class.</param>
		protected ExceptionHandlerData(string name, string typeName) : base(name, typeName)
		{
		}

        #region IXmlConfigurationBase Members

        /// <summary>
        /// Load configuration for the logging exception handler from the specified Xml node.
        /// </summary>
        /// <param name="node">Xml node containing configuration information for the exception handler.</param>
        public virtual void LoadFromXml(XmlNode node)
        {
            TypeName = XmlUtility.GetNodeText(node.SelectSingleNode("/loggingExceptionHandlerConfiguration/handlerType"), true);
            DefaultLogCategory = XmlUtility.GetNodeText(node.SelectSingleNode("/loggingExceptionHandlerConfiguration/defaultCategory"));
            DefaultEventId = XmlUtility.GetNodeInt(node.SelectSingleNode("/loggingExceptionHandlerConfiguration/defaultEventId")) ?? 0;
            DefaultSeverity = (Severity)XmlUtility.GetNodeEnum(node.SelectSingleNode("/loggingExceptionHandlerConfiguration/defaultSeverity"), typeof(Severity));
            DefaultTitle = XmlUtility.GetNodeText(node.SelectSingleNode("/loggingExceptionHandlerConfiguration/defaultTitle"));
            FormatterTypeName = XmlUtility.GetNodeText(node.SelectSingleNode("/loggingExceptionHandlerConfiguration/formatterTypeName"));
            MinimumPriority = XmlUtility.GetNodeInt(node.SelectSingleNode("/loggingExceptionHandlerConfiguration/minimumPriority")) ?? 0;
        }

        #endregion
	}
}
