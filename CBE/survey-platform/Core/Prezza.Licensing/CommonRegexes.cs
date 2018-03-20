///////////////////////////////////////////////////////////////////////////////
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
///////////////////////////////////////////////////////////////////////////////
//
// Class:		CommonRegexes
// Author:		Paul Alexander
// Created:		Wednesday, June 11, 2003 5:12:42 PM
//
///////////////////////////////////////////////////////////////////////////////

using System;

#if SHARED
namespace Xheo
#else
namespace Internal
#endif
{
	/// <summary>
	/// Declares static versions of some common regular expressions.
	/// </summary>
#if SHARED
	public sealed class CommonRegexes
#else
	internal sealed class CommonRegexes
#endif
	{
		////////////////////////////////////////////////////////////////////////////////
		#region Fields

		private const string _FileName		= @"([A-Za-z0-9\.\-_\[\]\(\)\$ ]*)";
		private const string _FilePath		= @"((([A-Za-z]:\\)|(\\\\[A-Za-z0-9]{0,16})\\)?(" + _FileName + @"\\)*" + _FileName + @")";
		private const string _IpAddress		= @"((25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9]))";
		private const string _DomainName	= @"(([a-zA-Z0-9\*\?][\-_a-zA-Z0-9\*\?]{0,25}\.)*[a-zA-Z0-9\*\?][\-_a-zA-Z0-9\*\?]{0,25}\.[a-zA-Z0-9\*\?]{2,5})";
		private const string _UrlFileName	= @"([\.\-_a-zA-Z0-9~%\?&#=]*)";
		private const string _UrlPath		= @"([\.\-_a-zA-Z0-9~%=]*)";

		/// <summary>
		/// Pattern for matching against any valid IP address.
		/// </summary>
		public const string IpAddress	= @"^" + _IpAddress + "$";

		/// <summary>
		/// Pattern for matching against any valid domain name.
		/// </summary>
		public const string DomainName	= @"^" + _DomainName + "$";

		/// <summary>
		/// Pattern for matching a Windows file path.
		/// </summary>
		public const string FileName	= @"^" + _FileName + "$";

		/// <summary>
		/// Pattern for matching a Windows file path.
		/// </summary>
		public const string FilePath	= @"^" + _FilePath + "$";

		/// <summary>
		/// Pattern for matching against any valid URL.
		/// </summary>
//		public const string Url					= @"^(((http://)|(https://)|(ftp://)|(file://))?((([a-zA-Z][\-_a-zA-Z0-9]{1,25}\.)*[a-zA-Z][\-_a-zA-Z0-9]{1,25}\.[a-zA-Z]{2,3})|(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9]))(:[0-9]+)?)?([/\\][\.\-_a-zA-Za-zA-Z0-9~%\?&#=]*)*$";
		public const string Url					= @"^(((((http://)|(https://)|(ftp://))?(" + _DomainName + "|" + _IpAddress + ")?(/" + _UrlPath + ")*(/" + _UrlFileName + ")?))|(file://" + _FilePath + "))$";

		/// <summary>
		/// Pattern for matching against any valid URL that has no file name.
		/// </summary>
		public const string UrlFolder			= @"^(((((http://)|(https://)|(ftp://))?(" + _DomainName + "|" + _IpAddress + ")?(/" + _UrlPath + ")*/))|(file://" + _FilePath + "))$";

		/// <summary>
		/// Pattern for matching against any valid URL.
		/// </summary>
//		public const string FullyQualifiedUrl	= @"^(((http://)|(https://)|(ftp://)|(file://))(([a-zA-Z][\-_a-zA-Z0-9]{1,25}\.)*[a-zA-Z][\-_a-zA-Z0-9]{1,25}\.[a-zA-Z]{2,3})|(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])){1}(:[0-9]+)?([/\\][\.\-_a-zA-Za-zA-Z0-9~%\?&#=]*)*$";
		public const string FullyQualifiedUrl	= @"^(((((http://)|(https://)|(ftp://))?(" + _DomainName + "|" + _IpAddress + ")(/" + _UrlPath + ")*(/0" + _UrlFileName + ")?))|(file://" + _FilePath + "))$";

		/// <summary>
		/// Pattern for matching a fully qualified URL that has no file name.
		/// </summary>
		public const string FullyQualifiedUrlFolder	= @"^(((((http://)|(https://)|(ftp://))?(" + _DomainName + "|" + _IpAddress + ")(/" + _UrlPath + ")*/))|(file://" + _FilePath + "))$";

		/// <summary>
		/// Pattern for matching against any valid email address.
		/// </summary>
		public const string Email				= @"^[a-zA-Z0-9]([\.\-_=a-zA-Z0-9])*@([a-zA-Z0-9][\-_a-zA-Z0-9]{0,25}\.)*[a-zA-Z0-9][\-_a-zA-Z0-9]{1,25}(\.[a-zA-Z]{2,3})?$";

		/// <summary>
		/// Pattern for a US 5-digit zip code.
		/// </summary>
		public const string UsZip5				= @"^[0-9]{5}$";

		/// <summary>
		/// Pattern for a US 5-digit zip code plus 4 digit exchange.
		/// </summary>
		public const string UsZip5Plus4			= @"^[0-9]{5}[- ]?[0-9]{4}$";

		/// <summary>
		/// Pattern for a US 5-digit zip code with an optional 4 digit exchange.
		/// </summary>
		public const string UsZip				= @"^[0-9]{5}[- ]?([0-9]{4})?$";

		/// <summary>
		/// Pattern for a US 10-digit phone number.
		/// </summary>
		public const string UsPhone				= @"^(([0-9]{3}[\.\- ]?[0-9]{3}[\.\- ]?[0-9]{4})|(\([0-9]{3}\)[ ]?[0-9]{3}[\.\- ]?[0-9]{4}))$";
//		public const string UsPhone				= @"^(\d{3}\.\d{3}\.\d{4})$";

		/// <summary>
		/// Pattern for a US social security number.
		/// </summary>
		public const string UsSsn				= @"^([0-9]{3}\-[0-9]{2}\-[0-9]{4})|([0-9]{9})$";

		/// <summary>
		/// Pattern for a US employer identification number.
		/// </summary>
		public const string UsEin				= @"^([0-9]{2}\-[0-9]{7})|([0-9]{9})$";

		/// <summary>
		/// Pattern for a US social security number or an employer identification number.
		/// </summary>
		public const string UsSsnOrEin			= @"^([0-9]{3}\-[0-9]{2}\-[0-9]{4})|([0-9]{2}\-[0-9]{7})|([0-9]{9})$";

		#endregion
		////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////
		#region Constructors/Destructors

		/// <summary>
		///	Initializes a new instance of the CommonRegexes class.
		/// </summary>
		private CommonRegexes()
		{
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
	} // End class CommonRegexes
} // End namespace Xheo

////////////////////////////////////////////////////////////////////////////////
#region Copyright © 2002-2005 XHEO, INC. All Rights Reserved.
//
#endregion
////////////////////////////////////////////////////////////////////////////////