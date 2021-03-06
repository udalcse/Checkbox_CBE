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
// Class:		MachineLimit
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
	/// Enforces a limit for a Machine name.
	/// </summary>
#if LICENSING
	public 
#else
	internal
#endif
		class MachineLimit : Limit
	{
		////////////////////////////////////////////////////////////////////////////////
		#region Fields

		string _machineName = null;

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Constructors/Destructors

		/// <summary>
		///	Initializes a new instance of the MachineLimit class.
		/// </summary>
		public MachineLimit()
		{
		}

		/// <summary>
		///	Initializes a new instance of the MachineLimit class.
		/// </summary>
		/// <param name="machineName">
		///		The machine name to limit to.
		/// </param>
		public MachineLimit( string machineName )
		{
			MachineName = machineName;
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Properties

		/// <summary>
		/// Gets or sets the name of the Machine that the limit applies to.
		/// </summary>
#if LICENSING
		[
			Category( "Limits" ),
			Description( "NETBIOS Machine Name of the computer the component must be run on." ),
		]
#endif
		public string MachineName
		{
			get
			{
				return _machineName;
			}
			set
			{
				AssertNotReadOnly();
				_machineName = value;
			}
		}

		/// <summary>
		/// Gets the description of the limit.
		/// </summary>
		public override string Description
		{
			get
			{
				return Internal.StaticResourceProvider.CurrentProvider.GetString( "MML_Description" );
			}
		}

		/// <summary>
		/// Gets the display name of the limit.
		/// </summary>
		public override string Name
		{
			get
			{
				return "Machine";
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
			if( String.Compare( _machineName, Environment.MachineName, true, System.Globalization.CultureInfo.InvariantCulture ) == 0 )
				return true;

			License.InvalidReason = Internal.StaticResourceProvider.CurrentProvider.FormatResourceString( "E_InvalidMachine", _machineName );
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
			if( String.Compare( node.Attributes[ "type" ].Value, "Machine", false, System.Globalization.CultureInfo.InvariantCulture ) != 0 )
				return false;

			if( node.Attributes[ "machine" ] == null )
				return false;

			_machineName = node.Attributes[ "machine" ].Value.Trim();

			if( _machineName == null || _machineName.Length == 0 )
				return false;

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
			attribute.Value = "Machine";
			limitNode.Attributes.Append( attribute );

			attribute = parent.OwnerDocument.CreateAttribute( "machine" );
			attribute.Value = _machineName;
			limitNode.Attributes.Append( attribute );

			parent.AppendChild( limitNode );			
			return limitNode;
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
	} // End class MachineLimit
} // End namespace Xheo.Licensing

////////////////////////////////////////////////////////////////////////////////
#region Copyright � 2002-2005 XHEO, INC. All Rights Reserved.
//
#endregion
////////////////////////////////////////////////////////////////////////////////
