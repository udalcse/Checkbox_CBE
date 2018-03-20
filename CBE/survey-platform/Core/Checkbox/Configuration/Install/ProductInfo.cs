using System;

namespace Checkbox.Configuration.Install
{
    /// <summary>
    /// Simple container for product information
    /// </summary>
    public class ProductInfo : IComparable<ProductInfo>
    {
        private string _name;
        private string _version;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="version"></param>
        public ProductInfo(string name, string version)
        {
            _name = name;
            _version = version;
        }

        /// <summary>
        /// Get/set the name of the product
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Get/set the version of the product
        /// </summary>
        public string Version
        {
            get { return _version; }
            set { _version = value; }
        }


        #region IComparable<ProductInfo> Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(ProductInfo other)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
