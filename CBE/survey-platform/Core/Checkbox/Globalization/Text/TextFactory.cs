/****************************************************************************
 * Factory to support creation/retrieval of text provider factories.		*
 ****************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using Prezza.Framework.Configuration;
using Prezza.Framework.ExceptionHandling;

using Checkbox.Globalization.Configuration;

namespace Checkbox.Globalization.Text
{
	/// <summary>
	/// Creates and initializes text provider objects.
	/// </summary>
	/// <remarks>
	/// The text factory creates text provider objects that
	/// implement the <see cref="ITextProvider" /> interface.
	/// </remarks>
	internal sealed class TextFactory
	{
		private static readonly Hashtable _providers;

		/// <summary>
		/// Constructor.  Since this class only exposes static methods, there
		/// should be no need to call the constructor.
		/// </summary>
		static TextFactory()
		{
			lock(typeof(TextFactory))
			{
				_providers = new Hashtable();
			}
			
            ConfigurationManager.ConfigurationChanged += ConfigurationManager_ConfigurationChanged;
		}

		/// <summary>
		/// Create and initialize an instance of the default text 
		/// provider. 
		/// </summary>
		/// <returns>Initialized instance of a text provider.</returns>
		/// <remarks>
		/// The returned text provider implements the 
		/// <see cref="ITextProvider" /> interface.</remarks>
		/// <exception cref="Exception">Unable to create default ITextProvider</exception>
		public static ITextProvider GetTextProvider() 
		{
			try
			{
				lock(_providers.SyncRoot)
				{
					if(_providers.Contains("[DEFAULT]"))
					{
						return (ITextProvider)_providers["[DEFAULT]"];
					}
				}

                TextProviderFactory factory = new TextProviderFactory("textProviderFactory", (GlobalizationConfiguration)ConfigurationManager.GetConfiguration("checkboxGlobalizationConfiguration"));
				ITextProvider provider = factory.GetTextProvider();

				lock(_providers.SyncRoot)
				{
					if(!_providers.Contains("[DEFAULT]"))
					{
						_providers.Add("[DEFAULT]", provider);
					}
				}

				return provider;
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
		/// Create and initialize an instance of the specified text 
		/// provider. 
		/// </summary>
		/// <param name="textProvider">Name of the text provider to instantiate and initialize.</param>
		/// <returns>Initialized instance of an text provider.</returns>
		/// <remarks>
		/// The returned text provider implements the 
		/// <see cref="ITextProvider" /> interface.</remarks>
		/// <exception cref="ArgumentNullException">textProvider is null</exception>
		/// <exception cref="ArgumentException">textProvider is empty</exception>
        /// <exception cref="Exception">Could not find instance specified in textProvider</exception>
		/// <exception cref="InvalidOperationException">Error processing configuration information defined in application configuration file.</exception>
		public static ITextProvider GetTextProvider(string textProvider) 
		{
			try
			{
				lock(_providers.SyncRoot)
				{
					if(_providers.Contains(textProvider))
					{
						return (ITextProvider)_providers[textProvider];
					}
				}

                TextProviderFactory factory = new TextProviderFactory("textProviderFactory", (GlobalizationConfiguration)ConfigurationManager.GetConfiguration("checkboxGlobalizationConfiguration"));
				ITextProvider provider = factory.GetTextProvider(textProvider);
				
				lock(_providers.SyncRoot)
				{
					if(!_providers.Contains(textProvider))
					{
						_providers.Add(textProvider, provider);
					}
				}

				return provider;
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
        /// Adds an application language to the cache
        /// </summary>
        /// <param name="language"></param>
        internal static void AddApplicationLanguage(string language)
        {
            if (_ApplicationLanguages == null)
                _ApplicationLanguages = new List<string>();
            if (!_ApplicationLanguages.Contains(language))
            {
                _ApplicationLanguages.Add(language);
            }
        }

        /// <summary>
        /// Removes an application language from the cache
        /// </summary>
        /// <param name="language"></param>
        internal static void RemoveApplicationLanguage(string language)
        {
            if (_ApplicationLanguages == null)
                _ApplicationLanguages = new List<string>();
            if (_ApplicationLanguages.Contains(language))
            {
                _ApplicationLanguages.Remove(language);
            }
        }

        static List<string> _ApplicationLanguages;
        /// <summary>
        /// Get an array list containing <see cref="ISOCode"/> objects representing application languages.
        /// </summary>
        internal static string[] ApplicationLanguages
        {
            get
            {
                if (_ApplicationLanguages != null)
                    return _ApplicationLanguages.ToArray();

                GlobalizationConfiguration config = (GlobalizationConfiguration)ConfigurationManager.GetConfiguration("checkboxGlobalizationConfiguration");

                if (config != null)
                {
                    _ApplicationLanguages = new List<string>((string[])config.ApplicationLanguages.ToArray(typeof(string)));

                    return (string[])config.ApplicationLanguages.ToArray(typeof(string));
                }
                
                return new string[0];
            }
        }

        /// <summary>
        /// Adds an survey language to the cache
        /// </summary>
        /// <param name="language"></param>
        internal static void AddSurveyLanguage(string language)
        {
            if (_SurveyLanguages == null)
                _SurveyLanguages = new List<string>();
            if (!_SurveyLanguages.Contains(language))
            {
                _SurveyLanguages.Add(language);
            }
        }

        /// <summary>
        /// Removes an survey language from the cache
        /// </summary>
        /// <param name="language"></param>
        internal static void RemoveSurveyLanguage(string language)
        {
            if (_SurveyLanguages == null)
                _SurveyLanguages = new List<string>();
            if (_SurveyLanguages.Contains(language))
            {
                _SurveyLanguages.Remove(language);
            }
        }

        static List<string> _SurveyLanguages;
        /// <summary>
        /// Get an array list containing <see cref="ISOCode"/> objects representing application languages.
        /// </summary>
        internal static string[] SurveyLanguages
        {
            get
            {
                if (_SurveyLanguages != null)
                    return _SurveyLanguages.ToArray();

                GlobalizationConfiguration config = (GlobalizationConfiguration)ConfigurationManager.GetConfiguration("checkboxGlobalizationConfiguration");

                if (config != null)
                {
                    _SurveyLanguages = new List<string>((string[])config.SurveyLanguages.ToArray(typeof(string)));

                    return (string[])config.SurveyLanguages.ToArray(typeof(string));
                }

                return new string[0];
            }
        }

        /// <summary>
        /// Get an array list containing the application default language.
        /// </summary>
        internal static string DefaultLanguage
        {
            get
            {
                GlobalizationConfiguration config = (GlobalizationConfiguration)ConfigurationManager.GetConfiguration("checkboxGlobalizationConfiguration");

                if (config != null)
                {
                    return config.DefaultLanguage;
                }
                
                return string.Empty;
            }
        }

		private static void ConfigurationManager_ConfigurationChanged(object sender, ConfigurationChangedEventArgs e)
		{
			if(_providers != null)
				_providers.Clear();
		}
    
        /// <summary>
        /// Clears the configuration cache for Checkbox Globalization section
        /// </summary>
        internal static void RefreshGlobalizationConfiguration()
        {
            //System.Configuration.ConfigurationManager.RefreshSection("checkboxGlobalizationConfiguration"); -- that does not work!!!
        }
    }
}
