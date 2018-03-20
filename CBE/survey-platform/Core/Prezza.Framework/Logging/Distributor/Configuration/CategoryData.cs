//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
// Contains software or other content adapted from Microsoft patterns & practices Enterprise Library, © 2006 Microsoft Corporation. All rights reserved.
//===============================================================================

namespace Prezza.Framework.Logging.Distributor.Configuration
{
	/// <summary>
	/// Container for information about a logging category.
	/// </summary>
	public class CategoryData
	{
		/// <summary>
		/// Name of the logging category
		/// </summary>
		private string name;

		/// <summary>
		/// Collection <see cref="DestinationData"/> objects that describe how to handle specific
		/// log entries depending on their categories.
		/// </summary>
		private DestinationDataCollection destinationDataCollection;

		/// <summary>
		/// Constructor.
		/// </summary>
		public CategoryData() : this(string.Empty)
		{
		}
		
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name">Name of the category.</param>
		public CategoryData(string name)
		{
			this.name = name;
			destinationDataCollection = new DestinationDataCollection();
		}

		/// <summary>
		/// Get/Set the name of the category.
		/// </summary>
		public string Name
		{
			get{return name;}
			set{name = value;}
		}

		/// <summary>
		/// Get the <see cref="DestinationDataCollection"/> of <see cref="DestinationData"/> objects that define
		/// how log entries for this category will be handled.
		/// </summary>
		public DestinationDataCollection DestinationDataCollection
		{
			get{return destinationDataCollection;}
		}
	}
}
