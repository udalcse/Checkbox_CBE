//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System;
using System.Text;

namespace Prezza.Framework.Logging.Formatters
{
	/// <summary>
	/// Base class for token formatting/replacement functionality for loggging.
	/// </summary>
	public abstract class TokenFunction
	{
		/// <summary>
		/// Start delimiter of a token.
		/// </summary>
		private string startDelimiter = string.Empty;

		/// <summary>
		/// End delimiter of a token.
		/// </summary>
		private string endDelimiter = ")}";

		/// <summary>
		/// Constructor.  Assumes the token end delimiter will be a default value.
		/// </summary>
		/// <param name="tokenStartDelimiter">Token start delimiter.</param>
		protected TokenFunction(string tokenStartDelimiter)
		{
			if(tokenStartDelimiter == null || tokenStartDelimiter.Length == 0)
			{
				throw new ArgumentNullException("tokenStartDelimiter");
			}

			startDelimiter = tokenStartDelimiter;

		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="tokenStartDelimiter">Start delimiter for the token.</param>
		/// <param name="tokenEndDelimiter">End delimiter for the token.</param>
		protected TokenFunction(string tokenStartDelimiter, string tokenEndDelimiter)
		{
			if(tokenStartDelimiter == null || tokenStartDelimiter.Length == 0)
			{
				throw new ArgumentNullException("tokenStartDelimiter");
			}

			if(tokenEndDelimiter == null || tokenEndDelimiter.Length == 0)
			{
				throw new ArgumentNullException("tokenEndDelimiter");
			}

			startDelimiter = tokenStartDelimiter;
			endDelimiter = tokenEndDelimiter;
		}

		/// <summary>
		/// Update a string to replace tokens with their values.
		/// </summary>
		/// <param name="messageBuilder"><see cref="System.Text.StringBuilder"/> containing string with tokens.</param>
		/// <param name="log">Log entry that will contain the formatted message.  The information in the object may be used by
		/// the formatting functions.</param>
		public virtual void Format(StringBuilder messageBuilder, LogEntry log)
		{
			if(startDelimiter.Length == 0)
			{
				return;
			}

			int pos = 0;

			while(pos < messageBuilder.Length)
			{
				string messageString = messageBuilder.ToString();

				if(messageString.IndexOf(startDelimiter) == -1)
				{
					break;
				}

                string tokenTemplate = GetInnerTemplate(pos, messageString);
				string tokenToReplace = startDelimiter + tokenTemplate + endDelimiter;

				pos = messageBuilder.ToString().IndexOf(tokenToReplace);

				string tokenValue = FormatToken(tokenTemplate, log);

				messageBuilder.Replace(tokenToReplace, tokenValue);
			}
		}

		/// <summary>
		/// Format the token string.
		/// </summary>
		/// <param name="tokenTemplate">Token to replace.</param>
		/// <param name="log">Log entry that will contain the message.  Passed in case the method
		/// requires an information about the log entry.</param>
		/// <returns></returns>
		public abstract string FormatToken(string tokenTemplate, LogEntry log);

		/// <summary>
		/// Get the next token string without its start and end delimiters.
		/// </summary>
		/// <param name="startPos">Position to start searching for the token from.</param>
		/// <param name="message">String containing the tokens.</param>
		/// <returns>Token string without the start and end delimiters.</returns>
		protected virtual string GetInnerTemplate(int startPos, string message)
		{
			int tokenStartPos = message.IndexOf(startDelimiter, startPos) + startDelimiter.Length;
			int endPos = message.IndexOf(endDelimiter, tokenStartPos);
			return message.Substring(tokenStartPos, endPos - tokenStartPos);
		}
	}
}
