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
// Class:		GuiBrandedLimit
// Author:		Paul Alexander
// Created:		Sunday, November 17, 2002 6:15:48 PM
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
using System.Drawing;
using System.IO;


namespace Xheo.Licensing
{
	/// <summary>
	/// Displays a "nag screen" before the license is granted. 
	/// </summary>
	/// <remarks>
	/// If a license with a GUI limit is used from a web application, it will still pause 
	/// the request for the time given in <see cref="ContinueDelay"/> on the first
	/// request, but will not show the form.
	/// </remarks>
#if LICENSING
	public 
#else
	internal
#endif
		class GuiBrandedLimit : Limit
	{
		////////////////////////////////////////////////////////////////////////////////
		#region Fields

		private string	_terms				= null;
		private string	_purchaseUrl		= null;
		private int		_continueDelay		= 0;
		private string	_bitmapResource		= null;
		private bool	_usePurchaseGui		= false;
		private bool	_shown				= false;
		
		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Constructors/Destructors

		/// <summary>
		///	Initializes a new instance of the GuiBrandedLimit class.
		/// </summary>
		public GuiBrandedLimit()
		{
		}

		/// <summary>
		///	Initializes a new instance of the GuiBrandedLimit class.
		/// </summary>
		/// <param name="terms">
		///		Description of the terms of the license.
		/// </param>
		public GuiBrandedLimit( string terms )
		{
			_terms = terms;
		}

		/// <summary>
		///	Initializes a new instance of the GuiBrandedLimit class.
		/// </summary>
		/// <param name="purchaseUrl">
		///		URL of the website where the user can purchase a full license.
		/// </param>
		/// <param name="terms">
		///		Description of the terms of the license.
		/// </param>
		public GuiBrandedLimit( string terms, string purchaseUrl )
		{
			_terms			= terms;
			_purchaseUrl	= new Uri( purchaseUrl ).ToString();
		}

		/// <summary>
		///	Initializes a new instance of the GuiBrandedLimit class.
		/// </summary>
		/// <param name="purchaseUrl">
		///		URL of the website where the user can purchase a full license.
		/// </param>
		/// <param name="terms">
		///		Description of the terms of the license.
		/// </param>
		/// <param name="usePurchaseGui">
		///		Indicates if the <see cref="BuyNowForm"/> should be used to purchase and
		///		unlock the license on demand.
		/// </param>
		public GuiBrandedLimit( string terms, string purchaseUrl, bool usePurchaseGui )
		{
			_terms			= terms;
			_purchaseUrl	= new Uri( purchaseUrl ).ToString();
			_usePurchaseGui = usePurchaseGui;
		}

		/// <summary>
		///	Initializes a new instance of the GuiBrandedLimit class.
		/// </summary>
		/// <param name="purchaseUrl">
		///		URL of the website where the user can purchase a full license.
		/// </param>
		/// <param name="terms">
		///		Description of the terms of the license.
		/// </param>
		public GuiBrandedLimit( string terms, Uri purchaseUrl )
		{
			_terms			= terms;
			_purchaseUrl	= purchaseUrl.ToString();
		}

		/// <summary>
		///	Initializes a new instance of the GuiBrandedLimit class.
		/// </summary>
		/// <param name="purchaseUrl">
		///		URL of the website where the user can purchase a full license.
		/// </param>
		/// <param name="terms">
		///		Description of the terms of the license.
		/// </param>
		/// <param name="usePurchaseGui">
		///		Indicates if the <see cref="BuyNowForm"/> should be used to purchase and
		///		unlock the license on demand.
		/// </param>
		public GuiBrandedLimit( string terms, Uri purchaseUrl, bool usePurchaseGui )
		{
			_terms			= terms;
			_purchaseUrl	= purchaseUrl.ToString();
			_usePurchaseGui = usePurchaseGui;
		}


		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Properties

		/// <summary>
		/// Gets the terms of the GuiBranded. If the terms contains a URL, the page will be
		/// displayed.
		/// </summary>
		[
			Category( "Limits" ),
			Description( "The terms of the GuiBranded. If the terms contains a URL, the page will be displayed." )
		]
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
		[
		Category( "Limits" ),
		Description( "The URL where the full version can be purchased. The license SerialNumber and a MachineProfile hash are appended to the end of the url automatically." )
		]
		public string PurchaseUrl
		{
			get
			{
				return _purchaseUrl;
			}
			set
			{
				AssertNotReadOnly();
				if( value == null || value.Length == 0 )
					_purchaseUrl = null;
				else
					_purchaseUrl = new Uri( value ).ToString();
			}
		}

		/// <summary>
		/// Gets or sets the time in seconds that the continue button should initially be disabled.
		/// </summary>
		[
			Category( "Limits" ),
			Description( "The time in seconds that the continue button should initially be disabled." ),
			DefaultValue( 0 )
		]
		public int ContinueDelay
		{
			get
			{
				return _continueDelay;
			}
			set
			{
				AssertNotReadOnly();
				_continueDelay = value;
			}
		}

