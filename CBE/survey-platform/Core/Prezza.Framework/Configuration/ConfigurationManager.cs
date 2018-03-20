//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================

using System;
using System.Xml;
using System.Reflection;
using System.Collections;
using System.Configuration;

using Prezza.Framework.Common;
using Prezza.Framework.ExceptionHandling.Configuration;

namespace Prezza.Framework.Configuration
{
	/// <summary>
	///	Handles loading, storing configuration for the application.
	/// </summary>
	/// <remarks>
	/// This version of the configuration manager supports loading
	/// loading configuration information from .config files such as
	/// web.config.  A developer creating an object that requires 
	/// configuration information should also develop a configuration
	/// section handler that implements the <see cref="System.Configuration.IConfigurationSectionHandler" />
	/// interface.
	/// Additionally, the configuration manager supports instantiating and loading objects that support the
	/// <see cref="Prezza.Framework.Configuration.IXmlConfigurationBase"/> interface.
	/// 
	/// NOTE: Exception handling and logging are only performed if the configurations have been loaded by
	/// another component.  The ConfigurationManager will not autonomously cause those configurations to be
	/// loaded.
	/// </remarks>
	public sealed class ConfigurationManager
	{
		/// <summary>
		/// Cache of created configuration items.
		/// </summary>
		private static readonly Hashtable _configurationCache;
		private static ArrayList _fileWatchers;

		/// <summary>
		/// <para>Occurs after configuration is changed.</para>
		/// </summary>
		public static event ConfigurationChangedEventHandler ConfigurationChanged;
		
		/// <summary>
		/// Cause configurations to be loaded and cached.
		/// </summary>
		static ConfigurationManager()
		{
			_configurationCache = new Hashtable();
			_fileWatchers = new ArrayList();
		}

		/// <summary>
		/// Return a <see cref="Configuration" /> object containing
		/// configuration information for the specified section.
		/// </summary>
		/// <param name="sectionName">Name of the section to load the 
		/// configuration from.</param>
		/// <returns>Configuration object containing the configuration 
		/// information</returns>
		public static object GetConfiguration(string sectionName)
		{
            try
            {
                ArgumentValidation.CheckForEmptyString(sectionName, "sectionName");

                if (_configurationCache.Contains(sectionName))
                {
                    LogMessage("[ConfigurationManager] Getting configuration for " + sectionName + " from the configuration cache.");
                    return _configurationCache[sectionName];
                }
                
                LogMessage("[ConfigurationManager] Loading configuration for " + sectionName + " from the application configuration file.");
                object config = System.Configuration.ConfigurationManager.GetSection(sectionName);

                if (config != null)
                {
                    // Disable the cache as this is cached internally by System.Configuration.ConfigurationSettings
                    //configurationCache.Add(sectionName, config);
                }

                return config;
            }
            catch (ConfigurationErrorsException ex)
            {
                if (ex.InnerException != null)
                {
                    throw ex.InnerException;
                }
                
                ConfigurationFileException newException = new ConfigurationFileException(ex.Message, ex) {FileName = ex.Filename, LineNumber = ex.Line};

                throw newException;
            }
            catch (ConfigurationFileException)
            {
                throw;
            }
            catch (XmlException ex)
            {
                //Build an exception containing the file name
                ConfigurationFileException newException = new ConfigurationFileException(ex.Message, ex) {FileName = ex.SourceUri, LineNumber = ex.LineNumber, LinePosition = ex.LinePosition};

                throw newException;
            }
            catch (Exception ex)
            {
                bool rethrow = HandleException(ex);

                if (rethrow)
                {
                    throw;
                }
                
                return null;
            }
		}

		/// <summary>
		/// Return a <see cref="IXmlConfigurationBase"/> object loaded with the specified xml configuration information.
		/// </summary>
		/// <param name="node">Node containing configuration information.</param>
		/// <param name="typeName">Type of object that implements <see cref="IXmlConfigurationBase"/> to create.</param>
		/// <param name="extraParams">Extra parameters to use to construct the configuration object.</param>
		/// <returns>Loaded configuration object.</returns>
		public static object GetConfiguration(XmlNode node, string typeName, object[] extraParams)
		{
			try
			{
				ArgumentValidation.CheckForNullReference(node, "node");
				ArgumentValidation.CheckForEmptyString(typeName, "typeName");

				Type type = Type.GetType(typeName, true, false);

				ValidateTypeIsIXmlConfigurationBase(type);

				IXmlConfigurationBase xmlConfigLoader = (IXmlConfigurationBase)Activator.CreateInstance(type, extraParams);
				
				xmlConfigLoader.LoadFromXml(node);
				return xmlConfigLoader;
			}
            catch (Exception ex)
            {
                bool rethrow = HandleException(ex);

                if (rethrow)
                {
                    throw;
                }
                
                return null;
            }
		}

