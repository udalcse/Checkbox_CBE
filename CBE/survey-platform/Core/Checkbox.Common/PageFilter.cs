//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
//===============================================================================
namespace Checkbox.Common
{
	/// <summary>
	/// Filter based on a page number.
	/// </summary>
	public class PageFilter
	{
		private int pageNumber;
		private int resultsPerPage;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="pageNumber">Page number of results to get.</param>
		/// <param name="resultsPerPage">Number of results to return per page.</param>
		public PageFilter(int pageNumber, int resultsPerPage)
		{
			this.pageNumber = pageNumber;
			this.resultsPerPage = resultsPerPage;
		}

		/// <summary>
		/// Get the page number of the results to return.
		/// </summary>
		public int PageNumber
		{
			get {return this.pageNumber; }

		}

		/// <summary>
		/// Get the number of results per page to return.
		/// </summary>
		public int ResultsPerPage
		{
			get{ return this.resultsPerPage;}
		}
	}
}
