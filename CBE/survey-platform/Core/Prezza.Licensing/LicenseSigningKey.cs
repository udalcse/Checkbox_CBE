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
// Class:		LicenseSigningKey
// Author:		Paul Alexander
// Created:		Monday, September 16, 2002 1:02:00 AM
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Xml;
using System.Security;
using System.Security.Cryptography;
using System.IO;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.Web;
using System.Configuration;
using System.Reflection;
using System.Collections;
using Internal;

namespace Xheo.Licensing
{
	/// <summary>
	/// Implements a key pair for signing licenses. 
	/// </summary>
	/// <remarks>
	///		See <see cref="ExtendedLicense"/> for details on how this class works with
	///		others in the library to enforce component licenses.
	/// </remarks>
	[ LicenseProvider( typeof( ExtendedLicenseProvider ) ) ]
#if LICENSING
	public 
#else
	internal
#endif
		sealed class LicenseSigningKey : IDisposable
	{
		////////////////////////////////////////////////////////////////////////////////
		#region Fields

		internal static readonly Version v1_0			= new Version( 1, 0 );
		internal static readonly Version v2_0			= new Version( 2, 0 );

		internal const string	XLPROKEY				= "XL_PRO";
		internal const string	XLPROCLIENTKEY			= "__XL_PRO";

		/// <summary>
		/// The current version of the LicenseSigningKey class. 
		/// </summary>
		/// <remarks>
		/// Later versions may introduce a new signing/file scheme. To maintain backwards 
		/// compatability the keys are versioned.
		/// </remarks>
#if SPECIALBUILD
		public static readonly Version CurrentVersion	= v1_0;
#else
		public static readonly Version CurrentVersion	= v2_0;
#endif	
		private bool						_initialized	= false;
		private RSACryptoServiceProvider	_key;
		private RandomNumberGenerator		_rng;
//		[ CLSCompliant( false) ]
		private KeyRecord					_rsa;
		private Version						_version	= CurrentVersion;
		private License						_license	= null;
		private KeyRecord[]					_snKeys		= null;
		internal string						_location	= null;

		private static	LicenseSigningKey	_globalKeys	= null;
		private static	Hashtable			_cachedKeys = new Hashtable();

		
		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Constructors/Destructors

		/// <summary>
		///	Initializes a new instance of the LicenseSigningKey class.
		/// </summary>
		public LicenseSigningKey()
		{
			// InitKey( true );
		}
		#region Helpers
		/// <summary>
		/// 
		/// </summary>
		/// <param name="initialize">
		///		Indicates if the keys must be initialized or just created.
		/// </param>
		private void InitKey( bool initialize )
		{
			_initialized	 = true;
			if( initialize )
			{
				try
				{
					do
					{
						_key = MakeKey( 1024 );
						RSAParameters rsap = _key.ExportParameters( true );
						_rsa = new KeyRecord( 0, 0, 0, 0, 0, 1024 );
						_rsa.d = new BigInteger( rsap.D );
						_rsa.e = new BigInteger( rsap.Exponent );
						_rsa.n = new BigInteger( rsap.Modulus );
						_rsa.p = new BigInteger( rsap.P );
						_rsa.q = new BigInteger( rsap.Q );

					} while( ! _rsa.VerifyKey() );
				}
				catch
				{
					// BUG in KeyRecord doubles size of key so we pass half of 1024 to get correct size.
					_rsa = new KeyRecord( 512 );
					_rsa.Bits = 1024;
				}
			}
			else
				_rsa = new KeyRecord( 0, 0, 0, 0, 0, 0 );

			_rng = RandomNumberGenerator.Create();
	
			if( ExtendedLicense.EmulateVersion != null )
			{
				if( ExtendedLicense.EmulateVersion < v2_0 )
					Version = v1_0;
			}
		}

		internal static RSACryptoServiceProvider MakeKey( int size )
		{
			RSACryptoServiceProvider key;
			string keyName = "Xheo.Licensing" + System.Threading.Thread.CurrentPrincipal.Identity.Name;
			try
			{
				CspParameters cspParameters = new CspParameters( 1 );
				cspParameters.KeyContainerName = null;
				cspParameters.Flags &= ~( CspProviderFlags.UseDefaultKeyContainer );
				key = new RSACryptoServiceProvider( size, cspParameters );
				key.PersistKeyInCsp = false;
			}
			catch
			{
				try
				{
					CspParameters cspParameters = new CspParameters( 1 );
					cspParameters.KeyContainerName = keyName;
					cspParameters.Flags &= ~( CspProviderFlags.UseDefaultKeyContainer );
					key = new RSACryptoServiceProvider( size, cspParameters );
					key.PersistKeyInCsp = false;
				}
				catch
				{
					try
					{
						CspParameters cspParameters = new CspParameters();
						cspParameters.Flags = CspProviderFlags.UseMachineKeyStore;
						cspParameters.KeyContainerName = null;
						key = new RSACryptoServiceProvider( size, cspParameters );
						key.PersistKeyInCsp = false;
					}
					catch
					{
						CspParameters cspParameters = new CspParameters();
						cspParameters.Flags = CspProviderFlags.UseMachineKeyStore;
						cspParameters.KeyContainerName = keyName;
						key = new RSACryptoServiceProvider( size, cspParameters );
						key.PersistKeyInCsp = false;
					}
				}
			}

			return key;
		}

		#endregion

		/// <summary>
		///	Initializes a new instance of the LicenseSigningKey class with the key from
		///	the given file.
		/// </summary>
		/// <param name="filename">
		///		Path to the file where the LicenseSigningKey is saved. 
		/// </param>
		public LicenseSigningKey( string filename )
		{
			Load( filename );
		}

		/// <summary>
		///	Initializes a new instance of the LicenseSigningKey class with the key from
		///	the given Stream object.
		/// </summary>
		/// <param name="stream">
		///		The Stream object that contains the data.
		///	</param>
		public LicenseSigningKey( Stream stream )
		{
			Load( stream );
		}

