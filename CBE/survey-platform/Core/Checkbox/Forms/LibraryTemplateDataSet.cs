using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Checkbox.Forms
{
    /// <summary>
    /// Template data for library templates
    /// </summary>
    class LibraryTemplateDataSet : TemplateDataSet
    {

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="owningObjectTypeName"></param>
        public LibraryTemplateDataSet(string owningObjectTypeName)
            : base(owningObjectTypeName)
        {
        }

        /// <summary>
        /// Get names of data tables for this object's configuration data set.
        /// </summary>
        public override List<string> ObjectDataTableNames
        {
            get
            {
                List<string> tables = base.ObjectDataTableNames;
                tables.Add("ItemLists");

                //TODO: This list might be not complete.

                return tables;
            }
        }
    }
}
