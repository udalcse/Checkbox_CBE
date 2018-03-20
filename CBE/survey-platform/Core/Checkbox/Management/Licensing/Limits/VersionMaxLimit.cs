using System;
using Checkbox.Configuration.Install;
using Checkbox.Globalization.Text;
using Checkbox.LicenseLibrary;

namespace Checkbox.Management.Licensing.Limits
{
    /// <summary>
    /// 
    /// </summary>
    public class VersionMaxLimit : VersionLimit
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="limitValue"></param>
        public VersionMaxLimit(string limitValue) : base(limitValue)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public VersionMaxLimit()
        {
        }

        public override LimitValidationResult Validate(out string messageTextId)
        {
            try
            {
                decimal currentVersion = ApplicationInstaller.ApplicationAssemblyShortVersion;

                if (currentVersion < _versionLimitValue.Value)
                {
                    messageTextId = string.Empty;
                    return LimitValidationResult.LimitNotReached;
                }

                messageTextId = TextManager.GetText("/versionLimit/maxLimitExceeded")
                    .Replace("{0}", _versionLimitValue.ToString()).Replace("{1}", currentVersion.ToString());
                
                return LimitValidationResult.LimitReached;
            }
            catch (Exception ex)
            {
                messageTextId = ex.Message;
                return LimitValidationResult.UnableToEvaluate;
            }
        }
    }
}
