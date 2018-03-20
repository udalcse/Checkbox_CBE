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
// Class:		FailureManager
// Author:		Paul Alexander
// Created:		Monday, December 30, 2002 4:23:54 PM
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Specialized;

namespace Xheo.Licensing
{
	/// <summary>
	/// Manages graceful failure periods for licenses that depend on remote or 
	/// volitale resources.
	/// </summary>
#if LICENSING
	public 
#else
	internal
#endif
		class FailureManager
	{
		////////////////////////////////////////////////////////////////////////////////
		#region Fields

		Hashtable _failures		= new Hashtable();
		Hashtable _lastContext	= new Hashtable();

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Constructors/Destructors

		/// <summary>
		///	Initializes a new instance of the FailureManager class.
		/// </summary>
		public FailureManager()
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

		/// <summary>
		/// Registers a failure for the given type and determines if a hard failure
		/// should occur.
		/// </summary>
		/// <param name="context">
		///		The LicenseContext of the request. The failure is only registered once
		///		per context.
		/// </param>
		/// <param name="type">
		///		The Type of the control or component being licensed. Failures are tracked
		///		per Type.
		/// </param>
		/// <param name="allowedFailures">
		///		The maximum number of failures that can occur before a hard failure occurs.
		/// </param>
		/// <returns>
		///		Returns true if a hard failure should occur, otherwise false.
		/// </returns>
		public bool RegisterFailure( LicenseContext context, Type type, int allowedFailures )
		{
			int		contextHash = context.GetHashCode();
			object	lastHash	= _lastContext[ type.FullName ];
			object	failures	= _failures[ type.FullName ];
			
			
			if( failures == null || lastHash == null || (int)lastHash != contextHash )
			{
				if( failures == null )
					failures = 1;
				else
					failures = ((int)failures) + 1;
				
				_lastContext[ type.FullName ]	= contextHash;
				_failures[ type.FullName ]		= failures;
			}

            return ((int)failures) > allowedFailures;
		}

		/// <summary>
		/// Registers a successful attempt to validate a license and resets the failure 
		/// count to 0.
		/// </summary>
		/// <param name="context">
		///		The LicenseContext of the request.
		/// </param>
		/// <param name="type">
		///		The Type of the control or component being licensed.
		/// </param>
		public void RegisterSuccess( LicenseContext context, Type type )
		{
			_lastContext[ type.FullName ] = null;
			_failures[ type.FullName ] = 0;
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
	} // End class FailureManager
} // End namespace Xheo.Licensing

////////////////////////////////////////////////////////////////////////////////
#region Copyright © 2002-2005 XHEO, INC. All Rights Reserved.
//
#endregion
////////////////////////////////////////////////////////////////////////////////
