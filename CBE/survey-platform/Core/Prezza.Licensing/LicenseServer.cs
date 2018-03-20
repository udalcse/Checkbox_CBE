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
// Class:		LicenseServer
// Author:		Paul Alexander
// Created:		Thursday, September 19, 2002 2:03:50 PM
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Configuration;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.Remoting.Messaging;
using System.Globalization;

namespace Xheo.Licensing
{
#if LICENSING || LICENSESERVERS
	/// <summary>
	/// Implements a licensing server over a web service. 
	/// </summary>
	/// <remarks>
	///	To implement your own licensing server, derive from this class in your web 
	///	service file and override the Validate method. This method gets a copy of
	///	the complete signed license requesting validation. The default implementation
	///	automatically returns true.
	///	<para>
	///	You can use this server to implement a central license registraion service.
	///	</para>
	///	</remarks>
	[ WebService( Name = "XHEO|Licensing Server", 
		  Description = "Licensing enforcement and registration service for the XHEO|Licensing package.",
		  Namespace = "http://www.xheo.com/licensing/" ) ]
	[ ToolboxItem( false ) ]
#if LICENSING
	public 
#else
	internal
#endif
		class LicenseServer : System.Web.Services.WebService
	{
		////////////////////////////////////////////////////////////////////////////////
		#region Fields

		private string	_keyFolder		= null;
		private bool	_autoRegister	= false;
		
		/// <summary>
		/// Additional custom headers to include with all web request calls.
		/// </summary>
		public LicenseCultureSoapHeader Headers = null;

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Constructors/Destructors

