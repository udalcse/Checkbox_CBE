//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System;

namespace Prezza.Framework.ExceptionHandling
{
	/// <summary>
	/// Base exception class for frameworks and applications
	/// </summary>
	internal class BaseException : Exception
	{
		/// <summary>
		/// Unique identifier for an instance of an exception.  This identifier should be shared among
		/// all related exceptions, such as for replacement and wrapper exceptions
		/// </summary>
		private Guid exceptionId;

		/// <summary>
		/// Constructor.
		/// </summary>
		public BaseException() : base()
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="exceptionId"></param>
		public BaseException(Guid exceptionId) : base()
		{
			this.exceptionId = exceptionId;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="message"></param>
		public BaseException(string message) : base(message)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="exceptionId"></param>
		public BaseException(string message, Guid exceptionId) : base(message)
		{
			this.exceptionId = exceptionId;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="innerException"></param>
		public BaseException(string message, Exception innerException) : base(message, innerException)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="innerException"></param>
		/// <param name="exceptionID"></param>
		public BaseException(string message, Exception innerException, Guid exceptionID) : base(message, innerException)
		{
			this.exceptionId = exceptionID;
		}
		
		/// <summary>
		/// Get the exception Id.
		/// </summary>
		public Guid ExceptionId
		{
			get{return exceptionId;}
			set{exceptionId = value;}
		}
	}
}
