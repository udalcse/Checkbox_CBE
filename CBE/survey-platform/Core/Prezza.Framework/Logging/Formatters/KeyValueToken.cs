//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================

namespace Prezza.Framework.Logging.Formatters
{
	/// <summary>
	/// Special token function that supports a name/value pair.
	/// </summary>
	public class KeyValueToken : TokenFunction
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public KeyValueToken() : base("{keyvalue(")
		{
		}

		/// <summary>
		/// Get the value of a token to replace, where the vakue is contained in a dictionary-type structure with
		/// the token template as the key.
		/// </summary>
		/// <param name="tokenTemplate">Template for the token.</param>
		/// <param name="log">Log entry containing context about the log entry.</param>
		/// <returns>Value of the token.</returns>
		public override string FormatToken(string tokenTemplate, LogEntry log)
		{
			if(log.ExtendedProperties == null)
			{
				return "";
			}

			string propertyString = "";
			object propertyObject = log.ExtendedProperties[tokenTemplate];

			if(propertyObject != null)
			{
				propertyString = propertyObject.ToString();
			}

			return propertyString;
		}
	}
}