		/// <summary>
		///	Initializes a new instance of the LicenseServer class.
		/// </summary>
		public LicenseServer()
		{
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Properties

		/// <summary>
		/// Gets the folder where the license keys are located. See <see cref="ExtendedLicense.KeyFolder"/>
		/// for configuration options.
		/// </summary>
		protected string KeyFolder
		{
			get
			{
				if( _keyFolder == null )
					return ExtendedLicense.KeyFolder;

				return _keyFolder;
			}
			set
			{
				_keyFolder = value;
			}
		}

		/// <summary>
		/// Gets or sets a value that indicates if a license should automatically be 
		/// registered when validating if it is not found in the registration database.
		/// </summary>
		[ 
		Category( "License Server" ),
		DesignerSerializationVisibility( DesignerSerializationVisibility.Visible ),
		NotifyParentProperty( true ),
		]
		public bool AutoRegister
		{
			get
			{
				return _autoRegister;
			}
			set
			{
				_autoRegister = value;
			}
		}

		/// <summary>
		/// Gets the culture information of the client that called the web service.
		/// </summary>
		public CultureInfo ClientCulture
		{
			get
			{
				string cn = "en-US";
				if( Headers != null && Headers.CultureName != null )
					cn = Headers.CultureName;
				return CultureInfo.CreateSpecificCulture( cn );
			}
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Operations

		/// <summary>
		/// When called from a web method, switches the current thread's culture to 
		/// match the client's culture.
		/// </summary>
		protected void UseClientCulture()
		{
			System.Threading.Thread.CurrentThread.CurrentUICulture = ClientCulture;
		}

		/// <summary>
		/// Un-registers a previously signed license for later validation.
		/// </summary>
		/// <param name="licenseXml">
		///		The signed license as an XML string to validate.
		/// </param>
		/// <param name="keys">
		///		Name of the assembly whos key file was used to sign the licenses. The public key
		///		is embedded in the component's assembly. The private key is stored
		///		securely on the license server's machine
		/// </param>
		/// <remarks>
		///		The license server must have full access to the original signing
		///		keys used to sign the license to enable validation.
		///		<para>
		///		Inerhitors should <b><i>not</i></b> call the base class's Validate
		///		method.
		///		</para>
		///	</remarks>
		[
			WebMethod( false, Description = "Registers a signed license for later validation." ),
			SoapHeader( "Headers", Direction = SoapHeaderDirection.InOut )
		]		 
		public virtual void Unregister( string licenseXml, string keys )
		{
			UseClientCulture();
			throw new ExtendedLicenseException( "E_UnregisterNotSupported" );
		}

		/// <summary>
		/// Registers a previously signed license for later validation.
		/// </summary>
		/// <param name="licenseXml">
		///		The signed license as an XML string to validate.
		/// </param>
		/// <param name="keys">
		///		Name of the assembly whos key file was used to sign the licenses. The public key
		///		is embedded in the component's assembly. The private key is stored
		///		securely on the license server's machine
		/// </param>
		/// <remarks>
		///		The license server must have full access to the original signing
		///		keys used to sign the license to enable validation.
		///		<para>
		///		Inerhitors should <b><i>not</i></b> call the base class's Validate
		///		method.
		///		</para>
		///	</remarks>
		[
			WebMethod( false, Description = "Registers a signed license for later validation." ),
			SoapHeader( "Headers", Direction = SoapHeaderDirection.InOut )
		]
		public virtual void Register( string licenseXml, string keys )
		{
			UseClientCulture();
			throw new ExtendedLicenseException( "E_RegisterNotSupported" );
		}

		/// <summary>
		///		Registers a license with additional user information. A new license can
		///		optionally be generated in response to the registration.
		/// </summary>
		/// <param name="licenseXml">
		///		The signed license as an XML string to validate. May be null.
		/// </param>
		/// <param name="keys">
		///		Name of the assembly whos key file was used to sign the licenses. The public key
		///		is embedded in the component's assembly. The private key is stored
		///		securely on the license server's machine
		/// </param>
		/// <param name="generateLicense">
		///		Indicates if a new license for the user should be generated. The license should
		///		have it's serial number set to the value in the serialNumber parameter.
		/// </param>
		/// <param name="serialNumber">
		///		The serial number included in the registration. This may or may not have been
		///		entered by the user.
		/// </param>
		/// <param name="valuesXml">
		///		Collection of values from the registration form. Use the <see cref="LicenseValuesCollection"/> to read the values.
		/// </param>
		/// <remarks>
		///		Two types of license registration are available. Either registering an already
		///		purchased license or creating a new one in response to a setup process. This 
		///		way publishers can gather contact information when the user installs their
		///		application.
		///		<h4 class="dtH4">Values XML Reference</h4>
		///		<table cellspacing="0" class="dtTABLE">
		///			<tr valign="top">
		///				<th width="50%">Name</th>
		///				<th width="50%">Description</th>
		///			</tr>
		///			<tr valign="top">
		///				<td width="50%">FirstName</td>
		///				<td width="50%">The first name provided in the registration form.</td>
		///			</tr>
		///			<tr valign="top">
		///				<td width="50%">LastName</td>
		///				<td width="50%">The last name provided in the registration form.</td>
		///			</tr>
		///			<tr valign="top">
		///				<td width="50%">MiddleName</td>
		///				<td width="50%">The middle name provided in the registration form.</td>
		///			</tr>
		///			<tr valign="top">
		///				<td width="50%">Organization</td>
		///				<td width="50%">The name of the organization provided in the registration form.</td>
		///			</tr>
		///			<tr valign="top">
		///				<td width="50%">AssemblyVersion</td>
		///				<td width="50%">The version of the assembly being registered.</td>
		///			</tr>
		///			<tr valign="top">
		///				<td width="50%">FileVersion</td>
		///				<td width="50%">The version of the file being registered.</td>
		///			</tr>
		///			<tr valign="top">
		///				<td width="50%">ProfileHash</td>
		///				<td width="50%">The hash of the <see cref="MachineProfile"/> of the client
		///				machine.</td>
		///			</tr>
		///		</table>
		/// </remarks>
		/// <returns>
		///		When generateLicense is false this method may return null. When true it 
		///		returns the XML of the new license pack.
		///		<blockquote class="dtBLOCK"><b>Note</b>   The return value
		///		is a <see cref="ExtendedLicensePack"/> which may include one or more licenses. At
		///		least one of them should have the serial number set to the value passed in the
		///		serialNumber parameter.</blockquote>
		///		<para>
		///		The base implementation calls <see cref="Register"/> if the licenseXml is not
		///		null.Inheritors should <b>not</b> call the base implementation when overriding the
		///		method.
		///		</para>
		/// </returns>
		[
			WebMethod( false, Description = "Registers an existing license with additional information, or optionally generates a new license with the given serial numnber." ),
			SoapHeader( "Headers", Direction = SoapHeaderDirection.InOut ),
		]
		public virtual string RegisterEx( string licenseXml, string keys, string serialNumber, bool generateLicense, string valuesXml )
		{
			UseClientCulture();
			if( licenseXml != null )
				Register( licenseXml, keys );

			if( generateLicense )
				throw new ExtendedLicenseException( "E_NewLicenseRegister" );

			return null;
		}

		/// <summary>
		/// Determines if the license has been registered with the server. 
		/// </summary>
		/// <param name="license">
		///		The license to look for.
		/// </param>
		/// <returns>
		///		Returns true if the license was previously registered, otherwise false.
		/// </returns>
		/// <remarks>
		///		The base implementaiton allways returns false. You should override this 
		///		method if you store registration information in a database or other
		///		storage medium.
		/// </remarks>
		protected virtual bool IsRegistered( ExtendedLicense license )
		{
			return false;
		}

		/// <summary>
		/// Validates a previously issued license.
		/// </summary>
		/// <param name="keys">
		///		Name of the assembly whos key file was used to sign the licenses. The public key
		///		is embedded in the component's assembly. The private key is stored
		///		securely on the license server's machine
		/// </param>
		/// <param name="licenseXml">
		///		The signed license as an XML string to validate.
		/// </param>
		/// <param name="salt">
		///		Salt used to ensure the response from the server is not intercepted
		///		or altered.
		/// </param>
		/// <remarks>
		///		The license server must have full access to the original signing
		///		keys used to sign the license to enable validation. A random string
		///		is generated (salt) from the ExtendedLicenseProvider and is then
		///		signed and returned on success.
		///		<para>
		///		Ineritors should call the base class's Validate method and capture
		///		the return value for success.
		///		</para>
		/// </remarks>
		/// <example>
		/// 	<code>
		///		public override string Validate( string licenseXml, string keys, string salt )
		///		{
		///			// Throws exception if license is not valid.
		///			string response = base.Validate( licenseXml, keys, salt );
		///			
		///			//Do more validation.
		///			
		///			return response;
		///		}
		///		</code>
		/// </example>
		/// <returns>
		///		Returns a signed response if valid otherwise throw an exception. The response
		///		is the signed version of the salt using the same signing keys used
		///		to sign the license.
		/// </returns>
		[	
			WebMethod( false, Description = "Validates a previously issued license." ),
			SoapHeader( "Headers", Direction = SoapHeaderDirection.InOut )
		]
		public virtual string Validate( string licenseXml, string keys, string salt )
		{
			UseClientCulture();
			LicenseSigningKey	key		= GetSigningKey( keys );
			ExtendedLicense		license	= new ExtendedLicense();

			license.FromXmlString( licenseXml );

			if( ! key.ValidateLicense( license ) )
				throw new ExtendedLicenseException( "E_InvalidLicenseCannotValidate" );

			if( AutoRegister && ! IsRegistered( license ) )
				Register( licenseXml, keys );

			return key.SignResponse( salt );
		}

		/// <summary>
		/// Validates a previously issued license.
		/// </summary>
		/// <param name="keys">
		///		Name of the assembly whos key file was used to sign the licenses. The public key
		///		is embedded in the component's assembly. The private key is stored
		///		securely on the license server's machine
		/// </param>
		/// <param name="licenseXml">
		///		The signed license as an XML string to validate.
		/// </param>
		/// <param name="salt">
		///		Salt used to ensure the response from the server is not intercepted
		///		or altered.
		/// </param>
		/// <param name="profileHash">
		///		The hash of the <see cref="MachineProfile"/> of the client machine requesting 
		///		the license be validated. Use to uniquely identify a given machine.
		/// </param>
		/// <param name="valuesXml">
		///		Additional values reserved for later use.
		/// </param>
		/// <remarks>
		///		The license server must have full access to the original signing
		///		keys used to sign the license to enable validation. A random string
		///		is generated (salt) from the ExtendedLicenseProvider and is then
		///		signed and returned on success.
		///		<para>
		///		Ineritors should call the base class's Validate method and capture
		///		the return value for success.
		///		</para>
		///		<para>
		///		This method simply calls the default <see cref="Validate"/> method unless
		///		overridden in a derived class.
		///		</para>
		/// </remarks>
		/// <example>
		/// 	<code>
		///		public override string ValidateEx( string licenseXml, string keys, string salt, string profileHash, string valuesXml )
		///		{
		///			// Throws exception if license is not valid.
		///			string response = base.ValidateEx( licenseXml, keys, salt, profileHash, valuesXml );
		///			
		///			LicenseValuesCollection values = new LicenseValuesCollection();
		///			values.FromXmlString( valuesXml );
		///			
		///			// if( values[ "Culture" ] == "en-US" )...
		///			
		///			return response;
		///		}
		///		</code>
		/// </example>
		/// <returns>
		///		Returns a signed response if valid otherwise throw an exception. The response
		///		is the signed version of the salt using the same signing keys used
		///		to sign the license.
		/// </returns>
		[
			WebMethod( false, Description = "Validates a previously issued license." ),
			SoapHeader( "Headers", Direction = SoapHeaderDirection.InOut ),
		]
		public virtual string ValidateEx( string licenseXml, string keys, string salt, string profileHash, string valuesXml )
		{
			UseClientCulture();
			return Validate( licenseXml, keys, salt );
		}

		/// <summary>
		/// Generates a new trial license on-demand for the assembly with the given name.
		/// </summary>
		/// <param name="assemblyName">
		///		The name of the assembly to get a trial license for.
		/// </param>
		/// <param name="profileHash">
		///		A profile hash of the computer that the trial is being requested
		///		from. Developers can use this to determine if a new trial license should
		///		be granted.
		/// </param>
		/// <param name="extra">
		///		The value of the <see cref="TrialServerAttribute.Extra"/> property for the
		///		<see cref="TrialServerAttribute"/> declared for the assembly.
		/// </param>
		/// <returns>
		///		Returns a new XML license pack for the assembly.
		/// </returns>
		/// <remarks>
		///		Trial licenses can be generated for new installations of an assembly. The 
		///		profile hash represents a machine profile that developers can use to 
		///		determine if a trial license has been requested for that machine before.
		///		<para>
		///		The base implementation throws an exception indicating that new
		///		trial licenses are not supported. 
		///		</para>
		/// </remarks>
		[
			WebMethod( false, Description = "Generates a new trial license." ),
			SoapHeader( "Headers", Direction = SoapHeaderDirection.InOut )
		]
		public virtual string GetTrialLicense( string extra, string assemblyName, string profileHash )
		{
			UseClientCulture();
			throw new ExtendedLicenseException( "E_NewTrialNotSupported" );
		}

		/// <summary>
		/// Generates a new volatile license according to the lease terms in the original license.
		/// </summary>
		/// <param name="keys">
		///		The name of the assembly whos keys should be used when signing the license.
		/// </param>
		/// <param name="originalLicenseXml">
		///		An XML string representing the original license.
		/// </param>
		/// <param name="profileHash">
		///		A profile hash of the machine requesting the renewal. The volatile leased license
		///		will be locked to the machine making the request.
		/// </param>
		/// <remarks>
		///		Leased licenses can be used to reduce network traffic, improve reliability,
		///		and improve performance. A leased license is essentially a short term durable
		///		validation from a license server. Once a leased license has been validated
		///		a leased copy is requested. This leased copy expires according to the <see cref="LicenseServerLimit.LeaseLength"/>
		///		and does not include the original license server limit, so no further checks are
		///		made back to the license server during the lease time.
		///		<para>
		///		The base implementation Validates the license, removes the original <see cref="LicenseServerLimit"/>,
		///		and adds an <see cref="ActivationLimit"/> with 0 tolerance and no activation server,
		///		signs the license and returns that copy.
		///		</para>
		/// </remarks>
		[
			WebMethod( false, Description = "Renews the original lease for a period of time established in the license." ),
			SoapHeader( "Headers", Direction = SoapHeaderDirection.InOut )
		]
		public virtual string RenewLease( string originalLicenseXml, string profileHash, string keys )
		{
			UseClientCulture();
			Validate( originalLicenseXml, keys, "salt" );

			ExtendedLicense		license = ExtendedLicense.FromXml( originalLicenseXml );
			LicenseSigningKey	key		= GetSigningKey( keys, KeyFolder );
			LicenseServerLimit	limit	= license.GetLimit( typeof( LicenseServerLimit ) ) as LicenseServerLimit;
            
			if( limit == null )
				throw new ExtendedLicenseException( "E_MissingLicenseServer" );
			
			limit.LeaseExpires	= DateTime.UtcNow.AddHours( limit.LeaseLength );
			limit.LeasedMachine	= profileHash;
			key.SignLicense( license );			

			return license.ToXmlString();
		}

		/// <summary>
		/// Internal helper method used to get a signing key pair given the name
		/// of the original signing keys.
		/// </summary>
		/// <param name="keys">
		///		Name of the key files used to sign licenses.
		/// </param>
		/// <returns>
		///		Returns a new signing key. If the keys were not found an 
		///		exception is thrown.
		/// </returns>
		public LicenseSigningKey GetSigningKey( string keys )
		{
			return GetSigningKey( keys, KeyFolder );
		}

		/// <summary>
		/// Internal helper method used to get a signing key pair given the name
		/// of the original signing keys.
		/// </summary>
		/// <param name="keys">
		///		Name of the key files used to sign licenses.
		/// </param>
		/// <param name="folder">
		///		Path to the folder that contains the keys. If null then the default is used.
		/// </param>
		/// <returns>
		///		Returns a new signing key. If the keys were not found an 
		///		exception is thrown.
		/// </returns>
		public static LicenseSigningKey GetSigningKey( string keys, string folder )
		{
			LicenseSigningKey key = LicenseSigningKey.GetSigningKey( keys, folder );

			if( key == null )
				throw new ExtendedLicenseException( "E_InvalidSigningKeys", keys );

			return key;
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
	} // End class LicenseServer
#endif

#if LICENSING || LICENSESERVERS || LICENSEACTIVATION
	/// <summary>
	/// Defines additional information to pass on each web service call.
	/// </summary>
	public class LicenseCultureSoapHeader : SoapHeader
	{
		/// <summary>
		/// The name of the current client's culture.
		/// </summary>
		public string CultureName;
	}
#endif
} // End namespace Xheo.Licensing

////////////////////////////////////////////////////////////////////////////////
#region Copyright © 2002-2005 XHEO, INC. All Rights Reserved.
//
#endregion
////////////////////////////////////////////////////////////////////////////////
