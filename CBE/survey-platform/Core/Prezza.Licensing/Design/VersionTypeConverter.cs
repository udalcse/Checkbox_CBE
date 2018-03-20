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
// Class:		VersionTypeConverter
// Author:		Paul Alexander
// Created:		Saturday, November 16, 2002 2:15:47 PM
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.ComponentModel;
using System.Net;
using System.Globalization;

namespace Xheo.Licensing.Design
{
	/// <summary>
	/// Implements a TypeConverter to for Version objects.
	/// </summary>
	public class VersionTypeConverter : TypeConverter
	{
		#region TypeConverter Implementation
		///<summary>
		///Summary of GetStandardValuesSupported.
		///</summary>
		///<param name="context"></param>
		///<returns></returns>	
		public override bool GetStandardValuesSupported( ITypeDescriptorContext context )
		{
			return false;
		}


		///<summary>
		///Summary of CanConvertFrom.
		///</summary>
		///<param name="context"></param>
		///<param name="type"></param>
		///<returns></returns>	
		public override bool CanConvertFrom( ITypeDescriptorContext context, Type type )
		{
			if( type == typeof( string ) )
				return true;

			return base.CanConvertFrom( context, type );
		}

		///<summary>
		///Summary of CanConvertTo.
		///</summary>
		///<param name="context"></param>
		///<param name="type"></param>
		///<returns></returns>	
		public override bool CanConvertTo( ITypeDescriptorContext context, Type type )
		{
			if( type.IsSubclassOf( typeof( Version ) ) )
				return true;

			return false;
		}

		///<summary>
		///Summary of ConvertFrom.
		///</summary>
		///<param name="context"></param>
		///<param name="culture"></param>
		///<param name="value"></param>
		///<returns></returns>	
		public override object ConvertFrom( ITypeDescriptorContext context, CultureInfo culture, object value )
		{
			if( value is string )
			{
				return new Version( value as string );
			}

			return base.ConvertFrom( context, culture, value );
		}

		///<summary>
		///Summary of ConvertTo.
		///</summary>
		///<param name="context"></param>
		///<param name="culture"></param>
		///<param name="value"></param>
		///<param name="destinationType"></param>
		///<returns></returns>	
		public override object ConvertTo( ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType )
		{
			if( destinationType == typeof( string ) )
				return ((Version)value).ToString();

			return base.ConvertTo( context, culture, value, destinationType );
		}
		


		#endregion
	}
} // End namespace Xheo.Licensing.Design

////////////////////////////////////////////////////////////////////////////////
#region Copyright © 2002-2005 XHEO, INC. All Rights Reserved.
//
#endregion
////////////////////////////////////////////////////////////////////////////////