        /// <summary>
        /// Determine base application path
        /// </summary>
        /// <returns></returns>
        public static string DetermineAppConfigPath()
        {
            string path = AppDomain.CurrentDomain.RelativeSearchPath ?? System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            return path.Replace("\\bin", "\\config");
        }


	    /// <summary>
		/// Create an object of the specified type, which must implement <see cref="IXmlConfigurationBase"/> inteface, and load
		/// it's configuration from the specified Xml file.
		/// </summary>
		/// <param name="filePath">Path of file containing configuration information.</param>
		/// <param name="typeName">TypeName of object to create and populate with configuration information.</param>
		/// <param name="extraParams">Extra parameters to use to construct the configuration object.</param>
		/// <returns>Object of specified type loaded with configuration information from specified file.</returns>
		public static object GetConfiguration(string filePath, string typeName, object[] extraParams)
		{
            try
            {
                //Check arguments
                ArgumentValidation.CheckForEmptyString(filePath, "filePath");
                ArgumentValidation.CheckForEmptyString(typeName, "typeName");

                //If file paths have no folders specified, attempt to resolve automatically
                if(!filePath.Contains("/")
                    && !filePath.Contains("\\"))
                {
                    filePath = DetermineAppConfigPath() + "\\" + filePath;
                }
                

                //Verify the file exists
                if (!System.IO.File.Exists(filePath))
                {
                    throw new Exception("Unable to load configuration with typeName: " + typeName + " because the specified path: " + filePath + " does not exist.");
                }

                //Attempt to load the file
                XmlDocument doc = new XmlDocument();
                doc.Load(filePath);

                //CreateFileWatcher(filePath);

                return GetConfiguration(doc.DocumentElement, typeName, extraParams);

            }
            catch (ConfigurationErrorsException ex)
            {
                //Build an exception containing the file name
                ConfigurationFileException newException = new ConfigurationFileException(ex.Message, ex) {FileName = filePath, LineNumber = ex.Line};

                throw newException;
            }
            catch (XmlException ex)
            {
                //Build an exception containing the file name
                ConfigurationFileException newException = new ConfigurationFileException(ex.Message, ex) {FileName = filePath, LineNumber = ex.LineNumber, LinePosition = ex.LinePosition};

                throw newException;
            }
            catch (ConfigurationFileException)
            {
                throw;
            }
            catch (Exception ex)
            {
                bool rethrow = HandleException(ex);

                if (rethrow)
                {
                    //If rethrowing, create a new configuration errors exception so the correct error information can be reported
                    ConfigurationFileException newException = new ConfigurationFileException(ex.Message, ex) {FileName = filePath};

                    throw newException;
                }
                
                return null;
            }
		}

		/// <summary>
		/// Validate that the supplied type is compatible with <see cref="IXmlConfigurationBase"/>.
		/// </summary>
		/// <param name="type">Type to verify.</param>
		private static void ValidateTypeIsIXmlConfigurationBase(Type type) 
		{
			ArgumentValidation.CheckForNullReference(type, "type");
            			
			if (!typeof(IXmlConfigurationBase).IsAssignableFrom(type))
			{
				throw new Exception("Type mismatch between Provider type [" + typeof(IXmlConfigurationBase).AssemblyQualifiedName + "] and requested type [" + type.AssemblyQualifiedName);
			}
		}

