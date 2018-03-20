//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================
using System.Xml.Serialization;

namespace Prezza.Framework.Logging.Configuration
{
	/// <summary>
	/// Configuration for logging category filters.
	/// </summary>
	[XmlRoot("categoryFilter")]
	public class CategoryFilterData
	{
		/// <summary>
		/// Name of the filter.
		/// </summary>
		private string name;

		/// <summary>
		/// Constructor.
		/// </summary>
		public CategoryFilterData() : this(string.Empty)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name">Name of the filter.</param>
		public CategoryFilterData(string name)
		{
			this.name = name;
		}

		/// <summary>
		/// Name of the category filter.
		/// </summary>
		public string Name
		{
			get{return name;}
			set{name = value;}
		}
	}
}
