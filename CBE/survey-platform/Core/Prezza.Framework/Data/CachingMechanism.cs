//===============================================================================
// Prezza Technologies Application Framework
// Copyright � Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, � 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System;
using System.Collections;
using System.Data;

namespace Prezza.Framework.Data
{
    /// <devdoc>
    /// CachingMechanism provides caching support for stored procedure 
    /// parameter discovery and caching
    /// </devdoc>
    internal class CachingMechanism
    {
        private Hashtable paramCache = Hashtable.Synchronized(new Hashtable());

        /// <devdoc>
        /// Create and return a copy of the IDataParameter array.
        /// </devdoc>        
        public static IDataParameter[] CloneParameters(IDataParameter[] originalParameters)
        {
            IDataParameter[] clonedParameters = new IDataParameter[originalParameters.Length];

            for (int i = 0, j = originalParameters.Length; i < j; i++)
            {
                clonedParameters[i] = (IDataParameter)((ICloneable)originalParameters[i]).Clone();
            }

            return clonedParameters;
        }

        /// <devdoc>
        /// Empties all items from the cache
        /// </devdoc>
        public void Clear()
        {
            this.paramCache.Clear();
        }

        /// <devdoc>
        /// Add a parameter array to the cache for the command.
        /// </devdoc>        
        public void AddParameterSetToCache(IDbCommand command, IDataParameter[] parameters)
        {
            string connectionString = command.Connection.ConnectionString;
            string storedProcedure = command.CommandText;
            string key = CreateHashKey(connectionString, storedProcedure);
            this.paramCache[key] = parameters;
        }

        /// <devdoc>
        /// Gets a parameter array from the cache for the command. Returns null if no parameters are found.
        /// </devdoc>        
        public IDataParameter[] GetCachedParameterSet(IDbCommand command)
        {
            string connectionString = command.Connection.ConnectionString;
            string storedProcedure = command.CommandText;
            string key = CreateHashKey(connectionString, storedProcedure);
            IDataParameter[] cachedParameters = (IDataParameter[])(this.paramCache[key]);
            return CloneParameters(cachedParameters);
        }

        /// <devdoc>
        /// Gets if a given stored procedure on a specific connection string has a cached parameter set
        /// </devdoc>        
        public bool IsParameterSetCached(IDbCommand command)
        {
            string hashKey = CreateHashKey(
                command.Connection.ConnectionString,
                command.CommandText);
            return this.paramCache[hashKey] != null;
        }

        private static string CreateHashKey(string connectionString, string storedProcedure)
        {
            return connectionString + ":" + storedProcedure;
        }
    }
}