/****************************************************************************
 * Factory for creation and initialization of text providers.			    *
 ****************************************************************************/

using System;
using Prezza.Framework.Configuration;

using Prezza.Framework.Common;
using Prezza.Framework.ExceptionHandling;

using Checkbox.Globalization.Configuration;

namespace Checkbox.Globalization.Text
{
	/// <summary>
	/// Handles creation and initialization of instances of text provider objects.
	/// </summary>
	public class TextProviderFactory : ProviderFactory
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="factoryName">Name of the provider factory.</param>
		public TextProviderFactory(string factoryName) : base(factoryName, typeof(ITextProvider)) 
		{
			try
			{
				ArgumentValidation.CheckForEmptyString(factoryName, "factoryName");
			}
			catch(Exception ex)
			{
				bool rethrow = ExceptionPolicy.HandleException(ex, "BusinessPublic");

				if(rethrow)
					throw;
			}
		}
		
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="factoryName">Name of the provider factory.</param>
		/// <param name="config">Globalization configuration information.</param>
		public TextProviderFactory(string factoryName, GlobalizationConfiguration config) : base(factoryName, typeof(ITextProvider), config) 
		{
			try
			{
				ArgumentValidation.CheckForEmptyString(factoryName, "factoryName");
				ArgumentValidation.CheckForNullReference(config, "config");
			}
			catch(Exception ex)
			{
				bool rethrow = ExceptionPolicy.HandleException(ex, "BusinessPublic");

				if(rethrow)
					throw;
			}
		}

		/// <summary>
		/// Get an instance of an text provider.
		/// </summary>
		/// <returns>Instance of the default <see cref="ITextProvider" />.</returns>
		public ITextProvider GetTextProvider()
		{
			try
			{
				return (ITextProvider)base.CreateDefaultInstance();
			}
			catch(Exception ex)
			{
				bool rethrow = ExceptionPolicy.HandleException(ex, "BusinessPublic");

				if(rethrow)
					throw;
				else
					return null;
			}
		}

		/// <summary>
		/// Get an instance of an text provider with the specified name.
		/// </summary>
		/// <param name="providerName">Name of the provider to get an instance of.</param>
		/// <returns>Instance of the specified <see cref="ITextProvider" />.</returns>
		public ITextProvider GetTextProvider(string providerName) 
		{
			try
			{
				ArgumentValidation.CheckForEmptyString(providerName, "providerName");

				return (ITextProvider)base.CreateInstance(providerName);
			}
			catch(Exception ex)
			{
				bool rethrow = ExceptionPolicy.HandleException(ex, "BusinessPublic");

				if(rethrow)
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

			GlobalizationConfiguration config = (GlobalizationConfiguration)base.Config;
			return config.GetTextProviderConfig(providerName);
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
			GlobalizationConfiguration config = (GlobalizationConfiguration)base.Config;
			return config.DefaultTextProvider;
		}
	}	
}
