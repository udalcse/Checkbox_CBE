using System;
using System.ComponentModel;
using System.IO;
using System.Configuration;

namespace Checkbox.Management.Licensing
{
    /// <summary>
    /// Handles resolution of license and issues valid licenses at runtime and design time 
    /// to the LicenseManager.Validate method.
    /// </summary>
    public sealed class CheckboxLicenseProvider : LicenseProvider
    {
        static CheckboxLicense _license;

        /// <summary>
        /// Local path where to look license file for
        /// </summary>
        public static string Path;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="type"></param>
        /// <param name="instance"></param>
        /// <param name="allowExceptions"></param>
        /// <returns></returns>
        public override License GetLicense(LicenseContext context, Type type, object instance, bool allowExceptions)
        {
            return GetLicense();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public License GetLicense()
        {
            if (_license != null && _license.IsValid)
            {
                return _license;
            }

            return (_license = CreateLicense());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static CheckboxLicense CreateLicense()
        {
            if(ConfigurationManager.AppSettings["LicenseRequired"] == "false")
            {
                var licenseStub = new CheckboxLicense();
                licenseStub.AddLimitsFromHostingDB();
                return licenseStub;
            }

            string[] files = Directory.GetFiles(Path + "\\Bin", "*.lic", SearchOption.TopDirectoryOnly);

            var license = new CheckboxLicense();

            foreach (string filePath in files)
            {
                license.ReadFromFile(filePath);

                license.AddLimitsFromHostingDB();

                // if we can not read the license file it is not valid or XML is bad
                if (license.IsValid)
                {
                    return license;
                }
            }

            //No License
            return new NullLicense(license.ValidationError);
        }
    }
}
