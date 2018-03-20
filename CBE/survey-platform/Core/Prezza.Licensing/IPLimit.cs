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
// Class:		IPLimit
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
using System.Text;
using System.Runtime.Serialization;

namespace Xheo.Licensing
{
	/// <summary>
	/// Enforces a limit for single or multiple IP addresses.
	/// </summary>
#if LICENSING
	public 
#else
	internal
#endif
		class IPLimit : Limit
	{
		////////////////////////////////////////////////////////////////////////////////
		#region Fields

		IPRangeCollection	_ranges = new IPRangeCollection();

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Constructors/Destructors

		/// <summary>
		///	Initializes a new instance of the IPLimit class.
		/// </summary>
		public IPLimit()
		{
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Properties

		/// <summary>
		/// Gets the collection of IPRanges for the Limit.
		/// </summary>
#if LICENSING
		[
			Category( "Limits" ),
			Description( "Collection of valid IP ranges." ),
		]
#endif
		public IPRangeCollection Ranges
		{
			get
			{
				if( License != null && License.IsReadOnly && ! _ranges.IsReadOnly )
					_ranges.SetReadOnly();
				return _ranges;
			}
		}

		/// <summary>
		/// Gets the description of the limit.
		/// </summary>
		public override string Description
		{
			get
			{
				return Internal.StaticResourceProvider.CurrentProvider.GetString( "MIPL_Description" );
			}
		}

		/// <summary>
		/// Gets the display name of the limit.
		/// </summary>
		public override string Name
		{
			get
			{
				return "IP Addresses";
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
			if( context.UsageMode == LicenseUsageMode.Designtime )
				return true;

			if( ! ExtendedLicense.IsWebRequest )
			{
				IPHostEntry host = Dns.GetHostEntry( Environment.MachineName );

				foreach( IPAddress address in host.AddressList )
					foreach( IPRange range in Ranges )
						if( range.Contains( address ) )
							return true;
			}
			else
			{
				IPAddress address = IPAddress.Parse( HttpContext.Current.Request.ServerVariables[ "LOCAL_ADDR" ] );
				foreach( IPRange range in Ranges )
					if( range.Contains( address ) )
						return true;
			}

			License.InvalidReason = Internal.StaticResourceProvider.CurrentProvider.FormatResourceString( "E_InvalidIP", HttpContext.Current.Request.ServerVariables[ "LOCAL_ADDR" ] );
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
			if( String.Compare( node.Attributes[ "type" ].Value, "IP", false, System.Globalization.CultureInfo.InvariantCulture ) != 0 )
				return false;

			foreach( XmlNode ipRangeNode in node.SelectNodes( "IPRange" ) )
			{
				if( ipRangeNode.Attributes[ "first" ] == null ||
					ipRangeNode.Attributes[ "last" ] == null )
					continue;

				Ranges.Add( new IPRange( ipRangeNode.Attributes[ "first" ].Value, ipRangeNode.Attributes[ "last" ].Value ) );
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
			attribute.Value = "IP";
			limitNode.Attributes.Append( attribute );

			XmlNode ipRangeNode;

			foreach( IPRange range in Ranges )
			{
				ipRangeNode = parent.OwnerDocument.CreateElement( "IPRange" );
				
				attribute = parent.OwnerDocument.CreateAttribute( "first" );
				attribute.Value = range.First.ToString();
				ipRangeNode.Attributes.Append( attribute );

				attribute = parent.OwnerDocument.CreateAttribute( "last" );
				attribute.Value = range.Last.ToString();
				ipRangeNode.Attributes.Append( attribute );

				limitNode.AppendChild( ipRangeNode );
			}

			parent.AppendChild( limitNode );
			return limitNode;
		}

		/// <summary>
		/// Overrides <see cref="Limit.GetDetailsDescription"/>.
		/// </summary>
		public override string GetDetailsDescription()
		{
			System.Text.StringBuilder ips = new System.Text.StringBuilder();

			foreach( IPRange range in Ranges )
			{
				if( ips.Length > 0 )
					ips.Append( System.Globalization.CultureInfo.CurrentUICulture.TextInfo.ListSeparator + " " );

				if( range.First.Equals( range.Last ) )
					ips.Append( range.First.ToString() );
				else
					ips.AppendFormat( System.Globalization.CultureInfo.InvariantCulture, "{0}-{1}", range.First, range.Last );
			}

			return ips.ToString();
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
	} // End class IPLimit

	/// <summary>
	/// Represents a range of IP addresses.
	/// </summary>
	[ Serializable ]
#if LICENSING
	public 
#else
	internal
#endif
		class IPRange
	{
		////////////////////////////////////////////////////////////////////////////////
		#region Fields

		IPAddress	_first;
		IPAddress	_last;
		IPRangeCollection _collection = null;

		#endregion
		////////////////////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////////////////////
		#region Constructors/Destructors

		/// <summary>
		///  Initializes a new instance of the IPRange class.
		/// </summary>
		public IPRange()
		{
		}

		/// <summary>
		/// Initializes a new instance of the IPRange class.
		/// </summary>
		/// <param name="first">
		///		The first IP address in the range.
		/// </param>
		/// <param name="last">
		///		The last IP address in the range.
		/// </param>
		public IPRange( string first, string last )
		{
			_first	= IPAddress.Parse( first );
			_last	= IPAddress.Parse( last );

			if( Limit.ParseIPAddress( _first ) >  Limit.ParseIPAddress( _last ) )
				throw new ExtendedLicenseException( "E_FirstIPRange" );
		}

		/// <summary>
		/// Initializes a new instance of the IPRange class.
		/// </summary>
		/// <param name="first">
		///		The first IP address in the range.
		/// </param>
		/// <param name="last">
		///		The last IP address in the range.
		/// </param>
		public IPRange( IPAddress first, IPAddress last )
		{
			_first	= first;
			_last	= last;

			if( Limit.ParseIPAddress( _first ) >  Limit.ParseIPAddress( _last ) )
				throw new ExtendedLicenseException( "E_FirstIPRange" );
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////////////////////
		#region Properties

		/// <summary>
		/// Gets or sets the collection that owns the range.
		/// </summary>
		internal IPRangeCollection Collection
		{
			get
			{
				return _collection;
			}
			set
			{
				_collection = value;
			}
		}

		/// <summary>
		/// Gets the first IP address in the range.
		/// </summary>
#if LICENSING
		[
			TypeConverter( typeof( Xheo.Licensing.Design.IPTypeConverter ) )
		]
#endif
		public IPAddress First
		{
			get
			{
				return _first;
			}
			set
			{
				if( Collection != null && Collection.IsReadOnly )
					throw new ExtendedLicenseException( "E_LicenseReadOnly" );
				_first = value;
			}
		}

		/// <summary>
		/// Gets the last IP address in the range.
		/// </summary>
#if LICENSING
		[
			TypeConverter( typeof( Xheo.Licensing.Design.IPTypeConverter ) )
		]
#endif
		public IPAddress Last
		{
			get
			{
				return _last;
			}
			set
			{
				if( Collection != null && Collection.IsReadOnly )
					throw new ExtendedLicenseException( "E_LicenseReadOnly" );
				_last = value;
			}
		}

		/// <summary>
		/// Gets the count of addresses in the range.
		/// </summary>
		[ Browsable( false ) ]
		public long Count
		{
			get
			{
				return ( Limit.ParseIPAddress( _last ) - Limit.ParseIPAddress( _first ) ) + 1;
			}
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////////////////////
		#region Operations

		/// <summary>
		/// Determines if the given IP address is within the range.
		/// </summary>
		/// <param name="address">
		///		The address to check.
		/// </param>
		/// <returns>
		///		Returns true if the address is within the first and last IP address of
		///		the range, otherwise false.
		/// </returns>
		public bool Contains( IPAddress address )
		{
			if( Limit.ParseIPAddress( address ) >= Limit.ParseIPAddress( First )
				&& Limit.ParseIPAddress( address ) <= Limit.ParseIPAddress( Last ) )
				return true;

			return false;
		}
		#region Overrides
		/// <summary>
		/// Determines if the given IP address is within the range.
		/// </summary>
		/// <param name="address">
		///		The address to check.
		/// </param>
		/// <returns>
		///		Returns true if the address is within the first and last IP address of
		///		the range, otherwise false.
		/// </returns>
		public bool Contains( string address )
		{
			return Contains( IPAddress.Parse( address ) );
		}
		#endregion

		///<summary>
		///Summary of ToString.
		///</summary>
		///<returns></returns>	
		public override string ToString()
		{
			if( First != null && Last != null )
				return First.ToString() + " - " + Last.ToString();
			return base.ToString();
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
	} // End class IPRange

	/// <summary>
	/// Implements a collection of IPRange objects.
	/// </summary>
	[ Serializable ]
#if LICENSING
	public 
#else
	internal
#endif
		sealed class IPRangeCollection : TransientReadOnlyCollection, ISerializable
	{
		////////////////////////////////////////////////////////////////////////////////
		#region Fields
		#endregion
		////////////////////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////////////////////
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the IPRangeCollection class.
		/// </summary>
		public IPRangeCollection()
		{
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////////////////////
		#region Properties

		/// <summary>
		/// Gets or sets the IPRange at the given index.
		/// </summary>
		public IPRange this[ int index ]
		{
			get
			{
				return List[ index ] as IPRange;
			}
			set
			{
				List[ index ] = value;
				value.Collection = this;
			}
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////////////////////
		#region Operations

		/// <summary>
		/// Adds the given IPRange to the collection.
		/// </summary>
		/// <param name="range">
		///		The IPRange to add.
		/// </param>
		public void Add( IPRange range )
		{
			List.Add( range );
			range.Collection = this;
		}

		/// <summary>
		/// Inserts the IPRange at the given index.
		/// </summary>
		/// <param name="index">
		///		The index to insert the IPRange.
		/// </param>
		/// <param name="range">
		///		The IPRange to insert.
		/// </param>
		public void Insert( int index, IPRange range )
		{
			List.Insert( index, range );
		}

		/// <summary>
		/// Removes the given IPRange from the collection.
		/// </summary>
		/// <param name="range">
		///		The IPRange to remove.
		/// </param>
		public void Remove( IPRange range )
		{
			List.Remove( range );
		}

		/// <summary>
		/// Copies the collection to an array.
		/// </summary>
		/// <param name="array">
		///		The array to copy to.
		/// </param>
		/// <param name="index">
		///		The index in the array to start copying.
		/// </param>
		public void CopyTo( IPRange[] array, int index )
		{
			List.CopyTo( array, index );
		}

		/// <summary>
		/// Gets the index of the given IPRange.
		/// </summary>
		/// <param name="range">
		///		The IPRange to look for.
		/// </param>
		/// <returns>
		///		Returns the index of the given IPRange if found, otherwise -1.
		/// </returns>
		public int IndexOf( IPRange range )
		{
			return List.IndexOf( range );
		}

		/// <summary>
		/// Determines if the collection contains the given IPRange.
		/// </summary>
		/// <param name="range">
		///		The IPRange to look for.
		/// </param>
		/// <returns>
		///		Returns true if the IPRange is found, otherwise false.
		/// </returns>
		public bool Contains( IPRange range )
		{
			return List.Contains( range );
		}

		/// <summary>
		/// Determines if a range with the given first and last IP address exists in the
		/// collection.
		/// </summary>
		/// <param name="first">
		///		The first IP address.
		/// </param>
		/// <param name="last">
		///		The last IP address.
		/// </param>
		/// <returns>
		///		Returns if the range exists, otherwise false.
		/// </returns>
		public bool Contains( IPAddress first, IPAddress last )
		{
			foreach( IPRange range in this )
				if( range.First == first && range.Last == last )
					return true;

			return false;
		}

		/// <summary>
		/// Determines if the range of IP addresses between the first and last IP address
		/// overlaps another range in the collection.
		/// </summary>
		/// <param name="first">
		///		The first IP address.
		/// </param>
		/// <param name="last">
		///		The last IP address.
		/// </param>
		/// <returns>
		///		Returns if the range overlaps another range in the collection, otherwise 
		///		false.
		/// </returns>
		public bool Overlaps( IPAddress first, IPAddress last )
		{
			foreach( IPRange range in this )
			{
				if( range.Contains( first ) || range.Contains( last ) )
					return true;
			}

			return false;
		}

		/// <summary>
		/// Determines if the range of IP addresses between the first and last IP address
		/// overlaps another range in the collection.
		/// </summary>
		/// <param name="range">
		///		The IPRange representing the first and last IP addresses in the block.
		/// </param>
		/// <returns>
		///		Returns if the range overlaps another range in the collection, otherwise 
		///		false.
		/// </returns>
		public bool Overlaps( IPRange range )
		{
			return Overlaps( range.First, range.Last );
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region ISerializable

		/// <summary>
		/// Implements <see cref="ISerializable.GetObjectData"/>.
		/// </summary>
		void ISerializable.GetObjectData( SerializationInfo info, StreamingContext context )
		{
			StringBuilder persist = new StringBuilder();

			foreach( IPRange range in this )
			{
				if( persist.Length > 0 )
					persist.Append( ',' );
				persist.Append( range.First.ToString() );
				persist.Append( '-' );
				persist.Append( range.Last.ToString() );
			}

			if( persist.Length > 0 )
				info.AddValue( "IPRangeCollection", persist.ToString() );
			else
				info.AddValue( "IPRangeCollection", (string)null );
		}

		/// <summary>
		/// Deserializes a license that was previously serialzed. Used by the
		/// .NET framework when marshalling an object between AppDomains.
		/// </summary>
		private IPRangeCollection( SerializationInfo info, StreamingContext context )
		{
			string		persisted	= info.GetValue( "IPRangeCollection", typeof( string ) ) as string;

			if( persisted == null )
				return;
			
			string[]	ranges		= persisted.Split( ',' );
			
			foreach( string range in ranges )
			{
				int index = range.IndexOf( '-' );
				if( index == -1 )
					throw new SerializationException( Internal.StaticResourceProvider.CurrentProvider.GetString( "E_InvalidDataIPRange" ) );
				IPRange ipRange = new IPRange( range.Substring( 0, index ), range.Substring( index + 1 ) );
				Add( ipRange  );
			}
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
	} // End class IPRange Collection
} // End namespace Xheo.Licensing

////////////////////////////////////////////////////////////////////////////////
#region Copyright © 2002-2005 XHEO, INC. All Rights Reserved.
//
#endregion
////////////////////////////////////////////////////////////////////////////////
