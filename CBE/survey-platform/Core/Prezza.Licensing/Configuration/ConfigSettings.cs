////////////////////////////////////////////////////////////////////////////////
#region Copyright © 2002-2005 XHEO, INC. All Rights Reserved.
// This file and its contents are protected by United States and 
// International copyright laws. Unauthorized reproduction and/or 
// distribution of all or any portion of the code contained herein 
// is strictly prohibited and will result in severe civil and criminal 
// penalties. Any violations of this copyright will be prosecuted 
// to the fullest extent possible under law. 
// 
// THE SOURCE CODE CONTAINED HEREIN AND IN RELATED FILES IS PROVIDED 
// TO THE REGISTERED DEVELOPER FOR THE PURPOSES OF EDUCATION AND 
// TROUBLESHOOTING. UNDER NO CIRCUMSTANCES MAY ANY PORTION OF THE SOURCE
// CODE BE DISTRIBUTED, DISCLOSED OR OTHERWISE MADE AVAILABLE TO ANY 
// THIRD PARTY WITHOUT THE EXPRESS WRITTEN CONSENT OF XHEO. 
// 
// UNDER NO CIRCUMSTANCES MAY THE SOURCE CODE BE USED IN WHOLE OR IN 
// PART, AS THE BASIS FOR CREATING A PRODUCT THAT PROVIDES THE SAME, OR 
// SUBSTANTIALLY THE SAME, FUNCTIONALITY AS ANY XHEO PRODUCT. 
// 
// THE REGISTERED DEVELOPER ACKNOWLEDGES THAT THIS SOURCE CODE 
// CONTAINS VALUABLE AND PROPRIETARY TRADE SECRETS OF XHEO, THE 
// REGISTERED DEVELOPER AGREES TO EXPEND EVERY EFFORT TO INSURE ITS 
// CONFIDENTIALITY. 
// 
// THE END USER LICENSE AGREEMENT (EULA) ACCOMPANYING THE PRODUCT 
// PERMITS THE REGISTERED DEVELOPER TO REDISTRIBUTE THE PRODUCT IN 
// EXECUTABLE FORM ONLY IN SUPPORT OF APPLICATIONS WRITTEN USING 
// THE PRODUCT. IT DOES NOT PROVIDE ANY RIGHTS REGARDING THE 
// SOURCE CODE CONTAINED HEREIN. 
// 
// THIS COPYRIGHT NOTICE MAY NOT BE REMOVED FROM THIS FILE. 
#endregion
////////////////////////////////////////////////////////////////////////////////
//
// Class:		ConfigSettings
// Author:		Paul Alexander
// Created:		Friday, November 15, 2002 2:44:51 PM
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Configuration;
using System.Collections;
using System.Collections.Specialized;
using System.Xml;
using System.Runtime.InteropServices;

#if SHARED
namespace Xheo.Configuration
{
	/// <summary>
	/// Helper utility for reading configuration settings.
	/// </summary>
	public sealed class ConfigSettings
	{
#else
	#if LICENSING
namespace Xheo.Licensing.Configuration
{
	/// <summary>
	/// Helper utility for reading configuration settings.
	/// </summary>
	public sealed class ConfigSettings
	{
	#else
namespace Internal
{
	/// <summary>
	/// Implements an large integer of arbitrary length.
	/// </summary>
	internal class ConfigSettings
	{
	#endif
#endif

		////////////////////////////////////////////////////////////////////////////////
		#region Fields

		private static int _iisIsAvailable = -1;
		private static int _threadIsWeb = -1;

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Constructors/Destructors

