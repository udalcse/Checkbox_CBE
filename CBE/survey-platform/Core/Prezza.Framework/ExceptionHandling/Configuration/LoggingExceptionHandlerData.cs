//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================

namespace Prezza.Framework.ExceptionHandling.Configuration
{
	/// <summary>
	/// Exception handler that logs the exception information to a file.
	/// </summary>
	public class LoggingExceptionHandlerData : ExceptionHandlerData
	{
	    /// <summary>
		/// Constructor.
		/// </summary>
		public LoggingExceptionHandlerData()
	    {
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name">Name of the exception handler.</param>
		public LoggingExceptionHandlerData(string name) : base(name, string.Empty)
		{
		}
		
		/// <summary>
		/// Get the TypeName of the exception handler.
		/// </summary>
		public override string TypeName
		{
			get{return typeof(LoggingExceptionHandler).AssemblyQualifiedName;}
		}
	}
}
