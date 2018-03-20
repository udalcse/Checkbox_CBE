//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System.Xml.Serialization;

namespace Prezza.Framework.Data.Configuration
{
    /// <summary>
    /// <para>Represents a database instance in configuration.</para>
    /// </summary>
    /// <remarks>
    /// <para>The class maps to the <c>instance</c> element in configuration.</para>
    /// </remarks>
    [XmlRoot("instance", Namespace = DatabaseSettings.ConfigurationNamespace)]
    public class InstanceData
    {
        private string _name;
        private string _typeName;
        private string _connectionString;

        /// <summary>
        /// <para>Initialize a new instance of the <see cref="InstanceData"/> class.</para>
        /// </summary>
        public InstanceData()
        {
            _name = string.Empty;
            _typeName = string.Empty;
            _connectionString = string.Empty;
        }

        /// <summary>
        /// <para>Initialize a new instance of the <see cref="InstanceData"/> class with a name, name of the <see cref="DatabaseTypeData"/> and the name of the <see cref="ConnectionString"/>.</para>
        /// </summary>
        /// <param name="name"><para>The name of the instance.</para></param>        
        public InstanceData(string name)
            : this(name, string.Empty, string.Empty)
        {
        }

        /// <summary>
        /// <para>Initialize a new instance of the <see cref="InstanceData"/> class with a name, name of the <see cref="DatabaseTypeData"/> and the name of the <see cref="ConnectionString"/>.</para>
        /// </summary>
        /// <param name="name">The name of the instance.</param>
        /// <param name="typeName">The fully qualified assembly name of the type.</param>
        /// <param name="connectionString">The connection string for the instance.</param>
        public InstanceData(string name, string typeName, string connectionString)
            : this()
        {
            _name = name;
            _typeName = typeName;
            _connectionString = connectionString;
        }

        /// <summary>
        /// <para>Gets or sets the name of the instance.</para>
        /// </summary>
        /// <value>
        /// <para>The name of the instance. The default is an empty string.</para>
        /// </value>
        /// <remarks>
        /// <para>This property maps to the <c>name</c> attribute in configuration.</para>
        /// </remarks>
        [XmlAttribute("name")]
        public string Name
        {
            get { return _name; }
            set {_name = value; }
        }

        /// <summary>
        /// <para>Gets or sets the type of the instance.</para>
        /// </summary>
        /// <value>
        /// <para>The type of the instance. The default is an empty string.</para>
        /// </value>
        /// <remarks>
        /// <para>This property maps to the <c>type</c> attribute in configuration.</para>
        /// </remarks>
        [XmlAttribute("type")]
        public string DatabaseTypeName
        {
            get { return _typeName; }
            set { _typeName = value; }
        }

        /// <summary>
        /// <para>Gets or sets name of the <see cref="ConnectionString"/> for the instance.</para>
        /// </summary>
        /// <value>
        /// <para>Gets or sets name of the <see cref="ConnectionString"/> for the instance. The default is an empty string.</para>
        /// </value>
        /// <remarks>
        /// <para>This property maps to the <c>connectionString</c> attribute in configuration.</para>
        /// </remarks>
        [XmlAttribute("connectionString")]
        public string ConnectionString
        {
            get { return _connectionString; }
            set { _connectionString = value; }
        }
    }

}