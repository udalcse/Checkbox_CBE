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
// Class:		LicenseTemplate
// Author:		Paul Alexander
// Created:		Monday, November 18, 2002 9:22:40 PM
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.ComponentModel;
using System.Xml;
using System.Configuration;
using System.Collections;
using System.Collections.Specialized;
using System.IO;


namespace Xheo.Licensing
{
	/// <summary>
	/// Represents a license template with predefined limits and other values.
	/// </summary>
#if LICENSING
	public 
#else
	internal
#endif
		class LicenseTemplate
	{
		////////////////////////////////////////////////////////////////////////////////
		#region Fields
		
		private string	_name			= null;
		private string	_description	= null;
		private string	_assembly		= null;
		private string	_issuerUrl		= null;
		private string	_tag			= null;
		private string	_type			= null;
		private string	_location		= null;
		private string	_serialNumber	= null;
		
		private TimeSpan	_expiration		= TimeSpan.Zero;

		private StringCollection		_components = new StringCollection();
		private LimitCollection			_limits		= new LimitCollection();
		private LicenseValuesCollection	_values		= new LicenseValuesCollection();


		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Constructors/Destructors

		/// <summary>
		///	Initializes a new instance of the LicenseTemplate class.
		/// </summary>
		public LicenseTemplate()
		{
		}

		/// <summary>
		///	Initializes a new instance of the LicenseTemplate class and loads the template
		///	from the given file name..
		/// </summary>
		/// <param name="file">
		///		Path to the template to load.
		/// </param>
		public LicenseTemplate( string file )
		{
			Load( file );
		}

		/// <summary>
		/// Initializes a new instance of the LicenseTemplate class from an existing
		/// license.
		/// </summary>
		/// <param name="license">
		///		The license to make a template from.
		/// </param>
		public LicenseTemplate( ExtendedLicense license )
		{
			Load( license );
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Properties

		/// <summary>
		/// Gets or sets the serial number for the template.
		/// </summary>
		public string SerialNumber
		{
			get
			{
				return _serialNumber;
			}
			set
			{
				_serialNumber = value;
			}
		}

		/// <summary>
		/// Gets or sets the path where the template is located.
		/// </summary>
		public string Location
		{
			get
			{
				return _location;
			}
			set
			{
				_location = value;
			}
		}

		/// <summary>
		/// Gets or sets a short descriptive name for the template.
		/// </summary>
		/// <remarks>
		///		No functional value is placed on this property. This is used for development and
		///		management purposes only in describing the contained functionality of the template.
		///		For instance, the License Manager displays a list of templates by name
		///		for you to choose from when creating a new license.
		/// </remarks>
		[
			Category( "Template" ),
			Description( "A short descriptive name for the template." )
		]
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
			}
		}

		/// <summary>
		/// Gets or sets a longer description of the template.
		/// </summary>
		/// <remarks>
		///		No functional value is placed on this property. This is used for development and
		///		management purposes only in describing the contained functionality of the template.
		///		For instance, the License Manager displays a list of templates with descriptions
		///		for you to choose from when creating a new license.
		/// </remarks>
		[
			Category( "Template" ),
			Description( "Longer description of the template." ),
		]
		public string Description
		{
			get
			{
				return _description;
			}
			set
			{
				_description = value;
			}
		}

		/// <summary>
		/// Gets or sets the name of the assembly that hosts the components that this license
		/// refers to.
		/// </summary>
		[
			Category( "Template" ),
			Description( "Name of the assembly that this license refers to." ),
		]
		public string Assembly
		{
			get
			{
				return _assembly;
			}
			set
			{
				_assembly = value;
			}
		}

       
		/// <summary>
		/// Gets a collection of components to be included in the license.
		/// </summary>
		[ Browsable( false ) ]
		public StringCollection Components
		{
			get
			{
				return _components;
			}
		}

		/// <summary>
		/// Gets a collection of initial limits to be included in the license.
		/// </summary>
		[ Browsable( false ) ]
		public LimitCollection Limits
		{
			get
			{
				return _limits;
			}
		}

		/// <summary>
		/// Gets or sets the URL of the company that issued the license.
		/// </summary>
		[
			Category( "Template" ),
			Description( "The URL of the company that issued the license." ),
		]
		public string IssuerUrl
		{
			get
			{
				return _issuerUrl;
			}
			set
			{
				_issuerUrl = value;
			}
		}

