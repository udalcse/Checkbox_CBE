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
// Class:		InstanceBroker
// Author:		Paul Alexander
// Created:		Friday, October 25, 2002 12:06:39 AM
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.ComponentModel;
using System.Runtime.Remoting;
using System.Collections;
using System.Collections.Specialized;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Serialization;

namespace Xheo.Licensing
{
	/// <summary>
	/// Implements a broker for tracking licenses applications on a single machine.
	/// <seealso cref="ApplicationLimit"/>
	/// </summary>
	[ Serializable ]
	internal class InstanceBroker : MarshalByRefObject, ISerializable
	{
		////////////////////////////////////////////////////////////////////////////////
		#region Fields

		[ NonSerialized ]
		private Hashtable	_types				= new Hashtable();
		[ NonSerialized ]
		private int			_allowedInstances	= -1;

		[ NonSerialized ]
		private static		InstanceBroker _broker				= null;

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Constructors/Destructors

		/// <summary>
		///	Initializes a new instance of the InstanceBroker class.
		/// </summary>
		public InstanceBroker()
		{
		}

		
		#region ISerializable Members

		void ISerializable.GetObjectData( SerializationInfo info, StreamingContext context )
		{
			info.AddValue( "Instances", _allowedInstances );
		}

		private InstanceBroker( SerializationInfo info, StreamingContext context )
		{
			_allowedInstances = info.GetInt32( "Instances" );
		}

		#endregion

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Properties

