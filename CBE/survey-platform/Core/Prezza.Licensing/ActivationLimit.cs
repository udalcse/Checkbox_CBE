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
// Class:		ActivationLimit
// Author:		Paul Alexander
// Created:		Monday, December 30, 2002 7:47:31 PM
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Xml;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Windows.Forms;

namespace Xheo.Licensing
{
#if LICENSING || LICENSEACTIVATION
	/// <summary>
	/// Restricts usage to an activated license. License are activated through
	/// an <see cref="ActivationServer"/>. You should not have more then one activation limit
	/// per license.
	/// </summary>
	/// <remarks>
	/// <b>NOTE:</b> The process requesting a license should have write access to the
	/// original license file to activate the license.
	/// </remarks>
#if LICENSING
	public 
#else
	internal
#endif
	class ActivationLimit : Limit
	{
		////////////////////////////////////////////////////////////////////////////////
		#region Fields

		private const string ACTIVATIONKEY		= "XL_ACTIVATIONUNLOCKEY";
		private const string ACTIVATIONHASH		= "XL_ACTIVATIONUNLOCKHASH";

		private string		_profileHash		= null;
		private int			_tolerance			= 3;
		private DateTime	_gracePeriod		= DateTime.Today.AddDays( 30 );
		private int			_graceDuration		= 0;
		private string		_url				= null;
		private bool		_brandOutput		= false;
		private bool		_working			= false;
		private bool		_cancelled			= false;
		private bool		_canActivateByKey	= false;
		private bool		_initDuration		= false;
		

		private string		_splashResource		= null;
		private string		_splashUrl			= null;

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Constructors/Destructors

		/// <summary>
		///	Initializes a new instance of the ActivationLimit class.
		/// </summary>
		public ActivationLimit()
		{
		}

		/// <summary>
		/// Initializes a new instance of the ActivationLimit class with the given
		/// hash and tolerance.
		/// </summary>
		/// <param name="tolerance">
		///		The difference tolerance level.
		/// </param>
		/// <param name="gracePeriod">
		///		The date by which the license must be activated.
		/// </param>
		/// <param name="url">
		///		The URL of the <see cref="ActivationServer"/>.
		/// </param>
		public ActivationLimit( string url, int tolerance, DateTime gracePeriod )
		{
			Url			= url;
			Tolerance	= tolerance;
			GracePeriod	= gracePeriod;
		}