		/// <summary>
		/// Gets or sets any arbitrary data to store with the license.
		/// </summary>
		[
			Category( "Template" ),
			Description( "Arbitrary data to store with the license." ),
		]
		public string Tag
		{
			get
			{
				return _tag;
			}
			set
			{
				_tag = value;
			}
		}

		/// <summary>
		/// Gets or sets an application/product specific description of the type of
		/// license. Example designer, single user, non-profit, etc. This can often be
		/// used a reference to the primary key in a database for the product that the
		/// license represents.
		/// </summary>
		[
			Category( "Template" ),
			Description( "Application or product specific description of the type of license. Example: Designer, SINGLE-USER, D44H etc. This can often be used as a reference to the primary key in a database for the product that the license represents." )
		]
		public string Type
		{
			get
			{
				return _type;
			}
			set
			{
				_type = value;
			}
		}

		/// <summary>
		/// Gets or sets the TimeSpan the license should initially be set for.
		/// </summary>
		[
			Category( "Template" ),
			Description( "TimeSpan the license should initially be set for." ),
		]
		public TimeSpan Expiration
		{
			get
			{
				return _expiration;
			}
			set
			{
				_expiration = value;
			}
		}

		/// <summary>
		/// Gets a collection of standard values to include in the template.
		/// </summary>
		public LicenseValuesCollection Values
		{
			get
			{
				return _values;
			}
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Operations

		/// <summary>
		/// Loads a template from properties in an existing license.
		/// </summary>
		/// <param name="license">
		///		The license to load initial values from.
		/// </param>
		public void Load( ExtendedLicense license )
		{
			Components.Clear();
			Limits.Clear();

			IssuerUrl	= license.IssuerUrl;
			Tag			= license.Tag;
			Type		= license.Type;
			Expiration	= license.Expires - DateTime.UtcNow;
			Assembly	= license.AssemblyName;

			if( license.SerialNumber != null && license.SerialNumber.Length != 36 )
				SerialNumber = license.SerialNumber;
			
			Components.Clear();
			foreach( string component in license.Components )
				Components.Add( component );

			Limits.Clear();
			foreach( Limit limit in license.Limits )
				Limits.Add( limit );

			Values.Clear();
			Values.Add( license.Values );
		}

		/// <summary>
		/// Loads the template from the location given in the file parameter.
		/// </summary>
		/// <param name="file">
		///		Path of the file to load the template from.
		/// </param>
		public void Load( string file )
		{
			XmlDocument doc			= new XmlDocument();
			XmlNode		node		= null;
			string		location	= file;

			location = Path.Combine( ExtendedLicense.TemplateFolder, file );
			
			doc.Load( location );
			node = doc.DocumentElement;

			if( node[ "Name" ] != null )
				Name = node[ "Name" ].InnerText;

			if( node[ "Description" ] != null )
				Description = node[ "Description" ].InnerXml;

			if( node[ "Assembly" ] != null )
				Assembly = node[ "Assembly" ].InnerText;

			if( node[ "IssuerUrl" ] != null )
				IssuerUrl = node[ "IssuerUrl" ].InnerText;
			
			if( node[ "Tag" ] != null )
				Tag = node[ "Tag" ].InnerText;
			
			if( node[ "Type" ] != null )
				Type = node[ "Type" ].InnerText;
			
			if( node[ "Expiration" ] != null )
				Expiration = XmlConvert.ToTimeSpan( node[ "Expiration" ].InnerText );

			if( node[ "SerialNumber" ] != null )
				SerialNumber = node[ "SerialNumber" ].InnerText;
			
			Components.Clear();
			foreach( XmlNode componentNode in node.SelectNodes( "Component" ) )
				Components.Add( componentNode.InnerText );

			Limits.Clear();
			Limits.LoadFromXml( node[ "Limits" ] );

			Values.Clear();
			Values.LoadFromXml( node[ "Values" ] );

			_location = location;
		}

		/// <summary>
		/// Saves the template to the location given in the file parameter.
		/// </summary>
		/// <param name="file">
		///		Path where the template should be saved.
		/// </param>
		public void Save( string file )
		{
			XmlDocument	doc		= new XmlDocument();
			XmlNode		root	= doc.CreateElement( "LicenseTemplate" );
			XmlNode		node	= null;

			doc.AppendChild( root );

			if( Name != null && Name.Length > 0 )
			{
				node = doc.CreateElement( "Name" );
				node.InnerText = Name;
				root.AppendChild( node );
			}

			if( Description != null && Description.Length > 0 )
			{
				node = doc.CreateElement( "Description" );
				node.InnerText = Description;
				root.AppendChild( node );
			}

			if( Assembly != null && Assembly.Length > 0 )
			{
				node = doc.CreateElement( "Assembly" );
				node.InnerText = Assembly;
				root.AppendChild( node );
			}

			if( SerialNumber != null && SerialNumber.Length > 0 )
			{
				node = doc.CreateElement( "SerialNumber" );
				node.InnerText = SerialNumber;
				root.AppendChild( node );
			}

			if( IssuerUrl != null && IssuerUrl.Length > 0 )
			{
				node = doc.CreateElement( "IssuerUrl" );
				node.InnerText = IssuerUrl;
				root.AppendChild( node );
			}

			if( Tag != null && Tag.Length > 0 )
			{
				node = doc.CreateElement( "Tag" );
				node.InnerText = Tag;
				root.AppendChild( node );
			}

			if( Type != null && Type.Length > 0 )
			{
				node = doc.CreateElement( "Type" );
				node.InnerText = Type;
				root.AppendChild( node );
			}

			if( Expiration != TimeSpan.Zero )
			{
				node = doc.CreateElement( "Expiration" );
				node.InnerText = XmlConvert.ToString( Expiration );
				root.AppendChild( node );
			}

			foreach( string component in Components )
			{
				node = doc.CreateElement( "Component" );
				node.InnerText = component;
				root.AppendChild( node );
			}

			Limits.SaveToXml( root );
			Values.SaveToXml( root );

			string location = Path.Combine( ExtendedLicense.TemplateFolder, file );
			doc.Save( location );
			_location = location;
		}

		/// <summary>
		/// Creates a new license from the template. The license still needs to be 
		/// signed and added to a license pack.
		/// </summary>
		/// <returns>
		///		Returns the new license with initial values from the template.
		/// </returns>
		/// <remarks>
		///		You can call this method multiple times on the same object without
		///		any adverse effects.
		/// </remarks>
		public ExtendedLicense CreateLicense()
		{
			ExtendedLicense license = new ExtendedLicense();

			foreach( string component in Components )
				license.Components.Add( component );

			foreach( Limit limit in Limits )
				license.Limits.Add( limit );

			if( Expiration != TimeSpan.Zero )
				license.Expires = DateTime.UtcNow.Add( Expiration );

			if( SerialNumber != null && SerialNumber.Length > 0 )
				license.SerialNumber = SerialNumber;

			license.IssuerUrl		= IssuerUrl;
			license.Tag				= Tag;
			license.Type			= Type;
			license.AssemblyName	= Assembly;
			license.Values.Add( Values );
			if( Location != null && Location.Length > 0 )
				license.MetaValues[ "LicenseTemplate" ] = Limit.MaskValue( Location );

			return license;
		}

		/// <summary>
		/// Gets the <see cref="LicenseTemplate"/> used to create the license.
		/// </summary>
		/// <param name="license">
		///		The license to process.
		/// </param>
		/// <returns>
		///		Returns the LicenseTemplate used to create the template if found,
		///		otherwise null.
		/// </returns>
		public static LicenseTemplate GetCreationTemplate( ExtendedLicense license )
		{
			string metaLocation = license.MetaValues[ "LicenseTemplate" ];
			if( metaLocation == null || metaLocation.Length == 0 )
				return null;
			string location = Limit.UnmaskValue( metaLocation );
			if( location == null || ! File.Exists( Path.Combine( ExtendedLicense.TemplateFolder, location ) ) )
				return null;
			return new LicenseTemplate( location );
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
	} // End class LicenseTemplate
} // End namespace Xheo.Licensing

////////////////////////////////////////////////////////////////////////////////
#region Copyright © 2002-2005 XHEO, INC. All Rights Reserved.
//
#endregion
////////////////////////////////////////////////////////////////////////////////
