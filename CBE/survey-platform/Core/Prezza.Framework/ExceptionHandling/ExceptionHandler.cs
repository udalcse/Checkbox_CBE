//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================

using System;

using Prezza.Framework.Configuration;

namespace Prezza.Framework.ExceptionHandling
{
    /// <summary>
    /// Base class for exception handler classes.
    /// </summary>
    public abstract class ExceptionHandler : ConfigurationProvider, IExceptionHandler
    {
        /// <summary>
        /// Default constructor.  Initializes the currentPolicyName and currentExceptionTypeName to empty strings.
        /// </summary>
        protected ExceptionHandler()
        {
            CurrentPolicyName = string.Empty;
            CurrentExceptionTypeName = string.Empty;
        }

        /// <summary>
        /// Get/Set the name of the current exception policy.
        /// </summary>
        public string CurrentPolicyName { get; set; }

        /// <summary>
        /// Get/Set the name of the current exception typename.f
        /// </summary>
        public string CurrentExceptionTypeName { get; set; }

        /// <summary>
        /// Handle the specified exception according to the specified policy.
        /// </summary>
        /// <param name="exception">Exception to handle.</param>
        /// <param name="policyName">Name of exception policy.</param>
        /// <param name="handlingInstanceId">Event handling instance Id, which can be used to link exceptions together.</param>
        /// <returns>Exception (possibly modified)</returns>
        public abstract Exception HandleException(Exception exception, string policyName, Guid handlingInstanceId);
    }
}