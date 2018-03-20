using System.Collections.Generic;

namespace Checkbox.Wcf.Services.Proxies
{
    public class ReportableSection
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        public List<int> Items { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportableSection"/> class.
        /// </summary>
        public ReportableSection()
        {
            this.Items = new List<int>();
        }
    }
}
