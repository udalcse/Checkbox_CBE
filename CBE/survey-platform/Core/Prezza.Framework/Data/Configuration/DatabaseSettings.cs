//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System.Xml.Serialization;

namespace Prezza.Framework.Data.Configuration
{
    /// <summary>
    /// <para>Represents the root configuration for data.</para>
    /// </summary>
    /// <remarks>
    /// <para>The class maps to the <c>databaseSettings</c> element in configuration.</para>
    /// </remarks>
    [XmlRoot("databaseSettings", Namespace = DatabaseSettings.ConfigurationNamespace)]
    public class DatabaseSettings
    {
        /// <summary>
        /// The name of the data configuration section.
        /// </summary>
        public const string SectionName = "dataConfiguration";
        private readonly DatabaseTypeDataCollection _databaseTypes;
        private readonly InstanceDataCollection _instances;

        /// <summary>
        /// <para>Gets the Xml namespace for this root node.</para>
        /// </summary>
        /// <value>
        /// <para>The Xml namespace for this root node.</para>
        /// </value>
        public const string ConfigurationNamespace = "";

        /// <summary>
        /// <para>Initialize a new instance of the <see cref="DatabaseSettings"/> class.</para>
        /// </summary>
        public DatabaseSettings()
        {
            _databaseTypes = new DatabaseTypeDataCollection();
            _instances = new InstanceDataCollection();
        }

        /// <summary>
        /// <para>Gets the <see cref="DatabaseTypeDataCollection"/>.</para>
        /// </summary>
        /// <value>
        /// <para>The database types available in configuration.</para>
        /// </value>
        /// <remarks>
        /// <para>This property maps to the <c>databaseTypes</c> element in configuration.</para>
        /// </remarks>
        [XmlArray(ElementName = "databaseTypes", Namespace = ConfigurationNamespace)]
        [XmlArrayItem(ElementName = "databaseType", Type = typeof(DatabaseTypeData), Namespace = ConfigurationNamespace)]
        public DatabaseTypeDataCollection DatabaseTypes
        {
            get { return _databaseTypes; }
        }

        /// <summary>
        /// <para>Gets the <see cref="InstanceDataCollection"/>.</para>
        /// </summary>
        /// <value>
        /// <para>The database instances available in configuration.</para>
        /// </value>
        /// <remarks>
        /// <para>This property maps to the <c>instances</c> element in configuration.</para>
        /// </remarks>
        [XmlArray(ElementName = "instances", Namespace = ConfigurationNamespace)]
        [XmlArrayItem(ElementName = "instance", Type = typeof(InstanceData), Namespace = ConfigurationNamespace)]
        public InstanceDataCollection Instances
        {
            get { return _instances; }
        }

        /// <summary>
        /// <para>Gets or sets the default database instance.</para>
        /// </summary>
        /// <value>
        /// <para>The default database instance.</para>
        /// </value>
        /// <remarks>
        /// <para>This property maps to the <c>defaultInstance</c> element in configuration.</para>
        /// </remarks>
        [XmlAttribute("defaultInstance")]
        public string DefaultInstance { get; set; }
    }
}