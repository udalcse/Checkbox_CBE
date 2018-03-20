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
// Class:		ExtendedLicenseProvider
// Author:		Paul Alexander
// Created:		Friday, September 13, 2002 3:16:27 PM
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Web;
using System.Web.UI;
using System.Web.Caching;
using System.IO;
using System.IO.IsolatedStorage;
using System.Text;
using System.Security.Cryptography;
using System.Xml;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters;
using System.Security.Permissions;
using System.Runtime.Serialization;
using System.Security;
using System.Globalization;
using Internal;
using Microsoft.Win32;

#if LICENSING
using Xheo.Licensing.Configuration;
#else
using Internal;
#endif

namespace Xheo.Licensing
{
	#region ExtendedLicenseProvider
	/// <summary>
	/// Handles resolution of license and issues valid licenses at runtime and design time 
	/// to the LicenseManager.Validate method.
	/// </summary>
	/// <remarks>
	/// To enable licensing for your component you must tag the class with the
	/// <seealso href="../Backgrounders/Probing.html">License File Probing</seealso>
	/// </remarks>
	/// <example>
	/// <code>
	/// [ LicenseProvider( Xheo.Licensing.ExtendedLicenseProvider ) ]
	/// public class MyLicensedControl : Component
	/// {
	///		License _license	= null;
	///		
	///		/// &lt;summary&gt;
	///		/// Initializes a new instance of a MyLicensedControl class.
	///		/// &lt;/summary&gt;
	///		public MyLicensedControl() : base()
	///		{
	///			_license = LicenseManager.Validate( typeof( MyLicensedControl ), this );
	///		}
	///		
	///		/// &lt;summary&gt;
	///		/// Implements &lt;see cref="IDisposable.Dispose"/&gt;.
	///		/// &lt;/summary&gt;
	///		public override void Dispose()
	///		{
	///			base.Dispose();
	///			Dispose( true );
	///			GC.SuppressFinalize( this );
	///		}
	///		
	///		/// &lt;summary&gt;
	///		/// Performs disposal of manage &amp; unmanged resources in the control
	///		/// &lt;/summary&gt;
	///		protected virtual void Dispose( bool disposing )
	///		{
	///			if( disposing )
	///			{
	///				if( _license != null )
	///				{
	///					_license.Dispose();
	///					_license = null;
	///				}
	///			}
	///		}
	/// }
	/// </code>
	/// </example>
#if LICENSING
	public 
#else
	internal
#endif
		class ExtendedLicenseProvider : LicenseProvider
	{
		////////////////////////////////////////////////////////////////////////////////
		#region Fields

		private static Hashtable _cachedLicenses		= new Hashtable();
		private static Hashtable _cachedKeys			= new Hashtable();
		private static Hashtable _cachedProbed			= new Hashtable();
		private static NameValueCollection	_watcherMap	= new NameValueCollection();
		private static Hashtable _watchers				= new Hashtable();

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Constructors/Destructors

		/// <summary>
		///	Initializes a new instance of the ExtendedLicenseProvider class.
		/// </summary>
		public ExtendedLicenseProvider()
		{
		}

