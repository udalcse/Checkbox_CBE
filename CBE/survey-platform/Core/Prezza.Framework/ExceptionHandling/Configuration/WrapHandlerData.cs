//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================

using Prezza.Framework.Common;

namespace Prezza.Framework.ExceptionHandling.Configuration
{
	/// <summary>
	/// Configuration information for the wrap exception handler, which wraps an exception inside
	/// a new exception.
	/// </summary>
	public class WrapHandlerData : ExceptionHandlerData
	{
	    /// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name">Name of the wrap handler.</param>
		public WrapHandlerData(string name) : this(name, string.Empty, string.Empty, string.Empty)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name">Name of the wrap handler.</param>
		/// <param name="wrapExceptionTypeName">TypeName of the exception to create.</param>
		public WrapHandlerData(string name, string wrapExceptionTypeName) : this(name, string.Empty, wrapExceptionTypeName)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name">Name of the wrap handler.</param>
		/// <param name="exceptionMessage">Message for the exception to create.</param>
		/// <param name="wrapExceptionTypeName">TypeName of the exception to create to wrap the thrown exception.</param>
		public WrapHandlerData(string name, string exceptionMessage, string wrapExceptionTypeName) : this(name, exceptionMessage, wrapExceptionTypeName, string.Empty)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name">Name of the wrap handler.</param>
		/// <param name="exceptionMessage">Message for the exception to create.</param>
		/// <param name="wrapExceptionTypeName">TypeName of the exception to create to wrap the thrown exception.</param>
		/// <param name="wrapHandlerTypeName">TypeName of the exception to create.</param>
		public WrapHandlerData(string name, string exceptionMessage, string wrapExceptionTypeName, string wrapHandlerTypeName) : base(name, wrapHandlerTypeName)
		{
			ExceptionMessage = exceptionMessage;
			WrapExceptionTypeName = wrapExceptionTypeName;
		}

	    /// <summary>
	    /// Get/Set message for the exception to create.
	    /// </summary>
	    public string ExceptionMessage { get; set; }

	    /// <summary>
	    /// Get/Set the TypeName of the wrapper exception to create.
	    /// </summary>
	    public string WrapExceptionTypeName { get; set; }

	    #region IXmlConfigurationBase Members

		/// <summary>
		/// Load wrap exception handler configuration from the specified Xml node.
		/// </summary>
		/// <param name="node">Xml node containing wrap handler configuration.</param>
		public override void LoadFromXml(System.Xml.XmlNode node)
		{
			TypeName = XmlUtility.GetNodeText(node.SelectSingleNode("/wrapExceptionHandlerConfiguration/handlerType") , true);
			WrapExceptionTypeName = XmlUtility.GetNodeText(node.SelectSingleNode("/wrapExceptionHandlerConfiguration/exceptionTypeName"), true);
			ExceptionMessage = XmlUtility.GetNodeText(node.SelectSingleNode("/wrapExceptionHandlerConfiguration/exceptionMessage"), true);
		}

		#endregion
	}
}
