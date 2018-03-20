using System;

namespace Checkbox.Management.Licensing
{
    ///<summary>
    ///</summary>
    public class CheckboxLicenseException : Exception
    {
        ///<summary>
        ///</summary>
        ///<param name="message"></param>
        public CheckboxLicenseException(string message) : base(message)
        {
        }
    }
}