		/// <summary>
		/// Initialize the library on first use.
		/// </summary>
		static ExtendedLicenseProvider()
		{
			ExtendedLicense.InitLib();
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Properties
		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Operations

		/// <summary>
		/// Resets the cache for any licenses used by the given Type. Use to "unlock"
		/// an application/component when a license is selected manually.
		/// </summary>
		/// <param name="type">
		///		The Type to reset.
		/// </param>
		/// <remarks>
		///		After calling this method, the next time a license for the given Type
		///		is requested, XHEO|Licensing will consider any new available license
		///		even if a trial version was granted before.
		/// </remarks>
#if LICENSING
		public 
#else
		internal
#endif
			static void ResetCacheForType( Type type )
		{
			string isolation	= type.Assembly.FullName;
			string key			= type.FullName;

			lock( typeof( ExtendedLicenseProvider ) )
			{
				if( _cachedLicenses[ key ] != null )
					_cachedLicenses.Remove( key );

				key = isolation + type.FullName;
				if( _cachedLicenses[ key ] != null )
					_cachedLicenses.Remove( key );

				key = isolation + type.Assembly.FullName + ".*";
				if( _cachedLicenses[ key ] != null )
					_cachedLicenses.Remove( key );
			}
		}

		/// <summary>
		/// Any cached instance of the given license is removed and any Types that 
		/// were granted that license in the past may receive a new license.
		/// </summary>
		/// <param name="license">
		///		The license to remove from the cache.
		/// </param>
		/// <remarks>
		///		After calling this method, the next time a license for the given Type
		///		is requested, XHEO|Licensing will consider any new available license
		///		even if a trial version was granted before.
		/// </remarks>
#if LICENSING
		public 
#else
		internal
#endif
			static void ResetCacheForLicense( ExtendedLicense license )
		{
			lock( typeof( ExtendedLicenseProvider ) )
			{
				foreach( ExtendedLicense lic in _cachedLicenses.Values )
				{
					if( lic.AbsoluteSerialNumber == license.AbsoluteSerialNumber &&
						lic.LicensePack.Location == license.LicensePack.Location )
						_cachedLicenses.Remove( lic );
				}
			}
		}

		/// <summary>
		/// Overrides <see cref="LicenseProvider.GetLicense"/>.
		/// </summary>
		/// <remarks>
		///		Called by the <see cref="LicenseManager"/> when validating a protected
		///		Type. 
		/// </remarks>
#if ! DEBUG
		[ DebuggerHidden ]
#endif
		public override License GetLicense( LicenseContext context, Type type, object instance, bool allowExceptions )
		{
			if( type == null )
				throw new ArgumentNullException( "type" );

			StringBuilder		invalidReason	= new StringBuilder();
			CultureInfo			savedCulture	= null;

			ExtendedLicenseContext thecontext = new ExtendedLicenseContext( context );

			try
			{
				savedCulture = Limit.SelectWebCulture();

				LicenseSigningKey	key				= null;
				ExtendedLicense		cachedLicense	= null;

				cachedLicense = GetCachedLicense( thecontext, type, instance );
				if( cachedLicense != null )
					return cachedLicense;

				lock( type )
				{
					key	= GetPublicKey( type );
					ExtendedLicense foundLicense = null;
					ArrayList delayLicenses = new ArrayList();

					// Check stored license
					foundLicense = FindSavedLicense( thecontext, type, instance, key, delayLicenses, invalidReason );
					if( foundLicense != null )
						return foundLicense;

					foreach( string path in GetSearchPaths( thecontext, type ) )
					{
						StringCollection files = null;
						try
						{
							files = GetFiles( path );
						}
						catch( Exception ex )
						{
							invalidReason.Append( Internal.StaticResourceProvider.CurrentProvider.FormatResourceString( "E_InvalidSearchPath", path, FailureReportForm.PreProcessException( ex ) ) );
							invalidReason.Append( "\r\n\r\n" );
							continue;
						}


						if( files.Count == 0 )
						{
							invalidReason.Append( Internal.StaticResourceProvider.CurrentProvider.FormatResourceString( "E_NoLicensesFound", path ) );
							invalidReason.Append( "\r\n\r\n" );
							continue;
						}

						foreach( string file in  files )
						{
							ExtendedLicensePack licenses = null;

							try
							{
								licenses = new ExtendedLicensePack( file, true );
							}
							catch( Exception ex )
							{
								invalidReason.Append( Internal.StaticResourceProvider.CurrentProvider.FormatResourceString( "E_ErrorLoadingPack", file, FailureReportForm.PreProcessException( ex ) ) );
								invalidReason.Append( "\r\n\r\n" );
								continue;
							}

							foundLicense = TryPack( thecontext, type, instance, licenses, key, false, delayLicenses, invalidReason );
							if( foundLicense != null )
								return foundLicense;
						}

					}


					foundLicense = TryPack( thecontext, type, instance, delayLicenses, key, true, null, invalidReason );
					if( foundLicense != null )
						return foundLicense;

					ExtendedLicensePack embeddedPack = GetEmbeddedLicense( type );
					delayLicenses.Clear();

					if( embeddedPack != null )
					{
						foundLicense = TryPack( thecontext, type, instance, embeddedPack, key, false, delayLicenses, invalidReason );
						if( foundLicense != null )
							return foundLicense;
					

						foundLicense = TryPack( thecontext, type, instance, delayLicenses, key, true, null, invalidReason );
						if( foundLicense != null )
							return foundLicense;
					}

#if LICENSING || ( LICENSETRIALS && LICENSESERVERS )
					if( ! thecontext.IsTrialShown && ! ExtendedLicense.DisableTrials )
					{
						foreach( string trialLicenseServer in GetTrialServers( thecontext, type ) )
						{
							try
							{
								string trialLicenseXml	= null;
								int index				= trialLicenseServer.IndexOf( ',' );
								string extra			= index == -1 ? null : trialLicenseServer.Substring( index + 1 );
								ExternalLicenseServer.XheoLicensingServer server = new ExternalLicenseServer.XheoLicensingServer();
								server.Url = index == -1 ? trialLicenseServer : trialLicenseServer.Substring( 0, index );
								server.Proxy = ExtendedLicense.Proxy;
								server.LicenseCultureSoapHeaderValue = new Xheo.Licensing.ExternalLicenseServer.LicenseCultureSoapHeader();
								server.LicenseCultureSoapHeaderValue.CultureName = System.Globalization.CultureInfo.CurrentUICulture.Name;

								bool retry = false;

								do
								{
									retry = false;
									try
									{
										try
										{
											trialLicenseXml = server.GetTrialLicense( extra, type.Assembly.GetName().Name, MachineProfile.Profile.GetComparableHash() );
										} 
										catch( System.Web.Services.Protocols.SoapException )
										{
											server.Proxy = System.Net.GlobalProxySelection.GetEmptyWebProxy();
											trialLicenseXml = server.GetTrialLicense( extra, type.Assembly.GetName().Name, MachineProfile.Profile.GetComparableHash() );
										}
									}
									catch( Exception ex )
									{
										retry = ProxyForm.RetryException( null, ex, server.Url );

										if( ! retry )
										{
											invalidReason.Append( Internal.StaticResourceProvider.CurrentProvider.FormatResourceString( "E_GetTrialFailure", FailureReportForm.PreProcessException( ex ) ) );
											invalidReason.Append( "\r\n\r\n" );
											continue;
										}
									}
								} while( retry );
								ExtendedLicensePack pack = new ExtendedLicensePack();
								pack.FromXmlString( trialLicenseXml );
								try
								{
									string fileName = "AutoTrialLicense.lic";
									if( pack.MetaValues[ "SuggestedSaveLocation" ] != null )
										fileName = pack.MetaValues[ "SuggestedSaveLocation" ];
									else if( pack.SaveToShared )
										fileName = Path.Combine( ExtendedLicense.SharedFolder, fileName );
									string path = Path.Combine( AppDomain.CurrentDomain.BaseDirectory, fileName );

									
									pack._location = path;
									pack.Save(  path, true );
								}
								catch{}
								finally{}

								foundLicense = TryPack( thecontext, type, instance, pack, key, true, null, invalidReason );
								if( foundLicense != null )
									return GrantLicense( thecontext, type, instance, foundLicense );

							}
							catch( Exception ex )
							{
								invalidReason.Append( Internal.StaticResourceProvider.CurrentProvider.FormatResourceString( "E_GetTrialUnexpectedFailure", FailureReportForm.PreProcessException( ex.InnerException == null ? ex : ex.InnerException ) ) );
								invalidReason.Append( "\r\n\r\n" );								
							}
						}
					}
#endif
				}
			}
			catch( Exception ex )
			{
				if( allowExceptions )
				{
					SupportInfo supportInfo = LicenseHelpAttribute.GetSupportInfo( type );
				
					if( supportInfo.ShowWindow && ! ExtendedLicense.IsWebRequest && ! FailureReportForm.DontShowAgain )
					{
						FailureReportForm.Show( thecontext, type, instance, Internal.StaticResourceProvider.CurrentProvider.FormatResourceString( "E_UnexpectedValidate", 
#if DEBUG
							ex.ToString(),
#else
							ex.Message,
#endif
							invalidReason.ToString() ) );
					}
				
					throw new LicenseException( 
						type, 
						instance, 
						Internal.StaticResourceProvider.CurrentProvider.FormatResourceString( "E_UnexpectedErrorValidate", 
						type.Name,
						supportInfo.Company,
						supportInfo.Url,
						supportInfo.Email,
						supportInfo.Phone, 
#if DEBUG
						ex.ToString() ), 
#else
						ex.Message ), 
#endif
						ex );				
				}
				
				return null;
			}
			finally
			{
				if( savedCulture != null )
					System.Threading.Thread.CurrentThread.CurrentUICulture = savedCulture;
			}

			if( allowExceptions )
			{
				SupportInfo supportInfo = LicenseHelpAttribute.GetSupportInfo( type );

				if( supportInfo.ShowWindow && ! ExtendedLicense.IsWebRequest && ! FailureReportForm.DontShowAgain && ! thecontext.IsTrialShown )
					FailureReportForm.Show( thecontext, type, instance, invalidReason.ToString() );

				throw new LicenseException( 
					type, 
					instance, 
					Internal.StaticResourceProvider.CurrentProvider.FormatResourceString( "E_NoLicenseEx", 
					type.Name,
					supportInfo.Company,
					supportInfo.Url,
					supportInfo.Email,
					supportInfo.Phone, 
					invalidReason.ToString() )
					);	
			}

			return null;
		}
		#region Helper Methods

		private ExtendedLicense TryPack( LicenseContext context, Type type, object instance, ICollection licenses, LicenseSigningKey key, bool allowDelayed, ArrayList delayLicenses, StringBuilder invalidReason )
		{
			if( licenses is ExtendedLicensePack && ((ExtendedLicensePack)licenses).MetaValues[ "OriginalLocation" ] != null )
			{
				try
				{
					if( ! File.Exists( ((ExtendedLicensePack)licenses).MetaValues[ "OriginalLocation" ] ) )
						return null;
				}
				catch{}
			}

			ArrayList trialLicenses = new ArrayList();
			
			if( licenses is ExtendedLicensePack )
			{
				string location = ((ExtendedLicensePack)licenses).Location;
				licenses = ((ExtendedLicensePack)licenses).GetLicensesForComponent( type.FullName );
				if( licenses.Count == 0 )
				{
					invalidReason.Append( Internal.StaticResourceProvider.CurrentProvider.FormatResourceString( "E_NoComponentInLicense", type.FullName, location ) );
					invalidReason.Append( "\r\n\r\n" );
					return null;
				}
			}

			foreach( ExtendedLicense license in licenses )
			{
				if( key.ValidateLicense( license ) )
				{
#if LICENSING || LICENSETRIALS
					if( license.IsTrial && ExtendedLicense.DisableTrials )
					{
						invalidReason.Append( Internal.StaticResourceProvider.CurrentProvider.FormatResourceString( "E_SkipTrial", license.LicensePack.Location ) );
						invalidReason.Append( "\r\n\r\n" );
						continue;
					}
#endif

					if( ( ( license.UsesGui() && ! allowDelayed ) 
#if LICENSING || LICENSETRIALS
						|| license.IsTrial 
#endif
						) && delayLicenses != null && ! license.IsUnlocked )
					{
#if LICENSING || LICENSETRIALS
						if( allowDelayed && license.IsTrial )
							trialLicenses.Add( license );
						else 
#endif
							if( delayLicenses != null )
							delayLicenses.Add( license );
					}
					else
					{
#if LICENSING || LICENSETRIALS
						if( license.IsTrial && allowDelayed && ! license.IsUnlocked )
						{
							trialLicenses.Add( license );
						}
						else
#endif
						{
							ExtendedLicense lic = TryLicense( license, context, type, instance, key, invalidReason );
							if( lic != null )
								return lic;
						}
					}
				}
				else
				{
					invalidReason.Append( Internal.StaticResourceProvider.CurrentProvider.FormatResourceString( "E_InvalidSignatureIn", license.LicensePack.Location ) );
					invalidReason.Append( "\r\n\r\n" );
				}
			}

			if( allowDelayed && trialLicenses.Count > 0 )
			{
				foreach( ExtendedLicense license in trialLicenses )
				{
					ExtendedLicense lic = TryLicense( license, context, type, instance, key, invalidReason );
					if( lic != null )
						return lic;
				}
			}
			return null;
		}
		private ExtendedLicense TryLicense( ExtendedLicense license, LicenseContext context, Type type, object instance, LicenseSigningKey key, StringBuilder invalidReason )
		{
			license._publicKey = key;
			license.LicensePack._publicKey = key;
			if( license.Validate( context, type, instance, true ) )
			{
				return GrantLicense( context, type, instance, license ) as ExtendedLicense;
			}
			else
			{
				ExtendedLicense surrogate = ProcessSurrogate( license, key, context, type, instance, invalidReason );
				if( surrogate != null )
					return surrogate;

				if( license.UnlockedLicense != null )
				{

					if( key.ValidateLicense( license.UnlockedLicense ) )
					{
						ExtendedLicense lic = TryLicense( license.UnlockedLicense, context, type, instance, key, invalidReason );
						if( lic != null )
							return lic;
					}
					else
					{
						invalidReason.Append( Internal.StaticResourceProvider.CurrentProvider.FormatResourceString( "E_InvalidSignatureIn", license.LicensePack.Location ) );
						invalidReason.Append( "\r\n\r\n" );
					}

				}
				else
				{
					invalidReason.Append( Internal.StaticResourceProvider.CurrentProvider.FormatResourceString( "E_CouldNotValidate", license.LicensePack.Location, license.InvalidReason, license.SerialNumber  ) );
					invalidReason.Append( "\r\n\r\n" );
				}
			}
			
			return null;
		}

		

