//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System;

namespace Prezza.Framework.ExceptionHandling.Configuration
{
	/// <summary>
	/// Configuration for exception type handling
	/// </summary>
	public class ExceptionTypeData
	{
		/// <summary>
		/// Collection of exception handlers for this exception type.
		/// </summary>
		private ExceptionHandlerDataCollection exceptionHandlers;

		/// <summary>
		/// Name of this exception type.
		/// </summary>
		private string name;

		/// <summary>
		/// TypeName of this exception type.
		/// </summary>
		private string type;

		/// <summary>
		/// Action to take after an exception of this type has been handled.  Default to notify and rethrow.
		/// </summary>
		private PostHandlingAction postHandlingAction = PostHandlingAction.NotifyRethrow;

		/// <summary>
		/// Constructor.  Initializes the collection of exception handlers and sets the name and type to empty strings.
		/// </summary>
		public ExceptionTypeData() : this(string.Empty, string.Empty)
		{
		}

		/// <summary>
		/// Constructor.  Initializes the collection of exception handlers.
		/// </summary>
		/// <param name="name">Name of the exception type.</param>
		/// <param name="type">TypeName of the exception the handlers will handle.</param>
		public ExceptionTypeData(string name, string type) : this(name, type, PostHandlingAction.None)
		{
		}

		/// <summary>
		/// Constructor.  Initializes the collection of exception handlers.
		/// </summary>
		/// <param name="name">Name of the exception type.</param>
		/// <param name="type">TypeName of the exception the handlers will handle.</param>
		/// <param name="postHandlingAction">Action to take after handling the exception.</param>
		public ExceptionTypeData(string name, string type, PostHandlingAction postHandlingAction)
		{
			this.name = name;
			this.type = type;
			this.postHandlingAction = postHandlingAction;
			this.exceptionHandlers = new ExceptionHandlerDataCollection();
		}

		/// <summary>
		/// Get/Set name of the exception type.
		/// </summary>
		public string Name
		{
			get{return name;}
			set{name = value;}
		}

		/// <summary>
		/// Get/Set typeName the exception type's handlers will handle.
		/// </summary>
		public string TypeName
		{
			get{return type;}
			set{type = value;}
		}

		/// <summary>
		/// Get/Set action to take after exception has been handled.
		/// </summary>
		public PostHandlingAction PostHandlingAction
		{
			get{return postHandlingAction;}
			set{postHandlingAction = value;}
		}
		
		/// <summary>
		/// Get the collection of exception handlers for this type.
		/// </summary>
		public ExceptionHandlerDataCollection ExceptionHandlers
		{
			get{return exceptionHandlers;}
		}
	}
}
