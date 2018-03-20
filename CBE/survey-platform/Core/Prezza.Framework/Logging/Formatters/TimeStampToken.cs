//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System.Globalization;

namespace Prezza.Framework.Logging.Formatters
{
	/// <summary>
	/// Token for inserting a timestamp in a string.
	/// </summary>
	public class TimeStampToken : TokenFunction
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public TimeStampToken() : base("{timestamp(")
		{
		}

		/// <summary>
		/// Replace the token template with the timestamp in the log entry.
		/// </summary>
		/// <param name="tokenTemplate">Template to replace.</param>
		/// <param name="log">Log entry containing message to replace the token in.</param>
		/// <returns>String containing formatted text.</returns>
		public override string FormatToken(string tokenTemplate, LogEntry log)
		{
			return log.TimeStamp.ToString(tokenTemplate, CultureInfo.CurrentCulture);
		}
	}
}
