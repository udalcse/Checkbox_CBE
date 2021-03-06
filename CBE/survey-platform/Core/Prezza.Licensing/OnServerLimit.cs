////////////////////////////////////////////////////////////////////////////////
#region Copyright � 2002-2005 XHEO, INC. All Rights Reserved.
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
// Class:		OnServerLimit
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
	/// Indicates whether the protected class can be used from a web server/web service
	/// and if it is required (not allowed to run on web forms).
	/// </summary>
#if LICENSING
	public 
#else
	internal
#endif
		class OnServerLimit : Limit
	{
		////////////////////////////////////////////////////////////////////////////////
		#region Fields

		private bool	_allowed		= true;
		private bool	_required		= false;

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Constructors/Destructors

		/// <summary>
		///	Initializes a new instance of the OnServerLimit class.
		/// </summary>
		public OnServerLimit()
		{
		}

		/// <summary>
		///	Initializes a new instance of the OnServerLimit class.
		/// </summary>
		/// <param name="allowed">
		///		Indicates if running on a server is allowed.
		/// </param>
		/// <param name="required">
		///		Indicats if running on a server is required.
		/// </param>
		public OnServerLimit( bool allowed, bool required )
		{
			_allowed	= allowed;
			_required	= required;
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Properties

		/// <summary>
		/// Gets or sets a value that indicates if the class can be used on a web server
		/// or web service.
		/// </summary>
#if LICENSING
		[
			Category( "Limits" ),
			Description( "Indicates if the class can be used on a web server or web service." ),
			DefaultValue( true )
		]
#endif
		public bool Allowed
		{
			get
			{
				return _allowed;
			}
			set
			{
				AssertNotReadOnly();
				_allowed = value;
			}
		}

		/// <summary>
		/// Gets or sets a value that indicates if the class must be used on a web server
		/// or web service.
		/// </summary>
#if LICENSING
		[
			Category( "Limits" ),
			Description( "Indicates if the class must be used on a web server or web service." ),
			DefaultValue( false )
		]
#endif
		public bool Required
		{
			get
			{
				return _required;
			}
			set
			{
				AssertNotReadOnly();
				_required = value;
			}
		}


		/// <summary>
		/// Gets the description of the limit.
		/// </summary>
		public override string Description
		{
			get
			{
				return Internal.StaticResourceProvider.CurrentProvider.GetString( "MASL_Description" );
			}
		}

		/// <summary>
		/// Gets the display name of the limit.
		/// </summary>
		public override string Name
		{
			get
			{
				return "OnServer";
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
			if( ExtendedLicense.IsWebRequest )
			{
				if( ! _allowed )
				{
					License.InvalidReason = Internal.StaticResourceProvider.CurrentProvider.FormatResourceString( "E_NotAllowedOnServer" );
					return false;
				}
			}
			else
			{
				if( _required )
				{
					License.InvalidReason = Internal.StaticResourceProvider.CurrentProvider.FormatResourceString( "E_MustBeOnServer" );
					return false;
				}
			}

			return true;
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
			if( String.Compare( node.Attributes[ "type" ].Value, "OnServer", false, System.Globalization.CultureInfo.InvariantCulture ) != 0 )
				return false;

			if( node.Attributes[ "allowed" ] != null )
				_allowed = XmlConvert.ToBoolean( node.Attributes[ "allowed" ].Value );

			if( node.Attributes[ "required" ] != null )
				_required = XmlConvert.ToBoolean( node.Attributes[ "required" ].Value );

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
			attribute.Value = "OnServer";
			limitNode.Attributes.Append( attribute );

			if( ! _allowed )
			{
				attribute = parent.OwnerDocument.CreateAttribute( "allowed" );
				attribute.Value = XmlConvert.ToString( Allowed );
				limitNode.Attributes.Append( attribute );
			}
			else if( _required )
			{
				attribute = parent.OwnerDocument.CreateAttribute( "required" );
				attribute.Value = XmlConvert.ToString( Required );
				limitNode.Attributes.Append( attribute );
			}

			parent.AppendChild( limitNode );			
			return limitNode;
		}


		/// <summary>
		/// Overrides <see cref="Limit.GetDetailsDescription"/>.
		/// </summary>
		public override string GetDetailsDescription()
		{
			return Internal.StaticResourceProvider.CurrentProvider.FormatResourceString( "MASL_DetailedDescription", _allowed, _required );
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
	} // End class OnServerLimit
} // End namespace Xheo.Licensing

////////////////////////////////////////////////////////////////////////////////
#region Copyright � 2002-2005 XHEO, INC. All Rights Reserved.
//
#endregion
////////////////////////////////////////////////////////////////////////////////