		///<summary>
		///Summary of ProcessSurrogate.
		///</summary>
		///<param name="originalLicense"></param>
		///<param name="key"></param>
		///<param name="context"></param>
		///<param name="type"></param>
		///<param name="instance"></param>
		///<param name="invalidReason"></param>
		///<returns></returns>	
		private ExtendedLicense ProcessSurrogate( ExtendedLicense originalLicense, LicenseSigningKey key, LicenseContext context, Type type, object instance, StringBuilder invalidReason )
		{
			if( originalLicense.SurrogateLicensePack != null )
			{
				ExtendedLicensePack surrogatePack = new ExtendedLicensePack( originalLicense.SurrogateLicensePack, true );
				surrogatePack._publicKey = originalLicense.PublicKey;

				ArrayList delay = new ArrayList();
				ExtendedLicense surrogate = TryPack( context, type, instance, surrogatePack, originalLicense.PublicKey, false, delay, invalidReason );

				if( surrogate != null )
					return surrogate;

				return TryPack( context, type, instance, delay, originalLicense.PublicKey, true, null, invalidReason );
			}

			return null;
		}
		///<summary>
		///Summary of GetCachedLicense.
		///</summary>
		///<param name="context"></param>
		///<param name="type"></param>
		///<param name="instance"></param>
		///<returns></returns>	
		private ExtendedLicense GetCachedLicense( LicenseContext context, Type type, object instance )
		{
			ExtendedLicense cachedLicense = null;
			LicenseHelper	helper = LicenseHelper.GetHelperForAssembly( type.Assembly );
			
			string isolation	= null; 
			string key			= null;

			if( helper != null && helper.ProvidesIsolationHelp )
				isolation = helper.GetIsolationKey( type, instance );
			
			if( isolation == null )
				isolation = instance != null ? instance.GetType().Assembly.FullName : null;

			key = isolation + type.FullName;

			cachedLicense = _cachedLicenses[ key ] as ExtendedLicense;
		
			if( cachedLicense == null )
			{
				key = isolation + type.Assembly.FullName + ".*";
				cachedLicense = _cachedLicenses[ key ] as ExtendedLicense;
			}

			if( cachedLicense == null )
				return null;

			if( cachedLicense.PeriodicChecks == null )
			{
				PeriodicLicenseChecksAttribute attribute = Attribute.GetCustomAttribute( type, typeof( PeriodicLicenseChecksAttribute ), true ) as PeriodicLicenseChecksAttribute;

				if( attribute == null )
					attribute = new PeriodicLicenseChecksAttribute( 1000 * 10 * 60 );

				cachedLicense.PeriodicChecks = attribute;
			}

			PeriodicLicenseChecksAttribute.ElapsedResponse response = cachedLicense.PeriodicChecks.Elapsed( cachedLicense );

			if( response != PeriodicLicenseChecksAttribute.ElapsedResponse.No )
			{
				if( ! cachedLicense.Validate( context, type, instance, ( response & PeriodicLicenseChecksAttribute.ElapsedResponse.Remote ) ==  PeriodicLicenseChecksAttribute.ElapsedResponse.Remote ) )
				{
					_cachedLicenses.Remove( key );
					return null;
				}
			}

			{
				try
				{
					cachedLicense.Limits.Granted( context, type, instance );
					cachedLicense.SaveOnValid();
					if( context.UsageMode == LicenseUsageMode.Designtime 
#if LICENSING || LICENSETRIALS
						&& ! cachedLicense.IsTrial 
#endif
						)
						context.SetSavedLicenseKey( type, cachedLicense.LicenseKey );
				}
				catch
				{
					_cachedLicenses.Remove( key );
					return null;
				}
			

				return cachedLicense.Duplicate();
			}

			//return null;
		}
		#endregion

		
		/// <summary>
		/// Performs any caching of the license and update before it is returned to the caller.
		/// </summary>
		/// <param name="context">
		///		The context from <see cref="GetLicense"/>.
		/// </param>
		/// <param name="type">
		///		The type from <see cref="GetLicense"/>.
		/// </param>
		/// <param name="instance">
		///		The instance from <see cref="GetLicense"/>.
		/// </param>
		/// <param name="license">
		///		The License to be granted.
		/// </param>
		/// <returns>
		///		Returns a cached version of the license.
		/// </returns>
		private License GrantLicense( LicenseContext context, Type type, object instance, ExtendedLicense license )
		{
			LicenseHelper	helper = LicenseHelper.GetHelperForAssembly( type.Assembly );
			string isolation	= null; 
			string key			= null;

			if( helper != null && helper.ProvidesIsolationHelp )
				isolation = helper.GetIsolationKey( type, instance );
			
			if( isolation == null )
				isolation = instance != null ? instance.GetType().Assembly.FullName : null;

			if( license.Components.Contains( "*" ) )
				key = isolation + type.Assembly.FullName + ".*";
			else
				key = isolation + type.FullName;

			_cachedLicenses[ key ] = license;

			if( ! license.IsEmbedded && license.LicensePack.Location != null && ! license.LicensePack.Location.StartsWith( "isolated:" ) )
				WatchFile( key, license.LicensePack.Location );

			if( context.UsageMode == LicenseUsageMode.Designtime 
#if LICENSING || LICENSETRIALS
				&& ! license.IsTrial 
#endif
				)
				context.SetSavedLicenseKey( type, license.LicenseKey );
			license.SetReadOnly();
			license.Limits.Granted( context, type, instance );
			ExtendedLicense duplicate = license.Duplicate();
			return duplicate;
		}

