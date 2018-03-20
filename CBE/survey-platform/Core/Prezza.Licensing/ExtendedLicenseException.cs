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
// Class:		ExtendedLicenseException
// Author:		Paul Alexander
// Created:		Jul 19th, 2002 12:27 AM
// Modified:	Jul 19th, 2002 12:40 AM
//
////////////////////////////////////////////////////////////////////////////////


using System;
using System.Diagnostics;
using System.Xml;
using System.Collections;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using Xheo;


namespace Xheo.Licensing
{
	#region ExtendedLicenseException
	/// <summary>
	/// Represents an exception in a Xheo assembly with a localized <see cref="FriendlyMessage"/>
	/// and <see cref="DetailedMessage"/>.
	/// </summary>
	/// <remarks>
	///		The <see cref="Message"/> is used to communicate a simple description
	///		of what went wrong and should be written to the non-developer audience. The
	///		<see cref="DetailedMessage"/> should be used to communicate additional technical details
	///		and how the problem might be fixed.
	/// </remarks>
#if LICENSING
	public
#else
	internal
#endif	
		class ExtendedLicenseException : ApplicationException
	{
		////////////////////////////////////////////////////////////////////////////////
		#region Fields
		
		private string	_detailedMessage	= null;
		private string	_message			= null;
				
		#endregion
		////////////////////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////////////////////
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the ExtendedLicenseException class.
		/// </summary>
		public ExtendedLicenseException()
		{
		}

		/// <summary>
		///	Initializes a new instance of the ExtendedLicenseException class and uses the
		///	resource given for the message.
		/// </summary>
		/// <param name="resourceName">
		///		The name of the resource to use for the message.
		/// </param>
		public ExtendedLicenseException( string resourceName ) 
		{
			_message = Internal.StaticResourceProvider.MakeProviderForAssembly( GetType().Assembly ).GetString( resourceName );
		}

		/// <summary>
		///	Initializes a new instance of the ExtendedLicenseException class and uses the
		///	resource given for the message.
		///	</summary>
		/// <param name="resourceName">
		///		The name of the resource to use for the message.
		/// </param>
		/// <param name="args">
		///		Variable list of arguments to pass to String.Format.
		/// </param>
		public ExtendedLicenseException( string resourceName, params object[] args )
		{
			_message = Internal.StaticResourceProvider.MakeProviderForAssembly( GetType().Assembly ).FormatResourceString( resourceName, args );
		}

		/// <summary>
		///	Initializes a new instance of the ExtendedLicenseException class and uses the
		///	resource given for the message.
		/// </summary>
		/// <param name="resourceName">
		///		The name of the resource to use for the message.
		/// </param>
		/// <param name="innerException">
		///		A reference to any inner exception.
		/// </param>
		public ExtendedLicenseException( string resourceName, Exception innerException ) : base( null, innerException )
		{
			_message = Internal.StaticResourceProvider.MakeProviderForAssembly( GetType().Assembly ).FormatResourceString( resourceName );
		}

		/// <summary>
		///	Initializes a new instance of the ExtendedLicenseException class and uses the
		///	resource given for the message.
		///	</summary>
		/// <param name="resourceName">
		///		The name of the resource to use for the message.
		/// </param>
		/// <param name="innerException">
		///		A reference to any inner exception.
		/// </param>
		/// <param name="args">
		///		Variable list of arguments to pass to String.Format.
		/// </param>
		public ExtendedLicenseException( string resourceName, Exception innerException, params object[] args ) : base( null, innerException )
		{
			_message =  Internal.StaticResourceProvider.MakeProviderForAssembly( GetType().Assembly ).FormatResourceString( resourceName, args );
		}


		/// <summary>
		/// Initializes a new instance of the ExtendedLicenseException class with serialized data.
		/// </summary>
		/// <param name="info">
		///		The object that holds the serialized object data.
		/// </param>
		/// <param name="context">
		///		The contextual information about the source or destination.
		/// </param>
		protected ExtendedLicenseException( SerializationInfo info, StreamingContext context ) : base( info, context )
		{
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Properties

		/// <summary>
		/// Gets a description of the exception.
		/// </summary>
		public override string Message
		{
			get
			{
				return _message;
			}
		}


		/// <summary>
		/// Gets or sets the friendly message.
		/// </summary>
		[ Obsolete( "Deprecated, use Message instead.", false ) ]
		public string FriendlyMessage
		{
			get
			{
				return _message;
			}
			set
			{
				_message = value;
			}
		}

		/// <summary>
		/// Gets or sets the detailed message.
		/// </summary>
		public string DetailedMessage
		{
			get
			{
				return _detailedMessage;
			}
			set
			{
				_detailedMessage = value;
			}
		}
		
		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Operations
		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
	} // End class ExtendedLicenseException
	#endregion

} // End namespace Xheo

////////////////////////////////////////////////////////////////////////////////
#region Copyright © 2002-2005 XHEO, INC. All Rights Reserved.

#endregion
////////////////////////////////////////////////////////////////////////////////