		/// <summary>
		/// Initializes a new instance of the ActivationLimit class with the given
		/// hash and tolerance.
		/// </summary>
		/// <param name="url">
		///		The URL of the <see cref="ActivationServer"/>.
		/// </param>
		public ActivationLimit( string url )
		{
			Url			= url;
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Properties

		/// <summary>
		/// Gets or sets the activation unlock key for the license.
		/// </summary>
#if LICENSING
		[
		Category( "Unprotected" ),
		Description( "The activation unlock key used when activating by key. The user can change this value without invalidating the license, but it must match the UnlockHash." )
		]
#endif
		public string ActivationKey
		{
			get
			{
				return License == null ? null : License.MetaValues[ ACTIVATIONKEY ];
			}
			set
			{
				License.MetaValues[ ACTIVATIONKEY ] = value;
			}
		}

		/// <summary>
		/// Gets or sets the profile hash used when unlocking by activation key.
		/// </summary>
#if LICENSING	
		[
		Category( "Unprotected" ),
		Description( "The profile hash used when activating by key. The user can change this value without invalidating the license, but it must match the ActivationKey." )
		]
#endif
		public string UnlockHash
		{
			get
			{
				return License == null ? null : License.MetaValues[ ACTIVATIONHASH ];
			}
			set
			{
				License.MetaValues[ ACTIVATIONHASH ] = value;
			}
		}

		/// <summary>
		/// Gets or sets the comparable profile hash generated from <see cref="MachineProfile.GetComparableHash()"/>.
		/// </summary>
#if LICENSING
		[
		Category( "Limits" ),
		Description( "The comparable profile hash generated from the MachineProfile of the machine the license was activated on." )
		]
#endif
		public string ProfileHash
		{
			get
			{
				return _profileHash;
			}
			set
			{
				AssertNotReadOnly();
				_profileHash = value;
			}
		}

		/// <summary>
		/// Gets or sets the level of difference allowed between the stored 
		/// <see cref="ProfileHash"/> and the hash of the current machine the license is 
		/// running on. A value of 0 means the hash values must be identical, a value
		/// of 10 means they can have the maximum number of differences and effectively 
		/// disables activation.
		/// </summary>
#if LICENSING
		[
		Category( "Limits" ),
		Description( "The level of difference allowed between the stored profile hash and the hash of the current machine the license is running on." ),
		DefaultValue( 3 )
		]
#endif
		public int Tolerance
		{
			get
			{
				return _tolerance;
			}
			set
			{
				AssertNotReadOnly();
				_tolerance = value;
			}
		}

		/// <summary>
		/// Gets or sets the date for the end of the grace period. The license must
		/// be activated by the date given.
		/// </summary>
#if LICENSING
		[
		Category( "Limits" ),
		Description( "Date for the end of the grace period. The license must be activated by the date given." )
		]	
#endif
		public DateTime GracePeriod
		{
			get
			{
				return _gracePeriod;
			}
			set
			{
				AssertNotReadOnly();
				_gracePeriod = value;
			}
		}

		/// <summary>
		/// Gets or sets the time in days before the user must activate. If greater than
		/// zero, it overrides the value of <see cref="GracePeriod"/>.
		/// </summary>
#if LICENSING
		[
			Category( "Limits" ),
			Description( "The time in days before the user must activate. If greater than zero, it overrides the value of GracePeriod." ),
			DefaultValue( 0 )
		]
#endif
		public int GraceDuration
		{
			get
			{
				return _graceDuration;
			}
			set
			{
				AssertNotReadOnly();
				_graceDuration = value;
			}
		}

		/// <summary>
		/// Gets or sets the URL of the <see cref="ActivationServer"/>.
		/// </summary>
#if LICENSING
		[
			Category( "Limits" ),
			Description( "URL of the activation WebService." )
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
				if( value == null || value.Length == 0 )
					_url = value;
				else
					_url = new Uri( value ).ToString();
			}
		}

		/// <summary>
		/// Gets or sets a value that indicates if the license can be activated by key.
		/// </summary>
		/// <remarks>
		///		When true, indicates that the license can be activated by key provided
		///		by email or read over the phone.
		/// </remarks>
#if LICENSING
		[
			Category( "Limits" ),
			Description( "Indicates if the license can be activated by key. If false then phone/email activation is disabled." ),
			DefaultValue( false ),
		]
#endif
		public bool CanActivateByKey
		{
			get
			{
				return _canActivateByKey;
			}
			set
			{
				AssertNotReadOnly();
				_canActivateByKey = value;
			}
		}

		/// <summary>
		/// Gets or sets the assembly qualified name of a resource to display in the bitmap
		/// area of the form. Should be 176 x 352. You can also specify a remote JPEG or GIF
		/// image by using a fully qualified URL.
		/// </summary>
		/// <remarks>
		/// Only URLs beginning with http, https or file can be used.
		/// </remarks>
#if LICENSING
		[
		Category( "Limits" ),
		Description( "The assembly qualified name of a 176 x 352 bitmap resource. You can also specify a remote JPEG or GIF image by using a fully qualified URL. Example: MyComponent.bmp,MyAssembly or http://www.xheo.com/images/licensing/webskin.jpg." )
		]
#endif
		public string SplashResource
		{
			get
			{
				return _splashResource;
			}
			set
			{
				AssertNotReadOnly();
				string vl = value == null ? null : value.ToLower( System.Globalization.CultureInfo.InvariantCulture );
				if( value != null && ( vl.StartsWith( "http://" ) || vl.StartsWith( "https://" ) || vl.StartsWith( "file://" ) ) )
					_splashResource = new Uri( value ).ToString();
				else
					_splashResource = value;
			}
		}
		
		/// <summary>
		/// Gets or sets the URL of a 176 x 550 JPEG or GIF file to be used in the splash area of the activation form when displayed on a web page.
		/// </summary>
#if LICENSING
		[
			Category( "Limits" ),
			Description( "URL of a 176 x 550 JPEG or GIF file to be used in the splash area of the activation form when displayed on a web page." ),
		]
#endif
		public string SplashUrl
		{
			get
			{
				return _splashUrl;
			}
			set
			{
				if( value == null || value.Length == 0 )
					_splashUrl = null;
				else
					_splashUrl = new Uri( value ).ToString();
			}
		}