		/// <summary>
		/// Attempt to log a message.
		/// </summary>
		/// <param name="message">Message to log.</param>
		/// <remarks>
		/// The message will be logged only if the following conditions are met:
		///   1) The configuration cache contains an object of type LoggingConfiguration.
		///   2) The Logger supports a Write method that accepts two strings as arguments.
		/// If the conditions are not met, no message will be logged.
		/// </remarks>
		private static void LogMessage(string message)
		{
			try
			{
                //Turn off logging for now, since it is not generally useful
			    return;

				Type loggingConfigType = Type.GetType("Prezza.Framework.Logging.Configuration.LoggingConfiguration,Prezza.Framework", false, false);

				if(loggingConfigType != null)
				{
					bool bContainsConfig = false;

					//see if the cache contains a logging configuration, so we don't end up in an infinite loop
					lock(_configurationCache.SyncRoot)
					{
						foreach(object itemValue in _configurationCache.Values)
						{
							if(itemValue.GetType().IsAssignableFrom(loggingConfigType))
							{
								bContainsConfig = true;
								break;
							}
						}
					}

					if(bContainsConfig)
					{

						Type loggerConfigType = Type.GetType("Prezza.Framework.Logging.Logger,Prezza.Framework", false, true);

						if(loggerConfigType != null)
						{
							Type[] types = {typeof(string), typeof(string)};
							MethodInfo info = loggerConfigType.GetMethod("Write", types);

							if(info != null)
							{
								object[] extraParams = {message, "Info"};
								info.Invoke(null, extraParams);
							}
						}
					}
				}
			}
			catch
			{
			}
		}

		/// <summary>
		/// Attempt to use the exception handling framework to handle and/or log exceptions.
		/// </summary>
		/// <param name="ex">Exception to handle.</param>
		/// <returns>Boolean indicating whether the calling component should rethrow the exception.</returns>
		/// <remarks>
		/// The exception handling framework will only be used if the configuration cache contains a
		/// configuration object of type ExceptionHandlingConfiguration.
		/// 
		/// If the excpetion is not handled by the framework, this method will return true, otherwise
		/// it will return the value indicated by the exception handling framework.
		/// </remarks>
		private static bool HandleException(Exception ex)
		{
			object[] extraParameters = {ex, "ConfigurationManager"};

			try
			{
				Type exceptionConfigType = Type.GetType("Prezza.Framework.ExceptionHandling.Configuration.ExceptionHandlingConfiguration,Prezza.Framework.ExceptionHandling", false, true);

				if(exceptionConfigType != null)
				{
					bool bContainsFlag = false;

					lock(_configurationCache.SyncRoot)
					{
						foreach(object itemValue in _configurationCache.Values)
						{
							if(itemValue.GetType().IsAssignableFrom(exceptionConfigType))
							{
								bContainsFlag = true;
								break;
							}
						}
					}

					if(bContainsFlag)
					{
						Type exceptionPolicyType = Type.GetType("Prezza.Framework.ExceptionHandling.ExceptionPolicy,Prezza.Framework.ExceptionHandling");

						if(exceptionPolicyType != null)
						{
							Type[] types = {Type.GetType("System.Exception&"), typeof(string)};

							MethodInfo info = exceptionPolicyType.GetMethod("HandleException", types);

							if(info != null)
							{
								bool rethrow = (bool)info.Invoke(null, extraParameters);

								if(rethrow)
									ex = (Exception)extraParameters[0];

								return rethrow;
							}
						}
							
					}
				}
			} 
			catch
			{
				throw ex;
			}

			return true;
		}

		/// <summary>
		/// Creates a ConfigurationChangeFileWatcher on all loaded config files.
		/// </summary>
		/// <param name="filePath"></param>
		private static void CreateFileWatcher(string filePath)
		{
			ConfigurationChangeFileWatcher watcher = new ConfigurationChangeFileWatcher(filePath);
			watcher.ConfigurationChanged += OnConfigurationChanged;
			watcher.StartWatching();
			if(_fileWatchers == null)
				_fileWatchers = new ArrayList();

			if(!_fileWatchers.Contains(watcher))
				_fileWatchers.Add(watcher);
		}

		/// <summary>
		/// Handles the ConfigurationChanged event fired by any ConfigurationChangeFileWatcher monitoring the config files loaded during GetConfiguration
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private static void OnConfigurationChanged(object sender, ConfigurationChangedEventArgs args)
		{
			FlushConfigurationCache();
			if(ConfigurationChanged != null)
			{
				ConfigurationChanged(sender, args);
			}
		}

		/// <summary>
		/// Flush the configuration cache, causing configuration items to be reloaded as necessary
		/// </summary>
		public static void FlushConfigurationCache()
		{
			lock(_configurationCache.SyncRoot)
			{
				_configurationCache.Clear();
			}
		}
	}
}
