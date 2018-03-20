//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================

using System;
using System.IO;
using System.Reflection;
using System.Configuration;

using Prezza.Framework.Common;
namespace Prezza.Framework.Configuration
{
	/// <summary>
	/// Summary description for ConfigurationFactory.
	/// </summary>
	/// <summary>
	/// Base configuration factory.
	/// </summary>
	/// <remarks>
	/// A developer of configuration-based factories will extend this class.  Currently, the main use of this class
	/// is as a placeholder for future extensions of the framework configuration.
	/// </remarks>
	public abstract class ConfigurationFactory
	{
		/// <summary>
		/// Name of the factory.
		/// </summary>
		private readonly string factoryName;

		/// <summary>
		/// Configuration information for the factory.
		/// </summary>
		private readonly object config;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="factoryName">Name of the factory.</param>
		protected ConfigurationFactory(string factoryName) 
		{
			//Check Arguments
			ArgumentValidation.CheckForEmptyString(factoryName, "factoryName");

			this.factoryName = factoryName;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="factoryName">Name of the factory.</param>
		/// <param name="config">Configuration information for the factory.</param>
		protected ConfigurationFactory(string factoryName, object config)
		{
			//Check Arguments
			ArgumentValidation.CheckForEmptyString(factoryName, "factoryName");
			ArgumentValidation.CheckForNullReference(config, "config");

			this.factoryName = factoryName;
			this.config = config;
		}

		/// <summary>
		/// Get the <see cref="System.Type"/> corresponding to <see cref="Configuration"/> specified by the <paramref name="configurationName" /> parameter.
		/// </summary>
		/// <param name="configurationName">Name of the configuration object to get the Type of.</param>
		/// <returns>System.Type corresponding to the specified <see cref="Configuration"/> object.</returns>
		/// <remarks>
		/// The <see cref="System.Type" /> returned by this method will be used by the factory to instantiate objects.
		/// </remarks>
		protected abstract Type GetConfigurationType(string configurationName);

		/// <summary>
		/// Use TBD.
		/// </summary>
		/// <param name="configurationName"></param>
		/// <param name="ex"></param>
		protected virtual void PublishFailureEvent(string configurationName, Exception ex) 
		{
		}
		
		/// <summary>
		/// Create an instance of the specified item.
		/// </summary>
		/// <param name="configurationName">Name of the <see cref="Configuration" /> object containing information about the object instance to create.</param>
		/// <returns><see cref="System.Type" />.</returns>
		protected virtual object CreateInstance(string configurationName) 
		{
			//Validate argument
			ArgumentValidation.CheckForEmptyString(configurationName, "configurationName");

			try
			{
				Type type = GetConfigurationType(configurationName);
				return CreateObject(configurationName, type);
			}
			catch(System.Exception ex)
			{
				throw ex;
			}
		}

		/// <summary>
		/// Create an object of the specified type.
		/// </summary>
		/// <param name="configurationName">Currently ignored</param>
		/// <param name="type"><see cref="System.Type" /> for the object to create.</param>
		/// <returns>Created object</returns>
		protected virtual object CreateObject(string configurationName, Type type)
		{
			//Validate arguments
			ArgumentValidation.CheckForEmptyString(configurationName, "configurationName");
			ArgumentValidation.CheckForNullReference(type, "type");

			ConstructorInfo constructor = type.GetConstructor(new Type[] {});

			if(constructor == null)
			{
				throw new Exception("Provider does not have a constructor: " + type.FullName);
			}

			object createdObject = null;

			try
			{
				createdObject = constructor.Invoke(null);
			} 
			catch(MethodAccessException ex)
			{
				throw new Exception(ex.Message, ex);
			}
			catch (TargetInvocationException ex)
			{
				throw new Exception(ex.Message, ex);
			}
			catch(TargetParameterCountException ex)
			{
				throw new Exception(ex.Message, ex);
			}
			catch(System.Exception ex)
			{
				throw ex;
			}

			return createdObject;

		}

		/// <summary>
		/// Get the <see cref="System.Type" /> for the specified <paramref name="typeName" />
		/// </summary>
		/// <param name="typeName">Name of type to get the <see cref="System.Type" /> for.</param>
		/// <returns><see cref="System.Type" /> for the specified type.</returns>
		protected Type GetType(string typeName)
		{
			//Check argument
			ArgumentValidation.CheckForEmptyString(typeName, "typeName");

			try
			{
				//Get the type
				// Throw error on failure = true
				// Ignore case = false
				return Type.GetType(typeName, true, false);

			}
			catch(TypeLoadException ex)
			{
				throw new Exception("A type-loading error occurred.  Type was: " + typeName + "   Factory was: " + FactoryName, ex);
			}
			catch(FileNotFoundException ex)
			{
				throw new Exception("An error occurred getting a type.  Type was: " + typeName + "   Factory was: " + FactoryName, ex);
			}
		}

		/// <summary>
		/// Name of the factory.
		/// </summary>
		public string FactoryName
		{
			get{return factoryName;}
		}
		
		/// <summary>
		/// Configuration object associated with the factory.
		/// </summary>
		public object Config
		{
			get{return config;}
		}
	}
}
