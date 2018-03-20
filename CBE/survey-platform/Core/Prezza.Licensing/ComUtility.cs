///////////////////////////////////////////////////////////////////////////////
#region Copyright © 2002-2005 XHEO. All Rights Reserve
// This file and its contents are protected by United States and
// International copyright laws. Unauthorized reproduction and/or
// distribution of all or any portion of the code contained herein
// is strictly prohibited and will result in severe civil and criminal
// penalties. Any violations of this copyright will be prosecuted
// to the fullest extent possible under law.
//
// THE SOURCE CODE CONTAINED HEREIN AND IN RELATED FILES IS PROVIDED
// TO THE REGISTERED DEVELOPER FOR THE PURPOSES OF EDUCATION AND
// TROUBLESHOOTING. UNDER NO CIRCUMSTANCES MAY ANY PORTION OF THE SOURC
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
///////////////////////////////////////////////////////////////////////////////
//
// Class:		ComUtility
// Author:		Paul Alexander
// Created:		Wednesday, April 16, 2003 7:11:00 PM
//
///////////////////////////////////////////////////////////////////////////////

using System;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Reflection.Emit;

namespace Xheo.Licensing
{
	/// <summary>
	/// Implements various utility methods for COM interoperability.
	/// </summary>
	internal sealed class ComUtility
	{
		////////////////////////////////////////////////////////////////////////////////
		#region Fields

		

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Constructors/Destructors

		/// <summary>
		///	Initializes a new instance of the ComUtility class.
		/// </summary>
		private ComUtility()
		{
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Properties
		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Operations
   
		[ DllImport( "oleaut32.dll", CharSet = CharSet.Unicode, PreserveSig = false )]
		private static extern void LoadTypeLibEx( String strTypeLibName, RegKind regKind, 
			[ MarshalAs( UnmanagedType.Interface )] out Object typeLib );


		/// <summary>
		/// Gets an assembly used to interop with the objects defined in the type
		/// library.
		/// </summary>
		/// <param name="typeLibraryName">
		///		Name of the type library such as SHDocVw.dll
		/// </param>
		/// <returns>
		///		Returns the assembly if found/created otherwise null.
		/// </returns>
		public static Assembly GetAssemblyForTypeLib( string typeLibraryName )
		{
			try
			{
				object typeLib = null;
				LoadTypeLibEx( typeLibraryName, RegKind.RegKind_None, out typeLib ); 
      
				if( typeLib == null )
					return null;
         
				TypeLibConverter converter = new TypeLibConverter();
				ConversionEventHandler eventHandler = new ConversionEventHandler();
				AssemblyBuilder asm = converter.ConvertTypeLibToAssembly( typeLib, "Interop." + System.IO.Path.GetFileName( typeLibraryName ), 0, eventHandler, null, null, System.IO.Path.GetFileNameWithoutExtension( typeLibraryName ), null );   
				return asm;
			}
			catch( Exception ex )
			{
				System.Diagnostics.Debug.WriteLine( ex.ToString() );
				return null;
			}
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////

		#region RegKind
		private enum RegKind
		{
			RegKind_Default = 0,
			RegKind_Register = 1,
			RegKind_None = 2
		}
		#endregion RegKind

		#region ConversionEventHandler
		/// <summary>
		/// 
		/// </summary>
		internal class ConversionEventHandler : ITypeLibImporterNotifySink
		{
			public void ReportEvent( ImporterEventKind eventKind, int eventCode, string eventMsg )
			{
				// handle warning event here...
			}
   
			public Assembly ResolveRef( object typeLib )
			{
				// resolve reference here and return a correct assembly...
				return null; 
			}   
		} // End class ConversionEventHandler
		#endregion

	} // End class ComUtility
} // End namespace Xheo

////////////////////////////////////////////////////////////////////////////////
#region Copyright © 2002-2005 XHEO, INC. All Rights Reserved.
//
#endregion
////////////////////////////////////////////////////////////////////////////////
