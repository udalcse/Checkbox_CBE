using System;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Indicates that an attempt to retrieve a response answer.
    /// The issue occurs when the answer does not exist or as a result of insufficient permissions.
    /// </summary>
    [Serializable]
    public class ResponseAnswerDoesNotExistException : Exception
    {
        /// <summary>
        /// Exception for web service authorization issues.
        /// </summary>
        public ResponseAnswerDoesNotExistException(long answerID)
            : base(string.Format("Unable to locate the answer with id {0}. Either the answer does not exist or you do not have sufficient permissions to manage the survey.", answerID))
        {
        }
    }
}
