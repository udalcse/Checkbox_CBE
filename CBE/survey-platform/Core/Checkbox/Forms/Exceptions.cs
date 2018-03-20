using System;

namespace Checkbox.Forms
{
    /// <summary>
    /// Exception base class specific to templates.
    /// </summary>
    public abstract class TemplateException : System.ApplicationException
    {
        private Nullable<int> _templateID;
        private string _templateGuid;

        /// <summary>
        /// Create a new exception for a specific template.
        /// </summary>
        /// <param name="templateID">ID of template associated with exception.</param>
        public TemplateException(int templateID) : base()
        {
            _templateID = templateID;
        }

        /// <summary>
        /// Create a new exception for a specific template.
        /// </summary>
        /// <param name="templateGuid">GUID of template associated with exception.</param>
        public TemplateException(string templateGuid) : base()
        {
            _templateGuid = templateGuid;
        }

        /// <summary>
        /// ID of template associated with exception.
        /// </summary>
        public Nullable<int> TemplateID
        {
            get { return _templateID; }
        }

        /// <summary>
        /// GUID of template associated with exception.
        /// </summary>
        public string TemplateGuid
        {
            get { return _templateGuid; }
        }

    }

    /// <summary>
    /// Exception thrown when an attempt is made to load a template that doesn't exist.
    /// </summary>
    public class TemplateDoesNotExist : TemplateException
    {
        /// <summary>
        /// Construct exception.
        /// </summary>
        /// <param name="templateID">ID of template that could not be found.</param>
        public TemplateDoesNotExist(int templateID)
            : base(templateID)
        {
        }

        /// <summary>
        /// Construct exception.
        /// </summary>
        /// <param name="templateGuid">GUID for template that does not exist.</param>
        public TemplateDoesNotExist(string templateGuid)
            : base(templateGuid)
        {
        }
    }

    /// <summary>
    /// Exception thrown when an error occurred while loading a template.
    /// </summary>
    public class TemplateLoadError : TemplateException
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="templateID">ID of template that could not be loaded.</param>
        public TemplateLoadError(int templateID)
            : base(templateID)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="templateGuid">GUID of template that could not be loaded.</param>
        public TemplateLoadError(string templateGuid)
            : base(templateGuid)
        {
        }
    }
}
