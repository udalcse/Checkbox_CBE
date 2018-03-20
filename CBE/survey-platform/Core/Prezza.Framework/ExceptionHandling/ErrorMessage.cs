//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System;
using System.Text.RegularExpressions;
using System.Data;

using Prezza.Framework.Data;


namespace Prezza.Framework.ExceptionHandling
{
    /// <summary>
    /// Encapsulates an error code and string parameters used to lookup the text of errors
    /// </summary>
    public class FrameworkErrorMessage
    {
        private const string TOKEN = "%%";
        /// <summary>
        /// The error code
        /// </summary>
        private Int32 _errorCode;
        /// <summary>
        /// string parameters used with tokenized string
        /// </summary>
        private string[] _parameters;
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="errorCode">the integer code for this error</param>
        public FrameworkErrorMessage(Int32 errorCode)
        {
            _errorCode = errorCode;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="errorCode">the integer code for this error</param>
        /// <param name="args">a list of parameters used for replace within the error message's tokenized string</param>
        public FrameworkErrorMessage(Int32 errorCode, params string[] args)
            : this(errorCode)
        {
            _parameters = args;
        }

        /// <summary>
        /// The Int32 code for this ErrorMessage
        /// </summary>
        public Int32 ErrorCode
        {
            get { return _errorCode; }
        }

        /// <summary>
        /// Overridden.  
        /// </summary>
        /// <remarks>
        /// When called, retrieves the Error Code text from the database for language code en-US.
        /// Then parses the string for any tokens, replacing those as found with the ErrorMessage parameters.
        /// </remarks> 
        /// <returns>a string description for this error, localized to en-US</returns>
        public override string ToString()
        {
            // always default to us english
            return ToString("en-US");
        }



        /// <summary>
        /// Overridden.  
        /// </summary>
        /// <remarks>
        /// When called, retrieves the Error Code text from the database for the given language code.
        /// Then parses the string for any tokens, replacing those as found with the ErrorMessage parameters.
        /// </remarks> 
        /// <param name="languageCode">the language code to retrieve in the ISO format, e.g., en-US</param>
        /// <returns>a string description for this error, localized to the language code</returns>
        public string ToString(string languageCode)
        {
            // using the ErrorCode, retrieves the error text from the database and parses for tokens
            // replaces each token with this._parameters values.
            Database db = DatabaseFactory.CreateDatabase();
            DBCommandWrapper command = db.GetSqlStringCommandWrapper("SELECT Text FROM ckbx_ErrorText WHERE ErrorCode = " + _errorCode +
                " AND LanguageCode = '" + languageCode + "'");

            string message = string.Empty;
            using (IDataReader reader = db.ExecuteReader(command))
            {
                try
                {
                    while (reader.Read())
                    {
                        message = (string)reader[0];
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

            if (message.Length > 0 && _parameters != null && _parameters.Length > 0)
            {
                Regex reg = new Regex(@"%%\w*\b", RegexOptions.IgnoreCase);
                Match m = reg.Match(message);
                int counter = 0;
                while (m.Success)
                {
                    message = Regex.Replace(message, m.Value + "\\b", _parameters[counter], RegexOptions.IgnoreCase);
                    m = m.NextMatch();
                    // increment the counter
                    counter++;
                    // there has been a mismatch of parameters for this error text, break out 
                    // and just return the message as is.
                    if (counter >= _parameters.Length)
                        return message;
                }
            }

            if (message.Length == 0)
                message = "ERROR CODE " + _errorCode + ".  No additional information is available at this time.";

            return message;
        }
    }
}