		/// <summary>
		/// Gets the description of the limit.
		/// </summary>
		public override string Description
		{
			get
			{
				return Internal.StaticResourceProvider.CurrentProvider.GetString( "MAL_Description" );
			}
		}

		/// <summary>
		/// Gets the display name of the limit.
		/// </summary>
		public override string Name
		{
			get
			{
				return "Activation";
			}
		}

		/// <summary>
		/// Gets a value that indicates if remote services are used to validate the license.
		/// </summary>
		public override bool IsRemote
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Gets a value that indicates if a GUI is displayed to interact with the user.
		/// </summary>
		public override bool IsGui
		{
			get
			{
				return ProfileHash == null || ProfileHash.Length == 0;
			}
		}

		/// <summary>
		/// Gets a value that indicates if the Activation limit is performing an asynchronous
		/// activation with the server.
		/// </summary>
#if LICENSING
		[ Browsable( false ) ]
#endif
		public bool IsWorking
		{
			get
			{
				return _working;
			}
		}

		/// <summary>
		/// Gets a value that indicates if the last asynchronous activation was 
		/// canceled using the <see cref="Abort"/> method.
		/// </summary>
#if LICENSING
		[ Browsable( false ) ]
#endif
		public bool IsCanceled
		{
			get
			{
				return _cancelled;
			}
		}

		/// <summary>
		/// Gets a value that indicates if the limit has been activated.
		/// </summary>
#if LICENSING
		[ Browsable( false ) ]
#endif
		public bool IsActivated
		{
			get
			{
				if( ( CanActivateByKey && ActivationKey != null && UnlockHash != null ) ||
					( ProfileHash != null && ProfileHash.Length > 0 ) )
					return true;

				return false;
			}
		}

		/// <summary>
		/// Gets a comma separated list of attributes that can be ignored for creating and
		/// validating the license signature.
		/// </summary>
		protected internal override string UnprotectedAttributes
		{
			get
			{
			    if( License.Version <= ExtendedLicense.v2_0 )
					return "splashResource,splashUrl";
			    
                return null;
			}
		}

		/// <summary>
		/// Gets a value that indicates if the activation limit is time sensitive.
		/// </summary>
		public override bool IsTimeSensitive
		{
			get
			{
				return ! IsActivated;
			}
		}


		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Operations


		/// <summary>
		/// Performs the actual activation of the license.
		/// </summary>
		/// <returns>
		///		Returns true if the license was activated or the user selected to
		///		continue in their grace period, otherwise false.
		/// </returns>
		private bool Activate( Type type )
		{
			//Check if the application is a WinForms app
			if( ! ExtendedLicense.IsWebRequest )
			{
				//Show the activation form and use it to activate the license
				using( ActivationForm form = new ActivationForm( License, this, type ) )
				{
					if( form.ShowDialog() == DialogResult.OK && form.CanContinue == true )
					{
						return true;
					}
				}
			}
			else
			{
				//This is an ASP.NET application...

				//See if the page is trying to activate the license
				string qs = HttpContext.Current.Request.QueryString[ "activateLicense" ];
				if( qs != null && qs.StartsWith( License.SerialNumber ) )
				{
					//Split up the query string so we can tell how to activate
					string[] parts = qs.Split( ',' );

					//If the query string only contains the serial number, the user
					// is trying to activate online using the web service
					if( parts.Length == 1 )
					{
						if( ActivateWithServer( type ) )
						{
							return true;
						}
					}
					else
					{
						//The query string contains both the serial number and the unlock key
						//meaning the user is trying to activate using the key provided via email
						string key = parts[ 1 ];

						//Validate that the unlock key matches the machine profile of this machine
						if( License.PublicKey.ValidateActivationUnlockKey( MachineProfile.Profile.Hash, key ) )
						{

							//If the key matches the current machine profile, save them
							//to the public part of the license
							UnlockHash		= MachineProfile.Profile.Hash;
							ActivationKey	= key;

							try
							{
								if( ! ExtendedLicense.IsWebRequest  ||
									HttpContext.Current.Request.QueryString[ "downloadLicense" ] != "true" )
								{
									License._saveOnValid = true;
									//License.LicensePack.Save( true );
								}
								else
								{
									License.LicensePack.WriteToResponse( System.IO.Path.GetFileName( License.LicensePack.Location ) );
									HttpContext.Current.Response.End();
								}

								return true;
							}
							catch( UnauthorizedAccessException )
							{
								License.InvalidReason	= Internal.StaticResourceProvider.CurrentProvider.FormatResourceString( "E_ActivateNotSaved", 
									HttpContext.Current.Request.Url );
							}

						}
						else
						{
							License.InvalidReason = Internal.StaticResourceProvider.CurrentProvider.GetString( "E_InvalidActivationKey" );
						}
					}
				}
				else
				{
					//This is a "normal" request (not trying to activate) so just check that 
					// The current date is within the grace period
					if( GracePeriod > DateTime.UtcNow )
					{
						//Set this flag that will cause the activation banner to be displayed
						_brandOutput = true;
						return true;
					}
					else
					{
						License.InvalidReason = Internal.StaticResourceProvider.CurrentProvider.FormatResourceString( "E_NotActivated", License.SerialNumber );
						return false;
					}	
				}
			}

			return false;
		}