		/// <summary>
		/// Gets the Public key embedded in the assembly hosting the given type.
		/// </summary>
		/// <param name="type">
		///		The reference type to use for obtaining the assembly.
		/// </param>
#if LICENSING
		public 
#else
		internal
#endif
			static LicenseSigningKey GetPublicKey( Type type )
		{
			LicenseSigningKey	key				= null;

			key = _cachedKeys[ type.Assembly.FullName ] as LicenseSigningKey;
			if( key != null )
				return key;
			string resourceName = null;

			using( Stream stream = GetEmbeddedResource( type, type.Assembly.GetName().Name + ".plsk", out resourceName ) )
			{			
				try
				{
					if( stream != null )
						key = new LicenseSigningKey( stream );
				}
				catch{}
			}

			if( key == null )
				throw new LicenseException( type, null, 
					Internal.StaticResourceProvider.CurrentProvider.FormatResourceString( "E_NoPublicKey",
					type.Assembly.GetName().Name,
					type.Namespace, 
					type.Assembly.GetName().Name ) );
			_cachedKeys[ type.Assembly.FullName ] = key;
			return key;
		}
		#region Helpers
		private static Stream GetEmbeddedResource( Type type, string name, out string foundResourceName )
		{
			string typeName = type.Namespace;
			string searchName = name;
			foundResourceName = null;
			
			if( typeName == null )
				typeName = "";
            
			do
			{
				if( typeName.Length > 0 )
					searchName = typeName + '.' + name;
				else
					searchName = name;
#if DEBUG
				System.Diagnostics.Debug.WriteLine( "Searching for: " + searchName );
#endif
				Stream stream = type.Assembly.GetManifestResourceStream( searchName );
				if( stream != null )
				{
					foundResourceName = searchName;
					return stream;
				}

				if( typeName.Length == 0 )
					break;

				int lastPeriod = typeName.IndexOf( '.' );
				if( lastPeriod > -1 )
					typeName = typeName.Substring( 0, lastPeriod );
				else
					typeName = "";
			}while( true );

			// Fall back and look through the whole thing
			Stream extensionBasedStream = null;	
			Stream result = null;
			string extension = System.IO.Path.GetExtension( name );

			try
			{
				foreach( string resourceName in type.Assembly.GetManifestResourceNames() )
				{
					if( resourceName.EndsWith( name ) )
					{
						result = type.Assembly.GetManifestResourceStream( resourceName );
						foundResourceName = resourceName;
						return result;
					}
					else if( resourceName.EndsWith( extension ) )
					{
						extensionBasedStream = type.Assembly.GetManifestResourceStream( resourceName );
						result = extensionBasedStream;
						foundResourceName = resourceName;
					}
				}
			}
			finally
			{
				if( extensionBasedStream != null && extensionBasedStream != result )
					extensionBasedStream.Close();
			}

			return extensionBasedStream;
		}

		#endregion

		/// <summary>
		/// Gets the runtime.lic embedded in the assembly hosting the given type.
		/// </summary>
		/// <param name="type">
		///		The reference type to use for obtaining the assembly.
		/// </param>
#if LICENSING
		public 
#else
		internal
#endif
			static ExtendedLicensePack GetEmbeddedLicense( Type type )
		{
			string resourceName;

			using( Stream stream = GetEmbeddedResource( type, "runtime.lic", out resourceName ) )
			{
				if( stream != null )
				{
					StreamReader reader = new StreamReader( stream );
					string xml = reader.ReadToEnd();
					reader.Close();

					ExtendedLicensePack pack = new ExtendedLicensePack();
					
					pack.FromXmlString( xml );
					pack._isEmbedded = true;
				
					foreach( ExtendedLicense license in pack )
						license.IsEmbedded = true;

					pack._location = new Uri( type.Assembly.CodeBase ).LocalPath + ".licenses." + Limit.GetCompatibleHashCode( type.Namespace == null ? "" : type.Namespace ).GetHashCode().ToString( "X", System.Globalization.CultureInfo.InvariantCulture );

					if( File.Exists( pack._location ) || ExtendedLicensePack.IsfFileExists( pack._location ) )
						pack.Load( pack._location, true );

					return pack;
				}
			}

			return null;
		}

#if LICENSING || ( LICENSETRIALS && LICENSESERVERS )
		/// <summary>
		/// Gets a collection of trial servers to request licenses from.
		/// </summary>
		private StringCollection GetTrialServers( LicenseContext context, Type type )
		{
			StringCollection servers = new StringCollection();
			TrialServerAttribute[] trialAttributes = Attribute.GetCustomAttributes( type.Assembly, typeof( TrialServerAttribute ), true ) as TrialServerAttribute[];
			foreach( TrialServerAttribute trialAttribute in trialAttributes )
			{
				if( trialAttribute.Extra != null && trialAttribute.Extra.Length > 0 )
					servers.Add( trialAttribute.ServerUrl + ',' + trialAttribute.Extra );
				else
					servers.Add( trialAttribute.ServerUrl );
			}

			RegistryKey optionsKey = null;
			try
			{
				string product = "";
				AssemblyProductAttribute attribute = Attribute.GetCustomAttribute( type.Assembly, typeof( AssemblyProductAttribute ), true ) as AssemblyProductAttribute;
				if( attribute != null )
					product = attribute.Product;
				else
					product = type.Assembly.GetName().Name;

				optionsKey = Registry.CurrentUser.OpenSubKey( "Software\\Xheo\\Licensing\\" + product  );

				if( optionsKey != null )
				{
					foreach( string valueName in optionsKey.GetSubKeyNames() )
					{
						if( valueName.StartsWith( "TrialServer" ) )
						{
							using( RegistryKey serverKey = optionsKey.OpenSubKey( valueName ) )
							{
								string trialServer = serverKey.GetValue( null ) as string; // (Default)
								if( serverKey.GetValue( "Extra" ) != null )
									trialServer += ',' + ( serverKey.GetValue( "Extra" ) as string );
								servers.Add( trialServer );
							}
						}
					}

					optionsKey.Close();
					optionsKey = null;
				}


				optionsKey = Registry.LocalMachine.OpenSubKey( "Software\\Xheo\\Licensing\\" + product  );

				if( optionsKey != null )
				{
					foreach( string valueName in optionsKey.GetSubKeyNames() )
					{
						if( valueName.StartsWith( "TrialServer" ) )
						{
							using( RegistryKey serverKey = optionsKey.OpenSubKey( valueName ) )
							{
								string trialServer = serverKey.GetValue( null ) as string; // (Default)
								if( serverKey.GetValue( "Extra" ) != null )
									trialServer += ',' + ( serverKey.GetValue( "Extra" ) as string );
								servers.Add( trialServer );
							}
						}
					}
					optionsKey.Close();
					optionsKey = null;
				}
			}
			catch{}
			finally
			{
				if( optionsKey != null )
					optionsKey.Close();
			}

			return servers;
		}
#endif

		/// <summary>
		/// Gets the license files in the given path.
		/// </summary>
		private StringCollection GetFiles( string path )
		{
			StringCollection collection = new StringCollection();
			collection.AddRange( Directory.GetFiles( path, "*.LIC" ) );
			collection.AddRange( Directory.GetFiles( path, "*.lic" ) );

			try
			{
				IsolatedStorageFile isf = ExtendedLicensePack.GetIsfStore();
				string searchPath = Path.Combine( ExtendedLicensePack.IsfGetDirectoryName( Path.GetDirectoryName( path ) ), "*.lic" );
				if( ExtendedLicensePack.IsfDirectoryExists( Path.GetDirectoryName( path ) ) )
				{
					foreach( string filename in isf.GetFileNames( searchPath ) )
					{
						string isolatedFile = Path.Combine( path, filename );
						if( collection.Contains( isolatedFile ) )
							collection.Remove( isolatedFile );
						collection.Insert( 0, "isolated:" + isolatedFile );
					}
				}
			}
			catch{}

			for( int index = 0; index < collection.Count; index++ )
				if( ! collection[ index ].ToLower( System.Globalization.CultureInfo.InvariantCulture ).EndsWith( ".lic" ) &&
					! collection[ index ].ToLower( System.Globalization.CultureInfo.InstalledUICulture ).EndsWith( ".lic.xml" ) )
				{
					collection.RemoveAt( index );
					index--;
				}

			return collection;
		}

