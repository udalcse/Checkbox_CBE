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
// Class:		LicenseServerLimit
// Author:		Paul Alexander
// Created:		Friday, September 13, 2002 5:25:39 PM
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.ComponentModel;
using System.Xml;
using System.Web;
using System.Web.UI;
using System.Net;
using System.Security.Cryptography;
using System.Collections;
using System.Collections.Specialized;

namespace Xheo.Licensing
{
#if LICENSING ||  LICENSESERVERS
	/// <summary>
	/// Enforces a limit for a LicenseServers license.
	/// </summary>
	[Serializable]
#if LICENSING
	public 
#else
	internal
#endif
		class LicenseServerLimit : Limit
	{
		////////////////////////////////////////////////////////////////////////////////
		#region Fields

		private string					_url				= null;
		private int						_allowedFailures	= 5;
		private StringCollection		_alternates			= new StringCollection();
		private bool					_isLeased			= false;
		private int						_leaseLength		= 3 * 24;
		private DateTime				_leaseExpires		= DateTime.MinValue;
		private string					_leasedMachine		= null;
		private bool					_useValidateEx			= true;
		private bool					_allowInitialFailure	= false;

		private static FailureManager	_failures			= new FailureManager();

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Constructors/Destructors

		/// <summary>
		///	Initializes a new instance of the LicenseServerLimit class.
		/// </summary>
		public LicenseServerLimit()
		{
		}
		
		/// <summary>
		///	Initializes a new instance of the LicenseServerLimit class.
		/// </summary>
		/// <param name="url">
		///		Url of the license server.
		/// </param>
		public LicenseServerLimit( string url ) : this( url, 5, false, 3 * 24 )
		{
		}

		/// <summary>
		///	Initializes a new instance of the LicenseServerLimit class.
		/// </summary>
		/// <param name="url">
		///		Url of the license server.
		/// </param>
		/// <param name="allowedFailures">
		///		Number of times the validation can fail before the limit is considered
		///		invalid.
		/// </param>
		public LicenseServerLimit( string url, int allowedFailures ) : this( url, allowedFailures, false, 3 * 24 )
		{
		}

		/// <summary>
		///	Initializes a new instance of the LicenseServerLimit class.
		/// </summary>
		/// <param name="url">
		///		Url of the license server.
		/// </param>
		/// <param name="allowedFailures">
		///		Number of times the validation can fail before the limit is considered
		///		invalid.
		/// </param>
		/// <param name="isLeased">
		///		Indicates if the license is leased.
		/// </param>
		public LicenseServerLimit( string url, int allowedFailures, bool isLeased ) : this( url, allowedFailures, isLeased, 3 * 24 )
		{
		}