		/// <summary>
		/// Contacts the activation server, activates the license, and on success
		/// overwrites the license file with the new license.
		/// </summary>
		internal bool ActivateWithServer( Type type )
		{
            throw new Exception("Activation not supported by Checkbox.");

////			ExternalActivationServer.XheoLicensingActivationServer server = new ExternalActivationServer.XheoLicensingActivationServer();

//            ExternalActivationServer.PrezzaActivationServer server = new Xheo.Licensing.ExternalActivationServer.PrezzaActivationServer();
//            bool retry;

//            do
//            {
//                try
//                {
//                    string			licenseXml	= null;

//                    server.Url = Url;
//                    server.Proxy = ExtendedLicense.Proxy;
//                    server.LicenseCultureSoapHeaderValue = new Xheo.Licensing.ExternalActivationServer.LicenseCultureSoapHeader();
//                    server.LicenseCultureSoapHeaderValue.CultureName = System.Globalization.CultureInfo.CurrentUICulture.Name;
//                    server.LicenseCultureSoapHeaderValue.MustUnderstand = false;
//                    licenseXml = License.ToXmlString();

//                    _working = true;
//                    _cancelled = false;
//                    object state = new object();
//                    IAsyncResult handle = server.BeginActivate( licenseXml, type.Assembly.GetName().Name + ".lsk", MachineProfile.Profile.Hash, null, state );

//                    while( ! _cancelled && ! handle.AsyncWaitHandle.WaitOne( 0, false ) )
//                    {
//                        System.Threading.Thread.Sleep( 100 );
//                    }

//                    if( _cancelled )
//                        return false;

//                    licenseXml = server.EndActivate( handle );
				
//                    ExtendedLicensePack pack = License.LicensePack.Clone() as ExtendedLicensePack;
//                    ExtendedLicense license = pack[ pack.IndexOf( License.SerialNumber ) ];
//                    license.FromXmlString( licenseXml );
//                    if( ! ExtendedLicense.IsWebRequest  ||
//                        HttpContext.Current.Request.QueryString[ "downloadLicense" ] != "true" )
//                    {
//                        if( pack.SaveToShared )
//                        {
//                            string filename = System.IO.Path.Combine( ExtendedLicense.SharedFolder, System.IO.Path.GetFileNameWithoutExtension( License.LicensePack.Location ) );
//                            System.Reflection.AssemblyName name = License.LicensedType.Assembly.GetName();
//                            filename += "." + name.Name + "." + name.Version.Major.ToString() + "." + name.Version.Minor.ToString() + ".lic";
//                            pack.Save( filename, true );
//                        }
//                        else
//                            pack.Save( true );
//                        License.SurrogateLicensePack = pack.Location;
//                        return false;
//                    }
//                    else
//                    {
//                        pack.WriteToResponse( System.IO.Path.GetFileName( pack.Location ) );
//                        HttpContext.Current.Response.End();
//                    }
//                    return true;
//                }
//                catch( UnauthorizedAccessException ex )
//                {
//                    retry = ProxyForm.RetryException( null, ex, server.Url );
//                    if( ! retry )
//                    {
//                        if( ! ExtendedLicense.IsWebRequest )
//                        {
//                            License.InvalidReason	= Internal.StaticResourceProvider.CurrentProvider.FormatResourceString( "E_CouldNotActivate", 
//                                Url, FailureReportForm.PreProcessException( ex ) );
//                        }
//                        else
//                        {
//                            License.InvalidReason = Internal.StaticResourceProvider.CurrentProvider.FormatResourceString( "E_ActivateNotSaved", 
//                                HttpContext.Current.Request.Url );
//                        }
//                    }
//                }
//                catch( Exception ex )
//                {
//                    retry = ProxyForm.RetryException( null, ex, server.Url );

//                    if( ! retry )
//                    License.InvalidReason	= Internal.StaticResourceProvider.CurrentProvider.FormatResourceString( "E_CouldNotActivate", 
//                        Url, FailureReportForm.PreProcessException( ex ) );
//                }
//                finally
//                {
//                    _working = false;
//                }
//            } while( retry );

//            return false;
		}

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
			bool skip = false;
			DateTime started;

