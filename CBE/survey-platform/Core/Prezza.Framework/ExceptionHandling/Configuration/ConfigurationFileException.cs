using System;

namespace Prezza.Framework.ExceptionHandling.Configuration
{
    /// <summary>
    /// Simple wrapper of Exception to provide some additional content.  The 
    /// System.Configuration.ConfigurationErrorsException is handled in such
    /// a way that handling it properly can be precluded by the System.Configuration
    /// handlers.
    /// </summary>
    public class ConfigurationFileException : Exception
    {
        private string _fileName;
        private Nullable<int> _lineNumber;
        private Nullable<int> _linePosition;

        /// <summary>
        /// Default construtor that accepts a message
        /// </summary>
        /// <param name="message"></param>
        public ConfigurationFileException(string message)
            : this(message, null)
        {
        }

        /// <summary>
        /// Constructor that accepts a message and inner exception
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public ConfigurationFileException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Get/set the file name
        /// </summary>
        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }

        /// <summary>
        /// Get/set the line number in the file
        /// </summary>
        public Nullable<int> LineNumber
        {
            get { return _lineNumber; }
            set { _lineNumber = value; }
        }

        /// <summary>
        /// Get/set the line position for the error
        /// </summary>
        public Nullable<int> LinePosition
        {
            get { return _linePosition; }
            set { _linePosition = value; }
        }
    }
}
