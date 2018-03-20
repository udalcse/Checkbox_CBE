using System;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// 
    /// </summary>
    public class NoUserException : Exception
    {
        public NoUserException()
            : base("No login context or login context expired.  Valid user required for this operation.")
        {
        }
    }
}
