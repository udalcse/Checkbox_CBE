using System;
using System.Collections;
using System.Configuration;
using Prezza.Framework.Common;
using Prezza.Framework.Configuration;
using Prezza.Framework.ExceptionHandling;

using Checkbox.Messaging.Configuration;

namespace Checkbox.Messaging.Email
{
    internal sealed class EmailFactory
    {
        private static Hashtable providers;

		/// <summary>
		/// Constructor.  Since this class only exposes static methods, there
		/// should be no need to call the constructor.
		/// </summary>
		static EmailFactory()
		{
			lock(typeof(EmailFactory))
			{
                providers = new Hashtable();
			}
			Prezza.Framework.Configuration.ConfigurationManager.ConfigurationChanged += new ConfigurationChangedEventHandler(ConfigurationManager_ConfigurationChanged);
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="providerName"></param>
        internal static void ChangeEmailProvider(string providerName)
        {
            lock (providers.SyncRoot)
            {
                if (!providers.Contains(providerName))
                {
                    EmailProviderFactory factory = new EmailProviderFactory("emailProviderFactory", (MessagingConfiguration)Prezza.Framework.Configuration.ConfigurationManager.GetConfiguration("checkboxMessagingConfiguration"));
                    IEmailProvider provider = factory.GetEmailProvider(providerName);
                    providers[providerName] = provider;
                    if (provider == null)
                        throw new Exception(string.Format("EMail provider {0} has not been found.", providerName));
                }

                providers["[DEFAULT]"] = providers[providerName];
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal static IEmailProvider GetEmailProvider()
        {
            try
            {
                lock (providers.SyncRoot)
                {
                    if (providers.Contains("[DEFAULT]"))
                    {
                        return (IEmailProvider)providers["[DEFAULT]"];
                    }
                }               

                EmailProviderFactory factory = new EmailProviderFactory("emailProviderFactory", (MessagingConfiguration)Prezza.Framework.Configuration.ConfigurationManager.GetConfiguration("checkboxMessagingConfiguration"));
                IEmailProvider provider = factory.GetEmailProvider();

                lock (providers.SyncRoot)
                {
                    if (!providers.Contains("[DEFAULT]"))
                    {
                        providers.Add("[DEFAULT]", provider);
                    }
                }

                return provider;
            }
            catch (Exception ex)
            {
                bool rethrow = ExceptionPolicy.HandleException(ex, "BusinessInternal");

                if (rethrow)
                    throw;
                else
                    return null;
            }
        }

        /// <summary>
        /// Create and initialize an instance of the specified email 
        /// provider. 
        /// </summary>
        /// <param name="emailProvider">Name of the email provider to instantiate and initialize.</param>
        /// <returns>Initialized instance of an email provider.</returns>
        /// <remarks>
        /// The returned email provider implements the 
        /// <see cref="IEmailProvider" /> interface.</remarks>
        /// <exception cref="ArgumentNullException">emailProvider is null</exception>
        /// <exception cref="ArgumentException">emailProvider is empty</exception>
        /// <exception cref="Exception">Could not find instance specified in emailProvider</exception>
        /// <exception cref="InvalidOperationException">Error processing configuration information defined in application configuration file.</exception>
        internal static IEmailProvider GetEmailProvider(string emailProvider)
        {
            try
            {
                lock (providers.SyncRoot)
                {
                    if (providers.Contains(emailProvider))
                    {
                        return (IEmailProvider)providers[emailProvider];
                    }
                }

                EmailProviderFactory factory = new EmailProviderFactory("emailProviderFactory", (MessagingConfiguration)Prezza.Framework.Configuration.ConfigurationManager.GetConfiguration("checkboxMessagingConfiguration"));
                IEmailProvider provider = factory.GetEmailProvider(emailProvider);

                lock (providers.SyncRoot)
                {
                    if (!providers.Contains(emailProvider))
                    {
                        providers.Add(emailProvider, provider);
                    }
                }

                return provider;
            }
            catch (Exception ex)
            {
                bool rethrow = ExceptionPolicy.HandleException(ex, "BusinessInternal");

                if (rethrow)
                    throw;
                else
                    return null;
            }
        }

        private static void ConfigurationManager_ConfigurationChanged(object sender, ConfigurationChangedEventArgs e)
        {
            if (providers != null)
                providers.Clear();
        }

        public static bool HasBatchSupportiveProvider
        {
            get
            {
                EmailProviderFactory factory = new EmailProviderFactory("emailProviderFactory", (MessagingConfiguration)Prezza.Framework.Configuration.ConfigurationManager.GetConfiguration("checkboxMessagingConfiguration"));
                return factory.HasBatchSupportiveProvider;

            }
        }

    }
}
