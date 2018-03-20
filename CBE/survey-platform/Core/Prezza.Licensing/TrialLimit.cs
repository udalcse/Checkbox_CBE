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
// Class:		TrialLimit
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
using System.Collections;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using Microsoft.Win32;

namespace Xheo.Licensing
{
	/// <summary>
	/// Marks a license as a trial license and limits the time they can use the
	/// product.
	/// </summary>
	/// <remarks>
	/// By including a trial limit, developers can determine allow end users to try
	/// their product before purchasing. Developers can also check the IsTrial property on the
	/// license returned from LicenseManger.Validate to see if the license is a trial 
	/// and alter code accordingly.
	/// <para>
	///		If the end date of the limit is more then 1 year from the started
	///		date, the license is not considered a timed trial.
	/// </para>
	/// <para>
	///		The expiration date of the license should be long after the end date of 
	///		the trial so that the GUI will still be displayed with only the buy option
	///		selected.
	/// </para>
	/// <p>
	///		See <see href="../Backgrounders/TrialVersions.html">Trial Versions</see> for
	///		more information how XHEO|Licensing supports trial versions.
	/// </p>
	/// <seealso href="../Backgrounders/TrialVersions.html">Trial Versions</seealso>
	/// </remarks>
	[Serializable]
#if LICENSING
	public 
#else
	internal
#endif
		class TrialLimit : Limit
	{
		////////////////////////////////////////////////////////////////////////////////
		#region Fields

		private string	_terms			= null;
		private string	_purchaseUrl	= null;
		private string	_infoUrl		= null;
		private bool	_useGui			= false;
		private string	_bitmapResource	= null;
		private string	_logoResource	= null;
		private	bool	_usePurchaseGui	= true;
		private int		_duration		= 0;
		private bool	_durationInit	= false;
		private DateTime	_started	= DateTime.Today;
		private DateTime	_ends		= DateTime.Today.AddDays( 30 );
		private bool	_shown			= false;
		private bool	_showRegisterIfAvailable	= true;

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Constructors/Destructors

		/// <summary>
		///	Initializes a new instance of the TrialLimit class.
		/// </summary>
		public TrialLimit()
		{
		}

		/// <summary>
		///	Initializes a new instance of the TrialLimit class.
		/// </summary>
		public TrialLimit( string terms )
		{
			_terms = terms;
		}

		/// <summary>
		///	Initializes a new instance of the TrialLimit class.
		/// </summary>
		public TrialLimit( string terms, string purchaseUrl )
		{
			_terms			= terms;
			_purchaseUrl	= purchaseUrl;
		}

		/// <summary>
		///	Initializes a new instance of the TrialLimit class.
		/// </summary>
		public TrialLimit( string terms, string purchaseUrl, bool useGui, string infoUrl, DateTime started, DateTime ends, string bitmapResource, string logoResource )
		{
			Terms			= terms;
			PurchaseUrl		= purchaseUrl;
			UseGui			= useGui;
			InfoUrl			= infoUrl;
			Started			= started;
			Ends			= ends;
			BitmapResource	= bitmapResource;
			LogoResource	= logoResource;
		}