			//Check if the license has been previously activated by typing in an unlock key
			//	If it has, set skip = true
			if( CanActivateByKey && ActivationKey != null && UnlockHash != null && 
				License.PublicKey.ValidateActivationUnlockKey( UnlockHash, ActivationKey ) )
				skip = true;

			//Check if the license has been previously activated online
			//	(AND with the check above (skip))
			if( ( ProfileHash == null || ProfileHash.Length == 0 ) && ! skip )
			{

				//This license has not been previously activated...

				//If the grace period is using a relative duration, make sure it's set up properly
				if( ! _initDuration && GraceDuration != 0 )
					_initDuration = InitDuration( type, GraceDuration, out started, out _gracePeriod );

				//Attempt to activate the license
				if( ! Activate( type ) )
				{
					License.InvalidReason = Internal.StaticResourceProvider.CurrentProvider.GetString( "E_DidNotActivateOrContinue" );
					return false;
				}
				else
					return true;
			}

			//If the license has already been activated, check that it belongs to this machine
			MachineProfile profile = new MachineProfile();
			if( MachineProfile.CompareHash( ProfileHash, profile.GetComparableHash() ) > Tolerance &&
				( ( ActivationKey == null || UnlockHash == null ) || 
				( 
					MachineProfile.CompareHash( UnlockHash, profile.GetComparableHash() ) > Tolerance )					
				)
			)
			{
				//If this license does not belong to this machine, attempt to activate it
				if( ! _initDuration && GraceDuration != 0 )
					_initDuration = InitDuration( type, GraceDuration, out started, out _gracePeriod );

				if( ! Activate( type ) )
				{
					License.InvalidReason = Internal.StaticResourceProvider.CurrentProvider.FormatResourceString( "E_ProfileMismatch", License.LicensePack.Location );
					return false;
				}
				else
					return true;
			}

			return true;
		}


		/// <summary>
		/// Inserts the HTML into the page.
		/// </summary>
		/// 
		#region Original Xheo implementation

//		private void InsertHtml( object sender, EventArgs e )
//		{
//			System.Web.UI.Control control = sender as System.Web.UI.Control;
//			SupportInfo supportInfo = LicenseHelpAttribute.GetSupportInfo( License.LicensedType );
//
//			TimeSpan days = GracePeriod - DateTime.UtcNow;
//			string url = HttpContext.Current.Request.Url.ToString();
//
//			if( url.IndexOf( "activateLicense" ) == -1 )
//			{
//				if( url.IndexOf( '?' ) == -1 )
//					url += "?activateLicense=" + License.SerialNumber;
//				else
//					url += "&activateLicense=" + License.SerialNumber;
//			}
//
//
//			control.Page.RegisterClientScriptBlock( License.SerialNumber + "2",
//				Internal.StaticResourceProvider.CurrentProvider.GetString( "MAL_HeaderHtml" ) );
//			control.Page.RegisterStartupScript( License.SerialNumber,
//				Internal.StaticResourceProvider.CurrentProvider.FormatResourceString( "MAL_FooterHtml", 
//				Convert.ToInt32( days.TotalDays, System.Globalization.CultureInfo.CurrentUICulture ),
//				supportInfo.Url,
//				supportInfo.Email,
//				supportInfo.Phone,
//				url,
//				License.InvalidReason == null ? "" : ( "<p>" + License.InvalidReason + "</p>" ),
//				supportInfo.Product,
//				License.SignedByProLicense ? "<img src=\"\" width=176 height=1 />" : "<a href=\"http://www.xheo.com/products/licensing/\" target=\"_blank\"><IMG alt=\"Protected By XHEO|Licensing\" src=\"http://www.xheo.com/images/activationpb.gif\" border=\"0\"></a>",
//				supportInfo.Company,
//				LicenseSigningKey.MakeActivationKey( License ),
//				License.SerialNumber,
//				( SplashUrl == null || SplashUrl.Length == 0 ) ? "http://www.xheo.com/images/licensing/activationback.jpg" : SplashUrl
//				)
//				);
//		}

