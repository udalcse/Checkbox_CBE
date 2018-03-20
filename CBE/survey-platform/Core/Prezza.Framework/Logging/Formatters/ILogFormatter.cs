//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using Prezza.Framework.Configuration;

namespace Prezza.Framework.Logging.Formatters
{
	/// <summary>
	/// Log formatter interface.  Any custom formatter used by the logging framework
	/// must implement this interface.
	/// </summary>
	public interface ILogFormatter : IConfigurationProvider
	{
		/// <summary>
		/// Return a string containing a formatted log entry.
		/// </summary>
		/// <param name="log">Log entry containing message to format and other information about a particular log entry.</param>
		/// <returns>String containing the formatted log entry.</returns>
		string Format(LogEntry log);
	}
}
