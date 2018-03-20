using System;
using System.Linq;
using Prezza.Framework.Configuration;

using Prezza.Framework.Common;
using Prezza.Framework.ExceptionHandling;

using Checkbox.Messaging.Configuration;

namespace Checkbox.Messaging.Email
{
    internal class EmailProviderFactory : ProviderFactory
    {
        /// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="factoryName">Name of the provider factory.</param>
        internal EmailProviderFactory(string factoryName)
            : base(factoryName, typeof(IEmailProvider)) 
		{            
			try
			{
				ArgumentValidation.CheckForEmptyString(factoryName, "factoryName");
			}
			catch(Exception ex)
			{
				bool rethrow = ExceptionPolicy.HandleException(ex, "BusinessInternal");

				if(rethrow)
					throw;
			}
		}

        /// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="factoryName">Name of the provider factory.</param>
		/// <param name="config">Messaging configuration information.</param>
        internal EmailProviderFactory(string factoryName, MessagingConfiguration config)
            : base(factoryName, typeof(IEmailProvider), config) 
		{
			try
			{
				ArgumentValidation.CheckForEmptyString(factoryName, "factoryName");
				ArgumentValidation.CheckForNullReference(config, "config");
			}
			catch(Exception ex)
			{
				bool rethrow = ExceptionPolicy.HandleException(ex, "BusinessInternal");

				if(rethrow)
					throw;
			}
		}

        /// <summary>
        /// Get an instance of an email provider.
        /// </summary>
        /// <returns>Instance of the default <see cref="IEmailProvider" />.</returns>
        internal IEmailProvider GetEmailProvider()
        {
            try
            {
                return (IEmailProvider)base.CreateDefaultInstance();
            }
            catch (Exception ex)
            {
                bool rethrow = ExceptionPolicy.HandleException(ex, "BusinessInternal");

                if (rethrow)
                    throw;
                
                return null;
            }
        }

        /// <summary>
        /// Get an instance of an email provider with the specified name.
        /// </summary>
        /// <param name="providerName">Name of the provider to get an instance of.</param>
        /// <returns>Instance of the specified <see cref="IEmailProvider" />.</returns>
        internal IEmailProvider GetEmailProvider(string providerName)
        {
            try
            {
                ArgumentValidation.CheckForEmptyString(providerName, "providerName");

                return (IEmailProvider)base.CreateInstance(providerName);
            }
            catch (Exception ex)
            {
                bool rethrow = ExceptionPolicy.HandleException(ex, "BusinessInternal");

                if (rethrow)
                    throw;
                
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

            MessagingConfiguration config = (MessagingConfiguration)Config;
            return config.GetEmailProviderConfig(providerName);
        }

        /// <summary>
        /// Get the <see cref="System.Type" /> of the specified email provider.
        /// </summary>
        /// <param name="emailProviderName">Name of the email provider.</param>
        /// <returns><see cref="System.Type" /> of the specified email provider.</returns>
        protected override Type GetConfigurationType(string emailProviderName)
        {

            ArgumentValidation.CheckForEmptyString(emailProviderName, "emailProviderName");

            ProviderData config = (ProviderData)GetConfigurationObject(emailProviderName);
            return GetType(config.TypeName);
        }

        /// <summary>
        /// Get the name of the default email provider (if specified in the configuration).
        /// </summary>
        /// <returns>Name of the default email provider.</returns>
        protected override string GetDefaultInstanceName()
        {
            MessagingConfiguration config = (MessagingConfiguration)Config;
            return config.DefaultEmailProvider;
        }

        private static bool? _hasBatchSupportiveProvider;
        /// <summary>
        /// 
        /// </summary>
        public bool HasBatchSupportiveProvider
        {         
            get
            {
                if (!_hasBatchSupportiveProvider.HasValue)
                {
                    _hasBatchSupportiveProvider = false;
                    foreach (ProviderData providerData in ((MessagingConfiguration)Config).EMailProviders)
                    {
                        IEmailProvider provider = GetEmailProvider(providerData.Name);
                        if (provider.SupportsBatches)
                        {
                            _hasBatchSupportiveProvider = true;
                            break;
                        }
                    }
                }
                return _hasBatchSupportiveProvider.Value;
            }
        }
    }
}
