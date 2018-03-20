using System;
using System.Collections.Generic;

namespace Checkbox.Panels
{
    /// <summary>
    /// Encapsulates an addressable member of a <see cref="Panel"/> as a collection of key/value string pairs
    /// </summary>
    [Serializable]
    public class Panelist : IComparable<Panelist>
    {
        private Dictionary<string, string> _propertiesDictionary;

        /// <summary>
        /// Get a reference to the properties dictionary
        /// </summary>
        protected Dictionary<string, string> PropertiesDictionary
        {
            get
            {
                if (_propertiesDictionary == null)
                {
                    _propertiesDictionary = new Dictionary<string, string>();
                }

                return _propertiesDictionary;
            }
        }

        /// <summary>
        /// Set the value of a panelist property
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="propertyValue"></param>
        public virtual void SetProperty(string propertyName, string propertyValue)
        {
            PropertiesDictionary[propertyName] = propertyValue;
        }

        /// <summary>
        /// Return a boolean indicating 
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public virtual bool ContainsProperty(string propertyName)
        {
            return PropertiesDictionary.ContainsKey(propertyName);
        }

        /// <summary>
        /// Get the value of panelist property
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public virtual string GetProperty(string propertyName)
        {
            if (ContainsProperty(propertyName))
            {
                return PropertiesDictionary[propertyName];
            }

            return string.Empty;
        }

        /// <summary>
        /// Get the panelist email
        /// </summary>
        public string Email
        {
            get { return GetProperty("Email"); }
            set { SetProperty("Email", value); }
        }

        /// <summary>
        /// Compute a hash key for internal use by invitation code to compare pending recipients with sent (persisted)
        /// recipients.
        /// </summary>
        /// <returns></returns>
        public string ComputeHashKey(int panelId)
        {
            string hashCode = panelId.ToString();

            if (ContainsProperty("UniqueIdentifier"))
            {
                hashCode += "__" + GetProperty("UniqueIdentifier");
            }
            else
            {
                hashCode += "__" + Email;
            }

            return hashCode;
        }

        /// <summary>
        /// Compare to a panelist
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(Panelist other)
        {
            return string.Compare(Email, other.Email);
        }
    }
}
