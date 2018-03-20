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
// Class:		PublisherLimit
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
using System.Diagnostics;
using System.Reflection;
using System.Collections;

namespace Xheo.Licensing
{
	/// <summary>
	/// Restricts the use of a protected Type to the assemblies strongly named by
	/// a certain publisher.
	/// </summary>
	/// <remarks>
	/// If an assembly signed with the publisher's strong name key exists anywhere
	/// on the stack then the license is considered valid.
	/// </remarks>
#if LICENSING
	public 
#else
	internal
#endif
		class PublisherLimit : Limit
	{
		////////////////////////////////////////////////////////////////////////////////
		#region Fields

		private string _publicKeyToken	= null;

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Constructors/Destructors

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
				return Internal.StaticResourceProvider.CurrentProvider.GetString( "MPL_Description" );
			}
		}

		/// <summary>
		/// Gets the display name of the limit.
		/// </summary>
		public override string Name
		{
			get
			{
				return "Publisher";
			}
		}

		/// <summary>
		/// Gets or sets the public key token of the publishers strong name key used to
		/// sign their assemblies.
		/// </summary>
#if LICENSING
		[
			Category( "Limits" ),
			Description( "The public key token of the publishers strong name key used to sign their assemblies." )
		]
#endif
		public string PublicKeyToken
		{
			get
			{
				return _publicKeyToken;
			}
			set
			{
				AssertNotReadOnly();
				_publicKeyToken = value;
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
			StackTrace trace = new StackTrace( 6, false );

			ArrayList tried = new ArrayList( trace.FrameCount );

			for( int index = 0; index < trace.FrameCount; index++ )
			{
				StackFrame frame = trace.GetFrame( index );
				
				MethodBase method = frame.GetMethod();
				Type	declaringType	= method.DeclaringType;
				if( declaringType == type || declaringType.Assembly == type.Assembly )
					continue;

				if( tried.Contains( declaringType.Assembly.FullName ) )
					continue;
				tried.Add( declaringType.Assembly.FullName );
				if( declaringType.Assembly == typeof( ExtendedLicense ).Assembly )
					continue;
				if( declaringType.Assembly == typeof( object ).Assembly )
					continue;

				Assembly assembly		= method.DeclaringType.Assembly;

				string	name	= assembly.FullName;
				string	token	= null;
				int		pkt		= name.IndexOf( "PublicKeyToken=" );
				int		pkt2	= 0;
				if( pkt > -1 )
				{
					pkt += 15;
					pkt2	= name.IndexOf( ',', pkt );
					if( pkt2 > -1 )
						token = name.Substring( pkt, pkt2 - pkt );
					else
						token = name.Substring( pkt );

					if( String.Compare( PublicKeyToken, token, false, System.Globalization.CultureInfo.InvariantCulture ) == 0 )
						return true;
				}
			}

			License.InvalidReason = Internal.StaticResourceProvider.CurrentProvider.FormatResourceString( 
				"E_InvalidPublisher",
				( License.Organization != null && License.Organization.Length > 0 ) ? License.Organization : "publisher",
				PublicKeyToken );
			return false;
		}
		
		/// <summary>
		/// Called every time the limit is granted. Makes sure a publisher assembly is always
		/// in the call chain when validated.
		/// </summary>
		/// <param name="context">
		///		The LicenseContext used to determine the environment and settings
		///		of the licensing.
		/// </param>
		/// <param name="instance">
		///		The instance of the object being licensed.
		/// </param>
		/// <param name="type">
		///		The Type of the object being licensed.
		/// </param>
		public override void Granted( LicenseContext context, Type type, object instance )
		{
			if( ! Validate( context, type, instance ) )
				throw new LicenseException( type, instance, License.InvalidReason );
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
			if( String.Compare( node.Attributes[ "type" ].Value, "Publisher", false, System.Globalization.CultureInfo.InvariantCulture ) != 0 )
				return false;

			if( node.Attributes[ "publicKeyToken" ] != null )
				PublicKeyToken = node.Attributes[ "publicKeyToken" ].Value;

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
			attribute.Value = "Publisher";
			limitNode.Attributes.Append( attribute );

			attribute = parent.OwnerDocument.CreateAttribute( "publicKeyToken" );
			attribute.Value = PublicKeyToken;
			limitNode.Attributes.Append( attribute );

			parent.AppendChild( limitNode );	
			return limitNode;
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
	} // End class PublisherLimit
} // End namespace Xheo.Licensing

////////////////////////////////////////////////////////////////////////////////
#region Copyright © 2002-2005 XHEO, INC. All Rights Reserved.
//
#endregion
////////////////////////////////////////////////////////////////////////////////