		#endregion
		private void InsertHtml( object sender, EventArgs e )
		{
			System.Web.UI.Control control = sender as System.Web.UI.Control;
//			Xheo.Licensing.SupportInfo supportInfo = Xheo.Licensing.LicenseHelpAttribute.GetSupportInfo( License.LicensedType );
//
//			TimeSpan days = GracePeriod - DateTime.UtcNow;
//			string url = HttpContext.Current.Request.Url.ToString();
//
//			if( url.IndexOf( "activateLicense" ) == -1 )
//			{
//				if( url.IndexOf( '?' ) == -1 )
//					url += "?activateLicense=" + License.SerialNumber;
//				else
//					url += "&activateLicense=" + License.SerialNumber;
//			}

			string activationBanner = String.Empty;
			activationBanner +=	"<TABLE cellSpacing=\"1\" cellPadding=\"1\" width=\"75%\" align=\"center\" border=\"0\" bgcolor=\"red\">";
			activationBanner += "<TR><TD><TABLE cellSpacing=\"1\" cellPadding=\"1\" width=\"100%\" align=\"center\" border=\"0\" bgcolor=\"white\">";
            activationBanner += "<tr><td class=\"ErrorMessage\" align=\"center\">This installation of Checkbox&reg; Survey Server must be activated ";
			//activationBanner += "<a href=\"" + activationPath + "\">Click here to activate</a></td>";
			//activationBanner += "<a href=\"#\" onClick=\"window.open('" + activationPath + "?popUp=true','activationWindow',";
			//activationBanner += "'width=800,height=400,resizable=yes,scrollbars=no,toolbar=no,location=no,directories=no,status=no,menubar=no,copyhistory=no')\"";
			//activationBanner += ">Click here to activate</a></td>";
			activationBanner += "</td>";
			activationBanner +=  "</tr></table></TD></TR></TABLE>";

            control.Page.ClientScript.RegisterClientScriptBlock(GetType(), License.SerialNumber + "2", activationBanner);
		}

		/// <summary>
		/// Called every time the limit is granted. Used to enforce limits that
		/// must be checked each time a Type is created. 
		/// </summary>
		/// <param name="context">
		///		The LicenseContext used to determine the environment and settings
		///		of the licensing.
		/// </param>
		/// <param name="instance">
		///		The instance of the object being licensed.
		/// </param>
		/// <param name="type">
		///		The Type of the object being licensed.
		/// </param>
		/// <remarks>
		/// Checks in this method should be very fast because they are called every time a 
		/// licensed object is created. The default implementation does nothing.
		/// </remarks>
		public override void Granted( LicenseContext context, Type type, object instance )
		{
			if( _brandOutput && ! IsActivated )
			{
				if( HttpContext.Current.Items[ License.SerialNumber + ".Activation" ] == null )
				{
					HttpContext.Current.Items[ License.SerialNumber + ".Activation" ] = "Once";
					if( HttpContext.Current.Request.QueryString[ "activateLicense" ] != null )
					{
						if( Activate( type ) )
							return;
						else
							throw new ApplicationException( "Activating." );
					}

                    /*
					if( ExtendedLicense.IsWebRequest )
					{
						Page control = HttpContext.Current.Handler as Page;
						if( control == null )
						{
							if( instance is Page )
								control = instance as Page;
							else if( instance is System.Web.UI.Control )
								control = ((System.Web.UI.Control)control).Page;
						}
						if( control != null )
							control.PreRender += new EventHandler( InsertHtml );
					}*/
				}
			}
		}