		/// <summary>
		/// Gets a collection of paths to search for licenses in.
		/// </summary>
		private StringCollection GetSearchPaths( LicenseContext context, Type type )
		{
			System.Collections.Specialized.StringCollection paths = new System.Collections.Specialized.StringCollection();

			if( context == null )
				return null;

			if( type == null )
				return null;

			AddPath( paths, AppDomain.CurrentDomain.SetupInformation.DynamicBase );

			if( context.UsageMode == LicenseUsageMode.Designtime )
			{
				ITypeResolutionService	service = context.GetService( typeof( ITypeResolutionService ) ) as ITypeResolutionService;
				if( service != null )
				{
					AddPath( paths, Path.GetDirectoryName( service.GetPathOfAssembly( type.Assembly.GetName() ) ) );

					if( Assembly.GetEntryAssembly() != null )
						AddPath( paths, Path.GetDirectoryName( service.GetPathOfAssembly( Assembly.GetEntryAssembly().GetName() ) ) );
					
					if( Assembly.GetCallingAssembly() != null )
						AddPath( paths, Path.GetDirectoryName( service.GetPathOfAssembly( Assembly.GetCallingAssembly().GetName() ) ) );
				}
			}

			if( !StaticResourceProvider.IsDynamic(type.Assembly))
				AddPath( paths, Path.GetDirectoryName( new Uri( type.Assembly.CodeBase ).LocalPath ) );

			if( Assembly.GetEntryAssembly() != null )
				AddPath( paths, Path.GetDirectoryName( new Uri( Assembly.GetEntryAssembly().CodeBase ).LocalPath ) );

            if (Assembly.GetCallingAssembly() != null && !StaticResourceProvider.IsDynamic(Assembly.GetCallingAssembly()))
				AddPath( paths, Path.GetDirectoryName( new Uri( Assembly.GetCallingAssembly().CodeBase ).LocalPath ) );

			AddPath( paths, AppDomain.CurrentDomain.BaseDirectory );
			AddPath( paths, AppDomain.CurrentDomain.RelativeSearchPath );
			AddPath( paths, AppDomain.CurrentDomain.SetupInformation.ApplicationBase );
			AddPath( paths, AppDomain.CurrentDomain.SetupInformation.CachePath );

			// In case IIS is not installed, 
			try
			{
				if( ExtendedLicense.IsWebRequest )
				{
					AddPath( paths, HttpContext.Current.Request.PhysicalApplicationPath );
					AddPath( paths, HttpContext.Current.Request.PhysicalApplicationPath + "\\bin" );
				}
			}
			catch{}

			if( AppDomain.CurrentDomain.SetupInformation.PrivateBinPath != null )
				foreach( string path in AppDomain.CurrentDomain.SetupInformation.PrivateBinPath.Split( ';' ) )
					AddPath( paths, path.Trim() );

			if( AppDomain.CurrentDomain.SetupInformation.PrivateBinPathProbe != null )
				foreach( string path in AppDomain.CurrentDomain.SetupInformation.PrivateBinPathProbe.Split( ';' ) )
				{
					if( path.IndexOf( '*' ) == -1 )
						AddPath( paths, path.Trim() );
				}

			foreach( string path in ExtendedLicense.LicenseFolders )
				AddPath( paths, path );


			// Look for shared folder
			AddPath( paths, ExtendedLicense.SharedFolder );

			return paths;
		}
		#region Helper Methods
		/// <summary>
		/// Adds the path if doesn't already exist in the collection.
		/// </summary>
		private void AddPath( StringCollection collection, string path )
		{
			try
			{
				if( path == null )
					return;
				// HACK: for Web Matrix that returns assembly path as file:/[location]
				string actualPath = path.ToLower( System.Globalization.CultureInfo.InvariantCulture );
				if( path.StartsWith( "file:" ) )
				{
					int slash = path.IndexOf( "/" );
					if( slash == -1 )
						slash = path.IndexOf( "\\" );

					if( path[ slash + 1 ] == '/' || path[ slash + 1 ] == '\\' )
						slash++;
					actualPath = path.Substring( slash + 1 );
				}
				
				foreach(string subPath in path.Split(';'))
				{
					string fullPath = Path.GetFullPath( subPath );
					if ( ! fullPath.EndsWith( Path.DirectorySeparatorChar.ToString() ) )
							fullPath += Path.DirectorySeparatorChar;
				
					if( ! System.IO.Directory.Exists( fullPath ) )
						return;
					if( ! collection.Contains( fullPath ) )
					{
						collection.Add( fullPath );
					}
				}
			}
			catch{}
		}
		#endregion

		#region File Watcher Support

		private static void FSW_OnChanged( object source, FileSystemEventArgs e )
		{
			InvalidateFile( e.FullPath );
		}

		private static void FSW_OnRenamed( object source, RenamedEventArgs e )
		{
			InvalidateFile( e.FullPath );
		}

		private static void InvalidateFile( string path )
		{
			if( _watcherMap[ path ] != null )
			{
				lock( typeof( ExtendedLicenseProvider ) )
				{
					if( _watcherMap[ path ] != null )
					{
						foreach( string typeName in _watcherMap.GetValues( path ) )
						{
							License license = _cachedLicenses[ typeName ] as License;
							if( license != null )
								license.Dispose();
							_cachedLicenses.Remove( typeName );
						}

						_watcherMap.Remove( path );
					}
				}				
			}
		}

		private static void WatchFile( string typeName, string path )
		{
			if( Environment.OSVersion.Platform == PlatformID.Win32NT  || Environment.OSVersion.Platform == PlatformID.Unix )
			{
				lock( typeof( ExtendedLicenseProvider ) )
				{
					FileSystemWatcher watcher = _watchers[ Path.GetDirectoryName( path ) ] as FileSystemWatcher;
					if( watcher == null )
					{
						watcher = new FileSystemWatcher( Path.GetDirectoryName( path ), "*.lic*" );
						watcher.IncludeSubdirectories = false;
						watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.Size;
						watcher.Changed += new FileSystemEventHandler( FSW_OnChanged );
						watcher.Renamed += new RenamedEventHandler( FSW_OnRenamed );
						watcher.EnableRaisingEvents = true;
					}
				}

				_watcherMap[ path ] = typeName;
			}
		}
		
		#endregion


	    private static Hashtable _savedLicenses = new Hashtable();
		private ExtendedLicense FindSavedLicense( LicenseContext context, Type type, object instance, LicenseSigningKey key, ArrayList delayLicenses, StringBuilder invalidReason )
		{
			foreach( Assembly assembly in AppDomain.CurrentDomain.GetAssemblies() )
			{
				if(StaticResourceProvider.IsDynamic(assembly))
					continue;

				Hashtable saved = null;
				string resourceName = HttpUtility.UrlDecode( assembly.CodeBase.Substring( assembly.CodeBase.LastIndexOf( '/' ) + 1 ) );
				saved = _savedLicenses[ assembly.CodeBase ] as Hashtable;

				if( saved == null )
				{
					lock( typeof( ExtendedLicenseProvider ) )
					{
						if( saved == null )
						{
							Stream stream = assembly.GetManifestResourceStream( resourceName + ".licenses" );
							if( stream == null )
								stream = GetManifestResourceStreamCI( assembly, resourceName + ".licenses" );
							if( stream == null )
								continue;
				
							try
							{
								saved = Deserialize( stream, resourceName.ToUpper( System.Globalization.CultureInfo.InvariantCulture ) );
								if( saved == null )
									continue;
					
								_savedLicenses[ assembly.CodeBase ] = saved;
							}
							finally
							{
								stream.Close();
							}
						}
					}
				}

				string xml = saved[ type.AssemblyQualifiedName ] as string;
				if( xml == null )
					xml = FindVersionCompatibleLicense( saved, type.AssemblyQualifiedName );
				if( xml == null )
					continue;
				ExtendedLicensePack pack = new ExtendedLicensePack();
				pack.FromXmlString( xml );
				pack._location = new Uri( assembly.CodeBase ).LocalPath + ".licenses." + Limit.GetCompatibleHashCode( type.AssemblyQualifiedName ).ToString( "X", System.Globalization.CultureInfo.InvariantCulture );

				if( File.Exists( pack.Location ) || ExtendedLicensePack.IsfFileExists( pack.Location ) )
					pack.Load( pack.Location, true );
				else
					foreach( ExtendedLicense lic in pack )
						lic.IsEmbedded = true;

				ExtendedLicense license = TryPack( context, type, instance, pack, key, false, delayLicenses, invalidReason );
				if( license != null )
					return license;				

			}
			
			return null;
		}
		#region Helpers

		private string FindVersionCompatibleLicense( Hashtable strings, string key )
		{
			key = this.MakeVersionSrippedKey( key );

			foreach( string searchKey in strings.Keys )
				if( String.Compare( MakeVersionSrippedKey( searchKey ), key, true, System.Globalization.CultureInfo.InvariantCulture ) == 0 )
					return strings[ searchKey ] as string;

			return null;
		}

		private string MakeVersionSrippedKey( string key )
		{
			int index = key.IndexOf( "Version=" );
			if( index == -1 )
				return key;
			int index2 = key.IndexOf( ',', index );
			if( index2 == -1 )
				return key.Substring( 0, index -2 );
			return key.Remove( index - 2, index2 - index + 2 );
		}

		private Stream GetManifestResourceStreamCI( Assembly assembly, string resourceName )
		{
			string rn = resourceName.ToLower( System.Globalization.CultureInfo.InvariantCulture );
			foreach( string actualName in assembly.GetManifestResourceNames() )
			{
				if( actualName.ToLower( System.Globalization.CultureInfo.InvariantCulture ).EndsWith( rn ) )
					return assembly.GetManifestResourceStream( actualName );
			}

			return null;
		}