		/// <summary>
		/// Gets or sets the assembly qualified name of a resource to display in the bitmap
		/// area of the form. Should be 456 x 270. You can also specify a remote JPEG or GIF
		/// image by using a fully qualified URL.
		/// </summary>
		/// <remarks>
		/// Only URLs beginning with http://, https:// or file:// can be used.
		/// </remarks>
		[
			Category( "Limits" ),
			Description( "The assembly qualified name of the bitmap resource. Example: MyComponent.bmp,MyAssembly." )
		]
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
		/// Gets or sets a value that indicates if the <see cref="BuyNowForm"/> should be
		/// used to purchase the a license, or if a new browser window should be opened
		/// to the <see cref="PurchaseUrl"/> instead.
		/// </summary>
		[
			Category( "Limits" ),
			Description( "Indicates if the BuyNowForm should be used to purchase the a license, or if a new browser window should be opened to the PurchaseUrl instead." ),
			DefaultValue( false )
		]
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
		/// Gets the description of the limit.
		/// </summary>
		public override string Description
		{
			get
			{
				return Internal.StaticResourceProvider.CurrentProvider.GetString( "MGBL_Description" );
			}
		}

		/// <summary>
		/// Gets the display name of the limit.
		/// </summary>
		public override string Name
		{
			get
			{
				return "GUI Branded";
			}
		}

		/// <summary>
		/// Gets a value that indicates if a GUI is displayed to interact with the user.
		/// </summary>
		public override bool IsGui
		{
			get
			{
				return true;
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
			if( _shown )
				return true;

			if( ExtendedLicense.IsWebRequest )
			{
				if( ! _shown )
				{
					_shown = true;
					System.Threading.Thread.Sleep( ContinueDelay * 1000 );
					return true;
				}
			}
			else
			{
				Xheo.Licensing.GuiBrandedLimitForm form = new Xheo.Licensing.GuiBrandedLimitForm( this );
				SupportInfo info = LicenseHelpAttribute.GetSupportInfo( type );
				form.Text = info.Product + " - Licensing";
				form.Bitmap = Limit.GetBitmapResource( BitmapResource, this );
				if( form.ShowDialog() == DialogResult.OK )
				{
					_shown = true;
					return true;
				}
			}

            License.InvalidReason = Internal.StaticResourceProvider.CurrentProvider.GetString( "E_DidNotContinue" );
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
			if( String.Compare( node.Attributes[ "type" ].Value, "GUIBranded", false, System.Globalization.CultureInfo.InvariantCulture ) != 0 )
				return false;

			XmlNamespaceManager nm = new XmlNamespaceManager( node.OwnerDocument.NameTable );

			if( node.Attributes[ "terms" ] == null )
				Terms = "N/A";
			else
				Terms = node.Attributes[ "terms" ].Value;

			if( node.Attributes[ "purchaseUrl" ] != null )
				PurchaseUrl = node.Attributes[ "purchaseUrl" ].Value;

			if( node.Attributes[ "continueDelay" ] != null )
				ContinueDelay = XmlConvert.ToInt32( node.Attributes[ "continueDelay" ].Value );

			if( node.Attributes[ "bitmapResource" ] != null )
				BitmapResource = node.Attributes[ "bitmapResource" ].Value;

			if( License == null || License.Version >= ExtendedLicense.v1_1 )
			{
				if( node.Attributes[ "usePurchaseGui" ] != null )
					UsePurchaseGui = XmlConvert.ToBoolean( node.Attributes[ "usePurchaseGui" ].Value );
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
			Version v1_1 = ExtendedLicense.v1_1;

			XmlNode limitNode = parent.OwnerDocument.CreateElement( "Limit" );
			XmlAttribute attribute = parent.OwnerDocument.CreateAttribute( "type" );
			attribute.Value = "GUIBranded";
			limitNode.Attributes.Append( attribute );


			attribute = parent.OwnerDocument.CreateAttribute( "terms" );
			attribute.Value = Terms;
			limitNode.Attributes.Append( attribute );

			attribute = parent.OwnerDocument.CreateAttribute( "continueDelay" );
			attribute.Value = XmlConvert.ToString( ContinueDelay );
			limitNode.Attributes.Append( attribute );

			if( ( License != null && License.Version < v1_1 ) ||
				( BitmapResource != null && BitmapResource.Length > 0 ) )
			{
				attribute = parent.OwnerDocument.CreateAttribute( "bitmapResource" );
				attribute.Value = BitmapResource;
				limitNode.Attributes.Append( attribute );
			}

			if( ( License != null && License.Version < v1_1 ) ||
				( PurchaseUrl != null && PurchaseUrl.Length > 0 ) )
			{
				attribute = parent.OwnerDocument.CreateAttribute( "purchaseUrl" );
				attribute.Value = PurchaseUrl;
				limitNode.Attributes.Append( attribute );
			}

			if( License == null || License.Version >= ExtendedLicense.v1_1 )
			{
				if( UsePurchaseGui )
				{
					attribute = parent.OwnerDocument.CreateAttribute( "usePurchaseGui" );
					attribute.Value = XmlConvert.ToString( UsePurchaseGui );
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
			return Internal.StaticResourceProvider.CurrentProvider.FormatResourceString( "MGBL_DetailedDescription", Terms, PurchaseUrl );
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
	} // End class GuiBrandedLimit
} // End namespace Xheo.Licensing

////////////////////////////////////////////////////////////////////////////////
#region Copyright © 2002-2005 XHEO, INC. All Rights Reserved.
//
#endregion
////////////////////////////////////////////////////////////////////////////////
