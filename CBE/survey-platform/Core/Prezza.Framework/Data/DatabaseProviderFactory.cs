//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================

/****************************************************************************
 * Factory for creation and initialization of database providers.			    *
 ****************************************************************************/

using System;

using Prezza.Framework.Configuration;
using Prezza.Framework.Common;
using Prezza.Framework.ExceptionHandling;

namespace Prezza.Framework.Data
{
    /// <summary>
    /// Handles creation and initialization of instances of text provider objects.
    /// </summary>
    public class DatabaseProviderFactory : ProviderFactory
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="factoryName">Name of the provider factory.</param>
        public DatabaseProviderFactory(string factoryName)
            : base(factoryName, typeof(Database))
        {
            try
            {
                ArgumentValidation.CheckForEmptyString(factoryName, "factoryName");
            }
            catch (Exception ex)
            {
                bool rethrow = ExceptionPolicy.HandleException(ex, "FrameworkPublic");

                if (rethrow)
                    throw;
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="factoryName">Name of the provider factory.</param>
        /// <param name="config">Globalization configuration information.</param>
        public DatabaseProviderFactory(string factoryName, DatabaseConfiguration config)
            : base(factoryName, typeof(Database), config)
        {
            try
            {
                ArgumentValidation.CheckForEmptyString(factoryName, "factoryName");
                ArgumentValidation.CheckForNullReference(config, "config");
            }
            catch (Exception ex)
            {
                bool rethrow = ExceptionPolicy.HandleException(ex, "FrameworkPublic");

                if (rethrow)
                    throw;
            }
        }

        /// <summary>
        /// Get an instance of an text provider.
        /// </summary>
        /// <returns>Instance of the default <see cref="Database" />.</returns>
        public Database CreateDefaultDatabase()
        {
            try
            {
                return (Database)base.CreateDefaultInstance();
            }
            catch (Exception ex)
            {
                bool rethrow = ExceptionPolicy.HandleException(ex, "FrameworkPublic");

                if (rethrow)
                    throw;
                else
                    return null;
            }
        }

        /// <summary>
        /// Get an instance of an text provider with the specified name.
        /// </summary>
        /// <param name="providerName">Name of the provider to get an instance of.</param>
        /// <returns>Instance of the specified <see cref="Database" />.</returns>
        public Database CreateDatabase(string providerName)
        {
            try
            {
                ArgumentValidation.CheckForEmptyString(providerName, "providerName");

                return (Database)base.CreateInstance(providerName);
            }
            catch (Exception ex)
            {
                bool rethrow = ExceptionPolicy.HandleException(ex, "FrameworkPublic");

                if (rethrow)
                    throw;
                else
                    return null;
            }
        }

        /// <summary>
        /// Get the configuration object for the specified provider.
        /// </summary>
        /// <param name="providerName">Name of the provider to get configuration for.</param>
        /// <returns><see cref="ProviderData" /> object for the specified provider.</returns>
        protected override ConfigurationBase GetConfigurationObject(string providerName)
        {
            ArgumentValidation.CheckForEmptyString(providerName, "providerName");

            DatabaseConfiguration config = (DatabaseConfiguration)base.Config;
            return config.GetDatabaseProviderConfig(providerName);
        }

        /// <summary>
        /// Get the <see cref="System.Type" /> of the specified text provider.
        /// </summary>
        /// <param name="textProviderName">Name of the text provider.</param>
        /// <returns><see cref="System.Type" /> of the specified text provider.</returns>
        protected override Type GetConfigurationType(string textProviderName)
        {

            ArgumentValidation.CheckForEmptyString(textProviderName, "textProviderName");

            ProviderData config = (ProviderData)GetConfigurationObject(textProviderName);
            return GetType(config.TypeName);
        }

        /// <summary>
        /// Get the name of the default text provider (if specified in the configuration).
        /// </summary>
        /// <returns>Name of the default text provider.</returns>
        protected override string GetDefaultInstanceName()
        {
            DatabaseConfiguration config = (DatabaseConfiguration)base.Config;
            return config.DefaultDatabaseProvider;
        }
    }
}
