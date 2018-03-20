//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
//===============================================================================

using System;
using System.Collections.Generic;
using System.Xml;
using System.Collections;
using Prezza.Framework.Common;
using Prezza.Framework.Configuration;
using Prezza.Framework.ExceptionHandling;

namespace Checkbox.Globalization.Configuration
{
	/// <summary>
	///	Configuration object for framework security configuration information, including configuration information
	///	for the various security providers.
	/// </summary>
	public class GlobalizationConfiguration : ConfigurationBase, IXmlConfigurationBase
	{
	    /// <summary>
		/// Collection of text provider configuration objects.
		/// </summary>
        private Hashtable _textProviders;

	    /// <summary>
		/// Constructor.
		/// </summary>
		public GlobalizationConfiguration() : this(string.Empty)
		{
		}

		/// <summary>
		/// Class constructor.
		/// </summary>
		/// <param name="sectionName"></param>
		public GlobalizationConfiguration(string sectionName) : base(sectionName)
		{
			try
			{
				_textProviders = new Hashtable();
				DefaultTextProvider = string.Empty;
			} 
			catch(Exception ex)
			{
				bool rethrow = ExceptionPolicy.HandleException(ex, "BusinessPublic");

				if(rethrow)
					throw;
			}
		}

		/// <summary>
		/// Get the custom configuration object for the specified text provider.
		/// </summary>
		/// <param name="providerName">Name of the text provider.</param>
		/// <returns>Custom configuration object that extends the <see cref="Prezza.Framework.Configuration.ProviderData" /> class.</returns>
		public ProviderData GetTextProviderConfig(string providerName) 
		{
			try
			{
				ArgumentValidation.CheckForEmptyString(providerName, "providerName");

				if(_textProviders.Contains(providerName))
					return (ProviderData)_textProviders[providerName];
			
                throw new Exception("No configuration for specified text provider exists.  Specified provider was: " + providerName);
			}
			catch(Exception ex)
			{
				bool rethrow = ExceptionPolicy.HandleException(ex, "BusinessPublic");

				if(rethrow)
					throw;
			    
                return null;
			}	
		}

		/// <summary>
		/// Load globalization configuration from Xml.
		/// </summary>
		/// <param name="node">Xml node containing globalization information.</param>
		/// <remarks>
		/// In addition to loading globalization information, this method will cause the configurations
		/// for custom providers to be loaded through calls to ConfigurationManager.GetConfiguration (<see cref="Prezza.Framework.Configuration.ConfigurationManager"/>).  The GetConfiguration
		/// method will be called for each child node in the providers sub groups.
		/// </remarks>
		public void LoadFromXml(XmlNode node)
		{
			try
			{
				ArgumentValidation.CheckForNullReference(node, "node");

				//Default language
				DefaultLanguage = XmlUtility.GetNodeText(node.SelectSingleNode("/globalizationConfiguration/defaultLanguage"), true);

				//Text Providers
				_textProviders = new Hashtable();
				
				XmlNodeList textProvidersList = node.SelectNodes("/globalizationConfiguration/providers/provider[@type='text']");

				foreach(XmlNode provider in textProvidersList)
				{
					string providerName = XmlUtility.GetAttributeText(provider, "name");
					string providerConfigPath = XmlUtility.GetAttributeText(provider, "filePath");
					string providerDataTypeName = XmlUtility.GetAttributeText(provider, "configDataType");
					bool isDefault = XmlUtility.GetAttributeBool(provider, "default");
				
					if(providerName == string.Empty)
                        throw new Exception("A text provider was defined in the globalization configuration file, but the name was not specified: " + providerName);

					if(providerConfigPath == string.Empty)
                        throw new Exception("A text provider was defined in the globalization configuration file, but the provider's configuration file path was not specified: " + providerName);

					if(providerDataTypeName == string.Empty)
                        throw new Exception("A text provider was defined in the globalization configuration file, but the provider's configuration object type name was not specified: " + providerName);

					object[] extraParams = {providerName};

                    ProviderData config = (ProviderData)Prezza.Framework.Configuration.ConfigurationManager.GetConfiguration(providerConfigPath, providerDataTypeName, extraParams);

					if(config == null)
                        throw new Exception("Authentication provider [" + providerName + "] was specified but it's configuration could not be loaded: " + providerName);

					_textProviders.Add(config.Name, config);

					//See if this is the default
					if(isDefault)
					{
						DefaultTextProvider = config.Name;
					}
				}

                //Languages
                ApplicationLanguages = new ArrayList();
                XmlNodeList appLangNodes = node.SelectNodes("/globalizationConfiguration/applicationLanguages/language");

                foreach (XmlNode appLangNode in appLangNodes)
                {
                    if (appLangNode.InnerText != null && appLangNode.InnerText.Trim() != string.Empty)
                    {
                        ApplicationLanguages.Add(appLangNode.InnerText);
                    }
                }

                SurveyLanguages = new ArrayList();
                XmlNodeList surveyLangNodes = node.SelectNodes("/globalizationConfiguration/surveyLanguages/language");

                foreach (XmlNode surveyLangNode in surveyLangNodes)
                {
                    if (surveyLangNode.InnerText != null && surveyLangNode.InnerText.Trim() != string.Empty)
                    {
                        SurveyLanguages.Add(surveyLangNode.InnerText);
                    }
                }

                //Locales
                DatePickerLocalizationFiles = new Dictionary<string, string>();
                var locales = node.SelectNodes("/globalizationConfiguration/datePickerLocales/locale");
                
                foreach (XmlNode locale in locales)
                {
                    var language = XmlUtility.GetAttributeText(locale, "language");
                    var file = XmlUtility.GetAttributeText(locale, "file");
                    DatePickerLocalizationFiles.Add(language, file);
                }

                DatePickerDefaultLocalization = XmlUtility.GetNodeText(node.SelectSingleNode("/globalizationConfiguration/datePickerDefaultLocale"));
			} 
			catch(Exception ex)
			{
				bool rethrow = ExceptionPolicy.HandleException(ex, "BusinessPublic");

				if(rethrow)
					throw;
			}
		}

	    /// <summary>
	    /// Default language for globalization
	    /// </summary>
	    public string DefaultLanguage { get; private set; }

	    /// <summary>
	    /// Name of the default text provider (if specified in the configuration file)
	    /// </summary>
	    public string DefaultTextProvider { get; set; }

	    /// <summary>
	    /// Get supported application languages
	    /// </summary>
	    public ArrayList ApplicationLanguages { get; private set; }

	    /// <summary>
	    /// Get supported survey languages
	    /// </summary>
	    public ArrayList SurveyLanguages { get; private set; }

        /// <summary>
        /// Get DatePicker localization files by locale
        /// </summary>
        public IDictionary<string, string> DatePickerLocalizationFiles { get; private set; }

        /// <summary>
        /// Get default DatePicker locale
        /// </summary>
        public string DatePickerDefaultLocalization { get; private set; }
	}
}