		/// <summary>
		/// Implements <see cref="IDisposable.Dispose"/>.
		/// </summary>
		public void Dispose()
		{
			if( _license != null )
			{
				_license.Dispose();
				_license = null;
			}

			if( _key != null )
			{
				((IDisposable)_key).Dispose();
				_key = null;
			}

			GC.SuppressFinalize( this );
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Properties

		/// <summary>
		/// Gets the global key used by the trial version.
		/// </summary>
		private LicenseSigningKey GlobalKeys
		{
			get
			{
				if( _globalKeys == null )
				{
					lock( typeof( LicenseSigningKey ) )
					{
						if( _globalKeys == null )
						{
							_globalKeys = new LicenseSigningKey();
							_globalKeys.Load( typeof( LicenseSigningKey ).Assembly.GetManifestResourceStream( "Xheo.Licensing.Public.lsk" ) );
						}
					}
				}

				return _globalKeys;
			}
		}


		/// <summary>
		/// Gets the serial number keys
		/// </summary>
		private KeyRecord[] SnKeys
		{
			get
			{
				if( Version < v2_0 )
					return null;
				if( _snKeys == null )
				{
					_snKeys = new KeyRecord[] {
												  new KeyRecord( 32 ),
												  new KeyRecord( 48 ),
												  new KeyRecord( 64 ),
												  new KeyRecord( 160 ),
					};
				}
				if( ! _initialized )
					InitKey( true );
				return _snKeys;
			}
		}

		/// <summary>
		/// Gets a value that indicates if the keys are the global keys used during the trial.
		/// </summary>
		public bool IsGlobalKey
		{
			get
			{
				XmlDocument global	= GlobalKeys.GetKeyAsXml( false, SerialNumberSizes.None );
				XmlDocument key		= GetKeyAsXml( false, SerialNumberSizes.None );

				return global.DocumentElement[ "RSAKeyValue" ][ "Modulus" ].InnerText == key.DocumentElement[ "RSAKeyValue" ][ "Modulus" ].InnerText;
			}
		}

		/// <summary>
		/// Gets the location where the keys were loaded from.
		/// </summary>
		public string Location
		{
			get
			{
				return _location;
			}
		}

		/// <summary>
		/// Gets or sets the version of the signing key.
		/// </summary>
		public Version Version
		{
			get
			{
				return _version;
			}
			set
			{
				if( CurrentVersion.CompareTo( value ) < 0 )
					throw new ExtendedLicenseException( "E_InvalidKeysVersion", CurrentVersion );
                
				_version = value;
			}
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Operations

		///<summary>
		///Summary of AssertLicensed.
		///</summary>
		private void AssertLicensed()
		{
#if ! NOLICENSE && LICENSING
			if( _license == null )
				_license	= LicenseManager.Validate( typeof( LicenseSigningKey ), this );

#if LICENSING
			if( ((ExtendedLicense)_license).IsTrial )
				Load( typeof( LicenseSigningKey ).Assembly.GetManifestResourceStream( "Xheo.Licensing.Public.lsk" ) );
#endif

#endif
		}

		/// <summary>
		/// Saves the contents of the LicenseSigningKey to a file with the given
		/// file name.
		/// </summary>
		/// <param name="filename">
		///		Path to the file where the LicenseSigningKey should be saved.
		/// </param>
		/// <param name="includePrivateData">
		///		Indicates if the private portion of the key should be included.
		/// </param>
		[ Obsolete( "Use Save( string ) to save the private key and, Export( string, SerialNumberSizes ) to export the public key." ) ]
		public void Save( string filename, bool includePrivateData )
		{
			AssertLicensed();
			GetKeyAsXml( includePrivateData ).Save( filename );
		}

		/// <summary>
		/// Saves the contents of the LicenseSigningKey to a file with the given
		/// file name.
		/// </summary>
		/// <param name="filename">
		///		Path to the file where the LicenseSigningKey should be saved.
		/// </param>
		public void Save( string filename )
		{
			AssertLicensed();
			GetKeyAsXml( true ).Save( filename );
		}

		/// <summary>
		/// Saves the public key contents of the LicenseSigningKey to a file with the given
		/// file name.
		/// </summary>
		/// <param name="filename">
		///		Path to the file where the LicenseSigningKey should be saved.
		/// </param>
		public void Export( string filename )
		{
			AssertLicensed();
			GetKeyAsXml( false ).Save( filename );
		}

		/// <summary>
		/// Saves the contents of the LicenseSigningKey to a Stream object.
		/// </summary>
		/// <param name="stream">
		///		The Stream object to receive the data.
		/// </param>
		/// <param name="includePrivateData">
		///		Indicates if the private portion of the key should be included.
		/// </param>
		[ Obsolete( "Use Save( Stream ) to save the private key and, Export( Stream, SerialNumberSizes ) to export the public key." ) ]
		public void Save( Stream stream, bool includePrivateData )
		{
			AssertLicensed();
			GetKeyAsXml( includePrivateData ).Save( stream );
		}

		/// <summary>
		/// Saves the contents of the LicenseSigningKey to a Stream object.
		/// </summary>
		/// <param name="stream">
		///		The Stream object to receive the data.
		/// </param>
		public void Save( Stream stream )
		{
			AssertLicensed();
			GetKeyAsXml( true ).Save( stream );
		}

		/// <summary>
		/// Saves the contents of the LicenseSigningKey to a Stream object.
		/// </summary>
		/// <param name="stream">
		///		The Stream object to receive the data.
		/// </param>
		public void Export( Stream stream )
		{
			AssertLicensed();
			GetKeyAsXml( false ).Save( stream );
		}

		/// <summary>
		/// Loads the LicenseSigningKey with the contents of the given file.
		/// </summary>
		/// <param name="filename">
		///		Path to the file where the LicenseSigningKey is saved.
		/// </param>
		public void Load( string filename )
		{
			_location = filename;
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load( filename );
			LoadKeyFromXml( xmlDoc );			
		}

		/// <summary>
		/// Loads the LicenseSigningKey from a Stream object.
		/// </summary>
		/// <param name="stream">
		///		The Stream object that contains the data.
		/// </param>
		public void Load( Stream stream )
		{
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load( stream );
			LoadKeyFromXml( xmlDoc );
		}

		/// <summary>
		/// Gets the LicenseSigningKey as an XML strign.
		/// </summary>
		/// <param name="includePrivateData">
		///		Indicates if the private data shouls be included.
		/// </param>
		/// <returns>
		///		Returns an XML string representation of the object.
		/// </returns>
		public string ToXmlString( bool includePrivateData )
		{
			XmlNode node = GetKeyAsXml( includePrivateData );
			if( !( node is XmlDocument ) )
				node = node.OwnerDocument;

			using( MemoryStream ms = new MemoryStream() )
			{
				using( TextWriter writer = new StreamWriter( ms, new UTF8Encoding( false ) ) )
					((XmlDocument)node).Save( writer );

				return Encoding.UTF8.GetString( ms.ToArray() ); // ((XmlDocument)node).DocumentElement.OuterXml;
			}
		}

		/// <summary>
		/// Initializes the contents of the LicenseSigningKey from the XML string.
		/// </summary>
		/// <param name="xml">
		///		A string containing an XML representaion of the key.
		/// </param>
		public void FromXmlString( string xml )
		{
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.LoadXml( xml );
			LoadKeyFromXml( xmlDoc );
		}

		/// <summary>
		/// Persists the LicenseSigningKey into an XML document object.
		/// </summary>
		/// <param name="includePrivateData">
		///		True if the private data should be included.
		/// </param>
		/// <param name="sizes">
		///		The valid serial number sizes when exporting the public key.
		/// </param>
		/// <returns>
		///		Returns the persisted XML as an <see cref="XmlDocument"/>.
		/// </returns>
		private XmlDocument GetKeyAsXml( bool includePrivateData, SerialNumberSizes sizes )
		{
			if( ! _initialized )
				InitKey( true );
			XmlDocument xmlDoc = new XmlDocument();
			XmlNode rootNode = xmlDoc.CreateElement( "SigningKey" );
			XmlAttribute versionAttribute = xmlDoc.CreateAttribute( "version" );
			versionAttribute.Value = Version.ToString();
			rootNode.Attributes.Append( versionAttribute );
			xmlDoc.AppendChild( rootNode );
			
			XmlNode rsaNode = xmlDoc.CreateElement( "RSAKeyValue" );
			rootNode.AppendChild( rsaNode );

			XmlNode subNode = xmlDoc.CreateElement( "Modulus" );
			subNode.InnerXml = Convert.ToBase64String( _rsa.n.getBytes() );
			rsaNode.AppendChild( subNode );

			subNode = xmlDoc.CreateElement( "Exponent" );
			subNode.InnerXml = Convert.ToBase64String( _rsa.e.getBytes() );
			rsaNode.AppendChild( subNode );

			if( includePrivateData && _rsa.p != null && _rsa.p != 0 )
			{
				subNode = xmlDoc.CreateElement( "P" );
				subNode.InnerXml = Convert.ToBase64String( _rsa.p.getBytes() );
				rsaNode.AppendChild( subNode );

				subNode = xmlDoc.CreateElement( "Q" );
				subNode.InnerXml = Convert.ToBase64String( _rsa.q.getBytes() );
				rsaNode.AppendChild( subNode );

				subNode = xmlDoc.CreateElement( "DP" );
				BigInteger dp = _rsa.d % ( _rsa.p - 1 );
				subNode.InnerXml = Convert.ToBase64String( dp.getBytes() );
				rsaNode.AppendChild( subNode );

				subNode = xmlDoc.CreateElement( "DQ" );
				BigInteger dq = _rsa.d % ( _rsa.q - 1 );
				subNode.InnerXml = Convert.ToBase64String( dq.getBytes() );
				rsaNode.AppendChild( subNode );

				subNode = xmlDoc.CreateElement( "InverseQ" );
				BigInteger invq = _rsa.q.modInverse( _rsa.p );
				subNode.InnerXml = Convert.ToBase64String( invq.getBytes() );
				rsaNode.AppendChild( subNode );

				subNode = xmlDoc.CreateElement( "D" );
				subNode.InnerXml = Convert.ToBase64String( _rsa.d.getBytes() );
				rsaNode.AppendChild( subNode );
			}

			xmlDoc.AppendChild( rootNode );

			if( Version >= v2_0 )
			{
				XmlNode snKeysNode = xmlDoc.CreateElement( "SNKeys" );
				if( sizes != SerialNumberSizes.None )
				{
					AddSnKeyToNode( snKeysNode, SnKeys[ 0 ], includePrivateData );
					AddSnKeyToNode( snKeysNode, SnKeys[ 1 ], includePrivateData );
					AddSnKeyToNode( snKeysNode, SnKeys[ 2 ], includePrivateData );
					AddSnKeyToNode( snKeysNode, SnKeys[ 3 ], includePrivateData );
				}
				rootNode.AppendChild( snKeysNode );
			}
			return xmlDoc;
		}
		#region Overloads
		private XmlDocument GetKeyAsXml( bool includePrivateData )
		{
			return GetKeyAsXml( includePrivateData, 
				SerialNumberSizes.Medium | SerialNumberSizes.Large | SerialNumberSizes.Huge );
		}
		#endregion
		#region Helpers
		private void AddSnKeyToNode( XmlNode snKeysNode, KeyRecord key, bool includePrivateData )
		{
			XmlDocument		doc		= snKeysNode.OwnerDocument;
			XmlNode			node	= doc.CreateElement( "Key" );
			XmlAttribute	attr;

			attr = doc.CreateAttribute( "size" );
			attr.Value = XmlConvert.ToString( key.Bits );
			node.Attributes.Append( attr );

			attr = doc.CreateAttribute( "e" );
			attr.Value = Convert.ToBase64String( key.e.getBytes() );
			node.Attributes.Append( attr );

			attr = doc.CreateAttribute( "n" );
			attr.Value = Convert.ToBase64String( key.n.getBytes() );
			node.Attributes.Append( attr );

			if( includePrivateData )
			{
				attr = doc.CreateAttribute( "p" );
				attr.Value = Convert.ToBase64String( key.p.getBytes() );
				node.Attributes.Append( attr );

				attr = doc.CreateAttribute( "q" );
				attr.Value = Convert.ToBase64String( key.q.getBytes() );
				node.Attributes.Append( attr );

				attr = doc.CreateAttribute( "d" );
				attr.Value = Convert.ToBase64String( key.d.getBytes() );
				node.Attributes.Append( attr );
			}

			snKeysNode.AppendChild( node );
		}
		#endregion

		/// <summary>
		/// Loads the state of the LicenseSigningKey from an XML document.
		/// </summary>
		/// <param name="xmlDoc">
		///		The XmlDocument that contains the LicenseSigningKey.
		/// </param>
		private void LoadKeyFromXml( XmlDocument xmlDoc )
		{
			if( ! _initialized )
				InitKey( false );

			XmlNode rootNode = xmlDoc.DocumentElement;

			if( rootNode.LocalName != "SigningKey" )
				throw new ExtendedLicenseException( "E_InvalidKeyFile" );

			if( rootNode.Attributes[ "version" ] == null )
				throw new ExtendedLicenseException( "E_InvalidKeyFile" );

			Version = new Version( rootNode.Attributes[ "version" ].Value );
			_rsa = new KeyRecord( 0, 0, 0, 0, 0, 1024 );
			
			XmlNode node = rootNode.SelectSingleNode( "RSAKeyValue/P" );
			byte[] a;
			if( node != null )
			{
				a = Convert.FromBase64String( node.InnerXml );
				_rsa.p = new BigInteger( a );
			}
			node = rootNode.SelectSingleNode( "RSAKeyValue/Q" );
			if( node != null )
			{
				a = Convert.FromBase64String( node.InnerXml );
				_rsa.q = new BigInteger( a );
			}
			node = rootNode.SelectSingleNode( "RSAKeyValue/D" );
			if( node != null )
			{
				a = Convert.FromBase64String( node.InnerXml );
				_rsa.d = new BigInteger( a );
			}
			node = rootNode.SelectSingleNode( "RSAKeyValue/Exponent" );
			if( node != null )
			{
				a = Convert.FromBase64String( node.InnerXml );
				_rsa.e = new BigInteger( a );
			}
			node = rootNode.SelectSingleNode( "RSAKeyValue/Modulus" );
			if( node != null )
			{
				a = Convert.FromBase64String( node.InnerXml );
				_rsa.n = new BigInteger( a );
			}

			if( Version >= v2_0 )
			{
				_snKeys = new KeyRecord[ 6 ];
				_snKeys[ 0 ]			= ReadSnKeyFromNode( rootNode.SelectSingleNode( "SNKeys/Key[ @size = '32' ]" ), 32 );
				_snKeys[ 1 ]			= ReadSnKeyFromNode( rootNode.SelectSingleNode( "SNKeys/Key[ @size = '48' ]" ), 48 );
				_snKeys[ 2 ]			= ReadSnKeyFromNode( rootNode.SelectSingleNode( "SNKeys/Key[ @size = '64' ]" ), 64 );
				_snKeys[ 3 ]			= ReadSnKeyFromNode( rootNode.SelectSingleNode( "SNKeys/Key[ @size = '160' ]" ), 160 );
			}
		}
		#region Helpers
		private KeyRecord ReadSnKeyFromNode( XmlNode node, int trybits )
		{
			BigInteger p = 0;
			BigInteger q = 0;
			BigInteger n = 0;
			BigInteger e = 0;
			BigInteger d = 0;

			int bits;

			if( node == null )
				return new KeyRecord( p, q, e, n, d, trybits );

			if( node.Attributes[ "size" ] == null )
				throw new ExtendedLicenseException( "E_InvalidSigningKeyFormat" );

			bits = XmlConvert.ToInt32( node.Attributes[ "size" ].Value );

			if( node.Attributes[ "e" ] == null )
				throw new ExtendedLicenseException( "E_InvalidSigningKeyFormat" );

			e = new BigInteger( Convert.FromBase64String( node.Attributes[ "e" ].Value.Trim() ) );

			if( node.Attributes[ "n" ] == null )
				throw new ExtendedLicenseException( "E_InvalidSigningKeyFormat" );

			n = new BigInteger( Convert.FromBase64String( node.Attributes[ "n" ].Value.Trim() ) );

			if( node.Attributes[ "p" ] != null && node.Attributes[ "q" ] != null && node.Attributes[ "d" ] != null )
			{
				p = new BigInteger( Convert.FromBase64String( node.Attributes[ "p" ].Value.Trim() ) );
				q = new BigInteger( Convert.FromBase64String( node.Attributes[ "q" ].Value.Trim() ) );
				d = new BigInteger( Convert.FromBase64String( node.Attributes[ "d" ].Value.Trim() ) );				 
			}

			return new KeyRecord( p, q, e, n, d, bits );
		}
		#endregion


		/// <summary>
		/// Signs the ExtendedLicense and sets the LicenseKey property
		/// </summary>
		/// <param name="license"></param>
		public void SignLicense( ExtendedLicense license )
		{
			if( ! _initialized )
				InitKey( true );

			AssertLicensed();
#if LICENSING
			if( _license == null || ((ExtendedLicense)_license).IsTrial )
			{
				if( license.Expires > DateTime.Today.AddDays( 3 ) )
					license.Expires = DateTime.Today.AddDays( 3 );

				if( license.MetaValues[ "Xheo.Licensing.Trial.Version" ] == null )
				{
					license.MetaValues[ "Xheo.Licensing.Trial.Version" ] = "This license was created with a trial version of XHEO|Licensing. It will automatically expire 3 days after it was issued. You must purchase the full version to use longer expiration dates.";
					license.MetaValues[ "Xheo.Licensing.Trial.Comments" ]	= "If you have already purchased a license, make sure your .LIC file is copied to the same folder as the Xheo.Licensing.dll. For WebServices and Web sites, this is the \\bin folder of the application. You can also provide your serial number by setting the Xheo.Licensing.SerialNumber key in the appSettings of your configuration file.";
				}
			}
			else
			{
				if( IsGlobalKey )
					license.MetaValues[ "Xheo.Licensing.Warning" ] = "You are still using keys generated during the trial version. You should regenerate your license signing keys and re-export the public key.";
			}

			if( _license != null && 
				((ExtendedLicense)_license).Version >= ExtendedLicense.v2_0 &&
				((ExtendedLicense)_license).Values[ XLPROKEY ] == "TRUE" )
			{
				license.Values[ XLPROCLIENTKEY ] = "TRUE";

			}
			else
			{
				license.Values.Remove( XLPROCLIENTKEY );
			}
#else
			license.Values[ XLPROCLIENTKEY ] = "TRUE";
#endif

			string licenseXml = GetInnerXmlAsString( license );
			license.Signature = Convert.ToBase64String( SignData( Encoding.UTF8.GetBytes( licenseXml ) ) );
		}

		/// <summary>
		/// Signs a response so that the originator of the message can be validated.
		/// </summary>
		/// <param name="response">
		///		The response to sign.
		/// </param>
		/// <returns>
		///		Returns a signature for the response.
		/// </returns>
		public string SignResponse( string response )
		{
			if( ! _initialized )
				InitKey( true );

			AssertLicensed();
			return Convert.ToBase64String( SignData( Encoding.UTF8.GetBytes( response ) ) );
		}

		/// <summary>
		/// Validates that the contents of the license has not been changed since it
		/// was signed. 
		/// </summary>
		/// <param name="license">
		///		The license to validate.
		/// </param>
		/// <returns>
		///		Returns true if the contents of the license have not been changed,
		///		otherwise false.
		/// </returns>
		/// <remarks>
		///		You must use the same LicenseSigningKey to validate the license as you
		///		did to sign it. You can use either the full public/private key or just
		///		the public key.
		/// </remarks>
		public bool ValidateLicense( ExtendedLicense license )
		{
			try
			{
				if( ! _initialized )
					InitKey( true );

				string licenseXml = GetInnerXmlAsString( license );
				byte[] data = Encoding.UTF8.GetBytes( licenseXml );
				byte[] signature = Convert.FromBase64String( license.Signature );
				
				return VerifyData( data, signature );
			}
			catch{}

			return false;
		}
		#region Helpers
		/// <summary>
		/// 
		/// </summary>
		/// <param name="data"></param>
		/// <param name="signature"></param>
		/// <returns></returns>
		public bool VerifyData( byte[] data, byte[] signature )
		{
			if( ! _initialized )
				InitKey( true );

			SHA1Managed sha		= new SHA1Managed();
			byte[] test			= sha.ComputeHash( data );
			byte[] test2		= new byte[ 127 ];
			byte[] shader		= new byte[] { 0x30, 0x21, 0x30, 0x09, 0x06, 0x05, 0x2b, 0x0e, 0x03, 0x02, 0x1a, 0x05, 0x00, 0x04, 0x14 };

			Array.Copy( test, 0, test2, test2.Length - test.Length, test.Length );
			Array.Copy( shader, 0, test2, test2.Length - test.Length - shader.Length, shader.Length );

			for( int index = 1; index < test2.Length - test.Length - shader.Length; index++ )
				test2[ index ] = 0xFF;

			test2[ 0 ] = 0x1;
			test2[ test2.Length - 1 - test.Length - shader.Length ] = 0x0;

			BigInteger v = new BigInteger( signature );
			BigInteger c = v.modPow( _rsa.e, _rsa.n );
			byte[] cipher = c.getBytes();

			for( int index = 0; index < cipher.Length; index++ )
			{
				if( cipher[ index ] != test2[ index ] )
					return false;
			}

			return true;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public byte[] SignData( byte[] data )
		{
			if( ! _initialized )
				InitKey( true );

			if( _rsa.d == null || _rsa.d == 0 )
				throw new ExtendedLicenseException( "E_MissingPrivateKey" );

			SHA1Managed sha		= new SHA1Managed();
			byte[] test			= sha.ComputeHash( data );
			byte[] test2		= new byte[ 127 ];
			byte[] shader		= new byte[] { 0x30, 0x21, 0x30, 0x09, 0x06, 0x05, 0x2b, 0x0e, 0x03, 0x02, 0x1a, 0x05, 0x00, 0x04, 0x14 };

			Array.Copy( test, 0, test2, test2.Length - test.Length, test.Length );
			Array.Copy( shader, 0, test2, test2.Length - test.Length - shader.Length, shader.Length );

			for( int index = 1; index < test2.Length - test.Length - shader.Length; index++ )
				test2[ index ] = 0xFF;

			test2[ 0 ] = 0x1;
			test2[ test2.Length - 1 - test.Length - shader.Length ] = 0x0;

			BigInteger v = new BigInteger( test2 );
			System.Diagnostics.Debug.Assert( v < _rsa.n );
			BigInteger c = v.modPow( _rsa.d, _rsa.n );

			return c.getBytes();
		}
		#endregion

		/// <summary>
		/// Validates a response signature.
		/// </summary>
		/// <param name="response">
		///		The original response.
		/// </param>
		/// <param name="signature">
		///		The signature generated from <see cref="SignResponse"/>.
		/// </param>
		/// <returns>
		///		Returns true if the signature and response match.
		/// </returns>
		public bool ValidateResponse( string response, string signature )
		{
			if( ! _initialized )
				InitKey( true );

			return VerifyData( Encoding.UTF8.GetBytes( response ), Convert.FromBase64String( signature ) );
		}

		/// <summary>
		/// Encrypts the data using the key. Use <see cref="Decrypt"/> to retrieve the 
		/// encrypted value.
		/// </summary>
		/// <param name="data">
		///		The data to encrypt.
		/// </param>
		/// <returns>
		///		Returns the encrypted version of the data.
		/// </returns>
		public byte[] Encrypt( byte[] data )
		{
			if( ! _initialized )
				InitKey( true );

			if( _rsa.d == null || _rsa.d == 0 )
				throw new ExtendedLicenseException( "E_MissingPrivateKey" );

			AssertLicensed();
			using( MemoryStream stream = new MemoryStream( data.Length * 2 ) )
			{

				int len = _rsa.d.bitCount() / 8;
				if( _rsa.d.bitCount()% 8 > 0 )
					len++;
				len -= 11;

				byte[] block = new byte[ len ];
				if( data.Length >= len )
				{
					int max = data.Length / len;
					max *= len;
					for( int index = 0; index < max; index += len )
					{
						Array.Copy( data, index, block, 0, len );
						byte[] cipher = EncryptBlock( block );
						stream.Write( cipher, 0, cipher.Length );
					}
				}

				int lastblock = data.Length % len;
				if( lastblock > 0 )
				{
					block = new byte[ lastblock ];
					Array.Copy( data, data.Length - lastblock, block, 0, lastblock );
					byte[] cipher = EncryptBlock( block );
					stream.Write( cipher, 0, cipher.Length );
				}

				stream.Flush();
				return stream.ToArray();
			}
		}
		#region Helpers
		private byte[] EncryptBlock( byte[] block )
		{
			int len = _rsa.d.bitCount() / 8;
			if( _rsa.d.bitCount() % 8 > 0 )
				len++;
			if( block.Length > len - 11 ) 
				throw new CryptographicException( "Encryption error." );
			byte[] ps = new byte[ len - block.Length - 3 ];
			_rng.GetNonZeroBytes( ps );
			System.Diagnostics.Debug.Assert( ps.Length >= 8 );
			byte[] ms = new byte[ len ];
			ms[ 0 ] = 0x00;
			ms[ 1 ] = 0x02;
			ms[ ms.Length - block.Length - 2 ] = 0x00;
			Array.Copy( ps, 0, ms, 2, ps.Length );
			Array.Copy( block, 0, ms, ms.Length - block.Length, block.Length );

			BigInteger m = new BigInteger( ms );
			BigInteger c = m.modPow( _rsa.d, _rsa.n );
			
			byte[] cipher = c.getBytes();
			if( cipher.Length == len )
				return cipher;

			byte[] cipher2 = new byte[ len ];
			Array.Copy( cipher, 0, cipher2, 1, cipher.Length );
			return cipher2;			
		}
		#endregion

		/// <summary>
		/// Encrypts the data using the key. Use <see cref="DecryptString"/> to retrieve the 
		/// encrypted value.
		/// </summary>
		/// <param name="data">
		///		The data to encrypt.
		/// </param>
		/// <returns>
		///		Returns the encrypted version of the data.
		/// </returns>
		public byte[] EncryptString( string data )
		{
			return Encrypt( System.Text.Encoding.UTF8.GetBytes( data ) );
		}

		/// <summary>
		/// Decrypts the data using the key. Use <see cref="Encrypt"/> to encrypt
		/// values.
		/// </summary>
		/// <param name="data">
		///		Data previously encrypted with <see cref="Encrypt"/>.
		/// </param>
		/// <returns>
		///		Returns the original value.
		/// </returns>
		public byte[] Decrypt( byte[] data )
		{
			if( ! _initialized )
				InitKey( true );

			using( MemoryStream stream = new MemoryStream() )
			{
				int bits = _rsa.n.bitCount();
				int len = bits / 8;
				if( bits % 8 > 0 )
					len++;
				byte[] block = new byte[ len ];

				System.Diagnostics.Debug.Assert( data.Length % len == 0 );

				for( int index = 0; index < data.Length; index += len )
				{
					Array.Copy( data, index, block, 0, len );
					byte[] message = DecryptBlock( block );
					stream.Write( message, 0, message.Length );
				}

				stream.Flush();
				return stream.ToArray();
			}
		}
		#region Helpers
		private byte[] DecryptBlock( byte[] block )
		{
			int len = _rsa.d.bitCount() / 8;
			if( _rsa.d.bitCount() % 8 > 0 )
				len++;
			if( block.Length != len )
				throw new CryptographicException( "Decryption error." );

			BigInteger c = new BigInteger( block );
			if( c > _rsa.n )
				throw new CryptographicException( "Decryption error." );
			
			BigInteger m = c.modPow( _rsa.e, _rsa.n );
			byte[] em = m.getBytes();
			
			if( em[ 0 ] != 0x2 )
				throw new CryptographicException( "Decryption error." );
			int index = -1;
			for( int ix = 1; ix < em.Length; ix++)
				if( em[ ix ] == 0 )
				{
					index = ix;
					break;
				}
			if( index == -1 )
				throw new CryptographicException( "Descryption error." );
			byte[] message = new byte[ em.Length - index - 1 ];
			Array.Copy( em, index + 1, message, 0, message.Length );
			return message;
		}
		#endregion

		/// <summary>
		/// Decrypts the data using the key. Use <see cref="EncryptString"/> to encrypt
		/// values.
		/// </summary>
		/// <param name="data">
		///		Data previously encrypted with <see cref="EncryptString"/>.
		/// </param>
		/// <returns>
		///		Returns the original value.
		/// </returns>
		public string DecryptString( byte[] data )
		{
			return System.Text.Encoding.UTF8.GetString( Decrypt( data ) );
		}

		/// <summary>
		/// Gets the inner XML of the license as a string for signing.
		/// </summary>
		/// <param name="license">
		///		The license.
		/// </param>
		/// <returns>
		///		Returns the inner XML of the license as a string.
		/// </returns>
		private string GetInnerXmlAsString( ExtendedLicense license )
		{
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.AppendChild( xmlDoc.CreateXmlDeclaration( "1.0", "UTF-8", null ) );
			xmlDoc.AppendChild( xmlDoc.CreateElement( "Dummy" ) );
			XmlNode licenseNode = license.SaveToXml( xmlDoc.DocumentElement );

			XmlNode node = licenseNode.SelectSingleNode( "//Meta" );
			while( node != null )
			{
				node.ParentNode.RemoveChild( node );
				node = licenseNode.SelectSingleNode( "//Meta" );
			}

			node = licenseNode.SelectSingleNode( "//SerialNumber" );
			if( node != null && node.Attributes[ "xml:space" ] != null )
				node.Attributes.Remove( node.Attributes[ "xml:space" ] );

			foreach( XmlNode limitNode in licenseNode.SelectNodes( "//Limit" ) )
			{
				string name	= null;
				Limit limit = null;
				
				if( limitNode.Attributes[ "netType" ] == null )
					name = limitNode.Attributes[ "type" ].Value;
				else
					name = limitNode.Attributes[ "netType" ].Value;

				limit = Limit.GetLimitFromName( name );
				limit.License = license;

				if( limit != null && limit.UnprotectedAttributes != null )
				{
					string[] stripped = limit.UnprotectedAttributes.Split( ',' );
					foreach( string strip in stripped )
					{
						if( limitNode.Attributes[ strip ] != null )
							limitNode.Attributes.RemoveNamedItem( strip );
					}
				}
			}

			return licenseNode.InnerXml;
		}
		

		/// <summary>
		/// Attempts to locate a signing key pair by name in the key folder specified
		/// in <see cref="ExtendedLicense.KeyFolder"/>.
		/// </summary>
		/// <param name="name">
		///		Name of the keys.
		/// </param>
		/// <param name="folder">
		///		Path to the folder where the keys are located.
		/// </param>
		/// <returns>
		///		Returns the signing key if found, otherwise null.
		/// </returns>
		public static LicenseSigningKey GetSigningKey( string name, string folder )
		{
			string location = null;
			
			if( ! Path.IsPathRooted( name ) )
				location = Path.GetFullPath( String.Format( System.Globalization.CultureInfo.InvariantCulture, "{0}\\{1}", folder == null ? ExtendedLicense.KeyFolder : folder, name ) );
			else
				location = name;
			
			if( ! File.Exists( location ) )
			{
				if( File.Exists( location + ".lsk" ) )
					location += ".lsk";
				else if ( File.Exists( location + ".plsk" ) )
					location += ".plsk";
				else
					return null;
			}

			return new LicenseSigningKey( location );
		}
		#region Overloads

		/// <summary>
		/// Attempts to locate a signing key pair by name in the key folder specified
		/// in <see cref="ExtendedLicense.KeyFolder"/>.
		/// </summary>
		/// <param name="name">
		///		Name of the keys.
		/// </param>
		/// <returns>
		///		Returns the signing key if found, otherwise null.
		/// </returns>
		public static LicenseSigningKey GetSigningKey( string name )
		{
			return GetSigningKey( name, null );
		}
		#endregion


		/// <summary>
		/// Gets the runtime.lic embedded in the assembly hosting the given type.
		/// </summary>
		/// <param name="type">
		///		The reference type to use for obtaining the assembly.
		/// </param>
		public static LicenseSigningKey GetPublicKey( Type type )
		{
			LicenseSigningKey key = null;

			key = _cachedKeys[ type.Assembly.FullName ] as LicenseSigningKey;
			if( key != null )
				return key;

			string publicKeyFile = type.Assembly.GetName().Name + ".plsk";
			string name = null;
			string[] parts = type.FullName.Split( '.' );

#if DEBUG
			string debug = "";
#endif


			for( int part = 0; part <= parts.Length; part++ )
			{
				if( name != null )
					name += ".";

				if( part > 0 )
					name += parts[ part - 1 ];

#if DEBUG
				debug += String.Format( System.Globalization.CultureInfo.InvariantCulture, "Looking for {0}\r\n", name );
				System.Diagnostics.Trace.WriteLine( name, "GET PUBLIC KEY" );
#endif

				Stream stream = type.Assembly.GetManifestResourceStream( name + ( ( name == null ) ? "" : "." ) + publicKeyFile );

				if( stream == null )
					continue;

				try
				{
#if DEBUG
					debug += "Found Stream\r\n";
					System.Diagnostics.Trace.WriteLine( "Found stream", "GET PUBLIC KEY" );
#endif
					key = new LicenseSigningKey( stream );
				}
#if DEBUG
				catch( Exception ex )
				{
					debug += "Could not load from stream.\r\n" + ex.ToString() + "\r\n";
					System.Diagnostics.Trace.WriteLine( "Could not load from stream", "GET PUBLIC KEY" );
					System.Diagnostics.Trace.WriteLine( ex.ToString(), "GET PUBLIC KEY" );
#else
				catch
				{
#endif
					continue;
				}
				stream.Close();
				break;
			}

			if( key == null )
#if DEBUG
				throw new LicenseException( type, null, debug, null );
#else
				throw new LicenseException( type, null, 
					Internal.StaticResourceProvider.CurrentProvider.FormatResourceString( "E_NoPublicKey",
					type.Assembly.GetName().Name,
					type.Namespace, 
					type.Assembly.GetName().Name ) );
#endif

			_cachedKeys[ type.Assembly.FullName ] = key;
			return key;

		}

		/// <summary>
		/// Upgrades the keys to the format used by the specified version. 
		/// </summary>
		/// <param name="version">
		///		The version to upgrade to.
		/// </param>
		/// <remarks>
		///		The changes should be saved over the existing keys and the public key re-exported
		///		after the upgrade.
		/// </remarks>
		public void UpgradeVersion( Version version )
		{
			if( version >= v2_0 )
			{
				if( Version < version )
				{
					Version = v2_0;
				}
			}
		}

		/// <summary>
		/// Makes an encoded serial number for the given license.
		/// </summary>
		/// <param name="license">
		///		The license to generate a serial number for.
		/// </param>
		/// <param name="size">
		///		The key size to use when encrypting the serial number.
		/// </param>
		/// <param name="seed">
		///		The serial number value to encode.
		/// </param>
		/// <param name="groupSizes">
		///		Defines the group sizes used to split the serial number into smaller chunks. A null
		///		value will use the default sizes for the given key size.
		/// </param>
		/// <returns>
		///		Returns a new serial number encrypted with the key.
		/// </returns>
		/// <exception cref="ExtendedLicenseException">
		///		Thrown when the license keys have not been upgraded to at least version 2.0.
		/// </exception>
		public string MakeSerialNumber( ExtendedLicense license, SerialNumberSizes size, int[] groupSizes, int seed )
		{
			return MakeSerialNumber( license.AbsoluteSerialNumber, size, groupSizes, seed );
		}
		#region Overloads
		/// <summary>
		/// Makes an encoded serial number for the given license.
		/// </summary>
		/// <param name="license">
		///		The license to generate a serial number for.
		/// </param>
		/// <param name="size">
		///		The key size to use when encrypting the serial number.
		/// </param>
		/// <param name="seed">
		///		The serial number value to encode.
		/// </param>
		/// <returns>
		///		Returns a new serial number encrypted with the key.
		/// </returns>
		/// <exception cref="ExtendedLicenseException">
		///		Thrown when the license keys have not been upgraded to at least version 2.0.
		/// </exception>
		public string MakeSerialNumber( ExtendedLicense license, SerialNumberSizes size, int seed )
		{
			return MakeSerialNumber( license, size, GetGroupSizes( size ), seed );
		}
		#region Helpers
		private int[] GetGroupSizes( SerialNumberSizes size )
		{
			int[] groupSizes;
			switch( size )
			{
				case SerialNumberSizes.Medium:
					groupSizes = new int[] { 5, 5, 5, 5 };
					break;
				case SerialNumberSizes.Large:
					groupSizes = new int[] { 5, 5, 5, 5, 5 };
					break;
				case SerialNumberSizes.Huge:
					groupSizes = new int[] { 8, 8, 8, 8, 8, 8, 8, 8, 8 };
					break;
				default:
					groupSizes = new int[ 0 ];
					break;
			}

			return groupSizes;
		}
		#endregion
		#endregion

		/// <summary>
		/// Makes an encoded serial number for the given license.
		/// </summary>
		/// <param name="size">
		///		The key size to use when encrypting the serial number.
		/// </param>
		/// <param name="seed">
		///		The serial number value to encode.
		/// </param>
		/// <param name="groupSizes">
		///		Defines the group sizes used to split the serial number into smaller chunks. A null
		///		value will use the default sizes for the given key size.
		/// </param>
		/// <param name="prefix">
		///		The prefix to add to the serial number. It must match the <see cref="ExtendedLicense.AbsoluteSerialNumber"/>
		///		of the license it will be assigned to.
		/// </param>
		/// <returns>
		///		Returns a new serial number encrypted with the key.
		/// </returns>
		public string MakeSerialNumber( string prefix, SerialNumberSizes size, int[] groupSizes, int seed )
		{
			AssertLicensed();
			if( Version < v2_0 )
				throw new ExtendedLicenseException( "E_KeyMustBeUpgraded", v2_0 );
//			if( seed > 0x3FFFFFFF )
//				throw new ArgumentOutOfRangeException( "seed", seed, null );

			KeyRecord key;
			KeyRecord[] snKeys = SnKeys;
			switch( size )
			{
				case SerialNumberSizes.Medium:
					key = snKeys[ 1 ];
					break;
				case SerialNumberSizes.Large:
					key = snKeys[ 2 ];
					break;
				case SerialNumberSizes.Huge:
					key = snKeys[ 3 ];
					break;
				default:
					return null;
			}

			if( key.p == 0 || key.q == 0 || key.d == 0 )
				throw new ExtendedLicenseException( "E_MissingPrivateKey" );

			uint uv			= unchecked( (uint)seed );
			ulong value		= (ulong)( uv );
			uint invvalue	= unchecked( (uint)( ~uv ) );
			if( prefix != null )
			{
				ulong salt	= (ulong)Math.Abs( Limit.GetCompatibleHashCode( prefix ) ) & 0x3FFFFFFF;
				value ^= salt;
			}
			
			BigInteger v	= new BigInteger( value << 32 | (ulong)invvalue );
			return MakeSerialNumberInternal( v, key, prefix, size, groupSizes );
		}
		#region Helpers
		static string MakeSerialNumberInternal( BigInteger seed, KeyRecord key, string prefix, SerialNumberSizes size, int[] groupSizes )
		{
			BigInteger c	= seed.modPow( key.d, key.n );
			StringBuilder sn = new StringBuilder();
			sn.Append( c.ToString( 35 ) );

			BigInteger v2 = c.modPow( key.e, key.n );
			if( seed != v2 )
				throw new ApplicationException( "Something is messed up." );

			switch( size )
			{
				case SerialNumberSizes.Medium:
					if( sn.Length < 20 )
						sn.Append( 'Z', 20 - sn.Length );
					break;
				case SerialNumberSizes.Large:
					if( sn.Length < 25 )
						sn.Append( 'Z', 25 - sn.Length );
					break;
				case SerialNumberSizes.Huge:
					if( sn.Length < 64 )
						sn.Append( 'Z', 64 - sn.Length );
					break;
			}

			string scrambled = Scramble( sn.ToString() );
			sn.Length = 0;

			if( groupSizes.Length > 0 )
			{
				int position = 0;
				for( int index = 0; index < groupSizes.Length && position < scrambled.Length; index++ )
				{
					int len = ( position + groupSizes[ index ] ) < scrambled.Length ? groupSizes[ index ] : scrambled.Length - position;
					if( sn.Length > 0 )
						sn.Append( '-' );
					sn.Append( scrambled.Substring( position, len ) );
					position += len;
				}

				if( position < scrambled.Length )
					sn.Append( scrambled.Substring( position ) );
			}
			else
			{
				sn.Append( scrambled );
			}

			sn.Insert( 0, prefix );
			
			return sn.ToString();
		}

		static internal string Scramble( string source )
		{
			char[] chars = source.ToCharArray();
			int h = source.Length >> 1;
			
			for( int l = source.Length; l > 0; l -= 2 )
				Array.Reverse( chars, ( source.Length - l ) / 2, l );
			Array.Reverse( chars, 0, h );

			return new string( chars );
		}
		static internal string Descramble( string source )
		{
			char[] chars = source.ToCharArray();
			int h = source.Length >> 1;
			
			Array.Reverse( chars, 0, h );
			for( int l = source.Length; l > 0; l -= 2 )
				Array.Reverse( chars, ( source.Length - l ) / 2, l );

			return new string( chars );
		}
		#endregion
		#region Overloads
		/// <summary>
		/// Makes an encoded serial number for the given license.
		/// </summary>
		/// <param name="size">
		///		The key size to use when encrypting the serial number.
		/// </param>
		/// <param name="seed">
		///		The serial number value to encode.
		/// </param>
		/// <param name="prefix">
		///		The prefix to add to the serial number. It must match the <see cref="ExtendedLicense.AbsoluteSerialNumber"/>
		///		of the license it will be assigned to.
		/// </param>
		/// <returns>
		///		Returns a new serial number encrypted with the key.
		/// </returns>
		public string MakeSerialNumber( string prefix, SerialNumberSizes size, int seed )
		{
			return MakeSerialNumber( prefix, size, GetGroupSizes( size ), seed );
		}
		#endregion

#if LICENSING || LICENSEACTIVATION
		/// <summary>
		/// Makes an activation key for the current machine and the given license that can
		/// be used to generate an unlock key with <see cref="MakeActivationUnlockKey"/>.
		/// </summary>
		/// <param name="license">
		///		The license to generate the key for.
		/// </param>
		/// <returns>
		///		Returns the activation key.
		/// </returns>
		static public string MakeActivationKey( ExtendedLicense license )
		{
			unchecked
			{
				string hash		= MachineProfile.Profile.GetComparableHash( license.Version );
				uint code		= (uint)( Limit.GetCompatibleHashCode( hash ) & 0x3FFFFFFF );
				Random rand = new Random();
				uint xor = 0; //(uint)rand.Next( 255 );

				code = 
					( ( code ^ ( xor << 0 ) ) & 0x000000FF ) | 
					( ( code ^ ( xor << 8 ) ) & 0x0000FF00 ) | 
					( ( code ^ ( xor << 16 ) ) & 0x00FF0000 ) | 
					( ( code ^ ( xor << 24 ) ) & 0xFF000000 );
				string number	= "0000000000" + code.ToString( System.Globalization.CultureInfo.InvariantCulture );
				number = number.Substring( number.Length - 10 );
				string c = "00000" + ( xor << 8 ).ToString( System.Globalization.CultureInfo.InvariantCulture );
				c = c.Substring( c.Length - 5 );
				number = number.Substring( 0, number.Length / 2 ) + '-' + number.Substring( number.Length / 2 ) + '-' + c;
				number = Scramble( number );
				return number;
			}
		}

		/// <summary>
		/// Makes an unlock key for use with activation.
		/// </summary>
		/// <param name="activationKey">
		///		The activation key created by <see cref="MakeActivationKey"/>
		/// </param>
		/// <returns>
		///		Returns a new unlock key.
		/// </returns>
		public string MakeActivationUnlockKey( string activationKey )
		{
			unchecked
			{
				activationKey = Descramble( activationKey );
				string c		= activationKey.Substring( 12 );
				string number	= activationKey.Substring( 0, 5 ) + activationKey.Substring( 6, 5 );

				uint xor			= Convert.ToUInt32( c, System.Globalization.CultureInfo.InvariantCulture ) >> 8;
				uint code			= Convert.ToUInt32( number, System.Globalization.CultureInfo.InvariantCulture );

				code = 
					( ( code ^ ( xor << 0 ) ) & 0x000000FF ) | 
					( ( code ^ ( xor << 8 ) ) & 0x0000FF00 ) | 
					( ( code ^ ( xor << 16 ) ) & 0x00FF0000 ) | 
					( ( code ^ ( xor << 24 ) ) & 0xFF000000 );			

				return MakeSerialNumber( (string)null, SerialNumberSizes.Huge, new int[] { 8, 8, 8, 8, 8, 8, 8, 8 }, (int)code );
			}
		}

		/// <summary>
		/// Validates that the activation unlock key is valid and matches the profile hash.
		/// </summary>
		/// <param name="profileHash">
		///		The profile hash of the computer being activated.
		/// </param>
		/// <param name="unlockKey">
		///		The unlock key generated from <see cref="MakeActivationUnlockKey"/>.
		/// </param>
		/// <returns>
		///		Returns true if the unlock key is valid, otherwise false.
		/// </returns>
		public bool ValidateActivationUnlockKey( string profileHash, string unlockKey )
		{
			return ValidateSerialNumber( null, unlockKey, Limit.GetCompatibleHashCode( profileHash ) & 0x3FFFFFFF, Limit.GetCompatibleHashCode( profileHash ) & 0x3FFFFFFF, SerialNumberSizes.Huge );
		}
#endif

		/// <summary>
		/// Validates that a serial number was signed with the license key and falls within
		/// a given range of valid numbers.
		/// </summary>
		/// <param name="license">
		///		The license the seral number is checked against.
		/// </param>
		/// <param name="serialNumber">
		///		The serial number to check.
		/// </param>
		/// <returns>
		///		Returns true of the serial number is valid, otherwise false.
		/// </returns>
		public bool ValidateSerialNumber( ExtendedLicense license, string serialNumber )
		{
			return ValidateSerialNumber( license.AbsoluteSerialNumber, serialNumber, license.MinimumSerialNumberSeed, license.MaximumSerialNumberSeed, license.AcceptedSerialNumberSizes );
		}

		/// <summary>
		/// Validates that a serial number was signed with the license key and falls within
		/// a given range of valid numbers.
		/// </summary>
		/// <param name="maxAcceptedSeed">
		///		The maximim acceptable seed value.
		/// </param>
		/// <param name="minAcceptedSeed">
		///		The minimum acceptable seed value.
		/// </param>
		/// <param name="prefix">
		///		The serial number prefix required.
		/// </param>
		/// <param name="serialNumber">
		///		The serial number to check.
		/// </param>
		/// <param name="acceptedSizes">
		///		The accepted serial number sizes.
		/// </param>
		/// <returns>
		///		Returns true of the serial number is valid, otherwise false.
		/// </returns>				
		public bool ValidateSerialNumber( string prefix, string serialNumber, int minAcceptedSeed, int maxAcceptedSeed, SerialNumberSizes acceptedSizes )
		{
			try
			{
				SerialNumberSizes size;
				int value = GetSerialNumberSeed( prefix, serialNumber, out size );
				if( value < minAcceptedSeed || value > maxAcceptedSeed )
					return false;
				if( ( size & acceptedSizes ) == size )
					return true;
			}
			catch( ExtendedLicenseException )
			{
			}

			return false;
		}
		

		/// <summary>
		/// Gets the original unmasked serial number seed (Int32 value) from the encoded value.
		/// </summary>
		/// <param name="license">
		///		The license the serial number was generated for.
		/// </param>
		/// <param name="serialNumber">
		///		The masked serial number created by <see cref="MakeSerialNumber(ExtendedLicense,SerialNumberSizes,int)"/>.
		/// </param>
		/// <returns>
		///		Returns the original Int32 seed value for the masked serial number.
		/// </returns>
		public int GetSerialNumberSeed( ExtendedLicense license, string serialNumber )
		{
			SerialNumberSizes size;
			return GetSerialNumberSeed( license.AbsoluteSerialNumber, serialNumber, out size );
		}
		#region Helpers
		private BigInteger GetSerialNumberSeedInternal( string serialNumber, KeyRecord key )
		{
			serialNumber = serialNumber.ToUpper( System.Globalization.CultureInfo.InvariantCulture );
			
			serialNumber = serialNumber.Replace( "-", "" );
			serialNumber = Descramble( serialNumber );
			serialNumber = serialNumber.Replace( "Z", "" );

			if( serialNumber.Length == 0 )
				throw new ExtendedLicenseException( "E_BadSerialNumber" );

			BigInteger m = new BigInteger( serialNumber, 35 );

			return m.modPow( key.e, key.n );
		}
		private int GetSerialNumberSeed( string prefix, string serialNumber, out SerialNumberSizes size )
		{
			KeyRecord key;
			serialNumber = serialNumber.ToUpper( System.Globalization.CultureInfo.InvariantCulture );
			if( prefix != null )
			{
				if( ! serialNumber.StartsWith( prefix ) || serialNumber == prefix )
					throw new ExtendedLicenseException( "E_BadSerialNumber" );
			
				serialNumber = serialNumber.Substring( prefix.Length );
			}
			serialNumber = serialNumber.Replace( "-", "" );

			size = SerialNumberSizes.Medium;

			if( serialNumber.Length <= 20 ) // 48
			{
				key = SnKeys[ 1 ];
				size = SerialNumberSizes.Medium;
			}
			else if( serialNumber.Length <= 25 ) // 64
			{
				key = SnKeys[ 2 ];
				size = SerialNumberSizes.Large;
			}
			else if( serialNumber.Length <= 64 ) // 160
			{
				key = SnKeys[ 3 ];
				size = SerialNumberSizes.Huge;
			}
			else
			{
				throw new ExtendedLicenseException( "E_InvalidSerialKeySize" );
			}

			if( key.e == 0 || key.n == 0 )
				throw new ExtendedLicenseException( "E_CannotValidateSerialNumbersOfLength", key.Bits );

			BigInteger v = GetSerialNumberSeedInternal( serialNumber, key );

			if( v > 0xFFFFFFFFFFFFFFFFLU )
				throw new ExtendedLicenseException( "E_BadSerialNumber" );

			ulong	value		= v.ULongValue();
			uint	invvalue	= ~((uint)( value & 0xFFFFFFFF ) );

			value >>= 32;
			if( prefix != null )
			{
				ulong salt	= (ulong)Math.Abs( Limit.GetCompatibleHashCode( prefix ) ) & 0x3FFFFFFF;
				value ^= salt;
			}

			if( value != invvalue )
				throw new ExtendedLicenseException( "E_BadSerialNumber" );

			return unchecked( (int)( value ) );
		}
		#endregion

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		#region KeyRecord
		private struct KeyRecord
		{
			public BigInteger	p;
			public BigInteger	q;
			public BigInteger	e;
			public BigInteger	d;
			public BigInteger	n;
			public int			Bits;

			public KeyRecord( int bits )
			{
				Bits = bits;
				p = q = e = d = n = null;
				do
				{
					try
					{
						e	= 65537; // pq.GenerateCoPrime( bits, rand );
						p	= BigInteger.genPseudoPrime( bits, 4, null );
						q	= BigInteger.genPseudoPrime( bits, 4, null );
						if( p < q )
						{
							BigInteger tmp = p;
							p = q;
							q = p;
						}
						n	= p * q;
						BigInteger	pq	= ( p - 1 ) * ( q - 1 );
						d	= e.modInverse( pq );
					}
					catch
					{
						continue;
					}
				}
				while( ! VerifyKey() );
			}

			public KeyRecord( BigInteger p, BigInteger q, BigInteger e, BigInteger n, BigInteger d, int bits )
			{
				this.p	= p;
				this.q	= q;
				this.e	= e;
				this.n	= n;
				this.d	= d;
				Bits = bits;
			}

			public bool VerifyKey()
			{
				if( n.bitCount() < Bits - 2 )
					return false;
				if( d.bitCount() < Bits - 2 )
					return false;

				BigInteger c;
				if( Bits > 32 )
				{
					BigInteger v	= new BigInteger( 0xFFFFFFFFFFFFFFFFL );
					c	= v.modPow( e, n );
					BigInteger v2	= c.modPow( d, n );
					if( v != v2 )
						return false;

					v	= new BigInteger( (ulong)0x7FFFFFFFFFFFFFFFL );
					c	= v.modPow( e, n );
					v2	= c.modPow( d, n );
					if( v != v2 )
						return false;

					int len = d.bitCount() / 8;
					if( d.bitCount() % 8 > 0 )
						len++;
					byte[] block = new byte[ len - 11 ];
					byte[] ps = new byte[ len - block.Length - 3 ];
					byte[] ms = new byte[ len ];
					ms[ 0 ] = 0x00;
					ms[ 1 ] = 0x02;
					ms[ ms.Length - block.Length - 2 ] = 0x00;
					RandomNumberGenerator rng = RandomNumberGenerator.Create();
					rng.GetNonZeroBytes( ps );
					Array.Copy( ps, 0, ms, 2, ps.Length );
					Array.Copy( block, 0, ms, ms.Length - block.Length, block.Length );

					BigInteger m = new BigInteger( ms );
					c = m.modPow( d, n );
					BigInteger m1 = c.modPow( e, n );

					byte[] ms2 = m1.getBytes();

					for( int index = 0; index < ms2.Length; index++ )
						if( ms2[ index ] != ms[ index + 1 ] )
							return false;
				}

				return true;
			}
		}

		#endregion

	} // End class LicenseSigningKey
	
	#region SerialNumberSizes
	/// <summary>
	/// Possible key sizes for use when generating serial numbers. More bits makes it
	/// more difficult to crack but also increase the size of the serial number.
	/// </summary>
	[ Flags ]
#if LICENSING
	public
#else
	internal
#endif
		enum SerialNumberSizes
	{
		/// <summary>
		/// Indicates that no serial number sizes are valid.
		/// </summary>
		None	= 0,

		/// <summary>
		/// Uses a key of 64 bits in length. Generates a serial number ~13 characters in length.
		/// Recommended group size = 4.
		/// </summary>
		[ Obsolete( "Small values cannot be deterministically supported and result in incosistent results.", true ) ]
		Small	= 1,

		/// <summary>
		/// Uses a key of 96 bits in length. Generates a serial number ~20 characters in length.
		/// Recommended group size = 5.
		/// </summary>
		Medium	= 2,

		/// <summary>
		/// Uses a key of 128 bits in length. Generates a serial number ~25 characters in length.
		/// Recommended group size = 5.
		/// </summary>
		Large	= 4,
		
		/// <summary>
		/// Uses a key of 320 bits in length. Generates a serial number ~64 characters in length.
		/// Recommended group size = 8.
		/// </summary>
		Huge	= 8,

		/// <summary>
		/// Allow any of the stored serial number sizes.
		/// </summary>
		All	= Medium | Large | Huge,

		/// <summary>
		/// Allow the medium and bigger sizes.
		/// </summary>
		MediumAndBigger		= Medium | Large | Huge,

		/// <summary>
		/// Allow the large and bigger sizes.
		/// </summary>
		LargeAndBigger		= Large | Huge,

		/// <summary>
		/// Recommended sizes for licenses.
		/// </summary>
		Recommended = Medium | Huge,

	} // End enum
	#endregion

	#region BigInteger

	#region Copyright (c) 2002 Chew Keong TAN
	//************************************************************************************
	// BigInteger Class Version 1.03
	//
	// Copyright (c) 2002 Chew Keong TAN
	// All rights reserved.
	//
	// Permission is hereby granted, free of charge, to any person obtaining a
	// copy of this software and associated documentation files (the
	// "Software"), to deal in the Software without restriction, including
	// without limitation the rights to use, copy, modify, merge, publish,
	// distribute, and/or sell copies of the Software, and to permit persons
	// to whom the Software is furnished to do so, provided that the above
	// copyright notice(s) and this permission notice appear in all copies of
	// the Software and that both the above copyright notice(s) and this
	// permission notice appear in supporting documentation.
	//
	// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
	// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
	// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT
	// OF THIRD PARTY RIGHTS. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR
	// HOLDERS INCLUDED IN THIS NOTICE BE LIABLE FOR ANY CLAIM, OR ANY SPECIAL
	// INDIRECT OR CONSEQUENTIAL DAMAGES, OR ANY DAMAGES WHATSOEVER RESULTING
	// FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN ACTION OF CONTRACT,
	// NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF OR IN CONNECTION
	// WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.
	//
	//
	// Disclaimer
	// ----------
	// Although reasonable care has been taken to ensure the correctness of this
	// implementation, this code should never be used in any application without
	// proper verification and testing.  I disclaim all liability and responsibility
	// to any person or entity with respect to any loss or damage caused, or alleged
	// to be caused, directly or indirectly, by the use of this BigInteger class.
	//
	// Comments, bugs and suggestions to
	// (http://www.codeproject.com/csharp/biginteger.asp)
	//
	//
	// Overloaded Operators +, -, *, /, %, >>, <<, ==, !=, >, <, >=, <=, &, |, ^, ++, --, ~
	//
	// Features
	// --------
	// 1) Arithmetic operations involving large signed integers (2's complement).
	// 2) Primality test using Fermat little theorm, Rabin Miller's method,
	//    Solovay Strassen's method and Lucas strong pseudoprime.
	// 3) Modulo exponential with Barrett's reduction.
	// 4) Inverse modulo.
	// 5) Pseudo prime generation.
	// 6) Co-prime generation.
	//
	//
	// Known Problem
	// -------------
	// This pseudoprime passes my implementation of
	// primality test but failed in JDK's IsProbablePrime test.
	//
	//       byte[] pseudoPrime1 = { (byte)0x00,
	//             (byte)0x85, (byte)0x84, (byte)0x64, (byte)0xFD, (byte)0x70, (byte)0x6A,
	//             (byte)0x9F, (byte)0xF0, (byte)0x94, (byte)0x0C, (byte)0x3E, (byte)0x2C,
	//             (byte)0x74, (byte)0x34, (byte)0x05, (byte)0xC9, (byte)0x55, (byte)0xB3,
	//             (byte)0x85, (byte)0x32, (byte)0x98, (byte)0x71, (byte)0xF9, (byte)0x41,
	//             (byte)0x21, (byte)0x5F, (byte)0x02, (byte)0x9E, (byte)0xEA, (byte)0x56,
	//             (byte)0x8D, (byte)0x8C, (byte)0x44, (byte)0xCC, (byte)0xEE, (byte)0xEE,
	//             (byte)0x3D, (byte)0x2C, (byte)0x9D, (byte)0x2C, (byte)0x12, (byte)0x41,
	//             (byte)0x1E, (byte)0xF1, (byte)0xC5, (byte)0x32, (byte)0xC3, (byte)0xAA,
	//             (byte)0x31, (byte)0x4A, (byte)0x52, (byte)0xD8, (byte)0xE8, (byte)0xAF,
	//             (byte)0x42, (byte)0xF4, (byte)0x72, (byte)0xA1, (byte)0x2A, (byte)0x0D,
	//             (byte)0x97, (byte)0xB1, (byte)0x31, (byte)0xB3,
	//       };
	//
	//
	// Change Log
	// ----------
	// 1) September 23, 2002 (Version 1.03)
	//    - Fixed operator- to give correct data length.
	//    - Added Lucas sequence generation.
	//    - Added Strong Lucas Primality test.
	//    - Added integer square root method.
	//    - Added setBit/unsetBit methods.
	//    - New IsProbablePrime() method which do not require the
	//      confident parameter.
	//
	// 2) August 29, 2002 (Version 1.02)
	//    - Fixed bug in the exponentiation of negative numbers.
	//    - Faster modular exponentiation using Barrett reduction.
	//    - Added getBytes() method.
	//    - Fixed bug in ToHexString method.
	//    - Added overloading of ^ operator.
	//    - Faster computation of Jacobi symbol.
	//
	// 3) August 19, 2002 (Version 1.01)
	//    - Big integer is stored and manipulated as unsigned integers (4 bytes) instead of
	//      individual bytes this gives significant performance improvement.
	//    - Updated Fermat's Little Theorem test to use a^(p-1) mod p = 1
	//    - Added IsProbablePrime method.
	//    - Updated documentation.
	//
	// 4) August 9, 2002 (Version 1.0)
	//    - Initial Release.
	//
	//
	// References
	// [1] D. E. Knuth, "Seminumerical Algorithms", The Art of Computer Programming Vol. 2,
	//     3rd Edition, Addison-Wesley, 1998.
	//
	// [2] K. H. Rosen, "Elementary Number Theory and Its Applications", 3rd Ed,
	//     Addison-Wesley, 1993.
	//
	// [3] B. Schneier, "Applied Cryptography", 2nd Ed, John Wiley & Sons, 1996.
	//
	// [4] A. Menezes, P. van Oorschot, and S. Vanstone, "Handbook of Applied Cryptography",
	//     CRC Press, 1996, www.cacr.math.uwaterloo.ca/hac
	//
	// [5] A. Bosselaers, R. Govaerts, and J. Vandewalle, "Comparison of Three Modular
	//     Reduction Functions," Proc. CRYPTO'93, pp.175-186.
	//
	// [6] R. Baillie and S. S. Wagstaff Jr, "Lucas Pseudoprimes", Mathematics of Computation,
	//     Vol. 35, No. 152, Oct 1980, pp. 1391-1417.
	//
	// [7] H. C. Williams, "Édouard Lucas and Primality Testing", Canadian Mathematical
	//     Society Series of Monographs and Advance Texts, vol. 22, John Wiley & Sons, New York,
	//     NY, 1998.
	//
	// [8] P. Ribenboim, "The new book of prime number records", 3rd edition, Springer-Verlag,
	//     New York, NY, 1995.
	//
	// [9] M. Joye and J.-J. Quisquater, "Efficient computation of full Lucas sequences",
	//     Electronics Letters, 32(6), 1996, pp 537-538.
	//
	//************************************************************************************
	#endregion

	
	/// <summary>
	/// Implements an large integer of arbitrary length.
	/// </summary>
	internal class BigInteger
	{
		// maximum length of the BigInteger in uint (4 bytes)
		// change this to suit the required level of precision.

		private const int maxLength = 70;

		// primes smaller than 2000 to test the generated prime number

		public static readonly int[] primesBelow2000 = {
														   2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67, 71, 73, 79, 83, 89, 97,
														   101, 103, 107, 109, 113, 127, 131, 137, 139, 149, 151, 157, 163, 167, 173, 179, 181, 191, 193, 197, 199,
														   211, 223, 227, 229, 233, 239, 241, 251, 257, 263, 269, 271, 277, 281, 283, 293,
														   307, 311, 313, 317, 331, 337, 347, 349, 353, 359, 367, 373, 379, 383, 389, 397,
														   401, 409, 419, 421, 431, 433, 439, 443, 449, 457, 461, 463, 467, 479, 487, 491, 499,
														   503, 509, 521, 523, 541, 547, 557, 563, 569, 571, 577, 587, 593, 599,
														   601, 607, 613, 617, 619, 631, 641, 643, 647, 653, 659, 661, 673, 677, 683, 691,
														   701, 709, 719, 727, 733, 739, 743, 751, 757, 761, 769, 773, 787, 797,
														   809, 811, 821, 823, 827, 829, 839, 853, 857, 859, 863, 877, 881, 883, 887,
														   907, 911, 919, 929, 937, 941, 947, 953, 967, 971, 977, 983, 991, 997,
														   1009, 1013, 1019, 1021, 1031, 1033, 1039, 1049, 1051, 1061, 1063, 1069, 1087, 1091, 1093, 1097,
														   1103, 1109, 1117, 1123, 1129, 1151, 1153, 1163, 1171, 1181, 1187, 1193,
														   1201, 1213, 1217, 1223, 1229, 1231, 1237, 1249, 1259, 1277, 1279, 1283, 1289, 1291, 1297,
														   1301, 1303, 1307, 1319, 1321, 1327, 1361, 1367, 1373, 1381, 1399,
														   1409, 1423, 1427, 1429, 1433, 1439, 1447, 1451, 1453, 1459, 1471, 1481, 1483, 1487, 1489, 1493, 1499,
														   1511, 1523, 1531, 1543, 1549, 1553, 1559, 1567, 1571, 1579, 1583, 1597,
														   1601, 1607, 1609, 1613, 1619, 1621, 1627, 1637, 1657, 1663, 1667, 1669, 1693, 1697, 1699,
														   1709, 1721, 1723, 1733, 1741, 1747, 1753, 1759, 1777, 1783, 1787, 1789,
														   1801, 1811, 1823, 1831, 1847, 1861, 1867, 1871, 1873, 1877, 1879, 1889,
														   1901, 1907, 1913, 1931, 1933, 1949, 1951, 1973, 1979, 1987, 1993, 1997, 1999 };


		private uint[] data = null;             // stores bytes from the Big Integer
		public int dataLength;                 // number of actual chars used


		//***********************************************************************
		// Constructor (Default value for BigInteger is 0
		//***********************************************************************

		public BigInteger()
		{
			data = new uint[maxLength];
			dataLength = 1;
		}


		//***********************************************************************
		// Constructor (Default value provided by long)
		//***********************************************************************

		public BigInteger(long value)
		{
			data = new uint[maxLength];
			long tempVal = value;

			// copy bytes from long to BigInteger without any assumption of
			// the length of the long datatype

			dataLength = 0;
			while(value != 0 && dataLength < maxLength)
			{
				data[dataLength] = (uint)(value & 0xFFFFFFFF);
				value >>= 32;
				dataLength++;
			}

			if(tempVal > 0)         // overflow check for +ve value
			{
				if(value != 0 || (data[maxLength-1] & 0x80000000) != 0)
					throw(new ArithmeticException("Positive overflow in constructor."));
			}
			else if(tempVal < 0)    // underflow check for -ve value
			{
				if(value != -1 || (data[dataLength-1] & 0x80000000) == 0)
					throw(new ArithmeticException("Negative underflow in constructor."));
			}

			if(dataLength == 0)
				dataLength = 1;
		}


		//***********************************************************************
		// Constructor (Default value provided by ulong)
		//***********************************************************************

		public BigInteger(ulong value)
		{
			data = new uint[maxLength];

			// copy bytes from ulong to BigInteger without any assumption of
			// the length of the ulong datatype

			dataLength = 0;
			while(value != 0 && dataLength < maxLength)
			{
				data[dataLength] = (uint)(value & 0xFFFFFFFF);
				value >>= 32;
				dataLength++;
			}

			if(value != 0 || (data[maxLength-1] & 0x80000000) != 0)
				throw(new ArithmeticException("Positive overflow in constructor."));

			if(dataLength == 0)
				dataLength = 1;
		}



		//***********************************************************************
		// Constructor (Default value provided by BigInteger)
		//***********************************************************************

		public BigInteger(BigInteger bi)
		{
			data = new uint[maxLength];

			dataLength = bi.dataLength;

			for(int i = 0; i < dataLength; i++)
				data[i] = bi.data[i];
		}


		//***********************************************************************
		// Constructor (Default value provided by a string of digits of the
		//              specified base)
		//
		// Example (base 10)
		// -----------------
		// To initialize "a" with the default value of 1234 in base 10
		//      BigInteger a = new BigInteger("1234", 10)
		//
		// To initialize "a" with the default value of -1234
		//      BigInteger a = new BigInteger("-1234", 10)
		//
		// Example (base 16)
		// -----------------
		// To initialize "a" with the default value of 0x1D4F in base 16
		//      BigInteger a = new BigInteger("1D4F", 16)
		//
		// To initialize "a" with the default value of -0x1D4F
		//      BigInteger a = new BigInteger("-1D4F", 16)
		//
		// Note that string values are specified in the <sign><magnitude>
		// format.
		//
		//***********************************************************************

		public BigInteger(string value, int radix)
		{
			BigInteger multiplier = new BigInteger(1);
			BigInteger result = new BigInteger();
			value = (value.ToUpper()).Trim();
			int limit = 0;

			if(value[0] == '-')
				limit = 1;

			for(int i = value.Length - 1; i >= limit ; i--)
			{
				int posVal = (int)value[i];

				if(posVal >= '0' && posVal <= '9')
					posVal -= '0';
				else if(posVal >= 'A' && posVal <= 'Z')
					posVal = (posVal - 'A') + 10;
				else
					posVal = 9999999;       // arbitrary large


				if(posVal >= radix)
					throw(new ArithmeticException("Invalid string in constructor."));
				else
				{
					if(value[0] == '-')
						posVal = -posVal;

					result = result + (multiplier * posVal);

					if((i - 1) >= limit)
						multiplier = multiplier * radix;
				}
			}

			if(value[0] == '-')     // negative values
			{
				if((result.data[maxLength-1] & 0x80000000) == 0)
					throw(new ArithmeticException("Negative underflow in constructor."));
			}
			else    // positive values
			{
				if((result.data[maxLength-1] & 0x80000000) != 0)
					throw(new ArithmeticException("Positive overflow in constructor."));
			}

			data = new uint[maxLength];
			for(int i = 0; i < result.dataLength; i++)
				data[i] = result.data[i];

			dataLength = result.dataLength;
		}


		//***********************************************************************
		// Constructor (Default value provided by an array of bytes)
		//
		// The lowest index of the input byte array (i.e [0]) should contain the
		// most significant byte of the number, and the highest index should
		// contain the least significant byte.
		//
		// E.g.
		// To initialize "a" with the default value of 0x1D4F in base 16
		//      byte[] temp = { 0x1D, 0x4F };
		//      BigInteger a = new BigInteger(temp)
		//
		// Note that this method of initialization does not allow the
		// sign to be specified.
		//
		//***********************************************************************

		public BigInteger(byte[] inData)
		{
			dataLength = inData.Length >> 2;

			int leftOver = inData.Length & 0x3;
			if(leftOver != 0)         // length not multiples of 4
				dataLength++;


			if(dataLength > maxLength)
				throw(new ArithmeticException("Byte overflow in constructor."));

			data = new uint[maxLength];

			for(int i = inData.Length - 1, j = 0; i >= 3; i -= 4, j++)
			{
				data[j] = (((uint)inData[i-3]) << 24) + ((uint)inData[i-2] << 16) +
					((uint)inData[i-1] << 8) + (uint)inData[i];
			}

			if(leftOver == 1)
				data[dataLength-1] = (uint)inData[0];
			else if(leftOver == 2)
				data[dataLength-1] = (uint)(((uint)inData[0] << 8) + inData[1]);
			else if(leftOver == 3)
				data[dataLength-1] = (uint)(((uint)inData[0] << 16) + ((uint)inData[1] << 8) + inData[2]);


			while(dataLength > 1 && data[dataLength-1] == 0)
				dataLength--;

			//// Console.WriteLine("Len = " + dataLength);
		}


		//***********************************************************************
		// Constructor (Default value provided by an array of bytes of the
		// specified length.)
		//***********************************************************************

		public BigInteger(byte[] inData, int inLen)
		{
			dataLength = inLen >> 2;

			int leftOver = inLen & 0x3;
			if(leftOver != 0)         // length not multiples of 4
				dataLength++;

			if(dataLength > maxLength || inLen > inData.Length)
				throw(new ArithmeticException("Byte overflow in constructor."));


			data = new uint[maxLength];

			for(int i = inLen - 1, j = 0; i >= 3; i -= 4, j++)
			{
				data[j] = (uint)((inData[i-3] << 24) + (inData[i-2] << 16) +
					(inData[i-1] <<  8) + inData[i]);
			}

			if(leftOver == 1)
				data[dataLength-1] = (uint)inData[0];
			else if(leftOver == 2)
				data[dataLength-1] = (uint)((inData[0] << 8) + inData[1]);
			else if(leftOver == 3)
				data[dataLength-1] = (uint)((inData[0] << 16) + (inData[1] << 8) + inData[2]);


			if(dataLength == 0)
				dataLength = 1;

			while(dataLength > 1 && data[dataLength-1] == 0)
				dataLength--;

			//// Console.WriteLine("Len = " + dataLength);
		}


		//***********************************************************************
		// Constructor (Default value provided by an array of unsigned integers)
		//*********************************************************************

		public BigInteger(uint[] inData)
		{
			dataLength = inData.Length;

			if(dataLength > maxLength)
				throw(new ArithmeticException("Byte overflow in constructor."));

			data = new uint[maxLength];

			for(int i = dataLength - 1, j = 0; i >= 0; i--, j++)
				data[j] = inData[i];

			while(dataLength > 1 && data[dataLength-1] == 0)
				dataLength--;

			//// Console.WriteLine("Len = " + dataLength);
		}


		//***********************************************************************
		// Overloading of the typecast operator.
		// For BigInteger bi = 10;
		//***********************************************************************

		public static implicit operator BigInteger(long value)
		{
			return (new BigInteger(value));
		}

		public static implicit operator BigInteger(ulong value)
		{
			return (new BigInteger(value));
		}

		public static implicit operator BigInteger(int value)
		{
			return (new BigInteger((long)value));
		}

		public static implicit operator BigInteger(uint value)
		{
			return (new BigInteger((ulong)value));
		}


		//***********************************************************************
		// Overloading of addition operator
		//***********************************************************************

		public static BigInteger operator +(BigInteger bi1, BigInteger bi2)
		{
			BigInteger result = new BigInteger();

			result.dataLength = (bi1.dataLength > bi2.dataLength) ? bi1.dataLength : bi2.dataLength;

			long carry = 0;
			for(int i = 0; i < result.dataLength; i++)
			{
				long sum = (long)bi1.data[i] + (long)bi2.data[i] + carry;
				carry  = sum >> 32;
				result.data[i] = (uint)(sum & 0xFFFFFFFF);
			}

			if(carry != 0 && result.dataLength < maxLength)
			{
				result.data[result.dataLength] = (uint)(carry);
				result.dataLength++;
			}

			while(result.dataLength > 1 && result.data[result.dataLength-1] == 0)
				result.dataLength--;


			// overflow check
			int lastPos = maxLength - 1;
			if((bi1.data[lastPos] & 0x80000000) == (bi2.data[lastPos] & 0x80000000) &&
				(result.data[lastPos] & 0x80000000) != (bi1.data[lastPos] & 0x80000000))
			{
				throw (new ArithmeticException());
			}

			return result;
		}


		//***********************************************************************
		// Overloading of the unary ++ operator
		//***********************************************************************

		public static BigInteger operator ++(BigInteger bi1)
		{
			BigInteger result = new BigInteger(bi1);

			long val, carry = 1;
			int index = 0;

			while(carry != 0 && index < maxLength)
			{
				val = (long)(result.data[index]);
				val++;

				result.data[index] = (uint)(val & 0xFFFFFFFF);
				carry = val >> 32;

				index++;
			}

			if(index > result.dataLength)
				result.dataLength = index;
			else
			{
				while(result.dataLength > 1 && result.data[result.dataLength-1] == 0)
					result.dataLength--;
			}

			// overflow check
			int lastPos = maxLength - 1;

			// overflow if initial value was +ve but ++ caused a sign
			// change to negative.

			if((bi1.data[lastPos] & 0x80000000) == 0 &&
				(result.data[lastPos] & 0x80000000) != (bi1.data[lastPos] & 0x80000000))
			{
				throw (new ArithmeticException("Overflow in ++."));
			}
			return result;
		}


		//***********************************************************************
		// Overloading of subtraction operator
		//***********************************************************************

		public static BigInteger operator -(BigInteger bi1, BigInteger bi2)
		{
			BigInteger result = new BigInteger();

			result.dataLength = (bi1.dataLength > bi2.dataLength) ? bi1.dataLength : bi2.dataLength;

			long carryIn = 0;
			for(int i = 0; i < result.dataLength; i++)
			{
				long diff;

				diff = (long)bi1.data[i] - (long)bi2.data[i] - carryIn;
				result.data[i] = (uint)(diff & 0xFFFFFFFF);

				if(diff < 0)
					carryIn = 1;
				else
					carryIn = 0;
			}

			// roll over to negative
			if(carryIn != 0)
			{
				for(int i = result.dataLength; i < maxLength; i++)
					result.data[i] = 0xFFFFFFFF;
				result.dataLength = maxLength;
			}

			// fixed in v1.03 to give correct datalength for a - (-b)
			while(result.dataLength > 1 && result.data[result.dataLength-1] == 0)
				result.dataLength--;

			// overflow check

			int lastPos = maxLength - 1;
			if((bi1.data[lastPos] & 0x80000000) != (bi2.data[lastPos] & 0x80000000) &&
				(result.data[lastPos] & 0x80000000) != (bi1.data[lastPos] & 0x80000000))
			{
				throw (new ArithmeticException());
			}

			return result;
		}


		//***********************************************************************
		// Overloading of the unary -- operator
		//***********************************************************************

		public static BigInteger operator --(BigInteger bi1)
		{
			BigInteger result = new BigInteger(bi1);

			long val;
			bool carryIn = true;
			int index = 0;

			while(carryIn && index < maxLength)
			{
				val = (long)(result.data[index]);
				val--;

				result.data[index] = (uint)(val & 0xFFFFFFFF);

				if(val >= 0)
					carryIn = false;

				index++;
			}

			if(index > result.dataLength)
				result.dataLength = index;

			while(result.dataLength > 1 && result.data[result.dataLength-1] == 0)
				result.dataLength--;

			// overflow check
			int lastPos = maxLength - 1;

			// overflow if initial value was -ve but -- caused a sign
			// change to positive.

			if((bi1.data[lastPos] & 0x80000000) != 0 &&
				(result.data[lastPos] & 0x80000000) != (bi1.data[lastPos] & 0x80000000))
			{
				throw (new ArithmeticException("Underflow in --."));
			}

			return result;
		}


		//***********************************************************************
		// Overloading of multiplication operator
		//***********************************************************************

		public static BigInteger operator *(BigInteger bi1, BigInteger bi2)
		{
			int lastPos = maxLength-1;
			bool bi1Neg = false, bi2Neg = false;

			// take the absolute value of the inputs
			if((bi1.data[lastPos] & 0x80000000) != 0)     // bi1 negative
			{
				bi1Neg = true; bi1 = -bi1;
			}
			if((bi2.data[lastPos] & 0x80000000) != 0)     // bi2 negative
			{
				bi2Neg = true; bi2 = -bi2;
			}

			BigInteger result = new BigInteger();

			// multiply the absolute values
			try
			{
				for( int i = 0; i < bi1.dataLength; i++ )
				{
					if( bi1.data[ i ] == 0 ) continue;

					ulong mcarry = 0;
					for( int j = 0, k = i; j < bi2.dataLength; j++, k++ )
					{
						// k = i + j
						ulong val = ( (ulong)bi1.data[ i ] * (ulong)bi2.data[ j ] ) +
							(ulong)result.data[ k ] + mcarry;

						result.data[ k ] = (uint)( val & 0xFFFFFFFF );
						mcarry = ( val >> 32 );
					}

					if( mcarry != 0 )
						result.data[ i + bi2.dataLength ] = (uint)mcarry;
				}
			}
			catch( ArithmeticException )
			{
				throw ( new ArithmeticException( "Multiplication overflow." ) );
			}
			catch( IndexOutOfRangeException )
			{
				throw ( new ArithmeticException( "Multiplication overflow." ) );
			}


			result.dataLength = bi1.dataLength + bi2.dataLength;
			if(result.dataLength > maxLength)
				result.dataLength = maxLength;

			while(result.dataLength > 1 && result.data[result.dataLength-1] == 0)
				result.dataLength--;

			// overflow check (result is -ve)
			if((result.data[lastPos] & 0x80000000) != 0)
			{
				if(bi1Neg != bi2Neg && result.data[lastPos] == 0x80000000)    // different sign
				{
					// handle the special case where multiplication produces
					// a max negative number in 2's complement.

					if(result.dataLength == 1)
						return result;
					else
					{
						bool isMaxNeg = true;
						for(int i = 0; i < result.dataLength - 1 && isMaxNeg; i++)
						{
							if(result.data[i] != 0)
								isMaxNeg = false;
						}

						if(isMaxNeg)
							return result;
					}
				}

				throw(new ArithmeticException("Multiplication overflow."));
			}

			// if input has different signs, then result is -ve
			if(bi1Neg != bi2Neg)
				return -result;

			return result;
		}



		//***********************************************************************
		// Overloading of unary << operators
		//***********************************************************************

		public static BigInteger operator <<(BigInteger bi1, int shiftVal)
		{
			BigInteger result = new BigInteger(bi1);
			result.dataLength = shiftLeft(result.data, shiftVal);

			return result;
		}


		// least significant bits at lower part of buffer

		private static int shiftLeft(uint[] buffer, int shiftVal)
		{
			int shiftAmount = 32;
			int bufLen = buffer.Length;

			while(bufLen > 1 && buffer[bufLen-1] == 0)
				bufLen--;

			for(int count = shiftVal; count > 0;)
			{
				if(count < shiftAmount)
					shiftAmount = count;

				//// Console.WriteLine("shiftAmount = {0}", shiftAmount);

				ulong carry = 0;
				for(int i = 0; i < bufLen; i++)
				{
					ulong val = ((ulong)buffer[i]) << shiftAmount;
					val |= carry;

					buffer[i] = (uint)(val & 0xFFFFFFFF);
					carry = val >> 32;
				}

				if(carry != 0)
				{
					if(bufLen + 1 <= buffer.Length)
					{
						buffer[bufLen] = (uint)carry;
						bufLen++;
					}
				}
				count -= shiftAmount;
			}
			return bufLen;
		}


		//***********************************************************************
		// Overloading of unary >> operators
		//***********************************************************************

		public static BigInteger operator >>(BigInteger bi1, int shiftVal)
		{
			BigInteger result = new BigInteger(bi1);
			result.dataLength = shiftRight(result.data, shiftVal);


			if((bi1.data[maxLength-1] & 0x80000000) != 0) // negative
			{
				for(int i = maxLength - 1; i >= result.dataLength; i--)
					result.data[i] = 0xFFFFFFFF;

				uint mask = 0x80000000;
				for(int i = 0; i < 32; i++)
				{
					if((result.data[result.dataLength-1] & mask) != 0)
						break;

					result.data[result.dataLength-1] |= mask;
					mask >>= 1;
				}
				result.dataLength = maxLength;
			}

			return result;
		}


		private static int shiftRight(uint[] buffer, int shiftVal)
		{
			int shiftAmount = 32;
			int invShift = 0;
			int bufLen = buffer.Length;

			while(bufLen > 1 && buffer[bufLen-1] == 0)
				bufLen--;

			//// Console.WriteLine("bufLen = " + bufLen + " buffer.Length = " + buffer.Length);

			for(int count = shiftVal; count > 0;)
			{
				if(count < shiftAmount)
				{
					shiftAmount = count;
					invShift = 32 - shiftAmount;
				}

				//// Console.WriteLine("shiftAmount = {0}", shiftAmount);

				ulong carry = 0;
				for(int i = bufLen - 1; i >= 0; i--)
				{
					ulong val = (ulong)( buffer[i] >> shiftAmount );
					val |= carry;

					carry = ( ((ulong)buffer[i]) << invShift ) & 0xFFFFFFFF ;
					buffer[i] = (uint)(val);
				}

				count -= shiftAmount;
			}

			while(bufLen > 1 && buffer[bufLen-1] == 0)
				bufLen--;

			return bufLen;
		}


		//***********************************************************************
		// Overloading of the NOT operator (1's complement)
		//***********************************************************************

		public static BigInteger operator ~(BigInteger bi1)
		{
			BigInteger result = new BigInteger(bi1);

			for(int i = 0; i < maxLength; i++)
				result.data[i] = (uint)(~(bi1.data[i]));

			result.dataLength = maxLength;

			while(result.dataLength > 1 && result.data[result.dataLength-1] == 0)
				result.dataLength--;

			return result;
		}


		//***********************************************************************
		// Overloading of the NEGATE operator (2's complement)
		//***********************************************************************

		public static BigInteger operator -(BigInteger bi1)
		{
			// handle neg of zero separately since it'll cause an overflow
			// if we proceed.

			if(bi1.dataLength == 1 && bi1.data[0] == 0)
				return (new BigInteger());

			BigInteger result = new BigInteger(bi1);

			// 1's complement
			for(int i = 0; i < maxLength; i++)
				result.data[i] = (uint)(~(bi1.data[i]));

			// add one to result of 1's complement
			long val, carry = 1;
			int index = 0;

			while(carry != 0 && index < maxLength)
			{
				val = (long)(result.data[index]);
				val++;

				result.data[index] = (uint)(val & 0xFFFFFFFF);
				carry = val >> 32;

				index++;
			}

			if((bi1.data[maxLength-1] & 0x80000000) == (result.data[maxLength-1] & 0x80000000))
				throw (new ArithmeticException("Overflow in negation.\n"));

			result.dataLength = maxLength;

			while(result.dataLength > 1 && result.data[result.dataLength-1] == 0)
				result.dataLength--;
			return result;
		}


		//***********************************************************************
		// Overloading of equality operator
		//***********************************************************************

		public static bool operator ==(BigInteger bi1, BigInteger bi2)
		{
			return bi1.Equals(bi2);
		}


		public static bool operator !=(BigInteger bi1, BigInteger bi2)
		{
			return !(bi1.Equals(bi2));
		}


		public override bool Equals(object o)
		{
			if( object.Equals( o, null ) )
				return false;

			BigInteger bi = (BigInteger)o;

			if(this.dataLength != bi.dataLength)
				return false;

			for(int i = 0; i < this.dataLength; i++)
			{
				if(this.data[i] != bi.data[i])
					return false;
			}
			return true;
		}


		public override int GetHashCode()
		{
			return Limit.GetCompatibleHashCode( this.ToString() );
		}


		//***********************************************************************
		// Overloading of inequality operator
		//***********************************************************************

		public static bool operator >(BigInteger bi1, BigInteger bi2)
		{
			int pos = maxLength - 1;

			// bi1 is negative, bi2 is positive
			if((bi1.data[pos] & 0x80000000) != 0 && (bi2.data[pos] & 0x80000000) == 0)
				return false;

				// bi1 is positive, bi2 is negative
			else if((bi1.data[pos] & 0x80000000) == 0 && (bi2.data[pos] & 0x80000000) != 0)
				return true;

			// same sign
			int len = (bi1.dataLength > bi2.dataLength) ? bi1.dataLength : bi2.dataLength;
			for(pos = len - 1; pos >= 0 && bi1.data[pos] == bi2.data[pos]; pos--);

			if(pos >= 0)
			{
				if(bi1.data[pos] > bi2.data[pos])
					return true;
				return false;
			}
			return false;
		}


		public static bool operator <(BigInteger bi1, BigInteger bi2)
		{
			int pos = maxLength - 1;

			// bi1 is negative, bi2 is positive
			if((bi1.data[pos] & 0x80000000) != 0 && (bi2.data[pos] & 0x80000000) == 0)
				return true;

				// bi1 is positive, bi2 is negative
			else if((bi1.data[pos] & 0x80000000) == 0 && (bi2.data[pos] & 0x80000000) != 0)
				return false;

			// same sign
			int len = (bi1.dataLength > bi2.dataLength) ? bi1.dataLength : bi2.dataLength;
			for(pos = len - 1; pos >= 0 && bi1.data[pos] == bi2.data[pos]; pos--);

			if(pos >= 0)
			{
				if(bi1.data[pos] < bi2.data[pos])
					return true;
				return false;
			}
			return false;
		}


		public static bool operator >=(BigInteger bi1, BigInteger bi2)
		{
			return (bi1 == bi2 || bi1 > bi2);
		}


		public static bool operator <=(BigInteger bi1, BigInteger bi2)
		{
			return (bi1 == bi2 || bi1 < bi2);
		}


		//***********************************************************************
		// Private function that supports the division of two numbers with
		// a divisor that has more than 1 digit.
		//
		// Algorithm taken from [1]
		//***********************************************************************

		private static void multiByteDivide(BigInteger bi1, BigInteger bi2,
			BigInteger outQuotient, BigInteger outRemainder)
		{
			uint[] result = new uint[maxLength];

			int remainderLen = bi1.dataLength + 1;
			uint[] remainder = new uint[remainderLen];

			uint mask = 0x80000000;
			uint val = bi2.data[bi2.dataLength - 1];
			int shift = 0, resultPos = 0;

			while(mask != 0 && (val & mask) == 0)
			{
				shift++; mask >>= 1;
			}

			//// Console.WriteLine("shift = {0}", shift);
			//// Console.WriteLine("Before bi1 Len = {0}, bi2 Len = {1}", bi1.dataLength, bi2.dataLength);

			for(int i = 0; i < bi1.dataLength; i++)
				remainder[i] = bi1.data[i];
			shiftLeft(remainder, shift);
			bi2 = bi2 << shift;

			/*
					// Console.WriteLine("bi1 Len = {0}, bi2 Len = {1}", bi1.dataLength, bi2.dataLength);
					// Console.WriteLine("dividend = " + bi1 + "\ndivisor = " + bi2);
					for(int q = remainderLen - 1; q >= 0; q--)
							// Console.Write("{0:x2}", remainder[q]);
					// Console.WriteLine();
					*/

			int j = remainderLen - bi2.dataLength;
			int pos = remainderLen - 1;

			ulong firstDivisorByte = bi2.data[bi2.dataLength-1];
			ulong secondDivisorByte = bi2.data[bi2.dataLength-2];

			int divisorLen = bi2.dataLength + 1;
			uint[] dividendPart = new uint[divisorLen];

			while(j > 0)
			{
				ulong dividend = ((ulong)remainder[pos] << 32) + (ulong)remainder[pos-1];
				//// Console.WriteLine("dividend = {0}", dividend);

				ulong q_hat = dividend / firstDivisorByte;
				ulong r_hat = dividend % firstDivisorByte;

				//// Console.WriteLine("q_hat = {0:X}, r_hat = {1:X}", q_hat, r_hat);

				bool done = false;
				while(!done)
				{
					done = true;

					if(q_hat == 0x100000000 ||
						(q_hat * secondDivisorByte) > ((r_hat << 32) + remainder[pos-2]))
					{
						q_hat--;
						r_hat += firstDivisorByte;

						if(r_hat < 0x100000000)
							done = false;
					}
				}

				for(int h = 0; h < divisorLen; h++)
					dividendPart[h] = remainder[pos-h];

				BigInteger kk = new BigInteger(dividendPart);
				BigInteger ss = bi2 * (long)q_hat;

				//// Console.WriteLine("ss before = " + ss);
				while(ss > kk)
				{
					q_hat--;
					ss -= bi2;
					//// Console.WriteLine(ss);
				}
				BigInteger yy = kk - ss;

				//// Console.WriteLine("ss = " + ss);
				//// Console.WriteLine("kk = " + kk);
				//// Console.WriteLine("yy = " + yy);

				for(int h = 0; h < divisorLen; h++)
					remainder[pos-h] = yy.data[bi2.dataLength-h];

				/*
							// Console.WriteLine("dividend = ");
							for(int q = remainderLen - 1; q >= 0; q--)
									// Console.Write("{0:x2}", remainder[q]);
							// Console.WriteLine("\n************ q_hat = {0:X}\n", q_hat);
							*/

				result[resultPos++] = (uint)q_hat;

				pos--;
				j--;
			}

			outQuotient.dataLength = resultPos;
			int y = 0;
			for(int x = outQuotient.dataLength - 1; x >= 0; x--, y++)
				outQuotient.data[y] = result[x];
			for(; y < maxLength; y++)
				outQuotient.data[y] = 0;

			while(outQuotient.dataLength > 1 && outQuotient.data[outQuotient.dataLength-1] == 0)
				outQuotient.dataLength--;

			if(outQuotient.dataLength == 0)
				outQuotient.dataLength = 1;

			outRemainder.dataLength = shiftRight(remainder, shift);

			for(y = 0; y < outRemainder.dataLength; y++)
				outRemainder.data[y] = remainder[y];
			for(; y < maxLength; y++)
				outRemainder.data[y] = 0;
		}


		//***********************************************************************
		// Private function that supports the division of two numbers with
		// a divisor that has only 1 digit.
		//***********************************************************************

		private static void singleByteDivide(BigInteger bi1, BigInteger bi2,
			BigInteger outQuotient, BigInteger outRemainder)
		{
			uint[] result = new uint[maxLength];
			int resultPos = 0;

			// copy dividend to reminder
			for(int i = 0; i < maxLength; i++)
				outRemainder.data[i] = bi1.data[i];
			outRemainder.dataLength = bi1.dataLength;

			while(outRemainder.dataLength > 1 && outRemainder.data[outRemainder.dataLength-1] == 0)
				outRemainder.dataLength--;

			ulong divisor = (ulong)bi2.data[0];
			int pos = outRemainder.dataLength - 1;
			ulong dividend = (ulong)outRemainder.data[pos];

			//// Console.WriteLine("divisor = " + divisor + " dividend = " + dividend);
			//// Console.WriteLine("divisor = " + bi2 + "\ndividend = " + bi1);

			if(dividend >= divisor)
			{
				ulong quotient = dividend / divisor;
				result[resultPos++] = (uint)quotient;

				outRemainder.data[pos] = (uint)(dividend % divisor);
			}
			pos--;

			while(pos >= 0)
			{
				//// Console.WriteLine(pos);

				dividend = ((ulong)outRemainder.data[pos+1] << 32) + (ulong)outRemainder.data[pos];
				ulong quotient = dividend / divisor;
				result[resultPos++] = (uint)quotient;

				outRemainder.data[pos+1] = 0;
				outRemainder.data[pos--] = (uint)(dividend % divisor);
				//// Console.WriteLine(">>>> " + bi1);
			}

			outQuotient.dataLength = resultPos;
			int j = 0;
			for(int i = outQuotient.dataLength - 1; i >= 0; i--, j++)
				outQuotient.data[j] = result[i];
			for(; j < maxLength; j++)
				outQuotient.data[j] = 0;

			while(outQuotient.dataLength > 1 && outQuotient.data[outQuotient.dataLength-1] == 0)
				outQuotient.dataLength--;

			if(outQuotient.dataLength == 0)
				outQuotient.dataLength = 1;

			while(outRemainder.dataLength > 1 && outRemainder.data[outRemainder.dataLength-1] == 0)
				outRemainder.dataLength--;
		}


		//***********************************************************************
		// Overloading of division operator
		//***********************************************************************

		public static BigInteger operator /(BigInteger bi1, BigInteger bi2)
		{
			BigInteger quotient = new BigInteger();
			BigInteger remainder = new BigInteger();

			int lastPos = maxLength-1;
			bool divisorNeg = false, dividendNeg = false;

			if((bi1.data[lastPos] & 0x80000000) != 0)     // bi1 negative
			{
				bi1 = -bi1;
				dividendNeg = true;
			}
			if((bi2.data[lastPos] & 0x80000000) != 0)     // bi2 negative
			{
				bi2 = -bi2;
				divisorNeg = true;
			}

			if(bi1 < bi2)
			{
				return quotient;
			}

			else
			{
				if(bi2.dataLength == 1)
					singleByteDivide(bi1, bi2, quotient, remainder);
				else
					multiByteDivide(bi1, bi2, quotient, remainder);

				if(dividendNeg != divisorNeg)
					return -quotient;

				return quotient;
			}
		}


		//***********************************************************************
		// Overloading of modulus operator
		//***********************************************************************

		public static BigInteger operator %(BigInteger bi1, BigInteger bi2)
		{
			BigInteger quotient = new BigInteger();
			BigInteger remainder = new BigInteger(bi1);

			int lastPos = maxLength-1;
			bool dividendNeg = false;

			if((bi1.data[lastPos] & 0x80000000) != 0)     // bi1 negative
			{
				bi1 = -bi1;
				dividendNeg = true;
			}
			if((bi2.data[lastPos] & 0x80000000) != 0)     // bi2 negative
				bi2 = -bi2;

			if(bi1 < bi2)
			{
				return remainder;
			}

			else
			{
				if(bi2.dataLength == 1)
					singleByteDivide(bi1, bi2, quotient, remainder);
				else
					multiByteDivide(bi1, bi2, quotient, remainder);

				if(dividendNeg)
					return -remainder;

				return remainder;
			}
		}


		//***********************************************************************
		// Overloading of bitwise AND operator
		//***********************************************************************

		public static BigInteger operator &(BigInteger bi1, BigInteger bi2)
		{
			BigInteger result = new BigInteger();

			int len = (bi1.dataLength > bi2.dataLength) ? bi1.dataLength : bi2.dataLength;

			for(int i = 0; i < len; i++)
			{
				uint sum = (uint)(bi1.data[i] & bi2.data[i]);
				result.data[i] = sum;
			}

			result.dataLength = maxLength;

			while(result.dataLength > 1 && result.data[result.dataLength-1] == 0)
				result.dataLength--;

			return result;
		}


		//***********************************************************************
		// Overloading of bitwise OR operator
		//***********************************************************************

		public static BigInteger operator |(BigInteger bi1, BigInteger bi2)
		{
			BigInteger result = new BigInteger();

			int len = (bi1.dataLength > bi2.dataLength) ? bi1.dataLength : bi2.dataLength;

			for(int i = 0; i < len; i++)
			{
				uint sum = (uint)(bi1.data[i] | bi2.data[i]);
				result.data[i] = sum;
			}

			result.dataLength = maxLength;

			while(result.dataLength > 1 && result.data[result.dataLength-1] == 0)
				result.dataLength--;

			return result;
		}


		//***********************************************************************
		// Overloading of bitwise XOR operator
		//***********************************************************************

		public static BigInteger operator ^(BigInteger bi1, BigInteger bi2)
		{
			BigInteger result = new BigInteger();

			int len = (bi1.dataLength > bi2.dataLength) ? bi1.dataLength : bi2.dataLength;

			for(int i = 0; i < len; i++)
			{
				uint sum = (uint)(bi1.data[i] ^ bi2.data[i]);
				result.data[i] = sum;
			}

			result.dataLength = maxLength;

			while(result.dataLength > 1 && result.data[result.dataLength-1] == 0)
				result.dataLength--;

			return result;
		}




		//***********************************************************************
		// Returns min(this, bi)
		//***********************************************************************

		public BigInteger min(BigInteger bi)
		{
			if(this < bi)
				return (new BigInteger(this));
			else
				return (new BigInteger(bi));

		}


		//***********************************************************************
		// Returns the absolute value
		//***********************************************************************

		public BigInteger abs()
		{
			if((this.data[maxLength - 1] & 0x80000000) != 0)
				return (-this);
			else
				return (new BigInteger(this));
		}


		//***********************************************************************
		// Returns a string representing the BigInteger in base 10.
		//***********************************************************************

		public override string ToString()
		{
			return ToString(10);
		}


		//***********************************************************************
		// Returns a string representing the BigInteger in sign-and-magnitude
		// format in the specified radix.
		//
		// Example
		// -------
		// If the value of BigInteger is -255 in base 10, then
		// ToString(16) returns "-FF"
		//
		//***********************************************************************

		public string ToString(int radix)
		{
			if(radix < 2 || radix > 36)
				throw (new ArgumentException("Radix must be >= 2 and <= 36"));

			string charSet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
			string result = "";

			BigInteger a = this;

			bool negative = false;
			if((a.data[maxLength-1] & 0x80000000) != 0)
			{
				negative = true;
				a = -a;
			}

			BigInteger quotient = new BigInteger();
			BigInteger remainder = new BigInteger();
			BigInteger biRadix = new BigInteger(radix);

			if(a.dataLength == 1 && a.data[0] == 0)
				result = "0";
			else
			{
				while(a.dataLength > 1 || (a.dataLength == 1 && a.data[0] != 0))
				{
					singleByteDivide(a, biRadix, quotient, remainder);

					if(remainder.data[0] < 10)
						result = remainder.data[0] + result;
					else
						result = charSet[(int)remainder.data[0] - 10] + result;

					a = quotient;
				}
				if(negative)
					result = "-" + result;
			}

			return result;
		}


		//***********************************************************************
		// Returns a hex string showing the contains of the BigInteger
		//
		// Examples
		// -------
		// 1) If the value of BigInteger is 255 in base 10, then
		//    ToHexString() returns "FF"
		//
		// 2) If the value of BigInteger is -255 in base 10, then
		//    ToHexString() returns ".....FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF01",
		//    which is the 2's complement representation of -255.
		//
		//***********************************************************************

		public string ToHexString()
		{
			string result = data[dataLength - 1].ToString( "X", System.Globalization.CultureInfo.InvariantCulture );

			for(int i = dataLength - 2; i >= 0; i--)
			{
				result += data[ i ].ToString( "X8", System.Globalization.CultureInfo.InvariantCulture );
			}

			return result;
		}



		//***********************************************************************
		// Modulo Exponentiation
		//***********************************************************************

		public BigInteger modPow(BigInteger exp, BigInteger n)
		{
//			BigInteger result = 1;
//			BigInteger e = new BigInteger( exp );
//			BigInteger p = this;
//			while( e > 0 )
//			{
//				if( ( e.data[ 0 ] & 1 ) > 0 )
//					result = ( result * p ) % n;
//				e >>= 1;
//				if( e > 0 )
//					p = ( p * p ) % n;
//			}
//			
//			return result;


			if((exp.data[maxLength-1] & 0x80000000) != 0)
				throw (new ArithmeticException("Positive exponents only."));

			BigInteger resultNum = 1;
			BigInteger tempNum;
			bool thisNegative = false;

			if((this.data[maxLength-1] & 0x80000000) != 0)   // negative this
			{
				tempNum = -this % n;
				thisNegative = true;
			}
			else
				tempNum = this % n;  // ensures (tempNum * tempNum) < b^(2k)

			if((n.data[maxLength-1] & 0x80000000) != 0)   // negative n
				n = -n;

			// calculate constant = b^(2k) / m
			BigInteger constant = new BigInteger();

			int i = n.dataLength << 1;
			constant.data[i] = 0x00000001;
			constant.dataLength = i + 1;

			constant = constant / n;
			int totalBits = exp.bitCount();
			int count = 0;

			// perform squaring and multiply exponentiation
			for(int pos = 0; pos < exp.dataLength; pos++)
			{
				uint mask = 0x01;
				//// Console.WriteLine("pos = " + pos);

				for(int index = 0; index < 32; index++)
				{
					if((exp.data[pos] & mask) != 0)
						resultNum = BarrettReduction(resultNum * tempNum, n, constant);

					mask <<= 1;

					tempNum = BarrettReduction(tempNum * tempNum, n, constant);


					if(tempNum.dataLength == 1 && tempNum.data[0] == 1)
					{
						if(thisNegative && (exp.data[0] & 0x1) != 0)    //odd exp
							return -resultNum;
						return resultNum;
					}
					count++;
					if(count == totalBits)
						break;
				}
			}

			if(thisNegative && (exp.data[0] & 0x1) != 0)    //odd exp
				return -resultNum;

			return resultNum;
		}



		//***********************************************************************
		// Fast calculation of modular reduction using Barrett's reduction.
		// Requires x < b^(2k), where b is the base.  In this case, base is
		// 2^32 (uint).
		//
		// Reference [4]
		//***********************************************************************

		private BigInteger BarrettReduction(BigInteger x, BigInteger n, BigInteger constant)
		{
			int k = n.dataLength,
				kPlusOne = k+1,
				kMinusOne = k-1;

			BigInteger q1 = new BigInteger();

			// q1 = x / b^(k-1)
			for(int i = kMinusOne, j = 0; i < x.dataLength; i++, j++)
				q1.data[j] = x.data[i];
			q1.dataLength = x.dataLength - kMinusOne;
			if(q1.dataLength <= 0)
				q1.dataLength = 1;


			BigInteger q2 = q1 * constant;
			BigInteger q3 = new BigInteger();

			// q3 = q2 / b^(k+1)
			for(int i = kPlusOne, j = 0; i < q2.dataLength; i++, j++)
				q3.data[j] = q2.data[i];
			q3.dataLength = q2.dataLength - kPlusOne;
			if(q3.dataLength <= 0)
				q3.dataLength = 1;


			// r1 = x mod b^(k+1)
			// i.e. keep the lowest (k+1) words
			BigInteger r1 = new BigInteger();
			int lengthToCopy = (x.dataLength > kPlusOne) ? kPlusOne : x.dataLength;
			for(int i = 0; i < lengthToCopy; i++)
				r1.data[i] = x.data[i];
			r1.dataLength = lengthToCopy;


			// r2 = (q3 * n) mod b^(k+1)
			// partial multiplication of q3 and n

			BigInteger r2 = new BigInteger();
			for(int i = 0; i < q3.dataLength; i++)
			{
				if(q3.data[i] == 0)     continue;

				ulong mcarry = 0;
				int t = i;
				for(int j = 0; j < n.dataLength && t < kPlusOne; j++, t++)
				{
					// t = i + j
					ulong val = ((ulong)q3.data[i] * (ulong)n.data[j]) +
						(ulong)r2.data[t] + mcarry;

					r2.data[t] = (uint)(val & 0xFFFFFFFF);
					mcarry = (val >> 32);
				}

				if(t < kPlusOne)
					r2.data[t] = (uint)mcarry;
			}
			r2.dataLength = kPlusOne;
			while(r2.dataLength > 1 && r2.data[r2.dataLength-1] == 0)
				r2.dataLength--;

			r1 -= r2;
			if((r1.data[maxLength-1] & 0x80000000) != 0)        // negative
			{
				BigInteger val = new BigInteger();
				val.data[kPlusOne] = 0x00000001;
				val.dataLength = kPlusOne + 1;
				r1 += val;
			}

			while(r1 >= n)
				r1 -= n;

			return r1;
		}


		//***********************************************************************
		// Returns gcd(this, bi)
		//***********************************************************************

		public BigInteger gcd(BigInteger bi)
		{
			BigInteger x;
			BigInteger y;

			if((data[maxLength-1] & 0x80000000) != 0)     // negative
				x = -this;
			else
				x = this;

			if((bi.data[maxLength-1] & 0x80000000) != 0)     // negative
				y = -bi;
			else
				y = bi;

			BigInteger g = y;

			while(x.dataLength > 1 || (x.dataLength == 1 && x.data[0] != 0))
			{
				g = x;
				x = y % x;
				y = g;
			}

			return g;
		}


		//***********************************************************************
		// Populates "this" with the specified amount of random bits
		//***********************************************************************

		public void genRandomBits(int bits, RandomNumberGenerator rand)
		{
			int dwords = bits >> 5;
			int remBits = bits & 0x1F;

			if(remBits != 0)
				dwords++;

			if(dwords > maxLength)
				throw (new ArithmeticException("Number of required bits > maxLength."));

			byte[] bytes = new byte[ dwords * 4 ];
			rand.GetNonZeroBytes( bytes );
			for( int index = 0; index < dwords; index++ )
				data[ index ] = BitConverter.ToUInt32( bytes, index * 4 );

			for(int i = dwords; i < maxLength; i++)
				data[i] = 0;

			if(remBits != 0)
			{
				uint mask = (uint)(0x01 << (remBits-1));
				data[dwords-1] |= mask;

				mask = (uint)(0xFFFFFFFF >> (32 - remBits));
				data[dwords-1] &= mask;
			}
			else
				data[dwords-1] |= 0x80000000;

			dataLength = dwords;

			if(dataLength == 0)
				dataLength = 1;
		}


		//***********************************************************************
		// Returns the position of the most significant bit in the BigInteger.
		//
		// Eg.  The result is 0, if the value of BigInteger is 0...0000 0000
		//      The result is 1, if the value of BigInteger is 0...0000 0001
		//      The result is 2, if the value of BigInteger is 0...0000 0010
		//      The result is 2, if the value of BigInteger is 0...0000 0011
		//
		//***********************************************************************

		public int bitCount()
		{
			while(dataLength > 1 && data[dataLength-1] == 0)
				dataLength--;

			uint value = data[dataLength - 1];
			uint mask = 0x80000000;
			int bits = 32;

			while(bits > 0 && (value & mask) == 0)
			{
				bits--;
				mask >>= 1;
			}
			bits += ((dataLength - 1) << 5);

			return bits;
		}


		//***********************************************************************
		// Probabilistic prime test based on Fermat's little theorem
		//
		// for any a < p (p does not divide a) if
		//      a^(p-1) mod p != 1 then p is not prime.
		//
		// Otherwise, p is probably prime (pseudoprime to the chosen base).
		//
		// Returns
		// -------
		// True if "this" is a pseudoprime to randomly chosen
		// bases.  The number of chosen bases is given by the "confidence"
		// parameter.
		//
		// False if "this" is definitely NOT prime.
		//
		// Note - this method is fast but fails for Carmichael numbers except
		// when the randomly chosen base is a factor of the number.
		//
		//***********************************************************************

		public bool FermatLittleTest(int confidence)
		{
			BigInteger thisVal;
			if((this.data[maxLength-1] & 0x80000000) != 0)        // negative
				thisVal = -this;
			else
				thisVal = this;

			if(thisVal.dataLength == 1)
			{
				// test small numbers
				if(thisVal.data[0] == 0 || thisVal.data[0] == 1)
					return false;
				else if(thisVal.data[0] == 2 || thisVal.data[0] == 3)
					return true;
			}

			if((thisVal.data[0] & 0x1) == 0)     // even numbers
				return false;

			int bits = thisVal.bitCount();
			BigInteger a = new BigInteger();
			BigInteger p_sub1 = thisVal - (new BigInteger(1));
			RandomNumberGenerator rand = RandomNumberGenerator.Create();

			for(int round = 0; round < confidence; round++)
			{
				bool done = false;

				while(!done)		// generate a < n
				{
					int testBits = 0;

					// make sure "a" has at least 2 bits
					while(testBits < 2)
					{
						byte[] tests = new byte[ 1 ];
						rand.GetNonZeroBytes( tests );
						testBits = tests[ 0 ];
					}

					a.genRandomBits(testBits, rand);

					int byteLen = a.dataLength;

					// make sure "a" is not 0
					if(byteLen > 1 || (byteLen == 1 && a.data[0] != 1))
						done = true;
				}

				// check whether a factor exists (fix for version 1.03)
				BigInteger gcdTest = a.gcd(thisVal);
				if(gcdTest.dataLength == 1 && gcdTest.data[0] != 1)
					return false;

				// calculate a^(p-1) mod p
				BigInteger expResult = a.modPow(p_sub1, thisVal);

				int resultLen = expResult.dataLength;

				// is NOT prime is a^(p-1) mod p != 1

				if(resultLen > 1 || (resultLen == 1 && expResult.data[0] != 1))
				{
					//// Console.WriteLine("a = " + a.ToString());
					return false;
				}
			}

			return true;
		}


		//***********************************************************************
		// Probabilistic prime test based on Rabin-Miller's
		//
		// for any p > 0 with p - 1 = 2^s * t
		//
		// p is probably prime (strong pseudoprime) if for any a < p,
		// 1) a^t mod p = 1 or
		// 2) a^((2^j)*t) mod p = p-1 for some 0 <= j <= s-1
		//
		// Otherwise, p is composite.
		//
		// Returns
		// -------
		// True if "this" is a strong pseudoprime to randomly chosen
		// bases.  The number of chosen bases is given by the "confidence"
		// parameter.
		//
		// False if "this" is definitely NOT prime.
		//
		//***********************************************************************

		public bool RabinMillerTest(int confidence)
		{
			BigInteger thisVal;
			if((this.data[maxLength-1] & 0x80000000) != 0)        // negative
				thisVal = -this;
			else
				thisVal = this;

			if(thisVal.dataLength == 1)
			{
				// test small numbers
				if(thisVal.data[0] == 0 || thisVal.data[0] == 1)
					return false;
				else if(thisVal.data[0] == 2 || thisVal.data[0] == 3)
					return true;
			}

			if((thisVal.data[0] & 0x1) == 0)     // even numbers
				return false;


			// calculate values of s and t
			BigInteger p_sub1 = thisVal - (new BigInteger(1));
			int s = 0;

			for(int index = 0; index < p_sub1.dataLength; index++)
			{
				uint mask = 0x01;

				for(int i = 0; i < 32; i++)
				{
					if((p_sub1.data[index] & mask) != 0)
					{
						index = p_sub1.dataLength;      // to break the outer loop
						break;
					}
					mask <<= 1;
					s++;
				}
			}

			BigInteger t = p_sub1 >> s;

			int bits = thisVal.bitCount();
			BigInteger a = new BigInteger();
			RandomNumberGenerator rand = RandomNumberGenerator.Create();

			for(int round = 0; round < confidence; round++)
			{
				bool done = false;

				while(!done)		// generate a < n
				{
					int testBits = 0;

					// make sure "a" has at least 2 bits
					while(testBits < 2)
					{
						byte[] tests = new byte[ 1 ];
						rand.GetNonZeroBytes( tests );
						testBits = tests[ 0 ];
					}

					a.genRandomBits(testBits, rand);

					int byteLen = a.dataLength;

					// make sure "a" is not 0
					if(byteLen > 1 || (byteLen == 1 && a.data[0] != 1))
						done = true;
				}

				// check whether a factor exists (fix for version 1.03)
				BigInteger gcdTest = a.gcd(thisVal);
				if(gcdTest.dataLength == 1 && gcdTest.data[0] != 1)
					return false;

				BigInteger b = a.modPow(t, thisVal);

				/*
							// Console.WriteLine("a = " + a.ToString(10));
							// Console.WriteLine("b = " + b.ToString(10));
							// Console.WriteLine("t = " + t.ToString(10));
							// Console.WriteLine("s = " + s);
							*/

				bool result = false;

				if(b.dataLength == 1 && b.data[0] == 1)         // a^t mod p = 1
					result = true;

				for(int j = 0; result == false && j < s; j++)
				{
					if(b == p_sub1)         // a^((2^j)*t) mod p = p-1 for some 0 <= j <= s-1
					{
						result = true;
						break;
					}

					b = (b * b) % thisVal;
				}

				if(result == false)
					return false;
			}
			return true;
		}


		//***********************************************************************
		// Implementation of the Lucas Strong Pseudo Prime test.
		//
		// Let n be an odd number with gcd(n,D) = 1, and n - J(D, n) = 2^s * d
		// with d odd and s >= 0.
		//
		// If Ud mod n = 0 or V2^r*d mod n = 0 for some 0 <= r < s, then n
		// is a strong Lucas pseudoprime with parameters (P, Q).  We select
		// P and Q based on Selfridge.
		//
		// Returns True if number is a strong Lucus pseudo prime.
		// Otherwise, returns False indicating that number is composite.
		//***********************************************************************

		public bool LucasStrongTest()
		{
			BigInteger thisVal;
			if((this.data[maxLength-1] & 0x80000000) != 0)        // negative
				thisVal = -this;
			else
				thisVal = this;

			if(thisVal.dataLength == 1)
			{
				// test small numbers
				if(thisVal.data[0] == 0 || thisVal.data[0] == 1)
					return false;
				else if(thisVal.data[0] == 2 || thisVal.data[0] == 3)
					return true;
			}

			if((thisVal.data[0] & 0x1) == 0)     // even numbers
				return false;

			return LucasStrongTestHelper(thisVal);
		}


		private bool LucasStrongTestHelper(BigInteger thisVal)
		{
			// Do the test (selects D based on Selfridge)
			// Let D be the first element of the sequence
			// 5, -7, 9, -11, 13, ... for which J(D,n) = -1
			// Let P = 1, Q = (1-D) / 4

			long D = 5, sign = -1, dCount = 0;
			bool done = false;

			while(!done)
			{
				int Jresult = BigInteger.Jacobi(D, thisVal);

				if(Jresult == -1)
					done = true;    // J(D, this) = 1
				else
				{
					if(Jresult == 0 && Math.Abs(D) < thisVal)       // divisor found
						return false;

					if(dCount == 20)
					{
						// check for square
						BigInteger root = thisVal.sqrt();
						if(root * root == thisVal)
							return false;
					}

					//// Console.WriteLine(D);
					D = (Math.Abs(D) + 2) * sign;
					sign = -sign;
				}
				dCount++;
			}

			long Q = (1 - D) >> 2;

			/*
					// Console.WriteLine("D = " + D);
					// Console.WriteLine("Q = " + Q);
					// Console.WriteLine("(n,D) = " + thisVal.gcd(D));
					// Console.WriteLine("(n,Q) = " + thisVal.gcd(Q));
					// Console.WriteLine("J(D|n) = " + BigInteger.Jacobi(D, thisVal));
					*/

			BigInteger p_add1 = thisVal + 1;
			int s = 0;

			for(int index = 0; index < p_add1.dataLength; index++)
			{
				uint mask = 0x01;

				for(int i = 0; i < 32; i++)
				{
					if((p_add1.data[index] & mask) != 0)
					{
						index = p_add1.dataLength;      // to break the outer loop
						break;
					}
					mask <<= 1;
					s++;
				}
			}

			BigInteger t = p_add1 >> s;

			// calculate constant = b^(2k) / m
			// for Barrett Reduction
			BigInteger constant = new BigInteger();

			int nLen = thisVal.dataLength << 1;
			constant.data[nLen] = 0x00000001;
			constant.dataLength = nLen + 1;

			constant = constant / thisVal;

			BigInteger[] lucas = LucasSequenceHelper(1, Q, t, thisVal, constant, 0);
			bool isPrime = false;

			if((lucas[0].dataLength == 1 && lucas[0].data[0] == 0) ||
				(lucas[1].dataLength == 1 && lucas[1].data[0] == 0))
			{
				// u(t) = 0 or V(t) = 0
				isPrime = true;
			}

			for(int i = 1; i < s; i++)
			{
				if(!isPrime)
				{
					// doubling of index
					lucas[1] = thisVal.BarrettReduction(lucas[1] * lucas[1], thisVal, constant);
					lucas[1] = (lucas[1] - (lucas[2] << 1)) % thisVal;

					//lucas[1] = ((lucas[1] * lucas[1]) - (lucas[2] << 1)) % thisVal;

					if((lucas[1].dataLength == 1 && lucas[1].data[0] == 0))
						isPrime = true;
				}

				lucas[2] = thisVal.BarrettReduction(lucas[2] * lucas[2], thisVal, constant);     //Q^k
			}


			if(isPrime)     // additional checks for composite numbers
			{
				// If n is prime and gcd(n, Q) == 1, then
				// Q^((n+1)/2) = Q * Q^((n-1)/2) is congruent to (Q * J(Q, n)) mod n

				BigInteger g = thisVal.gcd(Q);
				if(g.dataLength == 1 && g.data[0] == 1)         // gcd(this, Q) == 1
				{
					if((lucas[2].data[maxLength-1] & 0x80000000) != 0)
						lucas[2] += thisVal;

					BigInteger temp = (Q * BigInteger.Jacobi(Q, thisVal)) % thisVal;
					if((temp.data[maxLength-1] & 0x80000000) != 0)
						temp += thisVal;

					if(lucas[2] != temp)
						isPrime = false;
				}
			}

			return isPrime;
		}


		//***********************************************************************
		// Determines whether a number is probably prime, using the Rabin-Miller's
		// test.  Before applying the test, the number is tested for divisibility
		// by primes < 2000
		//
		// Returns true if number is probably prime.
		//***********************************************************************

		public bool isProbablePrime(int confidence)
		{
			BigInteger thisVal;
			if((this.data[maxLength-1] & 0x80000000) != 0)        // negative
				thisVal = -this;
			else
				thisVal = this;


			// test for divisibility by primes < 2000
			for(int p = 0; p < primesBelow2000.Length; p++)
			{
				BigInteger divisor = primesBelow2000[p];

				if(divisor >= thisVal)
					break;

				BigInteger resultNum = thisVal % divisor;
				if(resultNum.IntValue() == 0)
				{
					/*
					// Console.WriteLine("Not prime!  Divisible by {0}\n",
													  primesBelow2000[p]);
									*/
					return false;
				}
			}

			if(thisVal.RabinMillerTest(confidence))
				return true;
			else
			{
				//// Console.WriteLine("Not prime!  Failed primality test\n");
				return false;
			}
		}


		//***********************************************************************
		// Determines whether this BigInteger is probably prime using a
		// combination of base 2 strong pseudoprime test and Lucas strong
		// pseudoprime test.
		//
		// The sequence of the primality test is as follows,
		//
		// 1) Trial divisions are carried out using prime numbers below 2000.
		//    if any of the primes divides this BigInteger, then it is not prime.
		//
		// 2) Perform base 2 strong pseudoprime test.  If this BigInteger is a
		//    base 2 strong pseudoprime, proceed on to the next step.
		//
		// 3) Perform strong Lucas pseudoprime test.
		//
		// Returns True if this BigInteger is both a base 2 strong pseudoprime
		// and a strong Lucas pseudoprime.
		//
		// For a detailed discussion of this primality test, see [6].
		//
		//***********************************************************************

		public bool isProbablePrime()
		{
			BigInteger thisVal;
			if((this.data[maxLength-1] & 0x80000000) != 0)        // negative
				thisVal = -this;
			else
				thisVal = this;

			if(thisVal.dataLength == 1)
			{
				// test small numbers
				if(thisVal.data[0] == 0 || thisVal.data[0] == 1)
					return false;
				else if(thisVal.data[0] == 2 || thisVal.data[0] == 3)
					return true;
			}

			if((thisVal.data[0] & 0x1) == 0)     // even numbers
				return false;


			// test for divisibility by primes < 2000
			for(int p = 0; p < primesBelow2000.Length; p++)
			{
				BigInteger divisor = primesBelow2000[p];

				if(divisor >= thisVal)
					break;

				BigInteger resultNum = thisVal % divisor;
				if(resultNum.IntValue() == 0)
				{
					//// Console.WriteLine("Not prime!  Divisible by {0}\n",
					//                  primesBelow2000[p]);

					return false;
				}
			}

			// Perform BASE 2 Rabin-Miller Test

			// calculate values of s and t
			BigInteger p_sub1 = thisVal - (new BigInteger(1));
			int s = 0;

			for(int index = 0; index < p_sub1.dataLength; index++)
			{
				uint mask = 0x01;

				for(int i = 0; i < 32; i++)
				{
					if((p_sub1.data[index] & mask) != 0)
					{
						index = p_sub1.dataLength;      // to break the outer loop
						break;
					}
					mask <<= 1;
					s++;
				}
			}

			BigInteger t = p_sub1 >> s;

			int bits = thisVal.bitCount();
			BigInteger a = 2;

			// b = a^t mod p
			BigInteger b = a.modPow(t, thisVal);
			bool result = false;

			if(b.dataLength == 1 && b.data[0] == 1)         // a^t mod p = 1
				result = true;

			for(int j = 0; result == false && j < s; j++)
			{
				if(b == p_sub1)         // a^((2^j)*t) mod p = p-1 for some 0 <= j <= s-1
				{
					result = true;
					break;
				}

				b = (b * b) % thisVal;
			}

			// if number is strong pseudoprime to base 2, then do a strong lucas test
			if(result)
				result = LucasStrongTestHelper(thisVal);

			return result;
		}



		//***********************************************************************
		// Returns the lowest 4 bytes of the BigInteger as an int.
		//***********************************************************************

		public int IntValue()
		{
			return (int)data[0];
		}


		//***********************************************************************
		// Returns the lowest 8 bytes of the BigInteger as a long.
		//***********************************************************************

		public long LongValue()
		{
			long val = 0;

			val = (long)data[0];
			if( maxLength > 0 )
				val |= (long)data[1] << 32;

			return val;
		}


		//***********************************************************************
		// Computes the Jacobi Symbol for a and b.
		// Algorithm adapted from [3] and [4] with some optimizations
		//***********************************************************************

		public static int Jacobi(BigInteger a, BigInteger b)
		{
			// Jacobi defined only for odd integers
			if((b.data[0] & 0x1) == 0)
				throw (new ArgumentException("Jacobi defined only for odd integers."));

			if(a >= b)      a %= b;
			if(a.dataLength == 1 && a.data[0] == 0)      return 0;  // a == 0
			if(a.dataLength == 1 && a.data[0] == 1)      return 1;  // a == 1

			if(a < 0)
			{
				if( (((b-1).data[0]) & 0x2) == 0)       //if( (((b-1) >> 1).data[0] & 0x1) == 0)
					return Jacobi(-a, b);
				else
					return -Jacobi(-a, b);
			}

			int e = 0;
			for(int index = 0; index < a.dataLength; index++)
			{
				uint mask = 0x01;

				for(int i = 0; i < 32; i++)
				{
					if((a.data[index] & mask) != 0)
					{
						index = a.dataLength;      // to break the outer loop
						break;
					}
					mask <<= 1;
					e++;
				}
			}

			BigInteger a1 = a >> e;

			int s = 1;
			if((e & 0x1) != 0 && ((b.data[0] & 0x7) == 3 || (b.data[0] & 0x7) == 5))
				s = -1;

			if((b.data[0] & 0x3) == 3 && (a1.data[0] & 0x3) == 3)
				s = -s;

			if(a1.dataLength == 1 && a1.data[0] == 1)
				return s;
			else
				return (s * Jacobi(b % a1, a1));
		}



		//***********************************************************************
		// Generates a positive BigInteger that is probably prime.
		//***********************************************************************

		public static BigInteger genPseudoPrime(int bits, int confidence, RandomNumberGenerator rand)
		{
			BigInteger result = new BigInteger();
			bool done = false;

			if( rand == null )
				rand = RandomNumberGenerator.Create();

			while(!done)
			{
				result.genRandomBits(bits, rand);
				result.data[0] |= 0x01;		// make it odd

				// prime test
				done = result.isProbablePrime(confidence);
			}
			return result;
		}

		//***********************************************************************
		// Returns the modulo inverse of this.  Throws ArithmeticException if
		// the inverse does not exist.  (i.e. gcd(this, modulus) != 1)
		//***********************************************************************

		public BigInteger modInverse(BigInteger modulus)
		{
			BigInteger[] p = { 0, 1 };
			BigInteger[] q = new BigInteger[2];    // quotients
			BigInteger[] r = { 0, 0 };             // remainders

			int step = 0;

			BigInteger a = modulus;
			BigInteger b = this;

			while(b.dataLength > 1 || (b.dataLength == 1 && b.data[0] != 0))
			{
				BigInteger quotient = new BigInteger();
				BigInteger remainder = new BigInteger();

				if(step > 1)
				{
					BigInteger pval = (p[0] - (p[1] * q[0])) % modulus;
					p[0] = p[1];
					p[1] = pval;
				}

				if(b.dataLength == 1)
					singleByteDivide(a, b, quotient, remainder);
				else
					multiByteDivide(a, b, quotient, remainder);

				/*
							// Console.WriteLine(quotient.dataLength);
							// Console.WriteLine("{0} = {1}({2}) + {3}  p = {4}", a.ToString(10),
											  b.ToString(10), quotient.ToString(10), remainder.ToString(10),
											  p[1].ToString(10));
							*/

				q[0] = q[1];
				r[0] = r[1];
				q[1] = quotient; r[1] = remainder;

				a = b;
				b = remainder;

				step++;
			}

			if(r[0].dataLength > 1 || (r[0].dataLength == 1 && r[0].data[0] != 1))
				throw (new ArithmeticException("No inverse!"));

			BigInteger result = ((p[0] - (p[1] * q[0])) % modulus);

			if((result.data[maxLength - 1] & 0x80000000) != 0)
				result += modulus;  // get the least positive modulus

			return result;
		}


		//***********************************************************************
		// Returns the value of the BigInteger as a byte array.  The lowest
		// index contains the MSB.
		//***********************************************************************

		public byte[] getBytes()
		{
			int numBits = bitCount();

			int numBytes = numBits >> 3;
			if((numBits & 0x7) != 0)
				numBytes++;

			byte[] result = new byte[numBytes];

			//// Console.WriteLine(result.Length);

			int pos = 0;
			uint tempVal, val = data[dataLength - 1];

//			Original
//			if((tempVal = (val >> 24 & 0xFF)) != 0)
//				result[pos++] = (byte)tempVal;
//			if((tempVal = (val >> 16 & 0xFF)) != 0)
//				result[pos++] = (byte)tempVal;
//			if((tempVal = (val >> 8 & 0xFF)) != 0)
//				result[pos++] = (byte)tempVal;
//			if((tempVal = (val & 0xFF)) != 0)
//				result[pos++] = (byte)tempVal;

			if((tempVal = (val >> 24 & 0xFF)) != 0)
				result[pos++] = (byte)tempVal;
			if( ! ( ((tempVal = (val >> 16 & 0xFF)) == 0) && pos==0 ) ) 
				result[pos++] = (byte)tempVal;
			if( ! ( ((tempVal = (val >> 8 & 0xFF)) == 0) && pos==0 ) )
				result[pos++] = (byte)tempVal;
			if( ! ( ((tempVal = (val & 0xFF)) == 0) && pos==0 ) )
				result[pos++] = (byte)tempVal;

			for(int i = dataLength - 2; i >= 0; i--, pos += 4)
			{
				val = data[i];
				result[pos+3] = (byte)(val & 0xFF);
				val >>= 8;
				result[pos+2] = (byte)(val & 0xFF);
				val >>= 8;
				result[pos+1] = (byte)(val & 0xFF);
				val >>= 8;
				result[pos] = (byte)(val & 0xFF);
			}

			return result;
		}


		//***********************************************************************
		// Sets the value of the specified bit to 1
		// The Least Significant Bit position is 0.
		//***********************************************************************

		public void setBit(uint bitNum)
		{
			uint bytePos = bitNum >> 5;             // divide by 32
			byte bitPos = (byte)(bitNum & 0x1F);    // get the lowest 5 bits

			uint mask = (uint)1 << bitPos;
			this.data[bytePos] |= mask;

			if(bytePos >= this.dataLength)
				this.dataLength = (int)bytePos + 1;
		}

		public bool getBit( uint bitNum )
		{
			uint bytePos = bitNum >> 5;             // divide by 32
			byte bitPos = (byte)(bitNum & 0x1F);    // get the lowest 5 bits

			uint mask = (uint)1 << bitPos;
			return ( this.data[bytePos] & mask ) != 0;
		}



		//***********************************************************************
		// Sets the value of the specified bit to 0
		// The Least Significant Bit position is 0.
		//***********************************************************************

		public void unsetBit(uint bitNum)
		{
			uint bytePos = bitNum >> 5;

			if(bytePos < this.dataLength)
			{
				byte bitPos = (byte)(bitNum & 0x1F);

				uint mask = (uint)1 << bitPos;
				uint mask2 = 0xFFFFFFFF ^ mask;

				this.data[bytePos] &= mask2;

				if(this.dataLength > 1 && this.data[this.dataLength - 1] == 0)
					this.dataLength--;
			}
		}


		//***********************************************************************
		// Returns a value that is equivalent to the integer square root
		// of the BigInteger.
		//
		// The integer square root of "this" is defined as the largest integer n
		// such that (n * n) <= this
		//
		//***********************************************************************

		public BigInteger sqrt()
		{
			uint numBits = (uint)this.bitCount();

			if((numBits & 0x1) != 0)        // odd number of bits
				numBits = (numBits >> 1) + 1;
			else
				numBits = (numBits >> 1);

			uint bytePos = numBits >> 5;
			byte bitPos = (byte)(numBits & 0x1F);

			uint mask;

			BigInteger result = new BigInteger();
			if(bitPos == 0)
				mask = 0x80000000;
			else
			{
				mask = (uint)1 << bitPos;
				bytePos++;
			}
			result.dataLength = (int)bytePos;

			for(int i = (int)bytePos - 1; i >= 0; i--)
			{
				while(mask != 0)
				{
					// guess
					result.data[i] ^= mask;

					// undo the guess if its square is larger than this
					if((result * result) > this)
						result.data[i] ^= mask;

					mask >>= 1;
				}
				mask = 0x80000000;
			}
			return result;
		}


		//***********************************************************************
		// Returns the k_th number in the Lucas Sequence reduced modulo n.
		//
		// Uses index doubling to speed up the process.  For example, to calculate V(k),
		// we maintain two numbers in the sequence V(n) and V(n+1).
		//
		// To obtain V(2n), we use the identity
		//      V(2n) = (V(n) * V(n)) - (2 * Q^n)
		// To obtain V(2n+1), we first write it as
		//      V(2n+1) = V((n+1) + n)
		// and use the identity
		//      V(m+n) = V(m) * V(n) - Q * V(m-n)
		// Hence,
		//      V((n+1) + n) = V(n+1) * V(n) - Q^n * V((n+1) - n)
		//                   = V(n+1) * V(n) - Q^n * V(1)
		//                   = V(n+1) * V(n) - Q^n * P
		//
		// We use k in its binary expansion and perform index doubling for each
		// bit position.  For each bit position that is set, we perform an
		// index doubling followed by an index addition.  This means that for V(n),
		// we need to update it to V(2n+1).  For V(n+1), we need to update it to
		// V((2n+1)+1) = V(2*(n+1))
		//
		// This function returns
		// [0] = U(k)
		// [1] = V(k)
		// [2] = Q^n
		//
		// Where U(0) = 0 % n, U(1) = 1 % n
		//       V(0) = 2 % n, V(1) = P % n
		//***********************************************************************

		public static BigInteger[] LucasSequence(BigInteger P, BigInteger Q,
			BigInteger k, BigInteger n)
		{
			if(k.dataLength == 1 && k.data[0] == 0)
			{
				BigInteger[] result = new BigInteger[3];

				result[0] = 0; result[1] = 2 % n; result[2] = 1 % n;
				return result;
			}

			// calculate constant = b^(2k) / m
			// for Barrett Reduction
			BigInteger constant = new BigInteger();

			int nLen = n.dataLength << 1;
			constant.data[nLen] = 0x00000001;
			constant.dataLength = nLen + 1;

			constant = constant / n;

			// calculate values of s and t
			int s = 0;

			for(int index = 0; index < k.dataLength; index++)
			{
				uint mask = 0x01;

				for(int i = 0; i < 32; i++)
				{
					if((k.data[index] & mask) != 0)
					{
						index = k.dataLength;      // to break the outer loop
						break;
					}
					mask <<= 1;
					s++;
				}
			}

			BigInteger t = k >> s;

			//// Console.WriteLine("s = " + s + " t = " + t);
			return LucasSequenceHelper(P, Q, t, n, constant, s);
		}


		//***********************************************************************
		// Performs the calculation of the kth term in the Lucas Sequence.
		// For details of the algorithm, see reference [9].
		//
		// k must be odd.  i.e LSB == 1
		//***********************************************************************

		private static BigInteger[] LucasSequenceHelper(BigInteger P, BigInteger Q,
			BigInteger k, BigInteger n,
			BigInteger constant, int s)
		{
			BigInteger[] result = new BigInteger[3];

			if((k.data[0] & 0x00000001) == 0)
				throw (new ArgumentException("Argument k must be odd."));

			int numbits = k.bitCount();
			uint mask = (uint)0x1 << ((numbits & 0x1F) - 1);

			// v = v0, v1 = v1, u1 = u1, Q_k = Q^0

			BigInteger v = 2 % n, Q_k = 1 % n,
				v1 = P % n, u1 = Q_k;
			bool flag = true;

			for(int i = k.dataLength - 1; i >= 0 ; i--)     // iterate on the binary expansion of k
			{
				//// Console.WriteLine("round");
				while(mask != 0)
				{
					if(i == 0 && mask == 0x00000001)        // last bit
						break;

					if((k.data[i] & mask) != 0)             // bit is set
					{
						// index doubling with addition

						u1 = (u1 * v1) % n;

						v = ((v * v1) - (P * Q_k)) % n;
						v1 = n.BarrettReduction(v1 * v1, n, constant);
						v1 = (v1 - ((Q_k * Q) << 1)) % n;

						if(flag)
							flag = false;
						else
							Q_k = n.BarrettReduction(Q_k * Q_k, n, constant);

						Q_k = (Q_k * Q) % n;
					}
					else
					{
						// index doubling
						u1 = ((u1 * v) - Q_k) % n;

						v1 = ((v * v1) - (P * Q_k)) % n;
						v = n.BarrettReduction(v * v, n, constant);
						v = (v - (Q_k << 1)) % n;

						if(flag)
						{
							Q_k = Q % n;
							flag = false;
						}
						else
							Q_k = n.BarrettReduction(Q_k * Q_k, n, constant);
					}

					mask >>= 1;
				}
				mask = 0x80000000;
			}

			// at this point u1 = u(n+1) and v = v(n)
			// since the last bit always 1, we need to transform u1 to u(2n+1) and v to v(2n+1)

			u1 = ((u1 * v) - Q_k) % n;
			v = ((v * v1) - (P * Q_k)) % n;
			if(flag)
				flag = false;
			else
				Q_k = n.BarrettReduction(Q_k * Q_k, n, constant);

			Q_k = (Q_k * Q) % n;


			for(int i = 0; i < s; i++)
			{
				// index doubling
				u1 = (u1 * v) % n;
				v = ((v * v) - (Q_k << 1)) % n;

				if(flag)
				{
					Q_k = Q % n;
					flag = false;
				}
				else
					Q_k = n.BarrettReduction(Q_k * Q_k, n, constant);
			}

			result[0] = u1;
			result[1] = v;
			result[2] = Q_k;

			return result;
		}

		public ulong ULongValue()
		{
			ulong value = 0L;
			if( dataLength > 2 )
				throw new OverflowException();
			if( dataLength > 0 )
				value = data[ 0 ];
			if( dataLength > 1 )
				value |= ((ulong)data[ 1 ]) << 32;

			return value;
		}
	}

	#endregion
} // End namespace Xheo.Licensing

////////////////////////////////////////////////////////////////////////////////
#region Copyright © 2002-2005 XHEO, INC. All Rights Reserved.
//
#endregion
////////////////////////////////////////////////////////////////////////////////
