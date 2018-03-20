using System.Xml;
using Checkbox.Common;

namespace Checkbox.Forms.Logic.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class RuleExportWriter
    {
        protected RuleDataService DataService { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataService"></param>
        protected RuleExportWriter(RuleDataService dataService)
        {
            DataService = dataService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="writer"></param>
        public abstract void WriteRuleData(PersistedDomainObject obj, XmlWriter writer);
    }
}
