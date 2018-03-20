using System;
using System.Threading;
using System.Web;
using Checkbox.Common;
using Prezza.Framework.Data;

namespace Checkbox.Web
{
    /// <summary>
    /// Data context provider based on session information.
    /// </summary>
    public class SessionDataContextProvider : DataContextProvider
    {
        /// <summary>
        /// Get/set application context
        /// </summary>
        public override string ApplicationContext
        {
            get 
            {
                object contextValueObject = null;

                if (HttpContext.Current != null)
                {
                    contextValueObject = HttpContext.Current.Items[APPLICATION_CONTEXT_KEY];
                }

                if (contextValueObject == null || Utilities.IsNullOrEmpty(contextValueObject.ToString()))
                {
                    contextValueObject = Thread.GetData(Thread.GetNamedDataSlot(APPLICATION_CONTEXT_KEY));
                }

                if (contextValueObject == null || Utilities.IsNullOrEmpty(contextValueObject.ToString()))
                {
                    throw new Exception("Unable to determine application context. Context value was null or empty.");
                }

                return contextValueObject.ToString();
            }
            set { }
        }

        /// <summary>
        /// Get whether context is secured
        /// </summary>
        public override bool Secured
        {
            get
            {
                object isSecureValueObject = Thread.GetData(Thread.GetNamedDataSlot(REQUEST_SECURED_KEY + ApplicationContext)) ??
                                             HttpContext.Current.Request.IsSecureConnection;

                if (isSecureValueObject == null)
                {
                    throw new Exception("Unable to determine if request is Secured. HttpContext.Current or HttpContext.Current.Items was null/empty.");
                }

                return (bool)isSecureValueObject;
            }
            set { }
        }
    }
}