		/// <summary>
		///	Initializes a new instance of the ConfigSettings class.
		/// </summary>
		private ConfigSettings()
		{
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Properties

		/// <summary>
		/// Indicates if the current request should use Web logic, even if the HttpContext
		/// has not been con
		/// </summary>
		public static bool ShouldUseWebLogic
		{
			get
			{
				if( _threadIsWeb == -1 && System.Web.HttpContext.Current != null )
					_threadIsWeb = 1;
				return _threadIsWeb == 1;	
			}
		}

		/// <summary>
		/// Gets a value indicating if a web request/HttpContext is currently available.
		/// </summary>
		public static bool IsWebRequest
		{
			get
			{
				if( ! IisIsAvailable )
					return false;
				
				if( ! ShouldUseWebLogic )
					return false;
				
				return System.Web.HttpContext.Current != null;
			}
		}

		/// <summary>
		/// Gets a value that indicates if IIS is installed and available on the
		/// machine. If false then HttpContext should not be checked because it will
		/// throw an exception.
		/// </summary>
		public static bool IisIsAvailable
		{
			get
			{
				if( _iisIsAvailable == -1 )
					InitLib();
				return _iisIsAvailable == 1;
			}
		}
		#region Helpers
		/// <summary>
		/// Initialize the library on first use.
		/// </summary>
		internal static void InitLib()
		{
			lock( typeof( ConfigSettings ) )
			{
				_iisIsAvailable = 0;
				try
				{
					try
					{
						if( System.Web.HttpContext.Current != null )
						{
							_iisIsAvailable = 1;
							System.Diagnostics.Debug.Write( "SUCCESS: IIS is available (Shortcut).\r\n" );
							return;
						}
					}
					catch{}

					string dll = System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory() + "aspnet_isapi.dll";

					if( 
						System.IO.File.Exists( dll ) )
					{
						IntPtr hModule	= LoadLibraryExW( dll, IntPtr.Zero, (uint)2 );

						if( hModule == IntPtr.Zero )
							hModule = LoadLibraryExW( dll, IntPtr.Zero, (uint)2 );

						if( hModule != IntPtr.Zero )
						{
							if( FreeLibrary( hModule ) != 0 )
							{
								_iisIsAvailable = 1;
								System.Diagnostics.Debug.Write( "SUCCESS: IIS is available.\r\n" );
							}
						}
					}
				}
				catch{}

				if( _iisIsAvailable == 0 )
					System.Diagnostics.Debug.Write( "ERROR: IIS Not Found!\r\n" );
			}
		}
		#region Helper Methods
		///<summary>
		///Summary of LoadLibraryExW.
		///</summary>
		///<param name="filename"></param>
		///<param name="hFile"></param>
		///<param name="flags"></param>
		///<returns></returns>	
		[ DllImport( "kernel32.dll", SetLastError = true ) ]
		private static extern IntPtr LoadLibraryExW( [ MarshalAs( UnmanagedType.LPWStr ) ] string filename, IntPtr hFile, uint flags );

		///<summary>
		///Summary of FreeLibrary.
		///</summary>
		///<param name="hModule"></param>
		///<returns></returns>	
		[ DllImport( "kernel32.dll", SetLastError = true ) ]
		private static extern int FreeLibrary( IntPtr hModule );
		
		#endregion
		#endregion

		/// <summary>
		/// Gets the path where configuration values should be relative from. In a Web Application
		/// this is the root folder, in any other application it is the launch assembly's 
		/// startup folder.
		/// </summary>
		public static string RootPath
		{
			get
			{
				if( IsWebRequest )
					return System.IO.Path.GetDirectoryName( AppDomain.CurrentDomain.SetupInformation.ConfigurationFile );
				else
					return AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
			}
		}


		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Operations

		/// <summary>
		/// Gets a collection of string settings for a given key in the given section.
		/// </summary>
		/// <param name="name">
		///		Name of the setting to return.
		/// </param>
		/// <param name="section">
		///		Name of the section where the value can be found. May be null. Use forward
		///		slashes to distinguish sub sections.
		/// </param>
		/// <returns>
		///		Returns a collection of values if found, otherwise null.
		/// </returns>
		/// <remarks>
		/// The design of this method allows developers to define configuration settings
		/// in custom configuration sections, or directly in the appSettings section. When
		/// setting values in the appSettings section, key names are prefixed with the prefix
		/// and section name eliminate possible naming collisions. If a value is set in
		/// the appSettings section and a custom section, both values are returned, with the
		/// custom settings first in the array.
		/// </remarks>
		public static string[] GetSettings( string name, string section )
		{
			object		customSection	= ConfigurationManager.GetSection( ( section == null || section.Length == 0 ) ? System.Reflection.Assembly.GetCallingAssembly().GetName().Name : section );
			string[]	appSettings		= ConfigurationManager.AppSettings.GetValues( GetAppSettingsKey( name, section ) );
			string[]	customSettings	= null;

			if( customSection != null )
			{
				if( customSection is NameValueCollection )
				{
					customSettings = ((NameValueCollection)customSection).GetValues( name );
				} 
				else if( customSection is Hashtable )
				{
					if( ((Hashtable)customSection)[ name ] != null )
						customSettings = new string[] { ((Hashtable)customSection)[ name ] as string };
				}
			}

			if( appSettings == null )
				return customSettings;

			if( customSettings == null )
				return appSettings;

			string[] merged = new string[ customSettings.Length + appSettings.Length ];
			int index = 0;

			foreach( string entry in customSettings )
				merged[ index++ ] = entry;

			foreach( string entry in appSettings )
				merged[ index++ ] = entry;

			return merged;
		}
		#region Overrides
		/// <summary>
		/// Gets a collection of string settings for a given key in the given section.
		/// </summary>
		/// <param name="name">
		///		Name of the setting to return.
		/// </param>
		/// <returns>
		///		Returns a collection of values if found, otherwise null.
		/// </returns>
		/// <remarks>
		/// The design of this method allows developers to define configuration settings
		/// in custom configuration sections, or directly in the appSettings section. When
		/// setting values in the appSettings section, key names are prefixed with the prefix
		/// and section name eliminate possible naming collisions. If a value is set in
		/// the appSettings section and a custom section, both values are returned, with the
		/// custom settings first in the array.
		/// </remarks>
		public static string[] GetSettings( string name )
		{
			return GetSettings( name, null );
		}
		#endregion

		/// <summary>
		/// Gets a collection of int settings for a given key in the given section.
		/// </summary>
		/// <param name="name">
		///		Name of the setting to return.
		/// </param>
		/// <param name="section">
		///		Name of the section where the value can be found. May be null. Use forward
		///		slashes to distinguish sub sections.
		/// </param>
		/// <returns>
		///		Returns a collection of values if found, otherwise null.
		/// </returns>
		/// <remarks>
		/// The design of this method allows developers to define configuration settings
		/// in custom configuration sections, or directly in the appSettings section. When
		/// setting values in the appSettings section, key names are prefixed with the prefix
		/// and section name eliminate possible naming collisions. If a value is set in
		/// the appSettings section and a custom section, both values are returned, with the
		/// custom settings first in the array.
		/// </remarks>
		public static int[] GetIntegerSettings( string name, string section )
		{
			object		customSection	= ConfigurationManager.GetSection( ( section == null || section.Length == 0 ) ? System.Reflection.Assembly.GetCallingAssembly().GetName().Name : section );
			string[]	appSettings		= ConfigurationManager.AppSettings.GetValues( GetAppSettingsKey( name, section ) );
			string[]	customSettings	= null;

			if( customSection != null )
			{
				if( customSection is NameValueCollection )
				{
					customSettings = ((NameValueCollection)customSection).GetValues( name );
				} 
				else if( customSection is Hashtable )
				{
					if( ((Hashtable)customSection)[ name ] != null )
						customSettings = new string[] { ((Hashtable)customSection)[ name ] as string };
				}
			}

			if( appSettings == null && customSettings == null )
				return null;

			int[] merged = new int[ ( customSettings == null ? 0 : customSettings.Length ) + ( appSettings == null ? 0 : appSettings.Length ) ];
			int index = 0;

			foreach( string entry in customSettings )
				merged[ index++ ] = XmlConvert.ToInt32( entry );

			foreach( string entry in appSettings )
				merged[ index++ ] = XmlConvert.ToInt32( entry );

			return merged;
		}
		#region Overrides
		/// <summary>
		/// Gets a collection of int settings for a given key in the given section.
		/// </summary>
		/// <param name="name">
		///		Name of the setting to return.
		/// </param>
		/// <returns>
		///		Returns a collection of values if found, otherwise null.
		/// </returns>
		/// <remarks>
		/// The design of this method allows developers to define configuration settings
		/// in custom configuration sections, or directly in the appSettings section. When
		/// setting values in the appSettings section, key names are prefixed with the prefix
		/// and section name eliminate possible naming collisions. If a value is set in
		/// the appSettings section and a custom section, both values are returned, with the
		/// custom settings first in the array.
		/// </remarks>
		public static int[] GetIntegerSettings( string name )
		{
			return GetIntegerSettings( name, null );
		}
		#endregion

		
#if SHARED
		/// <summary>
		/// Gets a collection of TriBool settings for a given key in the given section.
		/// </summary>
		/// <param name="name">
		///		Name of the setting to return.
		/// </param>
		/// <param name="section">
		///		Name of the section where the value can be found. May be null. Use forward
		///		slashes to distinguish sub sections.
		/// </param>
		/// <returns>
		///		Returns a collection of values if found, otherwise null.
		/// </returns>
		/// <remarks>
		/// The design of this method allows developers to define configuration settings
		/// in custom configuration sections, or directly in the appSettings section. When
		/// setting values in the appSettings section, key names are prefixed with the prefix
		/// and section name eliminate possible naming collisions. If a value is set in
		/// the appSettings section and a custom section, both values are returned, with the
		/// custom settings first in the array.
		/// </remarks>
		public static TriBool[] GetTriBoolSettings( string name, string section )
		{
			object		customSection	= ConfigurationManager.GetConfig( ( section == null || section.Length == 0 ) ? System.Reflection.Assembly.GetCallingAssembly().GetName().Name : section );
			string[]	appSettings		= ConfigurationManager.AppSettings.GetValues( GetAppSettingsKey( name, section ) );
			string[]	customSettings	= null;

			if( customSection != null )
			{
				if( customSection is NameValueCollection )
				{
					customSettings = ((NameValueCollection)customSection).GetValues( name );
				} 
				else if( customSection is Hashtable )
				{
					if( ((Hashtable)customSection)[ name ] != null )
						customSettings = new string[] { ((Hashtable)customSection)[ name ] as string };
				}
			}

			if( appSettings == null && customSettings == null )
				return null;

			TriBool[] merged = new TriBool[ ( customSettings == null ? 0 : customSettings.Length ) + ( appSettings == null ? 0 : appSettings.Length ) ];
			int index = 0;

			foreach( string entry in customSettings )
				merged[ index++ ] = TriBool.FromString( entry );

			foreach( string entry in appSettings )
				merged[ index++ ] = TriBool.FromString( entry );

			return merged;
		}
		#region Overloads
		/// <summary>
		/// Gets a collection of TriBool settings for a given key in the given section.
		/// </summary>
		/// <param name="name">
		///		Name of the setting to return.
		/// </param>
		/// <returns>
		///		Returns a collection of values if found, otherwise null.
		/// </returns>
		/// <remarks>
		/// The design of this method allows developers to define configuration settings
		/// in custom configuration sections, or directly in the appSettings section. When
		/// setting values in the appSettings section, key names are prefixed with the prefix
		/// and section name eliminate possible naming collisions. If a value is set in
		/// the appSettings section and a custom section, both values are returned, with the
		/// custom settings first in the array.
		/// </remarks>
		public static TriBool[] GetTriBoolSettings( string name )
		{
			return GetTriBoolSettings( name, null );
		}
		#endregion
#endif

		/// <summary>
		/// Gets a collection of bool settings for a given key in the given section.
		/// </summary>
		/// <param name="name">
		///		Name of the setting to return.
		/// </param>
		/// <param name="section">
		///		Name of the section where the value can be found. May be null. Use forward
		///		slashes to distinguish sub sections.
		/// </param>
		/// <returns>
		///		Returns a collection of values if found, otherwise null.
		/// </returns>
		/// <remarks>
		/// The design of this method allows developers to define configuration settings
		/// in custom configuration sections, or directly in the appSettings section. When
		/// setting values in the appSettings section, key names are prefixed with the prefix
		/// and section name eliminate possible naming collisions. If a value is set in
		/// the appSettings section and a custom section, both values are returned, with the
		/// custom settings first in the array.
		/// </remarks>
		public static bool[] GetBooleanSettings( string name, string section )
		{
			object		customSection	= ConfigurationManager.GetSection( ( section == null || section.Length == 0 ) ? System.Reflection.Assembly.GetCallingAssembly().GetName().Name : section );
			string[]	appSettings		= ConfigurationManager.AppSettings.GetValues( GetAppSettingsKey( name, section ) );
			string[]	customSettings	= null;

			if( customSection != null )
			{
				if( customSection is NameValueCollection )
				{
					customSettings = ((NameValueCollection)customSection).GetValues( name );
				} 
				else if( customSection is Hashtable )
				{
					if( ((Hashtable)customSection)[ name ] != null )
						customSettings = new string[] { ((Hashtable)customSection)[ name ] as string };
				}
			}

			if( appSettings == null && customSettings == null )
				return null;

			bool[] merged = new bool[ ( customSettings == null ? 0 : customSettings.Length ) + ( appSettings == null ? 0 : appSettings.Length ) ];
			int index = 0;

			foreach( string entry in customSettings )
				merged[ index++ ] = XmlConvert.ToBoolean( entry );

			foreach( string entry in appSettings )
				merged[ index++ ] = XmlConvert.ToBoolean( entry );

			return merged;
		}
		#region Overrides
		/// <summary>
		/// Gets a collection of int settings for a given key in the given section.
		/// </summary>
		/// <param name="name">
		///		Name of the setting to return.
		/// </param>
		/// <returns>
		///		Returns a collection of values if found, otherwise null.
		/// </returns>
		/// <remarks>
		/// The design of this method allows developers to define configuration settings
		/// in custom configuration sections, or directly in the appSettings section. When
		/// setting values in the appSettings section, key names are prefixed with the prefix
		/// and section name eliminate possible naming collisions. If a value is set in
		/// the appSettings section and a custom section, both values are returned, with the
		/// custom settings first in the array.
		/// </remarks>
		public static bool[] GetBooleanSettings( string name )
		{
			return GetBooleanSettings( name, null );
		}
		#endregion


		/// <summary>
		/// Gets a collection of decimal settings for a given key in the given section.
		/// </summary>
		/// <param name="name">
		///		Name of the setting to return.
		/// </param>
		/// <param name="section">
		///		Name of the section where the value can be found. May be null. Use forward
		///		slashes to distinguish sub sections.
		/// </param>
		/// <returns>
		///		Returns a collection of values if found, otherwise null.
		/// </returns>
		/// <remarks>
		/// The design of this method allows developers to define configuration settings
		/// in custom configuration sections, or directly in the appSettings section. When
		/// setting values in the appSettings section, key names are prefixed with the prefix
		/// and section name eliminate possible naming collisions. If a value is set in
		/// the appSettings section and a custom section, both values are returned, with the
		/// custom settings first in the array.
		/// </remarks>
		public static decimal[] GetDecimalSettings( string name, string section )
		{
			object		customSection	= ConfigurationManager.GetSection( ( section == null || section.Length == 0 ) ? System.Reflection.Assembly.GetCallingAssembly().GetName().Name : section );
			string[]	appSettings		= ConfigurationManager.AppSettings.GetValues( GetAppSettingsKey( name, section ) );
			string[]	customSettings	= null;

			if( customSection != null )
			{
				if( customSection is NameValueCollection )
				{
					customSettings = ((NameValueCollection)customSection).GetValues( name );
				} 
				else if( customSection is Hashtable )
				{
					if( ((Hashtable)customSection)[ name ] != null )
						customSettings = new string[] { ((Hashtable)customSection)[ name ] as string };
				}
			}

			if( appSettings == null && customSettings == null )
				return null;

			decimal[] merged = new decimal[ ( customSettings == null ? 0 : customSettings.Length ) + ( appSettings == null ? 0 : appSettings.Length ) ];
			int index = 0;

			foreach( string entry in customSettings )
				merged[ index++ ] = XmlConvert.ToDecimal( entry );

			foreach( string entry in appSettings )
				merged[ index++ ] = XmlConvert.ToDecimal( entry );

			return merged;
		}
		#region Overrides
		/// <summary>
		/// Gets a collection of int settings for a given key in the given section.
		/// </summary>
		/// <param name="name">
		///		Name of the setting to return.
		/// </param>
		/// <returns>
		///		Returns a collection of values if found, otherwise null.
		/// </returns>
		/// <remarks>
		/// The design of this method allows developers to define configuration settings
		/// in custom configuration sections, or directly in the appSettings section. When
		/// setting values in the appSettings section, key names are prefixed with the prefix
		/// and section name eliminate possible naming collisions. If a value is set in
		/// the appSettings section and a custom section, both values are returned, with the
		/// custom settings first in the array.
		/// </remarks>
		public static decimal[] GetDecimalSettings( string name )
		{
			return GetDecimalSettings( name, null );
		}
		#endregion

		/// <summary>
		/// Gets the value of the first setting found in the configuration file.
		/// </summary>
		/// <param name="name">
		///		Name of the setting to return.
		/// </param>
		/// <param name="section">
		///		Name of the section where the value can be found. May be null. Use forward
		///		slashes to distinguish sub sections.
		/// </param>
		/// <param name="defaultValue">
		///		The value to return if not found.
		/// </param>
		/// <returns>
		///		Returns the value of the first setting if found, otherwise defaultValue.
		/// </returns>
		public static string GetSetting( string name, string defaultValue, string section )
		{
			string[] settings = GetSettings( name, section );
			if( settings == null )
				return defaultValue;

			return settings[ 0 ];
		}
		#region Overrides

		/// <summary>
		/// Gets the value of the first setting found in the configuration file.
		/// </summary>
		/// <param name="name">
		///		Name of the setting to return.
		/// </param>
		/// <param name="defaultValue">
		///		The value to return if not found.
		/// </param>
		/// <returns>
		///		Returns the value of the first setting if found, otherwise defaultValue.
		/// </returns>
		public static string GetSetting( string name, string defaultValue )
		{
			return GetSetting( name, defaultValue, null );
		}

		#endregion
		#region Typed Overrides

		/// <summary>
		/// Gets the value of the first setting found in the configuration file.
		/// </summary>
		/// <param name="name">
		///		Name of the setting to return.
		/// </param>
		/// <param name="section">
		///		Name of the section where the value can be found. May be null. Use forward
		///		slashes to distinguish sub sections.
		/// </param>
		/// <param name="defaultValue">
		///		The value to return if not found.
		/// </param>
		/// <returns>
		///		Returns the value of the first setting if found, otherwise defaultValue.
		/// </returns>
		public static int GetSetting( string name, int defaultValue, string section )
		{
			string[] settings = GetSettings( name, section );
			if( settings == null )
				return defaultValue;

			return XmlConvert.ToInt32( settings[ 0 ] );
		}
		#region Overrides

		/// <summary>
		/// Gets the value of the first setting found in the configuration file.
		/// </summary>
		/// <param name="name">
		///		Name of the setting to return.
		/// </param>
		/// <param name="defaultValue">
		///		The value to return if not found.
		/// </param>
		/// <returns>
		///		Returns the value of the first setting if found, otherwise defaultValue.
		/// </returns>
		public static int GetSetting( string name, int defaultValue )
		{
			return GetSetting( name, defaultValue, null );
		}

		#endregion

		/// <summary>
		/// Gets the value of the first setting found in the configuration file.
		/// </summary>
		/// <param name="name">
		///		Name of the setting to return.
		/// </param>
		/// <param name="section">
		///		Name of the section where the value can be found. May be null. Use forward
		///		slashes to distinguish sub sections.
		/// </param>
		/// <param name="defaultValue">
		///		The value to return if not found.
		/// </param>
		/// <returns>
		///		Returns the value of the first setting if found, otherwise defaultValue.
		/// </returns>
		public static bool GetSetting( string name, bool defaultValue, string section )
		{
			string[] settings = GetSettings( name, section );
			if( settings == null )
				return defaultValue;

			return XmlConvert.ToBoolean( settings[ 0 ] );
		}
		#region Overrides

		/// <summary>
		/// Gets the value of the first setting found in the configuration file.
		/// </summary>
		/// <param name="name">
		///		Name of the setting to return.
		/// </param>
		/// <param name="defaultValue">
		///		The value to return if not found.
		/// </param>
		/// <returns>
		///		Returns the value of the first setting if found, otherwise defaultValue.
		/// </returns>
		public static bool GetSetting( string name, bool defaultValue )
		{
			return GetSetting( name, defaultValue, null );
		}

		#endregion

#if SHARED
		/// <summary>
		/// Gets the value of the first setting found in the configuration file.
		/// </summary>
		/// <param name="name">
		///		Name of the setting to return.
		/// </param>
		/// <param name="section">
		///		Name of the section where the value can be found. May be null. Use forward
		///		slashes to distinguish sub sections.
		/// </param>
		/// <param name="defaultValue">
		///		The value to return if not found.
		/// </param>
		/// <returns>
		///		Returns the value of the first setting if found, otherwise defaultValue.
		/// </returns>
		public static TriBool GetSetting( string name, TriBool defaultValue, string section )
		{
			TriBool[] settings = GetTriBoolSettings( name, section );
			if( settings == null )
				return defaultValue;

			return settings[ 0 ];
		}
		#region Overloads
		
		/// <summary>
		/// Gets the value of the first setting found in the configuration file.
		/// </summary>
		/// <param name="name">
		///		Name of the setting to return.
		/// </param>
		/// <param name="defaultValue">
		///		The value to return if not found.
		/// </param>
		/// <returns>
		///		Returns the value of the first setting if found, otherwise defaultValue.
		/// </returns>
		public static TriBool GetSetting( string name, TriBool defaultValue )
		{
			return GetSetting( name, defaultValue, null );
		}
		#endregion

#endif

		/// <summary>
		/// Gets the value of the first setting found in the configuration file.
		/// </summary>
		/// <param name="name">
		///		Name of the setting to return.
		/// </param>
		/// <param name="section">
		///		Name of the section where the value can be found. May be null. Use forward
		///		slashes to distinguish sub sections.
		/// </param>
		/// <param name="defaultValue">
		///		The value to return if not found.
		/// </param>
		/// <returns>
		///		Returns the value of the first setting if found, otherwise defaultValue.
		/// </returns>
		public static decimal GetSetting( string name, decimal defaultValue, string section )
		{
			string[] settings = GetSettings( name, section );
			if( settings == null )
				return defaultValue;

			return XmlConvert.ToDecimal( settings[ 0 ] );
		}
		#region Overrides

		/// <summary>
		/// Gets the value of the first setting found in the configuration file.
		/// </summary>
		/// <param name="name">
		///		Name of the setting to return.
		/// </param>
		/// <param name="defaultValue">
		///		The value to return if not found.
		/// </param>
		/// <returns>
		///		Returns the value of the first setting if found, otherwise defaultValue.
		/// </returns>
		public static decimal GetSetting( string name, decimal defaultValue )
		{
			return GetSetting( name, defaultValue, null );
		}

		#endregion


		#endregion

#if ! LICENSING
		/// <summary>
		/// Gets a collection of mandatory settings similar to <see cref="GetSettings"/>. However
		/// if the settings do not exist an exception is thrown.
		/// </summary>
		/// <param name="name">
		///		Name of the setting to return.
		/// </param>
		/// <param name="section">
		///		Name of the section where the value can be found. May be null. Use forward
		///		slashes to distinguish sub sections.
		/// </param>
		/// <returns>
		///		Returns the collection of settings.
		/// </returns>
		public static string[] GetMandatorySettings( string name, string section )
		{
			string[] settings = GetSettings( name, section );

			if( settings == null )
				throw new ApplicationException( String.Format( "Could not find setting '{0}' in <appSettings> or custom configuration section '{1}'", name, section ) );

			return settings;
		}
		#region Overrides
		/// <summary>
		/// Gets a collection of mandatory settings similar to <see cref="GetSettings"/>. However
		/// if the settings do not exist an exception is thrown.
		/// </summary>
		/// <param name="name">
		///		Name of the setting to return.
		/// </param>
		/// <returns>
		///		Returns the collection of settings.
		/// </returns>
		public static string[] GetMandatorySettings( string name )
		{
			return GetMandatorySettings( name, null );
		}
		#endregion

		/// <summary>
		/// Gets a collection of mandatory int settings similar to <see cref="GetSettings"/>. However
		/// if the settings do not exist an exception is thrown.
		/// </summary>
		/// <param name="name">
		///		Name of the setting to return.
		/// </param>
		/// <param name="section">
		///		Name of the section where the value can be found. May be null. Use forward
		///		slashes to distinguish sub sections.
		/// </param>
		/// <returns>
		///		Returns the collection of settings.
		/// </returns>
		public static int[] GetMandatoryIntegerSettings( string name, string section )
		{
			int[] settings = GetIntegerSettings( name, section );

			if( settings == null )
				throw new ApplicationException( String.Format( "Could not find setting '{0}' in <appSettings> or custom configuration section '{1}'", name, section ) );

			return settings;
		}
		#region Overrides
		/// <summary>
		/// Gets a collection of mandatory int settings similar to <see cref="GetSettings"/>. However
		/// if the settings do not exist an exception is thrown.
		/// </summary>
		/// <param name="name">
		///		Name of the setting to return.
		/// </param>
		/// <returns>
		///		Returns the collection of settings.
		/// </returns>
		public static int[] GetMandatoryIntegerSettings( string name )
		{
			return GetMandatoryIntegerSettings( name, null );
		}
		#endregion



		/// <summary>
		/// Gets a collection of mandatory bool settings similar to <see cref="GetSettings"/>. However
		/// if the settings do not exist an exception is thrown.
		/// </summary>
		/// <param name="name">
		///		Name of the setting to return.
		/// </param>
		/// <param name="section">
		///		Name of the section where the value can be found. May be null. Use forward
		///		slashes to distinguish sub sections.
		/// </param>
		/// <returns>
		///		Returns the collection of settings.
		/// </returns>
		public static bool[] GetMandatoryBooleanSettings( string name, string section )
		{
			bool[] settings = GetBooleanSettings( name, section );

			if( settings == null )
				throw new ApplicationException( String.Format( "Could not find setting '{0}' in <appSettings> or custom configuration section '{1}'", name, section ) );

			return settings;
		}
		#region Overrides
		/// <summary>
		/// Gets a collection of mandatory bool settings similar to <see cref="GetSettings"/>. However
		/// if the settings do not exist an exception is thrown.
		/// </summary>
		/// <param name="name">
		///		Name of the setting to return.
		/// </param>
		/// <returns>
		///		Returns the collection of settings.
		/// </returns>
		public static bool[] GetMandatoryBooleanSettings( string name )
		{
			return GetMandatoryBooleanSettings( name, null );
		}
		#endregion

#if SHARED
		/// <summary>
		/// Gets a collection of mandatory TriBool settings similar to <see cref="GetSettings"/>. However
		/// if the settings do not exist an exception is thrown.
		/// </summary>
		/// <param name="name">
		///		Name of the setting to return.
		/// </param>
		/// <param name="section">
		///		Name of the section where the value can be found. May be null. Use forward
		///		slashes to distinguish sub sections.
		/// </param>
		/// <returns>
		///		Returns the collection of settings.
		/// </returns>
		public static TriBool[] GetMandatoryTriBoolSettings( string name, string section )
		{
			TriBool[] settings = GetTriBoolSettings( name, section );

			if( settings == null )
				throw new ApplicationException( String.Format( "Could not find setting '{0}' in <appSettings> or custom configuration section '{1}'", name, section ) );

			return settings;
		}
		#region Overloads
		/// <summary>
		/// Gets a collection of mandatory TriBool settings similar to <see cref="GetSettings"/>. However
		/// if the settings do not exist an exception is thrown.
		/// </summary>
		/// <param name="name">
		///		Name of the setting to return.
		/// </param>
		/// <returns>
		///		Returns the collection of settings.
		/// </returns>
		public static TriBool[] GetMandatoryTriBoolSettings( string name )
		{
			return GetMandatoryTriBoolSettings( name, null );
		}
		#endregion

#endif


		/// <summary>
		/// Gets a collection of mandatory decimal settings similar to <see cref="GetSettings"/>. However
		/// if the settings do not exist an exception is thrown.
		/// </summary>
		/// <param name="name">
		///		Name of the setting to return.
		/// </param>
		/// <param name="section">
		///		Name of the section where the value can be found. May be null. Use forward
		///		slashes to distinguish sub sections.
		/// </param>
		/// <returns>
		///		Returns the collection of settings.
		/// </returns>
		public static decimal[] GetMandatoryDecimalSettings( string name, string section )
		{
			decimal[] settings = GetDecimalSettings( name, section );

			if( settings == null )
				throw new ApplicationException( String.Format( "Could not find setting '{0}' in <appSettings> or custom configuration section '{1}'", name, section ) );

			return settings;
		}
		#region Overrides
		/// <summary>
		/// Gets a collection of mandatory decimal settings similar to <see cref="GetSettings"/>. However
		/// if the settings do not exist an exception is thrown.
		/// </summary>
		/// <param name="name">
		///		Name of the setting to return.
		/// </param>
		/// <returns>
		///		Returns the collection of settings.
		/// </returns>
		public static decimal[] GetMandatoryDecimalSettings( string name )
		{
			return GetMandatoryDecimalSettings( name, null );
		}
		#endregion

		/// <summary>
		/// Gets the first setting found with the given name in the given section. If not
		/// found an exception is thrown.
		/// </summary>
		/// <param name="name">
		///		Name of the setting to return.
		/// </param>
		/// <param name="section">
		///		Name of the section where the value can be found. May be null. Use forward
		///		slashes to distinguish sub sections.
		/// </param>
		/// <returns>
		///		Returns the first setting found.
		/// </returns>
		public static string GetMandatorySetting( string name, string section )
		{
			string[] settings = GetMandatorySettings( name, section );
			return settings[ 0 ];
		}
		#region Overrides
		/// <summary>
		/// Gets the first setting found with the given name in the given section. If not
		/// found an exception is thrown.
		/// </summary>
		/// <param name="name">
		///		Name of the setting to return.
		/// </param>
		/// <returns>
		///		Returns the first setting found.
		/// </returns>
		public static string GetMandatorySetting( string name )
		{
			return GetMandatorySetting( name, null );
		}
		#endregion


		/// <summary>
		/// Gets the first int setting found with the given name in the given section. If not
		/// found an exception is thrown.
		/// </summary>
		/// <param name="name">
		///		Name of the setting to return.
		/// </param>
		/// <param name="section">
		///		Name of the section where the value can be found. May be null. Use forward
		///		slashes to distinguish sub sections.
		/// </param>
		/// <returns>
		///		Returns the first setting found.
		/// </returns>
		public static int GetMandatoryIntegerSetting( string name, string section )
		{
			int[] settings = GetMandatoryIntegerSettings( name, section );
			return settings[ 0 ];
		}
		#region Overrides

		/// <summary>
		/// Gets the first int setting found with the given name in the given section. If not
		/// found an exception is thrown.
		/// </summary>
		/// <param name="name">
		///		Name of the setting to return.
		/// </param>
		/// <returns>
		///		Returns the first setting found.
		/// </returns>
		public static int GetMandatoryIntegerSetting( string name )
		{
			return GetMandatoryIntegerSetting( name, null );
		}
		#endregion

		/// <summary>
		/// Gets the first bool setting found with the given name in the given section. If not
		/// found an exception is thrown.
		/// </summary>
		/// <param name="name">
		///		Name of the setting to return.
		/// </param>
		/// <param name="section">
		///		Name of the section where the value can be found. May be null. Use forward
		///		slashes to distinguish sub sections.
		/// </param>
		/// <returns>
		///		Returns the first setting found.
		/// </returns>
		public static bool GetMandatoryBooleanSetting( string name, string section )
		{
			bool[] settings = GetMandatoryBooleanSettings( name, section );
			return settings[ 0 ];
		}
		#region Overrides
		/// <summary>
		/// Gets the first bool setting found with the given name in the given section. If not
		/// found an exception is thrown.
		/// </summary>
		/// <param name="name">
		///		Name of the setting to return.
		/// </param>
		/// <returns>
		///		Returns the first setting found.
		/// </returns>
		public static bool GetMandatoryBooleanSetting( string name )
		{
			return GetMandatoryBooleanSetting( name, null );
		}
		#endregion

#if SHARED
		/// <summary>
		/// Gets the first TriBool setting found with the given name in the given section. If not
		/// found an exception is thrown.
		/// </summary>
		/// <param name="name">
		///		Name of the setting to return.
		/// </param>
		/// <param name="section">
		///		Name of the section where the value can be found. May be null. Use forward
		///		slashes to distinguish sub sections.
		/// </param>
		/// <returns>
		///		Returns the first setting found.
		/// </returns>
		public static TriBool GetMandatoryTriBoolSetting( string name, string section )
		{
			TriBool[] settings = GetMandatoryTriBoolSettings( name, section );
			return settings[ 0 ];
		}
		#region Overloads
		/// <summary>
		/// Gets the first TriBool setting found with the given name in the given section. If not
		/// found an exception is thrown.
		/// </summary>
		/// <param name="name">
		///		Name of the setting to return.
		/// </param>
		/// <returns>
		///		Returns the first setting found.
		/// </returns>
		public static TriBool GetMandatoryTriBoolSetting( string name )
		{
			return GetMandatoryTriBoolSetting( name, null );
		}
		#endregion
#endif

		/// <summary>
		/// Gets the first decimal setting found with the given name in the given section. If not
		/// found an exception is thrown.
		/// </summary>
		/// <param name="name">
		///		Name of the setting to return.
		/// </param>
		/// <param name="section">
		///		Name of the section where the value can be found. May be null. Use forward
		///		slashes to distinguish sub sections.
		/// </param>
		/// <returns>
		///		Returns the first setting found.
		/// </returns>
		public static decimal GetMandatoryDecimalSetting( string name, string section )
		{
			decimal[] settings = GetMandatoryDecimalSettings( name, section );
			return settings[ 0 ];
		}
		#region Overrides

		/// <summary>
		/// Gets the first decimal setting found with the given name in the given section. If not
		/// found an exception is thrown.
		/// </summary>
		/// <param name="name">
		///		Name of the setting to return.
		/// </param>
		/// <returns>
		///		Returns the first setting found.
		/// </returns>
		public static decimal GetMandatoryDecimalSetting( string name )
		{
			return GetMandatoryDecimalSetting( name, null );
		}
		#endregion

#endif
        
		/// <summary>
		/// Gets the key of the setting when set in the &lt;appSettings&gt; section.
		/// </summary>
		/// <param name="section">
		///		Name of the section. May be null.
		/// </param>
		/// <param name="name">
		///		Name of the key to build.
		/// </param>
		/// <returns>
		///		Returns the complete key name for the appSettings section.
		/// </returns>
		public static string GetAppSettingsKey( string name, string section )
		{
			if( section == null || section.Length == 0 )
				return String.Format( "{0}.{1}", System.Reflection.Assembly.GetCallingAssembly().GetName().Name, name );
			return String.Format( "{0}.{1}", section.Replace( '/', '.' ), name );
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
	} // End class ConfigSettings

	#region ConfigReadWriter
	/// <summary>
	/// Implements a class for reading and writing configuration sections in an 
	/// application's configuration file.
	/// </summary>
#if SHARED || LICENSING
	public
#else
	internal
#endif
	class ConfigReadWriter
	{
		////////////////////////////////////////////////////////////////////////////////
		#region Fields

		private string	_name				= null;
		private Type	_sectionHandlerType	= typeof( System.Configuration.NameValueSectionHandler );
		private bool	_useAppSettings		= false;
		private string	_file				= null;

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Constructors/Destructors

		/// <summary>
		///	Initializes a new instance of the ConfigReadWriter class.
		/// </summary>
		/// <param name="name">
		///		Name of the configuration section.
		/// </param>
		/// <remarks>
		/// You can access sub groups by including forward slasshes in the name. For instance
		/// xheo/web or xheo/data
		/// </remarks>
		public ConfigReadWriter( string name )
		{
			_name = name;
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Properties

		/// <summary>
		/// Gets the name of the configuration section.
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
		}

		/// <summary>
		/// Gets or sets the Type of the section handler used to read the settings in the
		/// configuration section.
		/// </summary>
		protected Type SectionHandlerType
		{
			get
			{
				return _sectionHandlerType;
			}
			set
			{
				_sectionHandlerType = value;
			}
		}

		/// <summary>
		/// Gets or sets a value that indicates if the values should be saved in the 
		/// &lt;appSettings&gt; section of the config.
		/// </summary>
		public bool UseAppSettings
		{
			get
			{
				return _useAppSettings;
			}
			set
			{
				_useAppSettings = value;
			}
		}

		/// <summary>
		/// Gets or sets the name of the file to use. The default is the application configuration file.
		/// </summary>
		public string File
		{
			get
			{
				return _file;
			}
			set
			{
				_file = value;
			}
		}
		
		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Operations

		/// <summary>
		/// Gets a collection of values for the given key name.
		/// </summary>
		/// <param name="name">
		///		The name of the values to retrieve.
		/// </param>
		/// <returns>
		///		Returns a collection of values with the given key name.
		/// </returns>
		public virtual string[] GetValues( string name )
		{
			string			realName	= UseAppSettings ? ConfigSettings.GetAppSettingsKey( name, Name ) : name;
			XmlNode			node		= GetConfigurationNode();
			ArrayList		values		= new ArrayList();

			foreach( XmlNode value in node.SelectNodes( String.Format( System.Globalization.CultureInfo.InvariantCulture, "add[ @key = '{0}']", name ) ) )
				if( value.Attributes[ "value" ] != null )
					values.Add( value.Attributes[ "value" ].InnerText );

			return values.ToArray( typeof( string ) ) as string[];
		}

		/// <summary>
		/// Gets a  value for the given key name.
		/// </summary>
		/// <param name="name">
		///		The name of the value to retrieve.
		/// </param>
		/// <returns>
		///		Returns a value with the given key name.
		/// </returns>
		public virtual string GetValue( string name )
		{
			string[] values = GetValues( name );
			if( values != null && values.Length > 0 )
				return values[ 0 ];
			return null;
		}

		/// <summary>
		/// Sets a collection of values with the same name. Any existing settings
		/// will be replaced.
		/// </summary>
		/// <param name="name">
		///		Name of the setting to set.
		/// </param>
		/// <param name="values">
		///		Values to set.
		/// </param>
		public virtual void SetValues( string name, string[] values )
		{
			RemoveValue( name );
			AddValues( name, values );
		}

		/// <summary>
		/// Sets the value of a setting. If the setting already exists its value is
		/// updated to the new setting.
		/// </summary>
		/// <param name="name">
		///		Name of the setting to set.
		/// </param>
		/// <param name="value">
		///		Value to set.
		/// </param>
		public virtual void SetValue( string name, string value )
		{
			if( value == null )
			{
				RemoveValue( name );
				return;
			}

			string			realName	= UseAppSettings ? ConfigSettings.GetAppSettingsKey( name, Name ) : name;
			XmlNode			node		= GetConfigurationNode();
			XmlNode			valueNode	= node.SelectSingleNode( String.Format( "add[ @key = '{0}' ]", realName ) );
			XmlAttribute	attribute	= null;

			if( valueNode == null )
			{
				valueNode = node.OwnerDocument.CreateElement( "add" );
				attribute = node.OwnerDocument.CreateAttribute( "key" );
				attribute.Value = realName;
				valueNode.Attributes.Append( attribute );
				node.AppendChild( valueNode );
			}

			attribute = valueNode.Attributes[ "value" ];
			if( attribute == null )
			{
				attribute = node.OwnerDocument.CreateAttribute( "value" );
				valueNode.Attributes.Append( attribute );
			}
			attribute.Value = value;

			node.OwnerDocument.Save( ( File == null || File.Length == 0 ) ? AppDomain.CurrentDomain.SetupInformation.ConfigurationFile : File );
		}

		/// <summary>
		/// Adds a new setting to the configuration.
		/// </summary>
		/// <param name="name">
		///		Name of the setting to set.
		/// </param>
		/// <param name="value">
		///		Value to set.
		/// </param>
		public virtual void AddValue( string name, string value )
		{
			if( value == null )
				return;
			string			realName	= UseAppSettings ? ConfigSettings.GetAppSettingsKey( name, Name ) : name;
			XmlNode			node		= GetConfigurationNode();
			XmlNode			valueNode	= node.OwnerDocument.CreateElement( "add" );
			XmlAttribute	attribute	= null;

			attribute = node.OwnerDocument.CreateAttribute( "key" );
			attribute.Value = realName;
			valueNode.Attributes.Append( attribute );
			node.AppendChild( valueNode );
			attribute = node.OwnerDocument.CreateAttribute( "value" );
			attribute.Value = value;
			valueNode.Attributes.Append( attribute );

			node.OwnerDocument.Save( ( File == null || File.Length == 0 ) ? AppDomain.CurrentDomain.SetupInformation.ConfigurationFile : File );
		}


		/// <summary>
		/// Adds a collection of new settings to the configuration.
		/// </summary>
		/// <param name="name">
		///		Name of the setting to set.
		/// </param>
		/// <param name="values">
		///		Values to set.
		/// </param>
		public virtual void AddValues( string name, string[] values )
		{
			if( values == null )
				return;

			foreach( string value in values )
				AddValue( name, value );
		}

		/// <summary>
		/// Removes an existing setting.
		/// </summary>
		/// <param name="name">
		///		Name of the setting to remove.
		/// </param>
		public virtual void RemoveValue( string name )
		{
			string			realName	= UseAppSettings ? ConfigSettings.GetAppSettingsKey( name, Name ) : name;
			XmlNode			node		= GetConfigurationNode();
			
			foreach( XmlNode child in node.SelectNodes( String.Format( "add[ @key = '{0}' ]", realName ) ) )
				node.RemoveChild( child );

			node.OwnerDocument.Save( ( File == null || File.Length == 0 ) ? AppDomain.CurrentDomain.SetupInformation.ConfigurationFile : File );
		}


		/// <summary>
		/// Adds a &lt;remove&gt; setting to remove a predefined setting from  the configuration 
		/// if supported.
		/// </summary>
		/// <param name="name">
		///		Name of the setting to remove.
		/// </param>
		public virtual void AddRemoveKey( string name )
		{
			string			realName	= UseAppSettings ? ConfigSettings.GetAppSettingsKey( name, Name ) : name;
			XmlNode			node		= GetConfigurationNode();
			XmlNode			valueNode	= node.SelectSingleNode( String.Format( "remove[ @key = '{0}' ]", realName ) );
			XmlAttribute	attribute	= null;

			if( valueNode != null )
				return;

			valueNode = node.OwnerDocument.CreateElement( "remove" );
			attribute = node.OwnerDocument.CreateAttribute( "key" );
			attribute.Value = realName;
			valueNode.Attributes.Append( attribute );
			node.InsertBefore( valueNode, node.FirstChild );

			node.OwnerDocument.Save( ( File == null || File.Length == 0 ) ? AppDomain.CurrentDomain.SetupInformation.ConfigurationFile : File );
		}

		/// <summary>
		/// Gets the XmlNode of the configuration section in the configuration file.
		/// </summary>
		/// <returns>
		///		Returns the XmlNode of the configuration section.
		/// </returns>
		protected XmlNode GetConfigurationNode()
		{
			XmlDocument doc = new XmlDocument();
			string file = ( File == null || File.Length == 0 ) ? AppDomain.CurrentDomain.SetupInformation.ConfigurationFile : File;

			if( System.IO.File.Exists( file ) )
				doc.Load( file );

			CreateConfigSection( doc );
			return GetConfigurationSection( doc );
		}

		/// <summary>
		/// Locates and returns the configuration section from the XmlDocument.
		/// </summary>
		/// <param name="doc">
		///		The configuration file as an XmlDocument.
		/// </param>
		/// <returns>
		///		Returns the configuration section in the XmlDocument.
		/// </returns>
		protected XmlNode GetConfigurationSection( XmlDocument doc )
		{
			XmlNode			node1		= null;
			XmlNode			node2		= null;
			string[] sections = Name.Split( '/' );

			node2 = doc.SelectSingleNode( "/configuration" );

			if( UseAppSettings )
			{
				node1 = node2.SelectSingleNode( "appSettings" );
				if( node1 == null )
				{
					node1 = doc.CreateElement( "appSettings" );
					node2.InsertBefore( node1, node2.FirstChild );
				}
				return node1;
			}

			foreach( string section in sections )
			{
				node1 = node2.SelectSingleNode( section );
				if( node1 == null )
				{
					node1 = doc.CreateElement( section );
					node2.AppendChild( node1 );
				}
				node2 = node1;
			}

			return node2;
		}

		/// <summary>
		/// Creates the configuration section definition in the config file if it does
		/// not already exist.
		/// </summary>
		/// <param name="doc"></param>
		protected virtual void CreateConfigSection( XmlDocument doc )
		{
			XmlNode			node1		= null;
			XmlNode			node2		= null;
			XmlAttribute	attribute	= null;
			string			name		= Name;

			// Removed because the config file may not be the config file for the application
			// and will return false positives/negatives.
			//			if( ConfigurationSettings.GetConfig( Name ) != null )
			//				return;

			node1 = doc.SelectSingleNode( "/configuration" );
			if( node1 == null )
			{
				node1 = doc.CreateElement( "configuration" );
				doc.AppendChild( node1 );
			}

			if( UseAppSettings )
			{
				node2 = node1.SelectSingleNode( "appSettings" );
				if( node2 == null )
				{
					node2 = doc.CreateElement( "appSettings" );
					node1.InsertBefore( node2, node1.FirstChild );
				}
				return;
			}

			node2 = node1.SelectSingleNode( "configSections" );
			if( node2 == null )
			{
				node2 = doc.CreateElement( "configSections" );
				node1.InsertBefore( node2, node1.FirstChild );
			}

			if( Name.IndexOf( '/' ) > -1 )
			{
				string[] sections = Name.Split( '/' );
				for( int index = 0; index < sections.Length - 1; index++ )
				{
					string section = sections[ index ];
					node1 = node2.SelectSingleNode( String.Format( "sectionGroup[ @name = '{0}' ]", section ) );
					if( node1 == null )
					{
						node1 = doc.CreateElement( "sectionGroup" );
						attribute = doc.CreateAttribute( "name" );
						attribute.Value = section;
						node1.Attributes.Append( attribute );
						node2.AppendChild( node1 );
					}
					node2 = node1;
				}

				name = sections[ sections.Length - 1 ];
			}

			node1 = node2.SelectSingleNode( String.Format( "section[ @name = '{0}' ]", name ) );
			if( node1 == null )
			{
				node1 = doc.CreateElement( "section" );

				attribute = doc.CreateAttribute( "name" );
				attribute.Value = name;
				node1.Attributes.Append( attribute );

				attribute = doc.CreateAttribute( "type" );
				Type type = SectionHandlerType;
				attribute.Value = String.Format( "{0},{1}", type.FullName, type.Assembly.FullName );
				node1.Attributes.Append( attribute );

				node2.AppendChild( node1 );
			}
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////

	} // End class ConfigReadWriter
	#endregion

} // End namespace Xheo.Configuration

////////////////////////////////////////////////////////////////////////////////
#region Copyright © 2002-2005 XHEO, INC. All Rights Reserved.
//
#endregion
////////////////////////////////////////////////////////////////////////////////
