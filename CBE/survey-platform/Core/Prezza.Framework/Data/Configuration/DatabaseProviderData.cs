//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System.Xml.Serialization;

using Prezza.Framework.Configuration;

namespace Prezza.Framework.Data.Configuration
{
    /// <summary>
    /// Represents the configuration data used to initialize
    /// a <see cref="Database"/> object.
    /// </summary>
    [XmlRoot("databaseProvider", Namespace = DatabaseSettings.ConfigurationNamespace)]
    public class DatabaseProviderData : ProviderData
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseProviderData"/> class.
        /// </summary>
        /// <param name="instance">An <see cref="InstanceData"/> object.</param>
        /// <param name="type">A <see cref="DatabaseTypeData"/> object.</param>
        public DatabaseProviderData(
            InstanceData instance,
            DatabaseTypeData type)
            : this(instance.Name, type.TypeName, instance.ConnectionString)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseProviderData"/> class.
        /// </summary>
        /// <param name="name">A name.</param>
        public DatabaseProviderData(string name)
            : this(name, string.Empty, string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseProviderData"/> class.
        /// </summary>
        /// <param name="name">A name.</param>
        /// <param name="typeName">A type name of a class that implements the <see cref="Database"/> class.</param>
        /// <param name="connectionStringName">Name of the connection string (store in the app/web .config file to use</param>
        public DatabaseProviderData(string name, string typeName, string connectionStringName)
            : base(name, typeName)
        {
            TypeName = typeName;
            ConnectionStringName = connectionStringName;
        }

        /// <summary>
        /// <para>When implemented by a class, Gets or sets the <see cref="System.Type"/> name of the provider.</para>
        /// </summary>
        /// <value>
        /// <para>The name of the provider. The default is an empty string.</para>
        /// </value>
        /// <remarks>
        /// <para><b>Not to implementers:</b> You do not have to implement the set operation.  If you have only one type for your data object you can ignore the set.</para>
        /// </remarks>
        public override string TypeName { get; set; }

        /// <summary>
        /// Get/set name of connection string associated with this db.  The string is stored in the app/web.config file
        /// </summary>
        public string ConnectionStringName { get; set; }
    }
}