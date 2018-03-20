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
// Class:		VersionLimit
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
	/// Requires that the assembly version be between a minimum and maximum value.
	/// </summary>
	/// <remarks>
	///		If no minimum value is specified then the version must match the maximum
	///		version exactly.
	/// </remarks>
#if LICENSING
	public 
#else
	internal
#endif
		class VersionLimit : Limit
	{
		////////////////////////////////////////////////////////////////////////////////
		#region Fields

		private Version	_minimum	= new Version( 0, 0 );
		private Version _maximum	= new Version( 0, 0 );

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Constructors/Destructors

		/// <summary>
		///	Initializes a new instance of the VersionLimit class.
		/// </summary>
		public VersionLimit()
		{
	
		}

		/// <summary>
		///	Initializes a new instance of the VersionLimit class.
		/// </summary>
		/// <param name="maximum">
		///		The maximum allowable version.
		/// </param>
		/// <param name="minimum">
		///		The minimum allowable version.
		/// </param>
		public VersionLimit( Version minimum, Version maximum )
		{
			if( Maximum < Minimum )
				throw new InvalidOperationException( Internal.StaticResourceProvider.CurrentProvider.FormatResourceString( "E_VersionOrder" ) );

			Maximum = maximum;
			Minimum = minimum;
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Properties

		/// <summary>
		/// Gets or sets the minimum version.
		/// </summary>
#if LICENSING
		[
			Category( "Limits" ),
			Description( "The minimum version required." ),
			TypeConverter( typeof( Xheo.Licensing.Design.VersionTypeConverter ) ),
		]
#endif
		public Version Minimum
		{
			get
			{
				return _minimum;
			}
			set
			{
				AssertNotReadOnly();
				_minimum = value;
			}
		}

		/// <summary>
		/// Gets or sets the maximum version.
		/// </summary>
#if LICENSING
		[
			Category( "Limits" ),
			Description( "The maximum version allowed." ),
			TypeConverter( typeof( Xheo.Licensing.Design.VersionTypeConverter ) ),
		]
#endif
		public Version Maximum
		{
			get
			{
				return _maximum;
			}
			set
			{
				AssertNotReadOnly();
				_maximum = value;
			}
		}

		/// <summary>
		/// Gets the description of the limit.
		/// </summary>
		public override string Description
		{
			get
			{
				return Internal.StaticResourceProvider.CurrentProvider.GetString( "MVL_Description" );
			}
		}

		/// <summary>
		/// Gets the display name of the limit.
		/// </summary>
		public override string Name
		{
			get
			{
				return "Version";
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
			if( Maximum < Minimum )
				throw new InvalidOperationException( Internal.StaticResourceProvider.CurrentProvider.FormatResourceString( "E_VersionOrder" ) );

			Version assemblyVersion = type.Assembly.GetName().Version;

			if( assemblyVersion >= Minimum && assemblyVersion <= Maximum )
				return true;

			if( Maximum.CompareTo( Minimum ) == 0 )
				License.InvalidReason = Internal.StaticResourceProvider.CurrentProvider.FormatResourceString( "E_VersionMismatch",  assemblyVersion, Maximum );
			else
				License.InvalidReason = Internal.StaticResourceProvider.CurrentProvider.FormatResourceString( "E_VersionMismatchRange", assemblyVersion, Minimum, Maximum );
			return false;
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
			if( String.Compare( node.Attributes[ "type" ].Value, "Version", false, System.Globalization.CultureInfo.InvariantCulture ) != 0 )
				return false;

			if( node.Attributes[ "minimum" ] == null ||
				node.Attributes[ "maximum" ] == null )
				return false;

			Maximum = new Version( node.Attributes[ "maximum" ].Value.Trim() );
			Minimum = new Version( node.Attributes[ "minimum" ].Value.Trim() );

			if( Maximum < Minimum )
				throw new InvalidOperationException( Internal.StaticResourceProvider.CurrentProvider.FormatResourceString( "E_VersionOrder" ) );

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
			if( Maximum < Minimum )
				throw new InvalidOperationException( Internal.StaticResourceProvider.CurrentProvider.FormatResourceString( "E_VersionOrder" ) );

			XmlNode limitNode = parent.OwnerDocument.CreateElement( "Limit" );
			XmlAttribute attribute = parent.OwnerDocument.CreateAttribute( "type" );
			attribute.Value = "Version";
			limitNode.Attributes.Append( attribute );

			attribute = parent.OwnerDocument.CreateAttribute( "maximum" );
			attribute.Value = Maximum.ToString();
			limitNode.Attributes.Append( attribute );

			attribute = parent.OwnerDocument.CreateAttribute( "minimum" );
			attribute.Value = Minimum.ToString();
			limitNode.Attributes.Append( attribute );
            
			parent.AppendChild( limitNode );			
			return limitNode;
		}

		/// <summary>
		/// Overrides <see cref="Limit.GetDetailsDescription"/>.
		/// </summary>
		public override string GetDetailsDescription()
		{
			if( Maximum.CompareTo( Minimum ) == 0 )
				return Maximum.ToString();
			else
				return Internal.StaticResourceProvider.CurrentProvider.FormatResourceString( "MVL_DetailedDescription", Minimum, Maximum );
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
	} // End class VersionLimit
} // End namespace Xheo.Licensing

////////////////////////////////////////////////////////////////////////////////
#region Copyright © 2002-2005 XHEO, INC. All Rights Reserved.
//
#endregion
////////////////////////////////////////////////////////////////////////////////
