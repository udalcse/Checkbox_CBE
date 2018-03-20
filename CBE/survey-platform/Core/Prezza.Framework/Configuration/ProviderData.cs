//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================

namespace Prezza.Framework.Configuration
{
    /// <summary>
    /// Base class for user-defined provider configuration classes. 
    /// </summary>
    /// 
    public abstract class ProviderData : ConfigurationBase
    {
        /// <summary>
        /// Type name of the the provider which will be instantiated by the
        /// appropriate provider factory.
        /// </summary>
        private string typeName;

        /// <summary>
        /// Base constructor that initializes the provider name and type.
        /// </summary>
        /// <param name="providerName">Name of the provider this configuration is associated with.</param>
        /// <param name="typeName">Type name of the provider.  The type name will be used to instantiate the provider.</param>
        protected ProviderData(string providerName, string typeName) : base(providerName)
        {
            this.typeName = typeName;
        }

        /// <summary>
        /// Type name of the provider.
        /// </summary>
        public virtual string TypeName
        {
            get { return typeName; }
            set { typeName = value; }
        }
    }
}