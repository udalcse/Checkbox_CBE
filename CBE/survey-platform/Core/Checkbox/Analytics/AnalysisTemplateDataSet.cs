using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Checkbox.Forms;

namespace Checkbox.Analytics
{
    public class AnalysisTemplateDataSet : TemplateDataSet
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="owningObjectTypeName"></param>
        public AnalysisTemplateDataSet(string owningObjectTypeName)
            : base(owningObjectTypeName)
        {
        }

        /// <summary>
        /// Set identity column
        /// </summary>
        public override string IdentityColumnName { get { return "AnalysisTemplateId"; } }
    }
}
