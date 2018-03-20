using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Checkbox.LicenseLibrary;

namespace Checkbox.Management.Licensing.Limits
{
    /// <summary>
    /// Base class to validate decrement limits.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class DecrementLicenseLimit<T> : ValueLimit<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="license"></param>
        public override void Initialize(CheckboxLicenseData license)
        {
            //Decrement limits are stored only in Hostring DB. So do nothing here.
        }

        public override string LimitValue
        {
            get { return "0"; }
        }

        public override T LicenseFileLimitValue
        {
            get { throw new NotImplementedException("The property 'LicenseFileLimitValue' isn't used with decrement value."); }
        }

        /// <summary>
        /// Get current value
        /// </summary>
        public virtual T CurrentValue
        {
            get { return GetCurrentValue(); }
        }

        /// <summary>
        /// Get base value
        /// </summary>
        public virtual T BaseValue
        {
            get { return GetBaseValue(); }
        }

        /// <summary>
        ///  Get the currently in-effect limit value.
        /// </summary>
        /// <remarks>Override RuntimeLimitValue because this information is stored only in the DB.</remarks>
        public override T RuntimeLimitValue
        {            
            get { return CurrentValue; }
        }

        /// <summary>
        /// Get the current value of decrement limit
        /// </summary>
        /// <returns></returns>
        public abstract T GetCurrentValue();

        /// <summary>
        /// Get the base value of decrement limit.
        /// </summary>
        /// <returns></returns>
        public abstract T GetBaseValue();

        /// <summary>
        /// Decrement current value.
        /// </summary>
        public abstract void Decrement();
    }
}