		/// <summary>
		///	Initializes a new instance of the LicenseServerLimit class.
		/// </summary>
		/// <param name="url">
		///		Url of the license server.
		/// </param>
		/// <param name="allowedFailures">
		///		Number of times the validation can fail before the limit is considered
		///		invalid.
		/// </param>
		/// <param name="isLeased">
		///		Indicates if the license is leased.
		/// </param>
		/// <param name="leaseLength">
		///		The length of thelease in hours.
		/// </param>
		public LicenseServerLimit( string url, int allowedFailures, bool isLeased, int leaseLength )
		{
			_url				= url;
			_allowedFailures	= allowedFailures;
			_isLeased			= isLeased;
			_leaseLength		= leaseLength;
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Properties

		/// <summary>
		/// Gets or sets the URL of the license server web service.
		/// </summary>
		/// <remarks>
		///		<seealso cref="LicenseServer"/>
		/// </remarks>
#if LICENSING
		[
			Category( "Limits" ),
			Description( "The URL of the licenser server WebService." )
		]
#endif
		public string Url
		{
			get
			{
				return _url;
			}
			set
			{
				AssertNotReadOnly();
				_url = value;
			}
		}

		/// <summary>
		/// Gets a collection of alternate server URLs to use in case a connection to the primary
		/// server cannot be established. If the license is read only then a copy of the collection
		/// is returned and any changes made are lost.
		/// </summary>
#if LICENSING
		[
			Category( "Limits" ),
			Description( "Collection of alternate server URLs to use in case a connection to the primary server cannot be established" ),
			Editor( typeof( Design.StringCollectionEditor ), typeof( System.Drawing.Design.UITypeEditor ) )
		]
#endif
		public StringCollection Alternates
		{
			get
			{
				if( License != null && License.IsReadOnly )
				{
					StringCollection readOnlyCollection = new StringCollection();
					string[] copy = new string[ _alternates.Count ];
					_alternates.CopyTo( copy, 0 );
					readOnlyCollection.AddRange( copy );
					return readOnlyCollection;
				}
				return _alternates;
			}
		}

		/// <summary>
		/// Gets or sets the number of times the remote validation can fail before the
		/// limit is considered violated. Allows for congestion, and other internet 
		/// problems.
		/// </summary>
#if LICENSING
		[
			Category( "Limits" ),
			Description( "The number of times remove validation can fail before the limit is considered violated. Allows for congestion, and other Internet problems." ),
			DefaultValue( 5 )
		]
#endif
		public int AllowedFailures
		{
			get
			{
				return _allowedFailures;
			}
			set
			{
				AssertNotReadOnly();
				_allowedFailures = value;
			}
		}

		/// <summary>
		/// Gets or sets a value that indicates if the license should be leased.
		/// </summary>
		/// <remarks>
		///		Leased licenses can help reduce network traffic and increase reliability by
		///		reducing the number of times a remote server is queried for validation. When
		///		a leased license is validated by a <see cref="LicenseServer"/>, the lease is 
		///		renewed for the amount of time  specified in <see cref="LeaseLength"/>. 		///		
		///		<para>
		///		XHEO|Licensing must have permission to overwrite the original license file for 
		///		the lease to renew. If it can't write to the file then the lease is not reviewed, but 
		///		the license validation still succeeds. The lease is only valid on the machine 
		///		it was originally granted to. If copied to a new machine a new lease is 
		///		requested from the license server.
		///		</para>
		/// </remarks>
#if LICENSING
		[
			Category( "Lease" ),
			Description( "Indicates if the license is leased. Requests to the license servers are only made once during the LeaseLength." ),
			DefaultValue( false )
		]
#endif
		public bool IsLeased
		{
			get
			{
				return _isLeased;
			}
			set
			{
				_isLeased = value;
			}
		}

		/// <summary>
		/// Gets or sets the length of a the lease in hours. 
		/// </summary>
#if LICENSING
		[
			Category( "Lease" ),
			Description( "The length of the lease in hours." ),
			DefaultValue( 72 )
		]
#endif
		public int LeaseLength
		{
			get
			{
				return _leaseLength;
			}
			set
			{
				_leaseLength = value;
			}
		}

		/// <summary>
		/// Gets or sets the date when the current lease expires.
		/// </summary>
#if LICENSING
		[ Browsable( false ) ]
#endif
		public DateTime LeaseExpires
		{
			get
			{
				return _leaseExpires;
			}
			set
			{
				_leaseExpires = value;
			}
		}

		/// <summary>
		/// Gets or sets the profile hash of the machine the license was leased to.
		/// </summary>
#if LICENSING
		[ Browsable( false ) ]
#endif
		public string LeasedMachine
		{
			get
			{
				return _leasedMachine;
			}
			set
			{
				_leasedMachine = value;
			}
		}

		/// <summary>
		/// Gets or sets a value that indicates if XHEO|Licensing should call the ValidateEx method
		/// on the license server. License server must be running version 2.0 or later.
		/// </summary>
#if LICENSING
		[
			Category( "Limits" ),
			Description( "Indicates if the XHEO|Licensing should call the ValidateEx method on the license server. License server must be running version 1.2 or later." ),
			DefaultValue( true )
		]
#endif
		public bool UseValidateEx
		{
			get
			{
				return _useValidateEx;
			}
			set
			{
				_useValidateEx = value;
			}
		}

		/// <summary>
		/// Gets or sets a value that indicates if a failure the first time the Type is requested
		/// after the process starts should cause a hard failure.
		/// </summary>
		/// <remarks>
		///		Previous versions of XHEO|Licensing allowed a minimum number of failures to 
		///		occur before a hard failure. If the process was restarted before the maximum
		///		number of failures occurred, a user could effectively run without ever checking
		///		back with the license server. By setting AllowInitialFailure to false, the
		///		process must acquire a valid response at least the first time it is requested.
		///		Further requests are allowed to fail to enable an ongoing application to continue
		///		running even if the license server goes down.
		///		<blockquote class="dtBLOCK"><b>Note</b> Rather then using a failure tolerance
		///		it is better to use <see cref="IsLeased"/> to force the license to obtain a
		///		long running validate to eliminate most intermittent problems associated with
		///		Internet connectivity.</blockquote>
		/// </remarks>
#if LICENSING
		[
			Category( "Limits" ),
			Description( "Indicates if failures on the first validation attempt are allowed." ),
			DefaultValue( false )
		]
#endif
		public bool AllowInitialFailure
		{
			get
			{
				return _allowInitialFailure;
			}
			set
			{
				_allowInitialFailure = value;
			}
		}

		/// <summary>
		/// Overrides <see cref="Limit.IsRemote"/>.
		/// </summary>
		public override bool IsRemote
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Gets the description of the limit.
		/// </summary>
		public override string Description
		{
			get
			{
				return Internal.StaticResourceProvider.CurrentProvider.GetString( "MLS_Description" );
			}
		}

		/// <summary>
		/// Gets the display name of the limit.
		/// </summary>
		public override string Name
		{
			get
			{
				return "License Server";
			}
		}

		/// <summary>
		/// Gets a value that indicates if the license server limit is time sensitive.
		/// </summary>
		public override bool IsTimeSensitive
		{
			get
			{
				return IsLeased;
			}
		}


		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Operations

		/// <summary>
		/// Validates that the license being granted is within the limits enforced by
		/// this object.
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
		/// <returns>
		///		Returns true if the limits are valid, otherwise false.
		/// </returns>
		public override bool Validate( LicenseContext context, Type type, object instance )
		{
			bool attemptRenew = false;
			Version v2_0 = ExtendedLicense.v2_0;

			if( _isLeased && _leaseExpires > DateTime.UtcNow && 
				MachineProfile.Profile.CompareTo( _leasedMachine, 0 ) )
			{
				TimeSpan time = _leaseExpires - DateTime.UtcNow;
				if( ( time.TotalHours / _leaseLength ) < 0.10 || time.TotalHours < 48 )
					attemptRenew = true;
				else
					return true;
			}

			ExternalLicenseServer.XheoLicensingServer server = new ExternalLicenseServer.XheoLicensingServer();

			if( _url == null )
			{
				License.InvalidReason = Internal.StaticResourceProvider.CurrentProvider.GetString( "E_NoLicenseUrl" );
				return false;
			}

			bool retry;

			do
			{
				retry = false;
				try
				{
					server.Proxy = ExtendedLicense.Proxy;
					RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
					byte[] saltBytes = new byte[ 32 ];
					string salt;
					string licenseXml;
					string response = "FAILED";
					LicenseSigningKey key = LicenseSigningKey.GetPublicKey( type );
			
					rng.GetBytes( saltBytes );
					salt = Convert.ToBase64String( saltBytes );

					server.Url = Url.ToString();
					licenseXml = License.ToXmlString();
				
					ArrayList servers = new ArrayList();
					servers.Add( Url );
					servers.AddRange( Alternates );
					Exception firstException = null;
					for( int index = 0; index < servers.Count; index++ )
					{
						server.Url = ((string)servers[ index ]).ToString();

						try
						{
							if( License.Version >= v2_0 && UseValidateEx )
							{
								server.LicenseCultureSoapHeaderValue = new Xheo.Licensing.ExternalLicenseServer.LicenseCultureSoapHeader();
								server.LicenseCultureSoapHeaderValue.CultureName = System.Globalization.CultureInfo.CurrentUICulture.Name;
								response = server.ValidateEx( licenseXml, type.Assembly.GetName().Name + ".lsk", salt, MachineProfile.Profile.Hash, null );
							}
							else
								response = server.Validate( licenseXml, type.Assembly.GetName().Name + ".lsk", salt );

							if( response != "FAILED" && key.ValidateResponse( salt, response ) )
								break;
						}
						catch( Exception ex )
						{
							response = "FAILED";
							if( index == 0 )
								firstException = ex;
						}
					}

					if( response == "FAILED" && ! attemptRenew )
					{
						if( firstException != null )
							throw firstException;

						if( _failures.RegisterFailure( context, type, AllowedFailures ) )
						{
							License.InvalidReason		= Internal.StaticResourceProvider.CurrentProvider.FormatResourceString( "E_CouldNotValidateWith", Url );
							return false;
						}
					
						return true;
					}

					if( key.ValidateResponse( salt, response ) )
					{
						_failures.RegisterSuccess( context, type );

						if( IsLeased )
						{
#if ! DEBUG
						try
						{
#endif
							string leased = server.RenewLease( License.ToXmlString(), MachineProfile.Profile.Hash, type.Assembly.GetName().Name + ".lsk" );
							ExtendedLicensePack pack = License.IsEmbedded ? License.LicensePack : new ExtendedLicensePack( License.LicensePack.Location, true );
							ExtendedLicense license = pack[ pack.IndexOf( License.AbsoluteSerialNumber ) ];
							license.FromXmlString( leased );
							try
							{
								pack.Save( true );
							}
							catch{}
							LicenseServerLimit limit = license.GetLimit( typeof( LicenseServerLimit ) ) as LicenseServerLimit;
							if( limit != null )
							{
								_leaseExpires = limit._leaseExpires;
								_leasedMachine = limit._leasedMachine;
							}
							else
							{
								_leaseExpires = DateTime.UtcNow.AddHours( LeaseLength );
								_leasedMachine = MachineProfile.Profile.Hash;
							}
							License.Signature = license.Signature;
#if ! DEBUG
						}
						catch{}
#endif
						}
						return true;
					}

					if( attemptRenew || ! _failures.RegisterFailure( context, type, AllowedFailures ) )
						return true;
					else
						License.InvalidReason		= Internal.StaticResourceProvider.CurrentProvider.FormatResourceString( "E_CouldNotValidateWithInvalid", Url );
				}
				catch( Exception ex )
				{
					retry = ProxyForm.RetryException( null, ex, _url );

					if( ! retry )
					License.InvalidReason	= Internal.StaticResourceProvider.CurrentProvider.FormatResourceString( "E_CouldNotValidateWithUnexpected", 
						Url, FailureReportForm.PreProcessException( ex ) );
				}
			} while( retry );
			return false;
		}

		/// <summary>
		/// Loads the contents of a Limit from an XML string.
		/// </summary>
		/// <param name="node">
		///		The XML string to load from.
		/// </param>
		/// <returns>
		///		Returns true if the load was successful, otherwise false.
		/// </returns>
		public override bool LoadFromXml( XmlNode node )
		{
			if( String.Compare( node.Attributes[ "type" ].Value, "LicenseServer", false, System.Globalization.CultureInfo.InvariantCulture ) != 0 )
				return false;

			if( node.Attributes[ "url" ] == null )
				return false;

			_url = node.Attributes[ "url" ].Value.Trim();

			if( node.Attributes[ "allowedFailures" ] != null )
				_allowedFailures = Convert.ToInt32( node.Attributes[ "allowedFailures" ].Value, System.Globalization.CultureInfo.InvariantCulture );

			_alternates.Clear();
			foreach( XmlNode alternate in node.SelectNodes( "Alternate" ) )
				Alternates.Add( alternate.InnerText );

			if( node.Attributes[ "isLeased" ] != null )
				_isLeased = XmlConvert.ToBoolean( node.Attributes[ "isLeased" ].Value );
			else
				_isLeased = false;

			if( node.Attributes[ "leaseLength" ] != null )
				_leaseLength = XmlConvert.ToInt32( node.Attributes[ "leaseLength" ].Value );
			else
				_leaseLength = 3 * 24;

			if( node.Attributes[ "leasedMachine" ] != null )
				_leasedMachine = node.Attributes[ "leasedMachine" ].Value;
			else
				_leasedMachine = null;

			if( node.Attributes[ "leaseExpires" ] != null )
				_leaseExpires = XmlConvert.ToDateTime( node.Attributes[ "leaseExpires" ].Value, XmlDateTimeSerializationMode.Unspecified );
			else
				_leaseExpires = DateTime.MinValue;

			if( node.Attributes[ "useValidateEx" ] != null )
				_useValidateEx = XmlConvert.ToBoolean( node.Attributes[ "useValidateEx" ].Value );
			else
				_useValidateEx = true;

			if( node.Attributes[ "allowInitialFailure" ] != null )
				_allowInitialFailure = XmlConvert.ToBoolean( node.Attributes[ "allowInitialFailure" ].Value );
			else
				_allowInitialFailure = false;

			return true;
		}

		/// <summary>
		/// Saves the contents of the Limit as an XML string.
		/// </summary>
		/// <returns>
		///		Returns the saved XML.
		/// </returns>
		public override XmlNode SaveToXml( XmlNode parent )
		{
			XmlNode limitNode = parent.OwnerDocument.CreateElement( "Limit" );
			XmlAttribute attribute = parent.OwnerDocument.CreateAttribute( "type" );
			attribute.Value = "LicenseServer";
			limitNode.Attributes.Append( attribute );

			attribute = parent.OwnerDocument.CreateAttribute( "url" );
			attribute.Value = _url == null ? null : _url.ToString();
			limitNode.Attributes.Append( attribute );

			attribute = parent.OwnerDocument.CreateAttribute( "allowedFailures" );
			attribute.Value = XmlConvert.ToString( _allowedFailures );
			limitNode.Attributes.Append( attribute );

			Version v1_1 = ExtendedLicense.v1_1;
			Version v2_0 = ExtendedLicense.v2_0;

			if( License == null || License.Version >= v1_1 )
			{
				foreach( string alternate in Alternates )
				{
					XmlNode alternateNode = parent.OwnerDocument.CreateElement( "Alternate" );
					alternateNode.InnerText = alternate;
					limitNode.AppendChild( alternateNode );
				}

				if( _isLeased )
				{
					attribute = parent.OwnerDocument.CreateAttribute( "isLeased" );
					attribute.Value = XmlConvert.ToString( _isLeased );
					limitNode.Attributes.Append( attribute );

					attribute = parent.OwnerDocument.CreateAttribute( "leaseLength" );
					attribute.Value = XmlConvert.ToString( _leaseLength );
					limitNode.Attributes.Append( attribute );

					attribute = parent.OwnerDocument.CreateAttribute( "leasedMachine" );
					attribute.Value = _leasedMachine;
					limitNode.Attributes.Append( attribute );

					attribute = parent.OwnerDocument.CreateAttribute( "leaseExpires" );
					attribute.Value = _leaseExpires.ToString( "s", System.Globalization.CultureInfo.InvariantCulture );
					limitNode.Attributes.Append( attribute );
				}

				if( License == null || License.Version >= v2_0 )
				{
					if( ! _useValidateEx )
					{
						attribute = parent.OwnerDocument.CreateAttribute( "useValidateEx" );
						attribute.Value = XmlConvert.ToString( false );
						limitNode.Attributes.Append( attribute );
					}

					if( _allowInitialFailure )
					{
						attribute = parent.OwnerDocument.CreateAttribute( "allowInitialFailure" );
						attribute.Value = XmlConvert.ToString( true );
						limitNode.Attributes.Append( attribute );
					}

				}
			}

			parent.AppendChild( limitNode );
			return limitNode;
		}


		/// <summary>
		/// Overrides <see cref="Limit.GetDetailsDescription"/>.
		/// </summary>
		public override string GetDetailsDescription()
		{
			return _url == null ? null : _url.ToString();
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
	} // End class LicenseServerLimit
	#endif
} // End namespace Xheo.Licensing

////////////////////////////////////////////////////////////////////////////////
#region Copyright © 2002-2005 XHEO, INC. All Rights Reserved.
//
#endregion
////////////////////////////////////////////////////////////////////////////////
