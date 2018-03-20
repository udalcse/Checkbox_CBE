//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System;
using System.Runtime.Serialization;


namespace Prezza.Framework.ExceptionHandling
{
    /// <summary>
    /// Exception class for exceptions that occur during the exception handling process.
    /// </summary>
    [Serializable]
    public class ExceptionHandlingException : Exception
    {
        /// <summary>
        /// Construtor.
        /// </summary>
        public ExceptionHandlingException()
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">Exception message.</param>
        public ExceptionHandlingException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">Exception handling message.</param>
        /// <param name="innerException">Inner exception for the handling exception.</param>
        public ExceptionHandlingException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Constructor for serialization.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected ExceptionHandlingException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
