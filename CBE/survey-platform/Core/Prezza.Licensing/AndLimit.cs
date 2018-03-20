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
// Class:		AndLimit
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
	/// Allows a group of limits to be declared using branch logic when used in 
	/// combination with the <see cref="AndLimit"/>.
	/// </summary>
#if LICENSING
	public 
#else
	internal
#endif
		class AndLimit : ContainerLimit
	{
		////////////////////////////////////////////////////////////////////////////////
		#region Fields

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Constructors/Destructors

		/// <summary>
		///	Initializes a new instance of the AndLimit class.
		/// </summary>
		public AndLimit()
		{
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
				return Internal.StaticResourceProvider.CurrentProvider.GetString( "MANDL_Description" );
			}
		}

		/// <summary>
		/// Gets the display name of the limit.
		/// </summary>
		public override string Name
		{
			get
			{
				return "And";
			}
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Operations

		///<summary>
		///Summary of Granted.
		///</summary>
		///<param name="context"></param>
		///<param name="type"></param>
		///<param name="instance"></param>
		public override void Granted( LicenseContext context, Type type, object instance )
		{
			foreach( Limit limit in Limits )
				limit.Granted( context, type, instance );
		}

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
			ArrayList guiLimits		= new ArrayList();
			ArrayList remoteLimits	= new ArrayList();

			foreach( Limit limit in Limits )
			{
				if( limit.License == null )
					limit.License = License;
				if( limit.IsGui )
					guiLimits.Add( limit );
				else if( limit.IsRemote )
					remoteLimits.Add( limit );
				else if( ! limit.Validate( context, type, instance ) )
					return false;
			}

			foreach( Limit limit in remoteLimits )
			{
				if( limit.IsGui )
					guiLimits.Add( limit );
				else if( ! limit.Validate( context, type, instance ) )
					return false;
			}

			foreach( Limit limit in guiLimits )
			{
				if( ! limit.Validate( context, type, instance ) )
					return false;
			}


			return true;
		}

		/// <summary>
		/// Overrides <see cref="Limit.GetDetailsDescription"/>.
		/// </summary>
		public override string GetDetailsDescription()
		{
			System.Text.StringBuilder limits = new System.Text.StringBuilder();

			foreach( Limit limit in Limits )
			{
				if( limits.Length > 0 )
					limits.Append( ", " );

				limits.AppendFormat( System.Globalization.CultureInfo.InvariantCulture, "{{{0} - {1} ({2})}}", limit.Name, limit.Description, limit.GetDetailsDescription() );
			}

			return limits.ToString();
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
	} // End class AndLimit
} // End namespace Xheo.Licensing

////////////////////////////////////////////////////////////////////////////////
#region Copyright � 2002-2005 XHEO, INC. All Rights Reserved.
//
#endregion
////////////////////////////////////////////////////////////////////////////////