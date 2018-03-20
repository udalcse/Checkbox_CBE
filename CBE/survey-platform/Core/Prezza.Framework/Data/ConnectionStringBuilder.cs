//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Prezza Technologies, Inc.  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System.Text;
using Prezza.Framework.Data.Configuration;

namespace Prezza.Framework.Data
{
    /// <devdoc>
    /// Builds a connection string out of individual parameters (username, password, server, database, etc).
    /// Reads configuration information from a connectionString node
    /// </devdoc>
    internal sealed class ConnectionStringBuilder
    {
        private ConnectionStringBuilder()
        {
        }

        /// <devdoc>
        /// Creates a connection string by reading a connectionString node from configuration.
        /// </devdoc>        
        public static string Build(ConnectionStringData connectionString)
        {
            StringBuilder connection = new StringBuilder();
            foreach (ParameterData setting in connectionString.Parameters)
            {
                connection.AppendFormat("{0}={1};", setting.Name, setting.Value);
            }
            return connection.ToString();
        }
    }
}