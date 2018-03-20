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
// Class:		DomainLimit
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

namespace Xheo.Licensing
{
	/// <summary>
	/// Restricts the use of a protected Type to a collection of domain names.
	/// </summary>
	/// <remarks>
	///		The current request must be for the given domain name. If the IPAddress is
	///		specified then the domain must also resolve to that address.
	///		<para>
	///		Domain names can include wild cards in their names. You can use the standard
	///		* and ? characters for any word or character respectively. If the name is
	///		prefixed with the # character, the text following it is treated as a
	///		regular expression and is compared with the given pattern.
	///		</para>
	/// </remarks>
#if LICENSING
	public 
#else
	internal
#endif
		class DomainLimit : Limit
	{
		////////////////////////////////////////////////////////////////////////////////
		#region Fields

		private StringCollection	_domains	= new StringCollection();
		private IPAddress			_ipaddress	= IPAddress.None;
		private int					_failures	= 0;

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Constructors/Destructors

		/// <summary>
		///	Initializes a new instance of the DomainLimit class.
		/// </summary>
		public DomainLimit()
		{
		}

		/// <summary>
		///	Initializes a new instance of the DomainLimit class.
		/// </summary>
		/// <param name="domains">
		///		Collection of domains to add to the limit.
		/// </param>
		public DomainLimit( params string[] domains )
		{
			foreach( string domain in domains )
				Domains.Add( domain );
		}

