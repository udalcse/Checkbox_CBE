using System;

namespace Checkbox.Messaging.Email
{
    /// <summary>
    /// Exception for cases where an attempt was made to call batch methods on an email provider that does not support
    /// batches.
    /// </summary>
    public class EmailProviderDoesNotSupportBatchesException : Exception
    {
        /// <summary>
        /// Get the error message.
        /// </summary>
        public override string Message { get { return "This email provider [" + GetType().AssemblyQualifiedName + "] does not support batch operations."; } }
    }
}
