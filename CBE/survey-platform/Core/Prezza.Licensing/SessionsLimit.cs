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
// Class:		sessionsLimit
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

namespace Xheo.Licensing
{
	/// <summary>
	/// Enforces a certain number of concurrent sessions. Valid only for web
	/// components.
	/// </summary>
	/// <remarks>
	///		The limit applies to all components in the license.
	/// </remarks>
#if LICENSING
	public 
#else
	internal
#endif
		class SessionsLimit : Limit
	{
		////////////////////////////////////////////////////////////////////////////////
		#region Fields

		int			_allowed	= 0;
		int			_timeout	= 60;
		
		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Constructors/Destructors

		/// <summary>
		///	Initializes a new instance of the sessionsLimit class.
		/// </summary>
		public SessionsLimit()
		{
		}

		/// <summary>
		/// Initializes a new instance of the SessionsLimit class.
		/// </summary>
		/// <param name="allowed">
		///		Max number of sessions allowed
		/// </param>
		/// <param name="timeout">
		///		Duration of a session in seconds. MIN( timeout, HttpSessionState.Timeout ) 
		///		determines the effective timeout.
		/// </param>
		public SessionsLimit( int allowed, int timeout )
		{
			Allowed		= allowed;
			Timeout		= timeout;
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Properties

		/// <summary>
		/// Gets or sets the number of allowed sessions.
		/// </summary>
#if LICENSING
		[
			Category( "Limits" ),
			Description( "Number of allowed sessions." ),
			DefaultValue( 0 )
		]
#endif
		public int Allowed
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
		///	Gets or sets the duration of a session in seconds. MIN( timeout, HttpSessionState.Timeout ) 
		///	determines the effective timeout.
		/// </summary>
#if LICENSING
		[
			Category( "Limits" ),
			Description( "Time before an inactive session is released." ),
			DefaultValue( 60 )
		]
#endif
		public int Timeout
		{
			get
			{
				return _timeout;
			}
			set
			{
				AssertNotReadOnly();
				_timeout = value;
			}
		}

		/// <summary>
		/// Gets the description of the limit.
		/// </summary>
		public override string Description
		{
			get
			{
				return Internal.StaticResourceProvider.CurrentProvider.GetString( "MSL_Description" );
			}
		}

		/// <summary>
		/// Gets the display name of the limit.
		/// </summary>
		public override string Name
		{
			get
			{
				return "Sessions";
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
			System.Web.SessionState.HttpSessionState session = null;

			if( instance != null )
			{
				if( ( instance is Page || instance is Control ) && ! ExtendedLicense.IsWebRequest )
				{
					License.InvalidReason = Internal.StaticResourceProvider.CurrentProvider.GetString( "E_MissingContextOrSession" );
					return false;
				}
			}

			HttpContext httpContext = HttpContext.Current;

			if( ! ExtendedLicense.IsWebRequest )
				return true;

			if( httpContext == null || httpContext.Session == null )
			{
				License.InvalidReason = Internal.StaticResourceProvider.CurrentProvider.GetString( "E_MissingContextOrSession" );
				return false;			
			}
			else
				session = httpContext.Session;

			if( session == null )
			{
				if( HttpContext.Current.Handler is Page )
					session = ((Page)HttpContext.Current.Handler).Session;
				if( session == null )
				{
					License.InvalidReason = Internal.StaticResourceProvider.CurrentProvider.GetString( "E_MissingContextOrSession" );
					return false;
				}
			}

			Hashtable sessions = License.PersistantState[ "Sessions" ] as Hashtable;
			if( sessions == null )
			{
				sessions = new Hashtable();
				License.PersistantState[ "Sessions" ] = sessions;
			}

			if( sessions[ session.SessionID ] == null )
			{
				if( sessions.Count >=  Allowed )
				{
					foreach( string sessionId in sessions.Keys )
						if( ((DateTime)sessions[ sessionId ]).AddSeconds( Timeout ) < DateTime.UtcNow )
						{
							sessions.Remove( sessionId );
							break;
						}

					if( sessions.Count >= Allowed )
					{
						License.InvalidReason = Internal.StaticResourceProvider.CurrentProvider.FormatResourceString( "E_MaxSessionsReached", Allowed );
						return false;
					}
				}

				sessions[ session.SessionID ] = DateTime.UtcNow;
				return true;
			}
			else
			{
				sessions[ session.SessionID ] = DateTime.UtcNow;
				return true;
			}
		}

		///<summary>
		///Summary of Granted.
		///</summary>
		///<param name="context"></param>
		///<param name="type"></param>
		///<param name="instance"></param>
		public override void Granted( LicenseContext context, Type type, object instance )
		{
			if( ! Validate( context, type, instance ) )
				throw new LicenseException( type, instance, Internal.StaticResourceProvider.CurrentProvider.FormatResourceString( "E_MaxSessionsReached", Allowed ) );
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
			if( String.Compare( node.Attributes[ "type" ].Value, "Sessions", false, System.Globalization.CultureInfo.InvariantCulture ) != 0 )
				return false;

			if( node.Attributes[ "allowed" ] == null )
				return false;

			if( node.Attributes[ "timeout" ] == null )
				Timeout = 60;
			else
				Timeout = Int32.Parse( node.Attributes[ "timeout" ].Value, System.Globalization.CultureInfo.InvariantCulture );

			Allowed = Int32.Parse( node.Attributes[ "allowed" ].Value, System.Globalization.CultureInfo.InvariantCulture );

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
			attribute.Value = "Sessions";
			limitNode.Attributes.Append( attribute );

			attribute = parent.OwnerDocument.CreateAttribute( "allowed" );
			attribute.Value = XmlConvert.ToString( Allowed );
			limitNode.Attributes.Append( attribute );

			attribute = parent.OwnerDocument.CreateAttribute( "timeout" );
			attribute.Value = XmlConvert.ToString( Timeout );
			limitNode.Attributes.Append( attribute );

			parent.AppendChild( limitNode );			
			return limitNode;
		}

		/// <summary>
		/// Overrides <see cref="Limit.GetDetailsDescription"/>.
		/// </summary>
		public override string GetDetailsDescription()
		{
			return Internal.StaticResourceProvider.CurrentProvider.FormatResourceString( "MSL_DetailedDescription", Allowed, TimeSpan.FromSeconds( Timeout ) );
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
	} // End class SessionsLimit
} // End namespace Xheo.Licensing

////////////////////////////////////////////////////////////////////////////////
#region Copyright © 2002-2005 XHEO, INC. All Rights Reserved.
//
#endregion
////////////////////////////////////////////////////////////////////////////////