		/// <summary>
		///	Initializes a new instance of the TrialLimit class.
		/// </summary>
		public TrialLimit( string terms, string purchaseUrl, bool useGui, string infoUrl, DateTime started, DateTime ends )
		{
			Terms			= terms;
			PurchaseUrl		= purchaseUrl;
			UseGui			= useGui;
			InfoUrl			= infoUrl;
			Started			= started;
			Ends			= ends;
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Properties

		/// <summary>
		/// Gets the terms of the trial. If the terms contains a URL, the page will be
		/// displayed.
		/// </summary>
#if LICENSING
		[
			Category( "Limits" ),
			Description( "The terms of the trial. If the terms contains a URL, the page will be displayed." )
		]
#endif
		public string Terms
		{
			get
			{
				return _terms;
			}
			set
			{
				AssertNotReadOnly();
				_terms = value;
			}
		}

		/// <summary>
		/// Gets the URL where the full version can be purchased. The license 
		/// <see cref="ExtendedLicense.SerialNumber"/> and a <see cref="MachineProfile"/> hash are
		/// appended to the end of the url automatically.
		/// </summary>
#if LICENSING		
		[
			Category( "Limits" ),
			Description( "The URL where the full version can be purchased. The license SerialNumber and a MachineProfile hash are appended to the end of the url automatically." )
		]
#endif
		public string PurchaseUrl
		{
			get
			{
				return _purchaseUrl;
			}
			set
			{
				AssertNotReadOnly();
				_purchaseUrl = value;
			}
		}

		/// <summary>
		/// Gets or sets the URL where the more information can be found.
		/// </summary>
#if LICENSING	
		[
			Category( "Limits" ),
			Description( "The URL where more information can be found." )
		]
#endif
		public string InfoUrl
		{
			get
			{
				return _infoUrl;
			}
			set
			{
				AssertNotReadOnly();
				_infoUrl = value;
			}
		}

		/// <summary>
		/// Gets or sets a value that indicates if a trial form should be displayed.
		/// </summary>
#if LICENSING	
		[
			Category( "Limits" ),
			Description( "Indicates if a trial form should be displayed." ),
			DefaultValue( false )
		]
#endif
		public bool UseGui
		{
			get
			{
				return _useGui;
			}
			set
			{
				AssertNotReadOnly();
				_useGui = value;
			}
		}

		/// <summary>
		/// Gets or sets a value that indicates if the <see cref="BuyNowForm"/> should be
		/// used to purchase the a license, or if a new browser window should be opened
		/// to the purchase url instead.
		/// </summary>
#if LICENSING
		[
			Category( "Limits" ),
			Description( "Indicates if the BuyNowForm should be used to purchase the a license, or if a new browser window should be opened to the purchase url instead." ),
			DefaultValue( true )
		]
#endif
		public bool UsePurchaseGui
		{
			get
			{
				return _usePurchaseGui;
			}
			set
			{
				AssertNotReadOnly();
				_usePurchaseGui = value;
			}
		}

		/// <summary>
		/// Gets or sets the date when the trial was started.
		/// </summary>
#if LICENSING
		[
			Category( "Limits" ),
			Description( "Date when the trial started." )
		]
#endif
		public DateTime Started
		{
			get
			{
				return _started;
			}
			set
			{
				AssertNotReadOnly();
				_started = value;
			}
		}

		/// <summary>
		/// Gets or sets the date when the trial ends.
		/// </summary>
#if LICENSING
		[
		Category( "Limits" ),
		Description( "Date when the trial ends." )
		]
#endif
		public DateTime Ends
		{
			get
			{
				return _ends;
			}
			set
			{
				AssertNotReadOnly();
				_ends = value;
			}
		}

		/// <summary>
		/// Gets or sets the duration of the trial in days.
		/// <seealso href="../Backgrounders/TrialVersions.html">Trial Versions</seealso>
		/// </summary>
#if LICENSING		
		[
		Category( "Limits" ),
		Description( "Duration of the trial in days." ),
		DefaultValue( 0 )
		]
#endif
		public int Duration
		{
			get
			{
				return _duration;
			}
			set
			{
				_duration = value;
			}
		}

		/// <summary>
		/// Gets or sets the assembly qualified name of a resource to display in the bitmap
		/// area of the form. Should be 456 x 280. You can also specify a remote JPEG or GIF
		/// image by using a fully qualified URL.
		/// </summary>
		/// <remarks>
		/// Only URLs beginning with http or https can be used.
		/// </remarks>
#if LICENSING
		[
		Category( "Limits" ),
		Description( "The assembly qualified name of the bitmap resource. You can also specify a remote JPEG or GIF image by using a fully qualified URL. Example: MyComponent.bmp,MyAssembly or http://www.xheo.com/images/licensing/webskin.jpg." )
		]
#endif
		public string BitmapResource
		{
			get
			{
				return _bitmapResource;
			}
			set
			{
				AssertNotReadOnly();
				string vl = value == null ? null : value.ToLower( System.Globalization.CultureInfo.InvariantCulture );
				if( value != null && ( vl.StartsWith( "http://" ) || vl.StartsWith( "https://" ) || vl.StartsWith( "file://" ) ) )
					_bitmapResource = new Uri( value ).ToString();
				else
					_bitmapResource = value;
			}
		}

		/// <summary>
		/// Gets or sets the assembly qualified name of a resource to display in the logo 
		/// area of the form. Should be 88x88.  You can also specify a remote JPEG or GIF
		/// image by using a fully qualified URL.
		/// </summary>
		/// <remarks>
		/// Only URLs beginning with http or https can be used.
		/// </remarks>
#if LICENSING
		[
		Category( "Limits" ),
		Description( "The assembly qualified name of the logo resource. You can also specify a remote JPEG or GIF image by using a fully qualified URL. Example: MyComponent.bmp,MyAssembly or http://www.xheo.com/images/licensing/webskin.jpg." )
		]
#endif
		public string LogoResource
		{
			get
			{
				return _logoResource;
			}
			set
			{
				AssertNotReadOnly();
				string vl = value == null ? null : value.ToLower( System.Globalization.CultureInfo.InvariantCulture );
				if( value != null && ( vl.StartsWith( "http://" ) || vl.StartsWith( "https://" ) || vl.StartsWith( "file://" ) ) )
					_logoResource = new Uri( value ).ToString();
				else
					_logoResource = value;
			}
		}

		/// <summary>
		/// Gets or sets a value that indicates if a Register button should be
		/// displayed when a registration limit is available in the same license.
		/// </summary>
#if LICENSING
		[
			Category( "Limits" ),
			Description( "Indicates if a register button should be displayed if the license also contains a registration limit." ),
			DefaultValue( true )
		]
#endif
		public bool ShowRegisterIfAvailable
		{
			get
			{
				return _showRegisterIfAvailable;
			}
			set
			{
				AssertNotReadOnly();
				_showRegisterIfAvailable = value;
			}
		}


		/// <summary>
		/// Gets the description of the limit.
		/// </summary>
		public override string Description
		{
			get
			{
				return Internal.StaticResourceProvider.CurrentProvider.GetString( "MTL_Description" );
			}
		}

		/// <summary>
		/// Gets the display name of the limit.
		/// </summary>
		public override string Name
		{
			get
			{
				return "Trial";
			}
		}

		/// <summary>
		/// Gets a value that indicates if a GUI is displayed to interact with the user.
		/// </summary>
		public override bool IsGui
		{
			get
			{
				return _useGui;
			}
		}

		/// <summary>
		/// Gets a value that indicates if the GUI was shown.
		/// </summary>
		[
			Browsable( false ),
			EditorBrowsable( EditorBrowsableState.Advanced ),
		]
		public bool GuiShown
		{
			get
			{
				return _shown;
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
					return "showRegisterIfAvailable";
				else
					return null;
			}
		}

		/// <summary>
		/// Gets a value that indicates if the beta limit is time sensitive.
		/// </summary>
		public override bool IsTimeSensitive
		{
			get
			{
				return true;
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
			// Load duration settings
			if( ! _durationInit && Duration != 0 )
				_durationInit = InitDuration( type, Duration, out _started, out _ends );

			if( UseGui && ! ExtendedLicense.IsWebRequest )
			{
				if( GuiShown )
					return true;
				using( TrialLimitForm form = new TrialLimitForm( this ) )
				{
					System.Drawing.Image img = Limit.GetBitmapResource( BitmapResource, this );
					if( img != null )
						form.Bitmap = img;

					img = Limit.GetBitmapResource( LogoResource, this );
					if( img != null )
						form.Logo = img;

					if( context is ExtendedLicenseContext )
						((ExtendedLicenseContext)context).IsTrialShown = true;
					if( form.ShowDialog() == DialogResult.OK )
					{
						_shown = true;
						return true;
					}
					else
					{
						License.InvalidReason = Internal.StaticResourceProvider.CurrentProvider.GetString( "E_DidNotAcceptTrial" );
						return false;
					}
				}

			}
			else
			{
				if( License.Version >= ExtendedLicense.v1_1 )
				{
					if( context is ExtendedLicenseContext )
						((ExtendedLicenseContext)context).IsTrialShown = true;

					if( DateTime.UtcNow > Ends )
					{
						License.InvalidReason = Internal.StaticResourceProvider.CurrentProvider.FormatResourceString( "E_TrialExpiredVisit", PurchaseUrl );
						return false;
					}
					else
					{
						_shown = true;
						return true;
					}
				}
			}

			return DateTime.UtcNow <= Ends;
		}
		#region Helper Methods
		#endregion 

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
			if( ! BaseLoadFromXml( node, "Trial" ) )
				return false;

			XmlNamespaceManager nm = new XmlNamespaceManager( node.OwnerDocument.NameTable );

			if( node.Attributes[ "terms" ] == null )
				_terms = "N/A";
			else
				_terms = node.Attributes[ "terms" ].Value;

			if( node.Attributes[ "purchaseUrl" ] != null )
				_purchaseUrl = node.Attributes[ "purchaseUrl" ].Value;
			else
				_purchaseUrl = null;

			if( License == null || License.Version >= ExtendedLicense.v1_1 )
			{
				if( node.Attributes[ "infoUrl" ] != null )
					_infoUrl = node.Attributes[ "infoUrl" ].Value;
				else
					_infoUrl = null;

				if( node.Attributes[ "bitmapResource" ] != null )
					_bitmapResource = node.Attributes[ "bitmapResource" ].Value;
				else
					_bitmapResource = null;

				if( node.Attributes[ "logoResource" ] != null )
					_logoResource = node.Attributes[ "logoResource" ].Value;
				else
					_logoResource = null;

				if( node.Attributes[ "started" ] != null )
					_started = XmlConvert.ToDateTime( node.Attributes[ "started" ].Value, XmlDateTimeSerializationMode.Unspecified );
				else
					_started = DateTime.MinValue;

				if( node.Attributes[ "ends" ] != null )
					_ends = XmlConvert.ToDateTime( node.Attributes[ "ends" ].Value, XmlDateTimeSerializationMode.Unspecified );
				else
					_ends = _started;

				if( node.Attributes[ "useGui" ] != null )
					_useGui = XmlConvert.ToBoolean( node.Attributes[ "useGui" ].Value );
				else
					_useGui = false;

				if( node.Attributes[ "usePurchaseGui" ] != null )
					_usePurchaseGui = XmlConvert.ToBoolean( node.Attributes[ "usePurchaseGui" ].Value );
				else
					_usePurchaseGui = true;

				if( node.Attributes[ "duration" ] != null )
					Duration = XmlConvert.ToInt32( node.Attributes[ "duration" ].Value );

				if( node.Attributes[ "showRegisterIfAvailable" ] != null )
					_showRegisterIfAvailable = XmlConvert.ToBoolean( node.Attributes[ "showRegisterIfAvailable" ].Value );
				else
					_showRegisterIfAvailable = true;
			}

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
			attribute.Value = "Trial";
			limitNode.Attributes.Append( attribute );
			Version v1_1 = ExtendedLicense.v1_1;

			attribute = parent.OwnerDocument.CreateAttribute( "terms" );
			attribute.Value = Terms;
			limitNode.Attributes.Append( attribute );

			if( ( License != null && License.Version < v1_1 ) || 
				_purchaseUrl != null )
			{
				attribute = parent.OwnerDocument.CreateAttribute( "purchaseUrl" );
				attribute.Value = _purchaseUrl.ToString();
				limitNode.Attributes.Append( attribute );
			}

			if( License == null || License.Version >= v1_1 )
			{

				if( _infoUrl != null )
				{
					attribute = parent.OwnerDocument.CreateAttribute( "infoUrl" );
					attribute.Value = _infoUrl.ToString();
					limitNode.Attributes.Append( attribute );
				}

				if( _bitmapResource != null && _bitmapResource.Length > 0 )
				{
					attribute = parent.OwnerDocument.CreateAttribute( "bitmapResource" );
					attribute.Value = _bitmapResource;
					limitNode.Attributes.Append( attribute );
				}

				if( _logoResource != null && _logoResource.Length > 0 )
				{
					attribute = parent.OwnerDocument.CreateAttribute( "logoResource" );
					attribute.Value = _logoResource;
					limitNode.Attributes.Append( attribute );
				}

				if( _duration == 0 )
				{
					attribute = parent.OwnerDocument.CreateAttribute( "started" );
					attribute.Value = _started.ToString( "s", System.Globalization.CultureInfo.InvariantCulture );
					limitNode.Attributes.Append( attribute );

					attribute = parent.OwnerDocument.CreateAttribute( "ends" );
					attribute.Value = _ends.ToString( "s", System.Globalization.CultureInfo.InvariantCulture );
					limitNode.Attributes.Append( attribute );
				}
			
				if( _useGui )
				{
					attribute = parent.OwnerDocument.CreateAttribute( "useGui" );
					attribute.Value = XmlConvert.ToString( _useGui );
					limitNode.Attributes.Append( attribute );
				}

				if( ! _usePurchaseGui )
				{
					attribute = parent.OwnerDocument.CreateAttribute( "usePurchaseGui" );
					attribute.Value = XmlConvert.ToString( _usePurchaseGui );
					limitNode.Attributes.Append( attribute );
				}

				if( _duration != 0 )
				{
					attribute = parent.OwnerDocument.CreateAttribute( "duration" );
					attribute.Value = XmlConvert.ToString( _duration );
					limitNode.Attributes.Append( attribute );
				}

				if( ! _showRegisterIfAvailable )
				{
					attribute = parent.OwnerDocument.CreateAttribute( "showRegisterIfAvailable" );
					attribute.Value = XmlConvert.ToString( _showRegisterIfAvailable );
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
			return Internal.StaticResourceProvider.CurrentProvider.FormatResourceString( "MTL_DetailedDescription", _terms, _purchaseUrl );
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
	} // End class TrialLimit
} // End namespace Xheo.Licensing

////////////////////////////////////////////////////////////////////////////////
#region Copyright © 2002-2005 XHEO, INC. All Rights Reserved.
//
#endregion
////////////////////////////////////////////////////////////////////////////////