		/// <summary>
		///	Initializes a new instance of the DomainLimit class.
		/// </summary>
		/// <param name="domains">
		///		Collection of domains to add to the limit.
		/// </param>
		/// <param name="ipaddress">
		///		The IP address all domains must resolve to.
		/// </param>
		public DomainLimit( IPAddress ipaddress, params string[] domains )
		{
			IPAddress = ipaddress;
			foreach( string domain in domains )
				Domains.Add( domain );
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Properties

		/// <summary>
		/// Gets the collection of valid domains. When the license is read only, the
		/// collection returned is a copy of the original and any changes to the
		/// collection are lost.
		/// </summary>
#if LICENSING
		[
			Category( "Limits" ),
			Description( "Collection of valid domains." ),
			Editor( typeof( Xheo.Licensing.Design.StringCollectionEditor ), typeof( System.Drawing.Design.UITypeEditor ) )
		]	
#endif
		public StringCollection Domains
		{
			get
			{
				if( License != null && License.IsReadOnly )
				{
					StringCollection readOnlyCollection = new StringCollection();
					foreach( string domain in _domains )
						readOnlyCollection.Add( domain );
					return readOnlyCollection;
				}
				return _domains;
			}
		}

		/// <summary>
		/// Gets or sets the IP address that the domains must resolve to. If empty then
		/// the address is ignored.
		/// </summary>
#if LICENSING
		[
			Category( "Limits" ),
			Description( "IP address that all domains must resolve to. If empty then ignored." ),
			TypeConverter( typeof( Xheo.Licensing.Design.IPTypeConverter ) )
		]
#endif
		public IPAddress IPAddress
		{
			get
			{
				return _ipaddress;
			}
			set
			{
				AssertNotReadOnly();
				_ipaddress = value;
			}
		}

		/// <summary>
		/// Gets the description of the limit.
		/// </summary>
		public override string Description
		{
			get
			{
				return Internal.StaticResourceProvider.CurrentProvider.GetString( "MDL_Description" );
			}
		}

		/// <summary>
		/// Gets the display name of the limit.
		/// </summary>
		public override string Name
		{
			get
			{
				return "Domain Name";
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
			if( ! ExtendedLicense.IsWebRequest )
				return true;

			string current = HttpContext.Current.Request.Url.Host;

			foreach( string domain in Domains )
				if( CompareDomain( current, domain ) )
				{
					if( IPAddress == IPAddress.None 
#if NET11
						|| IPAddress == IPAddress.IPv6None 
#endif
						)
						return true;

					IPHostEntry entry = null;
					try
					{
                        entry = Dns.GetHostEntry(domain);
					}
					catch
					{
						continue;
					}

					if( entry != null )
					{
						foreach( IPAddress address in entry.AddressList )
						{
							if( address.Equals( IPAddress ) )
							{
								_failures = 0;
								return true;
							}
						}
					}
					else
					{
						_failures++;
						if( _failures <= 5 )
							return true;
					}
				}

//			_failures++;
//			if( _failures <= 5 )
//				return true;

			License.InvalidReason = Internal.StaticResourceProvider.CurrentProvider.FormatResourceString( "E_InvalidDomain",  HttpContext.Current.Request.Url.Host );
			return false;
		}

		///<summary>
		///Summary of Granted.
		///</summary>
		///<param name="context"></param>
		///<param name="type"></param>
		///<param name="instance"></param>
		public override void Granted( LicenseContext context, Type type, object instance )
		{
			if( ! ExtendedLicense.IsWebRequest )
				return;

			string current = HttpContext.Current.Request.Url.Host;
			string key = "Xheo.Licensing.DomainLimit" + License.SerialNumber + current;
			if( ExtendedLicense.IsWebRequest && HttpContext.Current.Items[ key ] != null )
				if( ((string)HttpContext.Current.Items[ key ]) != "VALID" )
					throw new LicenseException( type, instance, Internal.StaticResourceProvider.CurrentProvider.FormatResourceString( "E_InvalidDomain", HttpContext.Current.Request.Url.Host ) );

			foreach( string domain in Domains )
				if( CompareDomain( current, domain ) )
				{
					HttpContext.Current.Items[ key ] = "VALID";
					return;
				}

			License.InvalidReason = Internal.StaticResourceProvider.CurrentProvider.FormatResourceString( "E_InvalidDomain",  HttpContext.Current.Request.Url.Host );
			throw new LicenseException( type, instance, License.InvalidReason );
		}

		/// <summary>
		/// Determines if the current domain matches the desired domain.
		/// </summary>
		private bool CompareDomain( string current, string desired )
		{
			if( String.Compare( current, desired, true, System.Globalization.CultureInfo.InvariantCulture ) == 0 )
				return true;

			string pattern = null;

			if( desired[ 0 ] == '#' )
			{
				pattern = desired.Substring( 1 );
			}
			else
			{
				pattern = desired.Replace( "*", "([-_\\d\\w]*?)" ).Replace( "?", "([-_\\d\\w]??)" );
			}

			return Regex.IsMatch( current, pattern, RegexOptions.IgnoreCase );
		}

		/// <summary>
		/// Validates that the format of the domain name is correct.
		/// </summary>
		/// <param name="domainName">
		///		The name of the domain to validate.
		/// </param>
		/// <param name="resolve">
		///		Indicates if the domain should be resolved as well.
		/// </param>
		/// <returns>
		///		Returns true if the domain is valid, otherwise false.
		/// </returns>
		public static bool ValidateDomainName( string domainName, bool resolve )
		{
			if( domainName == null || domainName.Length == 0 )
				return false;

			if( domainName.IndexOf( '*' ) > -1 || domainName.IndexOf( '?' ) > -1 || domainName[ 0 ] == '#' )
				return true;

			if( ! Limit.ValidateDomainName( domainName ) )
				return false;

			if( resolve )
			{
				try
				{
                    Dns.GetHostEntry(domainName);
				}
				catch
				{
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
			if( String.Compare( node.Attributes[ "type" ].Value, "Domain", false, System.Globalization.CultureInfo.InvariantCulture ) != 0 )
				return false;

			foreach( XmlNode domainNode in node.SelectNodes( "Domain" ) )
			{
				if( domainNode.Attributes[ "name" ] == null )
					return false;
				Domains.Add( domainNode.Attributes[ "name" ].Value );
			}

			if( node.Attributes[ "ipaddress" ] != null )
				IPAddress = IPAddress.Parse( node.Attributes[ "ipaddress" ].Value );

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
			attribute.Value = "Domain";
			limitNode.Attributes.Append( attribute );

			XmlNode domainNode;

			foreach( string domain in Domains )
			{
				domainNode = parent.OwnerDocument.CreateElement( "Domain" );
				attribute = parent.OwnerDocument.CreateAttribute( "name" );
				attribute.Value = domain;
				domainNode.Attributes.Append( attribute );
				limitNode.AppendChild( domainNode );
			}

			if( IPAddress != IPAddress.None 
#if NET11
				&& IPAddress != IPAddress.IPv6None 
#endif
				)
			{
				attribute = parent.OwnerDocument.CreateAttribute( "ipaddress" );
				attribute.Value = IPAddress.ToString();
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
			System.Text.StringBuilder domains = new System.Text.StringBuilder();
			
			foreach( string domain in Domains )
			{
				if( domains.Length > 0 )
					domains.Append( System.Globalization.CultureInfo.CurrentUICulture.TextInfo.ListSeparator + " " );

				domains.Append( domain );
			}

			if( IPAddress != IPAddress.None 
#if NET11
				&& IPAddress != IPAddress.IPv6None 
#endif
				)
				domains.Append( Internal.StaticResourceProvider.CurrentProvider.FormatResourceString( "MDL_DetailedDescription", IPAddress ) );

			return domains.ToString();
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
	} // End class DomainLimit
} // End namespace Xheo.Licensing

////////////////////////////////////////////////////////////////////////////////
#region Copyright © 2002-2005 XHEO, INC. All Rights Reserved.
//
#endregion
////////////////////////////////////////////////////////////////////////////////
