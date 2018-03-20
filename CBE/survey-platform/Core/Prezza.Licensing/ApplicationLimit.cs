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
// Class:		ApplicationLimit
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
using System.Collections.Specialized;


namespace Xheo.Licensing
{
	/// <summary>
	/// Limits the number of applications on a single machine that can simultaneously
	/// license a component.
	/// </summary>
#if LICENSING
	public 
#else
	internal
#endif
		class ApplicationLimit : Limit
	{
		////////////////////////////////////////////////////////////////////////////////
		#region Fields

		private int _allowedInstances	= 1;
		private int _port				= 8937;

		private static string	_cookie				= null;
		private static int		_applicationPort	= 8937;
		
		private static UnregisterFromBroker	_helper	= new UnregisterFromBroker();

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the ApplicationLimit class.
		/// </summary>
		public ApplicationLimit()
		{
		}

		/// <summary>
		///  Initializes a new instance of the ApplicationLimit class.
		/// </summary>
		/// <param name="allowedInstances">
		///		The total number of application Applications allowed per type
		///		on the machine.
		/// </param>
		/// <param name="port">
		///		The remoting port to use for managing Applications. Default is 8937.
		/// </param>
		public ApplicationLimit( int allowedInstances, int port )
		{
			AllowedInstances = allowedInstances;
			Port = port;
		}

		/// <summary>
		///  Initializes a new instance of the ApplicationLimit class.
		/// </summary>
		/// <param name="allowedInstances">
		///		The total number of application Applications allowed per type
		///		on the machine.
		/// </param>
		public ApplicationLimit( int allowedInstances )
		{
			AllowedInstances = allowedInstances;
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Properties

		/// <summary>
		/// Gets the description of the limit.
		/// </summary>
		public override string Description
		{
			get
			{
				return Internal.StaticResourceProvider.CurrentProvider.GetString( "MAPPL_Description" );
			}
		}

		/// <summary>
		/// Gets the display name of the limit.
		/// </summary>
		public override string Name
		{
			get
			{
				return "Application";
			}
		}

		/// <summary>
		/// Gets or sets the total number of applications allowed per type
		/// on the machine.
		/// </summary>
#if LICENSING
		[
			Category( "Limits" ),
			Description( "The total number of applications allowed per type on a single machine." ),
			DefaultValue( 1 )
		]
#endif
		public int AllowedInstances
		{
			get
			{
				return _allowedInstances;
			}
			set
			{
				AssertNotReadOnly();
				_allowedInstances = value;
			}
		}

		/// <summary>
		/// Gets or sets the remoting port to use for managing Applications. Default is 8937.
		/// </summary>
#if LICENSING
		[
			Category( "Limits" ),
			Description( "Remoting port to use for managing applications." ),
			DefaultValue( 8937 ),
		]
#endif
		public int Port
		{
			get
			{
				return _port;
			}
			set
			{
				AssertNotReadOnly();
				_port = value;
			}
		}

		///<summary>
		///Gets a value of bool.
		///</summary>
		public override bool IsRemote
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
            return true;

            /*
			InstanceBroker broker = InstanceBroker.GetBroker( Port, AllowedInstances );

			if( ! ExtendedLicense.IsWebRequest )
			{
				if( _cookie == null )
				{
					_cookie = broker.GetCookie();
					_helper.Cookies.Add( _cookie );
				}

				if( ! broker.Register( _cookie, type.FullName ) )
				{
					License.InvalidReason = Internal.StaticResourceProvider.CurrentProvider.FormatResourceString( "E_AppLimitReached", AllowedInstances );
					return false;
				}

				return true;
			}
			else
			{
				string cookie = HttpContext.Current.Request.ServerVariables[ "INSTANCE_META_PATH" ];
				if( ! _helper.Cookies.Contains( cookie ) )
					_helper.Cookies.Add( cookie );

				if( ! broker.Register( _cookie, type.FullName ) )
				{
					License.InvalidReason = Internal.StaticResourceProvider.CurrentProvider.FormatResourceString( "E_AppLimitReached", AllowedInstances );
					return false;
				}

				return true;
			}*/
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
			if( String.Compare( node.Attributes[ "type" ].Value, "Application", false, System.Globalization.CultureInfo.InvariantCulture ) != 0 )
				return false;

			if( node.Attributes[ "allowedInstances" ] == null )
				return false;

			AllowedInstances = XmlConvert.ToInt32( node.Attributes[ "allowedInstances" ].Value );

			if( node.Attributes[ "port" ] != null )
				Port = XmlConvert.ToInt32( node.Attributes[ "port" ].Value );

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
			attribute.Value = "Application";
			limitNode.Attributes.Append( attribute );

			attribute = parent.OwnerDocument.CreateAttribute( "allowedInstances" );
			attribute.Value = XmlConvert.ToString( AllowedInstances );
			limitNode.Attributes.Append( attribute );

			attribute = parent.OwnerDocument.CreateAttribute( "port" );
			attribute.Value = XmlConvert.ToString( Port );
			limitNode.Attributes.Append( attribute );

			parent.AppendChild( limitNode );			
			return limitNode;
		}


		/// <summary>
		/// Overrides <see cref="Limit.GetDetailsDescription"/>.
		/// </summary>
		public override string GetDetailsDescription()
		{
			return Internal.StaticResourceProvider.CurrentProvider.FormatResourceString( "MAPPL_DetailedDescription",  AllowedInstances, Port );
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		/// <summary>
		/// When an instance of this class is disposed it will unregister the cookies
		/// from the InstanceBroker for the application.
		/// </summary>
		sealed class UnregisterFromBroker : IDisposable
		{
			////////////////////////////////////////////////////////////////////////////////
			#region Fields

			private StringCollection	_cookies	= new StringCollection();

			#endregion
			////////////////////////////////////////////////////////////////////////////////

			////////////////////////////////////////////////////////////////////////////////
			#region Constructors/Destructors

		///<summary>
		///Summary of Dispose.
		///</summary>
			public void Dispose()
			{
				Dispose( true );
				GC.SuppressFinalize( this );
			}

		///<summary>
		///Summary of Dispose.
		///</summary>
		///<param name="disposing"></param>
			private void Dispose( bool disposing )
			{
				if( disposing )
				{
					InstanceBroker broker = InstanceBroker.GetBroker( ApplicationLimit._applicationPort, -1, false );

					if( broker != null )
					{
						foreach( string cookie in Cookies )
							broker.Unregister( cookie );
					}
				}
			}

		///<summary>
		///Summary of ~UnregisterFromBroker.
		///</summary>
			~UnregisterFromBroker()
			{
				Dispose( false );
			}


			#endregion
			////////////////////////////////////////////////////////////////////////////////

			////////////////////////////////////////////////////////////////////////////////
			#region Properties

			/// <summary>
			/// Gets the collection of application cookies to unregister.
			/// </summary>
			public StringCollection Cookies
			{
				get
				{
					return _cookies;
				}
			}

			#endregion
			////////////////////////////////////////////////////////////////////////////////

			////////////////////////////////////////////////////////////////////////////////
			#region Operations

			#endregion
			////////////////////////////////////////////////////////////////////////////////
		} // End class UnregisterFromBroker

	} // End class ApplicationLimit
} // End namespace Xheo.Licensing

////////////////////////////////////////////////////////////////////////////////
#region Copyright © 2002-2005 XHEO, INC. All Rights Reserved.
//
#endregion
////////////////////////////////////////////////////////////////////////////////
