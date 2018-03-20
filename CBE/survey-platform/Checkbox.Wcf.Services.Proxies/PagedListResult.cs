using System.Runtime.Serialization;

namespace Checkbox.Wcf.Services.Proxies
{
    /// <summary>
    /// Simple container for operations that return paged lists. Adds container for total
    /// result count.
    /// </summary>
    /// <typeparam name="T">Type of the data contained in the RestulPage.</typeparam>
    [DataContract]
    public class PagedListResult<T>
    {
        /// <summary>
        /// The total number of items in the result set.
        /// </summary>
        [DataMember]
        public int TotalItemCount { get; set; }

        /// <summary>
        /// A page of result data.
        /// </summary>
        [DataMember]
        public T ResultPage { get; set; }
    }
}
