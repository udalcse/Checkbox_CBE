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
// Class:		BetaLimit
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

namespace Xheo.Licensing
{
	/// <summary>
	/// Indicates that the licensed assembly is in BETA.
	/// </summary>
	[Serializable]
#if LICENSING
	public 
#else
	internal
#endif
		class BetaLimit : Limit
	{
		////////////////////////////////////////////////////////////////////////////////
		#region Fields

		private string			_updateUrl			= null;
		private DateTime	_expires			= DateTime.Today.AddDays( 30 );
		private bool		_browseUpdate		= true;
		private bool		_useGui				= true;

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Constructors/Destructors

		/// <summary>
		///	Initializes a new instance of the BetaLimit class.
		/// </summary>
		public BetaLimit()
		{
		}

		/// <summary>
		///	Initializes a new instance of the BetaLimit class.
		/// </summary>
		/// <param name="updateUrl">
		///		URL where the latest version can be downloaded.
		/// </param>
		/// <param name="expires">
		///		Date when the beta expires.
		/// </param>
		public BetaLimit( string updateUrl, DateTime expires )
		{
			_updateUrl		= updateUrl;
			_expires		= expires;
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Properties

		/// <summary>
		/// Gets or sets the name of the Beta that the limit applies to.
		/// </summary>
#if LICENSING
		[
			Category( "Limits" ),
			Description( "URL where latest version can be downloaded." ),
		]
#endif
		public string UpdateUrl
		{
			get
			{
				return _updateUrl;
			}
			set
			{
				AssertNotReadOnly();
				_updateUrl = value;
			}
		}

		
		/// <summary>
		/// Gets or sets the date when the BETA expires.
		/// </summary>
#if LICENSING	
		[
			Category( "Limits" ),
			Description( "Date when the beta expires." )
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
		/// Gets or sets a value that indicates if the notification window should browse directly to 
		/// the update url or simply display a notice.
		/// </summary>
#if LICENSING		
		[
			Category( "Limits" ),
			Description( "Indicates if the notification window should browse directly to the update url or simply display a notice." ),
			DefaultValue( true ),
		]
#endif
		public bool BrowseUpdate
		{
			get
			{
				return _browseUpdate;
			}
			set
			{
				_browseUpdate = value;
			}			
		}

		/// <summary>
		/// Gets or sets a value that indicates if the user will be presented with a notice about the
		/// beta expiring or if the license will just fail.
		/// </summary>
#if LICENSING		
		[
			Category( "Limits" ),
			Description( "Indicates if the user will be presented with a notice about the beta expiring or if the license will just fail." ),
			DefaultValue( true ),
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
				_useGui = value;
			}
		}

		/// <summary>
		/// Gets the description of the limit.
		/// </summary>
		public override string Description
		{
			get
			{
				return Internal.StaticResourceProvider.CurrentProvider.GetString( "MBL_Description" );
			}
		}

		/// <summary>
		/// Gets the display name of the limit.
		/// </summary>
		public override string Name
		{
			get
			{
				return "Beta";
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
			if( DateTime.UtcNow < _expires )
				return true;

			if( ! ExtendedLicense.IsWebRequest && _useGui )
			{
				if( _browseUpdate )
				{
					using( BetaForm form = new BetaForm( this ) )
						form.ShowDialog();
				}
				else
				{
					using( InfoForm form = new InfoForm( Internal.StaticResourceProvider.CurrentProvider.FormatResourceString( "E_BetaExpired", _updateUrl ), _updateUrl ) )
						form.ShowDialog();
				}
			}

			License.InvalidReason = Internal.StaticResourceProvider.CurrentProvider.FormatResourceString( "E_BetaExpired", _updateUrl );
			return false;

		}


		/// <summary>
		/// Determines if the beta has expired yet.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="type"></param>
		/// <param name="instance"></param>
		/// <returns></returns>
		public override PeekResult Peek( LicenseContext context, Type type, object instance )
		{
			return DateTime.UtcNow < _expires ? PeekResult.Valid : ( _useGui ? PeekResult.NeedsUser : PeekResult.Invalid );
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
			if( ! BaseLoadFromXml( node, "Beta" ) )
				return false;

			if( node.Attributes[ "updateUrl" ] == null )
				return false;

			_updateUrl = node.Attributes[ "updateUrl" ].Value.Trim();

			if( node.Attributes[ "expires" ] != null )
				_expires = XmlConvert.ToDateTime( node.Attributes[ "expires" ].Value, XmlDateTimeSerializationMode.Unspecified );

			if( node.Attributes[ "browseUpdate" ] != null )
				_browseUpdate = XmlConvert.ToBoolean( node.Attributes[ "browseUpdate" ].Value );

			if( node.Attributes[ "useGui" ] != null )
				_useGui = XmlConvert.ToBoolean( node.Attributes[ "useGui" ].Value );

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
			XmlNode limitNode = BaseSaveToXml( parent, "Beta" );
			XmlAttribute attribute = null;

			attribute = parent.OwnerDocument.CreateAttribute( "updateUrl" );
			attribute.Value = _updateUrl == null ? null : _updateUrl.ToString();
			limitNode.Attributes.Append( attribute );

			attribute = parent.OwnerDocument.CreateAttribute( "expires" );
			attribute.Value = _expires.ToString( "s", System.Globalization.CultureInfo.InvariantCulture );
			limitNode.Attributes.Append( attribute );

			if( ! _browseUpdate )
			{
				attribute = parent.OwnerDocument.CreateAttribute( "browseUpdate" );
				attribute.Value = XmlConvert.ToString( _browseUpdate );
				limitNode.Attributes.Append( attribute );
			}

			if( ! _useGui )
			{
				attribute = parent.OwnerDocument.CreateAttribute( "useGui" );
				attribute.Value = XmlConvert.ToString( _useGui );
				limitNode.Attributes.Append( attribute );
			}

			return limitNode;
		}

		/// <summary>
		/// Overrides <see cref="Limit.GetDetailsDescription"/>.
		/// </summary>
		public override string GetDetailsDescription()
		{
			return Internal.StaticResourceProvider.CurrentProvider.FormatResourceString( "MBL_DetailedDescription", _expires, _updateUrl );
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
	} // End class BetaLimit
} // End namespace Xheo.Licensing

////////////////////////////////////////////////////////////////////////////////
#region Copyright © 2002-2005 XHEO, INC. All Rights Reserved.
//
#endregion
////////////////////////////////////////////////////////////////////////////////
