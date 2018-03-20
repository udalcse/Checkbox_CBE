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
// Class:		ExtendedLicense
// Author:		Paul Alexander
// Created:		Friday, September 13, 2002 3:16:00 PM
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Xml;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using System.Reflection;
using System.Reflection.Emit;
using System.IO.IsolatedStorage;
using System.Globalization;
using System.Diagnostics;
using Microsoft.Win32;

#if LICENSING
using Xheo.Licensing.Configuration;
#else
using Internal;
#endif


namespace Xheo.Licensing
{
	/// <summary>
	/// Primary class for the XHEO|Licensing framework.
	/// </summary>
	/// <remarks>
	///		Each <see cref="System.Type"/> in an assembly can be protected by XHEO|Licensing. The environment
	///		that the Type is allowed to run in, is defined by a collection of <see cref="Limit"/>
	///		instances defined for the license. For instance the license may limit a component for use 
	///		on a computer with a specific IP address. These limits are implemented and enforced by 
	///		subclasses of the <see cref="Limit"/> class.
	///		<para>
	///		To ensure that the license files have not been altered since they were
	///		issued, each ExtendedLicense is signed with a <see cref="LicenseSigningKey"/>.
	///		The signing key uses a public key encryption method (See 
	///		<see cref="System.Security.Cryptography"/>) with the public key embedded in
	///		the assembly that the component is implemented in. The private key remains
	///		secured with the original developers ensuring that only they can grant
	///		valid licenses.
	///		</para>
	/// <seealso href="../Backgrounders/Probing.html">License File Probing</seealso>
	/// </remarks>
	[Serializable, DefaultProperty( "AbsoluteSerialNumber" )]
	[System.Security.SuppressUnmanagedCodeSecurity]
#if LICENSING
	public
#else
	internal 
#endif
		sealed class ExtendedLicense : License, ISerializable
	{
		////////////////////////////////////////////////////////////////////////////////
		#region Fields

		private const string USERKEY			= "XL_USER";
		private const string SERIALNUMBERKEY	= "XL_SERIALNUMBER";
		private const string ORGANIZATIONKEY	= "XL_ORGANIZATION";

		internal const string NamespaceUri		= "";
		internal const string NamespacePrefix		= "";
		internal static readonly Version v2_0					= new Version( 2, 0 );
		internal static readonly Version v1_1					= new Version( 1, 1 );
		internal static readonly Version v1_0					= new Version( 1, 0 );
		internal static readonly Version[] _releaseVersions = { v1_0, v1_1, v2_0 };

		/// <summary>
		/// The configuration section where XHEO|Licensing settings are stored in the application's
		/// configuration file.
		/// </summary>
		/// <remarks>
		///		All of XHEO|Licensing's configuration settings can be set from the application
		///		or machine's configuration file. You can use the shared &lt;appSettings&gt;
		///		section or the custom section to set settings.
		///		<para>
		///		Most configuration options can be set in the configuration file via code
		///		by setting the public static property to the desired value. For example to
		///		save the location of the KeyFolder simply set <see cref="ExtendedLicense.KeyFolder"/> to 
		///		the desired value.
		///		</para>
		/// </remarks>
		/// <example>
		/// <code>
		/// &lt;configuration&gt;
		/// &lt;appSettings&gt;
		///		&lt;add key="Xheo.Licensing.TemplateFolder" value="X:\Keys" /&gt;
		/// &lt;/appSettings&gt;
		/// 
		/// or
		/// 
		/// &lt;configSections&gt;
		///		&lt;sectionGroup name="Xheo"&gt;
		///			&lt;section name="Licensing" type="System.Configuration.NameValueSectionHandler, System, Version=1.0.3300.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" /&gt;
		///		&lt;/sectionGroup&gt;
		///	&lt;/configSections&gt;
		///	
		///	&lt;Xheo&gt;
		///		&lt;Licensing&gt;
		///			&lt;add key="TemplateFolder" value="D:\aaa" /&gt;
		///		&lt;/Licensing&gt;
		///	&lt;/Xheo&gt;
		///	&lt;/configuration&gt;
		///	</code>
		/// </example>
		public const string ConfigSection		= "Xheo/Licensing";

		/// <summary>
		/// The current license scheme file version.
		/// </summary>
		[ Browsable( false ) ]
		public readonly static Version CurrentVersion		= v2_0;


		// Version 1.0

		private StringCollection	_components		= new StringCollection();
		private string				_user			= null;
		private string				_organization	= null;
		private string				_signature		= null;
		private string				_serialNumber	= null;
		private string				_issuerUrl		= null;
		private string				_type			= null;
		private Version				_version		= EmulateVersion;
		private DateTime			_expires		= DateTime.Today.AddYears( 100 );
		private string				_tag			= null;
		private LimitCollection		_limits			= null;

		// Version 1.1 additions

		private LicenseValuesCollection		_values			= new LicenseValuesCollection();
		private LicenseValuesCollection		_metaValues		= new LicenseValuesCollection();
		private bool						_isReadOnly		= false;
		private string						_assemblyName	= null;

		// Version 2.0 additions

		private	bool						_unlockBySerial				= false;
		private SerialNumberSizes			_acceptedSerialNumberSizes	= SerialNumberSizes.Recommended;
		private int							_minimumSerialNumberSeed	= 0;
		private int							_maximumSerialNumberSeed	= 0x3FFFFFFF;


		// Instance helpers

		private DateTime						_localValidated		= DateTime.MinValue;
		private DateTime						_remoteValidated	= DateTime.MinValue;
		private PeriodicLicenseChecksAttribute	_periodicChecks		= null;
		private ExtendedLicensePack				_licensePack		= null;
		private string							_invalidReason		= null;
		private string							_surrogateLicensePack	= null;
		[ NonSerialized ]
		private Type							_licensedType		= null;
		private bool							_isEmbedded			= false;
		[ NonSerialized ]
		internal LicenseSigningKey				_publicKey			= null;
		private ExtendedLicense					_unlockedLicense	= null;
		internal bool							_saveOnValid		= false;
		private static Hashtable				_states				= new Hashtable();
		


		// configuration settings
		private static string			_keyFolder			= null;
		private static string			_templateFolder		= null;
		private static string			_connectionString	= null;
		private static ConfigReadWriter _config				= null;
		private static string[]			_licenseFolders		= null;
		private static bool				_iisIsAvailable		= false;
		private static string			_sharedFolder		= null;
#if LICENSING || LICENSETRIALS
		private static int				_disableTrials		= -1;
#endif
		private static IWebProxy		_proxy				= null;
		private static Version			_emulateVersion		= null;
		private static bool				_initialized		= false;
		private static int				_isTerminalServices	= -1;
		
		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Constructors/Destructors

		/// <summary>
		/// Initialize static values
		/// </summary>
		static ExtendedLicense()
		{
			InitLib();			
		}
		
		/// <summary>
		///	Initializes a new instance of the ExtendedLicense class.
		/// </summary>
		public ExtendedLicense()
		{
			if( Version > v1_1 )
				_serialNumber = Guid.NewGuid().ToString().ToUpper( System.Globalization.CultureInfo.InvariantCulture );
			else
				_serialNumber = Guid.NewGuid().ToString();
			_limits = new LimitCollection( this );
			_limits._refCount++;
		}

		/// <summary>
		///	Initializes a new instance of the ExtendedLicense class with the values
		///	of the given XmlNode.
		/// </summary>
		/// <param name="node">
		///		The XmlNode of the <see cref="ExtendedLicensePack"/> to load the license
		///		properties from.
		/// </param>
		public ExtendedLicense( XmlNode node )
		{
			if( Version > v1_1 )
				_serialNumber = Guid.NewGuid().ToString().ToUpper( System.Globalization.CultureInfo.InvariantCulture );
			else
				_serialNumber = Guid.NewGuid().ToString();
			_limits = new LimitCollection( this );
			_limits._refCount++;
			LoadFromXml( node );
		}

		/// <summary>
		/// Releases any managed or unmanaged resources.
		/// </summary>
		public override void Dispose()
		{
			lock( Limits )
			{
				Limits._refCount--;
				if( Limits._refCount <= 0 )
				{
					foreach( Limit limit in Limits )
						limit.Dispose();
					foreach( object obj in Values )
						if( obj != null && obj is IDisposable )
							((IDisposable)obj).Dispose();
				}
			}
		}

		/// <summary>
		/// Creates a new ExtendedLicense from an XML fragment.
		/// </summary>
		/// <param name="xml">
		///		The xml to create the license from.
		/// </param>
		/// <returns>
		///		Returns the newly created extended license.
		/// </returns>
		public static ExtendedLicense FromXml( string xml )
		{
			ExtendedLicense license = new ExtendedLicense();
			license.FromXmlString( xml );
			return license;
		}


		/// <summary>
		/// Creates a new ExtendedLicense from an XML fragment.
		/// </summary>
		/// <param name="xml">
		///		The xml to create the license from.
		/// </param>
		/// <returns>
		///		Returns the newly created extended license.
		/// </returns>
		public static ExtendedLicense FromXml( XmlNode xml )
		{
			ExtendedLicense license = new ExtendedLicense( xml );
			return license;
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Properties

		/// <summary>
		/// Overrides <see cref="License.LicenseKey"/>.
		/// </summary>
#if LICENSING
		[ Browsable( false ) ]
#endif
		public override string LicenseKey
		{
			get
			{
				if( LicensePack == null )
					return ToXmlString();

				return LicensePack.ToXmlString();
            }
		}

		/// <summary>
		/// Gets a reference to per-license specific state that works independent
		/// of any underlying state schemes. This is used to safely track values that
		/// should be kept in memory even when a license is reloaded from disk.
		/// </summary>
		[ Browsable( false ), DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ) ]
		public Hashtable PersistantState
		{
			get
			{
				StateEntry entry = _states[ _signature ] as StateEntry;
				if( entry == null )
				{
					lock( typeof( ExtendedLicense ) )
					{
						entry = _states[ _signature ] as StateEntry;

						if( entry == null )
						{

							entry = new StateEntry();
							_states[ _signature ] = entry;

							if( _states.Count > 20 )
							{
								int timeout = 60;

								try
								{
									if( HttpContext.Current.Session.Timeout > timeout )
										timeout = HttpContext.Current.Session.Timeout;
								}
								catch{}
								ArrayList deleteKeys = new ArrayList();
								foreach( DictionaryEntry de in _states )
								{
									if( ((StateEntry)de.Value).LastUsed.AddMinutes( timeout ) < DateTime.UtcNow )
										deleteKeys.Add( de.Key );
								}

								foreach( object key in deleteKeys )
									_states.Remove( key );
							}
						}
					}
				}
				
				entry.LastUsed = DateTime.UtcNow;
				return entry.SessionState;
			}
		}

		#region Meta Properties
		/// <summary>
		/// Gets or sets the name of the assembly used to sign the license
		/// originally. The value of this property is for reference only and
		/// has no impact on the validation of a license.
		/// </summary>
#if LICENSING
		[ Browsable( false ) ]
#endif
		public string AssemblyName
		{
			get
			{
				return _assemblyName;
			}
			set
			{
				AssertNotReadOnly();
				_assemblyName = value;
			}
		}

		/// <summary>
		/// Gets or sets the signature used to validate the license data.
		/// </summary>
#if LICENSING
		[ Browsable( false ) ]
#endif
		public string Signature
		{
			get
			{
				return _signature;
			}
			set
			{
				AssertNotReadOnly();
				_signature = value;
			}
		}

		/// <summary>
		/// Gets or sets the version of the licensing scheme used.
		/// </summary>
#if LICENSING
		[ 
			Category( "License" ),
			Description( "The version of the licensing scheme used to save the license. Only features available in the version specified will be saved to the license." ),
			TypeConverter( typeof( Design.VersionTypeConverter ) ),
			//DefaultValue( typeof( Version ), "2.0" ),
		]
#endif
		public Version Version
		{
			get
			{
				return _version;
			}
			set
			{
				AssertNotReadOnly();
				if( value < ExtendedLicense.v1_0 || value > CurrentVersion )
					throw new ExtendedLicenseException( "E_InvalidVersion", CurrentVersion );
				_version = value;
			}
		}

