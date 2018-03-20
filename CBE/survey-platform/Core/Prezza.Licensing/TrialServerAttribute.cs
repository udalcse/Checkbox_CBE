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
// Class:		TrialServerAttribute
// Author:		Paul Alexander
// Created:		Thursday, January 02, 2003 11:27:43 PM
//
////////////////////////////////////////////////////////////////////////////////

using System;

namespace Xheo.Licensing
{
#if LICENSING
	/// <summary>
	/// Declares the <see cref="LicenseServer"/> capable of issuing new
	/// trial licenses for an assembly.
	/// </summary>
#endif
	[ AttributeUsage( AttributeTargets.Assembly, AllowMultiple = true ) ]
#if LICENSING
	public 
#else
	internal
#endif
		sealed class TrialServerAttribute : Attribute
	{
		////////////////////////////////////////////////////////////////////////////////
		#region Fields

		private string _serverUrl	= null;
		private string _extra		= null;

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Constructors/Destructors

		/// <summary>
		/// Included for COM compliance, do not use.
		/// </summary>
		public TrialServerAttribute()
		{
		}

		/// <summary>
		///	Initializes a new instance of the TrialServerAttribute class.
		/// </summary>
		/// <param name="serverUrl">
		///		The URL of the <c>LicenseServer</c> WebService to request a trial license from.
		/// </param>
		public TrialServerAttribute( string serverUrl )
		{
			_serverUrl = serverUrl;
		}

		/// <summary>
		///	Initializes a new instance of the TrialServerAttribute class.
		/// </summary>
		/// <param name="extra">
		///		Extra information to pass verbatim to the <c>LicenseServer.GetTrialLicense</c> method.
		/// </param>
		/// <param name="serverUrl">
		///		The URL of the <c>LicenseServer</c> WebService to request a trial license from.
		/// </param>
		public TrialServerAttribute( string serverUrl, string extra )
		{
			_serverUrl = serverUrl;
			_extra = extra;
		}
		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Properties

		/// <summary>
		/// Gets the address of the acitvation server for issuing new trial licenses.
		/// </summary>
		public string ServerUrl
		{
			get
			{
				return _serverUrl;
			}
		}

		/// <summary>
		/// Gets a string of extra information to be included in the request. This can
		/// include things like a partner ID, originating URL, etc. The value is passed
		/// verbatim to the <c>LicenseServer.GetTrialLicense</c> method.
		/// </summary>
		public string Extra
		{
			get
			{
				return _extra;
			}
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Operations
		#endregion
		////////////////////////////////////////////////////////////////////////////////
	} // End class TrialServerAttribute
} // End namespace Xheo.Licensing

////////////////////////////////////////////////////////////////////////////////
#region Copyright © 2002-2005 XHEO, INC. All Rights Reserved.
//
#endregion
////////////////////////////////////////////////////////////////////////////////
