using System;
using Prezza.Framework.Data;
using System.Data;
using Prezza.Framework.ExceptionHandling;
using System.Configuration;

namespace Checkbox.Management.Licensing
{
    /// <summary>
    /// Class for validating licenses and extracting limits from the license.
    /// </summary>
    public class LicenseValidator : IDisposable
    {
        private readonly CheckboxLicense _license;
        
        /// <summary>
        /// Attempt to validate the license on creation.  For now, accept license as a parameter until all app pages
        /// transition to new hierarchy, then license will be managed entirely in this class
        /// </summary>
        public LicenseValidator(CheckboxLicense license)
        {
            _license = license;

            ValidateLicense();
        }

        /// <summary>
        /// 
        /// </summary>
        private void ValidateLicense()
        {
            if(ConfigurationManager.AppSettings["LicenseRequired"] == "false")
            {
                IsLicenseValid = true;
                IsTrial = false;
                return;
            }

            if (_license == null || _license is NullLicense)
            {
                IsLicenseValid = false;
                LicenseError = _license is NullLicense && !string.IsNullOrEmpty(_license.ValidationError) ? 
                    _license.ValidationError : "No license found in Checkbox /bin folder.";
                
                return;
            }

            IsLicenseValid = _license.IsValid;
            LicenseError = _license.ValidationError;
            IsTrial = _license.IsTrial;
            if (IsTrial)
            {
                IsLicenseExpired = checkLicenseExpiration();
            }
        }

        static DateTime? _checkboxInstallationDate;
        static object lockObject = new object();
        /// <summary>
        /// Checkes whether the license is expired
        /// </summary>
        /// <returns></returns>
        private bool checkLicenseExpiration()
        {
            lock (lockObject)
            {
                if (!_checkboxInstallationDate.HasValue)
                {
                    _checkboxInstallationDate = getCheckboxInstallationDate(); 
                }
                if (!_checkboxInstallationDate.HasValue)
                    return false;
            }

            return _checkboxInstallationDate.Value.AddDays(30) < DateTime.Now;
        }

        /// <summary>
        /// Reads installed date for the product
        /// </summary>
        /// <returns></returns>
        private DateTime? getCheckboxInstallationDate()
        {
            DateTime? res = null;
            try
            {
                Database db = DatabaseFactory.CreateDatabase();
                DBCommandWrapper command = db.GetStoredProcCommandWrapper("ckbx_sp_GetCheckboxInstallationDate");
                using (IDataReader reader = db.ExecuteReader(command))
                {
                    try
                    {
                        if (reader.Read())
                        {
                            res = DbUtility.GetValueFromDataReader<DateTime?>(reader, "InstallDate", null);
                        }
                    }
                    finally
                    {
                        reader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                //Ignore error, but log it
                ExceptionPolicy.HandleException(ex, "BusinessProtected");
            }
            return res;
        }

        /// <summary>
        /// Get whether the license is valid
        /// </summary>
        public bool IsLicenseValid
        {
            get; private set;
        }

        /// <summary>
        /// Get whether the license is expired
        /// </summary>
        public bool IsLicenseExpired
        {
            get;
            private set;
        }

        /// <summary>
        /// Get the license error in the case of an invalid license
        /// </summary>
        public string LicenseError
        {
            get; private set;
        }

        /// <summary>
        /// Get the active license
        /// </summary>
        public CheckboxLicense ActiveLicense
        {
            get { return _license; }
        }

        /// <summary>
        /// Get a value from a license
        /// </summary>
        /// <param name="valueName"></param>
        /// <returns></returns>
        public string GetLicenseValue(string valueName)
        {
            return _license != null ? _license.GetValue(valueName) : null;
        }

        #region License Information


        /// <summary>
        /// Get whether the active license is a trial license.
        /// </summary>
        public bool IsTrial { get; private set; }

        #endregion


        #region IDisposable Members

        /// <summary>
        /// Dispose the item
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            //Don't call the finalizer since this object has been
            // disposed.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Do the actual work of disposing
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_license != null)
                {
                    _license.Dispose();
                }
            }
        }

        #endregion

    }
}