		/// <summary>
		/// Gets the <see cref="ExtendedLicensePack"/> that owns the license.
		/// </summary>
#if LICENSING
		[ Browsable( false ) ]
#endif
		public ExtendedLicensePack LicensePack 
		{
			get
			{
				return _licensePack;
			}
		}

		#endregion

		#region License Properties

		/// <summary>
		/// Gets the collection of component names that the license applies to.
		/// </summary>
		/// <remarks>
		/// Each component name in the collection should be the <see cref="System.Type.FullName"/>
		/// of the <see cref="Type"/> including the namespace. When the license is read only then
		/// a copy of the collection is returned and any changes to the collection
		/// are lost.
		/// </remarks>
#if LICENSING
		[
			Browsable( false ),
			Category( "License" ),
			Description( "Collection of component names." ),
		]
#endif
		public StringCollection Components
		{
			get
			{
				if( IsReadOnly )
				{
					StringCollection _readOnlyComponents = new StringCollection();
					foreach( string component in _components )
						_readOnlyComponents.Add( component );
					return _readOnlyComponents;
				}
				return _components;
			}
		}

		/// <summary>
		/// Gets the collection of limits for the current license.
		/// </summary>
#if LICENSING
		[ Browsable( false ) ]
#endif
		public LimitCollection Limits
		{
			get
			{
				return _limits;
			}
		}

		/// <summary>
		/// Gets or sets the name of the user that the license is issued to.
		/// </summary>
#if LICENSING
		[
			Category( "User Info" ),
			Description( "User the license is issued to." ),
			RefreshProperties( RefreshProperties.All )
		]
#endif
		public string AbsoluteUser
		{
			get
			{
				return _user;
			}
			set
			{
				AssertNotReadOnly();
				_user = value;
			}
		}

