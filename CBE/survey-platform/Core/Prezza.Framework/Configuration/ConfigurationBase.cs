//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
namespace Prezza.Framework.Configuration
{
    /// <summary>
    /// Base-class for configuration objects.
    /// </summary>
    public abstract class ConfigurationBase
    {
        /// <summary>
        /// Name of the configuration
        /// </summary>
        private string name;

        /// <summary>
        /// Constructor.
        /// </summary>
        protected ConfigurationBase(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// Name of the configuration item.
        /// </summary>
        public string Name
        {
            get { return name; }
        }
    }
}