		/// <summary>
		/// Gets or sets the number of allowed applications per type.
		/// </summary>
		public int AllowedInstances
		{
			get
			{
				return _allowedInstances;
			}
			set
			{
				if( _allowedInstances != -1 )
					throw new InvalidOperationException( Internal.StaticResourceProvider.CurrentProvider.GetString( "E_AllowedAlreadySet" ) );

				_allowedInstances = value;
			}
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Operations

		/// <summary>
		/// Gets a new cookie for an application. The application must store this cookie
		/// for later use in registering and unregistering licenses.
		/// </summary>
		/// <returns>
		///		Returns a new cookie for an application.
		/// </returns>
		public string GetCookie()
		{
			return Guid.NewGuid().ToString( "B", System.Globalization.CultureInfo.InvariantCulture );
		}

		/// <summary>
		/// Registers an application's desire for a license on a given type.
		/// <seealso cref="GetCookie"/>
		/// </summary>
		/// <param name="cookie">
		///		Unique cookie returned by the first call to <see cref="GetCookie"/>.
		/// </param>
		/// <param name="type">
		///		The type of the object requesting a license.
		/// </param>
		/// <returns>
		///		Returns true if the request was registered successfully, otherwise false.
		/// </returns>
		public bool Register( string cookie, string type )
		{
			lock( this )
			{
				StringCollection collection = _types[ type ] as StringCollection;

				if( collection == null )
				{
					collection = new StringCollection();
					collection.Add( cookie );
					_types[ type ] = collection;
					return true;
				}

				if( ! collection.Contains( cookie ) )
				{
					if( collection.Count < AllowedInstances )
					{
						collection.Add( cookie );
						return true;
					}
				}
				else
				{
					return true;
				}

				return false;
			}
		}

		/// <summary>
		/// Unregisters an application's interest in all types. Should only be called
		/// when the application is ending.
		/// </summary>
		/// <param name="cookie">
		///		Unique cookie returned by the first call to <see cref="GetCookie"/>.
		/// </param>
		public void Unregister( string cookie )
		{
			lock( this )
			{
				foreach( StringCollection collection in _types.Values )
					collection.Remove( cookie );
			}
		}

		/// <summary>
		/// Returns the number of applications registered for the given type.
		/// </summary>
		/// <param name="type">
		///		The type of object being licensed.
		/// </param>
		/// <returns>
		///		Returns the total count of applications registered for the given type.
		/// </returns>
		public int GetCountRegistered( string type )
		{
			StringCollection collection = _types[ type ] as StringCollection;
			if( collection == null )
				return 0;

			return collection.Count;
		}

		/// <summary>
		/// Ensures that the server is alive.
		/// </summary>
		public void Ping()
		{
		}

		///<summary>
		///Summary of InitializeLifetimeService.
		///</summary>
		///<returns></returns>	
		public override object InitializeLifetimeService()
		{
			return null;
		}

		/// <summary>
		/// Gets the InstanceBroker for the current AppDomain.
		/// </summary>
		/// <param name="allowedInstances">
		///		If a new broker is created, the number of application instances 
		///		it should allow.
		/// </param>
		/// <param name="port">
		///		The port to look for the server on.
		/// </param>
		/// <param name="create">
		///		Indicates if a broker should be created if it doesn't already exist.
		/// </param>
		/// <returns>
		///		Returns the broker for the given AppDomain. If not found an
		///		exception is thrown.
		/// </returns>
		/// <remarks>
		///		If no broker exists on the machine, a new broker is created and
		///		registered for other clients.
		/// </remarks>
		public static InstanceBroker GetBroker( int port, int allowedInstances, bool create )
		{
			if( _broker != null )
				return _broker;

			lock( typeof( InstanceBroker ) )
			{
				if( _broker != null )
					return _broker;

				AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
				_broker = Activator.GetObject( typeof( InstanceBroker ), String.Format( System.Globalization.CultureInfo.InvariantCulture,"tcp://localhost:{0}/Broker", port ) ) as InstanceBroker;
				if( _broker != null )
				{
					try
					{
						_broker.Ping();
					}
					catch
					{
						_broker = null;
					}

					if( _broker != null )
						return _broker;
				}

				if( create )
				{
					TcpServerChannel serverChannel = ChannelServices.GetChannel( "InstanceBroker" ) as TcpServerChannel;
				
					if( serverChannel == null )
					{
						serverChannel = new TcpServerChannel( "InstanceBroker", port );
						ChannelServices.RegisterChannel( serverChannel, false );
					}

					RemotingConfiguration.RegisterWellKnownServiceType( typeof( InstanceBroker ), "Broker", WellKnownObjectMode.Singleton );
					_broker = Activator.GetObject( typeof( InstanceBroker ), String.Format( System.Globalization.CultureInfo.InvariantCulture,"tcp://localhost:{0}/Broker", port ) ) as InstanceBroker;
					if( _broker == null )
						throw new ExtendedLicenseException( "E_CouldNotCreateInstanceBroker" );
					_broker.AllowedInstances = allowedInstances;
					return _broker;
				}
			}

			return null;
		}
		#region Overrides
		/// <summary>
		/// Gets the InstanceBroker for the current machine. If the broker doesn't
		/// exist a new one is created.
		/// </summary>
		/// <param name="allowedInstances">
		///		If a new broker is created, the number of application instances 
		///		it should allow.
		/// </param>
		/// <param name="port">
		///		The port to look for the server on.
		/// </param>
		/// <returns>
		///		Returns the broker for the given AppDomain. If not found an
		///		exception is thrown.
		/// </returns>
		/// <remarks>
		///		If no broker exists on the machine, a new broker is created and
		///		registered for other clients.
		/// </remarks>
		public static InstanceBroker GetBroker( int port, int allowedInstances )
		{
			return GetBroker( port, allowedInstances, true );
		}
		#endregion
		#region Helpers
		
		private static System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
		{
			Type ot = typeof( object );
			if( args.Name.StartsWith( ot.Assembly.FullName ) )
				return ot.Assembly;

			return null;
		}

		#endregion


		#endregion
		////////////////////////////////////////////////////////////////////////////////

	} // End class InstanceBroker
} // End namespace Xheo.Licensing

////////////////////////////////////////////////////////////////////////////////
#region Copyright © 2002-2005 XHEO, INC. All Rights Reserved.
//
#endregion
////////////////////////////////////////////////////////////////////////////////
