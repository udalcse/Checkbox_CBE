//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System.Text;
using System.Collections;

namespace Prezza.Framework.Logging.Formatters
{
	/// <summary>
	/// Formatter for dictionary containing one or more name/value pairs.
	/// </summary>
	public class DictionaryToken : TokenFunction
	{
		/// <summary>
		/// Token to identify the position of dictionary key in the log message.
		/// </summary>
		private const string DictionaryKeyToken = "{key}";

		/// <summary>
		/// Token to identify the position of the dictionary value in the log message.
		/// </summary>
		private const string DictionaryValueToken = "{value}";

		/// <summary>
		/// Constructor.
		/// </summary>
		public DictionaryToken() : base("{dictionary(")
		{
		}

		/// <summary>
		/// Replace any dictionary item tokens found in the log entry with their values.  The items are stored in the 
		/// Extended properties property of the log entry.
		/// </summary>
		/// <param name="tokenTemplate">Template for the token replacement, e.g. "{key} {value}" or "{datetime}"</param>
		/// <param name="log">Log entry containing the message to replace any tokens in.</param>
		/// <returns>String containing result of token formatting.</returns>
		public override string FormatToken(string tokenTemplate, LogEntry log)
		{
			if(log.ExtendedProperties == null)
			{
				return "";
			}

			StringBuilder dictionaryBuilder = new StringBuilder();

			foreach(DictionaryEntry entry in log.ExtendedProperties)
			{
				StringBuilder singlePair = new StringBuilder(tokenTemplate);
				string keyName = "";

				if(entry.Key != null)
				{
					keyName = entry.Key.ToString();
				}

				singlePair.Replace(DictionaryKeyToken, keyName);

				string keyValue = "";

				if(entry.Value != null)
				{
					keyValue = entry.Value.ToString();
				}

				singlePair.Replace(DictionaryValueToken, keyValue);

				dictionaryBuilder.Append(singlePair.ToString());
			}

			return dictionaryBuilder.ToString();
		}
	}
}