		private Hashtable Deserialize( Stream o, string cryptoKey ) 
		{
			IFormatter formatter = new BinaryFormatter();

			object obj;

			obj = formatter.Deserialize(o);

			if( obj is object[] ) 
			{
				object[] value = (object[])obj;
				if( value[0] is string && (string)value[0] == cryptoKey ) 
				{
					return value[ 1 ] as Hashtable;
				}
			}

			return null;
		}

		#endregion

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
	} // End class ExtendedLicenseProvider
	#endregion

	#region LicenseHelper
	/// <summary>
	/// Serves as the base class for an assembly helper that can allow developers
	/// to offer unlock by serial number services where the serial number can not be
	/// provided by the user such as in an ASP.NET environment.
	/// </summary>
	/// <remarks>
	///		Use LicenseHelperAttribute to set the helper for an assembly.
	/// </remarks>
#if LICENSING
	public 
#else
	internal
#endif
		abstract class LicenseHelper
	{
		////////////////////////////////////////////////////////////////////////////////
		#region Fields
		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Constructors/Destructors

		/// <summary>
		///	Initializes a new instance of the LicenseHelper class.
		/// </summary>
		protected LicenseHelper()
		{
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Properties

		/// <summary>
		/// Gets a value that indicates if the helper provides <see cref="SupportInfo"/>
		/// for the assembly and overrides the <see cref="GetSupportInfo"/> method.
		/// </summary>
		public abstract bool ProvidesSupportInfo
		{
			get;
		}

		/// <summary>
		/// Gets a value that indicates if the helper can provide serial numbers for
		/// the assembly and overrides the <see cref="GetSerialNumber"/> method.
		/// </summary>
		public abstract bool ProvidesSerialNumber
		{
			get;
		}

		/// <summary>
		/// Gets a value that indicates if the helper provides license isolation help
		/// for determining how licenses are shared between assemblies in the same
		/// process.
		/// </summary>
		public virtual bool ProvidesIsolationHelp
		{
			get
			{
				return false;
			}
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Operations

		/// <summary>
		/// When overriddedn in a derived class, gets a serial number for the given license.
		/// The license can be stored in any external source to provided by the host
		/// assembly (such as registry, configuration file, etc.)
		/// </summary>
		/// <param name="license">
		///		The license to get the serial number for.
		/// </param>
		/// <returns>
		///		Returns a serial number if found, otherwise null.
		/// </returns>
		public virtual string GetSerialNumber( ExtendedLicense license )
		{
			return null;
		}

		/// <summary>
		/// When overridden in a derived class, gets additional support information for 
		/// the given license. 
		/// </summary>
		/// <param name="license">
		///		The license to get support info for. Caution, may be null.
		/// </param>
		/// <returns>
		///		Returns the SupportInfo for the given license if provided. Otherwise for
		///		the current assembly.
		/// </returns>
		public virtual SupportInfo GetSupportInfo( ExtendedLicense license )
		{
			return new SupportInfo();
		}

		/// <summary>
		/// When overridden in a derived class, gets a key that isolates the license
		/// for the given type when the type is used by more than one assembly in 
		/// the same process.
		/// </summary>
		/// <param name="type">
		///		The type that requires a license.
		/// </param>
		/// <param name="instance">
		///		The instance of that type (or a derived type) that is requesting the license.
		/// </param>
		/// <returns>
		///		Returns a key that uniquely identifies the isolation level for the type
		///		and assembly.
		/// </returns>
		/// <remarks>
		///		Normally XHEO|Licensing will track validated licenses per assembly so that
		///		a license granted for a given type in one assembly is not necessarily considered
		///		valid for a different assembly in the same process. The isolation key is used
		///		to uniquely identify the context for the type and instance. If the key returned
		///		is null, then the default isolation rules will be applied.
		/// </remarks>
		public virtual string GetIsolationKey( Type type, object instance )
		{
			return null;
		}

		internal static LicenseHelper GetHelperForAssembly( Assembly hostAssembly )
		{
			LicenseHelperAttribute attr = Attribute.GetCustomAttribute( hostAssembly, typeof( LicenseHelperAttribute ), true ) as LicenseHelperAttribute;
			if( attr == null )
				return null;
			return Activator.CreateInstance( attr.HelperType ) as LicenseHelper;
		}

#if ! LICENSING
		protected internal virtual Limit GetLimitFromName( string name )
		{
			return null;
		}
#endif

		#endregion
		////////////////////////////////////////////////////////////////////////////////
	} // End class LicenseHelper

	#region Internal helper
#if LICENSING
	internal class XheoLicenseHelper : Xheo.Licensing.LicenseHelper
	{
		public override bool ProvidesSerialNumber
		{
			get
			{
				return true;
			}
		} 

		public override bool ProvidesSupportInfo
		{
			get
			{
				return false;
			}
		}


		public override string GetSerialNumber(Xheo.Licensing.ExtendedLicense license)
		{
			return ConfigSettings.GetSetting( "SerialNumber", (string)null, Xheo.Licensing.ExtendedLicense.ConfigSection );
		}
	}
#endif
	#endregion

	#region LicenseHelperAttribute
	/// <summary>
	/// Provides the Type of the <see cref="LicenseHelper"/> that provides on demand
	/// support info and external serial number services.
	/// </summary>
	[ AttributeUsage( AttributeTargets.Assembly, AllowMultiple = true, Inherited = true ) ]
#if LICENSING
	public
#else
	internal
#endif
		sealed class LicenseHelperAttribute : Attribute
	{
		////////////////////////////////////////////////////////////////////////////////
		#region Fields

		private Type	_helperType;

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Constructors/Destructors

		/// <summary>
		///	Initializes a new instance of the LicenseHelpAttribute class.
		/// </summary>
		public LicenseHelperAttribute( Type helperType )
		{
			_helperType = helperType;
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Properties

		/// <summary>
		/// Gets the Type of the LicenseHelper for an the assembly.
		/// </summary>
		public Type HelperType
		{
			get
			{
				return _helperType;
			}
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Operations


		#endregion
		////////////////////////////////////////////////////////////////////////////////
	} // End class LicenseHelpAttribute
	#endregion

	#endregion

	#region LicenseHelpAttribute
	/// <summary>
	/// Declares contact information for users that need help with a license. Used when
	/// reporting license exceptions.
	/// <seealso cref="LicenseHelperAttribute"/>
	/// </summary>
	[ AttributeUsage( AttributeTargets.Class | AttributeTargets.Assembly, AllowMultiple = true, Inherited = true ) ]
#if LICENSING
	public
#else
	internal
#endif
		sealed class LicenseHelpAttribute : Attribute
	{
		////////////////////////////////////////////////////////////////////////////////
		#region Fields

		private SupportInfo _info	= new SupportInfo();

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Constructors/Destructors

		/// <summary>
		///	Initializes a new instance of the LicenseHelpAttribute class.
		/// </summary>
		public LicenseHelpAttribute()
		{
			_info.Company	= null;
			_info.Email		= "N/A";
			_info.Phone		= "N/A";
			_info.Url		= "N/A";
			_info.ShowWindow		= true;
			_info.IncludeAssemblies	= true;
			_info.IncludeDetails	= true;
			_info.IncludeSystemInfo	= true;
			_info.Product			= null;
		}

		/// <summary>
		///  Initializes a new instance of the LicenseHelpAttribute class.
		/// </summary>
		/// <param name="email">
		///		Email address for support requests.
		/// </param>
		/// <param name="url">
		///		URL of a website for support.
		/// </param>
		/// <param name="company">
		///		Company that created the component.
		/// </param>
		/// <param name="phone">
		///		Phone number for support requests.
		/// </param>
		/// <param name="showWindow">
		///		Indicates if a popup window should display the error message.
		/// </param>
		public LicenseHelpAttribute( string company, string email, string url, string phone, bool showWindow )
		{
			_info.Company	= company;
			_info.Email		= email;
			_info.Phone		= phone;
			_info.Url		= url;
			_info.ShowWindow		= showWindow;
			_info.IncludeAssemblies	= true;
			_info.IncludeDetails	= true;
			_info.IncludeSystemInfo	= true;
			_info.Product			= null;
		}

		/// <summary>
		///  Initializes a new instance of the LicenseHelpAttribute class.
		/// </summary>
		/// <param name="email">
		///		Email address for support requests.
		/// </param>
		/// <param name="url">
		///		URL of a website for support.
		/// </param>
		/// <param name="company">
		///		Company that created the component.
		/// </param>
		/// <param name="phone">
		///		Phone number for support requests.
		/// </param>
		public LicenseHelpAttribute( string company, string email, string url, string phone )
		{
			_info.Company	= company;
			_info.Email		= email;
			_info.Phone		= phone;
			_info.Url		= url;
			_info.ShowWindow		= true;
			_info.IncludeAssemblies	= true;
			_info.IncludeDetails	= true;
			_info.IncludeSystemInfo	= true;
			_info.Product			= null;
		}

		/// <summary>
		///  Initializes a new instance of the LicenseHelpAttribute class.
		/// </summary>
		/// <param name="email">
		///		Email address for support requests.
		/// </param>
		/// <param name="url">
		///		URL of a website for support.
		/// </param>
		/// <param name="company">
		///		Company that created the component.
		/// </param>
		public LicenseHelpAttribute( string company, string email, string url )
		{
			_info.Company	= company;
			_info.Email		= email;
			_info.Phone		= null;
			_info.Url		= url;
			_info.ShowWindow		= true;
			_info.IncludeAssemblies	= true;
			_info.IncludeDetails	= true;
			_info.IncludeSystemInfo	= true;
			_info.Product			= null;
		}

		/// <summary>
		///  Initializes a new instance of the LicenseHelpAttribute class.
		/// </summary>
		/// <param name="email">
		///		Email address for support requests.
		/// </param>
		/// <param name="company">
		///		Company that created the component.
		/// </param>
		public LicenseHelpAttribute( string company, string email )
		{
			_info.Company	= company;
			_info.Email		= email;
			_info.Phone		= null;
			_info.Url		= null;
			_info.ShowWindow		= true;
			_info.IncludeAssemblies	= true;
			_info.IncludeDetails	= true;
			_info.IncludeSystemInfo	= true;
			_info.Product			= null;
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Properties

		/// <summary>
		/// Gets or sets an email address where support questions should be directed.
		/// </summary>
		public string Email
		{
			get
			{
				return _info.Email;
			}
			set
			{
				_info.Email = value;
			}
		}

		/// <summary>
		/// Gets or sets a URL where support is available.
		/// </summary>
		public string Url
		{
			get
			{
				return _info.Url;
			}
			set
			{
				_info.Url = value;
			}
		}

		/// <summary>
		/// Gets or sets the name of the company that created the component.
		/// </summary>
		public string Company
		{
			get
			{
				return _info.Company;
			}
			set
			{
				_info.Company = value;
			}
		}

		/// <summary>
		/// Gets or sets a phone number where support is available.
		/// </summary>
		public string Phone
		{
			get
			{
				return _info.Phone;
			}
			set
			{
				_info.Phone = value;
			}
		}

		/// <summary>
		/// Gets or sets the name of the product as it should appear in licensing forms.
		/// </summary>
		public string Product
		{
			get
			{
				return _info.Product;
			}
			set
			{
				_info.Product = value;
			}
		}

		/// <summary>
		/// Gets or sets a value that indicates if a window should be used to display the
		/// support info.
		/// </summary>
		public bool ShowWindow
		{
			get
			{
				return _info.ShowWindow;
			}
			set
			{
				_info.ShowWindow = value;
			}
		}

		/// <summary>
		/// Gets or sets a value that indicates if the report window should include details
		/// about the failure.
		/// </summary>
		public bool IncludeDetails
		{
			get
			{
				return _info.IncludeDetails;
			}
			set
			{
				_info.IncludeDetails = value;
			}
		}

		/// <summary>
		/// Gets or sets a value that indicates if the report window should include a 
		/// list of loaded assemblies.
		/// </summary>
		public bool IncludeAssemblies
		{
			get
			{
				return _info.IncludeAssemblies;
			}
			set
			{
				_info.IncludeAssemblies = value;
			}
		}

		/// <summary>
		/// Gets or sets a value that indicates if the report window should include additional
		/// system information.
		/// </summary>
		public bool IncludeSystemInfo
		{
			get
			{
				return _info.IncludeSystemInfo;
			}
			set
			{
				_info.IncludeSystemInfo = value;
			}
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Operations

		/// <summary>
		/// Gets support information for the given assembly.
		/// </summary>
		/// <param name="theAssembly">
		///		The assembly to determine information from.
		/// </param>
		/// <returns>
		///		Returns a SupportInfo structure with details about support information.
		/// </returns>
		public static SupportInfo GetSupportInfo( Assembly theAssembly )
		{
			LicenseHelper helper = null;
			SupportInfo info;
			
			try
			{
				helper = LicenseHelper.GetHelperForAssembly( theAssembly );
				if( helper != null && helper.ProvidesSupportInfo )
					return helper.GetSupportInfo( null );

				info = new SupportInfo();

				LicenseHelpAttribute attribute = Attribute.GetCustomAttribute( theAssembly, typeof( LicenseHelpAttribute ), false ) as LicenseHelpAttribute;
				info.Company = null;

				if( attribute != null )
				{
					if( attribute.Company != null && attribute.Company.Length > 0 )
						info.Company	= attribute.Company;
					info.Email		= attribute.Email != null && attribute.Email.Length > 0 ? attribute.Email : "N/A";
					info.Phone		= attribute.Phone != null && attribute.Phone.Length > 0 ? attribute.Phone : "N/A";
					info.Url		= attribute.Url != null && attribute.Url.Length > 0 ? attribute.Url : "N/A";
					info.ShowWindow				= attribute.ShowWindow;
					info.IncludeAssemblies		= attribute.IncludeAssemblies;
					info.IncludeDetails			= attribute.IncludeDetails;
					info.IncludeSystemInfo		= attribute.IncludeSystemInfo;
					info.Product				= attribute.Product;
				}
				else
				{
					info.Email		= "N/A";
					info.Phone		= "N/A";
					info.Url		= "N/A";
					info.ShowWindow			= true;
					info.IncludeAssemblies	= true;
					info.IncludeDetails		= true;
					info.IncludeSystemInfo	= true;
					info.Product			= null;
				}

				if( info.Company == null )
				{
					AssemblyCompanyAttribute companyAttribute = Attribute.GetCustomAttribute( theAssembly, typeof( AssemblyCompanyAttribute ), false ) as AssemblyCompanyAttribute;
					if( companyAttribute != null && companyAttribute.Company != null && companyAttribute.Company.Length > 0 )
						info.Company = companyAttribute.Company;
					else
						info.Company = "the manufacturer";
				}
			}
			catch
			{
				info.Company	= "the manufacturer";
				info.Email		= "N/A";
				info.Phone		= "N/A";
				info.Url		= "N/A";
				info.ShowWindow			= true;
				info.IncludeAssemblies	= true;
				info.IncludeDetails		= true;
				info.IncludeSystemInfo	= true;
				info.Product			= null;
			}

			if( info.Product == null )
			{
				try
				{
					AssemblyProductAttribute apa = Attribute.GetCustomAttribute( theAssembly, typeof( AssemblyProductAttribute ), true ) as AssemblyProductAttribute;
					if( apa != null )
						info.Product = apa.Product;
					if( info.Product == null || info.Product.Length == 0 )
					{
						AssemblyTitleAttribute ata = Attribute.GetCustomAttribute( theAssembly, typeof( AssemblyTitleAttribute ), true ) as AssemblyTitleAttribute;
						if( ata != null )
							info.Product = ata.Title;

						if( info.Product == null || info.Product.Length == 0 )
							info.Product = theAssembly.GetName().Name;
					}
				}
				catch{}
			}

			return info;
		}

		/// <summary>
		/// Gets support information for the given type.
		/// </summary>
		/// <param name="theType">
		///		The Type to get information for.
		/// </param>
		/// <returns>
		///		Returns a SupportInfo structure with details about support information.
		/// </returns>
		public static SupportInfo GetSupportInfo( Type theType )
		{
			SupportInfo info;

			try
			{
				info = GetSupportInfo( theType.Assembly );
			}
			catch
			{
				info = new SupportInfo();
				info.Company	= "the manufacturer";
				info.Email		= "N/A";
				info.Phone		= "N/A";
				info.Url		= "N/A";
				info.ShowWindow			= true;
				info.IncludeAssemblies	= true;
				info.IncludeDetails		= true;
				info.IncludeSystemInfo	= true;
				info.Product			= null;
			}
			return info;
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
	} // End class LicenseHelpAttribute

	#region SupportInfo
	/// <summary>
	/// Support information for the assembly. Use <see cref="LicenseHelpAttribute.GetSupportInfo(System.Type)"/>
	/// to determine values.
	/// </summary>
#if LICENSING
	public
#else
	internal
#endif
		struct SupportInfo
	{
		/// <summary>
		/// The company that released the assembly.
		/// </summary>
		public string Company;

		/// <summary>
		/// The URL where support information can be obtained.
		/// </summary>
		public string Url;

		/// <summary>
		///  The phone number for additional support.
		/// </summary>
		public string Phone;

		/// <summary>
		/// The e-mail address for additional support.
		/// </summary>
		public string Email;

		/// <summary>
		/// Indicates if the failure report window should be displayed when a license cannot
		/// be obtained.
		/// </summary>
		public bool ShowWindow;

		/// <summary>
		/// The name of the product as it should appear in licensing forms. If empty it
		/// is filled by the AssemblyProductAttribute, AssemblyTitleAttribute or AssemblyName.
		/// </summary>
		public string Product;

		/// <summary>
		/// Include a detailed report of the failure.
		/// </summary>
		public bool IncludeDetails;

		/// <summary>
		/// Include listing of assemblies in the failure report.
		/// </summary>
		public bool IncludeAssemblies;

		/// <summary>
		/// Include information about the system in use.
		/// </summary>
		public bool IncludeSystemInfo;

	}
	#endregion
	#endregion

	#region PeriodicLicenseChecksAttribute
	/// <summary>
	/// Allows a developer to declaritively limit the license validation check
	/// frequency of an <see cref="ExtendedLicense"/>.
	/// </summary>
	/// <remarks>
	///		Often the validation process of a component license can be computationally 
	///		expensive. To limit the impact of using licensed components they can be 
	///		set to only check periodically.
	///		<para>
	///		There are two general types of license validations. <b>Local</b> checks
	///		that the limits of the license are valid for the current process and that
	///		the license has not yet expired. <b>Remote</b> checks to see if the license
	///		has been revoked by the issuer. Not all license have a remote limit. If
	///		not present, then the remote value has no meaning.
	///		</para>
	///		<para>
	///		A value of -1 (the default) for either type indicates that that type should
	///		be checked every time a license is requested for the component.
	///		</para>
	/// </remarks>
	[ AttributeUsage( AttributeTargets.Class, AllowMultiple = false, Inherited = true ) ]
#if LICENSING
		public 
#else
	internal
#endif
		sealed class PeriodicLicenseChecksAttribute : Attribute
	{
		////////////////////////////////////////////////////////////////////////////////
		#region Fields

		double _localPeriod		=	-1;
		double _remotePeriod	=	-1;

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Constructors/Destructors

		/// <summary>
		/// Included for COM compliance, do not use.
		/// </summary>
		public PeriodicLicenseChecksAttribute()
		{
		}

		/// <summary>
		///	Initializes a new instance of the PeriodicLicenseChecksAttribute class with
		///	the given period for both local and remote limits.
		/// </summary>
		/// <param name="period">
		///		Time in milliseconds between checks.
		/// </param>
		public PeriodicLicenseChecksAttribute( double period )
		{
			_localPeriod = period;
			_remotePeriod = period;
		}

		/// <summary>
		///	Initializes a new instance of the PeriodicLicenseChecksAttribute class with
		///	the given periods for local and remote limits.
		/// </summary>
		/// <param name="localPeriod">
		///		Time in milliseconds between local checks.
		/// </param>
		/// <param name="remotePeriod">
		///		Time in milliseconds between remote checks.
		/// </param>
		public PeriodicLicenseChecksAttribute( double localPeriod, double remotePeriod )
		{
			_localPeriod = localPeriod;
			_remotePeriod = remotePeriod;
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Properties

		/// <summary>
		///	The periodic rate in milliseconds when the local limits should be 
		///	re-validated.
		/// </summary>
		public double LocalPeriod
		{
			get
			{
				return _localPeriod;
			}
		}

		/// <summary>
		///	The periodic rate in milliseconds when the remote limits should be 
		///	re-validated.
		/// </summary>
		public double RemotePeriod
		{
			get
			{
				return _remotePeriod;
			}
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Operations

		/// <summary>
		/// Determines if either period has elapsed since the license was last validated.
		/// </summary>
		/// <returns>
		///		Returns a combination of ElapsedResponse values indicating if any
		///		periods have ellapsed.
		/// </returns>
		public ElapsedResponse Elapsed( ExtendedLicense license )
		{
			ElapsedResponse response = ElapsedResponse.No;

			if( license.LocalValidated.AddMilliseconds( LocalPeriod ) < DateTime.UtcNow )
				response |= ElapsedResponse.Local;

			if( license.RemoteValidated.AddMilliseconds( RemotePeriod ) < DateTime.UtcNow )
				response |= ElapsedResponse.Remote;

			return response;
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		/// <summary>
		/// Valid responses from the <see cref="Elapsed"/> method.
		/// </summary>
		[Flags]
			public enum ElapsedResponse
		{
			/// <summary>
			/// Neither the local or remote periods have ellapsed.
			/// </summary>
			No = 0,

			/// <summary>
			/// The local period has ellapsed
			/// </summary>
			Local = 1,

			/// <summary>
			/// The remote period has ellapsed.
			/// </summary>
			Remote = 2,

			/// <summary>
			/// Both the remote and local periods have ellapsed.
			/// </summary>
			Both = Local | Remote,

		} // End enum ElapseResponse
	} // End class PeriodicLicenseChecksAttribute
	#endregion

	#region ExtendedLicenseContext
	/// <summary>
	/// Implements a private context used to track custom information during licensing.
	/// </summary>
	internal class ExtendedLicenseContext : LicenseContext
	{
		////////////////////////////////////////////////////////////////////////////////
		#region Fields

		private LicenseContext			_original		= null;
		private Hashtable				_items			= new Hashtable();
		private bool					_isTrialShown	= false;

		#endregion
		////////////////////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////////////////////
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the ExtendedLicenseContext class.
		/// </summary>
		/// <param name="original">
		///		The original context to pass commands through to.
		/// </param>
		public ExtendedLicenseContext( LicenseContext original )
		{
			_original = original;
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////////////////////
		#region Properties

		/// <summary>
		/// Gets the usage mode of the context.
		/// </summary>
		public override LicenseUsageMode UsageMode
		{
			get
			{
				return _original.UsageMode;
			}
		}

		/// <summary>
		/// Gets a reference to a hashtable of items tracked during licensing.
		/// </summary>
		public Hashtable Items
		{
			get
			{
				return _items;
			}
		}

		/// <summary>
		/// Gets or sets a value that indicates if the trial window was shown.
		/// </summary>
		public bool IsTrialShown
		{
			get
			{
				return _isTrialShown;
			}
			set
			{
				_isTrialShown = value;
			}
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////////////////////
		#region Operations

		/// <summary>
		/// Gets the saved license key.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="resourceAssembly"></param>
		/// <returns></returns>
		public override string GetSavedLicenseKey(Type type, Assembly resourceAssembly)
		{
			return _original.GetSavedLicenseKey( type, resourceAssembly );
		}

		/// <summary>
		/// Saves the license key.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="key"></param>
		public override void SetSavedLicenseKey(Type type, string key)
		{
			_original.SetSavedLicenseKey( type, key );
		}

		/// <summary>
		/// Ges the designated service.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public override object GetService(Type type)
		{
			return _original.GetService( type );
		}


		#endregion
		////////////////////////////////////////////////////////////////////////////////
	} // End class ExtendedLicenseContext
	#endregion

} // End namespace Xheo.Licensing

////////////////////////////////////////////////////////////////////////////////
#region Copyright © 2002-2005 XHEO, INC. All Rights Reserved.
//
#endregion
////////////////////////////////////////////////////////////////////////////////