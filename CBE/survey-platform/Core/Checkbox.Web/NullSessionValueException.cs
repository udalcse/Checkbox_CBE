//===============================================================================
// Prezza Technologies Checkbox
// Copyright © Checkbox Survey Inc  All rights reserved.
//===============================================================================
using System;

namespace Checkbox.Web
{
	/// <summary>
	/// Exception indicating a session value was null.
	/// </summary>
	public class NullSessionValueException : System.NullReferenceException
	{
		private bool rethrow;

		/// <summary>
		/// Default constructor.  Rethrow will be set to false.
		/// </summary>
		public NullSessionValueException() : this(false)
		{
		}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="variable"></param>
        public NullSessionValueException(string variable) : base("An expected session variable [" + variable + "] was not found, possibly due to a session timeout.")
        {
        }
	
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="rethrow">Indicator of whether the exception should be rethrown or not by the handler.</param>
		public NullSessionValueException(bool rethrow) : base("An expected session variable was not found, possibly due to a session timeout.")
		{
			this.rethrow = rethrow;
		}

		/// <summary>
		/// Get/Set whether the error should be rethrown.
		/// </summary>
		public bool Rethrow
		{
			get{ return this.rethrow;}
			set{ this.rethrow = value;}
		}
	}
}
