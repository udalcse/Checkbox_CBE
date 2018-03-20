using System.Xml;
using Checkbox.Common;

namespace Checkbox.Forms.Logic.Configuration
{
    /// <summary>
    /// Reader for importing rules
    /// </summary>
    public abstract class RuleImportReader
    {
        protected RuleDataService DataService { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataService"></param>
        protected RuleImportReader(RuleDataService dataService)
        {
            DataService = dataService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="ruleNode"></param>
        public abstract void ReadRuleData(PersistedDomainObject obj, XmlNode ruleNode);
    }
}