		/// <summary>
		/// Gets or sets the value of the user as it should appear on screen. 
		/// </summary>
		/// <remarks>
		/// <blockquote class="dtBLOCK"><b>Warning</b> This value is unprotected and can be
		/// changed by the client. Do not assume the value is valid.</blockquote>
		/// </remarks>
#if LICENSING
		[
			Category( "Unprotected" ),
			Description( "Client updated user name. Use AbsoluteUser to set the non-client editable value." )
		]
#endif
		public string User
		{
			get
			{
				if( Version < v2_0 || ! UnlockBySerial || MetaValues[ USERKEY ] == null || MetaValues[ USERKEY ].Length == 0 )
					return AbsoluteUser;
				return MetaValues[ USERKEY ];
			}
			set
			{
				if( Version < v2_0 || ! UnlockBySerial )
				{
					AbsoluteUser = value;
				}
				else
				{
					if( value == AbsoluteUser )
					{
						MetaValues.Remove( USERKEY );
					}														   
					else
					{
						MetaValues[ USERKEY ] = value;
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the name of the organization that the license is issued to.
		/// </summary>
#if LICENSING
		[
			Category( "User Info" ),
			Description( "Organization the license is issued to." ),
			RefreshProperties( RefreshProperties.All )
		]
#endif
		public string AbsoluteOrganization
		{
			get
			{
				return _organization;
			}
			set
			{
				AssertNotReadOnly();
				_organization = value;
			}
		}

		/// <summary>
		/// Gets or sets the name of the organization that the license is issued to.
		/// </summary>
#if LICENSING
		[
			Category( "Unprotected" ),
			Description( "Organization the license is issued to. Use AbsoluteOrganization to set the non-client editable value." )
		]
#endif
		public string Organization
		{
			get
			{
				if( Version < v2_0 || ! UnlockBySerial || MetaValues[ ORGANIZATIONKEY ] == null || MetaValues[ ORGANIZATIONKEY ].Length == 0 )
					return AbsoluteOrganization;
				else
					return MetaValues[ ORGANIZATIONKEY ];
			}
			set
			{
				if( Version < v2_0 || ! UnlockBySerial )
				{
					AbsoluteOrganization = value;
				}
				else
				{
					if( value == AbsoluteOrganization )
						MetaValues.Remove( ORGANIZATIONKEY );
					else
						MetaValues[ ORGANIZATIONKEY ] = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the minimum serial number seed value accepted by this license. Use to partition
		/// serial number seeds to various vendors.
		/// </summary>
		/// <remarks>
		///		Use MinimumSerialNumberSeed and <see cref="MaximumSerialNumberSeed"/> to partition serial
		///		numbers to various resellers or vendors. You can then use this to determine which vendor
		///		sold your product in case of support requests.
		/// </remarks>
#if LICENSING
		[
		Category( "Serial Number" ),
		Description( "The minimum serial number seed value accepted by this license. Use to partition serial number seeds to various vendors." ),
		DefaultValue( 0 ),
		RefreshProperties( RefreshProperties.All )
		]
#endif
		public int MinimumSerialNumberSeed
		{
			get
			{
				return _minimumSerialNumberSeed;
			}
			set
			{
				AssertNotReadOnly();
				if( value > 0x3FFFFFFF )
					throw new ArgumentOutOfRangeException( "MinimumSerialNumberSeed", value, null );
				if( value > MaximumSerialNumberSeed )
				{
					_minimumSerialNumberSeed = MaximumSerialNumberSeed;
					MaximumSerialNumberSeed = value;
				}
				else
					_minimumSerialNumberSeed = value;
			}
		}
		
		/// <summary>
		/// Gets or sets the maximum serial number seed value accepted by this license. Use to partition
		/// serial number seeds to various vendors.
		/// </summary>
		/// <remarks>
		///		Use <see cref="MinimumSerialNumberSeed"/> and MaximumSerialNumberSeed to partition serial
		///		numbers to various resellers or vendors. You can then use this to determine which vendor
		///		sold your product in case of support requests.
		/// </remarks>
#if LICENSING
		[
		Category( "Serial Number" ),
		Description( "The maximum serial number seed value accepted by this license. Use to partition serial number seeds to various vendors." ),
		DefaultValue( 0x3FFFFFFF ),
		RefreshProperties( RefreshProperties.All )
		]
#endif
		public int MaximumSerialNumberSeed
		{
			get
			{
				return _maximumSerialNumberSeed;
			}
			set
			{
				AssertNotReadOnly();
				if( value > 0x3FFFFFFF )
					throw new ArgumentOutOfRangeException( "MaximumSerialNumberSeed", value, null );
				if( value < MinimumSerialNumberSeed )
				{
					_maximumSerialNumberSeed = MinimumSerialNumberSeed;
					MinimumSerialNumberSeed = value;
				}
				else
					_maximumSerialNumberSeed = value;
			}
		}

		/// <summary>
		/// Gets or sets a value that indicates if the user must provide a valid serial number to fully unlock the license.
		/// </summary>
#if LICENSING
		[
			Category( "Serial Number" ),
			Description( "Indicates if the user must provide a valid serial number to fully unlock the license. See \"Unlock By Serial Number\" in the documentation for more details." ),
			DefaultValue( false ),
		]
#endif
		public bool UnlockBySerial
		{
			get
			{
				return _unlockBySerial;
			}
			set
			{
				AssertNotReadOnly();
				_unlockBySerial = value;
			}
		}

		/// <summary>
		/// Gets a value that indicates if the license uses Unlock By Serial and the license has been unlocked. 
		/// Even if the value is true, that does not indicate that the serial number is valid.
		/// </summary>
#if LICENSING
		[ Browsable( false ), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden) ]
#endif
		internal bool IsUnlocked
		{
			get
			{
				if( ! _unlockBySerial )
					return false;

				return SerialNumber != AbsoluteSerialNumber;
			}
		}

		/// <summary>
		/// Gets or sets the serial number sizes that can be used to unlock the license. If a valid serial 
		/// number of a different size is used, license validation will fail.
		/// </summary>
		/// <remarks>
		///		It is recommended that you only allow the largest size you will reasonably expect to issue
		///		to your customers. A malicious user needs only to crack the smallest acceptable size to
		///		gain access to the license. The Recommended sizes are Medium (for regular serial numbers)
		///		and Huge (for activation keys).
		/// </remarks>
#if LICENSING
		[
			Category( "Serial Number" ),
			Description( "The serial number sizes that can be used to unlock the license. If a valid serial number of a different size is used, license validation will fail." ),
			DefaultValue( SerialNumberSizes.Recommended ),
		]
#endif
		public SerialNumberSizes AcceptedSerialNumberSizes
		{
			get
			{
				return _acceptedSerialNumberSizes;
			}
			set
			{
				AssertNotReadOnly();
				this._acceptedSerialNumberSizes = value;
			}
		}

		/// <summary>
		/// Gets or sets the serial number for the license.
		/// </summary>
		/// <remarks>
		///		Generally the value of the serial number property is completely arbitrary and can
		///		contain any value. However the values should be treated as a primary key that
		///		uniquely identifies the license. Generally this is used to reference licenses in a 
		///		back-end system and to uniquely identify the license.
		///		<para>
		///		By default a random GUID is generated for the serial number. You can
		///		later change it to any desired value.
		///		</para>
		///		<para>
		///		(Version 1.2+) When using <i><a href="../Backgrounders/UnlockBySerialNumber.html">Unlock by Serial</a></i> to unlock licenses, the Absolute
		///		Serial Number is used to match user's serial numbers to licenses. 
		///		</para>
		/// </remarks>
#if LICENSING
		[
			Category( "Serial Number" ),
			Description( "Unique serial number for the license. Used to match a serial number to a license." ),
			RefreshProperties( RefreshProperties.All )
		]
#endif
		public string AbsoluteSerialNumber
		{
			get
			{
				return _serialNumber;
			}
			set
			{
				AssertNotReadOnly();
				_serialNumber = value;
			}
		}

		/// <summary>
		/// Gets or sets the value of the absolute serial number. 
		/// </summary>
		/// <remarks>
		///	The absolute serial number is
		/// used in unlocking a license by serial number. The absolute serial number must begin
		/// with the <see cref="SerialNumber"/> for the license.
		/// <blockquote class="dtBLOCK"><b>Warning</b> This value is unprotected and can be
		/// changed by the client. Do not assume the value is valid.</blockquote>
		/// </remarks>
#if LICENSING
		[
			Category( "Serial Number" ),
			Description( "Client updated serial number. Use AbsoluteSerialNumber to set the non-client editable value (the value used to match against when using Unlock by Serial)." ),
		]
#endif
		public string SerialNumber
		{
			get
			{
				if( Version < v2_0 || ! UnlockBySerial || MetaValues[ SERIALNUMBERKEY ] == null || MetaValues[ SERIALNUMBERKEY ].Length == 0 )
					return AbsoluteSerialNumber;
				return MetaValues[ SERIALNUMBERKEY ];
			}
			set
			{
				if( Version < v2_0 || ! UnlockBySerial )
				{
					AbsoluteSerialNumber = value;
				}
				else
				{
					if( value == AbsoluteSerialNumber )
					{
						MetaValues.Remove( SERIALNUMBERKEY  );
					}														   
					else
					{
						if( value == null || value.Length == 0 || value.StartsWith( AbsoluteSerialNumber ) )
							MetaValues[ SERIALNUMBERKEY ] = value;
						else
							throw new ExtendedLicenseException( "E_SerialNumberMismatch", SerialNumber );
					}
				}
			}
		}

		/// <summary>
		/// Gets a collection of arbitrary values to store with the license. 
		/// </summary>
		/// <remarks>
		///		You can use this to store values for runtime adjustments such as
		///		maximum values, names, etc. The values are also signed so adjustments
		///		to the values after the license has been signed will invalidate
		///		the license. Non primitive values must be marked as Serializable.
		/// </remarks>
#if LICENSING
		[ 
		Category( "License" ),
		Description( "Collection of arbitrary values to store with the license." ),
		Editor( typeof( Design.NameValueCollectionEditor ), typeof( System.Drawing.Design.UITypeEditor ) ),
		]
#endif
		public LicenseValuesCollection Values
		{
			get
			{
				return _values;
			}
		}

		/// <summary>
		/// Gets a collection of arbitrary values to store with the license. These values
		/// <i>are not</i> protected and may be changed without affecting the validity
		/// of the license. Use for storing volitle information and other unsecured
		/// information.
		/// </summary>
		/// <remarks>
		///		The MetaValues collection will be persisted with a license. However any changes
		///		to the meta values collection after the license is signed will not affect the
		///		validity of the signature. Use this to track volitle information that need
		///		not be secured and is safe for the user to change.
		/// </remarks>
#if LICENSING
		[ 
			Category( "Unprotected" ),
			Description( "Collection of arbitrary values to store with the license. These values ARE NOT protected and may be changed without affecting the validity of the license. Use for storing volitle information and other unsecured information." ),
			Editor( typeof( Design.NameValueCollectionEditor ), typeof( System.Drawing.Design.UITypeEditor ) ),
		]
#endif
		public LicenseValuesCollection MetaValues
		{
			get
			{
				return _metaValues;
			}
		}

		/// <summary>
		/// Gets or sets the expiration date for the license.
		/// </summary>
#if LICENSING
		[
		Category( "License" ),
		Description( "Gets the date when the license should expire. Do not use to set terms of a trial license, use the Trial limit instead." ),
		]
#endif
		public DateTime Expires
		{
			get
			{
				return _expires;
			}
			set
			{
				AssertNotReadOnly();
				_expires = value;
			}
		}

		/// <summary>
		/// Gets or sets the URL to the company that issued the license.
		/// </summary>
#if LICENSING
		[ 
		Category( "License" ),
		Description( "Gets the URL of the company that issued the license." )
		]
#endif
		public string IssuerUrl
		{
			get
			{
				return _issuerUrl;
			}
			set
			{
				AssertNotReadOnly();
				_issuerUrl = value;
			}
		}

		/// <summary>
		/// Gets or sets an arbitrary piece of data to track with the license. This 
		/// is a "cheap" way of extending the license scheme by allowing you to 
		/// store any kind of additional data that can be saved as a string to 
		/// the license.
		/// </summary>
#if LICENSING
		[
		Category( "License" ),
		Description( "Arbitrary piece of data to save with the license. This is a \"cheap\" way of extending the license scheme by allowing you to store any kind of additional data that can be saved as a string to the license." )
		]
#endif
		public string Tag
		{
			get
			{
				return _tag;
			}
			set
			{
				AssertNotReadOnly();
				_tag = value;
			}
		}

		/// <summary>
		/// Gets or sets an application/product specific description of the type of
		/// license. For examples "designer", "single user", "non-profit", etc. This can often be
		/// used a reference to the primary key in a database, such as a SKU, for the product 
		/// that the license represents.
		/// </summary>
#if LICENSING
		[
		Category( "License" ),
		Description( "Application or product specific description of the type of license. Example: Designer, SINGLE-USER, D44H" )
		]
#endif
		public string Type
		{
			get
			{
				return _type;
			}
			set
			{
				AssertNotReadOnly();
				_type = value;
			}
		}

		#endregion

		#region Runtime Helpers

		[ Browsable( false ) ]
		internal bool SignedByProLicense
		{
			get
			{
				if( _publicKey == null )
					return false;
				if( Values[ LicenseSigningKey.XLPROCLIENTKEY ] == null )
					return false;
                return Values[ LicenseSigningKey.XLPROCLIENTKEY ] == "TRUE";
			}
		}

		/// <summary>
		/// Gets a reference to the <see cref="LicenseSigningKey"/> being used to validate the license.
		/// </summary>
#if LICENSING
		[ Browsable( false ) ]
#endif
		public LicenseSigningKey PublicKey
		{
			get
			{
				if( _publicKey == null && LicensePack != null )
					return LicensePack._publicKey;

				return _publicKey;
			}
		}

		/// <summary>
		/// Gets or sets the contents of the license as an XML string fragment.
		/// </summary>
#if LICENSING
		[ Browsable( false ) ]
#endif
		public string XmlString
		{
			get
			{
				return ToXmlString();
			}
			set
			{
				FromXmlString( value );
			}
		}

		/// <summary>
		/// Gets or sets the reason the license was not valid. If Validate returns true this is an empty string.
		/// </summary>
#if LICENSING
		[ Browsable( false ) ]
#endif
		public string InvalidReason
		{
			get
			{
				return _invalidReason;
			}
			set
			{
				if( value == null || value.Length == 0 )
					_invalidReason = null;
				else
				{
					if( _invalidReason != null && _invalidReason.Length > 0 )
						_invalidReason += " --> ";
					_invalidReason += value;
				}
			}
		}

		/// <summary>
		/// Gets the Type the license was/will be granted to.
		/// </summary>
#if LICENSING
		[ Browsable( false ) ]
#endif
		public Type LicensedType
		{
			get
			{
				return _licensedType;
			}
		}

		/// <summary>
		/// Gets or sets a value that indicates if the license was loaded from an embedded
		/// resource rather then an external .LIC file.
		/// </summary>
#if LICENSING
		[ 
			Browsable( false ),
			DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ) 
		]
#endif
		public bool IsEmbedded
		{
			get
			{
				return _isEmbedded;
			}
			set
			{
				AssertNotReadOnly();
				_isEmbedded = value;
			}
		}

		/// <summary>
		/// Gets or sets a surrogate license pack to use if validation of this license fails.
		/// </summary>
		/// <remarks>
		///		For limits that alter the current license, or download new licenses you can
		///		set this property so that the new or altered license pack is used instead. When 
		///		<see cref="Validate"/> is called on the current license, this is initially
		///		set to null. If Validate returns false, and this is not null then the
		///		license pack indicated in this property, if it contains a license, is used instead. 
		///		If a surrogate license pack also defines a surrogate license, it will not be checked, 
		///		the license provider only checks one level deep.
		///		<para>
		///		Generally you shouldn't set this to a license pack containing license with 
		///		with a Trial limit or GUI limit because it will interfere with the optimal license 
		///		selection of the license provider.
		///		</para>
		/// </remarks>
#if LICENSING
		[ Browsable( false ) ]
#endif
		public string SurrogateLicensePack
		{
			get
			{
				return _surrogateLicensePack;
			}
			set
			{
				_surrogateLicensePack = value;
			}
		}

		/// <summary>
		/// Gets or sets the ExtendedLicense that was unlocked by the registration process.
		/// </summary>
#if LICENSING
		[ Browsable( false ) ]
#endif
		public ExtendedLicense UnlockedLicense
		{
			get
			{
				return _unlockedLicense;
			}
			set
			{
				if( value == null || LicensePack.Contains( value ) )
					_unlockedLicense = value;
				else
					throw new ExtendedLicenseException( "E_UnlockedSamePack" );
			}
		}


#if LICENSING || LICENSETRIALS
		/// <summary>
		/// Gets a value that indicates if the license has a trial limit.
		/// </summary>
		[ Browsable( false ) ]
		public bool IsTrial
		{
			get
			{
				return GetLimit( typeof( TrialLimit ) ) != null;
			}
		}
#endif

#if LICENSING || LICENSEACTIVATION
		/// <summary>
		/// Gets a value that indicates if the license is an activateable license.
		/// </summary>
#if LICENSING
		[ Browsable( false ) ]
#endif
		public bool IsActivation
		{
			get
			{
				return GetLimit( typeof( ActivationLimit ) ) != null;
			}
		}

		/// <summary>
		/// Gets a value that indicates if the license has been activated.
		/// </summary>
#if LICENSING
		[ Browsable( false ) ]
#endif
		public bool IsActivated
		{
			get
			{
				ActivationLimit limit = GetLimit( typeof( ActivationLimit ) ) as ActivationLimit;
				if( limit == null )
					return false;
                return limit.IsActivated;
			}
		}
#endif

#if LICENSING || LICENSESERVERS
		/// <summary>
		/// Gets a value that indicates if the license uses a License Server for validation.
		/// </summary>
#if LICENSING
		[ Browsable( false ) ]
#endif
		public bool UsesLicenseServer
		{
			get
			{
				return GetLimit( typeof( LicenseServerLimit ) ) != null;
			}
		}
#endif

		/// <summary>
		/// Gets a value that indicates if the current license is read only.
		/// </summary>
#if LICENSING
		[ Browsable( false ) ]
#endif
		public bool IsReadOnly
		{
			get
			{
				return _isReadOnly;
			}
		}

		/// <summary>
		/// Gets a value that indicates if the license contians time sensitive data
		/// and the clock should be protected.
		/// </summary>
#if LICENSING
		[Browsable(false)]
#endif
		public bool IsTimeSensitive
		{
			get
			{
				if( Expires > DateTime.Now.AddYears( 10 ) )
				{
					foreach( Limit limit in Limits )
						if( limit.IsTimeSensitive )
							return true;
					return false;
				}

				return true;
			}
		}

		/// <summary>
		/// Gets or sets the time that this license was validated locally.
		/// </summary>
		[ Browsable( false ) ]
		internal DateTime LocalValidated
		{
			get
			{
				return _localValidated;
			}
			set
			{
				_localValidated = value;
			}
		}

		/// <summary>
		/// Gets or sets the time that this license was validated remotely.
		/// </summary>
		[ Browsable( false ) ]
		internal DateTime RemoteValidated
		{
			get
			{
				return _remoteValidated;
			}
			set
			{
				_remoteValidated = value;
			}
		}

		/// <summary>
		/// Gets or sets the times for periodic re-validations of the license.
		/// </summary>
		[ Browsable( false ) ]
		internal PeriodicLicenseChecksAttribute PeriodicChecks
		{
			get
			{
				return _periodicChecks;
			}
			set
			{
				_periodicChecks = value;
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
				return _iisIsAvailable;
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

				return System.Web.HttpContext.Current != null;
			}
		}

		#endregion

		#region Configuration Properties

		/// <summary>
		/// Gets the folder where the license keys are located as defined in the application's
		/// config file.
		/// </summary>
		/// <remarks>
		///		You can configure the value of this property in your application's configuration
		///		file. In the appSettings section add an entry for Xheo.Licensing.KeyFolder and
		///		set it to the path where keys are stored on your machine.
		/// </remarks>
		/// <example>
		/// <code>
		/// &lt;configuration&gt;
		/// &lt;appSettings&gt;
		///		&lt;add key="Xheo.Licensing.KeyFolder" value="X:\Keys" /&gt;
		/// &lt;/appSettings&gt;
		/// 
		/// or
		/// 
		/// &lt;configSections&gt;
		///		&lt;sectionGroup name="Xheo"&gt;
		///			&lt;section name="Licensing" type="System.Configuration.NameValueSectionHandler, System, Version=1.0.3300.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" /&gt;
		///		&lt;/sectionGroup&gt;
		///	&lt;/configSections&gt;
		///	
		///	&lt;Xheo&gt;
		///		&lt;Licensing&gt;
		///			&lt;add key="KeyFolder" value="D:\aaa" /&gt;
		///		&lt;/Licensing&gt;
		///	&lt;/Xheo&gt;
		///	&lt;/configuration&gt;
		///	</code>
		/// </example>
		public static string KeyFolder
		{
			get
			{
				lock( typeof( ExtendedLicense ) )
				{
					if( _keyFolder == null || _keyFolder.Length == 0 )
						_keyFolder = GetFullPath( ConfigSettings.GetSetting( "KeyFolder", (string)"Keys", ExtendedLicense.ConfigSection ) );
				}
                
				return _keyFolder;
			}
			set
			{
				lock( typeof( ExtendedLicense ) )
				{
					_keyFolder = ( value == null || value.Length == 0 ) ? value : Path.GetFullPath( value );
					if( value == null || value.Length == 0 )
						Config.RemoveValue( "KeyFolder" );
					else
						Config.SetValue( "KeyFolder", value );
				}
			}
		}

		/// <summary>
		/// Gets a collection of folders where the licenses are located as defined in the application's
		/// config file.
		/// </summary>
		/// <remarks>
		///		<para>
		///		Use this setting when you install XHEO|Licensing or licensed components into the GAC, or
		///		when using a shared install in a hosting environment.
		///		</para>
		///		You can configure the value of this property in your application's configuration
		///		file. In the appSettings section add an entry for Xheo.Licensing.LicenseFolders and
		///		set it to the path where keys are stored on your machine.
		/// </remarks>
		/// <example>
		/// <code>
		/// &lt;configuration&gt;
		/// &lt;appSettings&gt;
		///		&lt;add key="Xheo.Licensing.LicenseFolder" value="X:\Folder1" /&gt;
		///		&lt;add key="Xheo.Licensing.LicenseFolder" value="X:\Folder2" /&gt;
		/// &lt;/appSettings&gt;
		/// 
		/// or
		/// 
		/// &lt;configSections&gt;
		///		&lt;sectionGroup name="Xheo"&gt;
		///			&lt;section name="Licensing" type="Xheo.Licensing.Configuration.NameValueSectionHandler, System, Version=1.1.0.0, Culture=neutral, PublicKeyToken=798276055709c98a" /&gt;
		///		&lt;/sectionGroup&gt;
		///	&lt;/configSections&gt;
		///	
		///	&lt;Xheo&gt;
		///		&lt;Licensing&gt;
		///			&lt;add key="LicenseFolder" value="D:\aaa" /&gt;
		///			&lt;add key="LicenseFolder" value="C:\other" /&gt;
		///		&lt;/Licensing&gt;
		///	&lt;/Xheo&gt;
		///	&lt;/configuration&gt;
		///	</code>
		/// </example>
		public static string[] LicenseFolders
		{
			get
			{
				lock( typeof( ExtendedLicense ) )
				{
					if( _licenseFolders == null || _licenseFolders.Length == 0 )
					{
						_licenseFolders = ConfigSettings.GetSettings( "LicenseFolder", ConfigSection );

						if( _licenseFolders == null )
							_licenseFolders = new string[ 0 ];
						else
							for( int index = 0; index < _licenseFolders.Length; index++ )
								_licenseFolders[ index ] = GetFullPath( _licenseFolders[ index ] );
					}
				}

				return _licenseFolders;
			}
			set
			{
				lock( typeof( ExtendedLicense ) )
				{
					_licenseFolders = value;
					if( value == null || value.Length == 0 )
						Config.RemoveValue( "LicenseFolder" );
					else
						Config.SetValues( "LicenseFolder", value );
				}
			}
		}

		/// <summary>
		/// Gets the folder where the license templates are located as defined in the application's
		/// config file.
		/// </summary>
		/// <remarks>
		///		You can configure the value of this property in your application's configuration
		///		file. In the appSettings section add an entry for Xheo.Licensing.TemplateFolder and
		///		set it to the path where keys are stored on your machine.
		/// </remarks>
		/// <example>
		/// <code>
		/// &lt;configuration&gt;
		/// &lt;appSettings&gt;
		///		&lt;add key="Xheo.Licensing.TemplateFolder" value="X:\Templates" /&gt;
		/// &lt;/appSettings&gt;
		/// 
		/// or
		/// 
		/// &lt;configSections&gt;
		///		&lt;sectionGroup name="Xheo"&gt;
		///			&lt;section name="Licensing" type="System.Configuration.NameValueSectionHandler, System, Version=1.0.3300.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" /&gt;
		///		&lt;/sectionGroup&gt;
		///	&lt;/configSections&gt;
		///	
		///	&lt;Xheo&gt;
		///		&lt;Licensing&gt;
		///			&lt;add key="TemplateFolder" value="D:\aaa\Templates" /&gt;
		///		&lt;/Licensing&gt;
		///	&lt;/Xheo&gt;
		///	&lt;/configuration&gt;
		///	</code>
		/// </example>
		public static string TemplateFolder
		{
			get
			{
				lock( typeof( ExtendedLicense ) )
				{
					if( _templateFolder == null || _templateFolder.Length == 0 )
						_templateFolder = GetFullPath( ConfigSettings.GetSetting( "TemplateFolder", "Templates", ExtendedLicense.ConfigSection ) );
				}
				return _templateFolder;
			}
			set
			{
				lock( typeof( ExtendedLicense ) )
				{
					_templateFolder = ( value == null || value.Length == 0 ) ? value : Path.GetFullPath( value );
					if( value == null || value.Length == 0 )
						Config.RemoveValue( "TemplateFolder" );
					else
						Config.SetValue( "TemplateFolder", value );
				}
			}
		}

		/// <summary>
		/// Gets the default database connection string for licensing components.
		/// </summary>
		/// <remarks>
		///		You can configure the value of this property in your application's configuration
		///		file. In the appSettings section add an entry for Xheo.Licensing.ConnectionString and
		///		set it to the path where keys are stored on your machine.
		/// </remarks>
		/// <example>
		/// <code>
		/// &lt;configuration&gt;
		/// &lt;appSettings&gt;
		///		&lt;add key="Xheo.Licensing.ConnectionString" value="server=localhost;catalog=Northwind" /&gt;
		/// &lt;/appSettings&gt;
		/// 
		/// or
		/// 
		/// &lt;configSections&gt;
		///		&lt;sectionGroup name="Xheo"&gt;
		///			&lt;section name="Licensing" type="System.Configuration.NameValueSectionHandler, System, Version=1.0.3300.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" /&gt;
		///		&lt;/sectionGroup&gt;
		///	&lt;/configSections&gt;
		///	
		///	&lt;Xheo&gt;
		///		&lt;Licensing&gt;
		///			&lt;add key="ConnectionString" value="server=localhost;catalog=Northwind" /&gt;
		///		&lt;/Licensing&gt;
		///	&lt;/Xheo&gt;
		///	&lt;/configuration&gt;
		///	</code>
		/// </example>
		public static string ConnectionString
		{
			get
			{
				lock( typeof( ExtendedLicense ) )
				{
					if( _connectionString == null || _connectionString.Length == 0 )
						_connectionString = ConfigSettings.GetSetting( "ConnectionString", (string)null, ExtendedLicense.ConfigSection );
				}

				return _connectionString;
			}
			set
			{
				lock( typeof( ExtendedLicense ) )
				{
					_connectionString = value;
					if( value == null || value.Length == 0 )
						Config.RemoveValue( "ConnectionString" );
					else
						Config.SetValue( "ConnectionString", value );
				}
			}
		}

		/// <summary>
		/// Gets or sets the path to the shared license folder. 
		/// </summary>
		/// <remarks>
		///		The shared license folder is a location on the client machine where all 
		///		XHEO|Licensing protected applications can look for licenses. This helps
		///		with shared hosting environments and large applications that may include
		///		multiple assemblies, some in the GAC. Rather then having .LIC files spread
		///		over the machine they can be located into a single location.
		///		<para>
		///		This is a machine wide setting. The value of this property should be
		///		considered configured by the machine's administrator. You should not change
		///		it as it will effect all installed applications protected with XHEO|Licensing.
		///		</para>
		///		<para>
		///		The folder location by default is <code class="code">[Common Files]\Xheo\SharedLicenses</code>.
		///		This can be configured per machine or per user in the registry at 
		///		<code class="code">HKLM</code> or <code class="code">HKLU\Software\Xheo\Licensing\SharedLicenseFolder</code>.
		///		</para>
		///		See <see cref="LicenseFolders"/> for settings you can apply to the 
		///		application's config or machine.config file.
		///		<seealso cref="LicenseFolders"/>
		/// </remarks>
		public static string SharedFolder
		{
			get
			{
				if( _sharedFolder == null )
				{
					lock( typeof( ExtendedLicense ) )
					{
						if( _sharedFolder == null )
						{
							RegistryKey key = null;
							try
							{
								_sharedFolder = Environment.GetFolderPath( System.Environment.SpecialFolder.CommonProgramFiles ) + @"\Xheo\SharedLicenses";

								key = Registry.CurrentUser.OpenSubKey( @"Software\Xheo\Licensing" );
				
								if( key == null )
									key = Registry.LocalMachine.OpenSubKey( @"Software\Xheo\Licensing" );

								string sharedPath = null;

								if( key != null )
									sharedPath = key.GetValue( "SharedLicenseFolder" ) as string;

								if( sharedPath != null && sharedPath.Length > 0 )
									_sharedFolder = sharedPath;
							}
							catch{}
							finally
							{
								if( key != null )
									key.Close();
							}

						}
					}
				}
				return _sharedFolder;
			}
		}

		/// <summary>
		/// Gets the Proxy to use when connecting to a web service.
		/// </summary>
		/// <remarks>
		///		In most circumstances the default proxy settings for the machine will work
		///		for the user. However certain proxy configurations can cause communication 
		///		problems with XHEO|Licensing. XHEO|Licensing uses the default proxy settings
		///		for the machine, when alternate settings are needed you can provide them
		///		in the registry at <span class="code">HKLM</span> or 
		///		<span class="code">HKLU\Software\Xheo\Licensing\ProxyAddress</span>.
		///		<para>
		///		The value of the Proxy value in the registry should be the address of the
		///		proxy setting to use. If the value does not exist, XHEO|Licensing will use
		///		the machine's default settings. If it does exist they will override the
		///		machines settings. If the value exists, but is an empty string then no proxy
		///		will be used even if the machine defines one.
		///		</para>
		///		<para>
		///		To supply proxy credentials add ProxyUsername and ProxyPassword values to the
		///		same key.
		///		</para>
		///		<para>
		///		<blockquote class="dtBLOCK"><b>Note</b>This is a machine wide setting. Care should be taken not to overwrite any
		///		existing settings when not necessary.</blockquote></para> 
		///		<para>
		///		You can also configure the value of this property in your application's configuration
		///		file. In the appSettings section add an entry for Xheo.Licensing.ProxyAddress and
		///		set it to the address of the proxy server. You can specify a username and 
		///		password by using Xheo.Licensing.ProxyUsername and Xheo.Licensing.ProxyPassword.
		///		</para>
		///		<para>
		///		If you specify settings in the registry and the config file, the settings in
		///		the config file will be used.
		///		</para>
		/// </remarks>
		/// <example>
		/// <code>
		/// &lt;configuration&gt;
		/// &lt;appSettings&gt;
		///		&lt;add key="Xheo.Licensing.ProxyAddress" value="http://proxy:8080" /&gt;
		/// &lt;/appSettings&gt;
		/// 
		/// or
		/// 
		/// &lt;configSections&gt;
		///		&lt;sectionGroup name="Xheo"&gt;
		///			&lt;section name="Licensing" type="System.Configuration.NameValueSectionHandler, System, Version=1.0.3300.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" /&gt;
		///		&lt;/sectionGroup&gt;
		///	&lt;/configSections&gt;
		///	
		///	&lt;Xheo&gt;
		///		&lt;Licensing&gt;
		///			&lt;add key="ProxyAddress" value="http://proxy:8080" /&gt;
		///			&lt;add key="ProxyUsername" value="user" /&gt;
		///			&lt;add key="ProxyPassword" value="password" /&gt;
		///		&lt;/Licensing&gt;
		///	&lt;/Xheo&gt;
		///	&lt;/configuration&gt;
		///	</code>
		/// </example>
		public static IWebProxy Proxy
		{
			get
			{
				if( _proxy == null )
				{
					lock( typeof( ExtendedLicense ) )
					{
						if( _proxy == null )
						{
                            _proxy = WebRequest.DefaultWebProxy;

							RegistryKey key = null;
							try
							{
								string proxyAddress = null;
								string proxyUsername = null;
								string proxyPassword = null;

								proxyAddress = ConfigSettings.GetSetting( "ProxyAddress", (string)null, ConfigSection );
								if( proxyAddress != null )
								{
									proxyUsername = ConfigSettings.GetSetting( "ProxyUsername", (string)null, ConfigSection );
									proxyPassword = ConfigSettings.GetSetting( "ProxyPassword", (string)null, ConfigSection );									
								}
								else
								{
									key = Registry.CurrentUser.OpenSubKey( @"Software\Xheo\Licensing" );
				
									if( key == null )
										key = Registry.LocalMachine.OpenSubKey( @"Software\Xheo\Licensing" );

									if( key == null )
										return _proxy;

									if( key != null )
										proxyAddress = key.GetValue( "ProxyAddress" ) as string;

									proxyUsername = key.GetValue( "ProxyUsername" ) as string;
									proxyPassword = key.GetValue( "ProxyPassword" ) as string;
								}

								if( proxyAddress != null && proxyAddress.Length == 0 )
								{
									_proxy = GlobalProxySelection.GetEmptyWebProxy();
								}
								else
								{
									if( proxyUsername != null && proxyUsername.Length > 0 )
									{
										string username = null;
										string domain = null;

										int slash = proxyUsername.IndexOf( '\\' );
										if( slash == -1 )
											slash = proxyUsername.IndexOf( '/' );
										if( slash != -1 )
										{
											domain = proxyUsername.Substring( 0, slash );
											username = proxyUsername.Substring( slash + 1 );
										}
										else
										{
											username = proxyUsername;
										}						

										_proxy = new WebProxy( proxyAddress, false, new string[ 0 ], new NetworkCredential( username, proxyPassword, domain ) );
										
									}
									else
										_proxy = new WebProxy( proxyAddress, false );
								}
							}
							catch{}
							finally
							{
								if( key != null )
									key.Close();
							}
						}
					}
				}
				return _proxy;
			}
		}
		#region
		internal static void SetProxy( IWebProxy proxy )
		{
			lock( typeof( ExtendedLicense ) )
				_proxy = proxy;
		}
		#endregion

		
		/// <summary>
		/// Gets a value that indicates if the current session is running under terminal services.
		/// </summary>
		/// <value></value>
		public static bool IsTerminalServices
		{
			get
			{
				if( _isTerminalServices == -1 )
				{
					lock( typeof( ExtendedLicense ) )
					{
						if( _isTerminalServices == -1 )
						{
							if(
								( Environment.OSVersion.Platform & ( PlatformID.Win32NT | PlatformID.Win32Windows ) ) != 0 )
							{
								_isTerminalServices = GetSystemMetrics( 0x1000 ) != 0 ? 1 : 0;
							}
							else
								_isTerminalServices = 0;
						}
					}
				}

				return _isTerminalServices == 1;
			}
		}
		#region Helpers
		[DllImport( "user32.dll", CharSet = CharSet.Auto, ExactSpelling = true )]
		private static extern int GetSystemMetrics( int nIndex );
		#endregion

#if LICENSING || LICENSETRIALS
		/// <summary>
		/// Gets a value that indicates if trial licenses are disabled. 
		/// </summary>
		/// <remarks>
		///		You can configure the value of this property in your application's configuration
		///		file. In the appSettings section add an entry for Xheo.Licensing.DisableTrials and
		///		set it to either true or false.
		/// </remarks>
		/// <example>
		/// <code>
		/// &lt;configuration&gt;
		/// &lt;appSettings&gt;
		///		&lt;add key="Xheo.Licensing.DisableTrials" value="true" /&gt;
		/// &lt;/appSettings&gt;
		/// 
		/// or
		/// 
		/// &lt;configSections&gt;
		///		&lt;sectionGroup name="Xheo"&gt;
		///			&lt;section name="Licensing" type="System.Configuration.NameValueSectionHandler, System, Version=1.0.3300.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" /&gt;
		///		&lt;/sectionGroup&gt;
		///	&lt;/configSections&gt;
		///	
		///	&lt;Xheo&gt;
		///		&lt;Licensing&gt;
		///			&lt;add key="DisableTrials" value="true" /&gt;
		///		&lt;/Licensing&gt;
		///	&lt;/Xheo&gt;
		///	&lt;/configuration&gt;
		///	</code>
		/// </example>
		public static bool DisableTrials
		{
			get
			{
				if( _disableTrials == -1 )
					_disableTrials = ConfigSettings.GetSetting( "DisableTrials", false, ConfigSection ) ? 1 : 0;

				return _disableTrials == 1;
			}
		}
#endif

		/// <summary>
		/// Gets a value that determines the release version that XHEO|Licensing should
		/// emulate.
		/// </summary>
		/// <remarks>
		///		<para>
		///		Use this value when migrating a solution from an earlier version to a 
		///		newer one so that licenses generated with the new assembly will use the 
		///		previous version until you have fully upgraded all dependent assemblies.
		///		</para>
		///		<para>
		///		You can configure the value of this property in your application's configuration
		///		file. In the appSettings section add an entry for Xheo.Licensing.EmulateVersion and
		///		set it to either true or false.
		///		</para>
		/// </remarks>
		/// <example>
		/// <code>
		/// &lt;configuration&gt;
		/// &lt;appSettings&gt;
		///		&lt;add key="Xheo.Licensing.EmulateVersion" value="1.1" /&gt;
		/// &lt;/appSettings&gt;
		/// 
		/// or
		/// 
		/// &lt;configSections&gt;
		///		&lt;sectionGroup name="Xheo"&gt;
		///			&lt;section name="Licensing" type="System.Configuration.NameValueSectionHandler, System, Version=1.0.3300.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" /&gt;
		///		&lt;/sectionGroup&gt;
		///	&lt;/configSections&gt;
		///	
		///	&lt;Xheo&gt;
		///		&lt;Licensing&gt;
		///			&lt;add key="EmulateVersion" value="1.1" /&gt;
		///		&lt;/Licensing&gt;
		///	&lt;/Xheo&gt;
		///	&lt;/configuration&gt;
		///	</code>
		/// </example>
		public static Version EmulateVersion
		{
			get
			{
				if( _emulateVersion == null )
					_emulateVersion = new Version( ConfigSettings.GetSetting( "EmulateVersion", CurrentVersion.ToString(), ConfigSection ) );
				if( _emulateVersion > CurrentVersion )
					_emulateVersion = CurrentVersion;

				foreach( Version version in _releaseVersions )
					if( _emulateVersion == version )
                        return _emulateVersion;

				throw new ExtendedLicenseException( "E_InvalidEmulateVersion" );
			}
		}

		/// <summary>
		/// Gets the ConfigReadWriter for XHEO|Licensing configuration settings.
		/// <seealso cref="ConfigSection"/>
		/// </summary>
#if LICENSING
		public
#else
		internal
#endif
		static ConfigReadWriter Config
		{
			get
			{
				if( _config == null )
					_config = new ConfigReadWriter( ExtendedLicense.ConfigSection );
				return _config;
			}
		}

		#endregion

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Operations

		/// <summary>
		/// Initialize the library on first use.
		/// </summary>
		internal static void InitLib()
		{
			if( ! _initialized )
			{
				lock( typeof( ExtendedLicense ) )
				{
					if( ! _initialized )
					{
						_initialized = true;
						_iisIsAvailable = false;
						try
						{
							try
							{
								if( HttpContext.Current != null )
								{
									_iisIsAvailable = true;
									System.Diagnostics.Debug.Write( "SUCCESS: IIS is available (Shortcut).\r\n" );
									return;
								}
							}
							catch{}

							string dll = System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory() + "aspnet_isapi.dll";

							if( File.Exists( dll ) )
							{
								IntPtr hModule	= LoadLibraryExW( dll, IntPtr.Zero, (uint)2 );

								if( hModule == IntPtr.Zero )
									hModule = LoadLibraryExW( dll, IntPtr.Zero, (uint)2 );

								if( hModule != IntPtr.Zero )
								{
									if( FreeLibrary( hModule ) != 0 )
									{
										_iisIsAvailable = true;
										System.Diagnostics.Debug.Write( "SUCCESS: IIS is available.\r\n" );
									}
								}
							}
						}
						catch{}

						if( ! _iisIsAvailable )
							System.Diagnostics.Debug.Write( "ERROR: IIS Not Found!\r\n" );

					}
				}
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


#if LICENSING || LICENSEREGISTRATIONS
		/// <summary>
		/// If the license contains a registration limit, then calling this method will
		/// display the registration form. If the registration is successful will
		/// unlock the application.
		/// <seealso cref="RegistrationForm.InitializeFields"/>
		/// </summary>
		/// <param name="initValues">
		///		Initial values for the various fields on the registration form.
		/// </param>
		/// <returns>
		///		Returns true if the license has a registration limit and the registration
		///		was successful.
		/// </returns>
		public bool ShowRegistrationForm( NameValueCollection initValues )
		{
			RegistrationLimit limit = GetLimit( typeof( RegistrationLimit ) ) as RegistrationLimit;

			if( limit == null )
				return false;

			using( RegistrationForm form = limit.MakeForm() )
			{
				form.InitializeFields( initValues );
				if( form.ShowDialog() == System.Windows.Forms.DialogResult.OK )
				{

					if( form.GetLicense || form.GotLicense || form.License.UnlockBySerial )
					{
						form.License.SaveOnValid();
						//LicensePack.Save( true );
						ExtendedLicenseProvider.ResetCacheForLicense( this );
						SurrogateLicensePack = form.SaveLocation;
						return true;
					}
				}

				return false;
			}
		}
		#region Overloads

		/// <summary>
		/// If the license contains a registration limit, then calling this method will
		/// display the registration form. If the registration is successful will
		/// unlock the application.
		/// </summary>
		/// <returns>
		///		Returns true if the license has a registration limit and the registration
		///		was successful.
		/// </returns>
		public bool ShowRegistrationForm()
		{
			return ShowRegistrationForm( null );
		}
		#endregion
#endif

#if LICENSING || LICENSESERVERS
		/// <summary>
		/// Register the license with any <see cref="LicenseServer"/>s identified by
		/// <see cref="LicenseServerLimit"/> instances in the <see cref="Limits"/> collection.
		/// </summary>
		/// <param name="includeAlternates">
		///		Indicates if the license should be registered with the alternate servers
		///		as well. Normally this should be true.
		/// </param>
		/// <param name="keys">
		///		Name of the keys to use when validating and registering the license. If null
		///		then the AssemblyName will be used.
		/// </param>
		/// <returns>
		///		Returns true if the license was registered successfully with all the
		///		servers, otherwise false. Check InvalidReason for additional information
		///		on any failures.
		/// </returns>
		/// <remarks>
		///		If the license contains a <see cref="LicenseServerLimit"/> the license
		///		will be registered with the primary server. If includeAlternates is true
		///		then the license will also be registered with the alternate servers.
		/// </remarks>
		public bool RegisterWithServer( bool includeAlternates, string keys )
		{
			InvalidReason = null;
			StringBuilder invalidReason = new StringBuilder();

			if( keys == null || keys.Length == 0 )
				keys = GetShortAssemblyName();

			if( keys == null || keys.Length == 0 )
			{
				InvalidReason = "No keys defined.";
				return false;
			}

			bool success = RegisterWithServerRecursive( includeAlternates, Limits, keys, invalidReason, false );	
			if( ! success )
				InvalidReason = invalidReason.ToString();
			return success;
		}
		#region Helpers
		///<summary>
		///Summary of RegisterWithServerRecursive.
		///</summary>
		///<param name="includeAlternates"></param>
		///<param name="limits"></param>
		///<param name="keys"></param>
		///<param name="invalidReason"></param>
		///<param name="unregister"></param>
		///<returns></returns>	
		private bool RegisterWithServerRecursive( bool includeAlternates, LimitCollection limits, string keys, StringBuilder invalidReason, bool unregister )
		{
			bool success = true;
			if( Signature == null || Signature.Length == 0 )
			{
				invalidReason.Append( Internal.StaticResourceProvider.CurrentProvider.GetString( "E_LicenseNotSigned" ) );
				return false;
			}

			foreach( Limit limit in limits )
			{
				if( limit is LicenseServerLimit )
				{
					LicenseServerLimit serverLimit = limit as LicenseServerLimit;
					success &= RegisterWithServer( serverLimit.Url, keys, invalidReason, unregister );

					if( includeAlternates )
						foreach( string serverUrl in serverLimit.Alternates )
							success &= RegisterWithServer( serverUrl, keys, invalidReason, unregister );

				}
				else if( limit is ContainerLimit )
				{
					success &= RegisterWithServerRecursive( includeAlternates, ((ContainerLimit)limit).Limits, keys, invalidReason, unregister ) && success;
				}
			}

			return success;
		}

		private bool RegisterWithServer( string serverUrl, string keys, StringBuilder invalidReason, bool unregister )
		{
			ExternalLicenseServer.XheoLicensingServer server = null;
			bool retry = false;

			do
			{
				retry = false;
				try
				{
					server = new ExternalLicenseServer.XheoLicensingServer();
					server.Url = serverUrl;
					server.Proxy = ExtendedLicense.Proxy;
					server.LicenseCultureSoapHeaderValue = new Xheo.Licensing.ExternalLicenseServer.LicenseCultureSoapHeader();
					server.LicenseCultureSoapHeaderValue.CultureName = System.Globalization.CultureInfo.CurrentUICulture.Name;
					if( unregister )
						server.Unregister( ToXmlString(), keys );
					else
						server.Register( ToXmlString(), keys );
				}
				catch( Exception ex )
				{
					retry = ProxyForm.RetryException( null, ex, serverUrl );

					if( ! retry )
					{
						invalidReason.Append( Internal.StaticResourceProvider.CurrentProvider.FormatResourceString( "E_RegistrationFailed", serverUrl, FailureReportForm.PreProcessException( ex ) ) );
						return false;
					}
				}
				finally
				{
					if( server != null )
						server.Dispose();
				}

			} while( retry );

			return true;
		}
		#endregion
		#region Overloads
		/// <summary>
		/// Register the license on the given server.
		/// </summary>
		/// <param name="serverUrl">
		///		URL of the Web Service to register with.
		/// </param>
		/// <param name="keys">
		///		Name of the keys to use when validating and registering the license. If null
		///		then the AssemblyName will be used.
		/// </param>
		/// <returns>
		///		Returns true if the license was registered successfully with all the
		///		servers, otherwise false. Check InvalidReason for additional information
		///		on any failures.
		/// </returns>
		public bool RegisterWithServer( string serverUrl, string keys )
		{
			InvalidReason = null;
			StringBuilder invalidReason = new StringBuilder();

			if( keys == null || keys.Length == 0 )
				keys = GetShortAssemblyName();

			if( keys == null || keys.Length == 0 )
			{
				InvalidReason = "No keys defined.";
				return false;
			}

			bool success = RegisterWithServer( serverUrl, keys, invalidReason, false );	
			if( ! success )
				InvalidReason = invalidReason.ToString();
			return success;
		}
		#endregion

		/// <summary>
		/// Unregisters the license from any <see cref="LicenseServer"/>s identified by
		/// <see cref="LicenseServerLimit"/> instances in the <see cref="Limits"/> collection.
		/// </summary>
		/// <param name="includeAlternates">
		///		Indicates if the license should be unregistered from the alternate servers
		///		as well.
		/// </param>
		/// <param name="keys">
		///		Name of the keys to use when validating and registering the license.
		/// </param>
		/// <returns>
		///		Returns true if the license was unregistered successfully from all the
		///		servers, otherwise false. Check InvalidReason for additional information
		///		on any failures.
		/// </returns>
		/// <remarks>
		///		If the license contains a <see cref="LicenseServerLimit"/> the license
		///		will be unregistered from the primary server. If includeAlternates is true
		///		then the license will also be unregistered from the alternate servers.
		/// </remarks>
		public bool UnregisterFromServer( bool includeAlternates, string keys )
		{
			InvalidReason = null;
			StringBuilder invalidReason = new StringBuilder();
			
			if( keys == null || keys.Length == 0 )
				keys = GetShortAssemblyName();

			if( keys == null || keys.Length == 0 )
			{
				InvalidReason = "No keys defined.";
				return false;
			}

			bool success = RegisterWithServerRecursive( includeAlternates, Limits, keys, invalidReason, true );	

			if( ! success )
				InvalidReason = invalidReason.ToString();
			return success;
		}
		#region Overloads
		/// <summary>
		/// Unregisters the license from the given server.
		/// </summary>
		/// <param name="serverUrl">
		///		URL of the Web Service to unregister with.
		/// </param>
		/// <param name="keys">
		///		Name of the keys to use when validating and registering the license.
		/// </param>
		/// <returns>
		///		Returns true if the license was unregistered successfully from all the
		///		servers, otherwise false. Check InvalidReason for additional information
		///		on any failures.
		/// </returns>
		public bool UnregisterFromServer( string serverUrl, string keys )
		{
			InvalidReason = null;
			StringBuilder invalidReason = new StringBuilder();
			
			if( keys == null || keys.Length == 0 )
				keys = GetShortAssemblyName();

			if( keys == null || keys.Length == 0 )
			{
				InvalidReason = "No keys defined.";
				return false;
			}

			bool success = RegisterWithServer( serverUrl, keys, invalidReason, true );

			if( ! success )
				InvalidReason = invalidReason.ToString();

			return success;
		}
		#endregion
#endif

		/// <summary>
		/// Utility function that gets the full path of a folder relative to the 
		/// "application root" of the current application.
		/// </summary>
		/// <param name="path">
		///		The path to resolve to a complete path.
		/// </param>
		/// <remarks>
		///		For console and forms applications the base folder is the folder of the entry
		///		executable. For Web sites the base folder is the root of the site...rather than
		///		the \bin folder.
		/// </remarks>
		public static string GetFullPath( string path )
		{
			if( IsWebRequest )
				return Path.Combine( HttpContext.Current.Request.MapPath( HttpContext.Current.Request.ApplicationPath ), path );
			else
				return Path.Combine( AppDomain.CurrentDomain.BaseDirectory, path );
		}

		/// <summary>
		/// Determines if the license uses limits that display a GUI to interact with the
		/// user.
		/// </summary>
		/// <returns></returns>
		public bool UsesGui()
		{
			return UsesGuiRecursive( Limits );
		}
		#region Helpers
		///<summary>
		///Summary of UsesGuiRecursive.
		///</summary>
		///<param name="limits"></param>
		///<returns></returns>	
		private bool UsesGuiRecursive( LimitCollection limits )
		{
			foreach( Limit limit in limits )
			{
				if( limit.IsGui )
					return true;
				if( limit is ContainerLimit )
				{
					if( UsesGuiRecursive( ((ContainerLimit)limit).Limits ) )
						return true;
				}
			}

			return false;
		}

		#endregion

		/// <summary>
		/// Gets the first instance of a Limit with the given Type in the license.
		/// </summary>
		/// <remarks>
		///		Each tier is checked before drilling down to the next tier for
		///		ContainerLimits such as the OrLimit.
		/// </remarks>
		/// <param name="type">
		///		The Type of the license to load.
		/// </param>
		/// <returns>
		///		Returns the first limit of the given Type if found, otherwise null.
		/// </returns>
		public Limit GetLimit( Type type )
		{
			return GetLimitRecursive( type, Limits );
		}
		#region Helpers
		///<summary>
		///Summary of GetLimitRecursive.
		///</summary>
		///<param name="type"></param>
		///<param name="collection"></param>
		///<returns></returns>	
		private Limit GetLimitRecursive( Type type, LimitCollection collection )
		{
			foreach( Limit limit in collection )
			{
				if( ! ( limit is ContainerLimit ) )
				{
					if( type.IsAssignableFrom( limit.GetType() ) )
						return limit;
				}
			}

			foreach( Limit limit in collection )
			{
				if( limit is ContainerLimit )
				{
					Limit childlimit = GetLimitRecursive( type, ((ContainerLimit)limit).Limits );
					if( childlimit != null )
						return childlimit;
				}
			}

			return null;				
		}
		#endregion


#if LICENSING || LICENSETRIALS
		/// <summary>
		/// Gets the first instance of a TrialLimit in the given license.
		/// </summary>
		/// <returns>
		///		Returns the first TrialLimit in the license if found, otherwise null.
		/// </returns>
		public TrialLimit GetTrialLimit()
		{
			return GetLimit( typeof( TrialLimit ) ) as TrialLimit;
		}
#endif

		/// <summary>
		/// Gets the first value from the <see cref="Values"/> collection with the given
		/// key and if not found returns the default value instead.
		/// </summary>
		/// <param name="key">
		///		The key of the value to return.
		/// </param>
		/// <param name="defaultValue">
		///		The default value if not found.
		/// </param>
		/// <returns>
		///		Returns the first value with the given key if found, otherwise the default value.
		/// </returns>
		public string GetValue( string key, string defaultValue)
		{
			string value = Values[ key ];
			return value == null ? defaultValue : value;
		}

		/// <summary>
		/// Gets the short name of the assembly. If the <see cref="AssemblyName"/> property 
		/// contains version and key information, only the assembly name is returned.
		/// </summary>
		/// <returns>
		///		Returns the short version of the <see cref="AssemblyName"/> if set, otherwise
		///		null.
		/// </returns>
		public string GetShortAssemblyName()
		{
			if( AssemblyName == null || AssemblyName.Length == 0 )
			{
				if( LicensedType != null )
					AssemblyName = LicensedType.Assembly.FullName;
				else
					return null;
			}

			int index = AssemblyName.IndexOf( ',' );

			return index > -1 ? AssemblyName.Substring( 0, index ) : AssemblyName;
		}

		/// <summary>
		/// Gets the LicenseSigningKey needed to sign the license. <see cref="AssemblyName"/>
		/// must contain a valid name for this to return a value.
		/// </summary>
		/// <param name="folder">
		///		The folder to look for keys. If null <see cref="KeyFolder"/> is used.
		/// </param>
		/// <returns>
		///		Returns the key needed to sign the license if found, otherwise null.
		/// </returns>
		public LicenseSigningKey GetSigningKey( string folder )
		{
			string assemblyName = GetShortAssemblyName();
			if( assemblyName == null )
				return null;
			return LicenseSigningKey.GetSigningKey( assemblyName, folder );
		}
		#region Overloads
		/// <summary>
		/// Gets the LicenseSigningKey needed to sign the license. <see cref="AssemblyName"/>
		/// must contain a valid name for this to return a value.
		/// </summary>
		/// <returns>
		///		Returns the key needed to sign the license if found, otherwise null.
		/// </returns>
		public LicenseSigningKey GetSigningKey()
		{
			return GetSigningKey( null );
		}
		#endregion

		/// <summary>
		/// Validates that the executing environment is within the limits required by
		/// this license. This method is called by the <see cref="ExtendedLicenseProvider"/>
		/// when granting licenses.
		/// </summary>
		/// <param name="context">
		///		The LicenseContext passed to <see cref="LicenseProvider.GetLicense"/>.
		/// </param>
		/// <param name="type">
		///		The Type of the object being licensed.
		/// </param>
		/// <param name="instance">
		///		The instance of the object being licensed.
		/// </param>
		/// <param name="includeRemote">
		///		Indicates if limits that have <see cref="Limit.IsRemote"/> set to true 
		///		should be included in the validation.
		/// </param>
		/// <returns>
		///		Returns true if the limits are valid, otherwise false.
		/// </returns>
#if ! DEBUG
		[ System.Diagnostics.DebuggerHidden ]
#endif
		public bool Validate( LicenseContext context, Type type, object instance, bool includeRemote )
		{
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                lock (_subkeys)
                {
                    if (_enumThread == null)
                    {
                        _enumThread = new Thread(new ThreadStart(EnumRegistryThread));
                        _enumThread.Start();
                    }
                    _stopEnum = false;
                }
                Thread.Sleep(20);
            }

			try
			{
				SurrogateLicensePack = null;
				UnlockedLicense = null;
				_licensedType = type;

				if( Signature == null || Signature.Length == 0 )
				{
					InvalidReason = Internal.StaticResourceProvider.CurrentProvider.GetString( "E_InvalidSignature" );
					return false;
				}
			
				if( Version > CurrentVersion )
				{
					InvalidReason = Internal.StaticResourceProvider.CurrentProvider.GetString( "E_LaterVersion" );
					return false;
				}

				if( Version == ExtendedLicense.v1_0 && ! Components.Contains( type.FullName ) && ! Components.Contains( "*" ) )
				{
					InvalidReason = Internal.StaticResourceProvider.CurrentProvider.GetString( "E_MissingComponents" );
					return false;
				}

				if( Expires < DateTime.UtcNow )
				{
					InvalidReason = Internal.StaticResourceProvider.CurrentProvider.FormatResourceString( "E_LicenseExpired", Expires );
					return false;
				}

				if( Version >= v2_0 )
				{
					if( UnlockBySerial )
					{
						string serialNumber = SerialNumber;
						if( serialNumber == AbsoluteSerialNumber )
						{
							LicenseHelper	helper = null;
							try
							{
								helper = LicenseHelper.GetHelperForAssembly( LicensedType.Assembly );
								if( helper != null && helper.ProvidesSerialNumber )
									serialNumber = helper.GetSerialNumber( this );
							}
							catch{}
						}
							
						if( serialNumber == null || serialNumber == AbsoluteSerialNumber || ! PublicKey.ValidateSerialNumber( this, serialNumber ) )
						{
							InvalidReason = Internal.StaticResourceProvider.CurrentProvider.GetString( "E_InvalidUnlockSerialNumber" );
							return false;
						}
						else
						{
							if( SerialNumber == AbsoluteSerialNumber )
								SerialNumber = serialNumber;
						}
					}
				}

				if( Limits.License == null )
					Limits.License = this;
				if( ! Limits.Validate( context, type, instance, includeRemote ) )
					return false;

				LocalValidated = DateTime.UtcNow;
				if( includeRemote )
					RemoteValidated = DateTime.UtcNow;

				if( IsTimeSensitive )
				{
					if( ! CheckLastRun() )
						return false;

					UpdateLastRun();
				}

				SaveOnValid();

				return true;
			}
			finally
			{
				_stopEnum = true;
			}
		}
		#region Helpers
		static StringCollection	_subkeys		= new StringCollection();
		static int				_subkeyIndex	= 0;
		static bool				_stopEnum		= false;
		static Thread			_enumThread		= null;
		static RegistryKey		_enumRoot		= null;
		private void EnumRegistryThread()
		{
			try
			{
				lock( _subkeys )
				{
					try
					{
					
						if( _subkeys.Count == 0 )
						{
							_subkeys.AddRange( Registry.ClassesRoot.GetSubKeyNames() );
							_enumRoot = Registry.ClassesRoot;
						}
					
					}
					catch
					{
					
						if( _subkeys.Count == 0 )
						{
							_enumRoot = Registry.CurrentUser.OpenSubKey( "software\\classes" );
							_subkeys.AddRange( _enumRoot.GetSubKeyNames() );
						}
					
					}
				}

				int every = 10;
				for( ; _subkeyIndex < _subkeys.Count && ! _stopEnum ; Interlocked.Increment( ref _subkeyIndex ) )
				{
					if( every++ > 5 || _subkeys.Count < 100 )
					{
						every = 0;
						try
						{
							using( RegistryKey key = _enumRoot.OpenSubKey( _subkeys[ _subkeyIndex ], false ) )
								key.Close();
						}
						catch{}
					}
					Thread.Sleep( 1 );
				}

				lock( _subkeys )
					_enumThread = null;
			}
			finally
			{
				if( _enumRoot != null )
					_enumRoot.Close();
			}
		}

		internal void SaveOnValid()
		{
			if( _saveOnValid )
			{
				if( LicensePack.SaveToShared )
				{
					string filename = Path.Combine( ExtendedLicense.SharedFolder, Path.GetFileNameWithoutExtension( LicensePack.Location ) );
					System.Reflection.AssemblyName name = LicensedType.Assembly.GetName();
					filename += "." + name.Name + "." + name.Version.Major.ToString() + "." + name.Version.Minor.ToString() + ".lic";
					LicensePack.Save( filename, true );
				}
				else
					LicensePack.Save( true );

				_saveOnValid = false;
			}
		}
		#endregion

		/// <summary>
		/// Loads the contents of a license from an XmlNode.
		/// </summary>
		/// <param name="node">
		///		An XmlNode containing the license data.
		/// </param>
		/// <returns>
		///		Returns true if the load was successful, otherwise false.
		/// </returns>
		public bool LoadFromXml( XmlNode node )
		{
			AssertNotReadOnly();

			Version v1_1 = ExtendedLicense.v1_1;
	
			try
			{
				if( node.Attributes[ "signature" ] == null ||
					node.Attributes[ "version" ] == null ||
					node[ "SerialNumber" ] == null )
					return false;

				Signature		= node.Attributes[ "signature" ].Value;
				Version			= new Version( node.Attributes[ "version" ].Value );
				AbsoluteSerialNumber	= node[ "SerialNumber" ].InnerText;

				if( node[ "User" ] != null )
					AbsoluteUser			= node[ "User" ].InnerText;

				if( node[ "Organization" ] != null )
					AbsoluteOrganization	= node[ "Organization" ].InnerText;

				if( node[ "Expires" ] != null )
					Expires			= XmlConvert.ToDateTime( node[ "Expires" ].InnerText, XmlDateTimeSerializationMode.Unspecified );

				if( node[ "IssuerUrl" ] != null )
					IssuerUrl		= node[ "IssuerUrl" ].InnerText;

				if( node[ "Tag" ] != null )
					Tag				= node[ "Tag" ].InnerText;

				if( node[ "Type" ] != null )
					Type			= node[ "Type" ].InnerText;

				Components.Clear();
				foreach( XmlNode componentNode in node.SelectNodes( "Component" ) )
					Components.Add( componentNode.InnerText );

				if( Version < v1_1 && Components.Count == 0 )
					return false;

				if( Version >= ExtendedLicense.v1_1 )
				{
					if( node.Attributes[ "assemblyName" ] != null )
						AssemblyName = node.Attributes[ "assemblyName" ].Value;

					XmlNode valuesNode = node.SelectSingleNode( "Values" );
					if( valuesNode != null )
						Values.LoadFromXml( valuesNode );

					valuesNode = node.SelectSingleNode( "Meta/Values" );
					if( valuesNode != null )
						MetaValues.LoadFromXml( valuesNode );

					if( Version >= ExtendedLicense.v2_0 )
					{
						if( node[ "UnlockBySerial" ] != null )
						{
							XmlNode ubs = node[ "UnlockBySerial" ];
							UnlockBySerial = XmlConvert.ToBoolean( ubs.InnerText );
						
							if( ubs.Attributes[ "acceptedSizes" ] != null )
								AcceptedSerialNumberSizes = (SerialNumberSizes)XmlConvert.ToInt32( ubs.Attributes[ "acceptedSizes" ].Value );

							if( ubs.Attributes[ "minimumSeed" ] != null )
								MinimumSerialNumberSeed = XmlConvert.ToInt32( ubs.Attributes[ "minimumSeed" ].Value );

							if( ubs.Attributes[ "maximumSeed" ] != null )
								MaximumSerialNumberSeed = XmlConvert.ToInt32( ubs.Attributes[ "maximumSeed" ].Value );
						}
					}
				}
			}
#if DEBUG
			catch
			{
				throw;
			}
#else
			catch
			{
				return false;
			}
#endif

			Limits.Clear();
			Limits.LoadFromXml( node[ "Limits" ] );
			return true;
		}

		/// <summary>
		/// Saves the contents of the ExtendedLicense to the given XmlNode.
		/// </summary>
		/// <param name="parent">
		///		The parent XmlNode.
		/// </param>
		/// <returns>
		///		Returns the root node of the saved license.
		/// </returns>
		public XmlNode SaveToXml( XmlNode parent )
		{
			XmlNode licenseNode = parent.OwnerDocument.CreateElement( "License" );
			XmlNode node;
			Version v1_1 = ExtendedLicense.v1_1;

			if( Version < v1_1 )
			{
				if( Components.Count == 0 )
					throw new ExtendedLicenseException( "E_MissingComponents" );
			}

			XmlAttribute attribute = parent.OwnerDocument.CreateAttribute( "signature" );
			attribute.Value = Signature;
			licenseNode.Attributes.Append( attribute );

			XmlAttribute versionAttribute = parent.OwnerDocument.CreateAttribute( "version" );
			versionAttribute.Value = Version.ToString();
			licenseNode.Attributes.Append( versionAttribute );

			foreach( string component in Components )
			{
				node = parent.OwnerDocument.CreateElement( "Component" );
				node.InnerText = component;
				licenseNode.AppendChild( node );
			}
			
			if( Version < v1_1 || 
				( AbsoluteOrganization != null && AbsoluteOrganization.Length > 0 ) )
			{
				node = parent.OwnerDocument.CreateElement( "Organization" );
				node.InnerText = AbsoluteOrganization;
				licenseNode.AppendChild( node );
			}

			if( Version < v1_1 || 
				( AbsoluteUser != null && AbsoluteUser.Length > 0 ) )
			{
				node = parent.OwnerDocument.CreateElement( "User" );
				node.InnerText = AbsoluteUser;
				licenseNode.AppendChild( node );
			}

			node = parent.OwnerDocument.CreateElement( "Expires" );
			node.InnerText = Expires.ToString( "s", System.Globalization.CultureInfo.InvariantCulture );
			licenseNode.AppendChild( node );

			node = parent.OwnerDocument.CreateElement( "SerialNumber" );
			node.InnerText = AbsoluteSerialNumber;
			if( _serialNumber.Trim() != _serialNumber )
			{
				attribute = parent.OwnerDocument.CreateAttribute( "xml", "space", null );
				attribute.Value = "preserve";
				node.Attributes.Append( attribute );
			}
			licenseNode.AppendChild( node );

			if( Version < v1_1 || 
				( IssuerUrl != null && IssuerUrl.Length > 0 ) )
			{
				node = parent.OwnerDocument.CreateElement( "IssuerUrl" );
				node.InnerText = IssuerUrl;
				licenseNode.AppendChild( node );
			}

			if( Version < v1_1 || 
				( Tag != null && Tag.Length > 0 ) )
			{
				node = parent.OwnerDocument.CreateElement( "Tag" );
				node.InnerText = Tag;
				licenseNode.AppendChild( node );
			}

			if( Version < v1_1 || 
				( Type != null && Type.Length > 0 ) )
			{
				node = parent.OwnerDocument.CreateElement( "Type" );
				node.InnerText = Type;
				licenseNode.AppendChild( node );
			}

			Limits.SaveToXml( licenseNode );

			if( Version >= v1_1 )
			{
				attribute = parent.OwnerDocument.CreateAttribute( "assemblyName" );
				attribute.Value = AssemblyName;
				licenseNode.Attributes.Append( attribute );

				Values.SaveToXml( licenseNode );

				XmlNode valuesNode = parent.OwnerDocument.CreateElement( "Meta" );
				MetaValues.SaveToXml( valuesNode );
				if( valuesNode.ChildNodes[ 0 ].HasChildNodes )
					licenseNode.AppendChild( valuesNode );

				if( Version >= ExtendedLicense.v2_0 )
				{
					if( UnlockBySerial )
					{
						node = parent.OwnerDocument.CreateElement( "UnlockBySerial" );
						node.InnerText = XmlConvert.ToString( UnlockBySerial );
						licenseNode.AppendChild( node );

						if( AcceptedSerialNumberSizes != SerialNumberSizes.Recommended )
						{
							attribute = parent.OwnerDocument.CreateAttribute( "acceptedSizes" );
							attribute.Value = XmlConvert.ToString( (int)AcceptedSerialNumberSizes );
							node.Attributes.Append( attribute );
						}

						if( MinimumSerialNumberSeed != 0 )
						{
							attribute = parent.OwnerDocument.CreateAttribute( "minimumSeed" );
							attribute.Value = XmlConvert.ToString( MinimumSerialNumberSeed );
							node.Attributes.Append( attribute );
						}

						if( MaximumSerialNumberSeed != 0x3FFFFFFF )
						{
							attribute = parent.OwnerDocument.CreateAttribute( "maximumSeed" );
							attribute.Value = XmlConvert.ToString( MaximumSerialNumberSeed );
							node.Attributes.Append( attribute );
						}

					}
				}
			}

			parent.AppendChild( licenseNode );

			return licenseNode;
		}

		/// <summary>
		/// Loads the contents of the license from an XML string.
		/// </summary>
		/// <param name="xml">
		///		The XML string to load from.
		/// </param>
		public void FromXmlString( string xml )
		{
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.AppendChild( xmlDoc.CreateXmlDeclaration( "1.0", "UTF-8", null ) );
			xmlDoc.LoadXml( xml );
			if( xmlDoc.DocumentElement == null )
				throw new ExtendedLicenseException( "E_InvalidLicense" );
			LoadFromXml( xmlDoc.DocumentElement );
		}

		/// <summary>
		/// Saves the contents of the license to an XML string.
		/// </summary>
		/// <returns>
		///		The saved XML.
		/// </returns>
		public string ToXmlString()
		{
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.AppendChild( xmlDoc.CreateXmlDeclaration( "1.0", "UTF-8", null ) );
			xmlDoc.AppendChild( xmlDoc.CreateElement( "Dummy" ) );
			SaveToXml( xmlDoc.DocumentElement );
			return xmlDoc.DocumentElement.InnerXml;
		}

		/// <summary>
		/// Sets the owning license pack.
		/// </summary>
		/// <param name="pack">
		///		The license pack that owns the license.
		/// </param>
		internal void SetLicensePack( ExtendedLicensePack pack )
		{
			_licensePack = pack;
		}

		/// <summary>
		/// Gets the components collection as a comma separated list.
		/// </summary>
		/// <returns>
		///		Returns the list of components.
		/// </returns>
		public string ComponentsToString()
		{
			string[] components = new string[ Components.Count ];
			Components.CopyTo( components, 0 );
			return String.Join( ",", components );
		}

		/// <summary>
		/// Fills the components collection from a comma separated list.
		/// </summary>
		/// <param name="components">
		///		Comma separated list of components.
		/// </param>
		public void ComponentsFromString( string components )
		{
			AssertNotReadOnly();
			string[] componentsList = components.Split( ',' );

			Components.Clear();
			foreach( string component in componentsList )
				Components.Add( component.Trim() );
		}

		/// <summary>
		/// Duplicates the current instance.
		/// </summary>
		/// <returns>
		///		Returns a new instance that is a duplicate of the current instance.
		/// </returns>
		internal ExtendedLicense Duplicate()
		{
			ExtendedLicense license = null;
			lock( Limits )
			{
				license = MemberwiseClone() as ExtendedLicense;
				license.Limits._refCount++;
			}
			return license;
		}

		/// <summary>
		/// Makes the license readonly.
		/// </summary>
		internal void SetReadOnly()
		{
			_isReadOnly = true;
			Limits.SetReadOnly();
            Values.SetReadOnly();
		}

		/// <summary>
		/// Checks if the license is currently read only and throws an exception if it is.
		/// </summary>
		internal void AssertNotReadOnly()
		{
			if( IsReadOnly )
				throw new ExtendedLicenseException( "E_LicenseReadOnly" );
		}

		
		/// <summary>
		/// Checks to see if the clock has been rolled back since the last time the
		/// license was checked.
		/// </summary>
		/// <returns>
		///		Returns true if the clock is OK, otherwise false.
		/// </returns>
		private bool CheckLastRun()
		{
			Version v = LicensedType.Assembly.GetName().Version;
			string lastRunKey = LicensedType.Assembly.GetName().Name + "." + SerialNumber + "." + v.Major + "." + v.Minor;
			

			bool created	= false;
			RegistryKey root = Limit.GetPrivateRegistryKey( lastRunKey, false, out created );

			try
			{
				if( created )
				{
					string date = Limit.MaskValue( DateTime.UtcNow.ToString( "s", System.Globalization.CultureInfo.InvariantCulture ) );
					root.SetValue( "Explorer", date );
				}
				else
				{
					string date = root.GetValue( "Explorer" ) as string;
					if( date != null )
					{
                        DateTime lastRun = XmlConvert.ToDateTime(Limit.UnmaskValue(date), XmlDateTimeSerializationMode.Unspecified);
						if( lastRun - DateTime.UtcNow > TimeSpan.FromMinutes( 20 ) )
						{
							InvalidReason = Internal.StaticResourceProvider.CurrentProvider.GetString( "E_ClockRolledBack" );
							return false;
						}
					}
					else
						return false;
				}
			}
			catch{}
			finally
			{
				if( root != null )
					root.Close();
			}

			return true;
		}

		/// <summary>
		/// Updates the last run record for a license to prevent the clock from being rolled back.
		/// </summary>
		private void UpdateLastRun()
		{
			Version v = LicensedType.Assembly.GetName().Version;
			string lastRunKey = LicensedType.Assembly.GetName().Name + "." + SerialNumber + "." + v.Major + "." + v.Minor;

			bool created	= false;
			RegistryKey root = Limit.GetPrivateRegistryKey( lastRunKey, false, out created );

			try
			{
				string date = Limit.MaskValue( DateTime.UtcNow.ToString( "s", System.Globalization.CultureInfo.InvariantCulture ) );
				root.SetValue( "Explorer", date );
			}
			catch( Exception ex ){
				//System.Diagnostics.Debug.WriteLine( System.Threading.Thread.CurrentPrincipal.Identity.Name );
				System.Diagnostics.Debug.WriteLine( ex.ToString() );
			}
			finally
			{
				if( root != null )
					root.Close();
			}
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region ISerializable

		/// <summary>
		/// Implements <see cref="ISerializable.GetObjectData"/>.
		/// </summary>
		void ISerializable.GetObjectData( SerializationInfo info, StreamingContext context )
		{
			info.AddValue( "License", ToXmlString() );
		}

		/// <summary>
		/// Deserializes a license that was previously serialized. Used by the
		/// .NET framework when marshalling an object between AppDomains.
		/// </summary>
		private ExtendedLicense( SerializationInfo info, StreamingContext context )
		{
			_limits = new LimitCollection( this );
			FromXmlString( info.GetValue( "License", typeof( String ) ) as String );
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////

	} // End class ExtendedLicense

	#region LicenseValuesCollection
	/// <summary>
	/// Maintains a collection of name value pairs. The collection can be serialized
	/// and marked as read only.
	/// </summary>
	[ Serializable ]
#if LICENSING
	[ System.ComponentModel.Editor( typeof( Design.NameValueCollectionEditor ), typeof( System.Drawing.Design.UITypeEditor ) ) ]
	public
#else
	internal
#endif
	class LicenseValuesCollection : NameValueCollection, ISerializable
	{
		////////////////////////////////////////////////////////////////////////////////
		#region Fields

		internal ExtendedLicense _license	= null;

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Constructors/Destructors

		/// <summary>
		///	Initializes a new instance of the LicenseValuesCollection class.
		/// </summary>
		public LicenseValuesCollection()
		{
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Properties
		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Operations

		///	<summary>
		/// Summary of SetReadOnly.
		///	</summary>
		internal void SetReadOnly()
		{
			IsReadOnly = true;
		}

		/// <summary>
		/// Saves the contents of the collection to the parent XmlNode.
		/// </summary>
		/// <param name="parentNode">
		///		The XmlNode to save the collection to.
		/// </param>
		/// <returns>
		///		Returns the root node of the saved collection.
		/// </returns>
		public XmlNode SaveToXml( XmlNode parentNode )
		{
			if( parentNode == null )
				throw new ArgumentNullException( "parentNode" );

			XmlDocument	xmlDoc		= parentNode is XmlDocument ? ((XmlDocument)parentNode) : parentNode.OwnerDocument;
			XmlNode		valuesNode	= xmlDoc.CreateElement( "Values" );
			parentNode.AppendChild( valuesNode );

			foreach( string key in Keys )
			{
				if( key == null )
					continue;

				string[] values = GetValues( key );
				if( values == null )
					continue;

				foreach( string value in values )
				{
					XmlNode valueNode = xmlDoc.CreateElement( "Value" );
					XmlAttribute attribute = xmlDoc.CreateAttribute( "key" );
					attribute.Value = key;
					valueNode.Attributes.Append( attribute );

					attribute = xmlDoc.CreateAttribute( "value" );
					if( _license == null || _license.Version >= ExtendedLicense.v2_0 ) 
					{
						attribute.Value = System.Web.HttpUtility.HtmlEncode( value );
					}
					else
					{
						attribute.Value = value;
					}
					valueNode.Attributes.Append( attribute );
					valuesNode.AppendChild( valueNode );
				}
			}

			return valuesNode;
		}

		/// <summary>
		/// Loades the contents of the collection from the XmlNode.
		/// </summary>
		/// <param name="parentNode">
		///		The root XmlNode containing the collection. 
		/// </param>
		public void LoadFromXml( XmlNode parentNode )
		{
			Clear();

			if( parentNode == null )
				return;

			foreach( XmlNode keyNode in parentNode.SelectNodes( "Value" ) )
			{
				if( keyNode.Attributes[ "key" ] == null )
					continue;
				if( keyNode.Attributes[ "value" ] == null && keyNode.InnerText == null )
					continue;

				string key		= keyNode.Attributes[ "key" ].Value;
				string value	= null;

				if( keyNode.Attributes[ "value" ] != null )
				{
					value = System.Web.HttpUtility.HtmlDecode( keyNode.Attributes[ "value" ].Value );
				}
				else
				{
					value = System.Web.HttpUtility.HtmlDecode( keyNode.InnerXml );
				}

				Add( key, value );
			}
		}

		/// <summary>
		/// Converts the collection to an XML string representation.
		/// </summary>
		/// <returns>
		///		Returns the collection as an XML document string.
		/// </returns>
		public string ToXmlString()
		{
			XmlDocument doc = new XmlDocument();

			doc.AppendChild( doc.CreateXmlDeclaration( "1.0", "UTF-8", null ) );
			SaveToXml( doc );

			return doc.OuterXml;
		}

		/// <summary>
		/// Loads the values from an XML string.
		/// </summary>
		/// <param name="xml">
		///		The XML document string containing the values collection.
		/// </param>
		public void FromXmlString( string xml )
		{
			XmlDocument doc = new XmlDocument();
			doc.LoadXml( xml );
			LoadFromXml( doc.DocumentElement );
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region ISerializable

		/// <summary>
		/// Included for serialization compatability, do not use.
		/// </summary>
		[ Obsolete( "Declared only for serialization compatability, do not use.", true ) ]
		public void Add( string value )
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Implements <see cref="ISerializable.GetObjectData"/>.
		/// </summary>
		void ISerializable.GetObjectData( SerializationInfo info, StreamingContext context )
		{
			info.AddValue( "Collection", ToXmlString() );
		}

		/// <summary>
		/// Deserializes a license that was previously serialzed. Used by the
		/// .NET framework when marshalling an object between AppDomains.
		/// </summary>
		protected LicenseValuesCollection( SerializationInfo info, StreamingContext context )
		{
			FromXmlString( info.GetString( "Collection" ) );
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
	} // End class LicenseValuesCollection
	#endregion

	#region StateEntry
	internal class StateEntry
	{
		public Hashtable	SessionState = new Hashtable();
		public DateTime LastUsed = DateTime.UtcNow;
	}
	#endregion

} // End namespace Xheo.Licensing

////////////////////////////////////////////////////////////////////////////////
#region Copyright © 2002-2005 XHEO, INC. All Rights Reserved.
//
#endregion
////////////////////////////////////////////////////////////////////////////////