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
// Class:		ActivationServer
// Author:		Paul Alexander
// Created:		Thursday, September 19, 2002 2:03:50 PM
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Configuration;
using System.IO;
using System.Globalization;



namespace Xheo.Licensing
{
#if LICENSING || LICENSEACTIVATION
	/// <summary>
	/// Implements an Activation Server as an XML WebService. 
	/// </summary>
	/// <remarks>
	/// You can create an Activation Server manually or by using the License Manager
	/// Setup Wizard. From the License Manager select Tools | Setup Wizard and select
	/// the Web.config file of the web application that will host the Activation Server
	/// Web Service.
	/// 
	/// <para>
	///	To implement your activation server manually, derive from this class in your web 
	///	service file and override the Activate method. This method gets a copy of
	///	the complete signed license and the machine profile hash. The returned
	///	license must then set the profile hash of the Activation limit in the license
	///	and resign the license.
	///	</para>
	///	<para>
	///	You can use this server to implement a central license activation service.
	///	</para>
	///	</remarks>
	[ WebService( Name = "XHEO|Licensing Activation Server", 
		 Description = "Licensing activation service for the XHEO|Licensing package.",
		 Namespace = "http://www.xheo.com/licensing/activation" ) ]
	[ ToolboxItem( false ) ]
#if LICENSING
	public 
#else
	internal
#endif
	class ActivationServer : System.Web.Services.WebService
	{
		////////////////////////////////////////////////////////////////////////////////
		#region Fields

		private string _keyFolder = null;

		/// <summary>
		/// Additional custom headers to include with all web request calls.
		/// </summary>
		public LicenseCultureSoapHeader Headers = null;

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Constructors/Destructors

		/// <summary>
		///	Initializes a new instance of the ActivationServer class.
		/// </summary>
		public ActivationServer()
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
		/// Activates a previously issued license.
		/// </summary>
		/// <param name="keys">
		///		Name of the assembly whos keys were used to sign the licenses. The public key
		///		is embedded in the component's assembly. The private key is stored
		///		securely on the license server's machine
		/// </param>
		/// <param name="licenseXml">
		///		The signed license as an XML string to validate.
		/// </param>
		/// <param name="profileHash">
		///		The comparable profile hash of the <see cref="MachineProfile"/> from the machine 
		///		the license is being activated from.
		/// </param>
		/// <remarks>
		///		Generally you should perform some kind of tracking to make sure that the
		///		serial number for the license has not already been activated by tracking
		///		them in a database. If the profileHash is different then was previously 
		///		activated with the serial number, then the activation should fail.
		///		<para>
		///		The license server must have full access to the original signing
		///		keys used to sign the license to enable activation. Detailed failure
		///		messages should be communicated through exceptions.
		///		</para>
		///		<para>
		///		If there are multitle activation limits in the license then they
		///		are all updated with the same profile hash.
		///		</para>
		///		<para>
		///		Ineritors should call the base class's Validate method and capture
		///		the return value for success.
		///		<code>
		///		public override string Activate( string licenseXml, string keys, string profileHash )
		///		{
		///			string response = base.Activate( licenseXml, keys, profileHash ); 
		///								// ^-- Throws exception if invalid.
		///			
		///			// Do more validation.
		///			
		///			return response;
		///		}
		///		</code>
		///		</para>
		///	</remarks>
		/// <returns>
		///		On success returns the newly signed license, otherwise throws an
		///		exception.
		/// </returns>
		[
			WebMethod( false, Description = "Activates a previously issued license." ),
			SoapHeader( "Headers", Direction = SoapHeaderDirection.InOut )
		]
		public virtual string Activate( string licenseXml, string keys, string profileHash )
		{
			UseClientCulture();
			LicenseSigningKey	key		= GetSigningKey( keys );
			ExtendedLicense		license	= new ExtendedLicense();

			license.FromXmlString( licenseXml );

			if( ! key.ValidateLicense( license ) )
				throw new ExtendedLicenseException( "E_InvalidSignature" );

			if( profileHash == null || profileHash.Length == 0 )
				throw new ExtendedLicenseException( "E_InvalidProfileHash" );

			if( ! UpdateActivationLimit( license.Limits, profileHash ) )
				throw new ExtendedLicenseException( "E_MissingActivation" );

			key.SignLicense( license );
			return license.ToXmlString();
		}
		#region Helpers
		private bool UpdateActivationLimit( LimitCollection limits, string profileHash )
		{
			bool limitFound = false;
			foreach( Limit limit in limits )
			{
				if( limit is ActivationLimit )
				{ 
					limitFound = true;
					if( ( ((ActivationLimit)limit).ProfileHash == null || ((ActivationLimit)limit).ProfileHash.Length == 0 ) )
						((ActivationLimit)limit).ProfileHash = profileHash;
				}
				else if ( limit is ContainerLimit )
				{
					limitFound |= UpdateActivationLimit( ((ContainerLimit)limit).Limits, profileHash );
				}
			}

			return limitFound;
		}
		#endregion

		/// <summary>
		/// Internal helper method used to get a signing key pair given the name
		/// of the original signing keys.
		/// </summary>
		/// <param name="keys">
		///		Name of the key files used to sign licenses.
		/// </param>
		/// <returns>
		///		Returns a new signing key. If the keys were not found an exception is raised.
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
		///		The folder to look for licenses. If null then uses the default folder.
		/// </param>
		/// <returns>
		///		Returns a new signing key. If the keys were not found an exception is raised.
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
	} // End class ActivationServer
#endif
} // End namespace Xheo.Licensing

////////////////////////////////////////////////////////////////////////////////
#region Copyright © 2002-2005 XHEO, INC. All Rights Reserved.
//
#endregion
////////////////////////////////////////////////////////////////////////////////
