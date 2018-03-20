//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System.Xml;
using Prezza.Framework.Configuration;
using Prezza.Framework.Logging.Formatters;

namespace Prezza.Framework.Logging.Distributor.Configuration
{
    /// <summary>
    /// Container for the <see cref="TextFormatter"/>'s configuration information.
    /// </summary>
    public class NullFormatterData : FormatterData, IXmlConfigurationBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public NullFormatterData()
            : this(string.Empty, string.Empty)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Name of the formatter.</param>
        /// <param name="dummy">Not used, but required for proper operation.</param>
        public NullFormatterData(string name, string dummy)
            : base(name, typeof(NullLogFormatter).AssemblyQualifiedName)
        {
        }

        #region IXmlConfigurationBase Members

        /// <summary>
        /// Do nothing
        /// </summary>
        /// <param name="node"></param>
        public void LoadFromXml(XmlNode node)
        {
        }

        #endregion
    }
}
