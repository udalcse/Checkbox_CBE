//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Prezza Technologies, Inc.  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System.Xml.Serialization;

namespace Prezza.Framework.Data.Configuration
{
    /// <summary>
    /// <para>Represents a <see cref="ConnectionStringData"/> object for Oracle. The Oracle connection string contains a collection of
    /// <see cref="OraclePackageData"/> objects.</para>
    /// </summary>
    [XmlRoot("connectionString", Namespace = DatabaseSettings.ConfigurationNamespace)]
    public class OracleConnectionStringData : ConnectionStringData
    {
        private OraclePackageDataCollection oraclePackages;

        /// <summary>
        /// <para>Initialize a new instance of the <see cref="OracleConnectionStringData"/> class.</para>
        /// </summary>
        public OracleConnectionStringData()
            : this(string.Empty)
        {
        }

        /// <summary>
        /// <para>Initialize a new instance of the <see cref="OracleConnectionStringData"/> class.</para>
        /// </summary>
        /// <param name="name">
        /// <para>The name of the <see cref="OracleConnectionStringData"/>.</para>
        /// </param>
        public OracleConnectionStringData(string name)
            : base(name)
        {
            this.oraclePackages = new OraclePackageDataCollection();
        }

        /// <summary>
        /// <para>Gets or sets the name of the oraclePackageData used when calling store procedures on an Oracle instance.</para>
        /// </summary>
        /// <value>
        /// <para>The oraclePackageData used when calling store procedures on an Oracle instance.</para>
        /// </value>        
        [XmlArray(ElementName = "packages", Namespace = DatabaseSettings.ConfigurationNamespace)]
        [XmlArrayItem(ElementName = "package", Type = typeof(OraclePackageData), Namespace = DatabaseSettings.ConfigurationNamespace)]
        public OraclePackageDataCollection OraclePackages
        {
            get { return this.oraclePackages; }
            set { this.oraclePackages = value; }
        }
    }
}