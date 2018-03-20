using System;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Indicates that an attempt to retrieve response template for specified survey.
    /// The issue occurs when the response template does not exist or as a result of insufficient permissions.
    /// </summary>
    [Serializable]
    public class ResponseTemplateDoesNotExistException : Exception
    {
        /// <summary>
        /// Exception for web service authorization issues.
        /// </summary>
        public ResponseTemplateDoesNotExistException(int surveyId)
            : base(string.Format("Unable to locate response template for the survey with id {0}. Either the response template or the survey do not exist or you do not have sufficient permissions to manage them.", surveyId))
        {
        }
    }
}
