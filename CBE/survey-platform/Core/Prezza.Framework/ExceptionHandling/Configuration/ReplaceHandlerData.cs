//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System.Xml;
using Prezza.Framework.Common;

namespace Prezza.Framework.ExceptionHandling.Configuration
{
	/// <summary>
	/// Configuration information for the replace handler, which replaces one exception with another.
	/// </summary>
	public class ReplaceHandlerData : ExceptionHandlerData
	{
	    /// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name">Name of the replace handler.</param>
		public ReplaceHandlerData(string name) : this(name, string.Empty, string.Empty, string.Empty)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name">Name of the replace handler.</param>
		/// <param name="replaceExceptionTypeName">TypeName of the new exception to create.</param>
		public ReplaceHandlerData(string name, string replaceExceptionTypeName) : this(name, string.Empty, replaceExceptionTypeName)
		{
		}


		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name">Name of the replace handler.</param>
		/// <param name="exceptionMessage">Message for the new exception.</param>
		/// <param name="replaceExceptionTypeName">TypeName of the new exception to create.</param>
		public ReplaceHandlerData(string name, string exceptionMessage, string replaceExceptionTypeName) : this(name, exceptionMessage, replaceExceptionTypeName, string.Empty)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name">Name of the replace handler.</param>
		/// <param name="exceptionMessage">Message for the new exception.</param>
		/// <param name="replaceExceptionTypeName">TypeName of the exception to create.</param>
		/// <param name="replaceHandlerTypeName">TypeName of the replace handler.</param>
		public ReplaceHandlerData(string name, string exceptionMessage, string replaceExceptionTypeName, string replaceHandlerTypeName) : base(name, replaceHandlerTypeName)
		{
			ExceptionMessage = exceptionMessage;
			ReplaceExceptionTypeName = replaceExceptionTypeName;
		}

	    /// <summary>
	    /// Message for the created exception.
	    /// </summary>
	    public string ExceptionMessage { get; set; }

	    /// <summary>
	    /// TypeName [CLASS],[ASSEMBLY] of the exception to create.
	    /// </summary>
	    public string ReplaceExceptionTypeName { get; set; }

	    #region IXmlConfigurationBase Members

		/// <summary>
		/// Load replace handler configuration from the specified Xml node.
		/// </summary>
		/// <param name="node">Xml node containing configuration information for the replace handler.</param>
		public override void LoadFromXml(XmlNode node)
		{
			TypeName = XmlUtility.GetNodeText(node.SelectSingleNode("/replaceExceptionHandlerConfiguration/handlerType"), true);
			ReplaceExceptionTypeName = XmlUtility.GetNodeText(node.SelectSingleNode("/replaceExceptionHandlerConfiguration/exceptionTypeName"), true);
			ExceptionMessage = XmlUtility.GetNodeText(node.SelectSingleNode("/replaceExceptionHandlerConfiguration/exceptionMessage"), true);
		}

		#endregion
	}
}