		/// <summary>
		/// Loads the contents of a Limit from an XML string.
		/// </summary>
		/// <param name="node">
		///		The XmlNode to load from.
		/// </param>
		/// <returns>
		///		Returns true if the load was successful, otherwise false.
		/// </returns>
		public override bool LoadFromXml( XmlNode node )
		{
			if( String.Compare( node.Attributes[ "type" ].Value, "Activation", false, System.Globalization.CultureInfo.InvariantCulture ) != 0 )
				return false;

			Url = node.Attributes[ "url" ].Value.Trim();

			if( License != null && License.Version < ExtendedLicense.v2_0 && ( Url == null || Url.Length == 0 ) )
				return false;

			if( node.Attributes[ "profileHash" ] != null )
				ProfileHash = node.Attributes[ "profileHash" ].Value.Trim();

			if( node.Attributes[ "tolerance" ] != null )
				Tolerance = XmlConvert.ToInt32( node.Attributes[ "tolerance" ].Value.Trim() );

			if( node.Attributes[ "gracePeriod" ] != null )
				GracePeriod = XmlConvert.ToDateTime( node.Attributes[ "gracePeriod" ].Value.Trim(), XmlDateTimeSerializationMode.Unspecified );

			if( node.Attributes[ "canActivateByKey" ] != null )
				CanActivateByKey = XmlConvert.ToBoolean( node.Attributes[ "canActivateByKey" ].Value.Trim() );

			if( node.Attributes[ "graceDuration" ] != null )
				GraceDuration = XmlConvert.ToInt32( node.Attributes[ "graceDuration" ].Value.Trim() );

			if( node.Attributes[ "splashResource" ] != null )
				SplashResource = node.Attributes[ "splashResource" ].Value.Trim();

			if( node.Attributes[ "splashUrl" ] != null )
				SplashUrl = node.Attributes[ "splashUrl" ].Value.Trim();

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
			attribute.Value = "Activation";
			limitNode.Attributes.Append( attribute );

			attribute = parent.OwnerDocument.CreateAttribute( "url" );
			attribute.Value = Url;
			limitNode.Attributes.Append( attribute );

			attribute = parent.OwnerDocument.CreateAttribute( "tolerance" );
			attribute.Value = XmlConvert.ToString( Tolerance );
			limitNode.Attributes.Append( attribute );

			if( ( License != null && License.Version < ExtendedLicense.v2_0 ) || ( ( License == null || License.Version >= ExtendedLicense.v2_0 ) && GraceDuration == 0 ) )
			{
				attribute = parent.OwnerDocument.CreateAttribute( "gracePeriod" );
				attribute.Value = GracePeriod.ToString( "s", System.Globalization.CultureInfo.InvariantCulture );
				limitNode.Attributes.Append( attribute );
			}

			attribute = parent.OwnerDocument.CreateAttribute( "profileHash" );
			attribute.Value = ProfileHash;
			limitNode.Attributes.Append( attribute );

			if( License == null || License.Version >= ExtendedLicense.v2_0 )
			{
				if( CanActivateByKey )
				{
					attribute = parent.OwnerDocument.CreateAttribute( "canActivateByKey" );
					attribute.Value = XmlConvert.ToString( CanActivateByKey );
					limitNode.Attributes.Append( attribute );
				}

				if( GraceDuration != 0 )
				{
					attribute = parent.OwnerDocument.CreateAttribute( "graceDuration" );
					attribute.Value = XmlConvert.ToString( GraceDuration );
					limitNode.Attributes.Append( attribute );					
				}

				if( SplashResource != null && SplashResource.Length > 0 )
				{
					attribute = parent.OwnerDocument.CreateAttribute( "splashResource" );
					attribute.Value = SplashResource;
					limitNode.Attributes.Append( attribute );					
				}

				if( SplashUrl != null && SplashUrl.Length > 0 )
				{
					attribute = parent.OwnerDocument.CreateAttribute( "splashUrl" );
					attribute.Value = SplashUrl;
					limitNode.Attributes.Append( attribute );					
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
			if( ! IsActivated )
				return "Unactivated";
			else
				return "Activated";
		}

		/// <summary>
		/// Aborts a pending asynchronous activation.
		/// </summary>
		public void Abort()
		{
			_cancelled = true;
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
	} // End class ActivationLimit
#endif
} // End namespace Xheo.Licensing

////////////////////////////////////////////////////////////////////////////////
#region Copyright © 2002-2005 XHEO, INC. All Rights Reserved.
//
#endregion
////////////////////////////////////////////////////////////////////////////////
