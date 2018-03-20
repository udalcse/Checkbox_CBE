//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================

using System;
using System.Configuration;

using Prezza.Framework.Common;

namespace Prezza.Framework.Configuration
{
	/// <summary>
	/// Configuration factory specific to providers.  Supports default instances and initialization of providers
	/// upon instantiation.
	/// </summary>
	public abstract class ProviderFactory : ConfigurationFactory
	{
		/// <summary>
		/// Base type of providers this factory will create.
		/// </summary>
		private readonly Type type;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="factoryName">Name of the factory.</param>
		/// <param name="type">Base type of providers that the factory will create.</param>
		protected ProviderFactory(string factoryName, Type type) : base(factoryName) 
		{
			ArgumentValidation.CheckForNullReference(type, "type");
			
			this.type = type;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="factoryName">Name of the factory.</param>
		/// <param name="type">Base type of providers that the factory will create.</param>
		/// <param name="config">Configuration information for the provider.</param>
		protected ProviderFactory(string factoryName, Type type, ConfigurationBase config) : base(factoryName, config)
		{
			ArgumentValidation.CheckForNullReference(config, "config");

			this.type = type;
		}
		

		/// <summary>
		/// Each provider will implement the <see cref="IConfigurationProvider" /> interface, which requires
		/// implementation of an Initialize method that accepts an object argument.  The GetConfigurationObject
		/// method is used to get the configuration object to initialize the provider with.
		/// </summary>
		/// <param name="providerName">Name of the provider to create the configuration object for.</param>
		/// <returns>Configuration object for the specified provider.</returns>
		protected abstract ConfigurationBase GetConfigurationObject(string providerName);

		/// <summary>
		/// Initialize the specified configuration provider by calling it's known Initialize method and passing
		/// it a configuration object.
		/// </summary>
		/// <param name="providerName">Name of the provider to initialize.</param>
		/// <param name="provider">Provider to initialize.</param>
		protected virtual void InitializeConfigurationProvider(string providerName, IConfigurationProvider provider) 
		{
			ArgumentValidation.CheckForEmptyString(providerName, "providerName");
			ArgumentValidation.CheckForNullReference(provider, "provider");
			ArgumentValidation.CheckExpectedType(provider, provider.GetType());

			provider.Initialize(GetConfigurationObject(providerName));
		}
		
		/// <summary>
		/// Create and initialize object with the specified <paramref name="proverName" /> and of the specified <paramref name="type" />.
		/// </summary>
		/// <param name="providerName">Name of provider object to create.</param>
		/// <param name="type"><see cref="System.Type"/> of the object to create.</param>
		/// <returns></returns>
		protected override object CreateObject(string providerName, Type type) 
		{
			//Validate specified provider implements IConfigurationProvider interface
			ArgumentValidation.CheckForEmptyString(providerName, "providerName");
			ArgumentValidation.CheckForNullReference(type, "type");

			ValidateTypeIsIConfigurationProvider(type);

			object createdObject = base.CreateObject(providerName, type);
			InitializeObject(providerName, createdObject);
			return createdObject;
		}
		
		/// <summary>
		/// Initialize the specified object with it's associated configuration object.
		/// </summary>
		/// <param name="providerName">Name of provider to initialize.</param>
		/// <param name="Object">Provider object to initialize.</param>
		private void InitializeObject(string providerName, object Object) 
		{
			ArgumentValidation.CheckForEmptyString(providerName, "providerName");
			ArgumentValidation.CheckForNullReference(Object, "Object");

			IConfigurationProvider provider = (IConfigurationProvider)Object;
			provider.ConfigurationName = providerName;
			InitializeConfigurationProvider(providerName, provider);
		}

		/// <summary>
		/// Validate that the specified type is an <see cref="IConfigurationProvider" />.
		/// </summary>
		/// <param name="type"><see cref="System.Type" /> to verify.</param>
		private void ValidateTypeIsIConfigurationProvider(Type type) 
		{
			ArgumentValidation.CheckForNullReference(type, "type");

			if (!ProviderType.IsAssignableFrom(type))
			{
				throw new Exception("Type mismatch between Provider type [" + ProviderType.AssemblyQualifiedName + "] and requested type [" + type.AssemblyQualifiedName);
			}

			if (!typeof (IConfigurationProvider).IsAssignableFrom(type))
			{
				throw new Exception("Type [" + type.AssemblyQualifiedName + "] is not of same type as Provider type [" + ProviderType.AssemblyQualifiedName + "]");
			}
		}
		
		/// <summary>
		/// Create an instance of the default provider.
		/// </summary>
		/// <returns>An instance of the default provider.</returns>
		protected virtual object CreateDefaultInstance() 
		{
			string defaultName = GetDefaultInstanceName();

			if(type == null || defaultName == null || defaultName.Trim() == string.Empty)
			{
				throw new Exception("No default provider specified in configuration for factory: " + FactoryName);
			}

			return CreateInstance(defaultName);
		}
		
		/// <summary>
		/// Get the name of the default provider.
		/// </summary>
		/// <returns>Name of the default provider.</returns>
		protected virtual string GetDefaultInstanceName() 
		{
			return string.Empty;
		}

		/// <summary>
		/// <see cref="System.Type" /> of providers the factory will create.
		/// </summary>
		protected Type ProviderType
		{
			get{return type;}
		}
	}
}
